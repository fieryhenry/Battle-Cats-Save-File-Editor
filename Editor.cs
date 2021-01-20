using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Battle_Cats_save_editor
{

    class Editor
    {

        [STAThreadAttribute]
        static void Main()
        {
            string folderName = @"newversion.txt";

            WebClient webClient = new WebClient();
            webClient.DownloadFile("https://raw.githubusercontent.com/fieryhenry/Battle-Cats-Save-File-Editor/main/version.txt", folderName);


            string[] lines = System.IO.File.ReadAllLines(@"newversion.txt");

            if (lines[0] == "2.8.8")
            {
                Console.WriteLine("Application up to date");
            }
            else
            {
                System.Diagnostics.Process.Start(@"Updater.exe");
                System.Environment.Exit(1);

            }

            var FD = new System.Windows.Forms.OpenFileDialog();
            if (FD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileToOpen = FD.FileName;

                string path = Path.Combine(fileToOpen);
                Console.WriteLine("Backup your save before using this editor!\n");
                Console.WriteLine("\nWhat do you want to do?\n1. Change Cat food\n2. Change XP\n3. All treasures\n4. All cats upgraded 40+80\n5. Change leadership\n6. Change NP\n7. Change cat tickets\n8. change rare cat tickets" +
                    "\n9. Change platinum tickets\n10. All cats from clearing stages\n11. Change gacha seed\n12. All cats evolved\n13. Change battle item count\n14. Change Catamins" +
                    "\n15. Change base materials\n16. Change catseyes\n17. All cats\n18. Get a specific cat\n19. Upgrade a specific cat to a specific level\n20. Patch Data");
                int Choice = Convert.ToInt32(Console.ReadLine());


                switch (Choice)
                {
                    case 1:
                        CatFood(path);
                        break;
                    case 2:
                        XP(path);
                        break;
                    case 3:
                        Treasure(path);
                        break;
                    case 4:
                        CatUpgrades(path);
                        break;
                    case 5:
                        Leadership(path);
                        break;
                    case 6:
                        NP(path);
                        break;
                    case 7:
                        CatTicket(path);
                        break;
                    case 8:
                        CatTicketRare(path);
                        break;
                    case 9:
                        PlatTicketRare(path);
                        break;
                    case 10:
                        ClearStageCat(path);
                        break;
                    case 11:
                        Seed(path);
                        break;
                    case 12:
                        Evolve(path);
                        break;
                    case 13:
                        Items(path);
                        break;
                    case 14:
                        Catamin(path);
                        break;
                    case 15:
                        BaseMats(path);
                        break;
                    case 16:
                        Catseyes(path);
                        break;
                    case 17:
                        Cats(path);
                        break;
                    case 18:
                        SpecifiCat(path);
                        break;
                    case 19:
                        SpecifUpgrade(path);
                        break;
                    case 20:
                        Encrypt(path);
                        break;
                    default:
                        Console.WriteLine("Please input a number that is recognised");
                        break;

                }
                Console.WriteLine("Are you finished with the editor?");
                bool ChoiceExit = OnAskUser();
                if (ChoiceExit == false)
                {
                    Main();
                }
                else
                {
                    Encrypt(path);
                    Console.ReadLine();
                }
            }

            static void CatFood(string path)
            {
                Console.WriteLine("How much cat food do you want?(max 45000)");
                int CatFood = Convert.ToInt32(Console.ReadLine());
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
                int XP = Convert.ToInt32(Console.ReadLine());
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
                    Console.WriteLine("All Treasures");
                }
            }

            static void CatUpgrades(string path)
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
                for (int i = 9694; i <= 11878; i += 4)
                {
                    stream.Position = i;
                    stream.WriteByte(Convert.ToByte(80));
                    //Console.WriteLine("All cats +80");
                }
                for (int i = 9686; i <= 11880; i += 4)
                {
                    stream.Position = i;
                    stream.WriteByte(Convert.ToByte(40));
                    //Console.WriteLine("All cats level 40+80");
                }

                Console.WriteLine("all cats max level");
            }

            static void Leadership(string path)
            {
                Console.WriteLine("How much leadership do you want(max 65535)");
                int CatFood = Convert.ToInt32(Console.ReadLine());
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);


                char[] CatArrL = { };
                char[] CatArr0L = { '0' };

                bool found = false;

                Console.WriteLine("Scan Complete");
                byte[] bytes = Endian(CatFood);
                for (int j = 0; j < length -12; j++)
                {
                    //Console.WriteLine(j);
                    if (allData[j] == Convert.ToByte(128) && allData[j + 1] == Convert.ToByte(56) && allData[j + 2] == Convert.ToByte(01) && allData[j + 3] == Convert.ToByte(00) && allData[j + 4] == Convert.ToByte(00) && allData[j + 11] == Convert.ToByte(72) && allData[j + 12] == Convert.ToByte(57))
                    { 

                        stream.Position = j + 5;
                        stream.WriteByte(bytes[0]);
                        stream.Position = j + 6;
                        stream.WriteByte(bytes[1]);

                        found = true;


                        Console.WriteLine("Success");
                    }
                    
                }
                if (!found)
                {
                    Console.WriteLine("Couldn't find value please enter your current leadership amount(backup before doing this):");
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

            static void NP(string path)
            {
                Console.WriteLine("How much NP do you want(max 65535)");
                int CatFood = Convert.ToInt32(Console.ReadLine());
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);

                char[] CatArrL = { };
                char[] CatArr0L = { '0' };

                bool found = false;
                byte[] bytes = Endian(CatFood);

                Console.WriteLine("Scan Complete");
                for (int j = 0; j < length -12; j++)
                {
                    //Console.WriteLine(j);
                    if (allData[j] == Convert.ToByte(128) && allData[j + 1] == Convert.ToByte(56) && allData[j + 2] == Convert.ToByte(01) && allData[j + 3] == Convert.ToByte(00) && allData[j + 4] == Convert.ToByte(00) && allData[j + 11] == Convert.ToByte(72) && allData[j + 12] == Convert.ToByte(57))
                    {

                        stream.Position = j - 5;
                        stream.WriteByte(bytes[0]);
                        stream.Position = j - 4;
                        stream.WriteByte(bytes[1]);

                        found = true;
                        Console.WriteLine("Success");
                    }
                }
                if (!found)
                {
                    Console.WriteLine("Couldn't find value please enter your current NP amount(backup before doing this):");
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

            static void CatTicket(string path)
            {
                Console.WriteLine("How much Cat Tickets do you want(max 65565)");
                int catTickets = Convert.ToInt32(Console.ReadLine());
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);
                for (int i = 0; i <= length; i++)
                {

                }
                Console.WriteLine("Scan Complete");
                bool found = false;
                byte[] bytes = Endian(catTickets);
                for (int j = 0; j < length - 3; j++)
                {
                    //Console.WriteLine(j);
                    if (allData[j] == Convert.ToByte(131) && allData[j + 1] == Convert.ToByte(142) && allData[j + 2] == Convert.ToByte(123) && allData[j + 3] == Convert.ToByte(00) && allData[j - 2] == Convert.ToByte(122) && allData[j - 3] == Convert.ToByte(142))
                    {

                        found = true;
                        stream.Position = j + 24;
                        stream.WriteByte(bytes[0]);
                        stream.Position = j + 23;
                        stream.WriteByte(bytes[1]);
                        Console.WriteLine("Success");
                    }
                }
                if (!found)
                {
                    Console.WriteLine("Couldn't find value please enter your current Catticket amount(backup before doing this):");
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

            static void CatTicketRare(string path)
            {
                Console.WriteLine("How much Rare Cat Tickets do you want(max 65535)");
                int rareCatTickets = Convert.ToInt32(Console.ReadLine());
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);
                for (int i = 0; i <= length; i++)
                {

                }
                Console.WriteLine("Scan Complete");

                byte[] bytes = Endian(rareCatTickets);
                bool found = false;
                for (int j = 0; j < length -3; j++)
                {
                    //Console.WriteLine(j);
                    if (allData[j] == Convert.ToByte(131) && allData[j + 1] == Convert.ToByte(142) && allData[j + 2] == Convert.ToByte(123) && allData[j + 3] == Convert.ToByte(00) && allData[j - 2] == Convert.ToByte(122) && allData[j + -3] == Convert.ToByte(142))
                    {

                        found = true;
                        stream.Position = j + 28;
                        stream.WriteByte(bytes[0]);
                        stream.Position = j + 27;
                        stream.WriteByte(bytes[1]);
                        Console.WriteLine("Success");
                    }
                }
                if (!found)
                {
                    Console.WriteLine("Couldn't find value please enter your current Rare caticket amount (backup before doing this):");
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

            static void PlatTicketRare(string path)
            {
                Console.WriteLine("How much Platinum Cat Tickets do you want(max 9 any more and ban)");
                byte platCatTickets = Convert.ToByte(Console.ReadLine());
                if (platCatTickets > 9) platCatTickets = 9;
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);
                for (int i = 0; i <= length; i++)
                {

                }
                Console.WriteLine("Scan Complete");
                for (int j = 0; j < length - 8; j++)
                {
                    //Console.WriteLine(j);
                    if (allData[j] == Convert.ToByte(255) && allData[j + 1] == Convert.ToByte(255) && allData[j + 2] == Convert.ToByte(255) && allData[j + 3] == Convert.ToByte(255) && allData[j + 4] == Convert.ToByte(255) && allData[j + 5] == Convert.ToByte(255) && allData[j + 6] == Convert.ToByte(255) && allData[j + 7] == Convert.ToByte(00) && allData[j + 8] == Convert.ToByte(54))
                    {
                        stream.Position = j + 24;
                        stream.WriteByte(platCatTickets);
                        Console.WriteLine("Success");
                    }
                }
                
            }

            static void ClearStageCat(string path)
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);
                for (int i = 0; i <= length; i++)
                {

                }
                Console.WriteLine("Scan Complete");
                for (int j = 0; j < length - 1503; j++)
                {
                    if (allData[j] == Convert.ToByte(00) && allData[j + 2] == Convert.ToByte(00) && allData[j + 3] == Convert.ToByte(00) && allData[j + 4] == Convert.ToByte(00) && allData[j + 6] == Convert.ToByte(00) && allData[j + 7] == Convert.ToByte(00) && allData[j + 8] == Convert.ToByte(00) && allData[j + 10] == Convert.ToByte(00) && allData[j + 145] == Convert.ToByte(44) && allData[j + 146] == Convert.ToByte(01) && allData[j + 1503] == 250)
                    {
                        Console.WriteLine("Success");
                        for (int i = 0; i <= j + 482; i += 4)
                        {
                            stream.Position = i;
                            stream.WriteByte(01);
                        }
                    }
                }

            }

            static void Seed(string path)
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);
                for (int i = 0; i <= length; i++)
                {

                }
                Console.WriteLine("Scan Complete");
                bool found = false;
                Console.WriteLine("What seed do you want?(max 99999999)");
                int XP = (int)Convert.ToInt64(Console.ReadLine());
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
                Console.WriteLine("How much of each item do you want(max 255)");
                byte catTickets = Convert.ToByte(Console.ReadLine());
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);

                Console.WriteLine("Scan Complete");
                stream.Position = 14474;
                stream.WriteByte(catTickets);
                stream.Position = 14478;
                stream.WriteByte(catTickets);
                stream.Position = 14482;
                stream.WriteByte(catTickets);
                stream.Position = 14486;
                stream.WriteByte(catTickets);
                stream.Position = 14490;
                stream.WriteByte(catTickets);
                stream.Position = 14494;
                stream.WriteByte(catTickets);
            }

            static void Catamin(string path)
            {
                Console.WriteLine("How many Catimins do you want(Backup before doing this)(max 255)");
                byte platCatTickets = Convert.ToByte(Console.ReadLine());
                Console.WriteLine("How many Catimin A do you have");
                byte catA = Convert.ToByte(Console.ReadLine());
                Console.WriteLine("How many Catimin B do you have");
                byte catB = Convert.ToByte(Console.ReadLine());
                Console.WriteLine("How many Catimin C do you have");
                byte catC = Convert.ToByte(Console.ReadLine());
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);

                Console.WriteLine("Scan Complete");
                for (int j = 0; j < length - 59; j++)
                {
                    //Console.WriteLine(j);
                    if (allData[j] == Convert.ToByte(03) && allData[j + 4] == catA && allData[j + 8] == catB && allData[j + 12] == catC)
                    {
                        stream.Position = j + 4;
                        stream.WriteByte(platCatTickets);
                        stream.Position = j + 8;
                        stream.WriteByte(platCatTickets);
                        stream.Position = j + 12;
                        stream.WriteByte(platCatTickets);
                        Console.WriteLine("Success");
                    }
                }
            }

            static void BaseMats(string path)
            {
                Console.WriteLine("How many Base Materials do you want(max 255)");
                byte platCatTickets = Convert.ToByte(Console.ReadLine());
                Console.WriteLine("How many bricks do you have");
                byte catA = Convert.ToByte(Console.ReadLine());
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);

                Console.WriteLine("Scan Complete");
                for (int j = 0; j < length -194; j++)
                {
                    //Console.WriteLine(j);
                    if (allData[j] == Convert.ToByte(02) && allData[j + 4] == Convert.ToByte(03) && allData[j + 12] == Convert.ToByte(01) && allData[j + 16] == Convert.ToByte(02) && allData[j + 178] == Convert.ToByte(64) && allData[j + 194] == Convert.ToByte(65) && allData[j + -61] == Convert.ToByte(08) && allData[j - 57] == catA)
                    {
                        stream.Position = j - 57;
                        stream.WriteByte(platCatTickets);
                        stream.Position = j - 53;
                        stream.WriteByte(platCatTickets);
                        stream.Position = j - 49;
                        stream.WriteByte(platCatTickets);
                        stream.Position = j - 45;
                        stream.WriteByte(platCatTickets);
                        stream.Position = j - 42;
                        stream.WriteByte(platCatTickets);
                        stream.Position = j - 39;
                        stream.WriteByte(platCatTickets);
                        stream.Position = j - 36;
                        stream.WriteByte(platCatTickets);
                        stream.Position = j - 33;
                        stream.WriteByte(platCatTickets);
                        Console.WriteLine("Success");
                    }
                }
            }

            static void Catseyes(string path)
            {
                Console.WriteLine("How many Catseyes do you want(max 65535)");
                int platCatTickets = Convert.ToInt32(Console.ReadLine());
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                int length = (int)stream.Length;
                byte[] allData = new byte[length];
                stream.Read(allData, 0, length);

                Console.WriteLine("Scan Complete");
                bool found = false;
                byte[] bytes = Endian(platCatTickets);
                for (int j = 3; j < length - 80; j++)
                {
                    //Console.WriteLine(j);
                    if (allData[j - 3] == Convert.ToByte(05) && allData[j + 20] == Convert.ToByte(03) && allData[j + 57] == Convert.ToByte(10) && allData[j + 71] == Convert.ToByte(27))
                    {
                        string CatFoodHex = Convert.ToString(platCatTickets, 16);



                        char[] CatArr = { };
                        char[] CatArr0 = { '0' };
                        found = true;


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
                int catID = Convert.ToInt32(Console.ReadLine());

                int startPos = 7362;
                stream.Position = startPos + catID * 4;
                stream.WriteByte(01);

            }

            static void SpecifUpgrade(string path)
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

                Console.WriteLine("What is the cat ID?");
                int catID = Convert.ToInt32(Console.ReadLine());
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

        }

    }
}

    

