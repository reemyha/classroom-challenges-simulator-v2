using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Simple UI bubble that displays text above a model.
/// Attach this to a student model - it will automatically create a world-space canvas
/// and position itself above the model's head.
/// FIXED: Canvas and UI elements are now on UI layer and won't interfere with physics
/// </summary>
public class StudentResponseBubble : MonoBehaviour
{
    [Header("UI Elements (Auto-created if not assigned)")]
    [Tooltip("Canvas for the response bubble")]
    public Canvas responseCanvas;

    [Tooltip("Text component displaying the response")]
    public TextMeshProUGUI responseText;

    [Tooltip("Image background for the bubble")]
    public Image bubbleBackground;

    [Header("Positioning")]
    [Tooltip("Offset above the model (relative to this transform)")]
    public Vector3 headOffset = new Vector3(0, 2.0f, 0);

    [Tooltip("Camera to use for world-to-screen conversion (auto-finds Main Camera if not set)")]
    public Camera mainCamera;

    [Header("Sizing")]
    [Tooltip("Minimum width of the bubble")]
    public float minWidth = 100f;

    [Tooltip("Maximum width of the bubble")]
    public float maxWidth = 250f;

    [Tooltip("Padding around text")]
    public Vector2 textPadding = new Vector2(10f, 8f);

    [Header("Styling")]
    [Tooltip("Background color of the bubble")]
    public Color backgroundColor = new Color(1f, 1f, 1f, 0.95f);

    [Tooltip("Text color")]
    public Color textColor = new Color(0.2f, 0.2f, 0.2f, 1f);

    [Tooltip("Font size")]
    public int fontSize = 14;

    private string currentResponse = "";
    private RectTransform bubbleRect;

    void Start()
    {
        // Find camera if not assigned
        if (mainCamera == null)
            mainCamera = Camera.main;

        // Setup canvas and UI elements (delayed to avoid interfering with spawner)
        SetupBubbleUI();

        // Hide initially
        HideBubble();
    }

    void Update()
    {
        if (responseCanvas == null || !responseCanvas.gameObject.activeSelf)
            return;

        // Update position to follow model's head
        UpdatePosition();
    }

    /// <summary>
    /// Create the response canvas if it doesn't exist
    /// </summary>
    void CreateResponseCanvas()
    {
        GameObject canvasObj = new GameObject("ResponseCanvas");
        canvasObj.transform.SetParent(transform);
        canvasObj.transform.localPosition = Vector3.zero;
        canvasObj.transform.localRotation = Quaternion.identity;
        canvasObj.transform.localScale = Vector3.one;

        // CRITICAL FIX: Set canvas to UI layer to prevent physics interference
        canvasObj.layer = LayerMask.NameToLayer("UI");
        if (canvasObj.layer == -1)
        {
            // If UI layer doesn't exist, create it on layer 5 (standard Unity UI layer)
            canvasObj.layer = 5;
        }

        responseCanvas = canvasObj.AddComponent<Canvas>();
        responseCanvas.renderMode = RenderMode.WorldSpace;
        responseCanvas.worldCamera = mainCamera;
        
        // Disable GraphicRaycaster to prevent UI from blocking clicks
        GraphicRaycaster raycaster = canvasObj.GetComponent<GraphicRaycaster>();
        if (raycaster != null)
        {
            raycaster.enabled = false;
        }

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 10f;

        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(300, 150);
        
        // Set sorting order high so it renders above everything
        responseCanvas.sortingOrder = 1000;
    }

    /// <summary>
    /// Setup the bubble UI elements
    /// </summary>
    void SetupBubbleUI()
    {
        // Create canvas if needed
        if (responseCanvas == null)
        {
            responseCanvas = GetComponentInChildren<Canvas>();
            if (responseCanvas == null)
            {
                CreateResponseCanvas();
            }
        }

        if (responseCanvas == null) return;

        // Ensure canvas is on UI layer
        if (responseCanvas.gameObject.layer != 5 && responseCanvas.gameObject.layer != LayerMask.NameToLayer("UI"))
        {
            int uiLayer = LayerMask.NameToLayer("UI");
            responseCanvas.gameObject.layer = (uiLayer != -1) ? uiLayer : 5;
        }

        // Create bubble panel (RectTransform)
        if (bubbleRect == null)
        {
            GameObject panelObj = new GameObject("BubblePanel");
            panelObj.layer = responseCanvas.gameObject.layer; // Same layer as canvas
            panelObj.transform.SetParent(responseCanvas.transform, false);
            bubbleRect = panelObj.AddComponent<RectTransform>();
            bubbleRect.anchorMin = new Vector2(0.5f, 0.5f);
            bubbleRect.anchorMax = new Vector2(0.5f, 0.5f);
            bubbleRect.pivot = new Vector2(0.5f, 0.5f);
            bubbleRect.sizeDelta = new Vector2(minWidth, 40f);
        }

        // Create or find background
        if (bubbleBackground == null)
        {
            GameObject bgObj = new GameObject("Background");
            bgObj.layer = responseCanvas.gameObject.layer; // Same layer as canvas
            bgObj.transform.SetParent(bubbleRect, false);
            bubbleBackground = bgObj.AddComponent<Image>();
            bubbleBackground.color = backgroundColor;
            
            // Disable raycast target to prevent blocking clicks
            bubbleBackground.raycastTarget = false;
            
            RectTransform bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            bgRect.anchoredPosition = Vector2.zero;
        }

        // Create or find text
        if (responseText == null)
        {
            GameObject textObj = new GameObject("ResponseText");
            textObj.layer = responseCanvas.gameObject.layer; // Same layer as canvas
            textObj.transform.SetParent(bubbleRect, false);
            responseText = textObj.AddComponent<TextMeshProUGUI>();
            responseText.color = textColor;
            responseText.fontSize = fontSize;
            responseText.alignment = TextAlignmentOptions.Center;
            responseText.enableWordWrapping = true;
            responseText.overflowMode = TextOverflowModes.Ellipsis;
            // Enable RTL support for Hebrew text
            responseText.isRightToLeftText = true;
            
            // Disable raycast target to prevent blocking clicks
            responseText.raycastTarget = false;

            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = new Vector2(-textPadding.x * 2, -textPadding.y * 2);
            textRect.anchoredPosition = Vector2.zero;
        }
    }

    /// <summary>
    /// Update bubble position to follow model's head
    /// </summary>
    void UpdatePosition()
    {
        if (mainCamera == null || responseCanvas == null)
            return;

        // Calculate world position above the model (this transform + offset)
        Vector3 worldPosition = transform.position + headOffset;

        // Convert to screen space to check visibility
        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPosition);

        // Check if model is visible
        if (screenPos.z < 0)
        {
            responseCanvas.gameObject.SetActive(false);
            return;
        }

        // Position canvas above model and make it face the camera
        responseCanvas.transform.position = worldPosition;
        responseCanvas.transform.LookAt(mainCamera.transform);
        responseCanvas.transform.Rotate(0, 180, 0); // Flip to face camera
    }


    /// <summary>
    /// Show response text in the bubble.
    /// </summary>
    public void ShowResponse(string response)
    {
        if (string.IsNullOrEmpty(response))
        {
            HideBubble();
            return;
        }

        currentResponse = response;

        if (responseText != null)
        {
            responseText.text = response;
            
            // Auto-detect Hebrew and set RTL alignment
            bool isHebrew = ContainsHebrew(response);
            if (isHebrew)
            {
                responseText.isRightToLeftText = true;
                responseText.alignment = TextAlignmentOptions.Right;
            }
            else
            {
                responseText.isRightToLeftText = false;
                responseText.alignment = TextAlignmentOptions.Center;
            }
        }

        // Calculate size based on text
        if (responseText != null && bubbleRect != null)
        {
            // Force text to recalculate preferred size
            responseText.ForceMeshUpdate();
            
            float textWidth = responseText.preferredWidth;
            float textHeight = responseText.preferredHeight;
            
            // Set width (clamped to min/max)
            float width = Mathf.Clamp(textWidth + textPadding.x * 2, minWidth, maxWidth);
            float height = Mathf.Max(35f, textHeight + textPadding.y * 2);
            
            bubbleRect.sizeDelta = new Vector2(width, height);
        }

        // Show the bubble
        if (responseCanvas != null)
        {
            responseCanvas.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Hide the response bubble.
    /// </summary>
    public void HideBubble()
    {
        if (responseCanvas != null)
        {
            responseCanvas.gameObject.SetActive(false);
        }

        currentResponse = "";
    }

    /// <summary>
    /// Check if bubble is currently showing
    /// </summary>
    public bool IsShowing()
    {
        return responseCanvas != null && responseCanvas.gameObject.activeSelf && !string.IsNullOrEmpty(currentResponse);
    }

    /// <summary>
    /// Check if text contains Hebrew characters
    /// </summary>
    private bool ContainsHebrew(string text)
    {
        if (string.IsNullOrEmpty(text))
            return false;
            
        foreach (char c in text)
        {
            if (c >= 0x0590 && c <= 0x05FF) // Hebrew Unicode range
                return true;
        }
        return false;
    }

    /// <summary>
    /// Recursively set layer for all children
    /// </summary>
    void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}