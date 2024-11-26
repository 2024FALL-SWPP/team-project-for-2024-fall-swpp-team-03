using UnityEditor;
using UnityEngine;

namespace CodyDreams
{
    namespace Developer
    {
        [InitializeOnLoad]
        public class FeedbackWindow : EditorWindow
        {
            private const string AssetStoreFeedbackUrl = "https://assetstore.unity.com/packages/tools/particles-effects/fake-fog-296903"; // Replace with your asset store URL
            private const float WindowWidth = 600f;  // Fixed width
            private const float WindowHeight = 300f; // Fixed height
            private static FeedbackWindow window;
            private static bool showOnStartup;

            // Static constructor for initializing static members
            static FeedbackWindow()
            {
                // Read preference immediately upon load
                showOnStartup = EditorPrefs.GetBool("ShowFeedbackWindowOnStartup", true);
                // Show the feedback window on startup based on preference
                if (showOnStartup)
                {
                    ShowWindow();
                }
            }

            private void OnEnable()
            {
                // Ensure the preference is up-to-date
                showOnStartup = EditorPrefs.GetBool("ShowFeedbackWindowOnStartup", true);
            }

            [MenuItem("Window/Feedback Window")]
            public static void ShowWindow()
            {
                if (window == null)
                {
                    window = GetWindow<FeedbackWindow>("Feedback");
                }
                else
                {
                    FocusWindowIfItsOpen<FeedbackWindow>();
                }

                // Set window size constraints
                window.minSize = new Vector2(WindowWidth, WindowHeight);
                window.maxSize = new Vector2(WindowWidth, WindowHeight);
            }

            private void OnGUI()
            {
                // Centering the content both vertically and horizontally
                EditorGUILayout.BeginVertical(GUILayout.Width(WindowWidth), GUILayout.Height(WindowHeight));
                GUILayout.FlexibleSpace(); // Push content to center vertically

                // Centering the label and button
                GUILayout.Label("We'd love your feedback!", EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
                GUILayout.Label("Please take a moment to provide feedback on this asset.", GUILayout.ExpandWidth(true));

                if (GUILayout.Button("Give Feedback", GUILayout.ExpandWidth(true)))
                {
                    OpenFeedbackUrl();
                }

                GUILayout.Space(10); // Space before the toggle

                // Display and update the toggle for showing the window on startup
                bool newShowOnStartup = EditorGUILayout.Toggle("Show this window again", showOnStartup, GUILayout.ExpandWidth(true));
                if (newShowOnStartup != showOnStartup)
                {
                    // Save the new state if it has changed
                    showOnStartup = newShowOnStartup;
                    EditorPrefs.SetBool("ShowFeedbackWindowOnStartup", showOnStartup);
                }

                GUILayout.FlexibleSpace(); // Push content to center vertically
                EditorGUILayout.EndVertical();
            }

            private void OpenFeedbackUrl()
            {
                Application.OpenURL(AssetStoreFeedbackUrl);
            }

            private void OnDestroy()
            {
                // Save the preference when the window is closed
                EditorPrefs.SetBool("ShowFeedbackWindowOnStartup", showOnStartup);
            }
        }
    }
}
