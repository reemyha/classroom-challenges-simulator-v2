# סיכום השינויים - Classroom Simulator

## 1. StudentResponseBubble.cs - הקטנת בועות התגובה

```csharp
// שינויים בגדלים:
[Header("Sizing")]
public float minWidth = 60f;           // היה: 100f
public float maxWidth = 150f;          // היה: 250f
public Vector2 textPadding = new Vector2(6f, 4f);  // היה: (10f, 8f)
public float eagerBubbleWidth = 50f;   // היה: 80f
public float answerBubbleMaxWidth = 180f;  // היה: 300f

[Header("Styling")]
public int fontSize = 11;              // היה: 14
public int eagerFontSize = 12;         // היה: 16

// גובה מינימלי:
float height = Mathf.Max(24f, textHeight + textPadding.y * 2);  // היה: 35f
float height = Mathf.Max(20f, textHeight + textPadding.y);      // היה: 30f (eager)
```

---

## 2. WebSpeechClassroomIntegration.cs - תגובות דינמיות

### הגדרות חדשות:
```csharp
[Header("Settings")]
public bool allowMultipleResponders = true;
[Range(1, 10)]
public int maxResponders = 4;
public float responseDelayBetweenStudents = 0.5f;
```

### פונקציה חדשה - זיהוי פקודות לתלמיד ספציפי:
```csharp
private bool ProcessStudentSpecificCommand(string lowerText, string originalText, StudentAgent student)
{
    // מילות משמעת (עברית ואנגלית)
    string[] disciplineKeywords = {
        "די", "תפסיק", "תשתוק", "שקט", "מספיק", "תירגע", "הפסק", "אל",
        "stop", "quiet", "enough", "be quiet", "stop talking", "settle down"
    };

    // מילות שבח (עברית ואנגלית)
    string[] praiseKeywords = {
        "כל הכבוד", "יפה מאוד", "מצוין", "נהדר", "מעולה", "יופי", "טוב מאוד", "בראבו",
        "good job", "well done", "excellent", "great", "amazing", "perfect", "bravo"
    };

    // בדיקת פקודת משמעת על תלמיד ספציפי
    foreach (string keyword in disciplineKeywords)
    {
        if (lowerText.Contains(keyword))
        {
            ExecuteActionOnStudent(ActionType.Yell, student);
            ShowStudentEmotionalReaction(student, "discipline");
            return true;
        }
    }

    // בדיקת פקודת שבח על תלמיד ספציפי
    foreach (string keyword in praiseKeywords)
    {
        if (lowerText.Contains(keyword))
        {
            ExecuteActionOnStudent(ActionType.Praise, student);
            ShowStudentEmotionalReaction(student, "praise");
            return true;
        }
    }

    return false;
}
```

### פונקציה חדשה - ביצוע פעולה על תלמיד ספציפי:
```csharp
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

    classroomManager.ExecuteTeacherAction(action);

    // אפקט רגשי חזק יותר כשקוראים בשם
    float intensity = 1.5f;
    student.emotions.ApplyTeacherAction(action, intensity);
}
```

### פונקציה חדשה - תגובה רגשית בבועה:
```csharp
private void ShowStudentEmotionalReaction(StudentAgent student, string reactionType)
{
    var bubble = student.GetComponentInChildren<StudentResponseBubble>();
    if (bubble == null) return;

    string reaction = "";

    if (reactionType == "discipline")
    {
        // תגובות פגיעה לפי אישיות
        if (student.sensitivity > 0.7f)
        {
            // רגיש מאוד - נפגע מאוד
            string[] sensitiveReactions = { "...", "למה אני?", "אני לא עשיתי כלום", "זה לא הוגן" };
            reaction = sensitiveReactions[Random.Range(0, sensitiveReactions.Length)];
        }
        else if (student.rebelliousness > 0.7f)
        {
            // מורד - כועס
            string[] rebelliousReactions = { "מה?!", "אני לא עשיתי כלום!", "למה רק אני?!", "לא הוגן!" };
            reaction = rebelliousReactions[Random.Range(0, rebelliousReactions.Length)];
        }
        else
        {
            // תגובה רגילה
            string[] normalReactions = { "סליחה...", "בסדר...", "...", "טוב טוב" };
            reaction = normalReactions[Random.Range(0, normalReactions.Length)];
        }
    }
    else if (reactionType == "praise")
    {
        // תגובות שמחה לפי אישיות
        if (student.extroversion > 0.7f)
        {
            // מוחצן - מבטא שמחה
            string[] extrovertReactions = { "תודה!", "יש!", "מעולה!", "אני הכי!", "יאללה!" };
            reaction = extrovertReactions[Random.Range(0, extrovertReactions.Length)];
        }
        else if (student.extroversion < 0.3f)
        {
            // מופנם - ביישן
            string[] introvertReactions = { "תודה...", "...", "אה... תודה", "☺" };
            reaction = introvertReactions[Random.Range(0, introvertReactions.Length)];
        }
        else
        {
            // תגובה רגילה
            string[] normalReactions = { "תודה!", "יופי!", "כיף!", "תודה המורה" };
            reaction = normalReactions[Random.Range(0, normalReactions.Length)];
        }
    }

    if (!string.IsNullOrEmpty(reaction))
    {
        bubble.ShowResponse(reaction);
    }
}
```

### שינוי ב-ProcessTeacherQuestion - מספר תלמידים עונים:
```csharp
// אם מותר מספר מגיבים, הפעל תגובות מלאות ממספר תלמידים
if (allowMultipleResponders && respondingStudents.Count > 0)
{
    ShuffleList(respondingStudents);
    int respondersToTrigger = Mathf.Min(respondingStudents.Count, maxResponders);

    for (int i = 0; i < respondersToTrigger; i++)
    {
        float delay = i * responseDelayBetweenStudents;
        StartCoroutine(DelayedStudentResponse(respondingStudents[i], question, delay));
    }
}
```

---

## 3. StudentAIResponseGenerator.cs - שאלות ותגובות לפי מצב רוח

### פונקציה חדשה - זיהוי שאלות על הרגשה:
```csharp
private bool IsFeelingQuestion(string question)
{
    string[] feelingIndicators = {
        // עברית
        "מרגישים", "מרגיש", "שלומכם", "שלומך", "שלום", "איך אתם", "איך את", "מה נשמע",
        "מה קורה", "מה המצב", "הכל בסדר", "בסדר", "מה איתכם", "מה איתך",
        // אנגלית
        "how are you", "how do you feel", "feeling", "how's everyone", "what's up", "how's it going"
    };

    foreach (string indicator in feelingIndicators)
    {
        if (question.Contains(indicator))
            return true;
    }
    return false;
}
```

### פונקציה חדשה - תגובות לפי מצב רוח:
```csharp
private string GenerateFeelingResponse(StudentAgent student, bool useHebrew,
    bool isHappy, bool isSad, bool isAngry, bool isBored, bool isFrustrated, bool isNeutral)
{
    List<string> responses = new List<string>();

    if (useHebrew)
    {
        if (isHappy)
        {
            responses.AddRange(new[] {
                "בסדר המורה, איך את?",
                "מעולה!",
                "טוב מאוד!",
                "סבבה!",
                "הכל טוב!",
                "בסדר גמור!"
            });
        }
        else if (isNeutral)
        {
            responses.AddRange(new[] {
                "בסדר",
                "ככה ככה",
                "בסדר גמור",
                "סבבה",
                "הכל טוב"
            });
        }
        else if (isSad)
        {
            responses.AddRange(new[] {
                "לא טוב",
                "לא כל כך טוב...",
                "ככה...",
                "יכול להיות יותר טוב",
                "לא משהו"
            });
        }
        else if (isFrustrated || isBored)
        {
            responses.AddRange(new[] {
                "משעמם לנו",
                "משעמם...",
                "כבר רוצים הפסקה",
                "מתי הפסקה?",
                "עייפים...",
                "ממש משעמם"
            });
        }
        else if (isAngry)
        {
            responses.AddRange(new[] {
                "לא טוב",
                "לא בכיף",
                "רע",
                "עצבני היום",
                "לא במצב רוח"
            });
        }
    }
    // ... English responses ...

    return responses[Random.Range(0, responses.Count)];
}
```

### שאלות גנריות נוספות:
```csharp
private string CheckGenericQuestions(string question, StudentAgent student, bool useHebrew)
{
    // "הבנתם?" / "Did you understand?"
    // "מוכנים?" / "Ready?"
    // "יש שאלות?" / "Any questions?"
    // "רוצים הפסקה?" / "Want a break?"
    // "מי רוצה לענות?" / "Who wants to answer?"
    // "מה למדתם היום?" / "What did you learn today?"
    // "בוקר טוב" / "Good morning"
    // "עשיתם שיעורי בית?" / "Did you do homework?"
    // "מי מדבר?" / "Who's talking?"
    // "אתם מקשיבים?" / "Are you listening?"
    // "אתם אוהבים?" / "Do you like it?"
    // "מה התשובה?" / "What's the answer?"
    // "סיימתם?" / "Finished?"
    // "צריכים עזרה?" / "Do you need help?"

    // כל שאלה מחזירה תגובות שונות לפי מצב רוח ואישיות התלמיד
}
```

---

## דוגמאות שימוש:

### משמעת לתלמיד ספציפי:
- **קלט:** "די אמיר תפסיק לדבר"
- **תוצאה:** אמיר נפגע רגשית (כעס +3, עצב +1.5) ומציג תגובה בבועה

### שבח לתלמיד ספציפי:
- **קלט:** "כל הכבוד דנה"
- **תוצאה:** דנה שמחה יותר (שמחה +3) ומציגה תגובה בבועה

### שאלה כללית:
- **קלט:** "איך אתם מרגישים?"
- **תוצאה:** עד 4 תלמידים עונים לפי מצב הרוח שלהם
