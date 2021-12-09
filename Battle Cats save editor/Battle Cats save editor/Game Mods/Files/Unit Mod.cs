using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Battle_Cats_save_editor.Game_Mods
{
    public class UnitMod
    {
        public static void Unitcsv()
        {
            OpenFileDialog fd = new()
            {
                Filter = "files (unit*.csv)|unit*.csv",
                Title = "Select a unit*.csv file"
            };
            if (fd.ShowDialog() != DialogResult.OK)
            {
                Console.WriteLine("Please select .csv files");
                Editor.Options();
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
                Editor.ColouredText("Thanks to this resource: &https://pastebin.com/JrCTPnUV& for help with parsing the unit data\n", ConsoleColor.White, ConsoleColor.Green);
                Editor.ColouredText("What do you want to edit?:\n&1.& First form\n&2.& Second Form\n&3.& Third Form\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                choice = (int)Editor.Inputed();
                if (choice > 3)
                {
                    Console.WriteLine("Please enter a number that is recognised");
                    Unitcsv();
                }
            }
            // If unit doesn't have a true form don't display the option to edit it
            else
            {
                Editor.ColouredText("Thanks to this resource: &https://pastebin.com/JrCTPnUV& for help with parsing the unit data\n", ConsoleColor.White, ConsoleColor.Green);
                Editor.ColouredText("What do you want to edit?:\n&1.& First form\n&2.& Second Form\n", ConsoleColor.White, ConsoleColor.DarkYellow);
                choice = (int)Editor.Inputed();
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
            Editor.ColouredText(unitValues, ConsoleColor.White, ConsoleColor.DarkYellow);
            string ValueTypes = "";
            for (int i = 0; i < values.Length; i++)
            {
                ValueTypes += $"&{i + 1}.& {values[i]}\n";
            }
            Editor.ColouredText("What do you want to edit:\n" + ValueTypes + "\nWhat do you want to edit? You can enter multiple values separated by spaces to edit multiple values at once\n", ConsoleColor.White, ConsoleColor.DarkYellow);
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
                    int value = (int)Editor.Inputed();

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
                    Editor.ColouredText($"&Set contents of the &{Path.GetFileName(path)}& file to&\n{fin}&\n", ConsoleColor.White, ConsoleColor.DarkYellow);
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
    }
}
