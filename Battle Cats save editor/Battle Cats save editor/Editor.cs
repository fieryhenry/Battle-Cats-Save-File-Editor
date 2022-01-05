using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Windows.Forms;
using static System.Environment;
using System.Diagnostics;
using Battle_Cats_save_editor.SaveEdits;
using Battle_Cats_save_editor.Game_Mods;
using System.Linq;

namespace Battle_Cats_save_editor
{

    public class Editor
    {
        public static int catAmount = 0;
        public static string main_path;
        public static string gameVer;
        public static string version = "2.38.1";
        public static string multipleVals = "(You can enter multiple numbers sperated by spaces to edit multiple at once)";
        [STAThread]
        static void Main()
        {
            bool debug = false;
            if (!debug)
            {
                AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
                {
                    Console.WriteLine("An error has occurred\nPlease report this in #bug-reports:\n");
                    Console.WriteLine(eventArgs.ExceptionObject.ToString() + "\n");
                    PatchSaveFile.patchSaveFile(main_path);
                    Console.WriteLine("\nPress enter to exit");
                    Console.ReadLine();
                    Exit(0);
                };
            }
            CheckUpdate();
            SelSave();
            Options();
        }
        public static void SelSave()
        {
            Console.WriteLine("Select a battle cats save, or click cancel to download your save using transfer and confirmation codes");
            var FD = new OpenFileDialog
            {
                Filter = "battle cats save(*.*)|*.*",
                Title = "Select save",

            };
            if (FD.ShowDialog() == DialogResult.OK)
            {
                main_path = FD.FileName;
                ColouredText($"&Save: &\"{Path.GetFileName(main_path)}\"& is selected\n", ConsoleColor.White, ConsoleColor.Green);
            }
            else
            {
                ColouredText("&What game version are you using? (e.g en, jp, kr)\n");
                gameVer = Console.ReadLine();
                DownloadSaveData.Download_Save_Data();
                Options();
            }

            gameVer = PatchSaveFile.DetectGameVersion(main_path);
            if (gameVer.Length < 2)
            {
                Console.WriteLine("What game version are you using? (e.g en, jp, kr), note: en currently has the most support with the editor, so features may not work in other versions");
                gameVer = Console.ReadLine();
            }
            else
            {
                ColouredText($"&Detected game version: &{gameVer}&\n");
            }
        }
        public static string MakeRequest(WebRequest request)
        {
            request.Headers.Add("time-stamp", DateTime.Now.Ticks.ToString());
            WebResponse response = request.GetResponse();
            using Stream dataStream = response.GetResponseStream();
            StreamReader reader = new(dataStream);
            string responseFromServer = reader.ReadToEnd();
            return responseFromServer;
        }
        public static void UpgradeCats(string path, int[] catIDs, int[] plusLevels, int[] baseLevels, int ignore = 0)
        {
            int[] occurrence = GetCatRelatedHackPositions(path);
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int pos = occurrence[1] + 1;

            for (int i = 0; i < catIDs.Length; i++)
            {
                stream.Position = pos + (catIDs[i] * 4) + 3;
                if (ignore != 2)
                {
                    stream.WriteByte((byte)plusLevels[i]);
                    stream.Position--;
                }
                stream.Position += 2;
                if (ignore != 1)
                {
                    stream.WriteByte((byte)baseLevels[i]);
                };
            }
        }
        public static int[] GetCurrentCats(string path)
        {
            int[] occurrence = GetCatRelatedHackPositions(path);
            int startPos = occurrence[0] + 4;
            return GetItemData(path, catAmount, 4, startPos);
        }
        static void CheckUpdate()
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            try
            {
                Console.WindowHeight = 48;
                Console.WindowWidth = 200;
            }
            catch
            {
            }
            bool skip = false;
            string lines = "";
            try
            {
                lines = MakeRequest(WebRequest.Create("https://raw.githubusercontent.com/fieryhenry/Battle-Cats-Save-File-Editor/main/version.txt")).Trim('\n');
            }
            catch (WebException)
            {
                ColouredText("No internet connection to check for a new version\n", ConsoleColor.White, ConsoleColor.Red);
                skip = true;
            }
            if (lines == version && !skip)
            {
                ColouredText($"Application up to date - current version is {version}\n", ConsoleColor.White, ConsoleColor.Cyan);
            }
            else if (lines != version && !skip)
            {
                ColouredText($"A new version is available would you like to update to release {lines}?\n", ConsoleColor.White, ConsoleColor.Green);
                bool answer = OnAskUser($"A new version is available would you like to update to release {lines}?", "Updater");
                if (answer)
                {
                    try
                    {
                        Process.Start(@"Updater.exe");
                        Exit(0);
                    }
                    catch
                    {
                        ColouredText("Error, the updater cannot be found, please download the latest version from the github instead\n", ConsoleColor.White, ConsoleColor.Red);
                    }
                }
            }
        }
        public static void Options()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nCreate a copy of your save before using this editor!\nIf you get an error along the lines of \"Your save is active somewhere else\" then select option 20-->2, and select a save that doesn't have that error and never has had the error\n");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Thanks to: Lethal's editor for being a tool for me to use when figuring out how to patch save files,and how to edit cf/xp\nAnd thanks to beeven and csehydrogen's open source work, which I used to implement the save patching algorithm\n");
            Console.ForegroundColor = ConsoleColor.White;

            string[] Features =
            {
                "Select a new save", "Cat Food", "XP", "Tickets / Platinum Shards", "Leadership", "NP", "Treasures", "Battle Items", "Catseyes", "Cat Fruits / Seeds", "Talent Orbs", "Gamatoto", "Ototo", "Gacha Seed", "Equip Slots", "Gain / Remove Cats", "Cat / Stat Upgrades", "Cat Evolves", "Cat Talents",
                "Clear Levels / Outbreaks / Timed Score", "Inquiry Code / Elsewhere Fix / Unban", "Get Restart Pack", "Close the rank up bundle / offer menu", "Game Modding menu", "Calculate checksum of save file",
            };
            if (gameVer != "en")
            {
                ColouredText("Warning: if you are using a non en save, many features won't work, or they might corrupt your save data, so make sure you create a copy of your saves!\n", ConsoleColor.White, ConsoleColor.Red);
            }

            string toOutput = "&What would you like to do?&\n0.& Select a new save\n&";
            for (int i = 1; i < Features.Length; i++)
            {
                toOutput += $"&{i}.& ";
                toOutput += Features[i] + "\n";
            }
            ColouredText(toOutput);

            byte[] CatNumber = new byte[20];
            CatNumber[0] = GetCatNumber(main_path);
            CatNumber[1] = 0x02;
            catAmount = BitConverter.ToInt32(CatNumber, 0);
            int Choice = (int)Inputed();
            switch (Choice)
            {
                case 0: SelSave(); Options(); break;
                case 1: CatFood.catFood(main_path); break;
                case 2: XP.xp(main_path); break;
                case 3: Tickets(main_path); break;
                case 4: Leadership.leadership(main_path); break;
                case 5: NP.np(main_path); break;
                case 6: Treasures(main_path); break;
                case 7: BattleItems.Items(main_path); break;
                case 8: Catseye.Catseyes(main_path); break;
                case 9: CatFruits.CatFruit(main_path); break;
                case 10: TalentOrbs.TalentOrb(main_path); break;
                case 11: Gamatoto(main_path); break;
                case 12: Ototo(main_path); break;
                case 13: RareGachaSeed.Seed(main_path); break;
                case 14: EquipSlots.Slots(main_path); break;
                case 15: GetCats(main_path); break;
                case 16: Upgrades(main_path); break;
                case 17: Evolves(main_path); break;
                case 18: Talent(main_path); break;
                case 19: Levels(main_path); break;
                case 20: Inquiry(main_path); break;
                case 21: RestartPack.GetRestartPack(main_path); break;
                case 22: CloseBundle.Bundle(main_path); break;
                case 23: GameModdingMenu(main_path); break;
                case 24: break;
                default: Console.WriteLine("Please input a number that is recognised"); break;
            }
            PatchSaveFile.patchSaveFile(main_path);

            ColouredText("Press enter to continue\n");
            Console.ReadLine();
            Options();
        }
        public static int[] ConvertCharArrayToIntArray(char[] input)
        {
            int[] result = Array.ConvertAll(input, x => int.Parse($"{x}"));
            return result;
        }
        public static string CreateOptionsList<T>(string[] options, T[] extravalues = null, bool numerical = true, bool skipZero = false)
        {
            string toOutput = "";
            if (!skipZero)
            {
                for (int i = 0; i < options.Length; i++)
                {
                    if (numerical)
                    {
                        toOutput += $"&{i + 1}.& ";
                    }
                    toOutput += options[i];
                    if (extravalues != null)
                    {
                        try
                        {
                            toOutput += $" &: {extravalues[i]}&";
                        }
                        catch
                        {
                            break;
                        }
                    }
                    toOutput += "\n";
                }
            }
            else
            {
                for (int i = 0; i < options.Length; i++)
                {
                    int val = Convert.ToInt32(extravalues[i]);
                    if (val != 0)
                    {
                        if (numerical)
                        {
                            toOutput += $"&{i + 1}.& ";
                        }
                        toOutput += options[i];
                        if (extravalues != null)
                        {
                            try
                            {
                                toOutput += $" &: {extravalues[i]}&";
                            }
                            catch
                            {
                                break;
                            }
                        }
                        toOutput += "\n";
                    }
                }
            }
            return toOutput;
        }
        static void Inquiry(string path)
        {
            string[] Features =
            {
                "Go back",
                "Change Inquiry code",
                "Fix save is used elsewhere error - whilst selecting a save that has the error(the one you select when you open the editor) select a new save that has never had the save is used elsewhere bug ever(you can re-install the game to get a save like that)"
            };
            string toOutput = "&What would you like to edit?&\n0.& Go back\n&";
            for (int i = 1; i < Features.Length; i++)
            {
                toOutput += $"&{i}.& ";
                toOutput += Features[i] + "\n";
            }
            ColouredText(toOutput);
            int choice = (int)Inputed();

            switch (choice)
            {
                case 0: Options(); break;
                case 1: NewInquiryCode.NewIQ(path); break;
                case 2: FixElsewhere.Elsewhere(path); break;
                default: Console.WriteLine($"Please enter a number between 0 and {Features.Length}"); break;
            }
        }
        static void Levels(string path)
        {
            string[] Features =
            {
                "Go back",
                "Clear Main Story Chapters",
                "Clear Stories of Legend Subchapters",
                "Clear Uncanny Legends Subchapters",
                "Clear Other Event Stages",
                "Clear Zombie Stages / Outbreaks",
                "Clear Aku Realm",
                "Set Into The Future Timed Scores",
                "Clear Heavenly Tower Stages",
                "Clear Infernal Tower Stages",
            };

            string toOutput = "&What would you like to edit?&\n0.& Go back\n&";
            for (int i = 1; i < Features.Length; i++)
            {
                toOutput += $"&{i}.& ";
                toOutput += Features[i] + "\n";
            }
            ColouredText(toOutput);
            int choice = (int)Inputed();

            switch (choice)
            {
                case 0: Options(); break;
                case 1: MainStory.Stage(path); break;
                case 2: StoriesOfLegend.Sol(path); break;
                case 3: UncannyLegends.Uncanny_Legends(path); break;
                case 4: MainEventStages.EventStages(path); break;
                case 5: ZombieStages.Outbreaks(path); break;
                case 6: AkuRealm.ClearAku(path); break;
                case 7: ItFTimedScores.TimedScore(path); break;
                case 8: HeavenlyTower.ClearHeavenlyTower(path); break;
                case 9: InfernalTower.ClearInfernalTower(path); break;
                default: Console.WriteLine($"Please enter a number between 0 and {Features.Length}"); break;
            }
        }
        static void Talent(string path)
        {
            string[] Features =
            {
                "Go back",
                "Talent upgrade all cats",
                "Talent upgrade specific cats"
            };

            string toOutput = "&What would you like to edit?&\n0.& Go back\n&";
            for (int i = 1; i < Features.Length; i++)
            {
                toOutput += $"&{i}.& ";
                toOutput += Features[i] + "\n";
            }
            ColouredText(toOutput);
            int choice = (int)Inputed();

            switch (choice)
            {
                case 0: Options(); break;
                case 1: AllTalent.AllTalents(path); break;
                case 2: SpecificTalent.SpecificTalents(path); break;
                default: Console.WriteLine($"Please enter a number between 0 and {Features.Length}"); break;
            }
        }
        static void Evolves(string path)
        {
            string[] Features =
            {
                "Go back",
                "Evolve all cats",
                "Evolve specific cats"
            };

            string toOutput = "&What would you like to edit?&\n0.& Go back\n&";
            for (int i = 1; i < Features.Length; i++)
            {
                toOutput += $"&{i}.& ";
                toOutput += Features[i] + "\n";
            }
            ColouredText(toOutput);
            int choice = (int)Inputed();

            switch (choice)
            {
                case 0: Options(); break;
                case 1: EvolveCats.Evolve(path); break;
                case 2: EvolvesSpecific.EvolveSpecific(path); break;
                default: Console.WriteLine($"Please enter a number between 0 and {Features.Length}"); break;
            }
        }
        static void Upgrades(string path)
        {
            string[] Features =
            {
                "Go back",
                "Upgrade all cats",
                "Upgrade all cats that are currently unlocked",
                "Upgrade specific cats",
                "Upgrade Base / Special Skills (The ones that are blue)"
            };

            string toOutput = "&What would you like to edit?&\n0.& Go back\n&";
            for (int i = 1; i < Features.Length; i++)
            {
                toOutput += $"&{i}.& ";
                toOutput += Features[i] + "\n";
            }
            ColouredText(toOutput);
            int choice = (int)Inputed();

            switch (choice)
            {
                case 0: Options(); break;
                case 1: CatUpgrade.CatUpgrades(path); break;
                case 2: UpgradeCurrent.UpgradeCurrentCats(path); break;
                case 3: SpecificUpgrade.SpecifUpgrade(path); break;
                case 4: BlueUpgrade.Blue(path); break;
                default: Console.WriteLine($"Please enter a number between 0 and {Features.Length}"); break;
            }
        }
        static void GetCats(string path)
        {
            string[] Features =
            {
                "Go back",
                "Get all cats",
                "Get specific cats",
                "Remove all cats",
                "Remove specific cats"
            };

            string toOutput = "&What would you like to edit?&\n0.& Go back\n&";
            for (int i = 1; i < Features.Length; i++)
            {
                toOutput += $"&{i}.& ";
                toOutput += Features[i] + "\n";
            }
            ColouredText(toOutput);
            int choice = (int)Inputed();

            switch (choice)
            {
                case 0: Options(); break;
                case 1: GetCat.Cats(path); break;
                case 2: GetSpecificCats.SpecifiCat(path); break;
                case 3: RemoveCats.RemCats(path); break;
                case 4: RemoveSpecificCats.RemSpecifiCat(path); break;
                default: Console.WriteLine($"Please enter a number between 0 and {Features.Length}"); break;
            }
        }
        static void Treasures(string path)
        {
            string[] Features =
            {
                "Go back",
                "All Treasures In A Chapter / Chapters",
                "Specific Treasure Types e.g Energy Drink, Void Fruit"
            };
            string toOutput = "&What do you want to edit?&\n0.& Go back\n&";

            for (int i = 1; i < Features.Length; i++)
            {
                toOutput += $"&{i}.& ";
                toOutput += Features[i] + "\n";
            }
            ColouredText(toOutput);
            int choice = (int)Inputed();

            switch (choice)
            {
                case 0: Options(); break;
                case 1: AllTreasures.MaxTreasures(path); break;
                case 2: SpecificTreasures.VerySpecificTreasures(path); break;
                default: Console.WriteLine($"Please enter a number between 0 and {Features.Length}"); break;
            }
        }
        static void Tickets(string path)
        {
            string[] Features =
            {
                "Go back",
                "Normal Tickets",
                "Rare Tickets",
                "Platinum Tickets",
                "Legend Tickets",
                "Platinum Shards (using this instead of platinum tickets reduces the chance of a ban)"
            };

            string toOutput = "Warning: editing these at all has a risk of getting your save banned\n&What would you like to edit?&\n0.& Go back\n&";
            for (int i = 1; i < Features.Length; i++)
            {
                toOutput += $"&{i}.& ";
                toOutput += Features[i] + "\n";
            }
            ColouredText(toOutput);
            int choice = (int)Inputed();

            switch (choice)
            {
                case 0: Options(); break;
                case 1: NormalTickets.CatTicket(path); break;
                case 2: RareTickets.RareCatTickets(path); break;
                case 3: PlatTickets.PlatinumTickets(path); break;
                case 4: LegendTickets.LegendTicket(path); break;
                case 5: PlatinumShards.PlatShards(path); break;
                default: Console.WriteLine($"Please enter a number between 0 and {Features.Length}"); break;
            }
        }
        static void Gamatoto(string path)
        {
            string[] Features =
            {
                "Go back",
                "Catamins",
                "Helpers",
                "XP"
            };

            string toOutput = "&What would you like to edit?&\n0.& Go back\n&";
            for (int i = 1; i < Features.Length; i++)
            {
                toOutput += $"&{i}.& ";
                toOutput += Features[i] + "\n";
            }
            ColouredText(toOutput);
            int choice = (int)Inputed();

            switch (choice)
            {
                case 0: Options(); break;
                case 1: Catamins.Catamin(path); break;
                case 2: GamatotoHelper.GamHelp(path); break;
                case 3: GamatotoXP.GamXP(path); break;
                default: Console.WriteLine($"Please enter a number between 0 and {Features.Length}"); break;
            }
        }
        static void Ototo(string path)
        {
            string[] Features =
            {
                "Go back",
                "Base Materials",
                "Engineers",
                "Cat Cannon Upgrades"
            };

            string toOutput = "&What would you like to edit?&\n0.& Go back\n&";
            for (int i = 1; i < Features.Length; i++)
            {
                toOutput += $"&{i}.& ";
                toOutput += Features[i] + "\n";
            }
            ColouredText(toOutput);
            int choice = (int)Inputed();

            switch (choice)
            {
                case 0: Options(); break;
                case 1: BaseMaterials.BaseMats(path); break;
                case 2: OtotoEngineers.Engineers(path); break;
                case 3: OtotoCatCannon.CatCannon(path); break;
                default: Console.WriteLine($"Please enter a number between 0 and {Features.Length}"); break;
            }
        }
        static void GameModdingMenu(string path)
        {
            string[] Features =
            {
                "Go back",
                "Decrypt .list and .pack",
                "Encrypt a folder of game files and turn them into encrypted .pack and .list files",
                "Update the md5 sum of the modified .pack and .list files",
                "Enter the game file editing menu"
            };
            string toOutput = "&What would you like to edit?&\n0.& Go back\n&";
            for (int i = 1; i < Features.Length; i++)
            {
                toOutput += $"&{i}.& ";
                toOutput += Features[i] + "\n";
            }
            ColouredText(toOutput);
            int choice = (int)Inputed();

            switch (choice)
            {
                case 0: Options(); break;
                case 1: DecryptPack.Decrypt("b484857901742afc"); break;
                case 2: EncryptPack.EncryptData("b484857901742afc"); break;
                case 3: MD5Libnative.MD5Lib(path); break;
                case 4: GameFileParsing(path); break;
                default: Console.WriteLine($"Please enter a number between 0 and {Features.Length}"); break;
            }
        }
        static void GameFileParsing(string path)
        {
            string[] Features =
            {
                "Go back",
                "Edit unit*.csv (cat data)",
                "Edit stage*.csv (level data)",
                "Edit t_unit.csv (enemy data)"
            };
            string toOutput = "&What would you like to edit?&\n0.& Go back\n&";
            for (int i = 1; i < Features.Length; i++)
            {
                toOutput += $"&{i}.& ";
                toOutput += Features[i] + "\n";
            }
            ColouredText(toOutput);
            int choice = (int)Inputed();

            switch (choice)
            {
                case 0: GameModdingMenu(path); break;
                case 1: UnitMod.Unitcsv(); break;
                case 2: StageMod.Stagecsv(); break;
                case 3: EnemyMod.EnemyCSV(); break;
                default: Console.WriteLine($"Please enter a number between 0 and {Features.Length}"); break;
            }
        }
        public static string CalculateMD5(string path)
        {
            MD5 md5 = MD5.Create();
            byte[] allData = File.ReadAllBytes(path);
            byte[] hash = md5.ComputeHash(allData);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        // Search function for any number of conditions, instead of having to add more conditions to an if statement
        public static int[] Search(string path, byte[] conditions, bool negative = false, int startpoint = 0, byte[] mult = null, int endpoint = -1, int stop_after = -1)
        {
            byte[] allData = File.ReadAllBytes(path);
            if (mult == null)
            {
                mult = new byte[conditions.Length];
            }
            if (endpoint == -1)
            {
                endpoint = allData.Length - conditions.Length;
            }

            int count = 0;
            int pos = 0;
            int startpos = startpoint;
            int num = 1;
            List<int> values = new();
            int start = 0;
            int end = conditions.Length;
            int stop_count = 0;
            if (negative)
            {
                num = -1;
                end = 0;
                start = conditions.Length - 1;
            }
            for (int i = startpos; i < endpoint; i += num)
            {
                if (stop_after > -1 && stop_count >= stop_after && values.Count > 0)
                {
                    break;
                }
                count = 0;

                for (int j = 0; j < conditions.Length; j += 1)
                {
                    if (negative)
                    {
                        try
                        {
                            if (allData[i - j] == conditions[conditions.Length - 1 - j] || mult[conditions.Length - 1 - j] == 0x01)
                            {
                                if (stop_count > 0)
                                {
                                    stop_count = 0;
                                }
                                count++;
                                pos = i;
                            }
                            else
                            {
                                if (count > 0)
                                {
                                    stop_count++;
                                }
                                count = 0;
                            }
                        }
                        catch
                        {
                            if (values[0] > 0)
                            {
                                i = allData.Length;
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (allData[i + j] == conditions[j] || mult[j] == 0x01)
                        {
                            count++;
                            pos = i;
                        }
                        else
                        {
                            if (count > 0)
                            {
                                stop_count++;
                            }
                            count = 0;
                        }
                    }

                }
                if (count >= conditions.Length)
                {
                    values.Add(pos);
                    stop_count = 0;
                }
            }
            return values.ToArray();
        }
        public static int GetOtotoPos(string path)
        {
            byte[] conditions = { 0, 0, 0, 8, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 3, 0, 0, 0 };
            int pos = Search(path, conditions)[0];

            if (pos < 200)
            {
                Error();
            }
            return pos;
        }
        static bool OnAskUser(string title, string title2)
        {
            return DialogResult.Yes == MessageBox.Show(
             title, title2,
             MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        public static byte[] Endian(long num)
        {
            byte[] bytes = BitConverter.GetBytes(num);

            return bytes;
        }

        public static string ByteArrayToString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }

        public static void ColouredText(string input, ConsoleColor Base = ConsoleColor.White, ConsoleColor New = ConsoleColor.DarkYellow)
        {
            char[] chars = { '&' };

            string[] split = new string[input.Length];
            try { split = input.Split(chars); }
            catch (IndexOutOfRangeException)
            {
            }
            try
            {
                Console.ForegroundColor = New;
                for (int i = 0; i < split.Length; i += 2)
                {
                    Console.ForegroundColor = New;
                    Console.Write(split[i]);
                    Console.ForegroundColor = Base;
                    if (i == split.Length - 1)
                    {
                        break;
                    }
                    Console.Write(split[i + 1]);


                }
                Console.ForegroundColor = Base;
            }
            catch (IndexOutOfRangeException)
            {

            }
        }
        public static int AskSentances(int amount, string item, bool set = false, int max = int.MaxValue)
        {
            if (!set)
            {
                ColouredText($"&You have &{amount}& {item}\n");

                if (max == int.MaxValue)
                {
                    ColouredText($"&What do you want to set your {item} to?:\n");
                }
                else
                {
                    ColouredText($"&What do you want to set your {item} to? (max {max}):\n");
                }
                amount = (int)Inputed();
                amount = MaxMinCheck(amount, max);
                return amount;
            }
            else
            {
                ColouredText($"&Successfully set {item} to &{amount}&\n");
                return 0;
            }

        }
        public static int MaxMinCheck(int val, int max, int min = 0)
        {
            if (val > max) val = max;
            if (val < min) val = min;
            return val;
        }
        public static int[] GetItemData(string path, int amount, int separator, int startPos)
        {
            byte[] allData = File.ReadAllBytes(path);

            int[] items = new int[amount];

            for (int i = 0; i < amount; i++)
            {
                byte[] items_ba = allData.Skip(startPos + (i * separator)).Take(separator).ToArray();
                int item = 0;
                if (separator > 2)
                {
                    item = BitConverter.ToInt32(items_ba, 0);
                }
                else
                {
                    item = BitConverter.ToInt16(items_ba, 0);
                }
                items[i] = item;
            }
            Console.WriteLine("Note: If any of these numbers are incorrect, then don't edit them, as it could corrupt your save. If this is the case please report it on discord");
            return items;
        }
        public static List<T> SetPartOfList<T>(List<T> orig_list, List<T> data_to_set, int index, int skip = 1)
        {
            int counter = 0;
            for (int i = 0; i < orig_list.Count; i += skip / 4)
            {
                if (counter >= data_to_set.Count)
                {
                    break;
                }
                if (i >= index)
                {
                    orig_list[i] = data_to_set[counter];
                    counter++;
                }
            }
            return orig_list;
        }
        public static void SetItemData(string path, int[] items, int separator, int startPos)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            for (int i = 0; i < items.Length; i++)
            {
                byte[] item = Endian(items[i]);

                stream.Position = startPos + (i * separator);
                stream.Write(item, 0, separator);
            }
        }

        public static long Inputed()
        {
            long input = 0;
            try { input = Convert.ToInt64(Console.ReadLine()); }
            catch (OverflowException)
            {
                ColouredText("Input number was too large\n", ConsoleColor.White, ConsoleColor.DarkRed);
                Options();
            }
            catch (FormatException)
            {
                ColouredText("Input given was not a number or it wasn't an integer\n", ConsoleColor.White, ConsoleColor.DarkRed);
                Options();
            }
            return input;
        }

        public static int[] GetCatRelatedHackPositions(string path)
        {
            byte[] allData = File.ReadAllBytes(path);

            int amount = 0;
            int[] occurrence = new int[50];
            byte anchour = GetCatNumber(path);

            for (int i = 4000; i < allData.Length - 1; i++)
            {
                if (allData[i] == anchour)
                {
                    if (allData[i + 1] == 2 && allData[i + 2] == 0 && allData[i + 3] == 0)
                    {
                        occurrence[amount] = i;
                        amount++;
                    }
                }
            }

            return occurrence;
        }

        public static int[] GetPositionsFromYear(string path, byte[] Currentyear)
        {
            byte[] allData = File.ReadAllBytes(path);

            int amount = 0;
            int[] occurrence = new int[50];

            for (int i = 0; i < allData.Length - 1; i++)
            {
                if (allData[i] == Convert.ToByte(Currentyear[0]) && allData[i + 1] == Convert.ToByte(Currentyear[1]) && allData[i + 2] == 0 && allData[i + 3] == 0)
                {
                    occurrence[amount] = i;
                    amount++;
                }

            }

            return occurrence;
        }

        public static Dictionary<int, Tuple<string, int>> GetSkillData()
        {
            WebClient webClient = new();
            string[] MainData = webClient.DownloadString("https://raw.githubusercontent.com/fieryhenry/Battle-Cats-Save-File-Editor/main/talent%20ids.txt").Split('\n');

            Dictionary<int, Tuple<string, int>> data = new();


            for (int i = 0; i < MainData.Length; i++)
            {
                int id = int.Parse(MainData[i].Split(':')[0]);
                string name = MainData[i].Split(':')[1].Trim(' ');
                int max = int.Parse(MainData[i].Split(':')[2]);

                data.Add(id, Tuple.Create(name, max));
            }
            return data;
        }
        public static int GetEvolvePos(string path)
        {
            int[] occurrence = GetCatRelatedHackPositions(path);

            byte[] conditions = { 0x2c, 0x01, 0x00, 0x00 };
            int pos1 = Search(path, conditions, false, occurrence[5] - 400, null, occurrence[5])[0];
            int pos2 = occurrence[5];
            if (pos1 == 0)
            {
                pos1 = Search(path, conditions, true, occurrence[4] - 400, null, occurrence[4])[0];
                pos2 = occurrence[4];
            }
            if (pos1 == 0)
            {
                Error();
            }
            return pos2 + 40;
        }
        public static void Error(string text = "Error, a position couldn't be found, please report this in #bug-reports on discord")
        {
            Console.WriteLine(text + "\nPress enter to continue");
            Console.ReadLine();
            Options();
        }
        public static int[] GetPlatinumTicketPos(string path)
        {
            byte[] conditions = { 0xFF, 0xFF, 0x00, 0x36, 0x00, 0x00 };
            byte[] choice = { 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 };
            int pos1 = Search(path, conditions, false, 0, choice)[0];
            byte[] conditions2 = { 0x36, 0x00, 0x00, 0x00 };
            int pos2 = Search(path, conditions2, false, pos1 + conditions.Length)[0];

            if (pos1 == 0 || pos2 == 0)
            {
                Error();
            }

            return new int[] { pos1, pos2 };
        }
        static byte GetCatNumber(string path)
        {
            // If the save file is not a save file, don't modify/read it as one
            if (path.EndsWith(".list") || path.EndsWith(".pack") || path.EndsWith(".so") || path.EndsWith(".csv"))
            {
                return 0;
            }
            byte[] allData = File.ReadAllBytes(path);

            byte anchour = 0;

            for (int i = 7344; i < 10800; i++)
            {
                try
                {
                    if (allData[i] == 2)
                    {
                        anchour = allData[i - 1];
                        break;
                    }
                }
                catch
                {
                    Console.WriteLine("Error, this save file seems to be different/corrupted, if this is an actual bc save file, please report this to the discord");
                    return 0;
                }
            }
            return anchour;
        }
        public static int[] EvolvedFormsGetter()
        {
            Console.WriteLine("Downloading cat data");
            string[] catData = MakeRequest(WebRequest.Create("https://raw.githubusercontent.com/fieryhenry/Battle-Cats-Save-File-Editor/main/nyankoPictureBook_en.csv")).Split('\n');

            List<string> thirdDataS = new();
            foreach (string cat in catData)
            {
                thirdDataS.Add(cat.Split('|')[6]);
            }
            int[] thirdFormData = new int[catData.Length];
            for (int i = 0; i < thirdFormData.Length; i++)
            {
                string text = thirdDataS[i].ToLower();
                if (text.Length < 2 && i != 428)
                {
                    thirdFormData[i] = 0;
                }
                else
                {
                    thirdFormData[i] = 2;
                }
            }
            return thirdFormData;
        }
    }
}
