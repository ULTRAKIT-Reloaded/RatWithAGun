using HarmonyLib;
using RatMod.Weapon_Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULTRAKIT.Extensions;
using UnityEngine;

namespace RatMod.Patches
{
    [HarmonyPatch(typeof(NewMovement))]
    public static class NewMovementPatch
    {
        static readonly float HealthPerAmmo = 10f;
        static float storedHealth = 0f;

        [HarmonyPatch("GetHealth"), HarmonyPostfix]
        static void GetHealthPostfix(NewMovement __instance, int health)
        {
            int ammoToAdd = 0;
            storedHealth += health;
            while (storedHealth >= HealthPerAmmo)
            {
                storedHealth -= HealthPerAmmo;
                ammoToAdd++;
            }
            if (RatVariableManager.Instance)
            {
                RatVariableManager.Instance.GunRat_ammo = Mathf.Clamp(RatVariableManager.Instance.GunRat_ammo + ammoToAdd, 0, 7);
                if (GunScript.isActive)
                    GunScript.Refresh();
            }
        }
    }
}
