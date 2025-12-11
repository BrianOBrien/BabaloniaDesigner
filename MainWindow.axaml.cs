using System;
using System.ComponentModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;

namespace BabaloniaDesigner;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var vm = new MainWindowViewModel();
        DataContext = vm;

        // Keep TreeView selection in sync with the view-model
        vm.PropertyChanged += VmOnPropertyChanged;

        Opened += OnOpened;
    }
    private void ScrollPropertiesToTop()
    {
        if (PropertiesListBox == null)
            return;

        var first = PropertiesListBox.Items?.Cast<object>().FirstOrDefault();
        if (first != null)
        {
            PropertiesListBox.ScrollIntoView(first);
        }
    }
    /// <summary>
    /// When the window opens, select a sensible default node so
    /// the preview and properties panel are not empty.
    /// </summary>
    private void OnOpened(object? sender, EventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm)
            return;

        if (vm.RootNodes.Count == 0)
            return;

        // RootNodes[0] is the Window node.
        var windowNode = vm.RootNodes[0];
        var firstChild = windowNode.Children.FirstOrDefault();

        vm.SelectedNode = firstChild ?? windowNode;
    }

    /// <summary>
    /// Double-click on toolbox item: add a control via the VM.
    /// </summary>
    private void ToolboxListBox_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm)
            return;

        if (ToolboxListBox.SelectedItem is ToolboxItem item)
        {
            vm.AddControlFromToolbox(item);
        }
    }

    /// <summary>
    /// User clicked in the hierarchy tree: push selection to VM.
    /// </summary>
    private void HierarchyTree_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm)
            return;

        if (HierarchyTree.SelectedItem is DesignNode node)
        {
            vm.SelectedNode = node;
        }
    }

    /// <summary>
    /// VM changed SelectedNode (e.g. after AddControlFromToolbox):
    /// update TreeView.SelectedItem so the hierarchy visually follows.
    /// </summary>
    private void VmOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(MainWindowViewModel.SelectedNode))
            return;

        if (DataContext is not MainWindowViewModel vm)
            return;

        if (vm.SelectedNode != null)
        {
            HierarchyTree.SelectedItem = vm.SelectedNode;
        }

        // Make sure properties view starts at the top for the new selection
        ScrollPropertiesToTop();
    }
}
