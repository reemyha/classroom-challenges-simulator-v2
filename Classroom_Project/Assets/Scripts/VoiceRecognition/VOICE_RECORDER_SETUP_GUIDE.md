# Voice Recorder Button Setup Guide

This guide explains how to set up the yellow voice recorder button in Unity so it records teacher input and makes students react dynamically.

## Step 1: Create the Yellow Button in Unity Hierarchy

1. **Open your scene** (MainClassroom.unity)
2. **Create a UI Button:**
   - Right-click in Hierarchy → UI → Button
   - Name it `VoiceRecordButton` or `YellowRecordButton`
   - Position it where you want it on screen (e.g., top-right corner)

3. **Make the button yellow:**
   - Select the button in Hierarchy
   - In Inspector, find the `Button` component
   - Expand "Colors" section
   - Set Normal Color to Yellow (255, 255, 0)
   - Optionally add text: Add a TextMeshPro - Text (UI) child, or edit the existing Text component

4. **Optional: Add text label:**
   - If button has no text, add: Right-click button → UI → TextMeshPro - Text (UI)
   - Set text to "Record" or "🎤"

## Step 2: Add Required Components to Scene

You need these components in your scene:

### A. HuggingFaceVoiceRecognitionService (if not already present)

1. Create empty GameObject: Right-click Hierarchy → Create Empty
2. Name it `HuggingFaceVoiceService`
3. Add Component → Script → `HuggingFaceVoiceRecognitionService`
4. Configure in Inspector:
   - Set your HuggingFace API Token
   - Set Language (e.g., "he" for Hebrew, "en" for English)
   - Set Model ID (default: "openai/whisper-large-v3")

### B. TeacherVoiceRecorderUI Component

1. Find or create a GameObject in your scene (e.g., `TeacherUI` GameObject)
2. Add Component → Script → `TeacherVoiceRecorderUI`
3. In Inspector, assign:
   - **Record Button**: Drag your yellow button here
   - **Transcript Top Text**: (Optional) TextMeshPro UI element to show transcription
   - **Status Text**: (Optional) TextMeshPro UI element for status messages

### C. HuggingFaceVoiceCommandIntegration Component

This component processes transcriptions and makes students react!

1. On the same GameObject (or create new one), add Component → Script → `HuggingFaceVoiceCommandIntegration`
2. In Inspector, assign references:
   - **Voice Service**: Drag `HuggingFaceVoiceService` GameObject here
   - **Classroom Manager**: Drag your `ClassroomManager` GameObject here
   - **Teacher UI**: Drag your `TeacherUI` GameObject here
   - **Log Commands**: Check this to see commands in Console

## Step 3: Connect Button in Inspector

1. **Select the GameObject** with `TeacherVoiceRecorderUI` component
2. In Inspector, find the **Teacher Voice Recorder UI** component
3. **Drag the yellow button** from Hierarchy into the **"Record Button"** field
4. **Optional**: Drag UI Text elements into Transcript/Status text fields

## Step 4: Verify Setup

Your hierarchy should look something like this:

```
MainClassroom Scene
├── Canvas (or UI Canvas)
│   └── VoiceRecordButton (Yellow Button)
│       └── Text (UI) [optional]
├── HuggingFaceVoiceService (GameObject)
│   └── HuggingFaceVoiceRecognitionService (Component)
├── TeacherUI (GameObject)
│   ├── TeacherVoiceRecorderUI (Component) ✓
│   │   └── Record Button: [VoiceRecordButton] ✓
│   └── HuggingFaceVoiceCommandIntegration (Component) ✓
│       └── Voice Service: [HuggingFaceVoiceService] ✓
│       └── Classroom Manager: [ClassroomManager] ✓
│       └── Teacher UI: [TeacherUI] ✓
└── ClassroomManager (GameObject)
```

## Step 5: Test the Setup

1. **Press Play** in Unity
2. **Click the yellow button** - it should:
   - Change color to red (recording)
   - Show "Recording..." in status/transcript text
   - Start capturing microphone input

3. **Speak a command** (e.g., "quiet", "שקט", "good job")
4. **Click the button again** to stop recording
5. **Watch the Console** - you should see:
   - Transcription received
   - Command matched
   - Students reacting (via ClassroomManager)

## How It Works

1. **User clicks yellow button** → `TeacherVoiceRecorderUI.StartRecording()` called
2. **Recording starts** → `HuggingFaceVoiceRecognitionService` captures audio
3. **User clicks again** → Recording stops, audio sent to HuggingFace API
4. **Transcription received** → `HuggingFaceVoiceCommandIntegration.ProcessVoiceCommand()` called
5. **Command matched** → Students react via `ClassroomManager.ExecuteTeacherAction()` or `ExecuteClasswideAction()`

## Supported Voice Commands

### English Commands:
- **Praise**: "good job", "well done", "excellent"
- **Discipline**: "quiet", "silence", "stop talking"
- **Instructions**: "board", "come to board"
- **Break**: "break", "take a break"
- **Books**: "book", "open book"
- **Music**: "music", "song"

### Hebrew Commands:
- **Praise**: "כל הכבוד", "יפה מאוד", "מצוין"
- **Discipline**: "שקט", "תשקטו", "די"
- **Instructions**: "בוא ללוח"
- **Break**: "הפסקה"
- **Books**: "פתחו ספרים", "ספרים"

## Troubleshooting

### Button doesn't start recording:
- ✓ Check button is assigned in `TeacherVoiceRecorderUI` Inspector
- ✓ Check `HuggingFaceVoiceRecognitionService` exists in scene
- ✓ Check microphone permissions are granted
- ✓ Check Console for error messages

### Students don't react to voice:
- ✓ Check `HuggingFaceVoiceCommandIntegration` component is added
- ✓ Check all references are assigned (Voice Service, Classroom Manager, Teacher UI)
- ✓ Check Console for "[HuggingFace VoiceCommand]" messages
- ✓ Try speaking a supported command (see list above)

### Transcription not working:
- ✓ Check HuggingFace API Token is set correctly
- ✓ Check internet connection (API call required)
- ✓ Check Console for API errors
- ✓ Verify microphone is working in other apps

### Button color doesn't change:
- ✓ This is optional visual feedback - functionality still works
- ✓ Check button Colors are set to "Interactable" in Inspector
- ✓ Ensure button has correct color settings in Inspector

## Saving Teacher Input

The microphone input is automatically saved as an AudioClip during recording and sent to HuggingFace API for transcription. The transcribed text is:

1. **Displayed** in the UI (via `TeacherVoiceRecorderUI`)
2. **Processed** for commands (via `HuggingFaceVoiceCommandIntegration`)
3. **Used** to trigger student reactions (via `ClassroomManager`)

The audio clip itself is processed in memory and not saved to disk by default. If you need to save audio files, modify `HuggingFaceVoiceRecognitionService.ProcessAudioWithHuggingFace()` to write the WAV file.

## Next Steps

- Customize commands in `HuggingFaceVoiceCommandIntegration.InitializeCommands()`
- Add more student reactions in `ClassroomManager`
- Style the button with custom sprites or animations
- Add visual feedback (pulsing, glow) when recording
