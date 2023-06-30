using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DestroyWeaponMod
{
    [BepInPlugin("while.DestroyWeaponMod", "摧毁武器", "1.0.0")]
    public class ModLoad : BaseUnityPlugin
    {
        public static ConfigEntry<bool> MaskCountIntConfig;
        void Start()
        {
            MaskCountIntConfig = Config.Bind<bool>("config", "MastCountIntConfig", false, "是否返还古代硬币");
            new Harmony("while.DestroyWeaponMod").PatchAll();
        }
    }
}
