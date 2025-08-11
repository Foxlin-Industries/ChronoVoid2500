using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace ChronoVoid.Client.UI
{
    public class RealmListItem : MonoBehaviour
    {
        [Header("UI Components")]
        public TextMeshProUGUI realmNameText;
        public TextMeshProUGUI nodeCountText;
        public TextMeshProUGUI stationRateText;
        public TextMeshProUGUI createdDateText;
        public TextMeshProUGUI noDeadNodesText;
        public Button selectButton;
        public Image backgroundImage;

        private NexusRealmDto realmData;
        private System.Action<NexusRealmDto> onRealmSelected;

        public void SetupRealm(NexusRealmDto realm, System.Action<NexusRealmDto> onSelected)
        {
            realmData = realm;
            onRealmSelected = onSelected;

            // Update UI elements
            realmNameText.text = realm.name;
            nodeCountText.text = $"🌐 {realm.nodeCount} Neural Nodes";
            stationRateText.text = $"🏭 {realm.quantumStationSeedRate}% Quantum Stations";
            noDeadNodesText.text = realm.noDeadNodes ? "✅ No Dead Nodes" : "⚠️ Dead Nodes Allowed";
            
            // Format creation date
            if (DateTime.TryParse(realm.createdAt, out DateTime createdDate))
            {
                createdDateText.text = $"📅 Created: {createdDate:MMM dd, yyyy}";
            }
            else
            {
                createdDateText.text = "📅 Created: Unknown";
            }

            // Set up button
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(OnSelectRealm);

            // Visual styling
            SetupVisualStyling();
        }

        private void SetupVisualStyling()
        {
            // Color coding based on realm properties
            Color bgColor = Color.white;
            
            if (realmData.noDeadNodes)
            {
                bgColor = new Color(0.2f, 0.8f, 0.2f, 0.1f); // Green tint for no dead nodes
            }
            else
            {
                bgColor = new Color(0.8f, 0.8f, 0.2f, 0.1f); // Yellow tint for potential dead nodes
            }

            if (backgroundImage != null)
            {
                backgroundImage.color = bgColor;
            }

            // Size-based visual cues
            if (realmData.nodeCount >= 1000)
            {
                realmNameText.color = new Color(1f, 0.8f, 0f); // Gold for large realms
            }
            else if (realmData.nodeCount >= 100)
            {
                realmNameText.color = new Color(0.5f, 0.8f, 1f); // Light blue for medium realms
            }
            else
            {
                realmNameText.color = Color.white; // White for small realms
            }
        }

        private void OnSelectRealm()
        {
            Debug.Log($"Selecting realm: {realmData.name}");
            onRealmSelected?.Invoke(realmData);
        }

        // Visual feedback for button interactions
        public void OnPointerEnter()
        {
            if (backgroundImage != null)
            {
                Color currentColor = backgroundImage.color;
                currentColor.a = 0.3f;
                backgroundImage.color = currentColor;
            }
        }

        public void OnPointerExit()
        {
            if (backgroundImage != null)
            {
                Color currentColor = backgroundImage.color;
                currentColor.a = 0.1f;
                backgroundImage.color = currentColor;
            }
        }
    }
}