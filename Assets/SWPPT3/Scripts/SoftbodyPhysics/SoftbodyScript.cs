#region

using UnityEngine;

#endregion

namespace SWPPT3.SoftbodyPhysics
{
    [CreateAssetMenu(fileName = "SoftbodyScript", menuName = "SWPPT3/Scripts/SoftbodyScript")]
    public class SoftbodyScript : ScriptableObject
    {
        [SerializeField] private float mass;
        [SerializeField] private float physicsRoughness;
        [SerializeField] private float softness;
        [SerializeField] private float damp;

        [SerializeField] private float rubberJump;

        [SerializeField] private float resetSec;
        // [SerializeField] private float collissionSurfaceOffset;

        public float Mass {get => mass; set => mass = value; }
        public float PhysicsRoughness => physicsRoughness;
        public float Softness => softness;
        public float Damp => damp;
        public float RubberJump => rubberJump;
        public float ResetSec => resetSec;


    }
}
