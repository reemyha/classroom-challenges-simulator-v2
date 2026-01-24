using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Processes voice commands from Web Speech API and triggers classroom actions.
/// This connects the voice recognition to your existing classroom management system.
/// </summary>
public class WebSpeechClassroomIntegration : MonoBehaviour
{
    [Header("References")]
    public WebSpeechRecognition webSpeech;
    public ClassroomManager classroomManager;
    public TeacherUI teacherUI;
    public StudentAIResponseGenerator aiResponseGenerator;

    [Header("Settings")]
    public bool logCommands = true;
    public bool enableQuestionDetection = true; // Enable students to respond to questions
    [Tooltip("Auto-start voice recognition when scene loads (requires user permission)")]
    public bool autoStartVoiceRecognition = false;
    [Tooltip("Allow multiple students to respond with full answers (not just eager bubbles)")]
    public bool allowMultipleResponders = true;
    [Tooltip("Maximum number of students that can respond to a classwide question")]
    [Range(1, 10)]
    public int maxResponders = 4;
    [Tooltip("Delay between each student's response (in seconds)")]
    public float responseDelayBetweenStudents = 0.5f;

    [Header("Debug")]
    [Tooltip("Press this key to trigger a test question (for debugging bubbles without voice)")]
    public KeyCode debugQuestionKey = KeyCode.T;
    [Tooltip("Test question to ask when debug key is pressed")]
    public string debugTestQuestion = "מה שלומכם כיתה? איך אתם היום?";

    // Command mappings
    private Dictionary<string[], System.Action> commandActions;

    void Start()
    {
        // Find references if not assigned
        if (webSpeech == null)
            webSpeech = FindObjectOfType<WebSpeechRecognition>();

        if (classroomManager == null)
            classroomManager = FindObjectOfType<ClassroomManager>();

        if (teacherUI == null)
            teacherUI = FindObjectOfType<TeacherUI>();

        if (aiResponseGenerator == null)
            aiResponseGenerator = FindObjectOfType<StudentAIResponseGenerator>();

        // Subscribe to transcription events
        if (webSpeech != null)
        {
            webSpeech.OnTranscriptionReceived += ProcessVoiceCommand;
        }
        else
        {
            Debug.LogError("WebSpeechRecognition not found!");
        }

        // Initialize command mappings
        InitializeCommands();
        
        // Auto-start voice recognition if enabled
        if (autoStartVoiceRecognition && webSpeech != null)
        {
            // Delay slightly to ensure everything is initialized
            Invoke(nameof(StartVoiceRecognition), 1f);
        }
    }
    
    /// <summary>
    /// Start voice recognition (can be called manually or auto-started)
    /// </summary>
    public void StartVoiceRecognition()
    {
        if (webSpeech != null && !webSpeech.IsRecording())
        {
            webSpeech.StartRecording();
            if (logCommands)
                Debug.Log("[VoiceCommand] Voice recognition started automatically");
        }
    }

    void Update()
    {
        // Debug: Press T (or configured key) to trigger a test question
        if (Input.GetKeyDown(debugQuestionKey))
        {
            Debug.Log($"[DEBUG] Manually triggering test question: '{debugTestQuestion}'");

            // Show feedback like voice would
            if (teacherUI != null)
            {
                teacherUI.ShowFeedback($"המורה אמר: {debugTestQuestion}", Color.cyan);
            }

            // Trigger the question processing
            ProcessTeacherQuestion(debugTestQuestion, null);
        }
    }

    void OnDestroy()
    {
        if (webSpeech != null)
        {
            webSpeech.OnTranscriptionReceived -= ProcessVoiceCommand;
        }
    }

    void InitializeCommands()
    {
        commandActions = new Dictionary<string[], System.Action>
        {
            // English Praise commands
            { new[] { "good job", "well done", "excellent", "great work", "nice" }, 
              () => ExecuteAction(ActionType.Praise) },

            // Hebrew praise commands
            { new[] { "כל הכבוד", "יפה מאוד", "מצוין" }, 
              () => ExecuteAction(ActionType.Praise) },

            // English Discipline commands
            { new[] { "quiet", "silence", "stop talking", "be quiet", "shh", "settle down" }, 
              () => ExecuteAction(ActionType.Yell) },

            // Hebrew discipline commands
            { new[] { "שקט", "תשקטו", "די" }, 
              () => ExecuteAction(ActionType.Yell) },

            // English Instruction commands
            { new[] { "board", "come to board", "go to board", "write on board" }, 
              () => ExecuteAction(ActionType.CallToBoard) },

            // Hebrew instruction commands
            { new[] { "בוא ללוח" }, 
              () => ExecuteAction(ActionType.CallToBoard) },

            // Hebrew break command
            { new[] { "הפסקה" }, 
              () => ExecuteAction(ActionType.GiveBreak) },

            // Hebrew book command
            { new[] { "פתחו ספרים", "ספרים" }, 
              () => ExecuteBagItem(BagItemType.Book) },

            { new[] { "change seat", "move", "switch seats", "new seat" }, 
              () => ExecuteAction(ActionType.ChangeSeating) },

            { new[] { "break", "take a break", "rest", "relax", "five minutes" }, 
              () => ExecuteAction(ActionType.GiveBreak) },

            { new[] { "leave", "go outside", "time out", "step out" }, 
              () => ExecuteAction(ActionType.RemoveFromClass) },

            // Bag items
            { new[] { "ruler" }, 
              () => ExecuteBagItem(BagItemType.Ruler) },

            { new[] { "game", "play", "activity" }, 
              () => ExecuteBagItem(BagItemType.Game) },

            { new[] { "book", "read", "open book", "books" }, 
              () => ExecuteBagItem(BagItemType.Book) },

            { new[] { "music", "song", "listen" }, 
              () => ExecuteBagItem(BagItemType.Music) }
        };
    }

    void ProcessVoiceCommand(string transcript)
    {
        if (string.IsNullOrEmpty(transcript))
            return;

        string lower = transcript.ToLower().Trim();

        if (logCommands)
            Debug.Log($"[VoiceCommand] Processing: '{transcript}'");

        // Check for student name mentions FIRST
        StudentAgent targetStudent = DetectStudentMention(lower);

        // Check for student-specific commands (name + action)
        if (targetStudent != null)
        {
            bool handledSpecificCommand = ProcessStudentSpecificCommand(lower, transcript, targetStudent);
            if (handledSpecificCommand)
            {
                // Show feedback
                if (teacherUI != null)
                {
                    teacherUI.ShowFeedback($"Voice: {transcript}", Color.cyan);
                }
                return; // Command was handled for specific student
            }
        }

        // Try to match general command
        bool commandFound = false;
        foreach (var kvp in commandActions)
        {
            foreach (string keyword in kvp.Key)
            {
                if (lower.Contains(keyword))
                {
                    if (logCommands)
                        Debug.Log($"[VoiceCommand] Matched keyword: '{keyword}'");

                    kvp.Value?.Invoke();
                    commandFound = true;
                    break;
                }
            }
            if (commandFound) break;
        }

        // Check for complex commands
        if (!commandFound)
        {
            ProcessComplexCommand(lower, targetStudent);
        }

        // Check if teacher asked a question - make students respond
        if (enableQuestionDetection && IsQuestion(transcript))
        {
            ProcessTeacherQuestion(transcript, targetStudent);
        }

        // Show feedback
        if (teacherUI != null)
        {
            teacherUI.ShowFeedback($"Voice: {transcript}", Color.cyan);
        }
    }

    /// <summary>
    /// Process commands directed at a specific student by name
    /// Returns true if a command was handled
    /// </summary>
    private bool ProcessStudentSpecificCommand(string lowerText, string originalText, StudentAgent student)
    {
        // Discipline/Yell keywords (Hebrew and English)
        string[] disciplineKeywords = {
            "די", "תפסיק", "תשתוק", "שקט", "מספיק", "תירגע", "הפסק", "אל",
            "stop", "quiet", "enough", "be quiet", "stop talking", "settle down"
        };

        // Praise keywords (Hebrew and English)
        string[] praiseKeywords = {
            "כל הכבוד", "יפה מאוד", "מצוין", "נהדר", "מעולה", "יופי", "טוב מאוד", "בראבו",
            "good job", "well done", "excellent", "great", "amazing", "perfect", "bravo"
        };

        // Check for discipline command on specific student
        foreach (string keyword in disciplineKeywords)
        {
            if (lowerText.Contains(keyword))
            {
                ExecuteActionOnStudent(ActionType.Yell, student);
                // Show student's hurt reaction in bubble
                ShowStudentEmotionalReaction(student, "discipline");
                if (logCommands)
                    Debug.Log($"[VoiceCommand] Disciplined {student.studentName} - student feels hurt");
                return true;
            }
        }

        // Check for praise command on specific student
        foreach (string keyword in praiseKeywords)
        {
            if (lowerText.Contains(keyword))
            {
                ExecuteActionOnStudent(ActionType.Praise, student);
                // Show student's happy reaction in bubble
                ShowStudentEmotionalReaction(student, "praise");
                if (logCommands)
                    Debug.Log($"[VoiceCommand] Praised {student.studentName} - student feels happy");
                return true;
            }
        }

        return false; // No specific command found
    }

    /// <summary>
    /// Execute an action on a specific student
    /// </summary>
    private void ExecuteActionOnStudent(ActionType actionType, StudentAgent student)
    {
        if (classroomManager == null || student == null)
            return;

        TeacherAction action = new TeacherAction
        {
            Type = actionType,
            TargetStudentId = student.studentId,
            Context = $"Voice: {actionType} on {student.studentName}"
        };

        // Apply the action through classroom manager
        classroomManager.ExecuteTeacherAction(action);

        // Also apply emotional effect directly for enhanced reaction
        float intensity = 1.5f; // Stronger effect when called out by name
        student.emotions.ApplyTeacherAction(action, intensity);
    }

    /// <summary>
    /// Show student's emotional reaction in their speech bubble
    /// </summary>
    private void ShowStudentEmotionalReaction(StudentAgent student, string reactionType)
    {
        var bubble = student.GetComponentInChildren<StudentResponseBubble>();
        if (bubble == null) return;

        string reaction = "";

        if (reactionType == "discipline")
        {
            // Hurt/offended reactions based on personality
            if (student.sensitivity > 0.7f)
            {
                // Very sensitive - gets very hurt
                string[] sensitiveReactions = { "...", "למה אני?", "אני לא עשיתי כלום", "זה לא הוגן" };
                reaction = sensitiveReactions[Random.Range(0, sensitiveReactions.Length)];
            }
            else if (student.rebelliousness > 0.7f)
            {
                // Rebellious - gets angry
                string[] rebelliousReactions = { "מה?!", "אני לא עשיתי כלום!", "למה רק אני?!", "לא הוגן!" };
                reaction = rebelliousReactions[Random.Range(0, rebelliousReactions.Length)];
            }
            else
            {
                // Normal reaction
                string[] normalReactions = { "סליחה...", "בסדר...", "...", "טוב טוב" };
                reaction = normalReactions[Random.Range(0, normalReactions.Length)];
            }
        }
        else if (reactionType == "praise")
        {
            // Happy reactions based on personality
            if (student.extroversion > 0.7f)
            {
                // Extroverted - very expressive
                string[] extrovertReactions = { "תודה!", "יש!", "מעולה!", "אני הכי!", "יאללה!" };
                reaction = extrovertReactions[Random.Range(0, extrovertReactions.Length)];
            }
            else if (student.extroversion < 0.3f)
            {
                // Introverted - shy reaction
                string[] introvertReactions = { "תודה...", "...", "אה... תודה", "☺" };
                reaction = introvertReactions[Random.Range(0, introvertReactions.Length)];
            }
            else
            {
                // Normal reaction
                string[] normalReactions = { "תודה!", "יופי!", "כיף!", "תודה המורה" };
                reaction = normalReactions[Random.Range(0, normalReactions.Length)];
            }
        }

        if (!string.IsNullOrEmpty(reaction))
        {
            bubble.ShowResponse(reaction);
        }
    }

    /// <summary>
    /// Detect if the teacher's speech is a question
    /// </summary>
    private bool IsQuestion(string text)
    {
        if (string.IsNullOrEmpty(text))
            return false;
            
        string lower = text.ToLower().Trim();
        
        // Check for question words (English and Hebrew)
        string[] questionIndicators = {
            "what", "who", "when", "where", "why", "how", "can", "could", "would", "should", "do", "does", "did",
            "מה", "מי", "מתי", "איפה", "למה", "איך", "האם", "יכול", "רוצה", "איזה", "איך", "למה", "מי", "מה"
        };
        
        foreach (string indicator in questionIndicators)
        {
            if (lower.Contains(indicator))
                return true;
        }
        
        // Check for question marks (if transcribed)
        if (text.Contains("?") || text.EndsWith("?"))
            return true;
        
        // Check for question patterns in Hebrew (more comprehensive)
        if (ContainsHebrew(text))
        {
            // Hebrew question words
            if (lower.Contains("איזה") || lower.Contains("מי") || lower.Contains("מה") || 
                lower.Contains("למה") || lower.Contains("איך") || lower.Contains("מתי") ||
                lower.Contains("איפה") || lower.Contains("האם") || lower.Contains("כמה"))
                return true;
            
            // Hebrew question patterns
            if (lower.Contains("תוכל") || lower.Contains("תוכלי") || lower.Contains("תוכלו") ||
                lower.Contains("תגיד") || lower.Contains("תגידי") || lower.Contains("תגידו") ||
                lower.Contains("תסביר") || lower.Contains("תסבירי") || lower.Contains("תסבירו"))
                return true;
        }
        
        // Check for rising intonation patterns (common in questions)
        // Questions often end with certain words
        string[] questionEndings = { "?", "נכון", "כן", "לא", "right", "yes", "no" };
        foreach (string ending in questionEndings)
        {
            if (lower.EndsWith(ending) || lower.Contains(" " + ending))
                return true;
        }
        
        return false;
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
    /// Process teacher's question and make students respond with AI-generated answers
    /// Uses the new StudentQuestionResponder system for progressive interest
    /// </summary>
    private void ProcessTeacherQuestion(string question, StudentAgent targetStudent)
    {
        if (classroomManager == null || classroomManager.activeStudents == null)
            return;

        if (logCommands)
            Debug.Log($"[VoiceCommand] Teacher asked a question: '{question}'");

        // If specific student is mentioned, only that student responds
        if (targetStudent != null)
        {
            TriggerStudentResponse(targetStudent, question);
            if (logCommands)
                Debug.Log($"[VoiceCommand] {targetStudent.studentName} is responding to question");
        }
        else
        {
            // Classwide question - trigger all students' question responders
            // Each student will independently decide whether to show eagerness
            int eagerStudentCount = 0;
            List<StudentAgent> respondingStudents = new List<StudentAgent>();

            foreach (var student in classroomManager.activeStudents)
            {
                if (student == null)
                    continue;

                // Get or add StudentQuestionResponder component
                StudentQuestionResponder responder = student.GetComponent<StudentQuestionResponder>();
                if (responder == null)
                {
                    responder = student.gameObject.AddComponent<StudentQuestionResponder>();

                    // CRITICAL: Immediately initialize references since Start() won't be called until next frame
                    // This ensures responseBubble and other references are set before OnQuestionAsked is called
                    if (logCommands)
                        Debug.Log($"[VoiceCommand] Added StudentQuestionResponder to {student.studentName}");
                }

                // Trigger the question for this student
                // The responder will decide if and how to show eagerness
                // Note: InitializeReferences() is now called in OnQuestionAsked() to handle dynamic component addition
                responder.OnQuestionAsked(question);

                // Count how many students are showing eagerness
                if (responder.HasAnswerReady())
                {
                    eagerStudentCount++;
                    respondingStudents.Add(student);
                }
            }

            if (logCommands)
                Debug.Log($"[VoiceCommand] {eagerStudentCount} student(s) are eager to answer");

            // If multiple responders allowed, trigger full responses from multiple students
            if (allowMultipleResponders && respondingStudents.Count > 0)
            {
                // Shuffle the list and take up to maxResponders
                ShuffleList(respondingStudents);
                int respondersToTrigger = Mathf.Min(respondingStudents.Count, maxResponders);

                for (int i = 0; i < respondersToTrigger; i++)
                {
                    float delay = i * responseDelayBetweenStudents;
                    StartCoroutine(DelayedStudentResponse(respondingStudents[i], question, delay));
                }

                if (logCommands)
                    Debug.Log($"[VoiceCommand] Triggered full responses from {respondersToTrigger} students");
            }
        }
    }

    private System.Collections.IEnumerator DelayedStudentResponse(StudentAgent student, string question, float delay)
    {
        yield return new WaitForSeconds(delay);
        TriggerStudentResponse(student, question);
    }

    private void TriggerStudentResponse(StudentAgent student, string question)
    {
        if (student == null || string.IsNullOrWhiteSpace(question))
            return;

        if (aiResponseGenerator == null)
            aiResponseGenerator = FindObjectOfType<StudentAIResponseGenerator>();

        if (aiResponseGenerator == null)
        {
            if (logCommands)
                Debug.LogWarning("[VoiceCommand] StudentAIResponseGenerator not found; cannot generate student response.");
            return;
        }

        StartCoroutine(aiResponseGenerator.GenerateStudentResponse(student, question, (response) =>
        {
            if (string.IsNullOrWhiteSpace(response))
                return;

            var bubble = student.GetComponentInChildren<StudentResponseBubble>();
            if (bubble != null)
            {
                bubble.ShowResponse(response);
            }
            else if (logCommands)
            {
                Debug.LogWarning($"[VoiceCommand] StudentResponseBubble not found for {student.studentName}. Response was: {response}");
            }
        }));
    }

    StudentAgent DetectStudentMention(string text)
    {
        if (classroomManager == null || classroomManager.activeStudents == null)
            return null;

        // Check each student name
        foreach (var student in classroomManager.activeStudents)
        {
            string studentName = student.studentName.ToLower();
            string firstName = studentName.Split(' ')[0];
            
            if (text.Contains(firstName))
            {
                if (logCommands)
                    Debug.Log($"[VoiceCommand] Detected student: {student.studentName}");
                return student;
            }
        }

        // Use currently selected student
        if (teacherUI != null)
        {
            return teacherUI.GetSelectedStudent();
        }

        return null;
    }

    void ExecuteAction(ActionType actionType)
    {
        if (classroomManager == null)
            return;

        StudentAgent targetStudent = teacherUI?.GetSelectedStudent();

        if (targetStudent != null)
        {
            // Execute on specific student
            TeacherAction action = new TeacherAction
            {
                Type = actionType,
                TargetStudentId = targetStudent.studentId,
                Context = $"Voice: {actionType} on {targetStudent.studentName}"
            };
            classroomManager.ExecuteTeacherAction(action);

            if (logCommands)
                Debug.Log($"[VoiceCommand] {actionType} on {targetStudent.studentName}");
        }
        else
        {
            // Execute classwide
            classroomManager.ExecuteClasswideAction(actionType, $"Voice: {actionType}");

            if (logCommands)
                Debug.Log($"[VoiceCommand] {actionType} classwide");
        }
    }

    void ExecuteBagItem(BagItemType itemType)
    {
        if (classroomManager == null)
            return;

        StudentAgent targetStudent = teacherUI?.GetSelectedStudent();

        if (targetStudent != null)
        {
            classroomManager.ExecuteBagItemOnStudent(itemType, targetStudent);
            if (logCommands)
                Debug.Log($"[VoiceCommand] Used {itemType} on {targetStudent.studentName}");
        }
        else
        {
            classroomManager.ExecuteBagItem(itemType);
            if (logCommands)
                Debug.Log($"[VoiceCommand] Used {itemType} classwide");
        }
    }

    /// <summary>
    /// Shuffle a list using Fisher-Yates algorithm
    /// </summary>
    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    void ProcessComplexCommand(string text, StudentAgent targetStudent)
    {
        // Handle "everyone" commands
        if (text.Contains("everyone") || text.Contains("class"))
        {
            if (text.Contains("quiet") || text.Contains("attention"))
            {
                ExecuteAction(ActionType.Yell);
                return;
            }

            if (text.Contains("break"))
            {
                ExecuteAction(ActionType.GiveBreak);
                return;
            }

            if (text.Contains("book"))
            {
                ExecuteBagItem(BagItemType.Book);
                return;
            }
        }

        // No match
        if (logCommands)
            Debug.Log($"[VoiceCommand] No action matched for: '{text}'");
    }
}
