# Web Speech API - Complete Setup Guide

## 🎉 What You're Getting

**100% FREE voice recognition that works in your browser!**

- ✅ No API keys needed
- ✅ No accounts needed
- ✅ No setup required
- ✅ Works in Chrome, Edge, Safari
- ✅ Unlimited usage
- ✅ Good accuracy
- ✅ 20+ languages supported

---

## 📦 Files Included

1. **WebSpeechRecognition.cs** - Main Unity component
2. **WebSpeechRecognition.jslib** - JavaScript plugin
3. **WebSpeechUI.cs** - Simple UI controller
4. **WebSpeechClassroomIntegration.cs** - Classroom command processing

---

## 🚀 Quick Setup (10 Minutes)

### Step 1: Add Files to Unity (2 min)

1. **Create Folders:**
   ```
   Assets/
   ├── Scripts/
   │   └── VoiceRecognition/
   │       ├── WebSpeechRecognition.cs
   │       ├── WebSpeechUI.cs
   │       └── WebSpeechClassroomIntegration.cs
   └── Plugins/
       └── WebGL/
           └── WebSpeechRecognition.jslib
   ```

2. **Copy Files:**
   - Drag `WebSpeechRecognition.cs`, `WebSpeechUI.cs`, and `WebSpeechClassroomIntegration.cs` 
     into `Assets/Scripts/VoiceRecognition/`
   - Drag `WebSpeechRecognition.jslib` into `Assets/Plugins/WebGL/`

### Step 2: Setup Scene (3 min)

1. **Create Voice Recognition GameObject:**
   - Right-click in Hierarchy → Create Empty
   - Name it: "WebSpeechRecognition"
   - Add Component → WebSpeechRecognition
   - Add Component → WebSpeechClassroomIntegration

2. **Configure Settings:**
   ```
   WebSpeechRecognition:
   - Language: en-US (or your language)
   - Max Recording Time: 15
   - Continuous: false (unchecked)
   ```

3. **Assign References:**
   ```
   WebSpeechClassroomIntegration:
   - Classroom Manager: [drag from scene]
   - Teacher UI: [drag from scene]
   - Log Commands: ✓ (checked)
   ```

### Step 3: Create UI (3 min)

1. **Create UI Canvas** (if you don't have one):
   - Right-click Hierarchy → UI → Canvas

2. **Add Voice Control Panel:**
   - Right-click Canvas → UI → Panel
   - Name: "VoiceControlPanel"

3. **Add Button:**
   - Right-click VoiceControlPanel → UI → Button (TextMeshPro)
   - Name: "RecordButton"
   - Text: "🎤 Start Recording"

4. **Add Status Text:**
   - Right-click VoiceControlPanel → UI → Text (TextMeshPro)
   - Name: "StatusText"
   - Text: "Ready"

5. **Add Transcript Text:**
   - Right-click VoiceControlPanel → UI → Text (TextMeshPro)
   - Name: "TranscriptText"
   - Text: ""

6. **Add WebSpeechUI Component:**
   - Select VoiceControlPanel
   - Add Component → WebSpeechUI
   - Assign references:
     ```
     Web Speech: [drag WebSpeechRecognition GameObject]
     Record Button: [drag RecordButton]
     Status Text: [drag StatusText]
     Transcript Text: [drag TranscriptText]
     ```

### Step 4: Build for WebGL (2 min)

1. **Open Build Settings:**
   - File → Build Settings

2. **Switch to WebGL:**
   - Select "WebGL"
   - Click "Switch Platform"

3. **Player Settings:**
   - Click "Player Settings"
   - Under "Publishing Settings":
     - Compression Format: Disabled (for faster testing)
   - Under "Resolution and Presentation":
     - Default Canvas Width: 1920
     - Default Canvas Height: 1080

4. **Build:**
   - Click "Build and Run"
   - Choose output folder
   - Wait for build to complete
   - Browser will open automatically

### Step 5: Test! (1 min)

1. **Grant Microphone Permission:**
   - Browser will ask for mic permission
   - Click "Allow"

2. **Test Recording:**
   - Click "Start Recording" button
   - Say: "Good job David"
   - Watch the transcript appear!
   - See the classroom action execute!

---

## 🎯 Supported Commands

Once setup is complete, you can say:

### Student Praise
- "Good job"
- "Well done"
- "Excellent"
- "Great work"
- "Nice"

### Classroom Management
- "Everyone quiet"
- "Stop talking"
- "Settle down"
- "Be quiet"

### Instructions
- "Come to the board"
- "Take a break"
- "Open your books"
- "Change seats"

### Bag Items
- "Use the ruler"
- "Play a game"
- "Listen to music"
- "Open the book"

### With Student Names
- "David good job"
- "Maya be quiet"
- "Everyone take a break"

---

## 🌍 Supported Languages

Change the language in the Inspector:

| Language | Code |
|----------|------|
| English (US) | en-US |
| English (UK) | en-GB |
| Hebrew | he-IL |
| Spanish | es-ES |
| French | fr-FR |
| German | de-DE |
| Italian | it-IT |
| Portuguese | pt-PT |
| Russian | ru-RU |
| Chinese | zh-CN |
| Japanese | ja-JP |
| Korean | ko-KR |

**Full list:** [MDN Language Codes](https://developer.mozilla.org/en-US/docs/Web/API/SpeechRecognition/lang)

---

## 🌐 Browser Support

| Browser | Support | Notes |
|---------|---------|-------|
| **Chrome** | ✅ Full | Best support |
| **Edge** | ✅ Full | Chromium-based |
| **Safari** | ✅ Good | iOS 14.5+ |
| **Firefox** | ⚠️ Limited | Experimental |
| **Opera** | ✅ Good | Chromium-based |

**Recommended:** Chrome or Edge for best experience.

---

## 🎨 Customizing the UI

### Adding Visual Feedback

```csharp
// In WebSpeechUI.cs, you can customize:
public Color recordingColor = Color.red;     // Color when recording
public Color processingColor = Color.yellow; // Color when processing
public Color idleColor = Color.white;        // Color when idle
```

### Adding Recording Indicator

1. Create UI Image:
   - Right-click VoiceControlPanel → UI → Image
   - Name: "RecordingIndicator"
   - Make it a small circle

2. Assign to WebSpeechUI:
   ```
   Recording Indicator: [drag RecordingIndicator]
   ```

3. The indicator will pulse red when recording!

### Custom Button Styles

Edit the button appearance in Inspector:
- Colors
- Font size
- Background image
- Position/size

---

## 🔧 Advanced Configuration

### Continuous Recognition

Set `Continuous = true` to keep listening until you stop it:

```csharp
WebSpeechRecognition:
- Continuous: ✓ (checked)
```

**Use case:** For longer commands or conversations.

### Custom Commands

Edit `WebSpeechClassroomIntegration.cs`:

```csharp
void InitializeCommands()
{
    commandActions = new Dictionary<string[], System.Action>
    {
        // Add your custom commands here
        { new[] { "attention please", "listen up" }, 
          () => ExecuteAction(ActionType.Yell) },
        
        { new[] { "fantastic", "amazing" }, 
          () => ExecuteAction(ActionType.Praise) },
    };
}
```

### Handling Specific Student Names

The system automatically detects student names from your classroom:

```csharp
// Automatically works:
"David come to board"     → Calls David
"Maya stop talking"       → Tells Maya to be quiet
"Everyone be quiet"       → Affects whole class
```

---

## 🐛 Troubleshooting

### "Microphone permission denied"
**Solution:**
1. Click the microphone icon in browser address bar
2. Click "Allow"
3. Refresh page

### "Browser not supported"
**Solution:**
- Use Chrome, Edge, or Safari
- Update browser to latest version
- Check if browser has speech API: chrome://flags/#enable-speech-recognition

### "No speech detected"
**Solution:**
- Check microphone is connected
- Increase microphone volume
- Speak louder and clearer
- Reduce background noise

### Recording but no transcript
**Solution:**
- Wait 1-2 seconds after speaking
- Speak more clearly
- Check internet connection (Web Speech needs internet)
- Try different language code

### Commands not triggering actions
**Solution:**
1. Check Console for logs (F12 → Console)
2. Verify `Log Commands` is checked
3. Make sure ClassroomManager is assigned
4. Check command keywords match your language

### Building takes too long
**Solution:**
- Disable compression in Player Settings
- Use Development Build for faster iteration
- Build once, then use "Build and Run" for updates

---

## 📱 Testing in Unity Editor

**Note:** Web Speech API only works in WebGL builds, not in Unity Editor.

However, the script includes simulation mode:

```csharp
// In Unity Editor, after 3 seconds it will simulate:
"Simulated transcription - build for WebGL to test real recognition"
```

**For real testing:** Always build for WebGL!

---

## 🎯 Integration Examples

### Example 1: Simple Voice Command

```csharp
using UnityEngine;

public class MyVoiceTest : MonoBehaviour
{
    public WebSpeechRecognition webSpeech;

    void Start()
    {
        webSpeech.OnTranscriptionReceived += OnVoiceCommand;
    }

    void OnVoiceCommand(string text)
    {
        Debug.Log($"Teacher said: {text}");
        
        if (text.ToLower().Contains("hello"))
        {
            Debug.Log("Student: Hello teacher!");
        }
    }
}
```

### Example 2: Button Toggle

```csharp
using UnityEngine;
using UnityEngine.UI;

public class VoiceButton : MonoBehaviour
{
    public WebSpeechRecognition webSpeech;
    public Button toggleButton;
    public TMPro.TMP_Text buttonText;

    void Start()
    {
        toggleButton.onClick.AddListener(ToggleRecording);
    }

    void ToggleRecording()
    {
        if (webSpeech.IsRecording())
        {
            webSpeech.StopRecording();
            buttonText.text = "🎤 Start";
        }
        else
        {
            webSpeech.StartRecording();
            buttonText.text = "⏹ Stop";
        }
    }
}
```

### Example 3: Auto-Stop After Timeout

```csharp
using UnityEngine;
using System.Collections;

public class AutoStopRecording : MonoBehaviour
{
    public WebSpeechRecognition webSpeech;
    public float autoStopDelay = 10f;

    void Start()
    {
        webSpeech.OnRecordingStarted += OnStarted;
    }

    void OnStarted()
    {
        StartCoroutine(AutoStop());
    }

    IEnumerator AutoStop()
    {
        yield return new WaitForSeconds(autoStopDelay);
        
        if (webSpeech.IsRecording())
        {
            webSpeech.StopRecording();
            Debug.Log("Auto-stopped recording");
        }
    }
}
```

---

## 📊 Performance Tips

### Optimize Build Size
```
Player Settings → Publishing Settings:
- Compression Format: Brotli
- Strip Engine Code: ✓
- Managed Stripping Level: High
```

### Reduce Loading Time
```
- Use Asset Bundles for large assets
- Enable Code Stripping
- Minimize texture sizes
- Use WebGL 2.0
```

### Improve Accuracy
1. **Speak clearly** and at normal pace
2. **Reduce background noise**
3. **Use good microphone**
4. **Test in quiet environment**

---

## 🔒 Privacy & Security

### Data Handling
- **Recording:** Stays in browser memory
- **Processing:** Sent to browser's speech service (Google/Apple)
- **Storage:** Not stored by your app
- **Privacy:** Follow browser's privacy policy

### User Permissions
Always inform users:
- Microphone will be accessed
- Voice data sent to speech service
- Add privacy notice in your app

### Best Practices
```html
<!-- Add to your WebGL template: -->
<div id="privacy-notice">
  This app uses voice recognition.
  Your voice will be processed by your browser's speech service.
</div>
```

---

## 📚 API Reference

### WebSpeechRecognition Methods

```csharp
// Initialize (called automatically)
public void Initialize()

// Start recording
public void StartRecording()

// Stop recording
public void StopRecording()

// Toggle recording
public void ToggleRecording()

// Check status
public bool IsRecording()
```

### Events

```csharp
// When transcription is received
public event Action<string> OnTranscriptionReceived;

// When error occurs
public event Action<string> OnError;

// When recording starts
public event Action OnRecordingStarted;

// When recording ends
public event Action OnRecordingEnded;
```

---

## 🎉 You're Done!

Your classroom simulator now has **FREE voice recognition**!

### Next Steps:
1. ✅ Build for WebGL
2. ✅ Test basic commands
3. ✅ Add custom commands
4. ✅ Customize UI
5. ✅ Deploy to students!

### No costs. No limits. No API keys. Just works! 🎤

---

## 🆘 Need Help?

### Resources:
- **MDN Docs:** https://developer.mozilla.org/en-US/docs/Web/API/Web_Speech_API
- **Browser Support:** https://caniuse.com/speech-recognition
- **Unity WebGL:** https://docs.unity3d.com/Manual/webgl-building.html

### Common Issues:
- Check Unity Console (F12)
- Verify microphone permissions
- Use Chrome or Edge
- Build for WebGL (doesn't work in Editor)

**Happy voice controlling!** 🎤✨
