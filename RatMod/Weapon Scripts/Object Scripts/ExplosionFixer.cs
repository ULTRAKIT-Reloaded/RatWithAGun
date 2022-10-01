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

namespace RatMod.Weapon_Scripts.Object_Scripts
{
    public class ExplosionFixer : MonoBehaviour
    {
        private void Start()
        {
            foreach (Explosion explosion in GetComponentsInChildren<Explosion>())
            {
                explosion.enemy = false;
            }
        }
    }
}
