using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMM;
using UnityEngine;
using ULTRAKIT.Extensions;
using UnityEngine.AI;
using System.Reflection.Emit;
using System.Collections;

namespace RatMod.Weapon_Scripts.Object_Scripts
{
    public class KnifeObject : MonoBehaviour
    {
        public double timeLeft;
        private readonly float JumpForce = 1000f;
        private readonly float DAMAGE = 0.2f;

        LayerMask mask = LayerMask.GetMask("Environment", "Outdoors");

        private bool _enabled = false;
        private bool _traveling = true;
        private bool _attached = false;
        private bool _findingTarget = false;
        private EnemyIdentifier Target;
        private NavMeshAgent agent;
        private Rigidbody rb;
        private EnemyIdentifier CachedEnemy;

        public void Awake()
        {
            rb = GetComponent<Rigidbody>();
            agent = GetComponent<NavMeshAgent>();
            agent.areaMask = 13;
        }

        public void Start()
        {
            agent.enabled = true;
        }

        public void OnTriggerEnter(Collider collider)
        {
            if (mask == (mask | (1 << collider.gameObject.layer)) && !_attached)
            {
                _enabled = true;
                _traveling = true;
            }
            if (collider.gameObject.layer == 12 && !_attached)
            {
                rb.isKinematic = true;
                _traveling = false;
                transform.parent = collider.transform;
                _attached = true;
                RaycastHit hit;
                Physics.Raycast(transform.position, (collider.transform.position - transform.position).normalized, out hit, Mathf.Infinity, 12);
                transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            }
            agent.enabled = _traveling;
        }

        public void Update()
        {
            if (timeLeft <= 0f)
            {
                _enabled = false;
                Destroy(gameObject);
            }
            if (!_enabled) return;
            timeLeft -= Time.deltaTime;
            if (Target != null && Target.dead) 
            { 
                Target = null; 
                _traveling = true;
                transform.parent = null;
                _attached = false;
                agent.enabled = _traveling;
                agent.SetDestination(transform.position);
            }
            if (Target == null)
            {
                UKLogger.Log("Finding target");
                FindTarget();
            }

            if (_traveling) Travel();
        }

        private void FindTarget()
        {
            if (_findingTarget) return;
            _findingTarget = true;
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
                agent.SetDestination(enemyList[pair.Value].transform.position);
                if (agent.hasPath)
                {
                    Target = enemyList[pair.Value];
                    break;
                }
            }
            _findingTarget = false;
        }

        private void Travel()
        {
            if (!Target) return;
            //RaycastHit hit;
            //Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity);
            //transform.LookAt(Target.transform.position);
            //transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            agent.SetDestination(Target.transform.position);
            if (agent.remainingDistance < 8f)
            {
                agent.enabled = false;
                _traveling = false;
                StartCoroutine(Jump());
            }
        }

        private IEnumerator Jump()
        {
            float jumpTime = 0f;
            Vector3 startPos = transform.position;
            Vector3 target = new Vector3();
            bool isWeakpoint = Target.weakPoint;
            while (Target && !Target.dead && /*!_attached*/ Vector3.Distance(transform.position, target) > 0.2f)
            {
                if (isWeakpoint)
                {
                    target = Target.weakPoint.transform.position /*- transform.position*/;
                    //rb.AddForce(new Vector3(target.x * JumpForce, target.y * JumpForce, target.z * JumpForce));
                }
                else
                {
                    target = Target.transform.position /*- transform.position*/;
                    //rb.AddForce(new Vector3(target.x * JumpForce, (target.y + 1) * JumpForce, target.z * JumpForce));
                }

                jumpTime += Time.fixedDeltaTime;
                float mix = 10 * Mathf.Log10(jumpTime + 1);
                transform.position = Vector3.Lerp(startPos, target, mix);

                yield return new WaitForFixedUpdate();
            }

            if (_attached)
            {
                CachedEnemy = Target;
                Invoke("Damage", 0.25f);
            }

            yield return null;
        }

        private void Damage()
        {
            if (CachedEnemy != Target) return;
            if (Target != null && !Target.dead)
            {
                Target.DeliverDamage(Target.gameObject, new Vector3(), transform.position, DAMAGE, false);
                if (Target.dead) return;
                Invoke("Damage", 0.25f);
            }
        }

        public void Recall()
        {
            _enabled = false;
            Destroy(gameObject);
        }
    }
}
