using System;
using UnityEngine;

namespace SWPPT3.SoftbodyPhysics
{
    public class RubberJump : MonoBehaviour
    {
        private Rigidbody _rb;

        public SoftbodyGenerator _softbody;

        public float jumpForce;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _active = false;
        }

        private bool _active;

        public void SetActive(bool active)
        {
            _active = active;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_active)
            {
                foreach (ContactPoint contact in collision.contacts)
                {
                    if (contact.normal.y >= 0.7f)
                    {
                        Vector3 force = Vector3.up * (_rb.mass * jumpForce);
                        _rb.AddForce(force, ForceMode.Impulse);

                        break; // 하나라도 조건 만족하면 탈출
                    }
                }
            }
            _softbody.CollisionEnter(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            _softbody.CollisionStay(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            _softbody.CollisionExit(collision);
        }

        private void OnTriggerEnter(Collider other)
        {
            _softbody.TriggerEnter(other);
        }

        private void OnTriggerStay(Collider other)
        {
            _softbody.TriggerStay(other);
        }
    }
}
