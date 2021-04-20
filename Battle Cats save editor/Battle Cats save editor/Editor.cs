using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Windows.Forms;

namespace Battle_Cats_save_editor
{

    class Editor
    {
        static int catAmount = 0;
        [STAThread]
        static void Main()
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

            WebClient webClient = new WebClient();
            webClient.DownloadFile("https://raw.githubusercontent.com/fieryhenry/Battle-Cats-Save-File-Editor/main/version.txt", @"newversion.txt");

            string[] lines = File.ReadAllLines(@"newversion.txt");
            string version = "2.17.0";

            if (lines[0] == version)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Application up to date - current version is {0}", version);
            }
            else
            {
                System.Diagnostics.Process.Start(@"Updater.exe");
                Environment.Exit(1);
            }
            var FD = new OpenFileDialog();
            if (FD.ShowDialog() == DialogResult.OK)
            {
                //Console.WriteLine("Enter the path to your save(use backslashes)");
                string fileToOpen = FD.FileName;
                //string fileToOpen = Console.ReadLine();
                string path = Path.Combine(fileToOpen);
                string result = Path.GetFileName(path);

                Console.WriteLine("Save \"{0}\" is selected", result);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nBackup your save before using this editor!\nIf you get an error along the lines of \"Your save is active somewhere else\"then select option 25 and select generate new account code\n", fileToOpen);
                Console.ForegroundColor = ConsoleColor.White;
                ColouredText("&What would you like to do?&\n&1.& Change Cat food\n&2.& Change XP\n&3.& Get all treasures\n&4.& All cats upgraded " +
                    "40+80\n&5.& Change leadership\n&6.& Change NP\n&7.& Change cat tickets\n&8.& change rare cat tickets" +
                    "\n&9.& Change platinum tickets\n&10.& Change gacha seed\n&11.& All cats evolved(you must first have unlocked the ability to " +
                    "evolve cats + you need to click the \"cycle\" icon on the bottom right of your cat)\n&12.& Change battle item count\n&13.& " +
                    "Change Catamins" +
                    "\n&14.& Change base materials\n&15.& Change catseyes(must have catseyes unlocked)\n&16.& All cats\n&17.& Get a specific cat" +
                    "\n&18.& Upgrade a specific cat to a specific level\n" +
                    "&19.& change treasure level (game crashes when you enter the tresure menu but the effects of all those treasures are present)" +
                    "\n&20.& Evolve a specific cat\n&21.& Change cat fruits and cat fruit seeds\n&22.& Talent upgrade cats(Must have NP unlocked)\n" +
                    "&23.& Clear story chapters\n&24.& Patch data\n&25.& More small edits and fixes\n&26.& Display current gacha seed\n&27.& Change all into the future timed score rewards\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                byte[] anchour = new byte[20];
                anchour[0] = Anchour(path);
                anchour[1] = 0x02;
                catAmount = BitConverter.ToInt32(anchour, 0);
                int Choice = Inputed();
                bool ChoiceExit = false;

                switch (Choice)
                {
                    case 1: CatFood(path); break;
                    case 2: XP(path); break;
                    case 3: Treasure(path); break;
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
                    case 19: MaxTreasures(path); break;
                    case 20: EvolveSpecific(path); break;
                    case 21: CatFruit(path); break;
                    case 22: Talents(path); break;
                    case 23: Stage(path); break;
                    case 24: Encrypt(path); Console.WriteLine("Use the backup manager to restore the save\nPress enter to exit"); Console.ReadLine(); Environment.Exit(0); break;
                    case 25: menu(path); break;
                    case 26: GetSeed(path); break;
                    case 27: TimedScore(path); break;
                    default: Console.WriteLine("Please input a number that is recognised"); break;
                }
                Console.WriteLine("Are you finished with the editor?");
                ChoiceExit = OnAskUser();
                if (ChoiceExit == false) Main();
                else
                {
                    Encrypt(path);
                    Console.WriteLine("Pess enter to exit");
                    Console.ReadLine();
                }
            }
            else
            {
                ColouredText("\nPlease select your save\n\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                Main();
            }

            static void menu(string path)
            {
                ColouredText("Welcome to the small patches and tweaks menu\n&1.&Close all the bundle menus (if you have used upgrade all cats, you know what this is)\n&2.&Generate new account to avoid error \"Your save is being used somewhere else\" Warning " +
                    "this will cause your gamototo to crash your game if entered and some things will be wiped, such as plat tickets, leadership, NP, however those can be added back after you re-save your data\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                int choice = Inputed();

                switch (choice)
                {
                    case 1: Bundle(path); break;
                    case 2: NewIQ(path); break;
                    default: Console.WriteLine("Please input a number that is recognised"); break;

                }
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

            static void Treasure(string path)
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
                for (int i = 2986; i <= 3566; i += 4)
                {
                    stream.Position = i;
                    stream.WriteByte(03);
                }
                for (int i = 3770; i <= 4942; i += 4)
                {
                    stream.Position = i;
                    stream.WriteByte(03);
                }
                Console.WriteLine("All Treasures");
            }

            static void MaxTreasures(string path)
            {
                Console.WriteLine("What level of treasures of you want?(max 255)");
                int level = Inputed();
                if (level > 255) level = 255;
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
                for (int i = 2986; i <= 3566; i += 4)
                {
                    stream.Position = i;
                    stream.WriteByte((byte)level);
                }
                for (int i = 3770; i <= 4942; i += 4)
                {
                    stream.Position = i;
                    stream.WriteByte((byte)level);
                }
                Console.WriteLine("All treasures level {0}", level);
            }

            static void CatUpgrades(string path)
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

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
                        for (int i = j + 3; i <= j + (catAmount * 4) - 40; i += 4)
                        {
                            stream.Position = i + 38;
                            stream.WriteByte(Convert.ToByte(39));
                            stream.Position = i + 40;
                            stream.WriteByte(Convert.ToByte(80));
                        }
                    }
                }
                stream.Close();
                Bundle(path);
            }

            static void Leadership(string path)
            {
                Console.WriteLine("How much leadership do you want(max 65535)");
                int CatFood = Inputed();
                ColouredText("&How much leadership do you have? (must have more than 0 or leadership &might& get corrupted and/or give you the wrong amount - high chance it won't but small chance it will)", ConsoleColor.White, ConsoleColor.Blue);
                int leaderCurent = Inputed();
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);

                bool found = false;

                Console.WriteLine("Scan Complete");
                byte[] bytes = Endian(CatFood);
                byte[] bytesCurrent = Endian(leaderCurent);

                int offset = 0;
                for (int j = 230496; j < length - 12; j++)
                {
                    if (allData[j] == Convert.ToByte(128) && allData[j + 1] == Convert.ToByte(56) && allData[j + 2] == Convert.ToByte(01) && allData[j + 3] == Convert.ToByte(00))
                    {
                        if (allData[j + 5] == bytesCurrent[0] && allData[j + 6] == bytesCurrent[1]) offset = 5;
                        else if (allData[j + 4] == bytesCurrent[0] && allData[j + 5] == bytesCurrent[1]) offset = 4;

                        stream.Position = j + offset;
                        stream.WriteByte(bytes[0]);
                        stream.Position = j + offset + 1;
                        stream.WriteByte(bytes[1]);
                        Console.WriteLine("Success");
                        found = true;
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
                for (int j = 230496; j < length - 12; j++)
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
                stream2.Position = occurrence[3] - 5;
                stream2.WriteByte(bytes[1]);
                stream2.WriteByte(bytes[0]);
            }

            static void PlatTicketRare(string path)
            {
                Console.WriteLine("How many Platinum Cat Tickets do you want(max 9 - you'll get banned if you get more)");
                byte platCatTickets = Convert.ToByte(Console.ReadLine());
                if (platCatTickets > 9) platCatTickets = 9;
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);

                bool found = false;

                Console.WriteLine("Scan Complete");
                for (int j = 0; j < length - 11; j++)
                {
                    if (allData[j] == 0 && allData[j + 1] == 0xC8 && allData[j + 2] == 0 && allData[j + 365] == 0x37)
                    {
                        if (allData[j - 23] == 0x36 && allData[j - 7] != 0xEC)
                        {
                            stream.Position = j - 7;
                            stream.WriteByte(platCatTickets);
                            found = true;
                        }
                        else if (allData[j - 311] == 0x36)
                        {
                            found = true;
                            stream.Position = j - 295;
                            stream.WriteByte(platCatTickets);
                        }
                        else if (allData[j - 31] == 0x36 && allData[j - 15] != 0xEC)
                        {
                            found = true;
                            stream.Position = j - 15;
                            stream.WriteByte(platCatTickets);
                        }
                        else if (allData[j - 39] == 0x36)
                        {
                            found = true;
                            stream.Position = j - 23;
                            stream.WriteByte(platCatTickets);
                        }
                        else if (allData[j - 79] == 0x36)
                        {
                            found = true;
                            stream.Position = j - 63;
                            stream.WriteByte(platCatTickets);
                        }
                    }
                }
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
                    Main();
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
                    Main();
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
                catch { Console.WriteLine("You either haven't unlocked the ability to evolve cats or if you have - it's bugged and you should tell me on the discord"); return; }
                int[] form = EvolvedFormsGetter();
                bool stop = false;
                int t = 0;
                int pos = (int)stream2.Position;
                while (stream2.Position < pos + (catAmount * 4) - 37 && !stop)
                {
                    for (int i = 0; i < 24; i++)
                    {
                        if (allData[stream2.Position + i] != 0x01 && allData[stream2.Position + i] != 0 && allData[stream2.Position + i] != 0x02)
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
                Console.WriteLine("How many of each item do you want?(max 65535)");
                int CatFood = Inputed();
                byte[] bytes = new byte[10];
                byte[] year = new byte[2];
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);


                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);

                bytes = Endian(CatFood);
                int[] occurrence = new int[100];

                try
                {
                    year[0] = allData[15];
                    year[1] = allData[16];
                    stream.Close();
                    occurrence = OccurrenceE(path, year);
                }
                catch
                {
                    stream.Close();
                }
                using var stream2 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                stream2.Position = occurrence[2] - 220;
                for (int i = occurrence[2] -220 ; i < occurrence[2] - 199; i+=4)
                {
                    stream2.Position = i;
                    stream2.WriteByte(bytes[0]);
                    stream2.WriteByte(bytes[1]);
                }

                stream2.Close();
            }

            static void Catamin(string path)
            {
                Console.WriteLine("How many Catimins of each type do you want(max 65535)");
                int platCatTickets = Inputed();
                Console.WriteLine("How many type A catamins do you have?");
                int CurrentplatCatTickets = Inputed();

                byte[] bytes = Endian(platCatTickets);
                byte[] bytesCurrent = Endian(CurrentplatCatTickets);

                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                bool found = false;

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);

                Console.WriteLine("Scan Complete");
                for (int j = 0; j < length - 59; j++)
                {
                    if (allData[j] == Convert.ToByte(05) && allData[j + 1] == Convert.ToByte(0) && allData[j + 69] == Convert.ToByte(10) && allData[j + 73] == Convert.ToByte(01) && allData[j + 83] == Convert.ToByte(27) && allData[j + 28] == Convert.ToByte(bytesCurrent[0]) && allData[j + 29] == Convert.ToByte(bytesCurrent[1]))
                    {
                        found = true;

                        stream.Position = j + 28;
                        stream.WriteByte(bytes[0]);
                        stream.Position = j + 29;
                        stream.WriteByte(bytes[1]);
                        stream.Position = j + 32;
                        stream.WriteByte(bytes[0]);
                        stream.Position = j + 33;
                        stream.WriteByte(bytes[1]);
                        stream.Position = j + 36;
                        stream.WriteByte(bytes[0]);
                        stream.Position = j + 37;
                        stream.WriteByte(bytes[1]);

                        Console.WriteLine(j);
                    }
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
                Console.WriteLine("How many Catseyes do you want(max 65535)");
                int platCatTickets = Inputed();
                if (platCatTickets > 65535) platCatTickets = 65535;
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);

                bool found = false;
                Console.WriteLine("Scan Complete");

                byte[] bytes = Endian(platCatTickets);

                stream.Close();

                int[] occurance = OccurrenceB(path);

                using var stream2 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);


                stream2.Position = occurance[7];

                for (int i = (int)(stream2.Position + 500); i < stream2.Position + 8000; i++)
                {
                    if (allData[i] == Convert.ToByte("0a", 16) && allData[i + 4] == 1 && allData[i + 5] == 1 && allData[i + 14] == Convert.ToByte("1b", 16) && !found)
                    {
                        found = true;
                        stream2.Position = i - 65;
                        for (int e = 0; e < 20; e += 4)
                        {
                            stream2.WriteByte(bytes[0]);
                            stream2.WriteByte(bytes[1]);
                            stream2.Position += 2;
                        }
                        Console.WriteLine("Found values");
                    }
                }
                if (!found) Console.WriteLine("Sorry your catseye position couldn't be found\nYour save file is either invalid or the tool is bugged\nIf this is the case please create a bug report on github or tell me on discord\nThank you");

            }

            static void Cats(string path)
            {
                int[] occurrence = OccurrenceB(path);
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
                for (int i = occurrence[0] + 4; i <= occurrence[1] - 12; i += 4)
                {
                    stream.Position = i;
                    stream.WriteByte(Convert.ToByte(01));
                }

            }

            static void SpecifiCat(string path)
            {
                int[] occurrence = OccurrenceB(path);
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                Console.WriteLine("What is the cat ID?");
                int catID = Inputed();

                int startPos = occurrence[0] + 4;
                stream.Position = startPos + catID * 4;
                stream.WriteByte(01);

            }

            static void SpecifUpgrade(string path)
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                Console.WriteLine("What is the cat ID?");
                int catID = Inputed();
                if (catID > 590) catID = 590;
                int startPosID = 9694 + catID * 4;
                Console.WriteLine("What base level do you want?(max 40)");
                byte Levelbase = Convert.ToByte(Console.ReadLine());
                if (Levelbase > 40) Levelbase = 40;
                Console.WriteLine("What plus level do you want?(max +80)");
                byte Levelplus = Convert.ToByte(Console.ReadLine());
                if (Levelplus > 80) Levelplus = 80;

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);
                bool repeat = true;

                for (int j = 9600; j <= 12000; j++)
                {
                    if (allData[j] == 2 && repeat)
                    {
                        startPosID = catID * 4;
                        startPosID += j;
                        repeat = false;

                        stream.Position = startPosID + 3;
                        stream.WriteByte(Convert.ToByte(Levelplus));
                        stream.Position = startPosID + 5;
                        stream.WriteByte(Convert.ToByte(Levelbase - 1));

                    }
                }
            }

            static bool OnAskUser()
            {
                return DialogResult.Yes == MessageBox.Show(
                 "Finished with editor?", "Finshed",
                 MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }

            static byte[] Endian(long num)
            {
                byte[] bytes = BitConverter.GetBytes(num);

                return bytes;
            }

            static void Encrypt(string path)
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);

                byte[] toBeUsed = new byte[allData.Length - 32];
                for (int i = 0; i < allData.Length - 32; i++)
                    toBeUsed[i] = allData[i];
                Console.WriteLine("What version of the game are you in (en or jp)?");
                string choice = Console.ReadLine();
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
                    Encrypt(path);
                    return;
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
                    Main();
                }
                catch (FormatException)
                {
                    ColouredText("Input given was not a number or it wasn't an integer\n", ConsoleColor.White, ConsoleColor.DarkRed);
                    Main();
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

                return occurrence;
            }

            static int[] OccurrenceBf(string path)
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);

                int amount = 0;
                int[] occurrence = new int[50];
                stream.Close();

                byte anchour = Anchour(path);
                for (int i = 0; i < allData.Length - 2; i++)
                {
                    if (allData[i] == anchour)
                        if (allData[i + 1] == 2 && allData[i + 2] == 0 && allData[i - 1] == 0 && allData[i - 64] == 15)
                        {
                            occurrence[amount] = i;
                            amount++;
                        }
                }

                return occurrence;
            }

            static int[] OccurrenceT(string path)
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);

                int amount = 0;
                int[] occurrence = new int[50];

                for (int i = 0; i < allData.Length - 1; i++)
                {
                    if (allData[i] == Convert.ToByte("48", 16) && allData[i + 1] == Convert.ToByte("39", 16))
                    {
                        if (allData[i - 2] == 0 || allData[i - 1] == 0)
                        {
                            occurrence[amount] = i;
                            amount++;
                        }
                    }
                }

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
                int[] occurrence = OccurrenceB(path);

                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                Console.WriteLine("What is the cat id?");
                int id = Convert.ToInt32(Console.ReadLine());
                int idPos = id * 4;

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);

                Console.WriteLine("Scan Complete");

                for (int i = occurrence[4] + 3; i < allData.Length; i++)
                {
                    if (allData[i] == 1 || allData[i] == 2)
                    {
                        stream.Position = i + idPos;
                        i = allData.Length;
                    }
                }
                stream.WriteByte(2);
                stream.Close();

            }
            static void NewIQ(string path)
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
                    stream2.Position = occurrence[4] + 2397;
                }
                catch { Console.WriteLine("You either haven't unlocked the ability to evolve cats or if you have - it's bugged and you should tell me on the discord"); return; }
                stream2.WriteByte(0xC9);
                stream2.Position += 3;
                stream2.WriteByte(0x0D);
                stream2.Position += 3;
                stream2.WriteByte(0x05);
                Console.WriteLine("New inquiry code generated, you should now be able to use the backupmanager to restore the save");
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
                    Console.WriteLine("You either can't evolve cats or it's bugged and if it's bugged then:\nYour save file is either invalid or the tool is bugged\nIf this is the case please create a bug report on github or tell me on discord\nThank you");
                    Main();
                }
                byte[] catfruit = new byte[4];
                int[] FruitCat = new int[15];
                string[] fruits = { "Purple Seed", "Red Seed", "Blue Seed", "Green Seed", "Yellow Seed", "Purple Fruit", "Red Fruit", "Blue Fruit", "Green Fruit", "Yellow Fruit", "Epic Fruit", "Elder Seed", "Elder Fruit", "Epic Seed", "Gold Fruit" };
                int j = 0;
                for (int i = occurrence[6] - 60; i < occurrence[6] - 3; i+=4)
                {
                    catfruit[0] = allData[i];
                    catfruit[1] = allData[i+1];
                    FruitCat[j] = BitConverter.ToInt32(catfruit, 0);
                    j++;
                }
                Console.WriteLine("Total catfruit/seeds: " + FruitCat.Sum() + "\nWhat do you want to edit (1-15)");
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
                stream2.Position = (occurrence[6] - 60) + ((choice -1)* 4);
                stream2.WriteByte(bytes[0]);
                stream2.WriteByte(bytes[1]);
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
                    Console.WriteLine("You either havn't unlocked NP or it's bugged and if it's bugged then:\nYour save file is either invalid or the tool is bugged\nIf this is the case please create a bug report on github or tell me on discord\nThank you");
                    Main();
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
                Encrypt(path);
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
                string[] first = new string[600];
                string[] second = new string[600];
                int[] form = new int[600];
                string[] f = new string[3];
                for (int i = 0; i < 600; i++)
                {
                    f = listA[i].Split('/');
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
        }
    }
}
