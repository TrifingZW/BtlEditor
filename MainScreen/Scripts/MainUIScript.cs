using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BtlEditor.CoreScripts.Parser;
using Godot;
using static BtlEditor.CoreScripts.StaticRes;

namespace BtlEditor.MainScreen.Scripts;

public partial class MainUIScript : Control
{
	private RichTextLabel _richTextLabel;

	public override void _Ready()
	{
		_richTextLabel = GetNode<RichTextLabel>("%RichTextLabel");
		_richTextLabel.GetVScrollBar().Scale = Vector2.Zero;
		_richTextLabel.Newline();
		_richTextLabel.MetaClicked += url =>
		{
			if (url.AsString() == "start")
			{
				Start();
			}
		};
		Task.Run(LoadWc4Resource);
	}

	private void Start()
	{
		GetTree().ChangeSceneToFile("res://GameScreen/Game.tscn");
	}

	//加载资源
	private async Task LoadWc4Resource()
	{
		Directory.CreateDirectory(WorkPath);

		//外部资源
		await Task.Run(() =>
		{
			Btl = new();
			Bin = new();
			GeneralSettings = new();
			ArmySettings = new();
			Stringtable = new();
			TerrainConfig = new();
			MapConfig = new();
			Tacticalmap = new("tacticalmap", true);
			TerrainHd = new("terrain_hd", true);
			PlantHd = new("plant_hd", true);
			BuildingsHd = new("buildings_hd", true);
			ImageGeneralMedalHd = new("image_general_medal_hd", true, "image/");
			ImageRibbonHd = new("image_ribbon_hd", true, "image/");
			try
			{
				CallDeferred(nameof(PrintGreen), "正在导入资源中......");

				//TerrainConfig
				TerrainConfig.Parser();
				CallDeferred(nameof(PrintGreen), "已导入def_mapterrain");

				//MapConfig
				MapConfig.Parser();
				CallDeferred(nameof(PrintGreen), "已导入def_map");

				//Btl
				Btl.Parser();
				CallDeferred(nameof(Print),
					$"\n已解析btl  长:[color=green][b]{Btl.Master.地图宽}[/b][/color]  宽:[color=green][b]{Btl.Master.地图高}[/b][/color]  BTL版本:[color=green][b]{Btl.Master.Btl版本}[/b][/color]\n");

				//Bin
				if (!Btl.IndependentTerrain)
				{
					Map map = MapConfig.Maps.MapArray.First(m => m.Id == Btl.Master.地图序号);
					Bin.Parser(map.File);
					CallDeferred(nameof(Print),
						$"已解析bin  长:[color=green][b]{Bin.Width}[/b][/color]  宽:[color=green][b]{Bin.Height}[/b][/color]");
				}

				//GeneralSettings
				GeneralSettings.Parser();

				//ArmySettings
				ArmySettings.Parser();
				CallDeferred(nameof(PrintGreen), "已导入ArmySettings.json");

				//Stringtable
				Stringtable.Parser();
				CallDeferred(nameof(PrintGreen), "已导入ini文件");

				//Tacticalmap
				Tacticalmap.Parser();
				CallDeferred(nameof(PrintGreen), "已导入tacticalmap资源");

				//TerrainHd
				TerrainHd.Parser();
				CallDeferred(nameof(PrintGreen), "已导入terrain_hd资源");

				//PlantHd
				PlantHd.Parser();
				CallDeferred(nameof(PrintGreen), "已导入plant_hd资源");

				//BuildingsHd
				BuildingsHd.Parser();
				CallDeferred(nameof(PrintGreen), "已导入buildings_hd资源");

				//ImageGeneralMedalHd
				ImageGeneralMedalHd.Parser();
				CallDeferred(nameof(PrintGreen), "已导入image_general_medal_hd资源");

				if (Btl.Version2 || Btl.Version3)
				{
					//ImageRibbonHd
					ImageRibbonHd.Parser();
					CallDeferred(nameof(PrintGreen), "已导入image_ribbon_hd资源");
				}

				CallDeferred(nameof(PrintGreen), "导入完毕");
				CallDeferred(nameof(PrintGreen), $"[url=start][font_size=60][center]启动地图编辑器[/center][/font_size][/url]");
			}
			catch (Exception e)
			{
				CallDeferred(nameof(PrintRed), e.Message);
			}
		});
	}

	#region 输出函数

	private void PrintRed(string str)
	{
		_richTextLabel.Newline();
		_richTextLabel.PushColor(Colors.Red);
		_richTextLabel.AppendText(str);
		_richTextLabel.Pop();
	}

	private void PrintGreen(string str)
	{
		_richTextLabel.Newline();
		_richTextLabel.PushColor(Colors.Green);
		_richTextLabel.AppendText(str);
		_richTextLabel.Pop();
	}

	private void Print(string str) => _richTextLabel.AppendText(str);

	#endregion
}
