using System;
using System.IO;
using System.Threading.Tasks;
using Godot;
using static BtlEditor.CoreScripts.StaticRes;

namespace BtlEditor.MainScreen.Scripts;

public partial class LoadWindow : Window
{
    private RichTextLabel _richTextLabel;
    private bool _ready;

    public override void _Ready()
    {
        CloseRequested += Close;
        _richTextLabel = GetNode<RichTextLabel>("%RichTextLabel");
        _richTextLabel.GetVScrollBar().Scale = Vector2.Zero;
    }

    private void Close()
    {
        if (_ready) Hide();
    }

    public void Load(Action<bool> action)
    {
        Show();
        Task.Run(LoadWc4Resource).ContinueWith(task =>
        {
            if (task.IsFaulted)
                CallDeferred(nameof(PrintRed), task.Exception.Message);
            else
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
            CnStringtable = new();
            EnStringtable = new();
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
            if (RunSafely(() => TerrainConfig.Parser()))
                CallDeferred(nameof(PrintGreen), "已导入def_mapterrain");

            //MapConfig
            if (RunSafely(() => MapConfig.Parser()))
                CallDeferred(nameof(PrintGreen), "已导入def_map");

            //GeneralSettings
            if (RunSafely(() => GeneralSettings.Parser()))
                CallDeferred(nameof(PrintGreen), "已导入GeneralSettings.json");

            //ArmySettings
            if (RunSafely(() => ArmySettings.Parser()))
                CallDeferred(nameof(PrintGreen), "已导入ArmySettings.json");

            //Stringtable
            if (RunSafely(() => CnStringtable.Parser("stringtable_cn.ini")))
                CallDeferred(nameof(PrintGreen), "已导入stringtable_cn.ini文件");
            if (RunSafely(() => EnStringtable.Parser("stringtable_en.ini")))
                CallDeferred(nameof(PrintGreen), "已导入stringtable_en.ini文件");

            //Tacticalmap
            if (RunSafely(() => Tacticalmap.Parser()))
                CallDeferred(nameof(PrintGreen), "已导入tacticalmap资源");

            //TerrainHd
            if (RunSafely(() => TerrainHd.Parser()))
                CallDeferred(nameof(PrintGreen), "已导入terrain_hd资源");

            //PlantHd
            if (RunSafely(() => PlantHd.Parser()))
                CallDeferred(nameof(PrintGreen), "已导入plant_hd资源");

            //BuildingsHd
            if (RunSafely(() => BuildingsHd.Parser()))
                CallDeferred(nameof(PrintGreen), "已导入buildings_hd资源");

            //ImageGeneralMedalHd
            if (RunSafely(() => ImageGeneralMedalHd.Parser()))
                CallDeferred(nameof(PrintGreen), "已导入image_general_medal_hd资源");

            //ImageRibbonHd
            if (RunSafely(() => ImageRibbonHd.Parser()))
                CallDeferred(nameof(PrintGreen), "已导入image_ribbon_hd资源");

            CallDeferred(nameof(PrintGreen), "\n导入完毕，可以关闭窗口，请仔细阅读打印信息。");
            _ready = true;
            
        });
    }

    private bool RunSafely(Action action)
    {
        try
        {
            action();
            return true;
        }
        catch (Exception e)
        {
            CallDeferred(nameof(PrintRed), e.Message);
            return false;
        }
    }

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