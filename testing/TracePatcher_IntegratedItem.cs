using HarmonyLib;
using System;

using DR;
class MapperConfiguration {}


public class TracePatcher_IntegratedItem {
    
    public class TracerParams {
        public int method_id;
        public string method_name;
        
    }

    private static Action<TracerParams> m_callback;

    public TracePatcher_IntegratedItem(Action<TracerParams> callback) {
        m_callback = callback;
    }
    // Skipping set__GKLJINHNMKB_k__BackingField
    // Skipping get__PEBNPMHFPCH_k__BackingField
    // Skipping set__PEBNPMHFPCH_k__BackingField

    [HarmonyPatch(typeof(IntegratedItem), "get_POLKOIJPPBD", new Type[] {})]
    class HarmonyPatch_get_POLKOIJPPBD {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 0,
                method_name = "get_POLKOIJPPBD"
            });
        }
    }
    // Skipping set_POLKOIJPPBD

    [HarmonyPatch(typeof(IntegratedItem), "get_KMPJHOINJJM", new Type[] {})]
    class HarmonyPatch_get_KMPJHOINJJM {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 1,
                method_name = "get_KMPJHOINJJM"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "set_KMPJHOINJJM", new Type[] {typeof(MapperConfiguration)})]
    class HarmonyPatch_set_KMPJHOINJJM {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 2,
                method_name = "set_KMPJHOINJJM"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get_BuffID", new Type[] {})]
    class HarmonyPatch_get_BuffID {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 3,
                method_name = "get_BuffID"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "__BB_OBFUSCATOR_1", new Type[] {typeof(Int32)})]
    class HarmonyPatch___BB_OBFUSCATOR_1 {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 4,
                method_name = "__BB_OBFUSCATOR_1"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get_ItemWeight", new Type[] {})]
    class HarmonyPatch_get_ItemWeight {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 5,
                method_name = "get_ItemWeight"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "BuildItem", new Type[] {typeof(Items)})]
    class HarmonyPatch_BuildItem {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 6,
                method_name = "BuildItem"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get_ID", new Type[] {})]
    class HarmonyPatch_get_ID {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 7,
                method_name = "get_ID"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get_ItemDescID", new Type[] {})]
    class HarmonyPatch_get_ItemDescID {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 8,
                method_name = "get_ItemDescID"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "set_ItemSpecID", new Type[] {typeof(String)})]
    class HarmonyPatch_set_ItemSpecID {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 9,
                method_name = "set_ItemSpecID"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "set_ItemLevel", new Type[] {typeof(Int32)})]
    class HarmonyPatch_set_ItemLevel {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 10,
                method_name = "set_ItemLevel"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get_SpawnObjectName", new Type[] {})]
    class HarmonyPatch_get_SpawnObjectName {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 11,
                method_name = "get_SpawnObjectName"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "set_ItemTextID", new Type[] {typeof(String)})]
    class HarmonyPatch_set_ItemTextID {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 12,
                method_name = "set_ItemTextID"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get_ItemRank", new Type[] {})]
    class HarmonyPatch_get_ItemRank {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 13,
                method_name = "get_ItemRank"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "set_ItemDataID", new Type[] {typeof(Int32)})]
    class HarmonyPatch_set_ItemDataID {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 14,
                method_name = "set_ItemDataID"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get_ItemSellPrice", new Type[] {})]
    class HarmonyPatch_get_ItemSellPrice {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 15,
                method_name = "get_ItemSellPrice"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "set_TID", new Type[] {typeof(Int32)})]
    class HarmonyPatch_set_TID {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 16,
                method_name = "set_TID"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "__BB_OBFUSCATOR_2", new Type[] {})]
    class HarmonyPatch___BB_OBFUSCATOR_2 {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 17,
                method_name = "__BB_OBFUSCATOR_2"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get_ItemIcon", new Type[] {})]
    class HarmonyPatch_get_ItemIcon {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 18,
                method_name = "get_ItemIcon"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "BuildEquip", new Type[] {typeof(EquipmentItem)})]
    class HarmonyPatch_BuildEquip {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 19,
                method_name = "BuildEquip"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "set_ItemBuyPrice", new Type[] {typeof(Int32)})]
    class HarmonyPatch_set_ItemBuyPrice {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 20,
                method_name = "set_ItemBuyPrice"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "BuildInternal", new Type[] {typeof(IItemBase)})]
    class HarmonyPatch_BuildInternal {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 21,
                method_name = "BuildInternal"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get_TID", new Type[] {})]
    class HarmonyPatch_get_TID {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 22,
                method_name = "get_TID"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get_ItemGrade", new Type[] {})]
    class HarmonyPatch_get_ItemGrade {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 23,
                method_name = "get_ItemGrade"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get_SpawnObject", new Type[] {})]
    class HarmonyPatch_get_SpawnObject {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 24,
                method_name = "get_SpawnObject"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get_ItemMaxStackCount", new Type[] {})]
    class HarmonyPatch_get_ItemMaxStackCount {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 25,
                method_name = "get_ItemMaxStackCount"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "set_BuffID", new Type[] {typeof(Int32)})]
    class HarmonyPatch_set_BuffID {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 26,
                method_name = "set_BuffID"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get_ItemUIIcon", new Type[] {})]
    class HarmonyPatch_get_ItemUIIcon {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 27,
                method_name = "get_ItemUIIcon"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get_ItemBuyPrice", new Type[] {})]
    class HarmonyPatch_get_ItemBuyPrice {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 28,
                method_name = "get_ItemBuyPrice"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "set_ItemRank", new Type[] {typeof(Int32)})]
    class HarmonyPatch_set_ItemRank {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 29,
                method_name = "set_ItemRank"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get_specData", new Type[] {})]
    class HarmonyPatch_get_specData {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 30,
                method_name = "get_specData"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "set_ItemMaxStackCount", new Type[] {typeof(Int32)})]
    class HarmonyPatch_set_ItemMaxStackCount {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 31,
                method_name = "set_ItemMaxStackCount"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "set_ItemSellPrice", new Type[] {typeof(Int32)})]
    class HarmonyPatch_set_ItemSellPrice {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 32,
                method_name = "set_ItemSellPrice"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get_ItemLevel", new Type[] {})]
    class HarmonyPatch_get_ItemLevel {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 33,
                method_name = "get_ItemLevel"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "set_ItemWeight", new Type[] {typeof(Single)})]
    class HarmonyPatch_set_ItemWeight {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 34,
                method_name = "set_ItemWeight"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get_ItemSpecID", new Type[] {})]
    class HarmonyPatch_get_ItemSpecID {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 35,
                method_name = "get_ItemSpecID"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "set_ItemGrade", new Type[] {typeof(Int32)})]
    class HarmonyPatch_set_ItemGrade {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 36,
                method_name = "set_ItemGrade"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "CheckSameTypeItem", new Type[] {typeof(IntegratedItem)})]
    class HarmonyPatch_CheckSameTypeItem {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 37,
                method_name = "CheckSameTypeItem"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get_ItemDataID", new Type[] {})]
    class HarmonyPatch_get_ItemDataID {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 38,
                method_name = "get_ItemDataID"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "set_specData", new Type[] {typeof(SpecDataBase)})]
    class HarmonyPatch_set_specData {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 39,
                method_name = "set_specData"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "set_SpawnObject", new Type[] {typeof(String)})]
    class HarmonyPatch_set_SpawnObject {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 40,
                method_name = "set_SpawnObject"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "set_InvenSortOrder", new Type[] {typeof(Int32)})]
    class HarmonyPatch_set_InvenSortOrder {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 41,
                method_name = "set_InvenSortOrder"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "set_ItemDescID", new Type[] {typeof(String)})]
    class HarmonyPatch_set_ItemDescID {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 42,
                method_name = "set_ItemDescID"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "set_ItemIcon", new Type[] {typeof(String)})]
    class HarmonyPatch_set_ItemIcon {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 43,
                method_name = "set_ItemIcon"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "set_ItemUIIcon", new Type[] {typeof(String)})]
    class HarmonyPatch_set_ItemUIIcon {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 44,
                method_name = "set_ItemUIIcon"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get_ItemTextID", new Type[] {})]
    class HarmonyPatch_get_ItemTextID {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 45,
                method_name = "get_ItemTextID"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "__BB_OBFUSCATOR_0", new Type[] {})]
    class HarmonyPatch___BB_OBFUSCATOR_0 {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 46,
                method_name = "__BB_OBFUSCATOR_0"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "set_IntegratedType", new Type[] {typeof(Int32)})]
    class HarmonyPatch_set_IntegratedType {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 47,
                method_name = "set_IntegratedType"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get_IntegratedType", new Type[] {})]
    class HarmonyPatch_get_IntegratedType {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 48,
                method_name = "get_IntegratedType"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "IsEquipmentType", new Type[] {})]
    class HarmonyPatch_IsEquipmentType {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 49,
                method_name = "IsEquipmentType"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get_InvenSortOrder", new Type[] {})]
    class HarmonyPatch_get_InvenSortOrder {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 50,
                method_name = "get_InvenSortOrder"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get__MHHCFDDBLMP_k__BackingField", new Type[] {})]
    class HarmonyPatch_get__MHHCFDDBLMP_k__BackingField {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 51,
                method_name = "get__MHHCFDDBLMP_k__BackingField"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "set__MHHCFDDBLMP_k__BackingField", new Type[] {typeof(Int32)})]
    class HarmonyPatch_set__MHHCFDDBLMP_k__BackingField {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 52,
                method_name = "set__MHHCFDDBLMP_k__BackingField"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get__DBPDENGIILC_k__BackingField", new Type[] {})]
    class HarmonyPatch_get__DBPDENGIILC_k__BackingField {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 53,
                method_name = "get__DBPDENGIILC_k__BackingField"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "set__DBPDENGIILC_k__BackingField", new Type[] {typeof(Int32)})]
    class HarmonyPatch_set__DBPDENGIILC_k__BackingField {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 54,
                method_name = "set__DBPDENGIILC_k__BackingField"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get__LNAMOKPFOCP_k__BackingField", new Type[] {})]
    class HarmonyPatch_get__LNAMOKPFOCP_k__BackingField {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 55,
                method_name = "get__LNAMOKPFOCP_k__BackingField"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "set__LNAMOKPFOCP_k__BackingField", new Type[] {typeof(String)})]
    class HarmonyPatch_set__LNAMOKPFOCP_k__BackingField {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 56,
                method_name = "set__LNAMOKPFOCP_k__BackingField"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get__BKJMPGHDAJI_k__BackingField", new Type[] {})]
    class HarmonyPatch_get__BKJMPGHDAJI_k__BackingField {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 57,
                method_name = "get__BKJMPGHDAJI_k__BackingField"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "set__BKJMPGHDAJI_k__BackingField", new Type[] {typeof(String)})]
    class HarmonyPatch_set__BKJMPGHDAJI_k__BackingField {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 58,
                method_name = "set__BKJMPGHDAJI_k__BackingField"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get__OLLJPOOIKJG_k__BackingField", new Type[] {})]
    class HarmonyPatch_get__OLLJPOOIKJG_k__BackingField {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 59,
                method_name = "get__OLLJPOOIKJG_k__BackingField"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "set__OLLJPOOIKJG_k__BackingField", new Type[] {typeof(String)})]
    class HarmonyPatch_set__OLLJPOOIKJG_k__BackingField {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 60,
                method_name = "set__OLLJPOOIKJG_k__BackingField"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get__GJNCKGPIDMF_k__BackingField", new Type[] {})]
    class HarmonyPatch_get__GJNCKGPIDMF_k__BackingField {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 61,
                method_name = "get__GJNCKGPIDMF_k__BackingField"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "set__GJNCKGPIDMF_k__BackingField", new Type[] {typeof(String)})]
    class HarmonyPatch_set__GJNCKGPIDMF_k__BackingField {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 62,
                method_name = "set__GJNCKGPIDMF_k__BackingField"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get__IONMNKJEAND_k__BackingField", new Type[] {})]
    class HarmonyPatch_get__IONMNKJEAND_k__BackingField {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 63,
                method_name = "get__IONMNKJEAND_k__BackingField"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "set__IONMNKJEAND_k__BackingField", new Type[] {typeof(String)})]
    class HarmonyPatch_set__IONMNKJEAND_k__BackingField {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 64,
                method_name = "set__IONMNKJEAND_k__BackingField"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get__NCBPICMIKBD_k__BackingField", new Type[] {})]
    class HarmonyPatch_get__NCBPICMIKBD_k__BackingField {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 65,
                method_name = "get__NCBPICMIKBD_k__BackingField"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "set__NCBPICMIKBD_k__BackingField", new Type[] {typeof(Int32)})]
    class HarmonyPatch_set__NCBPICMIKBD_k__BackingField {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 66,
                method_name = "set__NCBPICMIKBD_k__BackingField"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get__KEJLOPCPILO_k__BackingField", new Type[] {})]
    class HarmonyPatch_get__KEJLOPCPILO_k__BackingField {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 67,
                method_name = "get__KEJLOPCPILO_k__BackingField"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "set__KEJLOPCPILO_k__BackingField", new Type[] {typeof(Int32)})]
    class HarmonyPatch_set__KEJLOPCPILO_k__BackingField {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 68,
                method_name = "set__KEJLOPCPILO_k__BackingField"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get__NOLPEFFNFKK_k__BackingField", new Type[] {})]
    class HarmonyPatch_get__NOLPEFFNFKK_k__BackingField {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 69,
                method_name = "get__NOLPEFFNFKK_k__BackingField"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "set__NOLPEFFNFKK_k__BackingField", new Type[] {typeof(Single)})]
    class HarmonyPatch_set__NOLPEFFNFKK_k__BackingField {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 70,
                method_name = "set__NOLPEFFNFKK_k__BackingField"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get__FJKFPMMEBMO_k__BackingField", new Type[] {})]
    class HarmonyPatch_get__FJKFPMMEBMO_k__BackingField {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 71,
                method_name = "get__FJKFPMMEBMO_k__BackingField"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "set__FJKFPMMEBMO_k__BackingField", new Type[] {typeof(Int32)})]
    class HarmonyPatch_set__FJKFPMMEBMO_k__BackingField {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 72,
                method_name = "set__FJKFPMMEBMO_k__BackingField"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get__OHOKECJBANP_k__BackingField", new Type[] {})]
    class HarmonyPatch_get__OHOKECJBANP_k__BackingField {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 73,
                method_name = "get__OHOKECJBANP_k__BackingField"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "set__OHOKECJBANP_k__BackingField", new Type[] {typeof(Int32)})]
    class HarmonyPatch_set__OHOKECJBANP_k__BackingField {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 74,
                method_name = "set__OHOKECJBANP_k__BackingField"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get__DCIIHEBGECM_k__BackingField", new Type[] {})]
    class HarmonyPatch_get__DCIIHEBGECM_k__BackingField {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 75,
                method_name = "get__DCIIHEBGECM_k__BackingField"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "set__DCIIHEBGECM_k__BackingField", new Type[] {typeof(Int32)})]
    class HarmonyPatch_set__DCIIHEBGECM_k__BackingField {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 76,
                method_name = "set__DCIIHEBGECM_k__BackingField"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get__BBNAEFGJKFL_k__BackingField", new Type[] {})]
    class HarmonyPatch_get__BBNAEFGJKFL_k__BackingField {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 77,
                method_name = "get__BBNAEFGJKFL_k__BackingField"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "set__BBNAEFGJKFL_k__BackingField", new Type[] {typeof(String)})]
    class HarmonyPatch_set__BBNAEFGJKFL_k__BackingField {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 78,
                method_name = "set__BBNAEFGJKFL_k__BackingField"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get__IKMAFIBJJLO_k__BackingField", new Type[] {})]
    class HarmonyPatch_get__IKMAFIBJJLO_k__BackingField {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 79,
                method_name = "get__IKMAFIBJJLO_k__BackingField"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "set__IKMAFIBJJLO_k__BackingField", new Type[] {typeof(Int32)})]
    class HarmonyPatch_set__IKMAFIBJJLO_k__BackingField {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 80,
                method_name = "set__IKMAFIBJJLO_k__BackingField"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get__LADAGNHFPDL_k__BackingField", new Type[] {})]
    class HarmonyPatch_get__LADAGNHFPDL_k__BackingField {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 81,
                method_name = "get__LADAGNHFPDL_k__BackingField"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "set__LADAGNHFPDL_k__BackingField", new Type[] {typeof(Int32)})]
    class HarmonyPatch_set__LADAGNHFPDL_k__BackingField {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 82,
                method_name = "set__LADAGNHFPDL_k__BackingField"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get__MGAOFFMKLEH_k__BackingField", new Type[] {})]
    class HarmonyPatch_get__MGAOFFMKLEH_k__BackingField {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 83,
                method_name = "get__MGAOFFMKLEH_k__BackingField"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "set__MGAOFFMKLEH_k__BackingField", new Type[] {typeof(Int32)})]
    class HarmonyPatch_set__MGAOFFMKLEH_k__BackingField {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 84,
                method_name = "set__MGAOFFMKLEH_k__BackingField"
            });
        }
    }

    [HarmonyPatch(typeof(IntegratedItem), "get__GKLJINHNMKB_k__BackingField", new Type[] {})]
    class HarmonyPatch_get__GKLJINHNMKB_k__BackingField {
        private static void Postfix() {
            m_callback(new TracerParams() {
                method_id = 85,
                method_name = "get__GKLJINHNMKB_k__BackingField"
            });
        }
    }
}