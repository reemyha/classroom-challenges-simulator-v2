# Classroom Challenges Simulator
## Project Poster Presentation

---

## Project Title
**The Class Simulator - AI-Powered Teacher Training Platform**

---

## Created By
**Reemy Halabi & Michael Trifonov**

---

## Project Overview

A **3D educational simulation** that allows teachers to practice classroom management skills in a safe, virtual environment. AI-controlled students with realistic emotions and behaviors provide real-time feedback on teaching effectiveness.

---

## Problem Statement

- New teachers struggle with classroom management
- Real-world practice opportunities are limited
- Mistakes in real classrooms affect actual students
- No safe environment to experiment with different teaching strategies

---

## Our Solution

An immersive **classroom simulator** where teachers can:
- Practice handling disruptive behaviors
- Learn to recognize student emotional states
- Experiment with different intervention strategies
- Receive immediate feedback on their performance

---

## Key Features

### 1. Realistic AI Students
- **6 Behavioral States**: Listening, Engaged, Distracted, Side-talking, Arguing, Withdrawn
- **Unique Personalities**: Each student has traits (extroversion, sensitivity, rebelliousness, academic motivation)
- **Dynamic Emotions**: 5-dimensional emotion system (Happiness, Sadness, Frustration, Boredom, Anger)

### 2. Teacher Intervention Tools
| Action | Purpose |
|--------|---------|
| Praise | Positive reinforcement |
| Call to Board | Encourage participation |
| Give Break | Reduce stress/boredom |
| Change Seating | Separate disruptions |
| Special Items | Ruler, Game, Book, Music |

### 3. Voice Command System
- Speak directly to virtual students
- AI generates contextual responses
- Bilingual support (Hebrew/English)

### 4. Real-Time Metrics
- Engagement Score
- Disruption Counter
- Intervention Balance
- Overall Performance Score

---

## Technologies Used

| Technology | Purpose |
|------------|---------|
| **Unity 3D** | Game Engine & 3D Rendering |
| **C#** | Core Programming (35 scripts) |
| **NavMesh AI** | Student Pathfinding |
| **HuggingFace API** | AI Response Generation |
| **Web Speech API** | Voice Recognition |
| **WebGL** | Web Deployment |
| **Vercel** | Cloud Hosting |

---

## System Architecture (High-Level)

```
┌─────────────────────────────────────────────────────────────────┐
│                    CLASSROOM SIMULATOR                          │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌─────────────┐    ┌─────────────┐    ┌─────────────┐        │
│  │   Teacher   │───▶│  Classroom  │◀───│   Student   │        │
│  │     UI      │    │   Manager   │    │   Agents    │        │
│  └─────────────┘    └──────┬──────┘    └─────────────┘        │
│                            │                                    │
│  ┌─────────────┐    ┌──────▼──────┐    ┌─────────────┐        │
│  │   Voice     │    │   Session   │    │   Emotion   │        │
│  │  Commands   │    │   Metrics   │    │   System    │        │
│  └─────────────┘    └─────────────┘    └─────────────┘        │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

---

## Student Emotion Model

```
         EMOTION VECTOR (1-10 Scale)
┌────────────────────────────────────────┐
│  Happiness  ████████░░  8              │
│  Sadness    ██░░░░░░░░  2              │
│  Frustration███░░░░░░░  3              │
│  Boredom    █████░░░░░  5              │
│  Anger      █░░░░░░░░░  1              │
└────────────────────────────────────────┘

Emotions naturally decay/grow and respond to:
• Teacher actions (praise ↑happiness, yell ↑anger)
• Peer influence (nearby students affect each other)
• Time passage (boredom increases without engagement)
```

---

## Student Behavioral State Machine

```
                    ┌──────────────┐
                    │   LISTENING  │
                    └──────┬───────┘
                           │
            ┌──────────────┼──────────────┐
            ▼              ▼              ▼
    ┌───────────┐   ┌───────────┐   ┌───────────┐
    │  ENGAGED  │   │ DISTRACTED│   │ WITHDRAWN │
    └───────────┘   └─────┬─────┘   └───────────┘
                          │
               ┌──────────┴──────────┐
               ▼                     ▼
        ┌───────────┐         ┌───────────┐
        │ SIDE_TALK │         │  ARGUING  │
        └───────────┘         └───────────┘
```

---

## Performance Scoring Algorithm

```
Final Score =
    (Engagement × 40%) +
    (Low Disruption × 30%) +
    (Positive Interventions × 20%) +
    (Efficiency × 10%)
```

**Score Interpretation:**
- 🟢 **90-100%** - Excellent classroom management
- 🟡 **70-89%** - Good, room for improvement
- 🟠 **50-69%** - Needs improvement
- 🔴 **0-49%** - Requires significant practice

---

## Demo Scenarios

1. **Basic Classroom** - 9th grade math, morning lesson
2. **Easy Lesson** - Introductory scenario for new users
3. **Hard Science** - Challenging scenario with difficult students

---

## User Flow

```
Login → Select Scenario → Enter Classroom →
Manage Students → End Session → View Feedback → Repeat
```

---

## Future Enhancements

- [ ] More classroom scenarios
- [ ] Multiplayer observation mode
- [ ] VR support for immersive training
- [ ] Machine learning for adaptive difficulty
- [ ] Student learning progress tracking

---

## Project Impact

- **Safe Practice Environment** - No real students affected
- **Immediate Feedback** - Learn from every decision
- **Repeatable Scenarios** - Practice until mastery
- **Data-Driven Insights** - Track improvement over time

---

## Contact

**Reemy Halabi & Michael Trifonov**

*Classroom Challenges Simulator v2*

---

## QR Code

[Add QR code linking to deployed WebGL version on Vercel]

---

*"Practice makes perfect - especially in a virtual classroom!"*
