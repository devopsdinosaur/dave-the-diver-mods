using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using BepInEx.Unity.IL2CPP;
using SushiBar.Customer;

[BepInPlugin("devopsdinosaur.davethediver.super_dave", "Super Dave", "0.0.1")]
public class TestingPlugin : BasePlugin {

	private Harmony m_harmony = new Harmony("devopsdinosaur.davethediver.super_dave");
	public static ManualLogSource logger;
	private static ConfigEntry<bool> m_enabled;
	private static ConfigEntry<bool> m_infinite_oxygen;
	private static ConfigEntry<bool> m_invincible;
	private static ConfigEntry<bool> m_infinite_customer_patience;

	public override void Load() {
		logger = base.Log;
		try {
			m_enabled = this.Config.Bind<bool>("General", "Enabled", true, "Set to false to disable this mod.");
			m_infinite_oxygen = this.Config.Bind<bool>("Diving", "Infinite Oxygen", false, "Set to true to have infinite oxygen when diving (and when not diving, but that's a freebie).");
			m_invincible = this.Config.Bind<bool>("Diving", "Invincible", false, "Set to true to take no damage when hit.");
			m_infinite_customer_patience = this.Config.Bind<bool>("Sushi", "Infinite Customer Patience", false, "Set to true to make customers never storm off if the food/drinks are too slow.");
			this.m_harmony.PatchAll();
			logger.LogInfo("devopsdinosaur.davethediver.super_dave v0.0.1 loaded.");
		} catch (Exception e) {
			logger.LogError("** Awake FATAL - " + e);
		}
	}

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