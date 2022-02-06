@echo off
set game_version=%1
if %1 == jp set game_version=
adb pull /data/data/jp.co.ponos.battlecats%game_version%/files/SAVE_DATA %2