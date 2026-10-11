
<div id="header" align="center">
 <kbd>
<img src="https://zupimages.net/up/23/46/jjt8.jpeg" width="250" height="200"></img> 
  </kbd>
<br/>

</div>
<br/>

# ✨ Cheryl UI

<br/>

## 🚀 Version 2.0 — Avalonia 12.1

> **Building for Avalonia 11?** Use CherylUI `1.2.0` (targets `netstandard2.0` + Avalonia 11.2).

CherylUI 2.0 moves the library onto **Avalonia 12.1** and is what you want for any
Avalonia 12.1 application:

| | CherylUI 1.2 | CherylUI 2.0 |
|---|---|---|
| Avalonia | 11.2 | **12.1.4** |
| Target frameworks | `netstandard2.0` | **`net8.0`**, **`net10.0`** |
| SkiaSharp | 3.0.0-preview | **3.119.4** |
| ReactiveUI | 20.x | **26.0.1** |

Avalonia 12 dropped .NET Standard / .NET Framework support, so the library now
multi-targets `net8.0` and `net10.0` only. Android and iOS consumers must target .NET 10.

```xml
<PackageReference Include="CherylUI" Version="2.0.0" />
```

Then pull the styles into your `App.axaml`:

```xml
<Application.Styles>
    <StyleInclude Source="avares://CherylUI/Index.axaml" />
</Application.Styles>
```

### Migrating an existing CherylUI 1.x app

* Change `TextBox.Watermark` to `TextBox.PlaceholderText` — the old name is obsolete in Avalonia 12.
* `Avalonia.ReactiveUI` was discontinued upstream: use
  [`ReactiveUI.Avalonia`](https://github.com/reactiveui/ReactiveUI.Avalonia) and call
  `.UseReactiveUI(rxui => { })` instead of `.UseReactiveUI()`.
* `Avalonia.Diagnostics` was removed: use `AvaloniaUI.DiagnosticsSupport` if you need DevTools.
* Compiled bindings are on by default, so add `x:DataType` to any view that binds to a view model.

<br/>

This library is curently not under active development.

###  Please check the port of CherylUI for Uno Platform that is more performant and complete : 
### 👉 [CherylUI.Uno](https://github.com/kikipoulet/CherylUI.Uno)



<br/>
<br/>






## 1.5 Release


### Mobile UI Library for AvaloniaUI ! 

[Install CherylUI](https://github.com/kikipoulet/CherylUI/wiki/1.-Installation) 

[Controls Documentation](https://github.com/kikipoulet/CherylUI/wiki/2.-Controls) 

<br/>

![151](https://github.com/user-attachments/assets/af48ca42-d49a-464b-bfda-2a748252d8b8)


![152](https://github.com/user-attachments/assets/ee8792ae-f55d-4b62-88b5-c9aa3e487567)
