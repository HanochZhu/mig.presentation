# Mig Presentation Framework

`com.mig.presentation` is the presentation and project-data package for MigSpace. It sits above the core and model layers and handles project aggregation, deserialization, snapshot-based playback state, and presentation-oriented element management.

## Package Info

- Package name: `com.mig.presentation`
- Display name: `Mig Presentation framework`
- Current version: `0.0.1`
- Unity version in `package.json`: `2019.4`

## What This Package Provides

This package focuses on project-level presentation workflows, including:

- collecting scene data into a project structure
- restoring project data back into the scene
- managing project names and local project lists
- deserializing packaged project files
- attaching and retrieving step-based Mig elements for presentation states
- JSON converters for Unity-specific data types

## Main Modules

### `Runtime/ProjectManager`

`ProjectManager` is the main orchestration class for project-level workflows.

It is responsible for:

- collecting scene content into `ProjectData`
- applying stored project data back to a loaded model root
- tracking the current project name
- listing locally cached projects
- downloading project files from remote storage
- triggering deserialization flows

### `Runtime/ProjectManager/ProjectData`

Defines the data model used to represent a Mig project.

This object aggregates:

- snapshot step data
- serialized `MigElement` collections

It acts as the transport structure between authoring, storage, and playback flows.

### `Runtime/Deserialize`

Contains deserialization tasks and helpers for turning stored project packages back into runtime content.

Key classes include:

- `ProjectDeserializer`
- `DeserializeProjectTask`

These classes handle extracting packaged project files and preparing them for use in the application.

### `Runtime/MigElementManager`

Provides helper methods for working with step-based elements on scene objects.

This module helps:

- retrieve or create the current-step element for a given object
- ensure a default element exists
- bind elements to `MigElementWrapper`
- initialize elements with the correct object path and snapshot identifier

### `Runtime/JsonExternal`

Contains JSON converters for Unity data types used during project serialization.

Examples include:

- `JsonColorConverter`
- `QuaternionJsonConverter`
- `Texture2DJsonConverter`
- `Vector3JsonConverter`

### `Runtime/Misc`

Contains utility classes used by the presentation workflow, such as:

- `PathManager`

## Folder Structure

```text
Packages/mig.presentation
|- package.json
|- README.md
|- Runtime
   |- Deserialize
   |- JsonExternal
   |- Misc
   |- ProjectManager
   |- MigElementManager.cs
   |- com.mig.presentation.asmdef
```

## Assembly

This package currently includes one runtime assembly:

- `com.mig.presentation`

## How To Use

### Install through `manifest.json`

Add the package to your Unity project's `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.mig.presentation": "git@github.com:HanochZhu/mig.presentation.git"
  }
}
```

### Use in code

After Unity resolves the package, you can reference it in your scripts:

```csharp
using Mig;
```

### Typical responsibilities

This package is generally used together with:

- `com.mig.core` for shared element, snapshot, and infrastructure support
- `com.mig.model` for loaded model roots and model-state integration

## Development Notes

- This package is built around project reconstruction and presentation playback workflows.
- Several runtime flows assume the presence of cached files, snapshot textures, and model roots prepared by other Mig systems.
- Deserialization currently focuses on unpacking project archives and rehydrating runtime data structures.

## License

This package follows the license terms used by the main MigSpace repository.
