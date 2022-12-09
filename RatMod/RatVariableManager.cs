using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using ULTRAKIT.Extensions;
using System.Collections;

namespace RatMod
{
    [ConfigureSingleton(SingletonFlags.PersistAutoInstance)]
    public class RatVariableManager : MonoSingleton<RatVariableManager>
    {
        // AssetBundle
        public AssetBundle assetBundle;

        // Prefabs
        public GameObject Asset_ExplosionPrime;
        public GameObject Asset_ExplosionSuper;
        public GameObject Asset_PhysicalShockwaveHarmless;
        public GameObject Asset_MindflayerExplosion;

        // Gun Rat
        public int GunRat_ammo = 7;
        public bool GunRat_reloading = false;

        // Builder Rat
        public bool BuilderRat_turretReady = true;
        public bool BuilderRat_statueReady = true;
        public int BuilderRat_numToBe = 15;
        public GameObject BuilderRat_lastStatue;


        public new void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            Asset_ExplosionPrime = DazeExtensions.PrefabFind("ExplosionPrime");
            Asset_ExplosionSuper = DazeExtensions.PrefabFind("ExplosionSuper");
            Asset_PhysicalShockwaveHarmless = DazeExtensions.PrefabFind("PhysicalShockwaveHarmless");
            Asset_MindflayerExplosion = DazeExtensions.PrefabFind("MindflayerExplosion");

            GunRat_ammo = 7;
            GunRat_reloading = false;
            BuilderRat_turretReady = true;
            BuilderRat_statueReady = true;
        }

        // Bomb Rat
        public bool BombRat_ready = true;

        // Coroutines

        public IEnumerator ReloadStatue(Weapon_Scripts.BuilderRat builderRat)
        {
            for (int i = Mathf.CeilToInt(builderRat.statueTimer); i > 0; i--)
            {
                BuilderRat_numToBe = i;
                if (builderRat)
                {
                    foreach (MeshRenderer renderer in builderRat.txt_num)
                        renderer.enabled = false;
                    builderRat.txt_num[i].enabled = true;
                }

                yield return new WaitForSeconds(1);
            }
            BuilderRat_statueReady = true;
            BuilderRat_numToBe = 15;
            if (builderRat)
            {
                foreach (MeshRenderer renderer in builderRat.txt_num)
                    renderer.enabled = false;
                builderRat.txt_ready.enabled = true;
            }
            yield return null;
        }
    }
}
