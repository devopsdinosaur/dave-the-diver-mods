using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using BepInEx.Unity.IL2CPP;
using DR;
using SushiBar.Customer;
using UnityEngine;
using Common.Contents;

[BepInPlugin("devopsdinosaur.davethediver.super_dave", "Super Dave", "0.0.6")]
public class SuperDavePlugin : BasePlugin {

	private Harmony m_harmony = new Harmony("devopsdinosaur.davethediver.super_dave");
	public static ManualLogSource logger;
	private static ConfigEntry<bool> m_enabled;

	// Boat
	private static ConfigEntry<float> m_boat_walk_speed_boost;

	// Diving
	private static ConfigEntry<bool> m_infinite_oxygen;
	private static ConfigEntry<bool> m_invincible;
	private static ConfigEntry<bool> m_toxic_aura_enabled;
	private static ConfigEntry<bool> m_toxic_aura_sleep;
	private static ConfigEntry<float> m_aura_radius;
	private static ConfigEntry<float> m_speed_boost;
	private static ConfigEntry<bool> m_infinite_bullets;
    private static ConfigEntry<bool> m_weightless_items;

	// Farm
	private static ConfigEntry<float> m_farm_walk_multiplier;

	// Farm
	private static ConfigEntry<float> m_fish_farm_walk_multiplier;

	// Sushi Bar
	private static ConfigEntry<bool> m_infinite_customer_patience;
	private static ConfigEntry<float> m_sushi_speed_boost;
	private static ConfigEntry<float> m_sushi_money_boost;
	private static ConfigEntry<float> m_staff_cook_multiplier;
	private static ConfigEntry<float> m_staff_walk_multiplier;
	private static ConfigEntry<bool> m_infinite_wasabi;

	public ConfigEntry<T> migrate_option<T>(string section, string old_key, string key, T val, string description) {
		ConfigEntry<T> new_opt = this.Config.Bind<T>(section, key, val, description);
		ConfigEntry<T> old_opt = this.Config.Bind<T>(section, old_key, new_opt.Value, description);
		new_opt.Value = old_opt.Value;
		this.Config.Remove(old_opt.Definition);
		return new_opt;
	}

	public override void Load() {
		logger = base.Log;
		try {
			m_enabled = this.Config.Bind<bool>("General", "Enabled", true, "Set to false to disable this mod.");

			// Boat
			m_boat_walk_speed_boost = this.Config.Bind<float>("Boat", "Boat - Walk Speed Boost", 0f, "Speed boost applied to Dave when walking on the boat (float, default 0f [set to 0 to disable]).");

			// Farm
			m_farm_walk_multiplier = this.Config.Bind<float>("Farm", "Farm - Walk Multiplier", 0f, "Multiplier applied to Dave when walking/sprinting on the farm (float, default 0f [ < 1 == faster, < 1 == slower, set to 0 to disable]) [NOTE: Setting this value too high (above ~5) can make it possible for Dave to walk fully off the screen and get stuck].");

			// Fish Farm
			m_fish_farm_walk_multiplier = this.Config.Bind<float>("Fish Farm", "Fish Farm - Walk Multiplier", 0f, "Multiplier applied to Dave when walking/sprinting on the fish farm (float, default 0f [ < 1 == faster, < 1 == slower, set to 0 to disable]).");

			// Diving
			m_infinite_bullets = this.migrate_option<bool>("Diving", "Infinite Bullets", "Diving - Infinite Bullets", false, "Set to true to have infinite bullets when diving.");
			m_infinite_oxygen = this.migrate_option<bool>("Diving", "Infinite Oxygen", "Diving - Infinite Oxygen", false, "Set to true to have infinite oxygen when diving (and when not diving, but that's a freebie).");
			m_invincible = this.migrate_option<bool>("Diving", "Invincible", "Diving - Invincible", false, "Set to true to take no damage when hit.");
			m_speed_boost = this.migrate_option<float>("Diving", "Speed Boost", "Diving - Speed Boost", 0f, "Permanent speed boost when diving (float, default 0f [set to 0 to disable]).");
			m_toxic_aura_enabled = this.migrate_option<bool>("Diving", "Toxic Aura: Enabled", "Diving - Toxic Aura: Enabled", false, "Set to true to enable the instant-fish-killing (or sleeping if 'Toxic Aura: Sleep Effect' is true) aura around Dave.");
			m_toxic_aura_sleep = this.Config.Bind<bool>("Diving", "Diving - Toxic Aura: Sleep Effect", false, "Set to true to switch from killing aura to sleep-inducing aura.");
			m_aura_radius = this.migrate_option<float>("Diving", "Toxic Aura: Radius", "Diving - Toxic Aura: Radius", 3.0f, "Radius (in meters?) around the character in which fish will be insta-killed, if Toxic Aura is enabled (float, default 3.0f).");
			m_weightless_items = this.migrate_option<bool>("Diving", "Weightless Items (Infinite Carry Weight)", "Diving - Weightless Items (Infinite Carry Weight)", false, "Set to true to have reduce the weight of all items to 0, effectively giving infinite carry weight and inventory space.");

			// Sushi Bar
			m_staff_cook_multiplier = this.Config.Bind<float>("Sushi", "Sushi - Cook Multiplier", 0f, "Multiplier applied to sushi bar staff cooking speed (float, default 0f [ < 1 == faster, < 1 == slower, set to 0 to disable]).");
			m_infinite_customer_patience = this.migrate_option<bool>("Sushi", "Infinite Customer Patience", "Sushi - Infinite Customer Patience", false, "Set to true to make customers never storm off if the food/drinks are too slow.");
			m_infinite_wasabi = this.Config.Bind<bool>("Sushi", "Sushi - Infinite Wasabi", false, "Set to true to never need to refill wasabi.");
			m_sushi_money_boost = this.Config.Bind<float>("Sushi", "Sushi - Money Boost", 0f, "Money boost applied to all customer purchases at sushi bar (float, default 0f [set to 0 to disable]).");
			m_sushi_speed_boost = this.migrate_option<float>("Sushi", "Sushi Speed Boost", "Sushi - Speed Boost", 0f, "Permanent speed boost when working in sushi bar (float, default 0f [set to 0 to disable]).");
			m_staff_walk_multiplier = this.Config.Bind<float>("Sushi", "Sushi - Walk Multiplier", 0f, "Multiplier applied to sushi bar staff walking speed [note: this also affects Dave on top of 'Sushi - Speed Boost'] (float, default 0f [ < 1 == faster, < 1 == slower, set to 0 to disable]).");

			this.m_harmony.PatchAll();
			logger.LogInfo("devopsdinosaur.davethediver.super_dave v0.0.6 loaded.");
		} catch (Exception e) {
			logger.LogError("** Awake FATAL - " + e);
		}
	}

	private static void debug_log(object text) {
		logger.LogInfo(text);
	}

	// ================================================================================
	// == Boat
	// ================================================================================

	[HarmonyPatch(typeof(LobbyPlayer), "Init")]
	class HarmonyPatch_LobbyPlayer_Init {

		private static void Postfix(LobbyPlayer __instance) {
			try {
				if (m_enabled.Value && m_boat_walk_speed_boost.Value >= 0) {
					ReflectionUtils.il2cpp_get_field(__instance, "m_MoveSpeed").SetValue(
						__instance, 
						ReflectionUtils.il2cpp_get_field_value<float>(__instance, "m_moveSpeed") + m_boat_walk_speed_boost.Value
					);
				}
			} catch (Exception e) {
				logger.LogError("** HarmonyPatch_LobbyPlayer_Init.Postfix ERROR - " + e);
			}
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

	[HarmonyPatch(typeof(BuffHandler), "Start")]
	class HarmonyPatch_BuffHandler_Start {

		private static void Postfix(BuffHandler __instance) {
			try {
				if (m_enabled.Value && m_speed_boost.Value > 0) {
					__instance.GetBuffComponents.AddMoveSpeedParam(1234567, m_speed_boost.Value);
				}
			} catch (Exception e) {
				logger.LogError("** HarmonyPatch_BuffHandler_Start.Postfix ERROR - " + e);
			}
		}
	}

	[HarmonyPatch(typeof(CharacterController2D), "FixedUpdate")]
	class HarmonyPatch_CharacterController2D_LateUpdate {

		private const float UPDATE_FREQUENCY = 0.1f;
		private static float m_elapsed = UPDATE_FREQUENCY;
		private const int SLEEP_BUFF_ID = 14080415;

		private static void Postfix(CharacterController2D __instance) {
			try {
				if (!m_enabled.Value || (m_elapsed += Time.fixedDeltaTime) < UPDATE_FREQUENCY) {
					return;
				}
				m_elapsed = 0f;
				if (m_toxic_aura_enabled.Value) {
					foreach (FishInteractionBody fish in Resources.FindObjectsOfTypeAll<FishInteractionBody>()) {
						if (Vector3.Distance(__instance.transform.position, fish.transform.position) <= m_aura_radius.Value) {
							if (m_toxic_aura_sleep.Value) {
								fish.gameObject.GetComponent<BuffHandler>().AddBuff(SLEEP_BUFF_ID);
							} else {
								fish.gameObject.GetComponent<Damageable>().OnDie();
							}	
						}
					}
				}
			} catch (Exception e) {
				logger.LogError("** HarmonyPatch_CharacterController2D_LateUpdate.Prefix ERROR - " + e);
			}
		}
	}

	[HarmonyPatch(typeof(PlayerCharacter), "Update")]
	class HarmonyPatch_PlayerCharacter_Update {

		private const float UPDATE_FREQUENCY = 1.0f;
		private static float m_elapsed = UPDATE_FREQUENCY;

		private static void Postfix(PlayerCharacter __instance) {
			try {
				if (!m_enabled.Value || !m_infinite_bullets.Value || (m_elapsed += Time.deltaTime) < UPDATE_FREQUENCY) {
					return;
				}
				try {
					__instance?.CurrentInstanceItemInventory?.gunHandler.ForceSetBulletCount(999);
				} catch {}
			} catch (Exception e) {
				logger.LogError("** HarmonyPatch_PlayerCharacter_Update.Postfix ERROR - " + e);
			}
		}
	}

    [HarmonyPatch(typeof(IntegratedItem), "BuildInternal")]
    class HarmonyPatch_IntegratedItem_BuildInternal {

        private static bool Prefix(ref IItemBase itemBase) {
            try {
				if (m_enabled.Value && m_weightless_items.Value) {
					itemBase.ItemWeight = 0;
				}
                return true;
            } catch (Exception e) {
                logger.LogError("** HarmonyPatch_IntegratedItem_BuildInternal.Postfix ERROR - " + e);
            }
            return true;
        }
    }

	// ================================================================================
	// == Farm
	// ================================================================================

	[HarmonyPatch(typeof(Farm.FarmPlayerView), "Setup")]
	class HarmonyPatch_Farm_FarmPlayerView_Setup {

		private static void Postfix(Farm.FarmPlayerView __instance) {
			try {
				if (!m_enabled.Value || m_farm_walk_multiplier.Value <= 0) {
					return;
				}
				ReflectionUtils.il2cpp_get_field(__instance, "m_Speed_Min").SetValue(__instance, ReflectionUtils.il2cpp_get_field_value<float>(__instance, "m_Speed_Min") * m_farm_walk_multiplier.Value);
				ReflectionUtils.il2cpp_get_field(__instance, "m_Speed_Max").SetValue(__instance, ReflectionUtils.il2cpp_get_field_value<float>(__instance, "m_Speed_Max") * m_farm_walk_multiplier.Value);
			} catch (Exception e) {
				logger.LogError("** HarmonyPatch_Farm_FarmPlayerView_Setup.Postfix ERROR - " + e);
			}
		}
	}

	// ================================================================================
	// == Fish Farm
	// ================================================================================

	[HarmonyPatch(typeof(FishFarm.FishFarmPlayerView), "Move")]
	class HarmonyPatch_FishFarm_FishFarmPlayerView_Move {

		private static List<int> m_patched_instance_hashes = new List<int>();

		private static void Postfix(FishFarm.FishFarmPlayerView __instance) {
			try {
				if (m_enabled.Value && m_fish_farm_walk_multiplier.Value > 0 && !m_patched_instance_hashes.Contains(__instance.GetHashCode())) {
					m_patched_instance_hashes.Add(__instance.GetHashCode());
					ReflectionUtils.il2cpp_get_field(__instance, "Dave_Speed").SetValue(__instance, ReflectionUtils.il2cpp_get_field_value<float>(__instance, "Dave_Speed") * m_fish_farm_walk_multiplier.Value);
				}
			} catch (Exception e) {
				logger.LogError("** HarmonyPatch_FishFarm_FishFarmPlayerView_Move.Postfix ERROR - " + e);
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
				if (!m_enabled.Value) {
					return;
				}
				if (m_infinite_customer_patience.Value) {
					__instance.WaitingSpeedParameter = (__instance.IsOrderWait || __instance.IsOrderWaitDrink ? 0 : 1);
				}
				if (m_sushi_money_boost.Value > 0) {
					__instance.RevenueBuffParameter = m_sushi_money_boost.Value;
				}
			} catch (Exception e) {
				logger.LogError("** HarmonyPatch_SushiBarCustomer_LateUpdate.Prefix ERROR - " + e);
			}
		}
	}
	
	[HarmonyPatch(typeof(DaveMoveValue), "speedMultiplier", MethodType.Getter)]
	class HarmonyPatch_DaveMoveValue_speedMultiplier_Getter {

		private static bool Prefix(ref float __result) {
			try {
				if (!m_enabled.Value || m_sushi_speed_boost.Value <= 0) {
					return true;
				}
				__result = m_sushi_speed_boost.Value;
				return false;
			} catch (Exception e) {
				logger.LogError("** HarmonyPatch_DaveMoveValue_speedMultiplier_Getter.Prefix ERROR - " + e);
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(SushiBarStaffBase), "CalcCookingTime")]
	class HarmonyPatch_SushiBarStaffBase_CalcCookingTime {

		private static void Postfix(ref float __result) {
			try {
				if (m_enabled.Value && m_staff_cook_multiplier.Value > 0) {
					__result *= m_staff_cook_multiplier.Value;
				}
			} catch (Exception e) {
				logger.LogError("** HarmonyPatch_SushiBarStaffBase_CalcCookingTime.Postfix ERROR - " + e);
			}
		}
	}

	[HarmonyPatch(typeof(SushiBarStaffBase), "CalcMoveSpeed")]
	class HarmonyPatch_SushiBarStaffBase_CalcMoveSpeed {

		private static void Postfix(ref float __result) {
			try {
				if (m_enabled.Value && m_staff_walk_multiplier.Value > 0) {
					__result *= m_staff_walk_multiplier.Value;
				}
			} catch (Exception e) {
				logger.LogError("** HarmonyPatch_SushiBarStaffBase_CalcMoveSpeed.Postfix ERROR - " + e);
			}
		}
	}

	[HarmonyPatch(typeof(SushiBarContext.WasabiGratersData), "UpdateWasabiCount")]
	class HarmonyPatch_SushiBarContext_WasabiGratersData_UpdateWasabiCount {

		private static bool Prefix(ref int count) {
			try {
				if (m_enabled.Value && m_infinite_wasabi.Value) {
					count = 0;
				}
				return true;
			} catch (Exception e) {
				logger.LogError("** HarmonyPatch_SushiBarContext_WasabiGratersData_UpdateWasabiCount.Postfix ERROR - " + e);
			}
			return true;
		}
	}
}