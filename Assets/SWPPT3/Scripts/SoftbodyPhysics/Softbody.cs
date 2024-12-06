using System;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField]
        private int _boneCount;

        public int BoneCount { get => _boneCount; set => _boneCount = value; }

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
            _boneCount = boneCount;

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

    [Serializable]
    internal struct Bone
    {
        [SerializeField]  private Vector3 _position;
        [SerializeField] private Vector3 _velocity;
        [SerializeField] private float _mass;

        public Vector3 Position{get => _position; set => _position = value;}
        public Vector3 Velocity { get => _velocity; set => _velocity = value; }
        public float Mass { get => _mass; set => _mass = value; }

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
             private Collider[] _outerColliders;

             [HideInInspector, SerializeField]
             private Bone[] _bones;

             [HideInInspector, SerializeField]
             private List<VertexWeightInfo> _vertexWeights;

             [HideInInspector, SerializeField]
             private List<Spring> _springs;

             [HideInInspector, SerializeField]
             private List<int> _springList;

             [HideInInspector, SerializeField]
             private int _outerIndexStart;

             private NativeArray<VertexWeightInfo> _vertexWeightsNa;
             private NativeArray<Vector3> _vertices;

             private NativeArray<Bone> _bonesBuffer1;
             private NativeArray<Bone> _bonesBuffer2;

             private NativeArray<Spring> _springsNa;
             private NativeArray<int> _springListNa;

             private TransformAccessArray _colliderTransforms;

             [SerializeField] private float _springStiffness = 10f;
             [SerializeField] private float _damping = 2f;

             [SerializeField] private int _pointDensityMultiplier = 5;
             [SerializeField] private int _totalLayers = 5;

             internal float ColliderRadius => _colliderRadius;

             private Vector3 _velocity;

             private float _elasticCoefficient;

             private bool _useBuffer1 = true;
             private void Start()
             {
                 Debug.Log(_springs[0].Bone1);
                 _colliderTransforms = new TransformAccessArray(_outerColliders.Length);
                 foreach (var collider in _outerColliders)
                 {
                     _colliderTransforms.Add(collider.transform);
                 }
                 var list = new List<Vector3>();
                 _meshFilter.mesh.GetVertices(list);

                 _vertices = new NativeArray<Vector3>(list.ToArray(), Allocator.Persistent);
                 _vertexWeightsNa = new NativeArray<VertexWeightInfo>(_vertexWeights.ToArray(), Allocator.Persistent);

                 _bonesBuffer1 = new NativeArray<Bone>(_bones.ToArray(), Allocator.Persistent);
                 _bonesBuffer2 = new NativeArray<Bone>(_bones.ToArray(), Allocator.Persistent);

                 _springsNa = new NativeArray<Spring>(_springs.ToArray(), Allocator.Persistent);
                 _springListNa = new NativeArray<int>(_springList.ToArray(), Allocator.Persistent);
                 Debug.Log(_springList.Count);
             }


             private void FixedUpdate()
             {
                 var _boneRead = _useBuffer1 ? _bonesBuffer1 : _bonesBuffer2;
                 var _boneWrite = _useBuffer1 ? _bonesBuffer2 : _bonesBuffer1;

                 var boneSpringJob = new BoneSpringJob
                 {
                     BoneRead = _boneRead,
                     BoneWrite = _boneWrite,
                     SpringInfos = _springsNa,
                     SpringList = _springListNa,
                     DeltaTime = Time.deltaTime,
                     SpringStiffness = _springStiffness,
                     Damping = _damping,
                     BoneCount = _bones.Length,
                 };
                 var boneSpringHandle = boneSpringJob.Schedule(_boneRead.Length, 16);
                 boneSpringHandle.Complete();

                 var applyBonePositionsJob = new ApplyBonePositionsJob
                 {
                     BoneRead = _boneWrite,
                     BoneWrite = _boneRead,
                     outerIndexStart = _outerIndexStart

                 };
                 var applyBonePositionsHandle = applyBonePositionsJob.Schedule(_colliderTransforms, boneSpringHandle);
                 applyBonePositionsHandle.Complete();
                 _useBuffer1 = !_useBuffer1;
             }

             private void Update()
             {
                 var vertexUpdateJob = new VertexUpdateJob
                 {
                     Vertices = _vertices,
                     VertexWeights = _vertexWeightsNa,
                     BoneRead = _useBuffer1 ? _bonesBuffer1 : _bonesBuffer2
                 };

                 var vertexUpdateHandle = vertexUpdateJob.Schedule(_vertices.Length, 16);
                 vertexUpdateHandle.Complete();

                 _meshFilter.mesh.SetVertices(_vertices);
             }

             private void OnDestroy()
             {
                 if (_vertices.IsCreated) _vertices.Dispose();
                 if (_vertexWeightsNa.IsCreated) _vertexWeightsNa.Dispose();
                 if (_bonesBuffer1.IsCreated) _bonesBuffer1.Dispose();
                 if (_bonesBuffer2.IsCreated) _bonesBuffer2.Dispose();
                 if (_springsNa.IsCreated) _springsNa.Dispose();
                 if (_springListNa.IsCreated) _springListNa.Dispose();
                 _colliderTransforms.Dispose();
             }

             // [BurstCompile]
             // private struct BonePositionsUpdateJob : IJobParallelFor
             // {
             //     [ReadOnly] public NativeArray<Bone> BoneRead;
             //     public NativeArray<Bone> BoneWrite;
             //
             //     public void Execute(int index)
             //     {
             //         BoneWrite[index] = BoneRead[index];
             //     }
             // }

             [BurstCompile]
             private struct BoneSpringJob : IJobParallelFor
             {

                 public NativeArray<Bone> BoneWrite;
                 [ReadOnly] public NativeArray<Spring> SpringInfos;
                 [ReadOnly] public NativeArray<int> SpringList;
                 [ReadOnly] public NativeArray<Bone> BoneRead;

                 public float DeltaTime;
                 public float SpringStiffness;
                 public float Damping;

                 public int BoneCount;

                 public void Execute(int index)
                 {
                     Vector3 position = BoneRead[index].Position;
                     Vector3 velocity = BoneRead[index].Velocity;
                     Vector3 force = Vector3.zero;

                     var start = index * BoneCount;
                     var end = start + BoneCount;

                     for (int i = start; i < end; i++)
                     {
                         var springIndex = SpringList[i];
                         if (springIndex != -1)
                         {
                             Spring spring = SpringInfos[springIndex];
                             var bone1 = spring.Bone1;
                             var bone2  = spring.Bone2;
                             var restLength = spring.RestLength;

                             Vector3 pos1 = BoneRead[bone1].Position;
                             Vector3 pos2 = BoneRead[bone2].Position;

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

                     Debug.Log($"{index} {velocity} {force},");

                     BoneWrite[index] = new Bone
                     {
                         Position = position,
                         Velocity = velocity,
                         Mass = BoneRead[index].Mass,
                     };
                 }
             }

             [BurstCompile]
             private struct ApplyBonePositionsJob : IJobParallelForTransform
             {
                 [ReadOnly] public NativeArray<Bone> BoneRead;
                 [ReadOnly] public int outerIndexStart;
                 public NativeArray<Bone> BoneWrite;

                 public void Execute(int index, TransformAccess transform)
                 {
                     BoneWrite[index] = BoneRead[index];
                     if(index >= outerIndexStart)
                         transform.localPosition = BoneRead[index - outerIndexStart].Position;
                 }
             }

             [BurstCompile]
             private struct VertexUpdateJob : IJobParallelFor
             {
                 public NativeArray<Vector3> Vertices;
                 [ReadOnly] public NativeArray<VertexWeightInfo> VertexWeights;
                 [ReadOnly] public NativeArray<Bone> BoneRead;

                 public void Execute(int index)
                 {
                     var boneInfo = VertexWeights[index];
                     var v = Vector3.zero;

                     for (var j = 0; j < boneInfo.BoneCount; j++)
                     {
                         var weightInfo = boneInfo[j];
                         var bonePosition = BoneRead[weightInfo.BoneIndex].Position;
                         var boneDesiredVertex = bonePosition + weightInfo.Offset;
                         v += boneDesiredVertex * weightInfo.Weight;
                     }

                     Vertices[index] = v;
                 }
             }

         }
}
