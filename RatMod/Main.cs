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

namespace RatMod
{
<<<<<<< HEAD
    //[UKDependency("petersone1.ultrakitreloaded", "1.3.2")]
    [UKPlugin("petersone1.ratwithagun", "Rat With A Gun", "0.2.2", "Adds a collection of rats with a lust for blood", false, true)]
=======
    [UKDependency("petersone1.ultrakitreloaded", "1.3.2")]
    [UKPlugin("petersone1.ratwithagun", "Rat With A Gun", "0.3.0", "Adds a collection of rats with a lust for blood", false, true)]
>>>>>>> 5fb73a1f2e081f1ce4d052f7ccf1ba320b63cebe
    public class Main : UKMod
    {
        private AssetBundle bundle = AssetBundle.LoadFromMemory(Properties.Resources.petersone1_ratwithagun);
        public Weapon[] weapons;

        public override void OnModLoaded()
        {
            Init();
            SceneManager.sceneLoaded += OnSceneLoaded;
            weapons = WeaponLoader.LoadWeapons(bundle);
            BuffLoader.RegisterBuff(new Bigger());
        }

        public override void OnModUnload()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            WeaponLoader.UnloadWeapons("petersone1_ratwithagun");
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            CheatsManager.Instance.RegisterCheat(Cheats.TurretOrb.OrbEnabler.cheat);
            if (RatVariableManager.Instance)
            {
                RatVariableManager.Instance.assetBundle = bundle;
            }
        }

        private static void Init()
        {
            Harmony harmony = new Harmony("RatWithAGun");
            harmony.PatchAll();
        }
    }
}
