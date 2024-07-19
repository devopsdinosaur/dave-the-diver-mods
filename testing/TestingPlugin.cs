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
using SushiBar.Customer;

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
			logger.LogInfo("devopsdinosaur.davethediver.testing v0.0.1 loaded.");
		} catch (Exception e) {
			logger.LogError("** Awake FATAL - " + e);
		}
	}

	/*
	[HarmonyPatch(typeof(GameBase), "LoadSavedData")]
	class HarmonyPatch_GameBase_Update {

		private static bool Prefix(GameBase __instance) {
			try {
				
				foreach (GameObject obj in SceneManager.GetActiveScene().GetRootGameObjects()) {
					//logger.LogInfo(obj.ToString());
				}

				return true;
			} catch (Exception e) {
				logger.LogError("** HarmonyPatch_GameBase_Update.Prefix ERROR - " + e);
			}
			return true;
		}
	}
	*/

	[HarmonyPatch(typeof(PlayerBreathHandler), "SetHP")]
	class HarmonyPatch_PlayerBreathHandler_SetHP {

		private static bool one_shot = false;

		private static bool Prefix(ref float pHP) {
			try {
				if (one_shot) {
					//Application.Quit();
					return true;
				}
				one_shot = true;
				Transform dave = null;
				foreach (GameObject obj in SceneManager.GetActiveScene().GetRootGameObjects()) {
					//logger.LogInfo(obj.name);
					if (obj.name == "PlayerGroup(Clone)") {
						dave = UnityUtils.find_first_descendant(obj.transform, "DaveCharacter");
					}// else if (fish == null) {
					 //	fish = UnityUtils.find_first_descendant(obj.transform, "Yellow_Tang");
					 //}
					if (dave != null) {
						break;
					}
				}
				//UnityUtils.list_ancestors(dave, logger.LogInfo);
				UnityUtils.json_dump(dave, "C:/tmp/dave.json");
				return true;
			} catch (Exception e) {
				logger.LogError("** HarmonyPatch_PlayerBreathHandler_Update.Prefix ERROR - " + e);
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(DefaultGunGear), "FireGun")]
	class HarmonyPatch_DefaultGunGear_FireGun {

		private static bool Prefix() {
			try {
				
				return true;
			} catch (Exception e) {
				logger.LogError("** XXXXX.Prefix ERROR - " + e);
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(CharacterController2D), "MovePosition")]
	class HarmonyPatch_CharacterController2D_MovePosition {

		private static void Postfix(ref Vector2 pos) {
			try {
				logger.LogInfo("HarmonyPatch_CharacterController2D_MovePosition - pos: " + pos);
			} catch (Exception e) {
				logger.LogError("** XXXXX.Prefix ERROR - " + e);
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