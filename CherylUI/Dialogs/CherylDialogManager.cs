using Avalonia.Threading;

namespace CherylUI.Dialogs;

/// <summary>
/// Default <see cref="ICherylDialogManager"/> implementation, ported from SukiUI's
/// <c>SukiDialogManager</c> (without the internal pooling, which CheryUI does not need).
/// </summary>
public class CherylDialogManager : ICherylDialogManager
{
    /// <summary>
    /// A process-wide manager, already hosted by every <c>CherylUI.Controls.InteractiveContainer</c>.
    /// Use it when you do not want to pass a manager around:
    /// <code>InteractiveContainer.DialogManager.CreateDialog().WithTitle("Hi").TryShow();</code>
    /// </summary>
    public static CherylDialogManager Default { get; } = new();

    public event CherylDialogManagerEventHandler? OnDialogShown;
    public event CherylDialogManagerEventHandler? OnDialogDismissed;

    private ICherylDialog? _activeDialog;

    public ICherylDialog? ActiveDialog => _activeDialog;

    public bool TryShowDialog(ICherylDialog dialog)
    {
        if (_activeDialog != null)
            return false;

        _activeDialog = dialog;
        Raise(OnDialogShown, dialog);
        return true;
    }

    public bool TryDismissDialog(ICherylDialog dialog)
    {
        if (_activeDialog == null || !ReferenceEquals(_activeDialog, dialog))
            return false;

        var dismissed = _activeDialog;
        _activeDialog = null;
        Raise(OnDialogDismissed, dismissed);
        dismissed.OnDismissed?.Invoke(dismissed);
        return true;
    }

    public void DismissDialog()
    {
        if (_activeDialog is not { } dismissed)
            return;

        _activeDialog = null;
        Raise(OnDialogDismissed, dismissed);
        dismissed.OnDismissed?.Invoke(dismissed);
    }

    // View models routinely raise this from a background thread; the hosts expect the UI thread.
    private void Raise(CherylDialogManagerEventHandler? handler, ICherylDialog dialog)
    {
        if (handler is null)
            return;

        if (Dispatcher.UIThread.CheckAccess())
            handler(this, new CherylDialogManagerEventArgs(dialog));
        else
            Dispatcher.UIThread.Post(() => handler(this, new CherylDialogManagerEventArgs(dialog)));
    }
}
