using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Editor;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SWPPT3.SoftbodyPhysics.Editor.SoftbodyInspector
{
    [CustomEditor(typeof(SimpleSoftbody))]
    public class SoftbodyInspector : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/SWPPT3/Scripts/SoftbodyPhysics/Editor/SoftbodyInspector/SoftbodyInspector.uxml");
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/SWPPT3/Scripts/SoftbodyPhysics/Editor/SoftbodyInspector/SoftbodyInspector.uss");
            VisualElement tree = visualTree.Instantiate();
            tree.styleSheets.Add(styleSheet);

            var initializeButton = tree.Q<Button>(name = "InitializeButton");

            initializeButton.clicked += () => OnClickInitialize().Forget();


            return tree;
        }

        private async UniTaskVoid OnClickInitialize()
        {
            Debug.Log("[Softbody] Initialize called.");

            var targetObject = target as SimpleSoftbody;
            var meshFilter = serializedObject.FindProperty("_meshFilter").objectReferenceValue as MeshFilter;
            // var collidersRoot = serializedObject.FindProperty("_collidersRoot").objectReferenceValue as Transform;
            // var colliders = serializedObject.FindProperty("_colliders");
            // var vertexWeights = serializedObject.FindProperty("_vertexWeights");

            if (meshFilter == null)
            {
                Debug.LogError("meshFilter is null.");
                return;
            }

            System.Diagnostics.Debug.Assert(targetObject != null, nameof(targetObject) + " != null");

            var mesh = meshFilter.sharedMesh;
            var vertices = mesh.vertices;

            foreach (var v in vertices)
            {
                var go = new GameObject("Particle");
                go.transform.SetParent(targetObject.transform);
                go.transform.localPosition = v;
            }
        }
    }
}
