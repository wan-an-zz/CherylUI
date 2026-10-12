# CherylUI 控件使用文档

> **适用版本**：CherylUI **2.0.0**（Avalonia **12.1.4** / .NET 8+）
> **文档依据**：以**本仓库当前源码**为主要依据；控件设计意图与命名参考原仓库 wiki（[kikipoulet/CherylUI](https://github.com/kikipoulet/CherylUI/wiki)）与**本仓库可运行示例** `CherylCrossTest/`。
> 原版 wiki 只有 3 个页面、17 个最简 XAML 片段，**没有任何属性表**；本文中的属性表全部从源码整理，并标注了注册方式与默认值。

---

## 目录

- [1. 本 fork 与原版的差异](#1-本-fork-与原版的差异)
- [2. 安装与初始化](#2-安装与初始化)
- [3. 命名空间速查表](#3-命名空间速查表)
- [4. 布局与导航](#4-布局与导航)
  - [4.1 InteractiveContainer —— 对话框 / Toast 宿主](#41-interactivecontainer--对话框--toast-宿主)
  - [4.2 MobileNavigation —— 页面栈导航](#42-mobilenavigation--页面栈导航)
  - [4.3 SliverPage](#43-sliverpage)
  - [4.4 SliverPageLong](#44-sliverpagelong)
  - [4.5 BusyArea](#45-busyarea)
  - [4.6 Loading](#46-loading)
  - [4.7 GroupBox](#47-groupbox)
  - [4.8 HorizontalSeparator](#48-horizontalseparator)
- [5. 玻璃拟态（GlassMorphism）](#5-玻璃拟态glassmorphism)
  - [5.1 GlassCard](#51-glasscard)
  - [5.2 CherylBackground](#52-cherylbackground)
  - [5.3 BlurBackground](#53-blurbackground)
- [6. 移动端选择器](#6-移动端选择器)
  - [6.1 MobilePicker](#61-mobilepicker)
  - [6.2 MobileNumberPicker](#62-mobilenumberpicker)
- [7. 表单行（FormFields）](#7-表单行formfields)
- [8. 内置样式化的 Avalonia 原生控件](#8-内置样式化的-avalonia-原生控件)
- [9. 图标资源](#9-图标资源)
- [10. 主题与颜色资源](#10-主题与颜色资源)
- [11. 动画扩展方法](#11-动画扩展方法)
- [12. 已知问题与注意事项](#12-已知问题与注意事项)
- [13. 从原版（Avalonia 11）迁移检查清单](#13-从原版avalonia-11迁移检查清单)
- [14. 链式对话框（SukiUI 同款 Fluent Dialog API）](#14-链式对话框sukiui-同款-fluent-dialog-api)

---

## 1. 本 fork 与原版的差异

| | 原仓库 | 本 fork |
|---|---|---|
| Avalonia | 11.2 | **12.1.4** |
| 目标框架 | `netstandard2.0` | **`net8.0`；`net10.0`** |
| SkiaSharp | 3.0.0-preview | **3.119.4** |
| ReactiveUI | 20.x | **26.0.1** |
| 包版本 | 1.2.0（升级前 `PackageVersion`） | **2.0.0** |

Avalonia 12 移除了 .NET Standard / .NET Framework 支持，因此库只多目标 `net8.0` 与 `net10.0`；**Android / iOS 消费者必须使用 .NET 10**。

升级过程中做的破坏性修正（会影响你写 XAML 的方式）：

1. **编译绑定在 Avalonia 12 中默认开启**。用 `{Binding}` 的视图需要 `x:DataType` 或显式 `x:CompileBindings="False"`，否则编译期报错。（示例工程用 `<AvaloniaUseCompiledBindingsByDefault>true</AvaloniaUseCompiledBindingsByDefault>` 显式声明。）
2. **`TextBox.Watermark` → `PlaceholderText`**，`UseFloatingWatermark` → `UseFloatingPlaceholder`。
3. **`GetTemplateChildren()` → `GetTemplateDescendants()`**（`MobileNavigation` 内部）。
4. **`Avalonia.Diagnostics` 包已不存在**；开发工具改用 `AvaloniaUI.DiagnosticsSupport`。
5. **`Avalonia.ReactiveUI` 停止维护**，继任者是 `ReactiveUI.Avalonia`（`UseReactiveUI(rxui => { })`）。

> **公开 API 与原版一致**：除上述内部改名外，所有控件的类型名、属性名、静态方法签名均未变化。原版 wiki 的用法示例在本 fork 上依然适用，只是要把 `Watermark` 改成 `PlaceholderText`。

---

## 2. 安装与初始化

### 2.1 引用库

```xml
<PackageReference Include="CherylUI" Version="2.0.0" />
```

或以项目引用的方式使用本仓库：`<ProjectReference Include="..\CherylUI\CherylUI.csproj" />`

`CherylUI.csproj` 开启了 `GeneratePackageOnBuild`，直接 build 会产出 `.nupkg`。

### 2.2 在 `App.axaml` 中挂载样式

```xml
<Application xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="YourApp.App"
             RequestedThemeVariant="Default">
    <Application.Styles>
        <StyleInclude Source="avares://CherylUI/Index.axaml" />
    </Application.Styles>
</Application>
```

`avares://CherylUI/Index.axaml` 一次性完成三件事：

1. 注册 **Light / Dark 两套颜色字典**（`ResourceDictionary.ThemeDictionaries`）与 `HarmonyFont`（Quicksand 字体）；
2. 合并 `Icons.axaml`（图标 `StreamGeometry`）与 `DatePickerStyles.axaml`（DatePicker 的 `ControlTheme`）；
3. **内置 `<SimpleTheme />`**，然后 include Button / Card / CheckBox / RadioButton / ToggleButton / ListBox / Slider / TextBox / TabControl / ProgressBar / Text 全部样式。

> ⚠️ **不要再额外引入 FluentTheme 或 SimpleTheme**。`Index.axaml` 已经带了 `<SimpleTheme />`；重复引入会让控件模板被后加载的主题覆盖掉 CherylUI 样式。

### 2.3 在视觉树根部放一个 `InteractiveContainer`

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:controls="clr-namespace:CherylUI.Controls;assembly=CherylUI"
             xmlns:views="clr-namespace:YourApp.Views"
             x:Class="YourApp.Views.MainView">
    <controls:InteractiveContainer>
        <!-- 你的 App 内容 -->
        <views:MenuControl />
    </controls:InteractiveContainer>
</UserControl>
```

`InteractiveContainer` 是**整个库的运行时宿主**：

- 它的模板里包含 `CherylBackground`（动态噪声渐变背景）；
- 它的模板里包含**唯一的 `MobileNavigation` 实例**，`MobileNavigation.Push/Pop` 就是操作它；
- 它负责渲染 Dialog 与 Toast。

如果不放这个控件，调用 `InteractiveContainer.*` 或 `MobileNavigation.*` 会抛：

```
You are trying to use a InteractiveContainer functionality without declaring one !
```

### 2.4 主题切换

```csharp
// 跟随系统：Application.Current.RequestedThemeVariant = ThemeVariant.Default;
Application.Current.RequestedThemeVariant = ThemeVariant.Dark;   // 或 ThemeVariant.Light
```

切换后 `CherylBackground` 会监听 `ActualThemeVariantChanged` 并重绘背景噪声。

### 2.5 最小可运行骨架

```csharp
// App.axaml.cs
public override void OnFrameworkInitializationCompleted()
{
    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        desktop.MainWindow = new MainWindow { DataContext = new MainViewModel() };
    else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
        singleView.MainView = new MainView { DataContext = new MainViewModel() };

    base.OnFrameworkInitializationCompleted();
}
```

---

## 3. 命名空间速查表

CherylUI 的类型分散在**多个 CLR 命名空间**，且部分类型所在命名空间与目录名不一致。这是最常见的编译错误来源，请直接照抄下表。

| 类型 | CLR 命名空间 | 建议 xmlns |
|---|---|---|
| `InteractiveContainer`、`MobileNavigation`、`SliverPage`、`SliverPageLong`、`BusyArea`、`GroupBox`、`Loading` | `CherylUI.Controls` | `xmlns:controls="clr-namespace:CherylUI.Controls;assembly=CherylUI"` |
| `MobilePicker`、`MobilePickerPopUp`、`MobilePickerPopUpVM` | `CherylUI.Controls.MobilePicker` | `xmlns:mobilePicker="clr-namespace:CherylUI.Controls.MobilePicker;assembly=CherylUI"` |
| `MobileNumberPicker`、`MobileNumberPickerPopup`、`IntToStringConverter` | `CherylUI.Controls.MobileNumberPicker` | `xmlns:mobileNumberPicker="clr-namespace:CherylUI.Controls.MobileNumberPicker;assembly=CherylUI"` |
| `FormFieldSwitch`、`FormFieldPicker`、`FormFieldNumberPicker`、`FormFieldAction` | `CherylUI.Controls.FormFields` | `xmlns:formFields="clr-namespace:CherylUI.Controls.FormFields;assembly=CherylUI"` |
| `GlassCard`、`CherylBackground` | `CherylUI.Controls.GlassMorphism` | `xmlns:glassMorphism="clr-namespace:CherylUI.Controls.GlassMorphism;assembly=CherylUI"` |
| ⚠️ `BlurBackground` | **`CherylUI.Glassmorphism`**（注意大小写，且**没有** `.Controls`） | `xmlns:glassmorphism="clr-namespace:CherylUI.Glassmorphism;assembly=CherylUI"` |
| ⚠️ `HorizontalSeparator` | **`CherylUI`**（根命名空间，不是 `CherylUI.Controls`） | `xmlns:cherylUi="clr-namespace:CherylUI;assembly=CherylUI"` |
| `AnimationHelper`（扩展方法） | `CherylUI` | `using CherylUI;` |
| `TextEraserButton`、`StringToDoubleConverter` | `CherylUI.Styles` | `xmlns:styles="clr-namespace:CherylUI.Styles;assembly=CherylUI"` |
| 转换器 `OffsetToHeightConverter` 等 | `CherylUI.Controls` | 同上 `controls:` |

**属性注册方式的含义**（下文属性表会标注）：

- **StyledProperty**：可以参与样式（`<Setter Property="X">`）、可以继承、有默认值。
- **DirectProperty**：值直接存在 CLR 字段上，`SetAndRaise` 通知变更。**不能**被 Style / ControlTheme 用 `<Setter>` 设置，只能直接赋值或数据绑定；**全部是 TwoWay 默认绑定**。

---

## 4. 布局与导航

### 4.1 InteractiveContainer —— 对话框 / Toast 宿主

全局单例式的宿主控件，用**静态方法**驱动，不需要自己 `new`。

> **想用链式 API 打开对话框（SukiUI 同款）？** 见 [第 14 节](#14-链式对话框sukiui-同款-fluent-dialog-api)。
> `InteractiveContainer` 的模板里已经内置了一个 `CherylDialogHost`（接 `CherylDialogManager.Default`），
> 所以 `InteractiveContainer.DialogManager.CreateDialog()...TryShow()` 零配置即可用；本节这套 `ShowDialog` 老 API 仍然保留、互不影响。

**XAML 属性**

| 属性 | 类型 | 默认值 | 注册 | 说明 |
|---|---|---|---|---|
| `ShowAtBottom` | `bool` | `false` | Styled | 对话框从底部弹出（`VerticalAlignment=Bottom`）还是居中 |
| `IsDialogOpen` | `bool` | `false` | Styled | 对话框开关；为 `true` 时给内容加 `blur(20)` 与半透明遮罩 |
| `IsToastOpen` | `bool` | `false` | Styled | Toast 开关（淡入 + 从上方滑入） |
| `DialogContent` | `Control` | `new Grid()` | Styled | 对话框内容 |
| `ToastContent` | `Control` | `new Grid()` | Styled | Toast 内容 |

`Content`（来自 `UserControl`）就是你的主内容，会被放进内部的 `MobileNavigation`。

**静态方法**

| 方法 | 说明 |
|---|---|
| `ShowDialog(Control content, bool showAtBottom = false)` | 打开对话框。**不会自动关闭**，必须自己调用 `CloseDialog()` |
| `CloseDialog()` | 关闭对话框 |
| `ShowToast(Control message, int seconds)` | 在顶部显示 Toast，`seconds` 秒后自动关闭 |
| `WaitUntilDialogIsClosed()` | ⚠️ 用 `Thread.Sleep` 轮询阻塞当前线程，已废弃，见 [第 12 节](#12-已知问题与注意事项) |

**示例**

```xml
<!-- DialogContent.axaml -->
<UserControl Height="150" Width="250" ...>
    <Grid>
        <TextBlock HorizontalAlignment="Center" VerticalAlignment="Center"
                   Margin="0,0,0,35" Text="This is a dialog." />
        <Button Classes="Text" Click="Button_OnClick" VerticalAlignment="Bottom">
            <TextBlock Text="Close" />
        </Button>
    </Grid>
</UserControl>
```

```csharp
// 打开居中对话框
InteractiveContainer.ShowDialog(new DialogContent());

// 从底部弹出
InteractiveContainer.ShowDialog(new DialogContent(), showAtBottom: true);

// 关闭
InteractiveContainer.CloseDialog();

// Toast：第二个参数单位是秒
InteractiveContainer.ShowToast(new TextBlock { Text = "Hit !" }, 3);
```

**对话框的视觉行为**（由模板样式决定，便于你覆盖）：

| 状态 | 效果 |
|---|---|
| `IsDialogOpen=True` | 内容层 `Opacity=0.5` + `blur(20)`，遮罩 `GridDialogBackground` 淡入到 0.3，对话框 `scale(1)` 淡入 |
| `IsDialogOpen=False` | 对话框 `scale(0.7)`、`Opacity=0`、`Margin="0,125,0,0"` |
| `ShowAtBottom=True` | 对话框 `VerticalAlignment=Bottom`、`Margin="0,0,0,20"` |

**Dialog / Toast 的默认外观**：圆角 30（Toast）、圆角 20（Dialog），背景 `{DynamicResource DialogBackground}`，边框 `{DynamicResource ComponentBackground}`。

> **注意**：`GetInteractiveContainerInstance()` 取的是视觉树里**第一个** `InteractiveContainer`，因此全局**只能有一个**。

---

### 4.2 MobileNavigation —— 页面栈导航

`MobileNavigation` 也是静态方法驱动的（它自动定位到 `InteractiveContainer` 模板里那个实例）。

**实例成员**

| 成员 | 类型 | 说明 |
|---|---|---|
| `Pages` | `Stack<Control>` | 页面栈 |
| `CurrentPage` | `Control` 字段 | 当前页（**字段**，不是属性） |
| `PopPage()` | 方法 | 实例版出栈 |

**静态方法**

| 方法 | 说明 |
|---|---|
| `Push(UserControl content, bool DisableComeBack = false)` | 把当前页压栈并显示 `content`。⚠️ 参数类型是 **`UserControl`**，不是 `Control` |
| `Pop()` | 弹出栈顶并显示；**栈为空时什么都不做** |
| `GetMobileStackInstance()` | 取到那个 `MobileNavigation` 实例 |

```csharp
MobileNavigation.Push(new SettingsPage());               // 进入新页
MobileNavigation.Push(new HomePage(), DisableComeBack: true); // 清空历史（登录后跳主页）
MobileNavigation.Pop();                                  // 返回上一页
```

> `DisableComeBack: true` 会 `Pages.Clear()`，效果是这一页成为新的栈底。

**Android 返回键**：`InteractiveContainer` 在 `Loaded` 时订阅了 `TopLevel.BackRequested`；当页面栈非空时自动 `MobileNavigation.Pop()` 并 `e.Handled = true`；栈空时交还系统默认行为（退出应用）。

---

### 4.3 SliverPage

Android 风格的「折叠标题栏 + 返回按钮 + 内置 ScrollViewer」页面容器。向下滚动时标题栏高度从 170 收缩到 100，标题字号从 45 缩到 25，并带实时背景模糊。

```xml
<controls:SliverPage Header="Log in" IsBackVisible="True">
    <StackPanel Margin="13,20">
        <TextBlock Text="内容" />
    </StackPanel>
</controls:SliverPage>
```

| 属性 | 类型 | 默认值 | 注册 | 说明 |
|---|---|---|---|---|
| `Header` | `string` | `"Header"` | Styled | 标题文字 |
| `IsBackVisible` | `bool` | **`false`** | Styled | 是否显示左上返回箭头。**默认不显示，要自己开** |

- 内容区自带 `ScrollViewer`（`IsScrollInertiaEnabled=True`，隐藏滚动条），并预留了 170px 的头部高度。
- **返回箭头点击时自动调用 `MobileNavigation.Pop()`**，所以 `SliverPage` 要和 `InteractiveContainer` + `MobileNavigation` 一起用，否则点返回没有效果。
- 模板内的转换器 `OffsetToHeightConverter` / `OffsetToMarginConverter` / `OffsetToFontSizeConverter` / `OffsetToMarginScrollConverter` 定义在 `CherylUI.Controls` 命名空间，可自行复用。

---

### 4.4 SliverPageLong

`SliverPage` 的「大标题」变体：头部高 337px，标题居中 45px；滚动后渐变为左对齐 25px 的小标题，便于拇指区操作。

```xml
<controls:SliverPageLong Header="Settings">
    <StackPanel>
        <!-- 内容 -->
    </StackPanel>
</controls:SliverPageLong>
```

| 属性 | 类型 | 默认值 | 注册 | 说明 |
|---|---|---|---|---|
| `Header` | `string` | `"Header"` | Styled | 标题文字 |

- **没有 `IsBackVisible` 属性**，返回按钮**始终显示**，点击同样调用 `MobileNavigation.Pop()`。
- 模板内的转换器：`OffsetToHeightConverterLong`、`OffsetToOpacityConverterLong`、`OffsetToInvertOpacityConverterLong`（均在 `CherylUI.Controls`）。

---

### 4.5 BusyArea

「内容变暗 + 中央转圈」的忙碌遮罩。

```xml
<controls:BusyArea IsBusy="{Binding IsLoading}">
    <StackPanel>
        <!-- 你的内容 -->
    </StackPanel>
</controls:BusyArea>
```

| 属性 | 类型 | 默认值 | 注册 | 说明 |
|---|---|---|---|---|
| `IsBusy` | `bool` | `false` | Styled | `true` 时内容 `Opacity=0.1`、中央 `Loading` 淡入；`false` 时反过来 |

- 转圈部分即 `Loading` 控件，`IsHitTestVisible="False"`，不会挡住点击。
- `Loading` 自带 0.35s 的 Opacity 过渡，切换是平滑的。

---

### 4.6 Loading

旋转弧线加载指示器。**没有公开属性**。

```xml
<controls:Loading />
<controls:Loading Classes="White" />   <!-- 纯白描边，用于深色/彩色背景 -->
```

| 样式类 | 效果 |
|---|---|
| （无） | 使用 `RadialGradientBrush`，渐变停止点取 `{DynamicResource SukiPrimaryColor}` / `{DynamicResource SukiPrimaryColorGradient}` |
| `White` | 把模板内的 `Arc` 描边写死为 `#fafafa` |

- 动画：整体旋转 0%→30% 停留→100% 转 360°（1.5s 无限循环）；同时 `SweepAngle` 在 60→250→60 之间呼吸（1.5s）。
- 尺寸固定：52×52，`StrokeThickness=4`，`StrokeLineCap=Round`。

> ⚠️ **重要**：`SukiPrimaryColor` 与 `SukiPrimaryColorGradient` 这两个资源键**在本仓库任何文件中都没有定义**（原版也是如此）。不定义的话默认弧形是透明的，看起来就是「不显示」。两个解决办法：
>
> ```xml
> <!-- 在 App.axaml 的 Application.Resources 里补上（颜色值可用 Accent） -->
> <Application.Resources>
>     <Color x:Key="SukiPrimaryColor">#0A59F7</Color>
>     <Color x:Key="SukiPrimaryColorGradient">#317AF7</Color>
> </Application.Resources>
> ```
>
> 或者直接 `<controls:Loading Classes="White" />`。

---

### 4.7 GroupBox

设置页常用的分组容器：一个 `Secondary` 小标题 + 一张 `GlassCard`。

```xml
<controls:GroupBox Header="View mode">
    <StackPanel Margin="10,2">
        <formFields:FormFieldSwitch Title="Dark Mode" />
        <cherylUi:HorizontalSeparator />
        <formFields:FormFieldAction Title="Eye Comfort" Command="{Binding LaunchToast}" />
    </StackPanel>
</controls:GroupBox>
```

| 属性 | 类型 | 默认值 | 注册 | 说明 |
|---|---|---|---|---|
| `Header` | `string` | `"Header"` | Styled | 分组标题 |
| `Text` | `string?` | — | Styled（`TextBlock.TextProperty` 的 AddOwner） | ⚠️ 模板中**未使用**，历史遗留，别用 |

- 内容会被包进 `GlassCard`（`Padding="0,10"`，内部 `Grid Margin="10,-6"`），所以**行间距靠你自己的 `StackPanel` 控制**。
- 分组之间建议用 `<Border Height="120" />` 之类的留白（示例工程的做法）。

---

### 4.8 HorizontalSeparator

1 像素分隔线，颜色 `{DynamicResource ListSeparator}`。

```xml
<cherylUi:HorizontalSeparator />
```

> ⚠️ 命名空间是 **`clr-namespace:CherylUI`**（根），不是 `CherylUI.Controls`。写错会报「找不到类型」。

无任何属性。高度固定 1，宽度随容器。

---

## 5. 玻璃拟态（GlassMorphism）

### 5.1 GlassCard

毛玻璃卡片，是 `GroupBox`、`RadioButton` 模板的基础。

```xml
<glassMorphism:GlassCard Margin="14,0" CornerRadius="20" Padding="15">
    <StackPanel>
        <TextBlock Classes="DemiBold" Text="Username" />
        <TextBox PlaceholderText="Bob" />
    </StackPanel>
</glassMorphism:GlassCard>
```

| 属性 | 类型 | 默认值 | 注册 | 说明 |
|---|---|---|---|---|
| `CornerRadius` | `CornerRadius` | `20` | Styled | 圆角 |
| `BorderBrush` | `IBrush` | `Brushes.Black`（被样式覆盖为 `{DynamicResource GlassBorderBrush}`） | Styled | 边框色 |
| `BorderThickness` | `Thickness` | `1` | Styled | 边框粗细 |
| `Padding` | `Thickness` | `10`（由样式设定） | Styled（来自 `UserControl`） | 内边距 |
| `ClipToBounds` | `bool` | — | Styled（来自 `UserControl`） | 会传给内层两个 `Border` |

**模板结构**：`Panel` 里叠了两层 `Border`——`PART_BorderCard`（背景 `{DynamicResource GlassCardBackground}` + `Opacity={DynamicResource GlassOpacity}`）和 `PART_ClipBorder`（透明 + `{DynamicResource PrimaryColor0}` 边框 + `ContentPresenter`）。两层都绑定了 `BorderThickness` / `CornerRadius` / `ClipToBounds`，并有 0.15s 的 `BrushTransition`。

> 注意 `PART_ClipBorder` 的 `BorderBrush` 是 `{DynamicResource PrimaryColor0}` —— 这个键**在主题字典里也没有定义**，所以你通过 `BorderBrush` 设的值作用在 `PART_BorderCard` 上（可见），覆盖层描边则为空。实际视觉边框来自 `GlassBorderBrush`。

> ⚠️ **属性遮蔽**：`GlassCard` 继承自 `UserControl`（→ `TemplatedControl`），而它**自己又注册了一遍** `CornerRadius` / `BorderBrush` / `BorderThickness`，遮蔽了基类的同名属性（编译时会看到 `CS0108: 隐藏继承的成员` 警告）。XAML 里写 `CornerRadius="20"` 解析到的是 `GlassCard` 自己的属性，行为正常；但针对基类写的选择器（如 `Selector="TemplatedControl"`）不会影响到这几个属性。

### 5.2 CherylBackground

`Image` 的子类，用 FastNoise（OpenSimplex2）实时生成 100×100 的噪声渐变位图并 `UniformToFill` 拉伸铺满，**是 CherylUI 的「壁纸」**。`InteractiveContainer` 的模板里已经放了一个，一般不需要自己加。

```xml
<glassMorphism:CherylBackground />
```

- 无 Avalonia 属性。
- 构造时创建 `WriteableBitmap` 并设置 `Stretch = Stretch.UniformToFill`。
- `EndInit()` 时按当前主题渲染一次，并订阅 `ActualThemeVariantChanged` 在切换主题时重绘。
- 实现了 `IDisposable`（释放位图）。
- 配色写死在 `FastNoiseBackgroundRenderer`：主色 `#0A59F7`、点缀色 `#5900FF`、底色亮色 `#F5F5F5` / 暗色 `#000000`。自定义需要替换 `FastNoiseBackgroundRenderer`。

### 5.3 BlurBackground

实时「背景模糊」控件：抓取自己所在位置的 Skia surface 快照，做 `SKImageFilter.CreateBlur(15, 15)` 后绘制回去。`SliverPage` / `SliverPageLong` / `TabControl` 的标题栏/标签栏用的就是它。

```xml
<Border ClipToBounds="True">
    <glassmorphism:BlurBackground Margin="-30" />
</Border>
```

| 属性 | 类型 | 默认值 | 注册 | 说明 |
|---|---|---|---|---|
| `Material` | `ExperimentalAcrylicMaterial` | `null` | Styled | 为 `null` 时按当前主题用内置的暗色（`MaterialOpacity=0.25, Tint=Black`）或亮色（`MaterialOpacity=0, Tint=White`）材质 |

> ⚠️ 命名空间是 **`clr-namespace:CherylUI.Glassmorphism`**（注意是小写 m 的 `Glassmorphism`，且没有 `.Controls` 前缀），与 `GlassCard` / `CherylBackground` 所在的 `CherylUI.Controls.GlassMorphism` **不是同一个**。
>
> 依赖 Skia：在软件渲染后端或未启用 Skia 的平台上不生效。想正确模糊自身背后的内容，外面通常要套一层 `ClipToBounds="True"` 的 `Border`。

---

## 6. 移动端选择器

### 6.1 MobilePicker

点击弹出一个列表选择弹窗（`MobilePickerPopUp`），选完点 Done 回填 `SelectedItem`。

```xml
<mobilePicker:MobilePicker
    Title="Text Style"
    SubTitle="Text style of your system apps."
    SelectedItem="{Binding SelectedTextStyle, Mode=TwoWay}">
    <mobilePicker:MobilePicker.Items>
        <objectModel:ObservableCollection x:TypeArguments="system:String">
            <system:String>Default</system:String>
            <system:String>Italic</system:String>
            <system:String>Bold</system:String>
        </objectModel:ObservableCollection>
    </mobilePicker:MobilePicker.Items>
</mobilePicker:MobilePicker>
```

| 属性 | 类型 | 默认值 | 注册 | 说明 |
|---|---|---|---|---|
| `SubTitle` | `string` | `null` | **Direct**（TwoWay） | 弹窗副标题 |
| `Title` | `string` | `null` | **Direct**（TwoWay） | 弹窗标题 |
| `SelectedItem` | `string` | `null` | **Direct**（TwoWay） | 当前选中项；也显示在按钮上 |
| `Items` | `ObservableCollection<string>` | 空集合 | Styled | 候选项，**只支持字符串** |
| `PopupScale` | `ScaleTransform` | `new ScaleTransform()` | Styled | 弹窗根 `Border` 的 `RenderTransform` |
| `PopupWidth` | `int` | `300` | Styled | 见下方注意事项 |

**必需条件**：`MobilePicker` 内部通过 `InteractiveContainer.ShowDialog(control, true)` 弹窗，因此**必须有 `InteractiveContainer`**。

**行为细节**

- 控件模板就是一个 `Classes="Void"` 的 `Button`（整行可点），里面用 `Classes="Secondary"` 的 `TextBlock` 显示 `SelectedItem`。
- 弹窗内 `ListBox` 高度限制在 120–170 之间，底部一个 `Classes="Small"` 的 Done 按钮；点 Done 时把 `ListBox.SelectedItem` 写回 `MobilePicker.SelectedItem` 并关闭对话框。
- 弹窗的 ViewModel 是 `MobilePickerPopUpVM`（`ReactiveObject`），公开 `Items` / `SelectedItem` / `Title` / `SubTitle`，可自行构造使用。

> ⚠️ **`PopupWidth` 视觉上基本无效**：`MobilePickerPopUp.axaml` 里根 `Border` 的 `Width` 被硬编码为 `300`，`PopupWidth` 只改了外层 `UserControl.Width`。要改宽度请直接改/重写 `MobilePickerPopUp` 的模板。
>
> ⚠️ 这两个属性的 AvaloniaProperty **名字都被注册成了 `"MobilePicker"`**（源码里写成了 `nameof(MobilePicker)`），实测：
> ```
> MobilePicker.PopupScaleProperty.Name == "MobilePicker"
> MobilePicker.PopupWidthProperty.Name == "MobilePicker"
> ```
> XAML 里直接写 `PopupWidth="400"` 是**可用**的（走 CLR 属性），但**不能**在 Style 里用 `<Setter Property="PopupWidth">` 寻址。

---

### 6.2 MobileNumberPicker

滚轮式的数字选择器：点击弹出上下滚动的数字盘。

```xml
<mobileNumberPicker:MobileNumberPicker Value="10" Minimum="0" Maximum="20" />
```

| 属性 | 类型 | 默认值 | 注册 | 说明 |
|---|---|---|---|---|
| `Value` | `int` | `0` | **Direct**（TwoWay） | 当前值 |
| `Minimum` | `int` | `0` | Styled | 下限（弹窗中会被夹紧） |
| `Maximum` | `int` | `100` | Styled | 上限（弹窗中会被夹紧） |

**必需条件**：同样依赖 `InteractiveContainer`。

**交互细节**

- 主控件是一个 `Classes="Void"` 的 `Button`，内部显示 `Value`（经 `IntToStringConverter`）。
- 弹窗 `MobileNumberPickerPopup` 用 `PointerPressed/Moved/Released` 实现拖动，**每 5 像素 = 1 个单位**；越界时会把起点重置到边界，形成「拖不动」的手感。
- 弹窗显示当前值 ±1、±2 的邻值，越界的位置显示空字符串（形成视觉上的滚轮边缘）。
- **松手（`PointerReleased`）时**才把结果写回 `_MobileNumberPicker.Value`；拖动过程中只刷新弹窗里的数字显示。没有「确认」按钮，只有 Done 用于关闭对话框。

---

## 7. 表单行（FormFields）

约定：每一行固定高 **47px**，左侧文字、右侧控件，行与行之间用 `HorizontalSeparator` 分隔，整体放进 `GroupBox`。这是原版 wiki 推荐的设置页写法。

### 7.1 FormFieldSwitch

```xml
<formFields:FormFieldSwitch Title="Dark Mode" IsChecked="{Binding DarkMode, Mode=TwoWay}" />
```

| 属性 | 类型 | 默认值 | 注册 | 说明 |
|---|---|---|---|---|
| `Title` | `string` | `"Title"` | Styled | 左侧文字 |
| `IsChecked` | `bool` | `false` | **Direct**（TwoWay） | 开关状态，模板内 `ToggleButton Classes="Switch"` 双向绑定 |

### 7.2 FormFieldPicker

`FormFieldSwitch` 的「选择器」版本，内部就是一个靠右的 `MobilePicker`。

```xml
<formFields:FormFieldPicker
    Title="Text Style"
    DialogTitle="Text Style"
    DialogSubTitle="Text style of your system apps."
    SelectedItem="{Binding SelectedTextStyle, Mode=TwoWay}">
    <formFields:FormFieldPicker.Items>
        <objectModel:ObservableCollection x:TypeArguments="system:String">
            <system:String>Default</system:String>
            <system:String>Italic</system:String>
            <system:String>Bold</system:String>
        </objectModel:ObservableCollection>
    </formFields:FormFieldPicker.Items>
</formFields:FormFieldPicker>
```

| 属性 | 类型 | 默认值 | 注册 | 说明 |
|---|---|---|---|---|
| `Title` | `string` | `"Title"` | Styled | 左侧文字 |
| `Items` | `ObservableCollection<string>` | 空集合 | Styled | 候选项，转发给内部 `MobilePicker` |
| `SelectedItem` | `string` | `""` | **Direct**（TwoWay） | 当前选中项（模板内 TwoWay 转发） |
| `DialogTitle` | `string` | `"DialogTitle"` | Styled | → `MobilePicker.Title` |
| `DialogSubTitle` | `string` | `"DialogSubTitle"` | Styled | → `MobilePicker.SubTitle` |

### 7.3 FormFieldNumberPicker

```xml
<formFields:FormFieldNumberPicker Value="10" Minimum="0" Maximum="20" />
```

| 属性 | 类型 | 默认值 | 注册 | 说明 |
|---|---|---|---|---|
| `Maximum` | `int` | `0` | **Direct**（TwoWay） | → `MobileNumberPicker.Maximum` |
| `Minimum` | `int` | `0` | **Direct**（TwoWay） | → `MobileNumberPicker.Minimum` |
| `Value` | `int` | `0` | **Direct**（TwoWay） | → `MobileNumberPicker.Value` |
| `Title` | `string` | `"Title"` | Styled | ⚠️ **无效**，见下 |

> ⚠️ **`Title` 不生效**：模板里左侧 `TextBlock` 的 `Text` 被硬编码成 `"Users"`，没有绑定 `Title`。想让标题可配，需要自己重写模板。

### 7.4 FormFieldAction

一整行可点击的「动作 / 跳转」项，右侧带 `chevron_right_regular` 箭头。

```xml
<formFields:FormFieldAction Title="Eye Comfort" Command="{Binding LaunchToast}" />
<formFields:FormFieldAction Title="Share" Command="{Binding Share}" CommandParameter="{Binding Id}" />
```

| 属性 | 类型 | 默认值 | 注册 | 说明 |
|---|---|---|---|---|
| `Title` | `string` | `"Title"` | Styled | 左侧文字 |
| `Command` | `ICommand` | `null` | **Direct** | 点击执行的命令 |
| `CommandParameter` | `object` | `null` | **Direct** | 命令参数 |

### 7.5 完整设置页示例

摘自可运行示例 `CherylCrossTest/Views/SettingsPage.axaml`：

```xml
<controls:SliverPageLong Header="Settings">
    <StackPanel>
        <controls:GroupBox Header="View mode">
            <StackPanel Margin="10,2">
                <formFields:FormFieldSwitch Title="Dark Mode" />

                <cherylUi:HorizontalSeparator />

                <formFields:FormFieldAction Command="{Binding LaunchToast}" Title="Eye Comfort" />
                <cherylUi:HorizontalSeparator />

                <formFields:FormFieldNumberPicker Maximum="20" Minimum="0" Value="10" />
                <cherylUi:HorizontalSeparator />

                <formFields:FormFieldPicker
                    DialogSubTitle="Text style of your system apps."
                    DialogTitle="Text Style"
                    SelectedItem="Default"
                    Title="Text Style">
                    <formFields:FormFieldPicker.Items>
                        <objectModel:ObservableCollection x:TypeArguments="system:String">
                            <system:String>Default</system:String>
                            <system:String>Italic</system:String>
                            <system:String>Bold</system:String>
                        </objectModel:ObservableCollection>
                    </formFields:FormFieldPicker.Items>
                </formFields:FormFieldPicker>
            </StackPanel>
        </controls:GroupBox>

        <Border Height="120" />
    </StackPanel>
</controls:SliverPageLong>
```

对应的 ViewModel（示例工程用 ReactiveUI）：

```csharp
public class MainViewModel : ReactiveObject
{
    private bool _description;
    public bool Description
    {
        get => _description;
        set => this.RaiseAndSetIfChanged(ref _description, value);
    }

    public void LaunchToast() => InteractiveContainer.ShowToast(new TextBlock { Text = "Hit !" }, 3);
    public void LaunchDialog() => InteractiveContainer.ShowDialog(new DialogContent());
}
```

---

## 8. 内置样式化的 Avalonia 原生控件

`Index.axaml` 会给你项目里**所有**这些原生控件套上 CherylUI 外观，不需要写额外 XAML。

### 8.1 Button

```xml
<Button><TextBlock Text="Normal" /></Button>
<Button Classes="Large"><TextBlock Text="Large" /></Button>
<Button Classes="Small"><TextBlock Text="Small" /></Button>
<Button Classes="Secondary"><TextBlock Text="Secondary" /></Button>
<Button Classes="Text"><TextBlock Text="Text" /></Button>
<Button Classes="Void"><PathIcon Data="{StaticResource chevron_right_regular}" /></Button>
```

| 类 | 效果 |
|---|---|
| （无） | 背景 `Accent`，`CornerRadius=100`，`Padding=30,12`，圆角胶囊按钮 |
| `Small` | `Padding=22,10`，内部文字 16 |
| `Large` | `Padding=40,15`，内部文字 23 |
| `Secondary` | 背景 `LightAccent`，文字 `Accent` |
| `Text` | 背景透明，文字 `Accent` |
| `Void` | 背景透明、无 focus 描边（库内多用于「整行可点」的容器按钮） |

**伪类**：`:pressed`（背景 `ContraryAccent` + `ScaleTransform 1.01`）、`:focus`（外圈 `Border` 描边 `Accent`）。

> 样式规则 `Button TextBlock` 会给按钮内**所有** `TextBlock` 设置 `Foreground=ContraryForeground`、`FontSize=18`、`SemiBold`。按钮里放复杂内容时注意这点；`Secondary` / `Text` 类有对应覆盖规则。

### 8.2 TextBlock

```xml
<TextBlock Text="普通正文" />                       <!-- 16 / Medium / PrimaryText -->
<TextBlock Classes="Secondary" Text="次要说明" />    <!-- 14 / SecondaryText -->
<TextBlock Classes="DemiBold" Text="小标题" />
```

**基类样式**：`FontFamily = {StaticResource HarmonyFont}`（Quicksand）、`FontSize=16`、`Foreground={DynamicResource PrimaryText}`、`FontWeight=Medium`。

### 8.3 Border.Card

```xml
<Border Classes="Card">
    <!-- 内容 -->
</Border>
```

背景 `{DynamicResource CardBackground}`，`CornerRadius=20`，`Padding=10,10`。

### 8.4 ToggleButton.Switch

```xml
<ToggleButton Classes="Switch" IsChecked="True" />
```

iOS 风格开关：轨道 54×30，滑块 24×24 白色，`Canvas.Left` 在 6↔30 之间过渡（0.15s）；`:checked` 时轨道变 `Accent`。模板第 0 行还有一个 `ContentPresenter`，可以给开关加上方说明文字。

### 8.5 CheckBox

```xml
<CheckBox />
<CheckBox Content="请勾选" />
```

26×26 圆角 6 的方框，`:checked` 时背景 `Accent` 并显示白色对勾。

> ⚠️ `:indeterminate` 的样式指向模板里并不存在的 `Rectangle#indeterminateMark`，**三态外观实际未实现**（勾选/未勾选正常）。

### 8.6 RadioButton

```xml
<RadioButton IsChecked="True">
    <StackPanel>
        <TextBlock FontSize="13" FontWeight="SemiBold" Text="MONTHLY" />
        <TextBlock FontSize="18" FontWeight="Bold" Text="$17.99 / month" />
    </StackPanel>
</RadioButton>
```

卡片式单选：整个模板是一张 `GlassCard`（`CornerRadius=15`），右侧有一个不可交互的 `CheckBox` 作指示器；`:checked` 时 `BorderBrush=Accent`、`BorderThickness=2`、`Padding=14,11`。`GroupName` 等原生行为照常可用。

### 8.7 ListBox / ListBoxItem

```xml
<ListBox ItemsSource="{Binding Items}" SelectedItem="{Binding Selected}" />
```

- `ListBox` 背景透明、无边框、纵向滚动条 `Auto`。
- `ListBoxItem` 每条右侧有一个禁用状态的 `CheckBox` 作选中指示器，底部有一条 `Separator`（1px，`#11333333`）。
- **`ListBoxItem` 就是 `MobilePicker` 弹窗里用的那个列表外观。**

### 8.8 Slider

```xml
<Slider Minimum="0" Maximum="100" Value="50" />
```

- 只在 `:horizontal` 方向有模板；轨道高 25、圆角 13，背景 `ComponentBackground`。
- 已填充部分（`Track.DecreaseButton`）为 `Accent`；滑块是 19×19 的白色圆点（外框 32×32）。
- `:disabled` 时整体 `Opacity = {DynamicResource ThemeDisabledOpacity}`。

### 8.9 ProgressBar

```xml
<ProgressBar Value="50" />
<ProgressBar IsIndeterminate="True" />
<ProgressBar Value="50" ShowProgressText="True" />   <!-- 叠一个 "50%" 文本 -->
```

轨道高 5、圆角 10，背景 `ComponentBackground`，进度条高 8、颜色 `Accent`；`Width` 有 0.8s 的 `CircularEaseOut` 过渡。

### 8.10 TextBox

```xml
<StackPanel>
    <TextBlock Classes="DemiBold" Margin="5,0,0,0" Text="Username" />
    <TextBox PlaceholderText="Bob" />
</StackPanel>

<TextBox PasswordChar="*" PlaceholderText="*****" />
<TextBox Classes="BottomBar" PlaceholderText="只有下划线" />
<TextBox Classes="FlatTextBox" PlaceholderText="无边框" />
```

| 类 / 伪类 | 效果 |
|---|---|
| （无） | 高 46，`BorderThickness=0,0,0,1` 下划线风格，`:focus` 时描边 `Accent` |
| `BottomBar` | 下划线加粗到 1.5 |
| `FlatTextBox` | 去掉 `BorderThickness` 与阴影 |
| `:error` | 描边 `{DynamicResource ErrorBrush}` |
| `:disabled` | `Opacity = {DynamicResource ThemeDisabledOpacity}` |

**内置能力**：

- **浮动占位符**：`UseFloatingPlaceholder` + `PlaceholderText`（Avalonia 12 的新名字，旧版是 `Watermark`）。有文字时占位符浮到上方。
- **一键清空**：文本非空时右侧出现 `TextEraserButton`（`CherylUI.Styles` 命名空间），透明度随文本长度过渡。
- **右键菜单**：默认 ContextFlyout 为 Cut / Copy / Paste。
- **密码显示切换**：Avalonia 的 `RevealPassword` 可配合 `PasswordChar` 使用。

### 8.11 TabControl / TabItem —— 底部标签栏

```xml
<TabControl>
    <TabItem>
        <TabItem.Header>
            <StackPanel>
                <PathIcon Classes="Tab" Data="{StaticResource home_regular}" />
                <TextBlock Classes="Tab" Text="Home" />
            </StackPanel>
        </TabItem.Header>

        <!-- 页面内容 -->
    </TabItem>
    <!-- 更多 TabItem ... -->
</TabControl>
```

- **`TabStripPlacement` 默认被设为 `Bottom`**，标签栏在底部，带 `BlurBackground` 实时模糊与 1px 顶部分隔线（`{DynamicResource Separator}`）。
- 标签栏用 `UniformGrid Rows="1"` 均分，几个 `TabItem` 就分几列。
- `TextBlock Classes="Tab"`：未选中 15 / `DemiBold` / `SecondaryText`；**选中 16 / `Bold` / `Accent`**。
- `PathIcon Classes="Tab"`：未选中 24×24 / `SecondaryText`；选中 27×27 / `Accent`，宽度有 0.3s 过渡。

### 8.12 DatePicker

```xml
<DatePicker />
```

CherylUI 通过 `Index.axaml` 的合并字典提供了一份 `ControlTheme`：按钮部分是 `Classes="Void"` 的透明按钮，弹窗是一个 `Card` 风格的 `DatePickerPresenter`（月/日/年三列滚轮 + Cancel / Apply）。

> ⚠️ 弹窗 `Popup` 的高度被写死为 `750`、`Placement="Center"`；按钮文案 `Cancel` / `Apply` 是硬编码英文。桌面端表现可能与预期不同。
>
> 样式定义在 `CherylUI/Styles/DatePickerStyles.axaml`，直接改这个文件即可覆盖。

### 8.13 样式类速查表

| 控件 | 可用类 | 关键伪类 |
|---|---|---|
| `Button` | `Small` `Large` `Secondary` `Text` `Void` | `:pressed` `:focus` |
| `TextBlock` | `Secondary` `DemiBold` | — |
| `Border` | `Card` | — |
| `ToggleButton` | `Switch` | `:checked` `:unchecked` |
| `CheckBox` | — | `:checked` `:pointerover` `:disabled` `:indeterminate`(未实现) |
| `RadioButton` | — | `:checked` |
| `ListBoxItem` | — | `:selected` `:pointerover` |
| `Slider` | — | `:horizontal` `:disabled` |
| `TextBox` | `BottomBar` `FlatTextBox` | `:focus` `:error` `:disabled` `:pointerover` |
| `TabItem`（内部元素） | `TextBlock.Tab` `PathIcon.Tab` | `:selected` |
| `Loading` | `White` | — |
| `Button`（TextBox 内部） | `textBoxClearButton` | — |

---

## 9. 图标资源

`CherylUI/Icons.axaml` 以 `StreamGeometry` 形式提供，用法：

```xml
<PathIcon Data="{StaticResource home_regular}"
          Width="24" Height="24"
          Foreground="{DynamicResource SecondaryText}" />
```

**可用键名**

| 键 | 图形 |
|---|---|
| `ios_arrow_left_regular` | iOS 风格左箭头 |
| `arrow_left_regular` | 左箭头（`SliverPage` 返回按钮用） |
| `chevron_right_regular` | 右尖括号（`FormFieldAction` 用） |
| `weather_sunny_regular` | 太阳（亮度） |
| `settings_regular` | 齿轮 |
| `clock_alarm_regular` | 闹钟 |
| `home_regular` | 房子 |
| `person_regular` | 人像 |
| `TextBoxClearButtonData` | 叉号（清空按钮） |

另外在 `TextBoxStyles.axaml` 的资源区还定义了 `PasswordBoxRevealButtonData` 与 `PasswordBoxHideButtonData`。

---

## 10. 主题与颜色资源

`Index.axaml` 注册了 `Light` / `Dark` 两个 `ThemeDictionary`，**所有键在明暗主题下同名**，切换主题时自动替换。要改配色，在 `Application.Resources` 里用同名键覆盖即可。

### 10.1 颜色键

| 键 | Dark | Light | 用途 |
|---|---|---|---|
| `Foreground` | `#fafbfc` | `#000000` | 主前景 |
| `ContraryForeground` | `#e5e5e5` | `#FFFFFF` | 主按钮上的文字 |
| `CardBackground` | `#252729` | `#FFFFFF` | `Border.Card` 背景 |
| `GlassBorderBrush` | `#252729` | `#e3e3e7` | 玻璃卡片描边 |
| `DialogBackground` | `#252729` | `#FFFFFF` | Dialog / Toast 背景 |
| `Background` | `Transparent` | `Transparent` | 页面背景（透明，露出噪声壁纸） |
| `GridDialogBackground` | `#77ffffff` | `#88000000` | 对话框遮罩 |
| `Accent` | `#317AF7` | `#0A59F7` | 主题强调色 |
| `ContraryAccent` | `#317AF7` | `#317AF7` | 按下态 |
| `SelectedAccent` | `#317AF7` | `#317AF7` | 选中态 |
| `LightAccent` | `#250A59F7` | `#250A59F7` | `Button.Secondary` 背景 |
| `ComponentBackground` | `#44ffffff` | `#19000000` | 组件底色（滑块轨道、开关轨道、边框） |
| `Separator` | `#33ffffff` | `#33000000` | 标签栏顶部分隔线 |
| `ListSeparator` | `#11dddddd` | `#11333333` | `HorizontalSeparator` |
| `TappingBackground` | `#19ffffff` | `#19000000` | 按下反馈底色 |
| `PrimaryText` | `#fdfdfd` | `#EE000000` | TextBlock 默认前景 |
| `SecondaryText` | `#c1ffffff` | `#9f000000` | `Classes="Secondary"` |
| `GlassCardBackground` | `#656565` | `#aaffffff` | `GlassCard` 背景 |
| `GlassCardOpaqueBackground` | `#222222` | `#fcfcfc` | 不透明玻璃背景 |

### 10.2 其他键

| 键 | 类型 | 值 | 用途 |
|---|---|---|---|
| `IsDark` | `bool` | Dark=`True` | 配合 `IsVisible` 做主题分支 |
| `IsLight` | `bool` | Light=`True` | 同上 |
| `GlassOpacity` | `double` | Dark `0.3` / Light `0.8` | `GlassCard` 背景透明度 |
| `GlassOpacityDark` | `double` | `0.45` / `0.5` | 备用 |
| `GlassOpacityLight` | `double` | `0.57` / `0.75` | 备用 |
| `HarmonyFont` | `FontFamily` | `avares://CherylUI/CustomFont#Quicksand` | 全局字体 |

> ⚠️ `PrimaryColor0`（`GlassCard` 的 `PART_ClipBorder` 描边）、`SukiPrimaryColor` / `SukiPrimaryColorGradient`（`Loading` 默认渐变）、`SukiBackground`（仅出现在设计器预览）这几个键**在本仓库未定义**。

---

## 11. 动画扩展方法

`CherylUI.AnimationHelper`（命名空间 `CherylUI`），`using CherylUI;` 后即可用。

```csharp
public static void Jump(this Control control)
public static void Vibrate(this Animatable control, TimeSpan duration)
public static void Animate<T>(this Animatable control, AvaloniaProperty property, T from, T to, TimeSpan duration, ulong count = 1)
public static void Animate<T>(this Animatable control, AvaloniaProperty property, T from, T to)   // 默认 500ms / 1 次
```

| 方法 | 效果 |
|---|---|
| `Jump()` | 800ms 的 Margin 弹跳（上移 8px → 回位 → 上移 2px → 回位），`QuadraticEaseOut` |
| `Vibrate(duration)` | 以 75ms 为一帧做 `ScaleX/Y 0.995↔1.005` + `RotateTransform.Angle 0.4↔-0.3` 的抖动，帧数 = `duration / 75ms`，`CubicEaseInOut` |
| `Animate(...)` | 通用 from→to 属性动画 |

```csharp
myCard.Jump();                                    // 错误提示式弹跳
myCard.Vibrate(TimeSpan.FromMilliseconds(300));   // 震动
myCard.Animate(OpacityProperty, 0d, 1d, TimeSpan.FromMilliseconds(400));
```

> 都是「发射后不管」的 `RunAsync`，`FillMode.Forward`，重复调用会叠加新的动画。

---

## 12. 已知问题与注意事项

以下均为**源码中可验证的事实**，写代码时请绕开。

### 命名空间类

1. **`HorizontalSeparator` 在 `CherylUI` 根命名空间**，不是 `CherylUI.Controls`。
2. **`BlurBackground` 在 `CherylUI.Glassmorphism`**，而 `GlassCard` / `CherylBackground` 在 `CherylUI.Controls.GlassMorphism`。两个 xmlns 都得声明。

### 属性注册类

3. **`FormFieldNumberPicker.TitleProperty` 与 `FormFieldAction.TitleProperty` 的所有者被写成了 `FormFieldSwitch`**（复制粘贴遗留）。三个同名属性各自独立工作（实测静态初始化不抛异常），但语义上是错的，别指望在 `FormFieldSwitch` 的类型级别样式里统一设置。
4. **`MobilePicker.PopupScale` / `PopupWidth` 的 AvaloniaProperty 名字都是 `"MobilePicker"`**（源码写成了 `nameof(MobilePicker)`）。XAML 直接写属性名可用，Style `<Setter>` 不可用。
5. 所有 **DirectProperty 都不能被 `Style` / `ControlTheme` 的 `<Setter>` 设置**：`FormFieldSwitch.IsChecked`、`FormFieldPicker.SelectedItem`、`FormFieldNumberPicker.{Value,Minimum,Maximum}`、`FormFieldAction.{Command,CommandParameter}`、`MobilePicker.{SelectedItem,Title,SubTitle}`、`MobileNumberPicker.Value`。

### 功能缺失 / 无效属性类

6. **`FormFieldNumberPicker.Title` 无效** —— 模板里左侧文字硬编码为 `"Users"`。
7. **`GroupBox.Text` 无效** —— 模板只绑定了 `Header`。
8. **`MobilePicker.PopupWidth` 视觉上无效** —— `MobilePickerPopUp` 的根 `Border` 宽度硬编码 300。
9. **`CheckBox` 三态外观未实现** —— `:indeterminate` 指向不存在的 `Rectangle#indeterminateMark`。
10. **`Loading` 默认渐变不可见** —— 依赖未定义的 `SukiPrimaryColor` / `SukiPrimaryColorGradient`。用 `Classes="White"` 或自己补资源键。
11. **`GlassCard` 的外层描边为空** —— `PART_ClipBorder` 引用未定义的 `PrimaryColor0`。

### 运行时行为类

12. **`InteractiveContainer.WaitUntilDialogIsClosed()` 不要用**。它先 `Dispatcher.UIThread.InvokeAsync` 去赋值局部变量、紧接着在调用线程上 `Thread.Sleep(200)` 轮询：在 UI 线程调用会**卡死界面**，而且 `container` 可能还是 `null`（`NullReferenceException`）。要等对话框关闭，请自己订阅 `IsDialogOpen` 变化或改用 `TaskCompletionSource`。
13. **`ShowToast` 连续调用会互相干扰**。每个 toast 都起一个 `Task.Run` + `Thread.Sleep(seconds)`，到点就把 `IsToastOpen = false`。先后弹两个 toast，前一个的定时器会把后一个提前关掉。
14. **`ShowDialog` 不会自动关闭**，务必在内容里调用 `InteractiveContainer.CloseDialog()`。
15. **`InteractiveContainer` 全局只能有一个** —— 内部用 `.First()` 取视觉树里的第一个实例。
16. **`InteractiveContainer` 订阅了 `TopLevel.BackRequested` 但从不退订**（在 `Loaded` 里挂、没有 `Unloaded` 里的 `-=`）。反复进出页面时注意这个引用。
17. **`MobileNavigation.Push` 的参数类型是 `UserControl`**，传 `Control` / `Window` 编译不过。
18. **`MobileNavigation.Pop()` 在栈空时静默返回**，不会抛异常也不会退出应用（桌面端需要自己处理）。
19. **`SliverPage` 的返回按钮默认不显示**（`IsBackVisible=false`），`SliverPageLong` 则**始终显示**。
20. **`SliverPage` / `SliverPageLong` / `MobilePicker` / `MobileNumberPicker` 都强依赖 `InteractiveContainer`**（前两者依赖 `MobileNavigation`），脱离 `InteractiveContainer` 使用会在运行时抛异常。
21. **`BlurBackground` 依赖 Skia**，在非 Skia 后端（例如某些软件渲染/无 GPU 场景）不产生模糊效果；请把它包在 `ClipToBounds="True"` 的容器里，否则模糊范围会溢出。
22. **`CherylBackground` 是 100×100 的低分辨率噪声位图**，靠 `UniformToFill` 拉伸。它保证的是整体氛围而不是细节。

> **关于编译警告**：本库自身在 net8.0 / net10.0 下会产生约 80 条警告（主要是可空引用 CS86xx 与成员隐藏 CS0108），**不影响运行**。你在自己的构建日志里看到来自 `CherylUI.csproj` 的警告，都属于这一类。

---

## 13. 从原版（Avalonia 11）迁移检查清单

- [ ] 目标框架改为 `net8.0` / `net10.0`（Android / iOS 需 .NET 10）。
- [ ] 包版本改为 `2.0.0`，Avalonia 升到 `12.1.4`。
- [ ] `TextBox Watermark="..."` → `PlaceholderText="..."`；`UseFloatingWatermark` → `UseFloatingPlaceholder`。
- [ ] 若开启了编译绑定：给所有用 `{Binding}` 的视图补 `x:DataType`（`MobilePickerPopUp` 已由库内部补齐）。
- [ ] 移除 `Avalonia.Diagnostics` 包，改用 `AvaloniaUI.DiagnosticsSupport`。
- [ ] 若用过 `Avalonia.ReactiveUI`：换成 `ReactiveUI.Avalonia`，`UseReactiveUI()` → `UseReactiveUI(rxui => { })`。
- [ ] Android：`AvaloniaMainActivity<TApp>` → 非泛型 `AvaloniaMainActivity` + `[Application] AvaloniaAndroidApplication<App>`；`minSdk` ≥ 23。
- [ ] iOS：最低版本 13.0 / arm64。
- [ ] **检查 `Loading` 是否可见**（未定义 `SukiPrimaryColor` 时是透明的），必要时加 `Classes="White"` 或补资源键。
- [ ] 复核 [第 12 节](#12-已知问题与注意事项)里你正在用的那几个「无效属性」。

---

## 14. 链式对话框（SukiUI 同款 Fluent Dialog API）

这一节是 **CherylUI 2.0.0 新增**的能力：把 SukiUI 的 `ISukiDialogManager` / `CreateDialog()` 链式 API 完整搬到 CherylUI，
外观（圆角 25 玻璃卡片、居中标题、右下角按钮、顶部彩色图标气泡、0.4 遮罩）也对齐 SukiUI。

### 14.1 三个新类型

| 类型 | CLR 命名空间 | 作用 |
|---|---|---|
| `CherylDialogHost` | `CherylUI.Controls` | 宿主控件：遮罩、开合动画、点击背景关闭都归它管 |
| `CherylDialog` | `CherylUI.Controls` | 对话框本体（`TemplatedControl`），一般不用手写 |
| `ICherylDialogManager` / `CherylDialogManager` | `CherylUI.Dialogs` | 管理"当前显示的对话框" |
| `ICherylDialog` / `CherylDialogBuilder` / `FluentCherylDialogBuilder` | `CherylUI.Dialogs` | 对话框契约、构建器、链式扩展方法 |

```xml
xmlns:controls="clr-namespace:CherylUI.Controls;assembly=CherylUI"
xmlns:dialogs="clr-namespace:CherylUI.Dialogs;assembly=CherylUI"
```

### 14.2 最小用法（SukiUI 同款）

**View** —— 一个宿主，放在视觉树里任意位置（通常盖在最外层）：

```xml
<Grid>
    <controls:InteractiveContainer>
        <!-- 你的内容 -->
    </controls:InteractiveContainer>

    <controls:CherylDialogHost Manager="{Binding DialogManager}" />
</Grid>
```

**ViewModel** —— 一行链式调用打开对话框：

```csharp
public class MainViewModel
{
    // 这就是全部接线工作：View 里的 Host 绑到它
    public ICherylDialogManager DialogManager { get; } = new CherylDialogManager();

    public void ShowBasic() =>
        DialogManager.CreateDialog()
            .WithTitle("基础对话框")
            .WithContent("对话框完全由 ViewModel 构造。")
            .WithActionButton("关闭", _ => { }, true)
            .TryShow();
}
```

**零配置写法** —— 不想绑 Host，就用 `InteractiveContainer` 内置的那个：

```csharp
InteractiveContainer.DialogManager.CreateDialog()
    .WithTitle("零配置")
    .WithContent("每个 InteractiveContainer 模板里都有一个 CherylDialogHost，接的是 CherylDialogManager.Default。")
    .WithActionButton("知道了", _ => { }, true)
    .TryShow();
```

### 14.3 链式 API 全表

`CreateDialog()` 返回 `CherylDialogBuilder`，以下扩展方法都在 `CherylUI.Dialogs.FluentCherylDialogBuilder` 里（`using CherylUI.Dialogs;`）：

| 方法 | 说明 |
|---|---|
| `CreateDialog()` | 在 `ICherylDialogManager` 上开始构建 |
| `WithTitle(string)` | 居中标题（22 / DemiBold） |
| `WithContent(object)` | 内容：`Control`、字符串（自动换行、14 号字）或任意对象 |
| `WithViewModel(Func<ICherylDialog, object>, bool isViewModelOnly = true)` | 用 ViewModel 驱动内容；`true` 时只渲染 ViewModel（需要自己的 `IDataTemplate` / ViewLocator） |
| `OfType(NotificationType)` | 消息框样式：`Information` / `Success` / `Warning` / `Error`，对应彩色渐变图标气泡（i / ✓ / ! ） |
| `ShowCardBackground(bool)` | 是否显示玻璃卡片背景，默认 `true` |
| `AtBottom(bool atBottom = true)` | **CherylUI 额外**：对话框贴宿主底部弹出（对应老 API 的 `showAtBottom`） |
| `WithActionButton(object? content, Action<ICherylDialog> onClicked, bool dismissOnClick = false, params string[] classes)` | 右下角按钮，可加任意多个；`classes` 默认 `Text Small` |
| `WithOkResult(object? content, params string[] classes)` | 加一个"确定"按钮，并让 `TryShowAsync()` 返回 `true` |
| `WithYesNoResult(object? yes, object? no, params string[] classes)` | 加 Yes / No 两个按钮，`TryShowAsync()` 分别返回 `true` / `false` |
| `OnDismissed(Action<ICherylDialog>)` | 无论怎么关闭都会回调 |
| `Dismiss().ByClickingBackground()` | 允许点击背景关闭；**不加这句的对话框是"钉住"的**，点背景只会让它弹一下 |
| `TryShow()` → `bool` | 显示；已有对话框打开时返回 `false` 且什么都不做 |
| `TryShowAsync(CancellationToken)` → `Task<bool>` | 显示并等待关闭；已有对话框打开时抛 `InvalidOperationException` |

```csharp
// 典型确认框
var confirmed = await DialogManager.CreateDialog()
    .WithTitle("删除文件")
    .WithContent("此操作无法撤销。")
    .WithYesNoResult("删除", "取消")
    .Dismiss().ByClickingBackground()
    .TryShowAsync();       // 点背景 -> false，点"删除" -> true
```

```csharp
// 消息框样式
DialogManager.CreateDialog()
    .OfType(NotificationType.Success)
    .WithTitle("保存成功")
    .WithContent("你的更改已经写入磁盘。")
    .WithOkResult("好的")
    .TryShow();
```

### 14.4 `CherylDialog` 的属性

| 属性 | 类型 | 默认值 | 注册 | 说明 |
|---|---|---|---|---|
| `Title` | `string?` | `null` | Styled | 标题，为空则整行收起 |
| `Content` | `object?` | `null` | Styled | 内容，为空则整行收起 |
| `ViewModel` | `object?` | `null` | Styled | `IsViewModelOnly=true` 时渲染它 |
| `IsViewModelOnly` | `bool` | `false` | Styled | 隐藏 Title / Content / 按钮，只渲染 `ViewModel` |
| `Icon` | `object?` | `null` | Styled | 图标几何（`StreamGeometry` 等） |
| `IconColor` | `IBrush?` | `null` | Styled | 图标气泡填充色；**为 null 时整个气泡不显示** |
| `ShowCardBackground` | `bool` | `true` | Styled | 玻璃卡片背景 |
| `ShowAtBottom` | `bool` | `false` | Styled | 贴底（`AtBottom()` 设的就是它） |
| `ActionButtons` | `ObservableCollection<object>` | 空集合 | Styled | 底部按钮，由 Builder 填充 |
| `Manager` | `ICherylDialogManager?` | `null` | CLR | 由 Builder 注入 |
| `CanDismissWithBackgroundClick` | `bool` | `false` | CLR | 是否允许点背景关闭 |
| `OnDismissed` | `Action<ICherylDialog>?` | `null` | CLR | 关闭回调 |

### 14.5 `CherylDialogHost` 的属性

| 属性 | 类型 | 默认值 | 注册 | 说明 |
|---|---|---|---|---|
| `Manager` | `ICherylDialogManager?` | `null` | Styled | 绑定管理器；运行时换绑会自动重订阅 |
| `Dialog` | `object?` | `null` | Styled | 当前对话框，**由宿主维护**，不要在外部赋值 |
| `IsDialogOpen` | `bool` | `false` | Styled | 驱动遮罩与开合动画，**由宿主维护** |

**视觉与行为**（都定义在 `CherylUI/Styles/DialogStyles.axaml`，可直接改这个文件覆盖）：

| 项目 | 值 |
|---|---|
| 卡片 | 圆角 25、`MinWidth 360`、`{DynamicResource DialogBackground}` + `GlassBorderBrush` 描边 + 投影，顶部留 55px 给图标气泡 |
| 标题 / 内容 | 22 DemiBold 居中 / 14 号自动换行 |
| 按钮行 | 右下角，按钮间距 12，行上方 26、下方 21 |
| 图标气泡 | 100×100，上移 15px 骑在卡片顶边，白色图标 28×28 |
| 打开 | 遮罩 0.25s 淡入到 `Opacity=0.4`；卡片 `scale(0.85) → scale(1)` + 淡入 0.3s `CircularEaseOut` |
| 关闭 | 反向播放，300ms 后才把 `Dialog` 置空（所以还能看到淡出） |
| 点背景 | `CanDismissWithBackgroundClick=true` → 关闭；否则用 `AnimationHelper.Jump()` 弹一下作为"钉住"反馈 |

### 14.6 注意事项与与 SukiUI 的差异

1. **一个 Manager 同时只能有一个对话框**：`TryShow()` 返回 `false`，`TryShowAsync()` 抛异常。需要叠加请用多个 Manager + 多个 Host。
2. **同一个 Manager 绑到多个 Host 会重复渲染**（每个 Host 都会显示同一个对话框）。一个 Manager 配一个 Host。
3. `CherylDialogManager.Default` 是进程级单例，`InteractiveContainer` 模板里的 Host 就接它；如果自己的 App 也把 Host 绑到 `Default`，会看到两份。
4. **字符串按钮内容会被自动包成 `TextBlock`**，这样 CherylUI 的 `Button TextBlock` 系列样式（含 `Text` / `Secondary` 类的配色）才会生效。
5. **没加 `.Dismiss().ByClickingBackground()` 就是"钉住"**：点背景不关闭，只会跳一下。
6. **没有 ESC / 回车快捷键**，也**没有对象池**（每次 `CreateDialog()` 都是新实例，SukiUI 有池化和 `ResetToDefault` 复用）。
7. **没有 `BackgroundStyle`（Blur / Flat）开关**，也没有 SukiUI 6.x 那套"从点击位置弹出 + 弹簧"的动画；CherylUI 用的是 `Transitions` 缩放淡入。
8. **`WithViewModel(..., isViewModelOnly: true)` 需要你自己的 `IDataTemplate` / ViewLocator**，CherylUI 本体不带 ViewLocator。
9. 老 API（`InteractiveContainer.ShowDialog/CloseDialog`）与新的 Host **是两层独立图层**，同时用会出现两个遮罩，不建议混用。
10. 宿主依赖 `Transitions` 与 `RenderTransform`，在极低端设备上关闭动画可以自行覆盖 `DialogStyles.axaml` 里的 `Transitions`。

### 14.7 完整可运行示例

`CherylDialogSample/`（Desktop，`dotnet run --project CherylDialogSample`）演示了：基础对话框、自定义控件内容、点背景关闭、
`await TryShowAsync()` 拿结果、`AtBottom()` 贴底、零配置 `InteractiveContainer.DialogManager`、`OfType()` 三种消息框样式，
以及暗色主题下的外观。

---

## 附：示例工程对照

`CherylCrossTest/` 是一个可直接运行的 CherylUI 示例（Desktop / Android / iOS / Browser 四个 head），建议对照阅读：

| 文件 | 演示内容 |
|---|---|
| `Views/MainView.axaml` | `InteractiveContainer` 作为根节点 |
| `Views/MenuControl.axaml` | `TabControl` 底部标签栏 + 图标 + `ToggleButton.Switch` 切主题 |
| `Views/LoginPage.axaml` | `SliverPage` + `GlassCard` + `TextBox` |
| `Views/SettingsPage.axaml` | `SliverPageLong` + `GroupBox` + 全部 4 种 FormField + `RadioButton` + `DatePicker` |
| `Views/ClockPage.axaml` | `SliverPage` + 多张 `GlassCard` |
| `Views/DialogContent.axaml` | 对话框内容与 `CloseDialog()` |
| `Views/LoggedView.axaml` | `MobileNavigation.Pop()` |
| `ViewModels/MainViewModel.cs` | `ShowToast` / `ShowDialog` 的调用方式 |

`CherylDialogSample/`（Desktop 单 head）是 [第 14 节](#14-链式对话框sukiui-同款-fluent-dialog-api) 链式对话框的最小可运行示例：

| 文件 | 演示内容 |
|---|---|
| `MainWindow.axaml` | `CherylDialogHost` 绑 `{Binding DialogManager}` + CherylUI 页面 |
| `MainViewModel.cs` | 全部 7 种链式用法（含 `await TryShowAsync()`） |
| `RelayCommand.cs` | 示例自带的极简 `ICommand`（不依赖任何 MVVM 包） |

