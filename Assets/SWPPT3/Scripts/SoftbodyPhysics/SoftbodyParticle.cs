using System;
using System.Numerics;
using Unity.Collections;

namespace SWPPT3.SoftbodyPhysics
{
    [Serializable]
    internal struct SoftbodyParticleArraySet
    {
        public int Count { get; set; }

        public NativeArray<Vector3> Positions { get; set; }
        public NativeArray<Vector3> RestPositions { get; set; }

        public NativeArray<Quaternion> Orientations { get; set; }
        public NativeArray<Quaternion> RestOrientations { get; set; }

        public NativeArray<Vector3> Velocities { get; set; }
        public NativeArray<Quaternion> AngularVelocities { get; set; }

        public NativeArray<float> InvMasses { get; set; }
        public NativeArray<float> AngularInvMasses { get; set; }
    }
}
