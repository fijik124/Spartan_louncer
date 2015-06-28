<h1 align="center">11th Marine Expeditionary Unit Launcher</h1>
<p align="center">
	<img src="https://raw.githubusercontent.com/11thmeu/launcher/master/doc/logo-transparent.png" width="250px" />
</p>

#####Current version: 2.0.0
This is the 11th Marine Expeditionary Unit launcher


## Screenshots
<p align="center">
	<img src="https://raw.githubusercontent.com/11thmeu/launcher/master/doc/capture1.png" width="250px" />

	<img src="https://raw.githubusercontent.com/11thmeu/launcher/master/doc/capture2.png" width="250px" />

	<img src="https://raw.githubusercontent.com/11thmeu/launcher/master/doc/capture3.png" width="250px" />
	
	<img src="https://raw.githubusercontent.com/11thmeu/launcher/master/doc/capture4.png" width="250px" />

	<img src="https://raw.githubusercontent.com/11thmeu/launcher/master/doc/capture5.png" width="250px" />

	<img src="https://raw.githubusercontent.com/11thmeu/launcher/master/doc/capture6.png" width="250px" />
	
	<img src="https://raw.githubusercontent.com/11thmeu/launcher/master/doc/capture7.png" width="250px" />
	
	<img src="https://raw.githubusercontent.com/11thmeu/launcher/master/doc/capture8.png" width="250px" />
</p>

## Authors
 * [Thrax](https://github.com/Thraxs/)


## Libraries
 * [MahApps.Metro] (http://github.com/MahApps/MahApps.Metro) 
 * [QueryMaster] (http://querymaster.codeplex.com/) 
 * [ArmA3Sync] (http://www.sonsofexiled.fr)


## Requirements
 * [Microsoft .NET Framework 4.5] (http://www.microsoft.com/en-us/download/details.aspx?id=30653)
 * (optional) [Oracle Java 7 or 8] (http://www.oracle.com/technetwork/java/javase/downloads/index.html)
 * (optional) [ArmA3Sync] (http://www.sonsofexiled.fr/wiki/index.php/ArmA3Sync_Wiki_English)


## Download
1.2.0 Stable Version - [Download] (https://raw.githubusercontent.com/11thmeu/launcher/master/bin/11thLauncher120_stable.zip) 
<p><b>MD5</b>: 9875e70c238fa92842789bd4ed6ddad8</p>


2.0.0 DEV Version (build 27062015) - [Download] (https://raw.githubusercontent.com/11thmeu/launcher/master/bin/11thLauncher200_dev27062015.zip) 
<p><b>MD5</b>: 2dba32606ed8509053910afc5d7b6710</p>

## Changelog
#####Version 2.0.0
 * New: Now the launcher detects all the addon folders at any subfolder depth inside the game directory
 * New: Program update notifications for both branches (stable and dev)
 * New: Auto-updater, automatically download, unzip, replace and launch the latest version
 * New: Call to dispose server query resources
 * New: More memory and video memory values over soft/hard-coded limits
 * New: Method to check again each server by clicking on the status icon
 * New: Now the external libraries are loaded in runtime from resources using reflection instead of being merged with ILMerge
 * New: Menu bar
 * New: Status bar
 * New: Java library to check repository using Arma3Sync code (requires manual configuration)
 * Changed: Application rewritten with WPF+XAML (without using MVVM)
 * Changed: Changed GUI library from MetroFramework(WinForms) to MahApps.Metro(WPF)
 * Changed: Moved application configuration settings to a separate window
 * Changed: Moved about information to a separate window
 * Changed: Modified server check functions to only resolve the server IP once
 * Fixed: Error when closing the launcher before the server check has finished
 * Fixed: Some messages were shown before the form finished loading
 * Fixed: Typo in process priority parameter tooltip
 * Removed: Unhandled exception handlers

#####Version 1.2.0
 * Initial public release