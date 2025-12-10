namespace BabaloniaDesigner;

public class ToolboxItemViewModel
{
    public string Name { get; }
    public string TypeId { get; }

    public ToolboxItemViewModel(string name, string typeId)
    {
        Name = name;
        TypeId = typeId;
    }

    public override string ToString() => Name;
}
