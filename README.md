# Unity Project Analyzer Tool

## 🧩 Overview

**Unity Project Analyzer** is a console tool that scans a Unity project directory and produces:
1. A **scene hierarchy dump** for each `.unity` scene file.
2. A list of all **unused scripts** (`.cs` files not referenced in any scene).
3. A list of **unused serializable scripts**, where serialized fields no longer exist in the source code (Bonus #1).

It helps developers clean up Unity projects, detect dead code, and inspect scene structures.

## ⚙️ Features

- ✅ Parses `.unity` scene files and builds full **object hierarchies**.
- ✅ Detects all **unused C# scripts**.
- ✅ (Bonus #1) Detects **serialized fields removed from C# code** using Roslyn.
- ✅ Outputs results in `.dump` (scene hierarchies) and `.csv` (unused scripts).
- ✅ Uses **YamlDotNet** for YAML parsing.
- ✅ (Bonus #2) Implements **parallel processing** for improved performance.

## 🚀 Usage
### Command-line syntax

``` bash
tool.exe <unity_project_path> <output_folder_path>
```

### Example
``` bash
tool.exe "C:\Projects\MyUnityGame" "C:\AnalysisOutput"
```

## 📄 Output Files
### 1. Scene Hierarchy Dumps (`.dump`)

For each `.unity` scene file found under `Assets/`, the tool generates a `.dump` file describing the full scene hierarchy.

### Example:
``` bash
C:\AnalysisOutput\SampleScene.unity.dump
```

This file shows all GameObjects and their child relationships

``` bash
Main Camera
UIDocument
ABC
--1
----1.1
----1.2
----1.3
--2
--3
----3.1
------3.2
--------3.3
--4
```

### 2. Unused Scripts Report (`UnusedScripts.csv`)

Lists all `.cs` files under `Assets/` that are **not referenced** by any scene, or are missing serialized fields (Bonus #1).

**Example content:**
``` bash
Relative Path,GUID
Assets/Scripts/ObsoleteController.cs,97bdf9aa3df241348a88c71f6049a92b
Assets/Scripts/LegacyUIManager.cs,12aacf2345f3aa84385a08b7dabb2b99
```

## 🧠 How It Works

1. **Scene Parsing** 

    Each `.unity` scene file is parsed using `YamlDotNet`.MonoBehaviour components are extracted and mapped by their `Script GUID`.

2. **Script Analysis**

    The tool scans all `.cs` files under `Assets/`, collecting GUIDs.
    Unused scripts are identified if their GUIDs don’t appear in any scene.

3. **Serializable Field Validation (Bonus #1)**

    For each used script, the tool parses its C# source with **Roslyn** to detect `[SerializeField]` and `public` fields.
    If serialized fields referenced in the scene are missing in the code, the script is flagged as “unused.”

4. **Parallel Execution (Bonus #2)**

    Scene and script analysis run concurrently using `Parallel.ForEach` and thread-safe collections (`ConcurrentBag`, `ConcurrentDictionary`), improving performance on multi-core CPUs.

## 🧱 Dependencies

**Library**	| **Purpose**
--------|--------
**YamlDotNet** | Parses Unity .unity scene YAML files
**Microsoft.CodeAnalysis.CSharp (Roslyn)** | Parses C# scripts for serialized field detection
**System.Threading.Tasks / Parallel LINQ** | Enables parallel and asynchronous processing

Install via NuGet:
``` bash
dotnet add package YamlDotNet
dotnet add package Microsoft.CodeAnalysis.CSharp
```

## 🧰 Example Workflow

``` bash
> tool.exe "C:\UnityProjects\MyGame" "C:\UnityAnalysis"

[INFO] Analyzing Unity project...
[INFO] Found 4 scene files
[INFO] Found 126 C# scripts
[WARN] MonoBehaviour '0976ed63b1edde14a9720f33969db853' in script 'EnemyController.cs' is missing serialized field 'health'
Parallel analysis took 2125 ms

Output written to: C:\UnityAnalysis
```

## 📦 Output Summary
File | Description
-----|------------
`*.dump` | Scene hierarchy text dump for each scene
`UnusedScripts.csv` | CSV list of unused or invalid scripts