# Camera Setup Guide - Fixing Camera Issues

This guide helps you fix camera problems in Unity so you only see the Main Camera view when the game starts.

---

## 🎥 Problem: Wrong Camera View on Start

### Symptoms:
- You see a different camera view than expected
- Multiple cameras are active
- Scene view shows instead of Game view
- Camera starts in wrong position

---

## ✅ Solution 1: Use MainCameraSetup Component (RECOMMENDED)

I've created an automatic script to fix all camera issues!

### Step 1: Add Component to Your Main Camera

1. **Open Unity**
2. In **Hierarchy**, find your **Main Camera** (the camera you want to use)
3. **Select it**
4. In **Inspector**, click **Add Component**
5. Type: **MainCameraSetup**
6. Click on it to add

### Step 2: Configure Settings (Optional)

The component has these settings (defaults are good):

| Setting | Default | Description |
|---------|---------|-------------|
| **Start Position** | (0, 15, -10) | Where camera starts |
| **Start Rotation** | (30, 0, 0) | Camera's initial angle |
| **Disable Other Cameras** | ✅ Yes | Turns off all other cameras |
| **Ensure MainCamera Tag** | ✅ Yes | Sets the MainCamera tag |
| **Ensure Single AudioListener** | ✅ Yes | Removes duplicate audio listeners |

### Step 3: Test

1. **Save the scene** (Ctrl+S / Cmd+S)
2. **Press Play**
3. **Check Console** for messages like:
   ```
   [MainCameraSetup] Main camera setup complete!
   [MainCameraSetup] Camera reset to start position
   ```
4. Your camera should now be in the correct position!

---

## ✅ Solution 2: Manual Setup (If Solution 1 Doesn't Work)

### A. Check You're Viewing the Game Tab

1. Look at the top of Unity window
2. Make sure you're on the **"Game"** tab, NOT "Scene" tab
3. If you see "Scene", click **"Game"** instead

### B. Verify Main Camera Tag

1. Select your **Main Camera** in Hierarchy
2. At the top of Inspector, find **Tag** dropdown
3. Make sure it's set to: **MainCamera**
4. If not, change it to **MainCamera**

### C. Disable Other Cameras

1. In **Hierarchy**, search for: **"camera"** (use search box)
2. You might see multiple cameras
3. For each camera that is NOT your main camera:
   - Select it
   - In Inspector, **uncheck** the checkbox at the very top (next to camera name)
   - This disables that camera

### D. Check Camera Depth/Priority

1. Select your **Main Camera**
2. In Camera component, find **Depth**
3. Set it to: **0** (zero)
4. Any other cameras should have negative depth (like -1)

### E. Fix AudioListener Warnings

If you see warnings like "There are 2 audio listeners in the scene":

1. Search for all cameras (Hierarchy → search "camera")
2. Select each camera that is NOT your main camera
3. Find **AudioListener** component
4. Click the **⚙️** (gear icon) → **Remove Component**

---

## ✅ Solution 3: Set Initial Camera Position

If camera is in wrong position at start:

### Option A: Using MainCameraSetup

1. Select Main Camera
2. Find **MainCameraSetup** component
3. Set **Start Position** to where you want (e.g., `0, 15, -10`)
4. Set **Start Rotation** to angle you want (e.g., `30, 0, 0`)

### Option B: Using CameraController

1. Select Main Camera
2. Find **CameraController** component
3. Call the **ResetCamera()** method

### Option C: Manual Setting

1. Select Main Camera
2. In Inspector, set **Transform**:
   - Position: `X: 0, Y: 15, Z: -10`
   - Rotation: `X: 30, Y: 0, Z: 0`
3. This gives you a good overview of the classroom

---

## 🔧 Advanced Troubleshooting

### Multiple Cameras in Scene

To find all cameras:
1. **Edit → Project Settings → Tags and Layers**
2. Make note of all camera tags
3. **Hierarchy → Search:** type "camera"
4. Check each result

### Canvas Cameras

UI Canvases might have their own cameras:
1. Find all Canvas objects in Hierarchy
2. Select each Canvas
3. If **Render Mode** is "Screen Space - Camera":
   - Either change to "Screen Space - Overlay"
   - Or assign your Main Camera to the **Render Camera** field

### Scene View Affecting Game View

1. Make sure **Game tab** is selected (not Scene tab)
2. Click on Game tab
3. Try pressing **Play** again

### Camera Component Disabled

1. Select Main Camera
2. Check that **Camera** component has checkbox enabled (✅)
3. Make sure GameObject itself is active (checkbox at top of Inspector)

---

## 🎮 Testing Checklist

After applying fixes, verify:

- [ ] Click Play in Unity
- [ ] Game view shows Main Camera's perspective
- [ ] Camera is at correct starting position
- [ ] No camera-related errors in Console
- [ ] No AudioListener warnings
- [ ] Camera controls work (WASD, mouse)
- [ ] Only ONE camera is active

---

## 📋 Quick Reference: Good Camera Settings

For a classroom overview camera:

```
Transform:
  Position: (0, 15, -10)
  Rotation: (30, 0, 0)
  Scale: (1, 1, 1)

Camera Component:
  Clear Flags: Skybox
  Culling Mask: Everything (except UI layer for camera offset prevention)
  Projection: Perspective
  Field of View: 60
  Depth: 0
  Target Display: Display 1

Tag: MainCamera

Components:
  ✅ Camera
  ✅ CameraController (for movement)
  ✅ MainCameraSetup (NEW - for auto-setup)
  ✅ AudioListener
```

---

## 🚀 Prevention Tips

To avoid camera issues in the future:

1. **Always use MainCameraSetup component** on your main camera
2. **Only have ONE active camera** at a time
3. **Always tag your main camera** as "MainCamera"
4. **Check Game tab** instead of Scene tab when playing
5. **Save scene after making camera changes**

---

## 💡 Common Mistakes

| Mistake | How to Identify | Fix |
|---------|----------------|-----|
| Viewing Scene tab | Top shows "Scene" tab | Click "Game" tab |
| Multiple cameras active | Console shows camera warnings | Disable extras |
| No MainCamera tag | Bubbles don't work | Add MainCamera tag |
| Wrong start position | Camera in weird spot on play | Use MainCameraSetup |
| Multiple AudioListeners | Console warning about listeners | Remove from extra cameras |

---

## 🎬 Summary

**The fastest fix:**
1. Add **MainCameraSetup** component to your Main Camera
2. Save and Play
3. Done! ✅

**Manual alternative:**
1. Make sure you're viewing **Game tab** (not Scene)
2. Set camera **Tag** to "MainCamera"
3. **Disable all other cameras**
4. Set camera **Position** to (0, 15, -10) and **Rotation** to (30, 0, 0)

Your camera will now work correctly every time you start the game!
