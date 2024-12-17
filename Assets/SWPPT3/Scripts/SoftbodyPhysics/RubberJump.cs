using UnityEngine;

namespace SWPPT3.SoftbodyPhysics
{
    public class RubberJump : MonoBehaviour
    {
        private Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            // foreach (ContactPoint contact in collision.contacts)
            // {
            //     if (contact.normal.y >= 0.7f)
            //     {
            //         Vector3 jumpForce = Vector3.up * (_rb.mass * 30f);
            //         _rb.AddForce(jumpForce, ForceMode.Impulse);
            //
            //         Debug.Log($"Applied Jump Force: {jumpForce} at Contact Point Normal: {contact.normal}");
            //         break; // 하나라도 조건 만족하면 탈출
            //     }
            // }
        }
    }
}
