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
    [UKPlugin("Rat With A Gun", "0.1.0", "Adds a collection of rats with a lust for blood", false, true)]
    public class Main : UKMod
    {
        private AssetBundle bundle = AssetBundle.LoadFromFile(Directory.GetCurrentDirectory() + @"\BepInEx\UMM Mods\RatWithAGun\petersone1_ratwithagun.assetBundle");
        private Weapon[] weapons;

        public override void OnModLoaded()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            weapons = ULTRAKIT.Loader.WeaponLoader.LoadWeapons(bundle);
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
