using System.Collections;
using TMPro;
using UnityEngine;

public class TeacherVoiceRecorderUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text transcriptTopText;     // הטקסט שמופיע למעלה במסך
    public TMP_Text statusText;            // אופציונלי - אם אין לך, תשאירי ריק

    [Header("Recording")]
    public int sampleRate = 16000;
    public int maxSeconds = 15;

    private string micDevice;
    private AudioClip clip;
    private bool isRecording;
    private Coroutine fakeTranscribeRoutine;

    void Start()
    {
        // בוחר מיקרופון ראשון שקיים
        if (Microphone.devices != null && Microphone.devices.Length > 0)
        {
            micDevice = Microphone.devices[0];
            SetStatus("Ready");
        }
        else
        {
            micDevice = null;
            SetStatus("No microphone detected");
            if (transcriptTopText != null)
                transcriptTopText.text = "";
        }
    }

    public void StartRecording()
    {
        if (isRecording)
            return;

        if (string.IsNullOrEmpty(micDevice))
        {
            SetStatus("No microphone");
            return;
        }

        StopFakeTranscribeIfRunning();

        // התחלה נקייה
        clip = null;

        if (transcriptTopText != null)
            transcriptTopText.text = "Recording...";

        SetStatus("Recording");

        clip = Microphone.Start(micDevice, false, maxSeconds, sampleRate);
        isRecording = true;
    }

    public void StopRecording()
    {
        if (!isRecording)
            return;

        if (string.IsNullOrEmpty(micDevice))
            return;

        int pos = Microphone.GetPosition(micDevice);
        Microphone.End(micDevice);
        isRecording = false;

        if (clip == null || pos <= 0)
        {
            if (transcriptTopText != null)
                transcriptTopText.text = "No audio captured";

            SetStatus("No audio");
            return;
        }

        if (transcriptTopText != null)
            transcriptTopText.text = "Processing...";

        SetStatus("Processing");

        // כרגע דמה כדי לוודא שה-UI עובד.
        // בהמשך מחליפים לפונקציית תמלול אמיתית.
        fakeTranscribeRoutine = StartCoroutine(FakeTranscribe());
    }

    public void ResetRecording()
    {
        // עצירה אם מקליטים
        if (isRecording && !string.IsNullOrEmpty(micDevice))
        {
            Microphone.End(micDevice);
            isRecording = false;
        }

        StopFakeTranscribeIfRunning();

        clip = null;

        if (transcriptTopText != null)
            transcriptTopText.text = "";

        SetStatus("Reset");
    }

    private IEnumerator FakeTranscribe()
    {
        yield return new WaitForSeconds(1f);

        if (transcriptTopText != null)
            transcriptTopText.text = "Teacher said: please open your notebooks";

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
        if (statusText != null)
            statusText.text = msg;

        Debug.Log(msg);
    }
}
