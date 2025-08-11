using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace ChronoVoid.Client.UI
{
    public class RealmListUI : MonoBehaviour
    {
        [Header("UI References")]
        public Transform realmListParent;
        public GameObject realmItemPrefab;
        public Button createRealmButton;
        public Button refreshButton;
        public Button logoutButton;
        public TextMeshProUGUI welcomeText;
        public TextMeshProUGUI statusText;

        [Header("Space Theme")]
        public ParticleSystem starField;
        public AudioSource backgroundMusic;
        public Image backgroundImage;

        private string currentUsername;
        private int currentUserId;
        private ApiClient apiClient;

        private void Start()
        {
            apiClient = ApiClient.Instance;
            if (apiClient == null)
            {
                Debug.LogError("ApiClient instance not found!");
                return;
            }

            LoadUserData();
            InitializeUI();
            LoadRealms();
        }

        private void LoadUserData()
        {
            // Get user data from PlayerPrefs (set during login)
            currentUserId = PlayerPrefs.GetInt("UserId", 0);
            currentUsername = PlayerPrefs.GetString("Username", "Unknown User");

            if (currentUserId == 0)
            {
                Debug.LogWarning("No user data found, returning to login");
                SceneManager.LoadScene("LoginScene");
                return;
            }
        }

        private void InitializeUI()
        {
            // Set welcome message with space-themed styling
            welcomeText.text = $"Hello, {currentUsername}!";
            statusText.text = "Loading realms from the cosmic nexus...";
            
            // Setup buttons
            createRealmButton.onClick.AddListener(OpenCreateRealmScene);
            refreshButton.onClick.AddListener(LoadRealms);
            logoutButton.onClick.AddListener(OnLogout);
            
            // Start space theme effects
            if (starField != null) starField.Play();
            if (backgroundMusic != null) backgroundMusic.Play();
        }

        private void LoadRealms()
        {
            statusText.text = "🌌 Scanning the cosmic nexus for realms...";
            ClearRealmList();
            
            apiClient.GetRealms(OnRealmsLoaded, OnRealmsError);
        }

        private void OnRealmsLoaded(NexusRealmDto[] realms)
        {
            if (realms.Length == 0)
            {
                statusText.text = "🚀 No realms detected. Create your first cosmic domain!";
                return;
            }

            statusText.text = $"✨ {realms.Length} realms discovered in the nexus";
            
            foreach (var realm in realms)
            {
                CreateRealmListItem(realm);
            }
        }

        private void OnRealmsError(string error)
        {
            statusText.text = $"⚠️ Nexus connection failed: {error}";
            Debug.LogError($"Failed to load realms: {error}");
        }

        private void CreateRealmListItem(NexusRealmDto realm)
        {
            GameObject item = Instantiate(realmItemPrefab, realmListParent);
            RealmListItem realmItem = item.GetComponent<RealmListItem>();
            
            if (realmItem != null)
            {
                realmItem.SetupRealm(realm, OnRealmSelected);
            }
        }

        private void OnRealmSelected(NexusRealmDto realm)
        {
            Debug.Log($"Selected realm: {realm.name}");
            statusText.text = $"🚀 Joining realm: {realm.name}...";
            
            // Join the realm through the API
            var joinRequest = new JoinRealmRequest
            {
                userId = currentUserId,
                realmId = realm.id
            };

            apiClient.JoinRealm(joinRequest, OnJoinRealmSuccess, OnJoinRealmError);
        }

        private void OnJoinRealmSuccess(JoinRealmResponse response)
        {
            statusText.text = $"✨ {response.message}";
            
            // Store realm data and navigate to game
            PlayerPrefs.SetInt("CurrentRealmId", response.startingNode.id);
            PlayerPrefs.SetString("CurrentRealmName", response.startingNode.realmName);
            PlayerPrefs.Save();

            // Load navigation scene after a brief delay
            Invoke(nameof(LoadNavigationScene), 1.5f);
        }

        private void OnJoinRealmError(string error)
        {
            statusText.text = $"⚠️ Failed to join realm: {error}";
            Debug.LogError($"Join realm error: {error}");
        }

        private void LoadNavigationScene()
        {
            SceneManager.LoadScene("NavigationScene");
        }

        private void ClearRealmList()
        {
            foreach (Transform child in realmListParent)
            {
                Destroy(child.gameObject);
            }
        }

        private void OnLogout()
        {
            // Clear stored user data
            PlayerPrefs.DeleteKey("UserId");
            PlayerPrefs.DeleteKey("Username");
            PlayerPrefs.DeleteKey("UserEmail");
            PlayerPrefs.DeleteKey("UserToken");
            PlayerPrefs.DeleteKey("CurrentRealmId");
            PlayerPrefs.DeleteKey("CurrentRealmName");
            PlayerPrefs.Save();

            // Return to login scene
            SceneManager.LoadScene("LoginScene");
        }

        private void OpenCreateRealmScene()
        {
            SceneManager.LoadScene("CreateRealmScene");
        }
    }
}