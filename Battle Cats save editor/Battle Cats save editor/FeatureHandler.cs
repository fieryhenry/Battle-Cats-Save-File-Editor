using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Security.Cryptography;
using System.Windows.Forms;
using static Battle_Cats_save_editor.Editor;
using Battle_Cats_save_editor.SaveEdits;

namespace Battle_Cats_save_editor
{
    public class FeatureHandler
    {
        public static Dictionary<string, Tuple<Delegate, Delegate>> copy_methods = new()
        {
            { "All Treasures In A Chapter / Chapters", Tuple.Create<Delegate, Delegate>(AllTreasures.GetTreasures, AllTreasures.SetTreasures) },
            { "Cat Fruits / Seeds", Tuple.Create<Delegate, Delegate>(CatFruits.GetCatFruit, CatFruits.SetCatFruit) },
            { "Talent Orbs", Tuple.Create<Delegate, Delegate>(TalentOrbs.GetTalentOrbs, TalentOrbs.SetTalentOrbs) },
            { "Helpers", Tuple.Create<Delegate, Delegate>(GamatotoHelper.GetHelpers, GamatotoHelper.SetHelpers) },
            { "Cat Cannon Upgrades", Tuple.Create<Delegate, Delegate>(OtotoCatCannon.GetCatCannons, OtotoCatCannon.SetCatCannons) },
            { "Get all cats", Tuple.Create<Delegate, Delegate>(GetSpecificCats.GetCats, GetSpecificCats.SetCats) },
            { "Upgrade all cats", Tuple.Create<Delegate, Delegate>(CatHandler.GetCatUpgrades, CatHandler.SetCatUpgrades) },
            { "Upgrade Base / Special Skills (The ones that are blue)", Tuple.Create<Delegate, Delegate>(BlueUpgrade.GetBlueUpgrades, BlueUpgrade.SetBlueUpgrades) },
            { "Evolve all cats", Tuple.Create<Delegate, Delegate>(EvolveCats.GetEvolveForms, EvolveCats.SetEvolveForms) },
            { "Talent upgrade all cats", Tuple.Create<Delegate, Delegate>(AllTalent.GetTalents, AllTalent.SetTalents) },
            { "Clear Main Story Chapters", Tuple.Create<Delegate, Delegate>(MainStory.GetStageData, MainStory.SetStageData) },
            { "Clear Uncanny Legends Subchapters", Tuple.Create<Delegate, Delegate>(UncannyLegends.GetSubchapterData, UncannyLegends.SetSubchapterData) },
            { "Clear Other Event Stages", Tuple.Create<Delegate, Delegate>(MainEventStages.GetSubchapterData, MainEventStages.SetSubchapterData) },
            { "Clear Aku Realm", Tuple.Create<Delegate, Delegate>(AkuRealm.GetAku, AkuRealm.SetAku) },
        };

        static readonly Dictionary<string, List<string>> features = new()
        {
            {
                "Top Level Features",
                new()
                {
                    "Select New Save",
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
                    "Calculate checksum of save file"
                }
            },
            {
                "Tickets",
                new()
                {
                    "Go back",
                    "Normal Tickets",
                    "Rare Tickets",
                    "Platinum Tickets",
                    "Legend Tickets",
                    "Platinum Shards (using this instead of platinum tickets reduces the chance of a ban)"
                }
            },
            {
                "Treasures",
                new()
                {
                    "Go back",
                    "All Treasures In A Chapter / Chapters",
                    "Specific Treasure Types e.g Energy Drink, Void Fruit"
                }
            },
            {
                "Gamatoto",
                new()
                {
                    "Go back",
                    "Catamins",
                    "Helpers",
                    "XP"
                }
            },
            {
                "Ototo",
                new()
                {
                    "Go back",
                    "Base Materials",
                    "Engineers",
                    "Cat Cannon Upgrades"
                }
            },
            {
                "Gain / Remove Cats",
                new()
                {
                    "Go back",
                    "Get all cats",
                    "Get specific cats",
                    "Remove all cats",
                    "Remove specific cats"
                }
            },
            {
                "Cat / Stat Upgrades",
                new()
                {
                    "Go back",
                    "Upgrade all cats",
                    "Upgrade all cats that are currently unlocked",
                    "Upgrade specific cats",
                    "Upgrade Base / Special Skills (The ones that are blue)"
                }
            },
            {
                "Cat Talents",
                new()
                {
                    "Go back",
                    "Talent upgrade all cats",
                    "Talent upgrade specific cats"
                }
            },
            {
                "Clear Levels / Outbreaks / Timed Score",
                new()
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
                    "Clear Infernal Tower Stages"
                }
            },
            {
                "Inquiry Code / Elsewhere Fix / Unban",
                new()
                {
                    "Go back",
                    "Change Inquiry code",
                    "Fix save is used elsewhere error (or unban account) - whilst selecting a save that has the error / ban (the one you select when you open the editor) select a new save that doesn't have the \"save is used elsewhere\" bug / is not banned (you can re-install the game to get a save like that)"
                }
            },
            {
                "Cat Evolves / Devolves",
                new()
                {
                    "Go back",
                    "Evolve all cats",
                    "Evolve specific cats",
                    "Evolve all curent cats",
                    "Devolve all cats",
                    "Devolve specific cats"
                }
            }
        };
        public static void Options()
        {
            ColouredText("Thanks to: Lethal's editor for being a tool for me to use when figuring out how to patch save files,and how to edit cf/xp\nAnd thanks to beeven and csehydrogen's open source work, which I used to implement the save patching algorithm\n\n&", New: ConsoleColor.Green);
            if (gameVer != "en")
            {
                ColouredText("Warning: if you are using a non en save, many features won't work, or they might corrupt your save data, so make sure you create a copy of your saves!\n", ConsoleColor.White, ConsoleColor.Red);
            }            
            ColouredText(GetOptionList("Top Level Features", "Select New Save"));
            Console.WriteLine("Features marked with a C means that you can copy that data from one save to another. Enter the feature number followed by a space and then the letter c, e.g 9 c:");
            string[] response = Console.ReadLine().Split(' ');
            int choice = int.Parse(response[0]);
            if (response.Length > 1 && response[1].ToLower() == "c")
            {
                CheckCopy("Top Level Features", main_path, choice);
            }
            switch (choice)
            {
                case 0: SelSave(); CreateBackup(main_path); Options(); break;
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
                case 23: break;
                default: Console.WriteLine("Please input a number that is recognised"); break;
            }
            PatchSaveFile.PatchSaveData(main_path);
            ColouredText("Press enter to continue\n");
            Console.ReadLine();
            Options();
        }
        public static void Evolves(string path)
        {
            string feature_name = "Cat Evolves / Devolves";
            ColouredText(GetOptionList(feature_name));
            string[] response = Console.ReadLine().Split(' ');
            int choice = int.Parse(response[0]);
            if (response.Length > 1 && response[1].ToLower() == "c")
            {
                CheckCopy(feature_name, main_path, choice);
            }

            switch (choice)
            {
                case 0: Options(); break;
                case 1: EvolveCats.EvolveAll(path); break;
                case 2: EvolveCats.EvolveSpecific(path); break;
                case 3: EvolveCurrentCats.EvolveCurrent(path); break;
                case 4: DevolveCats.DevolveAll(path); break;
                case 5: DevolveCats.DevolveSpecific(path); break;
                default: Console.WriteLine("Please enter a recognised number"); break;
            }
        }
        public static void Tickets(string path)
        {
            string feature_name = "Tickets";
            ColouredText(GetOptionList(feature_name));
            string[] response = Console.ReadLine().Split(' ');
            int choice = int.Parse(response[0]);
            if (response.Length > 1 && response[1].ToLower() == "c")
            {
                CheckCopy(feature_name, main_path, choice);
            }

            switch (choice)
            {
                case 0: Options(); break;
                case 1: NormalTickets.CatTicket(path); break;
                case 2: RareTickets.RareCatTickets(path); break;
                case 3: PlatTickets.PlatinumTickets(path); break;
                case 4: LegendTickets.LegendTicket(path); break;
                case 5: PlatinumShards.PlatShards(path); break;
                default: Console.WriteLine("Please enter a recognised number"); break;
            }
        }
        public static void Treasures(string path)
        {
            string feature_name = "Treasures";
            ColouredText(GetOptionList(feature_name));
            string[] response = Console.ReadLine().Split(' ');
            int choice = int.Parse(response[0]);
            if (response.Length > 1 && response[1].ToLower() == "c")
            {
                CheckCopy(feature_name, main_path, choice);
            }

            switch (choice)
            {
                case 0: Options(); break;
                case 1: AllTreasures.MaxTreasures(path); break;
                case 2: SpecificTreasures.VerySpecificTreasures(path); break;
                default: Console.WriteLine("Please enter a recognised number"); break;
            }
        }
        public static void Gamatoto(string path)
        {
            string feature_name = "Gamatoto";
            ColouredText(GetOptionList(feature_name));
            string[] response = Console.ReadLine().Split(' ');
            int choice = int.Parse(response[0]);
            if (response.Length > 1 && response[1].ToLower() == "c")
            {
                CheckCopy(feature_name, main_path, choice);
            }

            switch (choice)
            {
                case 0: Options(); break;
                case 1: Catamins.Catamin(path); break;
                case 2: GamatotoHelper.GamHelp(path); break;
                case 3: GamatotoXP.GamXP(path); break;
                default: Console.WriteLine("Please enter a recognised number"); break;
            }
        }
        public static void Ototo(string path)
        {
            string feature_name = "Ototo";
            ColouredText(GetOptionList(feature_name));
            string[] response = Console.ReadLine().Split(' ');
            int choice = int.Parse(response[0]);
            if (response.Length > 1 && response[1].ToLower() == "c")
            {
                CheckCopy(feature_name, main_path, choice);
            }

            switch (choice)
            {
                case 0: Options(); break;
                case 1: BaseMaterials.BaseMats(path); break;
                case 2: OtotoEngineers.Engineers(path); break;
                case 3: OtotoCatCannon.CatCannon(path); break;
                default: Console.WriteLine("Please enter a recognised number"); break;
            }
        }
        public static void GetCats(string path)
        {
            string feature_name = "Gain / Remove Cats";
            ColouredText(GetOptionList(feature_name));
            string[] response = Console.ReadLine().Split(' ');
            int choice = int.Parse(response[0]);
            if (response.Length > 1 && response[1].ToLower() == "c")
            {
                CheckCopy(feature_name, main_path, choice);
            }

            switch (choice)
            {
                case 0: Options(); break;
                case 1: GetCat.Cats(path); break;
                case 2: GetSpecificCats.SpecifiCat(path); break;
                case 3: RemoveCats.RemCats(path); break;
                case 4: RemoveSpecificCats.RemSpecifiCat(path); break;
                default: Console.WriteLine("Please enter a recognised number"); break;
            }
        }
        public static void Upgrades(string path)
        {
            string feature_name = "Cat / Stat Upgrades";
            ColouredText(GetOptionList(feature_name));
            string[] response = Console.ReadLine().Split(' ');
            int choice = int.Parse(response[0]);
            if (response.Length > 1 && response[1].ToLower() == "c")
            {
                CheckCopy(feature_name, main_path, choice);
            }

            switch (choice)
            {
                case 0: Options(); break;
                case 1: CatUpgrade.CatUpgrades(path); break;
                case 2: UpgradeCurrent.UpgradeCurrentCats(path); break;
                case 3: SpecificUpgrade.SpecifUpgrade(path); break;
                case 4: BlueUpgrade.Blue(path); break;
                default: Console.WriteLine("Please enter a recognised number"); break;
            }
        }
        public static void Talent(string path)
        {
            string feature_name = "Cat Talents";
            ColouredText(GetOptionList(feature_name));
            string[] response = Console.ReadLine().Split(' ');
            int choice = int.Parse(response[0]);
            if (response.Length > 1 && response[1].ToLower() == "c")
            {
                CheckCopy(feature_name, main_path, choice);
            }

            switch (choice)
            {
                case 0: Options(); break;
                case 1: AllTalent.MaxTalents(path); break;
                case 2: AllTalent.IndividualTalents(path); break;
                default: Console.WriteLine("Please enter a recognised number"); break;
            }
        }
        public static void Levels(string path)
        {
            string feature_name = "Clear Levels / Outbreaks / Timed Score";
            ColouredText(GetOptionList(feature_name));
            string[] response = Console.ReadLine().Split(' ');
            int choice = int.Parse(response[0]);
            if (response.Length > 1 && response[1].ToLower() == "c")
            {
                CheckCopy(feature_name, main_path, choice);
            }

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
                default: Console.WriteLine("Please enter a recognised number"); break;
            }
        }
        public static void Inquiry(string path)
        {
            string feature_name = "Inquiry Code / Elsewhere Fix / Unban";
            ColouredText(GetOptionList(feature_name));
            string[] response = Console.ReadLine().Split(' ');
            int choice = int.Parse(response[0]);
            if (response.Length > 1 && response[1].ToLower() == "c")
            {
                CheckCopy(feature_name, main_path, choice);
            }

            switch (choice)
            {
                case 0: Options(); break;
                case 1: NewInquiryCode.NewIQ(path); break;
                case 2: FixElsewhere.Elsewhere(path); break;
                default: Console.WriteLine("Please enter a recognised number"); break;
            }
        }
        public static string GetOptionList(string option, string first = "Go back")
        {
            string toOutput = "";
            for (int i = 0; i < features[option].Count; i++)
            {
                if (i == 0)
                {
                    toOutput += $"{i}.& ";
                    toOutput += first;
                    features[option][i] = "&";
                }
                else
                {
                    toOutput += $"&{i}.& ";
                }
                if (copy_methods.ContainsKey(features[option][i]))
                {
                    toOutput += "&C: &";
                }

                toOutput += features[option][i];
                toOutput += "\n";

            }
            return "&What do you want to edit?:\n&" + toOutput;
        }
        public static void CheckCopy(string feature_name, string path, int index = -1)
        {
            if (index != -1 && index < features[feature_name].Count)
            {
                feature_name = features[feature_name][index];
            }
            if (copy_methods.ContainsKey(feature_name))
            {
                CopyData.Copy(feature_name, path);
                Console.WriteLine("Successfly copied data");
                Options();
            }
        }
    }
}
