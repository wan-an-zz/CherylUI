using System;
using Avalonia.Controls.Notifications;

namespace CherylUI.Dialogs;

/// <summary>
/// The fluent (chained) dialog API - the CherylUI equivalent of SukiUI's
/// <c>FluentSukiDialogBuilder</c>. Everything here can be called straight from a view model:
/// <code>
/// DialogManager.CreateDialog()
///     .WithTitle("Delete file")
///     .WithContent("This cannot be undone.")
///     .WithActionButton("Cancel", _ =&gt; { })
///     .WithActionButton("Delete", _ =&gt; { }, true)
///     .Dismiss().ByClickingBackground()
///     .TryShow();
/// </code>
/// </summary>
public static class FluentCherylDialogBuilder
{
    #region Content

    /// <summary>
    /// Creates a <see cref="CherylDialogBuilder"/> used to construct a dialog.
    /// </summary>
    public static CherylDialogBuilder CreateDialog(this ICherylDialogManager manager) => new(manager);

    /// <summary>
    /// Gives the dialog a "type" which displays a colored icon bubble - message box style.
    /// </summary>
    public static CherylDialogBuilder OfType(this CherylDialogBuilder builder, NotificationType type)
    {
        builder.SetType(type);
        return builder;
    }

    /// <summary>
    /// Gives the dialog a title.
    /// </summary>
    public static CherylDialogBuilder WithTitle(this CherylDialogBuilder builder, string title)
    {
        builder.SetTitle(title);
        return builder;
    }

    /// <summary>
    /// Gives the dialog its content. A <c>Control</c>, a view model or a plain string
    /// (rendered as text) are all accepted.
    /// </summary>
    public static CherylDialogBuilder WithContent(this CherylDialogBuilder builder, object content)
    {
        builder.SetContent(content);
        return builder;
    }

    /// <summary>
    /// Gives the dialog a ViewModel. If <paramref name="isViewModelOnly"/> is true, Title and
    /// Content are ignored and only the ViewModel is rendered (view located by the usual strategy).
    /// </summary>
    public static CherylDialogBuilder WithViewModel(this CherylDialogBuilder builder,
        Func<ICherylDialog, object> viewModel, bool isViewModelOnly = true)
    {
        builder.SetViewModel(viewModel, isViewModelOnly);
        return builder;
    }

    /// <summary>
    /// Shows or hides the card background (glass surface) behind the content.
    /// </summary>
    public static CherylDialogBuilder ShowCardBackground(this CherylDialogBuilder builder, bool show)
    {
        builder.SetShowCardBackground(show);
        return builder;
    }

    /// <summary>
    /// Slides the dialog to the bottom of the host instead of centering it (CherylUI extra).
    /// </summary>
    public static CherylDialogBuilder AtBottom(this CherylDialogBuilder builder, bool atBottom = true)
    {
        builder.SetShowAtBottom(atBottom);
        return builder;
    }

    #endregion Content

    #region Dismissing

    /// <summary>
    /// Begins a dismiss chain: <c>.Dismiss().ByClickingBackground()</c>.
    /// </summary>
    public static CherylDialogBuilder.DismissDialog Dismiss(this CherylDialogBuilder builder) => new(builder);

    /// <summary>
    /// Allows the dialog to be dismissed by clicking the dimmed background.
    /// </summary>
    public static CherylDialogBuilder ByClickingBackground(this CherylDialogBuilder.DismissDialog dismissBuilder)
    {
        dismissBuilder.Builder.SetCanDismissWithBackgroundClick(true);
        return dismissBuilder.Builder;
    }

    #endregion Dismissing

    #region Actions

    /// <summary>
    /// Adds a button at the bottom right of the dialog. Any number of buttons can be added.
    /// </summary>
    /// <param name="classes">Style classes of the button; defaults to <c>Text Small</c>.</param>
    public static CherylDialogBuilder WithActionButton(this CherylDialogBuilder builder, object? content,
        Action<ICherylDialog> onClicked, bool dismissOnClick = false, params string[] classes)
    {
        builder.AddActionButton(content, onClicked, dismissOnClick, classes);
        return builder;
    }

    /// <summary>
    /// Provides a callback called when this dialog is dismissed for any reason.
    /// </summary>
    public static CherylDialogBuilder OnDismissed(this CherylDialogBuilder builder,
        Action<ICherylDialog> onDismissed)
    {
        builder.SetOnDismissed(onDismissed);
        return builder;
    }

    /// <summary>
    /// Adds a Yes and a No button that both dismiss the dialog, so
    /// <c>await builder.TryShowAsync()</c> resolves to true / false.
    /// </summary>
    public static CherylDialogBuilder WithYesNoResult(this CherylDialogBuilder builder, object? yesButtonContent,
        object? noButtonContent, params string[] classes)
    {
        builder.Completion = new TaskCompletionSource<bool>();

        builder.AddActionButton(yesButtonContent, _ => builder.Completion.TrySetResult(true), true, classes);
        builder.AddActionButton(noButtonContent, _ => builder.Completion.TrySetResult(false), true, classes);

        return builder;
    }

    /// <summary>
    /// Adds an Ok button that dismisses the dialog, so <c>await builder.TryShowAsync()</c> resolves
    /// to true.
    /// </summary>
    public static CherylDialogBuilder WithOkResult(this CherylDialogBuilder builder, object? okButtonContent,
        params string[] classes)
    {
        builder.Completion = new TaskCompletionSource<bool>();

        builder.AddActionButton(okButtonContent, _ => builder.Completion.TrySetResult(true), true, classes);

        return builder;
    }

    #endregion Actions
}
