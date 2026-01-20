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

    [Header("Settings")]
    public bool logCommands = true;

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

        // Check for student name mentions
        StudentAgent targetStudent = DetectStudentMention(lower);

        // Try to match command
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

        // Show feedback
        if (teacherUI != null)
        {
            teacherUI.ShowFeedback($"Voice: {transcript}", Color.cyan);
        }
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
