using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Styling;
using CherylUI.Controls;
using CherylUI.Dialogs;

namespace CherylDialogSample;

/// <summary>
/// Every dialog of this sample is opened from here - the view only contains
/// a single &lt;controls:CherylDialogHost Manager="{Binding DialogManager}" /&gt;.
/// </summary>
public class MainViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// The manager the view's CherylDialogHost is bound to. That is the whole wiring:
    /// no code-behind, no service locator.
    /// </summary>
    public ICherylDialogManager DialogManager { get; } = new CherylDialogManager();

    private string _status = "还没有打开过对话框。";

    public string Status
    {
        get => _status;
        private set
        {
            if (_status == value)
                return;

            _status = value;
            OnPropertyChanged();
        }
    }

    private bool _isDark;

    public bool IsDark
    {
        get => _isDark;
        set
        {
            if (_isDark == value)
                return;

            _isDark = value;
            OnPropertyChanged();

            if (Application.Current is { } app)
                app.RequestedThemeVariant = value ? ThemeVariant.Dark : ThemeVariant.Light;
        }
    }

    public ICommand ShowBasicDialog { get; }
    public ICommand ShowContentDialog { get; }
    public ICommand ShowBackgroundDismissDialog { get; }
    public ICommand ShowConfirmDialog { get; }
    public ICommand ShowBottomDialog { get; }
    public ICommand ShowZeroConfigDialog { get; }
    public ICommand ShowSuccessDialog { get; }
    public ICommand ShowWarningDialog { get; }
    public ICommand ShowErrorDialog { get; }

    public MainViewModel()
    {
        _isDark = Application.Current?.ActualThemeVariant == ThemeVariant.Dark;

        ShowBasicDialog = new RelayCommand(ShowBasic);
        ShowContentDialog = new RelayCommand(ShowCustomContent);
        ShowBackgroundDismissDialog = new RelayCommand(ShowBackgroundDismiss);
        ShowConfirmDialog = new RelayCommand(ShowConfirmAsync);
        ShowBottomDialog = new RelayCommand(ShowBottom);
        ShowZeroConfigDialog = new RelayCommand(ShowZeroConfig);
        ShowSuccessDialog = new RelayCommand(() => ShowTyped(NotificationType.Success, "保存成功", "你的更改已经写入磁盘。"));
        ShowWarningDialog = new RelayCommand(() => ShowTyped(NotificationType.Warning, "空间不足", "只剩下 128 MB 可用空间。"));
        ShowErrorDialog = new RelayCommand(() => ShowTyped(NotificationType.Error, "同步失败", "无法连接到服务器，请稍后再试。"));
    }

    /// <summary>Opened by the window 0.7s after startup.</summary>
    public void ShowWelcomeDialog()
    {
        SetStatus("欢迎对话框已打开：点背景或按钮都能关闭它。");

        Report(DialogManager.CreateDialog()
            .OfType(NotificationType.Information)
            .WithTitle("CherylUI Dialog")
            .WithContent("这是 SukiUI 同款链式 API 的默认外观。\n关掉它，然后试试下面 6 个按钮。")
            .WithActionButton("开始探索", _ => { }, true)
            .Dismiss().ByClickingBackground()
            .TryShow());
    }

    // 1 - the straight port of SukiUI's README example
    private void ShowBasic()
    {
        SetStatus("已打开基础对话框。");

        Report(DialogManager.CreateDialog()
            .WithTitle("基础对话框")
            .WithContent("对话框完全由 ViewModel 构造：\n" +
                         "CreateDialog().WithTitle(...).WithContent(...).WithActionButton(...).TryShow()")
            .WithActionButton("关闭", _ => SetStatus("基础对话框已关闭。"), true)
            .TryShow());
    }

    // 2 - any Control can be the content
    private void ShowCustomContent()
    {
        SetStatus("已打开自定义内容对话框。");

        var name = new TextBox { PlaceholderText = "你的名字", Width = 280 };
        var volume = new Slider { Minimum = 0, Maximum = 100, Value = 65, Width = 280 };
        var notifications = new ToggleButton { Classes = { "Switch" }, IsChecked = true };

        var content = new StackPanel
        {
            Spacing = 14,
            Children =
            {
                new TextBlock { Text = "内容区可以放任意控件：", FontSize = 14 },
                name,
                volume,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 10,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Children =
                    {
                        new TextBlock { Text = "接收通知", VerticalAlignment = VerticalAlignment.Center },
                        notifications
                    }
                }
            }
        };

        Report(DialogManager.CreateDialog()
            .WithTitle("自定义内容")
            .WithContent(content)
            .WithActionButton("取消", _ => SetStatus("自定义内容对话框已取消。"))
            .WithActionButton("保存",
                _ => SetStatus($"已保存：名字「{name.Text}」，音量 {volume.Value:0}，通知 {notifications.IsChecked}。"),
                true, "Small")
            .TryShow());
    }

    // 3 - .Dismiss().ByClickingBackground()
    private void ShowBackgroundDismiss()
    {
        SetStatus("已打开可点击背景关闭的对话框。");

        Report(DialogManager.CreateDialog()
            .WithTitle("点击背景关闭")
            .WithContent("链式调用里加上 .Dismiss().ByClickingBackground() 即可。\n" +
                         "没有加这一句的对话框是「钉住」的：点背景只会让它弹一下。")
            .WithActionButton("知道了", _ => { }, true)
            .Dismiss().ByClickingBackground()
            .OnDismissed(_ => SetStatus("对话框已关闭（不论按钮还是背景）。"))
            .TryShow());
    }

    // 4 - await the result
    private async Task ShowConfirmAsync()
    {
        SetStatus("已打开确认框，等待你的选择…");

        var confirmed = await DialogManager.CreateDialog()
            .WithTitle("删除文件")
            .WithContent("「设计稿.psd」将被永久删除，此操作无法撤销。")
            .WithYesNoResult("删除", "取消")
            .Dismiss().ByClickingBackground()
            .TryShowAsync();

        SetStatus(confirmed
            ? "TryShowAsync() 返回 true —— 你点了「删除」。"
            : "TryShowAsync() 返回 false —— 取消或点击了背景。");
    }

    // 5 - CherylUI extra: bottom sheet
    private void ShowBottom()
    {
        SetStatus("已打开贴底对话框。");

        Report(DialogManager.CreateDialog()
            .WithTitle("从底部弹出")
            .WithContent("AtBottom() 让对话框贴在宿主底部，适合移动端的手势区。")
            .WithActionButton("关闭", _ => { }, true)
            .Dismiss().ByClickingBackground()
            .AtBottom()
            .TryShow());
    }

    // 6 - the zero wiring path
    private void ShowZeroConfig()
    {
        SetStatus("已通过 InteractiveContainer.DialogManager 打开对话框。");

        InteractiveContainer.DialogManager.CreateDialog()
            .WithTitle("零配置")
            .WithContent("这个对话框不是通过 View 里那个 Host 打开的。\n" +
                         "每个 InteractiveContainer 都内置了一个 CherylDialogHost，接的是 CherylDialogManager.Default。")
            .WithActionButton("明白了", _ => { }, true)
            .Dismiss().ByClickingBackground()
            .TryShow();
    }

    private void ShowTyped(NotificationType type, string title, string content)
    {
        SetStatus($"已打开 {type} 消息框。");

        Report(DialogManager.CreateDialog()
            .OfType(type)
            .WithTitle(title)
            .WithContent(content)
            .WithOkResult("好的")
            .Dismiss().ByClickingBackground()
            .TryShow());
    }

    private void Report(bool shown)
    {
        if (!shown)
            SetStatus("已经有一个对话框打开了 —— TryShow() 返回 false，新的被忽略。");
    }

    private void SetStatus(string status) => Status = status;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
