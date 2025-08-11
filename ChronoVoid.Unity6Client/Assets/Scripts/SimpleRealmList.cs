using UnityEngine;
using UnityEngine.SceneManagement;
using ChronoVoid.Client;
using System.Collections;
using System.Collections.Generic;

namespace ChronoVoid.Client
{
    /// <summary>
    /// Simple realm list system with logout functionality
    /// </summary>
    public class SimpleRealmList : MonoBehaviour
    {
        [Header("Configuration")]
        public string loginSceneName = "LoginTestScene";
        
        private ApiClient apiClient;
        private string currentUsername;
        private int currentUserId;
        private string currentToken;
        private List<RealmInfo> realms = new List<RealmInfo>();
        private bool isLoading = false;
        private string statusMessage = "";
        private Color statusColor = Color.white;
        
        // Simple realm info class
        [System.Serializable]
        public class RealmInfo
        {
            public int id;
            public string name;
            public string description;
            public int playerCount;
            public bool isActive;
        }
        
        private void Start()
        {
            Debug.Log("=== ChronoVoid 2500 - Realm List ===");
            
            LoadUserData();
            
            // Find ApiClient
            apiClient = FindFirstObjectByType<ApiClient>();
            if (apiClient == null)
            {
                GameObject apiObject = new GameObject("ApiClient");
                apiClient = apiObject.AddComponent<ApiClient>();
                Debug.Log("Created ApiClient instance");
            }
            
            if (string.IsNullOrEmpty(currentUsername))
            {
                SetStatus("❌ No user logged in. Redirecting to login...", Color.red);
                Invoke(nameof(GoToLogin), 2f);
                return;
            }
            
            SetStatus($"Welcome, {currentUsername}! Loading realms...", Color.cyan);
            StartCoroutine(LoadRealms());
        }
        
        private void LoadUserData()
        {
            currentUserId = PlayerPrefs.GetInt("UserId", 0);
            currentUsername = PlayerPrefs.GetString("Username", "");
            currentToken = PlayerPrefs.GetString("UserToken", "");
            
            Debug.Log($"Loaded user data: ID={currentUserId}, Username={currentUsername}");
        }
        
        private IEnumerator LoadRealms()
        {
            isLoading = true;
            SetStatus("🌌 Loading available realms...", Color.yellow);
            
            // Simulate loading realms (replace with actual API call)
            yield return new WaitForSeconds(1f);
            
            // Mock realm data for now
            realms.Clear();
            realms.Add(new RealmInfo { id = 1, name = "Alpha Centauri", description = "Starter realm for new traders", playerCount = 42, isActive = true });
            realms.Add(new RealmInfo { id = 2, name = "Vega Prime", description = "Advanced trading hub", playerCount = 128, isActive = true });
            realms.Add(new RealmInfo { id = 3, name = "Orion Nebula", description = "Dangerous but profitable", playerCount = 67, isActive = true });
            realms.Add(new RealmInfo { id = 4, name = "Sol System", description = "Earth's home system", playerCount = 203, isActive = true });
            realms.Add(new RealmInfo { id = 5, name = "Kepler Station", description = "Research and development hub", playerCount = 89, isActive = false });
            
            isLoading = false;
            SetStatus($"✅ Found {realms.Count} realms available", Color.green);
            
            Debug.Log($"✅ Loaded {realms.Count} realms for {currentUsername}");
        }
        
        private void OnGUI()
        {
            // Set up GUI styling
            GUI.skin.button.fontSize = 16;
            GUI.skin.label.fontSize = 18;
            GUI.skin.box.fontSize = 20;
            
            // Main container
            GUILayout.BeginArea(new Rect(50, 50, Screen.width - 100, Screen.height - 100));
            
            // Header
            GUI.color = Color.cyan;
            GUILayout.Box("CHRONOVOID 2500 - REALM SELECTION", GUILayout.Height(50));
            GUI.color = Color.white;
            
            GUILayout.Space(10);
            
            // User info and logout
            GUILayout.BeginHorizontal();
            GUI.color = Color.green;
            GUILayout.Box($"👤 Logged in as: {currentUsername}", GUILayout.Height(40));
            GUI.color = Color.white;
            
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("🚪 LOGOUT", GUILayout.Height(40), GUILayout.Width(120)))
            {
                Logout();
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();
            
            GUILayout.Space(20);
            
            // Status
            GUI.color = statusColor;
            GUILayout.Box(statusMessage, GUILayout.Height(40));
            GUI.color = Color.white;
            
            GUILayout.Space(10);
            
            // Loading indicator
            if (isLoading)
            {
                GUI.color = Color.yellow;
                GUILayout.Box("⏳ LOADING REALMS... PLEASE WAIT", GUILayout.Height(40));
                GUI.color = Color.white;
            }
            else if (realms.Count > 0)
            {
                // Realm list
                GUILayout.Label("🌌 Available Realms:", GUI.skin.box);
                
                GUILayout.BeginVertical("box");
                
                foreach (var realm in realms)
                {
                    GUILayout.BeginHorizontal("box");
                    
                    // Realm info
                    GUILayout.BeginVertical();
                    GUI.color = realm.isActive ? Color.white : Color.gray;
                    GUILayout.Label($"🌟 {realm.name}", GUI.skin.box);
                    GUILayout.Label($"   {realm.description}");
                    GUILayout.Label($"   👥 {realm.playerCount} players online");
                    GUI.color = Color.white;
                    GUILayout.EndVertical();
                    
                    // Join button
                    GUI.enabled = realm.isActive;
                    GUI.backgroundColor = realm.isActive ? Color.green : Color.gray;
                    if (GUILayout.Button(realm.isActive ? "🚀 JOIN" : "OFFLINE", GUILayout.Width(100), GUILayout.Height(60)))
                    {
                        JoinRealm(realm);
                    }
                    GUI.backgroundColor = Color.white;
                    GUI.enabled = true;
                    
                    GUILayout.EndHorizontal();
                    GUILayout.Space(5);
                }
                
                GUILayout.EndVertical();
                
                GUILayout.Space(20);
                
                // Action buttons
                GUILayout.BeginHorizontal();
                GUI.backgroundColor = Color.blue;
                if (GUILayout.Button("🔄 REFRESH REALMS", GUILayout.Height(40)))
                {
                    StartCoroutine(LoadRealms());
                }
                
                GUI.backgroundColor = Color.orange;
                if (GUILayout.Button("➕ CREATE NEW REALM", GUILayout.Height(40)))
                {
                    CreateNewRealm();
                }
                GUI.backgroundColor = Color.white;
                GUILayout.EndHorizontal();
            }
            
            GUILayout.EndArea();
        }
        
        private void JoinRealm(RealmInfo realm)
        {
            SetStatus($"🚀 Joining {realm.name}...", Color.yellow);
            Debug.Log($"Player {currentUsername} joining realm: {realm.name}");
            
            // Here you would typically:
            // 1. Call API to join the realm
            // 2. Load the game scene for that realm
            // 3. Pass realm data to the game
            
            // For now, just show a message
            StartCoroutine(SimulateJoinRealm(realm));
        }
        
        private IEnumerator SimulateJoinRealm(RealmInfo realm)
        {
            yield return new WaitForSeconds(1f);
            SetStatus($"✅ Successfully joined {realm.name}! Loading game...", Color.green);
            
            // Store selected realm info
            PlayerPrefs.SetInt("SelectedRealmId", realm.id);
            PlayerPrefs.SetString("SelectedRealmName", realm.name);
            PlayerPrefs.Save();
            
            yield return new WaitForSeconds(2f);
            
            // Here you would load the actual game scene
            // SceneManager.LoadScene("GameScene");
            SetStatus($"🎮 Game loading for {realm.name} would happen here!", Color.cyan);
        }
        
        private void CreateNewRealm()
        {
            SetStatus("🛠️ Realm creation would open here!", Color.orange);
            Debug.Log("Create new realm functionality would be implemented here");
            
            // Here you would typically:
            // 1. Open a realm creation dialog
            // 2. Let user configure realm settings
            // 3. Call API to create the realm
            // 4. Refresh the realm list
        }
        
        private void Logout()
        {
            SetStatus("🚪 Logging out...", Color.yellow);
            Debug.Log($"Logging out user: {currentUsername}");
            
            // Clear user data
            FullLoginSystem.Logout();
            
            // Go back to login scene
            GoToLogin();
        }
        
        private void GoToLogin()
        {
            try
            {
                SceneManager.LoadScene(loginSceneName);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load login scene '{loginSceneName}': {e.Message}");
                SetStatus($"❌ Could not return to login", Color.red);
            }
        }
        
        private void SetStatus(string message, Color color)
        {
            statusMessage = message;
            statusColor = color;
            Debug.Log($"Status: {message}");
        }
    }
}