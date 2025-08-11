using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

namespace ChronoVoid.Client.Editor
{
    /// <summary>
    /// Unity 6000.2.0b12 Graphics API Optimizer
    /// Helps configure optimal graphics settings for Unity 6
    /// </summary>
    public class Unity6GraphicsOptimizer : EditorWindow
    {
        [MenuItem("ChronoVoid/Unity 6 Graphics Optimizer")]
        public static void ShowWindow()
        {
            GetWindow<Unity6GraphicsOptimizer>("Graphics Optimizer");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("Unity 6 Graphics API Optimizer", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            // Current settings display
            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            var currentAPIs = PlayerSettings.GetGraphicsAPIs(buildTarget);
            
            GUILayout.Label($"Current Build Target: {buildTarget}", EditorStyles.boldLabel);
            GUILayout.Label("Current Graphics APIs:", EditorStyles.boldLabel);
            
            foreach (var api in currentAPIs)
            {
                Color originalColor = GUI.color;
                
                switch (api)
                {
                    case GraphicsDeviceType.Direct3D11:
                        GUI.color = Color.green;
                        GUILayout.Label($"✅ {api} - RECOMMENDED for Unity 6");
                        break;
                    case GraphicsDeviceType.Direct3D12:
                        GUI.color = Color.red;
                        GUILayout.Label($"⚠️ {api} - KNOWN CRASHES in Unity 6000.2.0b12");
                        break;
                    case GraphicsDeviceType.Vulkan:
                        GUI.color = Color.yellow;
                        GUILayout.Label($"⚡ {api} - TEST THOROUGHLY");
                        break;
                    default:
                        GUILayout.Label($"ℹ️ {api}");
                        break;
                }
                
                GUI.color = originalColor;
            }
            
            GUILayout.Space(20);
            
            // Optimization buttons
            GUILayout.Label("Unity 6 Optimizations:", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Optimize for Unity 6 Stability (DirectX11 First)"))
            {
                OptimizeForUnity6Stability();
            }
            
            if (GUILayout.Button("Remove DirectX12 (Avoid Crashes)"))
            {
                RemoveDirectX12();
            }
            
            if (GUILayout.Button("Reset to Unity 6 Recommended Settings"))
            {
                ResetToRecommendedSettings();
            }
            
            GUILayout.Space(20);
            
            // Information box
            EditorGUILayout.HelpBox(
                "Unity 6000.2.0b12 Known Issues:\n" +
                "• DirectX12: Multiple crash scenarios (UUM-107390, UUM-111263)\n" +
                "• Metal on iOS: Player freezes (UUM-111494)\n" +
                "• DirectX11: Recommended for stability\n\n" +
                "This tool helps configure optimal graphics settings for Unity 6.",
                MessageType.Info
            );
        }
        
        private void OptimizeForUnity6Stability()
        {
            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            
            switch (buildTarget)
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    // Set DirectX11 as primary, remove DirectX12
                    PlayerSettings.SetGraphicsAPIs(buildTarget, new GraphicsDeviceType[] 
                    { 
                        GraphicsDeviceType.Direct3D11,
                        GraphicsDeviceType.Vulkan  // Keep Vulkan as fallback
                    });
                    break;
                    
                case BuildTarget.StandaloneOSX:
                    // macOS - Metal is primary but monitor for issues
                    PlayerSettings.SetGraphicsAPIs(buildTarget, new GraphicsDeviceType[] 
                    { 
                        GraphicsDeviceType.Metal,
                        GraphicsDeviceType.OpenGLCore
                    });
                    break;
                    
                case BuildTarget.StandaloneLinux64:
                    // Linux - Vulkan and OpenGL
                    PlayerSettings.SetGraphicsAPIs(buildTarget, new GraphicsDeviceType[] 
                    { 
                        GraphicsDeviceType.Vulkan,
                        GraphicsDeviceType.OpenGLCore
                    });
                    break;
                    
                case BuildTarget.Android:
                    // Android - Keep current settings, they look good
                    Debug.Log("Android graphics APIs are already optimized");
                    break;
                    
                case BuildTarget.iOS:
                    // iOS - Metal with OpenGLES fallback
                    PlayerSettings.SetGraphicsAPIs(buildTarget, new GraphicsDeviceType[] 
                    { 
                        GraphicsDeviceType.Metal,
                        GraphicsDeviceType.OpenGLES3
                    });
                    break;
            }
            
            Debug.Log($"Optimized graphics APIs for {buildTarget} - Unity 6 stability focused");
            EditorUtility.DisplayDialog("Optimization Complete", 
                $"Graphics APIs optimized for Unity 6 stability on {buildTarget}", "OK");
        }
        
        private void RemoveDirectX12()
        {
            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            var currentAPIs = PlayerSettings.GetGraphicsAPIs(buildTarget);
            
            var newAPIs = new System.Collections.Generic.List<GraphicsDeviceType>();
            bool removedDX12 = false;
            
            foreach (var api in currentAPIs)
            {
                if (api != GraphicsDeviceType.Direct3D12)
                {
                    newAPIs.Add(api);
                }
                else
                {
                    removedDX12 = true;
                }
            }
            
            if (removedDX12)
            {
                // Ensure we have at least DirectX11 for Windows
                if (buildTarget == BuildTarget.StandaloneWindows || buildTarget == BuildTarget.StandaloneWindows64)
                {
                    if (!newAPIs.Contains(GraphicsDeviceType.Direct3D11))
                    {
                        newAPIs.Insert(0, GraphicsDeviceType.Direct3D11);
                    }
                }
                
                PlayerSettings.SetGraphicsAPIs(buildTarget, newAPIs.ToArray());
                Debug.Log("Removed DirectX12 to avoid Unity 6 crashes");
                EditorUtility.DisplayDialog("DirectX12 Removed", 
                    "DirectX12 has been removed to avoid known crashes in Unity 6000.2.0b12", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("No Changes", 
                    "DirectX12 was not found in the current graphics APIs", "OK");
            }
        }
        
        private void ResetToRecommendedSettings()
        {
            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            
            switch (buildTarget)
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    PlayerSettings.SetGraphicsAPIs(buildTarget, new GraphicsDeviceType[] 
                    { 
                        GraphicsDeviceType.Direct3D11  // Only DirectX11 for maximum stability
                    });
                    break;
                    
                default:
                    PlayerSettings.SetUseDefaultGraphicsAPIs(buildTarget, true);
                    break;
            }
            
            Debug.Log($"Reset graphics APIs to Unity 6 recommended settings for {buildTarget}");
            EditorUtility.DisplayDialog("Settings Reset", 
                "Graphics APIs reset to Unity 6 recommended settings", "OK");
        }
    }
}