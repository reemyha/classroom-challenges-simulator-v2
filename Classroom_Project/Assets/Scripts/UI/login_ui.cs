using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

/// <summary>
/// Controls the login screen UI.
/// Handles user input, displays errors, and transitions to scenario selection.
/// </summary>
public class LoginUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Main login panel")]
    public GameObject loginPanel;
    
    [Tooltip("Input field for username")]
    public TMP_InputField usernameInput;
    
    [Tooltip("Input field for password")]
    public TMP_InputField passwordInput;
    
    [Tooltip("Login button")]
    public Button loginButton;
    
    [Tooltip("Text to show errors/messages")]
    public TextMeshProUGUI messageText;
    
    [Tooltip("Loading indicator (optional)")]
    public GameObject loadingIndicator;

    [Header("Scene References")]
    [Tooltip("The scenario selection screen to show after login")]
    public GameObject scenarioSelectionScreen;

    [Header("Visual Settings")]
    public Color errorColor = Color.red;
    public Color successColor = Color.green;
    public Color normalColor = Color.white;

    // Reference to auth manager
    private AuthenticationManager authManager;
    private bool isProcessingLogin = false;

    void Start()
    {
        // Find the authentication manager
        authManager = AuthenticationManager.Instance;
        
        if (authManager == null)
        {
            Debug.LogError("AuthenticationManager not found! Make sure it exists in the scene.");
            return;
        }

        // Set up button listener
        if (loginButton != null)
        {
            loginButton.onClick.AddListener(OnLoginButtonClicked);
        }

        // Allow Enter key to submit
        if (passwordInput != null)
        {
            passwordInput.onSubmit.AddListener(delegate { OnLoginButtonClicked(); });
        }

        // Hide loading indicator initially
        if (loadingIndicator != null)
            loadingIndicator.SetActive(false);

        // Hide scenario selection initially
        if (scenarioSelectionScreen != null)
            scenarioSelectionScreen.SetActive(false);

        // Show login panel
        if (loginPanel != null)
            loginPanel.SetActive(true);

        // Clear message
        ShowMessage("", normalColor);

        Debug.Log("Login screen initialized. Default credentials: admin / admin123");
    }

    /// <summary>
    /// Called when login button is clicked
    /// </summary>
    public void OnLoginButtonClicked()
    {
        if (isProcessingLogin)
        {
            Debug.Log("Login already in progress...");
            return;
        }

        // Get input values
        string username = usernameInput.text.Trim();
        string password = passwordInput.text;

        // Validate input
        if (string.IsNullOrEmpty(username))
        {
            ShowMessage("Please enter a username", errorColor);
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            ShowMessage("Please enter a password", errorColor);
            return;
        }

        // Start login process
        ProcessLogin(username, password);
    }

    /// <summary>
    /// Process the login asynchronously
    /// </summary>
    async void ProcessLogin(string username, string password)
    {
        isProcessingLogin = true;

        // Show loading state
        ShowMessage("Logging in...", normalColor);
        if (loadingIndicator != null)
            loadingIndicator.SetActive(true);
        if (loginButton != null)
            loginButton.interactable = false;

        try
        {
            // Call authentication manager
            LoginResponse response = await authManager.LoginAsync(username, password);

            // Handle response
            if (response.Success)
            {
                OnLoginSuccess(response);
            }
            else
            {
                OnLoginFailure(response.Message);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Login exception: {e.Message}");
            OnLoginFailure("An error occurred. Please try again.");
        }
        finally
        {
            isProcessingLogin = false;
            
            // Hide loading state
            if (loadingIndicator != null)
                loadingIndicator.SetActive(false);
            if (loginButton != null)
                loginButton.interactable = true;
        }
    }

    /// <summary>
    /// Handle successful login
    /// </summary>
    void OnLoginSuccess(LoginResponse response)
    {
        Debug.Log($"Login successful! Welcome {response.User.FullName}");
        
        ShowMessage($"Welcome, {response.User.FullName}!", successColor);

        // Wait a moment to show success message, then transition
        Invoke(nameof(TransitionToScenarioSelection), 1f);
    }

    /// <summary>
    /// Handle failed login
    /// </summary>
    void OnLoginFailure(string message)
    {
        Debug.Log($"Login failed: {message}");
        ShowMessage(message, errorColor);

        // Clear password field for security
        if (passwordInput != null)
            passwordInput.text = "";

        // Shake effect (optional visual feedback)
        ShakeLoginPanel();
    }

    /// <summary>
    /// Transition from login to scenario selection
    /// </summary>
    void TransitionToScenarioSelection()
    {
        // Hide login panel
        if (loginPanel != null)
            loginPanel.SetActive(false);

        // Show scenario selection
        if (scenarioSelectionScreen != null)
        {
            scenarioSelectionScreen.SetActive(true);
            
            // Tell scenario selection to refresh
            ScenarioSelectionUI scenarioUI = scenarioSelectionScreen.GetComponent<ScenarioSelectionUI>();
            if (scenarioUI != null)
            {
                scenarioUI.RefreshScenarioList();
            }
        }
    }

    /// <summary>
    /// Display a message to the user
    /// </summary>
    void ShowMessage(string message, Color color)
    {
        if (messageText != null)
        {
            messageText.text = message;
            messageText.color = color;
        }
    }

    /// <summary>
    /// Simple shake animation for error feedback
    /// </summary>
    void ShakeLoginPanel()
    {
        if (loginPanel != null)
        {
            // Simple shake implementation
            StartCoroutine(ShakeCoroutine(loginPanel.transform));
        }
    }

    System.Collections.IEnumerator ShakeCoroutine(Transform target)
    {
        Vector3 originalPos = target.localPosition;
        float shakeDuration = 0.5f;
        float shakeAmount = 10f;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = originalPos.x + Random.Range(-shakeAmount, shakeAmount);
            target.localPosition = new Vector3(x, originalPos.y, originalPos.z);
            
            elapsed += Time.deltaTime;
            yield return null;
        }

        target.localPosition = originalPos;
    }

    /// <summary>
    /// Optional: Register new user button (for testing/development)
    /// </summary>
    public void OnRegisterButtonClicked()
    {
        // This could open a registration form
        Debug.Log("Register button clicked - implement registration form if needed");
        ShowMessage("Registration not yet implemented", normalColor);
    }

    /// <summary>
    /// Optional: Forgot password button
    /// </summary>
    public void OnForgotPasswordClicked()
    {
        Debug.Log("Forgot password - implement password reset if needed");
        ShowMessage("Please contact administrator", normalColor);
    }
}