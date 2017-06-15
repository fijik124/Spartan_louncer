Versioning
{Codename - Major.Minor.Build.Revision}
Codename - Changed every Minor version
Major - Version of the launcher
Minor - Significant changes to the launcher
Build - Build number
Revision - Incremental patches, reset after major/minor

-------------------------
--UPDATING THE LAUNCHER-- TODO
-------------------------

Process:
1. Update version numbers:
- In assembly information (e.g. "2,1,0,0")
- In about window "AboutWindow.xaml" (e.g. "2.1.0" and date)
- In "/Net/Updater.cs" line 19 (e.g. _currentVersion = "210";)
2. Build project.
3. Compute the MD5 hash of the new "11thLauncher.exe" file (this hash will be used by the updater to verify file integrity).
4. Update inside the version file "/bin/version", the version number and hash.
5. Create a ZIP file with the resulting file (11thLauncher.exe).
6. IMPORTANT: Rename the created ZIP file to "11thLauncher*.zip" where * is the _currentVersion same as in Updater.cs (e.g. 11thLauncher210.zip).
This is important because the automatic updater requires this type of name.
7. Move the new ZIP file to the root "/bin" folder. This folder also contains previous versions.
8. Update inside the README.md file the current version numbers, the download link, MD5 hashes and changelog. Add name to authors list if new contributor.
9. Git commit and push.

---------------------------------
--UPDATING THE LAUNCHER UPDATER-- TODO
---------------------------------
NOTE: Only needed if there are changes to the updater

Info: The updater is inside the launcher, so it must be compiled and moved in the 
launcher's lib folder before compiling the launcher.

Process:
1. Update assembly information (e.g. "1,0,1,0")
2. Bulid project
3. Move the resulting binary (11thLauncherUpdater.exe) to "/11thLauncher/lib/"

---------------------------------------
--UPDATING THE ARMA3SYNC DESERIALIZER-- TODO
---------------------------------------
NOTE: Only needed if there are changes to the deserializer

Info: This is the Java library for opening Arma3Sync repositories, as with the updater 
it must be compiled and moved inside the lib folder before compiling the launcher.

Process:
1. Bulid as runnable JAR
2. Move the resulting binary (A3SDS.jar) to "/11thLauncher/lib/"