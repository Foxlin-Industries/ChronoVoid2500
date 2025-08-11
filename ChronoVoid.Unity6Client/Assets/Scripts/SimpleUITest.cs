using UnityEngine;

namespace ChronoVoid.Client
{
    /// <summary>
    /// Simple UI test using OnGUI - guaranteed to show something
    /// </summary>
    public class SimpleUITest : MonoBehaviour
    {
        private void OnGUI()
        {
            // Force white color and large font for visibility
            GUI.color = Color.white;
            GUI.backgroundColor = Color.white;
            GUI.contentColor = Color.white;
            
            // Create high-contrast styles
            GUIStyle titleStyle = new GUIStyle();
            titleStyle.fontSize = 32;
            titleStyle.normal.textColor = Color.white;
            titleStyle.alignment = TextAnchor.MiddleCenter;
            titleStyle.fontStyle = FontStyle.Bold;
            
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 18;
            buttonStyle.normal.textColor = Color.black;
            buttonStyle.fontStyle = FontStyle.Bold;
            
            GUIStyle textStyle = new GUIStyle();
            textStyle.fontSize = 16;
            textStyle.normal.textColor = Color.yellow;
            textStyle.alignment = TextAnchor.MiddleCenter;
            textStyle.fontStyle = FontStyle.Bold;
            
            // High-contrast background box
            GUI.backgroundColor = Color.red;
            GUI.Box(new Rect(50, 50, Screen.width - 100, Screen.height - 100), "");
            
            // Reset background for other elements
            GUI.backgroundColor = Color.white;
            
            // Large white title
            GUI.Label(new Rect(0, 80, Screen.width, 60), "CHRONOVOID 2500", titleStyle);
            
            // Bright yellow status
            GUI.Label(new Rect(0, 150, Screen.width, 40), "UNITY 6000.2.0b12 WORKING!", textStyle);
            
            // System info in bright colors
            textStyle.normal.textColor = Color.cyan;
            GUI.Label(new Rect(0, 200, Screen.width, 30), $"Unity: {Application.unityVersion}", textStyle);
            GUI.Label(new Rect(0, 230, Screen.width, 30), $"Graphics: {SystemInfo.graphicsDeviceType}", textStyle);
            GUI.Label(new Rect(0, 260, Screen.width, 30), $"Resolution: {Screen.width}x{Screen.height}", textStyle);
            
            // Large, high-contrast buttons
            GUI.backgroundColor = Color.green;
            if (GUI.Button(new Rect(Screen.width/2 - 150, 320, 300, 60), "RUN LOGIN TEST", buttonStyle))
            {
                Debug.Log("Button clicked! Loading LoginTestScene...");
                UnityEngine.SceneManagement.SceneManager.LoadScene("LoginTestScene");
            }
            
            GUI.backgroundColor = Color.blue;
            if (GUI.Button(new Rect(Screen.width/2 - 150, 390, 300, 60), "TEST GRAPHICS API", buttonStyle))
            {
                Debug.Log("Graphics API test button clicked!");
                TestGraphicsAPI();
            }
            
            GUI.backgroundColor = Color.magenta;
            if (GUI.Button(new Rect(Screen.width/2 - 150, 460, 300, 60), "CHECK DIRECTX12", buttonStyle))
            {
                Debug.Log("DirectX12 check button clicked!");
                CheckDirectX12();
            }
            
            // Bottom instructions in bright white
            textStyle.normal.textColor = Color.white;
            textStyle.fontSize = 14;
            GUI.Label(new Rect(0, Screen.height - 80, Screen.width, 60), 
                "IF YOU SEE THIS TEXT AND COLORED BUTTONS,\nUNITY 6000.2.0b12 UI IS WORKING PERFECTLY!", 
                textStyle);
        }
        
        private void TestGraphicsAPI()
        {
            var api = SystemInfo.graphicsDeviceType;
            Debug.Log($"=== Graphics API Test ===");
            Debug.Log($"Current API: {api}");
            Debug.Log($"Device Name: {SystemInfo.graphicsDeviceName}");
            Debug.Log($"Memory: {SystemInfo.graphicsMemorySize} MB");
            
            switch (api)
            {
                case UnityEngine.Rendering.GraphicsDeviceType.Direct3D11:
                    Debug.Log("✅ DirectX11 - Excellent choice for Unity 6!");
                    break;
                case UnityEngine.Rendering.GraphicsDeviceType.Direct3D12:
                    Debug.LogWarning("⚠️ DirectX12 - Known issues in Unity 6000.2.0b12!");
                    break;
                default:
                    Debug.Log($"ℹ️ Using {api}");
                    break;
            }
        }
        
        private void CheckDirectX12()
        {
            var api = SystemInfo.graphicsDeviceType;
            if (api == UnityEngine.Rendering.GraphicsDeviceType.Direct3D12)
            {
                Debug.LogError("🚨 DirectX12 Detected - This may cause crashes!");
                Debug.LogError("Go to Edit → Project Settings → Player → Graphics APIs");
                Debug.LogError("Remove DirectX12, keep only DirectX11");
            }
            else
            {
                Debug.Log("✅ DirectX12 not active - Graphics API is safe");
            }
        }
        
        private void Start()
        {
            Debug.Log("=== SimpleUITest Started ===");
            Debug.Log("Creating visible 3D objects for testing...");
            
            // Get camera position for reference
            Camera cam = Camera.main;
            Vector3 cameraPos = cam != null ? cam.transform.position : Vector3.zero;
            Debug.Log($"Camera position: {cameraPos}");
            
            // Create objects very close to camera for guaranteed visibility
            // Camera is at (0,0,-10), so put objects at (0,0,0) - right in front
            
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "TestCube";
            cube.transform.position = new Vector3(0, 0, 0);  // Right in front of camera
            cube.transform.localScale = Vector3.one * 3;     // Make it big
            
            // Make it bright red with emissive material
            Renderer cubeRenderer = cube.GetComponent<Renderer>();
            if (cubeRenderer != null)
            {
                Material redMat = new Material(Shader.Find("Standard"));
                redMat.color = Color.red;
                redMat.SetColor("_EmissionColor", Color.red);
                redMat.EnableKeyword("_EMISSION");
                cubeRenderer.material = redMat;
                Debug.Log("Created BRIGHT RED test cube at (0,0,0) - should be visible!");
            }
            
            // Create a second cube to the side
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = "TestSphere";
            sphere.transform.position = new Vector3(4, 0, 0);  // To the side
            sphere.transform.localScale = Vector3.one * 2f;
            
            // Make it bright green with emissive material
            Renderer sphereRenderer = sphere.GetComponent<Renderer>();
            if (sphereRenderer != null)
            {
                Material greenMat = new Material(Shader.Find("Standard"));
                greenMat.color = Color.green;
                greenMat.SetColor("_EmissionColor", Color.green);
                greenMat.EnableKeyword("_EMISSION");
                sphereRenderer.material = greenMat;
                Debug.Log("Created BRIGHT GREEN test sphere at (4,0,0) - should be visible!");
            }
            
            // Add rotation script
            sphere.AddComponent<SimpleRotator>();
            
            // Create a light to illuminate the objects
            GameObject lightObj = new GameObject("TestLight");
            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = Color.white;
            light.intensity = 2f;
            lightObj.transform.rotation = Quaternion.Euler(45, 45, 0);
            Debug.Log("Created directional light for illumination");
            
            Debug.Log("If you don't see UI, check:");
            Debug.Log("1. Game view is selected (not Scene view)");
            Debug.Log("2. Camera is active and rendering");
            Debug.Log("3. You should see a RED CUBE and GREEN SPHERE");
            
            // Log camera info
            if (cam != null)
            {
                Debug.Log($"Main Camera found: {cam.name}");
                Debug.Log($"Camera enabled: {cam.enabled}");
                Debug.Log($"Camera position: {cam.transform.position}");
                Debug.Log($"Camera rotation: {cam.transform.eulerAngles}");
                Debug.Log($"Camera FOV: {cam.fieldOfView}");
                Debug.Log($"Camera orthographic: {cam.orthographic}");
            }
            else
            {
                Debug.LogError("No main camera found!");
            }
        }
    }
}