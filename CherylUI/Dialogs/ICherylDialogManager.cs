namespace CherylUI.Dialogs;

/// <summary>
/// Owns the currently visible dialog. Bind one instance to a
/// <c>CherylUI.Controls.CherylDialogHost</c> and drive dialogs from any view model.
/// Mirrors SukiUI's <c>ISukiDialogManager</c>.
/// </summary>
public interface ICherylDialogManager
{
    /// <summary>
    /// Raised whenever a dialog is shown.
    /// </summary>
    event CherylDialogManagerEventHandler? OnDialogShown;

    /// <summary>
    /// Raised whenever a dialog is dismissed.
    /// </summary>
    event CherylDialogManagerEventHandler? OnDialogDismissed;

    /// <summary>
    /// The dialog currently being presented, or null when nothing is open.
    /// </summary>
    ICherylDialog? ActiveDialog { get; }

    /// <summary>
    /// Attempts to show a dialog - if one is already shown this returns false and shows nothing.
    /// </summary>
    bool TryShowDialog(ICherylDialog dialog);

    /// <summary>
    /// Attempts to dismiss a dialog - returns false if that dialog is not the active one.
    /// </summary>
    bool TryDismissDialog(ICherylDialog dialog);

    /// <summary>
    /// Dismisses the currently active dialog, if there is one.
    /// </summary>
    void DismissDialog();
}
