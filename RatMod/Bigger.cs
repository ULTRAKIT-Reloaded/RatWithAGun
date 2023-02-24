using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ULTRAKIT.Extensions;
using ULTRAKIT.Loader;
using ULTRAKIT.Extensions.Classes;
using ULTRAKIT.Extensions.Interfaces;

namespace RatMod
{
    public class Bigger : IBuff
    {
        public EnemyIdentifier eid { get => _eid; set => _eid = value; }
        public bool IsActive { get => active; }
        public string id => "RATWITHAGUN.bigger";

        private bool active = false;
        private EnemyIdentifier _eid;

        public void Enable()
        {
            active = true;
            Vector3 s = _eid.transform.localScale;
            _eid.transform.localScale = new Vector3(s.x * 2, s.y * 2, s.z * 2);
            _eid.DeliverDamage(_eid.gameObject, Vector3.zero, Vector3.zero, -(_eid.health * 2), false);
        }
        public void Disable() 
        {
            active = false;
            Vector3 s = _eid.transform.localScale;
            _eid.transform.localScale = new Vector3(s.x / 2, s.y / 2, s.z / 2);
            _eid.DeliverDamage(_eid.gameObject, Vector3.zero, Vector3.zero, _eid.health / 1.5f, false);
        }

        public void Update() 
        {
            _eid.DeliverDamage(_eid.gameObject, Vector3.zero, Vector3.zero, -(Time.deltaTime), false);
        }
    }

    /*public static class Bigger
    {
        public static void Enable()
        {
            Vector3 s = buff.eid.transform.localScale;
            buff.eid.transform.localScale = new Vector3(s.x * 2, s.y * 2, s.z * 2);
            buff.eid.DeliverDamage(buff.eid.gameObject, Vector3.zero, Vector3.zero, -(buff.eid.health * 2), false);
        }

        public static void Disable()
        {
            Vector3 s = buff.eid.transform.localScale;
            buff.eid.transform.localScale = new Vector3(s.x / 2, s.y / 2, s.z / 2);
            buff.eid.DeliverDamage(buff.eid.gameObject, Vector3.zero, Vector3.zero, buff.eid.health / 1.5f, false);
        }

        public static void OnUpdate()
        {
            buff.eid.DeliverDamage(buff.eid.gameObject, Vector3.zero, Vector3.zero, -(Time.deltaTime), false);
        }

        public static Buff buff = new Buff
        {
            id = "RATWITHAGUN.bigger",
            EnableScript = Enable,
            DisableScript = Disable,
            UpdateScript = OnUpdate,
        };
    }*/
}
