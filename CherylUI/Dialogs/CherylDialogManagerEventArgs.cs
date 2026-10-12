using System;

namespace CherylUI.Dialogs;

/// <summary>
/// Handler for <see cref="ICherylDialogManager.OnDialogShown"/> and
/// <see cref="ICherylDialogManager.OnDialogDismissed"/>.
/// </summary>
public delegate void CherylDialogManagerEventHandler(object sender, CherylDialogManagerEventArgs args);

/// <summary>
/// Carries the dialog involved in a manager event.
/// </summary>
public class CherylDialogManagerEventArgs : EventArgs
{
    public CherylDialogManagerEventArgs(ICherylDialog dialog)
    {
        Dialog = dialog;
    }

    /// <summary>
    /// The dialog that was shown or dismissed.
    /// </summary>
    public ICherylDialog Dialog { get; }
}
