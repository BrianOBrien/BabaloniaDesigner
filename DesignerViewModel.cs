using System.Collections.ObjectModel;

namespace BabaloniaDesigner;

public class DesignerViewModel
{
    public ObservableCollection<ToolboxItemViewModel> ToolboxItems { get; } =
        new ObservableCollection<ToolboxItemViewModel>();

    public ObservableCollection<TreeNodeViewModel> RootNodes { get; } =
        new ObservableCollection<TreeNodeViewModel>();

    public DesignerViewModel()
    {
        // Toolbox items that should appear on the left
        ToolboxItems.Add(new ToolboxItemViewModel("Button", "Button"));
        ToolboxItems.Add(new ToolboxItemViewModel("TextBox", "TextBox"));
        ToolboxItems.Add(new ToolboxItemViewModel("StackPanel", "StackPanel"));

        // Root node that should appear in the tree
        RootNodes.Add(new TreeNodeViewModel("Root"));
    }

    public void AddNodeFromToolbox(ToolboxItemViewModel item, TreeNodeViewModel? target)
    {
        var node = new TreeNodeViewModel(item.Name);

        if (target is null)
            RootNodes.Add(node);       // drop on empty area → root node
        else
            target.Children.Add(node); // drop on node → child
    }
}
