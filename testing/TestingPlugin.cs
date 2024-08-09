using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using BepInEx.Unity.IL2CPP;
using EvilFactory;
using CodeStage.AntiCheat.ObscuredTypes;
using DR;
using System.Linq;
using System.IO;
using Common.Contents;
using SushiBar.Customer;
using System.Runtime.InteropServices;

[BepInPlugin("devopsdinosaur.davethediver.testing", "Testing", "0.0.1")]
public class TestingPlugin : BasePlugin {

	private Harmony m_harmony = new Harmony("devopsdinosaur.davethediver.testing");
	public static ManualLogSource logger;
	private static ConfigEntry<bool> m_enabled;
	
	public override void Load() {
		logger = base.Log;
		try {
			m_enabled = this.Config.Bind<bool>("General", "Enabled", true, "Set to false to disable this mod.");
			this.m_harmony.PatchAll();
			PluginUpdater.create(this, logger);
			PluginUpdater.Instance.register("global", 1.0f, global_update);
			PluginUpdater.Instance.register("keypress", 0f, keypress_update);
			logger.LogInfo("devopsdinosaur.davethediver.testing v0.0.1 loaded.");
		} catch (Exception e) {
			logger.LogError("** Awake FATAL - " + e);
		}
	}

	private static void global_update() {
		
	}

	private static bool m_trigger_one_shot = false;

	private static void keypress_update() {
		if (Input.GetKeyDown(KeyCode.Backspace)) {
			dump_all_objects();
			Application.Quit();
		} else if (Input.GetKeyDown(KeyCode.F1)) {
			m_trigger_one_shot = true;
			
		}
	}

	[HarmonyPatch(typeof(Farm.FarmPlayerView), "Move")]
	class HarmonyPatch_Farm_FarmPlayerView_Move {

		private static void Postfix(Farm.FarmPlayerView __instance) {
			try {
				if (!m_trigger_one_shot) {
					return;
				}
				m_trigger_one_shot = false;
				debug_log(__instance.transform.position);
				__instance.transform.position = new Vector3(__instance.transform.position.x, __instance.transform.position.y, 5);
			} catch (Exception e) {
				logger.LogError("** HarmonyPatch_Farm_FarmPlayerView_Move.Postfix ERROR - " + e);
			}
		}
	}

	private static void dump_all_objects() {
		string DIRECTORY = "C:/tmp/dump";
		Directory.CreateDirectory(DIRECTORY);
		foreach (string file in Directory.GetFiles(DIRECTORY, "*.json", SearchOption.TopDirectoryOnly)) {
			File.Delete(Path.Combine(DIRECTORY, file));
		}
		foreach (GameObject obj in SceneManager.GetActiveScene().GetRootGameObjects()) {
			UnityUtils.json_dump(obj.transform, Path.Combine(DIRECTORY, obj.name + ".json"));
		}
	}

	private static void debug_log(object text) {
		logger.LogInfo(text);
	}

	[HarmonyPatch(typeof(LobbyPlayer), "FixedUpdate")]
	class HarmonyPatch_LobbyPlayer_FixedUpdate {

		private static bool Prefix(LobbyPlayer __instance) {
			try {
				if (!m_trigger_one_shot) {
					return true;
				}
				m_trigger_one_shot = false;
				///*
				foreach (SpecDataBase item in Resources.FindObjectsOfTypeAll<SpecDataBase>()) {
					debug_log($"{item.Name}");
					foreach (BuffDebuffEffectData buff in item.buffDatas) {
						debug_log($"--> type: {buff.BUFFTYPE}, val1: {buff.buffvalue1}");
					}
				}
				foreach (Il2CppSystem.Collections.Generic.KeyValuePair<int, BuffDebuffEffectData> item in DataManager.Instance.BuffEffectDataDic) {
					debug_log($"key: {item.Key}, type: {item.Value.BUFFTYPE}, suid: {item.Value.suid}, val1: {item.Value.buffvalue1}");
					if (item.Key == 14080415) {
						item.Value.buffvalue1 = 9999999;
						item.Value.buffvalue2 = 9999999;
						item.Value.buffvalue3 = 9999999;
					}
				}
				foreach (Il2CppSystem.Reflection.FieldInfo field in (new FishFarm.FishFarmPlayerView()).GetIl2CppType().GetFields((Il2CppSystem.Reflection.BindingFlags) 0xFFFFFFF)) {
					debug_log($"{field.Name} {field.FieldType}");
				}
				//*/
				//Application.Quit();
				return true;
			} catch (Exception e) {
				logger.LogError("** HarmonyPatch_LobbyPlayer_FixedUpdate.Prefix ERROR - " + e);
			}
			return true;
		}
	}

    [HarmonyPatch(typeof(TalkNPCPanel), "Init")]
	class HarmonyPatch_TalkNPCPanel_Init {

		private static void Postfix() {
			try {

			} catch (Exception e) {
				logger.LogError("** HarmonyPatch_TalkNPCPanel_Init.Postfix ERROR - " + e);
			}
		}
	}

	[HarmonyPatch(typeof(SushiBarManager), "Update")]
	class HarmonyPatch_SushiBarManager_Update {

		private static bool Prefix(SushiBarManager __instance) {
			try {
				if (!m_enabled.Value) {
					return true;
				}
				
				return true;
			} catch (Exception e) {
				logger.LogError("** HarmonyPatch_SushiBarManager_Update.Prefix ERROR - " + e);
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(DelayedDisappear), "SetTime")]
	class HarmonyPatch_DelayedDisappear_SetTime {

		private static bool Prefix(ref float inDisappearTime) {
			try {
				debug_log($"HarmonyPatch_DelayedDisappear_SetTime - inDisappearTime: {inDisappearTime}");
				inDisappearTime *= 10;
				return true;
			} catch (Exception e) {
				logger.LogError("** HarmonyPatch_DelayedDisappear_SetTime.Prefix ERROR - " + e);
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(PlayerCharacter), "Update")]
	class HarmonyPatch_PlayerCharacter_Update {

		private static void Postfix(PlayerCharacter __instance) {
			try {
				
			} catch (Exception e) {
				logger.LogError("** HarmonyPatch_PlayerCharacter_Update.Postfix ERROR - " + e);
			}
		}
	}

    // =====================================================================================================
    // == Experimenting
    // =====================================================================================================

    [HarmonyPatch(typeof(Farm.FarmCore), "Watering")]
    class HarmonyPatch_Farm_FamrCore_Watering {

        private static void Postfix(int itemIdx, int x, int y) {
            try {
                debug_log($"Watering - itemIdx: {itemIdx}, x: {x}, y: {y}");
            } catch (Exception e) {
                logger.LogError("** XXXXX.Postfix ERROR - " + e);
            }
        }
    }

    [HarmonyPatch(typeof(CountSecondaryWeaponController), "GetRemainedFireCount")]
    class HarmonyPatch_CountSecondaryWeaponController_GetRemainedFireCount {

        private static void Postfix(ref float __result) {
            try {
				debug_log($"CountSecondaryWeaponController: {__result}");
				__result = 99;
            } catch (Exception e) {
                logger.LogError("** HarmonyPatch_CountSecondaryWeaponController_GetRemainedFireCount.Postfix ERROR - " + e);
            }
        }
    }

    /*
	[HarmonyPatch(typeof(), "")]
	class HarmonyPatch_ {

		private static bool Prefix() {
			try {

				return false;
			} catch (Exception e) {
				logger.LogError("** XXXXX.Prefix ERROR - " + e);
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(), "")]
	class HarmonyPatch_ {

		private static void Postfix() {
			try {
				
			} catch (Exception e) {
				logger.LogError("** XXXXX.Postfix ERROR - " + e);
			}
		}
	}
	*/
}