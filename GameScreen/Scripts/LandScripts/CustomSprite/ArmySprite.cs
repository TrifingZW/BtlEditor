using System.Collections.Generic;
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

    public override void _Ready()
    {
        SpriteFrames = _spriteFrames;
    }

    public override void _Draw()
    {
        if (Army.等级 is >= 1 and <= 5)
            if (_levelFrames.GetFrameTexture("default", Army.等级 - 1) is { } levelTex)
                DrawTexture(levelTex, new(22f, 10f));

        if (Army.编制 is >= 1 and <= 4)
            if (_stackFrames.GetFrameTexture("default", Army.编制 - 1) is { } stackTex)
                DrawTexture(stackTex, new(-35f, 15f));
    }
}