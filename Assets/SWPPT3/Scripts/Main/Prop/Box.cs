using System;
using System.Collections.Generic;
using UnityEngine;

namespace SWPPT3.Main.Prop
{
    public class Box : StatelessProp
    {
        [SerializeField] private float rho;

        [SerializeField] private Rigidbody rb;
        [SerializeField] private BoxCollider collider;
        private readonly HashSet<Rigidbody> collidingRigidbodies = new HashSet<Rigidbody>();

        public float DownForce { get; set; }

        public float ChangeDownForce()
        {
            float totalDownForce = rb.mass;

            foreach (var otherRb in collidingRigidbodies)
            {
                float otherDownForce = otherRb.mass * Physics.gravity.magnitude;
                totalDownForce += otherDownForce;
            }

            return totalDownForce;
        }

        private void Awake()
        {
            float volume = collider.bounds.extents.x * collider.bounds.extents.y * collider.bounds.extents.z;
            rb.mass = volume * rho;
            DownForce = volume;
        }

        public void OnCollisionEnter(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (IsContactOnTopFace(contact.point))
                {
                    Rigidbody otherRb = collision.rigidbody;
                    if (otherRb != null && otherRb != rb)
                    {
                        collidingRigidbodies.Add(otherRb);
                        ChangeDownForce();
                    }
                }
            }
        }

        public void OnCollisionExit(Collision collision)
        {
            Rigidbody otherRb = collision.rigidbody;
            if (otherRb != null && otherRb != rb)
            {
                collidingRigidbodies.Remove(otherRb);
                ChangeDownForce();
            }
        }

        private bool IsContactOnTopFace(Vector3 contactPoint)
        {
            Vector3 localPoint = transform.InverseTransformPoint(contactPoint);
            Vector3 size = collider.size;

            float topFaceY = size.y / 2f;
            bool isOnTopFace = Mathf.Abs(localPoint.y - topFaceY) < 0.01f;
            bool isWithinX = Mathf.Abs(localPoint.x) <= size.x / 2f;
            bool isWithinZ = Mathf.Abs(localPoint.z) <= size.z / 2f;

            return isOnTopFace && isWithinX && isWithinZ;
        }
    }
}
