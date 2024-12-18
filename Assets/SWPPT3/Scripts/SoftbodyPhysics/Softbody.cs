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
        private Rigidbody _centerRb;

        private Rigidbody[] _rigidbodies;

        private float _colliderRadius = 0.02f;

        [HideInInspector, SerializeField]
        private GameObject[] _bones;

        [HideInInspector, SerializeField]
        private List<VertexWeightInfo> _vertexWeights;

        [HideInInspector, SerializeField]
        private List<Spring> _springs;

        [HideInInspector, SerializeField]
        private List<int> _springList;

        [HideInInspector, SerializeField]
        private int _connectedCount;

        // [HideInInspector, SerializeField]
        // private GameObject _centerObj;

        private NativeArray<VertexWeightInfo> _vertexWeightsNa;
        private NativeArray<Vector3> _verticesNa;

        private NativeArray<Spring> _springsNa;
        private NativeArray<int> _springListNa;

        private NativeArray<Vector3> _boneForcesNa;
        private NativeArray<Vector3>  _centerForcesNa;
        private NativeArray<Vector3> _bonePositionsNa;

        private int _connectedTriCount;
        private int _triCount;
        private NativeArray<int> _trianglesNa;
        private NativeArray<int> _triangleInfoNa;
        private NativeArray<Vector3> _faceNormalsNa;
        private NativeArray<Vector3> _vertexNormalsNa;


        private Matrix4x4 _worldToLocalMatrix;

        private TransformAccessArray _boneTransforms;

        [SerializeField] private float _springStiffness = 100f;
        [SerializeField] private float _damping = 0.9f;

        private int _boneCount;
        internal float ColliderRadius => _colliderRadius;

        private void Start()
        {
            _rigidbodies = new Rigidbody[_bones.Length];
            for (int i = 0; i < _rigidbodies.Length; i++)
            {
             _rigidbodies[i] = _bones[i].GetComponent<Rigidbody>();
            }
            Debug.Log(_bones.Length);
            _boneTransforms = new TransformAccessArray(_bones.Length+1);
            foreach (var bone in _bones)
            {
             _boneTransforms.Add(bone.transform);
            }
            _boneTransforms.Add(_centerRb.transform);
            var list = new List<Vector3>();
            _meshFilter.mesh.GetVertices(list);

            _verticesNa = new NativeArray<Vector3>(list.ToArray(), Allocator.Persistent);
            _vertexWeightsNa = new NativeArray<VertexWeightInfo>(_vertexWeights.ToArray(), Allocator.Persistent);

            _springsNa = new NativeArray<Spring>(_springs.ToArray(), Allocator.Persistent);
            _springListNa = new NativeArray<int>(_springList.ToArray(), Allocator.Persistent);

            _boneForcesNa = new NativeArray<Vector3>(_bones.Length, Allocator.Persistent);
            _centerForcesNa = new NativeArray<Vector3>(_bones.Length, Allocator.Persistent);

            _bonePositionsNa = new NativeArray<Vector3>(_bones.Length + 1, Allocator.Persistent);
            _boneCount = _bones.Length;

            var tri = _meshFilter.mesh.triangles;
            _trianglesNa = new NativeArray<int>(tri.ToArray(), Allocator.Persistent);
            _triangleInfoNa = BuildTriangleInfo(_trianglesNa, _verticesNa.Length, out _triCount, out _connectedTriCount);

            _faceNormalsNa = new NativeArray<Vector3>(_triCount, Allocator.Persistent);
            _vertexNormalsNa = new NativeArray<Vector3>(_verticesNa.Length, Allocator.Persistent);
            Debug.Log($"{_triCount}");
            SetHasModifiableContacts(true);
        }
        private void FixedUpdate()
        {
            var getBonePositionJob = new GetBonePositionJob
            {
             Positions = _bonePositionsNa,
            };
            var getBonePositionHandle = getBonePositionJob.Schedule(_boneTransforms);

            getBonePositionHandle.Complete();

            var boneSpringJob = new BoneSpringJob
            {
             Positions = _bonePositionsNa,
             Forces = _boneForcesNa,
             SpringInfos = _springsNa,
             SpringList = _springListNa,
             DeltaTime = Time.deltaTime,
             SpringStiffness = _springStiffness,
             ConnectedCount = _connectedCount,
             Bonecount = _boneCount,
             CenterForce = _centerForcesNa
            };

            var boneSpringHandle = boneSpringJob.Schedule(_bones.Length, 16);
            boneSpringHandle.Complete();
            Vector3 centerForce = Vector3.zero;
            for (int i = 0; i < _bones.Length; i++)
            {
                Vector3 currentVelocity = _rigidbodies[i].velocity;
                Vector3 dampingForce = -currentVelocity * _damping;
                Vector3 totalForce = _boneForcesNa[i] * Time.deltaTime + dampingForce;
                _rigidbodies[i].AddForce(totalForce);
                centerForce += _centerForcesNa[i];
            }
            Vector3 centerVelocity = _centerRb.velocity;
            Vector3 centerDampingForce = -centerVelocity * _damping;
            Vector3 totalCenterForce = centerForce * Time.deltaTime + centerDampingForce;
            _centerRb.AddForce(totalCenterForce);
        }

        private void Update()
        {

            _worldToLocalMatrix = transform.worldToLocalMatrix;
            var vertexUpdateJob = new VertexUpdateJob
            {
                Vertices = _verticesNa,
                VertexWeights = _vertexWeightsNa,
                Positions = _bonePositionsNa,
                WorldToLocalMatrix = _worldToLocalMatrix
            };

            var vertexUpdateHandle = vertexUpdateJob.Schedule(_verticesNa.Length, 16);

            var faceNormalUpdateJob = new FaceNormalUpdateJob
            {
                Vertices = _verticesNa,
                Triangles = _trianglesNa,
                FaceNormal = _faceNormalsNa,
            };

            var faceNormalUpdateHandle = faceNormalUpdateJob.Schedule(_triCount, 16, vertexUpdateHandle);

            var vertexNormalUpdateJob = new VertexNormalUpdateJob
            {
                FaceNormal = _faceNormalsNa,
                TriangleInfos = _triangleInfoNa,
                ConnectedTriCount = _connectedTriCount,
                VertexNormal = _vertexNormalsNa,
            };

            var vertexNormalUpdateHandle = vertexNormalUpdateJob.Schedule(_vertexNormalsNa.Length, 16, faceNormalUpdateHandle);
            vertexNormalUpdateHandle.Complete();

            _meshFilter.mesh.SetVertices(_verticesNa);
            _meshFilter.mesh.SetNormals(_vertexNormalsNa);
        }

        private NativeArray<int> BuildTriangleInfo(NativeArray<int> trianglesNa, int vertexCount, out int triCount, out int connectedTriCount)
        {
            int[] counts = new int[vertexCount];
            int triangleCount = trianglesNa.Length / 3;

            for (int tri = 0; tri < triangleCount; tri++)
            {
                for (int i = 0; i < 3; i++)
                {
                    int vertexIndex = trianglesNa[tri * 3 + i];
                    counts[vertexIndex]++;
                }
            }

            var maxTrianglesPerVertex = 0;
            for (int v = 0; v < vertexCount; v++)
            {
                if (counts[v] > maxTrianglesPerVertex)
                {
                    maxTrianglesPerVertex = counts[v];
                }
            }

            triCount = triangleCount;

            connectedTriCount = maxTrianglesPerVertex;

            var triangleInfoNa = new NativeArray<int>(vertexCount * maxTrianglesPerVertex, Allocator.Persistent);

            int[] currentTriangleCounts = new int[vertexCount];

            for (int tri = 0; tri < triangleCount; tri++)
            {
                for (int i = 0; i < 3; i++)
                {
                    int vertexIndex = trianglesNa[tri * 3 + i];
                    int count = currentTriangleCounts[vertexIndex];

                    triangleInfoNa[vertexIndex * maxTrianglesPerVertex + count] = tri;

                    currentTriangleCounts[vertexIndex]++;
                }
            }

            for (int i = 0; i < vertexCount; i++)
            {
                int start = i * maxTrianglesPerVertex + currentTriangleCounts[i];
                int end = (i + 1) * maxTrianglesPerVertex;
                for (int j = start; j < end; j++)
                {
                    triangleInfoNa[j] = -1;
                }
            }

            return triangleInfoNa;
        }

        private void OnDestroy()
        {
            if (_verticesNa.IsCreated) _verticesNa.Dispose();
            if (_vertexWeightsNa.IsCreated) _vertexWeightsNa.Dispose();
            if (_springsNa.IsCreated) _springsNa.Dispose();
            if (_springListNa.IsCreated) _springListNa.Dispose();
            if (_boneForcesNa.IsCreated) _boneForcesNa.Dispose();
            if (_centerForcesNa.IsCreated) _centerForcesNa.Dispose();
            if (_bonePositionsNa.IsCreated) _bonePositionsNa.Dispose();
            if (_trianglesNa.IsCreated) _trianglesNa.Dispose();
            if (_triangleInfoNa.IsCreated) _triangleInfoNa.Dispose();
            if (_faceNormalsNa.IsCreated) _faceNormalsNa.Dispose();
            if (_vertexNormalsNa.IsCreated) _vertexNormalsNa.Dispose();
            _boneTransforms.Dispose();
        }

        private void OnEnable()
        {
            //Physics.ContactModifyEvent += ModificationEvent;
        }

        private void OnDisable()
        {
            //Physics.ContactModifyEvent -= ModificationEvent;
        }

        public void SetHasModifiableContacts(bool enabled)
        {
            if (_bones == null) return;

            for (int i = 0; i < _bones.Length; i++)
            {
                if (_bones[i].GetComponent<Collider>() != null)
                {
                    _bones[i].GetComponent<Collider>().hasModifiableContacts = enabled;
                }
            }
        }

        // public void ModificationEvent(PhysicsScene scene, NativeArray<ModifiableContactPair> pairs)
        // {
        //     Debug.Log("modify : softbody");
        //     for (int i = 0; i < pairs.Length; i++)
        //     {
        //         var pair = pairs[i];

        //         var properties = pair.massProperties;
        //         properties.inverseMassScale = 1f;
        //         properties.inverseInertiaScale = 1f;
        //         properties.otherInverseMassScale = 0f;
        //         properties.otherInverseInertiaScale = 0f;

        //         pair.massProperties = properties;

        //         pairs[i] = pair;
        //     }
        // }

        [BurstCompile]
        private struct GetBonePositionJob : IJobParallelForTransform
        {
            public NativeArray<Vector3> Positions;

            public void Execute(int index, TransformAccess transform)
            {
                Positions[index] = transform.position;
            }
        }

        [BurstCompile]
        private struct BoneSpringJob : IJobParallelFor
        {

            public NativeArray<Vector3> Forces;
            public NativeArray<Vector3> CenterForce;
            [ReadOnly] public NativeArray<Spring> SpringInfos;
            [ReadOnly] public NativeArray<int> SpringList;
            [ReadOnly] public NativeArray<Vector3> Positions;

            public float DeltaTime;
            public float SpringStiffness;

            public int ConnectedCount;
            public int Bonecount;

            public void Execute(int index)
            {
                Vector3 force = Vector3.zero;

                var start = index * ConnectedCount;
                var end = start + ConnectedCount;

                // Debug.Log($"start: {start}, end: {end}");
                for (int i = start; i < end; i++)
                {
                    var springIndex = SpringList[i];
                    if (springIndex != -1)
                    {
                        Spring spring = SpringInfos[springIndex];
                        var bone1 = spring.Bone1;
                        var bone2  = spring.Bone2;
                        var restLength = spring.RestLength;

                        Vector3 pos1 = Positions[bone1];
                        Vector3 pos2 = Positions[bone2];

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

                        if (bone1 == Bonecount)
                        {
                            CenterForce[index] = springForce;
                        }
                        else if (bone2 == Bonecount)
                        {
                            CenterForce[index] = -springForce;
                        }
                        // Debug.Log($"i: {index} - bone1: {bone1} - bone2: {bone2} - force: {springForce} restLength {restLength} currentLength {currentLength} ");
                    }
                }

                Forces[index] = force;
            }
        }

        [BurstCompile]
        private struct VertexUpdateJob : IJobParallelFor
        {
            public NativeArray<Vector3> Vertices;
            [ReadOnly] public NativeArray<VertexWeightInfo> VertexWeights;
            [ReadOnly] public NativeArray<Vector3> Positions;
            [ReadOnly] public Matrix4x4 WorldToLocalMatrix;

            public void Execute(int index)
            {
                var boneInfo = VertexWeights[index];
                var v = Vector3.zero;

                for (var j = 0; j < boneInfo.BoneCount; j++)
                {
                    var weightInfo = boneInfo[j];

                    var bonePositionGlobal = Positions[weightInfo.BoneIndex];
                    var bonePositionLocal = WorldToLocalMatrix.MultiplyPoint3x4(bonePositionGlobal);

                    var boneDesiredVertex = bonePositionLocal + weightInfo.Offset;
                    v += boneDesiredVertex * weightInfo.Weight;
                }

                Vertices[index] = v;
            }
        }

        [BurstCompile]
        private struct FaceNormalUpdateJob : IJobParallelFor
        {

            [ReadOnly] public NativeArray<Vector3> Vertices;
            [ReadOnly] public NativeArray<int> Triangles;

            public NativeArray<Vector3> FaceNormal;

            public void Execute(int index)
            {
                int tri = index * 3;
                var v1 = Vertices[Triangles[tri]];
                var v2 = Vertices[Triangles[tri + 1]];
                var v3 = Vertices[Triangles[tri + 2]];

                var e1 = v2 - v1;
                var e2 = v3 - v2;

                var normal = Vector3.Cross(e1, e2).normalized;

                FaceNormal[index] = normal;
            }
        }

        [BurstCompile]
        private struct VertexNormalUpdateJob : IJobParallelFor
        {

            [ReadOnly] public NativeArray<Vector3> FaceNormal;
            [ReadOnly] public NativeArray<int> TriangleInfos;
            [ReadOnly] public int ConnectedTriCount;


            public NativeArray<Vector3> VertexNormal;

            public void Execute(int index)
            {
                var start = index * ConnectedTriCount;
                var end = start + ConnectedTriCount;

                Vector3 normal = Vector3.zero;
                int count = 0;
                for (int i = start; i < end; i++)
                {
                    if (TriangleInfos[i] != -1)
                    {
                        count++;
                        normal += FaceNormal[TriangleInfos[i]];
                    }
                }

                VertexNormal[index] = normal/count;
                // Debug.Log($"Vertex normal{index} {count} {normal.ToString()} {ConnectedTriCount}: {normal/count}");
            }
        }



        // [BurstCompile]
        // struct SumJob : IJobParallelFor
        // {
        //     [ReadOnly] public NativeArray<Vector3> Input;
        //     [WriteOnly] public NativeArray<Vector3> PartialSums;
        //
        //     public void Execute(int index)
        //     {
        //         PartialSums[index] = Input[index];
        //     }
        // }
        //
        // public static Vector3 CalculateAverage(NativeArray<Vector3> input)
        // {
        //     int length = input.Length;
        //
        //     if (length == 0)
        //         return Vector3.zero;
        //
        //     NativeArray<Vector3> partialSums = new NativeArray<Vector3>(length, Allocator.TempJob);
        //
        //     var sumJob = new SumJob
        //     {
        //         Input = input,
        //         PartialSums = partialSums
        //     };
        //
        //     JobHandle handle = sumJob.Schedule(length, 16);
        //     handle.Complete();
        //
        //     Vector3 totalSum = Vector3.zero;
        //     for (int i = 0; i < length; i++)
        //     {
        //         totalSum += partialSums[i];
        //     }
        //
        //     partialSums.Dispose();
        //
        //     return totalSum / length;
        // }
    }
}
