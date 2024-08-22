using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using BepInEx.Unity.IL2CPP;
using DR;
using SushiBar.Customer;
using UnityEngine;
using TMPro;
using Common.Contents;
using System.Runtime.InteropServices;
using DR.Save;

public static class PluginInfo {

    public const string TITLE = "Super Dave";
    public const string NAME = "super_dave";
	public const string SHORT_DESCRIPTION = "Lots of little improvements to make the game easier to play (and lots more to come)!";

    public const string VERSION = "0.0.10";

    public const string AUTHOR = "devopsdinosaur";
	public const string GAME_TITLE = "Dave the Diver";
    public const string GAME = "davethediver";
    public const string GUID = AUTHOR + "." + GAME + "." + NAME;
	public const string REPO = "dave-the-diver-mods";

	public static Dictionary<string, string> to_dict() {
		Dictionary<string, string> info = new Dictionary<string, string>();
		foreach (FieldInfo field in typeof(PluginInfo).GetFields((BindingFlags) 0xFFFFFFF)) {
			info[field.Name.ToLower()] = (string) field.GetValue(null);
		}
		return info;
	}
}

[BepInPlugin(PluginInfo.GUID, PluginInfo.TITLE, PluginInfo.VERSION)]
public class SuperDavePlugin : DDPlugin {
	private Harmony m_harmony = new Harmony(PluginInfo.GUID);

	public override void Load() {
		Singletons.Plugin = this;
		logger = Singletons.Log = base.Log;
		try {
			Settings.Instance.load(this);
			this.plugin_info = PluginInfo.to_dict();
			this.create_nexus_page();
			this.m_harmony.PatchAll();
			PluginUpdater.create(this, logger);
			Hotkeys.load();
			Diving.load();
			logger.LogInfo($"{PluginInfo.GUID} v{PluginInfo.VERSION} loaded.");
		} catch (Exception e) {
			logger.LogError("** Awake FATAL - " + e);
		}
	}

	// ================================================================================
	// == Boat
	// ================================================================================

	[HarmonyPatch(typeof(LobbyPlayer), "LateUpdate")]
	class HarmonyPatch_LobbyPlayer_Init {
		private static int m_modified_instance_hash = 0;
		private static void Postfix(LobbyPlayer __instance) {
			try {
				if (!Settings.m_enabled.Value) {
					return;
				}
				if (m_modified_instance_hash != __instance.GetHashCode() && Settings.m_boat_walk_speed_boost.Value > 0) {
					m_modified_instance_hash = __instance.GetHashCode();
					ReflectionUtils.il2cpp_get_field(__instance, "m_MoveSpeed").SetValue(
						__instance,
						ReflectionUtils.il2cpp_get_field_value<float>(__instance, "m_MoveSpeed") + Settings.m_boat_walk_speed_boost.Value
					);
				}
			} catch (Exception e) {
				logger.LogError("** HarmonyPatch_LobbyPlayer_Init.Postfix ERROR - " + e);
			}
		}
	}

	// ================================================================================
	// == Farm
	// ================================================================================

	[HarmonyPatch(typeof(Farm.FarmPlayerView), "Setup")]
	class HarmonyPatch_Farm_FarmPlayerView_Setup {
		private static void Postfix(Farm.FarmPlayerView __instance) {
			try {
				if (!Settings.m_enabled.Value || Settings.m_farm_walk_multiplier.Value <= 0) {
					return;
				}
				ReflectionUtils.il2cpp_get_field(__instance, "m_Speed_Min").SetValue(__instance, ReflectionUtils.il2cpp_get_field_value<float>(__instance, "m_Speed_Min") * Settings.m_farm_walk_multiplier.Value);
				ReflectionUtils.il2cpp_get_field(__instance, "m_Speed_Max").SetValue(__instance, ReflectionUtils.il2cpp_get_field_value<float>(__instance, "m_Speed_Max") * Settings.m_farm_walk_multiplier.Value);
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
				if (Settings.m_enabled.Value && Settings.m_fish_farm_walk_multiplier.Value > 0 && !m_patched_instance_hashes.Contains(__instance.GetHashCode())) {
					m_patched_instance_hashes.Add(__instance.GetHashCode());
					ReflectionUtils.il2cpp_get_field(__instance, "Dave_Speed").SetValue(__instance, ReflectionUtils.il2cpp_get_field_value<float>(__instance, "Dave_Speed") * Settings.m_fish_farm_walk_multiplier.Value);
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
				if (!Settings.m_enabled.Value) {
					return;
				}
				if (Settings.m_infinite_customer_patience.Value) {
					__instance.WaitingSpeedParameter = (__instance.IsOrderWait || __instance.IsOrderWaitDrink ? 0 : 1);
				}
				if (Settings.m_sushi_money_boost.Value > 0) {
					__instance.RevenueBuffParameter = Settings.m_sushi_money_boost.Value;
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
				if (!Settings.m_enabled.Value || Settings.m_sushi_speed_boost.Value <= 0) {
					return true;
				}
				__result = Settings.m_sushi_speed_boost.Value;
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
				if (Settings.m_enabled.Value && Settings.m_staff_cook_multiplier.Value > 0) {
					__result /= Settings.m_staff_cook_multiplier.Value;
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
				if (Settings.m_enabled.Value && Settings.m_staff_walk_multiplier.Value > 0) {
					__result *= Settings.m_staff_walk_multiplier.Value;
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
				if (Settings.m_enabled.Value && Settings.m_infinite_wasabi.Value) {
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