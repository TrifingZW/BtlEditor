using System.Collections.Generic;
using BtlEditor.CoreScripts.Parser;
using BtlEditor.CoreScripts.Structures;
using BtlEditor.CoreScripts.Utils;
using BtlEditor.GameScreen.Scripts.LandScripts;
using BtlEditor.GameScreen.Scripts.Windows;
using Godot;
using static BtlEditor.GameScreen.Scripts.MapHelper;

namespace BtlEditor.GameScreen.Scripts.MapUIScripts.Multi;

public partial class MultiContainer : PanelContainer, IInput
{
    private static MapController MapController => Game.Instance.MapController;
    private static GameLandUnit[] LandUnits => MapController.LandUnits;
    private static TileMap TileMap => MapController.TileMap;
    private static EditWindow EditWindow => Game.Instance.EditWindow;
    public List<GameLandUnit> MultiLandUnit { get; set; } = [];
    private TabBar _multiTabContainer;

    public override void _Ready()
    {
        _multiTabContainer = GetNode<TabBar>("%MultiModeTab");
    }

    private void MultiModeClear()
    {
        MultiLandUnit.Clear();
        MapController.TileMap.ClearLayer(MapController.MultiLayer);
    }

    private void MultiModeCreateArmy()
    {
        foreach (GameLandUnit landUnit in MultiLandUnit)
        {
            if (landUnit.Army != null) continue;
            if (Btl.Version1) landUnit.Army = new Army1 { 坐标 = landUnit.RegionIndex };
            if (Btl.Version2 || Btl.Version3) landUnit.Army = new Army2 { 坐标 = landUnit.RegionIndex };
        }
    }

    private void MultiModeCreateCity()
    {
        foreach (GameLandUnit landUnit in MultiLandUnit)
        {
            if (landUnit.City != null) continue;
            landUnit.City = new()
            {
                坐标 = landUnit.RegionIndex
            };
            landUnit.Province = landUnit.RegionIndex;
            landUnit.UpdateProvinceColor();
        }

        MapController.UpdateColorUV();
    }

    private void MultiModeSetProvince()
    {
        EditWindow.CreateEdit("设置省规划", province =>
        {
            foreach (GameLandUnit landUnit in MultiLandUnit)
            {
                landUnit.Province = (short)province;
                landUnit.UpdateProvinceColor();
            }

            MapController.UpdateColorUV();
        });
    }

    private void MultiModeSetBelong()
    {
        EditWindow.CreateEdit("设置归属", belong =>
        {
            foreach (GameLandUnit landUnit in MultiLandUnit)
            {
                landUnit.Belong = (byte)belong;
                landUnit.UpdateBelongColor();
            }

            MapController.UpdateColorUV();
        });
    }

    private void MultiModeDeleteArmy()
    {
        foreach (GameLandUnit landUnit in MultiLandUnit)
        {
            landUnit.ClearArmy();
            landUnit.ExamineBelong();
        }

        MapController.UpdateColorUV();
    }

    private void MultiModeDeleteCity()
    {
        foreach (GameLandUnit landUnit in MultiLandUnit)
        {
            landUnit.ClearCity();
            landUnit.ExamineBelong();
        }

        MapController.UpdateColorUV();
    }

    private bool _multiPressed;

    private static Vector2 MousePosition => MapController.GetGlobalMousePosition();

    public void Input(InputEvent @event)
    {
        switch (@event)
        {
            case InputEventMouseButton inputEventMouseButton:
            {
                if (inputEventMouseButton.ButtonIndex == MouseButton.Right)
                {
                    _multiPressed = inputEventMouseButton.Pressed;
                    Vector2I vector2I = TileMap.LocalToMap(MousePosition);
                    if (LandUnits.TryGetValue(MapHelper.GetIndex(vector2I), out GameLandUnit landUnit))
                        switch (_multiTabContainer.CurrentTab)
                        {
                            case 0:
                                if (!MultiLandUnit.Contains(landUnit))
                                {
                                    TileMap.SetCell(MapController.MultiLayer, vector2I, MapController.MultiTileSetAtlasId, new());
                                    MultiLandUnit.Add(landUnit);
                                }

                                break;
                            case 1:
                                if (MultiLandUnit.Contains(landUnit))
                                {
                                    TileMap.EraseCell(MapController.MultiLayer, vector2I);
                                    MultiLandUnit.Remove(landUnit);
                                }

                                break;
                        }
                }

                break;
            }
            case InputEventMouseMotion:
            {
                if (_multiPressed)
                {
                    Vector2I vector2I = TileMap.LocalToMap(MousePosition);
                    if (LandUnits.TryGetValue(MapHelper.GetIndex(vector2I), out GameLandUnit landUnit))
                        switch (_multiTabContainer.CurrentTab)
                        {
                            case 0:
                                if (!MultiLandUnit.Contains(landUnit))
                                {
                                    TileMap.SetCell(MapController.MultiLayer, vector2I, MapController.MultiTileSetAtlasId, new());
                                    MultiLandUnit.Add(landUnit);
                                }

                                break;
                            case 1:
                                if (MultiLandUnit.Contains(landUnit))
                                {
                                    TileMap.EraseCell(MapController.MultiLayer, vector2I);
                                    MultiLandUnit.Remove(landUnit);
                                }

                                break;
                        }
                }

                break;
            }
        }
    }
}