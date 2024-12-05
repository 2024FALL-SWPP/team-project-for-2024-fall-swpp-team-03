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
            var colliders = serializedObject.FindProperty("_colliders");
            var vertexWeights = serializedObject.FindProperty("_vertexWeights");
            var springs = serializedObject.FindProperty("_springs");
            var springListProperty = serializedObject.FindProperty("_springList");

            var pointDensityMultiplierProperty = serializedObject.FindProperty("_pointDensityMultiplier");
            var totalLayersProperty = serializedObject.FindProperty("_totalLayers");

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
            colliders.ClearArray();
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


            // var fibAngles = FibonacciAngles(30);
            // var bones = new Vector3[30];

            var radiusStep = (Softbody.SphereRadius - targetObject.ColliderRadius) / (totalLayers - 1);
            var totalBoneCount = 1;
            for (var layer = 1; layer < totalLayers; layer++)
            {
                var numPoints = pointDensityMultiplier * layer * layer;
                totalBoneCount += numPoints;
            }
            var bones = new Vector3[totalBoneCount];

            var collidersPerLayer = new List<List<Collider>>();
            var bonePositionsPerLayer = new List<List<Vector3>>();

            springs.ClearArray();

            var springsInfo = new List<Spring>();

            int boneIndex = 0;

            for (var layer = 0; layer < totalLayers; layer++)
            {
                var currentLayerColliders = new List<Collider>();
                var currentLayerPositions = new List<Vector3>();
                if (layer == 0)
                {
                    // var go = new GameObject($"Collider {boneIndex}", typeof(SphereCollider));
                    var go = new GameObject($"Collider {boneIndex}", typeof(SphereCollider), typeof(Rigidbody));
                    go.layer = LayerMask.NameToLayer("InnerSphere");
                    go.transform.SetParent(collidersRoot, false);
                    go.transform.localPosition = Vector3.zero;

                    var sc = go.GetComponent<SphereCollider>();
                    sc.radius = targetObject.ColliderRadius;

                    colliders.InsertArrayElementAtIndex(boneIndex);
                    colliders.GetArrayElementAtIndex(boneIndex).objectReferenceValue = sc;

                    bones[boneIndex] = Vector3.zero;
                    currentLayerColliders.Add(sc);
                    currentLayerPositions.Add(Vector3.zero);

                    boneIndex++;
                }
                else
                {
                    // var layerRadius = radiusStep * layer + targetObject.ColliderRadius;
                    var numPoints = pointDensityMultiplier * layer * layer;
                    var layerRadius = radiusStep * layer;

                    var fibAngles = FibonacciAngles(numPoints);

                    foreach (var angle in fibAngles)
                    {
                        var bonePosition = angle * layerRadius;
                        bones[boneIndex] = bonePosition;
                        currentLayerPositions.Add(bonePosition);

                        GameObject go;

                        if (layer == totalLayers - 1)
                        {
                            go = new GameObject($"Collider {boneIndex}", typeof(SphereCollider), typeof(Rigidbody));
                            go.layer = LayerMask.NameToLayer("OuterSphere");
                        }
                        else
                        {
                            // go = new GameObject($"Collider {boneIndex}", typeof(SphereCollider));
                            go = new GameObject($"Collider {boneIndex}", typeof(SphereCollider), typeof(Rigidbody));
                            go.layer = LayerMask.NameToLayer("InnerSphere");

                        }

                        go.transform.SetParent(collidersRoot, false);

                        var sc = go.GetComponent<SphereCollider>();
                        sc.radius = targetObject.ColliderRadius;

                        go.transform.localPosition = bonePosition;

                        colliders.InsertArrayElementAtIndex(boneIndex);
                        colliders.GetArrayElementAtIndex(boneIndex).objectReferenceValue = sc;

                        currentLayerColliders.Add(sc);


                        boneIndex++;
                    }
                }

                collidersPerLayer.Add(currentLayerColliders);
                bonePositionsPerLayer.Add(currentLayerPositions);
            }

            var colliderLength = colliders.arraySize;
            var springList = new List<int>(colliderLength * colliderLength);

            for (var i = 0; i < colliderLength*colliderLength; i++)
            {
                springList.Add(-1);
            }

            for (int i = 1; i < totalLayers; i++)
            {
                InitializeSpringInfosForLayer(
                    pointDensityMultiplier-1,
                    i,
                    collidersPerLayer,
                    bonePositionsPerLayer,
                    springsInfo,
                    springList,
                    colliderLength
                );
            }

            springListProperty.ClearArray();
            for (var i = 0; i < springList.Count; i++)
            {
                springListProperty.InsertArrayElementAtIndex(i);
                springListProperty.GetArrayElementAtIndex(i).intValue = springList[i];
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


            Debug.Log(colliders.arraySize);
            Debug.Log("[Softbody] Done setting colliders");

            var mesh = meshFilter.sharedMesh;

            var vertices = mesh.vertices;

            var surfaceBonesCount = pointDensityMultiplier * (totalLayers - 1) * (totalLayers - 1);
            Debug.Log(surfaceBonesCount);
            var vertexCount = mesh.vertexCount;

            var boneCount = Mathf.Clamp((surfaceBonesCount * 4) / vertexCount, 4, 8);

            var vertexWeightInfos = new VertexWeightInfo[mesh.vertexCount];

            var outerLayerStartIndex = bones.Length - (pointDensityMultiplier * (totalLayers - 1) * (totalLayers - 1));
            var outerBones = bones.Skip(outerLayerStartIndex).ToArray();

            // await UniTask.Delay(0);

            for (var i = 0; i < vertices.Length; i++)
            {
                var v = vertices[i];

                // var points = ClosestPoints(bones, v, boneCount);
                var points = ClosestPoints(outerBones, v, boneCount);

                var bws = new BoneWeightInfo[boneCount];

                for (var j = 0; j < bws.Length; j++)
                {
                    bws[j].BoneIndex = points[j] + outerLayerStartIndex;
                    bws[j].Offset = v - bones[points[j] + outerLayerStartIndex];
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
            List<List<Collider>> collidersPerLayer,
            List<List<Vector3>> bonePositionsPerLayer,
            List<Spring> springInfos,
            List<int> springList,
            int colliderLength)
        {
            var currentLayerColliders = collidersPerLayer[layer];
            var currentLayerPositions = bonePositionsPerLayer[layer];

            var previousLayerColliders = collidersPerLayer[layer - 1];
            var previousLayerPositions = bonePositionsPerLayer[layer - 1];

            for (int i = 0; i < currentLayerColliders.Count; i++)
            {
                var colliderIndex = GetColliderIndex(layer, i, collidersPerLayer);

                var currentPosition = currentLayerPositions[i];

                var sameLayerPositions = currentLayerPositions;
                var sameLayerIndices = Enumerable.Range(0, sameLayerPositions.Count).ToList();
                sameLayerIndices.Remove(i);

                var closestSameLayerIndices = sameLayerIndices
                    .OrderBy(index => Vector3.Distance(currentPosition, sameLayerPositions[index]))
                    .Take(connectCount)
                    .ToList();

                var belowLayerPositions = previousLayerPositions;
                var belowLayerIndices = Enumerable.Range(0, belowLayerPositions.Count).ToList();

                var closestBelowLayerIndex = belowLayerIndices
                    .OrderBy(index => Vector3.Distance(currentPosition, belowLayerPositions[index]))
                    .First();

                var boneCount = connectCount + 1;

                for (int j = 0; j < connectCount; j++)
                {
                    var connectedColliderIndex = GetColliderIndex(layer, closestSameLayerIndices[j], collidersPerLayer);
                    var connectedPosition = bonePositionsPerLayer[layer][closestSameLayerIndices[j]];
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

                var connectedBelowColliderIndex = GetColliderIndex(layer - 1, closestBelowLayerIndex, collidersPerLayer);
                var connectedBelowPosition = bonePositionsPerLayer[layer - 1][closestBelowLayerIndex];

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

        private int GetColliderIndex(int layer, int indexInLayer, List<List<Collider>> collidersPerLayer)
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
