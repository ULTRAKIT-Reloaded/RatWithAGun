using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULTRAKIT.Data;
using ULTRAKIT.Extensions;
using UnityEngine;

namespace RatMod.Cheats.TurretOrb
{
    public static class OrbEnabler
    {
        public static GameObject activeOrb;

        private static void Enable()
        {
            activeOrb = GameObject.Instantiate(RatVariableManager.Instance.assetBundle.PrefabFind(RatVariableManager.Instance.assetBundle.name, "turretorb"), 
                NewMovement.Instance.transform.position + new Vector3(0, 5, 0), Quaternion.identity, NewMovement.Instance.transform);
            foreach (var c in activeOrb.GetComponentsInChildren<Renderer>(true))
            {
                c.gameObject.layer = LayerMask.NameToLayer("Outdoors");

                var glow = c.gameObject.GetComponent<Glow>();

                if (glow)
                {
                    c.material.shader = Shader.Find("psx/railgun");
                    c.material.SetFloat("_EmissivePosition", 5);
                    c.material.SetFloat("_EmissiveStrength", glow.glowIntensity);
                    c.material.SetColor("_EmissiveColor", glow.glowColor);
                }
                else
                {
                    c.material.shader = Shader.Find(c.material.shader.name);
                }
            }
        }

        private static void Disable()
        {
            GameObject.Destroy(activeOrb);
        }

        private static void OnUpdate()
        {
        }

        public static Cheat cheat = new Cheat
        {
            LongName = "Gun Orb",
            Identifier = "RATWITHAGUN.gun_orb",
            ButtonEnabledOverride = "Orb Active",
            ButtonDisabledOverride = "Orb Inactive",
            DefaultState = false,
            PersistenceMode = StatePersistenceMode.Persistent,
            EnableScript = Enable,
            DisableScript = Disable,
            UpdateScript = OnUpdate,
        };
    }
}
