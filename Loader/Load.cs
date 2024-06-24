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
namespace Loader
{
    public class Load
    {
		static string URL = "https://donderhiroba.jp/index.php";
		static ChromeDriver Driver;
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
		static async public void Main(string[] args)
		{
			var driverService = ChromeDriverService.CreateDefaultService();
			driverService.HideCommandPromptWindow = true;
			Driver = new ChromeDriver(driverService, new ChromeOptions());
			Driver.Manage().Window.Position = new System.Drawing.Point(1920, 1080);
			Driver.Navigate().GoToUrl(URL);
			var actions = new Actions(Driver);
			var button = Driver.FindElement(By.ClassName("image_base"));
			actions.Click(button);
			actions.Perform();
			var textBox = Driver.FindElement(By.ClassName("c-input-label"));
			actions.SendKeys(textBox, args[0]);
			actions.Perform();
			textBox = Driver.FindElement(By.ClassName("c-input-pass"));
			actions.SendKeys(textBox, args[1]);
			actions.Perform();
			button = Driver.FindElement(By.Id("btn-idpw-login"));
			actions.Click(button);
			actions.Perform();
			while (Driver.Title != "ドンだーひろば")
			{
				Thread.Sleep(10);
			}
			button = Driver.FindElement(By.Id("form_user1"));
			actions.Click(button);
			actions.Perform();
			while (Driver.Url != "https://donderhiroba.jp/index.php")
			{
				Thread.Sleep(10);
			}
			Thread.Sleep(1000);
			button = Driver.FindElement(By.Id("onetrust-close-btn-container"));
			actions.Click(button);
			actions.Perform();
			var tasks = new List<Task>();
			for (int i = 0; i < 8; i++)
			{
				string url = "https://donderhiroba.jp/score_list.php?genre=" + (GenreList[i] + 1);
				
				int lankBit = Int32.Parse(args[2]);
				Parallel.Invoke(new ParallelOptions() { MaxDegreeOfParallelism = 8 },
					() => { GetDataBackGround(4, GenreList[0], url); },
					() => { GetDataBackGround(4, GenreList[1], url); },
					() => { GetDataBackGround(4, GenreList[2], url); },
					() => { GetDataBackGround(4, GenreList[3], url); },
					() => { GetDataBackGround(4, GenreList[4], url); },
					() => { GetDataBackGround(4, GenreList[5], url); },
					() => { GetDataBackGround(4, GenreList[6], url); },
					() => { GetDataBackGround(4, GenreList[7], url); }
					);
				break;
				for (int j = 0; j < 5; j++)
				{
					if ((lankBit & 0x01) == 0x01)
					{
						tasks.Add(GetDataBackGround(j, GenreList[i], url));
					}
					lankBit = lankBit >> 1;
				}
			}
			await Task.WhenAll(tasks);
			Driver.Quit();

			Save(args[3]);
			Console.ReadLine();

		}
		static async Task GetDataBackGround(int level, int id, string url)
		{
			Driver.SwitchTo().NewWindow(WindowType.Window);
			Driver.Navigate().GoToUrl(url);
			var handle = Driver.CurrentWindowHandle;
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
			for (int i = 0; i < buttons.Count; i++)
			{
				buttons = Driver.FindElements(By.ClassName(levelName[level] + Genre[id]));
				actions.Click(buttons[i]);
				actions.Perform();
				while (Driver.Url == url)
				{
					await Task.Delay(10);
				}
				Driver.SwitchTo().Window(handle);
				PrintSingle(level, id);
				Driver.Navigate().Back();
				while (Driver.Url != url)
				{
					await Task.Delay(10);
				}
				Driver.SwitchTo().Window(handle);
				if (i == 4) break;
			}
			Driver.SwitchTo().Window(handle);
			Driver.Quit();
			Driver.SwitchTo().Window(Driver.WindowHandles[0]);
			
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
			for (int i = 0; i < classNames.Length; i++)
			{
				var text = Driver.FindElement(By.ClassName(classNames[i])).Text;
				text = text.Replace("回", "");
				text = text.Replace("点", "");
				text = text.Replace("位", "");
				if (text == "---")
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
			foreach (var data in Datas)
			{
				sw.WriteLine(data.GetCsvLine());
			}
			sw.Close();
		}
	}
}
