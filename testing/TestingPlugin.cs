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
				//if (!m_trigger_one_shot) {
				//	return true;
				//}
				//m_trigger_one_shot = false;
				//foreach (Il2CppSystem.Collections.Generic.KeyValuePair<int, CustomerEntity> item in DataManager.Instance.CustomerDataDic) {
				//	debug_log($"[{item.Key}] - {item.Value.TID}");
				//}
				
				//Application.Quit();
				return true;
			} catch (Exception e) {
				logger.LogError("** HarmonyPatch_LobbyPlayer_FixedUpdate.Prefix ERROR - " + e);
			}
			return true;
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