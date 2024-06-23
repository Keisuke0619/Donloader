using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCs_http
{
	internal class Data
	{
		public string Title;
		public int Rank;
		public int HighScore;
		public int Good;
		public int Ok;
		public int Ng;
		public int Combo;
		public int Pound;
		public int Stage;
		public int Clear;
		public int FullCombo;
		public int DondafulCombo;
		public int Genre;
		public int Level;
		public Data(string title, int rank, int heigScore, int good, int ok, int ng, int combo, int pound, int stage, int clear, int fullCombo, int dondafulCombo, int genre, int level)
		{
			Title = title;
			Rank = rank;
			HighScore = heigScore;
			Good = good;
			Ok = ok;
			Ng = ng;
			Combo = combo;
			Pound = pound;
			Stage = stage;
			Clear = clear;
			FullCombo = fullCombo;
			DondafulCombo = dondafulCombo;
			Genre = genre;
			Level = level;
		}
		private Data() { }
		public string GetCsvLine()
		{
			string[] levelString =
			{
				"かんたん",
				"ふつう",
				"むずかしい",
				"おに",
				"おに裏",
			};
			string[] genreString =
			{
				"ポップス",
				"アニメ",
				"キッズ",
				"VOCALOID",
				"ゲームミュージック",
				"ナムコオリジナル",
				"バラエティ",
				"クラシック",
			};
			return genreString[Genre] + "," + levelString[Level] + "," + Title + "," + Rank + "," + HighScore + "," + Good + "," + Ok + "," + Ng + "," + Combo + "," + Pound + "," + Stage + "," + Clear + "," + FullCombo + "," + DondafulCombo;
		}
	}
}
