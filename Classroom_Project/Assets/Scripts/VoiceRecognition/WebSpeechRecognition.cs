using UnityEngine;
using System;
using System.Runtime.InteropServices;

/// <summary>
/// Simple Web Speech API voice recognition for Unity WebGL.
/// 100% FREE, works in Chrome/Edge/Safari, no setup required!
/// 
/// This is a standalone implementation - no other providers needed.
/// Just build for WebGL and it works!
/// </summary>
public class WebSpeechRecognition : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Language code (e.g., 'en-US', 'he-IL', 'es-ES')")]
    public string language = "en-US";

    [Tooltip("Maximum recording time in seconds")]
    public int maxRecordingTime = 15;

    [Tooltip("Continuous recognition (keeps listening)")]
    public bool continuous = false;

    [Header("Status Display (Optional)")]
    public TMPro.TMP_Text statusText;
    public TMPro.TMP_Text transcriptText;

    // Events
    public event Action<string> OnTranscriptionReceived;
    public event Action<string> OnError;
    public event Action OnRecordingStarted;
    public event Action OnRecordingEnded;

    // Internal state
    private bool isRecording = false;
    private bool isInitialized = false;

    // Singleton
    public static WebSpeechRecognition Instance { get; private set; }

#if UNITY_WEBGL && !UNITY_EDITOR
    // Import JavaScript functions
    [DllImport("__Internal")]
    private static extern void InitWebSpeechRecognition(string language, bool continuous);

    [DllImport("__Internal")]
    private static extern void StartWebSpeechRecognition();

    [DllImport("__Internal")]
    private static extern void StopWebSpeechRecognition();

    [DllImport("__Internal")]
    private static extern bool IsWebSpeechRecognizing();
#endif

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Initialize();
    }

    /// <summary>
    /// Initialize Web Speech API
    /// </summary>
    public void Initialize()
    {
        if (isInitialized)
            return;

#if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            InitWebSpeechRecognition(language, continuous);
            isInitialized = true;
            SetStatus("Web Speech API initialized");
            Debug.Log($"Web Speech API initialized with language: {language}");
        }
        catch (Exception e)
        {
            SetStatus($"Failed to initialize: {e.Message}");
            Debug.LogError($"Web Speech API initialization failed: {e.Message}");
        }
#else
        SetStatus("Web Speech API only works in WebGL builds");
        Debug.LogWarning("Web Speech API only available in WebGL builds");
#endif
    }

    /// <summary>
    /// Start recording voice
    /// </summary>
    public void StartRecording()
    {
        if (isRecording)
        {
            Debug.LogWarning("Already recording");
            return;
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        if (!isInitialized)
        {
            Initialize();
        }

        try
        {
            StartWebSpeechRecognition();
            isRecording = true;
            SetStatus("Listening...");
            OnRecordingStarted?.Invoke();
            Debug.Log("Web Speech Recognition started");
        }
        catch (Exception e)
        {
            SetStatus($"Error: {e.Message}");
            Debug.LogError($"Failed to start recognition: {e.Message}");
            OnError?.Invoke(e.Message);
        }
#else
        SetStatus("Only works in WebGL");
        Debug.LogWarning("Web Speech API only available in WebGL builds. Build for WebGL to test.");
#endif
    }

    /// <summary>
    /// Stop recording voice
    /// </summary>
    public void StopRecording()
    {
        if (!isRecording)
        {
            Debug.LogWarning("Not currently recording");
            return;
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            StopWebSpeechRecognition();
            isRecording = false;
            SetStatus("Processing...");
            Debug.Log("Web Speech Recognition stopped");
        }
        catch (Exception e)
        {
            SetStatus($"Error: {e.Message}");
            Debug.LogError($"Failed to stop recognition: {e.Message}");
        }
#endif
    }

    /// <summary>
    /// Toggle recording on/off
    /// </summary>
    public void ToggleRecording()
    {
        if (isRecording)
            StopRecording();
        else
            StartRecording();
    }

    /// <summary>
    /// Check if currently recording
    /// </summary>
    public bool IsRecording()
    {
        return isRecording;
    }

    // ============================================================================
    // CALLBACKS FROM JAVASCRIPT
    // These methods are called by the JavaScript plugin
    // ============================================================================

    /// <summary>
    /// Called by JavaScript when transcription is received
    /// </summary>
    public void OnWebSpeechResult(string transcript)
    {
        Debug.Log($"Transcription received: {transcript}");
        
        if (transcriptText != null)
            transcriptText.text = transcript;

        SetStatus("Complete");
        OnTranscriptionReceived?.Invoke(transcript);
    }

    /// <summary>
    /// Called by JavaScript when an error occurs
    /// </summary>
    public void OnWebSpeechError(string error)
    {
        Debug.LogError($"Web Speech error: {error}");
        
        SetStatus($"Error: {error}");
        isRecording = false;
        OnError?.Invoke(error);
    }

    /// <summary>
    /// Called by JavaScript when recognition starts
    /// </summary>
    public void OnWebSpeechStart()
    {
        Debug.Log("Web Speech started");
        isRecording = true;
        OnRecordingStarted?.Invoke();
    }

    /// <summary>
    /// Called by JavaScript when recognition ends
    /// </summary>
    public void OnWebSpeechEnd()
    {
        Debug.Log("Web Speech ended");
        isRecording = false;
        OnRecordingEnded?.Invoke();
    }

    // ============================================================================
    // HELPER METHODS
    // ============================================================================

    void SetStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
        Debug.Log($"[WebSpeech] {message}");
    }

    // ============================================================================
    // EDITOR SIMULATION (for testing in Editor)
    // ============================================================================

#if UNITY_EDITOR
    private float editorRecordingStartTime;

    void Update()
    {
        // Simulate recording in Editor for testing
        if (isRecording && Time.time - editorRecordingStartTime > 3f)
        {
            // Simulate transcription after 3 seconds
            OnWebSpeechResult("Simulated transcription - build for WebGL to test real recognition");
            StopRecording();
        }
    }
#endif
}
