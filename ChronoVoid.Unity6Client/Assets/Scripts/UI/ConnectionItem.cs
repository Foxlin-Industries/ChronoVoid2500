using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace ChronoVoid.Client.UI
{
    public class ConnectionItem : MonoBehaviour
    {
        [Header("UI Components")]
        public TextMeshProUGUI nodeNumberText;
        public TextMeshProUGUI coordinatesText;
        public TextMeshProUGUI quantumStationText;
        public Button travelButton;
        public Image backgroundImage;

        private ConnectedNodeDto connectionData;
        private System.Action<ConnectedNodeDto> onConnectionSelected;

        public void SetupConnection(ConnectedNodeDto connection, System.Action<ConnectedNodeDto> onSelected)
        {
            connectionData = connection;
            onConnectionSelected = onSelected;

            // Update UI elements
            nodeNumberText.text = $"Neural Node {connection.nodeNumber}";
            coordinatesText.text = $"📍 ({connection.coordinateX}, {connection.coordinateY})";
            quantumStationText.text = connection.hasQuantumStation ? "🏭" : "⚪";

            // Set up travel button
            travelButton.onClick.RemoveAllListeners();
            travelButton.onClick.AddListener(OnTravelButtonClicked);

            // Visual styling
            SetupVisualStyling();
        }

        private void SetupVisualStyling()
        {
            // Color coding based on quantum station presence
            Color bgColor;
            
            if (connectionData.hasQuantumStation)
            {
                bgColor = new Color(0.2f, 0.8f, 0.8f, 0.2f); // Cyan tint for quantum stations
                quantumStationText.color = new Color(1f, 0.8f, 0f); // Gold for station icon
            }
            else
            {
                bgColor = new Color(0.3f, 0.3f, 0.3f, 0.1f); // Neutral gray
                quantumStationText.color = Color.gray;
            }

            if (backgroundImage != null)
            {
                backgroundImage.color = bgColor;
            }

            // Distance-based visual cues (calculated from coordinates)
            float distance = Vector2.Distance(Vector2.zero, new Vector2(connectionData.coordinateX, connectionData.coordinateY));
            
            if (distance > 500f)
            {
                nodeNumberText.color = new Color(1f, 0.5f, 0.5f); // Red tint for distant nodes
            }
            else if (distance > 200f)
            {
                nodeNumberText.color = new Color(1f, 1f, 0.5f); // Yellow tint for medium distance
            }
            else
            {
                nodeNumberText.color = new Color(0.5f, 1f, 0.5f); // Green tint for close nodes
            }
        }

        private void OnTravelButtonClicked()
        {
            Debug.Log($"Traveling to Neural Node {connectionData.nodeNumber}");
            
            // Visual feedback
            StartCoroutine(TravelButtonFeedback());
            
            onConnectionSelected?.Invoke(connectionData);
        }

        private System.Collections.IEnumerator TravelButtonFeedback()
        {
            // Disable button temporarily
            travelButton.interactable = false;
            
            // Change button text
            TextMeshProUGUI buttonText = travelButton.GetComponentInChildren<TextMeshProUGUI>();
            string originalText = buttonText.text;
            buttonText.text = "Traveling...";
            
            // Wait
            yield return new WaitForSeconds(1f);
            
            // Restore button
            buttonText.text = originalText;
            travelButton.interactable = true;
        }

        // Visual feedback for hover effects
        public void OnPointerEnter()
        {
            if (backgroundImage != null)
            {
                Color currentColor = backgroundImage.color;
                currentColor.a = 0.4f;
                backgroundImage.color = currentColor;
            }
        }

        public void OnPointerExit()
        {
            if (backgroundImage != null)
            {
                Color currentColor = backgroundImage.color;
                currentColor.a = 0.2f;
                backgroundImage.color = currentColor;
            }
        }
    }
}