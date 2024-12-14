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
            var vertexWeights = serializedObject.FindProperty("_vertexWeights");
            var springs = serializedObject.FindProperty("_springs");
            var springListProperty = serializedObject.FindProperty("_springList");
            var bonesProperty = serializedObject.FindProperty("_bones");
            var connectedCount = serializedObject.FindProperty("_connectedCount");
            var centerObjProperty = serializedObject.FindProperty("_centerObj");
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

            Debug.Log("clear start");

            // Clear
            var childCount = collidersRoot.childCount;
            for (var i = childCount - 1; i >= 0; i--)
            {
                var child = collidersRoot.GetChild(i);
                DestroyImmediate(child.gameObject);
            }

            var springJoints = targetObject.GetComponents<SpringJoint>();
            foreach (var s in springJoints)
            {
                DestroyImmediate(s);
            }
            var list = new List<Vector3>();
            meshFilter.mesh.GetVertices(list);
            bonesProperty.ClearArray();
            var bones = new List<GameObject>();

            var boneIndex = 0;

            for (int i = 0; i < list.Count; i++)
            {
                var go = new GameObject($"Collider {boneIndex}", typeof(SphereCollider), typeof(Rigidbody));
                go.transform.SetParent(collidersRoot, false);
                go.layer = LayerMask.NameToLayer("OuterSphere");
                var rb = go.GetComponent<Rigidbody>();
                rb.mass = mass / list.Count;
                //rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                var sc = go.GetComponent<SphereCollider>();
                sc.radius = targetObject.ColliderRadius;
                go.transform.localPosition = list[i];
                bones.Add(go);
                bonesProperty.InsertArrayElementAtIndex(boneIndex);
                bonesProperty.GetArrayElementAtIndex(boneIndex).objectReferenceValue = go;
                boneIndex++;
            }

            var springList = new List<int>(boneIndex * boneIndex);
            for (var i = 0; i < boneIndex * boneIndex; i++)
            {
                springList.Add(-1);
            }

            var springsInfo = new List<Spring>();
            var triangles = meshFilter.mesh.triangles;

            for (int i = 0; i < triangles.Length; i += 3)
            {
                var index0 = triangles[i];
                var index1 = triangles[i + 1];
                var index2 = triangles[i + 2];

                AddSpring(springsInfo, springList, list, index0, index1, boneIndex);
                AddSpring(springsInfo, springList, list, index1, index2, boneIndex);
                AddSpring(springsInfo, springList, list, index2, index0, boneIndex);
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

            connectedCount.intValue = maxCount + 1;
            Debug.Log($"connectedCount {connectedCount.intValue}");
            var newSpringList = new List<int>(boneIndex * connectedCount.intValue);
            for (var i = 0; i < boneIndex * connectedCount.intValue; i++)
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
                        newSpringList[i * connectedCount.intValue + currentCount] = value;
                        currentCount++;
                    }
                }
            }

            var centerOfMass = Vector3.zero;
            for (var i = 0; i < list.Count; i++)
            {
                centerOfMass += list[i];
            }
            centerOfMass = centerOfMass / list.Count;

            var centerObj = new GameObject("centerOfMass");
            centerObj.transform.SetParent(collidersRoot, false);
            var centerRb = centerObj.AddComponent<Rigidbody>();
            var centerSc = centerObj.AddComponent<SphereCollider>();
            centerObj.layer = LayerMask.NameToLayer("InnerSphere");
            centerSc.radius = targetObject.ColliderRadius;
            centerObj.transform.localPosition = centerOfMass;

            centerObjProperty.objectReferenceValue = centerObj;

            for (int i = 0; i < list.Count; i++)
            {
                var distance = Vector3.Distance(list[i], centerOfMass);
                Spring sp = new Spring(distance, i, bones.Count);
                springsInfo.Add(sp);
                var springIndex = springsInfo.Count - 1;
                newSpringList[i * connectedCount.intValue + connectedCount.intValue -1] = springIndex;
            }


            springListProperty.ClearArray();

            for (var i = 0; i < newSpringList.Count; i++)
            {
                springListProperty.InsertArrayElementAtIndex(i);
                springListProperty.GetArrayElementAtIndex(i).intValue = newSpringList[i];
            }

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
            var vertexWeightInfos = new VertexWeightInfo[mesh.vertexCount];


            for (var i = 0; i < vertices.Length; i++)
            {
                var vertexWeightInfo = new VertexWeightInfo(1);
                var bws = new BoneWeightInfo();

                bws.BoneIndex = i;
                bws.Offset = Vector3.zero;
                bws.Weight = 1f;

                vertexWeightInfo[0] = bws;
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
                var boneElement = ae.FindPropertyRelative($"_b0");
                var boneWeight = vertexWeightInfo[0];
                boneElement.FindPropertyRelative("_boneIndex").intValue = boneWeight.BoneIndex;
                boneElement.FindPropertyRelative("_offset").vector3Value = boneWeight.Offset;
                boneElement.FindPropertyRelative("_weight").floatValue = boneWeight.Weight;
            }

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(targetObject.gameObject);
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
        private static void AddSpring(List<Spring> springsInfo, List<int> springList, List<Vector3> list, int index0, int index1, int boneCount)
        {
            if (springList[index0 * boneCount + index1] == -1 && springList[index1 * boneCount + index0] == -1)
            {
                var distance = Vector3.Distance(list[index0], list[index1]);
                Spring sp = new Spring(distance, index0, index1);
                springList[index0 * boneCount + index1] =  springsInfo.Count;
                springList[index1 * boneCount + index0] =  springsInfo.Count;
                springsInfo.Add(sp);
            }
        }
    }
}
