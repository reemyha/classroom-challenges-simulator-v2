# Classroom Challenges Simulator - System Architecture

## Overview

The Classroom Challenges Simulator is built using a **modular, component-based architecture** in Unity 3D. The system follows the principles of separation of concerns, with clear boundaries between the core simulation logic, UI systems, and AI behaviors.

---

## High-Level Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                          CLASSROOM SIMULATOR ARCHITECTURE                        │
└─────────────────────────────────────────────────────────────────────────────────┘

                              ┌─────────────────────┐
                              │    SCENE MANAGER    │
                              │  (Unity SceneManager)│
                              └──────────┬──────────┘
                                         │
         ┌───────────────────────────────┼───────────────────────────────┐
         │                               │                               │
         ▼                               ▼                               ▼
┌─────────────────┐           ┌─────────────────────┐         ┌─────────────────┐
│   LoginScene    │           │  TeacherHomeScreen  │         │  MainClassroom  │
│                 │           │                     │         │                 │
│ • Authentication│           │ • Scenario Selection│         │ • Core Simulation│
│ • Registration  │           │ • Session History   │         │ • All Gameplay  │
└─────────────────┘           └─────────────────────┘         └─────────────────┘
```

---

## Core System Architecture

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                              MAIN CLASSROOM SCENE                               │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                 │
│  ┌───────────────────────────────────────────────────────────────────────────┐ │
│  │                         CLASSROOM MANAGER (Central Hub)                    │ │
│  │  ┌─────────────┬──────────────┬──────────────┬─────────────────────────┐  │ │
│  │  │ Session     │ Student      │ Metrics      │ Action                  │  │ │
│  │  │ Controller  │ Registry     │ Tracker      │ Executor                │  │ │
│  │  └─────────────┴──────────────┴──────────────┴─────────────────────────┘  │ │
│  └───────────────────────────────────────────────────────────────────────────┘ │
│                                         │                                       │
│           ┌─────────────────────────────┼─────────────────────────────┐        │
│           │                             │                             │        │
│           ▼                             ▼                             ▼        │
│  ┌─────────────────┐         ┌─────────────────────┐       ┌─────────────────┐│
│  │  STUDENT SYSTEM │         │    TEACHER SYSTEM   │       │   UI SYSTEM     ││
│  │                 │         │                     │       │                 ││
│  │ • StudentAgent  │◀───────▶│ • TeacherUI         │◀─────▶│ • Feedback Panel││
│  │ • EmotionVector │         │ • TeacherBagUI      │       │ • Info Panels   ││
│  │ • AI Response   │         │ • VoiceRecorder     │       │ • Bubbles       ││
│  │ • Spawner       │         │                     │       │ • Seat Swap     ││
│  └─────────────────┘         └─────────────────────┘       └─────────────────┘│
│                                                                                 │
└─────────────────────────────────────────────────────────────────────────────────┘
```

---

## Component Architecture

### 1. Core Layer

```
┌─────────────────────────────────────────────────────────────────┐
│                         CORE LAYER                              │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │              ClassroomManager (Singleton)                │   │
│  ├─────────────────────────────────────────────────────────┤   │
│  │  Properties:                                             │   │
│  │  • studentAgents: List<StudentAgent>                     │   │
│  │  • sessionStartTime: float                               │   │
│  │  • sessionMetrics: SessionMetrics                        │   │
│  │  • currentScenario: ScenarioConfig                       │   │
│  │                                                          │   │
│  │  Methods:                                                │   │
│  │  • SpawnStudents(profiles)                               │   │
│  │  • ExecuteAction(action, student)                        │   │
│  │  • CalculateMetrics()                                    │   │
│  │  • EndSession()                                          │   │
│  │  • LogToDatabase()                                       │   │
│  └─────────────────────────────────────────────────────────┘   │
│                                                                 │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │              GameInitializer                             │   │
│  ├─────────────────────────────────────────────────────────┤   │
│  │  • ValidateReferences()                                  │   │
│  │  • InitializeScene()                                     │   │
│  │  • SetupDebugControls()                                  │   │
│  └─────────────────────────────────────────────────────────┘   │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

### 2. Student AI System

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                           STUDENT AI SYSTEM                                  │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │                        StudentAgent                                  │   │
│  ├─────────────────────────────────────────────────────────────────────┤   │
│  │                                                                      │   │
│  │   ┌──────────────────┐    ┌──────────────────┐                      │   │
│  │   │  Personality     │    │  Behavioral FSM  │                      │   │
│  │   │  ─────────────   │    │  ─────────────── │                      │   │
│  │   │  • extroversion  │    │  • LISTENING     │                      │   │
│  │   │  • sensitivity   │    │  • ENGAGED       │                      │   │
│  │   │  • rebelliousness│    │  • DISTRACTED    │                      │   │
│  │   │  • academicMotiv │    │  • SIDE_TALK     │                      │   │
│  │   └──────────────────┘    │  • ARGUING       │                      │   │
│  │                           │  • WITHDRAWN     │                      │   │
│  │                           └──────────────────┘                      │   │
│  │                                    │                                 │   │
│  │                    ┌───────────────┴───────────────┐                │   │
│  │                    ▼                               ▼                │   │
│  │          ┌──────────────────┐           ┌──────────────────┐        │   │
│  │          │  EmotionVector   │           │  NavMesh Agent   │        │   │
│  │          │  ──────────────  │           │  ──────────────  │        │   │
│  │          │  happiness: 1-10 │           │  • Move to seat  │        │   │
│  │          │  sadness: 1-10   │           │  • Walk to board │        │   │
│  │          │  frustration:1-10│           │  • Return path   │        │   │
│  │          │  boredom: 1-10   │           └──────────────────┘        │   │
│  │          │  anger: 1-10     │                                       │   │
│  │          └──────────────────┘                                       │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
│                                                                             │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │                   StudentAIResponseGenerator                         │   │
│  ├─────────────────────────────────────────────────────────────────────┤   │
│  │                                                                      │   │
│  │   ┌──────────────┐      ┌──────────────┐      ┌──────────────┐      │   │
│  │   │ HuggingFace  │─────▶│   Response   │─────▶│    Cache     │      │   │
│  │   │    API       │      │   Generator  │      │   Manager    │      │   │
│  │   └──────────────┘      └──────────────┘      └──────────────┘      │   │
│  │          │                     │                                     │   │
│  │          ▼                     ▼                                     │   │
│  │   ┌──────────────┐      ┌──────────────┐                            │   │
│  │   │   Fallback   │      │  Emotion     │                            │   │
│  │   │  Rule-Based  │      │  Modifier    │                            │   │
│  │   └──────────────┘      └──────────────┘                            │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

### 3. Emotion System

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                          EMOTION VECTOR SYSTEM                               │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│   EMOTION DIMENSIONS                    DYNAMIC BEHAVIOR                    │
│   ──────────────────                    ────────────────                    │
│                                                                             │
│   Happiness  ████████░░ 8               • Naturally decreases over time    │
│   Sadness    ██░░░░░░░░ 2               • Fades naturally                  │
│   Frustration███░░░░░░░ 3               • Context-dependent                │
│   Boredom    █████░░░░░ 5               • Increases without engagement     │
│   Anger      █░░░░░░░░░ 1               • Subsides naturally               │
│                                                                             │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│   TEACHER ACTION EFFECTS                                                    │
│   ──────────────────────                                                    │
│                                                                             │
│   Action              │ Happiness │ Sadness │ Frustration │ Boredom │ Anger │
│   ────────────────────┼───────────┼─────────┼─────────────┼─────────┼───────│
│   Yell                │    ↓      │    ↑    │      ↑      │    ↓    │   ↑   │
│   Praise              │    ↑↑     │    ↓    │      ↓      │    ↓    │   ↓   │
│   CallToBoard         │    ─      │    ↑    │      ─      │    ↓↓   │   ─   │
│   GiveBreak           │    ↑      │    ↓    │      ↓↓     │    ↓↓↓  │   ↓   │
│   RemoveFromClass     │    ↓↓     │    ↑↑   │      ↑      │    ─    │   ↑   │
│   PositiveReinforce   │    ↑      │    ↓    │      ↓      │    ↓    │   ↓   │
│                                                                             │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│   PEER CONTAGION                                                            │
│   ──────────────                                                            │
│                                                                             │
│   • Students within 3-unit radius influence each other                     │
│   • High negative emotions spread to nearby students                       │
│   • Positive emotions also propagate (but slower)                          │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

### 4. Teacher Interaction System

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                        TEACHER INTERACTION SYSTEM                           │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│    ┌─────────────────────────────────────────────────────────────────┐     │
│    │                        TeacherUI                                 │     │
│    ├─────────────────────────────────────────────────────────────────┤     │
│    │                                                                  │     │
│    │   INPUT METHODS                      OUTPUT DISPLAYS             │     │
│    │   ─────────────                      ───────────────             │     │
│    │                                                                  │     │
│    │   ┌──────────────┐                  ┌──────────────┐            │     │
│    │   │ Click Select │                  │ Engagement % │            │     │
│    │   │   Student    │                  │    Meter     │            │     │
│    │   └──────────────┘                  └──────────────┘            │     │
│    │                                                                  │     │
│    │   ┌──────────────┐                  ┌──────────────┐            │     │
│    │   │Action Buttons│                  │ Disruption   │            │     │
│    │   │  (8 types)   │                  │   Counter    │            │     │
│    │   └──────────────┘                  └──────────────┘            │     │
│    │                                                                  │     │
│    │   ┌──────────────┐                  ┌──────────────┐            │     │
│    │   │    Voice     │                  │ Session Time │            │     │
│    │   │   Command    │                  │   Display    │            │     │
│    │   └──────────────┘                  └──────────────┘            │     │
│    └─────────────────────────────────────────────────────────────────┘     │
│                                                                             │
│    ┌─────────────────────────────────────────────────────────────────┐     │
│    │                      TeacherBagUI                                │     │
│    ├─────────────────────────────────────────────────────────────────┤     │
│    │                                                                  │     │
│    │   SPECIAL ITEMS                                                  │     │
│    │   ─────────────                                                  │     │
│    │                                                                  │     │
│    │   ┌────────┐  ┌────────┐  ┌────────┐  ┌────────┐               │     │
│    │   │ Ruler  │  │  Game  │  │  Book  │  │ Music  │               │     │
│    │   │ Strict │  │  Fun   │  │ Struct │  │ Calming│               │     │
│    │   └────────┘  └────────┘  └────────┘  └────────┘               │     │
│    │                                                                  │     │
│    └─────────────────────────────────────────────────────────────────┘     │
│                                                                             │
│    ┌─────────────────────────────────────────────────────────────────┐     │
│    │                 TeacherVoiceRecorderUI                           │     │
│    ├─────────────────────────────────────────────────────────────────┤     │
│    │                                                                  │     │
│    │   ┌──────────┐    ┌──────────┐    ┌──────────┐                 │     │
│    │   │ Web      │───▶│ Speech   │───▶│ Student  │                 │     │
│    │   │ Speech   │    │ to Text  │    │ Response │                 │     │
│    │   │ API      │    │ Convert  │    │ Trigger  │                 │     │
│    │   └──────────┘    └──────────┘    └──────────┘                 │     │
│    │                                                                  │     │
│    └─────────────────────────────────────────────────────────────────┘     │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

### 5. Session & Metrics System

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                        SESSION & METRICS SYSTEM                              │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│   ┌─────────────────────────────────────────────────────────────────────┐  │
│   │                    SessionFeedbackPanelUI                            │  │
│   ├─────────────────────────────────────────────────────────────────────┤  │
│   │                                                                      │  │
│   │   METRICS COLLECTED                    SCORE CALCULATION             │  │
│   │   ─────────────────                    ─────────────────             │  │
│   │                                                                      │  │
│   │   • Lesson Duration           ┌─────────────────────────────────┐   │  │
│   │   • Satisfied Students        │                                 │   │  │
│   │   • Interaction %             │  Final = (Engagement × 0.4) +   │   │  │
│   │   • Noise Handling Score      │          (LowDisrupt × 0.3) +   │   │  │
│   │   • Positive Actions          │          (PositiveInt × 0.2) +  │   │  │
│   │   • Negative Actions          │          (Efficiency × 0.1)     │   │  │
│   │   • Total Actions             │                                 │   │  │
│   │                               └─────────────────────────────────┘   │  │
│   │                                                                      │  │
│   │   GRADING SCALE                                                      │  │
│   │   ─────────────                                                      │  │
│   │                                                                      │  │
│   │   ┌─────────────────────────────────────────────────────────────┐   │  │
│   │   │ 90-100% │ Excellent │ Green  │ Outstanding management!      │   │  │
│   │   │ 70-89%  │ Good      │ Yellow │ Good work, room to improve   │   │  │
│   │   │ 50-69%  │ Needs Imp │ Orange │ Needs improvement            │   │  │
│   │   │  0-49%  │ Poor      │ Red    │ Requires practice            │   │  │
│   │   └─────────────────────────────────────────────────────────────┘   │  │
│   │                                                                      │  │
│   └─────────────────────────────────────────────────────────────────────┘  │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## Data Flow Architecture

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                            DATA FLOW DIAGRAM                                 │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│   USER INPUT                 PROCESSING                    OUTPUT           │
│   ──────────                 ──────────                    ──────           │
│                                                                             │
│   ┌──────────┐         ┌─────────────────┐         ┌──────────────┐        │
│   │  Mouse   │────────▶│                 │────────▶│ Visual       │        │
│   │  Click   │         │                 │         │ Feedback     │        │
│   └──────────┘         │                 │         └──────────────┘        │
│                        │   CLASSROOM     │                                  │
│   ┌──────────┐         │   MANAGER       │         ┌──────────────┐        │
│   │  Action  │────────▶│                 │────────▶│ Student      │        │
│   │  Button  │         │   • Process     │         │ State Change │        │
│   └──────────┘         │   • Validate    │         └──────────────┘        │
│                        │   • Execute     │                                  │
│   ┌──────────┐         │   • Log         │         ┌──────────────┐        │
│   │  Voice   │────────▶│                 │────────▶│ AI Response  │        │
│   │  Input   │         │                 │         │ Bubble       │        │
│   └──────────┘         └─────────────────┘         └──────────────┘        │
│                                 │                                           │
│                                 ▼                                           │
│                        ┌─────────────────┐                                  │
│                        │   BACKEND API   │                                  │
│                        │   (Logging)     │                                  │
│                        └─────────────────┘                                  │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## File Structure

```
Classroom_Project/
├── Assets/
│   ├── Scripts/
│   │   ├── Core/
│   │   │   ├── classroom_manager.cs      # Central game controller
│   │   │   └── game_initializer.cs       # Scene bootstrap
│   │   │
│   │   ├── Students/
│   │   │   ├── StudentAgent.cs           # Student behavior FSM
│   │   │   ├── EmotionVector.cs          # Emotion system
│   │   │   ├── StudentAIResponseGenerator.cs  # AI responses
│   │   │   ├── StudentSpawner.cs         # Student instantiation
│   │   │   └── StudentQuestionResponder.cs    # Hand-raising logic
│   │   │
│   │   ├── Teacher/
│   │   │   ├── TeacherUI.cs              # Main teacher controls
│   │   │   ├── TeacherBagUI.cs           # Special items
│   │   │   └── TeacherVoiceRecorderUI.cs # Voice commands
│   │   │
│   │   ├── UI/
│   │   │   ├── SessionFeedbackPanelUI.cs # End-session feedback
│   │   │   ├── StudentInfoPanelUI.cs     # Student details view
│   │   │   ├── StudentResponseBubble.cs  # Speech bubbles
│   │   │   ├── SeatSwapPanelUI.cs        # Seat management
│   │   │   └── BreakDurationPanelUI.cs   # Break controls
│   │   │
│   │   ├── Classroom/
│   │   │   ├── ClassroomBuilder.cs       # Environment setup
│   │   │   └── ClassroomFurnitureBuilder.cs  # Furniture placement
│   │   │
│   │   └── Utilities/
│   │       ├── auth_manager.cs           # Authentication
│   │       ├── camera_controller.cs      # Camera system
│   │       ├── scenario_loader.cs        # JSON config loader
│   │       └── student_visual_feedback.cs # Visual indicators
│   │
│   ├── Scenes/
│   │   ├── LoginScene.unity              # Entry point
│   │   ├── TeacherHomeScreen.unity       # Main menu
│   │   └── MainClassroom.unity           # Simulation
│   │
│   ├── Prefabs/
│   │   └── Students/                     # Student avatars
│   │
│   └── StreamingAssets/
│       └── Scenarios/
│           ├── scenario_basic_classroom.json
│           ├── scenario_easy_lesson.json
│           └── scenario_hard_science.json
│
├── ARCHITECTURE.md                       # This document
├── POSTER_PRESENTATION.md               # Poster content
└── README.md                            # Project overview
```

---

## External Integrations

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                         EXTERNAL INTEGRATIONS                                │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│   ┌─────────────────┐         ┌─────────────────┐                          │
│   │  HuggingFace    │         │   Web Speech    │                          │
│   │  Inference API  │         │   Recognition   │                          │
│   ├─────────────────┤         ├─────────────────┤                          │
│   │ Purpose:        │         │ Purpose:        │                          │
│   │ Generate AI     │         │ Convert teacher │                          │
│   │ student responses│        │ voice to text   │                          │
│   │                 │         │                 │                          │
│   │ Fallback:       │         │ Browser:        │                          │
│   │ Rule-based      │         │ WebGL/Chrome    │                          │
│   │ responses       │         │ required        │                          │
│   └─────────────────┘         └─────────────────┘                          │
│                                                                             │
│   ┌─────────────────┐         ┌─────────────────┐                          │
│   │  Backend API    │         │     Vercel      │                          │
│   │  (Custom)       │         │   (Hosting)     │                          │
│   ├─────────────────┤         ├─────────────────┤                          │
│   │ Purpose:        │         │ Purpose:        │                          │
│   │ • User auth     │         │ Host WebGL      │                          │
│   │ • Session logs  │         │ build for       │                          │
│   │ • Analytics     │         │ web access      │                          │
│   └─────────────────┘         └─────────────────┘                          │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## Design Patterns Used

| Pattern | Implementation | Location |
|---------|---------------|----------|
| **Singleton** | ClassroomManager | `classroom_manager.cs` |
| **Finite State Machine** | Student behaviors | `StudentAgent.cs` |
| **Observer** | UI updates on state changes | Throughout UI scripts |
| **Component** | Unity MonoBehaviour | All scripts |
| **Factory** | Student spawning | `StudentSpawner.cs` |
| **Strategy** | Different response generators | `StudentAIResponseGenerator.cs` |

---

## Key Algorithms

### 1. Emotion Decay Algorithm
```
Every frame:
  happiness -= decayRate * Time.deltaTime
  sadness -= decayRate * Time.deltaTime
  boredom += growthRate * Time.deltaTime (if not engaged)
  anger -= decayRate * Time.deltaTime
  frustration = context-dependent
```

### 2. Performance Score Algorithm
```
engagementScore = (listeningStudents + engagedStudents) / totalStudents
disruptionPenalty = 1 - (disruptions / maxDisruptions)
interventionBalance = positiveActions / (positiveActions + negativeActions)
efficiency = 1 - (totalActions / expectedActions)

finalScore = (engagementScore * 0.4) +
             (disruptionPenalty * 0.3) +
             (interventionBalance * 0.2) +
             (efficiency * 0.1)
```

### 3. Peer Contagion Algorithm
```
For each student:
  Find neighbors within 3-unit radius
  For each neighbor with high negative emotion:
    Increase own negative emotions by contagionRate
```

---

## Scalability Considerations

- **Student Count**: Designed for 8-12 students per scenario
- **Scenarios**: JSON-based, easily extendable
- **Actions**: Modular action system, new actions can be added
- **Languages**: Bilingual support built-in (Hebrew/English)
- **Platforms**: WebGL for web, extensible to mobile/VR

---

*This architecture document provides a comprehensive overview of the Classroom Challenges Simulator system design.*
