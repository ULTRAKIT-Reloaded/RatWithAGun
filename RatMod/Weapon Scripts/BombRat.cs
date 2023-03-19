using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMM;
using UnityEngine;
using ULTRAKIT.Extensions;
using ULTRAKIT.Data;
using System.Collections;
using RatMod.Weapon_Scripts.Object_Scripts;

namespace RatMod.Weapon_Scripts
{
    public class BombRat : MonoBehaviour
    {
        private RatVariableManager _man = RatVariableManager.Instance;
        private InputActionState Fire1 = InputManager.Instance?.InputSource.Fire1;
        private InputActionState Fire2 = InputManager.Instance?.InputSource.Fire2;

        private Transform origin;
        private GameObject projectile;

        private bool cooldownOff;
        private bool readyToFire;

        private void OnEnable()
        {
            InitializeFields();
            try
            {
                cooldownOff = CheatsManager.Instance?.GetCheatState("ultrakill.no-weapon-cooldown") ?? false;
            }
            catch
            {
                cooldownOff = false;
            }
            Events.CheatStateChanged.AddListener(OnCheatChange);
        }

        private void OnDisable()
        {
            Events.CheatStateChanged.RemoveListener(OnCheatChange);
        }

        private void Update()
        {
            if (OptionsManager.Instance.paused)
                return;
            readyToFire = _man.BombRat_ready;
            if (Fire1.WasPerformedThisFrame && readyToFire)
            {
                if (!cooldownOff)
                {
                    readyToFire = false;
                    Invoke("Ready", 1.5f);
                }
                GameObject proj = Instantiate(projectile, origin.position, CameraController.Instance.transform.rotation, null);
                proj.GetComponent<ProjectileScript>().sourceWeapon = gameObject;
                proj.AddComponent<FloatingPointErrorPreventer>();
                proj.AddComponent<DestroyOnCheckpointRestart>();
                proj.AddComponent<RemoveOnTime>().time = 30;
            }

            _man.BombRat_ready = readyToFire;
        }

        private void Ready()
        {
            _man.BombRat_ready = true;
        }

        private void InitializeFields()
        {
            if (origin == null)
                origin = transform.Find("RAT/gun/MuzzleOrigin");
            if (projectile == null)
                projectile = _man.assetBundle.LoadAsset<GameObject>("explosiveprojectile.prefab");
        }

        private void OnCheatChange(string cheat)
        {
            cooldownOff = CheatsManager.Instance.GetCheatState("ultrakill.no-weapon-cooldown");
        }
    }
}
