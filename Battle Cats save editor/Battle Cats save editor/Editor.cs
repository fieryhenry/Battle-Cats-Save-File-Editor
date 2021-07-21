using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using static System.Environment;

namespace Battle_Cats_save_editor
{

    class Editor
    {
        static int catAmount = 0;
        static string[] Savepaths = new string[500];
        static string gameVer = "";
        [STAThread]
        static void Main()
        {
            CheckUpdate();
            SelSave();
            Options();

        }
        static void SelSave()
        {
            var FD = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "battle cats save(*.*)|*.*"
            };
            if (FD.ShowDialog() == DialogResult.OK)
            {
                string[] fileToOpen = FD.FileNames;
                for (int i = 0; i < fileToOpen.Length; i++)
                {
                    ColouredText("&Save: &\"" + Path.GetFileName(fileToOpen[i]) + "\"&\n", ConsoleColor.White, ConsoleColor.Green);
                }
                Savepaths = fileToOpen;
            }
            else
            {
                ColouredText("\nPlease select your save\n\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                SelSave();
            }
            Console.WriteLine("What game version are you using? (en or jp)");
            gameVer = Console.ReadLine();
            if (gameVer != "en" && gameVer != "jp")
            {
                Console.WriteLine("answer was not en or jp");
                SelSave();
            }
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
            WebClient webClient = new WebClient();
            webClient.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
            bool skip = false;
            string lines = "";
            try
            {
                lines = webClient.DownloadString("https://raw.githubusercontent.com/fieryhenry/Battle-Cats-Save-File-Editor/main/version.txt").Trim('\n');
            }
            catch (WebException)
            {
                ColouredText("No internet connection to check for a new version\n", ConsoleColor.White, ConsoleColor.Red);
                skip = true;
            }
            string version = "2.27.1";

            if (lines == version && !skip)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Application up to date - current version is {0}", version);
            }
            else if (lines != version && !skip)
            {
                ColouredText("A new version is available would you like to update?\n", ConsoleColor.White, ConsoleColor.Green);
                bool answer = OnAskUser("A new version is available would you like to update?", "Updater");
                if (answer)
                {
                    System.Diagnostics.Process.Start(@"Updater.exe");
                    Exit(1);
                }
            }
        }
        static void Options()
        {
            string[] fileToOpen = Savepaths;
            string path = Path.Combine(fileToOpen[0]);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nBackup your save before using this editor!\nIf you get an error along the lines of \"Your save is active somewhere else\"then select option 25-->2, and set your inquiry code to a save that doesn't have that error\n", fileToOpen);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Thanks to: Lethal's editor for being a tool for me to use when figuring out how to patch save files, uploading the save data onto the servers how to and edit cf/xp\nAnd thanks to beeven and csehydrogen's open source work, which I used to implement the save patching algorithm\n");
            Console.ForegroundColor = ConsoleColor.White;
            
            ColouredText("Warning: if you are using a jp save, many features won't work, or they might corrupt your save data, so make sure you back up your saves!\n", ConsoleColor.White, ConsoleColor.Red);

            ColouredText("&What would you like to do?&\n0.& Select a new save\n1.& Change Cat food\n&2.& Change XP\n&3.& Get all treasures to a specific level\n&4.& All cats upgraded to a specific level" +
                "\n&5.& Change leadership\n&6.& Change NP\n&7.& Change cat tickets\n&8.& change rare cat tickets" +
                "\n&9.& Change platinum tickets\n&10.& Change gacha seed\n&11.& All cats evolved(you must first have unlocked the ability to " +
                "evolve cats + you need to click the \"cycle\" icon on the bottom right of your cat)\n&12.& Change battle item count\n&13.& " +
                "Change Catamins" +
                "\n&14.& Change base materials\n&15.& Change catseyes(must have catseyes unlocked)\n&16.& Get/remove All cats\n&17.& Get/remove a specific cat" +
                "\n&18.& Upgrade a specific cat to a specific level\n" +
                "&19.& Unlock treasures of a specific chapter\n" +
                "&20.& Evolve a specific cat\n&21.& Change cat fruits and cat fruit seeds\n&22.& Talent upgrade cats(Must have NP unlocked)&(Experimental and buggy - use at your own risk)&\n" +
                "&23.& Clear story chapters\n&24.& Patch data\n&25.& More small edits and fixes\n&26.& Display current gacha seed\n&27.& Change all " +
                "into the future timed score rewards\n&28.& Clear stories of legends subchpaters chapters (doesn't include uncanny legends)\n" +
                "&29.& Edit gamatoto helpers\n&30.& Edit gamatoto xp\n&31.& Decrypt .pack and .list files in /files directory of the game (also the ones in the " +
                "apk if you are using an older game version)\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            byte[] anchour = new byte[20];
            anchour[0] = Anchour(path);
            anchour[1] = 0x02;
            catAmount = BitConverter.ToInt32(anchour, 0);
            int Choice = Inputed();
            for (int i = 0; i < fileToOpen.Length; i++)
            {
                path = fileToOpen[i];
                switch (Choice)
                {
                    case 0: SelSave(); Options(); break;
                    case 1: CatFood(path); break;
                    case 2: XP(path); break;
                    case 19: SepecTreasures(path); break;
                    case 4: CatUpgrades(path); break;
                    case 5: Leadership(path); break;
                    case 6: NP(path); break;
                    case 7: CatTicket(path); break;
                    case 8: CatTicketRare(path); break;
                    case 9: PlatTicketRare(path); break;
                    case 10: Seed(path); break;
                    case 11: Evolve(path); break;
                    case 12: Items(path); break;
                    case 13: Catamin(path); break;
                    case 14: BaseMats(path); break;
                    case 15: Catseyes(path); break;
                    case 16: Cats(path); break;
                    case 17: SpecifiCat(path); break;
                    case 18: SpecifUpgrade(path); break;
                    case 3: MaxTreasures(path); break;
                    case 20: EvolveSpecific(path); break;
                    case 21: CatFruit(path); break;
                    case 22: Talents(path); break;
                    case 23: Stage(path); break;
                    case 24:
                        Encrypt(gameVer, path);
                        if (i == fileToOpen.Length - 1)
                        {
                            Console.WriteLine("Press enter to exit");
                            Exit(0);
                        }
                        break;
                    case 25: Menu(path); break;
                    case 26: GetSeed(path); break;
                    case 27: TimedScore(path); break;
                    case 28: SoL(path); break;
                    case 29: GamHelp(path); break;
                    case 30: GamXP(path); break;
                    case 34: UploadSave(gameVer, path); break;
                    case 31:
                        Decrypt("b484857901742afc", "89a0f99078419c28");
                        if (i == fileToOpen.Length - 1)
                        {
                            Console.WriteLine("Press enter to exit");
                            Exit(0);
                        }
                        break;
                    case 32:
                        EncryptData(path, "b484857901742afc", "89a0f99078419c28");
                        if (i == fileToOpen.Length - 1)
                        {
                            Console.WriteLine("Press enter to exit");
                            Exit(0);
                        }
                        break;
                    case 33: TalentOrbs(path); break;
                    default: Console.WriteLine("Please input a number that is recognised"); break;
                }
                Encrypt(gameVer, path);
            }
            ColouredText("Press enter to continue\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            Console.ReadLine();
            Options();
        }
        static void Elsewhere(string path2)
        {
            Console.WriteLine("Please select a working save that doesn't have 'Save is used elsewhere' and has never had it in the past\nPress enter to select that save");
            Console.ReadLine();
            var FD = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "working battle cats save(*.*)|*.*"
            };
            string path = "";
            if (FD.ShowDialog() == DialogResult.OK)
            {
                string[] fileToOpen = FD.FileNames;
                path = Path.Combine(fileToOpen[0]);
            }
            else
            {
                ColouredText("\nPlease select your save\n\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                SelSave();
            }

            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);
            byte[] codeBytes = new byte[36];
            byte[] codeBytes2 = new byte[9];
            byte[] iqExtra = new byte[11];
            byte[] lastKey = new byte[40];
            int[] found = new int[6];

            for (int i = 0; i < allData.Length; i++)
            {
                if (allData[i] == 0 && allData[i+9] == 0x2d && allData[i+14] == 0x2d && allData[i+19] == 0x2d && allData[i+24] == 0x2d)
                {
                    Array.Copy(allData, i + 1, codeBytes, 0, 36);
                    Array.Copy(allData, i - 71, codeBytes2, 0, 9);
                    found[0] = 1;
                    break;
                }
            }
            for (int i = 0; i < allData.Length; i++)
            {
                if (allData[i] == 0x2D && allData[i + 1] == 0x0 && allData[i + 2] == 0x0 && allData[i + 3] == 0x0 && allData[i + 4] == 0x2E)
                {
                    for (int j = 1900; j < 2108; j++)
                    {
                        if (allData[i - j] == 09)
                        {
                            found[1] = 1;
                            Array.Copy(allData, i - j + 4, iqExtra, 0, 11);
                        }
                    }
                }
            }
            for (int i = allData.Length - 800; i < allData.Length; i++)
            {
                if (allData[i] == 0x78 && allData[i + 1] == 0x63 && allData[i + 2] == 1 && allData[i + 3] == 0 && allData[i - 1] == 0)
                {
                    Array.Copy(allData, i + 15, lastKey, 0, 40);
                    found[2] = 1;
                    break;
                }
            }
            if (found.Sum() < 3)
            {
                Console.WriteLine("Sorry your a position couldn't be found\nEither your save is invalid or the edtior is bugged, if it is please contact me on the discord linked in the readme.md");
                return;
            }
            stream.Close();
            using var stream2 = new FileStream(path2, FileMode.Open, FileAccess.ReadWrite);

            length = (int)stream2.Length;
            allData = new byte[length];
            stream2.Read(allData, 0, length);

            for (int i = 0; i < allData.Length; i++)
            {
                if (allData[i] == 0 && allData[i + 9] == 0x2d && allData[i + 14] == 0x2d && allData[i + 19] == 0x2d && allData[i + 24] == 0x2d)
                {
                    stream2.Position = i + 1;
                    stream2.Write(codeBytes, 0, 36);
                    stream2.Position = i -71;
                    stream2.Write(codeBytes2, 0, 9);
                    found[3] = 1;
                    break;
                }
            }
            for (int i = 0; i < allData.Length; i++)
            {
                if (allData[i] == 0x2D && allData[i + 1] == 0x0 && allData[i + 2] == 0x0 && allData[i + 3] == 0x0 && allData[i + 4] == 0x2E)
                {
                    for (int j = 1900; j < 2108; j++)
                    {
                        if (allData[i - j] == 09)
                        {
                            found[4] = 1;
                            stream2.Position = i - j + 4;
                            stream2.Write(iqExtra, 0, 11);
                            break;
                        }
                    }
                }
            }

            for (int i = allData.Length-800; i <allData.Length; i++)
            {
                if (allData[i] == 0x78 && allData[i + 1] == 0x63 && allData[i + 2] == 1 && allData[i + 3] == 0 && allData[i - 1] == 0)
                {
                    stream2.Position = i + 15;
                    stream2.Write(lastKey, 0, 40);
                    found[5] = 1;
                    break;
                }
            }
            if (found.Sum() < 6)
            {
                Console.WriteLine("Sorry a position couldn't be found\nEither your save is invalid or the edtior is bugged, if it is please contact me on the discord linked in the readme.md");
                return;
            }
            else
            {
                Console.WriteLine("Success");
            }
        }
        static void Decrypt(string key, string key2)
        {
            Console.WriteLine("Do you want the lists of the files contained within the .pack files to be outputed in the console?(yes/no) When slecting .pack and .list files, make sure the names match, you can select multiple packs and lists at the same time");
            string answer = Console.ReadLine();
            bool spam = false;
            if (answer == "yes")
            {
                spam = true;
            }
            OpenFileDialog fd = new OpenFileDialog();
            fd.Multiselect = true;
            fd.Filter = "files (*.pack; .list)|*.pack;*.list";
            if (fd.ShowDialog() != DialogResult.OK)
            {
                Console.WriteLine("Please select .pack and .list files");
                Options();
            }
            string[] paths = fd.FileNames;
            bool hasPack = false;
            for (int i = 0; i < paths.Length; i++)
            {
                if (paths[i].EndsWith(".pack"))
                {
                    hasPack = true;
                }
            }
            if (hasPack)
            {
                string[] lists = new string[paths.Length];
                string[] packs = new string[paths.Length];
                for (int i = 0; i < paths.Length; i++)
                {
                    if (paths[i].EndsWith(".pack"))
                    {
                        packs[i] = paths[i];
                    }
                    else if (paths[i].EndsWith(".list"))
                    {
                        lists[i] = paths[i];
                    }
                }
                string[] listLines = new string[25000];
                string[] names = new string[listLines.Length];
                int[] startpos = new int[listLines.Length];
                int[] offset = new int[listLines.Length];
                for (int i = 0; i < paths.Length; i++)
                {
                    if (paths[i].EndsWith(".list"))
                    {
                        using var stream = new FileStream(paths[i], FileMode.Open, FileAccess.ReadWrite);

                        int length = (int)stream.Length;
                        byte[] allData = new byte[length];
                        stream.Read(allData, 0, length);

                        byte[] IV = new byte[16];
                        byte[] Key = Encoding.ASCII.GetBytes(key);

                        string plaintext = null;

                        string result = Path.GetFileName(paths[i]);

                        using Aes aesAlg = Aes.Create();
                        aesAlg.Key = Key;
                        aesAlg.IV = IV;
                        aesAlg.Padding = PaddingMode.None;
                        aesAlg.Mode = CipherMode.ECB;

                        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                        using MemoryStream msDecrypt = new MemoryStream(allData);
                        using CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                        using StreamReader srDecrypt = new StreamReader(csDecrypt);

                        plaintext = srDecrypt.ReadToEnd();
                        if (!Directory.Exists("Decomp_lists/"))
                        {
                            Directory.CreateDirectory("Decomp_lists/");
                        }
                        File.WriteAllText(@"decomp_lists/" + result, plaintext);
                        listLines = File.ReadAllLines(@"decomp_lists/" + result);
                        for (int j = 1; j < listLines.Length - 1; j++)
                        {
                            try
                            {
                                names[j] = listLines[j].Split(',').ElementAt(0);
                                startpos[j] = int.Parse(listLines[j].Split(',').ElementAt(1));
                                offset[j] = int.Parse(listLines[j].Split(',').ElementAt(2));
                            }
                            catch
                            {
                                startpos[j] = startpos[0];
                                offset[j] = offset[0];
                                names[j] = names[0];
                            }

                        }
                    }
                    else if (paths[i].EndsWith(".pack"))
                    {
                        using var stream = new FileStream(paths[i], FileMode.Open, FileAccess.ReadWrite);

                        int length = (int)stream.Length;
                        byte[] allData = new byte[length];
                        stream.Read(allData, 0, length);

                        for (int j = 1; j < listLines.Length - 1; j++)
                        {
                            byte[] content = new byte[offset[j]];
                            Array.Copy(allData, startpos[j], content, 0, offset[j]);

                            byte[] IV = new byte[16];
                            byte[] Key = Encoding.ASCII.GetBytes(key2);

                            using Aes aesAlg = Aes.Create();
                            aesAlg.Key = Key;
                            aesAlg.IV = IV;
                            aesAlg.Padding = PaddingMode.None;
                            aesAlg.Mode = CipherMode.ECB;

                            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                            if (!Directory.Exists("game_files/"))
                            {
                                Directory.CreateDirectory("game_files");
                            }
                            File.WriteAllBytes(@"game_files/content", content);
                            using var stream2 = new FileStream(@"game_files/content", FileMode.Open, FileAccess.ReadWrite);

                            using CryptoStream csDecrypt = new CryptoStream(stream2, decryptor, CryptoStreamMode.Read);
                            byte[] bytes = new byte[offset[j]];
                            csDecrypt.Read(bytes, 0, offset[j]);

                            try
                            {
                                Directory.CreateDirectory(@"game_files/" + Path.GetFileName(paths[i]));
                                File.WriteAllBytes(@"game_files/" + Path.GetFileName(paths[i]) + "/" + names[j], bytes);
                            }
                            catch
                            {

                            }
                            if (spam)
                            {
                                ColouredText("&Extracted: &" + names[j] + " &from &" + Path.GetFileName(paths[i]) + " &" + j + "&/&" + (listLines.Length - 2) + " - " + (i + 1) / 2 + "/" + paths.Length / 2 + "\n", ConsoleColor.White, ConsoleColor.Green);
                            }
                        }
                        Console.WriteLine("Decompiled: " + Path.GetFileName(paths[i]) + " " + (i + 1) / 2 + "/" + paths.Length / 2);

                    }
                }
                Console.WriteLine("Finished: files can be found in " + Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/game_files/");
            }
        }
        static void EncryptData(string patht, string key, string key2)
        {
            Console.WriteLine("Enter name of .pack file to be outputed");
            string name = Console.ReadLine();
            FolderBrowserDialog fd = new FolderBrowserDialog();
            fd.SelectedPath = Path.GetDirectoryName(patht);
            if (fd.ShowDialog() != DialogResult.OK)
            {
                Console.WriteLine("Please select a folder of game content!");
                Options();
            }
            string path = fd.SelectedPath;
            string listFile = "";
            string[] files = Directory.GetFiles(path);
            listFile += files.Length + "\n";
            long amount = 0;
            for (int i = 0; i < files.Length; i++)
            {
                FileInfo attributes = new FileInfo(files[i]);
                try
                {
                    FileInfo attributesb = new FileInfo(files[i - 1]);
                    amount += attributes.Length;
                    listFile += Path.GetFileName(files[i]) + "," + amount + "," + attributes.Length + "\n";
                }
                catch
                {
                    listFile += Path.GetFileName(files[i]) + "," + "0" + "," + attributes.Length + "\n";
                }
                Console.WriteLine("Done: " + files[i] + ", " + i + "/" + files.Length);

            }
            if (!Directory.Exists("Complists/"))
            {
                Directory.CreateDirectory("Complists/");
            }
            File.WriteAllText(@"Complists/" + name, listFile);


            using var stream = new FileStream(@"Complists/" + name, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);
            int left = 0;
            stream.Close();
            if (allData.Length % 16 != 0)
            {
                for (int i = 0; i < 16 - (allData.Length % 16); i++)
                {
                    Console.WriteLine(allData.Length + i);
                    left++;
                    Console.WriteLine(left);
                }
                for (int i = 0; i < left; i++)
                {
                    allData.Append(Convert.ToByte(left));
                }
                File.WriteAllBytes(@"Complists/" + name, allData);
            }
        }
        static void UploadSave(string choice, string path)
        {
            string answer = Encrypt(choice, path);
            string url1 = "https://nyanko.ponosgames.com/?action=store&country=en";
            if (answer == "jp")
            {
                url1 = "https://nyanko.ponosgames.com/?action=store&country=jp";
            }
            else if (answer != "en")
            {
                Console.WriteLine("Error, please enter en or jp");
            }
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            var client = new WebClient
            {
                BaseAddress = url1,
            };
            client.Headers.Add("Content-Type", "multipart/form-data;boundary=");

            string data = Encoding.Default.GetString(allData);
            ColouredText("Uploading saves is possible thanks to Lethal's save editor, as I could see the requests it made to the game servers allowing me to figure out how to do this&\nUploading save...", ConsoleColor.White, ConsoleColor.Green);
            string response = client.UploadString(url1, "--\n" +
                "Content-Disposition: attachment; name=\"saveData\";filename=\"data.sav\"" +
                "\nContent-Type: application/octet-stream\n\n" + data + "\n----");

            ColouredText("&Your codes are:\nTransfer:&" + response.Substring(14, 9) + "\n&Confirmation:&" + response.Substring(32, 4) + "\nPut these codes back into the game\n", ConsoleColor.White, ConsoleColor.DarkYellow);


        }
        static void Menu(string path)
        {
            ColouredText("&Welcome to the small patches and tweaks menu&\n&1.&Close all the bundle menus (if you have used upgrade all cats, you know what this is)\n&2.&Set new account to avoid error \"Your save is being used somewhere else\" bug " +
                "\n&3.&Upgrade the blue upgrades on the right of the normal cat upgrades\n&4.&Fix gamatoto" +
                "(use if your gamatoto crashes your game)\n&5.&Fix save is used elsewhere error, whilst selecting a save that has the error(the one you select when you open the editor) select a new save that has never had the save is used elsewhere bug ever(you can re-install the game to get a save like that) and it should fix that error\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            int choice = Inputed();

            switch (choice)
            {
                case 1: Bundle(path); break;
                case 2: NewIQ(path); break;
                case 3: Blue(path); break;
                case 4: FixGam(path); break;
                case 5: Elsewhere(path); break;
                default: Console.WriteLine("Please input a number that is recognised"); break;

            }
        }
        static void TalentOrbs(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            long startPos = 0;
            long endPos = 0;
            int lengthst = 0;
            for (int i = 5; i < 1294; i++)
            {
                if (allData[allData.Length - i] == 0x84 && allData[allData.Length - i + 1] == 0x61 && allData[allData.Length - i + 2] == 0x01)
                {
                    startPos = allData.Length - i;
                    lengthst = allData[startPos + 4] * 3;
                    break;
                }
                else if (allData[allData.Length - i] == 0x01 && allData[allData.Length - i + 1] == 0x4C && allData[allData.Length - i + 2] == 0x62 && allData[allData.Length - i + 3] == 0x01)
                {
                    endPos = allData.Length - i;
                }
            }
            Console.WriteLine(lengthst);
            int[] orbs = new int[65];
            long[] orbPos = new long[65];
            int j = 0;
            for (int i = 6; i < endPos - startPos - 2; i++)
            {
                if (i == lengthst + 8)
                {
                    break;
                }
                if ((i - 6) % 3 == 0)
                {
                    try
                    {
                        orbs[allData[startPos + i - 3]] = allData[startPos + i - 1];
                        orbPos[allData[startPos + i - 3]] = startPos + i - 1;
                    }
                    catch
                    {
                        break;
                    }
                    j++;
                }
            }
            string[] orbList = { "Red D attack", "Red C attack", "Red B attack", "Red A attack", "Red S attack", "Red D defense", "Red C defense", "Red B defense", "Red A defense", "Red S defense", "Floating D attack", "Floating C attack", "Floating B attack", "Floating A attack", "Floating S attack", "Floating D defense", "Floating C defense", "Floating B defense", "Floating A defense", "Floating S defense", "Black D attack", "Black C attack", "Black B attack", "Black A attack", "Black S attack", "Black D defense", "Black C defense", "Black B defense", "Black A defense", "Black S defense", "Metal D defense", "Metal C defense", "Metal B defense", "Metal A defense", "Metal S defense", "Angel D attack", "Angel C attack", "Angel B attack", "Angel A attack", "Angel S attack", "Angel D defense", "Angel C defense", "Angel B defense", "Angel A defense", "Angel S defense", "Alien D attack", "Alien C attack", "Alien B attack", "Alien A attack", "Alien S attack", "Alien D defense", "Alien C defense", "Alien B defense", "Alien A defense", "Alien S defense", "Zombie D attack", "Zombie C attack", "Zombie B attack", "Zombie A attack", "Zombie S attack", "Zombie D defense", "Zombie C defense", "Zombie B defense", "Zombie A defense", "Zombie S defense" };
            Console.WriteLine("You have:");
            for (int i = 0; i < orbs.Length; i++)
            {
                if (i % 5 == 0)
                {
                    Console.Write("\n");
                }
                if (orbs[i] == 1)
                {
                    ColouredText(orbs[i] + " " + orbList[i] + " &orb\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                }
                else if (orbs[i] > 1)
                {
                    ColouredText(orbs[i] + " " + orbList[i] + " &orbs\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                }
            }
            var bytess = new List<byte>(allData);
            bytess.RemoveRange((int)(startPos + 8), lengthst-4);
            byte[] insert = new byte[64*3 -1];
            for (int i = 0; i < 64; i++)
            {
                insert[i * 3+1] = (byte)(i + 1);
                insert[i * 3] = (byte)orbs[i];
            }
            Console.WriteLine("What orb do you want?");
            int id = Inputed() -1;

            insert[id * 3] = 0x04;

            bytess.InsertRange((int)(startPos + 8), insert);

            stream.Close();

            File.WriteAllBytes(path, bytess.ToArray());

        }

        static void CatFood(string path)
        {
            Console.WriteLine("How much cat food do you want?(max 45000)");
            int CatFood = Inputed();
            if (CatFood > 45000) CatFood = 45000;

            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            Console.WriteLine("Set Cat food to " + CatFood);

            byte[] bytes = Endian(CatFood);

            stream.Position = 7;
            stream.WriteByte(bytes[0]);
            stream.Position = 8;
            stream.WriteByte(bytes[1]);
        }

        static void XP(string path)
        {

            Console.WriteLine("How much XP do you want?(max 99999999)");
            int XP = Inputed();
            if (XP > 99999999) XP = 99999999;

            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            Console.WriteLine("Set XP to " + XP);

            byte[] bytes = Endian(XP);

            stream.Position = 76;
            stream.WriteByte(bytes[0]);
            stream.Position = 77;
            stream.WriteByte(bytes[1]);
            stream.Position = 78;
            stream.WriteByte(bytes[2]);
            stream.Position = 79;
            stream.WriteByte(bytes[3]);

        }

        static void SepecTreasures(string path)
        {
            Console.WriteLine("What level of treasures of you want?(max 255) I would recommend going below 60 though, 1 = inferior, 2 = normal 3 = superior, anything above 3 just aplifies the treasure effect");
            int level = Inputed();
            ColouredText("What chapter do you want to edit:\n&1.&Empire of Cats chapter 1\n&2.&Empire of Cats chapter 2\n&3.&Empire of Cats chapter 3\n&4.&Into the Future chapter 1\n&5.&Into the Future chapter 2\n&6.&Into the Future  chapter 3\n&7.&Cats of the Cosmos chapter 1\n&8.&Cats of the Cosmos chapter 2\n&9.&Cats of the Cosmos  chapter 3\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            int answer = Inputed();
            if (level > 255) level = 255;
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            int j = 0;
            int id = 1;
            for (int i = 2986; i <= 4942; i += 4)
            {
                j++;
                if (j % 49 == 0)
                {
                    id++;
                }
                else if (j % 49 != 0)
                {
                    if (id == answer)
                    {
                        stream.Position = i;
                        stream.WriteByte((byte)level);
                    }
                }

            }
        }
        static void MaxTreasures(string path)
        {
            Console.WriteLine("What level of treasures of you want?(max 255) I would recommend going below 60 though, if you actually want to damage aliens, you should set this to 4 or lower");
            int level = Inputed();
            if (level > 255) level = 255;
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            int j = 0;
            for (int i = 2986; i <= 4942; i += 4)
            {
                j++;
                if (j % 49 != 0)
                {
                    stream.Position = i;
                    stream.WriteByte((byte)level);
                }

            }
            Console.WriteLine("All treasures level {0}", level);
        }

        static void CatUpgrades(string path)
        {
            Console.WriteLine("What base level do you want to upgrade all of your cats to? (max 50) - plus levels will be specified later");
            int baseLev = Inputed() - 1;
            Console.WriteLine("What plus level do you want to upgrade all of your cats to? (max 90)");
            int plusLev = Inputed();
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            if (baseLev > 49) baseLev = 49;
            else if (baseLev < 0) baseLev = 0;

            if (plusLev > 90) plusLev = 90;
            else if (plusLev < 0) plusLev = 0;

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);
            bool repeat = true;

            for (int j = 9600; j <= 12083; j++)
            {
                if (allData[j] == 2 && repeat)
                {
                    Console.WriteLine("all cats max level");

                    repeat = false;
                    for (int i = j + 3; i <= j + (catAmount * 4) - 2; i += 4)
                    {
                        stream.Position = i + 2;
                        stream.WriteByte(Convert.ToByte(baseLev));
                        stream.Position = i;
                        stream.WriteByte(Convert.ToByte(plusLev));
                    }
                }
            }
            Console.WriteLine("Upgraded all cats to level " + (baseLev + 1) + " +" + plusLev);
            stream.Close();
            Bundle(path);
        }
        static void Blue(string path)
        {
            int[] occurrence = OccurrenceB(path);
            Console.WriteLine("Do you want to upgrade all the blue upgrades at once? (yes/no)");
            string answer = Console.ReadLine();
            byte[] bytes = { 0x0A, 0x00, 0x13, 0x00, 0x0A, 0x00, 0x13, 0x00, 0x00, 0x00, 0x09, 0x00,
                0x0A, 0x00, 0x13, 0x00, 0x0A, 0x00, 0x13, 0x00, 0x0A, 0x00, 0x13, 0x00,
                0x0A, 0x00, 0x13, 0x00, 0x0A, 0x00, 0x13, 0x00, 0x0A, 0x00, 0x13, 0x00,
                0x0A, 0x00, 0x13, 0x00, 0x0A, 0x00, 0x13 };
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            if (answer == "no")
            {
                ColouredText("What do you want to upgrade?\n&1.& Power\n&2.& Range\n&3.& Charge\n&4.& Efficiency\n&5.& Wallet\n&6.& Health\n&7.& Research\n&8.& Accounting\n&9.& Study" +
                    "\n&10.& Energy\nInput more than 1 id to edit more than 1 at a time (separated by spaces)\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                string[] input = Console.ReadLine().Split(' ');
                int[] ids = Array.ConvertAll(input, int.Parse);

                Console.WriteLine("What base level do you want? (If you inputed more than 1 id before you must add more than 1 amount e.g if you said 1 4 5 before you need to input 3 numbers now)");
                string[] inputBase = Console.ReadLine().Split(' ');
                int[] idBase = Array.ConvertAll(inputBase, int.Parse);

                Console.WriteLine("What plus level do you want? (If you inputed more than 1 id before you must add more than 1 amount e.g if you said 1 4 5 before you need to input 3 numbers now)");
                string[] inputPlus = Console.ReadLine().Split(' ');
                int[] idPlus = Array.ConvertAll(inputPlus, int.Parse);

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);

                long pos = occurrence[2] + 2440;
                for (int i = 0; i < ids.Length; i++)
                {
                    if (ids[i] > 1) ids[i]++;
                    stream.Position = pos + (ids[i] * 4) + 4;
                    stream.WriteByte((byte)idPlus[i]);
                    stream.Position++;
                    stream.WriteByte((byte)((byte)idBase[i] - 1));
                }

            }
            if (answer == "yes")
            {
                stream.Position = occurrence[2] + 2448;
                stream.Write(bytes, 0, bytes.Length);
            }
            Console.WriteLine("Success");
        }

        static void Leadership(string path)
        {
            Console.WriteLine("How much leadership do you want(max 65535)");
            int CatFood = Inputed();
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            bool found = false;

            Console.WriteLine("Scan Complete");
            byte[] bytes = Endian(CatFood);

            for (int j = 0; j < length - 12; j++)
            {
                if (allData[j] == 0x80 && allData[j + 1] == 0x38)
                {
                    stream.Position = j + 5;
                    stream.Write(bytes, 0, 2);
                    Console.WriteLine("Success");
                    found = true;
                    break;
                }

            }
            if (!found) Console.WriteLine("Sorry your leadership couldn't be found\nYour save file is either invalid or the tool is bugged\nIf this is the case please create a bug report on github or tell me on discord\nThank you");
        }

        static void NP(string path)
        {
            Console.WriteLine("How much NP do you want(max 65535)");
            int CatFood = Inputed();
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            bool found = false;

            Console.WriteLine("Scan Complete");
            byte[] bytes = Endian(CatFood);
            for (int j = 0; j < length - 12; j++)
            {
                if (allData[j] == Convert.ToByte(128) && allData[j + 1] == Convert.ToByte(56) && allData[j + 2] == Convert.ToByte(01) && allData[j + 3] == Convert.ToByte(00))
                {
                    stream.Position = j - 5;
                    stream.WriteByte(bytes[0]);
                    stream.Position = j - 4;
                    stream.WriteByte(bytes[1]);
                    Console.WriteLine("Success");
                    found = true;
                }

            }
            if (!found) Console.WriteLine("Sorry your NP position couldn't be found\nYour save file is either invalid or the tool is bugged\nIf this is the case please create a bug report on github or tell me on discord\nThank you");
        }

        static void CatTicket(string path)
        {
            Console.WriteLine("How many Cat Tickets do you want(max 65535)");
            int catTickets = Inputed();
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            Console.WriteLine("Scan Complete");
            byte[] bytes = Endian(catTickets);

            stream.Close();

            int[] occurrence = OccurrenceB(path);

            using var stream2 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            stream2.Position = occurrence[3] - 8;

            stream2.WriteByte(bytes[0]);
            stream2.WriteByte(bytes[1]);
        }

        static void CatTicketRare(string path)
        {
            Console.WriteLine("How many Rare Cat Tickets do you want(max 65535)");
            int catTickets = Inputed();
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            Console.WriteLine("Scan Complete");
            byte[] bytes = Endian(catTickets);
            stream.Close();

            int[] occurrence = OccurrenceB(path);

            using var stream2 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            stream2.Position = occurrence[3] - 4;
            stream2.WriteByte(bytes[0]);
            stream2.WriteByte(bytes[1]);
        }
        static void GamXP(string path)
        {
            Console.WriteLine("How much gamatoto xp do you want?\nLevel bounderies: https://battle-cats.fandom.com/wiki/Gamatoto_Expedition#Level-up_Requirements");

            int amount = Inputed();
            byte[] bytes = Endian(amount);
            int pos = ThirtySix(path).Item1;
            bool c8 = ThirtySix(path).Item2;
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            bool found = false;
            if (pos > 0)
            {
                found = true;
            }
            if (c8)
            {
                stream.Position = pos - 2367;

            }
            else
            {
                stream.Position = pos - 3087;
            }
            stream.WriteByte(bytes[0]);
            stream.WriteByte(bytes[1]);
            stream.WriteByte(bytes[2]);
            stream.WriteByte(bytes[3]);


            if (found)
            {
                Console.WriteLine("Success");
            }
            if (!found) Console.WriteLine("Sorry your gamatoto xp position couldn't be found\nYour save file is either invalid or the tool is bugged\nIf this is the case please create a bug report on github or tell me on discord\nThank you");

        }
        static Tuple<int, bool, int> ThirtySix(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);
            int pos = 0;
            int[] pos2 = new int[10];
            bool c8 = true;
            int count = 0;
            for (int i = 0; i < allData.Length; i++)
            {
                if (allData[i] == 0x2c && allData[i - 1] == 0 && allData[i + 1] == 1 && allData[i + 2] == 0 && allData[i + 3] == 0 && allData[i + 4] == 0)
                {
                    count++;
                    for (int j = i - 700; j < i; j++)
                    {
                        if (allData[j] == 0xff && allData[j + 1] == 0xff && allData[j + 2] == 0xff && allData[j + 3] == 0xff && allData[j + 4] == 0xff)
                        {
                            pos = i;
                            c8 = false;
                        }
                    }
                }
            }
            if (c8)
            {
                for (int i = 0; i < allData.Length; i++)
                {
                    if (allData[i] == 0xc8 && allData[i - 1] == 0 && allData[i + 1] == 0 && allData[i + 2] == 0 && allData[i + 3] == 0 && allData[i + 4] == 0)
                    {
                        for (int j = i - 700; j < i; j++)
                        {
                            if (allData[j] == 0xff && allData[j + 1] == 0xff && allData[j + 2] == 0xff && allData[j + 3] == 0xff && allData[j + 4] == 0xff)
                            {
                                pos = i;
                            }
                        }
                    }
                }

            }
            for (int i = pos - 700; i < pos; i++)
            {
                if (allData[i] == 0x36)
                {
                    return Tuple.Create(i, c8, pos);
                }
            }
            return Tuple.Create(0, c8, pos);
        }
        static void GamHelp(string path)
        {
            ColouredText("What helpers do you want?&\n&Type numbers separated by spaces\nThe different helper ids are as follows:&\nIntern &1 - 53&\nLacky &54 - 83&\nUnderling &84 - 108&\nAssistant &109 - 128&\nLegend &129 - 148&\ne.g entering " +
                "&3 69 120 86 110 &would set your helpers to &1& intern, &1& lackey, &2& assistants, &1& underling\nThe ids must be different to eachother, the max helpers you can have is &10\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            string[] answer = Console.ReadLine().Split(' ');
            int[] answerInt = new int[answer.Length];
            try
            {
                answerInt = Array.ConvertAll(answer, s => int.Parse(s));
            }
            catch (Exception e)
            {
                ColouredText(e.Message + "\n", ConsoleColor.White, ConsoleColor.Red);
                GamHelp(path);
            }
            for (int i = 0; i < answerInt.Length; i++)
            {
                if (answerInt[i] < 1)
                {
                    ColouredText("Error: you can't have an id below 1\n", ConsoleColor.White, ConsoleColor.Red);
                    GamHelp(path);
                }
                if (answerInt[i] > 148)
                {
                    ColouredText("Error: you can't have an id above 148\n", ConsoleColor.White, ConsoleColor.Red);
                    GamHelp(path);
                }
            }
            byte[] bytes = answerInt.SelectMany(BitConverter.GetBytes).ToArray();

            int pos = ThirtySix(path).Item1;
            bool found = false;
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            if (pos > 0)
            {
                found = true;
            }
            stream.Position = pos - 1025;
            stream.Write(bytes, 0, bytes.Length);
            if (found)
            {
                Console.WriteLine("Success");
                int count = 0;
                if (answerInt.Length < 5)
                {
                    count = 5;
                }
                else
                {
                    count = answerInt.Length;
                }
                int[] helpNums = new int[count];
                for (int i = 0; i < answerInt.Length; i++)
                {
                    if (answerInt[i] <= 53) helpNums[0]++;
                    else if (answerInt[i] <= 83) helpNums[1]++;
                    else if (answerInt[i] <= 108) helpNums[2]++;
                    else if (answerInt[i] <= 128) helpNums[3]++;
                    else helpNums[4]++;
                }
                Console.WriteLine("\nSet helpers to:\n {0} intern(s)\n {1} lackey(s)\n {2} underling(s)\n {3} assistant(s)\n {4} legend(s)", helpNums[0], helpNums[1], helpNums[2], helpNums[3], helpNums[4]);
            }
            if (!found) Console.WriteLine("Sorry your gamatoto helper position couldn't be found\nYour save file is either invalid or the tool is bugged\nIf this is the case please create a bug report on github or tell me on discord\nThank you");
        }
        static void FixGam(string path)
        {
            int pos = ThirtySix(path).Item1;
            bool c8 = ThirtySix(path).Item2;
            int pos2 = ThirtySix(path).Item3;
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);
            Console.WriteLine("Scan Complete");
            var bytess = new List<byte>(allData);
            bytess.RemoveRange(pos, pos2 - (pos - 2));

            stream.Position = pos;
            byte[] bytes = {
                        0x36, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x36, 0x00, 0x00, 0x00,
                        0x07, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00,
                        0xEC, 0x13, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x9E, 0xC7, 0x00, 0x00,
                        0x02, 0x00, 0x00, 0x00, 0xCE, 0xC7, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
                        0xCF, 0xC7, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0xD4, 0xC7, 0x00, 0x00,
                        0x03, 0x00, 0x00, 0x00, 0xC8, 0x00
                    };
            if (!c8)
            {
                bytes[bytes.Length - 2] = 0x2C;
                bytes[bytes.Length - 1] = 0x01;

            }
            bytess.InsertRange(pos, bytes);
            allData = bytess.ToArray();
            stream.Close();
            File.WriteAllBytes(path, allData);
            bool found = true;
            if (found) Console.WriteLine("Success");
            if (!found) Console.WriteLine("Sorry your gamatoto position couldn't be found\nYour save file is either invalid or the tool is bugged\nIf this is the case please create a bug report on github or tell me on discord\nThank you");

        }

        static void PlatTicketRare(string path)
        {
            Console.WriteLine("How many Platinum Cat Tickets do you want(max 9 - you'll get banned if you get more)");
            byte platCatTickets = Convert.ToByte(Console.ReadLine());
            if (platCatTickets > 9) platCatTickets = 9;
            int pos = ThirtySix(path).Item1;

            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            bool found = false;
            if (pos > 0)
            {
                found = true;
            }
            stream.Position = pos + 16;
            stream.WriteByte(platCatTickets);
            if (found) Console.WriteLine("Success");
            if (!found) Console.WriteLine("Sorry your platinum cat ticket position couldn't be found\nYour save file is either invalid or the tool is bugged\nIf this is the case please create a bug report on github or tell me on discord\nThank you");

        }

        static void Seed(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            Console.WriteLine("Scan Complete");

            Console.WriteLine("What seed do you want?(max 4294967295)");
            long XP = Inputed();
            if (XP > 4294967295) XP = 4294967295;
            byte[] bytes = Endian(XP);

            byte[] year = new byte[2];
            year[0] = allData[15];
            year[1] = allData[16];

            stream.Close();

            int[] occurrence = OccurrenceE(path, year);

            using var stream2 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            try
            {
                stream2.Position = occurrence[4] - 21;
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Sorry your seed position couldn't be found\nYour save file is either invalid or the tool is bugged\nIf this is the case please create a bug report on github or tell me on discord\nThank you");
                Options();
            }

            Console.WriteLine("Set gacha seed to: {0}", XP);
            for (int i = 0; i < 5; i++)
                stream2.WriteByte(bytes[i]);
        }
        static void GetSeed(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            Console.WriteLine("Scan Complete");

            byte[] year = new byte[2];
            year[0] = allData[15];
            year[1] = allData[16];

            stream.Close();

            int[] occurrence = OccurrenceE(path, year);

            using var stream2 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            try
            {
                stream2.Position = occurrence[4] - 21;
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Sorry your seed position couldn't be found\nYour save file is either invalid or the tool is bugged\nIf this is the case please create a bug report on github or tell me on discord\nThank you");
                Options();
            }
            byte[] seed = new byte[100];
            int j = 0;
            for (int i = occurrence[4] - 21; i < occurrence[4] - 16; i++)
            {
                seed[j] = allData[i];
                j++;
            }
            seed = Endian(BitConverter.ToInt64(seed, 0));
            Console.WriteLine("Seed is:" + BitConverter.ToInt64(seed, 0));
        }

        static void Evolve(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            Console.WriteLine("Scan Complete");

            stream.Close();
            int[] occurrence = OccurrenceB(path);

            using var stream2 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            try
            {
                stream2.Position = occurrence[4] + 40;
            }
            catch { Console.WriteLine("You either haven't unlocked the ability to evolve cats or if you have - the tool is bugged and you should tell me on the discord"); return; }
            int[] form = EvolvedFormsGetter();
            bool stop = false;
            int t = 0;
            int pos = (int)stream2.Position;
            while (stream2.Position < pos + (catAmount * 4) - 37 && !stop)
            {
                for (int i = 0; i < 24; i++)
                {
                    if (allData[stream2.Position + i] != 0x01 && allData[stream2.Position + i] != 0 && allData[stream2.Position + i] != 0x02 && allData[stream2.Position + i] != 0x03)
                    {
                        stop = true;
                        break;
                    }
                }
                stream2.WriteByte((byte)form[t]);
                stream2.Position += 3;
                t++;
            }
        }
        static void Items(string path)
        {
            Console.WriteLine("Before using this feature make sure that this is a recent save otherwise your data could get corrupted!\nHow many of each item do you want?(max 65535)");
            int CatFood = Inputed();
            _ = new byte[10];
            byte[] year = new byte[2];
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);


            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            byte[] bytes = Endian(CatFood);
            int[] occurrence = new int[100];

            try
            {
                year[0] = allData[15];
                year[1] = allData[16];

                if (year[0] != 0x07)
                {
                    year[0] = allData[19];
                    year[1] = allData[20];
                }
                stream.Close();
                occurrence = OccurrenceE(path, year);
            }
            catch
            {
                stream.Close();
            }
            using var stream2 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            try
            {
                stream2.Position = occurrence[2] - 224;
                for (int i = occurrence[2] - 224; i < occurrence[2] - 203; i += 4)
                {
                    stream2.Position = i;
                    stream2.WriteByte(bytes[0]);
                    stream2.WriteByte(bytes[1]);
                }
            }
            catch
            {
                ColouredText("You either haven't unlocked battle items yet, or the editor is bugged - if that's true please contact me on discord/the discord server in #tool-help so I can fix it", ConsoleColor.Red, ConsoleColor.White);
            }

            stream2.Close();
        }

        static void Catamin(string path)
        {
            Console.WriteLine("How many Catimins of each type do you want(max 65535)");
            int platCatTickets = Inputed();

            byte[] bytes = Endian(platCatTickets);


            int pos = ThirtySix(path).Item1;
            bool c8 = ThirtySix(path).Item2;
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            bool found = false;
            if (pos > 0)
            {
                found = true;
            }
            if (c8)
            {
                stream.Position = pos - 2388;

            }
            else
            {
                stream.Position = pos - 3108;
            }
            for (int i = 0; i < 3; i++)
            {
                stream.Write(bytes, 0, 2);
                stream.Position += 2;
            }


            if (found)
            {
                Console.WriteLine("Success");
            }
            if (!found) Console.WriteLine("Sorry your Catamin position couldn't be found\nYour save file is either invalid or the tool is bugged\nIf this is the case please create a bug report on github or tell me on discord\nThank you");
        }

        static void BaseMats(string path)
        {
            Console.WriteLine("How many Base Materials do you want(max 65535)");
            int platCatTickets = (int)Convert.ToInt64(Console.ReadLine());
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] bytes = Endian(platCatTickets);
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);
            bool found = false;

            Console.WriteLine("Scan Complete");
            for (int j = 0; j < allData.Length; j++)
            {
                if (allData[j] == Convert.ToByte(01) && allData[j + 1] == Convert.ToByte("3F", 16) && allData[j + 2] == 0 && allData[j + 3] == 0 && allData[j + 4] == 0 && allData[j + 5] == 8)
                {
                    found = true;
                    for (int i = 0; i < 29; i += 4)
                    {
                        stream.Position = j + 9 + i;
                        stream.WriteByte(bytes[0]);
                        stream.WriteByte(bytes[1]);
                    }
                }
            }
            if (!found) Console.WriteLine("Sorry your base mats position couldn't be found\nYour save file is either invalid or the tool is bugged\nIf this is the case please create a bug report on github or tell me on discord\nThank you");
        }

        static void Catseyes(string path)
        {
            Console.WriteLine("How many catseyes of each type do you want(max 65535)");
            int platCatTickets = Inputed();

            byte[] bytes = Endian(platCatTickets);


            int pos = ThirtySix(path).Item1;
            bool c8 = ThirtySix(path).Item2;
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            bool found = false;
            if (pos > 0)
            {
                found = true;
            }
            if (c8)
            {
                stream.Position = pos - 2412;

            }
            else
            {
                stream.Position = pos - 3132;
            }
            for (int i = 0; i < 5; i++)
            {
                stream.Write(bytes, 0, 2);
                stream.Position += 2;
            }


            if (found)
            {
                Console.WriteLine("Success");
            }
            if (!found) Console.WriteLine("Sorry your catseye position couldn't be found\nYour save file is either invalid or the tool is bugged\nIf this is the case please create a bug report on github or tell me on discord\nThank you");

        }
        static void RemCats(string path)
        {
            int[] occurrence = OccurrenceB(path);
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            for (int i = occurrence[0] + 4; i <= occurrence[1] - 12; i += 4)
            {
                stream.Position = i;
                stream.WriteByte(Convert.ToByte(0));
            }
            Console.WriteLine("Removed all cats");
        }

        static void Cats(string path)
        {
            Console.WriteLine("Do you want to add cats? (yes/no)");
            string answer = Console.ReadLine();
            if (answer == "no")
            {
                RemCats(path);
                return;
            }
            else if (answer != "yes")
            {
                Console.WriteLine("Answer was not yes/no");
                return;
            }
            int[] occurrence = OccurrenceB(path);
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            for (int i = occurrence[0] + 4; i <= occurrence[1] - 12; i += 4)
            {
                stream.Position = i;
                stream.WriteByte(Convert.ToByte(01));
            }
            Console.WriteLine("Gave all cats");

        }

        static void SpecifiCat(string path)
        {
            Console.WriteLine("Do you want to add cats? (yes/no)");
            string answer = Console.ReadLine();
            if (answer == "no")
            {
                RemSpecifiCat(path);
                return;
            }
            else if (answer != "yes")
            {
                Console.WriteLine("Answer was not yes/no");
                return;
            }
            int[] occurrence = OccurrenceB(path);
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            Console.WriteLine("What is the cat ID?, input multiple ids separated by spaces to add multiple cats at a time");
            string input = Console.ReadLine();
            string[] catIds = input.Split(' ');
            for (int i = 0; i < catIds.Length; i++)
            {
                int catID = int.Parse(catIds[i]);
                int startPos = occurrence[0] + 4;
                stream.Position = startPos + catID * 4;
                stream.WriteByte(01);
                ColouredText("&Gave cat: &" + catID + "\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            }

        }
        static void RemSpecifiCat(string path)
        {
            int[] occurrence = OccurrenceB(path);
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            Console.WriteLine("What is the cat ID?, input multiple ids separated by spaces to add multiple cats at a time");
            string input = Console.ReadLine();
            string[] catIds = input.Split(' ');
            for (int i = 0; i < catIds.Length; i++)
            {
                int catID = int.Parse(catIds[i]);
                int startPos = occurrence[0] + 4;
                stream.Position = startPos + catID * 4;
                stream.WriteByte(0);
                ColouredText("&Removed cat: &" + catID + "\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            }
        }

        static void SpecifUpgrade(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            ColouredText("What is the cat id? (you can input more than 1 to upgrade more than 1 e.g 15 200 78 will select those cats)\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            string[] ids = Console.ReadLine().Split(' ');
            int[] idInt = Array.ConvertAll(ids, int.Parse);

            ColouredText("What base level do you want? (max 50) - (If you inputed more than 1 id before, then input the base upgrades that amount of times e.g if you inputed id 12 56, then you need to input 2 numbers)\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            string[] baselevel = Console.ReadLine().Split(' ');
            int[] baseID = Array.ConvertAll(baselevel, int.Parse);

            ColouredText("What plus level do you want? (max +90) - (If you inputed more than 1 id before, then input the plus levels that amount of times e.g if you inputed id 12 56, then you need to input 2 numbers)\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            string[] plusLevel = Console.ReadLine().Split(' ');
            int[] plusID = Array.ConvertAll(plusLevel, int.Parse);

            if (plusID.Length < ids.Length || baselevel.Length < ids.Length)
            {
                ColouredText("Error: not enough inputs were given", ConsoleColor.White, ConsoleColor.Red);
                SpecifUpgrade(path);
            }
            else if (plusID.Length > ids.Length || baselevel.Length > ids.Length)
            {
                ColouredText("Error: too many inputs were given", ConsoleColor.White, ConsoleColor.Red);
                SpecifUpgrade(path);
            }
            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);
            bool repeat = true;

            for (int j = 9600; j <= 12000; j++)
            {
                if (allData[j] == 2 && repeat)
                {
                    for (int i = 0; i < idInt.Length; i++)
                    {
                        stream.Position = j + (idInt[i] * 4) + 3;
                        stream.WriteByte((byte)plusID[i]);
                        stream.Position++;
                        stream.WriteByte((byte)((byte)baseID[i] - 1));
                    }
                    break;
                }
            }
            for (int i = 0; i < ids.Length; i++)
            {
                ColouredText("Upgraded cat &" + ids[i] + "& to level &" + baseID[i] + "& +&" + plusID[i] + "\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            }
        }

        static bool OnAskUser(string title, string title2)
        {
            return DialogResult.Yes == MessageBox.Show(
             title, title2,
             MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        static byte[] Endian(long num)
        {
            byte[] bytes = BitConverter.GetBytes(num);

            return bytes;
        }

        static string Encrypt(string choice, string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            byte[] toBeUsed = new byte[allData.Length - 32];
            for (int i = 0; i < allData.Length - 32; i++)
                toBeUsed[i] = allData[i];
            byte[] bytes = Encoding.ASCII.GetBytes("battlecats");
            if (choice == "en")
            {
                bytes = Encoding.ASCII.GetBytes("battlecatsen");
            }
            else if (choice == "jp")
            {
                bytes = Encoding.ASCII.GetBytes("battlecats");
            }
            else
            {
                Console.WriteLine("Answer was not jp or en");
                stream.Close();
                Encrypt(choice, path);
                return "";
            }
            int test = 32 - bytes.Length;

            byte[] Usable = new byte[allData.Length - test];
            bytes.CopyTo(Usable, 0);
            toBeUsed.CopyTo(Usable, bytes.Length);


            var md5 = MD5.Create();

            byte[] Data = new byte[16];
            Data = md5.ComputeHash(Usable);

            string hex = ByteArrayToString(Data);
            Console.WriteLine("Data patched");

            string EncyptedHex = ByteArrayToString(Data);

            hex = hex.ToLower();

            byte[] stuffs = Encoding.ASCII.GetBytes(hex);

            stream.Position = allData.Length - 32;
            stream.Write(stuffs, 0, stuffs.Length);
            return choice;
        }

        static string ByteArrayToString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }

        static void ColouredText(string input, ConsoleColor Base, ConsoleColor New)
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
                    Console.Write(split[i + 1]);

                }
                Console.ForegroundColor = Base;
            }
            catch (IndexOutOfRangeException) { }
        }

        static int Inputed()
        {
            int input = 0;
            try { input = (int)Convert.ToInt64(Console.ReadLine()); }
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

        static int[] OccurrenceB(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            int amount = 0;
            int[] occurrence = new int[50];
            stream.Close();
            byte anchour = Anchour(path);

            for (int i = 0; i < allData.Length - 1; i++)
            {
                if (allData[i] == anchour)
                    if (allData[i + 1] == 2 && allData[i + 2] == 0 && allData[i - 1] == 0)
                    {
                        occurrence[amount] = i;
                        amount++;
                    }
            }
            stream.Close();

            return occurrence;
        }

        static int[] OccurrenceE(string path, byte[] Currentyear)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            int amount = 0;
            int[] occurrence = new int[50];

            for (int i = 0; i < allData.Length - 1; i++)
            {
                if (allData[i] == Convert.ToByte(Currentyear[0]) && allData[i + 1] == Convert.ToByte(Currentyear[1]))
                {
                    occurrence[amount] = i;
                    amount++;
                }

            }

            return occurrence;
        }

        static void EvolveSpecific(string path)
        {
            Console.WriteLine("Do you want to evolve(1) or de-evolve(2)?:");
            string devolve = Console.ReadLine();
            if (devolve != "1" && devolve != "2")
            {
                Console.WriteLine("Answer must be 1 or 2!");
                EvolveSpecific(path);
            }
            int[] occurrence = OccurrenceB(path);

            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            Console.WriteLine("What is the cat id?, input multiple ids separated by spaces to evolve multiple cats ids must be 9 or above, normal cats cannot be evolved this way");
            string[] input = Console.ReadLine().Split(' ');

            try
            {
                stream.Position = occurrence[4] + 40;
            }
            catch { Console.WriteLine("You either haven't unlocked the ability to evolve cats or if you have - the tool is bugged and you should tell me on the discord"); return; }
            int[] form = EvolvedFormsGetter();
            int pos = (int)stream.Position;
            try {
                for (int i = 0; i < input.Length; i++)
                {
                    stream.Position = pos + (int.Parse(input[i]) - 9) * 4;
                    if (devolve == "1")
                    {
                        stream.WriteByte((byte)form[int.Parse(input[i]) - 9]);
                    }
                    else
                    {
                        stream.WriteByte(0);
                    }
                    stream.Position--;
                }
            }
            catch
            {
                Console.WriteLine("You need to input an id that isn't a normal cat!");
            }



        }
        static void NewIQ(string path)
        {

            using var stream2 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            int length = (int)stream2.Length;
            byte[] allData = new byte[length];
            stream2.Read(allData, 0, length);

            Console.WriteLine("What inquiry code do you want - this code must be set to an account code that actually lets you play without the save is used elsewhere bug");
            string iq = Console.ReadLine();
            byte[] bytes = Encoding.ASCII.GetBytes(iq);
            bool found = false;

            for (int i = 0; i < allData.Length; i++)
            {
                if (allData[i] == 0x2D && allData[i + 1] == 0x0 && allData[i + 2] == 0x0 && allData[i + 3] == 0x0 && allData[i + 4] == 0x2E)
                {
                    for (int j = 1900; j < 2108; j++)
                    {
                        if (allData[i - j] == 09)
                        {
                            stream2.Position = i - j + 4;
                            stream2.Write(bytes, 0, bytes.Length);
                            found = true;
                        }
                    }
                }
            }
            if (!found)
            {
                Console.WriteLine("Sorry your inquiry code position couldn't be found\nEither your save is invalid or the edtior is bugged, if it is please contact me on the discord linked in the readme.md");
                return;
            }
            Console.WriteLine("Success\nYour new account code is now: " + iq + " This should remove that \"save is being used elsewhere\" bug and if your account is banned, this should get you unbanned");
        }

        static void CatFruit(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);
            stream.Close();

            int[] occurrence = OccurrenceB(path);

            using var stream2 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            try
            {
                stream2.Position = occurrence[6] - 60;
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("You either can't evolve cats or the tool is bugged and if the tool is bugged then:\nYour save file is either invalid or the tool is bugged\nIf this is the case please create a bug report on github or tell me on discord\nThank you");
                Options();
            }
            byte[] catfruit = new byte[4];
            int[] FruitCat = new int[15];
            string[] fruits = { "Purple Seed", "Red Seed", "Blue Seed", "Green Seed", "Yellow Seed", "Purple Fruit", "Red Fruit", "Blue Fruit", "Green Fruit", "Yellow Fruit", "Epic Fruit", "Elder Seed", "Elder Fruit", "Epic Seed", "Gold Fruit" };

            int j = 0;
            for (int i = occurrence[6] - 60; i < occurrence[6] - 3; i += 4)
            {
                catfruit[0] = allData[i];
                catfruit[1] = allData[i + 1];
                FruitCat[j] = BitConverter.ToInt32(catfruit, 0);
                j++;
            }
            Console.WriteLine("Total catfruit/seeds: " + FruitCat.Sum());
            Console.WriteLine("Do you want to edit all a cat fruit/seed individually(1) or all at once? (2), (1 or 2)");
            string input = Console.ReadLine();

            if (input == "2")
            {
                Console.WriteLine("How many do you want?(max 256)");
                int num = Inputed();
                if (num > 256) num = 256;
                else if (num < 0) num = 0;

                byte[] bytes2 = Endian(num);

                for (int i = 1; i < fruits.Length + 1; i++)
                {
                    int choice2 = i;
                    stream2.Position = occurrence[6] - 60 + ((choice2 - 1) * 4);
                    stream2.WriteByte(bytes2[0]);
                    stream2.WriteByte(bytes2[1]);
                    ColouredText("&Set &" + fruits[choice2 - 1] + "& to &" + num + "\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                }

            }
            else if (input == "1")
            {
                for (int i = 0; i < fruits.Length; i++)
                {
                    Console.WriteLine(i + 1 + ". " + fruits[i] + ": " + FruitCat[i]);
                }

                int choice = Inputed();
                if (choice > 15) choice = 15;
                else if (choice < 1) choice = 1;

                Console.WriteLine("How many " + fruits[choice - 1] + "s do you want (max 256)");
                int amount = Inputed();
                if (amount > 256) amount = 256;
                else if (amount < 0) amount = 0;

                byte[] bytes = Endian(amount);

                stream2.Position = occurrence[6] - 60 + ((choice - 1) * 4);
                stream2.WriteByte(bytes[0]);
                stream2.WriteByte(bytes[1]);
            }

            Console.WriteLine("Have you finished editing cat fruits?(yes/no)");
            string answer = Console.ReadLine();
            stream2.Close();
            if (answer.ToLower() == "no") CatFruit(path);
        }

        static int[] Occurrence1(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            int amount = 0;
            int[] occurrence = new int[50];

            for (int i = 0; i < allData.Length - 1; i++)
            {
                if (allData[i] == Convert.ToByte(0) && allData[i + 1] == Convert.ToByte(0) && allData[i + 2] == Convert.ToByte(0) && allData[i + 3] == Convert.ToByte(0) && allData[i + 4] == Convert.ToByte(0) && allData[i + 5] == Convert.ToByte(1) && allData[i + 6] == Convert.ToByte("4d", 16) && allData[i + 7] == Convert.ToByte(0) && allData[i + 8] == Convert.ToByte(0) && allData[i + 9] == Convert.ToByte(0))
                {
                    if (allData[i - 1] == 0 && allData[i + 2] == 0)
                    {
                        occurrence[amount] = i;
                        amount++;
                    }
                }
            }

            return occurrence;
        }

        static void Talents(string path)
        {
            Console.WriteLine("What level of talents do you want to upgrade everything to?(max 65535)");
            int level = Inputed();
            if (level > 65535) level = 65535;

            byte[] talents = Endian(level);
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            stream.Close();

            int[] occurrence = Occurrence1(path);

            using var stream2 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            try
            {
                stream2.Position = occurrence[0] + 220;
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("You either havn't unlocked NP or the tool is bugged and if the tool is bugged then:\nYour save file is either invalid or the tool is bugged\nIf this is the case please create a bug report on github or tell me on discord\nThank you");
                Options();
            }
            for (int i = (int)stream2.Position; i < occurrence[0] + 5284; i += 8)
            {
                if (allData[i - 12] == Convert.ToByte(255) && allData[i - 11] == Convert.ToByte(255))
                {
                    i = occurrence[0] + 5300;
                    break;
                }
                stream2.Position = i;
                if (allData[stream2.Position] != 5)
                {
                    stream2.WriteByte(talents[0]);
                    stream2.WriteByte(talents[1]);
                }
            }
            Console.WriteLine("Set all talents to level: {0}", level);
            stream2.Close();
        }

        static byte Anchour(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);
            byte anchour = 0;

            for (int i = 7344; i < 7375; i++)
            {
                if (allData[i] == 2)
                {
                    anchour = allData[i - 1];
                }
            }
            stream.Close();
            return anchour;
        }

        static void Stage(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            ColouredText("What chapters do you want to complete?(1-9)\n1.&Empire of cats chapter 1&\n2.&Empire of cats chapter 2&\n3.&Empire of cats chapter 3&\n4.&Into the future chapter 1&\n5.&Into the future chapter 2&\n6.&Into the future chapter 3&\n7.&Cats of the cosmos chapter 1&" +
                "\n8.&Cats of the cosmos chapter 2&\n9.&Cats of the cosmos chapter 3&\n10.&All chapters&\n", ConsoleColor.White, ConsoleColor.Cyan);
            int choice = Inputed();

            if (choice == 10)
            {
                for (int i = 946; i <= 1134; i += 4)
                {
                    stream.Position = i;
                    stream.WriteByte(01);
                }
                for (int i = 1150; i <= 1338; i += 4)
                {
                    stream.Position = i;
                    stream.WriteByte(01);
                }
                for (int i = 1354; i <= 1542; i += 4)
                {
                    stream.Position = i;
                    stream.WriteByte(01);
                }
                for (int i = 1762; i <= 1950; i += 4)
                {
                    stream.Position = i;
                    stream.WriteByte(01);
                }
                for (int i = 1966; i <= 2154; i += 4)
                {
                    stream.Position = i;
                    stream.WriteByte(01);
                }
                for (int i = 2170; i <= 2358; i += 4)
                {
                    stream.Position = i;
                    stream.WriteByte(01);
                }
                for (int i = 2374; i <= 2562; i += 4)
                {
                    stream.Position = i;
                    stream.WriteByte(01);
                }
                for (int i = 2578; i <= 2766; i += 4)
                {
                    stream.Position = i;
                    stream.WriteByte(01);
                }
                for (int i = 2782; i <= 2970; i += 4)
                {
                    stream.Position = i;
                    stream.WriteByte(01);
                }
                stream.Position = 906;
                stream.WriteByte(0x30);
                stream.Position = 910;
                stream.WriteByte(0x30);
                stream.Position = 914;
                stream.WriteByte(0x30);
                stream.Position = 922;
                stream.WriteByte(0x30);
                stream.Position = 926;
                stream.WriteByte(0x30);
                stream.Position = 930;
                stream.WriteByte(0x30);
                stream.Position = 934;
                stream.WriteByte(0x30);
                stream.Position = 938;
                stream.WriteByte(0x30);
                stream.Position = 942;
                stream.WriteByte(0x30);
            }
            switch (choice)
            {
                case 1:
                    for (int i = 946; i <= 1134; i += 4)
                    {
                        stream.Position = i;
                        stream.WriteByte(01);
                    }
                    stream.Position = 906;
                    stream.WriteByte(0x30);
                    break;
                case 2:
                    for (int i = 1150; i <= 1338; i += 4)
                    {
                        stream.Position = i;
                        stream.WriteByte(01);
                    }
                    stream.Position = 910;
                    stream.WriteByte(0x30);
                    break;
                case 3:
                    for (int i = 1354; i <= 1542; i += 4)
                    {
                        stream.Position = i;
                        stream.WriteByte(01);
                    }
                    stream.Position = 914;
                    stream.WriteByte(0x30);
                    break;
                case 4:
                    for (int i = 1762; i <= 1950; i += 4)
                    {
                        stream.Position = i;
                        stream.WriteByte(01);
                    }
                    stream.Position = 922;
                    stream.WriteByte(0x30);
                    break;
                case 5:
                    for (int i = 1966; i <= 2154; i += 4)
                    {
                        stream.Position = i;
                        stream.WriteByte(01);
                    }
                    stream.Position = 926;
                    stream.WriteByte(0x30);
                    break;
                case 6:
                    for (int i = 2170; i <= 2358; i += 4)
                    {
                        stream.Position = i;
                        stream.WriteByte(01);
                    }
                    stream.Position = 930;
                    stream.WriteByte(0x30);
                    break;
                case 7:
                    for (int i = 2374; i <= 2562; i += 4)
                    {
                        stream.Position = i;
                        stream.WriteByte(01);
                    }
                    stream.Position = 934;
                    stream.WriteByte(0x30);
                    break;
                case 8:
                    for (int i = 2578; i <= 2766; i += 4)
                    {
                        stream.Position = i;
                        stream.WriteByte(01);
                    }
                    stream.Position = 938;
                    stream.WriteByte(0x30);
                    break;
                case 9:
                    for (int i = 2782; i <= 2970; i += 4)
                    {
                        stream.Position = i;
                        stream.WriteByte(01);
                    }
                    stream.Position = 942;
                    stream.WriteByte(0x30);
                    break;
                case 10:
                    break;
                default:
                    Console.WriteLine("Please enter a recognised number");
                    stream.Close();
                    Stage(path);
                    break;
            }

        }

        static void Bundle(string path)
        {
            int[] occurrence = OccurrenceB(path);
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            stream.Position = occurrence[5] - 16;
            stream.WriteByte(0xff);
            stream.WriteByte(0xff);
            Console.WriteLine("Closed all bundle menus");

        }

        static void TimedScore(string path)
        {
            Console.WriteLine("What timed score do you want? (max 9999)");
            int score = Inputed();
            if (score > 9999) score = 9999;
            byte[] scoreByte = Endian(score);
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            Console.WriteLine("Scan Complete");

            stream.Close();

            int[] occurance = OccurrenceB(path);

            using var stream2 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);


            stream2.Position = occurance[5];
            if (stream2.Position == 0)
            {
                Console.WriteLine("Sorry your timed score position couldn't be found please contact me on discord(linked in the readme)");
                return;
            }
            if (allData[occurance[5] - 2095] == 1) stream2.Position = occurance[5] - 2096;
            if (allData[occurance[5] - 2127] == 1) stream2.Position = occurance[5] - 2128;

            stream2.Position -= 616;
            for (int i = 0; i < 48; i++)
            {
                stream2.WriteByte(scoreByte[0]);
                stream2.WriteByte(scoreByte[1]);
                stream2.Position += 2;
            }
            stream2.Position += 12;
            for (int i = 0; i < 48; i++)
            {
                stream2.WriteByte(scoreByte[0]);
                stream2.WriteByte(scoreByte[1]);
                stream2.Position += 2;
            }
            stream2.Position += 12;
            for (int i = 0; i < 48; i++)
            {
                stream2.WriteByte(scoreByte[0]);
                stream2.WriteByte(scoreByte[1]);
                stream2.Position += 2;
            }
            Console.WriteLine("Set ItF timed score rewards to: " + score);

        }

        static int[] EvolvedFormsGetter()
        {
            WebClient client = new WebClient();
            client.DownloadFile("https://raw.githubusercontent.com/fieryhenry/Battle-Cats-Save-File-Editor/main/cats.csv", @"cats.csv");
            using var reader = new StreamReader(@"cats.csv");
            List<string> listA = new List<string>();
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] values = line.Split('?');

                listA.Add(values[0]);
            }
            string[] first = new string[700];
            string[] second = new string[700];
            int[] form = new int[700];
            string[] f = new string[3];
            for (int i = 0; i < 700; i++)
            {
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
            for (int i = 0; i < 600; i++)
            {
                if (form[i] == 0) form[i] = 2;
                if (form[i] == 1) form[i] = 0;
                if (listA[i].Contains("EX,rarity,") && form[i] == 2) form[i] = 1;
                if (listA[i].Contains("Cat God the Awesome") || listA[i].Contains("Ururun Cat ") || listA[i].Contains("Ururun Cat ") || listA[i].Contains("Dark Emperor Catdam") || listA[i].Contains("Crimson Mina") || listA[i].Contains("Heroic Musashi") || listA[i].Contains("Mecha-Bun Mk II")) form[i] = 2;
            }
            return form;
        }
        static byte[] Levels()
        {
            byte[] levels = { 8, 8, 8, 8, 6, 8, 8, 8, 8, 6, 8, 8, 8, 6, 6, 8, 8, 6, 6, 8, 6, 6, 6, 6, 8, 8, 8, 6, 5, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 8, 1 };
            return levels;
        }
        static void SoL(string path)
        {
            ColouredText("&What do you want to do?\nExample:\nEnter a single chapter &id& (e.g &1& = &legend begins&,& 2& = &passion land&) to complete just that chapter\nEnter a single chapter &id& + &*& + &number of stars& (e.g &1*4& = &legend begins 4 stars&)\n" +
                "Enter a &-& between &ids& to do a range of chapters (e.g &1 - 4& = &legend begins& to &swimming cats&)\nEnter a &-& between &ids& and a &number of stars& (e.g &1*4& - &4& = &legend begins 4 star& to &swimming cats 4 star&) - This will complete the chapters in between and make them all 4 star\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            string answer = Console.ReadLine();
            string[] split = answer.Split(' ');
            bool isStar = false;
            if (answer.Contains('*')) isStar = true;
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);
            long unlock = 0;
            long levels = 0;
            byte[] levelCount = Levels();

            for (int i = 0; i < allData.Length; i++)
            {
                if (allData[i] == 5 && allData[i + 1] == 0x2c && allData[i + 2] == 1 && allData[i + 3] == 4 && allData[i + 4] == 0x0c)
                {
                    stream.Position = i + 6005;
                    levels = i + 12005;
                    Console.WriteLine("stream: " + stream.Position + " levels: " + levels);
                }
                else if (allData[i] == 0x2C && allData[i + 1] == 01 && allData[i + 2] == 0 && allData[i - 1] == 0 && allData[i + 3] == 0 && allData[i - 2] == 0 && allData[i - 3] == 0)
                {
                    unlock = i;
                    Console.WriteLine("unlock: " + unlock);
                    break;
                }
            }
            if (levels == 0 || unlock == 0 || stream.Position == 0)
            {
                Console.WriteLine("Sorry your SoL position couldn't be found, you are either using an old save or the editor is bugged - if that is the case please contact me on discord or in #tool-help");
                return;
            }
            try
            {
                if (split.Length == 1)
                {
                    if (!isStar)
                    {
                        int id = int.Parse(answer) - 1;
                        stream.Position += id * 4;
                        stream.WriteByte(08);
                        stream.Position = unlock - 6152 + ((id + 1) * 4);
                        stream.WriteByte(03);
                        stream.Position = levels + (id * 97) - id;
                        for (int i = 0; i < levelCount[id]; i++)
                        {
                            stream.WriteByte(01);
                            stream.Position += 7;
                        }
                        Console.WriteLine("Cleared subchapter " + (id + 1));
                    }
                    else
                    {
                        string[] starr = answer.Split('*');
                        int id = int.Parse(starr[0]) - 1;
                        int stars = int.Parse(starr[1]);
                        if (stars > 4) stars = 4;
                        stream.Position += id * 4;
                        for (int i = 0; i < stars; i++)
                        {
                            stream.WriteByte(08);
                        }
                        stream.Position = unlock - 6152 + ((id + 1) * 4);
                        stream.WriteByte(03);
                        stream.Position = levels + (id * 97) - id;
                        long startpos = stream.Position;
                        for (int i = 0; i < stars; i++)
                        {
                            for (int j = 0; j < levelCount[id]; j++)
                            {
                                stream.WriteByte(03);
                                stream.Position += 7;
                            }
                            stream.Position = startpos + (i * 2) + 2;

                        }
                        Console.WriteLine("Cleared subchapter " + (id + 1) + " At level {0} star", stars);
                    }
                }
                else if (answer.Contains('-'))
                {
                    if (!isStar)
                    {
                        string[] splits = answer.Split('-');
                        int firstid = int.Parse(splits[0]) - 1;
                        int secondid = int.Parse(splits[1]) - 1;
                        stream.Position += firstid * 4;
                        for (int i = 0; i < secondid - firstid + 1; i++)
                        {
                            stream.WriteByte(08);
                            stream.Position += 3;

                        }
                        stream.Position = unlock - 6152 + ((firstid + 1) * 4);
                        //stream.Position += firstid * 4;
                        for (int i = 0; i < secondid - firstid + 1; i++)
                        {
                            stream.WriteByte(03);
                            stream.Position += 3;
                        }
                        stream.Position = levels + (firstid * 97) - firstid;
                        long startpos = stream.Position;
                        for (int j = 0; j < secondid - firstid + 1; j++)
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                stream.WriteByte(01);
                                stream.Position += 7;
                            }
                            stream.Position = startpos + 96;
                        }
                        Console.WriteLine("Cleared subchapters " + (firstid + 1) + " to " + (secondid + 1));

                    }
                    else
                    {
                        string[] splits = answer.Split('-');
                        string[] first = splits[0].Split('*');
                        string[] second = splits[1].Split('*');
                        int secondid = int.Parse(second[0]) - 1;
                        int firstid = int.Parse(first[0]) - 1;
                        int stars = int.Parse(first[1]);
                        long pos = stream.Position += firstid * 4;
                        stream.Position += firstid * 4;
                        if (stars > 4) stars = 4;
                        for (int i = 0; i < secondid - firstid + 1; i++)
                        {
                            for (int j = 0; j < stars; j++)
                            {
                                stream.Position = pos + j;
                                stream.WriteByte(08);
                                stream.Position -= 1;
                            }
                            pos += 4;
                        }
                        pos = unlock - 6152 + ((firstid + 1) * 4);
                        //stream.Position += firstid * 4;
                        for (int i = 0; i < secondid - firstid + 1; i++)
                        {
                            for (int j = 0; j < stars; j++)
                            {
                                stream.Position = pos + j;
                                stream.WriteByte(03);
                                stream.Position -= 1;
                            }
                            pos += 4;
                        }
                        stream.Position = levels + (firstid * 97) - firstid;
                        long startpos = stream.Position;
                        long pos2 = stream.Position;
                        for (int j = 0; j < secondid - firstid + 1; j++)
                        {
                            for (int k = 0; k < stars; k++)
                            {
                                for (int i = 0; i < 8; i++)
                                {
                                    stream.WriteByte(01);
                                    stream.Position += 7;
                                }
                                stream.Position = pos2 + (k * 2) + 2;
                            }
                            stream.Position = startpos + (j * 96) + 96;
                            pos2 = stream.Position;
                        }
                        Console.WriteLine("Cleared subchapters " + (firstid + 1) + " to " + (secondid + 1) + " At {0} stars", stars);
                    }
                }
            }
            catch (FormatException e)
            {
                ColouredText(e.Message + "\n", ConsoleColor.White, ConsoleColor.Red);
                stream.Close();
                SoL(path);
            }
        }
    }
}
