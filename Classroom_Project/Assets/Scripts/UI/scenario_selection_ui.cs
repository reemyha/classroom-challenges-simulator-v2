using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Displays available scenarios and allows user to select one.
/// Shows scenario details like difficulty, description, and student count.
/// </summary>
public class ScenarioSelectionUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Panel containing the scenario list")]
    public GameObject scenarioSelectionPanel;
    
    [Tooltip("Container where scenario buttons will be created")]
    public Transform scenarioListContainer;
    
    [Tooltip("Prefab for scenario button")]
    public GameObject scenarioButtonPrefab;
    
    [Tooltip("Text showing user info")]
    public TextMeshProUGUI userInfoText;
    
    [Tooltip("Panel showing selected scenario details")]
    public GameObject scenarioDetailsPanel;
    
    [Tooltip("Details text elements")]
    public TextMeshProUGUI scenarioNameText;
    public TextMeshProUGUI scenarioDescriptionText;
    public TextMeshProUGUI scenarioDifficultyText;
    public TextMeshProUGUI scenarioStudentCountText;
    
    [Tooltip("Start simulation button")]
    public Button startSimulationButton;
    
    [Tooltip("Logout button")]
    public Button logoutButton;

    [Header("Scene Settings")]
    [Tooltip("Name of the simulation scene to load")]
    public string simulationSceneName = "MainClassroom";

    // References
    private AuthenticationManager authManager;
    private ScenarioLoader scenarioLoader;
    private List<string> availableScenarios;
    private string selectedScenarioName;
    private ScenarioConfig selectedScenario;

    void Start()
    {
        // Get references
        authManager = AuthenticationManager.Instance;
        scenarioLoader = FindObjectOfType<ScenarioLoader>();

        if (authManager == null)
        {
            Debug.LogError("AuthenticationManager not found!");
            return;
        }

        if (scenarioLoader == null)
        {
            Debug.LogError("ScenarioLoader not found!");
            return;
        }

        // Set up button listeners
        if (startSimulationButton != null)
        {
            startSimulationButton.onClick.AddListener(OnStartSimulationClicked);
            startSimulationButton.interactable = false; // Disabled until scenario selected
        }

        if (logoutButton != null)
        {
            logoutButton.onClick.AddListener(OnLogoutClicked);
        }

        // Hide details panel initially
        if (scenarioDetailsPanel != null)
            scenarioDetailsPanel.SetActive(false);

        // Display user info
        UpdateUserInfo();

        // Load scenario list
        RefreshScenarioList();
    }

    /// <summary>
    /// Update the user information display
    /// </summary>
    void UpdateUserInfo()
    {
        if (userInfoText != null && authManager != null && authManager.currentUser != null)
        {
            UserModel user = authManager.currentUser;
            userInfoText.text = $"Welcome, {user.FullName}\nRole: {user.Role}\nSessions: {user.SessionCount}";
        }
    }

    /// <summary>
    /// Load and display all available scenarios
    /// </summary>
    public void RefreshScenarioList()
    {
        Debug.Log("Refreshing scenario list...");

        // Clear existing buttons
        foreach (Transform child in scenarioListContainer)
        {
            Destroy(child.gameObject);
        }

        // Get available scenarios
        availableScenarios = scenarioLoader.GetAvailableScenarios();

        if (availableScenarios.Count == 0)
        {
            Debug.LogWarning("No scenarios found!");
            CreateNoScenariosMessage();
            return;
        }

        // Create a button for each scenario
        foreach (string scenarioFileName in availableScenarios)
        {
            CreateScenarioButton(scenarioFileName);
        }

        Debug.Log($"Loaded {availableScenarios.Count} scenarios");
    }

    /// <summary>
    /// Create a button for a scenario
    /// </summary>
    void CreateScenarioButton(string scenarioFileName)
    {
        // Check if user has permission
        bool canAccess = authManager.CanAccessScenario(scenarioFileName);

        GameObject buttonObj;

        // Create button from prefab or create simple button
        if (scenarioButtonPrefab != null)
        {
            buttonObj = Instantiate(scenarioButtonPrefab, scenarioListContainer);
        }
        else
        {
            // Create simple button if no prefab provided
            buttonObj = new GameObject($"Button_{scenarioFileName}");
            buttonObj.transform.SetParent(scenarioListContainer);
            
            // Add components
            RectTransform rectTransform = buttonObj.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(300, 60);
            
            Image image = buttonObj.AddComponent<Image>();
            image.color = canAccess ? Color.white : Color.gray;
            
            Button button = buttonObj.AddComponent<Button>();
            
            // Add text
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform);
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = scenarioFileName;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.black;
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
        }

        // Get button component
        Button btn = buttonObj.GetComponent<Button>();
        
        // Get text component (might be child)
        TextMeshProUGUI btnText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
        if (btnText != null)
        {
            // Clean up filename for display
            string displayName = scenarioFileName.Replace(".json", "").Replace("scenario_", "").Replace("_", " ");
            btnText.text = displayName;
        }

        // Set interactable based on permissions
        btn.interactable = canAccess;

        // Add click listener
        string fileName = scenarioFileName; // Capture in closure
        btn.onClick.AddListener(() => OnScenarioButtonClicked(fileName));

        // Add tooltip if locked
        if (!canAccess)
        {
            if (btnText != null)
                btnText.text += " 🔒";
            Debug.Log($"Scenario {scenarioFileName} is locked for this user");
        }
    }

    /// <summary>
    /// Create message when no scenarios are found
    /// </summary>
    void CreateNoScenariosMessage()
    {
        GameObject textObj = new GameObject("NoScenariosMessage");
        textObj.transform.SetParent(scenarioListContainer);
        
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "No scenarios found.\nPlease add scenario files to StreamingAssets/Scenarios/";
        text.alignment = TextAlignmentOptions.Center;
        text.fontSize = 18;
        text.color = Color.red;
        
        RectTransform rect = text.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(400, 100);
    }

    /// <summary>
    /// Called when a scenario button is clicked
    /// </summary>
    void OnScenarioButtonClicked(string scenarioFileName)
    {
        Debug.Log($"Scenario selected: {scenarioFileName}");

        selectedScenarioName = scenarioFileName;

        // Load scenario data
        selectedScenario = scenarioLoader.LoadScenario(scenarioFileName);

        if (selectedScenario == null)
        {
            Debug.LogError($"Failed to load scenario: {scenarioFileName}");
            return;
        }

        // Show scenario details
        DisplayScenarioDetails(selectedScenario);

        // Enable start button
        if (startSimulationButton != null)
            startSimulationButton.interactable = true;
    }

    /// <summary>
    /// Display detailed information about selected scenario
    /// </summary>
    void DisplayScenarioDetails(ScenarioConfig scenario)
    {
        if (scenarioDetailsPanel != null)
            scenarioDetailsPanel.SetActive(true);

        if (scenarioNameText != null)
            scenarioNameText.text = scenario.scenarioName;

        if (scenarioDescriptionText != null)
            scenarioDescriptionText.text = scenario.description;

        if (scenarioDifficultyText != null)
        {
            scenarioDifficultyText.text = $"Difficulty: {scenario.difficulty}";
            
            // Color code difficulty
            switch (scenario.difficulty.ToLower())
            {
                case "easy":
                    scenarioDifficultyText.color = Color.green;
                    break;
                case "medium":
                    scenarioDifficultyText.color = Color.yellow;
                    break;
                case "hard":
                    scenarioDifficultyText.color = Color.red;
                    break;
            }
        }

        if (scenarioStudentCountText != null)
        {
            int studentCount = scenario.studentProfiles?.Count ?? 0;
            scenarioStudentCountText.text = $"Students: {studentCount}";
        }
    }

    /// <summary>
    /// Start the selected simulation
    /// </summary>
    void OnStartSimulationClicked()
    {
        if (selectedScenario == null)
        {
            Debug.LogError("No scenario selected!");
            return;
        }

        Debug.Log($"Starting simulation: {selectedScenario.scenarioName}");

        // Store selected scenario globally so MainClassroom scene can access it
        PlayerPrefs.SetString("SelectedScenario", selectedScenarioName);
        PlayerPrefs.Save();

        // Load simulation scene
        SceneManager.LoadScene(simulationSceneName);
    }

    /// <summary>
    /// Logout and return to login screen
    /// </summary>
    void OnLogoutClicked()
    {
        Debug.Log("Logging out...");

        // Clear authentication
        if (authManager != null)
            authManager.Logout();

        // Reload the login scene (assuming login is in same scene, just different panels)
        // If login is in different scene, use: SceneManager.LoadScene("LoginScene");
        
        if (scenarioSelectionPanel != null)
            scenarioSelectionPanel.SetActive(false);

        // Find and show login panel
        LoginUI loginUI = FindObjectOfType<LoginUI>();
        if (loginUI != null && loginUI.loginPanel != null)
        {
            loginUI.loginPanel.SetActive(true);
        }
    }
}