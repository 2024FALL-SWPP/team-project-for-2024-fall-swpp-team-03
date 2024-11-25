using System.Collections.Generic;
using UnityEngine;

namespace SWPPT3.Main.Prop
{
    public class PoisonPool : StatelessProp
    {
        public override void InteractWithPlayer()
        {
            // 플레이어와의 상호작용 로직
        }

        [Header("Liquid Properties")]
        public float liquidDensity = 1000f;

        [Header("Buoyancy Settings")]
        public float surfaceLevel = 0f;
        public float dampingFactor = 0.5f;

        private class ObjectInLiquid
        {
            public Rigidbody rb;
            public Collider collider;

            public ObjectInLiquid(Rigidbody rb, Collider collider)
            {
                this.rb = rb;
                this.collider = collider;
            }
        }

        private readonly List<ObjectInLiquid> objectsInLiquid = new List<ObjectInLiquid>();

        void FixedUpdate()
        {
            ApplyBuoyancy();
        }

        void ApplyBuoyancy()
        {
            foreach (var obj in objectsInLiquid)
            {
                ApplyBuoyantForce(obj);
            }
        }

        void ApplyBuoyantForce(ObjectInLiquid obj)
        {
            Rigidbody objRb = obj.rb;
            Collider objCollider = obj.collider;

            float objectHeight = objCollider.bounds.size.y;
            float objectBottom = objRb.position.y - (objectHeight / 2f);

            float submergedHeight = surfaceLevel - objectBottom;
            float totalSubmerged = Mathf.Clamp(submergedHeight / objectHeight, 0f, 1f);

            if (totalSubmerged > 0f)
            {
                float displacedVolume = totalSubmerged * objCollider.bounds.size.x * objCollider.bounds.size.z * objectHeight;
                float buoyantForceMagnitude = liquidDensity * displacedVolume * Physics.gravity.magnitude;

                Vector3 buoyantForce = new Vector3(0f, buoyantForceMagnitude, 0f);
                objRb.AddForce(buoyantForce);

                Vector3 dampingForce = -objRb.velocity * dampingFactor;
                objRb.AddForce(dampingForce);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            Rigidbody otherRb = other.attachedRigidbody;
            if (otherRb != null)
            {
                objectsInLiquid.Add(new ObjectInLiquid(otherRb, other));
            }
        }

        void OnTriggerExit(Collider other)
        {
            Rigidbody otherRb = other.attachedRigidbody;
            if (otherRb != null)
            {
                objectsInLiquid.RemoveAll(obj => obj.rb == otherRb);
            }
        }
    }
}
