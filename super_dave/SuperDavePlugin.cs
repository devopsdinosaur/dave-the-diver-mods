using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using BepInEx.Unity.IL2CPP;
using SushiBar.Customer;
using UnityEngine;

[BepInPlugin("devopsdinosaur.davethediver.super_dave", "Super Dave", "0.0.1")]
public class TestingPlugin : BasePlugin {

	private Harmony m_harmony = new Harmony("devopsdinosaur.davethediver.super_dave");
	public static ManualLogSource logger;
	private static ConfigEntry<bool> m_enabled;

	// Diving
	private static ConfigEntry<bool> m_infinite_oxygen;
	private static ConfigEntry<bool> m_invincible;
	private static ConfigEntry<bool> m_toxic_aura_enabled;
	private static ConfigEntry<float> m_aura_radius;

	// Sushi Bar
	private static ConfigEntry<bool> m_infinite_customer_patience;

	public override void Load() {
		logger = base.Log;
		try {
			m_enabled = this.Config.Bind<bool>("General", "Enabled", true, "Set to false to disable this mod.");
			
			// Diving
			m_infinite_oxygen = this.Config.Bind<bool>("Diving", "Infinite Oxygen", false, "Set to true to have infinite oxygen when diving (and when not diving, but that's a freebie).");
			m_invincible = this.Config.Bind<bool>("Diving", "Invincible", false, "Set to true to take no damage when hit.");
			m_toxic_aura_enabled = this.Config.Bind<bool>("Diving", "Toxic Aura: Enabled", false, "Set to true to enable the instant-fish-killing aura around Dave.");
			m_aura_radius = this.Config.Bind<float>("Diving", "Toxic Aura: Radius", 3.0f, "Radius (in meters?) around the character in which fish will be insta-killed, if Toxic Aura is enabled (float, default 3.0f)");
			
			// Sushi Bar
			m_infinite_customer_patience = this.Config.Bind<bool>("Sushi", "Infinite Customer Patience", false, "Set to true to make customers never storm off if the food/drinks are too slow.");
			
			this.m_harmony.PatchAll();
			logger.LogInfo("devopsdinosaur.davethediver.super_dave v0.0.1 loaded.");
		} catch (Exception e) {
			logger.LogError("** Awake FATAL - " + e);
		}
	}

	// ================================================================================
	// == Diving
	// ================================================================================

	[HarmonyPatch(typeof(PlayerBreathHandler), "SetHP")]
	class HarmonyPatch_PlayerBreathHandler_SetHP {

		private static bool Prefix(ref float pHP) {
			try {
				if (m_enabled.Value && m_infinite_oxygen.Value) {
					pHP = 999f;
				}
				return true;
			} catch (Exception e) {
				logger.LogError("** HarmonyPatch_PlayerBreathHandler_Update.Prefix ERROR - " + e);
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(PlayerCharacter), "SetHPDamage")]
	class HarmonyPatch_PlayerCharacter_SetHPDamage {

		private static bool Prefix() {
			try {
				return !(m_enabled.Value && m_invincible.Value);
			} catch (Exception e) {
				logger.LogError("** HarmonyPatch_PlayerCharacter_SetHPDamage.Prefix ERROR - " + e);
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(CharacterController2D), "FixedUpdate")]
	class HarmonyPatch_CharacterController2D_LateUpdate {

		private const float UPDATE_FREQUENCY = 0.1f;
		private static float m_elapsed = UPDATE_FREQUENCY;

		private static void Postfix(CharacterController2D __instance) {
			try {
				if (!m_enabled.Value || !m_toxic_aura_enabled.Value || (m_elapsed += Time.fixedDeltaTime) < UPDATE_FREQUENCY) {
					return;
				}
				m_elapsed = 0f;
				foreach (FishInteractionBody fish in Resources.FindObjectsOfTypeAll<FishInteractionBody>()) {
					if (Vector3.Distance(__instance.transform.position, fish.transform.position) <= m_aura_radius.Value) {
						fish.gameObject.GetComponent<Damageable>().OnDie();
					}
				}
			} catch (Exception e) {
				logger.LogError("** HarmonyPatch_CharacterController2D_LateUpdate.Prefix ERROR - " + e);
			}
		}
	}

	// ================================================================================
	// == Sushi Bar
	// ================================================================================

	[HarmonyPatch(typeof(SushiBarCustomer), "LateUpdate")]
	class HarmonyPatch_SushiBarCustomer_LateUpdate {

		private static void Postfix(SushiBarCustomer __instance) {
			try {
				if (m_enabled.Value && m_infinite_customer_patience.Value) {
					__instance.WaitingSpeedParameter = (__instance.IsOrderWait || __instance.IsOrderWaitDrink ? 0 : 1);
				}
			} catch (Exception e) {
				logger.LogError("** HarmonyPatch_SushiBarCustomer_LateUpdate.Prefix ERROR - " + e);
			}
		}
	}

}