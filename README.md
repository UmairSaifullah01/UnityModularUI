# UnityModularUI

A modular, extensible user interface system for Unity projects, designed for rapid development of MVVM-based UIs and state-driven application flows.

## Features
- **MVVM Architecture**: Clean separation of Model, View, and ViewModel for scalable UI code.
- **State Machine**: Powerful UI state management with transitions and conditions.
- **Custom Editor Tooling**: 
  - Enhanced inspector for ApplicationFlow and UIStateRecord.
  - Beautiful, table-like property drawer for UITransition arrays, with headers, checkboxes, and a "Run" button for each transition.
- **Component Library**: Ready-to-use UI components (Button, Dropdown, TabView, etc.).
- **Safe Area Support**: UI adapts to device safe areas.
- **Sample Scenes**: Example usage included.

## Installation

### Via Unity Package Manager (UPM)
1. Open `manifest.json` in your Unity project's `Packages` folder.
2. Add:
   ```json
   "com.thebaddest.unitymodulerui": "https://github.com/UmairSaifullah01/UnityModularUI.git"
   ```
3. Save and return to Unity. The package will be imported automatically.

### Manual Import
1. Download the latest release from [GitHub Releases](https://github.com/UmairSaifullah01/UnityModularUI/releases).
2. In Unity: `Assets > Import Package > Custom Package...` and select the downloaded file.

## Getting Started

1. **Add a UIVolume**: Create an empty GameObject and add the `UIVolume` component. This manages your UI state machine.
2. **Create UI Panels**: Inherit from `UIPanel` for your custom UI screens. Implement state logic and transitions.
3. **Define Application Flow**: Use the `ApplicationFlow` asset to visually manage your UI states and transitions. The inspector now features a table-like view for transitions, with easy add/remove and a "Run" button for testing transitions.
4. **MVVM Bindings**: Implement `IView`, `IViewModel`, and `IModel` for data-driven UI.
5. **Use Provided Components**: Drag and drop ready-made UI components from the `Components` folder.

## Editor Tooling
- **UITransition Property Drawer**: Transitions are now shown in a clear, tabular format with column headers for bools and a "Run" button for each transition. You can add/remove transitions as usual.
- **No setup required**: The editor scripts are included and work automatically when you select ApplicationFlow assets.

## Example
```csharp
public class MainMenuPanel : UIPanel { /* ... */ }

// In ApplicationFlow asset:
// - Add states and transitions in the inspector.
// - Use the "Run" button to test transitions in play mode.
```

## Contributing
Pull requests are welcome! For major changes, please open an issue first to discuss what you would like to change.

## License
[MIT](LICENSE)
