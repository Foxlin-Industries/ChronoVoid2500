using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ChronoVoid.Client.UI
{
    public class CreateRealmUI : MonoBehaviour
    {
        [Header("Input Fields")]
        public TMP_InputField realmNameInput;
        public Slider nodeCountSlider;
        public TextMeshProUGUI nodeCountText;
        public Slider stationSeedRateSlider;
        public TextMeshProUGUI stationSeedRateText;
        public Toggle noDeadNodesToggle;

        [Header("Buttons")]
        public Button createRealmButton;
        public Button backButton;

        [Header("Status")]
        public TextMeshProUGUI statusText;
        public GameObject loadingPanel;

        private void Start()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            // Setup button listeners
            createRealmButton.onClick.AddListener(CreateRealm);
            backButton.onClick.AddListener(BackToRealmList);

            // Setup slider listeners
            nodeCountSlider.onValueChanged.AddListener(OnNodeCountChanged);
            stationSeedRateSlider.onValueChanged.AddListener(OnStationSeedRateChanged);

            // Set initial values
            nodeCountSlider.minValue = 10;
            nodeCountSlider.maxValue = 1000;
            nodeCountSlider.value = 100;
            
            stationSeedRateSlider.minValue = 0;
            stationSeedRateSlider.maxValue = 100;
            stationSeedRateSlider.value = 30;

            noDeadNodesToggle.isOn = true;

            // Set default realm name
            realmNameInput.text = $"Galaxy {System.DateTime.Now:MMdd_HHmm}";

            // Update display texts
            OnNodeCountChanged(nodeCountSlider.value);
            OnStationSeedRateChanged(stationSeedRateSlider.value);

            statusText.text = "Configure your new Nexus Realm";
            
            if (loadingPanel != null)
                loadingPanel.SetActive(false);
        }

        private void OnNodeCountChanged(float value)
        {
            int nodeCount = Mathf.RoundToInt(value);
            nodeCountText.text = $"{nodeCount} Neural Nodes";
            
            // Update estimated generation time
            float estimatedTime = nodeCount * 0.01f; // Rough estimate
            if (estimatedTime > 1f)
            {
                statusText.text = $"Estimated generation time: ~{estimatedTime:F1} seconds";
            }
        }

        private void OnStationSeedRateChanged(float value)
        {
            int seedRate = Mathf.RoundToInt(value);
            stationSeedRateText.text = $"{seedRate}% Quantum Stations";
            
            // Calculate expected station count
            int nodeCount = Mathf.RoundToInt(nodeCountSlider.value);
            int expectedStations = Mathf.RoundToInt(nodeCount * seedRate / 100f);
            stationSeedRateText.text += $" (~{expectedStations} stations)";
        }

        private void CreateRealm()
        {
            string realmName = realmNameInput.text.Trim();
            
            // Validation
            if (string.IsNullOrEmpty(realmName))
            {
                statusText.text = "❌ Realm name cannot be empty";
                return;
            }

            if (realmName.Length < 3)
            {
                statusText.text = "❌ Realm name must be at least 3 characters";
                return;
            }

            int nodeCount = Mathf.RoundToInt(nodeCountSlider.value);
            int stationSeedRate = Mathf.RoundToInt(stationSeedRateSlider.value);
            bool noDeadNodes = noDeadNodesToggle.isOn;

            // Show loading
            statusText.text = "🚀 Creating Nexus Realm...";
            createRealmButton.interactable = false;
            
            if (loadingPanel != null)
                loadingPanel.SetActive(true);

            // Create realm via GameManager
            GameManager.Instance.CreateNewRealm(realmName, nodeCount, stationSeedRate, noDeadNodes);

            // Wait for creation, then handle result
            Invoke(nameof(CheckCreationResult), 3f);
        }

        private void CheckCreationResult()
        {
            // Re-enable button
            createRealmButton.interactable = true;
            
            if (loadingPanel != null)
                loadingPanel.SetActive(false);

            // Check if new realm was added (simple check)
            GameManager.Instance.LoadAvailableRealms();
            
            statusText.text = "✅ Realm creation initiated! Returning to realm list...";
            
            // Go back to realm list after a moment
            Invoke(nameof(BackToRealmList), 2f);
        }

        private void BackToRealmList()
        {
            GameManager.Instance.LoadRealmListScene();
        }

        // Preview updates
        private void Update()
        {
            // Update real-time preview information
            UpdatePreviewInfo();
        }

        private void UpdatePreviewInfo()
        {
            // Show dynamic information about the realm being created
            int nodeCount = Mathf.RoundToInt(nodeCountSlider.value);
            int stationSeedRate = Mathf.RoundToInt(stationSeedRateSlider.value);
            
            string previewText = $"🌌 Preview: {nodeCount} nodes";
            
            if (noDeadNodesToggle.isOn)
            {
                previewText += " (all connected)";
            }
            else
            {
                previewText += " (dead nodes possible)";
            }
            
            // Only update if not currently creating
            if (createRealmButton.interactable && !statusText.text.Contains("Creating"))
            {
                statusText.text = previewText;
            }
        }
    }
}