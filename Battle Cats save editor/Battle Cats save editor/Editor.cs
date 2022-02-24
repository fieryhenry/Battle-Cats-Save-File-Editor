using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Security.Cryptography;
using System.Windows.Forms;
using static Battle_Cats_save_editor.FeatureHandler;
using Battle_Cats_save_editor.SaveEdits;

namespace Battle_Cats_save_editor
{
    public class Editor
    {
        public static string main_path;
        public static string gameVer;
        public static bool override_warning_message = false;
        public static string version = "2.41.0";
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
                    PatchSaveFile.PatchSaveData(main_path);
                    Console.WriteLine("\nPress enter to exit");
                    Console.ReadLine();
                    Environment.Exit(0);
                };
            }
            CheckUpdate();
            SelSave();
            CreateBackup(main_path);
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
                SendAndReceiveSaveData.Download_Save_Data();
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
        public static T[][] SplitArray<T>(T[] array, int separator)
        {
            int i = 0;
            var query = from s in array
                        let num = i++
                        group s by num / separator into g
                        select g.ToArray();
            return query.ToArray();
        }
        public static string MakeRequest(WebRequest request)
        {
            request.Headers.Add("time-stamp", DateTime.Now.Ticks.ToString());
            HttpRequestCachePolicy noCachePolicy = new(HttpRequestCacheLevel.NoCacheNoStore);
            request.CachePolicy = noCachePolicy;
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
        public static void CreateBackup(string path)
        {
            try
            {
                byte[] save_data = File.ReadAllBytes(path);
                File.WriteAllBytes(path + "_backup", save_data);
                ColouredText($"\nBackup successfully created at: &{path + "_backup"}& \n\n", New: ConsoleColor.Green);
            }
            catch (Exception ex)
            {
                ColouredText($"\nCould not complete backup&: \n{ex.Message}&\n\n", New: ConsoleColor.Red);
            }
        }
        public static int GetCatAmount(string path)
        {
            byte[] CatNumber = { GetCatNumber(path), 2 };
            int catAmount = BitConverter.ToInt16(CatNumber, 0);
            return catAmount;
        }
        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length).Where(x => x % 2 == 0).Select(x => Convert.ToByte(hex.Substring(x, 2), 16)).ToArray();
        }
        public static int GetGameVersionOffset(string path)
        {
            byte[] data = File.ReadAllBytes(path);
            int offset;
            // jp
            if (data[132] == 0x0f) offset = -1;
            // en
            else if (data[133] == 0x0f) offset = 0;
            else
            {
                // jp
                if (gameVer == "jp") offset = -1;
                // en
                else offset = 0;
            }
            return offset;
        }

        public static int[] ConvertCharArrayToIntArray(char[] input)
        {
            return Array.ConvertAll(input, (char x) => int.Parse(string.Format("{0}", x)));
        }

        public static string CreateOptionsList<T>(string[] options, T[] extravalues = null, bool numerical = true, bool skipZero = false, string first = null, bool color = true)
        {
            string toOutput = "";
            for (int i = 0; i < options.Length; i++)
            {
                if (extravalues != null && skipZero)
                {
                    int val = Convert.ToInt32(extravalues[i]);
                    if (val == 0)
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
            if (color)
            {
                return toOutput;
            }
            return toOutput.Replace("&", "");
        }

        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
        public static double ConvertBytesToMegabytes(long bytes)
        {
            return Math.Round((bytes / 1024f) / 1024f, 2);
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

            int pos = 0;
            int num = 1;
            List<int> values = new();
            int stop_count = 0;
            if (negative)
            {
                num = -1;
            }
            for (int i = startpoint; i < endpoint; i += num)
            {
                if (stop_after > -1 && stop_count >= stop_after && values.Count > 0)
                {
                    break;
                }
                int count = 0;
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

        public static int[] GetItemData(string path, int amount, int separator, int startPos, bool warning = true)
        {
            if (override_warning_message)
            {
                warning = false;
            }
            List<byte> allData = File.ReadAllBytes(path).ToList();
            int[] items = new int[amount];
            for (int i = 0; i < amount; i++)
            {
                byte[] items_ba = allData.GetRange(startPos + i * separator, separator).ToArray();
                int item;
                if (separator > 2)
                {
                    item = BitConverter.ToInt32(items_ba, 0);
                }
                else if (separator == 1)
                {
                    item = items_ba[0];
                }
                else
                {
                    item = BitConverter.ToInt16(items_ba, 0);

                }
                items[i] = item;
            }
            if (warning)
            {
                Console.WriteLine("Note: If any of these numbers are incorrect, then don't edit them, as it could corrupt your save. If this is the case please report it on discord");
            }
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

        public static void SetItemData(string path, int[] items, int separator, int startPos, int force_length = 0)
        {
            using FileStream stream = new(path, FileMode.Open, FileAccess.ReadWrite);
            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);
            int len = items.Length;
            if (force_length > 0) len = force_length;
            if (len > items.Length) len = items.Length;
            for (int i = 0; i < len; i++)
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
            return Search(path, conditions, startpoint: 500);
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
