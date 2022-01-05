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

namespace Battle_Cats_save_editor
{

    public class Editor
    {
        public static int catAmount = 0;
        public static string[] Savepaths = new string[500];
        public static string gameVer = "";
        public static string version = "2.38.1";
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
            {
                Console.WriteLine("An error has occurred\nPlease report this in #bug-reports:\n");
                Console.WriteLine(eventArgs.ExceptionObject.ToString());
                Console.WriteLine("\nPress enter to exit");
                Console.ReadLine();
                Exit(0);
            };
            CheckUpdate();
            SelSave();
            Options();
        }
        public static void SelSave()
        {
            var FD = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "battle cats save(*.*)|*.*",
                Title = "Select save"
            };
            if (FD.ShowDialog() == DialogResult.OK)
            {
                string[] fileToOpen = FD.FileNames;
                for (int i = 0; i < fileToOpen.Length; i++)
                {
                    ColouredText($"&Save: &\"{Path.GetFileName(fileToOpen[i])}\"&\n", ConsoleColor.White, ConsoleColor.Green);
                }
                Savepaths = fileToOpen;
            }
            else
            {
                ColouredText("\nPlease select your save\n\n");
                SelSave();
            }
            Console.WriteLine("What game version are you using? (e.g en, jp, kr), note: en currently has the most support with the editor, so features may not work in other versions");
            gameVer = Console.ReadLine();
        }
        static string MakeRequest(WebRequest request)
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
            int[] occurrence = OccurrenceB(path);
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int pos = occurrence[1] +1;

            for (int i = 0; i < catIDs.Length; i++)
            {
                stream.Position = pos + (catIDs[i] * 4) + 3;
                if (ignore != 2)
                {
                    stream.WriteByte((byte)plusLevels[i]);
                    stream.Position--;
                }
                stream.Position+=2;
                if (ignore != 1)
                {
                    stream.WriteByte((byte)baseLevels[i]);
                };
            }
        }
        public static byte[] LoadData(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            return allData;
        }
        public static int[] GetCurrentCats(string path)
        {
            int[] occurrence = OccurrenceB(path);
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            List<int> catsL = new List<int>();

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            for (int i = 0; i < catAmount; i++)
            {
                int startPos = occurrence[0] + 4;
                if (allData[startPos + (i*4)] == 1)
                {
                    int catID = i;
                    catsL.Add(catID);
                }
            }
            return catsL.ToArray();
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
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"Application up to date - current version is {version}");
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
            string[] fileToOpen = Savepaths;
            string path = Path.Combine(fileToOpen[0]);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nBackup your save before using this editor!\nIf you get an error along the lines of \"Your save is active somewhere else\"then select option 20-->2, and select a save that doesn't have that error and never has had the error\n", fileToOpen);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Thanks to: Lethal's editor for being a tool for me to use when figuring out how to patch save files, uploading the save data onto the servers how to and edit cf/xp\nAnd thanks to beeven and csehydrogen's open source work, which I used to implement the save patching algorithm\n");
            Console.ForegroundColor = ConsoleColor.White;

            string[] Features =
            {
                "Select a new save", "Cat Food", "XP", "Tickets / Platinum Shards", "Leadership", "NP", "Treasures", "Battle Items", "Catseyes", "Cat Fruits / Seeds", "Talent Orbs", "Gamatoto", "Ototo", "Gacha Seed", "Equip Slots", "Gain / Remove Cats", "Cat / Stat Upgrades", "Cat Evolves", "Cat Talents",
                "Clear Levels / Outbreaks / Timed Score", "Inquiry Code / Elsewhere Fix / Unban", "Close the rank up bundle / offer menu", "Game Modding menu", "Calculate checksum of save file", 
            };
            ColouredText("Warning: if you are using a non en save, many features won't work, or they might corrupt your save data, so make sure you back up your saves!\n", ConsoleColor.White, ConsoleColor.Red);

            string toOutput = "&What would you like to do?&\n0.& Select a new save\n&";
            for (int i = 1; i < Features.Length; i++)
            {
                toOutput += $"&{i}.& ";
                toOutput += Features[i] + "\n";
            }
            ColouredText(toOutput);

            byte[] CatNumber = new byte[20];
            CatNumber[0] = GetCatNumber(path);
            CatNumber[1] = 0x02;
            catAmount = BitConverter.ToInt32(CatNumber, 0);
            int Choice = (int)Inputed();
            for (int i = 0; i < fileToOpen.Length; i++)
            {
                path = fileToOpen[i];
                switch (Choice)
                {
                    case 0: SelSave(); Options(); break;
                    case 1: CatFood.catFood(path); break;
                    case 2: XP.xp(path); break;
                    case 3: Tickets(path); break;
                    case 4: Leadership.leadership(path); break;
                    case 5: NP.np(path); break;
                    case 6: Treasures(path); break;
                    case 7: BattleItems.Items(path); break;
                    case 8: Catseye.Catseyes(path); break;
                    case 9: CatFruits.CatFruit(path); break;
                    case 10: TalentOrbs.TalentOrb(path); break;
                    case 11: Gamatoto(path); break;
                    case 12: Ototo(path); break;
                    case 13: RareGachaSeed.Seed(path); break;
                    case 14: EquipSlots.Slots(path); break;
                    case 15: GetCats(path); break;
                    case 16: Upgrades(path); break;
                    case 17: Evolves(path); break;
                    case 18: Talent(path); break;
                    case 19: Levels(path); break;
                    case 20: Inquiry(path); break;
                    case 21: CloseBundle.Bundle(path); break;
                    case 22: GameModdingMenu(); break;
                    case 23: break;
                    default: Console.WriteLine("Please input a number that is recognised"); break;
                }
                PatchSaveFile.patchSaveFile(gameVer, path);
            }
            ColouredText("Press enter to continue\n");
            Console.ReadLine();
            Options();
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
                "Clear Zombie Stages / Outbreaks",
                "Clear Aku Realm",
                "Set Into The Future Timed Scores"
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
                case 2: StoriesofLegend.SoL(path); break;
                case 3: ZombieStages.Outbreaks(path); break;
                case 4: AkuRealm.ClearAku(path); break;
                case 5: ItFTimedScores.TimedScore(path); break;
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
                case 2: RareTickets.CatTicketRare(path); break;
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
        static void GameModdingMenu()
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
                case 3: MD5Libnative.MD5Lib(); break;
                case 4: GameFileParsing(); break;
                default: Console.WriteLine($"Please enter a number between 0 and {Features.Length}"); break;
            }
        }
        static void GameFileParsing()
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
                case 0: GameModdingMenu(); break;
                case 1: UnitMod.Unitcsv(); break;
                case 2: StageMod.Stagecsv(); break;
                case 3: EnemyMod.EnemyCSV(); break;
                default: Console.WriteLine($"Please enter a number between 0 and {Features.Length}"); break;
            }
        }
        public static string CalculateMD5(string filename)
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(filename);
            var hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        // Search function for any number of conditions, instead of having to add more conditions to an if statement
        public static int[] Search(string path, byte[] conditions, bool negative = false, int startpoint = 0, byte[] mult = null, int endpoint = -1)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

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
            int[] values = new int[50];
            int iter = 0;
            int start = 0;
            int end = conditions.Length;
            if (negative)
            {
                num = -1;
                end = 0;
                start = conditions.Length - 1;
            }
            for (int i = startpos; i < endpoint; i += num)
            {
                count = 0;
                for (int j = 0; j < conditions.Length; j += 1)
                {
                    if (negative)
                    {
                        try
                        {
                            if (allData[i - j] == conditions[conditions.Length - 1 - j] || mult[conditions.Length - 1 - j] == 0x01)
                            {
                                count++;
                                pos = i;
                            }
                            else
                            {
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
                            count = 0;
                        }
                    }

                }
                if (count >= conditions.Length)
                {
                    try
                    {
                        values[iter] = pos;
                    }
                    catch (IndexOutOfRangeException)
                    {
                        break;
                    }
                    iter++;
                }
            }
            if (iter > 0)
            {
                return values;
            }
            else
            {
                return values;
            }
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
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("\nNo & characters in inputed string!");
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

        public static int[] OccurrenceB(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            int amount = 0;
            int[] occurrence = new int[50];
            stream.Close();
            byte anchour = GetCatNumber(path);

            for (int i = 4000; i < allData.Length - 1; i++)
            {
                if (allData[i] == anchour)
                    if (allData[i + 1] == 2 && allData[i + 2] == 0 && allData[i + 3] == 0)
                    {
                        occurrence[amount] = i;
                        amount++;
                    }
            }
            stream.Close();

            return occurrence;
        }

        public static int[] OccurrenceE(string path, byte[] Currentyear)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

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
            int[] occurrence = OccurrenceB(path);

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
        public static int[] ThirtySix(string path)
        {
            byte[] conditions = { 0xFF, 0xFF, 0x00, 0x36, 0x00, 0x00 };
            byte[] choice = { 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 };
            int pos1 = Search(path, conditions, false, 0, choice)[0];
            byte[] conditions2 = { 0x36, 0x00, 0x00, 0x00 };
            int pos2 = Search(path, conditions2, false, pos1+conditions.Length)[0];

            if (pos1 == 0 || pos2 == 0)
            {
                Error();
            }

            return new int[] {pos1, pos2};
        }
        static byte GetCatNumber(string path)
        {
            // If the save file is not a save file, don't modify/read it as one
            if (path.EndsWith(".list") || path.EndsWith(".pack") || path.EndsWith(".so") || path.EndsWith(".csv"))
            {
                return 0;
            }
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);
            byte anchour = 0;

            for (int i = 7344; i < 7375; i++)
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
            if (anchour == 0)
            {
                for (int i = 7375; i < 10800; i++)
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
            }
            stream.Close();
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
                if (text.Length < 2)
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

        public static int[] EvolvedFormsGetter_old()
        {
            Console.WriteLine("Downloading cat data...");
            WebClient client = new();
            string[] catData = new string[1000];
            try
            {
                catData = client.DownloadString("https://raw.githubusercontent.com/fieryhenry/Battle-Cats-Save-File-Editor/main/cats.csv").Split('\n');
            }
            catch
            {
                Console.WriteLine("Error, no internet connection exists to get evolve form data");
            }
            // Old editor versions download cats.csv as a physical file, not just a string
            if (File.Exists(@"cats.csv"))
            {
                File.Delete(@"cats.csv");
            }
            Console.WriteLine("Done\nParsing cat evolve forms...");
            List<string> listA = new();
            for (int i = 0; i < catData.Length; i++)
            {
                // Completely unnecessary, but I need to keep the ?s there to make sure older editor versions still work
                listA.Add(catData[i].Split('?')[0]);
            }
            int len = catData.Length;
            string[] first = new string[len];
            string[] second = new string[len];
            int[] form = new int[len];
            for (int i = 0; i < len; i++)
            {
                string[] f;
                try
                {
                    f = listA[i].Split('/');
                }
                catch
                {
                    break;
                }
                first[i] = f[0];
                try
                {
                    second[i] = f[1];
                }
                catch { form[i] = 1; }
            }
            for (int i = 0; i < len; i++)
            {
                if (form[i] == 0) form[i] = 2;
                else if (form[i] == 1) form[i] = 0;
                if (listA[i].Contains("EX,rarity,") && form[i] == 2) form[i] = 1;
                if (listA[i].Contains("Cat God the Awesome") || listA[i].Contains("Ururun Cat ") || listA[i].Contains("Ururun Cat ") || listA[i].Contains("Dark Emperor Catdam") || listA[i].Contains("Crimson Mina") || listA[i].Contains("Heroic Musashi") || listA[i].Contains("Mecha-Bun Mk II")) form[i] = 2;
            }
            Console.WriteLine("Done\nEditing your save...");
            return form;
        }
    }
}
