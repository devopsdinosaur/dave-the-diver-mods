using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

public class Singletons {
    public static DDPlugin Plugin { get; set; }
    public static ManualLogSource Log { get; set; }
    private static PlayerCharacter m_player = null;
    public static PlayerCharacter Player {
        get {
            return get_instance<PlayerCharacter>(ref m_player);
        }
        private set {
            m_player = value;
        }
    }
    private static CharacterController2D m_character = null;
    public static CharacterController2D Character {
        get {
            return get_instance<CharacterController2D>(ref m_character);
        }
        private set {
            m_character = value;
        }
    }

    private static T get_instance<T>(ref T instance) where T : MonoBehaviour {
        try {
            if (instance != null) {
                _ = instance.enabled;
                return instance;
            }
        } catch { }
        return instance = null;
    }

    [HarmonyPatch(typeof(PlayerCharacter), "Awake")]
    class HarmonyPatch_PlayerCharacter_Awake {
        private static void Postfix(PlayerCharacter __instance) {
            Player = __instance;
        }
    }

    [HarmonyPatch(typeof(CharacterController2D), "Awake")]
    class HarmonyPatch_CharacterController2D_Awake {
        private static void Postfix(CharacterController2D __instance) {
            Character = __instance;
        }
    }
}
