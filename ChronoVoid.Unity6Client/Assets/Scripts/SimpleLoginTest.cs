using UnityEngine;
using ChronoVoid.Client;

namespace ChronoVoid.Client.Test
{
    public class SimpleLoginTest : MonoBehaviour
    {
        [Header("Test Configuration")]
        public bool autoTest = true;
        public string testUsername = "gameuser123";
        public string testEmail = "gameuser@example.com";
        public string testPassword = "password123";

        private ApiClient apiClient;
        private bool testCompleted = false;

        private void Start()
        {
            Debug.Log("=== ChronoVoid 2500 - Simple Login Test ===");
            
            // Find or create ApiClient
            apiClient = FindFirstObjectByType<ApiClient>();
            if (apiClient == null)
            {
                GameObject apiObject = new GameObject("ApiClient");
                apiClient = apiObject.AddComponent<ApiClient>();
                Debug.Log("Created ApiClient instance");
            }

            Debug.Log($"API Base URL: {apiClient.apiBaseUrl}");

            // First, do a quick connectivity test
            TestConnectivity();

            if (autoTest && !testCompleted)
            {
                Debug.Log("Starting full test in 3 seconds...");
                Invoke(nameof(StartTest), 3f);
            }
        }

        private void StartTest()
        {
            StartCoroutine(RunTestSequence());
        }

        private System.Collections.IEnumerator RunTestSequence()
        {
            testCompleted = true;
            
            Debug.Log("Starting automated test sequence...");
            yield return new WaitForSeconds(1f);

            // Test 1: Register a new user
            Debug.Log("=== TEST 1: User Registration ===");
            bool registrationComplete = false;
            string registrationResult = "";

            // Use a unique username to avoid conflicts
            string uniqueUsername = testUsername + System.DateTime.Now.Ticks.ToString().Substring(10);
            
            var registerRequest = new RegisterRequest
            {
                username = uniqueUsername,
                email = $"user{System.DateTime.Now.Ticks}@example.com",
                password = testPassword
            };

            Debug.Log($"Attempting to register user: {uniqueUsername}");

            apiClient.Register(registerRequest, 
                (response) => {
                    Debug.Log($"✅ Registration successful! User: {response.username}, ID: {response.userId}");
                    registrationResult = "success";
                    registrationComplete = true;
                    
                    // Update test username for login test
                    testUsername = uniqueUsername;
                },
                (error) => {
                    Debug.Log($"⚠️ Registration failed: {error}");
                    registrationResult = "failed";
                    registrationComplete = true;
                });

            // Wait for registration to complete with timeout
            float timeout = 10f;
            float elapsed = 0f;
            while (!registrationComplete && elapsed < timeout)
            {
                elapsed += 0.1f;
                yield return new WaitForSeconds(0.1f);
            }

            if (!registrationComplete)
            {
                Debug.LogError("❌ Registration timed out after 10 seconds");
                yield break;
            }

            yield return new WaitForSeconds(1f);

            // Test 2: Login with the user (only if registration succeeded)
            Debug.Log("=== TEST 2: User Login ===");
            bool loginComplete = false;
            string loginResult = "";

            if (registrationResult == "success")
            {
                var loginRequest = new LoginRequest
                {
                    username = testUsername,
                    password = testPassword
                };

                Debug.Log($"Attempting to login user: {testUsername}");

                apiClient.Login(loginRequest,
                    (response) => {
                        Debug.Log($"✅ Login successful! Welcome {response.username}!");
                        Debug.Log($"User ID: {response.userId}, Email: {response.email}");
                        loginResult = "success";
                        loginComplete = true;
                        
                        // Store user data
                        PlayerPrefs.SetInt("UserId", response.userId);
                        PlayerPrefs.SetString("Username", response.username);
                        PlayerPrefs.Save();
                    },
                    (error) => {
                        Debug.LogError($"❌ Login failed: {error}");
                        loginResult = "failed";
                        loginComplete = true;
                    });

                // Wait for login to complete with timeout
                elapsed = 0f;
                while (!loginComplete && elapsed < timeout)
                {
                    elapsed += 0.1f;
                    yield return new WaitForSeconds(0.1f);
                }

                if (!loginComplete)
                {
                    Debug.LogError("❌ Login timed out after 10 seconds");
                }
            }
            else
            {
                Debug.Log("⏭️ Skipping login test due to registration failure");
                loginComplete = true;
            }

            yield return new WaitForSeconds(1f);

            // Test 3: Get available realms
            Debug.Log("=== TEST 3: Loading Realms ===");
            bool realmsComplete = false;

            apiClient.GetRealms(
                (realms) => {
                    Debug.Log($"✅ Found {realms.Length} realms:");
                    foreach (var realm in realms)
                    {
                        Debug.Log($"  - {realm.name} ({realm.nodeCount} nodes)");
                    }
                    realmsComplete = true;
                },
                (error) => {
                    Debug.LogError($"❌ Failed to load realms: {error}");
                    realmsComplete = true;
                });

            // Wait for realms to load with timeout
            elapsed = 0f;
            while (!realmsComplete && elapsed < timeout)
            {
                elapsed += 0.1f;
                yield return new WaitForSeconds(0.1f);
            }

            if (!realmsComplete)
            {
                Debug.LogError("❌ Realms loading timed out after 10 seconds");
            }

            Debug.Log("=== TEST SEQUENCE COMPLETE ===");
            Debug.Log("Check the console for results. If all tests passed, the login system is working!");
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 400, 300));
            
            GUILayout.Label("ChronoVoid 2500 - Login Test", GUI.skin.box);
            GUILayout.Space(10);
            
            GUILayout.Label($"API URL: {(apiClient != null ? apiClient.apiBaseUrl : "Not found")}");
            GUILayout.Label($"Test User: {testUsername}");
            GUILayout.Label($"Test Email: {testEmail}");
            
            GUILayout.Space(10);
            
            if (!testCompleted && GUILayout.Button("Run Manual Test"))
            {
                StartCoroutine(RunTestSequence());
            }
            
            if (GUILayout.Button("Test Registration Only"))
            {
                TestRegistration();
            }
            
            if (GUILayout.Button("Test Login Only"))
            {
                TestLogin();
            }
            
            if (GUILayout.Button("Test Get Realms"))
            {
                TestGetRealms();
            }
            
            GUILayout.EndArea();
        }

        private void TestRegistration()
        {
            var registerRequest = new RegisterRequest
            {
                username = testUsername + Random.Range(100, 999),
                email = $"user{Random.Range(100, 999)}@example.com",
                password = testPassword
            };

            apiClient.Register(registerRequest,
                (response) => Debug.Log($"✅ Registration: {response.username} created!"),
                (error) => Debug.LogError($"❌ Registration failed: {error}"));
        }

        private void TestLogin()
        {
            var loginRequest = new LoginRequest
            {
                username = testUsername,
                password = testPassword
            };

            apiClient.Login(loginRequest,
                (response) => Debug.Log($"✅ Login: Welcome {response.username}!"),
                (error) => Debug.LogError($"❌ Login failed: {error}"));
        }

        private void TestGetRealms()
        {
            apiClient.GetRealms(
                (realms) => Debug.Log($"✅ Found {realms.Length} realms"),
                (error) => Debug.LogError($"❌ Realms failed: {error}"));
        }

        private void TestConnectivity()
        {
            Debug.Log("🔍 Testing API connectivity...");
            Debug.Log($"Unity Version: {Application.unityVersion}");
            Debug.Log($"Graphics API: {SystemInfo.graphicsDeviceType}");
            StartCoroutine(ConnectivityTest());
        }

        private System.Collections.IEnumerator ConnectivityTest()
        {
            bool connectivityComplete = false;
            
            apiClient.GetRealms(
                (realms) => {
                    Debug.Log($"🌐 ✅ API Connection successful! Found {realms.Length} realms");
                    connectivityComplete = true;
                },
                (error) => {
                    Debug.LogError($"🌐 ❌ API Connection failed: {error}");
                    Debug.LogError("Check that the API server is running on http://localhost:7000");
                    connectivityComplete = true;
                });

            // Wait for connectivity test with short timeout
            float elapsed = 0f;
            float timeout = 5f;
            while (!connectivityComplete && elapsed < timeout)
            {
                elapsed += 0.1f;
                yield return new WaitForSeconds(0.1f);
            }

            if (!connectivityComplete)
            {
                Debug.LogError("🌐 ❌ API connectivity test timed out - API server may not be running");
            }
        }
    }
}