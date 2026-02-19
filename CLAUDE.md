# CLAUDE.md — FRL-UnityScripts

This file describes the codebase structure, conventions, and development guidelines for AI assistants working on this repository.

---

## Repository Overview

**FRL-UnityScripts** is a collection of reusable Unity C# MonoBehaviour scripts maintained by **Future Realities Lab (FRL)**. The scripts provide event-driven interaction utilities that can be wired up entirely in the Unity Inspector without writing additional code. They are intended to be dropped onto GameObjects in Unity scenes to handle common interaction patterns: collision/trigger detection, keyboard input, scene loading, and spatial audio playback.

This is a **scripts-only** repository — there is no Unity project, no Assets folder, and no scene files. The scripts are meant to be copied into Unity projects.

---

## Repository Structure

```
FRL-UnityScripts/
├── EventInvokationCollision.cs   # Physics collision events (Rigidbody-based)
├── EventInvokationKeyCode.cs     # Keyboard input events
├── EventInvokationTrigger.cs     # Physics trigger events (older, tag comparison style)
├── LoadSceneEvents.cs            # Scene management helpers
├── TriggerColliderEvents.cs      # Unified trigger/collision events (newer, preferred)
├── TriggeredAudioPlayer.cs       # Proximity-based spatial audio player
├── .gitattributes                # Unity LFS and YAML merge settings
└── .gitignore                    # Standard Unity gitignore
```

There is no build system, package manager, or test runner. Scripts are compiled by the Unity Editor when imported into a project.

---

## Script Descriptions

### EventInvokationCollision.cs
Listens for physics **collision** events (requires Rigidbody on the colliding object) and exposes three `UnityEvent` callbacks in the Inspector: `OnCollisionEnterEvent`, `OnCollisionStayEvent`, and `OnCollisionExitEvent`.

- `EventInvokationCollisionActive` (bool): master on/off toggle
- `ColliderTag` (string): optional tag filter; leave empty to react to all collisions
- Uses `string.IsNullOrEmpty` and `CompareTag` for tag comparison (preferred pattern)

### EventInvokationKeyCode.cs
Listens for keyboard input each frame via `Update()` and exposes three `UnityEvent` callbacks: `OnKeyDownEvent`, `OnKeyHoldEvent`, `OnKeyUpEvent`.

- `EventInvokationKeyCodeActive` (bool): declared but **not checked in Update** — this is a known inconsistency; the toggle has no effect at runtime
- `ActivationKey` (KeyCode): the key to listen for

### EventInvokationTrigger.cs
Listens for physics **trigger** events and exposes three `UnityEvent` callbacks: `OnTriggerEnterEvent`, `OnTriggerStayEvent`, `OnTriggerExitEvent`.

- `EventInvokationTriggerActive` (bool): declared but **not checked in trigger methods** — same inconsistency as `EventInvokationKeyCode`
- `ColliderTag` (string): uses `ColliderTag == ""` (older pattern, inconsistent with `EventInvokationCollision`)
- **Prefer `TriggerColliderEvents.cs` for new work** — it is the more complete, consistent replacement

### LoadSceneEvents.cs
Provides public methods for scene navigation, designed to be called from UnityEvent callbacks or UI buttons.

- `LoadSceneByName(string)`: loads a scene by name if not null
- `LoadNextScene()`: loads the next scene by build index; wraps to index 0 at the end
- `LoadPreviousScene()`: **contains a bug** — uses `buildIndex + 1` instead of `- 1`, and the comparison direction is inverted; in practice it wraps to index 0 in most conditions

### TriggerColliderEvents.cs
A unified, more polished replacement for `EventInvokationTrigger.cs` and `EventInvokationCollision.cs`. Handles both trigger and collision modes from a single component.

- `useTrigger` (bool): `true` = trigger mode, `false` = collision mode
- `eventActive` (bool): master toggle that is **correctly checked** in all handlers
- `filterTag` (string): uses `string.IsNullOrEmpty` (preferred pattern)
- Helper method `IsTagValid(string)` centralizes tag logic
- Exposes `OnEnterEvent`, `OnStayEvent`, `OnExitEvent`

### TriggeredAudioPlayer.cs
Plays an `AudioSource` when a tagged collider enters a configurable sphere radius. Pauses playback when the target moves beyond `audioSource.maxDistance`. Adds a `SphereCollider` at runtime in `Start()`.

- `[RequireComponent(typeof(AudioSource))]` enforced
- `ColliderTag` defaults to `"Player"`
- `triggerRadius`: radius of the auto-generated sphere trigger
- `rewindOnPause` / `rewindSeconds`: optionally rewinds audio when paused
- `resetOnTrigger`: if false, resumes rather than restarts already-paused audio
- `is3DAudio`: toggles `spatialBlend` between 1.0 (3D) and 0.0 (2D)
- `minAudioDistance` / `maxAudioDistance`: 3D audio falloff distances (linear rolloff mode)
- `OnTriggerExit` is stubbed out (empty body)
- Uses `other.tag == ColliderTag` (older string comparison pattern)

---

## Code Conventions

### Style
- **No namespaces** — all classes are in the global namespace
- **PascalCase** for class names, public fields, and methods
- **camelCase** for private fields
- Public fields are used for Inspector-exposed properties (no `[SerializeField]` on private fields in this codebase)
- `[Header("...")]` and `[Space(N)]` attributes used to organize the Inspector
- `[RequireComponent(typeof(...))]` used where a dependency is always required

### Tag Comparison
Two patterns exist in this codebase. Prefer the newer pattern for any new scripts:

```csharp
// Older pattern (EventInvokationTrigger, TriggeredAudioPlayer)
if ((ColliderTag == "") || other.tag == ColliderTag)

// Newer/preferred pattern (EventInvokationCollision, TriggerColliderEvents)
if (string.IsNullOrEmpty(filterTag) || other.CompareTag(filterTag))
```

`CompareTag` is preferred because it avoids GC allocations and will throw a descriptive error if the tag doesn't exist in the project.

### UnityEvent Pattern
All event-dispatch scripts follow this pattern:
1. Declare public `UnityEvent` fields with descriptive names
2. Check an active/enabled guard at the top of each handler
3. Check optional tag filter
4. Invoke the event

### Known Issues / Bugs
| File | Issue |
|------|-------|
| `EventInvokationKeyCode.cs` | `EventInvokationKeyCodeActive` field is not read in `Update()`; toggle has no effect |
| `EventInvokationTrigger.cs` | `EventInvokationTriggerActive` field is not read in trigger methods |
| `LoadSceneEvents.cs` | `LoadPreviousScene()` uses `buildIndex + 1` and inverted comparison; effectively wraps to scene 0 in most cases |
| `TriggeredAudioPlayer.cs` | `OnTriggerExit` is empty; `minAudioDistance` default equals `maxAudioDistance` (both 15f) |

Do not silently fix these bugs when making unrelated changes — note them or create a separate commit so changes are traceable.

---

## Git Configuration

### Branching
- `master` is the main branch
- Feature branches follow the pattern `claude/<description>-<id>`

### Git LFS
Binary assets (audio, textures, 3D models, fonts, archives) are tracked via **Git LFS** as configured in `.gitattributes`. When working in a context that does not have LFS configured, do not commit binary files.

### Unity YAML Merge
Scene, prefab, and other Unity YAML files are configured to use **UnityYAMLMerge** as the merge driver. This is set in `.gitattributes` via the `unity-yaml` macro. Do not attempt to manually resolve merge conflicts in `.unity`, `.prefab`, or `.asset` files without this tool.

### Commit Style
Commits in this repository use brief GitHub-style messages:
- `Create <FileName>.cs`
- `Add files via upload`

Keep commit messages concise and descriptive of the change.

---

## Development Workflow

Since there is no Unity project in this repository, the typical workflow is:

1. Edit or create `.cs` script files in this repo
2. Copy or symlink the scripts into the `Assets/` folder of a Unity project for testing
3. Test in the Unity Editor (Play Mode) or on-device
4. Commit and push changes back to this repo

There is no CI pipeline, no automated tests, and no linter configured. Code correctness is validated manually in the Unity Editor.

---

## Adding New Scripts

When adding a new MonoBehaviour script:

1. Place the `.cs` file at the repository root (all scripts are flat, no subdirectories)
2. Follow the naming convention: descriptive PascalCase, e.g., `TriggerColliderEvents.cs`
3. Use the preferred tag comparison pattern (`string.IsNullOrEmpty` + `CompareTag`)
4. Include an `active` / `enabled` boolean toggle and check it in all Unity callback methods
5. Annotate Inspector fields with `[Header()]` and `[Space()]` for usability
6. Use `[RequireComponent(typeof(...))]` for hard dependencies
7. Avoid namespaces to remain consistent with the existing codebase

---

## Relationship Between Scripts

`TriggerColliderEvents.cs` is the **recommended** modern equivalent for both `EventInvokationTrigger.cs` and `EventInvokationCollision.cs`. The older scripts are retained for backward compatibility with existing Unity scenes that already reference them.

```
Older (kept for compatibility)         Newer (preferred)
─────────────────────────────    →    ──────────────────────────────
EventInvokationTrigger.cs              TriggerColliderEvents.cs
EventInvokationCollision.cs            (useTrigger = false mode)
```
