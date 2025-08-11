using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using ChronoVoid.Client.Compatibility;

namespace ChronoVoid.Client
{
    public class GameManager : MonoBehaviour
    {
        [Header("Game State")]
        public int currentUserId = 0;
        public string currentUsername = "";
        public int currentNodeId = 0;
        public int currentRealmId = 0;
        
        [Header("UI References")]
        public GameObject loadingPanel;
        public GameObject errorPanel;
        
        public static GameManager Instance { get; private set; }
        
        // Current game state
        public NexusRealmDto[] availableRealms;
        public NodeDetailDto currentNode;
        public UserLocationDto userLocation;
        public AuthResponse currentUser;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeGame();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeGame()
        {
            Debug.Log("ChronoVoid 2500 - Initializing Game", this);
            
            // Unity 6 enhanced logging
            Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, this, 
                "Unity Version: {0}, Platform: {1}", Application.unityVersion, Application.platform);
            
            // Unity 6 compatibility check
            Unity6Compatibility.ValidateGraphicsAPI();
            
            // Load user data from PlayerPrefs if available
            LoadUserSession();
        }

        public void LoadUserSession()
        {
            currentUserId = PlayerPrefs.GetInt("UserId", 0);
            currentUsername = PlayerPrefs.GetString("Username", "");
            
            if (currentUserId > 0)
            {
                Debug.Log($"Loaded user session: {currentUsername} (ID: {currentUserId})");
            }
        }

        public void SetCurrentUser(AuthResponse user)
        {
            currentUser = user;
            currentUserId = user.userId;
            currentUsername = user.username;
            
            // Store in PlayerPrefs for persistence
            PlayerPrefs.SetInt("UserId", user.userId);
            PlayerPrefs.SetString("Username", user.username);
            PlayerPrefs.SetString("UserEmail", user.email);
            PlayerPrefs.SetString("UserToken", user.token);
            PlayerPrefs.Save();
            
            Debug.Log($"Set current user: {user.username}");
        }

        public void LoadAvailableRealms()
        {
            ShowLoading(true);
            
            ApiClient.Instance.GetRealms(
                onSuccess: (realms) =>
                {
                    availableRealms = realms;
                    ShowLoading(false);
                    Debug.Log($"Loaded {realms.Length} realms");
                },
                onError: (error) =>
                {
                    ShowLoading(false);
                    ShowError($"Failed to load realms: {error}");
                }
            );
        }

        public void LoadUserLocation(int userId)
        {
            currentUserId = userId;
            ShowLoading(true);
            
            ApiClient.Instance.GetUserLocation(userId,
                onSuccess: (location) =>
                {
                    userLocation = location;
                    currentNodeId = location.currentNodeId;
                    currentRealmId = location.realmId;
                    ShowLoading(false);
                    
                    // Load current node details
                    LoadCurrentNodeDetails();
                },
                onError: (error) =>
                {
                    ShowLoading(false);
                    Debug.LogWarning($"User location not found: {error}");
                    // User doesn't exist yet - will be created on first movement
                }
            );
        }

        public void LoadCurrentNodeDetails()
        {
            if (currentNodeId <= 0) return;
            
            ShowLoading(true);
            
            ApiClient.Instance.GetNodeDetails(currentNodeId,
                onSuccess: (node) =>
                {
                    currentNode = node;
                    ShowLoading(false);
                    Debug.Log($"Loaded node {node.nodeNumber} in {node.realmName}");
                },
                onError: (error) =>
                {
                    ShowLoading(false);
                    ShowError($"Failed to load node details: {error}");
                }
            );
        }

        public void MoveToNode(int toNodeId)
        {
            if (currentNodeId <= 0)
            {
                ShowError("Current location not set");
                return;
            }

            ShowLoading(true);
            
            var moveRequest = new MoveRequestDto
            {
                userId = currentUserId,
                fromNodeId = currentNodeId,
                toNodeId = toNodeId
            };

            ApiClient.Instance.MoveToNode(moveRequest,
                onSuccess: (result) =>
                {
                    currentNodeId = toNodeId;
                    currentNode = result.currentNode;
                    ShowLoading(false);
                    
                    Debug.Log($"Movement successful: {result.message}");
                    
                    // Update user location
                    LoadUserLocation(currentUserId);
                },
                onError: (error) =>
                {
                    ShowLoading(false);
                    ShowError($"Movement failed: {error}");
                }
            );
        }

        public void CreateNewRealm(string realmName, int nodeCount, int stationSeedRate, bool noDeadNodes)
        {
            ShowLoading(true);
            
            var createRequest = new CreateRealmRequest
            {
                name = realmName,
                nodeCount = nodeCount,
                quantumStationSeedRate = stationSeedRate,
                noDeadNodes = noDeadNodes
            };

            ApiClient.Instance.CreateRealm(createRequest,
                onSuccess: (realm) =>
                {
                    ShowLoading(false);
                    Debug.Log($"Created realm: {realm.name} with {realm.nodeCount} nodes");
                    
                    // Refresh realm list
                    LoadAvailableRealms();
                },
                onError: (error) =>
                {
                    ShowLoading(false);
                    ShowError($"Failed to create realm: {error}");
                }
            );
        }

        public void ShowLoading(bool show)
        {
            if (loadingPanel != null)
                loadingPanel.SetActive(show);
        }

        public void ShowError(string message)
        {
            Debug.LogError(message);
            
            if (errorPanel != null)
            {
                errorPanel.SetActive(true);
                // You could add a Text component to show the error message
            }
        }

        public void HideError()
        {
            if (errorPanel != null)
                errorPanel.SetActive(false);
        }

        // Scene management
        public void LoadLoginScene()
        {
            SceneManager.LoadScene("LoginScene");
        }

        public void LoadRealmListScene()
        {
            SceneManager.LoadScene("RealmListScene");
        }

        public void LoadNavigationScene()
        {
            SceneManager.LoadScene("NavigationScene");
        }

        public void LoadCreateRealmScene()
        {
            SceneManager.LoadScene("CreateRealmScene");
        }

        public void Logout()
        {
            // Clear all user data
            currentUser = null;
            currentUserId = 0;
            currentUsername = "";
            currentNodeId = 0;
            currentRealmId = 0;
            
            // Clear PlayerPrefs
            PlayerPrefs.DeleteKey("UserId");
            PlayerPrefs.DeleteKey("Username");
            PlayerPrefs.DeleteKey("UserEmail");
            PlayerPrefs.DeleteKey("UserToken");
            PlayerPrefs.DeleteKey("CurrentRealmId");
            PlayerPrefs.DeleteKey("CurrentRealmName");
            PlayerPrefs.Save();
            
            // Return to login
            LoadLoginScene();
        }
    }
}