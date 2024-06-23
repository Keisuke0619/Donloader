
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Threading;
using System.Net;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System.Runtime.CompilerServices;
using System.IO;
namespace TestCs_http
{
	internal class Program
	{
		static string URL = "https://donderhiroba.jp/index.php";
		static ChromeDriver Driver = new ChromeDriver();
		static List<Data> Datas = new List<Data>();
		
		static string[] Genre =
			{
				"jpop",
				"anime",
				"kids",
				"vocaloid",
				"game",
				"namco",
				"variety",
				"classic",
			};
		static int[] GenreList = { 0, 2, 1, 3, 4, 6, 7, 5 };
		static void Main(string[] args)
		{
			Driver.Navigate().GoToUrl(URL);


			var button = Driver.FindElement(By.ClassName("image_base"));
			button.Click();
			var textBox = Driver.FindElement(By.ClassName("c-input-label"));
			textBox.SendKeys(args[0]);
			textBox = Driver.FindElement(By.ClassName("c-input-pass"));
			textBox.SendKeys(args[1]);
			button = Driver.FindElement(By.Id("btn-idpw-login"));
			button.Click();
			while(Driver.Title != "ドンだーひろば")
			{
				Thread.Sleep(10);
			}
			button = Driver.FindElement(By.Id("form_user1"));
			button.Click();
			while(Driver.Url != "https://donderhiroba.jp/index.php")
			{
				Thread.Sleep(10);
			}
			Thread.Sleep(1000);
			button = Driver.FindElement(By.Id("onetrust-close-btn-container"));
			button.Click();
			for(int i = 0; i < 8; i++)
			{
				string url = "https://donderhiroba.jp/score_list.php?genre=" + (GenreList[i] + 1);
				Driver.Navigate().GoToUrl(url);
				int lankBit = Int32.Parse(args[2]);
				for(int j = 0; j < 5; j++)
				{
					if((lankBit & 0x01) == 0x01)
					{
						GetDataBackGround(j, GenreList[i], url);
					}
					lankBit = lankBit >> 1;
				}
			}
			Driver.Quit();

			Save(args[3]);


		}
		static void GetDataBackGround(int level, int id, string url)
		{
			string[] levelName =
			{
				"easy",
				"normal",
				"hard",
				"oni",
				"oni_ura",
			};
			var buttons = Driver.FindElements(By.ClassName(levelName[level] + Genre[id]));
			var actions = new Actions(Driver);
			for(int i =0; i < buttons.Count; i++)
			{
				buttons = Driver.FindElements(By.ClassName(levelName[level] + Genre[id]));
				actions.Click(buttons[i]);
				actions.Perform();
				while(Driver.Url == url)
				{
					Thread.Sleep(10);
				}
				PrintSingle(level, id);
				Driver.Navigate().Back();
				while(Driver.Url != url)
				{
					Thread.Sleep(10);
				}
			}
		}
		static void PrintSingle(int level, int id)
		{
			string title = Driver.FindElement(By.ClassName("songNameFont" + Genre[id])).Text;
			title = title.Replace(",", ".");
			string[] classNames =
			{
				"ranking",
				"high_score",
				"good_cnt",
				"ok_cnt",
				"ng_cnt",
				"combo_cnt",
				"pound_cnt",
				"stage_cnt",
				"clear_cnt",
				"full_combo_cnt",
				"dondaful_combo_cnt",
			};
			int[] datas = new int[classNames.Length + 1];
			for(int i = 0; i < classNames.Length; i++)
			{
				var text = Driver.FindElement(By.ClassName(classNames[i])).Text;
				text = text.Replace("回", "");
				text = text.Replace("点", "");
				text = text.Replace("位", "");
				if(text == "---")
				{
					text = "-1";
				}
				Int32.TryParse(text, out datas[i]);
			}
			Datas.Add(new Data(title, datas[0], datas[1], datas[2], datas[3], datas[4], datas[5], datas[6], datas[7], datas[8], datas[9], datas[10], id, level));
		}
		static void Save(string path)
		{
			StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8);
			sw.WriteLine("ジャンル,むずかしさ,楽曲名,ランキング,最大スコア,良,可,不可,最大コンボ,連打数,プレイ回数,クリア回数,フルコンボ,ドンダフルコンボ");
			foreach(var data in Datas)
			{
				sw.WriteLine(data.GetCsvLine());
			}
			sw.Close();
		}
	}
}
