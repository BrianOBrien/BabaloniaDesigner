using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace BabaloniaDesigner;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        Console.WriteLine("MainWindow ctor: initializing");
        InitializeComponent();

        Console.WriteLine($"MainWindow ctor: ToolboxList is {(ToolboxList == null ? "NULL" : "NOT NULL")}");
        Console.WriteLine($"MainWindow ctor: HierarchyTree is {(HierarchyTree == null ? "NULL" : "NOT NULL")}");

        // Make sure we always see pointer events from the toolbox
        if (ToolboxList is not null)
        {
            ToolboxList.AddHandler(
                InputElement.PointerPressedEvent,
                ToolboxList_OnPointerPressed,
                RoutingStrategies.Tunnel | RoutingStrategies.Bubble);

            Console.WriteLine("MainWindow ctor: hooked ToolboxList PointerPressed via AddHandler");
        }

        if (HierarchyTree is not null)
        {
            HierarchyTree.AddHandler(DragDrop.DragOverEvent, HierarchyTree_OnDragOver);
            HierarchyTree.AddHandler(DragDrop.DropEvent, HierarchyTree_OnDrop);
            Console.WriteLine("MainWindow ctor: hooked HierarchyTree DragOver/Drop");
        }

        DataContext = new DesignerViewModel();
        Console.WriteLine("MainWindow ctor: DataContext set");
    }

    // --------------------
    //  TOOLBOX → DRAG
    // --------------------
    private async void ToolboxList_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        Console.WriteLine("ToolboxList_OnPointerPressed: entered");

        if (sender is not ListBox listBox)
        {
            Console.WriteLine("ToolboxList_OnPointerPressed: sender is not ListBox");
            return;
        }

        var pt = e.GetCurrentPoint(listBox);
        if (!pt.Properties.IsLeftButtonPressed)
        {
            Console.WriteLine("ToolboxList_OnPointerPressed: left button not pressed");
            return;
        }

        var pos = e.GetPosition(listBox);
        Console.WriteLine($"ToolboxList_OnPointerPressed: pos={pos}");

        var hit = listBox.GetVisualAt(pos);
        if (hit == null)
        {
            Console.WriteLine("ToolboxList_OnPointerPressed: hit is null");
            return;
        }

        var item = hit
            .GetSelfAndVisualAncestors()
            .OfType<ListBoxItem>()
            .FirstOrDefault();

        if (item?.DataContext is not ToolboxItemViewModel toolboxItem)
        {
            Console.WriteLine("ToolboxList_OnPointerPressed: no ToolboxItemViewModel under pointer");
            return;
        }

        Console.WriteLine($"ToolboxList_OnPointerPressed: starting drag for '{toolboxItem.Name}'");

        // Old but reliable API: DataObject + DoDragDrop
        var data = new DataObject();
        data.Set("toolbox-item", toolboxItem);

        var result = await DragDrop.DoDragDrop(e, data, DragDropEffects.Copy);
        Console.WriteLine($"ToolboxList_OnPointerPressed: DoDragDrop result={result}");
    }

    // --------------------
    //  TREE → DRAG OVER
    // --------------------
    private void HierarchyTree_OnDragOver(object? sender, DragEventArgs e)
    {
        Console.WriteLine("HierarchyTree_OnDragOver: entered");

        if (e.Data.Contains("toolbox-item"))
        {
            e.DragEffects = DragDropEffects.Copy;
            Console.WriteLine("HierarchyTree_OnDragOver: allowing Copy");
        }
        else
        {
            e.DragEffects = DragDropEffects.None;
            Console.WriteLine("HierarchyTree_OnDragOver: denying (no toolbox-item)");
        }

        e.Handled = true;
    }

    // --------------------
    //  TREE → DROP
    // --------------------
    private void HierarchyTree_OnDrop(object? sender, DragEventArgs e)
    {
        Console.WriteLine("HierarchyTree_OnDrop: entered");

        if (!e.Data.Contains("toolbox-item"))
        {
            Console.WriteLine("HierarchyTree_OnDrop: no 'toolbox-item' in e.Data");
            return;
        }

        if (DataContext is not DesignerViewModel vm)
        {
            Console.WriteLine("HierarchyTree_OnDrop: DataContext is not DesignerViewModel");
            return;
        }

        var obj = e.Data.Get("toolbox-item");
        if (obj is not ToolboxItemViewModel toolboxItem)
        {
            Console.WriteLine("HierarchyTree_OnDrop: data is not ToolboxItemViewModel");
            return;
        }

        Console.WriteLine($"HierarchyTree_OnDrop: dropping '{toolboxItem.Name}'");

        TreeNodeViewModel? target = null;

        if (sender is TreeView tree)
        {
            var pos = e.GetPosition(tree);
            Console.WriteLine($"HierarchyTree_OnDrop: drop pos={pos}");

            var hit = tree.GetVisualAt(pos);
            var tvi = hit?
                .GetSelfAndVisualAncestors()
                .OfType<TreeViewItem>()
                .FirstOrDefault();

            if (tvi?.DataContext is TreeNodeViewModel nodeVm)
            {
                Console.WriteLine($"HierarchyTree_OnDrop: dropping onto node '{nodeVm.Name}'");
                target = nodeVm;
            }
            else
            {
                Console.WriteLine("HierarchyTree_OnDrop: dropping on empty area → root");
            }
        }

        vm.AddNodeFromToolbox(toolboxItem, target);
        Console.WriteLine("HierarchyTree_OnDrop: AddNodeFromToolbox completed");
        e.Handled = true;
    }

    // --------------------
    //  DEBUG BUTTON
    // --------------------
    private void DebugAddButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine("DebugAddButton_OnClick: fired");

        if (DataContext is not DesignerViewModel vm)
        {
            Console.WriteLine("DebugAddButton_OnClick: DataContext is not DesignerViewModel");
            return;
        }

        var toolboxItem = vm.ToolboxItems.FirstOrDefault();
        var rootNode = vm.RootNodes.FirstOrDefault();

        if (toolboxItem == null || rootNode == null)
        {
            Console.WriteLine("DebugAddButton_OnClick: toolboxItem or rootNode is null");
            return;
        }

        vm.AddNodeFromToolbox(toolboxItem, rootNode);
        Console.WriteLine("DebugAddButton_OnClick: node added");
    }
}
