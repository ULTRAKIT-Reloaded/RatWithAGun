using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMM;
using UnityEngine;
using ULTRAKIT.Extensions;
using ULTRAKIT.Data;
using System.Collections;
using static UnityEngine.GraphicsBuffer;

namespace RatMod.Weapon_Scripts.Object_Scripts
{
    public class ProjectileScript : MonoBehaviour
    {
        public GameObject sourceWeapon = null;

        private RatVariableManager _man = RatVariableManager.Instance;
        private LayerMask mask = LayerMask.GetMask("Environment", "Outdoors", "EnemyTrigger", "BigCorpse");

        private Rigidbody rb;
        private Projectile proj;

        private Transform part1;
        private Transform part2;
        private Transform part3;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            proj = gameObject.AddComponent<Projectile>();

            part1 = transform.Find("Component-1");
            part2 = transform.Find("Component-2");
            part3 = transform.Find("Component-3");

            proj.RenderObject(LayerMask.NameToLayer("Projectile"));

            proj.target = FindTarget();
            SetProjectile();
        }
        
        private void Update()
        {
            part1.rotation = UnityEngine.Random.rotation;
            part2.rotation = UnityEngine.Random.rotation;
            part3.rotation = UnityEngine.Random.rotation;
            part1.transform.position = transform.position;
            part2.transform.position = transform.position;
            part3.transform.position = transform.position;
        }

        private void SetProjectile()
        {
            proj.damage = 15;
            proj.speed = 20;
            proj.turningSpeedMultiplier = 1;
            proj.decorative = false;
            proj.undeflectable = false;
            proj.friendly = true;
            proj.playerBullet = true;
            proj.bulletType = "HomingGun";
            proj.weaponType = "Rat";
            proj.explosive = true;
            proj.bigExplosion = true;
            proj.explosionEffect = _man.Asset_MindflayerExplosion;
            if (proj.target != null)
                proj.homingType = HomingType.Loose;
            proj.explosionEffect.AddComponent<ExplosionFixer>().sourceWeapon = sourceWeapon;

            Grenade gren = gameObject.AddComponent<Grenade>();
            gren.explosion = proj.explosionEffect;
            gren.superExplosion = proj.explosionEffect;
            gren.enemy = false;
            gren.rideable = false;
            gren.rocket = false;
            gren.hitterWeapon = "Rat";
            gren.sourceWeapon = gameObject;
        }

        private Transform FindTarget()
        {
            bool targetFound = false;
            if (EnemyTracker.Instance.GetCurrentEnemies().Count == 0)
                return null;
            RaycastHit searchCast;
            Vector3 searchOrigin = CameraController.Instance.transform.position;
            if(Physics.Raycast(CameraController.Instance.transform.position + CameraController.Instance.transform.forward, CameraController.Instance.transform.forward, out searchCast, Mathf.Infinity, mask))
            {
                searchOrigin = searchCast.point;
            }

            SortedDictionary<float, int> distances = new SortedDictionary<float, int>();
            EnemyIdentifier[] enemies = EnemyTracker.Instance.GetCurrentEnemies().ToArray();
            for (int i = 0; i < enemies.Length; i++)
            {
                distances.Add(Vector3.Distance(enemies[i].transform.position, searchOrigin), i);
            }

            EnemyIdentifier chosenEnemy = new EnemyIdentifier();
            foreach (var pair in distances)
            {
                RaycastHit hit;
                Physics.Raycast(searchOrigin, (enemies[pair.Value].transform.position - searchOrigin).normalized, out hit, Mathf.Infinity, mask);
                if (hit.collider.GetComponent<EnemyIdentifier>() || hit.collider.GetComponent<EnemyIdentifierIdentifier>())
                {
                    chosenEnemy = enemies[pair.Value];
                    targetFound = true;
                    break;
                }
            }

            if (!targetFound)
            {
                EnemyIdentifier fleshPrison = enemies.Where(n => n.enemyType == EnemyType.FleshPrison).ToArray()?.First();
                if (fleshPrison != null)
                    return fleshPrison.transform;
                return null;
            }
            return chosenEnemy.transform;
        }
    }
}
