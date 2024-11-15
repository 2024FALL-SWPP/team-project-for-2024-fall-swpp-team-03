using SWPPT3.Main.Prop;
using UnityEngine;

namespace SWPPT3.Main.PlayerLogic.State
{
    public class MetalState : PlayerState
    {
        public override void InteractWithProp(Player player, PropBase obstacle)
        {
            if (false)
            {
                obstacle.InteractWithPlayer();
            }
            else
            {
                base.InteractWithProp(player, obstacle);
            }
        }

        public override void ChangeRigidbody(Rigidbody rb)
        {
            rb.mass = 10f;
        }

        public override void ChangePhysics(Collider collider, PhysicMaterial physicMaterial)
        {
            physicMaterial.bounciness = 0f;
            physicMaterial.dynamicFriction = 0f;
            physicMaterial.staticFriction = 0f;

            physicMaterial.bounceCombine = PhysicMaterialCombine.Maximum;

            collider.material = physicMaterial;
        }
    }
}
