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
        private readonly float DAMAGE = 0.2f;

        LayerMask mask = LayerMask.GetMask("Environment", "Outdoors");

        private bool _enabled = false;
        private bool _traveling = false;
        private bool _attached = false;
        private bool _findingTarget = false;
        private bool _hurting = false;
        private EnemyIdentifier Target;
        private NavMeshAgent agent;
        private Rigidbody rb;
        private EnemyIdentifier CachedEnemy;

        public void Awake()
        {
            rb = GetComponent<Rigidbody>();
            agent = GetComponent<NavMeshAgent>();
            agent.areaMask = 13;
            gameObject.AddComponent<DestroyOnCheckpointRestart>();
        }

        public void OnDisable()
        {
            GreechKnife.ActiveKnife = null;
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
                _enabled = true;
                rb.isKinematic = true;
                _traveling = false;
                transform.parent = collider.transform;
                _attached = true;
                Target = collider.GetComponentInChildren<EnemyIdentifier>() ?? collider.GetComponentInChildren<EnemyIdentifierIdentifier>().eid;
                if (!_hurting)
                {
                    _hurting = true;
                    CachedEnemy = Target;
                    Invoke("Damage", 0.25f);
                }
            }
            agent.enabled = _traveling;
        }

        public void Update()
        {
            if (timeLeft <= 0f)
            {
                _enabled = false;
                GreechKnife.ActiveKnife = null;
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
                FindTarget();
            }

            if (_traveling) Travel();
        }

        private void FindTarget()
        {
            if (_findingTarget) return;
            _findingTarget = true;

            if (transform.parent != null)
            {
                Target = transform.parent.GetComponentInChildren<EnemyIdentifier>() ?? transform.parent.GetComponentInChildren<EnemyIdentifierIdentifier>().eid;
                if (Target) return;
            }

            EnemyIdentifier[] enemyList = EnemyTracker.Instance.GetCurrentEnemies().ToArray();
            if (enemyList.Length == 0)
                return;

            Dictionary<float, int> distances = new Dictionary<float, int>();
            for (int i = 0; i < enemyList.Length; i++)
            {
                agent.SetDestination(enemyList[i].transform.position);
                if (agent.hasPath)
                {
                    float dist = Vector3.Distance(transform.position, enemyList[i].transform.position);
                    if (!distances.ContainsKey(dist))
                        distances.Add(dist, i);
                }
            }

            if (distances.Count > 0)
            {
                Target = enemyList[distances[distances.Min(k => k.Key)]];
                agent.SetDestination(Target.transform.position);
            }
            _findingTarget = false;
        }

        private void Travel()
        {
            if (!Target || Target.dead) return;
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
            while (Target && !Target.dead && Vector3.Distance(transform.position, target) > 0.2f)
            {
                if (isWeakpoint)
                {
                    target = Target.weakPoint.transform.position;
                }
                else
                {
                    target = Target.transform.position;
                }

                jumpTime += Time.fixedDeltaTime;
                float mix = 10 * Mathf.Log10(jumpTime + 1);
                transform.position = Vector3.Lerp(startPos, target, mix);

                yield return new WaitForFixedUpdate();
            }

            if (_attached && !_hurting)
            {
                _hurting = true;
                CachedEnemy = Target;
                Invoke("Damage", 0.25f);
            }

            yield return null;
        }

        private void Damage()
        {
            _hurting = true;
            if (CachedEnemy != Target) { _hurting = false; return; }
            if (Target != null && !Target.dead)
            {
                Target.DeliverDamage(Target.gameObject, new Vector3(), transform.position, DAMAGE, false);
                if (Target.dead) { _hurting = false; return; }
                Invoke("Damage", 0.25f);
            }
            _hurting = false;
        }

        public void Recall()
        {
            _enabled = false;
            Destroy(gameObject);
        }
    }
}
