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
        public GameObject sourceWeapon;

        private RatVariableManager _man = RatVariableManager.Instance;
        private List<EnemyIdentifier> harmedEnemies;

        private bool active = true;
        private int counter = 0;

        private void Start()
        {
            harmedEnemies = new List<EnemyIdentifier>();
            GameObject.Instantiate(_man.Asset_PhysicalShockwaveHarmless, transform.position, Quaternion.identity);
            Explosion exp1 = GameObject.Instantiate(_man.Asset_ExplosionPrime, transform.position, Quaternion.identity).GetComponentInChildren<Explosion>();
            Explosion exp2 = GameObject.Instantiate(_man.Asset_ExplosionSuper, transform.position, Quaternion.identity).GetComponentInChildren<Explosion>();
            exp2.transform.localScale = new Vector3(100, 100, 100);
            exp1.enemyDamageMultiplier = 0;
            exp2.enemyDamageMultiplier = 0;
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
            if (active && collider.gameObject.GetComponentInParent<EnemyIdentifier>() && !harmedEnemies.Contains(collider.gameObject.GetComponentInParent<EnemyIdentifier>()))
            {
                EnemyIdentifier enemy = collider.gameObject.GetComponentInParent<EnemyIdentifier>();
                if (enemy.dead) return;

                harmedEnemies.Add(enemy);
                bool boss = enemy.bigEnemy;
                if (Damage(enemy, RatVariableManager.isUnbalanced))
                {
                    counter++;
                    StyleHUD.Instance.DecayFreshness(sourceWeapon, "ultrakill.exploded", boss);
                }
                StyleHUD.Instance.DecayFreshness(sourceWeapon, "ultrakill.explosionhit", boss);

                if (counter == 0)
                    return;
                if (counter == 1)
                {
                    StyleHUD.Instance.AddPoints(350, "<color=green>FRESHLY MURDERED</color>");
                    return;
                }
                StyleHUD.Instance.AddPoints(350, $"<color=green>FRESHLY MURDERED x{counter}</color>");
            }
        }

        private bool Damage(EnemyIdentifier enemy, bool isUnbalanced)
        {
            if (isUnbalanced)
            {
                enemy.Explode();
                if (enemy.enemyType == EnemyType.Idol)
                    enemy.GetComponent<Idol>().Death();
                enemy.hitter = "greechStatue";
                return true;
            }
            enemy.DeliverDamage(enemy.gameObject, Vector3.up, enemy.transform.position, 5, true, 0, sourceWeapon);
            enemy.hitter = "greechStatue";
            if (!enemy.hitterWeapons.Contains("greechStatue"))
            {
                enemy.hitterWeapons.Add("greechStatue");
            }
            if (enemy.dead)
                return true;
            return false;
        }

        private void Deactivate()
        {
            active = false;
        }
    }
}
