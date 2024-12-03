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

            var pointDensityMultiplierProperty = serializedObject.FindProperty("_pointDensityMultiplier");
            var totalLayersProperty = serializedObject.FindProperty("_totalLayers");

            var boneCountProperty = serializedObject.FindProperty("_boneCount");

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

            var radiusStep = (Softbody.SphereRadius - targetObject.ColliderRadius) / totalLayers;
            var totalBoneCount = 1;
            for (var layer = 1; layer < totalLayers; layer++)
            {
                var numPoints = pointDensityMultiplier * layer * layer;
                totalBoneCount += numPoints;
            }
            var bones = new Vector3[totalBoneCount];

            int boneIndex = 0;

            for (var layer = 0; layer < totalLayers; layer++)
            {
                if (layer == 0)
                {
                    var go = new GameObject($"Collider {boneIndex}", typeof(SphereCollider));
                    go.layer = LayerMask.NameToLayer("InnerSphere");
                    go.transform.SetParent(collidersRoot, false);
                    go.transform.localPosition = Vector3.zero;

                    var sc = go.GetComponent<SphereCollider>();
                    sc.radius = targetObject.ColliderRadius;

                    colliders.InsertArrayElementAtIndex(boneIndex);
                    colliders.GetArrayElementAtIndex(boneIndex).objectReferenceValue = sc;

                    boneIndex++;
                    continue;
                }

                var layerRadius = radiusStep * layer + targetObject.ColliderRadius;
                var numPoints = pointDensityMultiplier * layer * layer;

                var fibAngles = FibonacciAngles(numPoints);

                foreach (var angle in fibAngles)
                {
                    var bonePosition = angle * layerRadius;
                    bones[boneIndex] = bonePosition;

                    GameObject go;

                    if (layer == totalLayers - 1)
                    {
                        go = new GameObject($"Collider {boneIndex}", typeof(SphereCollider), typeof(Rigidbody));
                        go.layer = LayerMask.NameToLayer("OuterSphere");
                    }
                    else
                    {
                        go = new GameObject($"Collider {boneIndex}", typeof(SphereCollider));
                        go.layer = LayerMask.NameToLayer("InnerSphere");
                    }

                    go.transform.SetParent(collidersRoot, false);

                    var sc = go.GetComponent<SphereCollider>();
                    sc.radius = targetObject.ColliderRadius;

                    go.transform.localPosition = bonePosition;

                    colliders.InsertArrayElementAtIndex(boneIndex);
                    colliders.GetArrayElementAtIndex(boneIndex).objectReferenceValue = sc;

                    boneIndex++;
                }
            }


            Debug.Log("[Softbody] Done setting colliders");

            var mesh = meshFilter.sharedMesh;

            var vertices = mesh.vertices;

            var surfaceBonesCount = pointDensityMultiplier * (totalLayers+1) * (totalLayers+1);
            var vertexCount = mesh.vertexCount;

            var boneCount = Mathf.Clamp((surfaceBonesCount * 4) / vertexCount, 4, 8);
            boneCountProperty.intValue = boneCount;

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
    }
}
