using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Media;

namespace BabaloniaDesigner
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ToolboxItem> ToolboxItems { get; } =
            new ObservableCollection<ToolboxItem>();

        public ObservableCollection<DesignNode> RootNodes { get; } =
            new ObservableCollection<DesignNode>();

        private DesignNode? _selectedNode;
        public DesignNode? SelectedNode
        {
            get => _selectedNode;
            set
            {
                if (ReferenceEquals(_selectedNode, value))
                    return;

                // clear previous selection
                if (_selectedNode != null)
                    _selectedNode.IsSelected = false;

                _selectedNode = value;

                if (_selectedNode != null)
                {
                    _selectedNode.IsSelected = true;

                    // expand ancestors
                    var parent = _selectedNode.Parent;
                    while (parent != null)
                    {
                        parent.IsExpanded = true;
                        parent = parent.Parent;
                    }
                }

                OnPropertyChanged(nameof(SelectedNode));
                RebuildSelectedProperties();
                UpdatePreviewRoot();
            }
        }

        public ObservableCollection<PropertyItem> SelectedProperties { get; } =
            new ObservableCollection<PropertyItem>();

        private object? _previewRoot;
        public object? PreviewRoot
        {
            get => _previewRoot;
            set
            {
                if (!ReferenceEquals(_previewRoot, value))
                {
                    _previewRoot = value;
                    OnPropertyChanged(nameof(PreviewRoot));
                }
            }
        }

        public MainWindowViewModel()
        {
            BuildToolbox();
            BuildInitialTree();
        }

        private void BuildToolbox()
        {
            // Controls
            ToolboxItems.Add(new ToolboxItem("Button", typeof(Button)));
            ToolboxItems.Add(new ToolboxItem("TextBlock", typeof(TextBlock)));
            ToolboxItems.Add(new ToolboxItem("TextBox", typeof(TextBox)));
            ToolboxItems.Add(new ToolboxItem("CheckBox", typeof(CheckBox)));
            ToolboxItems.Add(new ToolboxItem("RadioButton", typeof(RadioButton)));
            ToolboxItems.Add(new ToolboxItem("ComboBox", typeof(ComboBox)));
            ToolboxItems.Add(new ToolboxItem("ListBox", typeof(ListBox)));
            ToolboxItems.Add(new ToolboxItem("Slider", typeof(Slider)));
            ToolboxItems.Add(new ToolboxItem("ProgressBar", typeof(ProgressBar)));
            ToolboxItems.Add(new ToolboxItem("Image", typeof(Image)));
            ToolboxItems.Add(new ToolboxItem("Border", typeof(Border)));

            // Layout containers
            ToolboxItems.Add(new ToolboxItem("StackPanel", typeof(StackPanel)));
            ToolboxItems.Add(new ToolboxItem("Grid", typeof(Grid)));
            ToolboxItems.Add(new ToolboxItem("DockPanel", typeof(DockPanel)));
            ToolboxItems.Add(new ToolboxItem("Canvas", typeof(Canvas)));
            ToolboxItems.Add(new ToolboxItem("ScrollViewer", typeof(ScrollViewer)));

            ToolboxItems.Add(new ToolboxItem("TabControl", typeof(TabControl)));
            ToolboxItems.Add(new ToolboxItem("Expander", typeof(Expander)));
        }

        private void BuildInitialTree()
        {
            var rootStack = new StackPanel();

            var window = new Window
            {
                Width = 400,
                Height = 300,
                Content = rootStack
            };

            var windowNode = new DesignNode
            {
                DisplayName = "Window",
                Instance = window,
                Parent = null
            };

            var rootLayoutNode = new DesignNode
            {
                DisplayName = "StackPanel",
                Instance = rootStack,
                Parent = windowNode
            };

            windowNode.Children.Add(rootLayoutNode);
            RootNodes.Add(windowNode);

            PreviewRoot = rootStack;
        }

        // ---- ADD CONTROL FROM TOOLBOX (simplified leaf/container logic) ----
        public void AddControlFromToolbox(ToolboxItem item)
        {
            if (RootNodes.Count == 0)
                return;

            // Root layout (the StackPanel inside the Window)
            var rootLayoutNode = RootNodes.First().Children.FirstOrDefault();
            if (rootLayoutNode == null || rootLayoutNode.Instance is not Panel)
                return;

            // Decide which panel to use as the container
            DesignNode containerNode = rootLayoutNode; // default

            if (SelectedNode != null)
            {
                // If the selected node itself is a Panel, use it
                if (SelectedNode.Instance is Panel)
                {
                    containerNode = SelectedNode;
                }
                // Otherwise, if its parent is a Panel, use the parent (add as sibling)
                else if (SelectedNode.Parent?.Instance is Panel)
                {
                    containerNode = SelectedNode.Parent;
                }
            }

            if (containerNode.Instance is not Panel containerPanel)
                return;

            // Create the new control
            var newControlObject = Activator.CreateInstance(item.ControlType);
            if (newControlObject is not Control control)
                return;

            // Add to visual tree
            containerPanel.Children.Add(control);

            // Add to hierarchy tree
            var node = new DesignNode
            {
                DisplayName = item.Name,
                Instance = control,
                Parent = containerNode
            };
            containerNode.Children.Add(node);

            // Select the newly added control
            SelectedNode = node;
        }

        private static bool IsContainer(object? instance)
        {
            if (instance is Panel)
                return true;

            if (instance is Window)
                return true;

            // some ContentControls (like Border) can act as containers
            if (instance is Border)
                return true;

            return false;
        }

        private void RebuildSelectedProperties()
        {
            SelectedProperties.Clear();

            if (SelectedNode?.Instance == null)
                return;

            var target = SelectedNode.Instance;
            var type = target.GetType();

            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Where(p => p.CanWrite)
                            .Where(p => p.GetIndexParameters().Length == 0);

            foreach (var p in props)
            {
                try
                {
                    SelectedProperties.Add(new PropertyItem(target, p));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Skipping property {p.Name}: {ex.Message}");
                }
            }

            Console.WriteLine($"Added {SelectedProperties.Count} properties for {SelectedNode.DisplayName}");
        }
        private void UpdatePreviewRoot()
        {
            var windowNode = RootNodes.FirstOrDefault();
            if (windowNode?.Instance is Window w && w.Content != null)
            {
                PreviewRoot = w.Content;
            }
            else if (SelectedNode?.Instance != null)
            {
                PreviewRoot = SelectedNode.Instance;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
