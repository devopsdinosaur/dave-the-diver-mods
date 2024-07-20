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

	private static void debug_log(object text) {
		logger.LogInfo(text);
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

	[HarmonyPatch(typeof(GunBullet), "Shoot", new Type[] {
		typeof(IAdvancedDamager), typeof(Vector3), typeof(GunSpecData), 
		typeof(BulletSoundList), typeof(int), typeof(string), 
		typeof(bool)
	})]
	class HarmonyPatch_GunBullet_Shoot1 {

		private static bool Prefix(GunBullet __instance, IAdvancedDamager attacker, Vector3 direction, GunSpecData gunSpecData, BulletSoundList soundList, int bulletID, string bulletIDTag, bool IsDoNotGiveInvincible) {
			try {
				debug_log("** SHOOT (1)! **");
				gunSpecData._GunType = GunItemType.BlackHole_GrenadeLauncher;
				gunSpecData._ExposionRadius *= 100;
				return true;
			} catch (Exception e) {
				logger.LogError("** XXXXX.Postfix ERROR - " + e);
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(GunBullet), "Shoot", new Type[] {
		typeof(Vector3), typeof(GunSpecData), typeof(BulletSoundList)
	})]
	class HarmonyPatch_GunBullet_Shoot2 {

		private static bool Prefix(GunBullet __instance, Vector3 direction, GunSpecData gunSpecData, BulletSoundList soundList) {
			try {
				debug_log("** SHOOT (2)! **");

				//foreach (GameObject obj in SceneManager.GetActiveScene().GetRootGameObjects()) {
				//	UnityUtils.json_dump(obj.transform, "C:/tmp/dump/" + obj.name + ".json");
				//}

				foreach (FishInteractionBody fish in Resources.FindObjectsOfTypeAll<FishInteractionBody>()) {
					/*
					GunBullet bullet = GameObject.Instantiate<GunBullet>(__instance);
					bullet.OnDoAttack(new AttackData() {
						damager = bullet.GetDamager,
						attackType = AttackType.Player_Gun,
						damage = 99999,
						direction = direction
					}, new DefenseData() {
						damageable = fish.gameObject.GetComponent<Damageable>()
					});
					GameObject.Destroy(bullet);
					*/
					fish.gameObject.GetComponent<Damageable>().OnDie();
				}

				return true;
			} catch (Exception e) {
				logger.LogError("** XXXXX.Postfix ERROR - " + e);
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(GunBullet), "Shoot", new Type[] {
		typeof(IDamager), typeof(Vector3 ), typeof(GunSpecData), typeof(BulletSoundList)
	})]
	class HarmonyPatch_GunBullet_Shoot4 {

		private static bool Prefix(GunBullet __instance, IDamager attacker, Vector3 direction, GunSpecData gunSpecData, BulletSoundList soundList) {
			try {
				debug_log("** SHOOT (4)! **");
				gunSpecData._GunType = GunItemType.BlackHole_GrenadeLauncher;
				gunSpecData._ExposionRadius *= 100;
				return true;
			} catch (Exception e) {
				logger.LogError("** XXXXX.Postfix ERROR - " + e);
			}
			return true;
		}
	}

	/*
	[HarmonyPatch(typeof(CharacterController2D), "FixedUpdate")]
	class HarmonyPatch_CharacterController2D_LateUpdate {
		
		private const float MINIMUM_DISTANCE = 0.01f;
		private const float MAXIMUM_DISTANCE = 0.10f;
		private static Vector3 m_prev_pos = Vector3.zero;

		private static bool m_one_shot = true; //false;

		private static void Postfix(CharacterController2D __instance) {
			try {

				if (!m_one_shot) {
					m_one_shot = true;
					foreach (GameObject obj in SceneManager.GetActiveScene().GetRootGameObjects()) {
						UnityUtils.json_dump(obj.transform, "C:/tmp/dump/" + obj.name + ".json");
					}
					//Application.Quit();
				}
				Damageable damageable = __instance.gameObject.GetComponent<Damageable>();
				if (damageable == null || damageable.CanNotDamaged) {
					debug_log("** can not damage");
					return;
				}
				Vector3 pos = __instance.transform.localPosition;
				if (m_prev_pos == Vector3.zero || pos == m_prev_pos) {
					m_prev_pos = pos;
					return;
				}
				float distance = Vector3.Distance(pos, m_prev_pos);
				//if (distance >= MINIMUM_DISTANCE && distance < MAXIMUM_DISTANCE) {
				//	debug_log(distance);
				//	__instance.transform.localPosition = pos = (m_prev_pos - pos).normalized * distance * 5;
				//	debug_log(__instance.transform.localPosition);
				//}
				m_prev_pos = pos;
			} catch (Exception e) {
				logger.LogError("** HarmonyPatch_CharacterController2D_LateUpdate.Prefix ERROR - " + e);
			}
		}
	}
	*/

	[HarmonyPatch(typeof(PlayerCharacter), "TryStoreItem")]
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