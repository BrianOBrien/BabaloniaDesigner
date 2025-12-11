using System;

namespace BabaloniaDesigner
{
    public class ToolboxItem
    {
        public string Name { get; }
        public Type ControlType { get; }

        public ToolboxItem(string name, Type controlType)
        {
            Name = name;
            ControlType = controlType;
        }
    }
}
