using System;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using CherylUI.Dialogs;

namespace CherylUI.Controls;

/// <summary>
/// The dialog surface rendered by a <see cref="CherylDialogHost"/>: glass card, centered title,
/// content and a row of action buttons, plus an optional colored icon bubble on top.
/// Ported from SukiUI's <c>SukiDialog</c>; the look lives in <c>Styles/DialogStyles.axaml</c>.
/// </summary>
public class CherylDialog : TemplatedControl, ICherylDialog
{
    public static readonly StyledProperty<object?> ViewModelProperty =
        AvaloniaProperty.Register<CherylDialog, object?>(nameof(ViewModel));

    public object? ViewModel
    {
        get => GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<CherylDialog, string?>(nameof(Title));

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly StyledProperty<object?> ContentProperty =
        AvaloniaProperty.Register<CherylDialog, object?>(nameof(Content));

    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public static readonly StyledProperty<bool> IsViewModelOnlyProperty =
        AvaloniaProperty.Register<CherylDialog, bool>(nameof(IsViewModelOnly));

    public bool IsViewModelOnly
    {
        get => GetValue(IsViewModelOnlyProperty);
        set => SetValue(IsViewModelOnlyProperty, value);
    }

    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<CherylDialog, object?>(nameof(Icon));

    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly StyledProperty<bool> ShowCardBackgroundProperty =
        AvaloniaProperty.Register<CherylDialog, bool>(nameof(ShowCardBackground), defaultValue: true);

    public bool ShowCardBackground
    {
        get => GetValue(ShowCardBackgroundProperty);
        set => SetValue(ShowCardBackgroundProperty, value);
    }

    public static readonly StyledProperty<bool> ShowAtBottomProperty =
        AvaloniaProperty.Register<CherylDialog, bool>(nameof(ShowAtBottom));

    public bool ShowAtBottom
    {
        get => GetValue(ShowAtBottomProperty);
        set => SetValue(ShowAtBottomProperty, value);
    }

    public static readonly StyledProperty<IBrush?> IconColorProperty =
        AvaloniaProperty.Register<CherylDialog, IBrush?>(nameof(IconColor));

    public IBrush? IconColor
    {
        get => GetValue(IconColorProperty);
        set => SetValue(IconColorProperty, value);
    }

    public static readonly StyledProperty<ObservableCollection<object>> ActionButtonsProperty =
        AvaloniaProperty.Register<CherylDialog, ObservableCollection<object>>(nameof(ActionButtons));

    public ObservableCollection<object> ActionButtons
    {
        get => GetValue(ActionButtonsProperty);
        set => SetValue(ActionButtonsProperty, value);
    }

    /// <inheritdoc />
    public ICherylDialogManager? Manager { get; set; }

    /// <inheritdoc />
    public Action<ICherylDialog>? OnDismissed { get; set; }

    /// <inheritdoc />
    public bool CanDismissWithBackgroundClick { get; set; }

    public CherylDialog()
    {
        ActionButtons = [];
    }

    /// <inheritdoc />
    public void Dismiss() => Manager?.TryDismissDialog(this);

    /// <inheritdoc />
    public void ResetToDefault()
    {
        ActionButtons.Clear();
        ViewModel = null;
        Title = null;
        Content = null;
        IsViewModelOnly = false;
        Icon = null;
        IconColor = null;
        ShowCardBackground = true;
        ShowAtBottom = false;
        CanDismissWithBackgroundClick = false;
        OnDismissed = null;
        Manager = null;
    }
}
