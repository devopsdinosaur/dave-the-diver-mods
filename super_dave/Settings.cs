using BepInEx.Configuration;

public class Settings {
    private static Settings m_instance = null;
    public static Settings Instance {
        get {
            if (m_instance == null) {
                m_instance = new Settings();
            }
            return m_instance;
        }
    }
    private DDPlugin m_plugin = null;

    // General
    public static ConfigEntry<bool> m_enabled;

    // Boat
    public static ConfigEntry<float> m_boat_walk_speed_boost;

    // Diving
    public static ConfigEntry<float> m_auto_pickup_radius;
    public static ConfigEntry<bool> m_auto_pickup_fish;
    public static ConfigEntry<bool> m_auto_pickup_items;
    public static ConfigEntry<string> m_auto_pickup_item_category_whitelist;
    public static ConfigEntry<string> m_auto_pickup_item_blacklist;
    public static ConfigEntry<bool> m_auto_drop_crab_traps;
    public static ConfigEntry<float> m_auto_pickup_frequency;
    public static ConfigEntry<bool> m_infinite_oxygen;
    public static ConfigEntry<bool> m_invincible;
    public static ConfigEntry<bool> m_toxic_aura_enabled;
    public static ConfigEntry<bool> m_toxic_aura_sleep;
    public static ConfigEntry<float> m_aura_radius;
    public static ConfigEntry<float> m_aura_update_frequency;
    public static ConfigEntry<float> m_speed_boost;
    public static ConfigEntry<bool> m_infinite_bullets;
    public static ConfigEntry<bool> m_weightless_items;
    public static ConfigEntry<bool> m_infinite_crab_traps;
    public static ConfigEntry<bool> m_infinite_drones;
    public static ConfigEntry<bool> m_large_pickups;
    //public static ConfigEntry<string> m_harpoon_type;
    //public static ConfigEntry<string> m_harpoon_head_type;
    //public static ConfigEntry<int> m_harpoon_head_level;
    public static ConfigEntry<bool> m_disable_item_info_popup;

    // Farm
    public static ConfigEntry<float> m_farm_walk_multiplier;

    // Farm
    public static ConfigEntry<float> m_fish_farm_walk_multiplier;

    // Sushi Bar
    public static ConfigEntry<bool> m_infinite_customer_patience;
    public static ConfigEntry<float> m_sushi_speed_boost;
    public static ConfigEntry<float> m_sushi_money_boost;
    public static ConfigEntry<float> m_staff_cook_multiplier;
    public static ConfigEntry<float> m_staff_walk_multiplier;
    public static ConfigEntry<bool> m_infinite_wasabi;

    // Hotkeys
    public static ConfigEntry<string> m_hotkey_modifier;
    public static ConfigEntry<string> m_hotkey_toggle_aura_on;
    public static ConfigEntry<string> m_hotkey_change_aura_type;

    public void load(DDPlugin plugin) {
        this.m_plugin = plugin;

        // General
        m_enabled = this.m_plugin.Config.Bind<bool>("General", "Enabled", true, "Set to false to disable this mod.");

        // Boat
        m_boat_walk_speed_boost = this.m_plugin.Config.Bind<float>("Boat", "Boat - Walk Speed Boost", 0f, "Speed boost applied to Dave when walking on the boat (float, default 0f [set to 0 to disable]).");

        // Farm
        m_farm_walk_multiplier = this.m_plugin.Config.Bind<float>("Farm", "Farm - Walk Multiplier", 0f, "Multiplier applied to Dave when walking/sprinting on the farm (float, default 0f [ < 1 == faster, < 1 == slower, set to 0 to disable]) [NOTE: Setting this value too high (above ~5) can make it possible for Dave to walk fully off the screen and get stuck].");

        // Fish Farm
        m_fish_farm_walk_multiplier = this.m_plugin.Config.Bind<float>("Fish Farm", "Fish Farm - Walk Multiplier", 0f, "Multiplier applied to Dave when walking/sprinting on the fish farm (float, default 0f [ < 1 == faster, < 1 == slower, set to 0 to disable]).");

        // Diving
        m_auto_pickup_fish = this.m_plugin.Config.Bind<bool>("Diving", "Diving - Auto-pickup: Fish", false, "Set to true to enable auto pickup of sleeping/netted fish (NOTE: This will disable the Large Pickups option to prevent drone glitches; just be sure to enable Infinite Drones when using this one).");
        m_auto_pickup_frequency = this.m_plugin.Config.Bind<float>("Diving", "Diving - Auto Pickup: Update Frequency", 0.5f, "Time (in seconds) between ticks/pulses of auto-pickup (float, default 0.5f [i.e. 2 pulses per second], lower numbers may impact frame rate).");
        m_auto_pickup_items = this.m_plugin.Config.Bind<bool>("Diving", "Diving - Auto-pickup: Items", false, "Set to true to enable auto pickup of items (wood, pots, etc).");
        m_auto_drop_crab_traps = this.m_plugin.Config.Bind<bool>("Diving", "Diving - Auto-drop: Crab Traps", false, "Set to true to enable auto dropping crab traps (with max bait).");
        m_auto_pickup_radius = this.m_plugin.Config.Bind<float>("Diving", "Diving - Auto-pickup: Radius", 5.0f, "Radius (in meters?) around the character in which objects will be automatically picked up based on enabled pickup settings (float, default 5.0f).");
        m_auto_pickup_item_category_whitelist = this.m_plugin.Config.Bind<string>("Diving", "Diving - Auto-pickup: Item Category Whitelist", "Bullet,Charm,UpgradeKit,Loot,Farm", "Comma-separatated list of item categories (any combination of: Gun, Harpoon, HarpoonHead, Melee, Bullet, SubHelper, Charm, UpgradeKit, Loot, Farm) representing the categories of items to be automatically picked up.  Items within these categories that are not specified in the Auto-pickup Item Blacklist will be picked up.");
        //m_auto_pickup_item_blacklist = this.m_plugin.Config.Bind<string>("Diving", "Diving - Auto-pickup: Item Blacklist", "", "Comma-separatated list of item names");
        //m_harpoon_type = this.m_plugin.Config.Bind<string>("Diving", "Diving - Harpoon Type", "", "Harpoon type (one of: Old, Iron, Pump, Merman, NewMV, Alloy) [case sensitive, set to blank to disable].");
        //m_harpoon_head_type = this.m_plugin.Config.Bind<string>("Diving", "Diving - Harpoon Head Type", "", "Harpoon head type (one of: Normal, Electric, Poison, Chain, Sleep, Paralysis, Strong, Fire, Ice) [case sensitive, set to blank to disable].");
        //m_harpoon_head_level = this.m_plugin.Config.Bind<int>("Diving", "Diving - Harpoon Head Level", 0, "Harpoon head level [ignored if Harpoon Head Type is not set] (int, 1 (weakest) - 5 (strongest)).");			
        m_infinite_bullets = this.migrate_option<bool>("Diving", "Infinite Bullets", "Diving - Infinite Bullets", false, "Set to true to have infinite bullets when diving.");
        m_infinite_crab_traps = this.m_plugin.Config.Bind<bool>("Diving", "Diving - Infinite Crab Traps", false, "Set to true to enable infinite crab traps (NOTE: The popup window will still show 0 traps but can keep dropping them).");
        m_infinite_drones = this.m_plugin.Config.Bind<bool>("Diving", "Diving - Infinite Drones", false, "Set to true to enable infinite salvage drones.");
        m_infinite_oxygen = this.migrate_option<bool>("Diving", "Infinite Oxygen", "Diving - Infinite Oxygen", false, "Set to true to have infinite oxygen when diving (and when not diving, but that's a freebie).");
        m_invincible = this.migrate_option<bool>("Diving", "Invincible", "Diving - Invincible", false, "Set to true to take no damage when hit.");
        m_disable_item_info_popup = this.m_plugin.Config.Bind<bool>("Diving", "Diving - Disable Item Info Popups", false, "Set to true to disable the item info popup windows that tend to get WAY behind if using auto-pickup.");
        m_large_pickups = this.m_plugin.Config.Bind<bool>("Diving", "Diving - Enable Large Pickups", false, "Set to true to enable large fish to be picked up without the need for drones.");
        m_speed_boost = this.migrate_option<float>("Diving", "Speed Boost", "Diving - Speed Boost", 0f, "Permanent speed boost when diving (float, default 0f [set to 0 to disable]).");
        m_toxic_aura_enabled = this.migrate_option<bool>("Diving", "Toxic Aura: Enabled", "Diving - Toxic Aura: Enabled", false, "Set to true to enable the instant-fish-killing (or sleeping if 'Toxic Aura: Sleep Effect' is true) aura around Dave.");
        m_toxic_aura_sleep = this.m_plugin.Config.Bind<bool>("Diving", "Diving - Toxic Aura: Sleep Effect", false, "Set to true to switch from killing aura to sleep-inducing aura.");
        m_aura_radius = this.migrate_option<float>("Diving", "Toxic Aura: Radius", "Diving - Toxic Aura: Radius", 5.0f, "Radius (in meters?) around the character in which fish will be insta-killed, if Toxic Aura is enabled (float, default 5.0f).");
        m_aura_update_frequency = this.m_plugin.Config.Bind<float>("Diving", "Diving - Toxic Aura: Update Frequency", 0.5f, "Time (in seconds) between ticks/pulses of toxic aura (float, default 0.5f [i.e. 2 pulses per second], lower numbers may impact frame rate).");
        m_weightless_items = this.migrate_option<bool>("Diving", "Weightless Items (Infinite Carry Weight)", "Diving - Weightless Items (Infinite Carry Weight)", false, "Set to true to have reduce the weight of all items to 0, effectively giving infinite carry weight and inventory space.");

        // Hotkeys
        m_hotkey_modifier = this.m_plugin.Config.Bind<string>("Hotkeys", "Hotkey - Modifier", "LeftControl,RightControl", "Comma-separated list of Unity Keycodes used as the special modifier key (i.e. ctrl,alt,command) one of which is required to be down for hotkeys to work.  Set to '' (blank string) to not require a special key (not recommended).  See this link for valid Unity KeyCode strings (https://docs.unity3d.com/ScriptReference/KeyCode.html)");
        m_hotkey_toggle_aura_on = this.m_plugin.Config.Bind<string>("Hotkeys", "Hotkey - Toggle Toxic Aura On/Off", "Backspace", "Comma-separated list of Unity Keycodes, any of which will toggle the toxic aura on/off (if enabled).  See this link for valid Unity KeyCode strings (https://docs.unity3d.com/ScriptReference/KeyCode.html)");
        m_hotkey_change_aura_type = this.m_plugin.Config.Bind<string>("Hotkeys", "Hotkey - Change Toxic Aura Mode", "Backslash", "Comma-separated list of Unity Keycodes, any of which will change the toxic aura mode (if enabled) between Kill/Sleep.  See this link for valid Unity KeyCode strings (https://docs.unity3d.com/ScriptReference/KeyCode.html)");

        // Sushi Bar
        m_staff_cook_multiplier = this.m_plugin.Config.Bind<float>("Sushi", "Sushi - Cook Multiplier", 0f, "Multiplier applied to sushi bar staff cooking speed (float, default 0f [ < 1 == faster, > 1 == slower, set to 0 to disable]).");
        m_infinite_customer_patience = this.migrate_option<bool>("Sushi", "Infinite Customer Patience", "Sushi - Infinite Customer Patience", false, "Set to true to make customers never storm off if the food/drinks are too slow.");
        m_infinite_wasabi = this.m_plugin.Config.Bind<bool>("Sushi", "Sushi - Infinite Wasabi", false, "Set to true to never need to refill wasabi.");
        m_sushi_money_boost = this.m_plugin.Config.Bind<float>("Sushi", "Sushi - Money Boost", 0f, "Money boost applied to all customer purchases at sushi bar (float, default 0f [set to 0 to disable]).");
        m_sushi_speed_boost = this.migrate_option<float>("Sushi", "Sushi Speed Boost", "Sushi - Speed Boost", 0f, "Permanent speed boost when working in sushi bar (float, default 0f [set to 0 to disable]).");
        m_staff_walk_multiplier = this.m_plugin.Config.Bind<float>("Sushi", "Sushi - Walk Multiplier", 0f, "Multiplier applied to sushi bar staff walking speed [note: this also affects Dave on top of 'Sushi - Speed Boost'] (float, default 0f [ < 1 == faster, > 1 == slower, set to 0 to disable]).");

    }

    private ConfigEntry<T> migrate_option<T>(string section, string old_key, string key, T val, string description) {
        ConfigEntry<T> new_opt = this.m_plugin.Config.Bind<T>(section, key, val, description);
        ConfigEntry<T> old_opt = this.m_plugin.Config.Bind<T>(section, old_key, new_opt.Value, description);
        new_opt.Value = old_opt.Value;
        this.m_plugin.Config.Remove(old_opt.Definition);
        return new_opt;
    }
}