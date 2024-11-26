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
            public Box Box;

            public ObjectInLiquid(Rigidbody rb, Collider collider, Box box)
            {
                this.rb = rb;
                this.collider = collider;
                this.Box = box;
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

            // 객체의 높이 및 밑면적 계산
            float objectHeight = objCollider.bounds.size.y;
            float baseArea = objCollider.bounds.size.x * objCollider.bounds.size.z;

            // 객체의 아래쪽 위치 계산
            float objectBottom = objRb.position.y - (objectHeight / 2f);

            // 잠긴 높이(h) 계산, 0과 객체 높이 사이로 클램프
            float submergedHeight = Mathf.Clamp(surfaceLevel - objectBottom, 0f, objectHeight);

            if (submergedHeight > 0f)
            {
                // 잠긴 부피(V) 계산: 밑면적 × 잠긴 높이
                float submergedVolume = baseArea * submergedHeight;

                // 부력 크기 계산: e * V * g
                float buoyantForceMagnitude = liquidDensity * submergedVolume * Physics.gravity.magnitude;

                // 객체의 Downforce 가져오기 (있을 경우)
                float downforce = obj.Box.ChangeDownForce();


                // 부력에서 Downforce 차감
                buoyantForceMagnitude -= downforce;

                // 부력 적용 (Y축 방향)
                Vector3 buoyantForce = new Vector3(0f, buoyantForceMagnitude, 0f);
                objRb.AddForce(buoyantForce);

                // 감쇠력 적용 (선택 사항)
                Vector3 dampingForce = -objRb.velocity * dampingFactor;
                objRb.AddForce(dampingForce);
            }
        }


        void OnTriggerEnter(Collider other)
        {
            Rigidbody otherRb = other.attachedRigidbody;
            if (otherRb != null)
            {
                Box box = otherRb.GetComponent<Box>();
                objectsInLiquid.Add(new ObjectInLiquid(otherRb, other, box));
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
