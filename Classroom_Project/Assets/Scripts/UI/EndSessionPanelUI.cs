using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Enhanced End Session Panel UI with animations, better layout, and visual feedback.
/// Displays session summary in an attractive, organized manner with smooth transitions.
/// </summary>
public class EndSessionPanelUI : MonoBehaviour
{
    [Header("Panel References")]
    [Tooltip("Main panel container")]
    public GameObject panel;

    [Tooltip("Canvas Group for fade animations")]
    public CanvasGroup canvasGroup;

    [Tooltip("Panel background image")]
    public Image panelBackground;

    [Tooltip("Content container (for slide animation)")]
    public RectTransform contentContainer;

    [Header("Header Section")]
    [Tooltip("Title text (e.g., 'סיכום שיעור')")]
    public TextMeshProUGUI titleText;

    [Tooltip("Subtitle/grade text")]
    public TextMeshProUGUI subtitleText;

    [Tooltip("Header background/banner")]
    public Image headerBackground;

    [Header("Score Display")]
    [Tooltip("Large score number display")]
    public TextMeshProUGUI scoreText;

    [Tooltip("Score label (e.g., '/100')")]
    public TextMeshProUGUI scoreLabelText;

    [Tooltip("Circular or linear score progress bar")]
    public Slider scoreProgressBar;

    [Tooltip("Score progress bar fill image")]
    public Image scoreProgressFill;

    [Tooltip("Score container for animations")]
    public RectTransform scoreContainer;

    [Header("Statistics Section")]
    [Tooltip("Duration display (minutes:seconds)")]
    public TextMeshProUGUI durationText;

    [Tooltip("Positive actions count")]
    public TextMeshProUGUI positiveActionsText;

    [Tooltip("Negative actions count")]
    public TextMeshProUGUI negativeActionsText;

    [Tooltip("Total actions count")]
    public TextMeshProUGUI totalActionsText;

    [Tooltip("Engagement percentage")]
    public TextMeshProUGUI engagementText;

    [Tooltip("Engagement progress bar")]
    public Slider engagementProgressBar;

    [Tooltip("Disruptions count")]
    public TextMeshProUGUI disruptionsText;

    [Header("Achievement Badges")]
    [Tooltip("Container for achievement badges")]
    public Transform badgesContainer;

    [Tooltip("Badge prefab for instantiation")]
    public GameObject badgePrefab;

    [Tooltip("Pre-placed badge images (alternative to prefab)")]
    public Image[] badgeImages;

    [Tooltip("Badge tooltips")]
    public TextMeshProUGUI[] badgeTooltips;

    [Header("Action Buttons")]
    [Tooltip("Return to home button")]
    public Button returnHomeButton;

    [Tooltip("Retry/New session button")]
    public Button retryButton;

    [Tooltip("View detailed report button")]
    public Button detailsButton;

    [Header("Visual Effects")]
    [Tooltip("Particle system for celebration (high scores)")]
    public ParticleSystem celebrationParticles;

    [Tooltip("Star/sparkle effect for excellent performance")]
    public GameObject excellentEffect;

    [Header("Animation Settings")]
    [Tooltip("Duration for panel fade in")]
    public float fadeInDuration = 0.3f;

    [Tooltip("Duration for content slide up")]
    public float slideUpDuration = 0.4f;

    [Tooltip("Duration for score count-up animation")]
    public float scoreCountDuration = 1.5f;

    [Tooltip("Delay between stat reveals")]
    public float statRevealDelay = 0.1f;

    [Tooltip("Use animations")]
    public bool useAnimations = true;

    [Header("Color Scheme")]
    public Color excellentColor = new Color(0.2f, 0.8f, 0.2f);      // Green
    public Color veryGoodColor = new Color(0.4f, 0.9f, 0.4f);       // Light green
    public Color goodColor = new Color(1f, 0.8f, 0.2f);             // Yellow
    public Color sufficientColor = new Color(1f, 0.6f, 0.2f);       // Orange
    public Color failedColor = new Color(1f, 0.3f, 0.3f);           // Red
    public Color panelDarkColor = new Color(0.08f, 0.08f, 0.12f, 0.98f);

    [Header("Audio (Optional)")]
    [Tooltip("Sound for panel appearance")]
    public AudioClip panelOpenSound;

    [Tooltip("Sound for score counting")]
    public AudioClip scoreTickSound;

    [Tooltip("Sound for achievement unlock")]
    public AudioClip achievementSound;

    [Tooltip("Audio source for UI sounds")]
    public AudioSource audioSource;

    // Private state
    private SessionReport currentReport;
    private Coroutine animationCoroutine;
    private List<GameObject> spawnedBadges = new List<GameObject>();
    private Vector2 contentOriginalPosition;

    // Grade thresholds
    private const float EXCELLENT_THRESHOLD = 90f;
    private const float VERY_GOOD_THRESHOLD = 80f;
    private const float GOOD_THRESHOLD = 70f;
    private const float SUFFICIENT_THRESHOLD = 60f;

    void Awake()
    {
        // Store original position for slide animation
        if (contentContainer != null)
        {
            contentOriginalPosition = contentContainer.anchoredPosition;
        }

        // Setup button listeners
        SetupButtons();

        // Ensure panel starts hidden
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    void SetupButtons()
    {
        if (returnHomeButton != null)
        {
            returnHomeButton.onClick.AddListener(OnReturnHomeClicked);
        }

        if (retryButton != null)
        {
            retryButton.onClick.AddListener(OnRetryClicked);
        }

        if (detailsButton != null)
        {
            detailsButton.onClick.AddListener(OnDetailsClicked);
        }
    }

    /// <summary>
    /// Display the session summary with animations
    /// </summary>
    public void ShowSessionSummary(SessionReport report)
    {
        currentReport = report;

        // Stop any running animation
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        // Clear previous badges
        ClearBadges();

        // Show panel
        if (panel != null)
        {
            panel.SetActive(true);
        }

        // Play open sound
        PlaySound(panelOpenSound);

        if (useAnimations)
        {
            animationCoroutine = StartCoroutine(AnimatedReveal(report));
        }
        else
        {
            InstantReveal(report);
        }
    }

    /// <summary>
    /// Animated reveal sequence
    /// </summary>
    IEnumerator AnimatedReveal(SessionReport report)
    {
        // Setup initial state
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        if (contentContainer != null)
        {
            contentContainer.anchoredPosition = contentOriginalPosition + new Vector2(0, -50f);
        }

        // Reset all text to empty/zero
        ResetDisplays();

        // Phase 1: Fade in panel
        yield return StartCoroutine(FadeInPanel());

        // Phase 2: Slide up content
        yield return StartCoroutine(SlideUpContent());

        // Phase 3: Reveal header and title
        UpdateHeader(report);
        yield return new WaitForSeconds(0.2f);

        // Phase 4: Animate score counter
        yield return StartCoroutine(AnimateScoreCounter(report.score));

        // Phase 5: Reveal statistics one by one
        yield return StartCoroutine(RevealStatistics(report));

        // Phase 6: Show badges
        yield return StartCoroutine(RevealBadges(report));

        // Phase 7: Celebration effects for high scores
        if (report.score >= EXCELLENT_THRESHOLD)
        {
            TriggerCelebration();
        }
    }

    /// <summary>
    /// Instant reveal without animations
    /// </summary>
    void InstantReveal(SessionReport report)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }

        if (contentContainer != null)
        {
            contentContainer.anchoredPosition = contentOriginalPosition;
        }

        UpdateHeader(report);
        UpdateScoreDisplay(report.score);
        UpdateStatistics(report);
        GenerateBadges(report);
    }

    /// <summary>
    /// Fade in the panel
    /// </summary>
    IEnumerator FadeInPanel()
    {
        if (canvasGroup == null) yield break;

        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    /// <summary>
    /// Slide up the content
    /// </summary>
    IEnumerator SlideUpContent()
    {
        if (contentContainer == null) yield break;

        Vector2 startPos = contentOriginalPosition + new Vector2(0, -50f);
        float elapsed = 0f;

        while (elapsed < slideUpDuration)
        {
            elapsed += Time.deltaTime;
            float t = EaseOutBack(elapsed / slideUpDuration);
            contentContainer.anchoredPosition = Vector2.Lerp(startPos, contentOriginalPosition, t);
            yield return null;
        }
        contentContainer.anchoredPosition = contentOriginalPosition;
    }

    /// <summary>
    /// Animate score counting up
    /// </summary>
    IEnumerator AnimateScoreCounter(float targetScore)
    {
        float currentScore = 0f;
        float elapsed = 0f;
        Color targetColor = GetScoreColor(targetScore);

        while (elapsed < scoreCountDuration)
        {
            elapsed += Time.deltaTime;
            float t = EaseOutQuart(elapsed / scoreCountDuration);
            currentScore = Mathf.Lerp(0f, targetScore, t);

            UpdateScoreDisplay(currentScore, Color.Lerp(Color.white, targetColor, t));

            // Update progress bar
            if (scoreProgressBar != null)
            {
                scoreProgressBar.value = currentScore / 100f;
            }

            // Play tick sound occasionally
            if (Mathf.FloorToInt(currentScore) % 10 == 0 && elapsed > 0.1f)
            {
                PlaySound(scoreTickSound, 0.3f);
            }

            yield return null;
        }

        // Final value
        UpdateScoreDisplay(targetScore, targetColor);
        if (scoreProgressBar != null)
        {
            scoreProgressBar.value = targetScore / 100f;
        }

        // Update progress fill color
        if (scoreProgressFill != null)
        {
            scoreProgressFill.color = targetColor;
        }
    }

    /// <summary>
    /// Reveal statistics with staggered animation
    /// </summary>
    IEnumerator RevealStatistics(SessionReport report)
    {
        int minutes = Mathf.FloorToInt(report.sessionData.duration / 60f);
        int seconds = Mathf.FloorToInt(report.sessionData.duration % 60f);

        // Duration
        if (durationText != null)
        {
            yield return StartCoroutine(AnimateTextReveal(durationText, $"{minutes:00}:{seconds:00}"));
        }

        yield return new WaitForSeconds(statRevealDelay);

        // Positive actions
        if (positiveActionsText != null)
        {
            yield return StartCoroutine(AnimateNumberReveal(positiveActionsText, report.positiveActions, GetPositiveColor()));
        }

        yield return new WaitForSeconds(statRevealDelay);

        // Negative actions
        if (negativeActionsText != null)
        {
            yield return StartCoroutine(AnimateNumberReveal(negativeActionsText, report.negativeActions, GetNegativeColor()));
        }

        yield return new WaitForSeconds(statRevealDelay);

        // Total actions
        if (totalActionsText != null)
        {
            yield return StartCoroutine(AnimateNumberReveal(totalActionsText, report.totalActions, Color.white));
        }

        yield return new WaitForSeconds(statRevealDelay);

        // Engagement
        if (engagementText != null)
        {
            int engagementPercent = Mathf.RoundToInt(report.averageEngagement * 100f);
            Color engColor = GetEngagementColor(report.averageEngagement);
            yield return StartCoroutine(AnimateNumberReveal(engagementText, engagementPercent, engColor, "%"));
        }

        if (engagementProgressBar != null)
        {
            yield return StartCoroutine(AnimateProgressBar(engagementProgressBar, report.averageEngagement));
        }

        yield return new WaitForSeconds(statRevealDelay);

        // Disruptions
        if (disruptionsText != null)
        {
            Color disruptColor = GetDisruptionColor(report.totalDisruptions);
            yield return StartCoroutine(AnimateNumberReveal(disruptionsText, report.totalDisruptions, disruptColor));
        }
    }

    /// <summary>
    /// Animate text reveal with fade
    /// </summary>
    IEnumerator AnimateTextReveal(TextMeshProUGUI textComponent, string text)
    {
        textComponent.text = text;
        textComponent.alpha = 0f;

        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            textComponent.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            yield return null;
        }
        textComponent.alpha = 1f;
    }

    /// <summary>
    /// Animate number counting up
    /// </summary>
    IEnumerator AnimateNumberReveal(TextMeshProUGUI textComponent, int targetValue, Color color, string suffix = "")
    {
        float duration = 0.5f;
        float elapsed = 0f;
        int currentValue = 0;

        textComponent.color = color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = EaseOutQuart(elapsed / duration);
            currentValue = Mathf.RoundToInt(Mathf.Lerp(0, targetValue, t));
            textComponent.text = $"{currentValue}{suffix}";
            yield return null;
        }

        textComponent.text = $"{targetValue}{suffix}";
    }

    /// <summary>
    /// Animate progress bar fill
    /// </summary>
    IEnumerator AnimateProgressBar(Slider slider, float targetValue)
    {
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = EaseOutQuart(elapsed / duration);
            slider.value = Mathf.Lerp(0f, targetValue, t);
            yield return null;
        }

        slider.value = targetValue;
    }

    /// <summary>
    /// Reveal achievement badges
    /// </summary>
    IEnumerator RevealBadges(SessionReport report)
    {
        List<BadgeData> earnedBadges = CalculateBadges(report);

        for (int i = 0; i < earnedBadges.Count; i++)
        {
            if (badgeImages != null && i < badgeImages.Length && badgeImages[i] != null)
            {
                // Use pre-placed badge images
                badgeImages[i].gameObject.SetActive(true);
                badgeImages[i].color = earnedBadges[i].color;

                if (badgeTooltips != null && i < badgeTooltips.Length && badgeTooltips[i] != null)
                {
                    badgeTooltips[i].text = earnedBadges[i].name;
                }

                // Animate badge pop-in
                yield return StartCoroutine(AnimateBadgePopIn(badgeImages[i].transform));
                PlaySound(achievementSound, 0.5f);
            }
            else if (badgePrefab != null && badgesContainer != null)
            {
                // Instantiate badge prefab
                GameObject badge = Instantiate(badgePrefab, badgesContainer);
                spawnedBadges.Add(badge);

                var badgeImage = badge.GetComponent<Image>();
                if (badgeImage != null)
                {
                    badgeImage.color = earnedBadges[i].color;
                }

                var badgeText = badge.GetComponentInChildren<TextMeshProUGUI>();
                if (badgeText != null)
                {
                    badgeText.text = earnedBadges[i].name;
                }

                yield return StartCoroutine(AnimateBadgePopIn(badge.transform));
                PlaySound(achievementSound, 0.5f);
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    /// <summary>
    /// Animate badge pop-in effect
    /// </summary>
    IEnumerator AnimateBadgePopIn(Transform badge)
    {
        badge.localScale = Vector3.zero;
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = EaseOutBack(elapsed / duration);
            badge.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            yield return null;
        }

        badge.localScale = Vector3.one;
    }

    /// <summary>
    /// Update header section
    /// </summary>
    void UpdateHeader(SessionReport report)
    {
        Color gradeColor = GetScoreColor(report.score);
        string gradeText = GetGradeText(report.score);
        string emoji = GetGradeEmoji(report.score);

        if (titleText != null)
        {
            titleText.text = "סיכום שיעור";
            titleText.isRightToLeftText = true;
        }

        if (subtitleText != null)
        {
            subtitleText.text = $"{emoji} {gradeText} {emoji}";
            subtitleText.color = gradeColor;
            subtitleText.isRightToLeftText = true;
        }

        if (headerBackground != null)
        {
            // Subtle gradient tint based on performance
            headerBackground.color = Color.Lerp(panelDarkColor, gradeColor, 0.15f);
        }
    }

    /// <summary>
    /// Update score display
    /// </summary>
    void UpdateScoreDisplay(float score, Color? color = null)
    {
        Color displayColor = color ?? GetScoreColor(score);

        if (scoreText != null)
        {
            scoreText.text = $"{score:F1}";
            scoreText.color = displayColor;
            scoreText.isRightToLeftText = false;
        }

        if (scoreLabelText != null)
        {
            scoreLabelText.text = "/100";
            scoreLabelText.color = new Color(displayColor.r, displayColor.g, displayColor.b, 0.7f);
        }
    }

    /// <summary>
    /// Update all statistics instantly
    /// </summary>
    void UpdateStatistics(SessionReport report)
    {
        int minutes = Mathf.FloorToInt(report.sessionData.duration / 60f);
        int seconds = Mathf.FloorToInt(report.sessionData.duration % 60f);

        if (durationText != null)
        {
            durationText.text = $"{minutes:00}:{seconds:00}";
        }

        if (positiveActionsText != null)
        {
            positiveActionsText.text = $"{report.positiveActions}";
            positiveActionsText.color = GetPositiveColor();
        }

        if (negativeActionsText != null)
        {
            negativeActionsText.text = $"{report.negativeActions}";
            negativeActionsText.color = GetNegativeColor();
        }

        if (totalActionsText != null)
        {
            totalActionsText.text = $"{report.totalActions}";
        }

        if (engagementText != null)
        {
            engagementText.text = $"{Mathf.RoundToInt(report.averageEngagement * 100f)}%";
            engagementText.color = GetEngagementColor(report.averageEngagement);
        }

        if (engagementProgressBar != null)
        {
            engagementProgressBar.value = report.averageEngagement;
        }

        if (disruptionsText != null)
        {
            disruptionsText.text = $"{report.totalDisruptions}";
            disruptionsText.color = GetDisruptionColor(report.totalDisruptions);
        }
    }

    /// <summary>
    /// Reset all displays to initial state
    /// </summary>
    void ResetDisplays()
    {
        if (scoreText != null) scoreText.text = "0";
        if (durationText != null) durationText.text = "--:--";
        if (positiveActionsText != null) positiveActionsText.text = "-";
        if (negativeActionsText != null) negativeActionsText.text = "-";
        if (totalActionsText != null) totalActionsText.text = "-";
        if (engagementText != null) engagementText.text = "-";
        if (disruptionsText != null) disruptionsText.text = "-";
        if (scoreProgressBar != null) scoreProgressBar.value = 0f;
        if (engagementProgressBar != null) engagementProgressBar.value = 0f;

        // Hide all badge images initially
        if (badgeImages != null)
        {
            foreach (var badge in badgeImages)
            {
                if (badge != null) badge.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Generate badges instantly
    /// </summary>
    void GenerateBadges(SessionReport report)
    {
        List<BadgeData> badges = CalculateBadges(report);

        for (int i = 0; i < badges.Count; i++)
        {
            if (badgeImages != null && i < badgeImages.Length && badgeImages[i] != null)
            {
                badgeImages[i].gameObject.SetActive(true);
                badgeImages[i].color = badges[i].color;

                if (badgeTooltips != null && i < badgeTooltips.Length && badgeTooltips[i] != null)
                {
                    badgeTooltips[i].text = badges[i].name;
                }
            }
        }
    }

    /// <summary>
    /// Calculate earned badges based on performance
    /// </summary>
    List<BadgeData> CalculateBadges(SessionReport report)
    {
        List<BadgeData> badges = new List<BadgeData>();

        // Score-based badges
        if (report.score >= EXCELLENT_THRESHOLD)
        {
            badges.Add(new BadgeData("מצוין!", excellentColor, "🌟"));
        }
        else if (report.score >= VERY_GOOD_THRESHOLD)
        {
            badges.Add(new BadgeData("טוב מאוד", veryGoodColor, "⭐"));
        }

        // No negative actions badge
        if (report.negativeActions == 0 && report.totalActions > 0)
        {
            badges.Add(new BadgeData("מורה חיובי", new Color(0.3f, 0.8f, 0.9f), "💙"));
        }

        // High engagement badge
        if (report.averageEngagement >= 0.85f)
        {
            badges.Add(new BadgeData("מעורבות גבוהה", new Color(0.9f, 0.7f, 0.2f), "🔥"));
        }

        // No disruptions badge
        if (report.totalDisruptions == 0)
        {
            badges.Add(new BadgeData("כיתה שקטה", new Color(0.6f, 0.8f, 0.6f), "🤫"));
        }

        // Efficiency badge (low action count with good score)
        if (report.totalActions <= 10 && report.score >= GOOD_THRESHOLD)
        {
            badges.Add(new BadgeData("יעילות", new Color(0.8f, 0.6f, 0.9f), "⚡"));
        }

        return badges;
    }

    /// <summary>
    /// Clear spawned badges
    /// </summary>
    void ClearBadges()
    {
        foreach (var badge in spawnedBadges)
        {
            if (badge != null) Destroy(badge);
        }
        spawnedBadges.Clear();

        // Hide pre-placed badges
        if (badgeImages != null)
        {
            foreach (var badge in badgeImages)
            {
                if (badge != null) badge.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Trigger celebration effects
    /// </summary>
    void TriggerCelebration()
    {
        if (celebrationParticles != null)
        {
            celebrationParticles.Play();
        }

        if (excellentEffect != null)
        {
            excellentEffect.SetActive(true);
        }
    }

    /// <summary>
    /// Play a sound effect
    /// </summary>
    void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (clip == null || audioSource == null) return;
        audioSource.PlayOneShot(clip, volume);
    }

    #region Button Handlers

    void OnReturnHomeClicked()
    {
        HidePanel();
        SceneManager.LoadScene("TeacherHomeScreen");
    }

    void OnRetryClicked()
    {
        HidePanel();
        // Reload current scene or start new session
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnDetailsClicked()
    {
        // Show detailed report (can be implemented as needed)
        Debug.Log("Show detailed report clicked");
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Hide the panel with animation
    /// </summary>
    public void HidePanel()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        if (useAnimations && canvasGroup != null)
        {
            StartCoroutine(FadeOutPanel());
        }
        else
        {
            if (panel != null) panel.SetActive(false);
        }
    }

    IEnumerator FadeOutPanel()
    {
        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }

        if (panel != null) panel.SetActive(false);
        canvasGroup.alpha = 1f;
    }

    /// <summary>
    /// Get the current report
    /// </summary>
    public SessionReport GetCurrentReport()
    {
        return currentReport;
    }

    #endregion

    #region Color Helpers

    Color GetScoreColor(float score)
    {
        if (score >= EXCELLENT_THRESHOLD) return excellentColor;
        if (score >= VERY_GOOD_THRESHOLD) return veryGoodColor;
        if (score >= GOOD_THRESHOLD) return goodColor;
        if (score >= SUFFICIENT_THRESHOLD) return sufficientColor;
        return failedColor;
    }

    Color GetPositiveColor()
    {
        return new Color(0.3f, 0.8f, 0.3f); // Green
    }

    Color GetNegativeColor()
    {
        return new Color(0.9f, 0.3f, 0.3f); // Red
    }

    Color GetEngagementColor(float engagement)
    {
        if (engagement >= 0.8f) return new Color(0.3f, 0.8f, 0.3f);
        if (engagement >= 0.6f) return new Color(1f, 0.8f, 0.2f);
        if (engagement >= 0.4f) return new Color(1f, 0.6f, 0.2f);
        return new Color(0.9f, 0.3f, 0.3f);
    }

    Color GetDisruptionColor(int disruptions)
    {
        if (disruptions <= 2) return new Color(0.3f, 0.8f, 0.3f);
        if (disruptions <= 5) return new Color(1f, 0.8f, 0.2f);
        if (disruptions <= 8) return new Color(1f, 0.6f, 0.2f);
        return new Color(0.9f, 0.3f, 0.3f);
    }

    #endregion

    #region Text Helpers

    string GetGradeText(float score)
    {
        if (score >= EXCELLENT_THRESHOLD) return "מצוין - ניהול מעולה!";
        if (score >= VERY_GOOD_THRESHOLD) return "טוב מאוד - עבודה טובה!";
        if (score >= GOOD_THRESHOLD) return "טוב - מספק";
        if (score >= SUFFICIENT_THRESHOLD) return "מספיק - צריך שיפור";
        return "נכשל - בדוק אסטרטגיות";
    }

    string GetGradeEmoji(float score)
    {
        if (score >= EXCELLENT_THRESHOLD) return "🌟";
        if (score >= VERY_GOOD_THRESHOLD) return "⭐";
        if (score >= GOOD_THRESHOLD) return "👍";
        if (score >= SUFFICIENT_THRESHOLD) return "📝";
        return "📚";
    }

    #endregion

    #region Easing Functions

    float EaseOutQuart(float t)
    {
        return 1f - Mathf.Pow(1f - t, 4f);
    }

    float EaseOutBack(float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }

    #endregion

    /// <summary>
    /// Badge data structure
    /// </summary>
    private struct BadgeData
    {
        public string name;
        public Color color;
        public string emoji;

        public BadgeData(string name, Color color, string emoji)
        {
            this.name = name;
            this.color = color;
            this.emoji = emoji;
        }
    }
}
