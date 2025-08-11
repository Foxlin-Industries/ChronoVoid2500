using UnityEngine;
using UnityEngine.UI;

public class TestSimpleUI : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== TEST SIMPLE UI STARTED ===");
        
        // Create Canvas
        GameObject canvasGO = new GameObject("TestCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasGO.AddComponent<GraphicRaycaster>();
        
        Debug.Log("Canvas created successfully");
        
        // Create a simple button
        GameObject buttonGO = new GameObject("TestButton");
        buttonGO.transform.SetParent(canvas.transform, false);
        
        RectTransform buttonRect = buttonGO.AddComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.sizeDelta = new Vector2(200, 50);
        buttonRect.anchoredPosition = Vector2.zero;
        
        // Button background
        Image buttonBg = buttonGO.AddComponent<Image>();
        buttonBg.color = Color.red;
        
        // Button component
        Button button = buttonGO.AddComponent<Button>();
        button.onClick.AddListener(() => {
            Debug.Log("BUTTON CLICKED!");
        });
        
        // Button text
        GameObject textGO = new GameObject("ButtonText");
        textGO.transform.SetParent(buttonGO.transform, false);
        
        RectTransform textRect = textGO.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
        
        Text text = textGO.AddComponent<Text>();
        text.text = "CLICK ME";
        text.fontSize = 20;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        
        Debug.Log("Button created successfully");
        
        // Create a simple input field
        GameObject inputGO = new GameObject("TestInput");
        inputGO.transform.SetParent(canvas.transform, false);
        
        RectTransform inputRect = inputGO.AddComponent<RectTransform>();
        inputRect.anchorMin = new Vector2(0.5f, 0.5f);
        inputRect.anchorMax = new Vector2(0.5f, 0.5f);
        inputRect.sizeDelta = new Vector2(300, 40);
        inputRect.anchoredPosition = new Vector2(0, 100);
        
        // Input background
        Image inputBg = inputGO.AddComponent<Image>();
        inputBg.color = Color.blue;
        
        // Input field
        InputField inputField = inputGO.AddComponent<InputField>();
        
        // Input text
        GameObject inputTextGO = new GameObject("InputText");
        inputTextGO.transform.SetParent(inputGO.transform, false);
        
        RectTransform inputTextRect = inputTextGO.AddComponent<RectTransform>();
        inputTextRect.anchorMin = Vector2.zero;
        inputTextRect.anchorMax = Vector2.one;
        inputTextRect.sizeDelta = Vector2.zero;
        inputTextRect.anchoredPosition = Vector2.zero;
        inputTextRect.offsetMin = new Vector2(10, 0);
        inputTextRect.offsetMax = new Vector2(-10, 0);
        
        Text inputTextComponent = inputTextGO.AddComponent<Text>();
        inputTextComponent.fontSize = 16;
        inputTextComponent.color = Color.white;
        inputTextComponent.alignment = TextAnchor.MiddleLeft;
        inputTextComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        
        inputField.textComponent = inputTextComponent;
        inputField.text = "Type here...";
        
        Debug.Log("Input field created successfully");
        Debug.Log("=== TEST UI CREATION COMPLETE ===");
    }
}