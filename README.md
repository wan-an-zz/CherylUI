
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
