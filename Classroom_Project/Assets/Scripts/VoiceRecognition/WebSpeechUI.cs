using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Simple UI for Web Speech Recognition
/// Connects buttons and displays to the WebSpeechRecognition component
/// </summary>
public class WebSpeechUI : MonoBehaviour
{
    [Header("References")]
    public WebSpeechRecognition webSpeech;

    [Header("UI Elements")]
    public Button recordButton;
    public Button stopButton;
    public TMP_Text buttonText;
    public TMP_Text statusText;
    public TMP_Text transcriptText;
    public Image recordingIndicator;

    [Header("Visual Feedback")]
    public Color idleColor = Color.white;
    public Color recordingColor = Color.red;
    public Color processingColor = Color.yellow;

    void Start()
    {
        // Find WebSpeechRecognition if not assigned
        if (webSpeech == null)
        {
            webSpeech = FindObjectOfType<WebSpeechRecognition>();
            if (webSpeech == null)
            {
                Debug.LogError("WebSpeechRecognition component not found in scene!");
                return;
            }
        }

        // Setup buttons
        if (recordButton != null)
        {
            recordButton.onClick.AddListener(OnRecordClicked);
        }

        if (stopButton != null)
        {
            stopButton.onClick.AddListener(OnStopClicked);
            stopButton.gameObject.SetActive(false);
        }

        // Subscribe to events
        webSpeech.OnRecordingStarted += OnRecordingStarted;
        webSpeech.OnRecordingEnded += OnRecordingEnded;
        webSpeech.OnTranscriptionReceived += OnTranscriptionReceived;
        webSpeech.OnError += OnError;

        // Initial state
        UpdateUI(false);
    }

    void OnDestroy()
    {
        if (webSpeech != null)
        {
            webSpeech.OnRecordingStarted -= OnRecordingStarted;
            webSpeech.OnRecordingEnded -= OnRecordingEnded;
            webSpeech.OnTranscriptionReceived -= OnTranscriptionReceived;
            webSpeech.OnError -= OnError;
        }
    }

    void OnRecordClicked()
    {
        if (webSpeech != null)
        {
            webSpeech.StartRecording();
        }
    }

    void OnStopClicked()
    {
        if (webSpeech != null)
        {
            webSpeech.StopRecording();
        }
    }

    void OnRecordingStarted()
    {
        UpdateUI(true);
        SetStatus("מאזין... דבר עכשיו!");
    }

    void OnRecordingEnded()
    {
        UpdateUI(false);
        SetStatus("מעבד...");
    }

    void OnTranscriptionReceived(string transcript)
    {
        if (transcriptText != null)
        {
            transcriptText.text = $"אמרת: {transcript}";
        }
        SetStatus("הושלם!");
    }

    void OnError(string error)
    {
        SetStatus($"שגיאה: {error}");
        UpdateUI(false);
    }

    void UpdateUI(bool isRecording)
    {
        // Toggle buttons
        if (recordButton != null)
        {
            recordButton.gameObject.SetActive(!isRecording);
        }

        if (stopButton != null)
        {
            stopButton.gameObject.SetActive(isRecording);
        }

        // Update button text
        if (buttonText != null)
        {
            buttonText.text = isRecording ? "Stop Recording" : "Start Recording";
        }

        // Update indicator
        if (recordingIndicator != null)
        {
            recordingIndicator.color = isRecording ? recordingColor : idleColor;
            
            // Pulse animation when recording
            if (isRecording)
            {
                StartCoroutine(PulseIndicator());
            }
        }
    }

    void SetStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
        Debug.Log($"[WebSpeechUI] {message}");
    }

    System.Collections.IEnumerator PulseIndicator()
    {
        while (webSpeech != null && webSpeech.IsRecording())
        {
            if (recordingIndicator != null)
            {
                // Pulse alpha
                float alpha = (Mathf.Sin(Time.time * 3f) + 1f) / 2f;
                alpha = Mathf.Lerp(0.3f, 1f, alpha);

                Color col = recordingColor;
                col.a = alpha;
                recordingIndicator.color = col;
            }

            yield return null;
        }

        // Reset
        if (recordingIndicator != null)
        {
            recordingIndicator.color = idleColor;
        }
    }
}
