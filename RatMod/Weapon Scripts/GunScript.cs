using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMM;
using UnityEngine;
using ULTRAKIT.Extensions;

namespace RatMod.Weapon_Scripts
{
    public class GunScript : MonoBehaviour
    {
        readonly float ReloadTime = 2.5f;
        private bool cooldownOff;
        Transform origin;
        MeshRenderer txt_reloading;
        MeshRenderer[] txt_num;
        GameObject beam;
        
        private RatVariableManager _man = RatVariableManager.Instance;
        private InputActionState Fire1 = InputManager.Instance?.InputSource.Fire1;
        private InputActionState Fire2 = InputManager.Instance?.InputSource.Fire2;

        private void OnEnable()
        {
            InitializeFields();
            if (_man.GunRat_reloading)
                txt_reloading.enabled = true;
            else
                txt_num[_man.GunRat_ammo].enabled = true;
            try
            {
                cooldownOff = CheatsManager.Instance?.GetCheatState("ultrakill.no-weapon-cooldown") ?? false;
            }
            catch
            {
                cooldownOff = false;
            }
            CheatsManagerPatch.CheatStateChanged.AddListener(OnCheatChange);
        }

        private void OnDisable()
        {
            CheatsManagerPatch.CheatStateChanged.RemoveListener(OnCheatChange);
        }

        private void Update()
        {
            if (OptionsManager.Instance.paused)
                return;

            int ammo = _man.GunRat_ammo;
            bool reloading = _man.GunRat_reloading;
            if (Fire1.WasPerformedThisFrame && ammo > 0 && !reloading)
            {
                txt_num[ammo].enabled = false;
                if (!cooldownOff)
                    ammo--;
                txt_num[ammo].enabled = true;

                GameObject shot = Instantiate(beam);
                shot.transform.position = origin.transform.position;
                shot.transform.rotation = CameraController.Instance.transform.rotation;

                if (ammo == 0)
                {
                    reloading = true;
                    txt_num[ammo].enabled = false;
                    txt_reloading.enabled = true;
                    Invoke("Reload", ReloadTime);
                }
            }
            if (Fire2.WasPerformedThisFrame && ammo != 7 && !reloading)
            {
                reloading = true;
                txt_num[ammo].enabled = false;
                txt_reloading.enabled = true;
                Invoke("Reload", ReloadTime);
            }
            _man.GunRat_ammo = ammo;
            _man.GunRat_reloading = reloading;
        }

        private void InitializeFields()
        {
            if (origin == null)
            {
                origin = transform.Find("RAT/gun/MuzzleOrigin");
            }
            if (txt_reloading == null) 
            {
                txt_reloading = transform.Find("RAT/gun/Screen/TEXT/Reloading").gameObject.GetComponent<MeshRenderer>();
            }
            if (txt_num == null)
            {
                txt_num = transform.Find("RAT/gun/Screen/TEXT/NUM").gameObject.GetComponentsInChildren<MeshRenderer>();
            }
            if (beam == null)
            {
                beam = DazeExtensions.PrefabFind("RevolverBeamAlt");
            }
        }

        private void Reload()
        {
            _man.GunRat_ammo = 7;
            _man.GunRat_reloading = false;
            txt_reloading.enabled = false;
            txt_num[_man.GunRat_ammo].enabled = true;
        }

        private void OnCheatChange()
        {
            cooldownOff = CheatsManager.Instance.GetCheatState("ultrakill.no-weapon-cooldown");
        }
    }
}
