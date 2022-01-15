using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Battle_Cats_save_editor.SaveEdits
{
    public class LevelSelect
    {
		public static void Levels(string path)
		{
			string[] Features = new string[]
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
			};
			string toOutput = "&What would you like to edit?&\n0.& Go back\n&";
			for (int i = 1; i < Features.Length; i++)
			{
				toOutput += string.Format("&{0}.& ", i);
				toOutput = toOutput + Features[i] + "\n";
			}
			Editor.ColouredText(toOutput);
			switch ((int)Editor.Inputed())
			{
				case 0:
					Editor.Options();
					break;
				case 1:
					MainStory.Stage(path);
					break;
				case 2:
					StoriesOfLegend.Sol(path);
					break;
				case 3:
					UncannyLegends.Uncanny_Legends(path);
					break;
				case 4:
					MainEventStages.EventStages(path);
					break;
				case 5:
					ZombieStages.Outbreaks(path);
					break;
				case 6:
					AkuRealm.ClearAku(path);
					break;
				case 7:
					ItFTimedScores.TimedScore(path);
					break;
				case 8:
					HeavenlyTower.ClearHeavenlyTower(path);
					break;
				case 9:
					InfernalTower.ClearInfernalTower(path);
					break;
				default:
					Console.WriteLine(string.Format("Please enter a number between 0 and {0}", Features.Length));
					break;
			}
		}
	}
}
