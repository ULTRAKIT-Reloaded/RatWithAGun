﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMM;
using UnityEngine;
using ULTRAKIT.Extensions;
using ULTRAKIT.Data;
using System.Collections;

namespace RatMod.Weapon_Scripts
{
    public class BuilderRat : MonoBehaviour
    {
        public readonly float turretTimer = 10f;
        public readonly float statueTimer = 15f;

        private GameObject _turret;
        private GameObject _statue;
        private Transform origin;
        private MeshRenderer sight;
        private MeshRenderer altSight;
        public MeshRenderer txt_ready;
        public MeshRenderer[] txt_num;

        private bool cooldownOff;
        private RatVariableManager _man = RatVariableManager.Instance;
        private InputActionState Fire1 = InputManager.Instance?.InputSource.Fire1;
        private InputActionState Fire2 = InputManager.Instance?.InputSource.Fire2;
        private Transform head = CameraController.Instance?.transform;

        LayerMask mask = LayerMask.GetMask("Environment", "Outdoors");

        private void OnEnable()
        {
            InitializeFields();
            if (_man.BuilderRat_turretReady)
            {
                sight.enabled = true;
                altSight.enabled = false;
            }
            else
            {
                altSight.enabled = true;
                sight.enabled = false;
            }
            txt_ready.enabled = false;
            foreach (MeshRenderer renderer in txt_num)
                renderer.enabled = false;

            if (_man.BuilderRat_statueReady)
                txt_ready.enabled = true;
            else
                txt_num[_man.BuilderRat_numToBe].enabled = true;

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

            bool turretReady = _man.BuilderRat_turretReady;
            bool statueReady = _man.BuilderRat_statueReady;

            if (Fire1.WasPerformedThisFrame && turretReady)
            {
                RaycastHit hit;
                if (Physics.Raycast(head.position + head.forward, head.forward, out hit, Mathf.Infinity, mask))
                {
                    if (!cooldownOff)
                    {
                        turretReady = false;
                        sight.enabled = false;
                        altSight.enabled = true;
                        Invoke("ReloadTurret", turretTimer);
                    }
                    GameObject turret = Instantiate(_turret, hit.point, Quaternion.identity, null);
                    turret.transform.LookAt(head);
                    turret.transform.rotation = Quaternion.FromToRotation(turret.transform.up, hit.normal) * transform.rotation;
                    turret.AddComponent<DestroyOnCheckpointRestart>();
                    foreach (var c in turret.GetComponentsInChildren<Renderer>(true))
                    {
                        c.gameObject.layer = LayerMask.NameToLayer("Outdoors");

                        var glow = c.gameObject.GetComponent<Glow>();

                        if (glow)
                        {
                            c.material.shader = Shader.Find("psx/railgun");
                            c.material.SetFloat("_EmissivePosition", 5);
                            c.material.SetFloat("_EmissiveStrength", glow.glowIntensity);
                            c.material.SetColor("_EmissiveColor", glow.glowColor);
                        }
                        else
                        {
                            c.material.shader = Shader.Find(c.material.shader.name);
                        }
                    }
                    turret.SetActive(true);
                }
            }
            if (Fire2.WasPerformedThisFrame && statueReady)
            {
                RaycastHit hit;
                if (Physics.Raycast(head.position + head.forward, head.forward, out hit, Mathf.Infinity, mask))
                {
                    if (!cooldownOff)
                    {
                        statueReady = false;
                        ReloadStatue();
                    }
                    if (_man.BuilderRat_lastStatue)
                        Destroy(_man.BuilderRat_lastStatue);
                    GameObject statue = Instantiate(_statue, hit.point, Quaternion.identity, null);
                    statue.transform.LookAt(head);
                    statue.transform.rotation = Quaternion.FromToRotation(statue.transform.up, hit.normal) * transform.rotation;
                    statue.AddComponent<DestroyOnCheckpointRestart>();
                    foreach (var c in statue.GetComponentsInChildren<Renderer>(true))
                    {
                        c.gameObject.layer = LayerMask.NameToLayer("Outdoors");

                        var glow = c.gameObject.GetComponent<Glow>();

                        if (glow)
                        {
                            c.material.shader = Shader.Find("psx/railgun");
                            c.material.SetFloat("_EmissivePosition", 5);
                            c.material.SetFloat("_EmissiveStrength", glow.glowIntensity);
                            c.material.SetColor("_EmissiveColor", glow.glowColor);
                        }
                        else
                        {
                            c.material.shader = Shader.Find(c.material.shader.name);
                        }
                    }
                    _man.BuilderRat_lastStatue = statue;
                    statue.SetActive(true);
                }
            }

            _man.BuilderRat_turretReady = turretReady;
            _man.BuilderRat_statueReady = statueReady;
        }

        private void ReloadTurret()
        {
            _man.BuilderRat_turretReady = true;
            sight.enabled = true;
            altSight.enabled = false;
        }

        private void ReloadStatue()
        {
            txt_ready.enabled = false;
            IEnumerator coroutine = _man.ReloadStatue(this);
            _man.StartCoroutine(coroutine);
        }

        private void InitializeFields()
        {
            if (origin == null)
                origin = transform.Find("RAT/gun/MuzzleOrigin");
            if (sight == null)
                sight = transform.Find("RAT/gun/Slide/FrontSight").gameObject.GetComponent<MeshRenderer>();
            if (altSight == null)
                altSight = transform.Find("RAT/gun/Slide/RedSight").gameObject.GetComponent<MeshRenderer>();
            if (txt_num == null || txt_num.Length == 0)
                txt_num = transform.Find("RAT/gun/Screen/TEXT/NUM").gameObject.GetComponentsInChildren<MeshRenderer>();
            if (txt_ready == null)
                txt_ready = transform.Find("RAT/gun/Screen/TEXT/Ready").gameObject.GetComponent<MeshRenderer>();
            if (_turret == null)
                _turret = _man.assetBundle.PrefabFind(_man.assetBundle.name, "turret");
            if (_statue == null)
                _statue = _man.assetBundle.PrefabFind(_man.assetBundle.name, "statue");
        }

        private void OnCheatChange()
        {
            cooldownOff = CheatsManager.Instance.GetCheatState("ultrakill.no-weapon-cooldown");
        }
    }
}
