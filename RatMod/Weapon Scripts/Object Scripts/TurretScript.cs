using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMM;
using UnityEngine;
using ULTRAKIT.Extensions;
using System.Collections;

namespace RatMod.Weapon_Scripts.Object_Scripts
{
    public class TurretScript : MonoBehaviour
    {
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

                SortedDictionary<float, int> distances = new SortedDictionary<float, int>();
                for (int i = 0; i < enemyList.Length; i++)
                {
                    distances.Add(Vector3.Distance(transform.position, enemyList[i].transform.position), i);
                }

                foreach (var pair in distances)
                {
                    EnemyIdentifier enemy = enemyList[pair.Value];

                    if (enemy.enemyType == EnemyType.Idol)
                        continue;

                    Vector3 d = (enemy.transform.position - gun.position).normalized;
                    if (enemy.weakPoint)
                        d = (enemy.weakPoint.transform.position - gun.position).normalized;

                    RaycastHit hit;
                    Physics.Raycast(gun.position, d, out hit, Mathf.Infinity, mask);
                    if (hit.transform.GetComponentInChildren<EnemyIdentifier>() || hit.transform.GetComponentInChildren<EnemyIdentifierIdentifier>())
                    {
                        target = enemy;
                        direction = d;
                        foundTarget = true;
                        break;
                    }
                }

                if (foundTarget)
                {
                    gun.rotation = Quaternion.LookRotation(direction);
                    Instantiate(beam, origin.position, gun.rotation);
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
                beam = DazeExtensions.PrefabFind("RevolverBeamAlt");
        }

        private void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}
