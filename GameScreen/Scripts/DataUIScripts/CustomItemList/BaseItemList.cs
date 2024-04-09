using Godot;

namespace BtlEditor.GameScreen.Scripts.DataUIScripts.CustomItemList;

public abstract partial class BaseItemList : ItemList
{
    protected static MapController MapController => Game.Instance.MapController;
    public abstract void Delete();
    public abstract void Add();
    public abstract void Set();
    public abstract void Copy();
}