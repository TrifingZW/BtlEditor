using System.Collections.Generic;
using Godot;

namespace BtlEditor.CoreScripts.Structures;

public class LandUnit
{
    public short Index { get; init; }
    public short RegionIndex { get; init; }
    public int X { get; init; }
    public int Y { get; init; }
    public Vector2 Position { get; init; }
    public Vector2I Coords => new(X, Y);
    public bool Sea => Topography.地块类型 == 1;


    public Topography Topography { get; set; }
    public City City { get; set; }
    public Army CityHp { get; set; }
    public Army Army { get; set; }
    public Pitfall Pitfall { get; set; }
    public Scheme Scheme { get; set; }
    public List<Reinforcement> Reinforcements { get; } = [];
    public List<AirRaid> AirRaids { get; }= [];
    public ArmyPlacement ArmyPlacement { get; set; }
    public Capital Capital { get; set; }
    public short Province { get; set; }
    public byte Belong { get; set; }
}