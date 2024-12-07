using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Editor;
using Unity.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SWPPT3.SoftbodyPhysics.Editor.SoftbodyInspector
{
    [CustomEditor(typeof(Softbody))]
    public class SoftbodyInspector : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/SWPPT3/Scripts/SoftbodyPhysics/Editor/SoftbodyInspector/SoftbodyInspector.uxml");
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/SWPPT3/Scripts/SoftbodyPhysics/Editor/SoftbodyInspector/SoftbodyInspector.uss");
            VisualElement tree = visualTree.Instantiate();
            tree.styleSheets.Add(styleSheet);

            var pointDensityMultiplierProperty = serializedObject.FindProperty("_pointDensityMultiplier");
            var totalLayersProperty = serializedObject.FindProperty("_totalLayers");

            var pointDensityField = new PropertyField(pointDensityMultiplierProperty, "Point Density Multiplier");
            var totalLayersField = new PropertyField(totalLayersProperty, "Total Layers");

            tree.Insert(0, pointDensityField);
            tree.Insert(1, totalLayersField);

            var initializeButton = tree.Q<Button>(name = "InitializeButton");

            initializeButton.clicked += () => OnClickInitialize().Forget();

            return tree;
        }

        private async UniTaskVoid OnClickInitialize()
        {
            Debug.Log("[Softbody] Initialize called.");

            var targetObject = target as Softbody;
            var meshFilter = serializedObject.FindProperty("_meshFilter").objectReferenceValue as MeshFilter;
            var collidersRoot = serializedObject.FindProperty("_collidersRoot").objectReferenceValue as Transform;
            var outerColliders = serializedObject.FindProperty("_outerColliders");
            var vertexWeights = serializedObject.FindProperty("_vertexWeights");
            var springs = serializedObject.FindProperty("_springs");
            var springListProperty = serializedObject.FindProperty("_springList");

            var bones = serializedObject.FindProperty("_bones");

            var connectedCount = serializedObject.FindProperty("_connectedCount");

            var outerIndexStart = serializedObject.FindProperty("_outerIndexStart");

            var pointDensityMultiplierProperty = serializedObject.FindProperty("_pointDensityMultiplier");
            var totalLayersProperty = serializedObject.FindProperty("_totalLayers");

            // Rigidbody rb = serializedObject.FindProperty("_rigidbody").objectReferenceValue as Rigidbody;
            var mass = 5f;

            if (targetObject is null)
            {
                Debug.LogError("[Softbody] Editing target is missing");
                return;
            }

            if (meshFilter is null)
            {
                Debug.LogError("[Softbody] Mesh Filter is missing");
                return;
            }

            if (collidersRoot is null)
            {
                Debug.LogError("[Softbody] Colliders Root is missing");
                return;
            }
            outerColliders.ClearArray();
            Debug.Log("clear start");

            // Clear
            var childCount = collidersRoot.childCount;
            for (var i = childCount - 1; i >= 0; i--)
            {
                var child = collidersRoot.GetChild(i);
                DestroyImmediate(child.gameObject);
            }
            Debug.Log("sj clear start");
            var springJoints = targetObject.GetComponents<SpringJoint>();

            foreach (var s in springJoints)
            {
                DestroyImmediate(s);
            }

            var pointDensityMultiplier = pointDensityMultiplierProperty.intValue;
            var totalLayers = totalLayersProperty.intValue;

            var radiusStep = (Softbody.SphereRadius - targetObject.ColliderRadius) / (totalLayers - 1);
            var totalBoneCount = 1;
            for (var layer = 1; layer < totalLayers; layer++)
            {
                var numPoints = pointDensityMultiplier * layer * layer;
                totalBoneCount += numPoints;
            }

            var massPerBone = mass / totalBoneCount;

            bones.ClearArray();

            var bonesPerLayer = new List<List<Bone>>();

            springs.ClearArray();

            var springsInfo = new List<Spring>();

            int boneIndex = 0;

            int outerIndex = 0;

            Vector3[] outerPositions = new Vector3[pointDensityMultiplier * (totalLayers-1) * (totalLayers-1)];

            for (var layer = 0; layer < totalLayers; layer++)
            {
                var currentLayerBones = new List<Bone>();

                if (layer == 0)
                {
                    bones.InsertArrayElementAtIndex(boneIndex);
                    var boneProperty = bones.GetArrayElementAtIndex(boneIndex);

                    boneProperty.FindPropertyRelative("_mass").floatValue = massPerBone;
                    boneProperty.FindPropertyRelative("_position").vector3Value = Vector3.zero;
                    boneProperty.FindPropertyRelative("_velocity").vector3Value = Vector3.zero;

                    currentLayerBones.Add(new Bone
                    {
                        Mass = massPerBone,
                        Position = Vector3.zero,
                        Velocity = Vector3.zero
                    });

                    boneIndex++;
                }
                else
                {
                    var numPoints = pointDensityMultiplier * layer * layer;
                    var layerRadius = radiusStep * layer;

                    var fibAngles = FibonacciAngles(numPoints);

                    foreach (var angle in fibAngles)
                    {
                        var bonePosition = angle * layerRadius;
                        bones.InsertArrayElementAtIndex(boneIndex);
                        var boneProperty = bones.GetArrayElementAtIndex(boneIndex);

                        boneProperty.FindPropertyRelative("_mass").floatValue = massPerBone;
                        boneProperty.FindPropertyRelative("_position").vector3Value = bonePosition;
                        boneProperty.FindPropertyRelative("_velocity").vector3Value = Vector3.zero;

                        currentLayerBones.Add(new Bone
                        {
                            Mass = massPerBone,
                            Position = bonePosition,
                            Velocity = Vector3.zero
                        });

                        boneIndex++;

                        if (layer == totalLayers - 1)
                        {
                            var go = new GameObject($"Collider {outerIndex}", typeof(SphereCollider));
                            // go.layer = LayerMask.NameToLayer("OuterSphere");
                            // var rb = go.AddComponent<Rigidbody>();
                            // rb.mass = massPerBone;
                            go.transform.SetParent(collidersRoot, false);

                            var sc = go.GetComponent<SphereCollider>();
                            sc.radius = targetObject.ColliderRadius;
                            go.transform.localPosition = bonePosition;

                            outerColliders.InsertArrayElementAtIndex(outerIndex);
                            outerColliders.GetArrayElementAtIndex(outerIndex).objectReferenceValue = sc;
                            outerPositions[outerIndex] = bonePosition;
                            outerIndex++;
                        }
                    }
                }

                bonesPerLayer.Add(currentLayerBones);
            }

            outerIndexStart.intValue = boneIndex - outerIndex;
            Debug.Log($"outerIndexStart {outerIndexStart}");

            var springList = new List<int>(boneIndex * boneIndex);

            for (var i = 0; i < boneIndex*boneIndex; i++)
            {
                springList.Add(-1);
            }

            for (int i = 1; i < totalLayers; i++)
            {
                InitializeSpringInfosForLayer(
                    pointDensityMultiplier-1,
                    i,
                    bonesPerLayer,
                    springsInfo,
                    springList,
                    boneIndex
                );
            }

            var maxCount = 0;

            for (var i = 0; i < boneIndex; i++)
            {
                var count = 0;
                for (var j = 0; j < boneIndex; j++)
                {
                    if (springList[i * boneIndex + j] != -1)
                    {
                        count++;
                    }
                }
                if (count > maxCount)
                {
                    maxCount = count;
                }
            }

            connectedCount.intValue = maxCount;
            Debug.Log($"connectedCount {connectedCount.intValue}");

            var newSpringList = new List<int>(boneIndex * maxCount);

            for (var i = 0; i < boneIndex * maxCount; i++)
            {
                newSpringList.Add(-1);
            }

            for (var i = 0; i < boneIndex; i++)
            {
                var currentCount = 0;
                for (var j = 0; j < boneIndex; j++)
                {
                    var value = springList[i * boneIndex + j];
                    if (value != -1)
                    {
                        newSpringList[i * maxCount + currentCount] = value;
                        currentCount++;
                    }
                }
            }

            springListProperty.ClearArray();
            for (var i = 0; i < newSpringList.Count; i++)
            {
                springListProperty.InsertArrayElementAtIndex(i);
                springListProperty.GetArrayElementAtIndex(i).intValue = newSpringList[i];
            }

            Debug.Log(springsInfo.Count);
            for (int i = 0; i < springsInfo.Count; i++)
            {
                springs.InsertArrayElementAtIndex(i);
                var element = springs.GetArrayElementAtIndex(i);

                element.FindPropertyRelative("restLength").floatValue = springsInfo[i].RestLength;
                element.FindPropertyRelative("_bone1").intValue = springsInfo[i].Bone1;
                element.FindPropertyRelative("_bone2").intValue = springsInfo[i].Bone2;
            }


            Debug.Log("[Softbody] Done setting colliders");

            var mesh = meshFilter.sharedMesh;

            var vertices = mesh.vertices;

            var surfaceBonesCount = pointDensityMultiplier * (totalLayers - 1) * (totalLayers - 1);
            Debug.Log(surfaceBonesCount);
            var vertexCount = mesh.vertexCount;

            var boneCount = Mathf.Clamp((surfaceBonesCount * 4) / vertexCount, 4, 8);

            var vertexWeightInfos = new VertexWeightInfo[mesh.vertexCount];

            // await UniTask.Delay(0);

            for (var i = 0; i < vertices.Length; i++)
            {
                var v = vertices[i];

                var points = ClosestPoints(outerPositions, v, boneCount);

                var bws = new BoneWeightInfo[boneCount];

                for (var j = 0; j < bws.Length; j++)
                {
                    bws[j].BoneIndex = points[j];
                    var tmp = bones.GetArrayElementAtIndex(points[j]);
                    var v3 = tmp.FindPropertyRelative("_position").vector3Value;
                    bws[j].Offset = v - v3;
                }

                var offsetMagSum = bws.Sum(bw => bw.Offset.magnitude);

                for (var j = 0; j < bws.Length; j++)
                {
                    bws[j].Weight = bws[j].Offset.magnitude / offsetMagSum;
                }

                var vertexWeightInfo = new VertexWeightInfo(boneCount);
                for (var j = 0; j < boneCount; j++)
                {
                    vertexWeightInfo[j] = bws[j];
                }

                vertexWeightInfos[i] = vertexWeightInfo;
            }

            vertexWeights.ClearArray();

            for (var i = 0; i < vertexWeightInfos.Length; i++)
            {
                vertexWeights.InsertArrayElementAtIndex(i);
                var ae = vertexWeights.GetArrayElementAtIndex(i);

                var vertexWeightInfo = vertexWeightInfos[i];

                var boneCountProperty = ae.FindPropertyRelative("_boneCount");
                boneCountProperty.intValue = vertexWeightInfo.BoneCount;
                for (var j = 0; j < vertexWeightInfo.BoneCount; j++)
                {
                    var boneElement = ae.FindPropertyRelative($"_b{j}");
                    var boneWeight = vertexWeightInfo[j];
                    boneElement.FindPropertyRelative("_boneIndex").intValue = boneWeight.BoneIndex;
                    boneElement.FindPropertyRelative("_offset").vector3Value = boneWeight.Offset;
                    boneElement.FindPropertyRelative("_weight").floatValue = boneWeight.Weight;
                }
            }

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(targetObject.gameObject);
        }

        /// <summary>
        ///     Get evenly distributed n points in unit sphere surface. <br/>
        ///     http://arxiv.org/pdf/0912.4540
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private static Vector3[] FibonacciAngles(int count)
        {
            var points = new Vector3[count];
            var phi = Mathf.PI * (Mathf.Sqrt(5) - 1);

            for (var i = 0; i < count; i++)
            {
                var y = 1 - ((float) i / (count - 1)) * 2;
                var radius = Mathf.Sqrt(1 - y * y);

                var theta = phi * i;

                points[i] =
                    new Vector3(
                        Mathf.Cos(theta) * radius,
                        y,
                        Mathf.Sin(theta) * radius);
            }

            return points;
        }

        private static int[] ClosestPoints(Vector3[] array, Vector3 point, int count)
        {
            return array
                .Select((bone, index) => (index, distance: Vector3.Distance(bone, point)))
                .OrderBy(pair => pair.distance)
                .Take(count)
                .Select(pair => pair.index)
                .ToArray();
        }

        private void InitializeSpringInfosForLayer(
            int connectCount,
            int layer,
            List<List<Bone>> bonesPerLayer,
            List<Spring> springInfos,
            List<int> springList,
            int colliderLength)
        {
            var currentLayerBones = bonesPerLayer[layer];

            var previousLayerBones = bonesPerLayer[layer - 1];

            for (int i = 0; i < currentLayerBones.Count; i++)
            {
                var colliderIndex = GetColliderIndex(layer, i, bonesPerLayer);

                var currentPosition = currentLayerBones[i].Position;

                var sameLayerIndices = Enumerable.Range(0, currentLayerBones.Count).ToList();
                sameLayerIndices.Remove(i);

                var closestSameLayerIndices = sameLayerIndices
                    .OrderBy(index => Vector3.Distance(currentPosition, currentLayerBones[index].Position))
                    .Take(connectCount)
                    .ToList();

                var belowLayerIndices = Enumerable.Range(0, previousLayerBones.Count).ToList();

                var closestBelowLayerIndex = belowLayerIndices
                    .OrderBy(index => Vector3.Distance(currentPosition, previousLayerBones[index].Position))
                    .First();

                var boneCount = connectCount + 1;

                for (int j = 0; j < connectCount; j++)
                {
                    var connectedColliderIndex = GetColliderIndex(layer, closestSameLayerIndices[j], bonesPerLayer);
                    var connectedPosition = currentLayerBones[closestSameLayerIndices[j]].Position;
                    if (springList[connectedColliderIndex * colliderLength + colliderIndex] == -1)
                    {
                        var restLength = Vector3.Distance(connectedPosition, currentPosition);
                        Spring sp = new Spring(restLength,colliderIndex, connectedColliderIndex);
                        springList[connectedColliderIndex * colliderLength + colliderIndex] =
                            springList[colliderIndex * colliderLength + connectedColliderIndex] =
                                springInfos.Count;

                        springInfos.Add(sp);
                    }
                }

                var connectedBelowColliderIndex = GetColliderIndex(layer - 1, closestBelowLayerIndex, bonesPerLayer);
                var connectedBelowPosition = previousLayerBones[closestBelowLayerIndex].Position;

                if (springList[connectedBelowColliderIndex * colliderLength + colliderIndex] == -1)
                {
                    var restLength = Vector3.Distance(connectedBelowPosition, currentPosition);
                    Spring sp = new Spring(restLength,colliderIndex, connectedBelowColliderIndex);
                    springList[connectedBelowColliderIndex * colliderLength + colliderIndex] =
                        springList[colliderIndex * colliderLength + connectedBelowColliderIndex] =
                            springInfos.Count;

                    springInfos.Add(sp);
                }
            }
        }

        private int GetColliderIndex(int layer, int indexInLayer, List<List<Bone>> collidersPerLayer)
        {
            int colliderIndex = 0;
            for (int i = 0; i < layer; i++)
            {
                colliderIndex += collidersPerLayer[i].Count;
            }
            colliderIndex += indexInLayer;
            return colliderIndex;
        }


    }
}
