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
using TMPro;
using DR.Save;
using Common.UI;

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
			/*
			PluginUpdater.create(this, logger);
			PluginUpdater.Instance.register("global", 1.0f, global_update);
			PluginUpdater.Instance.register("keypress", 0f, keypress_update);
			*/
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
				_debug_log(__instance.transform.position);
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

	private static void _debug_log(object text) {
		logger.LogInfo(text);
	}

	[HarmonyPatch(typeof(LobbyPlayer), "FixedUpdate")]
	class HarmonyPatch_LobbyPlayer_FixedUpdate {

		private static bool Prefix(LobbyPlayer __instance) {
			try {
				keypress_update();
				if (!m_trigger_one_shot) {
					return true;
				}
				m_trigger_one_shot = false;
				//DayManager.Instance.SetTimeState(DayTimeState.Morning);
				/*
				foreach (SpecDataBase item in Resources.FindObjectsOfTypeAll<SpecDataBase>()) {
					_debug_log($"{item.Name}");
					foreach (BuffDebuffEffectData buff in item.buffDatas) {
						_debug_log($"--> type: {buff.BUFFTYPE}, val1: {buff.buffvalue1}");
					}
				}
				foreach (Il2CppSystem.Collections.Generic.KeyValuePair<int, BuffDebuffEffectData> item in DataManager.Instance.BuffEffectDataDic) {
					_debug_log($"key: {item.Key}, type: {item.Value.BUFFTYPE}, suid: {item.Value.suid}, val1: {item.Value.buffvalue1}");
					if (item.Key == 14080415) {
						item.Value.buffvalue1 = 9999999;
						item.Value.buffvalue2 = 9999999;
						item.Value.buffvalue3 = 9999999;
					}
				}
				foreach (Il2CppSystem.Reflection.FieldInfo field in (new FishFarm.FishFarmPlayerView()).GetIl2CppType().GetFields((Il2CppSystem.Reflection.BindingFlags) 0xFFFFFFF)) {
					_debug_log($"{field.Name} {field.FieldType}");
				}
				*/
				/*
				if (!m_trigger_one_shot) {
					return true;
				}
				m_trigger_one_shot = false;
				
				Dictionary<HarpoonHeadItemType, List<HarpoonHeadSpecData>> harpoon_heads = new Dictionary<HarpoonHeadItemType, List<HarpoonHeadSpecData>>();
				foreach (HarpoonHeadSpecData spec in Resources.FindObjectsOfTypeAll<HarpoonHeadSpecData>()) {
					HarpoonHeadItemType head_type = (HarpoonHeadItemType) ReflectionUtils.il2cpp_get_field_value<int>(spec, "m_HarpoonHeadType");
					if (!harpoon_heads.ContainsKey(head_type)) {
						harpoon_heads[head_type] = new List<HarpoonHeadSpecData>();
					}
					harpoon_heads[head_type].Add(spec);
					//_debug_log($"{spec.TID} {spec.Name} {spec.Damage} {spec.IsMaxLevel} {head_type}");
				}
				foreach (List<HarpoonHeadSpecData> specs in harpoon_heads.Values) {
					specs.Sort((x, y) => x.Damage.CompareTo(y.Damage));
				}
				foreach (HarpoonHeadItemType type in harpoon_heads.Keys) {
					_debug_log(type);
					foreach (HarpoonHeadSpecData spec in harpoon_heads[type]) {
						_debug_log($"id: {spec.TID}, damage: {spec.Damage}");
					}
				}
				*/
				/*
				Il2CppSystem.Reflection.FieldInfo field = ReflectionUtils.il2cpp_get_field(ResourceManager.Instance, "_HarpoonSpecDataList");
				_debug_log("1");
				Il2CppSystem.Object obj = field.GetValue(ResourceManager.Instance);
				_debug_log("2");
				int size = ReflectionUtils.il2cpp_get_field_value<int>(obj, "_size");
				_debug_log($"size: {size}");
				field = ReflectionUtils.il2cpp_get_field(obj, "_items");
				_debug_log("3");
				obj = field.GetValue(obj);
				_debug_log("4");

				Il2CppSystem.Collections.Generic.List<int> things = new Il2CppSystem.Collections.Generic.List<int>();
				FieldInfo info = things.GetType().GetField("NativeFieldInfoPtr__items", (BindingFlags) 0xFFFFFFF);
				_debug_log(info.Name);
				System.IntPtr NativeFieldInfoPtr__items = (System.IntPtr) info.GetValue(things);
				_debug_log(NativeFieldInfoPtr__items);
				System.IntPtr num = Il2CppInterop.Runtime.IL2CPP.Il2CppObjectBaseToPtrNotNull(things);
				Il2CppInterop.Runtime.IL2CPP.il2cpp_gc_wbarrier_set_field(num, (nint) num + (int) Il2CppInterop.Runtime.IL2CPP.il2cpp_field_get_offset(NativeFieldInfoPtr__items), Il2CppInterop.Runtime.IL2CPP.Il2CppObjectBaseToPtr(obj));
				things._size = size;
				foreach (int i in things) {
					_debug_log(i);
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

    [HarmonyPatch(typeof(CharacterController2D), "FixedUpdate")]
	class HarmonyPatch_CharacterController2D_FixedUpdate {

		private static void Postfix(CharacterController2D __instance) {
			try {
				keypress_update();
				if (!m_trigger_one_shot) {
					return;
				}
				m_trigger_one_shot = false;
				foreach (Il2CppSystem.Collections.Generic.KeyValuePair<EquipmentType, SpecDataBase> item in __instance.GetComponent<InstanceItemInventory>().currentEquipInInventory) {
					_debug_log($"key: {item.Key}, name: {item.Value.Name}");
				}
				foreach (IntegratedItem item in DataManager.Instance.IntegratedItemDic.Values) {
					if (item.specData == null) {
						continue;
					}
					_debug_log($"id: {item.ID}, name: {item.specData.Name}");
					if (item.ID == 3013133) {
						__instance.GetComponent<InstanceItemInventory>().currentEquipInInventory[EquipmentType.Harpoon] = item.specData;
					}
				}
				GameObject template = null;
				foreach (TextMeshProUGUI tmp in Resources.FindObjectsOfTypeAll<TextMeshProUGUI>()) {
					if (tmp.gameObject.name == "GoldText") {
						template = tmp.gameObject;
						break;
					}
				}
				GameObject obj = GameObject.Instantiate<GameObject>(template, template.transform.parent);
				RectTransform rect_transform = obj.GetComponent<RectTransform>();
				obj.transform.localPosition += Vector3.down * 10;
				obj.SetActive(true);
			} catch (Exception e) {
				logger.LogError("** HarmonyPatch_CharacterController2D_FixedUpdate.Postfix ERROR - " + e);
			}
		}
	}

	[HarmonyPatch(typeof(SushiBarManager), "Update")]
	class HarmonyPatch_SushiBarManager_Update {

		private static bool Prefix(SushiBarManager __instance) {
			try {
				keypress_update();
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
				_debug_log($"HarmonyPatch_DelayedDisappear_SetTime - inDisappearTime: {inDisappearTime}");
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
				keypress_update();
				/*
				if (m_trigger_one_shot) {
					m_trigger_one_shot = false;
					_debug_log(__instance.harpoonHeadSpec.HarpoonHeadType);
					_debug_log(__instance.harpoonHeadSpec.Damage);
					_debug_log(__instance.harpoonHeadSpec.IsMaxLevel);
					ReflectionUtils.il2cpp_get_field(__instance.harpoonSpec, "_harpoonItemType").SetValue(__instance.harpoonSpec, (int) HarpoonItemType.AlloyHarpoon);
					ReflectionUtils.il2cpp_get_field(__instance.harpoonHeadSpec, "m_HarpoonHeadType").SetValue(__instance.harpoonHeadSpec, (int) HarpoonHeadItemType.FireHead);
					ReflectionUtils.il2cpp_get_field(__instance.harpoonHeadSpec, "_Damage").SetValue(__instance.harpoonHeadSpec, 99);
					//foreach (SpecDataBase spec in Resources.FindObjectsOfTypeAll<SpecDataBase>()) {
					//	if (spec is HarpoonHeadSpecData) {
					//		_debug_log($"name: {spec.Name}, element: {spec.elementType}, damage: {spec.Damage}, isDefault: {spec.IsDefaultItem}, isMaxLevel: {spec.IsMaxLevel}");
					//	}
					//}
					if (false) {
						foreach (Il2CppSystem.Reflection.FieldInfo field in __instance.CurrentInstanceItemInventory.harpoonHandler.GetIl2CppType().GetFields((Il2CppSystem.Reflection.BindingFlags) 0xFFFFFFF)) {
							_debug_log($"name: {field.Name}, type: {field.FieldType}, val: " +
								field.FieldType.ToString() switch {
									"int" => "123123",
									_ => "---"
								}
							);
						}
					}
				}
				*/
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
                _debug_log($"Watering - itemIdx: {itemIdx}, x: {x}, y: {y}");
            } catch (Exception e) {
                logger.LogError("** XXXXX.Postfix ERROR - " + e);
            }
        }
    }

	[HarmonyPatch(typeof(SetUPCrabTrapCommand_SO), "UpExecute")]
	class HarmonyPatch_SetUPCrabTrapCommand_SO_1 {

		private static void Postfix() {
			//_debug_log("UpExecute");
		}
	}

	[HarmonyPatch(typeof(SetUPCrabTrapCommand_SO), "HoldExecute")]
	class HarmonyPatch_SetUPCrabTrapCommand_SO_2 {

		private static void Postfix(SetUPCrabTrapCommand_SO __instance) {
			//_debug_log("HoldExecute");
			//ReflectionUtils.il2cpp_get_field(__instance, "isExecuted").SetValue(__instance, true);
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