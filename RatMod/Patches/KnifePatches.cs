using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;
using RatMod.Weapon_Scripts.Object_Scripts;
using ULTRAKIT.Extensions;

namespace RatMod.Patches
{
    [HarmonyPatch(typeof(EnemyIdentifier))]
    public static class EnemyIdentifierPatch
    {
        [HarmonyPatch("Death"), HarmonyPrefix]
        static void DeathPrefix(EnemyIdentifier __instance)
        {
            KnifeObject knife = __instance.GetComponentInChildren<KnifeObject>();
            if (knife)
                knife.transform.parent = null;
        }
    }
}
