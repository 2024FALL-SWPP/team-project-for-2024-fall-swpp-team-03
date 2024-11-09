using SWPPT3.Main.Prop;
using UnityEngine;

namespace SWPPT3.Main.PlayerLogic.State
{
    public class RubberState : PlayerState
    {
        public override void InteractWithProp(Player player, PropBase obstacle)
        {
            if (false)
            {
                // SomeSpecificProp에 대한 특별한 상호작용 처리
                // 여기에서 player 또는 obstacle에 대한 특정 작업 수행
            }
            else
            {
                base.InteractWithProp(player, obstacle);
            }
        }
        public override void ChangeRigidbody(Rigidbody rb)
        {
            rb.mass = 5f;
        }
        public override void ChangePhysics(Collider collider, PhysicMaterial physicMaterial)
        {
            if (physicMaterial == null)
            {
                physicMaterial = new PhysicMaterial();
            }
            physicMaterial.bounciness = 0.5f;
            physicMaterial.dynamicFriction = 0f;
            physicMaterial.staticFriction = 0f;

            physicMaterial.bounceCombine = PhysicMaterialCombine.Maximum;

            collider.material = physicMaterial;
        }
    }
}
