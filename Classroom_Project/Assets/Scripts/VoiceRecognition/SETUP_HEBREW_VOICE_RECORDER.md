# הוראות חיבור כפתור הקלטה עברית

מדריך זה מסביר איך לחבר את הכפתור הקיים ביוניטי כך שיקליט בעברית ויגרום לתלמידים להגיב.

## שלב 1: חיבור הכפתור הקיים

1. **פתח את הסצנה** (MainClassroom.unity)
2. **מצא את הכפתור** שכבר קיים בהיררכיה
3. **בחר את GameObject** עם הקומפוננטה `TeacherVoiceRecorderUI` (או צור אחד חדש)
4. **בחלון Inspector**, מצא את הקומפוננטה **Teacher Voice Recorder UI**
5. **גרור את הכפתור** מההיררכיה לשדה **"Record Button"**

## שלב 2: הוספת הקומפוננטות הנדרשות

### A. HuggingFaceVoiceRecognitionService

1. בדוק אם כבר קיים GameObject עם הקומפוננטה הזו בסצנה
2. אם לא, צור: Right-click Hierarchy → Create Empty
3. שם: `HuggingFaceVoiceService`
4. הוסף קומפוננטה: Add Component → Script → `HuggingFaceVoiceRecognitionService`
5. **בחלון Inspector**, הגדר:
   - **Hugging Face Token**: הזן את ה-API Token שלך
   - **Language**: `he` (עברית) - כבר מוגדר כברירת מחדל ✓
   - **Model ID**: `openai/whisper-large-v3` (תומך בעברית מצוין)

### B. TeacherVoiceRecorderUI

1. מצא או צור GameObject (למשל `TeacherUI`)
2. הוסף קומפוננטה: Add Component → Script → `TeacherVoiceRecorderUI`
3. **בחלון Inspector**, חבר:
   - **Record Button**: גרור את הכפתור הקיים כאן ✓
   - **Transcript Top Text**: (אופציונלי) טקסט להצגת התמליל
   - **Status Text**: (אופציונלי) טקסט לסטטוס

### C. HuggingFaceVoiceCommandIntegration

**קומפוננטה זו מעבדת את התמליל וגורמת לתלמידים להגיב!**

1. באותו GameObject, הוסף: Add Component → Script → `HuggingFaceVoiceCommandIntegration`
2. **בחלון Inspector**, חבר את כל ההפניות:
   - **Voice Service**: גרור את `HuggingFaceVoiceService` כאן
   - **Classroom Manager**: גרור את `ClassroomManager` כאן
   - **Teacher UI**: גרור את `TeacherUI` כאן
   - **Log Commands**: סמן כדי לראות פקודות בקונסול

## שלב 3: בדיקת הקונפיגורציה

ההיררכיה צריכה להיראות כך:

```
MainClassroom Scene
├── Canvas
│   └── [הכפתור הקיים שלך] (Record Button)
├── HuggingFaceVoiceService (GameObject)
│   └── HuggingFaceVoiceRecognitionService (Component)
│       └── Language: "he" ✓
│       └── Model: "openai/whisper-large-v3" ✓
├── TeacherUI (GameObject)
│   ├── TeacherVoiceRecorderUI (Component) ✓
│   │   └── Record Button: [הכפתור שלך] ✓
│   └── HuggingFaceVoiceCommandIntegration (Component) ✓
│       └── Voice Service: [HuggingFaceVoiceService] ✓
│       └── Classroom Manager: [ClassroomManager] ✓
│       └── Teacher UI: [TeacherUI] ✓
└── ClassroomManager (GameObject)
```

## שלב 4: בדיקת הפעולה

1. **לחץ Play** ביוניטי
2. **לחץ על הכפתור** - צריך:
   - להשתנות לאדום (מקליט)
   - להציג "Recording..." או "מקליט..."
   - להתחיל להקליט מהמיקרופון

3. **דבר בעברית** (למשל: "שקט", "כל הכבוד", "פתחו ספרים")
4. **לחץ שוב על הכפתור** כדי לעצור
5. **בדוק את הקונסול** - אמור לראות:
   - Transcription received - התמליל התקבל
   - Matched keyword - פקודה זוהתה
   - Students reacting - תלמידים מגיבים!

## פקודות עברית נתמכות

### פקודות עידוד:
- "כל הכבוד"
- "יפה מאוד"
- "מצוין"

### פקודות משמעת:
- "שקט"
- "תשקטו"
- "די"

### פקודות הוראה:
- "בוא ללוח" - קורא לתלמיד ללוח

### פקודות כלליות:
- "הפסקה" - הפסקה לכיתה
- "פתחו ספרים" / "ספרים" - פתיחת ספרים

## פתרון בעיות

### הכפתור לא מתחיל להקליט:
- ✓ בדוק שהכפתור מחובר ב-`TeacherVoiceRecorderUI` Inspector
- ✓ בדוק ש-`HuggingFaceVoiceRecognitionService` קיים בסצנה
- ✓ בדוק שהמיקרופון זמין והותרת הרשאות
- ✓ בדוק את הקונסול לשגיאות

### התלמידים לא מגיבים:
- ✓ בדוק ש-`HuggingFaceVoiceCommandIntegration` נוסף
- ✓ בדוק שכל ההפניות מחוברות (Voice Service, Classroom Manager, Teacher UI)
- ✓ בדוק את הקונסול להודעות "[HuggingFace VoiceCommand]"
- ✓ נסה לדבר פקודה נתמכת (ראה רשימה למעלה)

### התמלול לא עובד:
- ✓ בדוק שה-HuggingFace API Token מוגדר נכון
- ✓ בדוק חיבור לאינטרנט (נדרש חיבור לשרת API)
- ✓ בדוק את הקונסול לשגיאות API
- ✓ ודא שהמיקרופון עובד באפליקציות אחרות
- ✓ ודא ש-**Language מוגדר ל-"he"** ב-HuggingFaceVoiceRecognitionService

### זיהוי עברית לא מדויק:
- ✓ ודא ש-**Language = "he"** ב-HuggingFaceVoiceRecognitionService
- ✓ ודא שה-Model ID הוא `openai/whisper-large-v3` (תומך בעברית)
- ✓ דבר בבירור וקרוב למיקרופון
- ✓ בדוק איכות אודיו - רעש רקע יכול להפריע

## איך זה עובד

1. **לחיצה על הכפתור** → `TeacherVoiceRecorderUI.StartRecording()` מתחיל הקלטה
2. **הקלטה מתחילה** → `HuggingFaceVoiceRecognitionService` קולט אודיו מהמיקרופון
3. **לחיצה שוב** → הקלטה נעצרת, אודיו נשלח ל-HuggingFace API
4. **תמליל מתקבל** → `HuggingFaceVoiceCommandIntegration.ProcessVoiceCommand()` מעבד
5. **פקודה מזוהה** → תלמידים מגיבים דרך `ClassroomManager.ExecuteTeacherAction()`

## שמירת הקלטות המורה

ההקלטה נשמרת אוטומטית כ-AudioClip בזמן ההקלטה ונשלחת ל-HuggingFace API לתמלול. הטקסט המתומלל:

1. **מוצג** ב-UI (דרך `TeacherVoiceRecorderUI`)
2. **מעובד** לפקודות (דרך `HuggingFaceVoiceCommandIntegration`)
3. **משמש** להפעלת תגובות תלמידים (דרך `ClassroomManager`)

האודיו עצמו מעובד בזיכרון ולא נשמר לדיסק כברירת מחדל. אם צריך לשמור קבצי אודיו, ניתן לשנות את `HuggingFaceVoiceRecognitionService.ProcessAudioWithHuggingFace()`.

## הבא

- הוסף פקודות נוספות ב-`HuggingFaceVoiceCommandIntegration.InitializeCommands()`
- הוסף תגובות תלמידים ב-`ClassroomManager`
- התאם אישית את הכפתור עם sprites או אנימציות
- הוסף משוב ויזואלי (הבהוב, זוהר) בעת הקלטה
