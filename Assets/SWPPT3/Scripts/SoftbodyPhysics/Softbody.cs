using System;
using System.Collections.Generic;
using System.IO;
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
        private Vector3 _newPosition;
        private Vector3 _fisrtPosition;

        public Vector3 FirstPosition{get => _fisrtPosition; set => _fisrtPosition = value;}
        public Vector3 Position{get => _position; set => _position = value;}
        public Vector3 NewPosition{get => _newPosition; set => _newPosition = value;}
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

             [HideInInspector, SerializeField]
             private int _connectedCount;

             private NativeArray<VertexWeightInfo> _vertexWeightsNa;
             private NativeArray<Vector3> _vertices;

             private NativeArray<Bone> _bonesBuffer1;
             private NativeArray<Bone> _bonesBuffer2;

             private NativeArray<Spring> _springsNa;
             private NativeArray<int> _springListNa;

             private NativeArray<RaycastCommand> _raycastCommands;
             private NativeArray<RaycastHit> _raycastHits;

             private TransformAccessArray _colliderTransforms;

             [SerializeField] private float _springStiffness = 0.4f;
             [SerializeField] private float _damping = 0.1f;

             [SerializeField] private int _pointDensityMultiplier = 5;
             [SerializeField] private int _totalLayers = 5;

             internal float ColliderRadius => _colliderRadius;

             private Vector3 _velocity;

             private float _elasticCoefficient;

             private bool _useBuffer1 = true;
             private void Start()
             {
                 if (_rigidbody == null)
                 {
                     _rigidbody = GetComponent<Rigidbody>();
                 }
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
                 for (int i = 0; i < _bones.Length; i++)
                 {
                     var bone = _bonesBuffer1[i];
                     bone.FirstPosition = bone.Position;
                     _bonesBuffer1[i] = _bonesBuffer2[i] = bone;
                 }

                 _springsNa = new NativeArray<Spring>(_springs.ToArray(), Allocator.Persistent);
                 _springListNa = new NativeArray<int>(_springList.ToArray(), Allocator.Persistent);

                 _raycastCommands = new NativeArray<RaycastCommand>(_bones.Length,Allocator.Persistent);
                 _raycastHits = new NativeArray<RaycastHit>(_bones.Length,Allocator.Persistent);

                 SetHasModifiableContacts(true);
             }


             private void FixedUpdate()
             {
                 var _boneRead = _useBuffer1 ? _bonesBuffer1 : _bonesBuffer2;
                 var _boneWrite = _useBuffer1 ? _bonesBuffer2 : _bonesBuffer1;

                 var localToWorldMatrix = _rigidbody.transform.localToWorldMatrix;

                 var boneSpringJob = new BoneSpringJob
                 {
                     BoneRead = _boneRead,
                     BoneWrite = _boneWrite,
                     SpringInfos = _springsNa,
                     SpringList = _springListNa,
                     DeltaTime = Time.deltaTime,
                     SpringStiffness = _springStiffness,
                     Damping = _damping,
                     BoneCount = _connectedCount,
                 };

                 var boneSpringHandle = boneSpringJob.Schedule(_boneRead.Length, 16);
                 boneSpringHandle.Complete();

                 var createRaycastJob = new CreateRaycastCommandsJob
                 {
                     BoneRead = _boneWrite,
                     RaycastCommands = _raycastCommands,
                     localToWorldMatrix = localToWorldMatrix,
                     Radius = _colliderRadius,
                 };
                 var createRaycastHandle = createRaycastJob.Schedule(_boneWrite.Length, 16,boneSpringHandle);

                 JobHandle raycastHandle = RaycastCommand.ScheduleBatch(_raycastCommands, _raycastHits, 1, createRaycastHandle);
                 raycastHandle.Complete();

                 var applyRaycastJob = new ApplyRaycastResultsJob
                 {
                     RaycastHits = _raycastHits,
                     BoneWrite = _boneWrite,
                     Radius = _colliderRadius,
                     LocalToWorldMatrix = localToWorldMatrix,
                     BoundaryRadius = 0.3f,
                 };

                 var applyRaycastHandle = applyRaycastJob.Schedule(_boneWrite.Length,16,  raycastHandle);
                 applyRaycastHandle.Complete();

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
                 if (_raycastCommands.IsCreated) _raycastCommands.Dispose();
                 if (_raycastHits.IsCreated) _raycastHits.Dispose();
                 _colliderTransforms.Dispose();
             }

             private void OnEnable()
             {
                 Physics.ContactModifyEvent += ModificationEvent;
             }

             private void OnDisable()
             {
                 Physics.ContactModifyEvent -= ModificationEvent;
             }

             public void SetHasModifiableContacts(bool enabled)
             {
                 if (_outerColliders == null) return;

                 for (int i = 0; i < _outerColliders.Length; i++)
                 {
                     if (_outerColliders[i] != null)
                     {
                         _outerColliders[i].hasModifiableContacts = enabled;
                     }
                 }
             }

             public void ModificationEvent(PhysicsScene scene, NativeArray<ModifiableContactPair> pairs)
             {
                 for (int i = 0; i < pairs.Length; i++)
                 {
                     var pair = pairs[i];

                     var properties = pair.massProperties;
                     properties.inverseMassScale = 1f;
                     properties.inverseInertiaScale = 1f;
                     properties.otherInverseMassScale = 0f;
                     properties.otherInverseInertiaScale = 0f;

                     pair.massProperties = properties;

                     pairs[i] = pair;
                 }
             }

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
                     // Vector3 force = Vector3.zero;
                     Vector3 force = new Vector3(0f, -BoneRead[index].Mass * 1f, 0f);

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
                             // Debug.Log($"i: {index} - bone1: {bone1} - bone2: {bone2} - force: {springForce} restLength {restLength} currentLength {currentLength} ");
                         }
                     }

                     velocity += force * DeltaTime;
                     velocity *= Damping;

                     var newPosition = position + velocity * DeltaTime;


                     if(index == 15) Debug.Log($"{index} v: {velocity} f: {force} np: {newPosition} p: {position.ToString("F3")}");

                     Bone bone = BoneWrite[index];
                     bone.Position = position;
                     bone.NewPosition = newPosition;
                     bone.Velocity = velocity;
                     BoneWrite[index] = bone;
                 }
             }

             [BurstCompile]
             private struct CreateRaycastCommandsJob : IJobParallelFor
             {
                 [ReadOnly] public NativeArray<Bone> BoneRead;
                 public NativeArray<RaycastCommand> RaycastCommands;
                 public Matrix4x4 localToWorldMatrix;
                 public float Radius;

                 public void Execute(int index)
                 {
                     Vector3 globalPosition = localToWorldMatrix.MultiplyPoint(BoneRead[index].Position);
                     Vector3 globalNewPosition = localToWorldMatrix.MultiplyPoint(BoneRead[index].NewPosition);
                     Vector3 direction = globalNewPosition - globalPosition;
                     float distance = direction.magnitude;
                     RaycastCommands[index] = new RaycastCommand(globalPosition, direction.normalized, distance + 4 * Radius);
                 }
             }

             [BurstCompile]
             private struct ApplyRaycastResultsJob : IJobParallelFor
             {
                 [ReadOnly] public NativeArray<RaycastHit> RaycastHits;
                 public NativeArray<Bone> BoneWrite;
                 public float Radius;
                 public float BoundaryRadius;
                 public Matrix4x4 LocalToWorldMatrix;

                 private static float _threshold = 0.1f;

                 public void Execute(int index)
                 {
                     var bone = BoneWrite[index];
                     Vector3 newPosition = bone.NewPosition;
                     bool zeroVel = false;
                     bool hasHit = RaycastHits[index].normal != Vector3.zero;

                     if (hasHit)
                     {

                         Vector3 hitPoint = RaycastHits[index].point;
                         Vector3 hitNormal = RaycastHits[index].normal;
                         Vector3 globalPosition = LocalToWorldMatrix.MultiplyPoint(newPosition);
                         Vector3 direction = (globalPosition - hitPoint).normalized;
                         float distance = Vector3.Distance(globalPosition, hitPoint);

                         float cosTheta = Vector3.Dot(direction, hitNormal);

                         float secTheta = 1 / cosTheta;

                         float height = distance * cosTheta;

                         // Debug.Log($"{index} Ray cast hit point: {hitPoint} normal : {hitNormal} distance: {distance} height: {height} cosTheta: {cosTheta}");

                         if (height <= Radius)
                         {
                             zeroVel = true;
                             Vector3 adjustedPosition = hitPoint + direction * Radius * secTheta;
                             newPosition = LocalToWorldMatrix.inverse.MultiplyPoint(adjustedPosition);
                             // Debug.Log($"{index} Ray cast close global point: {adjustedPosition}");
                         }
                     }

                     Vector3 firstPosition = bone.FirstPosition;
                     Vector3 offsetFromFirst = newPosition - firstPosition;
                     float distFromFirst = offsetFromFirst.magnitude;
                     if (distFromFirst > BoundaryRadius)
                     {
                         zeroVel = true;
                         offsetFromFirst = offsetFromFirst.normalized * BoundaryRadius;
                         newPosition = firstPosition + offsetFromFirst;
                     }

                     float positionDelta = (newPosition - bone.Position).magnitude;
                     if (positionDelta > _threshold)
                     {
                         // Debug.Log("change");
                         bone.Position = newPosition;
                     }
                     else
                     {
                         // zeroVel = true;
                     }
                     if(index == 15)  Debug.Log($"{index} first position: {firstPosition} -> new Position: {newPosition} velocity: {bone.Velocity}");
                     bone.Velocity = zeroVel ? Vector3.zero : bone.Velocity;
                     BoneWrite[index] = bone;
                     // Debug.Log($"global raycast  {index} {newPosition} ");
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
                     if(index < outerIndexStart)
                         transform.localPosition = BoneRead[index + outerIndexStart].Position;
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
