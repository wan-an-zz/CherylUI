using System;
using System.Collections.ObjectModel;
using Avalonia.Media;

namespace CherylUI.Dialogs;

/// <summary>
/// A single dialog that a <see cref="ICherylDialogManager"/> can show.
/// Mirrors SukiUI's <c>ISukiDialog</c> so the fluent builder API feels identical.
/// </summary>
public interface ICherylDialog
{
    /// <summary>
    /// The manager currently presenting this dialog.
    /// </summary>
    ICherylDialogManager? Manager { get; set; }

    /// <summary>
    /// When <see cref="IsViewModelOnly"/> is true this object is rendered as-is (located by an
    /// <c>IDataTemplate</c> / view locator) instead of the Title + Content layout.
    /// </summary>
    object? ViewModel { get; set; }

    /// <summary>
    /// Dialog title, rendered centered above the content.
    /// </summary>
    string? Title { get; set; }

    /// <summary>
    /// Dialog body. A <c>Control</c>, a view model or a plain string are all accepted.
    /// </summary>
    object? Content { get; set; }

    /// <summary>
    /// Icon geometry shown inside the colored bubble on top of the dialog (message box style).
    /// </summary>
    object? Icon { get; set; }

    /// <summary>
    /// Render <see cref="ViewModel"/> only, ignoring <see cref="Title"/> / <see cref="Content"/>.
    /// </summary>
    bool IsViewModelOnly { get; set; }

    /// <summary>
    /// Brush used for the icon bubble. A null value hides the bubble entirely.
    /// </summary>
    IBrush? IconColor { get; set; }

    /// <summary>
    /// Buttons rendered at the bottom right of the dialog, added by
    /// <c>WithActionButton</c> / <c>WithOkResult</c> / <c>WithYesNoResult</c>.
    /// </summary>
    ObservableCollection<object> ActionButtons { get; }

    /// <summary>
    /// Called whenever the dialog is dismissed, for any reason.
    /// </summary>
    Action<ICherylDialog>? OnDismissed { get; set; }

    /// <summary>
    /// When true, clicking the dimmed background dismisses the dialog.
    /// </summary>
    bool CanDismissWithBackgroundClick { get; set; }

    /// <summary>
    /// Show the glass card surface behind the content.
    /// </summary>
    bool ShowCardBackground { get; set; }

    /// <summary>
    /// Show the dialog at the bottom of the host instead of centered (CherylUI extra).
    /// </summary>
    bool ShowAtBottom { get; set; }

    /// <summary>
    /// Dismisses this dialog through its <see cref="Manager"/>.
    /// </summary>
    void Dismiss();

    /// <summary>
    /// Restores every property to its default so the instance can be reused.
    /// </summary>
    void ResetToDefault();
}
