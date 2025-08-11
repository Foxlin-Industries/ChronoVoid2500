using UnityEngine;
using UnityEngine.UI;

namespace ChronoVoid.Client
{
    /// <summary>
    /// Canvas-based UI test - creates UI programmatically
    /// </summary>
    public class CanvasUITest : MonoBehaviour
    {
        private void Start()
        {
            Debug.Log("=== Creating Canvas UI Test ===");
            
            // Create Canvas
            GameObject canvasObj = new GameObject("TestCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100; // Make sure it's on top
            
            // Add CanvasScaler
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            // Add GraphicRaycaster
            canvasObj.AddComponent<GraphicRaycaster>();
            
            Debug.Log("Created Canvas");
            
            // Create background panel
            GameObject panelObj = new GameObject("BackgroundPanel");
            panelObj.transform.SetParent(canvasObj.transform, false);
            
            Image panelImage = panelObj.AddComponent<Image>();
            panelImage.color = new Color(0.2f, 0.2f, 0.4f, 0.8f); // Semi-transparent blue
            
            RectTransform panelRect = panelObj.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            
            Debug.Log("Created background panel");
            
            // Create title text
            GameObject titleObj = new GameObject("TitleText");
            titleObj.transform.SetParent(panelObj.transform, false);
            
            Text titleText = titleObj.AddComponent<Text>();
            titleText.text = "CHRONOVOID 2500 - UNITY 6000.2.0b12";
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            titleText.fontSize = 36;
            titleText.color = Color.white;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.fontStyle = FontStyle.Bold;
            
            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0.8f);
            titleRect.anchorMax = new Vector2(1, 0.9f);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;
            
            Debug.Log("Created title text");
            
            // Create status text
            GameObject statusObj = new GameObject("StatusText");
            statusObj.transform.SetParent(panelObj.transform, false);
            
            Text statusText = statusObj.AddComponent<Text>();
            statusText.text = "✅ UNITY 6 CANVAS UI WORKING!\n" +
                             $"Unity: {Application.unityVersion}\n" +
                             $"Graphics: {SystemInfo.graphicsDeviceType}\n" +
                             $"Resolution: {Screen.width}x{Screen.height}";
            statusText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            statusText.fontSize = 24;
            statusText.color = Color.yellow;
            statusText.alignment = TextAnchor.MiddleCenter;
            
            RectTransform statusRect = statusObj.GetComponent<RectTransform>();
            statusRect.anchorMin = new Vector2(0, 0.5f);
            statusRect.anchorMax = new Vector2(1, 0.7f);
            statusRect.offsetMin = Vector2.zero;
            statusRect.offsetMax = Vector2.zero;
            
            Debug.Log("Created status text");
            
            // Create test button
            GameObject buttonObj = new GameObject("TestButton");
            buttonObj.transform.SetParent(panelObj.transform, false);
            
            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = Color.green;
            
            Button button = buttonObj.AddComponent<Button>();
            button.onClick.AddListener(() => {
                Debug.Log("Canvas button clicked! Unity 6 UI is working!");
                statusText.text = "BUTTON CLICKED! UI IS WORKING!";
                statusText.color = Color.green;
            });
            
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.3f, 0.3f);
            buttonRect.anchorMax = new Vector2(0.7f, 0.4f);
            buttonRect.offsetMin = Vector2.zero;
            buttonRect.offsetMax = Vector2.zero;
            
            // Button text
            GameObject buttonTextObj = new GameObject("ButtonText");
            buttonTextObj.transform.SetParent(buttonObj.transform, false);
            
            Text buttonText = buttonTextObj.AddComponent<Text>();
            buttonText.text = "CLICK ME - TEST BUTTON";
            buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            buttonText.fontSize = 20;
            buttonText.color = Color.black;
            buttonText.alignment = TextAnchor.MiddleCenter;
            buttonText.fontStyle = FontStyle.Bold;
            
            RectTransform buttonTextRect = buttonTextObj.GetComponent<RectTransform>();
            buttonTextRect.anchorMin = Vector2.zero;
            buttonTextRect.anchorMax = Vector2.one;
            buttonTextRect.offsetMin = Vector2.zero;
            buttonTextRect.offsetMax = Vector2.zero;
            
            Debug.Log("Created test button");
            
            // Create instructions
            GameObject instructObj = new GameObject("Instructions");
            instructObj.transform.SetParent(panelObj.transform, false);
            
            Text instructText = instructObj.AddComponent<Text>();
            instructText.text = "IF YOU SEE THIS CANVAS UI, UNITY 6000.2.0b12 IS WORKING PERFECTLY!\n" +
                               "This proves the UI system is functional.";
            instructText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            instructText.fontSize = 18;
            instructText.color = Color.cyan;
            instructText.alignment = TextAnchor.MiddleCenter;
            
            RectTransform instructRect = instructObj.GetComponent<RectTransform>();
            instructRect.anchorMin = new Vector2(0, 0.1f);
            instructRect.anchorMax = new Vector2(1, 0.25f);
            instructRect.offsetMin = Vector2.zero;
            instructRect.offsetMax = Vector2.zero;
            
            Debug.Log("Created instructions");
            Debug.Log("=== Canvas UI Test Complete ===");
            Debug.Log("You should now see a blue panel with white title, yellow status, green button, and cyan instructions");
        }
    }
}