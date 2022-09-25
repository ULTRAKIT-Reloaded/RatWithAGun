using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMM;
using ULTRAKIT.Data;
using UnityEngine;

namespace RatMod
{
    [UKPlugin("Rat With A Gun", "1.0.0", "Adds a collection of rats with a lust for blood", false, true)]
    public class Main : UKMod
    {
        private AssetBundle bundle = AssetBundle.LoadFromFile(Directory.GetCurrentDirectory() + @"\BepInEx\UMM Mods\RatWithAGun\petersone1_ratwithagun.assetBundle");
        private Weapon[] weapons;

        public override void OnModLoaded()
        {
            weapons = ULTRAKIT.Loader.WeaponLoader.LoadWeapons(bundle);
        }

        public override void OnModUnload()
        {
            ULTRAKIT.Loader.WeaponLoader.UnloadWeapons("petersone1_ratwithagun");
        }
    }
}
