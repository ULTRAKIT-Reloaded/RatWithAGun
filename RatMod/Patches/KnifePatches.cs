using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;
using RatMod.Weapon_Scripts.Object_Scripts;
using ULTRAKIT.Extensions;
using RatMod.Weapon_Scripts;

namespace RatMod.Patches
{
    [HarmonyPatch(typeof(EnemyIdentifier))]
    public static class EnemyIdentifierPatch
    {
        private static readonly float HEAL_DISTANCE = 3f;
        private static readonly float HEAL_AMOUNT = 1.5f;
        [HarmonyPatch("Death"), HarmonyPrefix]
        static void DeathPrefix(EnemyIdentifier __instance)
        {
            KnifeObject knife = __instance.GetComponentInChildren<KnifeObject>();
            if (knife)
                knife.transform.parent = null;
            knife = GreechKnife.ActiveKnife?.GetComponent<KnifeObject>();
            if (knife && Vector3.Distance(__instance.transform.position, knife.transform.position) < HEAL_DISTANCE)
                knife.timeLeft += HEAL_AMOUNT;
        }
    }
}
