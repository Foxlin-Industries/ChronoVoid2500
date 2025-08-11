using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace ChronoVoid.Client.UI
{
    public class NavigationUI : MonoBehaviour
    {
        [Header("Current Location Info")]
        public TextMeshProUGUI currentNodeText;
        public TextMeshProUGUI realmNameText;
        public TextMeshProUGUI coordinatesText;
        public TextMeshProUGUI quantumStationText;
        public TextMeshProUGUI userInfoText;

        [Header("Navigation Controls")]
        public Transform connectionListParent;
        public GameObject connectionItemPrefab;
        public Button refreshButton;
        public Button backToRealmsButton;

        [Header("User Controls")]
        public TMP_InputField userIdInput;
        public Button getUserLocationButton;
        public Button loadNodeDetailsButton;

        [Header("Status")]
        public TextMeshProUGUI statusText;

        private List<GameObject> connectionItems = new List<GameObject>();

        private void Start()
        {
            InitializeUI();
            LoadCurrentNodeDetails();
        }

        private void InitializeUI()
        {
            // Setup button listeners
            refreshButton.onClick.AddListener(LoadCurrentNodeDetails);
            backToRealmsButton.onClick.AddListener(BackToRealmList);
            getUserLocationButton.onClick.AddListener(GetUserLocation);
            loadNodeDetailsButton.onClick.AddListener(LoadCurrentNodeDetails);

            // Set default user ID
            userIdInput.text = GameManager.Instance.currentUserId.ToString();

            // Initial status
            statusText.text = "Loading navigation data...";
        }

        private void LoadCurrentNodeDetails()
        {
            statusText.text = "Loading node details...";
            ClearConnectionList();

            // Update user ID from input
            if (int.TryParse(userIdInput.text, out int userId))
            {
                GameManager.Instance.currentUserId = userId;
            }

            GameManager.Instance.LoadCurrentNodeDetails();
            
            // Wait for the API call to complete, then update UI
            Invoke(nameof(UpdateNodeUI), 1f);
        }

        private void UpdateNodeUI()
        {
            var currentNode = GameManager.Instance.currentNode;
            
            if (currentNode == null)
            {
                statusText.text = "Failed to load node details";
                return;
            }

            // Update current location info
            currentNodeText.text = $"Neural Node {currentNode.nodeNumber}";
            realmNameText.text = $"🌌 {currentNode.realmName}";
            coordinatesText.text = $"📍 Coordinates: ({currentNode.coordinateX}, {currentNode.coordinateY})";
            quantumStationText.text = currentNode.hasQuantumStation ? "🏭 Quantum Station Present" : "❌ No Quantum Station";

            // Update user info
            var userLocation = GameManager.Instance.userLocation;
            if (userLocation != null)
            {
                userInfoText.text = $"👤 User: {userLocation.username} (ID: {userLocation.userId})";
            }
            else
            {
                userInfoText.text = $"👤 User: TestUser{GameManager.Instance.currentUserId} (ID: {GameManager.Instance.currentUserId})";
            }

            // Create connection items
            CreateConnectionItems(currentNode.connectedNodes);
            
            statusText.text = $"Found {currentNode.connectedNodes.Length} connected nodes";
        }

        private void CreateConnectionItems(ConnectedNodeDto[] connections)
        {
            ClearConnectionList();

            foreach (var connection in connections)
            {
                GameObject item = Instantiate(connectionItemPrefab, connectionListParent);
                ConnectionItem connectionItem = item.GetComponent<ConnectionItem>();
                
                if (connectionItem != null)
                {
                    connectionItem.SetupConnection(connection, OnConnectionSelected);
                    connectionItems.Add(item);
                }
            }
        }

        private void OnConnectionSelected(ConnectedNodeDto connection)
        {
            statusText.text = $"Moving to Neural Node {connection.nodeNumber}...";
            
            GameManager.Instance.MoveToNode(connection.nodeId);
            
            // Refresh UI after movement
            Invoke(nameof(LoadCurrentNodeDetails), 2f);
        }

        private void ClearConnectionList()
        {
            foreach (var item in connectionItems)
            {
                if (item != null)
                    Destroy(item);
            }
            connectionItems.Clear();
        }

        private void GetUserLocation()
        {
            if (int.TryParse(userIdInput.text, out int userId))
            {
                statusText.text = $"Getting location for User {userId}...";
                GameManager.Instance.LoadUserLocation(userId);
                
                // Update UI after getting location
                Invoke(nameof(UpdateNodeUI), 1f);
            }
            else
            {
                statusText.text = "Invalid User ID";
            }
        }

        private void BackToRealmList()
        {
            GameManager.Instance.LoadRealmListScene();
        }

        // Update UI when GameManager state changes
        private void Update()
        {
            // Auto-refresh if we detect the current node has changed
            if (GameManager.Instance.currentNode != null && 
                currentNodeText.text != $"Neural Node {GameManager.Instance.currentNode.nodeNumber}")
            {
                UpdateNodeUI();
            }
        }
    }
}