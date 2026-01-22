using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Generates AI responses for students based on teacher's questions and each student's emotional state.
/// Each student gets a unique response based on their emotion vector and personality traits.
/// </summary>
public class StudentAIResponseGenerator : MonoBehaviour
{
    [Header("AI Configuration")]
    [Tooltip("Use HuggingFace API for response generation")]
    public bool useHuggingFace = true;
    
    [Tooltip("HuggingFace API Token (optional - can leave empty if using free tier)")]
    public string huggingFaceToken = "";
    
    [Tooltip("Model for text generation (e.g., 'microsoft/DialoGPT-medium', 'gpt2')")]
    public string modelId = "gpt2"; // Can be changed to better models
    
    [Header("Settings")]
    [Tooltip("Enable to log AI responses to console")]
    public bool logResponses = true;
    
    [Tooltip("Maximum response length")]
    public int maxResponseLength = 100;
    
    [Tooltip("Default language for responses (Hebrew by default)")]
    public bool preferHebrew = true;

    // Cache for responses (to avoid regenerating same responses)
    private Dictionary<string, string> responseCache = new Dictionary<string, string>();

    /// <summary>
    /// Generate a unique AI response for a student based on teacher's question and student's emotional state
    /// </summary>
    public IEnumerator GenerateStudentResponse(
        StudentAgent student,
        string teacherQuestion,
        System.Action<string> onResponseGenerated)
    {
        if (string.IsNullOrEmpty(teacherQuestion))
        {
            onResponseGenerated?.Invoke("");
            yield break;
        }

        // Create a unique prompt based on student's emotional state and personality
        string prompt = CreateEmotionBasedPrompt(student, teacherQuestion);
        
        // Check cache first
        string cacheKey = $"{student.studentId}_{teacherQuestion.GetHashCode()}";
        if (responseCache.ContainsKey(cacheKey))
        {
            if (logResponses)
                Debug.Log($"[StudentAI] Cached response for {student.studentName}: {responseCache[cacheKey]}");
            onResponseGenerated?.Invoke(responseCache[cacheKey]);
            yield break;
        }

        if (useHuggingFace && !string.IsNullOrEmpty(huggingFaceToken))
        {
            // Use HuggingFace API for generation
            yield return StartCoroutine(GenerateWithHuggingFace(prompt, (response) =>
            {
                if (!string.IsNullOrEmpty(response))
                {
                    responseCache[cacheKey] = response;
                    onResponseGenerated?.Invoke(response);
                }
                else
                {
                    // Fallback to rule-based response
                    string fallback = GenerateFallbackResponse(student, teacherQuestion);
                    onResponseGenerated?.Invoke(fallback);
                }
            }));
        }
        else
        {
            // Use rule-based response generation (no API needed)
            string response = GenerateFallbackResponse(student, teacherQuestion);
            onResponseGenerated?.Invoke(response);
        }
    }

    /// <summary>
    /// Create a prompt that incorporates student's emotions and personality
    /// </summary>
    private string CreateEmotionBasedPrompt(StudentAgent student, string question)
    {
        StringBuilder prompt = new StringBuilder();
        
        // Base personality description
        prompt.Append($"You are a student named {student.studentName}. ");
        prompt.Append($"Your emotional state: ");
        
        // Add emotional context
        if (student.emotions.Happiness >= 7f)
            prompt.Append("very happy and enthusiastic. ");
        else if (student.emotions.Happiness <= 3f)
            prompt.Append("not very happy. ");
        
        if (student.emotions.Sadness >= 7f)
            prompt.Append("feeling sad or anxious. ");
        
        if (student.emotions.Anger >= 7f)
            prompt.Append("feeling angry or frustrated. ");
        
        if (student.emotions.Boredom >= 7f)
            prompt.Append("very bored and disinterested. ");
        
        if (student.emotions.Frustration >= 7f)
            prompt.Append("frustrated and struggling. ");
        
        // Add personality traits
        if (student.extroversion > 0.7f)
            prompt.Append("You are outgoing and confident. ");
        else if (student.extroversion < 0.3f)
            prompt.Append("You are shy and reserved. ");
        
        if (student.academicMotivation > 0.7f)
            prompt.Append("You are motivated to learn. ");
        else if (student.academicMotivation < 0.3f)
            prompt.Append("You lack motivation. ");
        
        if (student.rebelliousness > 0.7f)
            prompt.Append("You tend to be rebellious. ");
        
        // Add the question
        prompt.Append($"The teacher asks: \"{question}\" ");
        
        // Prefer Hebrew by default, but respond in the language of the question
        bool isHebrewQuestion = ContainsHebrew(question);
        if (isHebrewQuestion || preferHebrew)
        {
            prompt.Append("Respond naturally as this student would, in Hebrew. ");
        }
        else
        {
            prompt.Append("Respond naturally as this student would, in the same language as the question. ");
        }
        prompt.Append("Keep the response short and realistic (1-2 sentences max).");
        
        return prompt.ToString();
    }

    /// <summary>
    /// Generate response using HuggingFace API
    /// </summary>
    private IEnumerator GenerateWithHuggingFace(string prompt, System.Action<string> onComplete)
    {
        string apiUrl = $"https://api-inference.huggingface.co/models/{modelId}";
        
        // Create JSON payload
        string jsonPayload = $"{{\"inputs\": \"{EscapeJson(prompt)}\", \"parameters\": {{\"max_length\": {maxResponseLength}, \"return_full_text\": false}}}}";
        
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
        
        using (var www = UnityEngine.Networking.UnityWebRequest.PostWwwForm(apiUrl, ""))
        {
            www.uploadHandler = new UnityEngine.Networking.UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            
            if (!string.IsNullOrEmpty(huggingFaceToken))
            {
                www.SetRequestHeader("Authorization", $"Bearer {huggingFaceToken}");
            }
            
            yield return www.SendWebRequest();
            
            if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                string response = ParseHuggingFaceResponse(www.downloadHandler.text);
                onComplete?.Invoke(response);
            }
            else
            {
                Debug.LogWarning($"[StudentAI] HuggingFace API error: {www.error}");
                onComplete?.Invoke("");
            }
        }
    }

    /// <summary>
    /// Generate fallback response using rule-based system (no API needed)
    /// </summary>
    private string GenerateFallbackResponse(StudentAgent student, string question)
    {
        string lowerQuestion = question.ToLower();
        
        // Determine response style based on emotions
        bool isHappy = student.emotions.Happiness >= 7f;
        bool isSad = student.emotions.Sadness >= 7f;
        bool isAngry = student.emotions.Anger >= 7f;
        bool isBored = student.emotions.Boredom >= 7f;
        bool isFrustrated = student.emotions.Frustration >= 7f;
        bool isShy = student.extroversion < 0.3f;
        bool isMotivated = student.academicMotivation >= 0.7f;
        
        // Check if question is in Hebrew
        bool isHebrew = ContainsHebrew(question);
        
        List<string> responses = new List<string>();
        
        // Generate responses based on emotional state
        // Default to Hebrew if preferHebrew is true, otherwise use question language
        bool useHebrew = preferHebrew || isHebrew;
        
        if (isHappy && isMotivated)
        {
            if (useHebrew)
                responses.AddRange(new[] { "אני חושב שהתשובה היא...", "כן, אני יודע!", "בואו ננסה יחד!", "אני יכול לענות על זה!", "זה קל!" });
            else
                responses.AddRange(new[] { "I think the answer is...", "Yes, I know this!", "Let's try together!" });
        }
        else if (isBored)
        {
            if (useHebrew)
                responses.AddRange(new[] { "אה... אולי?", "לא בטוח.", "לא יודע בדיוק.", "זה משעמם...", "לא מעניין אותי." });
            else
                responses.AddRange(new[] { "Uh... maybe?", "Not sure.", "Don't really know." });
        }
        else if (isSad || isFrustrated)
        {
            if (useHebrew)
                responses.AddRange(new[] { "אני לא בטוח.", "קשה לי להבין.", "לא חושב שאני יודע.", "זה קשה מדי.", "אני לא מבין." });
            else
                responses.AddRange(new[] { "I'm not sure.", "It's hard for me.", "Don't think I know." });
        }
        else if (isAngry)
        {
            if (useHebrew)
                responses.AddRange(new[] { "אני לא רוצה לענות.", "לא אכפת לי.", "למה אני?", "תשאלו מישהו אחר.", "לא!" });
            else
                responses.AddRange(new[] { "I don't want to answer.", "Don't care.", "Why me?" });
        }
        else if (isShy)
        {
            if (useHebrew)
                responses.AddRange(new[] { "אה... אולי...", "אני לא בטוח...", "יכול להיות...", "אני חושב ש...", "אולי..." });
            else
                responses.AddRange(new[] { "Um... maybe...", "I'm not sure...", "Could be..." });
        }
        else
        {
            // Neutral responses - more variety for Hebrew
            if (useHebrew)
                responses.AddRange(new[] { "אני חושב ש...", "נראה לי ש...", "יכול להיות ש...", "אולי התשובה היא...", "אני מאמין ש...", "זה נראה כמו..." });
            else
                responses.AddRange(new[] { "I think...", "It seems like...", "Could be that..." });
        }
        
        // Select random response
        if (responses.Count > 0)
        {
            string response = responses[Random.Range(0, responses.Count)];
            
            if (logResponses)
                Debug.Log($"[StudentAI] {student.studentName} says: {response}");
            
            return response;
        }
        
        return isHebrew ? "לא יודע" : "I don't know";
    }

    /// <summary>
    /// Parse HuggingFace API response
    /// </summary>
    private string ParseHuggingFaceResponse(string jsonResponse)
    {
        try
        {
            // Simple JSON parsing - extract generated text
            if (jsonResponse.Contains("\"generated_text\""))
            {
                int start = jsonResponse.IndexOf("\"generated_text\"") + 17;
                int end = jsonResponse.IndexOf("\"", start + 1);
                if (end > start)
                {
                    string text = jsonResponse.Substring(start, end - start);
                    // Clean up the response
                    text = text.Replace("\\n", " ").Replace("\\r", " ").Trim();
                    if (text.Length > maxResponseLength)
                        text = text.Substring(0, maxResponseLength);
                    return text;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[StudentAI] Failed to parse response: {e.Message}");
        }
        
        return "";
    }

    private string EscapeJson(string text)
    {
        return text.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");
    }

    private bool ContainsHebrew(string text)
    {
        foreach (char c in text)
        {
            if (c >= 0x0590 && c <= 0x05FF) // Hebrew Unicode range
                return true;
        }
        return false;
    }

    /// <summary>
    /// Clear response cache
    /// </summary>
    public void ClearCache()
    {
        responseCache.Clear();
    }
}
