using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using UnityEngine;
using UnityEngine.Jobs;

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
        public override string ToString()
        {
            return $"BoneIndex: {_boneIndex}, Weight: {_weight}, Offset: {_offset}";
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
                    4 => _b4,
                    5 => _b5,
                    6 => _b6,
                    7 => _b7,
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
                    4 => _b4 = value,
                    5 => _b5 = value,
                    6 => _b6 = value,
                    7 => _b7 = value,
                    _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
                };
            }
        }

        public int BoneCount { get; private set; }

        [SerializeField] private BoneWeightInfo _b0;
        [SerializeField] private BoneWeightInfo _b1;
        [SerializeField] private BoneWeightInfo _b2;
        [SerializeField] private BoneWeightInfo _b3;
        [SerializeField] private BoneWeightInfo _b4;
        [SerializeField] private BoneWeightInfo _b5;
        [SerializeField] private BoneWeightInfo _b6;
        [SerializeField] private BoneWeightInfo _b7;

        public VertexWeightInfo(int boneCount)
        {
            if (boneCount < 0 || boneCount > 8)
            {
                throw new ArgumentOutOfRangeException(nameof(boneCount), "BoneCount must be between 0 and 8.");
            }

            BoneCount = boneCount;

            _b0 = default;
            _b1 = default;
            _b2 = default;
            _b3 = default;
            _b4 = default;
            _b5 = default;
            _b6 = default;
            _b7 = default;
        }
        public override string ToString()
        {
            return $"B0: {_b0}, B1: {_b1}, B2: {_b2}, B3: {_b3}";
        }
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

        private NativeArray<VertexWeightInfo> _vertexWeightsNa;
        private NativeArray<Vector3> _vertices;
        private NativeArray<Vector3> _bonePositions;
        private TransformAccessArray _colliderTransforms;

        [SerializeField] private int _pointDensityMultiplier = 5;
        [SerializeField] private int _totalLayers = 5;

        [HideInInspector, SerializeField]
        private int _boneCount;

        internal float ColliderRadius => _colliderRadius;

        private Vector3 _velocity;

        private float _elasticCoefficient;

        private void Start()
        {
            _colliderTransforms = new TransformAccessArray(_colliders.Length);
            foreach (var collider in _colliders)
            {
                _colliderTransforms.Add(collider.transform);
            }
            var list = new List<Vector3>();
            _meshFilter.mesh.GetVertices(list);

            _vertices = new NativeArray<Vector3>(list.ToArray(), Allocator.Persistent);
            _vertexWeightsNa = new NativeArray<VertexWeightInfo>(_vertexWeights.ToArray(), Allocator.Persistent);
            _bonePositions = new NativeArray<Vector3>(_colliders.Length, Allocator.Persistent);
        }

        private void Update()
        {
            var boneUpdateJob = new BonePositionsUpdateJob
            {
                BonePositions = _bonePositions
            };

            var boneUpdateHandle = boneUpdateJob.Schedule(_colliderTransforms);
            boneUpdateHandle.Complete();

            var vertexUpdateJob = new VertexUpdateJob
            {
                Vertices = _vertices,
                VertexWeights = _vertexWeightsNa,
                BoneCount = _boneCount,
                BonePositions = _bonePositions
            };

            var vertexUpdateHandle = vertexUpdateJob.Schedule(_vertices.Length, 16, boneUpdateHandle);

            vertexUpdateHandle.Complete();

            _meshFilter.mesh.SetVertices(_vertices);
        }


        private void OnDestroy()
        {
            if (_vertices.IsCreated) _vertices.Dispose();
            if (_vertexWeightsNa.IsCreated) _vertexWeightsNa.Dispose();
            if (_bonePositions.IsCreated) _bonePositions.Dispose();
            _colliderTransforms.Dispose();
        }

        // [BurstCompile]
        private struct BonePositionsUpdateJob : IJobParallelForTransform
        {
            public NativeArray<Vector3> BonePositions;

            public void Execute(int index, TransformAccess transform)
            {
                BonePositions[index] = transform.position;
            }
        }


        // [BurstCompile]
        private struct VertexUpdateJob : IJobParallelFor
        {
            public NativeArray<Vector3> Vertices;
            [ReadOnly] public NativeArray<VertexWeightInfo> VertexWeights;
            [ReadOnly] public int BoneCount;
            [ReadOnly] public NativeArray<Vector3> BonePositions;

            public void Execute(int index)
            {
                var boneInfo = VertexWeights[index];
                var v = Vector3.zero;

                for (var j = 0; j < BoneCount; j++)
                {
                    var weightInfo = boneInfo[j];
                    Debug.Log(weightInfo.BoneIndex);
                    var bonePosition = BonePositions[weightInfo.BoneIndex];
                    var boneDesiredVertex = bonePosition + weightInfo.Offset;
                    v += boneDesiredVertex * weightInfo.Weight;
                }

                Vertices[index] = v;
            }
        }

    }
}
