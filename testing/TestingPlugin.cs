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
			/*
			ReflectionUtils.generate_trace_patcher(typeof(IntegratedItem), "C:/tmp/TracePatcher_IntegratedItem.cs", @"
using DR;
class MapperConfiguration {}
",
				new string[] {
                    "get_ItemSpecID",
                    "set_ItemSpecID"
                }
			);
			Application.Quit();
			*/

			/*
			TracePatcher_IntegratedItem.callback = delegate(TracePatcher_IntegratedItem.TracerParams args) {
				debug_log(args.method_name);
			};
			*/
			
			logger.LogInfo("devopsdinosaur.davethediver.testing v0.0.1 loaded.");
		} catch (Exception e) {
			logger.LogError("** Awake FATAL - " + e);
		}
	}

	private static void global_update() {
		debug_log(".");
	}

	private static void debug_log(object text) {
		logger.LogInfo(text);
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
				
				/*
				if ((elapsed += Time.deltaTime) >= 15) {				
					foreach (GameObject obj in SceneManager.GetActiveScene().GetRootGameObjects()) {
						UnityUtils.json_dump(obj.transform, "C:/tmp/dump/" + obj.name + ".json");
					}
					Application.Quit();
				}
				*/

			} catch (Exception e) {
				logger.LogError("** HarmonyPatch_PlayerCharacter_Update.Postfix ERROR - " + e);
			}
		}
	}

	/*
	[HarmonyPatch(typeof(OverweightProperty), "GetDebuff")]
	class HarmonyPatch_OverweightProperty_GetDebuff {

		private static bool Prefix(ref OverweightProperty.Debuff __result) {
			__result = null;
			return false;
		}
	}
	*/

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