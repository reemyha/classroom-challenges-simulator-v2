using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeacherVoiceRecorderUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text transcriptTopText;     // text shown at top
    public TMP_Text statusText;            // optional
    [Tooltip("The yellow record button that starts/stops recording")]
    public Button recordButton;            // Record button
    [Tooltip("Restart button - clears and resets recording")]
    public Button restartButton;           // Restart button (if exists in scene)
    [Tooltip("Cancel button - cancels current recording")]
    public Button cancelButton;            // Cancel button (if exists in scene)

    [Header("Recording")]
    public int sampleRate = 16000;
    public int maxSeconds = 15;

    private bool isRecording;
    private Coroutine fakeTranscribeRoutine;
    private WebSpeechRecognition webSpeech;

    void Start()
    {
        // Setup button listeners
        if (recordButton != null)
        {
            recordButton.onClick.AddListener(StartRecording);
            Debug.Log("Record button connected successfully");
        }
        else
        {
            Debug.LogWarning("Record button not assigned in Inspector! Please drag the yellow button to the 'Record Button' field.");
        }

        // Connect restart button
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(ResetRecording);
            Debug.Log("Restart button connected successfully");
        }

        // Connect cancel button
        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(CancelRecording);
            Debug.Log("Cancel button connected successfully");
        }

        // Get reference to WebSpeechRecognition (for Hebrew support)
        webSpeech = WebSpeechRecognition.Instance;
        if (webSpeech == null)
        {
            // Try to find it in the scene
            webSpeech = FindObjectOfType<WebSpeechRecognition>();
        }

        if (webSpeech != null)
        {
            // Make sure it's configured for Hebrew
            if (webSpeech.language != "he-IL")
            {
                webSpeech.language = "he-IL"; // Hebrew - Israel
                Debug.Log("Web Speech API language set to Hebrew (he-IL)");
            }

            // Subscribe to transcription events
            webSpeech.OnTranscriptionReceived += OnTranscriptionReceived;
            webSpeech.OnError += OnTranscriptionError;
            webSpeech.OnRecordingStarted += OnRecordingStarted;
            webSpeech.OnRecordingEnded += OnRecordingEnded;
            
            SetStatus("מוכן - לחץ על הכפתור להקלטה בעברית");
        }
        else
        {
            Debug.LogWarning("WebSpeechRecognition not found in scene! Please add WebSpeechRecognition component to a GameObject.");
            SetStatus("Web Speech API not available");
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (webSpeech != null)
        {
            webSpeech.OnTranscriptionReceived -= OnTranscriptionReceived;
            webSpeech.OnError -= OnTranscriptionError;
            webSpeech.OnRecordingStarted -= OnRecordingStarted;
            webSpeech.OnRecordingEnded -= OnRecordingEnded;
        }
    }

    public void StartRecording()
    {
        // Sync state with service
        if (webSpeech != null)
        {
            isRecording = webSpeech.IsRecording();
        }

        // Toggle: If already recording, stop. Otherwise, start.
        if (isRecording)
        {
            StopRecording();
            return;
        }

        StopFakeTranscribeIfRunning();

        // Use WebSpeechRecognition (works in WebGL browsers)
        if (webSpeech != null)
        {
            webSpeech.StartRecording();
            // Note: isRecording will be set by OnRecordingStarted event
            // But set it here for immediate UI feedback
            isRecording = true;
            
            if (transcriptTopText != null)
                transcriptTopText.text = "מקליט...";
            
            // Update button appearance
            UpdateButtonState(true);
            
            SetStatus("מקליט (עברית)...");
            return;
        }

        // Fallback: show error if WebSpeech not available
        SetStatus("Web Speech API לא זמין - בנה עבור WebGL");
        Debug.LogError("WebSpeechRecognition not found! Add it to the scene and build for WebGL.");
    }

    public void StopRecording()
    {
        if (!isRecording) return;

        // Use WebSpeechRecognition
        if (webSpeech != null)
        {
            webSpeech.StopRecording();
            isRecording = false;
            
            if (transcriptTopText != null)
                transcriptTopText.text = "מעבד...";
            
            // Update button appearance
            UpdateButtonState(false);
            
            SetStatus("מעבד...");
            return;
        }

        // Fallback: reset state
        isRecording = false;
        UpdateButtonState(false);
        SetStatus("Stopped");
    }

    /// <summary>
    /// Reset/restart recording - clears transcription and resets state
    /// </summary>
    public void ResetRecording()
    {
        // Stop recording if using WebSpeech
        if (webSpeech != null && isRecording)
        {
            webSpeech.StopRecording();
        }

        isRecording = false;
        StopFakeTranscribeIfRunning();

        if (transcriptTopText != null) transcriptTopText.text = "";
        UpdateButtonState(false);
        SetStatus("מוכן - לחץ על הכפתור להקלטה בעברית");
        
        Debug.Log("Recording reset - ready for new recording");
    }

    /// <summary>
    /// Cancel current recording - stops and clears everything
    /// </summary>
    public void CancelRecording()
    {
        // Stop recording if using WebSpeech
        if (webSpeech != null && isRecording)
        {
            webSpeech.StopRecording();
        }

        isRecording = false;
        StopFakeTranscribeIfRunning();

        if (transcriptTopText != null) transcriptTopText.text = "";
        UpdateButtonState(false);
        SetStatus("Cancelled");
        
        Debug.Log("Recording cancelled");
    }


    private void OnTranscriptionReceived(string transcript)
    {
        // Ensure recording state is false
        isRecording = false;
        
        // Update button appearance back to yellow
        UpdateButtonState(false);
        
        // Display transcription using the coroutine
        if (fakeTranscribeRoutine != null)
        {
            StopCoroutine(fakeTranscribeRoutine);
        }
        
        fakeTranscribeRoutine = StartCoroutine(DisplayTranscription(transcript));
    }

    private void OnTranscriptionError(string error)
    {
        if (transcriptTopText != null)
            transcriptTopText.text = $"שגיאה: {error}";
        
        SetStatus($"שגיאה: {error}");
        isRecording = false;
        
        // Update button appearance back to yellow on error
        UpdateButtonState(false);
    }

    private void OnRecordingStarted()
    {
        isRecording = true;
        UpdateButtonState(true);
        if (transcriptTopText != null)
            transcriptTopText.text = "מקליט...";
        SetStatus("Recording (Hebrew)...");
    }

    private void OnRecordingEnded()
    {
        isRecording = false;
        // Recording ended, waiting for transcription
        if (transcriptTopText != null)
            transcriptTopText.text = "מעבד...";
        
        SetStatus("מעבד...");
    }

    private IEnumerator DisplayTranscription(string transcript)
    {
        // Small delay to show processing state
        yield return new WaitForSeconds(0.5f);

        if (transcriptTopText != null)
            transcriptTopText.text = $"המורה אמר: {transcript}";

        SetStatus("Done");
        fakeTranscribeRoutine = null;
    }

    private void StopFakeTranscribeIfRunning()
    {
        if (fakeTranscribeRoutine != null)
        {
            StopCoroutine(fakeTranscribeRoutine);
            fakeTranscribeRoutine = null;
        }
    }

    private void SetStatus(string msg)
    {
        if (statusText != null) statusText.text = msg;
        Debug.Log(msg);
    }

    /// <summary>
    /// Update button visual state based on recording status
    /// </summary>
    private void UpdateButtonState(bool recording)
    {
        if (recordButton == null) return;

        // Update button colors or text based on state
        var colors = recordButton.colors;
        if (recording)
        {
            // Change to red when recording
            colors.normalColor = Color.red;
            colors.highlightedColor = new Color(1f, 0.5f, 0.5f);
        }
        else
        {
            // Yellow when not recording
            colors.normalColor = Color.yellow;
            colors.highlightedColor = new Color(1f, 1f, 0.5f);
        }
        recordButton.colors = colors;

        // Optionally update button text if it has a TextMeshProUGUI component
        TMPro.TextMeshProUGUI buttonText = recordButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.text = recording ? "עצור הקלטה" : "התחל הקלטה";
        }
    }
}
