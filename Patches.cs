using HarmonyLib;
using hex;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UI;

namespace DestroyWeaponMod
{
    public class Patches
    {
        [HarmonyPatch(typeof(EquipUnLockIcon), nameof(EquipUnLockIcon.UnLock))]
        public static class EquipUnLockIcon_UnLockPatch
        {
            public static void addLockedEquipment(EquipUnLockIcon __instance, int id)
            {
                typeof(GameItemTools).GetMethod("InitLockItems", BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Static).Invoke(null, null);
                TLockedEquipData data = Singleton<TempDataManager>.Instance.lockedEquipDataMgr.getData(id);
                if (data == null)
                {
                    return;
                }
                HashSet<int> unlockEquips = (HashSet<int>)typeof(GameItemTools).GetField("unlockEquips",
                    BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Static).GetValue(null);
                Dictionary<int, int> lockItems = (Dictionary<int, int>)typeof(GameItemTools).GetField("lockItems",
                    BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Static).GetValue(null);
                if (unlockEquips.Contains(data.equipId))
                {
                    unlockEquips.Remove(data.equipId);
                }
                if (!lockItems.ContainsKey(id))
                {
                    lockItems[id] = data.rate;
                }
                if (ModLoad.MaskCountIntConfig.Value)
                {
                    MonoSingleton<SceneManager>.Instance.player.AddAncientCoin(__instance.data.ancientGold);
                }
            }
            public static bool Prefix(bool free, EquipUnLockIcon __instance, ref bool __result, ref bool ___unLock, Image ___icon)
            {
                if (__instance.data != null && ___unLock)
                {
                    GlobalDBInfo.Instance.unLockEquips.Remove(__instance.data.id);
                    addLockedEquipment(__instance, __instance.data.id);
                    ___unLock = false;
                    __instance.SetData(__instance.data);
                    __result=false;
                    return false;
                }
                if (__instance.data == null || ___unLock)
                {
                    __result=false;
                    return false;
                }
                hex.Player player = MonoSingleton<SceneManager>.Instance.player;
                int ancientGold = __instance.data.ancientGold;
                if (!free && player.pAttr.ancientCoin < ancientGold)
                {
                    MonoSingleton<BattleUILayer>.Instance.SetWarningUI(1047, WarningAudio.LACK);
                    if (__instance.unLockCallFailback != null)
                    {
                        __instance.unLockCallFailback(__instance.data);
                    }
                    __result=false;
                    return false;
                }
                GameItemTools.UnLockEquip(__instance.data.id);
                if (!free)
                {
                    player.AddAncientCoin(-ancientGold);
                }
                ___icon.material = null;
                ___unLock = true;
                __instance.SetData(__instance.data);
                if (__instance.unLockCallback != null)
                {
                    __instance.unLockCallback(__instance.data);
                    __instance.unLockCallback = null;
                }
                typeof(EquipUnLockIcon).GetMethod("PlaySpine", BindingFlags.Instance|BindingFlags.NonPublic).Invoke(__instance, new object[] { "idle" });
                player.SaveDB();
                MonoSingleton<AchievementManager>.Instance.UpdateTypeAchievement(3, 1);
                __result=true;
                return false;
            }
        }
    }
}
