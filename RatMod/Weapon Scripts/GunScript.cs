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
        public static Action Refresh;

        private bool delay => _man.GunRat_delay;
        private int ammo { get { return _man.GunRat_ammo; } set { _man.GunRat_ammo = value; } }

        private bool cooldownOff;
        Transform origin;
        MeshRenderer[] txt_num;
        GameObject beam;
        
        private RatVariableManager _man = RatVariableManager.Instance;
        private InputActionState Fire1 = InputManager.Instance?.InputSource.Fire1;
        private InputActionState Fire2 = InputManager.Instance?.InputSource.Fire2;

        private void RefreshCounter()
        {
            foreach (Renderer renderer in txt_num)
                renderer.enabled = false;
            txt_num[_man.GunRat_ammo].enabled = true;
        }

        private void OnEnable()
        {
            InitializeFields();
            txt_num[_man.GunRat_ammo].enabled = true;
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

            if (Fire1.WasPerformedThisFrame && ammo > 0 && (cooldownOff || !delay))
            {
                txt_num[ammo].enabled = false;
                if (!cooldownOff)
                    ammo--;
                txt_num[ammo].enabled = true;

                GameObject shot = Instantiate(beam);
                shot.GetComponent<RevolverBeam>().sourceWeapon = gameObject;
                shot.transform.position = origin.transform.position;
                shot.transform.rotation = CameraController.Instance.transform.rotation;
            }
        }

        private void InitializeFields()
        {
            Refresh = RefreshCounter;

            if (origin == null)
            {
                origin = transform.Find("RAT/gun/MuzzleOrigin");
            }
            if (txt_num == null)
            {
                txt_num = transform.Find("RAT/gun/Screen/TEXT/NUM").gameObject.GetComponentsInChildren<MeshRenderer>();
            }
            if (beam == null)
            {
                beam = AssetLoader.AssetFind<GameObject>("RevolverBeamAlt.prefab");
            }
        }

        private void OnCheatChange(string cheat)
        {
            cooldownOff = CheatsManager.Instance.GetCheatState("ultrakill.no-weapon-cooldown");
        }
    }
}
