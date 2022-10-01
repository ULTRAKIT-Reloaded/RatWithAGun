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
    public class StatueScript : MonoBehaviour
    {
        private RatVariableManager _man = RatVariableManager.Instance;
        private List<EnemyIdentifier> killedEnemies;

        bool active = true;
        int counter = 0;

        private void Start()
        {
            killedEnemies = new List<EnemyIdentifier>();
            GameObject.Instantiate(_man.Asset_ExplosionPrime, transform.position, Quaternion.identity);
            GameObject.Instantiate(_man.Asset_PhysicalShockwaveHarmless, transform.position, Quaternion.identity);
            GameObject expl = GameObject.Instantiate(_man.Asset_ExplosionSuper, transform.position, Quaternion.identity);
            expl.transform.localScale = new Vector3(100, 100, 100);
            float timeToWait = Time.deltaTime * 3;
            Invoke("Deactivate", timeToWait);

            foreach (EnemyIdentifier enemy in EnemyTracker.Instance.GetEnemiesOfType(EnemyType.HideousMass))
            {
                Collider collider = GetComponent<Collider>();
                Vector3 targetPoint = enemy.transform.position + new Vector3(0, 8, 0);
                Vector3 closestPoint = collider.ClosestPoint(targetPoint);
                float distX = Mathf.Abs(closestPoint.x - targetPoint.x);
                float distY = Mathf.Abs(closestPoint.y - targetPoint.y);
                float distZ = Mathf.Abs(closestPoint.z - targetPoint.z);
                if (distX < 3f && distY < 8f && distZ < 5f)
                    enemy.Explode();
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (active && collider.gameObject.GetComponentInParent<EnemyIdentifier>() && !killedEnemies.Contains(collider.gameObject.GetComponentInParent<EnemyIdentifier>()))
            {
                EnemyIdentifier enemy = collider.gameObject.GetComponentInParent<EnemyIdentifier>();

                enemy.Explode();
                if (enemy.enemyType == EnemyType.Idol)
                    enemy.GetComponent<Idol>().Death();
                killedEnemies.Add(enemy);
                counter++;

                if (counter == 1)
                {
                    StyleHUD.Instance.AddPoints(350, "<color=green>FRESHLY MURDERED</color>");
                    return;
                }
                StyleHUD.Instance.AddPoints(350, $"<color=green>FRESHLY MURDERED x{counter}</color>");
            }
        }

        private void Deactivate()
        {
            active = false;
        }
    }
}
