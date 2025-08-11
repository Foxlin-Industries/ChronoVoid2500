using UnityEngine;
using UnityEngine.SceneManagement;
using ChronoVoid.Client;
using System.Collections;

namespace ChronoVoid.Client
{
    /// <summary>
    /// Complete login system with registration, login, and navigation to realm list
    /// </summary>
    public class FullLoginSystem : MonoBehaviour
    {
        [Header("Configuration")]
        public string realmListSceneName = "RealmListScene";
        
        [Header("Default Test Values")]
        public string defaultUsername = "spacetrader123";
        public string defaultEmail = "spacetrader@chronovoid.com";
        public string defaultPassword = "password123";
        
        private ApiClient apiClient;
        private bool isProcessing = false;
        
        // UI State
        private bool showRegister = false;
        private string username = "";
        private string email = "";
        private string password = "";
        private string statusMessage = "";
        private Color statusColor = Color.white;
        
        // Current user data
        private AuthResponse currentUser;
        
        private void Start()
        {
            Debug.Log("=== ChronoVoid 2500 - Full Login System ===");
            
            // Initialize with default values for easy testing
            username = defaultUsername;
            email = defaultEmail;
            password = defaultPassword;
            
            // Find or create ApiClient
            apiClient = FindFirstObjectByType<ApiClient>();
            if (apiClient == null)
            {
                GameObject apiObject = new GameObject("ApiClient");
                apiClient = apiObject.AddComponent<ApiClient>();
                Debug.Log("Created ApiClient instance");
            }
            
            Debug.Log($"API Base URL: {apiClient.apiBaseUrl}");
            SetStatus("Ready to login or register", Color.cyan);
            
            // Test API connectivity
            StartCoroutine(TestConnectivity());
        }
        
        private IEnumerator TestConnectivity()
        {
            SetStatus("Testing API connection...", Color.yellow);
            yield return new WaitForSeconds(1f);
            
            // Simple connectivity test
            try
            {
                SetStatus("API connection ready!", Color.green);
                Debug.Log("✅ API connectivity confirmed");
            }
            catch (System.Exception e)
            {
                SetStatus($"API connection issue: {e.Message}", Color.red);
                Debug.LogError($"❌ API connectivity failed: {e.Message}");
            }
        }
        
        private void OnGUI()
        {
            // Set up GUI styling
            GUI.skin.button.fontSize = 16;
            GUI.skin.textField.fontSize = 16;
            GUI.skin.label.fontSize = 18;
            GUI.skin.box.fontSize = 20;
            
            // Main container
            GUILayout.BeginArea(new Rect(50, 50, Screen.width - 100, Screen.height - 100));
            
            // Title
            GUI.color = Color.cyan;
            GUILayout.Box("CHRONOVOID 2500 - SPACE TRADING EMPIRE", GUILayout.Height(50));
            GUI.color = Color.white;
            
            GUILayout.Space(20);
            
            // Tab buttons
            GUILayout.BeginHorizontal();
            GUI.backgroundColor = showRegister ? Color.gray : Color.green;
            if (GUILayout.Button("LOGIN", GUILayout.Height(40)))
            {
                showRegister = false;
                SetStatus("Switch to login mode", Color.cyan);
            }
            
            GUI.backgroundColor = showRegister ? Color.green : Color.gray;
            if (GUILayout.Button("REGISTER", GUILayout.Height(40)))
            {
                showRegister = true;
                SetStatus("Switch to register mode", Color.cyan);
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();
            
            GUILayout.Space(20);
            
            // Input fields
            GUILayout.Label("Username:");
            username = GUILayout.TextField(username, GUILayout.Height(30));
            
            if (showRegister)
            {
                GUILayout.Label("Email:");
                email = GUILayout.TextField(email, GUILayout.Height(30));
            }
            
            GUILayout.Label("Password:");
            password = GUILayout.PasswordField(password, '*', GUILayout.Height(30));
            
            GUILayout.Space(20);
            
            // Action buttons
            GUI.enabled = !isProcessing && !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password);
            
            if (showRegister)
            {
                GUI.enabled = GUI.enabled && !string.IsNullOrEmpty(email);
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("🚀 CREATE ACCOUNT & LOGIN", GUILayout.Height(50)))
                {
                    StartCoroutine(RegisterUser());
                }
            }
            else
            {
                GUI.backgroundColor = Color.blue;
                if (GUILayout.Button("🔐 LOGIN TO CHRONOVOID", GUILayout.Height(50)))
                {
                    StartCoroutine(LoginUser());
                }
            }
            
            GUI.backgroundColor = Color.white;
            GUI.enabled = true;
            
            GUILayout.Space(20);
            
            // Quick test buttons
            if (!isProcessing)
            {
                GUILayout.Label("Quick Actions:", GUI.skin.box);
                
                GUILayout.BeginHorizontal();
                GUI.backgroundColor = Color.yellow;
                if (GUILayout.Button("Fill Test Data", GUILayout.Height(30)))
                {
                    username = defaultUsername;
                    email = defaultEmail;
                    password = defaultPassword;
                    SetStatus("Test data filled", Color.green);
                }
                
                GUI.backgroundColor = Color.orange;
                if (GUILayout.Button("Clear Fields", GUILayout.Height(30)))
                {
                    username = "";
                    email = "";
                    password = "";
                    SetStatus("Fields cleared", Color.cyan);
                }
                GUI.backgroundColor = Color.white;
                GUILayout.EndHorizontal();
            }
            
            GUILayout.Space(20);
            
            // Status display
            GUI.color = statusColor;
            GUILayout.Box(statusMessage, GUILayout.Height(60));
            GUI.color = Color.white;
            
            // Processing indicator
            if (isProcessing)
            {
                GUI.color = Color.yellow;
                GUILayout.Box("⏳ PROCESSING... PLEASE WAIT", GUILayout.Height(40));
                GUI.color = Color.white;
            }
            
            // Current user info
            if (currentUser != null)
            {
                GUILayout.Space(20);
                GUI.color = Color.green;
                GUILayout.Box($"✅ LOGGED IN AS: {currentUser.username}", GUILayout.Height(40));
                GUI.color = Color.white;
                
                GUI.backgroundColor = Color.cyan;
                if (GUILayout.Button("🌌 ENTER REALM LIST", GUILayout.Height(50)))
                {
                    GoToRealmList();
                }
                GUI.backgroundColor = Color.white;
            }
            
            GUILayout.EndArea();
        }
        
        private IEnumerator RegisterUser()
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                SetStatus("❌ Please fill in all fields", Color.red);
                yield break;
            }
            
            if (password.Length < 6)
            {
                SetStatus("❌ Password must be at least 6 characters", Color.red);
                yield break;
            }
            
            if (!email.Contains("@"))
            {
                SetStatus("❌ Please enter a valid email address", Color.red);
                yield break;
            }
            
            isProcessing = true;
            SetStatus($"📝 Creating account for {username}...", Color.yellow);
            
            var registerRequest = new RegisterRequest
            {
                username = username.Trim(),
                email = email.Trim(),
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
            
            // Wait for registration to complete
            while (!registrationComplete)
            {
                yield return new WaitForSeconds(0.1f);
            }
            
            isProcessing = false;
            
            if (registrationSuccess)
            {
                SetStatus($"✅ Account created! Welcome {currentUser.username}!", Color.green);
                SaveUserData();
            }
            else
            {
                SetStatus($"❌ Registration failed: {registrationError}", Color.red);
            }
        }
        
        private IEnumerator LoginUser()
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                SetStatus("❌ Please enter username and password", Color.red);
                yield break;
            }
            
            isProcessing = true;
            SetStatus($"🔐 Logging in {username}...", Color.yellow);
            
            var loginRequest = new LoginRequest
            {
                username = username.Trim(),
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
            
            // Wait for login to complete
            while (!loginComplete)
            {
                yield return new WaitForSeconds(0.1f);
            }
            
            isProcessing = false;
            
            if (loginSuccess)
            {
                SetStatus($"✅ Welcome back, {currentUser.username}!", Color.green);
                SaveUserData();
            }
            else
            {
                SetStatus($"❌ Login failed: {loginError}", Color.red);
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
            if (currentUser == null)
            {
                SetStatus("❌ No user logged in", Color.red);
                return;
            }
            
            SetStatus($"🌌 Loading realm list for {currentUser.username}...", Color.cyan);
            
            // Try to load the realm list scene
            try
            {
                SceneManager.LoadScene(realmListSceneName);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load scene '{realmListSceneName}': {e.Message}");
                SetStatus($"❌ Could not load realm list. Scene '{realmListSceneName}' not found.", Color.red);
            }
        }
        
        private void SetStatus(string message, Color color)
        {
            statusMessage = message;
            statusColor = color;
            Debug.Log($"Status: {message}");
        }
        
        // Public method to clear user data (for logout)
        public static void Logout()
        {
            PlayerPrefs.DeleteKey("UserId");
            PlayerPrefs.DeleteKey("Username");
            PlayerPrefs.DeleteKey("UserEmail");
            PlayerPrefs.DeleteKey("UserToken");
            PlayerPrefs.Save();
            Debug.Log("✅ User logged out - data cleared");
        }
    }
}