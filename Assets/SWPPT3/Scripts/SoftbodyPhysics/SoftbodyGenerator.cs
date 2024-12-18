using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEditor;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.Jobs;
using Random = UnityEngine.Random;

namespace SWPPT3.SoftbodyPhysics
{
    public enum SoftStates
    {
        Slime = 0,
        Metal,
        Rubber,
    }
    public class SoftbodyGenerator : MonoBehaviour
    {
        [SerializeField] private SoftbodyScript _script;

        private MeshFilter _originalMeshFilter;
        private List<Vector3> WritableVertices { get; set; }
        private List<Vector3> WritableNormals { get; set; }

        private readonly List<SphereCollider> _sphereColliderList = new List<SphereCollider>();
        private SphereCollider[] _sphereColliderArray;

        private List<Rigidbody> _rigidbodyList = new List<Rigidbody>();
        private Rigidbody[] _rigidbodyArray;

        private Rigidbody rootRB;

        public event Action<bool> OnRubberJump;

        private bool _isRubberJump;

        public bool IsRubberJump
        {
            get => _isRubberJump;
            set
            {
                if (_isRubberJump != value)
                {
                    _isRubberJump = value;
                    OnRubberJump?.Invoke(_isRubberJump);
                }
            }
        }


        public SoftStates PlayerStates { get; set; }


        // NativeArray for Job system
        private TransformAccessArray _optVerticesTransform; // optimized된 vertex의 transform을 저장
        private NativeArray<Vector3> _optVerticesBuffer;    // optimize된 vertex를 저장하기 위한 buffer
        private NativeArray<int> _optToOriDic;              // opt -> ori 를 위한 dictionary
        private NativeArray<Vector3> _oriPositions;

        // private List<Vector3> _oriJointAnchorsList;
        // private Vector3[] _oriJointAnchorArray;
        // private NativeArray<Vector3> _bufferJointAnchors;

        private List<ConfigurableJoint> _configurableJointList;
        private ConfigurableJoint[] _configurableJointsArray;

        // private List<(int, int)> _jointsDict;
        // private NativeArray<(int,int)> _jointsDictNa;

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
                        gObject.GetComponent<ConfigurableJoint>().linearLimitSpring = new SoftJointLimitSpring { spring = _softness, damper = gObject.GetComponent<ConfigurableJoint>().linearLimitSpring.damper };

                Springlimit.spring = _softness;
            }
        }
        private float _damp = .5f;
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
                        gObject.GetComponent<ConfigurableJoint>().linearLimitSpring = new SoftJointLimitSpring { spring = gObject.GetComponent<ConfigurableJoint>().linearLimitSpring.spring, damper = _damp };

                Springlimit.damper = _damp;
            }
        }
        private float _mass = 1f;
        public float Mass
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
                    // if (centerOfMasObj != null)
                    //     centerOfMasObj.hideFlags = HideFlags.HideAndDontSave;
                } else {
                    if (_phyisicedVertexes != null)
                        foreach (var gObject in _phyisicedVertexes)
                            gObject.hideFlags = HideFlags.None;
                    if(centerOfMasObj!=null)
                        centerOfMasObj.hideFlags = HideFlags.None;
                }

            }
        }


        private float _physicsRoughness = 2;
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

        private float _rubberJump = 30;
        public float RubberJump {
            get {
                return _rubberJump;
            }
            set {
                _rubberJump = value;
            }
        }

        public void OnEnable()
        {
            Physics.ContactModifyEvent += ModificationEvent;
        }

        public void OnDisable()
        {
            Physics.ContactModifyEvent -= ModificationEvent;
        }

        public void ModificationEvent(PhysicsScene scene, NativeArray<ModifiableContactPair> pairs)
        {
            for (int i = 0; i < pairs.Length; i++)
            {
                var pair = pairs[i];

                var properties = pair.massProperties;

                properties.inverseMassScale = 1f;
                properties.inverseInertiaScale = 1f;
                properties.otherInverseMassScale = 0;
                properties.otherInverseInertiaScale = 0;

                pair.massProperties = properties;

                pairs[i] = pair;
            }
        }

        public event Action<Collision>HandleCollisionEnterEvent;
        public event Action<Collision>HandleCollisionStayEvent;
        public event Action<Collision>HandleCollisionExitEvent;
        public event Action<Collider> HandleTriggerEnterEvent;
        public event Action<Collider> HandleTriggerExitEvent;

        public void CollisionEnter(Collision collision)
        {
            HandleCollisionEnterEvent?.Invoke(collision);
        }

        public void CollisionStay(Collision collision)
        {
            HandleCollisionStayEvent?.Invoke(collision);
        }

        public void CollisionExit(Collision collision)
        {
            HandleCollisionExitEvent?.Invoke(collision);
        }

        public void TriggerEnter(Collider other)
        {
            HandleTriggerEnterEvent?.Invoke(other);
        }

        public void TriggerExit(Collider other)
        {
            HandleTriggerExitEvent?.Invoke(other);
        }



        private GameObject centerOfMasObj = null;
        private Rigidbody _rbOfCenter = null;

        private bool _isJumpKey;

        public bool IsJumpKey
        {
            get => _isJumpKey;
            set => _isJumpKey = value;
        }

        private void Awake()
        {
            Softness = _script.Softness;
            Mass = _script.Mass;
            PhysicsRoughness = _script.PhysicsRoughness;
            Damp = _script.Damp;
            RubberJump = _script.RubberJump;

            _configurableJointList = new List<ConfigurableJoint>();

            _isJumpKey = false;

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

                // add current collider to Collider list ;
                _sphereColliderList.Add(sphereColider);


                // add rigidBody to each of vertex
                var _tempRigidBody = _tempObj.AddComponent<Rigidbody>();
                _tempRigidBody.mass = Mass / _optimizedVertex.Count;
                _tempRigidBody.drag = PhysicsRoughness;
                _tempRigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

                var rubberJump = _tempObj.AddComponent<Particle>();

                OnRubberJump += rubberJump.SetActive;
                rubberJump._softbody = this;
                rubberJump.jumpForce = _script.RubberJump;

                _rigidbodyList.Add(_tempRigidBody);

                _tempObj.AddComponent<DebugColorGameObject>().Color = Random.ColorHSV();

                _phyisicedVertexes.Add(_tempObj);
            }



            // calculate center of mass
            Vector3 tempCenter = Vector3.zero;

            foreach (var vertecs in _optimizedVertex)
                tempCenter = new Vector3(tempCenter.x + vertecs.x, tempCenter.y + vertecs.y,tempCenter.z + vertecs.z );


            // Add sphere collider and rigidbody to root object
            rootRB = gameObject.GetComponent<Rigidbody>();
            rootRB.mass = 1;
            rootRB.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            _rigidbodyList.Add(rootRB);

            _sphereColliderArray = _sphereColliderList.ToArray();
            _rigidbodyArray  = _rigidbodyList.ToArray();


            _optVerticesTransform = new TransformAccessArray(_rigidbodyArray.Length);
            for (int i = 0; i < _rigidbodyArray.Length; ++i)
            {
                _optVerticesTransform.Add(_rigidbodyArray[i].transform);
            }
            _optVerticesBuffer = new NativeArray<Vector3>(_optVerticesTransform.length, Allocator.Persistent);

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
                var thisBodyJoint = thisGameObject.AddComponent<ConfigurableJoint>();
                var destinationBody = _phyisicedVertexes[jointIndex.y].GetComponent<Rigidbody>();
                float distanceBetween = Vector3.Distance(thisGameObject.transform.position, destinationBody.transform.position);

                // configure current spring joint
                thisBodyJoint.connectedBody = destinationBody;

                thisBodyJoint.xMotion = ConfigurableJointMotion.Limited;
                thisBodyJoint.yMotion = ConfigurableJointMotion.Limited;
                thisBodyJoint.zMotion = ConfigurableJointMotion.Limited;

                //thisBodyJoint.

                Springlimit.damper = Damp;
                Springlimit.spring = Softness;

                thisBodyJoint.linearLimitSpring = Springlimit;

                _configurableJointList.Add(thisBodyJoint);
            }

            centerOfMasObj = gameObject;
            _rbOfCenter = rootRB;


            // Decelare Center of mass variable
            for (int i = 0 ; i < _phyisicedVertexes.Count; i++ )
            {
                var jointIndex = _phyisicedVertexes[i];
                var destinationBodyJoint = jointIndex.AddComponent<ConfigurableJoint>();

                destinationBodyJoint.xMotion = ConfigurableJointMotion.Limited;
                destinationBodyJoint.yMotion = ConfigurableJointMotion.Limited;
                destinationBodyJoint.zMotion = ConfigurableJointMotion.Limited;

                destinationBodyJoint.connectedBody = rootRB;
                destinationBodyJoint.linearLimitSpring = new SoftJointLimitSpring { spring = Softness, damper = Damp};

                destinationBodyJoint.massScale = 0.001f;

                _configurableJointList.Add(destinationBodyJoint);
            }

            _configurableJointsArray = _configurableJointList.ToArray();
        }

        public void FixJoint()
        {
            for (int i = 0; i < _configurableJointsArray.Length; i++)
            {
                var joint = _configurableJointsArray[i];
                joint.xMotion = ConfigurableJointMotion.Locked;
                joint.yMotion = ConfigurableJointMotion.Locked;
                joint.zMotion = ConfigurableJointMotion.Locked;
                // joint.connectedAnchor = _bufferJointAnchors[i];
            }
        }

        public void FreeJoint()
        {
            for (int i = 0; i < _configurableJointsArray.Length; i++)
            {
                var joint = _configurableJointsArray[i];
                joint.xMotion = ConfigurableJointMotion.Limited;
                joint.yMotion = ConfigurableJointMotion.Limited;
                joint.zMotion = ConfigurableJointMotion.Limited;
                // joint.connectedAnchor = _oriJointAnchorArray[i];
            }
        }

        List<Vector2Int> noDupesListOfSprings = new List<Vector2Int>();



        public void SetSlime()
        {
            if (PlayerStates != SoftStates.Slime)
            {
                FreeJoint();
                foreach (var sc in _sphereColliderArray)
                {
                    sc.hasModifiableContacts = true;
                }
            }

            PlayerStates = SoftStates.Slime;

        }

        public void SetMetal()
        {
            if (PlayerStates == SoftStates.Slime)
            {
                FixJoint();
                foreach (var sc in _sphereColliderArray)
                {
                    sc.hasModifiableContacts = false;
                }
            }

            PlayerStates = SoftStates.Metal;
        }

        public void SetRubber()
        {
            if (PlayerStates == SoftStates.Slime)
            {
                FixJoint();
                foreach (var sc in _sphereColliderArray)
                {
                    sc.hasModifiableContacts = false;
                }
            }

            PlayerStates = SoftStates.Rubber;
        }

        public void SoftbodyJump(float jumpForce)
        {
            // _isJumping = true;
            foreach (var rb in _rigidbodyArray)
            {
                rb.AddForce(Vector3.up * (jumpForce * rb.mass));
            }
        }

        public void move(Vector3 force)
        {
            foreach (var rb in _rigidbodyArray)
            {
                rb.MovePosition(rb.position + force);
            }
        }


        public void Update()
        {
            Softness = _script.Softness;
            Mass = _script.Mass;
            PhysicsRoughness = _script.PhysicsRoughness;
            Damp = _script.Damp;
            // RubberJump = _script.RubberJump;

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

           if (PlayerStates == SoftStates.Rubber && _isJumpKey == true)
           {
               IsRubberJump = true;
           }
           else
           {
               IsRubberJump = false;
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

}
