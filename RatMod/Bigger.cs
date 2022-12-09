using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ULTRAKIT.Extensions;

namespace RatMod
{
    /*public class Bigger : IBuff
    {
        public EnemyIdentifier eid { get => _eid; }
        public bool IsActive { get => active; }
        public string id => "RatWithAGun.bigger";

        private bool active = false;
        private EnemyIdentifier _eid;

        public void Enable()
        {
            active = true;
            Vector3 s = eid.transform.localScale;
            s = new Vector3(s.x * 2, s.y * 2, s.z * 2);
            eid.health *= 2;
        }

        public void Disable() 
        {
            active = false;
            Vector3 s = eid.transform.localScale;
            s = new Vector3(s.x / 2, s.y / 2, s.z / 2);
            eid.health /= 2;
        }

        public void Update() 
        {
            eid.health += Time.deltaTime;
        }
    }*/

    public static class Bigger
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
            buff.eid.health += Time.deltaTime;
        }

        public static Buff buff = new Buff
        {
            id = "RATWITHAGUN.bigger",
            EnableScript = Enable,
            DisableScript = Disable,
            UpdateScript = OnUpdate,
        };
    }
}
