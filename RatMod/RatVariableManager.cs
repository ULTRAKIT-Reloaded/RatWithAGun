using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RatMod.Weapon_Scripts
{
    [ConfigureSingleton(SingletonFlags.PersistAutoInstance)]
    public class RatVariableManager : MonoSingleton<RatVariableManager>
    {
        public int GunRat_ammo = 7;
        public bool GunRat_reloading = false;

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
            GunRat_ammo = 7;
            GunRat_reloading = false;
        }
    }
}
