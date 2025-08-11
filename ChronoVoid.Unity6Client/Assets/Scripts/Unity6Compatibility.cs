using UnityEngine;
using System.Collections;

namespace ChronoVoid.Client.Compatibility
{
    /// <summary>
    /// Unity 6000.2.0b12 Compatibility Helper
    /// Addresses known issues and breaking changes in Unity 6 beta
    /// </summary>
    public static class Unity6Compatibility
    {
        /// <summary>
        /// Safe instantiation of prefabs with Rigidbody components
        /// Addresses UUM-108799: Crash when instantiating Rigidbody prefabs after AssetBundle loading
        /// </summary>
        public static GameObject SafeInstantiate(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            // Check if prefab has Rigidbody component
            bool hasRigidbody = prefab.GetComponent<Rigidbody>() != null || prefab.GetComponentInChildren<Rigidbody>() != null;
            
            if (hasRigidbody)
            {
                Debug.LogWarning($"[Unity6Compatibility] Instantiating prefab with Rigidbody: {prefab.name}. Using safe instantiation.");
                
                // Instantiate without immediate physics activation
                GameObject instance = Object.Instantiate(prefab, position, rotation, parent);
                
                // Disable Rigidbody temporarily
                Rigidbody[] rigidbodies = instance.GetComponentsInChildren<Rigidbody>();
                foreach (var rb in rigidbodies)
                {
                    rb.isKinematic = true;
                }
                
                // Re-enable after one frame
                if (Application.isPlaying)
                {
                    CoroutineRunner.Instance.StartCoroutine(ReenableRigidbodies(rigidbodies));
                }
                
                return instance;
            }
            else
            {
                return Object.Instantiate(prefab, position, rotation, parent);
            }
        }
        
        private static IEnumerator ReenableRigidbodies(Rigidbody[] rigidbodies)
        {
            yield return null; // Wait one frame
            
            foreach (var rb in rigidbodies)
            {
                if (rb != null)
                {
                    rb.isKinematic = false;
                }
            }
        }
        
        /// <summary>
        /// Safe AssetBundle operations with delay
        /// Addresses potential crashes when unloading/loading same bundle rapidly
        /// </summary>
        public static IEnumerator SafeAssetBundleReload(string bundlePath, System.Action<AssetBundle> onLoaded, System.Action<string> onError)
        {
            // Add delay between operations
            yield return new WaitForSeconds(0.1f);
            
            AssetBundleCreateRequest bundleRequest = null;
            System.Exception loadException = null;
            
            try
            {
                bundleRequest = AssetBundle.LoadFromFileAsync(bundlePath);
            }
            catch (System.Exception ex)
            {
                loadException = ex;
            }
            
            if (loadException != null)
            {
                onError?.Invoke($"AssetBundle loading error: {loadException.Message}");
                yield break;
            }
            
            if (bundleRequest != null)
            {
                yield return bundleRequest;
                
                if (bundleRequest.assetBundle != null)
                {
                    onLoaded?.Invoke(bundleRequest.assetBundle);
                }
                else
                {
                    onError?.Invoke("Failed to load AssetBundle");
                }
            }
            else
            {
                onError?.Invoke("Failed to create AssetBundle request");
            }
        }
        
        /// <summary>
        /// Animation loop count safety check
        /// Addresses UUM-111056: AnimatorController states stop working at high loop counts
        /// </summary>
        public static void CheckAnimationLoopCount(Animator animator, float maxNormalizedTime = 50000f)
        {
            if (animator == null) return;
            
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.normalizedTime > maxNormalizedTime)
            {
                Debug.LogWarning($"[Unity6Compatibility] Animation loop count too high ({stateInfo.normalizedTime}). Resetting animation on {animator.name}");
                animator.Play(stateInfo.shortNameHash, 0, 0f);
            }
        }
        
        /// <summary>
        /// Graphics API validation for Unity 6 compatibility
        /// </summary>
        public static void ValidateGraphicsAPI()
        {
            var graphicsAPI = SystemInfo.graphicsDeviceType;
            
            switch (graphicsAPI)
            {
                case UnityEngine.Rendering.GraphicsDeviceType.Direct3D12:
                    Debug.LogWarning("[Unity6Compatibility] DirectX12 detected. Known crashes in Unity 6000.2.0b12. Consider switching to DirectX11.");
                    break;
                    
                case UnityEngine.Rendering.GraphicsDeviceType.Metal:
                    if (Application.platform == RuntimePlatform.IPhonePlayer)
                    {
                        Debug.LogWarning("[Unity6Compatibility] Metal on iOS detected. Monitor for freezing issues (UUM-111494).");
                    }
                    break;
                    
                case UnityEngine.Rendering.GraphicsDeviceType.Direct3D11:
                    Debug.Log("[Unity6Compatibility] DirectX11 detected. Recommended for Unity 6 stability.");
                    break;
            }
        }
        
        /// <summary>
        /// Log Unity 6 compatibility status
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void LogCompatibilityStatus()
        {
            Debug.Log($"[Unity6Compatibility] Unity Version: {Application.unityVersion}");
            Debug.Log($"[Unity6Compatibility] Platform: {Application.platform}");
            Debug.Log($"[Unity6Compatibility] Graphics API: {SystemInfo.graphicsDeviceType}");
            
            ValidateGraphicsAPI();
            
            // Check for Unity 6000.2.0b12 specific version
            if (Application.unityVersion.Contains("6000.2.0"))
            {
                Debug.Log("[Unity6Compatibility] Unity 6000.2.0 detected. Compatibility measures active.");
            }
        }
    }
    
    /// <summary>
    /// Helper class for running coroutines from static contexts
    /// </summary>
    public class CoroutineRunner : MonoBehaviour
    {
        private static CoroutineRunner _instance;
        public static CoroutineRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("CoroutineRunner");
                    _instance = go.AddComponent<CoroutineRunner>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }
    }
}