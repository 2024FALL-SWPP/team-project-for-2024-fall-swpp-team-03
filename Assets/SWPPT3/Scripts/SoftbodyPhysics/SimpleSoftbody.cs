using System.Collections.Generic;
using UnityEngine;

namespace SWPPT3.SoftbodyPhysics
{
    public class SimpleSoftbody : MonoBehaviour
    {
        private SoftbodyParticleArraySet _particleArraySet;

        internal SoftbodyParticleArraySet ParticleArraySet => _particleArraySet;


    #region Unity Events
        private void Awake()
        {
        }

        private void FixedUpdate()
        {
            SoftbodySolver.SolveFixedStep();
        }
    #endregion
    }
}
