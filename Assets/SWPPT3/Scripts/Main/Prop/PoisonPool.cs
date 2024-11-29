using System.Collections.Generic;
using UnityEngine;

namespace SWPPT3.Main.Prop
{
    public class PoisonPool : StatelessProp
    {
        public override void InteractWithPlayer()
        {
        }

        [SerializeField] private Collider collider;
        [SerializeField] private float buoyancyCoefficient; // 또는 config파일로 관리

        public float surfaceLevel;
        [SerializeField] public float dampingFactor = 0.5f;

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

        private void Awake()
        {
            surfaceLevel = collider.transform.position.y + collider.bounds.extents.y/2;
        }

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
            submergedHeight = Mathf.Clamp(submergedHeight, 0f, objectHeight);

            float targetSubmergedHeight = objectHeight / 4f;

            float submergedDifference = submergedHeight - targetSubmergedHeight;

            float buoyantForceMagnitude = submergedDifference * buoyancyCoefficient + Mathf.Abs(Physics.gravity.y) * objRb.mass;

            Vector3 buoyantForce = new Vector3(0f, buoyantForceMagnitude, 0f);
            objRb.AddForce(buoyantForce);
            Debug.Log($"Buoyant Force: {buoyantForce}, Submerged Height: {submergedHeight}");

            Vector3 dampingForce = -objRb.velocity * dampingFactor;
            objRb.AddForce(dampingForce);

            // 추가: 밑면을 평행하게 유지
            RestrictRotation(objRb);
        }

        void RestrictRotation(Rigidbody rb)
        {
            Quaternion currentRotation = rb.rotation;
            Vector3 currentEulerAngles = currentRotation.eulerAngles;

            float clampedX = ClampAngle(currentEulerAngles.x, -3f, 3f);
            float clampedZ = ClampAngle(currentEulerAngles.z, -3f, 3f);

            float yRotation = currentEulerAngles.y;

            Quaternion targetRotation = Quaternion.Euler(clampedX, yRotation, clampedZ);

            rb.MoveRotation(targetRotation);
        }

        float ClampAngle(float angle, float min, float max)
        {
            if (angle > 180f)
                angle -= 360f;

            return Mathf.Clamp(angle, min, max);
        }


        void OnTriggerEnter(Collider other)
        {
            Rigidbody otherRb = other.attachedRigidbody;
            if (otherRb != null)
            {
                objectsInLiquid.Add(new ObjectInLiquid(otherRb, other));
                Debug.Log(otherRb.gameObject);
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
