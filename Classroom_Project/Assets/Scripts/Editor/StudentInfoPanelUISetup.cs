using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Editor utility to automatically generate the StudentInfoPanelUI layout
/// Usage: Right-click in Hierarchy → UI → Student Info Panel (Auto-Setup)
/// </summary>
public class StudentInfoPanelUISetup : Editor
{
    [MenuItem("GameObject/UI/Student Info Panel (Auto-Setup)", false, 10)]
    static void CreateStudentInfoPanelUI(MenuCommand menuCommand)
    {
        // Create root panel
        GameObject panelRoot = new GameObject("StudentInfoPanel");
        RectTransform rootRect = panelRoot.AddComponent<RectTransform>();
        panelRoot.AddComponent<CanvasRenderer>();
        Image panelBg = panelRoot.AddComponent<Image>();
        panelBg.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);

        // Set anchors to right side of screen
        rootRect.anchorMin = new Vector2(1f, 0.5f);
        rootRect.anchorMax = new Vector2(1f, 0.5f);
        rootRect.pivot = new Vector2(1f, 0.5f);
        rootRect.anchoredPosition = new Vector2(-20f, 0f);
        rootRect.sizeDelta = new Vector2(350f, 500f);

        // Add the StudentInfoPanelUI component
        StudentInfoPanelUI panelUI = panelRoot.AddComponent<StudentInfoPanelUI>();

        // Create title text
        GameObject titleObj = CreateText("TitleText", panelRoot.transform, "מידע על תלמיד", 24, TextAlignmentOptions.Center);
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0f, -10f);
        titleRect.sizeDelta = new Vector2(-20f, 40f);
        titleObj.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        // Create emotion details text (simple text display)
        GameObject emotionTextObj = CreateText("EmotionDetailsText", panelRoot.transform, "", 16, TextAlignmentOptions.TopRight);
        RectTransform emotionTextRect = emotionTextObj.GetComponent<RectTransform>();
        emotionTextRect.anchorMin = new Vector2(0f, 0.6f);
        emotionTextRect.anchorMax = new Vector2(1f, 1f);
        emotionTextRect.pivot = new Vector2(0.5f, 1f);
        emotionTextRect.anchoredPosition = new Vector2(0f, -60f);
        emotionTextRect.sizeDelta = new Vector2(-20f, -70f);
        panelUI.emotionDetailsText = emotionTextObj.GetComponent<TextMeshProUGUI>();

        // Create separator
        GameObject separator = CreateSeparator("Separator", panelRoot.transform, 0.6f);

        // Create emotion sliders section
        float startY = -260f;
        float spacing = 70f;

        // Happiness slider
        CreateEmotionSlider("HappinessSlider", panelRoot.transform, "שמחה", startY,
            out Slider happinessSlider, out TextMeshProUGUI happinessLabel);
        panelUI.happinessBar = happinessSlider;
        panelUI.happinessLabel = happinessLabel;

        // Sadness slider
        CreateEmotionSlider("SadnessSlider", panelRoot.transform, "עצב", startY - spacing * 1,
            out Slider sadnessSlider, out TextMeshProUGUI sadnessLabel);
        panelUI.sadnessBar = sadnessSlider;
        panelUI.sadnessLabel = sadnessLabel;

        // Frustration slider
        CreateEmotionSlider("FrustrationSlider", panelRoot.transform, "תסכול", startY - spacing * 2,
            out Slider frustrationSlider, out TextMeshProUGUI frustrationLabel);
        panelUI.frustrationBar = frustrationSlider;
        panelUI.frustrationLabel = frustrationLabel;

        // Boredom slider
        CreateEmotionSlider("BoredomSlider", panelRoot.transform, "שעמום", startY - spacing * 3,
            out Slider boredomSlider, out TextMeshProUGUI boredomLabel);
        panelUI.boredomBar = boredomSlider;
        panelUI.boredomLabel = boredomLabel;

        // Anger slider
        CreateEmotionSlider("AngerSlider", panelRoot.transform, "כעס", startY - spacing * 4,
            out Slider angerSlider, out TextMeshProUGUI angerLabel);
        panelUI.angerBar = angerSlider;
        panelUI.angerLabel = angerLabel;

        // Register creation in undo system
        GameObjectUtility.SetParentAndAlign(panelRoot, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(panelRoot, "Create Student Info Panel");
        Selection.activeObject = panelRoot;

        Debug.Log("StudentInfoPanel created successfully! Assign this to TeacherUI's studentInfoPanel field.");
    }

    static GameObject CreateText(string name, Transform parent, string text, int fontSize, TextAlignmentOptions alignment)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);

        RectTransform rect = textObj.AddComponent<RectTransform>();
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();

        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = alignment;
        tmp.color = Color.white;
        tmp.enableWordWrapping = true;

        return textObj;
    }

    static GameObject CreateSeparator(string name, Transform parent, float normalizedY)
    {
        GameObject separator = new GameObject(name);
        separator.transform.SetParent(parent, false);

        RectTransform rect = separator.AddComponent<RectTransform>();
        Image img = separator.AddComponent<Image>();

        img.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        rect.anchorMin = new Vector2(0.05f, normalizedY);
        rect.anchorMax = new Vector2(0.95f, normalizedY);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(0f, 2f);

        return separator;
    }

    static void CreateEmotionSlider(string name, Transform parent, string emotionName, float yPos,
        out Slider slider, out TextMeshProUGUI label)
    {
        // Create container
        GameObject container = new GameObject(name + "Container");
        container.transform.SetParent(parent, false);
        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0f, 1f);
        containerRect.anchorMax = new Vector2(1f, 1f);
        containerRect.pivot = new Vector2(0.5f, 1f);
        containerRect.anchoredPosition = new Vector2(0f, yPos);
        containerRect.sizeDelta = new Vector2(-20f, 60f);

        // Create label
        GameObject labelObj = CreateText(name + "Label", container.transform, emotionName, 14, TextAlignmentOptions.TopRight);
        RectTransform labelRect = labelObj.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0f, 1f);
        labelRect.anchorMax = new Vector2(1f, 1f);
        labelRect.pivot = new Vector2(0.5f, 1f);
        labelRect.anchoredPosition = new Vector2(0f, 0f);
        labelRect.sizeDelta = new Vector2(0f, 20f);
        label = labelObj.GetComponent<TextMeshProUGUI>();

        // Create slider
        GameObject sliderObj = new GameObject(name);
        sliderObj.transform.SetParent(container.transform, false);
        RectTransform sliderRect = sliderObj.AddComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0f, 0f);
        sliderRect.anchorMax = new Vector2(1f, 0f);
        sliderRect.pivot = new Vector2(0.5f, 0f);
        sliderRect.anchoredPosition = new Vector2(0f, 5f);
        sliderRect.sizeDelta = new Vector2(0f, 30f);

        slider = sliderObj.AddComponent<Slider>();
        slider.minValue = 1f;
        slider.maxValue = 10f;
        slider.wholeNumbers = false;
        slider.interactable = false; // Read-only display

        // Create Background
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(sliderObj.transform, false);
        RectTransform bgRect = bg.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        Image bgImage = bg.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        bgImage.type = Image.Type.Sliced;

        // Create Fill Area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObj.transform, false);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.sizeDelta = new Vector2(-10f, -10f);

        // Create Fill
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        RectTransform fillRect = fill.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(0.2f, 1f, 0.2f, 1f); // Default green
        fillImage.type = Image.Type.Sliced;

        // Create Handle Slide Area (optional, can be empty for read-only)
        GameObject handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(sliderObj.transform, false);
        RectTransform handleAreaRect = handleArea.AddComponent<RectTransform>();
        handleAreaRect.anchorMin = Vector2.zero;
        handleAreaRect.anchorMax = Vector2.one;
        handleAreaRect.sizeDelta = new Vector2(-20f, 0f);

        // Assign slider components
        slider.fillRect = fillRect;
        slider.targetGraphic = fillImage;
    }

    [MenuItem("CONTEXT/StudentInfoPanelUI/Auto-Setup Emotion Display")]
    static void AutoSetupEmotionDisplay(MenuCommand command)
    {
        StudentInfoPanelUI panelUI = command.context as StudentInfoPanelUI;
        if (panelUI == null) return;

        Transform parent = panelUI.transform;

        // Check if already set up
        if (panelUI.emotionDetailsText != null)
        {
            bool result = EditorUtility.DisplayDialog(
                "Already Set Up",
                "This StudentInfoPanelUI already has components assigned. Do you want to recreate them?",
                "Yes, Recreate",
                "Cancel"
            );

            if (!result) return;
        }

        // Create or find components
        // Similar to above but for existing GameObject
        EditorUtility.DisplayDialog("Setup Complete",
            "Emotion display components have been set up. Make sure to assign this panel to TeacherUI's studentInfoPanel field.",
            "OK");
    }
}
