using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CherylUI.Dialogs;

namespace CherylUI.Controls;

/// <summary>
/// Hosts the dialog of an <see cref="ICherylDialogManager"/>: dimmed backdrop, open / close
/// transitions and background-click dismissal. Put one anywhere in your (visual) tree:
/// <code>
/// &lt;controls:CherylDialogHost Manager="{Binding DialogManager}" /&gt;
/// </code>
/// <see cref="InteractiveContainer"/> already contains one wired to
/// <see cref="CherylDialogManager.Default"/>, so an explicit host is optional when using
/// CherylUI end to end.
/// </summary>
public class CherylDialogHost : TemplatedControl
{
    /// <summary>How long the card stays in the tree after a dismissal so the close transition can play.</summary>
    private static readonly TimeSpan CloseTransitionDuration = TimeSpan.FromMilliseconds(300);

    private ICherylDialogManager? _attachedManager;
    private bool _isAttachedToLogicalTree;
    private CancellationTokenSource? _clearDialogCts;

    public static readonly StyledProperty<ICherylDialogManager?> ManagerProperty =
        AvaloniaProperty.Register<CherylDialogHost, ICherylDialogManager?>(nameof(Manager));

    /// <summary>
    /// The manager whose dialogs are presented. Changing it re-subscribes automatically.
    /// </summary>
    public ICherylDialogManager? Manager
    {
        get => GetValue(ManagerProperty);
        set => SetValue(ManagerProperty, value);
    }

    public static readonly StyledProperty<object?> DialogProperty =
        AvaloniaProperty.Register<CherylDialogHost, object?>(nameof(Dialog));

    /// <summary>
    /// The dialog currently presented. Managed by the host - do not set by hand.
    /// </summary>
    public object? Dialog
    {
        get => GetValue(DialogProperty);
        set => SetValue(DialogProperty, value);
    }

    public static readonly StyledProperty<bool> IsDialogOpenProperty =
        AvaloniaProperty.Register<CherylDialogHost, bool>(nameof(IsDialogOpen));

    /// <summary>
    /// Whether a dialog is currently presented; drives the transitions. Managed by the host.
    /// </summary>
    public bool IsDialogOpen
    {
        get => GetValue(IsDialogOpenProperty);
        set => SetValue(IsDialogOpenProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ManagerProperty && _isAttachedToLogicalTree)
            AttachManager(Manager);
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        _isAttachedToLogicalTree = true;
        AttachManager(Manager);
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        _isAttachedToLogicalTree = false;
        DetachManager();
        IsDialogOpen = false;
    }

    private void AttachManager(ICherylDialogManager? manager)
    {
        if (ReferenceEquals(_attachedManager, manager))
            return;

        DetachManager();

        if (manager is null)
            return;

        _attachedManager = manager;
        manager.OnDialogShown += ManagerOnDialogShown;
        manager.OnDialogDismissed += ManagerOnDialogDismissed;
    }

    private void DetachManager()
    {
        if (_attachedManager is null)
            return;

        _attachedManager.OnDialogShown -= ManagerOnDialogShown;
        _attachedManager.OnDialogDismissed -= ManagerOnDialogDismissed;
        _attachedManager = null;

        CancelPendingClear();
        Dialog = null;
        IsDialogOpen = false;
    }

    private void ManagerOnDialogShown(object sender, CherylDialogManagerEventArgs args)
    {
        RunOnUiThread(() =>
        {
            // A dismissal that is still playing out must not clear the dialog we are about to show.
            CancelPendingClear();

            Dialog = args.Dialog;
            IsDialogOpen = true;
        });
    }

    private void ManagerOnDialogDismissed(object sender, CherylDialogManagerEventArgs args)
    {
        RunOnUiThread(() =>
        {
            IsDialogOpen = false;

            // Keep the instance around until the close transition has finished playing.
            CancelPendingClear();
            var cts = new CancellationTokenSource();
            _clearDialogCts = cts;
            ClearDialogLater(args.Dialog, cts);
        });
    }

    private async void ClearDialogLater(ICherylDialog dismissed, CancellationTokenSource cts)
    {
        try
        {
            await Task.Delay(CloseTransitionDuration, cts.Token).ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        if (ReferenceEquals(_clearDialogCts, cts))
        {
            cts.Dispose();
            _clearDialogCts = null;
        }

        if (ReferenceEquals(Dialog, dismissed))
            Dialog = null;
    }

    private void CancelPendingClear()
    {
        _clearDialogCts?.Cancel();
        _clearDialogCts?.Dispose();
        _clearDialogCts = null;
    }

    // Clicking the backdrop dismisses the dialog (when allowed), otherwise the card bounces -
    // the pinned-dialog feedback SukiUI gives with a spring.
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (!IsDialogOpen || Dialog is not ICherylDialog dialog)
            return;

        // Presses that landed on the card - or on one of its buttons - are not backdrop clicks.
        if (dialog is Visual card && IsWithin(card, e.Source))
            return;

        e.Handled = true;

        if (dialog.CanDismissWithBackgroundClick)
        {
            Manager?.TryDismissDialog(dialog);
            return;
        }

        // Pinned dialog: bounce it instead of closing.
        AnimationHelper.Jump(dialog as Control ?? this);
    }

    private static bool IsWithin(Visual ancestor, object? source)
    {
        for (var visual = source as Visual; visual is not null; visual = visual.GetVisualParent())
        {
            if (ReferenceEquals(visual, ancestor))
                return true;
        }

        return false;
    }

    private static void RunOnUiThread(Action action)
    {
        if (Dispatcher.UIThread.CheckAccess())
            action();
        else
            Dispatcher.UIThread.Post(action);
    }
}
