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
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

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
            KnifeObject knife = GetKnife(__instance.transform);
            if (knife)
                knife.transform.parent = null;
            knife = GreechKnife.ActiveKnife?.GetComponent<KnifeObject>() ?? null;
            if (knife && Vector3.Distance(__instance.transform.position, knife.transform.position) < HEAL_DISTANCE)
                knife.timeLeft += HEAL_AMOUNT;
        }

        private static KnifeObject GetKnife(Transform instance)
        {
            KnifeObject knife;
            foreach (Transform child in instance.ListChildren())
            {
                if (child.TryGetComponent<KnifeObject>(out knife))
                    return knife;
            }
            return null;
        }
    }
}
