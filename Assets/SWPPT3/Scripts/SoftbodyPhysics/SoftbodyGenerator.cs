using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;
using UnityEngine.Jobs;
using Random = UnityEngine.Random;

namespace SWPPT3.SoftbodyPhysics
{
    public class SoftbodyGenerator : MonoBehaviour
    {
        private MeshFilter _originalMeshFilter;
        private List<Vector3> WritableVertices { get; set; }
        private List<Vector3> WritableNormals { get; set; }

        private readonly List<SphereCollider> _sphereColliderList = new List<SphereCollider>();
        private SphereCollider[] _sphereColliderArray;

        private List<Rigidbody> _rigidbodyList = new List<Rigidbody>();
        private Rigidbody[] _rigidbodyArray;


        // NativeArray for Job system
        private TransformAccessArray _optVerticesTransform; // optimized된 vertex의 transform을 저장
        private NativeArray<Vector3> _optVerticesBuffer;    // optimize된 vertex를 저장하기 위한 buffer
        private NativeArray<int> _optToOriDic;              // opt -> ori 를 위한 dictionary
        private NativeArray<Vector3> _oriPositions;
        // NativeArray for Job system

        private int[] WritableTris { get; set; }

        private Mesh _writableMesh;

        private List<GameObject> _phyisicedVertexes;
        private new Dictionary<int, int> _vertexDictunery;

        private bool _isSlime = true;
        public bool IsSlime
        {
            get => _isSlime;
            set => _isSlime = value;
        }
        /** public variable to controll softbody **/
        private float _collissionSurfaceOffset = 0.1f;
        public float CollissionSurfaceOffset
        {
            get
            {
                return _collissionSurfaceOffset;
            }
            set
            {
                _collissionSurfaceOffset = value;
                if (_phyisicedVertexes != null)
                    foreach (var sCollider in _sphereColliderArray)
                        sCollider.radius = _collissionSurfaceOffset;
            }
        }

        public SoftJointLimitSpring Springlimit;
        private float _softness = 1f;
        public float Softness
        {
            get
            {
                return _softness;
            }
            set
            {
                _softness = value;
                if(_phyisicedVertexes!=null)
                    foreach (var gObject in _phyisicedVertexes)
                        gObject.GetComponent<SpringJoint>().spring = _softness;

                Springlimit.spring = _softness;
            }
        }
        private float _damp = .2f;
        public float Damp
        {
            get
            {
                return _damp;
            }
            set
            {
                _damp = value;
                if (_phyisicedVertexes != null)
                    foreach (var gObject in _phyisicedVertexes)
                        gObject.GetComponent<SpringJoint>().damper = _damp;

                Springlimit.damper = _damp;
            }
        }
        public float _mass = 1f;
        public float mass
        {
            get
            {
                return _mass;
            }
            set
            {
                _mass = value;
                if (_phyisicedVertexes != null)
                    foreach (var rb in _rigidbodyArray)
                        rb.mass = _mass;
            }
        }

        private bool _debugMode = false;
        public bool DebugMode
        {
            get
            {
                return _debugMode;
            }
            set
            {
                _debugMode = value;
                if (_debugMode == false)
                {
                    if (_phyisicedVertexes != null)
                        foreach (var gObject in _phyisicedVertexes)
                            gObject.hideFlags = HideFlags.HideAndDontSave;
                    if (centerOfMasObj != null)
                        centerOfMasObj.hideFlags = HideFlags.HideAndDontSave;
                } else {
                    if (_phyisicedVertexes != null)
                        foreach (var gObject in _phyisicedVertexes)
                            gObject.hideFlags = HideFlags.None;
                    if(centerOfMasObj!=null)
                        centerOfMasObj.hideFlags = HideFlags.None;
                }

            }
        }


        private float _physicsRoughness = 4;
        public float PhysicsRoughness {
            get {
                return _physicsRoughness;
            }
            set {
                _physicsRoughness = value;
                if (_phyisicedVertexes != null)
                    foreach (var rb in _rigidbodyArray)
                        rb.drag = PhysicsRoughness;
            }
        }
        private bool _gravity = true;
        public bool Gravity
        {
            get
            {
                return _gravity;
            }
            set
            {
                _gravity = value;
                if (_phyisicedVertexes != null)
                    foreach (var rb in _rigidbodyArray)
                        rb.useGravity = _gravity;
                if (centerOfMasObj != null)
                    _rbOfCenter.useGravity = _gravity;
            }
        }
        public GameObject centerOfMasObj = null;
        private Rigidbody _rbOfCenter = null;

        private PhysicMaterial _physicsMaterial = null;

        private bool _isJumping;

        private int i = 0;
        private void Awake()
        {
            i = 0;
            _isJumping = false;

            _physicsMaterial = new PhysicMaterial();
            _physicsMaterial.bounciness = 0f;
            _physicsMaterial.dynamicFriction = 1f;
            _physicsMaterial.staticFriction = 1f;

            _physicsMaterial.bounceCombine = PhysicMaterialCombine.Maximum;

            WritableVertices = new List<Vector3>();
            WritableNormals = new List<Vector3>();
            _phyisicedVertexes = new List<GameObject>();

            _originalMeshFilter = GetComponent<MeshFilter>();
            _originalMeshFilter.mesh.GetVertices(WritableVertices);
            _originalMeshFilter.mesh.GetNormals(WritableNormals);
            WritableTris = _originalMeshFilter.mesh.triangles;

            _oriPositions = new NativeArray<Vector3>(_originalMeshFilter.mesh.vertices, Allocator.Persistent);

            var localToWorld = transform.localToWorldMatrix;
            for (int i = 0; i < WritableVertices.Count; ++i)
            {
                WritableVertices[i] = localToWorld.MultiplyPoint3x4(WritableVertices[i]);
            }

            _writableMesh = new Mesh();
            _writableMesh.MarkDynamic();
            _writableMesh.SetVertices(WritableVertices);
            _writableMesh.SetNormals(WritableNormals);
            _writableMesh.triangles = WritableTris;
            _originalMeshFilter.mesh = _writableMesh;
            // remove duplicated vertex
            var _optimizedVertex = new List<Vector3>();

            // first column = original vertex index , last column = optimized vertex index
            _vertexDictunery = new Dictionary<int, int>();
            for (int i = 0; i < WritableVertices.Count; i++)
            {
                bool isVertexDuplicated = false;
                for (int j = 0; j < _optimizedVertex.Count; j++)
                    if (_optimizedVertex[j] == WritableVertices[i])
                    {
                        isVertexDuplicated = true;
                        _vertexDictunery.Add(i, j);
                        break;
                    }
                if (!isVertexDuplicated)
                {
                    _optimizedVertex.Add(WritableVertices[i]);
                    _vertexDictunery.Add(i, _optimizedVertex.Count - 1);
                }
            }

            _optToOriDic = new NativeArray<int>(WritableVertices.Count, Allocator.Persistent);
            for (int i = 0; i < WritableVertices.Count; ++i)
            {
                _optToOriDic[i] = _vertexDictunery[i];
            }


            // create balls at each of vertex also center of mass
            foreach (var vertecs in _optimizedVertex)
            {
                var _tempObj = new GameObject("Point "+ _optimizedVertex.IndexOf(vertecs));

                if (!DebugMode)
                    _tempObj.hideFlags = HideFlags.HideAndDontSave;

                _tempObj.transform.parent = this.transform;
                _tempObj.transform.position = vertecs;


                // add collider to each of vertex ( sphere collider )
                var sphereColider = _tempObj.AddComponent<SphereCollider>() as SphereCollider;
                sphereColider.radius = CollissionSurfaceOffset;
                sphereColider.material = _physicsMaterial;

                // add current collider to Collider list ;
                _sphereColliderList.Add(sphereColider);


                // add rigidBody to each of vertex
                var _tempRigidBody = _tempObj.AddComponent<Rigidbody>();
                _tempRigidBody.mass = mass / _optimizedVertex.Count;
                _tempRigidBody.drag = PhysicsRoughness;

                _rigidbodyList.Add(_tempRigidBody);

                _tempObj.AddComponent<DebugColorGameObject>().Color = Random.ColorHSV();

                _phyisicedVertexes.Add(_tempObj);
            }



            // calculate center of mass
            Vector3 tempCenter = Vector3.zero;

            foreach (var vertecs in _optimizedVertex)
                tempCenter = new Vector3(tempCenter.x + vertecs.x, tempCenter.y + vertecs.y,tempCenter.z + vertecs.z );

            Vector3 centerOfMass = new Vector3(
                  tempCenter.x / _optimizedVertex.Count
                , tempCenter.y / _optimizedVertex.Count
                , tempCenter.z / _optimizedVertex.Count
            );
            // add center of mass vertex to OptimizedVertex list
            {
                var _tempObj = new GameObject("centerOfMass");

                if (!DebugMode)
                    _tempObj.hideFlags = HideFlags.HideAndDontSave;
                _tempObj.transform.parent = this.transform;
                _tempObj.transform.position = centerOfMass;

                // add collider to center of mass as a sphere collider
                var sphereColider = _tempObj.AddComponent<SphereCollider>() as SphereCollider;
                sphereColider.radius = CollissionSurfaceOffset;
                // add current collider to Collider list ;
                _sphereColliderList.Add(sphereColider);

                // add rigidBody to center of mass as a sphere collider
                var _tempRigidBody = _tempObj.AddComponent<Rigidbody>();
                _rigidbodyList.Add(_tempRigidBody);
                _rbOfCenter = _tempRigidBody;

                centerOfMasObj = _tempObj;
            }

            _sphereColliderArray = _sphereColliderList.ToArray();
            _rigidbodyArray  = _rigidbodyList.ToArray();


            _optVerticesTransform = new TransformAccessArray(_rigidbodyArray.Length);
            for (int i = 0; i < _rigidbodyArray.Length; ++i)
            {
                _optVerticesTransform.Add(_rigidbodyArray[i].transform);
            }
            _optVerticesBuffer = new NativeArray<Vector3>(_optVerticesTransform.length, Allocator.Persistent);

            // Debug.Log($"{_rigidbodyArray.Length}  {_sphereColliderArray.Length} {_verticesTransform.length} {_verticesNa.Length}");



            // IGNORE COLLISTION BETWEEN ALL OF THE VERTEXES AND CENTER OFF MASS
            foreach (var collider1 in _sphereColliderArray)
            {
                foreach (var collider2 in _sphereColliderArray)
                {
                    Physics.IgnoreCollision(collider1, collider2, true);
                }
            }

            // extract Lines from quad of mesh
            List<Vector2Int> tempListOfSprings = new List<Vector2Int>();
            bool isFirstTrisOfQuad = true;
            for (int i=0;i<WritableTris.Length;i+=3)
            {
                int index0 = _vertexDictunery[WritableTris[i]];
                int index1 = _vertexDictunery[WritableTris[i+1]];
                int index2 = _vertexDictunery[WritableTris[i+2]];

                tempListOfSprings.Add(new Vector2Int(index1, index2));
                // this System convert Tris To Quad
                if (isFirstTrisOfQuad)
                {
                    tempListOfSprings.Add(new Vector2Int(index0, index1));
                    isFirstTrisOfQuad = false;
                }
                else
                {
                    tempListOfSprings.Add(new Vector2Int(index2, index0));
                    isFirstTrisOfQuad = true;
                }
            }


            // distinct normal Duplicates with check revers
            for (int i = 0; i < tempListOfSprings.Count; i++)
            {
                bool isDuplicated = false;
                Vector2Int normal = tempListOfSprings[i];
                Vector2Int reversed = new Vector2Int(tempListOfSprings[i].y, tempListOfSprings[i].x);
                for (int j = 0; j < noDupesListOfSprings.Count; j++)
                {
                    if (normal == tempListOfSprings[j])
                    {
                        isDuplicated = true;
                        break;
                    }
                    else if (reversed == tempListOfSprings[j])
                    {
                        isDuplicated = true;
                        break;
                    }

                }
                if (isDuplicated == false)
                    noDupesListOfSprings.Add(tempListOfSprings[i]);
            }

            // making Springs bodies
            foreach (var jointIndex in noDupesListOfSprings)
            {
                var thisGameObject = _phyisicedVertexes[jointIndex.x];
                var thisBodyJoint = thisGameObject.AddComponent<CharacterJoint>();
                var destinationBody = _phyisicedVertexes[jointIndex.y].GetComponent<Rigidbody>();
                float distanceBetween = Vector3.Distance(thisGameObject.transform.position, destinationBody.transform.position);


                // configure current spring joint
                thisBodyJoint.connectedBody = destinationBody;
                SoftJointLimit jointlimitHihj = new SoftJointLimit();
                jointlimitHihj.bounciness = 1.1f;
                jointlimitHihj.contactDistance = distanceBetween;
                jointlimitHihj.limit = 10;

                SoftJointLimit jointlimitLow = new SoftJointLimit();
                jointlimitLow.bounciness = 1.1f;
                jointlimitLow.contactDistance = distanceBetween;
                jointlimitLow.limit = -10;


                thisBodyJoint.highTwistLimit = jointlimitHihj;
                thisBodyJoint.lowTwistLimit = jointlimitLow;
                thisBodyJoint.swing1Limit = jointlimitLow;
                thisBodyJoint.swing2Limit = jointlimitHihj;


                //thisBodyJoint.

                Springlimit.damper = Damp;
                Springlimit.spring = Softness;

                thisBodyJoint.swingLimitSpring = Springlimit;
                thisBodyJoint.twistLimitSpring = Springlimit;

            }

            // Decelare Center of mass variable
            foreach (var jointIndex in _phyisicedVertexes)
            {
                var destinationBodyJoint = jointIndex.AddComponent<SpringJoint>();

                float distanceToCenterOfmass = Vector3.Distance(
                      centerOfMasObj.transform.localPosition
                    , destinationBodyJoint.transform.localPosition
                );

                destinationBodyJoint.connectedBody = centerOfMasObj.GetComponent<Rigidbody>();
                destinationBodyJoint.spring = Softness;
                destinationBodyJoint.damper = Damp;

                //destinationBodyJoint.massScale = 0.001f;
                //destinationBodyJoint.connectedMassScale = 0.001f;
            }


        }
        List<Vector2Int> noDupesListOfSprings = new List<Vector2Int>();

        // public void SetSlime()
        // {
        //     _physicsMaterial.bounciness = 0f;
        //     _physicsMaterial.dynamicFriction = 0f;
        //     _physicsMaterial.staticFriction = 0f;
        //     _physicsMaterial.bounceCombine = PhysicMaterialCombine.Maximum;
        //
        //     IsSlime = true;
        // }
        //
        // public void SetMetal()
        // {
        //     _physicsMaterial.bounciness = 0f;
        //     _physicsMaterial.dynamicFriction = 0f;
        //     _physicsMaterial.staticFriction = 0f;
        //     _physicsMaterial.bounceCombine = PhysicMaterialCombine.Maximum;
        //     IsSlime = false;
        // }
        //
        // public void SetRubberNonJump()
        // {
        //     _physicsMaterial.bounciness = 0.5f;
        //     _physicsMaterial.dynamicFriction = 0f;
        //     _physicsMaterial.staticFriction = 0f;
        //     _physicsMaterial.bounceCombine = PhysicMaterialCombine.Maximum;
        //     IsSlime = false;
        // }
        //
        // public void SetRubberJump()
        // {
        //     _physicsMaterial.bounciness = 1f;
        //     _physicsMaterial.dynamicFriction = 0f;
        //     _physicsMaterial.staticFriction = 0f;
        //     _physicsMaterial.bounceCombine = PhysicMaterialCombine.Maximum;
        //     IsSlime = false;
        // }

        // public void SoftbodyJump(float jumpForce)
        // {
        //     _isJumping = true;
        //     foreach (var rb in _rigidbodyArray)
        //     {
        //         rb.AddForce(Vector3.up * (jumpForce * rb.mass));
        //     }
        //     Debug.Log($"{_rigidbodyArray.Length} Rigidbodies added");
        // }

        // private Vector3 GetMoveDirection()
        // {
        //     Vector3 direction = Vector3.zero;
        //
        //     if (Input.GetKey(KeyCode.W))
        //         direction += Vector3.forward;
        //     if (Input.GetKey(KeyCode.S))
        //         direction += Vector3.back;
        //     if (Input.GetKey(KeyCode.A))
        //         direction += Vector3.left;
        //     if (Input.GetKey(KeyCode.D))
        //         direction += Vector3.right;
        //
        //     if (direction != Vector3.zero)
        //         direction = direction.normalized;
        //
        //     return direction;
        // }


        public void Update()
        {
            // i++;
            // if (i % 1000 == 0)
            // {
            //     _isSlime = !_isSlime;
            //     Debug.Log($"change to {_isSlime}");
            // }
            //
            // if (i % 500 == 0)
            // {
            //     SoftbodyJump(500.0f);
            //     // SetRubberJump();
            //     Debug.Log("Softbody Jumped");
            // }

            // Vector3 moveDirection = GetMoveDirection();
            // Debug.Log($"moveDirection: {moveDirection}");
            // Vector3 force = moveDirection * 0.01f;

            // foreach (var rb in _rigidbodyArray)
            // {
            //     rb.MovePosition(rb.position + force);
            // }
            //
            // if (Input.GetKeyDown(KeyCode.Space) && !_isJumping)
            // {
            //     SoftbodyJump(500f);
            // }

           if (DebugMode)
           {
                foreach (var jointIndex in noDupesListOfSprings)
                {
                    Debug.DrawLine(
                        _phyisicedVertexes[jointIndex.x].transform.position
                        , _phyisicedVertexes[jointIndex.y].transform.position
                        , _phyisicedVertexes[jointIndex.x].GetComponent<DebugColorGameObject>().Color
                    );

                }
                foreach (var jointIndex in noDupesListOfSprings)
                {
                    Debug.DrawLine(
                          _phyisicedVertexes[jointIndex.x].transform.position
                        , centerOfMasObj.transform.position
                        , Color.red
                    );

                }
           }

           var setVertexUpdateJob = new SetVertexUpdateJob
           {
               LocalPositions = _optVerticesBuffer,
               OptToOriDic = _optToOriDic,
               OriPositions = _oriPositions
           };
           var setVertexUpdateHandle = setVertexUpdateJob.Schedule(_oriPositions.Length, 16);
           setVertexUpdateHandle.Complete();
           _originalMeshFilter.mesh.vertices = _oriPositions.ToArray();
           _originalMeshFilter.mesh.RecalculateBounds();
           _originalMeshFilter.mesh.RecalculateNormals();
        }

        public void FixedUpdate()
        {
            // if (IsSlime)
            // {
                var getVertexLocalPositionJob = new GetVertexLocalPositionJob
                {
                    Buffer = _optVerticesBuffer,
                };
                var getVertexLocalPositionHandle = getVertexLocalPositionJob.Schedule(_optVerticesTransform);
                getVertexLocalPositionHandle.Complete();
            // }
            // else
            // {
            //     var setVertexLocalPositionJob = new SetVertexLocalPositionJob
            //     {
            //         LocalPositions = Buffer,
            //     };
            //     var getVertexLocalPositionHandle = setVertexLocalPositionJob.Schedule(_optVerticesTransform);
            //     getVertexLocalPositionHandle.Complete();
            // }

        }

        public void OnDestroy()
        {
            if(_optVerticesBuffer.IsCreated) _optVerticesBuffer.Dispose();
            if(_optVerticesTransform.isCreated) _optVerticesTransform.Dispose();
            if(_optToOriDic.IsCreated) _optToOriDic.Dispose();
            if(_oriPositions.IsCreated) _oriPositions.Dispose();
        }
    }

    /// <summary>
    ///
    /// LocalPosition buffer에 transformacess에 있는 local position을 대입
    /// </summary>
    [BurstCompile]
    public struct GetVertexLocalPositionJob : IJobParallelForTransform
    {
        public NativeArray<Vector3> Buffer;

        public void Execute(int index, TransformAccess transform)
        {
            Buffer[index] = transform.localPosition;
        }
    }
    /// <summary>
    /// GetVertexLocalPostionJob과는 반대로 LocalPosition에 있는 값을 transformaccess에 대입
    /// </summary>
    [BurstCompile]
    public struct SetVertexLocalPositionJob : IJobParallelForTransform
    {
        public NativeArray<Vector3> Buffer;

        public void Execute(int index, TransformAccess transform)
        {
            transform.localPosition = Buffer[index];
        }
    }

    /// <summary>
    /// vertex를 optimize하면서 optimize된 vertex를 Original vertex에 대응하기 위한 job
    /// </summary>
    [BurstCompile]
    public struct SetVertexUpdateJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Vector3> LocalPositions;
        [ReadOnly] public NativeArray<int> OptToOriDic;

        public NativeArray<Vector3> OriPositions;

        public void Execute(int index)
        {
            OriPositions[index] = LocalPositions[OptToOriDic[index]];
        }

    }



    public class DebugColorGameObject : MonoBehaviour
    {
        public Color Color { get; set; }
    }

    [CustomEditor(typeof(SoftbodyGenerator))]
    public class LookAtPointEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            SoftbodyGenerator softbody = target as SoftbodyGenerator;

            softbody.DebugMode = EditorGUILayout.Toggle("#Debug mod", softbody.DebugMode);
            EditorGUILayout.Space();

            string[] options = new string[] { "  version 1", "  version 2" };


            softbody.Gravity = EditorGUILayout.Toggle("Gravity", softbody.Gravity);
            softbody.mass = EditorGUILayout.FloatField("Mass(KG)", softbody.mass);
            softbody.PhysicsRoughness = EditorGUILayout.FloatField("Drag (roughness)", softbody.PhysicsRoughness);
            softbody.Softness = EditorGUILayout.FloatField("Softbody hardness", softbody.Softness);
            softbody.Damp = EditorGUILayout.FloatField("Softbody damper", softbody.Damp);
            softbody.CollissionSurfaceOffset = EditorGUILayout.FloatField("Softbody Offset", softbody.CollissionSurfaceOffset);

        }
    }
}
