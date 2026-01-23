# Bubble and Camera Issues - Quick Fixes

## Issue 1: Bubbles Not Showing

### Root Cause
The `StudentResponseBubble` component requires `Camera.main` to work. If your camera doesn't have the "MainCamera" tag, bubbles won't display.

### Fix in Unity:
1. **Find your main camera** in the scene
2. **Select it** in Hierarchy
3. **In Inspector**, find the **Tag dropdown** (top of Inspector)
4. **Set Tag to: MainCamera**
5. **Save the scene**

### Alternative Fix (Code):
If you can't use the MainCamera tag, edit `StudentResponseBubble.cs` line 66-67:

**Change from:**
```csharp
if (mainCamera == null)
    mainCamera = Camera.main;
```

**To:**
```csharp
if (mainCamera == null)
    mainCamera = Camera.main ?? FindObjectOfType<Camera>();
```

---

## Issue 2: Camera Offset Issues

### Potential Causes:
1. World-space canvas is pushing camera
2. UI layer collision with camera
3. Canvas scaler affecting camera position

### Quick Fixes:

### Fix A: Check Camera Culling Mask
1. Select your **Main Camera**
2. In Inspector, find **Culling Mask**
3. Make sure **UI layer** is UNCHECKED
4. This prevents UI from affecting camera

### Fix B: Verify Bubble Layer
The bubble code already sets UI to layer 5, but verify:
1. Go to **Edit → Project Settings → Tags and Layers**
2. Make sure **Layer 5** is set to "UI"
3. Save

### Fix C: Check Canvas Settings
If bubbles still interfere with camera:
1. Play the scene
2. Find a student with a bubble
3. In Hierarchy, expand the student → find "ResponseCanvas"
4. In Inspector, check:
   - **Render Mode**: Should be "World Space"
   - **Sorting Order**: Should be 1000 (high number)
   - **Canvas Scaler**: Dynamic Pixels Per Unit = 10

---

## Testing Checklist

### Test Bubbles:
1. ✅ Play scene
2. ✅ Make sure a question is asked (check console for question detection)
3. ✅ Look for students raising hands (animations should play)
4. ✅ Click on a student with raised hand
5. ✅ Bubble should appear above their head with Hebrew text

### Test Camera:
1. ✅ Camera should not move when bubbles appear
2. ✅ Camera should not jitter or offset
3. ✅ UI elements should not block camera view

---

## Debug Console Errors

Looking at your screenshot, I see an error message at the bottom. Common errors:

### "SendMessage selectStudent has no receiver!"
**Fix**: Make sure the student GameObject has all required components:
- StudentAgent
- StudentQuestionResponder
- StudentResponseBubble
- StudentReactionAnimator

### "Camera.main is null"
**Fix**: Tag your camera as "MainCamera" (see Issue 1 fix above)

### "Canvas has no camera assigned"
**Fix**: The StudentResponseBubble auto-assigns the camera. If it fails:
1. Select student in play mode
2. Find ResponseCanvas child
3. Manually drag Main Camera to Canvas → World Camera field

---

##Quick Unity Checklist

Run through these in Unity Editor:

1. **Camera Setup**:
   - [ ] Camera has "MainCamera" tag
   - [ ] Culling Mask doesn't include UI layer
   - [ ] Camera is at reasonable position (not inside walls/floor)

2. **Student Setup** (check any student):
   - [ ] Has StudentAgent component
   - [ ] Has StudentQuestionResponder component
   - [ ] Has StudentResponseBubble component (auto-added by spawner)
   - [ ] Has StudentReactionAnimator component

3. **Layers** (Edit → Project Settings → Tags and Layers):
   - [ ] Layer 5 = "UI"
   - [ ] Default layer exists

4. **Canvas** (Play mode, expand student):
   - [ ] ResponseCanvas exists as child
   - [ ] Canvas → World Camera is assigned to Main Camera
   - [ ] Canvas layer = UI (5)

---

## Still Not Working?

### Enable Debug Logs:
Add this to `StudentResponseBubble.cs` in `ShowResponse()` method (line 238):

```csharp
public void ShowResponse(string response)
{
    Debug.Log($"[Bubble] ShowResponse called: {response}");
    Debug.Log($"[Bubble] responseCanvas: {responseCanvas != null}");
    Debug.Log($"[Bubble] mainCamera: {mainCamera != null}");
    Debug.Log($"[Bubble] responseText: {responseText != null}");

    // ... rest of existing code
}
```

This will show in console what's failing.

---

## Common Solutions Summary

| Problem | Solution |
|---------|----------|
| Bubbles don't appear | Tag camera as "MainCamera" |
| Camera moves/offsets | Uncheck UI layer in camera culling mask |
| Bubbles behind students | Increase Canvas sorting order to 1000+ |
| Console errors | Verify all components are on students |
| Bubbles show but empty | Check if question is being asked (debug log) |

---

## Prevention

To prevent these issues in future scenes:
1. Always tag your main camera as "MainCamera"
2. Keep UI elements on UI layer (5)
3. Don't put physics colliders on UI elements
4. Use World Space canvas for 3D-positioned UI
