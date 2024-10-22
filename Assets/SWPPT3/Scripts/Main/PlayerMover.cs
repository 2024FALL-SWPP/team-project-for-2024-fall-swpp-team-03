
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace SWPPT3.Main
{
    /// <summary>
    ///     This is VERY sketchy class made for demo
    /// </summary>
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody _rb;

        private Vector2 _moveVector;

        private void Update()
        {
            var force = new Vector3(_moveVector.x, 0, _moveVector.y) * (3);
            _rb.AddForce(force, ForceMode.VelocityChange);
        }

        public void OnMove(InputAction.CallbackContext callbackContext)
        {
            var value = callbackContext.ReadValue<Vector2>();

            _moveVector = value;
        }
    }
}
