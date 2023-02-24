using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMM;
using UnityEngine;
using ULTRAKIT.Extensions;
using RatMod.Weapon_Scripts.Object_Scripts;

namespace RatMod.Weapon_Scripts
{
    public class GreechKnife : MonoBehaviour
    {
        private const float _launchSpeed = 10f;
        private RatVariableManager _man = RatVariableManager.Instance;
        private InputActionState Fire1 = InputManager.Instance?.InputSource.Fire1;
        private InputActionState Fire2 = InputManager.Instance?.InputSource.Fire2;

        private GameObject _knifePrefab;
        private GameObject ActiveKnife;

        private double timeToRun = 10d;

        public void OnEnable()
        {
            InitializeFields();
        }

        public void Update()
        {
            if (Fire1.WasPerformedThisFrame && ActiveKnife == null)
            {
                ActiveKnife = Instantiate(_knifePrefab, CameraController.Instance.transform.position + CameraController.Instance.transform.forward, Quaternion.identity, null);
                ActiveKnife.transform.RenderObject(LayerMask.NameToLayer("Projectile"));
                ActiveKnife.GetComponent<KnifeObject>().timeLeft = timeToRun;
                Vector3 launchForce = new Vector3(CameraController.Instance.transform.forward.x * _launchSpeed, CameraController.Instance.transform.forward.y * _launchSpeed, CameraController.Instance.transform.forward.z * _launchSpeed);
                ActiveKnife.GetComponent<Rigidbody>().AddForce( launchForce, ForceMode.VelocityChange);
            }
            if (Fire2.WasPerformedThisFrame && ActiveKnife != null)
            {
                ActiveKnife.GetComponent<KnifeObject>().Recall();
            }
        }

        private void InitializeFields()
        {
            if (_knifePrefab == null)
            {
                _knifePrefab = _man.assetBundle.AssetFind<GameObject>("knifeobject");
            }
        }
    }
}
