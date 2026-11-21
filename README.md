# 🐦 Bird Blast — Angry Birds Clone (FGU301 Final Project)

> **Final Course Project for FGU301 — FPT University**  
> A 2D physics-based slingshot puzzle game inspired by *Angry Birds*, built with **Unity 6 (C#)**.

[![Play on itch.io](https://img.shields.io/badge/Play%20on-itch.io-FA5C5C?style=for-the-badge&logo=itchdotio&logoColor=white)](https://takumi-ji.itch.io/bird-blast)

🎮 **Play Now:** [Bird Blast by takumi_ji on itch.io](https://takumi-ji.itch.io/bird-blast)

---

## 📖 Overview

**Bird Blast** is an academic project developed for the **FGU301** course at **FPT University**. Players use a slingshot to launch birds with unique abilities to demolish structures and eliminate all pigs across 6 progressively challenging levels.

The project demonstrates core 2D Unity game development concepts, including 2D physics (`Physics2D`), object-oriented programming (inheritance & interfaces), `ScriptableObject` data architecture, dynamic camera control with **Cinemachine 3**, the **Unity Input System**, audio mixing via **AudioMixer**, and a local JSON save & leaderboard system.

---

## ✨ Key Features

### 🎯 Gameplay Mechanics
- **Slingshot & Trajectory Preview:** Drag and release to aim and adjust launch power, featuring elastic slingshot bands (`LineRenderer`) and a physics-based trajectory arc.
- **4 Playable Bird Types with Unique Abilities:**
  | Bird Type | Special Ability (activated mid-flight) |
  | :--- | :--- |
  | **Red Bird** | Classic bird with no active ability. |
  | **Yellow Bird** | Triggers a sudden speed boost along its current flight direction to smash through structures. |
  | **Blue Bird** | Splits into 3 birds spreading in a fan pattern (`±15°`), ideal for wide-area coverage. |
  | **Black Bird** | Bomb bird — detonates to deal area-of-effect damage and knockback when triggered manually, or automatically explodes `1.5s` after impact. |
- **Enemies & Destructible Objects:**
  - **3 Pig Variants:** Regular Pig, Helmet Pig, and King Pig — dynamically update sprites based on remaining health.
  - **3 Structure Materials:** Wood, Glass, and Stone blocks/boxes, each with distinct durability, impact audio, and break particle effects.
  - **TNT Box:** Explodes upon taking sufficient impact damage, triggering chain reactions with radial damage and force.

### 🎮 Systems & Polish
- **6 Levels (`Level 1` – `Level 6`):** Sequentially unlocked by clearing the previous level; level thresholds and bonuses are configured via `ScriptableObject` (`LevelData`).
- **Dynamic Camera (Cinemachine 3):** Pans to preview pigs at the start of each level (skippable by clicking), follows launched birds in flight, holds focus on the target area after impact, and smoothly returns to the slingshot.
- **Slow-Motion Finisher:** Triggers a dramatic slow-motion effect (`0.25x` timescale) when the final pig in a level is defeated.
- **Combo & Scoring System:**
  - Consecutive destructions within `1.5s` build a combo multiplier: **x1.5** (3+ hits), **x2** (6+ hits), and **x3** (10+ hits).
  - Unused birds grant bonus points upon level completion (`birdBonus`).
  - **1 to 3-star** rating per level, with high-score tracking and a `NEW HIGHEST SCORE` highlight.
- **Player Profiles & Local Leaderboard:**
  - Supports entering and switching player names (3–12 characters), with a confirmation prompt if an existing profile name is entered.
  - Persists level progress, stars, high scores, and leaderboard entries locally in JSON format at `Application.persistentDataPath/save.json`.
  - Overall leaderboard ranked by total score and total stars.
- **Audio & Settings:** Independent Music and SFX volume sliders routed through `AudioMixer`, saved via `PlayerPrefs`.

---

## 🕹️ Controls

| Action | Description |
| :--- | :--- |
| **Click & Drag (Left Mouse)** on the bird | Aim direction and adjust launch power |
| **Release Left Mouse** | Launch the bird |
| **Left Click** mid-flight | Activate special ability (Yellow / Blue / Black Bird) |
| **Left Click** during level intro | Skip intro camera pan |
| **HUD Pause Button** | Pause, restart level, or return to Main Menu |

---

## 🛠️ Tech Stack

- **Game Engine:** Unity `6000.3.11f1` (Unity 6)
- **Language:** C#
- **Render Pipeline:** Universal Render Pipeline 2D (`URP 17.3.0`)
- **Key Packages:**
  - `com.unity.cinemachine` (`3.1.7`)
  - `com.unity.inputsystem` (`1.19.0`)
  - `com.unity.ugui` (`2.0.0`) & TextMeshPro
  - `com.unity.2d.sprite`, `com.unity.2d.tilemap`, `com.unity.2d.animation`

---

## 📂 Project Structure

```text
Assets/_Project/
├── Arts/               # Character & block spritesheets, backgrounds, PhysicsMaterial2D
├── Audios/             # Background music, SFX clips, and AudioMixer (Master.mixer)
├── Effects/            # Textures & materials for fire, explosion, smoke, and spark VFX
├── Levels/             # ScriptableObject configs for all 6 levels (Level1.asset -> Level6.asset)
├── Prefabs/            # Bird, pig, block, TNT, VFX, and UI panel prefabs
├── Scenes/             # MainMenu, LevelSelect, and Level 1 -> Level 6
├── ScriptableObjects/  # LevelData.cs definition
├── Scripts/
│   ├── Bird/           # Bird.cs (base class), BlackBird.cs, BlueBird.cs, YellowBird.cs
│   ├── Common/         # Destructible, IDamageable, Explosion, SlowMotion, Volume, MainMenu...
│   ├── System/         # LevelManager, ScoreManager, ComboSystem, SaveSystem, Sound/MusicManager...
│   ├── UI/             # HudUI, ResultUI, LeaderboardUI, NameEntryUI
│   ├── Pig.cs
│   ├── Slingshot.cs
│   └── TntBox.cs
└── UI/                 # Icons, wood UI panels, and fonts
```

---

## 🚀 Getting Started

### Play via itch.io
You can play/download the published build directly here:  
👉 **[Bird Blast by takumi_ji](https://takumi-ji.itch.io/bird-blast)**

### Run from Source (Unity Editor)
1. Install **Unity Hub** and **Unity `6000.3.11f1`** (recommended to match the project version and avoid package compatibility issues).
2. Clone or download this repository:
   ```bash
   git clone <repository-url>
   ```
3. Open **Unity Hub** → click **Add** → select the `FGU301_Final Project` directory.
4. Open the project in the Unity Editor.
5. In the Project window, open the starting scene:
   ```text
   Assets/_Project/Scenes/MainMenu.unity
   ```
6. Press **Play** to test the game (or open **File → Build Profiles** to build a standalone executable; all 8 scenes are pre-configured in `EditorBuildSettings`).

---

## 🎨 Credits & Disclaimer

- **Purpose:** This is a non-commercial academic project created for the **FGU301** course at **FPT University**.
- **Inspiration:** Core gameplay mechanics are inspired by *Angry Birds*.
- **Assets & Resources:**
  - Audio clips, sprites, icons, and fonts used in this project were sourced by the author from open and free community resources.
  - Selected art assets were generated with the assistance of **Claude AI**.
- All rights to any third-party assets belong to their respective original creators.
