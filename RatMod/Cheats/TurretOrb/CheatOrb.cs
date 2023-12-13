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

            Dictionary<float, int> distances = new Dictionary<float, int>();
            for (int i = 0; i < enemyList.Length; i++)
            {
                EnemyIdentifier enemy = enemyList[i];

                if (enemy.enemyType == EnemyType.Idol)
                    continue;

                Vector3 d = enemy.weakPoint ? (enemy.weakPoint.transform.position - origin.position).normalized : (enemy.transform.position - origin.position).normalized;

                RaycastHit hit;
                Physics.Raycast(origin.position, d, out hit, Mathf.Infinity, mask);
                if (hit.transform.GetComponentInChildren<EnemyIdentifier>() || hit.transform.GetComponentInChildren<EnemyIdentifierIdentifier>())
                {
                    distances.Add(Vector3.Distance(origin.position, enemyList[i].transform.position), i);
                }
            }

            if (distances.Count > 0)
            {
                target = enemyList[distances[distances.Min(v => v.Key)]];
                direction = target.weakPoint ? (target.weakPoint.transform.position - origin.position).normalized : (target.transform.position - origin.position).normalized;
                foundTarget = true;
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
                beam = AssetLoader.AssetFind<GameObject>("Revolver Beam Alternative.prefab");
        }
    }
}
