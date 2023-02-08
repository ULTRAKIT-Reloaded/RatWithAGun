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

namespace RatMod
{
    [UKDependency("petersone1.ultrakitreloaded", "1.3.2")]
<<<<<<< HEAD
    [UKPlugin("petersone1.ratwithagun", "Rat With A Gun", "0.2.2", "Adds a collection of rats with a lust for blood", false, true)]
    public class Main : UKMod
    {
        private AssetBundle bundle = AssetBundle.LoadFromMemory(Properties.Resources.petersone1_ratwithagun);
        private Weapon[] weapons;
=======
    [UKPlugin("petersone1.ratwithagun", "Rat With A Gun", "0.2.1", "Adds a collection of rats with a lust for blood", false, true)]
    public class Main : UKMod
    {
        private AssetBundle bundle = AssetBundle.LoadFromMemory(Properties.Resources.petersone1_ratwithagun);
        public Weapon[] weapons;
>>>>>>> 8a42be14651c428c53e8cf2082cc0564d66756bd

        public override void OnModLoaded()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            weapons = ULTRAKIT.Loader.WeaponLoader.LoadWeapons(bundle);
            ULTRAKIT.Loader.BuffLoader.RegisterBuff(Bigger.buff);
        }

        public override void OnModUnload()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            ULTRAKIT.Loader.WeaponLoader.UnloadWeapons("petersone1_ratwithagun");
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            CheatsManager.Instance.RegisterCheat(Cheats.TurretOrb.OrbEnabler.cheat);
            if (RatVariableManager.Instance)
            {
                RatVariableManager.Instance.assetBundle = bundle;
            }
        }
    }
}
