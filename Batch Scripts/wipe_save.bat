@echo off
set game_version=%1
if %1 == jp set game_version=
set PACKAGE_NAME=jp.co.ponos.battlecats%game_version%
adb shell rm /data/data/jp.co.ponos.battlecatsen/shared_prefs -r -f
adb shell rm /data/data/jp.co.ponos.battlecatsen/files/*SAVE_DATA*

adb shell am force-stop %PACKAGE_NAME%
adb shell monkey -p %PACKAGE_NAME% -v 1