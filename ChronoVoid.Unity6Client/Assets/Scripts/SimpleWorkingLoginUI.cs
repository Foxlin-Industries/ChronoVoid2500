using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ChronoVoid.Client;
using System.Collections;

namespace ChronoVoid.Client.UI
{
    /// <summary>
    /// Simple, working login UI using standard Unity UI components (no TextMeshPro)
    /// </summary>
    public class SimpleWorkingLoginUI : MonoBehaviour
    {
        [Header("Configuration")]
        public string realmListSceneName = "SimpleRealmListScene";
        
        // UI References (created programmatically)
        private Canvas canvas;
        private GameObject loginPanel;
        private GameObject registerPanel;
        private GameObject loadingPanel;
        
        // Login UI
        private InputField loginUsername;
        private InputField loginPassword;
        private Button loginButton;
        private Button switchToRegisterButton;
        
        // Register UI
        private InputField registerUsername;
        private InputField registerEmail;
        private InputField registerPassword;
        private Button registerButton;
        private Button switchToLoginButton;
        
        // Feedback UI
        private Text statusText;
        private Text titleText;
        private Text loadingText;
        
        private ApiClient apiClient;
        private bool isProcessing = false;
        private AuthResponse currentUser;
        
        private void Start()
        {
            Debug.Log("=== Simple Working Login UI Started ===");
            
            // Find or create ApiClient
            apiClient = FindFirstObjectByType<ApiClient>();
            if (apiClient == null)
            {
                GameObject apiObject = new GameObject("ApiClient");
                apiClient = apiObject.AddComponent<ApiClient>();
                Debug.Log("Created ApiClient instance");
            }
            
            CreateUI();
            ShowLoginPanel();
        }
        
        private void CreateUI()
        {
            // Create Canvas
            GameObject canvasGO = new GameObject("LoginCanvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            
            canvasGO.AddComponent<GraphicRaycaster>();
            
            // Create Background
            CreateBackground();
            
            // Create Title
            CreateTitle();
            
            // Create Login Panel
            CreateLoginPanel();
            
            // Create Register Panel
            CreateRegisterPanel();
            
            // Create Loading Panel
            CreateLoadingPanel();
            
            // Create Status Text
            CreateStatusText();
        }
        
        private void CreateBackground()
        {
            GameObject bgGO = new GameObject("Background");
            bgGO.transform.SetParent(canvas.transform, false);
            
            RectTransform bgRect = bgGO.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            bgRect.anchoredPosition = Vector2.zero;
            
            Image bgImage = bgGO.AddComponent<Image>();
            bgImage.color = new Color(0.05f, 0.05f, 0.2f, 1f); // Dark blue background
        }
        
        private void CreateTitle()
        {
            GameObject titleGO = new GameObject("Title");
            titleGO.transform.SetParent(canvas.transform, false);
            
            RectTransform titleRect = titleGO.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.8f);
            titleRect.anchorMax = new Vector2(0.5f, 0.8f);
            titleRect.sizeDelta = new Vector2(800, 100);
            titleRect.anchoredPosition = Vector2.zero;
            
            titleText = titleGO.AddComponent<Text>();
            titleText.text = "CHRONOVOID 2500";
            titleText.fontSize = 48;
            titleText.fontStyle = FontStyle.Bold;
            titleText.color = new Color(0f, 1f, 1f, 1f); // Cyan
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
        
        private void CreateLoginPanel()
        {
            loginPanel = new GameObject("LoginPanel");
            loginPanel.transform.SetParent(canvas.transform, false);
            
            RectTransform panelRect = loginPanel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.sizeDelta = new Vector2(500, 400);
            panelRect.anchoredPosition = Vector2.zero;
            
            // Panel background
            Image panelBg = loginPanel.AddComponent<Image>();
            panelBg.color = new Color(0.1f, 0.1f, 0.3f, 0.9f);
            
            // Username field
            loginUsername = CreateInputField(loginPanel, "Username", new Vector2(0, 100), "spacetrader123");
            
            // Password field
            loginPassword = CreateInputField(loginPanel, "Password", new Vector2(0, 40), "password123", true);
            
            // Login button
            loginButton = CreateButton(loginPanel, "LOGIN", new Vector2(0, -20), new Color(0f, 0.8f, 0f, 1f), OnLoginClicked);
            
            // Switch to register button
            switchToRegisterButton = CreateButton(loginPanel, "Create Account", new Vector2(0, -80), new Color(0f, 0.6f, 1f, 1f), ShowRegisterPanel);
            switchToRegisterButton.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 40);
        }
        
        private void CreateRegisterPanel()
        {
            registerPanel = new GameObject("RegisterPanel");
            registerPanel.transform.SetParent(canvas.transform, false);
            
            RectTransform panelRect = registerPanel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.sizeDelta = new Vector2(500, 450);
            panelRect.anchoredPosition = Vector2.zero;
            
            // Panel background
            Image panelBg = registerPanel.AddComponent<Image>();
            panelBg.color = new Color(0.1f, 0.1f, 0.3f, 0.9f);
            
            // Username field
            registerUsername = CreateInputField(registerPanel, "Username", new Vector2(0, 120), "newuser123");
            
            // Email field
            registerEmail = CreateInputField(registerPanel, "Email", new Vector2(0, 60), "newuser@chronovoid.com");
            
            // Password field
            registerPassword = CreateInputField(registerPanel, "Password", new Vector2(0, 0), "password123", true);
            
            // Register button
            registerButton = CreateButton(registerPanel, "CREATE ACCOUNT", new Vector2(0, -60), new Color(0f, 0.8f, 0f, 1f), OnRegisterClicked);
            
            // Switch to login button
            switchToLoginButton = CreateButton(registerPanel, "Back to Login", new Vector2(0, -120), new Color(0.6f, 0.6f, 0.6f, 1f), ShowLoginPanel);
            switchToLoginButton.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 40);
            
            registerPanel.SetActive(false);
        }
        
        private void CreateLoadingPanel()
        {
            loadingPanel = new GameObject("LoadingPanel");
            loadingPanel.transform.SetParent(canvas.transform, false);
            
            RectTransform panelRect = loadingPanel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.sizeDelta = new Vector2(400, 200);
            panelRect.anchoredPosition = Vector2.zero;
            
            // Panel background
            Image panelBg = loadingPanel.AddComponent<Image>();
            panelBg.color = new Color(0.1f, 0.1f, 0.3f, 0.95f);
            
            // Loading text
            GameObject loadingTextGO = new GameObject("LoadingText");
            loadingTextGO.transform.SetParent(loadingPanel.transform, false);
            
            RectTransform loadingTextRect = loadingTextGO.AddComponent<RectTransform>();
            loadingTextRect.anchorMin = new Vector2(0.5f, 0.5f);
            loadingTextRect.anchorMax = new Vector2(0.5f, 0.5f);
            loadingTextRect.sizeDelta = new Vector2(350, 100);
            loadingTextRect.anchoredPosition = Vector2.zero;
            
            loadingText = loadingTextGO.AddComponent<Text>();
            loadingText.text = "Loading...";
            loadingText.fontSize = 24;
            loadingText.color = Color.yellow;
            loadingText.alignment = TextAnchor.MiddleCenter;
            loadingText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            
            loadingPanel.SetActive(false);
        }
        
        private void CreateStatusText()
        {
            GameObject statusGO = new GameObject("StatusText");
            statusGO.transform.SetParent(canvas.transform, false);
            
            RectTransform statusRect = statusGO.AddComponent<RectTransform>();
            statusRect.anchorMin = new Vector2(0.5f, 0.2f);
            statusRect.anchorMax = new Vector2(0.5f, 0.2f);
            statusRect.sizeDelta = new Vector2(800, 60);
            statusRect.anchoredPosition = Vector2.zero;
            
            statusText = statusGO.AddComponent<Text>();
            statusText.text = "Welcome to ChronoVoid 2500";
            statusText.fontSize = 18;
            statusText.color = Color.white;
            statusText.alignment = TextAnchor.MiddleCenter;
            statusText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
        
        private InputField CreateInputField(GameObject parent, string placeholder, Vector2 position, string defaultValue = "", bool isPassword = false)
        {
            GameObject fieldGO = new GameObject(placeholder + "Field");
            fieldGO.transform.SetParent(parent.transform, false);
            
            RectTransform fieldRect = fieldGO.AddComponent<RectTransform>();
            fieldRect.anchorMin = new Vector2(0.5f, 0.5f);
            fieldRect.anchorMax = new Vector2(0.5f, 0.5f);
            fieldRect.sizeDelta = new Vector2(400, 50);
            fieldRect.anchoredPosition = position;
            
            // Background
            Image fieldBg = fieldGO.AddComponent<Image>();
            fieldBg.color = new Color(0.2f, 0.2f, 0.4f, 1f);
            
            // Input field
            InputField inputField = fieldGO.AddComponent<InputField>();
            
            // Create text component for input
            GameObject textGO = new GameObject("Text");
            textGO.transform.SetParent(fieldGO.transform, false);
            RectTransform textRect = textGO.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;
            textRect.offsetMin = new Vector2(10, 0);
            textRect.offsetMax = new Vector2(-10, 0);
            
            Text textComponent = textGO.AddComponent<Text>();
            textComponent.fontSize = 18;
            textComponent.color = Color.white;
            textComponent.alignment = TextAnchor.MiddleLeft;
            textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            
            // Create placeholder
            GameObject placeholderGO = new GameObject("Placeholder");
            placeholderGO.transform.SetParent(fieldGO.transform, false);
            RectTransform placeholderRect = placeholderGO.AddComponent<RectTransform>();
            placeholderRect.anchorMin = Vector2.zero;
            placeholderRect.anchorMax = Vector2.one;
            placeholderRect.sizeDelta = Vector2.zero;
            placeholderRect.anchoredPosition = Vector2.zero;
            placeholderRect.offsetMin = new Vector2(10, 0);
            placeholderRect.offsetMax = new Vector2(-10, 0);
            
            Text placeholderText = placeholderGO.AddComponent<Text>();
            placeholderText.text = placeholder;
            placeholderText.fontSize = 18;
            placeholderText.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            placeholderText.alignment = TextAnchor.MiddleLeft;
            placeholderText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            
            // Set up input field
            inputField.textComponent = textComponent;
            inputField.placeholder = placeholderText;
            inputField.text = defaultValue;
            
            // Set password mode
            if (isPassword)
            {
                inputField.contentType = InputField.ContentType.Password;
            }
            
            return inputField;
        }
        
        private Button CreateButton(GameObject parent, string text, Vector2 position, Color color, System.Action onClick)
        {
            GameObject buttonGO = new GameObject(text + "Button");
            buttonGO.transform.SetParent(parent.transform, false);
            
            RectTransform buttonRect = buttonGO.AddComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
            buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
            buttonRect.sizeDelta = new Vector2(300, 50);
            buttonRect.anchoredPosition = position;
            
            // Background
            Image buttonBg = buttonGO.AddComponent<Image>();
            buttonBg.color = color;
            
            // Button component
            Button button = buttonGO.AddComponent<Button>();
            button.targetGraphic = buttonBg;
            button.onClick.AddListener(() => onClick());
            
            // Text
            GameObject textGO = new GameObject("Text");
            textGO.transform.SetParent(buttonGO.transform, false);
            
            RectTransform textRect = textGO.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;
            
            Text buttonText = textGO.AddComponent<Text>();
            buttonText.text = text;
            buttonText.fontSize = 20;
            buttonText.fontStyle = FontStyle.Bold;
            buttonText.color = Color.white;
            buttonText.alignment = TextAnchor.MiddleCenter;
            buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            
            return button;
        }
        
        private void ShowLoginPanel()
        {
            loginPanel.SetActive(true);
            registerPanel.SetActive(false);
            loadingPanel.SetActive(false);
            SetStatus("Enter your credentials to login", Color.white);
        }
        
        private void ShowRegisterPanel()
        {
            loginPanel.SetActive(false);
            registerPanel.SetActive(true);
            loadingPanel.SetActive(false);
            SetStatus("Create a new account", Color.white);
        }
        
        private void ShowLoadingPanel(string message)
        {
            loginPanel.SetActive(false);
            registerPanel.SetActive(false);
            loadingPanel.SetActive(true);
            if (loadingText != null) loadingText.text = message;
        }
        
        private void OnLoginClicked()
        {
            string username = loginUsername.text.Trim();
            string password = loginPassword.text;
            
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                SetStatus("Please fill in all fields", Color.red);
                return;
            }
            
            StartCoroutine(LoginUser(username, password));
        }
        
        private void OnRegisterClicked()
        {
            string username = registerUsername.text.Trim();
            string email = registerEmail.text.Trim();
            string password = registerPassword.text;
            
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                SetStatus("Please fill in all fields", Color.red);
                return;
            }
            
            if (username.Length < 3)
            {
                SetStatus("Username must be at least 3 characters", Color.red);
                return;
            }
            
            if (!email.Contains("@"))
            {
                SetStatus("Please enter a valid email address", Color.red);
                return;
            }
            
            if (password.Length < 6)
            {
                SetStatus("Password must be at least 6 characters", Color.red);
                return;
            }
            
            StartCoroutine(RegisterUser(username, email, password));
        }
        
        private IEnumerator LoginUser(string username, string password)
        {
            isProcessing = true;
            ShowLoadingPanel("Logging in...");
            SetStatus("Authenticating user...", Color.yellow);
            
            var loginRequest = new LoginRequest
            {
                username = username,
                password = password
            };
            
            bool loginComplete = false;
            bool loginSuccess = false;
            string loginError = "";
            
            apiClient.Login(loginRequest,
                (response) => {
                    loginComplete = true;
                    loginSuccess = true;
                    currentUser = response;
                    Debug.Log($"✅ Login successful: {response.username}");
                },
                (error) => {
                    loginComplete = true;
                    loginSuccess = false;
                    loginError = error;
                    Debug.LogError($"❌ Login failed: {error}");
                });
            
            while (!loginComplete)
            {
                yield return new WaitForSeconds(0.1f);
            }
            
            isProcessing = false;
            
            if (loginSuccess)
            {
                SetStatus($"Welcome back, {currentUser.username}!", Color.green);
                SaveUserData();
                yield return new WaitForSeconds(1f);
                GoToRealmList();
            }
            else
            {
                ShowLoginPanel();
                SetStatus($"Login failed: {loginError}", Color.red);
            }
        }
        
        private IEnumerator RegisterUser(string username, string email, string password)
        {
            isProcessing = true;
            ShowLoadingPanel("Creating account...");
            SetStatus("Creating new user account...", Color.yellow);
            
            var registerRequest = new RegisterRequest
            {
                username = username,
                email = email,
                password = password
            };
            
            bool registrationComplete = false;
            bool registrationSuccess = false;
            string registrationError = "";
            
            apiClient.Register(registerRequest, 
                (response) => {
                    registrationComplete = true;
                    registrationSuccess = true;
                    currentUser = response;
                    Debug.Log($"✅ Registration successful: {response.username}");
                },
                (error) => {
                    registrationComplete = true;
                    registrationSuccess = false;
                    registrationError = error;
                    Debug.LogError($"❌ Registration failed: {error}");
                });
            
            while (!registrationComplete)
            {
                yield return new WaitForSeconds(0.1f);
            }
            
            isProcessing = false;
            
            if (registrationSuccess)
            {
                SetStatus($"Account created! Welcome {currentUser.username}!", Color.green);
                SaveUserData();
                yield return new WaitForSeconds(1f);
                GoToRealmList();
            }
            else
            {
                ShowRegisterPanel();
                SetStatus($"Registration failed: {registrationError}", Color.red);
            }
        }
        
        private void SaveUserData()
        {
            if (currentUser != null)
            {
                PlayerPrefs.SetInt("UserId", currentUser.userId);
                PlayerPrefs.SetString("Username", currentUser.username);
                PlayerPrefs.SetString("UserEmail", currentUser.email);
                PlayerPrefs.SetString("UserToken", currentUser.token);
                PlayerPrefs.Save();
                Debug.Log("✅ User data saved to PlayerPrefs");
            }
        }
        
        private void GoToRealmList()
        {
            try
            {
                SceneManager.LoadScene(realmListSceneName);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load scene '{realmListSceneName}': {e.Message}");
                SetStatus($"Could not load realm list", Color.red);
                ShowLoginPanel();
            }
        }
        
        private void SetStatus(string message, Color color)
        {
            if (statusText != null)
            {
                statusText.text = message;
                statusText.color = color;
            }
            Debug.Log($"Status: {message}");
        }
    }
}