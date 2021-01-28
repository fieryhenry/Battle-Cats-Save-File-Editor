using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Battle_Cats_save_editor
{

    class Editor
    {

        [STAThreadAttribute]
        static void Main()
        {
            Console.WindowWidth = 125;
            string folderName = @"newversion.txt";

            WebClient webClient = new WebClient();
            webClient.DownloadFile("https://raw.githubusercontent.com/fieryhenry/Battle-Cats-Save-File-Editor/main/version.txt", folderName);


            string[] lines = File.ReadAllLines(@"newversion.txt");
            string version = "2.9.7";

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
                string fileToOpen = FD.FileName;
                string path = Path.Combine(fileToOpen);
                string result = Path.GetFileName(path);

                Console.WriteLine("Save \"{0}\" is selected", result);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nBackup your save before using this editor!", fileToOpen);
                Console.ForegroundColor = ConsoleColor.White;
                ColouredText("\n&What do you want to do?&(Note many features are currently broken I am working on fixing them)", ConsoleColor.White, ConsoleColor.Magenta);
                ColouredText("\n&1.& Change Cat food\n&2.& Change XP\n&3.& Get all treasures\n&4.& All cats upgraded 40+80\n&5.& Change leadership\n&6.& Change NP\n&7.& Change cat tickets\n&8.& change rare cat tickets" +
                    "\n&9.& Change platinum tickets\n&10.& Change gacha seed\n&11.& All cats evolved\n&12.& Change battle item count\n&13.& Change Catamins" +
                    "\n&14.& Change base materials\n&15.& Change catseyes\n&16.& All cats\n&17.& Get a specific cat\n&18.& Upgrade a specific cat to a specific level\n" +
                    "&19.& change treasure level (game crashes when you enter the tresure menu but the effects of all those treasures are present)\n&20.& Patch Data\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                
                int Choice = Inputed();

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
                    case 20: Encrypt(path); break;
                    default: Console.WriteLine("Please input a number that is recognised"); break;
                }
                Console.WriteLine("Are you finished with the editor?");
                bool ChoiceExit = OnAskUser();
                if (ChoiceExit == false) Main();
                else
                {
                    Encrypt(path);
                    Console.WriteLine("Use the backup manager to restore the save\nPress enter to exit");
                    Console.ReadLine();
                }
            }
            else
            {
                ColouredText("\nPlease select your save\n\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                Main();
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
                for (int i = 9694; i <= 11878; i += 4)
                {
                    stream.Position = i;
                    stream.WriteByte(Convert.ToByte(80));
                }
                for (int i = 9686; i <= 11880; i += 4)
                {
                    stream.Position = i;
                    stream.WriteByte(Convert.ToByte(40));
                }

                Console.WriteLine("all cats max level");
            }

            static void Leadership(string path)
            {
                Console.WriteLine("How much leadership do you want(max 65535)");
                int CatFood = Inputed();
                Console.WriteLine("How much leadership do you have?");
                int leaderCurent = Convert.ToInt32(Console.ReadLine());
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);

                bool found = false;

                Console.WriteLine("Scan Complete");
                byte[] bytes = Endian(CatFood);
                byte[] bytesCurrent = Endian(leaderCurent);
                for (int j = 212992; j < length - 12; j++)
                {
                    if (allData[j] == Convert.ToByte(72) && allData[j + 1] == Convert.ToByte(57) && allData[j + 2] == Convert.ToByte(01) && allData[j + 3] == Convert.ToByte(00) && allData[j - 6] == bytesCurrent[0] && allData[j - 5] == bytesCurrent[1])
                    {

                        stream.Position = j - 6;
                        stream.WriteByte(bytes[0]);
                        stream.Position = j - 5;
                        stream.WriteByte(bytes[1]);

                        found = true;

                        Console.WriteLine("Success");
                    }

                }
                if (!found) Console.WriteLine("Sorry your leadership couldn't be found\nPlease upload your save onto the save editor discord linked in the readme.md of the github\nBecome a save donater and put it in #save-files in the discord\nThank you");
            }

            static void NP(string path)
            {
                Console.WriteLine("How much NP do you want(max 65535)");
                int CatFood = Inputed();
                Console.WriteLine("How much leadership do you have?");
                int leaderCurent = Convert.ToInt32(Console.ReadLine());
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);

                bool found = false;

                Console.WriteLine("Scan Complete");
                byte[] bytes = Endian(CatFood);
                byte[] bytesCurrent = Endian(leaderCurent);
                for (int j = 212992; j < length - 12; j++)
                {
                    if (allData[j] == Convert.ToByte(72) && allData[j + 1] == Convert.ToByte(57) && allData[j + 2] == Convert.ToByte(01) && allData[j + 3] == Convert.ToByte(00) && allData[j - 6] == bytesCurrent[0] && allData[j - 5] == bytesCurrent[1])
                    {
                        stream.Position = j - 16;
                        stream.WriteByte(bytes[0]);
                        stream.Position = j - 15;
                        stream.WriteByte(bytes[1]);

                        found = true;

                        Console.WriteLine("Success");
                    }

                }
                if (!found) Console.WriteLine("Sorry your NP position couldn't be found\nPlease upload your save onto the save editor discord linked in the readme.md of the github\nBecome a save donater and put it in #save-files in the discord\nThank you");
            }

            static void CatTicket(string path)
            {
                Console.WriteLine("How much Cat Tickets do you want(max 65535)");
                int catTickets = Inputed();
                Console.WriteLine("How much Normal Cat Tickets do you have?");
                int catTicketsCurrent = Convert.ToInt32(Console.ReadLine());
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);

                bool found = false;
                Console.WriteLine("Scan Complete");
                byte[] bytes = Endian(catTickets);
                byte[] bytesCurrent = Endian(catTicketsCurrent);
                for (int j = 0; j < length - 3; j++)
                {
                    if (allData[j] == Convert.ToByte(131) && allData[j + 3] == Convert.ToByte(0) && allData[j + 4] == Convert.ToByte(131) && allData[j + 7] == Convert.ToByte(00) && allData[j + 8] == Convert.ToByte(131) && allData[j + 11] == Convert.ToByte(0) && allData[j + 12] == Convert.ToByte(131) && allData[j + 15] == Convert.ToByte(0) && allData[j + 45] == Convert.ToByte(241) && allData[j + 44] == Convert.ToByte(132) && allData[j + 16] == Convert.ToByte(131) && allData[j + 73] == Convert.ToByte(02) && allData[j + 74] == Convert.ToByte(0) && allData[j + 64] == Convert.ToByte(bytesCurrent[0]) && allData[j + 65] == Convert.ToByte(bytesCurrent[1]))
                    {
                        found = true;
                        stream.Position = j + 64;
                        stream.WriteByte(bytes[0]);
                        stream.Position = j + 65;
                        stream.WriteByte(bytes[1]);
                        Console.WriteLine("Success");
                    }
                }
                if (!found)
                    Console.WriteLine("Sorry your cat ticket position couldn't be found\nPlease upload your save onto the save editor discord linked in the readme.md of the github\nBecome a save donater and put it in #save-files in the discord\nThank you");
            }

            static void CatTicketRare(string path)
            {
                Console.WriteLine("How much Rare Cat Tickets do you want(max 65535)");
                int catTickets = Inputed();
                Console.WriteLine("How much Normal Cat Tickets do you have?");
                int catTicketsCurrent = Convert.ToInt32(Console.ReadLine());
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);

                bool found = false;
                Console.WriteLine("Scan Complete");
                byte[] bytes = Endian(catTickets);
                byte[] bytesCurrent = Endian(catTicketsCurrent);
                for (int j = 0; j < length - 3; j++)
                {
                    if (allData[j] == Convert.ToByte(131) && allData[j + 3] == Convert.ToByte(0) && allData[j + 4] == Convert.ToByte(131) && allData[j + 7] == Convert.ToByte(00) && allData[j + 8] == Convert.ToByte(131) && allData[j + 11] == Convert.ToByte(0) && allData[j + 12] == Convert.ToByte(131) && allData[j + 15] == Convert.ToByte(0) && allData[j + 45] == Convert.ToByte(241) && allData[j + 44] == Convert.ToByte(132) && allData[j + 16] == Convert.ToByte(131) && allData[j + 73] == Convert.ToByte(02) && allData[j + 74] == Convert.ToByte(0) && allData[j + 64] == Convert.ToByte(bytesCurrent[0]) && allData[j + 65] == Convert.ToByte(bytesCurrent[1]))
                    {
                        found = true;
                        stream.Position = j + 68;
                        stream.WriteByte(bytes[0]);
                        stream.Position = j + 69;
                        stream.WriteByte(bytes[1]);
                        Console.WriteLine("Success");
                    }
                }
                if (!found) Console.WriteLine("Sorry your rare cat ticket position couldn't be found\nPlease upload your save onto the save editor discord linked in the readme.md of the github\nBecome a save donater and put it in #save-files in the discord\nThank you");
            }

            static void PlatTicketRare(string path)
            {
                Console.WriteLine("How much Platinum Cat Tickets do you want(max 9 - you'll get banned if you get more)");
                byte platCatTickets = Convert.ToByte(Console.ReadLine());
                Console.WriteLine("How many Platinum Cat Tickets do you have?");
                byte platCurrent = Convert.ToByte(Console.ReadLine());
                if (platCatTickets > 9) platCatTickets = 9;
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);

                bool found = false;

                Console.WriteLine("Scan Complete");
                for (int j = 0; j < length - 11; j++)
                {
                    if (allData[j] == Convert.ToByte(255) && allData[j + 1] == Convert.ToByte(255) && allData[j+2] == 0 && allData[j+3] == Convert.ToByte(54) && allData[j + 4] == Convert.ToByte(0) && allData[j + 5] == Convert.ToByte(0) && allData[j + 11] == Convert.ToByte(54) && allData[j + 12] == Convert.ToByte(0) && allData[j + 19] == Convert.ToByte(platCurrent))
                    {
                        found = true;
                        stream.Position = j + 19;
                        stream.WriteByte(platCatTickets);
                        Console.WriteLine("Success");
                    }
                }
                if (!found) Console.WriteLine("Sorry your rare cat ticket position couldn't be found\nPlease upload your save onto the save editor discord linked in the readme.md of the github\nBecome a save donater and put it in #save-files in the discord\nThank you");

            }

            static void Seed(string path)
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);

                Console.WriteLine("Scan Complete");
                bool found = false;
                Console.WriteLine("What seed do you want?(max 99999999), Curently broken for most save files");
                int XP = Inputed();
                if (XP > 99999999) XP = 99999999;
                byte[] bytes = Endian(XP);

                for (int j = 0; j < length - 1503; j++)
                {
                    if (allData[j] == Convert.ToByte(01) && allData[j + 5] == Convert.ToByte(228) && allData[j + 6] == Convert.ToByte(07) && allData[j + 9] == Convert.ToByte(11) && allData[j + 1] == Convert.ToByte(00) && allData[j + 2] == Convert.ToByte(00) && allData[j + 3] == Convert.ToByte(00) && allData[j + 4] == Convert.ToByte(00) && allData[j + 7] == Convert.ToByte(00))
                    {


                        char[] XPArr = { };
                        char[] XPArr0 = { '0' };

                        found = true;

                        stream.Position = j - 16;
                        stream.WriteByte(bytes[0]);
                        stream.Position = j - 15;
                        stream.WriteByte(bytes[1]);
                        stream.Position = j - 14;
                        stream.WriteByte(bytes[2]);
                        stream.Position = j - 13;
                        stream.WriteByte(bytes[3]);

                    }
                }
                if (!found)
                {
                    Console.WriteLine("Couldn't find value please enter your current seed (backup before doing this):");
                    int Current = (int)Convert.ToInt64(Console.ReadLine());

                    byte[] currentBytes = Endian(Current);


                    for (int j = 0; j < length - 3; j++)
                    {
                        if (allData[j] == currentBytes[0] && allData[j + 1] == currentBytes[1])
                        {
                            stream.Position = j;
                            stream.WriteByte(bytes[0]);
                            stream.Position = j + 1;
                            stream.WriteByte(bytes[1]);

                            Console.WriteLine("Success");
                        }
                    }
                }
            }

            static void Evolve(string path)
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);
                for (int i = 0; i <= length; i++)
                {

                }
                Console.WriteLine("Scan Complete");
                for (int j = 0; j < length - 25; j++)
                {
                    //Console.WriteLine(j);
                    if (allData[j] == Convert.ToByte(01) && allData[j + 1] == Convert.ToByte(01) && allData[j + 2] == Convert.ToByte(01) && allData[j + 3] == Convert.ToByte(01) && allData[j + 4] == Convert.ToByte(01) && allData[j + 5] == Convert.ToByte(01) && allData[j + 6] == Convert.ToByte(01) && allData[j + 7] == Convert.ToByte(00) && allData[j + 24] == Convert.ToByte(70) && allData[j + 25] == Convert.ToByte(02))
                    {
                        for (int i = j + 60; i < j + 2710; i += 4)
                        {
                            stream.Position = j;
                            stream.WriteByte(02);
                        }
                        Console.WriteLine("Success");
                    }
                }
            }

            static void Items(string path)
            {
                Console.WriteLine("How much of each item do you want(max 65535)");
                int catTickets = Inputed();
                Console.WriteLine("What is your base level for cat cannon attack power?");
                byte CatCannonAttack = Convert.ToByte(Console.ReadLine());
                Console.WriteLine("What is your amount of speed ups?");
                int SpeedUps = Inputed();
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);

                bool found = false;

                byte[] bytesItems = Endian(catTickets);
                byte[] bytesSpeedUps = Endian(SpeedUps);
                for (int j = 14080; j < 15104; j++)
                {
                    if (allData[j + 64] == Convert.ToByte(bytesSpeedUps[0]) && allData[j + 65] == Convert.ToByte(bytesSpeedUps[1]) && allData[j] == Convert.ToByte(02) && allData[j + 1] == Convert.ToByte(0) && allData[j + 2] == Convert.ToByte(0) && allData[j + 3] == Convert.ToByte(0) && allData[j - 54] == CatCannonAttack - 1 && allData[j + 4] == Convert.ToByte(02) && allData[j + 5] == 0 && allData[j - 58] == 0 && allData[j + 24] == 0) //&& allData[j + 28] == 8) 
                    {
                        for (int i = 64; i <= 84; i += 4)
                        {
                            found = true;
                            stream.Position = i + j;
                            stream.WriteByte(bytesItems[0]);
                            stream.Position = i + 1 + j;
                            stream.WriteByte(bytesItems[1]);
                        }
                    }
                }
                if (!found) Console.WriteLine("Sorry your item position couldn't be found\nPlease upload your save onto the save editor discord linked in the readme.md of the github\nBecome a save donater and put it in #save-files in the discord\nThank you");
            }

            static void Catamin(string path)
            {
                Console.WriteLine("How many Catimins of each type do you want(max 65535)");
                int platCatTickets = Inputed();
                Console.WriteLine("How many Catimin  of type A do you have?");
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
                if (!found) Console.WriteLine("Sorry your Catamin position couldn't be found\nPlease upload your save onto the save editor discord linked in the readme.md of the github\nBecome a save donater and put it in #save-files in the discord\nThank you");
            }

            static void BaseMats(string path)
            {
                Console.WriteLine("How many Base Materials do you want(max 65535)");
                int platCatTickets = (int)Convert.ToInt64(Console.ReadLine());
                Console.WriteLine("How many bricks do you have");
                byte catA = Convert.ToByte(Console.ReadLine());
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                int length = (int)stream.Length;
                byte[] bytes = Endian(platCatTickets);
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);
                bool found = false;

                Console.WriteLine("Scan Complete");
                for (int j = 0; j < allData.Length; j++)
                {
                    if (allData[j] == Convert.ToByte(01) && allData[j + 1] != Convert.ToByte(0) && allData[j + 3] == Convert.ToByte(0) && allData[j + 5] == Convert.ToByte(1) && allData[j + 10] == Convert.ToByte(1) && allData[j + 56] == Convert.ToByte(63) && allData[j + 64] == catA)
                    {
                        found = true;
                        for (int i = 0; i < 29; i+=4)
                        {
                            stream.Position = j + 64 + i;
                            stream.WriteByte(bytes[0]);
                            stream.Position = j + 65 + i;
                            stream.WriteByte(bytes[1]);
                        }
                    }
                }
                if (!found) Console.WriteLine("Sorry your base mats position couldn't be found\nPlease upload your save onto the save editor discord linked in the readme.md of the github\nBecome a save donater and put it in #save-files in the discord\nThank you");
            }

            static void Catseyes(string path)
            {
                Console.WriteLine("How many Catseyes do you want(max 65535)");
                int platCatTickets = Inputed();
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);

                Console.WriteLine("Scan Complete");
                bool found = false;
                byte[] bytes = Endian(platCatTickets);
                for (int j = 3; j < length - 80; j++)
                {
                    if (allData[j - 3] == Convert.ToByte(05) && allData[j + 20] == Convert.ToByte(03) && allData[j + 57] == Convert.ToByte(10) && allData[j + 71] == Convert.ToByte(27))
                    {
                        stream.Position = j;
                        stream.WriteByte(bytes[0]);
                        stream.Position = j + 1;
                        stream.WriteByte(bytes[1]);
                        stream.Position = j + 4;
                        stream.WriteByte(bytes[0]);
                        stream.Position = j + 5;
                        stream.WriteByte(bytes[1]);
                        stream.Position = j + 8;
                        stream.WriteByte(bytes[0]);
                        stream.Position = j + 9;
                        stream.WriteByte(bytes[1]);
                        stream.Position = j + 12;
                        stream.WriteByte(bytes[0]);
                        stream.Position = j + 13;
                        stream.WriteByte(bytes[1]);
                        stream.Position = j + 16;
                        stream.WriteByte(bytes[0]);
                        stream.Position = j + 17;
                        stream.WriteByte(bytes[1]);
                    }
                }
                if (!found)
                {
                    Console.WriteLine("Couldn't find value please enter your special catseye amount(backup before doing this):");
                    int CurrentSpec = (int)Convert.ToInt64(Console.ReadLine());
                    Console.WriteLine("Couldn't find value please enter your rare catseye amount(backup before doing this):");
                    int Currentrare = (int)Convert.ToInt64(Console.ReadLine());
                    Console.WriteLine("Couldn't find value please enter your super rare catseye amount(backup before doing this):");
                    int CurrentSuprare = (int)Convert.ToInt64(Console.ReadLine());
                    Console.WriteLine("Couldn't find value please enter your uber rare catseye amount(backup before doing this):");
                    int CurrentUberrare = (int)Convert.ToInt64(Console.ReadLine());
                    Console.WriteLine("Couldn't find value please enter your legend rare catseye amount(backup before doing this):");
                    int Currentlegrare = (int)Convert.ToInt64(Console.ReadLine());

                    byte[] currentspecBytes = Endian(CurrentSpec);
                    byte[] currentRareBytes = Endian(Currentrare);
                    byte[] currentSupRareBytes = Endian(CurrentSuprare);
                    byte[] currentUberRareBytes = Endian(CurrentUberrare);
                    byte[] currentlegBytes = Endian(Currentlegrare);


                    for (int j = 0; j < length - 3; j++)
                    {
                        if (allData[j] == currentspecBytes[0] && allData[j + 1] == currentspecBytes[1] && allData[j + 4] == currentRareBytes[0] && allData[j + 5] == currentRareBytes[1] && allData[j + 8] == currentSupRareBytes[0] && allData[j + 9] == currentSupRareBytes[1] && allData[j + 12] == currentUberRareBytes[0] && allData[j + 13] == currentUberRareBytes[1] && allData[j + 16] == currentlegBytes[0] && allData[j + 17] == currentlegBytes[1])
                        {
                            stream.Position = j;
                            stream.WriteByte(bytes[0]);
                            stream.Position = j + 1;
                            stream.WriteByte(bytes[1]);
                            stream.Position = j + 4;
                            stream.WriteByte(bytes[0]);
                            stream.Position = j + 5;
                            stream.WriteByte(bytes[1]);
                            stream.Position = j + 8;
                            stream.WriteByte(bytes[0]);
                            stream.Position = j + 9;
                            stream.WriteByte(bytes[1]);
                            stream.Position = j + 12;
                            stream.WriteByte(bytes[0]);
                            stream.Position = j + 13;
                            stream.WriteByte(bytes[1]);
                            stream.Position = j + 16;
                            stream.WriteByte(bytes[0]);
                            stream.Position = j + 17;
                            stream.WriteByte(bytes[1]);

                            Console.WriteLine("Success");
                        }
                    }
                }
            }

            static void Cats(string path)
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
                for (int i = 7362; i <= 9686; i += 4)
                {
                    stream.Position = i;
                    stream.WriteByte(Convert.ToByte(01));
                }

            }

            static void SpecifiCat(string path)
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                Console.WriteLine("What is the cat ID?");
                int catID = Inputed();

                int startPos = 7362;
                stream.Position = startPos + catID * 4;
                stream.WriteByte(01);

            }

            static void SpecifUpgrade(string path)
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                Console.WriteLine("What is the cat ID?");
                int catID = Inputed();
                int startPosID = 9694 + catID * 4;
                Console.WriteLine("What base level do you want?(max 40)");
                byte Levelbase = Convert.ToByte(Console.ReadLine());
                if (Levelbase > 40) Levelbase = 40;
                Console.WriteLine("What plus level do you want?(max +80)");
                byte Levelplus = Convert.ToByte(Console.ReadLine());
                if (Levelplus > 80) Levelplus = 80;

                stream.Position = startPosID;
                stream.WriteByte(Levelplus);
                Console.WriteLine("cat +" + Levelplus);

                stream.Position = startPosID + 2;
                stream.WriteByte(Levelbase);
                Console.WriteLine("cat level " + Levelbase);

            }

            static bool OnAskUser()
            {
                return DialogResult.Yes == MessageBox.Show(
                 "Finished with editor?", "Check for updates",
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
                byte[] bytes = Encoding.ASCII.GetBytes("battlecatsen");
                int test = 32 - bytes.Length;

                byte[] Usable = new byte[allData.Length - test];
                bytes.CopyTo(Usable, 0);
                toBeUsed.CopyTo(Usable, bytes.Length);


                var md5 = MD5.Create();

                byte[] Data = new byte[16];
                Data = md5.ComputeHash(Usable);

                string hex = ByteArrayToString(Data);
                Console.WriteLine(hex);

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
                    ColouredText("Input given was not an integer\n", ConsoleColor.White, ConsoleColor.DarkRed);
                    Main();
                }
                return input;
            }

        }
    }
}
    

