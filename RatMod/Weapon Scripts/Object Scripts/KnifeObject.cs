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

namespace RatMod.Weapon_Scripts.Object_Scripts
{
    public class KnifeObject : MonoBehaviour
    {
        public double timeLeft;
        private readonly float JumpForce = 1000f;

        LayerMask mask = LayerMask.GetMask("Environment", "Outdoors");

        private bool _enabled = false;
        private bool _traveling = true;
        private bool _attached = false;
        private EnemyIdentifier Target;
        private NavMeshAgent agent;
        private Rigidbody rb;

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
            }

            if (_traveling) Travel();
            if (_attached)
            {
                Target.Explode();
            }
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
                rb.isKinematic = false;
                if (Target.weakPoint)
                {
                    Vector3 target = Target.weakPoint.transform.position - transform.position;
                    rb.AddForce(new Vector3(target.x * JumpForce, target.y * JumpForce, target.z * JumpForce));
                }
                else
                {
                    Vector3 target = Target.transform.position - transform.position;
                    rb.AddForce(new Vector3(target.x * JumpForce, (target.y + 1) * JumpForce, target.z * JumpForce));
                }
            }
        }

        public void Recall()
        {
            _enabled = false;
            Destroy(gameObject);
        }
    }
}
