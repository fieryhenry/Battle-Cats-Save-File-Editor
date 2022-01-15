using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Windows.Forms;
using Battle_Cats_save_editor.Game_Mods;
using Battle_Cats_save_editor.SaveEdits;

namespace Battle_Cats_save_editor
{
    public class Editor
    {
        public static int catAmount = 0;
        public static string main_path;
        public static string gameVer;
        public static string version = "2.39.2";
        public static string multipleVals = "(You can enter multiple numbers seperated by spaces to edit multiple at once)";
        [STAThread]
        private static void Main()
        {
            bool error_catching = true;
            if (error_catching)
            {
                AppDomain.CurrentDomain.UnhandledException += delegate (object sender, UnhandledExceptionEventArgs eventArgs)
                {
                    Console.WriteLine("An error has occurred\nPlease report this in #bug-reports:\n");
                    Console.WriteLine(eventArgs.ExceptionObject.ToString() + "\n");
                    PatchSaveFile.patchSaveFile(main_path);
                    Console.WriteLine("\nPress enter to exit");
                    Console.ReadLine();
                    Environment.Exit(0);
                };
            }
            CheckUpdate();
            SelSave();
            CreateBackup();
            Options();
        }

        public static void SelSave()
        {
            Console.WriteLine("Select a battle cats save, or click cancel to download your save using transfer and confirmation codes");
            OpenFileDialog FD = new()
            {
                Filter = "battle cats save(*.*)|*.*",
                Title = "Select save"
            };
            if (FD.ShowDialog() == DialogResult.OK)
            {
                main_path = FD.FileName;
                ColouredText("&Save: &\"" + Path.GetFileName(main_path) + "\"& is selected\n", ConsoleColor.White, ConsoleColor.Green);
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
                ColouredText("&Detected game version: &" + gameVer + "&\n");
            }
        }

        public static string MakeRequest(WebRequest request)
        {
            request.Headers.Add("time-stamp", DateTime.Now.Ticks.ToString());
            WebResponse response = request.GetResponse();
            string result;
            using (Stream dataStream = response.GetResponseStream())
            {
                StreamReader reader = new(dataStream);
                string responseFromServer = reader.ReadToEnd();
                result = responseFromServer;
            }
            return result;
        }
        private static void CheckUpdate()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
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
                ColouredText("Application up to date - current version is " + version + "\n", ConsoleColor.White, ConsoleColor.Cyan);
            }
            else
            {
                if (lines != version && !skip)
                {
                    ColouredText("A new version is available would you like to update to release " + lines + "?\n", ConsoleColor.White, ConsoleColor.Green);
                    bool answer = OnAskUser("A new version is available would you like to update to release " + lines + "?", "Updater");
                    if (answer)
                    {
                        try
                        {
                            Process.Start("Updater.exe");
                            Environment.Exit(0);
                        }
                        catch
                        {
                            ColouredText("Error, the updater cannot be found, please download the latest version from the github instead\n", ConsoleColor.White, ConsoleColor.Red);
                        }
                    }
                }
            }
        }
        public static void CreateBackup()
        {
            try
            {
                byte[] save_data = File.ReadAllBytes(main_path);
                File.WriteAllBytes(main_path + "_backup", save_data);
                ColouredText($"\nBackup successfully created at: &{main_path + "_backup"}& \n\n", New: ConsoleColor.Green);
            }
            catch (Exception ex)
            {
                ColouredText($"\nCould not complete backup&: \n{ex.Message}&\n\n", New: ConsoleColor.Red);
            }
        }
        public static void Options()
        {
            ColouredText("Thanks to: Lethal's editor for being a tool for me to use when figuring out how to patch save files,and how to edit cf/xp\nAnd thanks to beeven and csehydrogen's open source work, which I used to implement the save patching algorithm\n\n&", New: ConsoleColor.Green);
            string[] Features = new string[]
            {
                "Select a new save",
                "Cat Food",
                "XP",
                "Tickets / Platinum Shards",
                "Leadership",
                "NP",
                "Treasures",
                "Battle Items",
                "Catseyes",
                "Cat Fruits / Seeds",
                "Talent Orbs",
                "Gamatoto",
                "Ototo",
                "Gacha Seed",
                "Equip Slots",
                "Gain / Remove Cats",
                "Cat / Stat Upgrades",
                "Cat Evolves",
                "Cat Talents",
                "Clear Levels / Outbreaks / Timed Score",
                "Inquiry Code / Elsewhere Fix / Unban",
                "Get Restart Pack",
                "Close the rank up bundle / offer menu",
                "Game Modding menu",
                "Calculate checksum of save file"
            };
            if (gameVer != "en")
            {
                ColouredText("Warning: if you are using a non en save, many features won't work, or they might corrupt your save data, so make sure you create a copy of your saves!\n", ConsoleColor.White, ConsoleColor.Red);
            }
            ColouredText($"&What would you like to do?&\n{CreateOptionsList<string>(Features, first: "Select a new save")}&");
            byte[] CatNumber = new byte[20];
            CatNumber[0] = GetCatNumber(main_path);
            CatNumber[1] = 2;
            catAmount = BitConverter.ToInt32(CatNumber, 0);
            switch ((int)Inputed())
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
                case 17: EvolveCats.Evolves(main_path); break;
                case 18: AllTalent.Talent(main_path); break;
                case 19: LevelSelect.Levels(main_path); break;
                case 20: NewInquiryCode.Inquiry(main_path); break;
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
            return Array.ConvertAll(input, (char x) => int.Parse(string.Format("{0}", x)));
        }

        public static string CreateOptionsList<T>(string[] options, T[] extravalues = null, bool numerical = true, bool skipZero = false, string first = null)
        {
            string toOutput = "";
            for (int i = 0; i < options.Length; i++)
            {
                if (extravalues != null)
                {
                    int val = Convert.ToInt32(extravalues[i]);
                    if (skipZero && val == 0)
                    {
                        continue;
                    }
                }
                if (numerical)
                {
                    if (first != null)
                    {
                        if (i == 0)
                        {
                            toOutput += $"{i}.& ";
                            toOutput += first;
                            options[i] = "&";
                        }
                        else
                        {
                            toOutput += $"&{i}.& ";
                        }
                    }
                    else
                    {
                        toOutput += $"&{i + 1}.& ";
                    }
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
            return toOutput;
        }

        private static void Upgrades(string path)
        {
            string[] Features = new string[]
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
                toOutput += string.Format("&{0}.& ", i);
                toOutput = toOutput + Features[i] + "\n";
            }
            ColouredText(toOutput);
            switch ((int)Inputed())
            {
                case 0:
                    Options();
                    break;
                case 1:
                    CatUpgrade.CatUpgrades(path);
                    break;
                case 2:
                    UpgradeCurrent.UpgradeCurrentCats(path);
                    break;
                case 3:
                    SpecificUpgrade.SpecifUpgrade(path);
                    break;
                case 4:
                    BlueUpgrade.Blue(path);
                    break;
                default:
                    Console.WriteLine(string.Format("Please enter a number between 0 and {0}", Features.Length));
                    break;
            }
        }

        private static void GetCats(string path)
        {
            string[] Features = new string[]
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
                toOutput += string.Format("&{0}.& ", i);
                toOutput = toOutput + Features[i] + "\n";
            }
            ColouredText(toOutput);
            switch ((int)Inputed())
            {
                case 0:
                    Options();
                    break;
                case 1:
                    GetCat.Cats(path);
                    break;
                case 2:
                    GetSpecificCats.SpecifiCat(path);
                    break;
                case 3:
                    RemoveCats.RemCats(path);
                    break;
                case 4:
                    RemoveSpecificCats.RemSpecifiCat(path);
                    break;
                default:
                    Console.WriteLine(string.Format("Please enter a number between 0 and {0}", Features.Length));
                    break;
            }
        }

        private static void Treasures(string path)
        {
            string[] Features = new string[]
            {
                "Go back",
                "All Treasures In A Chapter / Chapters",
                "Specific Treasure Types e.g Energy Drink, Void Fruit"
            };
            string toOutput = "&What do you want to edit?&\n0.& Go back\n&";
            for (int i = 1; i < Features.Length; i++)
            {
                toOutput += string.Format("&{0}.& ", i);
                toOutput = toOutput + Features[i] + "\n";
            }
            ColouredText(toOutput);
            switch ((int)Inputed())
            {
                case 0:
                    Options();
                    break;
                case 1:
                    AllTreasures.MaxTreasures(path);
                    break;
                case 2:
                    SpecificTreasures.VerySpecificTreasures(path);
                    break;
                default:
                    Console.WriteLine(string.Format("Please enter a number between 0 and {0}", Features.Length));
                    break;
            }
        }

        private static void Tickets(string path)
        {
            string[] Features = new string[]
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
                toOutput += string.Format("&{0}.& ", i);
                toOutput = toOutput + Features[i] + "\n";
            }
            ColouredText(toOutput);
            switch ((int)Inputed())
            {
                case 0:
                    Options();
                    break;
                case 1:
                    NormalTickets.CatTicket(path);
                    break;
                case 2:
                    RareTickets.RareCatTickets(path);
                    break;
                case 3:
                    PlatTickets.PlatinumTickets(path);
                    break;
                case 4:
                    LegendTickets.LegendTicket(path);
                    break;
                case 5:
                    PlatinumShards.PlatShards(path);
                    break;
                default:
                    Console.WriteLine(string.Format("Please enter a number between 0 and {0}", Features.Length));
                    break;
            }
        }

        private static void Gamatoto(string path)
        {
            string[] Features = new string[]
            {
                "Go back",
                "Catamins",
                "Helpers",
                "XP"
            };
            string toOutput = "&What would you like to edit?&\n0.& Go back\n&";
            for (int i = 1; i < Features.Length; i++)
            {
                toOutput += string.Format("&{0}.& ", i);
                toOutput = toOutput + Features[i] + "\n";
            }
            ColouredText(toOutput);
            switch ((int)Inputed())
            {
                case 0:
                    Options();
                    break;
                case 1:
                    Catamins.Catamin(path);
                    break;
                case 2:
                    GamatotoHelper.GamHelp(path);
                    break;
                case 3:
                    GamatotoXP.GamXP(path);
                    break;
                default:
                    Console.WriteLine(string.Format("Please enter a number between 0 and {0}", Features.Length));
                    break;
            }
        }

        private static void Ototo(string path)
        {
            string[] Features = new string[]
            {
                "Go back",
                "Base Materials",
                "Engineers",
                "Cat Cannon Upgrades"
            };
            string toOutput = "&What would you like to edit?&\n0.& Go back\n&";
            for (int i = 1; i < Features.Length; i++)
            {
                toOutput += string.Format("&{0}.& ", i);
                toOutput = toOutput + Features[i] + "\n";
            }
            ColouredText(toOutput);
            switch ((int)Inputed())
            {
                case 0:
                    Options();
                    break;
                case 1:
                    BaseMaterials.BaseMats(path);
                    break;
                case 2:
                    OtotoEngineers.Engineers(path);
                    break;
                case 3:
                    OtotoCatCannon.CatCannon(path);
                    break;
                default:
                    Console.WriteLine(string.Format("Please enter a number between 0 and {0}", Features.Length));
                    break;
            }
        }

        private static void GameModdingMenu(string path)
        {
            string[] Features = new string[]
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
                toOutput += string.Format("&{0}.& ", i);
                toOutput = toOutput + Features[i] + "\n";
            }
            ColouredText(toOutput);
            switch ((int)Inputed())
            {
                case 0:
                    Options();
                    break;
                case 1:
                    DecryptPack.Decrypt("b484857901742afc");
                    break;
                case 2:
                    EncryptPack.EncryptData("b484857901742afc");
                    break;
                case 3:
                    MD5Libnative.MD5Lib(path);
                    break;
                case 4:
                    GameFileParsing(path);
                    break;
                default:
                    Console.WriteLine(string.Format("Please enter a number between 0 and {0}", Features.Length));
                    break;
            }
        }

        private static void GameFileParsing(string path)
        {
            string[] Features = new string[]
            {
                "Go back",
                "Edit unit*.csv (cat data)",
                "Edit stage*.csv (level data)",
                "Edit t_unit.csv (enemy data)",
            };
            string toOutput = "&What would you like to edit?&\n0.& Go back\n&";
            for (int i = 1; i < Features.Length; i++)
            {
                toOutput += string.Format("&{0}.& ", i);
                toOutput = toOutput + Features[i] + "\n";
            }
            ColouredText(toOutput);
            switch ((int)Inputed())
            {
                case 0:
                    GameModdingMenu(path);
                    break;
                case 1:
                    UnitMod.Unitcsv();
                    break;
                case 2:
                    StageMod.Stagecsv();
                    break;
                case 3:
                    EnemyMod.EnemyCSV();
                    break;
                case 4:
                    FileHandler.AddExtraBytes(path);
                    break;
                default:
                    Console.WriteLine(string.Format("Please enter a number between 0 and {0}", Features.Length));
                    break;
            }
        }

        public static string CalculateMD5(string path)
        {
            MD5 md5 = MD5.Create();
            byte[] allData = File.ReadAllBytes(path);
            byte[] hash = md5.ComputeHash(allData);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

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
            int num = 1;
            List<int> values = new();
            int end = conditions.Length;
            int stop_count = 0;
            if (negative)
            {
                num = -1;
                int start = conditions.Length - 1;
            }
            for (int i = startpoint; i < endpoint; i += num)
            {
                if (stop_after > -1 && stop_count >= stop_after && values.Count > 0)
                {
                    break;
                }
                count = 0;
                for (int j = 0; j < conditions.Length; j++)
                {
                    if (negative)
                    {
                        try
                        {
                            if (allData[i - j] == conditions[conditions.Length - 1 - j] || mult[conditions.Length - 1 - j] == 1)
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
                        if (allData[i + j] == conditions[j] || mult[j] == 1)
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
            if (values.Count == 0)
            {
                values.Add(0);
            }
            return values.ToArray();
        }

        public static int GetOtotoPos(string path)
        {
            byte[] conditions = new byte[]
            {
                0,0,0,8,0,0,0,0,0,0,0,2,0,0,0,3,0,0,0
            };
            int pos = Search(path, conditions, false, 0, null, -1, -1)[0];
            if (pos < 200)
            {
                Error("Error, a position couldn't be found, please report this in #bug-reports on discord");
            }
            return pos;
        }

        private static bool OnAskUser(string title, string title2)
        {
            return DialogResult.Yes == MessageBox.Show(title, title2, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        public static byte[] Endian(long num)
        {
            return BitConverter.GetBytes(num);
        }

        public static string ByteArrayToString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }

        public static void ColouredText(string input, ConsoleColor Base = ConsoleColor.White, ConsoleColor New = ConsoleColor.DarkYellow)
        {
            string[] split = input.Split('&');
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

        public static int AskSentances(int amount, string item, bool set = false, int max = 2147483647)
        {
            int result;
            if (!set)
            {
                ColouredText(string.Format("&You have &{0}& {1}\n", amount, item));
                if (max == int.MaxValue)
                {
                    ColouredText("&What do you want to set your " + item + " to?:\n");
                }
                else
                {
                    ColouredText(string.Format("&What do you want to set your {0} to? (max {1}):\n", item, max));
                }
                amount = (int)Inputed();
                amount = MaxMinCheck(amount, max, 0);
                result = amount;
            }
            else
            {
                ColouredText(string.Format("&Successfully set {0} to &{1}&\n", item, amount));
                result = 0;
            }
            return result;
        }

        public static int MaxMinCheck(int val, int max, int min = 0)
        {
            if (val > max)
            {
                val = max;
            }
            if (val < min)
            {
                val = min;
            }
            return val;
        }

        public static int[] GetItemData(string path, int amount, int separator, int startPos)
        {
            byte[] allData = File.ReadAllBytes(path);
            int[] items = new int[amount];
            for (int i = 0; i < amount; i++)
            {
                byte[] items_ba = allData.Skip(startPos + i * separator).Take(separator).ToArray();
                int item;
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
            using FileStream stream = new(path, FileMode.Open, FileAccess.ReadWrite);
            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);
            for (int i = 0; i < items.Length; i++)
            {
                byte[] item = Endian(items[i]);
                stream.Position = startPos + i * separator;
                stream.Write(item, 0, separator);
            }
        }

        public static long Inputed()
        {
            long input = 0L;
            try
            {
                input = Convert.ToInt64(Console.ReadLine());
            }
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
            byte[] conditions = { Currentyear[0], Currentyear[1], 0, 0 };
            return Search(path, conditions);
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
            byte[] array = new byte[4];
            array[0] = 44;
            array[1] = 1;
            byte[] conditions = array;
            int pos = Search(path, conditions, false, occurrence[5] - 400, null, occurrence[5], -1)[0];
            int pos2 = occurrence[5];
            if (pos == 0)
            {
                pos = Search(path, conditions, true, occurrence[4] - 400, null, occurrence[4], -1)[0];
                pos2 = occurrence[4];
            }
            if (pos == 0)
            {
                Error("Error, a position couldn't be found, please report this in #bug-reports on discord");
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
            byte[] conditions = new byte[]
            {
                byte.MaxValue, byte.MaxValue, 0, 54, 0, 0
            };
            byte[] array = new byte[6];
            array[2] = 1;
            byte[] choice = array;
            int pos = Search(path, conditions, false, 0, choice, -1, -1)[0];
            byte[] array2 = new byte[4];
            array2[0] = 54;
            byte[] conditions2 = array2;
            int pos2 = Search(path, conditions2, false, pos + conditions.Length, null, -1, -1)[0];
            if (pos == 0 || pos2 == 0)
            {
                Error("Error, a position couldn't be found, please report this in #bug-reports on discord");
            }
            return new int[]
            {
                pos,
                pos2
            };
        }

        private static byte GetCatNumber(string path)
        {
            byte result;
            if (path.EndsWith(".list") || path.EndsWith(".pack") || path.EndsWith(".so") || path.EndsWith(".csv"))
            {
                result = 0;
            }
            else
            {
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
                result = anchour;
            }
            return result;
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
