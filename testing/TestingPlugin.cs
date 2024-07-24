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
			
			ReflectionUtils.generate_trace_patcher(typeof(IntegratedItem), "C:/tmp/TracePatcher_IntegratedItem.cs", @"
using DR;
class MapperConfiguration {}
",
				new string[] {
				}
			);
			Application.Quit();

			logger.LogInfo("devopsdinosaur.davethediver.testing v0.0.1 loaded.");
		} catch (Exception e) {
			logger.LogError("** Awake FATAL - " + e);
		}
	}

	private static void debug_log(object text) {
		logger.LogInfo(text);
	}

	[HarmonyPatch(typeof(GunBullet), "Shoot", new Type[] {
		typeof(Vector3), typeof(GunSpecData), typeof(BulletSoundList)
	})]
	class HarmonyPatch_GunBullet_Shoot2 {

		private static bool Prefix(GunBullet __instance, Vector3 direction, GunSpecData gunSpecData, BulletSoundList soundList) {
			try {
				debug_log("** SHOOT (2)! **");
				return true;
			} catch (Exception e) {
				logger.LogError("** XXXXX.Postfix ERROR - " + e);
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

	[HarmonyPatch(typeof(OverweightProperty), "GetDebuff")]
	class HarmonyPatch_OverweightProperty_GetDebuff {

		private static bool Prefix(ref OverweightProperty.Debuff __result) {
			__result = null;
			return false;
		}
	}

	[HarmonyPatch(typeof(InhaleItem), "Init")]
	class HarmonyPatch_InhaleItem_Init {

		private static bool m_one_shot = true;

		private static void Postfix(Transform targetItem, Action onComplete) {
			try {
				int get_fish_id(string name) {
					try {
						return int.Parse(name.Split('_')[1]);
					} catch {}
					return -1;
				}
				
				int fish_id = get_fish_id(targetItem.name);
				if (!DataManager.Instance.FishInfoDataDic.TryGetValue(fish_id, out FishInfoDataEntity fish_data) || 
					!DataManager.Instance.FishDropPackageDataDic.TryGetValue(fish_data.DropItemID, out FishDropPackageEntity fish_drop)
				) {
					return;
				}
				debug_log($"HarmonyPatch_InhaleItem_Init - type: {targetItem.name}, fish_id: {fish_id}, drop_id: {fish_data.DropItemID}");
				foreach (FishDropPackageEntity.FishDropNode drop in fish_drop.FishDropList) {
					debug_log($"{drop.itemTid} {drop.itemWeight}");
				}
				foreach (int weight in fish_drop.PlusItemWeightList) {
					debug_log($"{weight}");
				}
				if (!m_one_shot) {
					m_one_shot = true;
					foreach (var item in DataManager.Instance.FishDropPackageDataDic) {
						FishDropPackageEntity drop = DataManager.Instance.FishDropPackageDataDic[item.Key];
						Il2CppSystem.Collections.Generic.List<FishDropPackageEntity.FishDropNode> new_nodes = new Il2CppSystem.Collections.Generic.List<FishDropPackageEntity.FishDropNode>();
						foreach (FishDropPackageEntity.FishDropNode node in drop.FishDropList) {
							new_nodes.Add(new FishDropPackageEntity.FishDropNode(node.itemTid, node.itmeTier, 0));
						}
						drop.GetIl2CppType().GetField("FishDropList").SetValue(drop, new_nodes);
					}
				}
				//Application.Quit();
			} catch (Exception e) {
				logger.LogError("** XXXXX.Postfix ERROR - " + e);
			}
		}
	}

	/*
	[HarmonyPatch(typeof(DataManager), "FishDropPackageDataDic", MethodType.Getter)]
	class HarmonyPatch_DataManager_FishDropPackageDataDic {

		private static bool Prefix(ref Dictionary<int, FishDropPackageEntity> __result) {
			try {

				return false;
			} catch (Exception e) {
				logger.LogError("** XXXXX.Prefix ERROR - " + e);
			}
			return true;
		}
	}
	*/

	[HarmonyPatch(typeof(CharacterController2D), "FixedUpdate")]
	class HarmonyPatch_CharacterController2D_FixedUpdate {

		private static bool m_one_shot = true;

		private static bool Prefix(CharacterController2D __instance) {
			try {
				if (!m_one_shot) {
					m_one_shot = true;
					BuffHandler buff_handler = __instance.gameObject.GetComponent<BuffHandler>();
					//BuffDataContainer buff_container = buff_handler.GetBuffComponents;
					/*
					buff_handler.AddBuff(new BuffDebuffEffectData() {
						tid = 999999,
						suid = "suid",
						desconlyeditor = "Carry weight buff description",
						eelement = EElement.None,
						buffdescid = "buffdescid",
						bufficon = "bufficon",
						buffvfxhead = "buffvfxhead",
						buffvfxbody = "buffvfxbody",
						buffvfxcolor = "buffvfxcolor",
						bufftype = BuffType.Cargo_Weight,
						level = 999999,
						isoverride = true,
						duration = 999999,
						tickinterval = 0,
						buffvalue1 = 999999,
						buffvalue2 = 999999,
						buffvalue3 = 999999,
						chancerate = 1
					});
					*/
					foreach (SpecDataBase item in Resources.FindObjectsOfTypeAll<SpecDataBase>()) {
						if (item == null || string.IsNullOrEmpty(item.Name)) {
							continue;
						}
						debug_log("item: " + item.Name);
						if (item.buffDatas == null) {
							continue;
						}
						foreach (BuffDebuffEffectData buff in item.buffDatas) {
							debug_log("--> buffType: " + buff.BUFFTYPE + ", value: " + buff.Buffvalue1);
							buff_handler.AddBuff(buff);
							return true;
						}
					}
				}
				return true;
			} catch (Exception e) {
				logger.LogError("** HarmonyPatch_CharacterController2D_FixedUpdate.Prefix ERROR - " + e);
			}
			return true;
		}
	}

	/*
	[HarmonyPatch(typeof(InstanceItemInventory), "EquipItem")]
	class HarmonyPatch_InstanceItemInventory_EquipItem {

		private static void Postfix(InstanceItemInventory __instance) {
			try {
				//__instance.gameObject.GetComponent<SubHelperItemInventory>().ApplyMoveSpeed(9999);
				debug_log("HarmonyPatch_InstanceItemInventory_EquipItem");
				foreach (Il2CppSystem.Collections.Generic.KeyValuePair<EquipmentType, SpecDataBase> item in __instance.currentEquipInInventory) {
					
					if (item == null || string.IsNullOrEmpty(item.Name)) {
						continue;
					}
					debug_log("item: " + item.Name);
					if (item.buffDatas == null) {
						continue;
					}
					foreach (BuffDebuffEffectData buff in item.buffDatas) {
						debug_log("--> buffType: " + buff.BUFFTYPE + ", value: " + buff.Buffvalue1);
					}
				}
			} catch (Exception e) {
				logger.LogError("** XXXXX.Postfix ERROR - " + e);
			}
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