using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMM;
using UnityEngine;
using ULTRAKIT.Extensions;
using System.Collections;
using static UnityEngine.UI.Image;

namespace RatMod.Cheats.TurretOrb
{
    public class CheatOrb : MonoBehaviour
    {
        private Transform[] arms;
        LayerMask mask = LayerMask.GetMask("Environment", "Outdoors", "EnemyTrigger", "BigCorpse");
        private Transform origin;
        private GameObject beam;

        public void Start()
        {
            arms = transform.Find("Arms").GetComponentsInChildren<Transform>();
            InitializeFields();
        }

        public void Update()
        {
            RandomizeArms();

            bool foundTarget = false;
            EnemyIdentifier target = new EnemyIdentifier();
            Vector3 direction = new Vector3();

            EnemyIdentifier[] enemyList = EnemyTracker.Instance.GetCurrentEnemies().ToArray();
            if (enemyList.Length == 0)
                return;

            SortedDictionary<float, int> distances = new SortedDictionary<float, int>();
            for (int i = 0; i < enemyList.Length; i++)
            {
                distances.Add(Vector3.Distance(origin.position, enemyList[i].transform.position), i);
            }

            foreach (var pair in distances)
            {
                EnemyIdentifier enemy = enemyList[pair.Value];

                if (enemy.enemyType == EnemyType.Idol)
                    continue;

                Vector3 d = (enemy.transform.position - origin.position).normalized;
                if (enemy.weakPoint)
                    d = (enemy.weakPoint.transform.position - origin.position).normalized;

                RaycastHit hit;
                Physics.Raycast(origin.position, d, out hit, Mathf.Infinity, mask);
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
                Instantiate(beam, origin.position, Quaternion.LookRotation(direction));
            }
        }

        private void RandomizeArms()
        {
            foreach (Transform arm in arms)
            {
                arm.rotation *= UnityEngine.Random.rotation;
            }
        }

        private void InitializeFields()
        {
            if (origin == null)
                origin = transform.Find("Origin");
            if (beam == null)
                beam = AssetLoader.AssetFind<GameObject>("RevolverBeamAlt.prefab");
        }
    }
}
