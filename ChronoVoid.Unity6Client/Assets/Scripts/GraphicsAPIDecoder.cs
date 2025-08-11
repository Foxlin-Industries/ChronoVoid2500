using UnityEngine;

namespace ChronoVoid.Client.Compatibility
{
    /// <summary>
    /// Helper to decode and validate graphics API settings
    /// </summary>
    public static class GraphicsAPIDecoder
    {
        /// <summary>
        /// Decode the graphics API hex values from ProjectSettings
        /// Windows Standalone APIs: 0200000012000000
        /// </summary>
        public static void DecodeCurrentAPIs()
        {
            Debug.Log("=== Graphics API Analysis ===");
            
            // Current runtime API
            var currentAPI = SystemInfo.graphicsDeviceType;
            Debug.Log($"Current Graphics API: {currentAPI}");
            
            // Decode the hex values from ProjectSettings
            // 0200000012000000 = [0x02000000, 0x12000000]
            // 0x02000000 = 33554432 = Direct3D11
            // 0x12000000 = 301989888 = Direct3D12
            
            Debug.Log("Configured APIs for Windows Standalone:");
            Debug.Log("- Direct3D11 (0x02000000) - ✅ RECOMMENDED for Unity 6");
            Debug.Log("- Direct3D12 (0x12000000) - ⚠️ WARNING: Known crashes in Unity 6000.2.0b12");
            
            // Validate current choice
            ValidateCurrentAPI(currentAPI);
        }
        
        private static void ValidateCurrentAPI(UnityEngine.Rendering.GraphicsDeviceType api)
        {
            switch (api)
            {
                case UnityEngine.Rendering.GraphicsDeviceType.Direct3D11:
                    Debug.Log("✅ EXCELLENT: DirectX11 is the recommended API for Unity 6 stability");
                    break;
                    
                case UnityEngine.Rendering.GraphicsDeviceType.Direct3D12:
                    Debug.LogWarning("⚠️ CAUTION: DirectX12 has known crash issues in Unity 6000.2.0b12");
                    Debug.LogWarning("   Issues: UUM-107390, UUM-111263 - Crashes during rendering operations");
                    Debug.LogWarning("   Recommendation: Switch to DirectX11 for development");
                    break;
                    
                case UnityEngine.Rendering.GraphicsDeviceType.Vulkan:
                    Debug.Log("ℹ️ INFO: Vulkan support improved in Unity 6, but test thoroughly");
                    break;
                    
                default:
                    Debug.Log($"ℹ️ INFO: Using {api} - Monitor for Unity 6 compatibility issues");
                    break;
            }
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void AutoAnalyze()
        {
            DecodeCurrentAPIs();
        }
    }
}