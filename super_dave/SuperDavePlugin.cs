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

    public const string VERSION = "0.0.9";

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
	private static ConfigEntry<bool> m_enabled;

	// Boat
	private static ConfigEntry<float> m_boat_walk_speed_boost;

	// Diving
	private static ConfigEntry<float> m_auto_pickup_radius;
	private static ConfigEntry<bool> m_auto_pickup_fish;
	private static ConfigEntry<bool> m_auto_pickup_items;
	private static ConfigEntry<bool> m_infinite_oxygen;
	private static ConfigEntry<bool> m_invincible;
	private static ConfigEntry<bool> m_toxic_aura_enabled;
	private static ConfigEntry<bool> m_toxic_aura_sleep;
	private static ConfigEntry<float> m_aura_radius;
	private static ConfigEntry<float> m_aura_update_frequency;
	private static ConfigEntry<float> m_speed_boost;
	private static ConfigEntry<bool> m_infinite_bullets;
    private static ConfigEntry<bool> m_weightless_items;
	private static ConfigEntry<bool> m_infinite_crab_traps;
	private static ConfigEntry<bool> m_infinite_drones;
	private static ConfigEntry<bool> m_large_pickups;
	//private static ConfigEntry<string> m_harpoon_type;
	//private static ConfigEntry<string> m_harpoon_head_type;
	//public static ConfigEntry<int> m_harpoon_head_level;
	private static ConfigEntry<bool> m_disable_item_info_popup;

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

	// Hotkeys
	private static ConfigEntry<string> m_hotkey_modifier;
	private static ConfigEntry<string> m_hotkey_toggle_aura_on;
	private static ConfigEntry<string> m_hotkey_change_aura_type;

	private const int HOTKEY_MODIFIER = 0;
	private const int HOTKEY_AURA_ON = 1;
	private const int HOTKEY_AURA_TYPE = 2;
	private static Dictionary<int, List<KeyCode>> m_hotkeys = null;

	class Singletons {
		private static T get_instance<T>(ref T instance) where T : MonoBehaviour {
			try {
				if (instance != null) {
					_ = instance.enabled;
					return instance;
				}
			} catch {}
			foreach (T obj in Resources.FindObjectsOfTypeAll<T>()) {
				return instance = obj;
			}
			return instance = null;
		}

		private static PlayerCharacter m_player = null;
		public static PlayerCharacter Player {
			get {
				return get_instance<PlayerCharacter>(ref m_player);
			}
		}
	}

	class DivingVars {
		private static DivingVars m_instance = null;
		public static DivingVars Instance {
			get {
				if (m_instance == null) {
					m_instance = new DivingVars();
				}
				return m_instance;
			}
		}
		public bool toxic_aura_enabled_by_hotkey = true;
		public bool toxic_aura_sleep_by_hotkey = true;
		public bool crab_traps_unlocked = false;
		public bool drones_unlocked = false;
		public Dictionary<HarpoonHeadItemType, List<HarpoonHeadSpecData>> harpoon_heads = new Dictionary<HarpoonHeadItemType, List<HarpoonHeadSpecData>>();
		public Dictionary<HarpoonItemType, HarpoonSpecData> harpoon_specs = new Dictionary<HarpoonItemType, HarpoonSpecData>();
		
		private DivingVars() {
			this.toxic_aura_enabled_by_hotkey = m_toxic_aura_enabled.Value;
			this.toxic_aura_sleep_by_hotkey = m_toxic_aura_sleep.Value;
			foreach (HarpoonSpecData spec in Resources.FindObjectsOfTypeAll<HarpoonSpecData>()) {
				//_debug_log($"{spec.Name}");
				this.harpoon_specs[spec.HarpoonType] = spec;
			}
			foreach (HarpoonHeadSpecData spec in Resources.FindObjectsOfTypeAll<HarpoonHeadSpecData>()) {
				HarpoonHeadItemType head_type = (HarpoonHeadItemType) ReflectionUtils.il2cpp_get_field_value<int>(spec, "m_HarpoonHeadType");
				if (!this.harpoon_heads.ContainsKey(head_type)) {
					this.harpoon_heads[head_type] = new List<HarpoonHeadSpecData>();
				}
				//_debug_log($"{head_type} {spec.Damage}");
				this.harpoon_heads[head_type].Add(spec);
			}
			foreach (List<HarpoonHeadSpecData> specs in this.harpoon_heads.Values) {
				specs.Sort((x, y) => x.Damage.CompareTo(y.Damage));
			}
		}

		public void initialize() {

		}

		/*
		public void set_harpoon_item(InstanceItemInventory inventory) {
			HarpoonItemType type;
			if (string.IsNullOrEmpty(m_harpoon_type.Value)) {
				return;
			}
			if (!Enum.TryParse<HarpoonItemType>(m_harpoon_type.Value + "Harpoon", out type)) {
				logger.LogError($"* set_harpoon_item WARNING - '{m_harpoon_type.Value}' is not a recognized type (see help info in config file).");
				return;
			}
			inventory.currentEquipInInventory[EquipmentType.Harpoon] = this.harpoon_specs[type];
			_debug_log($"{inventory.GetEquipedItem(EquipmentType.Harpoon).Name}");
		}

		public void set_harpoon_head(InstanceItemInventory inventory) {
			HarpoonHeadItemType type;
			if (string.IsNullOrEmpty(m_harpoon_head_type.Value)) {
				return;
			}
			if (!Enum.TryParse<HarpoonHeadItemType>(m_harpoon_head_type.Value + "Head", out type)) {
				logger.LogError($"* set_harpoon_head WARNING - '{m_harpoon_head_type.Value}' is not a recognized type (see help info in config file).");
				return;
			}
			int level = m_harpoon_head_level.Value;
			inventory.currentEquipInInventory[EquipmentType.HarpoonHead] = harpoon_heads[type][(level < 0 ? 0 : (level > harpoon_heads[type].Count - 1 ? harpoon_heads[type].Count - 1 : level))];
			_debug_log($"{inventory.GetEquipedItem(EquipmentType.HarpoonHead).Name}");
		}
		*/
	}

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
			m_auto_pickup_fish = this.Config.Bind<bool>("Diving", "Diving - Auto-pickup: Fish", false, "Set to true to enable auto pickup of sleeping/netted fish (NOTE: This will disable the Large Pickups option to prevent drone glitches; just be sure to enable Infinite Drones when using this one).");
			m_auto_pickup_items = this.Config.Bind<bool>("Diving", "Diving - Auto-pickup: Items", false, "Set to true to enable auto pickup of items (wood, pots, etc).");
			m_auto_pickup_radius = this.Config.Bind<float>("Diving", "Diving - Auto-pickup: Radius", 3.0f, "Radius (in meters?) around the character in which objects will be automatically picked up based on enabled pickup settings (float, default 3.0f).");
			//m_harpoon_type = this.Config.Bind<string>("Diving", "Diving - Harpoon Type", "", "Harpoon type (one of: Old, Iron, Pump, Merman, NewMV, Alloy) [case sensitive, set to blank to disable].");
			//m_harpoon_head_type = this.Config.Bind<string>("Diving", "Diving - Harpoon Head Type", "", "Harpoon head type (one of: Normal, Electric, Poison, Chain, Sleep, Paralysis, Strong, Fire, Ice) [case sensitive, set to blank to disable].");
			//m_harpoon_head_level = this.Config.Bind<int>("Diving", "Diving - Harpoon Head Level", 0, "Harpoon head level [ignored if Harpoon Head Type is not set] (int, 1 (weakest) - 5 (strongest)).");			
			m_infinite_bullets = this.migrate_option<bool>("Diving", "Infinite Bullets", "Diving - Infinite Bullets", false, "Set to true to have infinite bullets when diving.");
			m_infinite_crab_traps = this.Config.Bind<bool>("Diving", "Diving - Infinite Crab Traps", false, "Set to true to enable infinite crab traps.");
			m_infinite_drones = this.Config.Bind<bool>("Diving", "Diving - Infinite Drones", false, "Set to true to enable infinite salvage drones.");
			m_infinite_oxygen = this.migrate_option<bool>("Diving", "Infinite Oxygen", "Diving - Infinite Oxygen", false, "Set to true to have infinite oxygen when diving (and when not diving, but that's a freebie).");
			m_invincible = this.migrate_option<bool>("Diving", "Invincible", "Diving - Invincible", false, "Set to true to take no damage when hit.");
			m_disable_item_info_popup = this.Config.Bind<bool>("Diving", "Diving - Disable Item Info Popups", false, "Set to true to disable the item info popup windows that tend to get WAY behind if using auto-pickup.");
			m_large_pickups = this.Config.Bind<bool>("Diving", "Diving - Enable Large Pickups", false, "Set to true to enable large fish to be picked up without the need for drones.");
			m_speed_boost = this.migrate_option<float>("Diving", "Speed Boost", "Diving - Speed Boost", 0f, "Permanent speed boost when diving (float, default 0f [set to 0 to disable]).");
			m_toxic_aura_enabled = this.migrate_option<bool>("Diving", "Toxic Aura: Enabled", "Diving - Toxic Aura: Enabled", false, "Set to true to enable the instant-fish-killing (or sleeping if 'Toxic Aura: Sleep Effect' is true) aura around Dave.");
			m_toxic_aura_sleep = this.Config.Bind<bool>("Diving", "Diving - Toxic Aura: Sleep Effect", false, "Set to true to switch from killing aura to sleep-inducing aura.");
			m_aura_radius = this.migrate_option<float>("Diving", "Toxic Aura: Radius", "Diving - Toxic Aura: Radius", 3.0f, "Radius (in meters?) around the character in which fish will be insta-killed, if Toxic Aura is enabled (float, default 3.0f).");
			m_aura_update_frequency = this.Config.Bind<float>("Diving", "Diving - Toxic Aura: Update Frequency", 0.5f, "Time (in seconds) between ticks/pulses of toxic aura (float, default 0.5f [i.e. 2 pulses per second], lower numbers may impact frame rate).");
			m_weightless_items = this.migrate_option<bool>("Diving", "Weightless Items (Infinite Carry Weight)", "Diving - Weightless Items (Infinite Carry Weight)", false, "Set to true to have reduce the weight of all items to 0, effectively giving infinite carry weight and inventory space.");

			// Hotkeys
			m_hotkey_modifier = this.Config.Bind<string>("Hotkeys", "Hotkey - Modifier", "LeftControl,RightControl", "Comma-separated list of Unity Keycodes used as the special modifier key (i.e. ctrl,alt,command) one of which is required to be down for hotkeys to work.  Set to '' (blank string) to not require a special key (not recommended).  See this link for valid Unity KeyCode strings (https://docs.unity3d.com/ScriptReference/KeyCode.html)");
			m_hotkey_toggle_aura_on = this.Config.Bind<string>("Hotkeys", "Hotkey - Toggle Toxic Aura On/Off", "Backspace", "Comma-separated list of Unity Keycodes, any of which will toggle the toxic aura on/off (if enabled).  See this link for valid Unity KeyCode strings (https://docs.unity3d.com/ScriptReference/KeyCode.html)");
			m_hotkey_change_aura_type = this.Config.Bind<string>("Hotkeys", "Hotkey - Change Toxic Aura Mode", "Backslash", "Comma-separated list of Unity Keycodes, any of which will change the toxic aura mode (if enabled) between Kill/Sleep.  See this link for valid Unity KeyCode strings (https://docs.unity3d.com/ScriptReference/KeyCode.html)");
			
			// Sushi Bar
			m_staff_cook_multiplier = this.Config.Bind<float>("Sushi", "Sushi - Cook Multiplier", 0f, "Multiplier applied to sushi bar staff cooking speed (float, default 0f [ < 1 == faster, > 1 == slower, set to 0 to disable]).");
			m_infinite_customer_patience = this.migrate_option<bool>("Sushi", "Infinite Customer Patience", "Sushi - Infinite Customer Patience", false, "Set to true to make customers never storm off if the food/drinks are too slow.");
			m_infinite_wasabi = this.Config.Bind<bool>("Sushi", "Sushi - Infinite Wasabi", false, "Set to true to never need to refill wasabi.");
			m_sushi_money_boost = this.Config.Bind<float>("Sushi", "Sushi - Money Boost", 0f, "Money boost applied to all customer purchases at sushi bar (float, default 0f [set to 0 to disable]).");
			m_sushi_speed_boost = this.migrate_option<float>("Sushi", "Sushi Speed Boost", "Sushi - Speed Boost", 0f, "Permanent speed boost when working in sushi bar (float, default 0f [set to 0 to disable]).");
			m_staff_walk_multiplier = this.Config.Bind<float>("Sushi", "Sushi - Walk Multiplier", 0f, "Multiplier applied to sushi bar staff walking speed [note: this also affects Dave on top of 'Sushi - Speed Boost'] (float, default 0f [ < 1 == faster, > 1 == slower, set to 0 to disable]).");

			this.plugin_info = PluginInfo.to_dict();
			this.create_nexus_page();

			m_hotkeys = new Dictionary<int, List<KeyCode>>();
			set_hotkey(m_hotkey_modifier.Value, HOTKEY_MODIFIER);
			set_hotkey(m_hotkey_toggle_aura_on.Value, HOTKEY_AURA_ON);
			set_hotkey(m_hotkey_change_aura_type.Value, HOTKEY_AURA_TYPE);

			this.m_harmony.PatchAll();
			PluginUpdater.create(this, logger);
			PluginUpdater.Instance.register("global", 1.0f, global_update);
			PluginUpdater.Instance.register("keypress", 0f, keypress_update);
			PluginUpdater.Instance.register("toxic_aura_update", 0f, toxic_aura_update);
			logger.LogInfo($"{PluginInfo.GUID} v{PluginInfo.VERSION} loaded.");
		} catch (Exception e) {
			logger.LogError("** Awake FATAL - " + e);
		}
	}

	private static void _debug_log(object text) {
		logger.LogInfo(text);
	}

	private static void set_hotkey(string keys_string, int key_index) {
		m_hotkeys[key_index] = new List<KeyCode>();
		foreach (string key in keys_string.Split(',')) {
			string trimmed_key = key.Trim();
			if (trimmed_key != "") {
				m_hotkeys[key_index].Add((KeyCode) System.Enum.Parse(typeof(KeyCode), trimmed_key));
			}
		}
	}

	private static bool is_modifier_hotkey_down() {
		if (m_hotkeys[HOTKEY_MODIFIER].Count == 0) {
			return true;
		}
		foreach (KeyCode key in m_hotkeys[HOTKEY_MODIFIER]) {
			if (Input.GetKey(key)) {
				return true;
			}
		}
		return false;
	}

	private static bool is_hotkey_down(int key_index) {
		foreach (KeyCode key in m_hotkeys[key_index]) {
			if (Input.GetKeyDown(key)) {
				return true;
			}
		}
		return false;
	}

	private static void global_update() {

	}

	private static void keypress_update() {
		if (!is_modifier_hotkey_down()) {
			return;
		}
		if (is_hotkey_down(HOTKEY_AURA_ON)) {
			if (DivingVars.Instance != null) {
				DivingVars.Instance.toxic_aura_enabled_by_hotkey = !DivingVars.Instance.toxic_aura_enabled_by_hotkey;
			}
		}
		if (is_hotkey_down(HOTKEY_AURA_TYPE)) {
			if (DivingVars.Instance != null) {
				DivingVars.Instance.toxic_aura_sleep_by_hotkey = !DivingVars.Instance.toxic_aura_sleep_by_hotkey;
			}
		}
	}

	private static void toxic_aura_update() {
		try {
			if (!m_enabled.Value || Singletons.Player == null) {
				return;
			}
			_debug_log(".");
		} catch (Exception e) {
			logger.LogError("** toxic_aura_update ERROR - " + e);
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
				if (!m_enabled.Value) {
					return;
				}
				DivingVars.Instance.initialize();
				if (m_modified_instance_hash != __instance.GetHashCode() && m_boat_walk_speed_boost.Value > 0) {
					m_modified_instance_hash = __instance.GetHashCode();
					ReflectionUtils.il2cpp_get_field(__instance, "m_MoveSpeed").SetValue(
						__instance,
						ReflectionUtils.il2cpp_get_field_value<float>(__instance, "m_MoveSpeed") + m_boat_walk_speed_boost.Value
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
				if (m_enabled.Value && m_speed_boost.Value > 0 && __instance.gameObject.name == "DaveCharacter") {
					__instance.GetBuffComponents.AddMoveSpeedParam(1234567, m_speed_boost.Value);
				}
			} catch (Exception e) {
				logger.LogError("** HarmonyPatch_BuffHandler_Start.Postfix ERROR - " + e);
			}
		}
	}

	[HarmonyPatch(typeof(CharacterController2D), "FixedUpdate")]
	class HarmonyPatch_CharacterController2D_FixedUpdate {

		private const float UPDATE_FREQUENCY = 0.1f;
		private static float m_elapsed = UPDATE_FREQUENCY;
		private const int SLEEP_BUFF_ID = 14080415;
		private const float SLEEP_BUFF_VALUE = 9999999999f;
		private static bool m_did_set_sleep_buff_value = false;
		private static int m_modified_instance_hash = 0;
		private static PlayerCharacter m_player = null;
		private static List<GameObject> m_next_frame_destroy_objects = new List<GameObject>();
		
		private static void auto_pickup<T>(bool enabled, Vector3 player_pos) where T : MonoBehaviour {
			if (!enabled) {
				return;
			}
			foreach (T item in Resources.FindObjectsOfTypeAll<T>()) {
				if (Vector3.Distance(player_pos, item.transform.position) > m_auto_pickup_radius.Value || m_next_frame_destroy_objects.Contains(item.gameObject)) {
					continue;
				}
				if (item is FishInteractionBody fish) {
					if (fish.CheckAvailableInteraction(m_player) && fish.InteractionType == FishInteractionBody.FishInteractionType.Pickup) {
						fish.SuccessInteract(m_player);
						m_next_frame_destroy_objects.Add(fish.gameObject);
					}
				} else if (item is PickupInstanceItem pickup) {
					if (pickup.CheckAvailableInteraction(m_player)) {
						pickup.SuccessInteract(m_player);
						m_next_frame_destroy_objects.Add(pickup.gameObject);
					}
				} else if (item is InstanceItemChest chest && chest.gameObject.name.Contains("IngredientPot")) {
					chest.SuccessInteract(m_player);
					m_next_frame_destroy_objects.Add(chest.gameObject);
				} else if (item is BreakableLootObject breakable) {
					breakable.OnTakeDamage(new AttackData() {
						damage = 99999,
						attackType = AttackType.Player_Melee
					}, new DefenseData() {
					});
					if (breakable.IsDead()) {
						m_next_frame_destroy_objects.Add(breakable.gameObject);
					}
				} else if (item is CrabTrapZone crab_trap_zone && !item.transform.Find("CrabTrap(Clone)")) {
					if (crab_trap_zone.CheckAvailableInteraction(m_player)) {
						crab_trap_zone.SetUpCrabTrap(9);
					}
				}
			}
		}

		private static void Postfix(CharacterController2D __instance) {
			try {
				if (!m_enabled.Value || (m_elapsed += Time.fixedDeltaTime) < UPDATE_FREQUENCY) {
					return;
				}
				foreach (GameObject obj in m_next_frame_destroy_objects) {
					GameObject.Destroy(obj);
				}
				m_elapsed = 0f;
				if (DivingVars.Instance != null) {
					if (m_modified_instance_hash != __instance.GetHashCode()) {
						m_modified_instance_hash = __instance.GetHashCode();
						m_player = Resources.FindObjectsOfTypeAll<PlayerCharacter>()[0];
						InstanceItemInventory inventory = __instance.GetComponent<InstanceItemInventory>();
						//DivingVars.Instance.set_harpoon_item(inventory);
						//DivingVars.Instance.set_harpoon_head(inventory);
					}
				}
				if (m_toxic_aura_enabled.Value && DivingVars.Instance.toxic_aura_enabled_by_hotkey) {
					foreach (FishInteractionBody fish in Resources.FindObjectsOfTypeAll<FishInteractionBody>()) {
						if (!m_auto_pickup_fish.Value && fish.InteractionType == FishInteractionBody.FishInteractionType.Calldrone && m_large_pickups.Value) {
							fish.InteractionType = FishInteractionBody.FishInteractionType.Pickup;
						}
						if (Vector3.Distance(__instance.transform.position, fish.transform.position) <= m_aura_radius.Value) {
							if (m_toxic_aura_sleep.Value && DivingVars.Instance.toxic_aura_sleep_by_hotkey) {
								if (!m_did_set_sleep_buff_value) {
									DataManager.Instance.BuffEffectDataDic[SLEEP_BUFF_ID].buffvalue1 = SLEEP_BUFF_VALUE;
									DataManager.Instance.BuffEffectDataDic[SLEEP_BUFF_ID].buffvalue2 = SLEEP_BUFF_VALUE;
								}
								BuffHandler buff_handler = fish.gameObject.GetComponent<BuffHandler>();
								if (buff_handler != null) {
									Il2CppSystem.Object buff_dict = ReflectionUtils.il2cpp_get_field(buff_handler, "CJCBPPIBGLB")?.GetValue(buff_handler);
									bool is_asleep = false;
									if (buff_dict != null && ReflectionUtils.il2cpp_get_field_value<int>(buff_dict, "count") > 0) {
										// TODO: Assuming any buff is sleep is not likely to work.  Should dig into the dict entries.
										// debug_log(buff_handler.HasBuffType(BuffType.Sleep));
										is_asleep = true;
									}
									if (!is_asleep) {
										buff_handler.AddBuff(SLEEP_BUFF_ID);
									} else if (fish.InteractionType == FishInteractionBody.FishInteractionType.Pickup) {
										
									}
								}
							} else {
								fish.gameObject.GetComponent<Damageable>().OnDie();
							}
						}
					}
				}
				auto_pickup<FishInteractionBody>(m_auto_pickup_fish.Value, __instance.transform.position);
				auto_pickup<PickupInstanceItem>(m_auto_pickup_items.Value, __instance.transform.position);
				auto_pickup<InstanceItemChest>(m_auto_pickup_items.Value, __instance.transform.position);
				//auto_pickup<BreakableLootObject>(m_auto_pickup_items.Value, __instance.transform.position);
				auto_pickup<CrabTrapZone>(m_infinite_crab_traps.Value, __instance.transform.position);
			} catch (Exception e) {
				logger.LogError("** HarmonyPatch_CharacterController2D_FixedUpdate.Prefix ERROR - " + e);
			}
		}
	}

	[HarmonyPatch(typeof(PlayerCharacter), "Update")]
	class HarmonyPatch_PlayerCharacter_Update {

		private const float UPDATE_FREQUENCY = 1.0f;
		private static float m_elapsed = UPDATE_FREQUENCY;
		private static int m_modified_instance_hash = 0;

		private static void Postfix(PlayerCharacter __instance) {
			try {
				if (!m_enabled.Value || (m_elapsed += Time.deltaTime) < UPDATE_FREQUENCY) {
					return;
				}
				if (DivingVars.Instance != null && m_modified_instance_hash != __instance.GetHashCode()) {
					m_modified_instance_hash = __instance.GetHashCode();
					DivingVars.Instance.drones_unlocked = __instance.AvailableLiftDroneCount > 0;
					DivingVars.Instance.crab_traps_unlocked = __instance.AvailableCrabTrapCount > 0;
				}
				if (m_infinite_drones.Value) {
					SROptions.Current.RefillDrone();
				}
				try {
					if (m_infinite_bullets.Value) {
						__instance?.CurrentInstanceItemInventory?.gunHandler.ForceSetBulletCount(999);
					}
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

	[HarmonyPatch(typeof(GetInfoPanelUI), "WaitOnPopup")]
	class HarmonyPatch_GetInfoPanelUI_WaitOnPopup {
		private static void Postfix(GetInfoPanelUI __instance, GetInfoPanelUI.GetItemInfo info) {
			_debug_log($"WaitOnPopup - tid: {info.itemTID}");
			if (m_enabled.Value && m_disable_item_info_popup.Value) {
				__instance.gameObject.SetActive(false);
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
					__result /= m_staff_cook_multiplier.Value;
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