@echo off
set game_version=%1
if %1 == jp set game_version=
set PACKAGE_NAME=jp.co.ponos.battlecats%game_version%
adb push %2SAVE_DATA /data/data/%PACKAGE_NAME%/files/SAVE_DATA
adb shell am force-stop %PACKAGE_NAME%
adb shell monkey -p %PACKAGE_NAME% -v 1