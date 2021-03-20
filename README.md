# CameraPlus
CameraPlus is a Beat Saber mod that allows for multiple wide FOV cameras with smoothed movement, which makes for a much more pleasant overall spectator experience.

ModAssistant has released [Camera2](https://github.com/kinsi55/CS_BeatSaber_Camera2), which is newly designed and lighter.
Therefore, CameraPlus is no longer registered in ModAssistant.
This is the version where I will add the features I want without permission.

# Latest version Download
The version here may be a version that has not yet been registered with ModAssistant, or a version that will not be registered.
[Release Page](https://github.com/Snow1226/CameraPlus/releases)
### To install manually:
	1. Make sure that Beat Saber is not running.
	2. Extract the contents of the zip into Beat Saber's installation folder.
		For Oculus Home: \Oculus Apps\Software\hyperbolic-magnetism-beat-saber\
		For Steam: \steamapps\common\Beat Saber\
		(The folder that contains Beat Saber.exe)
	3. Done! You've installed the CameraPlus Plugin.

### When using CameraPlus, "SmoothCamera" is disabled in the base game.
The latest version will automatically force SmoothCamera to be turned off, ignoring the game's settings.

# Usage
To edit the settings of any camera in real time, right click on the Beat Saber game window! A context menu will appear with options specific to the camera that you right clicked on!

Press <kbd>F1</kbd> to toggle the main camera between first and third person.

# Configuration file description
## UserData/CameraPlus.ini
[CameraPlus.ini in wiki](https://github.com/Snow1226/CameraPlus/wiki/Configuration-file-description-CameraPlus.ini)

## CameraConfig
[CameraConfig in wiki](https://github.com/Snow1226/CameraPlus/wiki/Configuration-file-description-*.cfg)

## Movement Script
[Movement Script in wiki](https://github.com/Snow1226/CameraPlus/wiki/MovementScript)

## If you need help, ask us at the Beat Saber Mod Group Discord Server:  
https://discord.gg/BeatSaberMods

## For developers

### Contributing to CameraPlus
In order to build this project, please create the file `CameraPlus.csproj.user` and add your Beat Saber directory path to it in the project directory.
This file should not be uploaded to GitHub and is in the .gitignore.

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <!-- Set "YOUR OWN" Beat Saber folder here to resolve most of the dependency paths! -->
    <BeatSaberDir>E:\Program Files (x86)\Steam\steamapps\common\Beat Saber</BeatSaberDir>
  </PropertyGroup>
</Project>
```

If you plan on adding any new dependencies which are located in the Beat Saber directory, it would be nice if you edited the paths to use `$(BeatSaberDir)` in `CameraPlus.csproj`

```xml
...
<Reference Include="BS_Utils">
  <HintPath>$(BeatSaberDir)\Plugins\BS_Utils.dll</HintPath>
</Reference>
<Reference Include="IPA.Loader">
  <HintPath>$(BeatSaberDir)\Beat Saber_Data\Managed\IPA.Loader.dll</HintPath>
</Reference>
...
```
