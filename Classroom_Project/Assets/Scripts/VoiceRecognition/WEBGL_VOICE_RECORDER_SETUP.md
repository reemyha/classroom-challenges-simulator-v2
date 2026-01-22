# WebGL Voice Recorder Setup (Hebrew) - Simple Guide

This guide explains how to connect the existing yellow button to use **Web Speech API** for Hebrew voice recognition. **No API keys needed** - it works directly in browsers!

## ✅ What You Need

1. **The existing yellow button** in Unity (already created)
2. **WebSpeechRecognition** component (add to scene)
3. **WebSpeechClassroomIntegration** component (processes commands)
4. **TeacherVoiceRecorderUI** component (already updated)

## Step 1: Add WebSpeechRecognition Component

1. **Create a GameObject** in your scene:
   - Right-click Hierarchy → Create Empty
   - Name it `WebSpeechService`

2. **Add the component:**
   - Select `WebSpeechService`
   - Add Component → Script → `WebSpeechRecognition`
   - **In Inspector**, verify:
     - **Language**: `he-IL` (Hebrew - Israel) ✓
     - **Continuous**: Unchecked (stop after each recording)
     - **Max Recording Time**: 15 seconds

## Step 2: Connect TeacherVoiceRecorderUI

1. **Find or create GameObject** with `TeacherVoiceRecorderUI` component
2. **In Inspector**, connect:
   - **Record Button**: Drag your yellow button here ✓
   - **Transcript Top Text**: (Optional) TextMeshPro UI for showing transcriptions
   - **Status Text**: (Optional) TextMeshPro UI for status messages

## Step 3: Add WebSpeechClassroomIntegration

This makes students react to your voice commands!

1. **On the same GameObject** (or create new one), add:
   - Add Component → Script → `WebSpeechClassroomIntegration`

2. **In Inspector**, connect all references:
   - **Web Speech**: Drag `WebSpeechService` GameObject here ✓
   - **Classroom Manager**: Drag your `ClassroomManager` GameObject here ✓
   - **Teacher UI**: Drag your `TeacherUI` GameObject here ✓
   - **Log Commands**: ✓ Check to see commands in Console

## Step 4: Verify Setup

Your hierarchy should look like this:

```
MainClassroom Scene
├── Canvas
│   └── [Your Yellow Button] (Record Button)
├── WebSpeechService (GameObject)
│   └── WebSpeechRecognition (Component)
│       └── Language: "he-IL" ✓
│       └── Continuous: false ✓
├── TeacherUI (GameObject)
│   ├── TeacherVoiceRecorderUI (Component) ✓
│   │   └── Record Button: [Your Button] ✓
│   └── WebSpeechClassroomIntegration (Component) ✓
│       └── Web Speech: [WebSpeechService] ✓
│       └── Classroom Manager: [ClassroomManager] ✓
│       └── Teacher UI: [TeacherUI] ✓
└── ClassroomManager (GameObject)
```

## Step 5: Build for WebGL and Test

**⚠️ Important**: Web Speech API only works in **WebGL builds**, not in Unity Editor!

1. **Build for WebGL:**
   - File → Build Settings
   - Platform: WebGL
   - Click "Build and Run"

2. **Test in browser:**
   - Click the yellow button → Should turn red (recording)
   - **Speak in Hebrew** (e.g., "שקט", "כל הכבוד", "פתחו ספרים")
   - Click button again → Should stop and process
   - Check Console for: `[VoiceCommand] Matched keyword: 'שקט'`
   - **Students should react!** 🎉

## Supported Hebrew Commands

### פקודות עידוד (Praise):
- "כל הכבוד"
- "יפה מאוד"
- "מצוין"

### פקודות משמעת (Discipline):
- "שקט"
- "תשקטו"
- "די"

### פקודות הוראה (Instructions):
- "בוא ללוח" - Call to board

### פקודות כלליות (General):
- "הפסקה" - Give break
- "פתחו ספרים" / "ספרים" - Open books

## How It Works

1. **Click yellow button** → `TeacherVoiceRecorderUI.StartRecording()` called
2. **Web Speech API starts** → Browser's built-in speech recognition (Hebrew)
3. **You speak** → Browser transcribes in real-time (Hebrew)
4. **Click again** → Recording stops, transcription received
5. **WebSpeechClassroomIntegration** → Processes Hebrew text, matches commands
6. **ClassroomManager** → Students react dynamically! 🎯

## Troubleshooting

### Button doesn't work in Editor:
- ✓ **Normal!** Web Speech API only works in WebGL builds
- ✓ Build for WebGL to test

### Button doesn't work in WebGL:
- ✓ Check browser supports Web Speech API (Chrome/Edge/Safari)
- ✓ Check microphone permissions in browser
- ✓ Check Console for errors
- ✓ Verify `WebSpeechRecognition` component exists and Language = "he-IL"

### Students don't react:
- ✓ Check `WebSpeechClassroomIntegration` component added
- ✓ Check all references connected (Web Speech, Classroom Manager, Teacher UI)
- ✓ Check Console for "[VoiceCommand]" messages
- ✓ Try speaking a supported Hebrew command (see list above)

### Hebrew recognition not working:
- ✓ Verify `WebSpeechRecognition.language = "he-IL"` in Inspector
- ✓ Check browser supports Hebrew (Chrome/Edge usually best)
- ✓ Speak clearly and close to microphone
- ✓ Check browser language settings

### "Web Speech API not available" message:
- ✓ Component not found - add `WebSpeechRecognition` to scene
- ✓ Build for WebGL (doesn't work in Editor)
- ✓ Check browser compatibility (Chrome/Edge recommended)

## Advantages of Web Speech API

✅ **100% FREE** - No API keys needed  
✅ **Works in browsers** - Built into Chrome/Edge/Safari  
✅ **Hebrew support** - Native browser support  
✅ **Real-time** - Transcription happens instantly  
✅ **No setup** - Just build for WebGL  

## Next Steps

- Customize commands in `WebSpeechClassroomIntegration.InitializeCommands()`
- Add more Hebrew commands for your classroom
- Style the button with custom colors/animations
- Add visual feedback when recording (pulsing, glow)

---

**Note**: The old HuggingFace integration has been removed. This uses the simpler, free Web Speech API that works directly in browsers with Hebrew support!
