﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using DR;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Runtime.CompilerServices;
using Il2CppSystem.Globalization;

class Diving {
    private static Diving m_instance = null;
    public static Diving Instance {
        get {
            if (m_instance == null) {
                m_instance = new Diving();
            }
            return m_instance;
        }
    }
    private static DDPlugin m_plugin = null;
    private bool m_is_initialized = false;
    private bool m_toxic_aura_enabled_by_hotkey = true;
    private bool m_toxic_aura_sleep_by_hotkey = true;
    private const int SLEEP_BUFF_ID = 14080415;
    private const float SLEEP_BUFF_VALUE = 9999999999f;
    private static bool m_did_set_sleep_buff_value = false;
    private Dictionary<HarpoonHeadItemType, List<HarpoonHeadSpecData>> harpoon_heads = new Dictionary<HarpoonHeadItemType, List<HarpoonHeadSpecData>>();
    private Dictionary<HarpoonItemType, HarpoonSpecData> harpoon_specs = new Dictionary<HarpoonItemType, HarpoonSpecData>();
    private static List<GameObject> m_next_frame_destroy_objects = new List<GameObject>();
    private static List<IntegratedItemType> m_pickup_item_types = new List<IntegratedItemType>();
    private static List<string> m_all_pickup_item_names = new List<string>();
    private static Dictionary<string, bool> m_enabled_pickup_items = new Dictionary<string, bool>();

    private void initialize() {
        if (this.m_is_initialized) {
            return;
        }
        this.m_toxic_aura_enabled_by_hotkey = Settings.m_toxic_aura_enabled.Value;
        this.m_toxic_aura_sleep_by_hotkey = Settings.m_toxic_aura_sleep.Value;
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
        this.load_enabled_pickup_items();
        this.m_is_initialized = true;
    }

    public static void load(DDPlugin plugin) {
        m_plugin = plugin;
        PluginUpdater.Instance.register("Diving.auto_pickup_update", Settings.m_auto_pickup_frequency.Value, Updaters.auto_pickup_update);
        PluginUpdater.Instance.register("Diving.general_update", 1f, Updaters.general_update);
        PluginUpdater.Instance.register("Diving.toxic_aura_update", Settings.m_aura_update_frequency.Value, Updaters.toxic_aura_update);
    }

    public void load_enabled_pickup_items() {
        int counter = 1;
        bool did_error = false;
        string path = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "config", "auto-pickup-items.txt"));
        try {
            m_all_pickup_item_names.Sort();
            foreach (string key in m_all_pickup_item_names) {
                m_enabled_pickup_items[key] = DefaultAutoPickupItems.enabled_items.Contains(key);
            }
            DDPlugin._info_log($"Loading enabled auto-pickup item info from '{path}'.");
            if (File.Exists(path)) {
                foreach (string _line in File.ReadAllText(path).Split('\n')) {
                    counter++;
                    string line = _line.Trim();
                    if (string.IsNullOrEmpty(line) || line[0] == '#' || !m_all_pickup_item_names.Contains(line)) {
                        continue;
                    }
                    m_enabled_pickup_items[line] = true;
                }
            } else {
                DDPlugin._info_log("File does not exist.");
            }
        } catch (Exception e) {
            did_error = true;
            DDPlugin._warn_log($"* load_enabled_pickup_items WARNING - error parsing auto-pickup-items.txt file (line: {counter}); auto-pickup of items will be disabled until this issue is resolved.  Error = " + e);
            foreach (string key in m_all_pickup_item_names) {
                m_enabled_pickup_items[key] = false;
            }
        }
        try {
            if (did_error && File.Exists(path)) {
                return;
            }
            string output = @"

# Notes on this file:
#
# - Each line in this file represents an item name key in Dave the Diver.
# - This is a list of *EVERY* item in the game, and not all
#   items will be something that can be picked up.
# - Fish are picked up using separate logic, so fish names can be ignored.
# - Set 'Auto-pickup: Debug Mode' to true to have the mod print debug statements to
#   LogOutput.log (and the console) when the mod does not pick up an item.  Use the
#   'name_key' info in those log statements to find the item to enable in this file.
# - Un-comment the line (remove the #) to enable pickup of the item.
# - Empty lines or lines beginning with # will be ignored.
# - Be sure to set 'Auto-pickup: Debug Mode' to false when not debugging, as it can
#   clutter up the log file.

";
            foreach (string key in m_all_pickup_item_names) {
                output += $"{(m_enabled_pickup_items[key] ? "" : "# ")}{key}\n";
            }
            File.WriteAllText(path, output);
        } catch (Exception e) {
            DDPlugin._warn_log($"* load_enabled_pickup_items WARNING - unable to write item info to file.  Error = " + e);
        }
    }

    /*
    public void set_harpoon_item(InstanceItemInventory inventory) {
        HarpoonItemType type;
        if (string.IsNullOrEmpty(m_harpoon_type.Value)) {
            return;
        }
        if (!Enum.TryParse<HarpoonItemType>(m_harpoon_type.Value + "Harpoon", out type)) {
            DDPlugin._error_log($"* set_harpoon_item WARNING - '{m_harpoon_type.Value}' is not a recognized type (see help info in config file).");
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
            DDPlugin._error_log($"* set_harpoon_head WARNING - '{m_harpoon_head_type.Value}' is not a recognized type (see help info in config file).");
            return;
        }
        int level = m_harpoon_head_level.Value;
        inventory.currentEquipInInventory[EquipmentType.HarpoonHead] = harpoon_heads[type][(level < 0 ? 0 : (level > harpoon_heads[type].Count - 1 ? harpoon_heads[type].Count - 1 : level))];
        _debug_log($"{inventory.GetEquipedItem(EquipmentType.HarpoonHead).Name}");
    }
    */

    class Patches {
        [HarmonyPatch(typeof(BuffHandler), "Start")]
        class HarmonyPatch_BuffHandler_Start {
            private static void Postfix(BuffHandler __instance) {
                try {
                    if (Settings.m_enabled.Value && Settings.m_speed_boost.Value > 0 && __instance.gameObject.name == "DaveCharacter") {
                        __instance.GetBuffComponents.AddMoveSpeedParam(1234567, Settings.m_speed_boost.Value);
                    }
                } catch (Exception e) {
                    DDPlugin._error_log("** HarmonyPatch_BuffHandler_Start.Postfix ERROR - " + e);
                }
            }
        }

        [HarmonyPatch(typeof(GetInfoPanelUI), "WaitOnPopup")]
        class HarmonyPatch_GetInfoPanelUI_WaitOnPopup {
            private static void Postfix(GetInfoPanelUI __instance, GetInfoPanelUI.GetItemInfo info) {
                if (Settings.m_enabled.Value && Settings.m_disable_item_info_popup.Value) {
                    __instance.gameObject.SetActive(false);
                }
            }
        }

        [HarmonyPatch(typeof(IntegratedItem), "BuildItem")]
        class HarmonyPatch_IntegratedItem_BuildItem {
            private static bool Prefix(Items itemBase) {
                try {
                    if (Settings.m_enabled.Value && Settings.m_weightless_items.Value) {
                        ReflectionUtils.get_property(itemBase, "ItemWeight").SetValue(itemBase, 0);
                    }
                    if (!string.IsNullOrEmpty(itemBase.ItemTextID) && !m_all_pickup_item_names.Contains(itemBase.ItemTextID)) {
                        m_all_pickup_item_names.Add(itemBase.ItemTextID);
                    }
                    return true;
                } catch (Exception e) {
                    DDPlugin._error_log("** HarmonyPatch_IntegratedItem_BuildItem.Prefix ERROR - " + e);
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(PlayerBreathHandler), "Update")]
        class HarmonyPatch_PlayerBreathHandler_Update {
            private static bool Prefix(PlayerBreathHandler __instance) {
                if (Settings.m_enabled.Value && Settings.m_infinite_oxygen.Value) {
                    __instance.SetBreathInvincible(true);
                }
                if (Settings.m_enabled.Value && Settings.m_invincible.Value) {
                    __instance.SetDamageInvicible(true);
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(PlayerCharacter), "IsCrabTrapAvailable", MethodType.Getter)]
        class HarmonyPatch_PlayerCharacter_IsCrabTrapAvailable {

            private static void Postfix(PlayerCharacter __instance, ref bool __result) {
                try {
                    if (Settings.m_enabled.Value && Settings.m_infinite_crab_traps.Value) {
                        __result = true;
                    }
                } catch (Exception e) {
                    DDPlugin._error_log("** HarmonyPatch_PlayerCharacter_IsCrabTrapAvailable.Prefix ERROR - " + e);
                }
            }
        }

        [HarmonyPatch(typeof(PlayerCharacter), "SetHPDamage")]
        class HarmonyPatch_PlayerCharacter_SetHPDamage {
            private static bool Prefix() {
                try {
                    return !(Settings.m_enabled.Value && Settings.m_invincible.Value);
                } catch (Exception e) {
                    DDPlugin._error_log("** HarmonyPatch_PlayerCharacter_SetHPDamage.Prefix ERROR - " + e);
                }
                return true;
            }
        }
    }

    public class Updaters {
        private static void auto_pickup<T>(bool enabled, PlayerCharacter player, Vector3 player_pos, Func<T, bool> callback) where T : MonoBehaviour {
            if (!enabled) {
                return;
            }
            foreach (T item in Resources.FindObjectsOfTypeAll<T>()) {
                if (item.transform.position == Vector3.zero || Vector3.Distance(player_pos, item.transform.position) > Settings.m_auto_pickup_radius.Value || m_next_frame_destroy_objects.Contains(item.gameObject)) {
                    continue;
                }
                if (item is FishInteractionBody fish) {
                    if (fish.CheckAvailableInteraction(player) && fish.InteractionType == FishInteractionBody.FishInteractionType.Pickup && (callback == null || callback(item))) {
                        fish.SuccessInteract(player);
                        m_next_frame_destroy_objects.Add(fish.gameObject);
                    }
                } else if (item is PickupInstanceItem pickup && (callback == null || callback(item))) {
                    try {
                        if (pickup.CheckAvailableInteraction(player)) {
                            pickup.SuccessInteract(player);
                            m_next_frame_destroy_objects.Add(pickup.gameObject);
                        }
                    } catch { }
                } else if (item is InstanceItemChest chest && (callback == null || callback(item))) {
                    chest.SuccessInteract(player);
                    m_next_frame_destroy_objects.Add(chest.gameObject);
                } else if (item is BreakableLootObject breakable && (callback == null || callback(item))) {
                    breakable.OnTakeDamage(new AttackData() {
                        damage = 99999,
                        attackType = AttackType.Player_Melee
                    }, new DefenseData() {
                    });
                    if (breakable.IsDead()) {
                        m_next_frame_destroy_objects.Add(breakable.gameObject);
                    }
                } else if (item is CrabTrapZone crab_trap_zone && !item.transform.Find("CrabTrap(Clone)") && player.AvailableCrabTrapCount > 0 && (callback == null || callback(item))) {
                    if (crab_trap_zone.CheckAvailableInteraction(player)) {
                        crab_trap_zone.SetUpCrabTrap(9);
                    }
                }
            }
        }

        private static List<int> m_debug_echoed_item_hashes = new List<int>();

        public static bool auto_pickup_callback_PickupInstanceItem(PickupInstanceItem _item) {
            try {
                if (_item == null) {
                    return false;
                }
                string text_id = "";
                int hash = 0;
                try {
                    IntegratedItem item = DataManager.Instance.GetIntegratedItem((_item.usePreset ? _item.presetItemID : _item.GetItemID()));
                    text_id = item.ItemTextID;
                    hash = item.GetHashCode();
                } catch {
                    return false;
                }
                if (!m_enabled_pickup_items[text_id] && Settings.m_auto_pickup_debug_mode.Value && !m_debug_echoed_item_hashes.Contains(hash)) {
                    DDPlugin._debug_log($"^^ Auto-Pickup Item DEBUG - name_key: {text_id}; not enabled for pickup.");
                    if (!m_enabled_pickup_items[text_id]) {
                        m_debug_echoed_item_hashes.Add(hash);
                    }
                }
                return m_enabled_pickup_items[text_id];
            } catch (Exception e) {
                DDPlugin._warn_log($"* auto_pickup_callback_PickupInstanceItem WARNING - {_item.usePreset} {_item.presetItemID} {DataManager.Instance.GetIntegratedItem(_item.GetItemID())}" + e);
            }
            return false;
        }

        public static void auto_pickup_update() {
            try {
                PlayerCharacter player;
                CharacterController2D character;
                if (!Settings.m_enabled.Value || (player = Singletons.Player) == null || !player.isActiveAndEnabled || (character = Singletons.Character) == null || !character.isActiveAndEnabled) {
                    return;
                }
                Diving.Instance.initialize();
                auto_pickup<FishInteractionBody>(Settings.m_auto_pickup_fish.Value, player, character.transform.position, null);
                auto_pickup<PickupInstanceItem>(Settings.m_auto_pickup_items.Value, player, character.transform.position, auto_pickup_callback_PickupInstanceItem);
                auto_pickup<InstanceItemChest>(Settings.m_auto_pickup_items.Value, player, character.transform.position, null);
                //auto_pickup<BreakableLootObject>(Settings.m_auto_pickup_items.Value, player, character.transform.position);
                auto_pickup<CrabTrapZone>(Settings.m_auto_drop_crab_traps.Value, player, character.transform.position, null);
            } catch (Exception e) {
                DDPlugin._error_log("** Diving.auto_pickup_update ERROR - " + e);
            }
        }

        public static void general_update() {
            try {
                PlayerCharacter player;
                if (!Settings.m_enabled.Value || (player = Singletons.Player) == null || !player.isActiveAndEnabled) {
                    return;
                }
                Diving.Instance.initialize();
                if (Settings.m_infinite_drones.Value) {
                    SROptions.Current.RefillDrone();
                }
                try {
                    if (Settings.m_infinite_bullets.Value) {
                        player?.CurrentInstanceItemInventory?.gunHandler.ForceSetBulletCount(999);
                    }
                } catch { }
            } catch (Exception e) {
                DDPlugin._error_log("** Diving.general_update ERROR - " + e);
            }
        }

        public static void keypress_update() {
            if (!Diving.Instance.m_is_initialized) { 
                return; 
            }
            if (Hotkeys.is_hotkey_down(Hotkeys.HOTKEY_AURA_ON)) {
                Diving.Instance.m_toxic_aura_enabled_by_hotkey = !Diving.Instance.m_toxic_aura_enabled_by_hotkey;
                PluginUpdater.Instance.trigger("toxic_aura_update");
            }
            if (Hotkeys.is_hotkey_down(Hotkeys.HOTKEY_AURA_TYPE)) {
                Diving.Instance.m_toxic_aura_sleep_by_hotkey = !Diving.Instance.m_toxic_aura_sleep_by_hotkey;
                PluginUpdater.Instance.trigger("toxic_aura_update");
            }
        }

        public static int m_modified_character_hash = 0;

        public static void toxic_aura_update() {
            try {
                CharacterController2D character;
                if (!Settings.m_enabled.Value || (character = Singletons.Character) == null || !character.isActiveAndEnabled || !Diving.Instance.m_toxic_aura_enabled_by_hotkey) {
                    return;
                }
                Diving.Instance.initialize();
                if (m_modified_character_hash != character.GetHashCode()) {
                    m_modified_character_hash = character.GetHashCode();
                    
                }
                foreach (FishInteractionBody fish in Resources.FindObjectsOfTypeAll<FishInteractionBody>()) {
                    if (!Settings.m_auto_pickup_fish.Value && fish.InteractionType == FishInteractionBody.FishInteractionType.Calldrone && Settings.m_large_pickups.Value) {
                        fish.InteractionType = FishInteractionBody.FishInteractionType.Pickup;
                    }
                    if (Vector3.Distance(character.transform.position, fish.transform.position) <= Settings.m_aura_radius.Value) {
                        if (Diving.Instance.m_toxic_aura_sleep_by_hotkey) {
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
            } catch (Exception e) {
                DDPlugin._error_log("** Diving.toxic_aura_update ERROR - " + e);
            }
        }
    }
}