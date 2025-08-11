using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ChronoVoid.Client;
using UnityEngine.SceneManagement;

namespace ChronoVoid.Client.UI
{
    public class LoginUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject loginPanel;
        [SerializeField] private GameObject registerPanel;
        [SerializeField] private GameObject forgotPasswordPanel;
        [SerializeField] private GameObject welcomePanel;

        [Header("Login Panel")]
        [SerializeField] private TMP_InputField loginUsernameField;
        [SerializeField] private TMP_InputField loginPasswordField;
        [SerializeField] private Button loginButton;
        [SerializeField] private Button showRegisterButton;
        [SerializeField] private Button showForgotPasswordButton;

        [Header("Register Panel")]
        [SerializeField] private TMP_InputField registerUsernameField;
        [SerializeField] private TMP_InputField registerEmailField;
        [SerializeField] private TMP_InputField registerPasswordField;
        [SerializeField] private Button registerButton;
        [SerializeField] private Button backToLoginFromRegisterButton;

        [Header("Forgot Password Panel")]
        [SerializeField] private TMP_InputField forgotPasswordField;
        [SerializeField] private Button resetPasswordButton;
        [SerializeField] private Button backToLoginFromForgotButton;

        [Header("Welcome Panel")]
        [SerializeField] private TextMeshProUGUI welcomeText;
        [SerializeField] private Button continueToRealmsButton;

        [Header("UI Feedback")]
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private GameObject loadingIndicator;

        [Header("Space Theme")]
        [SerializeField] private ParticleSystem starField;
        [SerializeField] private AudioSource backgroundMusic;

        private AuthResponse currentUser;
        private ApiClient apiClient;

        private void Start()
        {
            apiClient = ApiClient.Instance;
            if (apiClient == null)
            {
                Debug.LogError("ApiClient instance not found! Make sure ApiClient is in the scene.");
                return;
            }

            SetupUI();
            ShowLoginPanel();
            
            // Start space theme effects
            if (starField != null) starField.Play();
            if (backgroundMusic != null) backgroundMusic.Play();
        }

        private void SetupUI()
        {
            // Login panel buttons
            loginButton.onClick.AddListener(OnLoginClicked);
            showRegisterButton.onClick.AddListener(ShowRegisterPanel);
            showForgotPasswordButton.onClick.AddListener(ShowForgotPasswordPanel);

            // Register panel buttons
            registerButton.onClick.AddListener(OnRegisterClicked);
            backToLoginFromRegisterButton.onClick.AddListener(ShowLoginPanel);

            // Forgot password panel buttons
            resetPasswordButton.onClick.AddListener(OnForgotPasswordClicked);
            backToLoginFromForgotButton.onClick.AddListener(ShowLoginPanel);

            // Welcome panel buttons
            continueToRealmsButton.onClick.AddListener(OnContinueToRealms);

            // Input field validation
            loginUsernameField.onValueChanged.AddListener(ValidateLoginFields);
            loginPasswordField.onValueChanged.AddListener(ValidateLoginFields);
            registerUsernameField.onValueChanged.AddListener(ValidateRegisterFields);
            registerEmailField.onValueChanged.AddListener(ValidateRegisterFields);
            registerPasswordField.onValueChanged.AddListener(ValidateRegisterFields);

            // Enter key support
            loginPasswordField.onSubmit.AddListener((_) => OnLoginClicked());
            registerPasswordField.onSubmit.AddListener((_) => OnRegisterClicked());
            forgotPasswordField.onSubmit.AddListener((_) => OnForgotPasswordClicked());
        }

        private void ShowLoginPanel()
        {
            loginPanel.SetActive(true);
            registerPanel.SetActive(false);
            forgotPasswordPanel.SetActive(false);
            welcomePanel.SetActive(false);
            ClearStatus();
            ValidateLoginFields("");
        }

        private void ShowRegisterPanel()
        {
            loginPanel.SetActive(false);
            registerPanel.SetActive(true);
            forgotPasswordPanel.SetActive(false);
            welcomePanel.SetActive(false);
            ClearStatus();
            ValidateRegisterFields("");
        }

        private void ShowForgotPasswordPanel()
        {
            loginPanel.SetActive(false);
            registerPanel.SetActive(false);
            forgotPasswordPanel.SetActive(true);
            welcomePanel.SetActive(false);
            ClearStatus();
        }

        private void ShowWelcomePanel(AuthResponse user)
        {
            loginPanel.SetActive(false);
            registerPanel.SetActive(false);
            forgotPasswordPanel.SetActive(false);
            welcomePanel.SetActive(true);
            
            welcomeText.text = $"Hello, {user.username}!";
            currentUser = user;
            ClearStatus();
        }

        private void ValidateLoginFields(string _)
        {
            bool isValid = !string.IsNullOrEmpty(loginUsernameField.text) && 
                          !string.IsNullOrEmpty(loginPasswordField.text);
            loginButton.interactable = isValid;
        }

        private void ValidateRegisterFields(string _)
        {
            bool isValid = !string.IsNullOrEmpty(registerUsernameField.text) && 
                          registerUsernameField.text.Length >= 3 &&
                          !string.IsNullOrEmpty(registerEmailField.text) && 
                          registerEmailField.text.Contains("@") &&
                          !string.IsNullOrEmpty(registerPasswordField.text) && 
                          registerPasswordField.text.Length >= 6;
            registerButton.interactable = isValid;
        }

        private void OnLoginClicked()
        {
            if (string.IsNullOrEmpty(loginUsernameField.text) || string.IsNullOrEmpty(loginPasswordField.text))
            {
                ShowStatus("Please fill in all fields.", true);
                return;
            }

            SetLoading(true);
            var loginRequest = new LoginRequest
            {
                username = loginUsernameField.text.Trim(),
                password = loginPasswordField.text
            };

            apiClient.Login(loginRequest, OnLoginSuccess, OnLoginError);
        }

        private void OnRegisterClicked()
        {
            if (string.IsNullOrEmpty(registerUsernameField.text) || 
                string.IsNullOrEmpty(registerEmailField.text) || 
                string.IsNullOrEmpty(registerPasswordField.text))
            {
                ShowStatus("Please fill in all fields.", true);
                return;
            }

            if (registerUsernameField.text.Length < 3)
            {
                ShowStatus("Username must be at least 3 characters long.", true);
                return;
            }

            if (!registerEmailField.text.Contains("@"))
            {
                ShowStatus("Please enter a valid email address.", true);
                return;
            }

            if (registerPasswordField.text.Length < 6)
            {
                ShowStatus("Password must be at least 6 characters long.", true);
                return;
            }

            SetLoading(true);
            var registerRequest = new RegisterRequest
            {
                username = registerUsernameField.text.Trim(),
                email = registerEmailField.text.Trim(),
                password = registerPasswordField.text
            };

            apiClient.Register(registerRequest, OnRegisterSuccess, OnRegisterError);
        }

        private void OnForgotPasswordClicked()
        {
            if (string.IsNullOrEmpty(forgotPasswordField.text))
            {
                ShowStatus("Please enter your username or email.", true);
                return;
            }

            SetLoading(true);
            var forgotRequest = new ForgotPasswordRequest
            {
                emailOrUsername = forgotPasswordField.text.Trim()
            };

            apiClient.ForgotPassword(forgotRequest, OnForgotPasswordSuccess, OnForgotPasswordError);
        }

        private void OnLoginSuccess(AuthResponse response)
        {
            SetLoading(false);
            ShowStatus($"Welcome back, {response.username}!", false);
            ShowWelcomePanel(response);
        }

        private void OnLoginError(string error)
        {
            SetLoading(false);
            ShowStatus($"Login failed: {error}", true);
        }

        private void OnRegisterSuccess(AuthResponse response)
        {
            SetLoading(false);
            ShowStatus($"Registration successful! Welcome, {response.username}!", false);
            ShowWelcomePanel(response);
        }

        private void OnRegisterError(string error)
        {
            SetLoading(false);
            ShowStatus($"Registration failed: {error}", true);
        }

        private void OnForgotPasswordSuccess(string message)
        {
            SetLoading(false);
            ShowStatus(message, false);
            // Clear the field and show login panel after a delay
            Invoke(nameof(ShowLoginPanel), 3f);
        }

        private void OnForgotPasswordError(string error)
        {
            SetLoading(false);
            ShowStatus($"Password reset failed: {error}", true);
        }

        private void OnContinueToRealms()
        {
            if (currentUser == null)
            {
                ShowStatus("No user data available.", true);
                return;
            }

            // Store user data for the realm selection scene
            PlayerPrefs.SetInt("UserId", currentUser.userId);
            PlayerPrefs.SetString("Username", currentUser.username);
            PlayerPrefs.SetString("UserEmail", currentUser.email);
            PlayerPrefs.SetString("UserToken", currentUser.token);
            PlayerPrefs.Save();

            // Load the realm selection scene
            SceneManager.LoadScene("RealmListScene");
        }

        private void ShowStatus(string message, bool isError)
        {
            if (statusText != null)
            {
                statusText.text = message;
                statusText.color = isError ? Color.red : Color.green;
            }
        }

        private void ClearStatus()
        {
            if (statusText != null)
            {
                statusText.text = "";
            }
        }

        private void SetLoading(bool loading)
        {
            if (loadingIndicator != null)
            {
                loadingIndicator.SetActive(loading);
            }

            // Disable all buttons during loading
            loginButton.interactable = !loading;
            registerButton.interactable = !loading;
            resetPasswordButton.interactable = !loading;
            showRegisterButton.interactable = !loading;
            showForgotPasswordButton.interactable = !loading;
            backToLoginFromRegisterButton.interactable = !loading;
            backToLoginFromForgotButton.interactable = !loading;
        }

        // Username validation for inappropriate content (placeholder for future implementation)
        private bool IsValidUsername(string username)
        {
            if (string.IsNullOrEmpty(username) || username.Length < 3)
                return false;

            // TODO: Add inappropriate content filtering here
            // For now, just basic validation
            return true;
        }
    }
}