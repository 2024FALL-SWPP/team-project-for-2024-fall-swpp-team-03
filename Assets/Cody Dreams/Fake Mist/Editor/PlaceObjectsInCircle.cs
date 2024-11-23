using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace CodyDreams
{
    namespace Utility
    {
public class PlaceObjectsInCircle : EditorWindow
{
    // Variables for Editor Window
    Transform circleCenter;
    float radius = 5f;
    int objectCount = 10;
    GameObject objectToSpawn;
    float angleOffset = 0f; // Offset in angle for shifting objects
    List<GameObject> spawnedObjects = new List<GameObject>(); // Store spawned objects

    // Create menu item to open the custom window
    [MenuItem("Tools/Place Objects In Circle")]
    public static void ShowWindow()
    {
        GetWindow<PlaceObjectsInCircle>("Place Objects In Circle");
    }

    // GUI layout for the editor window
    void OnGUI()
    {
        GUILayout.Label("Place Objects In Circle", EditorStyles.boldLabel);

        // Input fields for settings
        circleCenter = (Transform)EditorGUILayout.ObjectField("Center Transform", circleCenter, typeof(Transform), true);
        objectToSpawn = (GameObject)EditorGUILayout.ObjectField("Object to Spawn", objectToSpawn, typeof(GameObject), false);

        radius = EditorGUILayout.FloatField("Circle Radius", radius);
        objectCount = EditorGUILayout.IntField("Amount of Objects", objectCount);
        angleOffset = EditorGUILayout.FloatField("Angle Offset", angleOffset); // Add angle offset field

        // Place Objects button
        if (GUILayout.Button("Place Objects"))
        {
            if (circleCenter == null)
            {
                Debug.LogError("Please assign a Center Transform!");
                return;
            }

            if (objectToSpawn == null)
            {
                Debug.LogError("Please assign an Object to Spawn!");
                return;
            }

            PlaceObjects();
        }

        // Delete Objects button
        if (GUILayout.Button("Delete All Spawned Objects"))
        {
            DeleteSpawnedObjects();
        }
    }

    // Function to place objects in a circle
    void PlaceObjects()
    {

        float angleStep = 360f / objectCount;
        Vector3 centerPosition = circleCenter.position;

        for (int i = 0; i < objectCount; i++)
        {
            // Calculate base angle and apply angle offset for random variation
            float angle = i * angleStep + angleOffset;
            float radianAngle = angle * Mathf.Deg2Rad;

            Vector3 spawnPos = new Vector3(
                Mathf.Cos(radianAngle) * radius,
                0f, // Keep the objects on the Y plane
                Mathf.Sin(radianAngle) * radius
            ) + centerPosition;

            // Spawn the object at the calculated position
            GameObject spawnedObject = Instantiate(objectToSpawn, spawnPos, Quaternion.identity);

            // Make the object look at the center (only Y-axis rotation)
            Vector3 direction = (centerPosition - spawnedObject.transform.position).normalized;
            direction.y = 0; // Lock the rotation on the Y-axis
            spawnedObject.transform.rotation = Quaternion.LookRotation(direction);

            // Add spawned object to the list for later deletion
            spawnedObjects.Add(spawnedObject);
        }
    }

    // Function to delete all spawned objects
    void DeleteSpawnedObjects()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
            {
                DestroyImmediate(obj); // Delete the object immediately in the editor
            }
        }

        spawnedObjects.Clear(); // Clear the list after deletion
    }
}
    }
}

