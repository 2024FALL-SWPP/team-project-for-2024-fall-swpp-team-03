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
    }

    [Serializable]
    internal struct Spring
    {
        [SerializeField]
        private float restLength;
        [SerializeField]
        private int _bone1;
        [SerializeField]
        private int _bone2;

        public float RestLength => restLength;
        public int Bone1 => _bone1;
        public int Bone2 => _bone2;

        public Spring(float restLength, int bone1, int bone2)
        {
            this.restLength = restLength;
            this._bone1 = bone1;
            this._bone2 = bone2;
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

             [HideInInspector, SerializeField]
             private List<Spring> _springs;

             [HideInInspector, SerializeField]
             private List<int> _springList;

             private NativeArray<VertexWeightInfo> _vertexWeightsNa;
             private NativeArray<Vector3> _vertices;
             private NativeArray<Vector3> _bonePositionsReadOnly;
             private NativeArray<Vector3> _bonePositionsWrite;
             private NativeArray<Spring> _springsNa;
             private NativeArray<int> _springListNa;

             private TransformAccessArray _colliderTransforms;



             private NativeArray<Vector3> _boneVelocities;

             [SerializeField] private float _springStiffness = 1f;
             [SerializeField] private float _damping = 0.5f;

             [SerializeField] private int _pointDensityMultiplier = 5;
             [SerializeField] private int _totalLayers = 5;

             internal float ColliderRadius => _colliderRadius;

             private Vector3 _velocity;

             private float _elasticCoefficient;

             private void Start()
             {
                 Debug.Log(_springs[0].Bone1);
                 _colliderTransforms = new TransformAccessArray(_colliders.Length);
                 _boneVelocities = new NativeArray<Vector3>(_colliders.Length, Allocator.Persistent);
                 foreach (var collider in _colliders)
                 {
                     _colliderTransforms.Add(collider.transform);
                 }
                 var list = new List<Vector3>();
                 _meshFilter.mesh.GetVertices(list);

                 _vertices = new NativeArray<Vector3>(list.ToArray(), Allocator.Persistent);
                 _vertexWeightsNa = new NativeArray<VertexWeightInfo>(_vertexWeights.ToArray(), Allocator.Persistent);
                 _bonePositionsReadOnly = new NativeArray<Vector3>(_colliders.Length, Allocator.Persistent);
                 _bonePositionsWrite = new NativeArray<Vector3>(_colliders.Length, Allocator.Persistent);
                 _springsNa = new NativeArray<Spring>(_springs.ToArray(), Allocator.Persistent);
                 _springListNa = new NativeArray<int>(_springList.ToArray(), Allocator.Persistent);
                 Debug.Log(_springList.Count);
             }

             private void FixedUpdate()
             {
                 var boneUpdateJob = new BonePositionsUpdateJob
                 {
                     BonePositionsReadOnly = _bonePositionsReadOnly
                 };

                 var boneUpdateHandle = boneUpdateJob.Schedule(_colliderTransforms);
                 boneUpdateHandle.Complete();

                 var boneSpringJob = new BoneSpringJob
                 {
                     BonePositionsReadOnly = _bonePositionsReadOnly,
                     BonePositionsWrite = _bonePositionsWrite,
                     BoneVelocities = _boneVelocities,
                     SpringInfos = _springsNa,
                     SpringList = _springListNa,
                     DeltaTime = Time.deltaTime,
                     SpringStiffness = _springStiffness,
                     Damping = _damping
                 };
                 var boneSpringHandle = boneSpringJob.Schedule(_bonePositionsReadOnly.Length, 16, boneUpdateHandle);
                 boneSpringHandle.Complete();

                 var applyBonePositionsJob = new ApplyBonePositionsJob
                 {
                     BonePositionsReadOnly = _bonePositionsReadOnly,
                     BonePositionsWrite = _bonePositionsWrite

                 };
                 var applyBonePositionsHandle = applyBonePositionsJob.Schedule(_colliderTransforms, boneSpringHandle);
                 applyBonePositionsHandle.Complete();


                 var vertexUpdateJob = new VertexUpdateJob
                 {
                     Vertices = _vertices,
                     VertexWeights = _vertexWeightsNa,
                     BonePositions = _bonePositionsReadOnly
                 };

                 var vertexUpdateHandle = vertexUpdateJob.Schedule(_vertices.Length, 16, applyBonePositionsHandle);
                 vertexUpdateHandle.Complete();

                 _meshFilter.mesh.SetVertices(_vertices);
             }



             private void OnDestroy()
             {
                 if (_vertices.IsCreated) _vertices.Dispose();
                 if (_vertexWeightsNa.IsCreated) _vertexWeightsNa.Dispose();
                 if (_bonePositionsReadOnly.IsCreated) _bonePositionsReadOnly.Dispose();
                 if (_bonePositionsWrite.IsCreated) _bonePositionsWrite.Dispose();
                 if (_boneVelocities.IsCreated) _boneVelocities.Dispose();
                 if (_springsNa.IsCreated) _springsNa.Dispose();
                 if (_springListNa.IsCreated) _springListNa.Dispose();
                 _colliderTransforms.Dispose();
             }

             [BurstCompile]
             private struct BonePositionsUpdateJob : IJobParallelForTransform
             {
                 public NativeArray<Vector3> BonePositionsReadOnly;

                 public void Execute(int index, TransformAccess transform)
                 {
                     BonePositionsReadOnly[index] = transform.localPosition;
                 }
             }

             [BurstCompile]
             private struct BoneSpringJob : IJobParallelFor
             {

                 public NativeArray<Vector3> BonePositionsWrite;
                 public NativeArray<Vector3> BoneVelocities;
                 [ReadOnly] public NativeArray<Spring> SpringInfos;
                 [ReadOnly] public NativeArray<int> SpringList;
                 [ReadOnly] public NativeArray<Vector3> BonePositionsReadOnly;

                 public float DeltaTime;
                 public float SpringStiffness;
                 public float Damping;

                 public int ColliderLength;

                 public void Execute(int index)
                 {
                     Vector3 position = BonePositionsReadOnly[index];
                     Vector3 velocity = BoneVelocities[index];
                     Vector3 force = Vector3.zero;

                     var start = index * ColliderLength;
                     var end = start + ColliderLength;

                     for (int i = start; i < end; i++)
                     {
                         var springIndex = SpringList[i];
                         if (springIndex != -1)
                         {
                             Spring spring = SpringInfos[springIndex];
                             var bone1 = spring.Bone1;
                             var bone2  = spring.Bone2;
                             var restLength = spring.RestLength;

                             Vector3 pos1 = BonePositionsReadOnly[bone1];
                             Vector3 pos2 = BonePositionsReadOnly[bone2];

                             Vector3 springVector = pos2 - pos1;
                             float currentLength = springVector.magnitude;
                             Vector3 springForce = SpringStiffness * (currentLength - restLength) * springVector.normalized;

                             if (index == bone1)
                             {
                                 force += springForce;
                             }
                             else if (index == bone2)
                             {
                                 force -= springForce;
                             }
                         }
                     }

                     velocity += force * DeltaTime;
                     velocity *= Damping;

                     position += velocity * DeltaTime;

                     BoneVelocities[index] = velocity;
                     BonePositionsWrite[index] = position;
                 }
             }

             [BurstCompile]
             private struct ApplyBonePositionsJob : IJobParallelForTransform
             {
                 [ReadOnly] public NativeArray<Vector3> BonePositionsWrite;
                 public NativeArray<Vector3> BonePositionsReadOnly;

                 public void Execute(int index, TransformAccess transform)
                 {
                     transform.localPosition = BonePositionsWrite[index];
                     BonePositionsReadOnly[index] = transform.position;
                 }
             }

             [BurstCompile]
             private struct VertexUpdateJob : IJobParallelFor
             {
                 public NativeArray<Vector3> Vertices;
                 [ReadOnly] public NativeArray<VertexWeightInfo> VertexWeights;
                 [ReadOnly] public NativeArray<Vector3> BonePositions;

                 public void Execute(int index)
                 {
                     var boneInfo = VertexWeights[index];
                     var v = Vector3.zero;

                     for (var j = 0; j < boneInfo.BoneCount; j++)
                     {
                         var weightInfo = boneInfo[j];
                         var bonePosition = BonePositions[weightInfo.BoneIndex];
                         var boneDesiredVertex = bonePosition + weightInfo.Offset;
                         v += boneDesiredVertex * weightInfo.Weight;
                     }

                     Vertices[index] = v;
                 }
             }

         }
}
