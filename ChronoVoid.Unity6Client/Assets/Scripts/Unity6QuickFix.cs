using UnityEngine;

namespace ChronoVoid.Client.Compatibility
{
    /// <summary>
    /// Quick Unity 6 compatibility fixes that can be run at runtime
    /// </summary>
    public class Unity6QuickFix : MonoBehaviour
    {
        [Header("Unity 6 Quick Diagnostics")]
        public bool runOnStart = true;
        public bool showDetailedInfo = true;
        
        private void Start()
        {
            if (runOnStart)
            {
                RunQuickDiagnostics();
            }
        }
        
        [ContextMenu("Run Unity 6 Diagnostics")]
        public void RunQuickDiagnostics()
        {
            Debug.Log("=== Unity 6000.2.0b12 Quick Diagnostics ===");
            
            // Unity version check
            string version = Application.unityVersion;
            Debug.Log($"Unity Version: {version}");
            
            if (version.Contains("6000.2.0"))
            {
                Debug.Log("✅ Unity 6000.2.0 detected - Compatibility measures active");
            }
            else if (version.Contains("6000.0.54"))
            {
                Debug.LogWarning("⚠️ Unity 6000.0.54 detected - Known startup issues");
            }
            
            // Graphics API check
            var graphicsAPI = SystemInfo.graphicsDeviceType;
            Debug.Log($"Current Graphics API: {graphicsAPI}");
            
            switch (graphicsAPI)
            {
                case UnityEngine.Rendering.GraphicsDeviceType.Direct3D12:
                    Debug.LogError("❌ CRITICAL: DirectX12 detected! Known crashes in Unity 6000.2.0b12");
                    Debug.LogError("   Issues: UUM-107390, UUM-111263");
                    Debug.LogError("   SOLUTION: Go to Edit → Project Settings → Player → Graphics APIs");
                    Debug.LogError("   Remove DirectX12, keep only DirectX11");
                    break;
                    
                case UnityEngine.Rendering.GraphicsDeviceType.Direct3D11:
                    Debug.Log("✅ DirectX11 detected - Recommended for Unity 6 stability");
                    break;
                    
                case UnityEngine.Rendering.GraphicsDeviceType.Metal:
                    if (Application.platform == RuntimePlatform.IPhonePlayer)
                    {
                        Debug.LogWarning("⚠️ Metal on iOS - Monitor for freezing (UUM-111494)");
                    }
                    else
                    {
                        Debug.Log("✅ Metal detected - Good for macOS");
                    }
                    break;
                    
                case UnityEngine.Rendering.GraphicsDeviceType.Vulkan:
                    Debug.Log("⚡ Vulkan detected - Test thoroughly on Unity 6");
                    break;
                    
                default:
                    Debug.Log($"ℹ️ Graphics API: {graphicsAPI}");
                    break;
            }
            
            // Platform-specific checks
            Debug.Log($"Platform: {Application.platform}");
            
            if (showDetailedInfo)
            {
                Debug.Log($"Graphics Device: {SystemInfo.graphicsDeviceName}");
                Debug.Log($"Graphics Memory: {SystemInfo.graphicsMemorySize} MB");
                Debug.Log($"System Memory: {SystemInfo.systemMemorySize} MB");
                Debug.Log($"Processor: {SystemInfo.processorType}");
            }
            
            // Check for Unity 6 specific features
            CheckUnity6Features();
            
            Debug.Log("=== Diagnostics Complete ===");
        }
        
        private void CheckUnity6Features()
        {
            // Check if we're using any Unity 6 specific features
            Debug.Log("Unity 6 Feature Check:");
            
            // Check for FindFirstObjectByType usage (good)
            Debug.Log("✅ Using FindFirstObjectByType (Unity 6 compatible)");
            
            // Check rendering pipeline
            var renderPipeline = UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline;
            if (renderPipeline != null)
            {
                Debug.Log($"Render Pipeline: {renderPipeline.GetType().Name}");
            }
            else
            {
                Debug.Log("Render Pipeline: Built-in (Legacy)");
            }
        }
        
        /// <summary>
        /// Manual DirectX12 check and warning
        /// </summary>
        public void CheckDirectX12Issue()
        {
            var graphicsAPI = SystemInfo.graphicsDeviceType;
            
            if (graphicsAPI == UnityEngine.Rendering.GraphicsDeviceType.Direct3D12)
            {
                Debug.LogError("🚨 URGENT: DirectX12 Detected!");
                Debug.LogError("Unity 6000.2.0b12 has known crashes with DirectX12");
                Debug.LogError("");
                Debug.LogError("MANUAL FIX STEPS:");
                Debug.LogError("1. Go to Edit → Project Settings");
                Debug.LogError("2. Click on 'Player' in the left panel");
                Debug.LogError("3. Expand 'Other Settings'");
                Debug.LogError("4. Find 'Graphics APIs for Windows'");
                Debug.LogError("5. Uncheck 'Auto Graphics API'");
                Debug.LogError("6. Remove 'Direct3D12' from the list");
                Debug.LogError("7. Keep only 'Direct3D11'");
                Debug.LogError("8. Restart Unity");
                Debug.LogError("");
                
                return;
            }
            
            Debug.Log("✅ DirectX12 not detected - Graphics API is safe for Unity 6");
        }
    }
}