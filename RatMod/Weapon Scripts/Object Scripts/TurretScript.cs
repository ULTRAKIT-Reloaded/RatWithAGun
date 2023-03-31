using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMM;
using UnityEngine;
using ULTRAKIT.Extensions;
using System.Collections;
using static UnityEngine.EventSystems.EventTrigger;

namespace RatMod.Weapon_Scripts.Object_Scripts
{
    public class TurretScript : MonoBehaviour
    {
        public GameObject sourceWeapon = null;
        LayerMask mask = LayerMask.GetMask("Environment", "Outdoors", "EnemyTrigger", "BigCorpse");

        Transform gun;
        Transform origin;
        GameObject beam;

        private bool cooldownOff;
        private readonly float timer = 10f;
        private readonly float fireDelay = 0.5f;
        private float lastFired = Time.time;
        
        private void Start()
        {
            cooldownOff = CheatsManager.Instance.GetCheatState("ultrakill.no-weapon-cooldown");
            InitializeFields();
            Invoke("DestroySelf", timer);
        }

        private void Update()
        {
            if (cooldownOff || (Time.time - fireDelay >= lastFired))
            {
                bool foundTarget = false;
                EnemyIdentifier target = new EnemyIdentifier();
                Vector3 direction = new Vector3();

                EnemyIdentifier[] enemyList = EnemyTracker.Instance.GetCurrentEnemies().ToArray();
                if (enemyList.Length == 0)
                    return;

                Dictionary<float, int> distances = new Dictionary<float, int>();
                for (int i = 0; i < enemyList.Length; i++)
                {
                    EnemyIdentifier enemy = enemyList[i];

                    if (enemy.enemyType == EnemyType.Idol || enemy.blessed)
                        continue;

                    Vector3 d = (enemy.transform.position - gun.position).normalized;
                    if (enemy.weakPoint)
                        d = (enemy.weakPoint.transform.position - gun.position).normalized;

                    RaycastHit hit;
                    Physics.Raycast(gun.position, d, out hit, Mathf.Infinity, mask);
                    if (hit.transform.GetComponentInChildren<EnemyIdentifier>() || hit.transform.GetComponentInChildren<EnemyIdentifierIdentifier>())
                    {
                        float dist = Vector3.Distance(transform.position, enemy.transform.position);
                        if (!distances.ContainsKey(dist))
                            distances.Add(dist, i);
                    }
                }

                if (distances.Count > 0)
                {
                    target = enemyList[distances[distances.Min(v => v.Key)]];
                    direction = target.weakPoint ? (target.weakPoint.transform.position - gun.position).normalized : (target.transform.position - gun.position).normalized;
                    foundTarget = true;
                }

                if (foundTarget)
                {
                    gun.rotation = Quaternion.LookRotation(direction);
                    GameObject shot = Instantiate(beam, origin.position, gun.rotation);
                    shot.GetComponent<RevolverBeam>().sourceWeapon = sourceWeapon;
                    lastFired = Time.time;
                }
            }
        }

        private void InitializeFields()
        {
            if (gun == null)
                gun = transform.Find("RAT/GUN");
            if (origin == null)
                origin = transform.Find("RAT/GUN/BARREL/MuzzleOrigin");
            if (beam == null)
                beam = AssetLoader.AssetFind<GameObject>("RevolverBeamAlt.prefab");
        }

        private void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}
