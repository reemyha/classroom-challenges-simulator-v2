using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// UI Panel that displays session feedback metrics when ending a lesson.
/// Shows: lesson time, satisfied students, interaction percentage, noise handling.
/// </summary>
public class SessionFeedbackPanelUI : MonoBehaviour
{
    [Header("Panel References")]
    public GameObject panelRoot;
    public Button closeButton;
    public Button returnToHomeButton;

    [Header("Metric Display")]
    public TextMeshProUGUI lessonTimeText;
    public TextMeshProUGUI satisfiedStudentsText;
    public TextMeshProUGUI interactionPercentageText;
    public TextMeshProUGUI noiseHandlingText;
    public TextMeshProUGUI overallScoreText;

    [Header("Visual Feedback")]
    public Image lessonTimeIcon;
    public Image satisfiedStudentsIcon;
    public Image interactionIcon;
    public Image noiseHandlingIcon;

    [Header("Colors")]
    public Color excellentColor = new Color(0.2f, 0.8f, 0.2f);
    public Color goodColor = new Color(0.8f, 0.8f, 0.2f);
    public Color needsImprovementColor = new Color(0.8f, 0.4f, 0.2f);
    public Color poorColor = new Color(0.8f, 0.2f, 0.2f);

    private SessionFeedbackData currentFeedback;

    void Start()
    {
        // Setup button listeners
        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePanel);

        if (returnToHomeButton != null)
            returnToHomeButton.onClick.AddListener(ReturnToHome);

        // Hide panel initially
        if (panelRoot != null)
            panelRoot.SetActive(false);
    }

    /// <summary>
    /// Show the feedback panel with session data
    /// </summary>
    public void ShowFeedback(SessionFeedbackData feedback)
    {
        if (panelRoot == null)
        {
            Debug.LogError("SessionFeedbackPanelUI: panelRoot is not assigned!");
            return;
        }

        currentFeedback = feedback;
        panelRoot.SetActive(true);

        // Update all display fields
        UpdateLessonTimeDisplay(feedback.lessonDurationSeconds);
        UpdateSatisfiedStudentsDisplay(feedback.satisfiedStudentsCount, feedback.totalStudentsCount);
        UpdateInteractionDisplay(feedback.studentInteractionPercentage);
        UpdateNoiseHandlingDisplay(feedback.noiseHandlingScore, feedback.totalDisruptions, feedback.disruptionsHandled);
        UpdateOverallScore(feedback.overallScore);

        Debug.Log($"[SessionFeedbackPanel] Displaying feedback - Time: {feedback.lessonDurationSeconds}s, " +
                  $"Satisfied: {feedback.satisfiedStudentsCount}/{feedback.totalStudentsCount}, " +
                  $"Interaction: {feedback.studentInteractionPercentage:P0}, " +
                  $"Noise Score: {feedback.noiseHandlingScore:F1}");
    }

    /// <summary>
    /// Update lesson time display
    /// </summary>
    void UpdateLessonTimeDisplay(float durationSeconds)
    {
        if (lessonTimeText == null) return;

        int minutes = Mathf.FloorToInt(durationSeconds / 60f);
        int seconds = Mathf.FloorToInt(durationSeconds % 60f);
        lessonTimeText.text = $"זמן שיעור: {minutes:00}:{seconds:00}";

        // Color based on duration (longer sessions show more engagement)
        if (lessonTimeIcon != null)
        {
            if (durationSeconds >= 600) // 10+ minutes
                lessonTimeIcon.color = excellentColor;
            else if (durationSeconds >= 300) // 5+ minutes
                lessonTimeIcon.color = goodColor;
            else if (durationSeconds >= 120) // 2+ minutes
                lessonTimeIcon.color = needsImprovementColor;
            else
                lessonTimeIcon.color = poorColor;
        }
    }

    /// <summary>
    /// Update satisfied students display
    /// </summary>
    void UpdateSatisfiedStudentsDisplay(int satisfied, int total)
    {
        if (satisfiedStudentsText == null) return;

        float percentage = total > 0 ? (float)satisfied / total : 0f;
        satisfiedStudentsText.text = $"תלמידים מרוצים: {satisfied}/{total} ({percentage:P0})";

        // Color based on satisfaction rate
        if (satisfiedStudentsIcon != null)
        {
            if (percentage >= 0.8f)
                satisfiedStudentsIcon.color = excellentColor;
            else if (percentage >= 0.6f)
                satisfiedStudentsIcon.color = goodColor;
            else if (percentage >= 0.4f)
                satisfiedStudentsIcon.color = needsImprovementColor;
            else
                satisfiedStudentsIcon.color = poorColor;
        }
    }

    /// <summary>
    /// Update interaction percentage display
    /// </summary>
    void UpdateInteractionDisplay(float interactionPercentage)
    {
        if (interactionPercentageText == null) return;

        interactionPercentageText.text = $"אינטראקציה עם תלמידים: {interactionPercentage:P0}";

        // Color based on interaction level
        if (interactionIcon != null)
        {
            if (interactionPercentage >= 0.7f)
                interactionIcon.color = excellentColor;
            else if (interactionPercentage >= 0.5f)
                interactionIcon.color = goodColor;
            else if (interactionPercentage >= 0.3f)
                interactionIcon.color = needsImprovementColor;
            else
                interactionIcon.color = poorColor;
        }
    }

    /// <summary>
    /// Update noise handling display
    /// </summary>
    void UpdateNoiseHandlingDisplay(float score, int totalDisruptions, int disruptionsHandled)
    {
        if (noiseHandlingText == null) return;

        string scoreDescription = GetNoiseHandlingDescription(score);
        noiseHandlingText.text = $"התמודדות עם רעש: {scoreDescription}\n" +
                                 $"הפרעות: {disruptionsHandled}/{totalDisruptions} טופלו";

        // Color based on handling score
        if (noiseHandlingIcon != null)
        {
            if (score >= 80f)
                noiseHandlingIcon.color = excellentColor;
            else if (score >= 60f)
                noiseHandlingIcon.color = goodColor;
            else if (score >= 40f)
                noiseHandlingIcon.color = needsImprovementColor;
            else
                noiseHandlingIcon.color = poorColor;
        }
    }

    /// <summary>
    /// Update overall score display
    /// </summary>
    void UpdateOverallScore(float score)
    {
        if (overallScoreText == null) return;

        string grade = GetGradeFromScore(score);
        overallScoreText.text = $"ציון כללי: {score:F0}/100\n{grade}";

        // Set color based on score
        if (score >= 80f)
            overallScoreText.color = excellentColor;
        else if (score >= 60f)
            overallScoreText.color = goodColor;
        else if (score >= 40f)
            overallScoreText.color = needsImprovementColor;
        else
            overallScoreText.color = poorColor;
    }

    /// <summary>
    /// Get description for noise handling score
    /// </summary>
    string GetNoiseHandlingDescription(float score)
    {
        if (score >= 80f) return "מצוין";
        if (score >= 60f) return "טוב";
        if (score >= 40f) return "סביר";
        if (score >= 20f) return "צריך שיפור";
        return "חלש";
    }

    /// <summary>
    /// Get grade text from score
    /// </summary>
    string GetGradeFromScore(float score)
    {
        if (score >= 90f) return "מצוין! ניהול כיתה מעולה";
        if (score >= 80f) return "טוב מאוד! עבודה יפה";
        if (score >= 70f) return "טוב - יש מקום לשיפור";
        if (score >= 60f) return "סביר - המשך לתרגל";
        if (score >= 50f) return "צריך שיפור";
        return "נסה שוב - למד מהטעויות";
    }

    /// <summary>
    /// Close the feedback panel
    /// </summary>
    public void ClosePanel()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);
    }

    /// <summary>
    /// Return to home screen
    /// </summary>
    void ReturnToHome()
    {
        ClosePanel();
        UnityEngine.SceneManagement.SceneManager.LoadScene("TeacherHomeScreen");
    }
}

/// <summary>
/// Data structure for session feedback metrics
/// </summary>
[Serializable]
public class SessionFeedbackData
{
    public string sessionId;
    public DateTime sessionDate;
    public float lessonDurationSeconds;
    public int satisfiedStudentsCount;
    public int totalStudentsCount;
    public float studentInteractionPercentage;
    public float noiseHandlingScore;
    public int totalDisruptions;
    public int disruptionsHandled;
    public float overallScore;

    // Additional tracking data
    public int totalTeacherActions;
    public int positiveActions;
    public int negativeActions;
    public float averageEngagement;

    /// <summary>
    /// Convert to JSON for database logging
    /// </summary>
    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
}
