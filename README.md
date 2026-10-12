
<div id="header" align="center">
 <kbd>
<img src="https://zupimages.net/up/23/46/jjt8.jpeg" width="250" height="200"></img> 
  </kbd>
<br/>

</div>
<br/>

# ✨ Cheryl UI

<br/>

> Original repo: https://github.com/kikipoulet/CherylUI

**What this fork is:** CherylUI running on **Avalonia 12.1**, with all original controls kept
(public API unchanged) plus a **SukiUI-style fluent dialog API** that you can call straight
from a view model. Two runnable samples and a full control reference ship with it.

## Changed To Avalonia 12.1

> **Building for Avalonia 11?** Use CherylUI `1.2.0` (targets `netstandard2.0` + Avalonia 11.2).

This fork moves the library onto **Avalonia 12.1** and is what you want for any
Avalonia 12.1 application:

| | original repo | this fork |
|---|---|---|
| Avalonia | 11.2 | **12.1.4** |
| Target frameworks | `netstandard2.0` | **`net8.0`**, **`net10.0`** |
| SkiaSharp | 3.0.0-preview | **3.119.4** |
| ReactiveUI | 20.x | **26.0.1** |

Avalonia 12 dropped .NET Standard / .NET Framework support, so the library now
multi-targets `net8.0` and `net10.0` only. Android and iOS consumers must target .NET 10.

Breaking fixes that shipped with the port (they change how you write XAML):
compiled bindings are on by default, `TextBox.Watermark` → `PlaceholderText`
(`UseFloatingWatermark` → `UseFloatingPlaceholder`), `Avalonia.Diagnostics` →
`AvaloniaUI.DiagnosticsSupport`, and `Avalonia.ReactiveUI` → `ReactiveUI.Avalonia`.
The public API of the original controls is otherwise unchanged.

## Fluent dialog API

The library now ships the **SukiUI dialog API**: dialogs are built with a chain of calls and
opened straight from a view model — no code-behind, no service locator, no window reference.

**ViewModel**

```csharp
public class MainViewModel
{
    // The only wiring there is: the view binds a host to this manager.
    public ICherylDialogManager DialogManager { get; } = new CherylDialogManager();

    public void ShowBasic() =>
        DialogManager.CreateDialog()
            .WithTitle("Hello")
            .WithContent("Built entirely from the view model.")
            .WithActionButton("Close", _ => { }, dismissOnClick: true)
            .Dismiss().ByClickingBackground()
            .TryShow();

    public async Task<bool> ConfirmDeleteAsync() =>
        await DialogManager.CreateDialog()
            .WithTitle("Delete file")
            .WithContent("This cannot be undone.")
            .WithYesNoResult("Delete", "Cancel")
            .TryShowAsync();     // true / false
}
```

**View**

```xml
<controls:CherylDialogHost Manager="{Binding DialogManager}" />
```

| | |
|---|---|
| **Chain** | `CreateDialog().WithTitle(..).WithContent(..).WithActionButton(..).Dismiss().ByClickingBackground().TryShow()` |
| **Message boxes** | `.OfType(NotificationType.Information / Success / Warning / Error)` → colored gradient icon bubble |
| **Await the result** | `.WithOkResult("Ok")` or `.WithYesNoResult("Yes", "No")` + `await TryShowAsync()` (also accepts a `CancellationToken`) |
| **Any content** | a `Control`, a plain string, or a view model (`.WithViewModel(..)`) |
| **Styling** | 25 px rounded glass card, centered 22 px title, right-aligned action buttons, dimmed backdrop, scale-in transition |
| **Extras over SukiUI** | `.AtBottom()` bottom sheet, and `InteractiveContainer.DialogManager` for zero-config usage |

The manager keeps the "one dialog at a time" contract: `TryShow()` returns `false`
(and `TryShowAsync()` throws) when a dialog is already open, while a dialog without
`.Dismiss().ByClickingBackground()` is *pinned* — clicking the backdrop only bounces it.

The previous static API — `InteractiveContainer.ShowDialog(...)` / `CloseDialog()` /
`ShowToast(...)` — is untouched and still works.
