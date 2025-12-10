using System.Collections.ObjectModel;

namespace BabaloniaDesigner;

public class TreeNodeViewModel
{
    public string Name { get; set; }

    public ObservableCollection<TreeNodeViewModel> Children { get; } =
        new ObservableCollection<TreeNodeViewModel>();

    public TreeNodeViewModel(string name)
    {
        Name = name;
    }
}
