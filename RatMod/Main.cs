using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMM;
using ULTRAKIT.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using HarmonyLib;
using ULTRAKIT.Loader.Loaders;
using ULTRAKIT.Extensions;
using RatMod.Weapon_Scripts;
using BepInEx;

namespace RatMod
{
    [BepInPlugin("agent.rat_with_a_gun", "Rat With A Gun", "0.5.0")]
    [BepInDependency("ULTRAKIT.core_module")]
    public class Main : BaseUnityPlugin
    {
        private AssetBundle bundle = AssetBundle.LoadFromMemory(Properties.Resources.petersone1_ratwithagun);
        public Weapon[] weapons;

        private void Awake()
        {
            Init();
            OptionsLoader.RegisterCheckbox("Rat With A Gun", "Balance Levels", "Unbalanced Mode", "rat.unbalance", false);
            SceneManager.sceneLoaded += OnSceneLoaded;
            weapons = WeaponLoader.LoadWeapons(bundle);
            BuffLoader.RegisterBuff(new Bigger());
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (CheatsManager.Instance)
                CheatsManager.Instance.RegisterCheat(Cheats.TurretOrb.OrbEnabler.cheat);
            if (RatVariableManager.Instance)
                RatVariableManager.Instance.assetBundle = bundle;
            GreechKnife.ActiveKnife = null;
        }

        private static void Init()
        {
            Harmony harmony = new Harmony("RatWithAGun");
            harmony.PatchAll();
        }
    }
}
