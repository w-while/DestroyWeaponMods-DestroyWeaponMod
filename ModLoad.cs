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
        public static ConfigEntry<float> AncitentCoinReturnRate;
        void Start()
        {
            AncitentCoinReturnRate = Config.Bind<float>("config", "AncitentCoinReturnRate", 0.5f, "古代硬币返还比例");
            new Harmony("while.DestroyWeaponMod").PatchAll();
        }
    }
}
