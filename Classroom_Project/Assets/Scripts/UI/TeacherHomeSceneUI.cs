using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

/// <summary>
/// Main UI for Teacher Home Scene with dashboard, session management, and feedback panels.
/// </summary>
public class TeacherHomeSceneUI : MonoBehaviour
{
    [Header("Main Dashboard Panel")]
    [Tooltip("Main panel containing the dashboard")]
    public GameObject dashboardPanel;

    [Header("Navigation Buttons")]
    [Tooltip("Button to start a new session")]
    public Button startSessionButton;

    [Tooltip("Button to view supervisor feedback")]
    public Button supervisorFeedbackButton;

    [Tooltip("Button to view session history")]
    public Button sessionHistoryButton;

    [Header("Supervisor Feedback Panel")]
    [Tooltip("Panel displaying supervisor feedback")]
    public GameObject supervisorFeedbackPanel;

    [Tooltip("Text displaying supervisor feedback content")]
    public TextMeshProUGUI supervisorFeedbackText;

    [Tooltip("Button to close supervisor feedback panel")]
    public Button closeSupervisorFeedbackButton;

    [Header("Session History Panel")]
    [Tooltip("Panel displaying session history")]
    public GameObject sessionHistoryPanel;

    [Tooltip("Container for session history items (should be Content of ScrollRect)")]
    public Transform sessionHistoryContainer;

    [Tooltip("Prefab for session history item (optional)")]
    public GameObject sessionHistoryItemPrefab;

    [Tooltip("Button to close session history panel")]
    public Button closeSessionHistoryButton;

    [Tooltip("Text displayed when no session history exists")]
    public TextMeshProUGUI noHistoryText;

    [Header("Scenario Selection")]
    [Tooltip("Reference to ScenarioSelectionUI component (should be on a GameObject in the scene)")]
    public ScenarioSelectionUI scenarioSelectionUI;

    [Tooltip("Panel containing the scenario selection UI (should be assigned to ScenarioSelectionUI)")]
    public GameObject scenarioSelectionPanel;

    [Header("Dashboard Info")]
    [Tooltip("Text displaying user information")]
    public TextMeshProUGUI userInfoText;

    [Tooltip("Text displaying total sessions completed")]
    public TextMeshProUGUI totalSessionsText;

    private AuthenticationManager authManager;

    void Awake()
    {
        // Hide scenario selection panel initially (before ScenarioSelectionUI initializes)
        if (scenarioSelectionPanel != null)
            scenarioSelectionPanel.SetActive(false);
    }

    void Start()
    {
        // Get references
        authManager = AuthenticationManager.Instance;

        // Find ScenarioSelectionUI if not assigned
        if (scenarioSelectionUI == null)
        {
            scenarioSelectionUI = FindObjectOfType<ScenarioSelectionUI>();
            if (scenarioSelectionUI == null)
            {
                Debug.LogWarning("ScenarioSelectionUI not found in scene. Please add ScenarioSelectionUI component to a GameObject in the scene.");
            }
        }

        // Setup button listeners
        if (startSessionButton != null)
            startSessionButton.onClick.AddListener(ShowScenarioSelection);

        if (supervisorFeedbackButton != null)
            supervisorFeedbackButton.onClick.AddListener(ShowSupervisorFeedback);

        if (sessionHistoryButton != null)
            sessionHistoryButton.onClick.AddListener(ShowSessionHistory);

        if (closeSupervisorFeedbackButton != null)
            closeSupervisorFeedbackButton.onClick.AddListener(CloseSupervisorFeedbackPanel);

        if (closeSessionHistoryButton != null)
            closeSessionHistoryButton.onClick.AddListener(CloseSessionHistoryPanel);

        // Initialize panels
        if (dashboardPanel != null)
            dashboardPanel.SetActive(true);

        // Ensure scenario selection panel is disabled at start
        if (scenarioSelectionPanel != null)
            scenarioSelectionPanel.SetActive(false);

        if (supervisorFeedbackPanel != null)
            supervisorFeedbackPanel.SetActive(false);

        if (sessionHistoryPanel != null)
            sessionHistoryPanel.SetActive(false);

        // Update dashboard info
        UpdateDashboardInfo();

        // Override logout button behavior in ScenarioSelectionUI to go back to dashboard instead
        // We need to do this after ScenarioSelectionUI initializes, so use a coroutine
        StartCoroutine(OverrideScenarioSelectionLogoutButton());
    }

    /// <summary>
    /// Override the logout button in ScenarioSelectionUI to go back to dashboard
    /// </summary>
    System.Collections.IEnumerator OverrideScenarioSelectionLogoutButton()
    {
        // Wait a frame to ensure ScenarioSelectionUI has initialized
        yield return null;

        if (scenarioSelectionUI != null && scenarioSelectionUI.logoutButton != null)
        {
            // Remove existing listeners and add our own to go back to dashboard
            scenarioSelectionUI.logoutButton.onClick.RemoveAllListeners();
            scenarioSelectionUI.logoutButton.onClick.AddListener(CloseScenarioSelectionPanel);
        }
    }

    /// <summary>
    /// Show scenario selection panel using ScenarioSelectionUI
    /// </summary>
    void ShowScenarioSelection()
    {
        Debug.Log("ShowScenarioSelection called");

        if (scenarioSelectionUI == null)
        {
            Debug.LogError("ScenarioSelectionUI is not assigned! Please assign it in the Inspector.");
            return;
        }

        // Ensure ScenarioLoader exists in the scene
        ScenarioLoader loader = FindObjectOfType<ScenarioLoader>();
        if (loader == null)
        {
            Debug.LogWarning("ScenarioLoader not found in scene. Creating one automatically...");
            GameObject loaderObj = new GameObject("ScenarioLoader");
            loader = loaderObj.AddComponent<ScenarioLoader>();
            Debug.Log("ScenarioLoader created successfully.");
        }
        else
        {
            Debug.Log("ScenarioLoader found in scene.");
        }

        // Hide dashboard first
        if (dashboardPanel != null)
        {
            dashboardPanel.SetActive(false);
            Debug.Log("Dashboard panel hidden");
        }

        // Show the scenario selection panel
        if (scenarioSelectionPanel != null)
        {
            scenarioSelectionPanel.SetActive(true);
            Debug.Log("Scenario selection panel shown");
        }
        else
        {
            Debug.LogError("scenarioSelectionPanel is null! Please assign it in the Inspector.");
        }

        // Ensure ScenarioSelectionUI has all its references initialized
        // Refresh the scenario list - this will use the ScenarioLoader
        if (scenarioSelectionUI != null)
        {
            Debug.Log("Refreshing scenario list...");
            scenarioSelectionUI.RefreshScenarioList();
        }
    }

    /// <summary>
    /// Close scenario selection panel and return to dashboard
    /// </summary>
    void CloseScenarioSelectionPanel()
    {
        // Hide scenario selection panel
        if (scenarioSelectionPanel != null)
            scenarioSelectionPanel.SetActive(false);

        // Show dashboard again
        if (dashboardPanel != null)
            dashboardPanel.SetActive(true);
    }

    /// <summary>
    /// Display supervisor feedback panel
    /// </summary>
    void ShowSupervisorFeedback()
    {
        if (supervisorFeedbackPanel != null)
        {
            supervisorFeedbackPanel.SetActive(true);
            
            // Load and display supervisor feedback
            LoadSupervisorFeedback();
        }

        // Hide dashboard while showing feedback
        if (dashboardPanel != null)
            dashboardPanel.SetActive(false);
    }

    /// <summary>
    /// Close supervisor feedback panel and return to dashboard
    /// </summary>
    void CloseSupervisorFeedbackPanel()
    {
        if (supervisorFeedbackPanel != null)
            supervisorFeedbackPanel.SetActive(false);

        if (dashboardPanel != null)
            dashboardPanel.SetActive(true);
    }

    /// <summary>
    /// Display session history panel
    /// </summary>
    void ShowSessionHistory()
    {
        if (sessionHistoryPanel != null)
        {
            sessionHistoryPanel.SetActive(true);
            LoadSessionHistory();
        }

        // Hide dashboard while showing history
        if (dashboardPanel != null)
            dashboardPanel.SetActive(false);
    }

    /// <summary>
    /// Close session history panel and return to dashboard
    /// </summary>
    void CloseSessionHistoryPanel()
    {
        if (sessionHistoryPanel != null)
            sessionHistoryPanel.SetActive(false);

        if (dashboardPanel != null)
            dashboardPanel.SetActive(true);
    }

    /// <summary>
    /// Load and display supervisor feedback
    /// </summary>
    void LoadSupervisorFeedback()
    {
        if (supervisorFeedbackText == null)
            return;

        // Load supervisor feedback from storage
        // For now, using placeholder data - can be replaced with actual database calls
        string feedback = PlayerPrefs.GetString("SupervisorFeedback", "");
        
        if (string.IsNullOrEmpty(feedback))
        {
            feedback = "No supervisor feedback available yet.\n\n" +
                       "Your supervisor will provide feedback here after reviewing your teaching sessions.";
        }

        supervisorFeedbackText.text = feedback;
    }

    /// <summary>
    /// Load and display session history with feedback
    /// </summary>
    void LoadSessionHistory()
    {
        if (sessionHistoryContainer == null)
            return;

        // Clear existing items
        foreach (Transform child in sessionHistoryContainer)
        {
            Destroy(child.gameObject);
        }

        // Load session history
        List<SessionHistoryEntry> history = LoadSessionHistoryData();

        if (history == null || history.Count == 0)
        {
            if (noHistoryText != null)
                noHistoryText.gameObject.SetActive(true);
            return;
        }

        if (noHistoryText != null)
            noHistoryText.gameObject.SetActive(false);

        // Display each session in history
        for (int i = history.Count - 1; i >= 0; i--) // Show most recent first
        {
            CreateSessionHistoryItem(history[i]);
        }

        // Force layout update
        Canvas.ForceUpdateCanvases();
    }

    /// <summary>
    /// Create a UI item for a session history entry
    /// </summary>
    void CreateSessionHistoryItem(SessionHistoryEntry entry)
    {
        GameObject itemObj;

        if (sessionHistoryItemPrefab != null)
        {
            itemObj = Instantiate(sessionHistoryItemPrefab, sessionHistoryContainer);
        }
        else
        {
            // Create a simple UI item programmatically
            itemObj = new GameObject($"SessionHistoryItem_{entry.sessionId}");
            itemObj.transform.SetParent(sessionHistoryContainer, false);

            var rectTransform = itemObj.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(0, 120);

            var image = itemObj.AddComponent<Image>();
            image.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

            // Add layout element
            var layoutElement = itemObj.AddComponent<UnityEngine.UI.LayoutElement>();
            layoutElement.minHeight = 120;
            layoutElement.preferredHeight = 120;

            // Create text for session info
            var textObj = new GameObject("SessionInfo");
            textObj.transform.SetParent(itemObj.transform, false);
            var textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0, 0);
            textRect.anchorMax = new Vector2(1, 1);
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;
            textRect.offsetMin = new Vector2(10, 10);
            textRect.offsetMax = new Vector2(-10, -10);

            var text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = FormatSessionEntry(entry);
            text.fontSize = 14;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.TopLeft;
        }
    }

    /// <summary>
    /// Format session entry for display
    /// </summary>
    string FormatSessionEntry(SessionHistoryEntry entry)
    {
        string dateStr = entry.date.ToString("MM/dd/yyyy HH:mm");
        string durationStr = FormatDuration(entry.duration);
        
        return $"Session: {entry.sessionId}\n" +
               $"Date: {dateStr}\n" +
               $"Duration: {durationStr}\n" +
               $"Score: {entry.score:F1}/100\n" +
               $"Feedback: {entry.feedback}";
    }

    /// <summary>
    /// Format duration in seconds to readable format
    /// </summary>
    string FormatDuration(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60);
        int secs = Mathf.FloorToInt(seconds % 60);
        return $"{minutes}m {secs}s";
    }

    /// <summary>
    /// Load session history data from storage
    /// </summary>
    List<SessionHistoryEntry> LoadSessionHistoryData()
    {
        List<SessionHistoryEntry> history = new List<SessionHistoryEntry>();

        // Load from PlayerPrefs (can be replaced with database calls)
        int count = PlayerPrefs.GetInt("SessionHistoryCount", 0);

        for (int i = 0; i < count; i++)
        {
            string key = $"SessionHistory_{i}";
            string data = PlayerPrefs.GetString(key, "");

            if (!string.IsNullOrEmpty(data))
            {
                try
                {
                    SessionHistoryEntry entry = JsonUtility.FromJson<SessionHistoryEntry>(data);
                    if (entry != null)
                        history.Add(entry);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Failed to parse session history entry: {e.Message}");
                }
            }
        }

        return history;
    }

    /// <summary>
    /// Update dashboard information
    /// </summary>
    void UpdateDashboardInfo()
    {
        // Update user info
        if (userInfoText != null && authManager != null && authManager.currentUser != null)
        {
            var user = authManager.currentUser;
            userInfoText.text = $"Welcome, {user.fullName}\nRole: {user.role}";
        }

        // Update total sessions
        if (totalSessionsText != null)
        {
            int totalSessions = PlayerPrefs.GetInt("SessionHistoryCount", 0);
            totalSessionsText.text = $"Total Sessions: {totalSessions}";
        }
    }


    /// <summary>
    /// Called when a session ends to save session history
    /// This can be called from ClassroomManager when a session ends
    /// </summary>
    public static void SaveSessionToHistory(SessionReport report, string feedback = "")
    {
        if (report == null || report.sessionData == null)
            return;

        // Load existing history
        int count = PlayerPrefs.GetInt("SessionHistoryCount", 0);
        
        // Create history entry
        SessionHistoryEntry entry = new SessionHistoryEntry
        {
            sessionId = report.sessionData.sessionId,
            date = report.sessionData.endTime,
            duration = report.sessionData.duration,
            score = report.score,
            engagement = report.averageEngagement,
            totalActions = report.totalActions,
            disruptions = report.totalDisruptions,
            feedback = string.IsNullOrEmpty(feedback) ? "No feedback provided." : feedback
        };

        // Save to PlayerPrefs
        string key = $"SessionHistory_{count}";
        string json = JsonUtility.ToJson(entry);
        PlayerPrefs.SetString(key, json);
        
        // Update count
        count++;
        PlayerPrefs.SetInt("SessionHistoryCount", count);
        PlayerPrefs.Save();
    }
}

/// <summary>
/// Data structure for session history entries
/// </summary>
[System.Serializable]
public class SessionHistoryEntry
{
    public string sessionId;
    public string dateString; // Stored as ISO string for JSON serialization
    public float duration;
    public float score;
    public float engagement;
    public int totalActions;
    public int disruptions;
    public string feedback;

    // Property to get/set DateTime
    public DateTime date
    {
        get
        {
            if (string.IsNullOrEmpty(dateString))
                return DateTime.Now;
            try
            {
                return DateTime.Parse(dateString);
            }
            catch
            {
                return DateTime.Now;
            }
        }
        set
        {
            dateString = value.ToString("O"); // ISO 8601 format
        }
    }
}
