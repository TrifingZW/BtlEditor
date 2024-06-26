﻿using System.Collections.Generic;
using BtlEditor.CoreScripts.Structures;
using Godot;
using static BtlEditor.CoreScripts.TileSetDictionary;

namespace BtlEditor.GameScreen.Scripts.LandScripts.CustomSprite;

public partial class ArmySprite : AnimatedSprite2D
{
    private readonly SpriteFrames _spriteFrames = ResourceLoader.Load<SpriteFrames>("res://Assets/Textures/army_frames.tres");
    private readonly SpriteFrames _levelFrames = ResourceLoader.Load<SpriteFrames>("res://Assets/Textures/level_frames.tres");
    private readonly SpriteFrames _stackFrames = ResourceLoader.Load<SpriteFrames>("res://Assets/Textures/stack_frames.tres");

    private Army _army;

    public Army Army
    {
        get => _army;
        set
        {
            _army = value;
            Frame = ArmyDictionary.GetValueOrDefault(value.兵种, 0);
            FlipH = value.方向 == 1;
            QueueRedraw();
        }
    }

    private bool _select;

    public bool Select
    {
        get => _select;
        set
        {
            _select = value;
            QueueRedraw();
        }
    }

    public override void _Ready()
    {
        SpriteFrames = _spriteFrames;
    }

    public override void _Draw()
    {
        if (Army.盾牌标志 == 9)
            DrawTexture(_levelFrames.GetFrameTexture("default", 5), new(22f, 10f));
        else if (Army.等级 is >= 2 and <= 6)
            if (_levelFrames.GetFrameTexture("default", Army.等级 - 2) is { } levelTex)
                DrawTexture(levelTex, new(22f, 10f));

        if (Army.编制 is >= 1 and <= 4)
            if (_stackFrames.GetFrameTexture("default", Army.编制 - 1) is { } stackTex)
                DrawTexture(stackTex, new(-35f, 15f));

        if (!Select) return;
        Rect2 rect2 = new()
        {
            Position = -new Vector2(100f, 100f) / 2f,
            Size = new(100f, 100f)
        };
        DrawRect(rect2, Colors.Red, false, 5f);
    }
}