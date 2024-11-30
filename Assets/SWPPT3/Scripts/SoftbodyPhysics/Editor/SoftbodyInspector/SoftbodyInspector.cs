using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Editor;
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
            var bones = new List<Vector3>();
            Debug.Log(radiusStep);

            for (var layer = 0; layer < totalLayers; layer++)
            {
                var layerRadius = radiusStep * layer + targetObject.ColliderRadius;
                var numPoints = pointDensityMultiplier * layer * layer;

                var fibAngles = FibonacciAngles(numPoints);

                foreach (var angle in fibAngles)
                {
                    var bonePosition = angle * layerRadius;
                    bones.Add(bonePosition);

                    if (layer == totalLayers - 1)
                    {
                        var go = new GameObject($"Collider {bones.Count - 1}", typeof(SphereCollider), typeof(Rigidbody));
                        go.transform.SetParent(collidersRoot, false);

                        var sc = go.GetComponent<SphereCollider>();
                        sc.radius = targetObject.ColliderRadius;

                        go.transform.localPosition = bonePosition;

                        colliders.InsertArrayElementAtIndex(colliders.arraySize);
                        colliders.GetArrayElementAtIndex(colliders.arraySize - 1).objectReferenceValue = sc;

                        var rb = go.GetComponent<Rigidbody>();
                        rb.freezeRotation = true;
                    }
                    else
                    {
                        var point = new GameObject($"Point {bones.Count - 1}");
                        point.transform.SetParent(collidersRoot, false);
                        point.transform.localPosition = bonePosition;
                    }
                }
            }


            Debug.Log("[Softbody] Done setting colliders");

            // var mesh = meshFilter.sharedMesh;
            //
            // var vertices = mesh.vertices;
            //
            // var vertexWeightInfos = new VertexWeightInfo[mesh.vertexCount];
            //
            // // await UniTask.Delay(0);
            //
            // for (var i = 0; i < vertices.Length; i++)
            // {
            //     // await UniTask.Delay(0);
            //     var v = vertices[i];
            //     var points = Closest4Points(bones, v);
            //
            //     var bws = new BoneWeightInfo[4];
            //
            //     for (var j = 0; j < bws.Length; j++)
            //     {
            //         bws[j].BoneIndex = points[j];
            //         bws[j].Offset = v - bones[points[j]];
            //     }
            //
            //     var offsetMagSum = bws.Sum(bw => bw.Offset.magnitude);
            //
            //     for (var j = 0; j < bws.Length; j++)
            //     {
            //         bws[j].Weight = bws[j].Offset.magnitude / offsetMagSum;
            //         vertexWeightInfos[i][j] = bws[j];
            //     }
            // }
            //
            // vertexWeights.ClearArray();
            //
            // for (var i = 0; i < vertexWeightInfos.Length; i++)
            // {
            //     vertexWeights.InsertArrayElementAtIndex(i);
            //     var ae = vertexWeights.GetArrayElementAtIndex(i);
            //     var arr = new[]
            //     {
            //         ae.FindPropertyRelative("_b0"),
            //         ae.FindPropertyRelative("_b1"),
            //         ae.FindPropertyRelative("_b2"),
            //         ae.FindPropertyRelative("_b3"),
            //     };
            //
            //     for (var j = 0; j < 4; j++)
            //     {
            //         arr[j].FindPropertyRelative("_boneIndex").intValue = vertexWeightInfos[i][j].BoneIndex;
            //         arr[j].FindPropertyRelative("_offset").vector3Value = vertexWeightInfos[i][j].Offset;
            //         arr[j].FindPropertyRelative("_weight").floatValue = vertexWeightInfos[i][j].Weight;
            //     }
            // }

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

        private static int[] Closest4Points(Vector3[] array, Vector3 point)
        {
            var sorting = new SortedList<float, SortedSet<int>>();

            for (var i = 0; i < array.Length; i++)
            {
                var mag = (point - array[i]).magnitude;

                if (!sorting.ContainsKey(mag))
                {
                    sorting.Add(mag, new SortedSet<int>());
                }

                sorting[mag].Add(i);
            }

            var flatten = sorting.SelectMany(s => s.Value.AsEnumerable());

            return flatten.Take(4).ToArray();
        }
    }
}
