using System;
using System.IO;
using System.Threading.Tasks;
using Godot;
using static BtlEditor.CoreScripts.StaticRes;

namespace BtlEditor.MainScreen.Scripts;

public partial class LoadWindow : Window
{
    private RichTextLabel _richTextLabel;

    public override void _Ready()
    {
        CloseRequested += () =>
        {
         
        };
        _richTextLabel = GetNode<RichTextLabel>("RichTextLabel");
        _richTextLabel.GetVScrollBar().Scale = Vector2.Zero;
    }

    public void Load(Action<bool> action)
    {
        Show();
        Task.Run(LoadWc4Resource).ContinueWith(task =>
        {
            if (!task.IsFaulted)
                action(true);
        });
    }

    private async Task LoadWc4Resource()
    {
        Directory.CreateDirectory(WorkPath);

        //外部资源
        await Task.Run(() =>
        {
            GeneralSettings = new();
            Stringtable = new();
            TerrainConfig = new();
            MapConfig = new();
            Tacticalmap = new("tacticalmap", true);
            ArmySettings = new();
            TerrainHd = new("terrain_hd", true);
            PlantHd = new("plant_hd", true);
            BuildingsHd = new("buildings_hd", true);
            ImageGeneralMedalHd = new("image_general_medal_hd", true, "image/");
            ImageRibbonHd = new("image_ribbon_hd", true, "image/");
            CallDeferred(nameof(PrintGreen), "正在导入资源中......");

            //TerrainConfig
            TerrainConfig.Parser();
            CallDeferred(nameof(PrintGreen), "已导入def_mapterrain");

            //MapConfig
            MapConfig.Parser();
            CallDeferred(nameof(PrintGreen), "已导入def_map");

            //GeneralSettings
            GeneralSettings.Parser();
            CallDeferred(nameof(PrintGreen), "已导入GeneralSettings.json");

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

            //ImageRibbonHd
            ImageRibbonHd.Parser();
            CallDeferred(nameof(PrintGreen), "已导入image_ribbon_hd资源");

            CallDeferred(nameof(PrintGreen), "导入完毕");
            
            CallDeferred(nameof(Diss));
        });
    }

    private void Diss() => Hide();

    #region 输出函数

    private void PrintRed(string str)
    {
        _richTextLabel.PushColor(Colors.Red);
        _richTextLabel.AppendText(str);
        _richTextLabel.Pop();
        _richTextLabel.Newline();
    }

    private void PrintGreen(string str)
    {
        _richTextLabel.PushColor(Colors.Green);
        _richTextLabel.AppendText(str);
        _richTextLabel.Pop();
        _richTextLabel.Newline();
    }

    private void Print(string str) => _richTextLabel.AppendText(str);

    #endregion
}