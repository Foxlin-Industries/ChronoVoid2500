using UnityEngine;

namespace ChronoVoid.Client
{
    /// <summary>
    /// Emergency diagnostics - logs everything to console
    /// </summary>
    public class EmergencyDiagnostics : MonoBehaviour
    {
        private void Start()
        {
            Debug.Log("=== EMERGENCY DIAGNOSTICS ===");
            Debug.Log($"Unity Version: {Application.unityVersion}");
            Debug.Log($"Platform: {Application.platform}");
            Debug.Log($"Graphics Device: {SystemInfo.graphicsDeviceName}");
            Debug.Log($"Graphics API: {SystemInfo.graphicsDeviceType}");
            Debug.Log($"Screen Resolution: {Screen.width}x{Screen.height}");
            Debug.Log($"Is Editor: {Application.isEditor}");
            
            // Camera check (Unity 6 compatible)
            Camera[] cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
            Debug.Log($"Found {cameras.Length} cameras");
            
            foreach (Camera cam in cameras)
            {
                Debug.Log($"Camera: {cam.name}, Enabled: {cam.enabled}, Active: {cam.gameObject.activeInHierarchy}");
                Debug.Log($"  Position: {cam.transform.position}");
                Debug.Log($"  Rotation: {cam.transform.eulerAngles}");
                Debug.Log($"  FOV: {cam.fieldOfView}");
                Debug.Log($"  Near: {cam.nearClipPlane}, Far: {cam.farClipPlane}");
                Debug.Log($"  Clear Flags: {cam.clearFlags}");
                Debug.Log($"  Background Color: {cam.backgroundColor}");
                Debug.Log($"  Culling Mask: {cam.cullingMask}");
                Debug.Log($"  Orthographic: {cam.orthographic}");
            }
            
            // Canvas check (Unity 6 compatible)
            Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            Debug.Log($"Found {canvases.Length} canvases");
            
            foreach (Canvas canvas in canvases)
            {
                Debug.Log($"Canvas: {canvas.name}, Enabled: {canvas.enabled}, Active: {canvas.gameObject.activeInHierarchy}");
            }
            
            Debug.Log("=== END DIAGNOSTICS ===");
            Debug.Log("If you see this message, Unity is working!");
            Debug.Log("Check the Game view (not Scene view) for UI elements.");
        }
        
        private void Update()
        {
            // Log every 5 seconds that we're still running
            if (Time.time % 5f < Time.deltaTime)
            {
                Debug.Log($"Still running at {Time.time:F1} seconds - Unity 6 is working!");
            }
        }
    }
}