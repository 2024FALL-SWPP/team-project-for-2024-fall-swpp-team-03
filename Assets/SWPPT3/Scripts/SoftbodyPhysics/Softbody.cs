using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace SWPPT3.SoftbodyPhysics
{
    [Serializable]
    internal struct BoneWeightInfo
    {
        [SerializeField]
        private int _boneIndex;

        public int BoneIndex
        {
            get => _boneIndex;
            set => _boneIndex = value;
        }

        [SerializeField]
        private float _weight;

        public float Weight
        {
            get => _weight;
            set => _weight = value;
        }

        [SerializeField]
        private Vector3 _offset;

        public Vector3 Offset
        {
            get => _offset;
            set => _offset = value;
        }
    }

    [Serializable]
    internal struct VertexWeightInfo
    {
        public BoneWeightInfo this[int index]
        {
            get
            {
                return index switch
                {
                    0 => _b0,
                    1 => _b1,
                    2 => _b2,
                    3 => _b3,
                    _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
                };
            }

            set
            {
                _ = index switch
                {
                    0 => _b0 = value,
                    1 => _b1 = value,
                    2 => _b2 = value,
                    3 => _b3 = value,
                    _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
                };
            }
        }

        [SerializeField]
        private BoneWeightInfo _b0;

        [SerializeField]
        private BoneWeightInfo _b1;

        [SerializeField]
        private BoneWeightInfo _b2;

        [SerializeField]
        private BoneWeightInfo _b3;
    }

    public class Softbody : MonoBehaviour
    {
        internal const float SphereRadius = 0.5f;

        [SerializeField]
        private MeshFilter _meshFilter;

        [SerializeField]
        private Transform _collidersRoot;

        [SerializeField]
        private Rigidbody _rigidbody;

        [SerializeField]
        private float _colliderRadius = 0.05f;

        [HideInInspector, SerializeField]
        private Collider[] _colliders;

        [HideInInspector, SerializeField]
        private List<VertexWeightInfo> _vertexWeights;

        [SerializeField] private int _pointDensityMultiplier = 5;
        [SerializeField] private int _totalLayers = 5;

        internal float ColliderRadius => _colliderRadius;

        private Vector3 _velocity;

        private float _elasticCoefficient;

        private NativeArray<Vector3> _vertices;

        private void Start()
        {
            var list = new List<Vector3>();
            _meshFilter.mesh.GetVertices(list);

            _vertices = new NativeArray<Vector3>(list.ToArray(), Allocator.Persistent);
        }

        private void OnDestroy()
        {
            _vertices.Dispose();
        }

        private void Update()
        {
            // for (var i = 0; i < _vertices.Length; i++)
            // {
            //     var boneInfo = _vertexWeights[i];
            //
            //     var v = Vector3.zero;
            //
            //     for (var j = 0; j < 4; j++)
            //     {
            //         var weightInfo = boneInfo[j];
            //         var boneDesiredVertex =
            //             _colliders[weightInfo.BoneIndex].transform.localPosition
            //             + weightInfo.Offset;
            //
            //         v += boneDesiredVertex * weightInfo.Weight;
            //     }
            //
            //     _vertices[i] = v;
            // }
            //
            // _meshFilter.mesh.SetVertices(_vertices);
        }
    }
}
