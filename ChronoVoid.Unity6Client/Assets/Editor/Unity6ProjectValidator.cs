using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ChronoVoid.Client.Editor
{
    /// <summary>
    /// Unity 6000.2.0b12 Project Validation Tool
    /// Checks project settings for Unity 6 compatibility
    /// </summary>
    public class Unity6ProjectValidator : EditorWindow
    {
        private Vector2 scrollPosition;
        private List<ValidationResult> validationResults = new List<ValidationResult>();
        
        [MenuItem("ChronoVoid/Unity 6 Project Validator")]
        public static void ShowWindow()
        {
            GetWindow<Unity6ProjectValidator>("Unity 6 Validator");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("Unity 6000.2.0b12 Project Validator", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            if (GUILayout.Button("Run Validation"))
            {
                RunValidation();
            }
            
            GUILayout.Space(10);
            
            if (validationResults.Count > 0)
            {
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                
                foreach (var result in validationResults)
                {
                    DrawValidationResult(result);
                }
                
                EditorGUILayout.EndScrollView();
            }
        }
        
        private void DrawValidationResult(ValidationResult result)
        {
            Color originalColor = GUI.color;
            
            switch (result.severity)
            {
                case ValidationSeverity.Error:
                    GUI.color = Color.red;
                    break;
                case ValidationSeverity.Warning:
                    GUI.color = Color.yellow;
                    break;
                case ValidationSeverity.Info:
                    GUI.color = Color.green;
                    break;
            }
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField($"[{result.severity}] {result.title}", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(result.description, EditorStyles.wordWrappedLabel);
            
            if (!string.IsNullOrEmpty(result.recommendation))
            {
                EditorGUILayout.LabelField("Recommendation:", EditorStyles.miniBoldLabel);
                EditorGUILayout.LabelField(result.recommendation, EditorStyles.wordWrappedMiniLabel);
            }
            
            EditorGUILayout.EndVertical();
            GUI.color = originalColor;
            GUILayout.Space(5);
        }
        
        private void RunValidation()
        {
            validationResults.Clear();
            
            // Check Unity version
            CheckUnityVersion();
            
            // Check graphics API settings
            CheckGraphicsAPIs();
            
            // Check for problematic components
            CheckForProblematicComponents();
            
            // Check project settings
            CheckProjectSettings();
            
            // Check for deprecated API usage
            CheckDeprecatedAPIs();
            
            Debug.Log($"Unity 6 validation complete. Found {validationResults.Count} items.");
        }
        
        private void CheckUnityVersion()
        {
            string version = Application.unityVersion;
            
            if (version.Contains("6000.2.0"))
            {
                validationResults.Add(new ValidationResult
                {
                    severity = ValidationSeverity.Info,
                    title = "Unity Version Compatible",
                    description = $"Using Unity {version} - Compatible with Unity 6000.2.0b12 guidelines",
                    recommendation = "Continue monitoring Unity release notes for updates"
                });
            }
            else if (version.Contains("6000.0.54"))
            {
                validationResults.Add(new ValidationResult
                {
                    severity = ValidationSeverity.Warning,
                    title = "Unity Version Issue",
                    description = $"Using Unity {version} - Known startup issues with 6000.0.54f1",
                    recommendation = "Upgrade to Unity 6000.2.0b12 or later for better stability"
                });
            }
            else
            {
                validationResults.Add(new ValidationResult
                {
                    severity = ValidationSeverity.Warning,
                    title = "Unity Version Unknown",
                    description = $"Using Unity {version} - Compatibility with Unity 6 breaking changes unknown",
                    recommendation = "Review breaking changes document and test thoroughly"
                });
            }
        }
        
        private void CheckGraphicsAPIs()
        {
            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            var graphicsAPIs = PlayerSettings.GetGraphicsAPIs(buildTarget);
            
            foreach (var api in graphicsAPIs)
            {
                switch (api)
                {
                    case UnityEngine.Rendering.GraphicsDeviceType.Direct3D12:
                        validationResults.Add(new ValidationResult
                        {
                            severity = ValidationSeverity.Warning,
                            title = "DirectX12 Graphics API",
                            description = "DirectX12 has known crash issues in Unity 6000.2.0b12",
                            recommendation = "Consider using DirectX11 for development until issues are resolved"
                        });
                        break;
                        
                    case UnityEngine.Rendering.GraphicsDeviceType.Direct3D11:
                        validationResults.Add(new ValidationResult
                        {
                            severity = ValidationSeverity.Info,
                            title = "DirectX11 Graphics API",
                            description = "DirectX11 is recommended for Unity 6 stability",
                            recommendation = "Good choice for Unity 6 development"
                        });
                        break;
                }
            }
        }
        
        private void CheckForProblematicComponents()
        {
            // Check for Rigidbody components in prefabs
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
            int rigidbodyPrefabs = 0;
            
            foreach (string guid in prefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                
                if (prefab != null && (prefab.GetComponent<Rigidbody>() != null || prefab.GetComponentInChildren<Rigidbody>() != null))
                {
                    rigidbodyPrefabs++;
                }
            }
            
            if (rigidbodyPrefabs > 0)
            {
                validationResults.Add(new ValidationResult
                {
                    severity = ValidationSeverity.Warning,
                    title = "Rigidbody Prefabs Found",
                    description = $"Found {rigidbodyPrefabs} prefabs with Rigidbody components",
                    recommendation = "Use Unity6Compatibility.SafeInstantiate() when instantiating these prefabs after AssetBundle loading"
                });
            }
            
            // Check for AnimatorControllers
            string[] animatorGuids = AssetDatabase.FindAssets("t:AnimatorController");
            if (animatorGuids.Length > 0)
            {
                validationResults.Add(new ValidationResult
                {
                    severity = ValidationSeverity.Info,
                    title = "Animator Controllers Found",
                    description = $"Found {animatorGuids.Length} AnimatorController assets",
                    recommendation = "Monitor animation loop counts to avoid issues with high normalized times (>100,000 loops)"
                });
            }
        }
        
        private void CheckProjectSettings()
        {
            // Check color space
            if (PlayerSettings.colorSpace == ColorSpace.Linear)
            {
                validationResults.Add(new ValidationResult
                {
                    severity = ValidationSeverity.Info,
                    title = "Linear Color Space",
                    description = "Using Linear color space - good for modern rendering",
                    recommendation = "Continue using Linear color space for better visual quality"
                });
            }
            
            // Check scripting backend
            var scriptingBackend = PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (scriptingBackend == ScriptingImplementation.IL2CPP)
            {
                validationResults.Add(new ValidationResult
                {
                    severity = ValidationSeverity.Info,
                    title = "IL2CPP Scripting Backend",
                    description = "Using IL2CPP - recommended for performance",
                    recommendation = "Good choice for production builds"
                });
            }
        }
        
        private void CheckDeprecatedAPIs()
        {
            // This would require more complex analysis of scripts
            // For now, just check if Unity6Compatibility is being used
            string[] scriptGuids = AssetDatabase.FindAssets("t:MonoScript");
            bool hasCompatibilityScript = false;
            
            foreach (string guid in scriptGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains("Unity6Compatibility"))
                {
                    hasCompatibilityScript = true;
                    break;
                }
            }
            
            if (hasCompatibilityScript)
            {
                validationResults.Add(new ValidationResult
                {
                    severity = ValidationSeverity.Info,
                    title = "Unity 6 Compatibility Script Found",
                    description = "Unity6Compatibility.cs is present in the project",
                    recommendation = "Use compatibility methods for safe instantiation and AssetBundle operations"
                });
            }
        }
        
        private struct ValidationResult
        {
            public ValidationSeverity severity;
            public string title;
            public string description;
            public string recommendation;
        }
        
        private enum ValidationSeverity
        {
            Info,
            Warning,
            Error
        }
    }
}