# Battle Cats Save File Editor

I have a discord server:https://discord.gg/DvmMgvn5ZB, it's the best way report bugs and you can leave your suggestions for new features to be implemented in the editor

If you want to support my work and keep me motivated to continue to work
 on this project then maybe consider gifting me some ko-fi here: https://ko-fi.com/fieryhenry

## Thanks To:

Lethal's editor for giving me inspiration to start the 
project and it helped me work out how to patch the save data and edit 
cf/xp: https://www.reddit.com/r/BattleCatsCheats/comments/djehhn/editoren/

Beeven and csehydrogen's open source code, which helped me figure out how to patch save data: [GitHub - beeven/battlecats](https://github.com/beeven/battlecats), [GitHub - csehydrogen/BattleCatsHacker](https://github.com/csehydrogen/BattleCatsHacker)

## How To Use

This got way more complicated due to added security to the
 transfer system, you now need to have a rooted device - either an 
emulator or a real device and extract the save that way.

Download the tool [Releases · fieryhenry/Battle-Cats-Save-File-Editor · GitHub](https://github.com/fieryhenry/Battle-Cats-Save-File-Editor/releases) (get Battle Cats Save File Editor.zip, not the exe)

Watch this tutorial video that goes you 
through the steps of getting the save from the game, editing it, putting
 it back in, and fixing save data is used elsewhere bug: [BC help vid Updated - YouTube](https://youtu.be/D6hPnJTlq-U)

### Scripts

#### <u>Pull</u>

Gets the save data from the game files.

Run the script with the argument of the game version that you want, e.g script.bat en

You can add a second agrument with the path to pull to, otherwise it will pull the save data to the current directory

[Download](https://raw.githubusercontent.com/fieryhenry/Battle-Cats-Save-File-Editor/main/Batch%20Scripts/adb_pull.bat) (Right click -> save as)

```batch
@echo off
set game_version=%1
if %1 == jp set game_version=
adb pull /data/data/jp.co.ponos.battlecats%game_version%/files/SAVE_DATA %2
```

#### <u>Push</u>

Puts the save data into the game files, closes and re-opens the game.

Run the script with the argument of the game version that you want, e.g script.bat en

You can add a second agrument with the path to push from, otherwise it will push the save data located in the current directory.

[Download](https://raw.githubusercontent.com/fieryhenry/Battle-Cats-Save-File-Editor/main/Batch%20Scripts/adb_push.bat) (Right click -> save as)

```batch
@echo off
set game_version=%1
if %1 == jp set game_version=
set PACKAGE_NAME=jp.co.ponos.battlecats%game_version%
adb push %2SAVE_DATA /data/data/%PACKAGE_NAME%/files/SAVE_DATA
adb shell am force-stop %PACKAGE_NAME%
adb shell monkey -p %PACKAGE_NAME% -v 1
```
#### <u>Wipe Save Data</u>

Creates a fresh game ready for unban/fix elsewhere features

Run the script with the argument of the game version that you want, e.g script.bat en

You can add a second agrument with the path to push from, otherwise it will push the save data located in the current directory.

[Download](https://raw.githubusercontent.com/fieryhenry/Battle-Cats-Save-File-Editor/main/Batch%20Scripts/wipe_save.bat) (Right click -> save as)

```batch
@echo off
set game_version=%1
if %1 == jp set game_version=
set PACKAGE_NAME=jp.co.ponos.battlecats%game_version%
adb shell rm /data/data/jp.co.ponos.battlecatsen/shared_prefs -r -f
adb shell rm /data/data/jp.co.ponos.battlecatsen/files/*SAVE_DATA*

adb shell am force-stop %PACKAGE_NAME%
adb shell monkey -p %PACKAGE_NAME% -v 1
```

## Features

1. Cat Food
2. XP
3. Tickets / Platinum Shards<br>
   3.1 Normal Tickets<br>3.2 Rare Tickets<br>3.3 Platinum Tickets<br>3.4 Legend Tickets<br>3.5 Platinum Shards (using this instead of platinum shards reduces the chance of a ban)<br>
4. Leadership
5. NP
6. Treasures<br>6.1 All Treasures In A Chapter / Chapters<br>6.2 Specific Treasure Types e.g Energy Drink, Void Fruit<br>
7. Battle Items
8. Catseyes
9. Cat Fruits / Seeds
10. Talent Orbs
11. Gamatoto<br>11.1 Catamins<br>11.2 Helpers<br>11.3 XP<br>
12. Ototo<br>12.1 Base Materials<br>12.2 Engineers<br>12.3 Cat Cannon Upgrades<br>
13. Gacha Seed
14. Equip Slots
15. Gain / Remove Cats<br>15.1 Get all cats<br>15.2 Get specific cats<br>15.3 Remove all cats<br>15.3 Remove specific cats<br>
16. Cat / Stat Upgrades<br>16.1 Upgrade all cats<br>16.2 Upgrade all cats that are currently unlocked<br>16.3 Upgrade specific cats<br>16.4 Upgrade Base / Special Skills (The ones that are blue)<br>
17. Cat Evolves<br>17.1 Evolve all cats<br>17.2 Evolve specific cats<br>
    17.3 Evolve all current cats<br>
    17.4 Devolve all cats<br>
    17.5 Devolve specific cats<br>
18. Cat Talents<br>
19. Clear Levels / Outbreaks / Timed Score<br>19.1 Clear Main Story Chapters<br>19.2 Clear Stories of Legend Subchapters<br>19.3 Clear Uncanny Legends Subchapters<br>19.4 Clear Other Event Stages<br>19.5 Clear Zombie Stages / Outbreaks<br>19.6 Clear Aku Realm<br>19.7 Set Into The Future Timed Scores<br>19.8 Clear Heavenly Tower<br>19.9 Clear Infernal Tower<br>
20. Inquiry Code / Elsewhere Fix / Unban<br>20.1 Change Inquiry Code<br>20.2 Fix save is used elsewhere error - whilst selecting a save that has the error(the one you select when you open the editor) select a new save that has never had the save is used elsewhere bug ever(you can re-install the game to get a save like that)<br>
21. Get Restart Pack<br>
22. Close rank up bundle / offer menu<br>
23. Calculate checksum of save file
