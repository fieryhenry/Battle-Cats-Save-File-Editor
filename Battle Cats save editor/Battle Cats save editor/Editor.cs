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
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Diagnostics;

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
                Filter = "battle cats save(*.*)|*.*",
                Title = "Select save"
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
            Console.WriteLine("What game version are you using? (e.g en, jp, kr), note: en currently has the most support with the editor, so features may not work in other versions");
            gameVer = Console.ReadLine();
        }
        static string MakeRequest(WebRequest request)
        {
            WebResponse response = request.GetResponse();
            using Stream dataStream = response.GetResponseStream();
            StreamReader reader = new(dataStream);
            string responseFromServer = reader.ReadToEnd();
            return responseFromServer;
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
            string version = "2.36.0";

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
        static void Options()
        {
            string[] brokenFeatures = new string[50];
            int[] nums = new int[brokenFeatures.Length];
            string[] name = new string[brokenFeatures.Length];
            string[] txt = new string[brokenFeatures.Length];
            try
            {
                brokenFeatures = MakeRequest(WebRequest.Create("https://raw.githubusercontent.com/fieryhenry/Battle-Cats-Save-File-Editor/main/BrokenFeatures.txt")).Split('\n');

                for (int i = 0; i < brokenFeatures.Length; i++)
                {
                    string[] allSplits = brokenFeatures[i].Split('/');
                    nums[i] = int.Parse(allSplits[0]);
                    name[i] = allSplits[1];
                    txt[i] = allSplits[2];
                }
            }
            catch
            {

            }
            string[] fileToOpen = Savepaths;
            string path = Path.Combine(fileToOpen[0]);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nBackup your save before using this editor!\nIf you get an error along the lines of \"Your save is active somewhere else\"then select option 25-->5, and select a save that doesn't have that error and never has had the error\n", fileToOpen);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Thanks to: Lethal's editor for being a tool for me to use when figuring out how to patch save files, uploading the save data onto the servers how to and edit cf/xp\nAnd thanks to beeven and csehydrogen's open source work, which I used to implement the save patching algorithm\n");
            Console.ForegroundColor = ConsoleColor.White;

            ColouredText("Warning: if you are using a non en save, many features won't work, or they might corrupt your save data, so make sure you back up your saves!\n", ConsoleColor.White, ConsoleColor.Red);

            ColouredText("&What would you like to do?&\n0.& Select a new save\n1.& Change Cat food\n&2.& Change XP\n&3.& Get all treasures to a specific level\n&4.& All cats upgraded to a specific level" +
                "\n&5.& Change leadership\n&6.& Change NP\n&7.& Change cat tickets\n&8.& change rare cat tickets" +
                "\n&9.& Change platinum tickets\n&10.& Change gacha seed\n&11.& All cats evolved(you must first have unlocked the ability to " +
                "evolve cats + you need to click the \"cycle\" icon on the bottom right of your cat)\n&12.& Change battle item count" +
                "\n&13.& Change catseyes(must have catseyes unlocked)\n&14.& Get/remove All cats\n&15.& Get/remove a specific cat" +
                "\n&16.& Upgrade a specific cat to a specific level\n&17.& Unlock treasures of a specific chapter\n&18.& Evolve a specific cat\n&19.& Change cat fruits and cat fruit seeds\n" +
                "&20.& Talent upgrade cats\n" +
                "&21.& Clear story chapters\n&22.& Patch data(not necessary to use, because your save is automatically patched after every edit)\n&23.& More small edits and fixes\n&24.& Display current gacha seed" +
                "\n&25.& Change all into the future timed score rewards\n&26.& Clear stories of legends subchpaters chapters (doesn't include uncanny legends)\n&27.& Enter game modding menu, contains stuff on .packs and .lists" +
                "\n&28.& Change talent orbs(must have talent orbs unlocked)\n&29.& Change treasure level for specific " +
                "benefits, e.g energy drink or aqua crystal\n&30.& Clear all zombie stages\n&31.& Enter gamatoto/ototo menu, contains all gamatoto/ototo related edits\n&32.& Clear all aku stages(must have the aku realm unlocked)\n&33.& Set amount of unlocked slots in the equip menu\n" +
                "&34.& Change legend tickets\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            byte[] CatNumber = new byte[20];
            CatNumber[0] = GetCatNumber(path);
            CatNumber[1] = 0x02;
            catAmount = BitConverter.ToInt32(CatNumber, 0);
            int Choice = (int)Inputed();
            bool found = false;
            int broken = 0;
            for (int i = 0; i < brokenFeatures.Length; i++)
            {
                if (nums[i] == Choice)
                {
                    ColouredText($"&WARNING:&{txt[i]}\n", ConsoleColor.DarkYellow, ConsoleColor.Red);
                    Console.ForegroundColor = ConsoleColor.White;
                    found = true;
                    broken = i;
                }
            }
            if (found)
            {
                Console.WriteLine("Do you want to continue?(yes/no):");
                string stay = Console.ReadLine();
                if (stay.ToLower() != "yes")
                {
                    Console.WriteLine($"Exiting {name[broken]} now");
                    Options();
                }
            }
            for (int i = 0; i < fileToOpen.Length; i++)
            {
                path = fileToOpen[i];
                switch (Choice)
                {
                    case 0: SelSave(); Options(); break;
                    case 1: CatFood(path); break;
                    case 2: XP(path); break;
                    case 3: MaxTreasures(path); break;
                    case 4: CatUpgrades(path); break;
                    case 5: Leadership(path); break;
                    case 6: NP(path); break;
                    case 7: CatTicket(path); break;
                    case 8: CatTicketRare(path); break;
                    case 9: PlatinumTickets(path); break;
                    case 10: Seed(path); break;
                    case 11: Evolve(path); break;
                    case 12: Items(path); break;
                    case 13: Catseyes(path); break;
                    case 14: Cats(path); break;
                    case 15: SpecifiCat(path); break;
                    case 16: SpecifUpgrade(path); break;
                    case 17: SepecTreasures(path); break;
                    case 18: EvolveSpecific(path); break;
                    case 19: CatFruit(path); break;
                    case 20: Talents(path); break;
                    case 21: Stage(path); break;
                    case 22: break;
                    case 23: Menu(path); break;
                    case 24: GetSeed(path); break;
                    case 25: TimedScore(path); break;
                    case 26: SoL(path); break;
                    case 27: GameModdingMenu(); break;
                    case 28: TalentOrbs(path); break;
                    case 29: VerySpecificTreasures(path); break;
                    case 30: Outbreaks(path); break;
                    case 31: GamOtotoMenu(path); break;
                    case 32: ClearAku(path); break;
                    case 33: Slots(path); break;
                    case 34: LegendTicket(path); break;
                    //case 33: CreateMetaData(50, "catfood", path);break;
                    default: Console.WriteLine("Please input a number that is recognised"); break;
                }
                PatchSaveFile(gameVer, path);
            }
            ColouredText("Press enter to continue\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            Console.ReadLine();
            Options();
        }
        static void GamOtotoMenu(string path)
        {
            ColouredText("&Welcome to the gamatoto/ototo menu&\n&1.& Edit base materials\n&2.& Edit catamins\n&3.& Edit gamatoto helpers\n&4.& Edit gamatoto XP\n&5.& Edit ototo engineers\n&6.& Edit cat cannon/base upgrades\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            int choice = (int)Inputed();

            switch (choice)
            {
                case 1: BaseMats(path); break;
                case 2: Catamin(path); break;
                case 3: GamHelp(path); break;
                case 4: GamXP(path); break;
                case 5: Engineers(path); break;
                case 6: CatCannon(path); break;
                default: Console.WriteLine("Please input a number that is recognised"); break;

            }
        }
        static void GameModdingMenu()
        {
            ColouredText("&Welcome to the game modding menu&\n&1.&Decrypt .list and .pack files\n&2.&Encrypt a folder of game files and turn them into encrypted .pack and .list files\n&3.&Update the md5 sum in the libnative file for modified .list and .pack files (required to do before putting the .pack and .list files into the game, otherwise you get dataread error h01)\n" +
                "&4.&Enter the game file parsing/editing menu - contains things like editng the unit*.csv files\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            int choice = (int)Inputed();

            switch (choice)
            {
                case 1: Decrypt("b484857901742afc"); break;
                case 2: EncryptData("b484857901742afc"); break;
                case 3: MD5Lib(); break;
                case 4: GameFileParsing(); break;
                default: Console.WriteLine("Please input a number that is recognised"); break;

            }
        }
        static void GameFileParsing()
        {
            ColouredText("&Welcome to the game file parsing/editing menu&\n&1.& Edit unit*.csv files\n&2.& Edit stage*.csv files\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            int choice = (int)Inputed();

            switch (choice)
            {
                case 1: Unitcsv(); break;
                case 2: Stagecsv(); break;
                default: Console.WriteLine("Please enter a recognised number"); break;
            }
        }
        static void ClearAku(string path)
        {
            byte[] akuConditions = { 0x31, 0x01 };
            byte[] tenPointNineConditions = { 0x24, 0x8A, 0x01 };

            using var stream1 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            int length = (int)stream1.Length;
            byte[] allData = new byte[length];
            stream1.Read(allData, 0, length);
            stream1.Close();

            // Search for version 10.9 content
            int verPos = Search(path, tenPointNineConditions, true, allData.Length - 16)[0];
            // Search for aku realm position from 10.9 content
            int pos = Search(path, akuConditions, true, verPos)[0];

            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            // Set stages to clear
            for (int j = 0; j < 49; j++)
            {
                stream.Position = pos + 2 + (j * 2);
                stream.WriteByte(1);
            }

            Console.WriteLine("Successfully cleared all aku stages, if you haven't unlocked the aku realm yet, this feature won't work for you");
        }
        [DataContract]
        public class ManagedItemDetails
        {
            [DataMember]
            public int amount { get; set; }
            [DataMember]
            public string detailCode { get; set; }
            [DataMember]
            public int detailCreatedAt { get; set; }
            [DataMember]
            public string detailType { get; set; }
            [DataMember]
            public string managedItemType { get; set; }
        }
        [DataContract]
        public class BackupMetaData
        {
            [DataMember]
            public List<ManagedItemDetails> managedItemDetails { get; set; }
            [DataMember]
            public int playTime { get; set; }
            [DataMember]
            public int rank { get; set; }
            [DataMember]
            public int[] receiptLogIds { get; set; }
            [DataMember]
            public string signature_v1 { get; set; }
        }
        public static string RandomString(int length)
        {
            Random random = new();
            const string chars = "0123456789abcdef";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        static void CreateMetaData(int amount, string type, string path)
        {
            FileStream writeStream = File.Create("BACKUP_META_DATA");
            BackupMetaData meta = new();

            Guid guid = Guid.NewGuid();
            StringBuilder stringBuilder = new(guid.ToString());
            stringBuilder[14] = '4';
            stringBuilder[19] = '8';

            int time = (int)DateTimeOffset.Now.ToUnixTimeSeconds();

            var details = new List<ManagedItemDetails>
            {
                new ManagedItemDetails { amount = amount, detailCode = stringBuilder.ToString(), detailCreatedAt = time, detailType = "get", managedItemType = type }
            };
            int rank = CalculateUR(path);
            Console.WriteLine($"Is {rank} exactly your user Rank?(yes/no):");
            string answer = Console.ReadLine().ToLower();
            if (answer == "no")
            {
                Console.WriteLine("Please enter your user rank accurately:");
                rank = (int)Inputed();
            }
            meta.managedItemDetails = details;
            meta.playTime = 38812;
            meta.rank = rank;
            meta.receiptLogIds = new int[0];
            meta.signature_v1 = RandomString(80);

            DataContractJsonSerializer serializer = new(typeof(BackupMetaData));
            serializer.WriteObject(writeStream, meta);
            writeStream.Close();

            Console.WriteLine("Contents of " + "BACKUP_META_DATA");
            Console.WriteLine(File.ReadAllText("BACKUP_META_DATA"));

            //var json = Serialize(book);
        }
        static void LegendTicket(string path)
        {
            using var stream1 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream1.Length;
            byte[] allData = new byte[length];
            stream1.Read(allData, 0, length);

            stream1.Close();

            // Search for legend ticket position
            byte[] condtions2 = { 0x00, 0x78, 0x63, 0x01, 0x00 };
            int pos = Search(path, condtions2, false, allData.Length - 800)[0];

            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            if (pos <= 0)
            {
                Console.WriteLine("Error, your legend ticket position couldn't be found, please report this on discord");
                return;
            }
            byte[] ticketB = new byte[4];
            stream.Position = pos + 5;
            stream.Read(ticketB, 0, 4);

            int tickets = BitConverter.ToInt16(ticketB, 0);
            Console.WriteLine($"You have {tickets} legend tickets");
            Console.WriteLine("How many legend tickets do you want? (max unknown, but probably 9 or less)");
            tickets = (int)Inputed();
            byte[] bytes = Endian(tickets);

            stream.Position = pos + 5;
            stream.Write(bytes, 0, 4);
            Console.WriteLine($"Set legend tickets to {tickets}");
        }
        static void Elsewhere(string path)
        {
            Console.WriteLine("Please select a working save that doesn't have 'Save is used elsewhere' and has never had it in the past\nPress enter to select that save");
            Console.ReadLine();
            var FD = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "working battle cats save(*.*)|*.*"
            };
            string path2 = "";
            if (FD.ShowDialog() == DialogResult.OK)
            {
                string[] fileToOpen = FD.FileNames;
                path2 = Path.Combine(fileToOpen[0]);
            }
            else
            {
                ColouredText("\nPlease select your save\n\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                SelSave();
            }
            byte[] condtions1 = { 0x2d, 0x00, 0x00, 0x00, 0x2e };
            // Search for rough inquiry code position in second save
            int pos1 = Search(path2, condtions1)[0];

            using var stream1 = new FileStream(path2, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream1.Length;
            byte[] allData = new byte[length];
            stream1.Read(allData, 0, length);

            byte[] codeBytes = new byte[36];
            byte[] codeBytes2 = new byte[9];
            byte[] iqExtra = new byte[11];
            byte[] lastKey = new byte[45];
            int[] found = new int[6];
            stream1.Close();

            // Search for token position in second save
            byte[] condtions2 = { 0x00, 0x78, 0x63, 0x01, 0x00 };
            int pos2 = Search(path2, condtions2, false, allData.Length - 800)[0];

            using var stream = new FileStream(path2, FileMode.Open, FileAccess.ReadWrite);

            // Search for inquiry code position from rough position in second save
            for (int j = 1900; j < 2108; j++)
            {
                if (allData[pos1 - j] == 9 && allData[pos1 - j + 1] == 0 && allData[pos1 - j + 2] == 0 && allData[pos1 - j + 3] == 0 && allData[pos1 - j - 1] == 0 && allData[pos1 - j + 23] == 0x2c)
                {
                    found[0] = 1;
                    // Save it in an array
                    Array.Copy(allData, pos1 - j + 4, iqExtra, 0, 11);
                    break;
                }
            }
            // Check for token
            for (int i = pos2 + 8; i < pos2 + 100; i++)
            {
                if (allData[i] >= 48 && allData[i+1] >= 48 && allData[i+2] >= 48 && allData[i+3] >= 48)
                {
                    pos2 = i;
                    found[1] = 1;
                    break;
                }
            }
            // Save token to array
            Array.Copy(allData, pos2, lastKey, 0, 45);

            if (found.Sum() < 2)
            {
                Console.WriteLine("Sorry a position couldn't be found\nEither your save is invalid or the edtior is bugged, if it is please contact me on the discord linked in the readme.md");
                return;
            }
            // Search for rough inquiry code in first save
            int pos3 = Search(path, condtions1)[0];
            stream.Close();
            using var stream3 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            length = (int)stream3.Length;
            allData = new byte[length];
            stream3.Read(allData, 0, length);

            stream3.Close();
            // Search for token position in first save
            int pos4 = Search(path, condtions2, false, allData.Length - 800)[0];

            using var stream2 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            // Search for inquiry code position starting from rough position
            for (int j = 1900; j < 2108; j++)
            {
                if (allData[pos3 - j] == 9 && allData[pos3 - j + 1] == 0 && allData[pos3 - j + 2] == 0 && allData[pos3 - j + 3] == 0 && allData[pos3 - j - 1] == 0 && allData[pos3 - j + 23] == 0x2c)
                {
                    found[3] = 1;
                    stream2.Position = pos3 - j + 4;
                    // Set inquiry code in first save to inquiry code in second save
                    stream2.Write(iqExtra, 0, 11);
                    break;
                }
            }
            for (int i = pos4 + 8; i < pos4 + 100; i++)
            {
                if (allData[i] >= 48 && allData[i + 1] >= 48 && allData[i + 2] >= 48 && allData[i + 3] >= 48)
                {
                    pos4 = i;
                    break;
                }
            }
            stream2.Position = pos4;
            // Set token in first save to token in second save
            stream2.Write(lastKey, 0, 45);
            found[4] = 1;

            if (found.Sum() < 4)
            {
                Console.WriteLine("Sorry a position couldn't be found\nEither your save is invalid or the edtior is bugged, if it is please contact me on the discord linked in the readme.md");
                return;
            }
            else
            {
                Console.WriteLine("Success");
            }
        }
        static void Stagecsv()
        {
            OpenFileDialog fd = new()
            {
                Filter = "files (stage*.csv)|stage*.csv",
                Title = "Select a stage*.csv file"
            };
            if (fd.ShowDialog() != DialogResult.OK)
            {
                Console.WriteLine("Please select .csv files");
                Options();
            }
            string path = fd.FileName;
            string[] csvData = File.ReadAllLines(path);

            string stageID = string.Join("", csvData[0].Split(','));
            int hasID = 2;
            int hasID2 = 1;
            Console.WriteLine(csvData[0].Split(',').Length);
            // Check if stage csv contains a stage id
            if (csvData[0].Split(',').Length > 7)
            {
                hasID = 1;
                hasID2 = 0;
                stageID = "None";
            }
            int index = stageID.IndexOf('/');

            // Remove comments from file
            string stageIDTrim = stageID;
            try
            {
                stageIDTrim = stageID.Remove(index);
            }
            catch
            {

            }
            // Store main data about the stage, e.g base health, stage width, max enemies
            string[] baseData = csvData[hasID2].Split(',');

            string[] BaseStrings =
            {
                "Stage Width", "Base health", "Minimum spawn frame", "Maximum spawn frame", "Background type", "Maximum enemies",
            };
            string BaseCol = "";
            for (int i = 0; i < BaseStrings.Length; i++)
            {
                BaseCol += $"&{BaseStrings[i]}:& {baseData[i]}\n";
            }
            string[] EnemyData = new string[csvData.Length - hasID];

            string[] EnemyStrings =
            {
                "Enemy ID", "Amount to spawn in total", "First spawn frame", "Time between spawns in frames min",
                "Time between spawns in frames max", "Spawn when base health has reached %", "Front z-layer", "Back z-layer", "Boss flag",
                "Strength multiplier"
            };
            int fail = 0;
            List<List<string>> EnemySlotData = new();
            // Loop through enemy slots
            for (int i = 0; i < csvData.Length - hasID; i++)
            {
                // Set enemy data i to enemy slot i
                EnemyData[i] = csvData[i + hasID];
                // Split data into an array
                string[] allData = EnemyData[i].Split(',');
                // Turn data into list
                List<string> LsData = allData.ToList();
                // Check if this is the end of the enemy slots
                if (LsData.Count < 5)
                {
                    fail = i;
                    break;
                }
                // Add enemy slot data to list
                EnemySlotData.Add(LsData);
            }

            ColouredText($"Stage ID:&{stageIDTrim}\n{BaseCol}", ConsoleColor.White, ConsoleColor.DarkYellow);
            for (int i = 0; i < EnemySlotData.Count; i++)
            {
                ColouredText($"\n&Enemy Slot &{i + 1}&:\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                for (int j = 0; j < EnemySlotData[i].Count; j++)
                {
                    if (EnemySlotData[i][0] == "0")
                    {
                        ColouredText("Empty\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                        break;
                    }
                    if (j == 1 && EnemySlotData[i][j] == "0")
                    {
                        EnemySlotData[i][j] = "unlimited";
                    }
                    try
                    {
                        ColouredText($"&{EnemyStrings[j]}:&{EnemySlotData[i][j]}&\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                    }
                    catch
                    {

                    }
                }
            }
            ColouredText("&What do you want to edit?(1 &stage data&, 2 &enemy spawning data&):\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            int answer = (int)Inputed();
            string complete = "";
            // Stage data
            if (answer == 1)
            {
                Console.WriteLine("What do you want to edit?(you can enter multiple ids separated by spaces to edit multiple at once):");
                for (int i = 0; i < BaseStrings.Length; i++)
                {
                    ColouredText($"&{i + 1}. &{BaseStrings[i]}&\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                }
                string[] response = Console.ReadLine().Split(' ');
                for (int j = 0; j < response.Length; j++)
                {
                    int id = int.Parse(response[j]);
                    ColouredText($"&What do you want to set &{BaseStrings[id - 1]}& to?:\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                    string val = Console.ReadLine();
                    baseData[id - 1] = val;
                }
                for (int i = 0; i < baseData.Length; i++)
                {
                    if (i == baseData.Length - 1 && hasID == 1)
                    {
                        // If it's the final item, don't add a comma
                        complete += $"{baseData[i]}";
                    }
                    else
                    {
                        complete += $"{baseData[i]},";
                    }
                }
                // Set base data to edited base data
                csvData[hasID2] = complete;
            }
            // Enemy data
            else if (answer == 2)
            {
                Console.WriteLine("What enemy slot do you want to edit?(you can enter multiple slots separated by spaces to edit multiple at once):");
                for (int i = 0; i < EnemySlotData.Count; i++)
                {
                    ColouredText($"{i + 1}. &Enemy id:& {EnemySlotData[i][0]}&\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                }
                string[] response = Console.ReadLine().Split(' ');
                for (int i = 0; i < response.Length; i++)
                {
                    int slot = int.Parse(response[i]);
                    ColouredText($"&What do you want to edit in slot &{slot}&?(you can enter multiple slots separated by spaces to edit multiple at once):\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                    for (int j = 0; j < EnemyStrings.Length; j++)
                    {
                        ColouredText($"&{j + 1}.& {EnemyStrings[j]}&\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                    }
                    string[] response2 = Console.ReadLine().Split(' ');
                    for (int j = 0; j < response2.Length; j++)
                    {
                        int toEdit = int.Parse(response2[j]);
                        ColouredText($"&What do you want to set &{EnemyStrings[toEdit - 1]}& to?:\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                        string val = Console.ReadLine();
                        Console.WriteLine(slot);
                        EnemySlotData[slot - 1][toEdit - 1] = val;
                    }
                }
                for (int i = 0; i < EnemySlotData.Count; i++)
                {
                    for (int j = 0; j < EnemySlotData[i].Count; j++)
                    {
                        if (EnemySlotData[i][j] == "unlimited")
                        {
                            EnemySlotData[i][j] = "0";
                        }
                        if (j == EnemySlotData[i].Count - 1)
                        {
                            // If it's the final item, don't add a comma
                            complete += $"{EnemySlotData[i][j]}";
                        }
                        else
                        {
                            complete += $"{EnemySlotData[i][j]},";
                        }
                    }
                    complete += "\n";
                }
            }
            else
            {
                Console.WriteLine("Please enter either 1 or 2");
                Stagecsv();
            }
            string Final = "";
            // If csv has a stage id
            if (hasID == 2)
            {
                Final += csvData[0] + "\n";
            }
            // If base data was modified
            if (answer == 1)
            {
                // Add edited base data to final string
                Final += complete + "\n";
                // Add enemy data to final string
                for (int i = 0; i < EnemyData.Length; i++)
                {
                    Final += EnemyData[i] + "\n";
                }
            }
            else
            {
                // Add base data to final string
                Final += csvData[hasID2] + "\n";
                // Add enemy data to final string
                Final += complete;
            }
            // Removing trailing newlines from file to make sure file only has 1 newline
            Final = Final.Trim('\n');
            Final += "\n";
            // If no other data exists at the end of the enemy slots, skip the next for loop and add a newline
            if (fail == 0)
            {
                fail = 50;
                Final += "\n";
            }
            // If other data exists at the end of the enemy slots, add it to the final string
            for (int i = fail + hasID; i < csvData.Length - 1; i++)
            {
                Final += csvData[i] + "\n";
            }
            // Write final string to the file
            File.WriteAllText(path, Final);
            Console.WriteLine("\nData: \n" + Final + "\n");

            // Make sure file length is divisible by 16, so encryption works
            List<byte> ls = File.ReadAllBytes(path).ToList();
            int rem = (int)Math.Ceiling((decimal)ls.Count / 16);
            rem *= 16;
            rem -= ls.Count;
            // Add data to end of file so file length is divisible by 16
            for (int i = 0; i < rem && rem != 16; i++)
            {
                ls.Add((byte)rem);
            }
            // Write finished data to file
            File.WriteAllBytes(path, ls.ToArray());
        }
        static void Unitcsv()
        {
            OpenFileDialog fd = new()
            {
                Filter = "files (unit*.csv)|unit*.csv",
                Title = "Select a unit*.csv file"
            };
            if (fd.ShowDialog() != DialogResult.OK)
            {
                Console.WriteLine("Please select .csv files");
                Options();
            }
            string path = fd.FileName;
            string[] csvData = File.ReadAllLines(path);

            string[] firstFormData = csvData[0].Split(',');
            string[] secondFormData = csvData[1].Split(',');
            string[] thirdFormData = new string[secondFormData.Length];
            bool hasTrue = false;
            // Check if unit has true form data (although all cats seem to have true form data even if they don't have a true form in game)
            if (csvData.Length >= 3)
            {
                thirdFormData = csvData[2].Split(',');
                if (thirdFormData.Length > 5)
                {
                    hasTrue = true;
                }

            }

            string[] values =
            {
                "HP", "Knockback amount", "Movement Speed", "Attack Power", "Time between attacks", "Attack Range", "Base cost", "Recharge time",
                "Hit box position", "Hit box size", "Red effective flag", "Always zero", "Area attack flag", "Attack animation", "Min z layer",
                "Max z layer", "Floating effective flag", "Black effective flag", "Metal effective flag", "White effective flag", "Angel effective flag", "Alien effective flag",
                "Zombie effective flag", "Strong against flag", "Knockback chance", "Freeze chance", "Freeze duration", "Slow chance", "Slow duration",
                "Resistant flag", "Triple damage flag", "Critical chance", "Attack only flag", "Extra money from enemies flag", "Base destroyer flag", "Wave chance",
                "Wave attack level", "Weaken chance", "Weaken duration", "Weaken to (decrease attack to percentage left)", "HP remain strength",
                "Boost strength multiplier", "Survive chance", "If unit is metal flag", "Long range start", "Long range append", "Immune to wave flag", "Block wave flag",
                "Resist knockbacks flag", "Resist freeze flag", "Resist slow flag", "Resist weaken flag", "Zombie killer flag", "Witch killer flag","Witch effective flag", "Not effected by boss wave flag",
                "Frames before automatically dying -1 to never die automatically", "Always -1", "Death after attack flag", "Second attack power", "Third attack power", "Second attack animation", "Third attack animation", "Use ability on first hit flag",
                "Second attack flag", "Third attack flag", "Spawn animation, -1, 0", "Soul animation -1, 0, 1, 2, 3, 5, 6, 7", "Unike spawn animation", "Gudetama soul animation",
                "Barrier break chance", "Warp Chance", "Warp Duration", "Min warp distance", "Max warp Distance", "Warp blocker flag", "Eva Angel Effective",
                "Eva angel killer flag", "Relic effective flag", "Immune to curse flag", "Insanely tough flag", "Insane damage flag", "Savage blow chance", "Savage blow level", "Dodge attack chance",
                "Dodge attack duration", "surge attack chance", "Surge attack min range", " Surge attack min range", "Surge attack level", "Toxic immunity flag", "Surge immunity flag", "Curse chance", "Curse duration", "Unkown", "Aku shield break chance",
                "Aku effective flag"
            };

            int choice;
            // If unit has true form display the option to edit it
            if (hasTrue)
            {
                ColouredText("Thanks to this resource: &https://pastebin.com/JrCTPnUV& for help with parsing the unit data\n", ConsoleColor.White, ConsoleColor.Green);
                ColouredText("What do you want to edit?:\n&1.& First form\n&2.& Second Form\n&3.& Third Form\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                choice = (int)Inputed();
                if (choice > 3)
                {
                    Console.WriteLine("Please enter a number that is recognised");
                    Unitcsv();
                }
            }
            // If unit doesn't have a true form don't display the option to edit it
            else
            {
                ColouredText("Thanks to this resource: &https://pastebin.com/JrCTPnUV& for help with parsing the unit data\n", ConsoleColor.White, ConsoleColor.Green);
                ColouredText("What do you want to edit?:\n&1.& First form\n&2.& Second Form\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                choice = (int)Inputed();
                if (choice > 2)
                {
                    Console.WriteLine("Please enter a number that is recognised");
                    Unitcsv();
                }
            }
            string unitValues = "";
            string[] dataToUse = new string[firstFormData.Length];
            switch (choice)
            {
                case 1:
                    Console.WriteLine("First form data:");
                    dataToUse = firstFormData;
                    break;
                case 2:
                    Console.WriteLine("Second form data:");
                    dataToUse = secondFormData;
                    break;
                case 3:
                    Console.WriteLine("Third form data:");
                    dataToUse = thirdFormData;
                    break;
            }
            // Form string to output what data the cat already has
            for (int i = 0; i < dataToUse.Length; i++)
            {
                if (dataToUse[i] != "0" && dataToUse[i].Length > 0)
                {
                    try
                    {
                        unitValues += $"&{i + 1}.& {values[i]}";
                        unitValues += $" : &{dataToUse[i]}&\n";
                    }
                    catch
                    {

                    }
                }
            }
            ColouredText(unitValues, ConsoleColor.White, ConsoleColor.DarkYellow);
            string ValueTypes = "";
            for (int i = 0; i < values.Length; i++)
            {
                ValueTypes += $"&{i + 1}.& {values[i]}\n";
            }
            ColouredText("What do you want to edit:\n" + ValueTypes + "\nWhat do you want to edit? You can enter multiple values separated by spaces to edit multiple values at once\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            string[] EditIDs = Console.ReadLine().Split(' ');
            for (int k = 0; k < EditIDs.Length; k++)
            {
                // Read csv again, since if editing multiple ids at once, the data would change
                csvData = File.ReadAllLines(path);

                firstFormData = csvData[0].Split(',');
                secondFormData = csvData[1].Split(',');
                thirdFormData = new string[secondFormData.Length];
                hasTrue = false;
                if (csvData.Length >= 3)
                {
                    thirdFormData = csvData[2].Split(',');
                    if (thirdFormData.Length > 5)
                    {
                        hasTrue = true;
                    }

                }
                int toEdit = int.Parse(EditIDs[k]);

                dataToUse = firstFormData;
                List<string> firstFormList = new(firstFormData.Length);
                List<string> secondFormList = new(firstFormData.Length);
                List<string> thirdFormList = new(firstFormData.Length);
                // Loop through each of the form data
                for (int i = 0; i < 3; i++)
                {
                    string amToAdd = "";
                    int id = dataToUse.Length - 1;
                    // Check if the id that you want to edit is outside the number of values in the csv
                    if (toEdit > dataToUse.Length - 1)
                    {
                        int amountToAdd = toEdit - (dataToUse.Length - 1);
                        for (int j = 1; j < amountToAdd + 1; j++)
                        {
                            // Set ids that must be a specifc value for the cat to function in game
                            string val = "0";
                            if (j + id == 58)
                            {
                                val = "-1";
                            }
                            else if (j + id == 56)
                            {
                                val = "-1";
                            }
                            else if (j + id == 64)
                            {
                                val = "1";
                            }
                            else if (j + id == 67)
                            {
                                val = "-1";
                            }
                            // If this is the last item, don't add a comma to the end of the data
                            if (j == amountToAdd)
                            {
                                amToAdd += $"{val}";
                            }
                            else
                            {
                                amToAdd += $"{val},";
                            }
                        }
                    }
                    string[] amountToAddArr = amToAdd.Split(',');
                    List<string> dataToUseList2 = dataToUse.ToList();
                    for (int j = 0; j < amountToAddArr.Length; j++)
                    {
                        if (amountToAddArr[0] != "")
                        {
                            // Insert the extra space for the id
                            dataToUseList2.Insert((dataToUse.Length - 1) + j, amountToAddArr[j]);
                        }
                    }
                    // First form
                    if (i == 0)
                    {
                        dataToUse = secondFormData;
                        firstFormList = dataToUseList2;
                    }
                    // Second form
                    else if (i == 1)
                    {
                        secondFormList = dataToUseList2;
                        if (hasTrue)
                        {
                            dataToUse = thirdFormData;
                        }
                    }
                    // Third form if it has it
                    else if (i == 2 && hasTrue)
                    {
                        thirdFormList = dataToUseList2;
                    }
                    // If it doesn't have a true form, break
                    else if (i == 1 && !hasTrue)
                    {
                        break;
                    }
                }
                bool stopNow = false;
                try
                {
                    Console.WriteLine($"What value do you want to set {values[toEdit - 1]} to (for proc chance enter as a percentage(without the % sign), for flag values enter a 1 to enable them)");
                }
                catch
                {
                    Console.WriteLine("Error, id is too large");
                    stopNow = true;
                }
                if (!stopNow)
                {
                    int value = (int)Inputed();

                    // First form
                    if (choice == 1)
                    {
                        firstFormList[toEdit - 1] = value.ToString();
                    }
                    // Second form
                    else if (choice == 2)
                    {
                        secondFormList[toEdit - 1] = value.ToString();
                    }
                    // Third form
                    else
                    {
                        thirdFormList[toEdit - 1] = value.ToString();
                    }
                    List<string> dataToUseList = firstFormList;
                    string fin = "";
                    // Loop through form data
                    for (int i = 0; i < 3; i++)
                    {
                        string dataToUseFinal = "";
                        for (int j = 0; j < dataToUseList.Count; j++)
                        {
                            // If this is the last item, don't add a comma
                            if (j == dataToUseList.Count - 1)
                            {
                                dataToUseFinal += $"{dataToUseList[j]}";

                            }
                            // Otherwise add a comma
                            else
                            {
                                dataToUseFinal += $"{dataToUseList[j]},";
                            }
                        }
                        fin += dataToUseFinal + "\n";
                        // First form
                        if (i == 0)
                        {
                            dataToUseList = secondFormList;
                        }
                        // Second form if it has true form
                        else if (i == 1 && hasTrue)
                        {
                            dataToUseList = thirdFormList;
                        }
                        // Second form if it doesn't have a true form
                        else if (i == 1 && !hasTrue)
                        {
                            break;
                        }
                    }
                    ColouredText($"&Set contents of the &{Path.GetFileName(path)}& file to&\n{fin}&\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                    // Write data to file
                    File.WriteAllText(path, fin);
                    byte[] allBytes = File.ReadAllBytes(path);

                    // Make sure file length is divisible by 16, so it encrypts properly
                    List<byte> ls = allBytes.ToList();
                    int rem = (int)Math.Ceiling((decimal)ls.Count / 16);
                    rem *= 16;
                    rem -= ls.Count;
                    for (int i = 0; i < rem && rem != 16; i++)
                    {
                        ls.Add((byte)rem);
                    }
                    File.WriteAllBytes(path, ls.ToArray());
                }
            }
        }
        static void Decrypt(string key)
        {
            Console.WriteLine("Are you running game version jp 10.8 and up? (yes, no)?");
            string ver = Console.ReadLine();
            OpenFileDialog fd = new()
            {
                Multiselect = true,
                Filter = "files (*.pack; .list)|*.pack;*.list"
            };
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
            if (paths.Length % 2 != 0)
            {
                Console.WriteLine("Please enter a .pack and a .list file\nPress enter to continue");
                Console.ReadLine();
                Options();
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

                        using MemoryStream msDecrypt = new(allData);
                        using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
                        using StreamReader srDecrypt = new(csDecrypt);

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
                        if (paths[i].ToLower().Contains("server"))
                        {
                            for (int j = 1; j < listLines.Length - 1; j++)
                            {
                                byte[] content = new byte[offset[j]];
                                Array.Copy(allData, startpos[j], content, 0, offset[j]);

                                byte[] IV = new byte[16];
                                byte[] Key = Encoding.ASCII.GetBytes("89a0f99078419c28");

                                using Aes aesAlg = Aes.Create();
                                aesAlg.Key = Key;
                                aesAlg.IV = IV;
                                aesAlg.Padding = PaddingMode.None;
                                aesAlg.Mode = CipherMode.ECB;

                                byte[] bytes = DecryptData(aesAlg, content);

                                try
                                {
                                    Directory.CreateDirectory(@"game_files/" + Path.GetFileName(paths[i]));
                                    File.WriteAllBytes(@"game_files/" + Path.GetFileName(paths[i]) + "/" + names[j], content);
                                }
                                catch
                                {

                                }
                                float percentageF = ((float)j / (listLines.Length - 2)) * 100;
                                string percentage = percentageF.ToString("0.0");
                                float totalPercentageF = ((float)(i + 1) / 2) / (paths.Length / 2) * 100;
                                string totalPercentage = totalPercentageF.ToString("0.0");
                                ColouredText($"\r&Extracted: &{names[j]} &from &{Path.GetFileName(paths[i])} & {j}&/&{(listLines.Length - 2)}  &{percentage}%& - {(i + 1) / 2}/{paths.Length / 2}  &{totalPercentage}%&                                   ", ConsoleColor.White, ConsoleColor.Green);
                            }
                            Console.WriteLine("Decrypted: " + Path.GetFileName(paths[i]) + " " + (i + 1) / 2 + "/" + paths.Length / 2);
                        }
                        else if (paths[i].ToLower().Contains("local"))
                        {
                            for (int j = 1; j < listLines.Length - 1; j++)
                            {
                                byte[] content = new byte[offset[j]];
                                Array.Copy(allData, startpos[j], content, 0, offset[j]);
                                byte[] IV = new byte[16];
                                byte[] Key = new byte[16];

                                if (ver.ToLower() == "yes")
                                {
                                    byte[] ivtemp = { 0x40, 0xb2, 0x13, 0x1a, 0x9f, 0x38, 0x8a, 0xd4, 0xe5, 0x00, 0x2a, 0x98, 0x11, 0x8f, 0x61, 0x28 };
                                    byte[] keytemp = { 0xd7, 0x54, 0x86, 0x8d, 0xe8, 0x9d, 0x71, 0x7f, 0xa9, 0xe7, 0xb0, 0x6d, 0xa4, 0x5a, 0xe9, 0xe3 };
                                    Key = keytemp;
                                    IV = ivtemp;
                                }
                                else
                                {
                                    byte[] leytemp = { 0x0a, 0xd3, 0x9e, 0x4a, 0xea, 0xf5, 0x5a, 0xa7, 0x17, 0xfe, 0xb1, 0x82, 0x5e, 0xde, 0xf5, 0x21 };
                                    byte[] ivtemp = { 0xd1, 0xd7, 0xe7, 0x08, 0x09, 0x19, 0x41, 0xd9, 0x0c, 0xdf, 0x8a, 0xa5, 0xf3, 0x0b, 0xb0, 0xc2 };
                                    Key = leytemp;
                                    IV = ivtemp;
                                }

                                using Aes aesAlg = Aes.Create();
                                aesAlg.Key = Key;
                                aesAlg.IV = IV;
                                aesAlg.Padding = PaddingMode.None;
                                aesAlg.Mode = CipherMode.CBC;


                                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                                if (!Directory.Exists("game_files/"))
                                {
                                    Directory.CreateDirectory("game_files");
                                }
                                try
                                {
                                    Directory.CreateDirectory(@"game_files/" + Path.GetFileName(paths[i]));
                                }
                                catch { }
                                if (paths[i].ToLower().Contains("imagedata"))
                                {
                                    File.WriteAllBytes(@"game_files/" + Path.GetFileName(paths[i]) + "/" + names[j], content);
                                }
                                else
                                {
                                    byte[] bytes = DecryptData(aesAlg, content);

                                    try
                                    {
                                        File.WriteAllBytes(@"game_files/" + Path.GetFileName(paths[i]) + "/" + names[j], bytes);
                                    }
                                    catch { }
                                }

                                float percentageF = ((float)j / (listLines.Length - 2)) * 100;
                                string percentage = percentageF.ToString("0.0");
                                float totalPercentageF = ((float)(i + 1) / 2) / (paths.Length / 2) * 100;
                                string totalPercentage = totalPercentageF.ToString("0.0");
                                ColouredText($"\r&Extracted: &{names[j]} &from &{Path.GetFileName(paths[i])} & {j}&/&{(listLines.Length - 2)}  &{percentage}%& - {(i + 1) / 2}/{paths.Length / 2}  &{totalPercentage}%&                                   ", ConsoleColor.White, ConsoleColor.Green);

                            }
                            Console.WriteLine("\nDecrypted: " + Path.GetFileName(paths[i]) + " " + (i + 1) / 2 + "/" + paths.Length / 2);
                        }
                    }
                }
                Console.WriteLine("Finished: files can be found in " + Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/game_files/");
            }
        }
        static byte[] DecryptData(Aes aesAlg, byte[] content)
        {
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            if (!Directory.Exists("game_files/"))
            {
                Directory.CreateDirectory("game_files");
            }
            MemoryStream memory = new(content);

            using CryptoStream csDecrypt = new(memory, decryptor, CryptoStreamMode.Read);
            byte[] bytes = new byte[content.Length];
            csDecrypt.Read(bytes, 0, content.Length);

            return bytes;
        }
        static void EncryptData(string key)
        {
            Console.WriteLine("Enter name of .pack file name to be outputed, e.g DataLocal, ImapServer (don't include .pack) + capatalisation must be correct");
            string name = Console.ReadLine();

            FolderBrowserDialog fd = new()
            {
                SelectedPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\game_files",
                Description = "Select a folder of game files to be encrypted into .pack and .list files",
                ShowNewFolderButton = false
            };
            if (fd.ShowDialog() != DialogResult.OK)
            {
                Console.WriteLine("Please select a folder of game content!");
                Options();
            }
            string path = fd.SelectedPath;
            string listFile = "";
            string[] files = Directory.GetFiles(path);
            listFile += files.Length + "\n";
            int previous = 0;
            Console.WriteLine("Making .list file, this may take a minute depending on how many files are used...");
            for (int i = 0; i < files.Length; i++)
            {
                string fileName = Path.GetFileName(files[i]);
                int FileLength = File.ReadAllBytes(files[i]).Length;
                listFile += fileName;
                listFile += "," + previous + "," + FileLength + "\n";
                previous += FileLength;
                //ColouredText($"&Created list line: &{i}& for &{fileName}& - &{i}&/&{files.Length}&\n", ConsoleColor.White, ConsoleColor.DarkYellow);

            }
            Console.WriteLine("Done");
            List<byte> ls = Encoding.ASCII.GetBytes(listFile).ToList();
            int rem = (int)Math.Ceiling((decimal)ls.Count / 16);
            rem *= 16;
            rem -= ls.Count;
            for (int i = 0; i < rem && rem != 16; i++)
            {
                ls.Add((byte)rem);
            }
            if (!Directory.Exists("CompFiles/"))
            {
                Directory.CreateDirectory("CompFiles/");
            }
            Console.WriteLine("List file successfully created");
            File.WriteAllBytes(@"CompFiles/" + name + ".list", ls.ToArray());

            using var stream = new FileStream(@"CompFiles/" + name + ".list", FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            byte[] IV = new byte[16];
            byte[] Key = Encoding.ASCII.GetBytes(key);


            string result = name + ".list";

            using Aes aesAlg = Aes.Create();
            aesAlg.Key = Key;
            aesAlg.IV = IV;
            aesAlg.Padding = PaddingMode.None;
            aesAlg.Mode = CipherMode.ECB;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msEncrypt = new(allData);
            using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Read);

            byte[] bytess = new byte[length];

            csEncrypt.Read(bytess, 0, length);
            stream.Close();

            File.WriteAllBytes(@"CompFiles/" + result, bytess);
            byte[] PackBytes = new byte[previous];
            int preIndex = 0;
            Console.WriteLine("Are you using jp 10.8 and up?(yes/no)");
            string ver = Console.ReadLine();
            Console.WriteLine("Making .pack file, this may take a minute depending on how many files are used...");
            for (int i = 0; i < files.Length; i++)
            {
                byte[] bytef = File.ReadAllBytes(files[i]);
                int FileLen = File.ReadAllBytes(files[i]).Length;
                if (name.Contains("Server"))
                {
                    byte[] IV2 = new byte[16];
                    byte[] Key2 = Encoding.ASCII.GetBytes("89a0f99078419c28");

                    using Aes aesAlg2 = Aes.Create();
                    aesAlg2.Key = Key2;
                    aesAlg2.IV = IV2;
                    aesAlg2.Padding = PaddingMode.None;
                    aesAlg2.Mode = CipherMode.ECB;

                    ICryptoTransform encryptor2 = aesAlg2.CreateEncryptor(aesAlg2.Key, aesAlg2.IV);

                    using MemoryStream ms = new(bytef);
                    using CryptoStream cs = new(ms, encryptor2, CryptoStreamMode.Read);

                    cs.Read(bytef, 0, FileLen);

                    //Console.WriteLine($"Decrypted: {Path.GetFileName(files[i])} {i}/{files.Length}");
                }
                else if (name.Contains("Local"))
                {
                    if (!name.Contains("ImageData"))
                    {
                        byte[] IV2 = new byte[16];
                        byte[] Key2 = new byte[16];

                        if (ver.ToLower() == "yes")
                        {
                            byte[] ivtemp = { 0x40, 0xb2, 0x13, 0x1a, 0x9f, 0x38, 0x8a, 0xd4, 0xe5, 0x00, 0x2a, 0x98, 0x11, 0x8f, 0x61, 0x28 };
                            byte[] keytemp = { 0xd7, 0x54, 0x86, 0x8d, 0xe8, 0x9d, 0x71, 0x7f, 0xa9, 0xe7, 0xb0, 0x6d, 0xa4, 0x5a, 0xe9, 0xe3 };
                            Key = keytemp;
                            IV = ivtemp;
                        }
                        else
                        {
                            byte[] leytemp = { 0x0a, 0xd3, 0x9e, 0x4a, 0xea, 0xf5, 0x5a, 0xa7, 0x17, 0xfe, 0xb1, 0x82, 0x5e, 0xde, 0xf5, 0x21 };
                            byte[] ivtemp = { 0xd1, 0xd7, 0xe7, 0x08, 0x09, 0x19, 0x41, 0xd9, 0x0c, 0xdf, 0x8a, 0xa5, 0xf3, 0x0b, 0xb0, 0xc2 };
                            Key = leytemp;
                            IV = ivtemp;
                        }

                        using Aes aesAlg2 = Aes.Create();
                        aesAlg2.Key = Key;
                        aesAlg2.IV = IV;
                        aesAlg2.Padding = PaddingMode.None;
                        aesAlg2.Mode = CipherMode.CBC;

                        ICryptoTransform encryptor2 = aesAlg2.CreateEncryptor(aesAlg2.Key, aesAlg2.IV);

                        using MemoryStream ms = new(bytef);
                        using CryptoStream cs = new(ms, encryptor2, CryptoStreamMode.Read);

                        cs.Read(bytef, 0, FileLen);
                    }

                    //Console.WriteLine($"Decrypted: {Path.GetFileName(files[i])} {i}/{files.Length}");
                }
                bytef.CopyTo(PackBytes, preIndex);
                preIndex += bytef.Length;
            }
            File.WriteAllBytes(@"CompFiles/" + name + ".pack", PackBytes);
            Console.WriteLine("Done\nThe .list and .pack file can be found in " + Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/CompFiles/");
        }
        static void MD5Lib()
        {
            Console.WriteLine("Please select an so file");
            OpenFileDialog fd = new()
            {
                Filter = "files (*.so)|*.so"
            };
            if (fd.ShowDialog() != DialogResult.OK)
            {
                Console.WriteLine("Please select .so files");
                Options();
            }
            string path = fd.FileName;

            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            List<KeyValuePair<string, byte[]>> listsHash = new();

            string[] order = { "DataLocal.list", "ImageDataLocal.list", "ImageLocal.list", "MapLocal.list", "NumberLocal.list", "resLocal.list", "UnitLocal.list", "ImageDataLocal_fr.list", "ImageLocal_fr.list", "MapLocal_fr.list", "resLocal_fr.list", "ImageDataLocal_it.list", "ImageLocal_it.list", "MapLocal_it.list", "resLocal_it.list", "ImageDataLocal_de.list", "ImageLocal_de.list", "MapLocal_de.list", "resLocal_de.list", "ImageDataLocal_es.list", "ImageLocal_es.list", "MapLocal_es.list", "resLocal_es.list", "DataLocal.pack", "ImageDataLocal.pack", "ImageLocal.pack", "MapLocal.pack", "NumberLocal.pack", "resLocal.pack", "UnitLocal.pack", "ImageDataLocal_fr.pack", "ImageLocal_fr.pack", "MapLocal_fr.pack", "resLocal_fr.pack", "ImageDataLocal_it.pack", "ImageLocal_it.pack", "MapLocal_it.pack", "resLocal_it.pack", "ImageDataLocal_de.pack", "ImageLocal_de.pack", "MapLocal_de.pack", "resLocal_de.pack", "ImageDataLocal_es.pack", "ImageLocal_es.pack", "MapLocal_es.pack", "resLocal_es.pack", "ImageServer_101000_00_en.list", "MapServer_101000_00_en.list", "NumberServer_101000_00_en.list", "UnitServer_101000_00_en.list", "ImageServer_100900_00_en.list", "MapServer_100900_00_en.list", "NumberServer_100900_00_en.list", "UnitServer_100900_00_en.list", "ImageServer_100800_00_en.list", "MapServer_100800_00_en.list", "NumberServer_100800_00_en.list", "UnitServer_100800_00_en.list", "ImageServer_100700_00_en.list", "MapServer_100700_00_en.list", "NumberServer_100700_00_en.list", "UnitServer_100700_00_en.list", "", "ImageServer_100600_01_en.list", "MapServer_100600_02_en.list", "NumberServer_100600_03_en.list", "UnitServer_100600_04_en.list", "LImageServer.list", "LMapServer.list", "LNumberServer.list", "LUnitServer.list", "KImageServer.list", "KMapServer.list", "KNumberServer.list", "KUnitServer.list", "JImageSever.list", "JMapServer.list", "JNumberServer.list", "JUnitServer.list", "IImageServer.list", "IMapServer.list", "INumberServer.list", "IUnitServer.list", "HImageServer.list", "HMapServer.list", "HNumberServer.list", "HUnitServer.list", "GImageServer.list", "GMapServer.list", "GNumberServer.list", "GUnitServer.list", "FImageServer.list", "FMapServer.list", "FNumberServer.list", "FUnitServer.list", "EImageServer.list", "EMapServer.list", "ENumberServer.list", "EUnitServer.list", "DImageServer.list", "DMapServer.list", "DNumberServer.list", "DUnitServer.list", "CImageServer.list", "CMapServer.list", "CNumberServer.list", "CUnitServer.list", "BNumberServer.list", "BUnitServer.list", "AMapServer.list", "ANumberServer.list", "AUnitServer.list", "ImageServer.list", "MapServer.list" };
            int prevIndex = 0;
            for (int i = 6000000; i < length - 10000; i++)
            {
                if (allData[i] == 0x2e && allData[i + 1] == 0x70 && allData[i + 2] == 0x61 && allData[i + 3] == 0x63 && allData[i + 4] == 0x6b)
                {
                    prevIndex = i + 6;
                    for (int j = i + 4; j > i - 64; j--)
                    {
                        if (allData[j] == 0x00)
                        {
                            break;
                        }
                    }
                }
            }
            int count = 0;
            int num = 0;
            int pos = 0;
            for (int i = prevIndex; i < length; i++)
            {
                num++;
                string listName = "";
                try
                {
                    listName = order[count];
                }
                catch
                {
                    pos = i;
                    break;
                }
                string hash = "";
                if (num % 33 == 0)
                {
                    for (int j = i - 32; j < i; j++)
                    {
                        hash += Convert.ToChar(allData[j]);
                    }
                    count++;
                    byte[] hash3 = Encoding.ASCII.GetBytes(hash);
                    listsHash.Add(new KeyValuePair<string, byte[]>(listName, hash3));
                }
            }
            Console.WriteLine("Please select .pack and .list files, you can select multiple at once");
            OpenFileDialog fd2 = new()
            {
                Filter = "files (*.pack; .list)|*.pack;*.list",
                Multiselect = true
            };
            if (fd2.ShowDialog() != DialogResult.OK)
            {
                Console.WriteLine("Please select .pack/.list files");
                Options();
            }
            string[] paths = fd2.FileNames;
            foreach (string path2 in paths)
            {
                string hash2 = CalculateMD5(path2);
                byte[] hashBytes = Encoding.ASCII.GetBytes(hash2);
                bool found = false;
                for (int i = 0; i < listsHash.Count; i++)
                {
                    if (Path.GetFileName(path2) == listsHash[i].Key)
                    {
                        found = true;
                        for (int j = prevIndex; j < length - 10000; j++)
                        {
                            if (allData[j] == listsHash[i].Value[0] && allData[j + 1] == listsHash[i].Value[1] && allData[j + 2] == listsHash[i].Value[2] && allData[j + 3] == listsHash[i].Value[3] && allData[j + 4] == listsHash[i].Value[4] && allData[j + 5] == listsHash[i].Value[5] && allData[j + 6] == listsHash[i].Value[6] && allData[j + 7] == listsHash[i].Value[7] && allData[j + 8] == listsHash[i].Value[8] && allData[j + 9] == listsHash[i].Value[9] && allData[j + 10] == listsHash[i].Value[10] && allData[j + 11] == listsHash[i].Value[11] && allData[j + 12] == listsHash[i].Value[12] && allData[j + 13] == listsHash[i].Value[13] && allData[j + 14] == listsHash[i].Value[14] && allData[j + 15] == listsHash[i].Value[15])
                            {
                                stream.Position = j;
                                stream.Write(hashBytes, 0, hashBytes.Length);
                                break;
                            }
                        }
                    }
                }
                if (!found)
                {
                    Console.WriteLine("The .pack/.list's md5 sum doesn't get checked(so is good to use without getting an error), or the file name is spelt wrong");
                }
                Console.WriteLine("Done!, you should now be able to put the lib file and the .pack/.lists into the game without data read error h01");
            }
        }
        static string CalculateMD5(string filename)
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(filename);
            var hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        static void Menu(string path)
        {
            ColouredText("&Welcome to the small patches and tweaks menu&\n&1.&Close all the bundle menus (if you have used upgrade all cats, you know what this is)\n&2.&Set new account code" +
                "\n&3.&Upgrade the blue upgrades on the right of the normal cat upgrades\n&4.&Fix save is used elsewhere error, whilst selecting a save that has the error(the one you select when you open the editor) select a new save that has never had the save is used elsewhere bug ever(you can re-install the game to get a save like that) and it should fix that error\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            int choice = (int)Inputed();

            switch (choice)
            {
                case 1: Bundle(path); break;
                case 2: NewIQ(path); break;
                case 3: Blue(path); break;
                case 4: Elsewhere(path); break;
                default: Console.WriteLine("Please input a number that is recognised"); break;

            }
        }
        static void VerySpecificTreasures(string path)
        {
            string[] treasrureTypes1 =
            {
                "Energy Drink", "Giant Safe", "Relativity Clock", "Philosopher's Stone", "Smart Material Wall", "Super Register", "Legendary Cat Shield", "Legendary Cat Sword", "Energy Core", "Turbo Machine", "Management Bible",
            };
            string[] treasureTypes2 =
            {
                "Aqua Crystal", "Plasma Crystal", "Ancient Tablet", "Mysterious Force", "Cosmic Energy", "Void Fruit", "Blood Fruit", "Sky Fruit", "Heaven's Fruit", "Time Machine", "Future Tech",
            };
            string[] treasureTypes3 =
            {
                "Stellar Garnet", "Phoebe Beryl", "Lunar Citrine", "Ganymede Topaz", "Callisto Amethyst", "Titanium Fruit", "Antimatter Fruit", "Enigma Fruit", "Dark Matter", "Neutrino", "Mystery Mask"
            };
            // What stages have which treasure
            int[][] treasureLevels1 = new int[][]
            {
            new int[] { 46, 45, 44, 43, 42, 41, 40 },
            new int[] { 39, 38, 37, 36 },
            new int[] { 35, 34, 33, 32, 31 },
            new int[] {30, 29, 28, 27, 26, 25, 24},
            new int[] {23, 22, 19},
            new int[] {20, 21, 18},
            new int[] {17, 16, 15},
            new int[] {14, 13, 12, 11, 10, 9, 8},
            new int[] {7, 6, 5, 4, 3, 2},
            new int[] {1},
            new int[] {47, 48}

            };
            int[][] treasureLevels2 = new int[][]
            {
            new int[] { 46, 42, 39, 36, 33, 30, 27, 24},
            new int[] { 22, 19, 16, 13, 10, 7, 4, 1},
            new int[] {45, 44, 43},
            new int[] {23},
            new int[] { 41, 40, 38, 37, 35 },
            new int[] { 18, 17, 15, 14},
            new int[] { 26, 25, 21, 20 },
            new int[] { 12, 11, 9, 8 },
            new int[] { 6, 5, 3, 2 },
            new int[] { 34, 32, 31, 29, 28 },
            new int[] {47, 48}
            };
            int[][] treasureLevels3 = new int[][]
            {
            new int[] {46, 45, 44, 43, 42},
            new int[] {37, 36, 35, 34, 33},
            new int[] {28, 27, 26, 25, 24},
            new int[] {19, 18, 17, 16, 15},
            new int[] {10, 9, 8, 7, 6},
            new int[] {41, 40, 39, 38},
            new int[] {32, 31, 30, 29},
            new int[] {23, 22, 21, 20},
            new int[] {14, 13, 12, 11},
            new int[] {5, 3, 1, 48},
            new int[] {44, 42, 47}
            };

            Console.WriteLine("What level of treasures of you want?(max 255) 1 = inferior, 2 = normal 3 = superior, anything above 3 just aplifies the treasure effect");
            int level = (int)Inputed();
            if (level > 255) level = 255;
            Console.WriteLine("Do you want a list of the types of treasures?(yes,no):");
            if (Console.ReadLine().ToLower() == "yes")
            {
                ColouredText("Empire of Cats:&" + "\n" + "\n    &Energy Drink& – Worker Cat Efficiency increased! (EoC 1-7) (East Asia Quarantine Zone for Zombie Outbreaks)" + "\n    &Giant Safe& – Worker Cat Wallet Capacity increased! (EoC 8-11) (Indian Ocean QZ for ZO)" + "\n    &Relativity Clock& – Production Speed of Cats increased! (EoC 12-16) (Himalaya-Rift QZ for ZO)" + "\n    &Philosopher's Stone& – XP obtained from battle increased! (EoC 17-23) (Afro-Mediterranean QZ for ZO)" + "\n    &Smart Material Wall& – Cat Base health increased! (EoC 24, 25, 28) (Alps QZ for ZO)" + "\n    &Super Register& – Money for defeating enemies increased! (EoC 27, 26, 29) (West Europe QZ for ZO)" + "\n    &Legendary Cat Shield& – Cat Health increased! (EoC 30-32) (North Atlantic QZ for ZO)" + "\n    &Legendary Cat Sword& – Cat ATK increased! (EoC 33-39) (East Americas QZ for ZO)" + "\n    &Energy Core& – Cat Cannon ATK increased! (EoC 40-45) (Pacific QZ for ZO)" + "\n    &Turbo Machine& – Cat Cannon recharge speed increased! (EoC 46) (Fairbanks QZ for ZO)" + "\n    &Management Bible& – Max Cat Energy increased! (EoC 47-48) (Mauna Kea QZ for ZO)" + "\n" + "\n&Into the Future:&" + "\n" + "\n    &Aqua Crystal& – Attacks against unstarred Aliens are much more powerful! (ItF 1, 5, 8, 11, 14, 17, 20, 23) (? QZ for ZO)" + "\n    &Plasma Crystal& – Attacks against unstarred Aliens are much more powerful! (ItF 25, 28, 31, 34, 37, 40, 43, 46) (? QZ for ZO)" + "\n    &Ancient Tablet& – Your Cat Base's defense is increased! (ItF 2-4) (? QZ for ZO)" + "\n    &Mysterious Force& – Cat Cannon recharge time is decreased. (ItF 24) (? QZ for ZO)" + "\n    &Cosmic Energy& – Cat Cannon attacks are now more powerful! (ItF 6, 7, 9, 10, 12) (? QZ for ZO)" + "\n    &Void Fruit& – Abilities used on Black enemies are more effective! (ItF 29, 30, 32-33) (? QZ for ZO)" + "\n    &Blood Fruit& – Abilities used on Red enemies are more effective! (ItF 21, 22, 26, 27) (? QZ for ZO)" + "\n    &Sky Fruit& – Abilities used on Floating enemies are more effective! (ItF 35, 36, 38, 39) (? QZ for ZO)" + "\n    &Heaven's Fruit& – Abilities used on Angel enemies enemies are more effective! (ItF 41, 42, 44, 45) (? QZ for ZO)" + "\n    &Time Machine& – Energy recovery speed is increased! (ItF 13, 15, 16, 18, 19) (? QZ for ZO)" + "\n    &Future Tech& – Maximum energy total increased! (ItF 47, 48) (? QZ for ZO)" + "\n" + "\n&Cats of the Cosmos:&" + "\n" + "\n    &Stellar Garnet& – Attacks against Starred Aliens are much more powerful! (CotC 1-5)" + "\n    &Phoebe Beryl& – Attacks against Starred Aliens are much more powerful! (CotC 10-14)" + "\n    &Lunar Citrine& – Attacks against Starred Aliens are much more powerful! (CotC 19-23)" + "\n    &Ganymede Topaz& – Attacks against Starred Aliens are much more powerful! (CotC 28-32)" + "\n    &Callisto Amethyst& – Attacks against Starred Aliens are much more powerful! (CotC 37-41)" + "\n    &Titanium Fruit& – Anti-Metal abilities have increased effect! (CotC 6-9)" + "\n    &Antimatter Fruit& – Anti-Zombie abilities have increased effect! (CotC 15-18)" + "\n    &Enigma Fruit& – Anti-Alien abilities have increased effect! (CotC 24-27)" + "\n    &Dark Matter& – Maximum energy total is increased! (CotC 33-36)" + "\n    &Neutrino& – XP received from battle increased! (CotC 42, 44, 46, 48)" + "\n    &Mystery Mask& – A strange effect will activate when Ch.X is cleared! (CotC 43, 45, 47)\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            }
            ColouredText("\nWhat treasures do you want to edit(enter the name of the treasures,e.g energy drink,or ancient tablet), you can enter multiple treasures,separated by underscores, e.g giant safe_neutrino_Energy drink:\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            string[] answer = Console.ReadLine().Trim(' ').Split('_');
            for (int i = 0; i < answer.Length; i++)
            {
                int MainChapterToEdit = -1;
                bool skip = false;
                // Check if treasure exists
                int one = Array.FindIndex(treasrureTypes1, type => type.ToLower() == answer[i].ToLower());
                int two = Array.FindIndex(treasureTypes2, type => type.ToLower() == answer[i].ToLower());
                int three = Array.FindIndex(treasureTypes3, type => type.ToLower() == answer[i].ToLower());
                // If it exists in Empire of Cats
                if (one != -1)
                {
                    MainChapterToEdit = 0;
                }
                // If it exists in Into the Future
                else if (two != -1)
                {
                    MainChapterToEdit = 3;
                }
                // If it exists in Cats of the Cosmos
                else if (three != -1)
                {
                    MainChapterToEdit = 6;
                }
                else
                {
                    skip = true;
                    Console.WriteLine("Treasure type " + answer[i] + " doesn't exist!");
                }
                if (!skip)
                {
                    ColouredText("&What chapters for treasure type &" + answer[i] + "& do you want? (1, 2 or 3) you can enter more chapters separated by spaces:", ConsoleColor.White, ConsoleColor.DarkYellow);
                    string[] anS = Console.ReadLine().Trim(' ').Split(' ');
                    for (int v = 0; v < anS.Length; v++)
                    {
                        bool end = false;
                        int chapterID = 0;
                        try
                        {
                            chapterID = int.Parse(anS[v]);
                            if (chapterID > 3) chapterID = 3;
                            else if (chapterID < 1) chapterID = 1;
                        }
                        catch
                        {
                            Console.WriteLine("Input string was not in the correct format");
                            end = true;
                        }
                        chapterID += MainChapterToEdit;
                        if (chapterID > 3)
                        {
                            chapterID++;
                        }
                        using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
                        int j = 0;
                        int id = 1;
                        int startPos = 2986;
                        int endPos = 4942;
                        // Loop through treasure data
                        for (int k = startPos; k <= endPos && !end; k += 4)
                        {
                            j++;
                            // If it's the end of a section of chapter data, increment the chapter id, and don't write anything
                            if (j % 49 == 0)
                            {
                                id++;
                            }
                            else if (j % 49 != 0)
                            {
                                // If the current chapter == the chapterID that you want to edit
                                if (id == chapterID)
                                {
                                    switch (MainChapterToEdit)
                                    {
                                        // Empire of Cats
                                        case 0:
                                            {
                                                // Loop through all of the types of treasures in Empire of Cats
                                                for (int g = 0; g < treasureLevels1[one].Length; g++)
                                                {
                                                    stream.Position = k - 4 + (treasureLevels1[one][g] * 4);
                                                    stream.WriteByte((byte)level);
                                                }
                                                end = true;
                                                break;
                                            }
                                        // Into the future
                                        case 3:
                                            {
                                                // Loop through all of the types of treasures in Into the Future
                                                for (int g = 0; g < treasureLevels2[two].Length; g++)
                                                {
                                                    stream.Position = k - 4 + (treasureLevels2[two][g] * 4);
                                                    stream.WriteByte((byte)level);
                                                }
                                                end = true;
                                                break;
                                            }
                                        // Cats of the Cosmos
                                        case 6:
                                            {
                                                // Loop through all of the types of treasures in Cats of the Cosmos
                                                for (int g = 0; g < treasureLevels3[three].Length; g++)
                                                {
                                                    stream.Position = k - 4 + (treasureLevels3[three][g] * 4);
                                                    stream.WriteByte((byte)level);
                                                }
                                                end = true;
                                                break;
                                            }
                                    }
                                }
                            }

                        }
                    }

                }
            }

        }
        static void TalentOrbs(string path)
        {
            using var stream2 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream2.Length;
            byte[] allData = new byte[length];
            stream2.Read(allData, 0, length);

            stream2.Close();

            // Talent orb data is between game version 9.5 and 9.7
            // Search for game version 9.5
            byte[] conditions = { 0x84, 0x61, 0x01 };
            int startPos = Search(path, conditions, true, allData.Length - 16)[0] - 2;

            // Search for game version 9.7
            byte[] conditions2 = { 0x4c, 0x62, 0x01 };
            int endPos = Search(path, conditions2, true, allData.Length - 16)[0];
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            // Number of orb slots in your data
            int orbCountTypes = allData[startPos + 4] * 3;

            // 155 total different orb types
            int totalNumOrbTypes = 155;
            int[] orbs = new int[totalNumOrbTypes];

            // Loop through your orbs and store what you have
            for (int i = 0; i < orbCountTypes; i += 3)
            {
                int orbID = allData[startPos + i + 9] - 1;
                orbs[orbID] = allData[startPos + i + 8];
            }

            string[] orbList = { "Red D attack", "Red C attack", "Red B attack", "Red A attack", "Red S attack", "Red D defense", "Red C defense", "Red B defense", "Red A defense", "Red S defense", "Floating D attack", "Floating C attack", "Floating B attack", "Floating A attack", "Floating S attack", "Floating D defense", "Floating C defense", "Floating B defense", "Floating A defense", "Floating S defense", "Black D attack", "Black C attack", "Black B attack", "Black A attack", "Black S attack", "Black D defense", "Black C defense", "Black B defense", "Black A defense", "Black S defense", "Metal D defense", "Metal C defense", "Metal B defense", "Metal A defense", "Metal S defense", "Angel D attack", "Angel C attack", "Angel B attack", "Angel A attack", "Angel S attack", "Angel D defense", "Angel C defense", "Angel B defense", "Angel A defense", "Angel S defense", "Alien D attack", "Alien C attack", "Alien B attack", "Alien A attack", "Alien S attack", "Alien D defense", "Alien C defense", "Alien B defense", "Alien A defense", "Alien S defense", "Zombie D attack", "Zombie C attack", "Zombie B attack", "Zombie A attack", "Zombie S attack", "Zombie D defense", "Zombie C defense", "Zombie B defense", "Zombie A defense", "Zombie S defense" };
            string[] orbTargets = { "Red", "Floating", "Black", "Metal", "Angel", "Alien", "Zombie" };
            string[] orbGrades = { "D", "C", "B", "A", "S" };
            string[] orbTypes = { "Strong", "Massive", "Tough", };

            List<string> orbS = new();

            orbS.AddRange(orbList);
            int length2 = orbS.Count;
            // Create other orb types for Strong, Massive, and Tough
            for (int i = 0; i < orbTargets.Length; i++)
            {
                // Metal only has defense up orbs
                if (orbTargets[i] != "Metal")
                {
                    for (int k = 0; k < orbTypes.Length; k++)
                    {
                        for (int l = 0; l < orbGrades.Length; l++)
                        {
                            orbS.Add(orbTargets[i] + " " + orbGrades[l] + " " + orbTypes[k]);
                        }
                    }
                }
            }
            Console.WriteLine("You have:");
            string toOutput = "";
            // Format string to output your current orbs
            for (int i = 0; i < orbs.Length; i++)
            {
                if (orbs[i] == 1)
                {
                    toOutput += "&" + orbs[i] + "& " + orbS[i] + " &orb\n&";
                }
                else if (orbs[i] > 1)
                {
                    toOutput += "&" + orbs[i] + "& " + orbS[i] + " &orbs\n&";
                }
            }
            ColouredText(toOutput, ConsoleColor.White, ConsoleColor.DarkYellow);
            List<byte> allDataList = new(allData);

            // Remove your orb data from save file
            allDataList.RemoveRange(startPos + 8, orbCountTypes);

            ColouredText("\n&What orbs do you want?(Enter the full name, in format - {&type&} {&letter&} {&attack&/&defense&/&strong&/&massive&/&tough&}, e.g &red d attack&, or &floating s defense&, note that for metal, &only defense up orbs exist&\nIf you want to edit multiple, enter 1 full" +
                " orb name and then another orb name, separated by and underscore, e.g, &red s strong&_&alien c tough&\nYou can also enter orb ids instead if you want to. You can enter &clear& if you want to remove all of your talent orbs\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            string input = Console.ReadLine();
            string[] orbNames = input.Split('_');

            List<int> ids = new();
            List<int> amounts = new();

            bool skip = false;
            // Clear orb storage
            if (orbNames[0].ToLower() == "clear")
            {
                skip = true;
                for (int i = 0; i < orbs.Length; i++)
                {
                    orbs[i] = 0;
                }
                ColouredText("Cleared all talent orbs from storage\n", ConsoleColor.White, ConsoleColor.Red);
            }
            // Orb data to insert into save file
            byte[] insert = new byte[totalNumOrbTypes * 3];
            // Loop through all orb types
            for (int i = 0; i < totalNumOrbTypes; i++)
            {
                // Set orb id
                insert[(i * 3) + 1] = (byte)(i + 1);
                // Set orb data to previous amounts
                insert[i * 3] = (byte)orbs[i];
            }
            // Loop through orbs entered by user
            for (int i = 0; i < orbNames.Length && !skip; i++)
            {
                // If the user entered an orb id instead of name
                try
                {
                    int id = int.Parse(orbNames[i]) - 1;
                    if (id > orbS.Count + 1)
                    {
                        Console.WriteLine("orb id is too large");
                    }
                    else
                    {
                        ids.Add(id);
                    }
                }
                // If the user entered name instead of an orb id
                catch
                {
                    // Humiliate user if they spelt angel wrong
                    if (orbNames[i].ToLower().Contains("angle"))
                    {
                        ColouredText("You put angle instead of angel, I assume you mean angel so it has been corrected\n\n", ConsoleColor.White, ConsoleColor.Red);
                        orbNames[i] = orbNames[i].ToLower();
                        orbNames[i] = orbNames[i].Replace("angle", "angel");
                    }
                    // Check if entered orb exists
                    if (!orbS.Exists(orb => orb.ToLower() == orbNames[i].ToLower()))
                    {
                        Console.WriteLine("Orb: " + orbNames[i] + " doesn't exist!");
                    }
                    else
                    {
                        // Add the id of the orb to a list
                        ids.Add(orbS.FindIndex(orb => orb.ToLower() == orbNames[i].ToLower()));
                    }
                }
            }
            // Loop through user entered orb ids
            for (int i = 0; i < ids.Count; i++)
            {
                ColouredText("&What amount of &" + orbS[ids[i]] + "& Orbs do you want to set?(max 255 per orb): ", ConsoleColor.White, ConsoleColor.DarkYellow);
                amounts.Add((int)Inputed());
                // Set orb amount
                insert[ids[i] * 3] = (byte)amounts[i];
            }
            // Set total number of orb types in save file
            allDataList[startPos + 4] = (byte)totalNumOrbTypes;
            // Insert modified orb data into save file
            allDataList.InsertRange(startPos + 8, insert);

            stream.Close();
            // Write data to save file
            File.WriteAllBytes(path, allDataList.ToArray());
        }

        static void CatFood(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            byte[] catfoodB = new byte[4];
            stream.Position = 7;
            stream.Read(catfoodB, 0, 4);

            int CatFood = BitConverter.ToInt16(catfoodB, 0);
            Console.WriteLine($"You have {CatFood} cat food");

            Console.WriteLine("How much cat food do you want?(max 45000, but I recommend below 20k, to be safe");
            
            CatFood = (int)Inputed();
            if (CatFood > 45000) CatFood = 45000;
            else if (CatFood < 0) CatFood = 0;

            byte[] bytes = Endian(CatFood);

            stream.Position = 7;
            stream.Write(bytes, 0, 2);
            Console.WriteLine("Set Cat food to " + CatFood);
        }

        static void XP(string path)
        {

            Console.WriteLine("How much XP do you want?(max 99999999)");
            int XP = (int)Inputed();
            if (XP > 99999999) XP = 99999999;
            else if (XP < 0) XP = 0;

            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            Console.WriteLine("Set XP to " + XP);

            byte[] bytes = Endian(XP);
            int startPos = 76;

            // If using jp, xp is stored 1 offset less
            if (gameVer == "jp")
            {
                startPos = 75;
            }
            stream.Position = startPos;
            stream.Write(bytes, 0, bytes.Length);
        }

        static void SepecTreasures(string path)
        {
            Console.WriteLine("What level of treasures of you want?(max 255) I would recommend going below 60 though, 1 = inferior, 2 = normal 3 = superior, anything above 3 just aplifies the treasure effect");
            int level = (int)Inputed();
            ColouredText("What chapter do you want to edit:\n&1.&Empire of Cats chapter 1\n&2.&Empire of Cats chapter 2\n&3.&Empire of Cats chapter 3\n&4.&Into the Future chapter 1\n&5.&Into the Future chapter 2\n&6.&Into the Future  chapter 3\n&7.&Cats of the Cosmos chapter 1\n&8.&Cats of the Cosmos chapter 2\n&9.&Cats of the Cosmos  chapter 3\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            int answer = (int)Inputed();
            if (level > 255) level = 255;
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            int j = 0;
            int id = 1;
            int startPos = 2986;
            int endPos = 4942;
            // Loop through treasure data
            for (int i = startPos; i <= endPos; i += 4)
            {
                j++;
                // If it's the end of the chapter treasure data, increment chapter id, and don't write anything
                if (j % 49 == 0)
                {
                    id++;
                }
                else if (j % 49 != 0)
                {
                    // If the current chapter is the one you want to edit
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
            Console.WriteLine("What level of treasures of you want?(max 255) I would recommend going below 60 though, if you actually want to damage aliens, you should set this to 4 or lower, 1=inferior, 2=normal, 3=superior");
            int level = (int)Inputed();
            if (level > 255) level = 255;
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            int j = 0;
            int startPos = 2986;
            int endPos = 4942;
            for (int i = startPos; i <= endPos; i += 4)
            {
                j++;
                // If it's not end of the chapter treasure data write data
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
            int[] occurrence = OccurrenceB(path);
            Console.WriteLine("What base level do you want to upgrade all of your cats to? (max 50) - plus levels will be specified later, enter -1 to keep the base level the same");
            int baseLev = (int)(Inputed() - 1);
            Console.WriteLine("What plus level do you want to upgrade all of your cats to? (max 90), enter -1, to keep plus level the same");
            int plusLev = (int)Inputed();
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            if (baseLev > 49) baseLev = 49;

            if (plusLev > 90) plusLev = 90;

            int pos = occurrence[1] + 1;
            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            // Loop through cat upgrade data
            for (int i = pos + 3; i <= pos + (catAmount * 4) - 2; i += 4)
            {
                // If base data wants to be modified
                if (baseLev > -1)
                {
                    stream.Position = i + 2;
                    stream.WriteByte(Convert.ToByte(baseLev));
                }
                // If plus data wants to be modified
                if (plusLev > -1)
                {
                    stream.Position = i;
                    stream.WriteByte(Convert.ToByte(plusLev));
                }
            }

            Console.WriteLine("Upgraded all cats to level " + (baseLev + 1) + " +" + plusLev);
            stream.Close();
            // Close rank up bundle menu offer thing popping up 100s of times
            Bundle(path);
        }
        static int CalculateUR(string path)
        {
            int[] occurrence = OccurrenceB(path);
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);
            bool repeat = true;
            int baseLev = 0;
            int plusLev = 0;

            for (int j = 9600; j <= 12083; j++)
            {
                if (allData[j] == 2 && repeat)
                {
                    repeat = false;
                    for (int i = j + 3; i <= j + (catAmount * 4) - 2; i += 4)
                    {
                        baseLev += allData[i + 2];
                        plusLev += allData[i];
                    }
                }
            }
            int ids = 0;
            for (int i = occurrence[0] + 4; i <= occurrence[1] - 12; i += 4)
            {
                ids += allData[i];
            }

            int pos = occurrence[2] + (catAmount * 4);
            stream.Position = pos;
            int[] ids2 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            int baseID = 0;
            int plusID = 0;
            for (int i = 0; i < ids2.Length; i++)
            {
                if (ids2[i] == 1)
                {
                    stream.Position = pos + (ids2[i] * 4);
                }
                else
                {
                    stream.Position = pos + ((ids2[i] + 1) * 4);
                }
                plusID += allData[stream.Position];
                stream.Position += 2;
                baseID += allData[stream.Position];
                stream.Position += 1;
            }
            return ids + baseLev + plusLev + plusID + baseID - 109;
        }
        static void Blue(string path)
        {
            int[] occurrence = OccurrenceB(path);
            Console.WriteLine("Do you want to upgrade all the blue upgrades at once? (yes/no)");
            string answer = Console.ReadLine();
            byte[] maxData = { 0x0A, 0x00, 0x13, 0x00, 0x0A, 0x00, 0x13, 0x00, 0x00, 0x00, 0x09, 0x00,
                0x0A, 0x00, 0x13, 0x00, 0x0A, 0x00, 0x13, 0x00, 0x0A, 0x00, 0x13, 0x00,
                0x0A, 0x00, 0x13, 0x00, 0x0A, 0x00, 0x13, 0x00, 0x0A, 0x00, 0x13, 0x00,
                0x0A, 0x00, 0x13, 0x00, 0x0A, 0x00, 0x13 };
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            // Individual upgrades
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
                int pos = occurrence[2] + (catAmount * 4);
                stream.Position = pos;

                // Loop through user entered ids
                for (int i = 0; i < ids.Length; i++)
                {
                    // For some reason there is a gap between the Cat cannon power and the rest of the upgrades
                    if (ids[i] == 1)
                    {
                        stream.Position = pos + (ids[i] * 4);
                    }
                    else
                    {
                        stream.Position = pos + ((ids[i] + 1) * 4);
                    }
                    stream.WriteByte((byte)idPlus[i]);
                    stream.Position++;
                    stream.WriteByte((byte)((byte)idBase[i] - 1));
                }

            }
            // All upgrades at once
            if (answer == "yes")
            {
                stream.Position = occurrence[2] + (catAmount * 4) + 4;
                stream.Write(maxData, 0, maxData.Length);
            }
            Console.WriteLine("Success");
        }

        static void Leadership(string path)
        {
            Console.WriteLine("How much leadership do you want(max 32767)");
            int Leadership = (int)Inputed();
            // Leadership is stored as a signed integer for some reason
            if (Leadership > 32767) Leadership = 32767;

            byte[] conditions = { 0x80, 0x38 };
            // Search for leadership position
            int pos = Search(path, conditions)[0];
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            Console.WriteLine("Scan Complete");
            byte[] bytes = Endian(Leadership);

            stream.Position = pos + 5;
            stream.Write(bytes, 0, 2);
            Console.WriteLine("Success");
        }

        static void NP(string path)
        {
            Console.WriteLine("How much NP do you want(max 65535)");
            int CatFood = (int)Inputed();
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
            if (!found) Console.WriteLine("Sorry your NP position couldn't be found\nYour save file is either invalid or the tool is bugged\nIf this is the case please tell me on discord\nThank you");
        }

        static void CatTicket(string path)
        {
            Console.WriteLine("How many Cat Tickets do you want(max 1999)");
            int catTickets = (int)Inputed();
            if (catTickets > 1999) catTickets = 1999;
            else if (catTickets < 0) catTickets = 0;
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

            stream2.Write(bytes, 0, 2);
        }

        static void CatTicketRare(string path)
        {
            Console.WriteLine("How many Rare Cat Tickets do you want(max 299)");
            int catTickets = (int)Inputed();
            if (catTickets > 299) catTickets = 299;
            else if (catTickets < 0) catTickets = 0;
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
            stream2.Write(bytes, 0, 2);
        }
        static void GamXP(string path)
        {
            Console.WriteLine("How much gamatoto xp do you want?\nLevel bounderies: https://battle-cats.fandom.com/wiki/Gamatoto_Expedition#Level-up_Requirements");

            long amount = Inputed();
            byte[] bytes = Endian(amount);
            int[] occurrence = OccurrenceB(path);

            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            bool found = false;
            int pos = occurrence[8] + (catAmount * 4) + 53;
            if (pos > 0)
            {
                found = true;
                stream.Position = pos;
            }
            stream.Write(bytes, 0, 4);
            if (found)
            {
                Console.WriteLine("Success");
            }
            if (!found) Console.WriteLine("Sorry your Catamin position couldn't be found\nYour save file is either invalid or the tool is bugged\nIf this is the case please tell me on discord\nThank you");

        }
        static int ThirtySix(string path)
        {
            byte[] conditions = { 0x36, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x36 };
            int pos = Search(path, conditions)[0];

            return pos;
        }
        // Search function for any number of conditions, instead of having to add more conditions to an if statement
        static int[] Search(string path, byte[] conditions, bool negative = false, int startpoint = 0, byte[] mult = null)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            if (mult == null)
            {
                mult = new byte[conditions.Length];
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
            for (int i = startpos; i < (allData.Length - conditions.Length); i += num)
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
                Console.WriteLine("Error, position couldn't be found, please report this to me on discord");
                Console.WriteLine("Press enter to continue");
                Console.ReadLine();
                Options();
                return values;
            }
        }
        static void GamHelp(string path)
        {
            ColouredText("What helpers do you want?&\n&Type numbers separated by spaces\nThe different helper ids are as follows:&\nIntern &1 - 53&\nLacky &54 - 83&\nUnderling &84 - 108&\nAssistant &109 - 128&\nLegend &129 - 148&\ne.g entering " +
                "&3 69 120 86 110 &would set your helpers to &1& intern, &1& lackey, &2& assistants, &1& underling\nThe ids must be different to eachother, the max helpers you can have is &10\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            string[] answer = Console.ReadLine().Split(' ');
            int[] answerInt = new int[answer.Length];
            try
            {
                // Convert ids into ints
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
            // Turn ids into byte array
            byte[] bytes = answerInt.SelectMany(BitConverter.GetBytes).ToArray();

            int pos = ThirtySix(path);
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
                // Format string to output what was edited
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
            if (!found) Console.WriteLine("Sorry your gamatoto helper position couldn't be found\nYour save file is either invalid or the tool is bugged\nIf this is the case please tell me on discord\nThank you");
        }
        static void PlatinumTickets(string path)
        {
            Console.WriteLine("How many Platinum Cat Tickets do you want(max 9)");
            byte platCatTickets = Convert.ToByte(Console.ReadLine());
            if (platCatTickets > 9) platCatTickets = 9;
            int pos = ThirtySix(path);

            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            bool found = false;
            if (pos > 0)
            {
                found = true;
            }
            stream.Position = pos + 16;
            stream.WriteByte(platCatTickets);
            if (found) Console.WriteLine("Success");
            if (!found) Console.WriteLine("Sorry your platinum cat ticket position couldn't be found\nYour save file is either invalid or the tool is bugged\nIf this is the case please tell me on discord\nThank you");

        }
        static void Outbreaks(string path)
        {
            // Length of first outbreak chunk
            int len = 237;
            byte[] toSearch = new byte[len];
            byte[] choice = new byte[len];
            // Generate search terms
            for (int i = 0; i < 47; i++)
            {
                toSearch[(i * 5) + 1] = (byte)(i + 1);
                choice[i * 5] = 1;
            }
            toSearch[236] = 0x01;
            choice[235] = 0x01;
            int pos = ThirtySix(path);

            bool found = false;
            int StartPos = 0;

            // Search for outbreak position
            int pos2 = Search(path, toSearch, false, pos, choice)[0];
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);
            if (pos2 > 0)
            {
                found = true;
                StartPos = pos2;
            }
            if (!found)
            {
                Console.WriteLine("Sorry your outbreak position couldn't be found\nYour save file is either invalid or the tool is bugged\nIf this is the case please tell me on discord\nThank you");
                return;
            }
            if (StartPos < 100)
            {
                Console.WriteLine("Sorry your outbreak position couldn't be found\nYour save file is either invalid or the tool is bugged\nIf this is the case please tell me on discord\nThank you");
                return;
            }
            for (int j = 0; j < length; j++)
            {
                stream.Position = StartPos + (j * 5);
                stream.WriteByte(1);
                // If it reaches the end of a chapter, skip forward to the next one and write some data
                if (allData[StartPos + (j * 5) + 10] == 0x30)
                {
                    StartPos += 5;
                    stream.Position = StartPos + (j * 5);
                    stream.WriteByte(1);
                    if (allData[StartPos + (j * 5) + 1] >= 0x07)
                    {
                        break;
                    }
                    StartPos += 8;
                }
                else if (allData[StartPos + (j * 5) + 13] >= 0x40)
                {
                    break;
                }
            }
            Console.WriteLine("Successfully cleared all zombie stages");
        }
        static void CatCannon(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);
            int pos = 0;

            // Search for cat cannon position
            for (int i = 0; i < allData.Length; i++)
            {
                if (allData[i] == 0 && allData[i + 1] == 0 && allData[i + 2] == 0 && allData[i + 3] == 8 && allData[i + 4] == 0 && allData[i + 5] == 0 && allData[i + 6] == 0 && allData[i + 7] == 0 && allData[i + 8] == 0 && allData[i + 9] == 0 && allData[i + 10] == 0 && allData[i + 11] == 2 && allData[i + 12] == 0 && allData[i + 13] == 0 && allData[i + 14] == 0 && allData[i + 15] == 3 && allData[i + 16] == 0 && allData[i + 17] == 0 && allData[i + 18] == 0)
                {
                    pos = i + 15;
                    break;
                }
            }
            ColouredText("What cat cannon type do you want to edit?:\n&1.& Imporve Base\n&2.& Develop Slow Beam\n&3.& Develop Iron Wall\n&4.& Develop Thunderbolt\n&5.& Develop Waterblast\n&6.& Develop Holy Blast\n&7.& Develop Breakerblast\n&8.& Develop Curseblast\n&You can edit multiple at once by entering multiple numbers separated by spaces\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            string[] answer = Console.ReadLine().Split(' ');
            for (int i = 0; i < answer.Length; i++)
            {
                int choice = int.Parse(answer[i]);
                int max = 0;

                // Set max upgrade amount
                switch (choice)
                {
                    case 1:
                        max = 20;
                        break;
                    case 2:
                        max = 30;
                        break;
                    case 3:
                        max = 30;
                        break;
                    case 4:
                        max = 30;
                        break;
                    case 5:
                        max = 20;
                        break;
                    case 6:
                        max = 25;
                        break;
                    case 7:
                        max = 30;
                        break;
                    case 8:
                        max = 30;
                        break;
                }
                // Position for the cat cannon is unlocked flag
                int unlockPos = pos + (16 * (choice - 1));
                stream.Position = unlockPos;
                stream.WriteByte(3);

                Console.WriteLine($"What level do you want for {choice}? (max {max})");
                int level = (int)Inputed();
                if (level == 0)
                {
                    stream.Position = unlockPos;
                    stream.WriteByte(0);
                }
                else if (level > max) level = max;
                else if (level < 1) level = 1;
                level -= 1;

                int levelPos = unlockPos + 4;
                stream.Position = levelPos;
                stream.WriteByte((byte)level);
            }
        }
        static void Engineers(string path)
        {
            Console.WriteLine("How many engineers do you want? (max 5)");
            int engineers = (int)Inputed();
            if (engineers > 5) engineers = 5;
            else if (engineers < 0) engineers = 0;
            byte engi = Convert.ToByte(engineers);
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            for (int i = 0; i < allData.Length; i++)
            {
                if (allData[i] == 0 && allData[i + 1] == 0 && allData[i + 2] == 0 && allData[i + 3] == 8 && allData[i + 4] == 0 && allData[i + 5] == 0 && allData[i + 6] == 0 && allData[i + 7] == 0 && allData[i + 8] == 0 && allData[i + 9] == 0 && allData[i + 10] == 0 && allData[i + 11] == 2 && allData[i + 12] == 0 && allData[i + 13] == 0 && allData[i + 14] == 0 && allData[i + 15] == 3 && allData[i + 16] == 0 && allData[i + 17] == 0 && allData[i + 18] == 0)
                {
                    stream.Position = i - 1;
                    stream.WriteByte(engi);
                    break;
                }
            }
            Console.WriteLine($"Set current amount of ototo engineers to {engineers}");
        }
        static void Seed(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            Console.WriteLine("Scan Complete");

            Console.WriteLine("What seed do you want?");
            long Seed = Inputed();
            if (Seed < 0) Seed = 0;
            byte[] bytes = Endian(Seed);

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
                Console.WriteLine("Sorry your seed position couldn't be found\nYour save file is either invalid or the tool is bugged\nIf this is the case please tell me on discord\nThank you");
                Options();
            }

            Console.WriteLine("Set gacha seed to: {0}", Seed);
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
                Console.WriteLine("Sorry your seed position couldn't be found\nYour save file is either invalid or the tool is bugged\nIf this is the case please tell me on discord\nThank you");
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
            int backVal = 300;
            // Different game version correction
            if (allData[occurrence[5] - backVal - 4] == 0x2c && allData[occurrence[5] - backVal - 3] == 0x01)
            {
                stream2.Position = occurrence[5] + 40;
            }
            else if (allData[occurrence[4] - backVal - 4] == 0x2c && allData[occurrence[4] - backVal - 3] == 0x01)
            {
                stream2.Position = occurrence[4] + 40;
            }
            else
            {
                Console.WriteLine("Error, your evolved cat position couldn't be found, please report this to me on discord");
            }

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
                try
                {
                    stream2.WriteByte((byte)form[t]);
                }
                catch
                {
                    stream2.WriteByte(0);
                }
                stream2.Position += 3;
                t++;
            }
        }
        static void Items(string path)
        {
            Console.WriteLine("\nHow many of each item do you want?(max 3999)");
            int Items = (int)Inputed();
            if (Items > 3999) Items = 3999;
            if (Items < 0) Items = 0;

            byte[] year = new byte[2];
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);


            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            byte[] bytes = Endian(Items);
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
            int platCatTickets = (int)Inputed();

            byte[] bytes = Endian(platCatTickets);

            int[] occurrence = OccurrenceB(path);

            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            bool found = false;
            int pos = occurrence[8] + (catAmount * 4) + 32;
            if (pos > 0)
            {
                found = true;
                stream.Position = pos;
            }
            for (int i = 0; i < 3; i++)
            {
                stream.Write(bytes, 0, 4);
            }
            if (found)
            {
                Console.WriteLine("Success");
            }
            if (!found) Console.WriteLine("Sorry your Catamin position couldn't be found\nYour save file is either invalid or the tool is bugged\nIf this is the case please tell me on discord\nThank you");
        }

        static void BaseMats(string path)
        {
            Console.WriteLine("How many Base Materials do you want(max 65535)");
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);
            int pos = 0;
            for (int i = 0; i < allData.Length; i++)
            {
                if (allData[i] == 0 && allData[i + 1] == 0 && allData[i + 2] == 0 && allData[i + 3] == 8 && allData[i + 4] == 0 && allData[i + 5] == 0 && allData[i + 6] == 0 && allData[i + 7] == 0 && allData[i + 8] == 0 && allData[i + 9] == 0 && allData[i + 10] == 0 && allData[i + 11] == 2 && allData[i + 12] == 0 && allData[i + 13] == 0 && allData[i + 14] == 0 && allData[i + 15] == 3 && allData[i + 16] == 0 && allData[i + 17] == 0 && allData[i + 18] == 0)
                {
                    pos = i - 46;
                    break;
                }
            }
            if (pos < 200)
            {
                Console.WriteLine("Your base mats position couldn't be found, please report this on the discord");
                return;
            }
            string[] types = { "Brick", "Feather", "Coal", "Sprocket", "Gold", "Meteorite", "Beast Bone", "Ammonite" };
            ColouredText("&What base material type do you want to edit, you can enter multiple ids separated by spaces&\n&1.& " +
                "Bricks\n&2.& Feathers\n&3.& Coal\n&4.& Sprockets\n&5.& Gold\n&6.& Meteorite\n&7.& Beast Bones\n&8.& Ammonite\n&9.& All materials at once\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            string[] answer = Console.ReadLine().Split(' ');
            for (int i = 0; i < answer.Length; i++)
            {
                int id = int.Parse(answer[i]);
                if (id == 9)
                {
                    Console.WriteLine("How much of each material do you want?(max 65535)");
                    int platCatTickets = (int)Inputed();

                    if (platCatTickets > 65535) platCatTickets = 65535;
                    else if (platCatTickets < 0) platCatTickets = 0;

                    byte[] bytes = Endian(platCatTickets);

                    for (int j = 0; j < 8; j++)
                    {
                        stream.Position = pos + (j * 4);
                        stream.Write(bytes, 0, 4);
                        ColouredText($"&Set current amount of &{types[j]}& to &{platCatTickets}&\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                    }
                }
                else
                {
                    id -= 1;

                    Console.WriteLine($"How much {types[id]} do you want?(max 65535)");
                    int platCatTickets = (int)Inputed();

                    if (platCatTickets > 65535) platCatTickets = 65535;
                    else if (platCatTickets < 0) platCatTickets = 0;

                    byte[] bytes = Endian(platCatTickets);

                    stream.Position = pos + (id * 4);
                    stream.Write(bytes, 0, 4);

                    ColouredText($"&Set current amount of &{types[id]}& to &{platCatTickets}&\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                }
            }
        }

        static void Catseyes(string path)
        {
            Console.WriteLine("How many catseyes of each type do you want(max 65535)");
            int platCatTickets = (int)Inputed();

            byte[] bytes = Endian(platCatTickets);

            int[] occurrence = OccurrenceB(path);

            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            bool found = false;
            int pos = occurrence[8] + (catAmount * 4) + 8;
            if (pos > 0)
            {
                found = true;
                stream.Position = pos;
            }
            for (int i = 0; i < 5; i++)
            {
                stream.Write(bytes, 0, 4);
            }
            if (found)
            {
                Console.WriteLine("Success");
            }
            if (!found) Console.WriteLine("Sorry your catseye position couldn't be found\nYour save file is either invalid or the tool is bugged\nIf this is the case please tell me on discord\nThank you");

        }
        static void RemCats(string path)
        {
            int[] occurrence = OccurrenceB(path);
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            for (int i = occurrence[0] + 4; i <= occurrence[1] - 4; i += 4)
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
            int ids = 0;
            for (int i = occurrence[0] + 4; i <= occurrence[1] - 4; i += 4)
            {
                if (ids != 542)
                {
                    stream.Position = i;
                    stream.WriteByte(Convert.ToByte(01));
                }
                ids++;
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

        static string PatchSaveFile(string choice, string path)
        {
            string name = Path.GetFileName(path);
            if (name.EndsWith(".pack") || name.EndsWith(".list") || name.EndsWith(".so") || name.EndsWith(".csv"))
            {
                return "";
            }

            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            byte[] toBeUsed = new byte[allData.Length - 32];
            for (int i = 0; i < allData.Length - 32; i++)
                toBeUsed[i] = allData[i];
            byte[] bytes = Encoding.ASCII.GetBytes("battlecats");
            if (choice != "jp")
            {
                bytes = Encoding.ASCII.GetBytes("battlecats" + choice);
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

        static long Inputed()
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

        static int[] OccurrenceB(string path)
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
                if (allData[i] == Convert.ToByte(Currentyear[0]) && allData[i + 1] == Convert.ToByte(Currentyear[1]) && allData[i + 2] == 0 && allData[i + 3] == 0)
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

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            Console.WriteLine("What is the cat id?, input multiple ids separated by spaces to evolve multiple cats ids must be 9 or above, normal cats cannot be evolved this way");
            string[] input = Console.ReadLine().Split(' ');

            try
            {
                // Game version correcting
                if (allData[occurrence[5] - 304] == 0x2c && allData[occurrence[5] - 303] == 0x01)
                {
                    stream.Position = occurrence[5] + 40;
                }
                else if (allData[occurrence[4] - 304] == 0x2c && allData[occurrence[4] - 303] == 0x01)
                {
                    stream.Position = occurrence[4] + 40;
                }
                else
                {
                    Console.WriteLine("Error, your evolved cat position couldn't be found, please report this to me on discord");
                }
            }
            catch { Console.WriteLine("You either haven't unlocked the ability to evolve cats or if you have - the tool is bugged and you should tell me on the discord"); return; }
            int[] form = EvolvedFormsGetter();
            int pos = (int)stream.Position;
            try
            {
                // Loop through all inputed ids
                for (int i = 0; i < input.Length; i++)
                {
                    int id = int.Parse(input[i]);
                    // Find its position
                    stream.Position = pos + (id - 9) * 4;
                    // Evolve
                    if (devolve == "1")
                    {
                        bool stay = false;
                        // If inputed id doesn't exist in cats.csv yet
                        if (id - 9 >= form.Length)
                        {
                            stay = true;
                            input[i] = "9";
                        }
                        if (form[id - 9] == 0 || stay)
                        {
                            Console.WriteLine("Does the cat need catfruit/catfruit seeds to evolve?(yes/no)");
                            string answer = Console.ReadLine().ToLower();
                            if (answer == "yes")
                            {
                                stream.WriteByte(2);
                            }
                            else
                            {
                                stream.WriteByte(1);
                            }
                        }
                        else
                        {
                            stream.WriteByte((byte)form[id - 9]);
                        }
                    }
                    // Devolve
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
                stream2.Position = occurrence[7] - 68;
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("You either can't evolve cats or the tool is bugged and if the tool is bugged then:\ntell me on discord\nThank you");
                Options();
            }
            byte[] catfruit = new byte[4];
            int[] FruitCat = new int[17];
            string[] fruits = { "Purple Seed", "Red Seed", "Blue Seed", "Green Seed", "Yellow Seed", "Purple Fruit", "Red Fruit", "Blue Fruit", "Green Fruit", "Yellow Fruit", "Epic Fruit", "Elder Seed", "Elder Fruit", "Epic Seed", "Gold Fruit", "Aku Seed", "Aku Fruit" };

            int j = 0;
            for (int i = occurrence[7] - 68; i < occurrence[7] - 3; i += 4)
            {
                catfruit[0] = allData[i];
                catfruit[1] = allData[i + 1];
                FruitCat[j] = BitConverter.ToInt32(catfruit, 0);
                j++;
            }
            ColouredText("&Total catfruit/seeds: &" + FruitCat.Sum() + "\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            ColouredText("&Do you want to edit all the cat fruits individually(&1&) or all at once? (&2&), (&1& or &2&)\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            string input = Console.ReadLine();

            if (input == "2")
            {
                ColouredText("&How many do you want?(max &28&)\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                int num = (int)Inputed();
                if (num > 32) num = 32;
                else if (num < 0) num = 0;

                byte[] bytes2 = Endian(num);

                for (int i = 0; i < 17; i++)
                {
                    int choice2 = i;
                    stream2.Position = occurrence[7] - 68 + ((choice2) * 4);
                    stream2.WriteByte(bytes2[0]);
                    stream2.WriteByte(bytes2[1]);
                    ColouredText("&Set &" + fruits[choice2] + "& to &" + num + "\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                }

            }
            else if (input == "1")
            {
                Console.WriteLine("Enter a number to edit that type of catfruit, enter multiple numbers separated by spaces to change multiple at a time");
                for (int i = 0; i < fruits.Length; i++)
                {
                    ColouredText("&" + (i + 1) + ".& " + fruits[i] + "&:& " + FruitCat[i] + "\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                }
                string[] enteredIDs = Console.ReadLine().Split(' ');
                for (int i = 0; i < enteredIDs.Length; i++)
                {
                    bool skip = false;
                    int choice = 0;
                    try
                    {
                        choice = int.Parse(enteredIDs[i]);
                    }
                    catch
                    {
                        skip = true;
                    }
                    if (!skip)
                    {
                        if (choice > 17) choice = 17;
                        else if (choice < 1) choice = 1;

                        ColouredText("&How many &" + fruits[choice - 1] + "s& do you want (max &256&)\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                        int amount = (int)Inputed();
                        if (amount > 256) amount = 256;
                        else if (amount < 0) amount = 0;

                        byte[] bytes = Endian(amount);

                        stream2.Position = occurrence[7] - 68 + ((choice - 1) * 4);
                        stream2.WriteByte(bytes[0]);
                        stream2.WriteByte(bytes[1]);
                    }
                }
            }

            ColouredText("&Have you finished editing cat fruits?(&yes&/&no&)\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            string answer = Console.ReadLine();
            stream2.Close();
            if (answer.ToLower() == "no") CatFruit(path);
        }
        static Dictionary<int, Tuple<string, int>> GetSkillData()
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
        static void AllTalents(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            int pos = 0;
            int len = 0;

            // Search for talent position
            for (int i = 0; i < allData.Length; i++)
            {
                if (allData[i] == 0x37 && allData[i - 1] == 0x00 && allData[i + 1] == 0x00 && allData[i + 225] == 0x4d && allData[i + 226] == 0x00)
                {
                    pos = i + 319;
                    len = allData[pos];
                    break;

                }
            }
            if (pos == 0)
            {
                Console.WriteLine("Error, your talent position couldn't be found, please report this to me on discord");
                return;
            }
            // Get all types of skills and their maxes
            Dictionary<int, Tuple<string, int>> data = GetSkillData();

            // Loop through talent data
            for (int i = pos + 4; i < pos + len * 48; i += 1)
            {
                // Cat id
                byte[] idData = { allData[i], allData[i + 1] };
                byte[] idB = Endian(BitConverter.ToInt16(idData, 0));
                int id = BitConverter.ToInt16(idB, 0);

                // Number of skills
                int len2 = allData[i + 4];
                // Loop through each skill and set it to its max value
                for (int j = 1; j <= len2; j++)
                {
                    int skillID = allData[i + (8 * j)];
                    int value = allData[i + 4 + (8 * j)];
                    stream.Position = i + 4 + (8 * j);
                    if (value > 10 || id > catAmount)
                    {
                        i = pos + len * 48;
                        break;
                    }
                    // If cameraman cat critical, max value is 1
                    if (id == 149 && skillID == 13)
                    {
                        stream.WriteByte(1);
                    }
                    // If Catasaurus critical, max value is 5 
                    else if (id == 46 && skillID == 13)
                    {
                        stream.WriteByte(5);
                    }
                    else
                    {
                        stream.WriteByte((byte)data[skillID].Item2);
                    }
                }
                // Move to the next cat
                i += (8 * len2) + 7;
            }
        }
        static void SpecificTalents(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            int pos = 0;
            int len = 0;

            // Search for talent data position
            for (int i = 0; i < allData.Length; i++)
            {
                if (allData[i] == 0x37 && allData[i - 1] == 0x00 && allData[i + 1] == 0x00 && allData[i + 225] == 0x4d && allData[i + 226] == 0x00)
                {
                    pos = i + 319;
                    len = allData[pos];
                    break;

                }
            }
            if (pos == 0)
            {
                Console.WriteLine("Error, your talent position couldn't be found, please report this to me on discord");
                return;
            }
            // Get all types of skills and their maxes
            Dictionary<int, Tuple<string, int>> data = GetSkillData();

            Console.WriteLine("Enter cat id(you can enter multiple ids separated by spaces to edit multiple cats at once):");
            string[] catIDsAnswer = Console.ReadLine().Split(' ');
            // Loop through each entered id
            for (int l = 0; l < catIDsAnswer.Length; l++)
            {
                int catID = int.Parse(catIDsAnswer[l]);
                bool found = false;
                // Loop through all talent data
                for (int i = pos + 4; i < pos + len * 48; i += 1)
                {
                    // Cat id
                    byte[] idData = { allData[i], allData[i + 1] };
                    byte[] idB = Endian(BitConverter.ToInt16(idData, 0));
                    int id = BitConverter.ToInt32(idB, 0);

                    // Number of skills
                    int len2 = allData[i + 4];
                    if (id == catID)
                    {
                        found = true;
                        Console.WriteLine("Do you want to max out the talent level for this cat,(1), or do you want to edit each skill individually(2)?:");
                        string choice = Console.ReadLine();
                        // Max
                        if (choice == "1")
                        {
                            // Loop through this cat's talent data and set it to max
                            for (int j = 1; j <= len2; j++)
                            {
                                int skillID = allData[i + (8 * j)];
                                int value = allData[i + 4 + (8 * j)];
                                stream.Position = i + 4 + (8 * j);
                                if (value > 10 || id > catAmount)
                                {
                                    i = pos + len * 48;
                                    break;
                                }
                                // If cameraman cat critical, max value is 1
                                if (id == 149 && skillID == 13)
                                {
                                    stream.WriteByte(1);
                                }
                                // If Catasaurus critical, max value is 5 
                                else if (id == 46 && skillID == 13)
                                {
                                    stream.WriteByte(5);
                                }
                                else
                                {
                                    stream.WriteByte((byte)data[skillID].Item2);
                                }

                            }
                        }
                        else if (choice == "2")
                        {
                            Dictionary<int, int> CatData = new();
                            // Loop through cat data and store what skills it has
                            for (int j = 1; j <= len2; j++)
                            {
                                int skillID = allData[i + (8 * j)];
                                CatData.Add(skillID, i + (8 * j));
                            }
                            int index = 1;
                            string[] skillsDesc = new string[5];
                            string[] skillsIDs = new string[5];
                            string[] skillsMax = new string[5];
                            // Find out the name, id and max of each of its skills
                            foreach (KeyValuePair<int, int> catSkillData in CatData)
                            {
                                int id2 = index;
                                string desc = data[catSkillData.Key].Item1;
                                skillsDesc[index - 1] = desc;
                                skillsIDs[index - 1] = catSkillData.Key.ToString();
                                skillsMax[index - 1] = data[catSkillData.Key].Item2.ToString();
                                ColouredText($"&{id2}.& {desc}\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                                index++;
                            }
                            Console.WriteLine("Enter skill id, you can enter multiple values separated by spaces:");
                            string[] ids = Console.ReadLine().Split(' ');
                            // Loop through entered ids
                            for (int k = 0; k < ids.Length; k++)
                            {
                                int idInt = int.Parse(ids[k]);
                                int realID = int.Parse(skillsIDs[idInt - 1]);
                                bool stop = false;
                                if (idInt > 5)
                                {
                                    Console.WriteLine("Error, skill id is too large");
                                    stop = true;
                                }
                                else if (idInt < 1)
                                {
                                    Console.WriteLine("Error, skill id is too small");
                                    stop = true;
                                }
                                if (!stop)
                                {
                                    int value = 0;
                                    // If cameraman cat critical, max value is 1
                                    if (id == 149 && realID == 13)
                                    {
                                        ColouredText($"&What do you want to set &{skillsDesc[idInt - 1]}& to? (max:1):", ConsoleColor.White, ConsoleColor.DarkYellow);
                                        value = (int)Inputed();
                                        if (value > 1) value = 1;
                                        else if (value < 0) value = 0;
                                    }
                                    // If Catasaurus critical, max value is 5 
                                    else if (id == 46 && realID == 13)
                                    {
                                        ColouredText($"&What do you want to set &{skillsDesc[idInt - 1]}& to? (max:5):", ConsoleColor.White, ConsoleColor.DarkYellow);
                                        value = (int)Inputed();
                                        if (value > 5) value = 5;
                                        else if (value < 0) value = 0;
                                    }
                                    else
                                    {
                                        ColouredText($"&What do you want to set &{skillsDesc[idInt - 1]}& to? (max:{skillsMax[idInt - 1]}):", ConsoleColor.White, ConsoleColor.DarkYellow);
                                        value = (int)Inputed();
                                        if (value > int.Parse(skillsMax[idInt - 1])) value = int.Parse(skillsMax[idInt - 1]);
                                        else if (value < 0) value = 0;
                                    }
                                    stream.Position = CatData[realID] + 4;
                                    stream.WriteByte((byte)value);
                                }
                            }

                        }
                    }
                    // Go to next cat
                    i += (8 * len2) + 7;
                }
                if (!found)
                {
                    Console.WriteLine("Error, this cat doesn't exist, or have any talents");
                }
            }
        }

        static void Talents(string path)
        {
            Console.WriteLine("Do you want to edit all talents at once?: (yes/no)");
            string answer = Console.ReadLine();
            if (answer.ToLower() == "yes")
            {
                AllTalents(path);
                return;
            }
            else if (answer.ToLower() == "no")
            {
                SpecificTalents(path);
                return;
            }
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

        static void Stage(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            ColouredText("What chapters do you want to complete?(1-9)\n1.&Empire of cats chapter 1&\n2.&Empire of cats chapter 2&\n3.&Empire of cats chapter 3&\n4.&Into the future chapter 1&\n5.&Into the future chapter 2&\n6.&Into the future chapter 3&\n7.&Cats of the cosmos chapter 1&" +
                "\n8.&Cats of the cosmos chapter 2&\n9.&Cats of the cosmos chapter 3&\n10.&All chapters&\n", ConsoleColor.White, ConsoleColor.Cyan);
            int choice = (int)Inputed();
            // Starting position of stage cleared flags
            int startPos = 946;
            // Length of each chapter's stage cleared flags, 16 0x00 bytes separate each chapter
            int blockLen = (47 * 4) + 16;
            // Position of total number of stages cleared
            int lvlCountPos = 906;
            // All chapters
            if (choice == 10)
            {
                // Set stages to be cleared
                for (int j = 0; j < 10; j++)
                {
                    for (int i = 0; i < 48; i++)
                    {
                        stream.Position = startPos + (i * 4);
                        stream.WriteByte(02);
                    }
                    startPos += blockLen;
                }
                // Set total number of stages cleared
                for (int i = 0; i < 9; i++)
                {
                    stream.Position = lvlCountPos + (i * 4);
                    stream.WriteByte(48);
                }
            }
            // Specific chapter
            else if (choice < 10)
            {
                if (choice > 3)
                {
                    choice++;
                }
                // Set start point to correct chapter
                startPos += (choice - 1) * blockLen;
                // Set stages to be cleared
                for (int i = 0; i < 48; i++)
                {
                    stream.Position = startPos + (i * 4);
                    stream.WriteByte(03);
                }
                // Set total number of stages cleared
                stream.Position = lvlCountPos + ((choice - 1) * 4);
                stream.WriteByte(48);
            }
            else
            {
                Console.WriteLine("Please enter a recognised number");
                stream.Close();
                Stage(path);
            }

        }

        static void Bundle(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);
            bool found = false;

            // Search for bundle counter position
            for (int i = 0; i < length; i++)
            {
                if (allData[i] == 0x31 && allData[i + 1] == 0 && allData[i + 2] == 0 && allData[i + 3] == 0 && allData[i + 4] == 0x32 && allData[i + 5] == 0 && allData[i + 6] == 0 && allData[i + 7] == 0 && allData[i + 8] == 0x33 && allData[i + 9] == 0 && allData[i + 10] == 0 && allData[i + 11] == 0)
                {
                    stream.Position = i - 4;
                    // Set total counter for bundle menus seen to 65535, stopping the game from opening any more
                    stream.WriteByte(0xff);
                    stream.WriteByte(0xff);
                    found = true;
                }
            }
            if (!found)
            {
                Console.WriteLine("Your bundle menu position couldn't be found, please contact me on discord or in #tool-help");
                return;
            }
            Console.WriteLine("Closed all bundle menus");

        }
        static void Slots(string path)
        {
            Console.WriteLine("How many slots do you want to have unlocked?(max 15):");
            int slots = (int)Inputed();
            if (slots > 15) slots = 15;
            else if (slots < 0) slots = 0;

            byte[] conditions = { 0x2c, 0x01, 0x00, 0x00 };
            // Search for slot position
            int pos = Search(path, conditions, false, 0)[1];

            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);

            stream.Position = pos - 5;
            stream.WriteByte((byte)slots);
            Console.WriteLine("Set unlocked slot amount to " + slots);
        }
        static void TimedScore(string path)
        {
            Console.WriteLine("What timed score do you want? (max 9999)");
            int score = (int)Inputed();
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
            bool found = false;
            // Search for timed score position
            for (int i = 0; i < allData.Length; i++)
            {
                if (allData[i] == 0x2D && allData[i + 1] == 0x0 && allData[i + 2] == 0x0 && allData[i + 3] == 0x0 && allData[i + 4] == 0x2E)
                {
                    for (int j = 1900; j < 2108; j++)
                    {
                        if (allData[i - j] == 09)
                        {
                            stream2.Position = i - j + 31;
                            found = true;
                        }
                    }
                }
            }
            if (!found)
            {
                Console.WriteLine("Sorry your timed score position couldn't be found please contact me on discord(linked in the readme)");
                return;
            }
            // Set each of the 3 ItF chapter's timed scores
            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 48; i++)
                {
                    stream2.WriteByte(scoreByte[0]);
                    stream2.WriteByte(scoreByte[1]);
                    stream2.Position += 2;
                }
                stream2.Position += 12;
            }
            Console.WriteLine("Set ItF timed score rewards to: " + score);

        }

        static int[] EvolvedFormsGetter()
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
            string[] f = new string[3];
            for (int i = 0; i < len; i++)
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
        static void SoL(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            int length = (int)stream.Length;
            byte[] allData = new byte[length];
            stream.Read(allData, 0, length);
            // Stars unlocked/unlock next chapter
            long unlock = 0;
            // Times beaten/graphical fix
            long levels = 0;
            // Levels beaten/unlock next chapter/levels
            long levsBeaten = 0;

            // Search for SoL positions
            for (int i = 0; i < allData.Length; i++)
            {
                if (allData[i] == 5 && allData[i + 1] == 0x2c && allData[i + 2] == 1 && allData[i + 3] == 4 && allData[i + 4] == 0x0c)
                {
                    levsBeaten = i + 6005;
                    levels = i + 12005;
                }
                else if (allData[i] == 0x2C && allData[i + 1] == 01 && allData[i + 2] == 0 && allData[i - 1] == 0 && allData[i + 3] == 0 && allData[i - 2] == 0 && allData[i - 3] == 0)
                {
                    unlock = i;
                    break;
                }
            }
            if (levels == 0 || unlock == 0 || stream.Position == 0)
            {
                Console.WriteLine("Sorry your SoL position couldn't be found, you are either using an old save or the editor is bugged - if that is the case please contact me on discord or in #tool-help");
                return;
            }
            ColouredText("&What subchapter do you want to edit?, enter an id starting at &1& = &Legend Begins&, &2& = &Passion land& etc, you can enter multiple ids seperated by spaces, e.g &1 5 4 7&, or you can enter 2 ids seperated by a &-& to edit a range of" +
                " chapters, e.g &1&-&7&, or you can enter &all& to edit all subchapters at once\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            string input = Console.ReadLine();
            int totalChapters = 49;
            List<int> chaptersToEdit = new();
            if (input.Contains("-"))
            {
                int start = int.Parse(input.Split('-')[0]);
                int end = int.Parse(input.Split('-')[1]);
                for (int i = start; i <= end; i++)
                {
                    chaptersToEdit.Add(i);
                }
            }
            else if (input.ToLower() == "all")
            {
                int start = 1;
                int end = totalChapters;
                for (int i = start; i < end; i++)
                {
                    chaptersToEdit.Add(i);
                }
            }
            else
            {
                string[] ids = input.Split(' ');
                int[] idsInt = Array.ConvertAll(ids, int.Parse);
                chaptersToEdit.AddRange(idsInt);
            }
            ColouredText("&Do you want to set all of the stars/crowns at the same time (&1&), or individually (&2&)?\n", ConsoleColor.White, ConsoleColor.DarkYellow);
            string sameOrIndividual = Console.ReadLine();
            int stars = 0;
            // Same
            if (sameOrIndividual == "1")
            {
                ColouredText("&How many stars/crowns do you want to complete for each chapter (&1&-&4&)\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                stars = (int)Inputed();
            }
            for (int i = 0; i < chaptersToEdit.Count; i++)
            {
                // Individual
                if (sameOrIndividual == "2")
                {
                    ColouredText($"&How many stars/crowns do you want to complete for chapter &{chaptersToEdit[i]}&? (&1&-&4&)\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                    stars = (int)Inputed();
                }
                // Levels beaten, required for next chapter to unlock
                int id = chaptersToEdit[i] - 1;
                stream.Position = levsBeaten + (id * 4);
                for (int j = 0; j < stars; j++)
                {
                    stream.WriteByte(8);
                }
                // Stars/crowns unlocked and required for next chapter to unlock
                stream.Position = unlock - 6152 + ((id + 1) * 4);
                stream.WriteByte(3);
                // Times stage has been beaten, required to avoid graphical issues
                stream.Position = levels + (id * 97) - id;
                long startpos = stream.Position;
                for (int j = 0; j < stars; j++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        stream.WriteByte(1);
                        stream.Position += 7;
                    }
                    stream.Position = startpos + (j * 2) + 2;
                }
            }
        }
    }
}
