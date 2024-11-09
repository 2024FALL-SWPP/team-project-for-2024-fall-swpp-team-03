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
                // SomeSpecificProp에 대한 특별한 상호작용 처리
                Debug.Log("Interacting with a specific prop in SlimeState.");
                // 여기에서 player 또는 obstacle에 대한 특정 작업 수행
                obstacle.InteractWithPlayer(States.Metal);
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
            if (physicMaterial == null)
            {
                physicMaterial = new PhysicMaterial();
            }
            physicMaterial.bounciness = 0f;
            physicMaterial.dynamicFriction = 0f;
            physicMaterial.staticFriction = 0f;

            physicMaterial.bounceCombine = PhysicMaterialCombine.Maximum;

            collider.material = physicMaterial;
        }

    }
}
