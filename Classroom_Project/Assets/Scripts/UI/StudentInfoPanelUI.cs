using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// UI panel that displays detailed information about a selected student.
/// Shows emotion vectors, behavioral traits, and current state.
/// </summary>
public class StudentInfoPanelUI : MonoBehaviour
{
    [Header("Panel References")]
    [Tooltip("The main panel GameObject")]
    public GameObject panel;

    [Header("Student Info Display")]
    [Tooltip("Text displaying student name")]
    public TextMeshProUGUI studentNameText;

    [Tooltip("Text displaying student ID")]
    public TextMeshProUGUI studentIdText;

    [Tooltip("Text displaying current behavioral state")]
    public TextMeshProUGUI stateText;

    [Header("Emotion Vectors Display")]
    [Tooltip("Text displaying happiness value")]
    public TextMeshProUGUI happinessText;

    [Tooltip("Text displaying sadness value")]
    public TextMeshProUGUI sadnessText;

    [Tooltip("Text displaying frustration value")]
    public TextMeshProUGUI frustrationText;

    [Tooltip("Text displaying boredom value")]
    public TextMeshProUGUI boredomText;

    [Tooltip("Text displaying anger value")]
    public TextMeshProUGUI angerText;

    [Header("Emotion Sliders (Optional)")]
    [Tooltip("Slider for happiness visualization")]
    public Slider happinessSlider;

    [Tooltip("Slider for sadness visualization")]
    public Slider sadnessSlider;

    [Tooltip("Slider for frustration visualization")]
    public Slider frustrationSlider;

    [Tooltip("Slider for boredom visualization")]
    public Slider boredomSlider;

    [Tooltip("Slider for anger visualization")]
    public Slider angerSlider;

    [Header("Behavioral Traits Display")]
    [Tooltip("Text displaying extroversion")]
    public TextMeshProUGUI extroversionText;

    [Tooltip("Text displaying sensitivity")]
    public TextMeshProUGUI sensitivityText;

    [Tooltip("Text displaying rebelliousness")]
    public TextMeshProUGUI rebelliousnessText;

    [Tooltip("Text displaying academic motivation")]
    public TextMeshProUGUI academicMotivationText;

    [Header("Close Button")]
    [Tooltip("Button to close the panel")]
    public Button closeButton;

    private StudentAgent currentStudent;
    private bool isUpdating = false;

    void Start()
    {
        // Hide panel initially
        if (panel != null)
            panel.SetActive(false);

        // Setup close button
        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePanel);
    }

    void Update()
    {
        // Update display if student is selected
        if (currentStudent != null && isUpdating)
        {
            UpdateDisplay();
        }
    }

    /// <summary>
    /// Show the panel and display information for the selected student
    /// </summary>
    public void ShowStudentInfo(StudentAgent student)
    {
        if (student == null)
        {
            Debug.LogWarning("StudentInfoPanelUI: Cannot show info for null student");
            return;
        }

        currentStudent = student;
        isUpdating = true;

        if (panel != null)
            panel.SetActive(true);

        UpdateDisplay();
    }

    /// <summary>
    /// Update all display elements with current student data
    /// </summary>
    void UpdateDisplay()
    {
        if (currentStudent == null) return;

        // Update basic info
        if (studentNameText != null)
            studentNameText.text = $"Name: {currentStudent.studentName}";

        if (studentIdText != null)
            studentIdText.text = $"ID: {currentStudent.studentId}";

        if (stateText != null)
        {
            stateText.text = $"State: {currentStudent.currentState}";
            stateText.color = GetStateColor(currentStudent.currentState);
        }

        // Update emotion vectors
        EmotionVector emotions = currentStudent.emotions;

        if (happinessText != null)
            happinessText.text = $"Happiness: {emotions.Happiness:F1}/10";

        if (sadnessText != null)
            sadnessText.text = $"Sadness: {emotions.Sadness:F1}/10";

        if (frustrationText != null)
            frustrationText.text = $"Frustration: {emotions.Frustration:F1}/10";

        if (boredomText != null)
            boredomText.text = $"Boredom: {emotions.Boredom:F1}/10";

        if (angerText != null)
            angerText.text = $"Anger: {emotions.Anger:F1}/10";

        // Update sliders if available
        if (happinessSlider != null)
            happinessSlider.value = emotions.Happiness / 10f;

        if (sadnessSlider != null)
            sadnessSlider.value = emotions.Sadness / 10f;

        if (frustrationSlider != null)
            frustrationSlider.value = emotions.Frustration / 10f;

        if (boredomSlider != null)
            boredomSlider.value = emotions.Boredom / 10f;

        if (angerSlider != null)
            angerSlider.value = emotions.Anger / 10f;

        // Update behavioral traits
        if (extroversionText != null)
            extroversionText.text = $"Extroversion: {currentStudent.extroversion:F2}";

        if (sensitivityText != null)
            sensitivityText.text = $"Sensitivity: {currentStudent.sensitivity:F2}";

        if (rebelliousnessText != null)
            rebelliousnessText.text = $"Rebelliousness: {currentStudent.rebelliousness:F2}";

        if (academicMotivationText != null)
            academicMotivationText.text = $"Academic Motivation: {currentStudent.academicMotivation:F2}";
    }

    /// <summary>
    /// Get color for student state
    /// </summary>
    Color GetStateColor(StudentState state)
    {
        switch (state)
        {
            case StudentState.Listening:
                return Color.green;
            case StudentState.Engaged:
                return Color.cyan;
            case StudentState.Distracted:
                return Color.yellow;
            case StudentState.SideTalk:
                return new Color(1f, 0.7f, 0.3f); // Orange
            case StudentState.Arguing:
                return Color.red;
            case StudentState.Withdrawn:
                return Color.gray;
            default:
                return Color.white;
        }
    }

    /// <summary>
    /// Close the panel
    /// </summary>
    public void ClosePanel()
    {
        if (panel != null)
            panel.SetActive(false);

        currentStudent = null;
        isUpdating = false;
    }

    /// <summary>
    /// Get the currently selected student
    /// </summary>
    public StudentAgent GetCurrentStudent()
    {
        return currentStudent;
    }

    /// <summary>
    /// Check if panel is currently showing a student
    /// </summary>
    public bool IsShowingStudent()
    {
        return currentStudent != null && panel != null && panel.activeSelf;
    }
}
