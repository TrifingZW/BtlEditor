using Godot;

namespace BtlEditor.GameScreen.Scripts.DataUIScripts.CustomItemList;

public abstract partial class BaseItemList : ItemList
{
    public abstract void Delete();
    public abstract void Add();
    public abstract void Set();
}