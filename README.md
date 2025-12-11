# 📐 Babalonia Designer
*A lightweight visual UI designer built with Avalonia UI (.NET 10).*

---

## ✨ Overview

Babalonia Designer is an experimental, live-updating UI layout tool for Avalonia.  
It provides:

- A toolbox of controls  
- A hierarchy tree of the UI structure  
- A property editor bound to the selected element  
- A live preview that updates instantly  

This project serves as a foundation for a future full Avalonia layout editor.

---

## 🧩 Current Features

### ✔ Toolbox
Double-click controls to insert them into the currently selected container.

### ✔ Hierarchy Tree
- Shows Window → StackPanel → Controls  
- Selecting a node updates Properties + Preview  
- New child nodes expand automatically  

### ✔ Property Grid
- Reflective property binding  
- Auto-generated editors  
- Two-column compact layout  

### ✔ Live Preview
- Renders real Avalonia controls  
- Reflects property changes in real time  

### ✔ Dark Theme UI
- Minimalist dark layout  
- Four columns: Controls, Hierarchy, Properties, Preview  

---

## 🏛 Architecture

```
BabaloniaDesigner/
├── App.axaml
├── App.axaml.cs
├── MainWindow.axaml
├── MainWindow.axaml.cs
├── MainWindowViewModel.cs
├── DesignNode.cs
├── ToolboxItem.cs
├── PropertyItem.cs
├── Program.cs
└── BabaloniaDesigner.csproj
```

### Data Flow

Toolbox → ViewModel → DesignNode Tree → Property Grid → Preview

---

## 🚀 Roadmap (Future)

- Drag-and-drop controls into preview  
- Better property editors (Brush, Thickness, Enums)  
- Undo/redo  
- Save/load layout  
- Export to Avalonia XAML  
- Snap lines + alignment indicators  

---

## 🔧 Requirements

- .NET 10 (or .NET 8+ with small adjustments)  
- Avalonia UI (current stable)  

Runs on Linux, Windows, macOS, and Raspberry Pi ARM64.

---

## 🛠 Build

```bash
dotnet restore
dotnet run
```

---

## 🤝 Contributing

Open to ideas, features, UI improvements, and architecture cleanups.

---

## 📜 License

MIT (or whichever you choose).

