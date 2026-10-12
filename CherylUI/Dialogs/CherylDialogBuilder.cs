using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using CherylUI.Controls;

namespace CherylUI.Dialogs;

/// <summary>
/// Collects the configuration of a dialog and shows it through an
/// <see cref="ICherylDialogManager"/>. Never construct directly - start from
/// <c>manager.CreateDialog()</c> (see <see cref="FluentCherylDialogBuilder"/>).
/// Ported from SukiUI's <c>SukiDialogBuilder</c>.
/// </summary>
public class CherylDialogBuilder
{
    public ICherylDialogManager Manager { get; }

    public ICherylDialog Dialog { get; }

    /// <summary>
    /// Set by <c>WithOkResult</c> / <c>WithYesNoResult</c> to allow awaiting
    /// <see cref="TryShowAsync"/>.
    /// </summary>
    public TaskCompletionSource<bool>? Completion { get; set; }

    public CherylDialogBuilder(ICherylDialogManager manager)
    {
        ArgumentNullException.ThrowIfNull(manager);
        Manager = manager;
        Dialog = new CherylDialog { Manager = manager };
    }

    /// <summary>
    /// Shows the dialog. Returns false when another dialog is already open.
    /// </summary>
    public bool TryShow() => Manager.TryShowDialog(Dialog);

    /// <summary>
    /// Shows the dialog and awaits its dismissal.
    /// </summary>
    /// <returns>
    /// The dialog result: true when the OK / Yes button was pressed, false otherwise
    /// (No button, background click or an explicit <c>Dismiss()</c>).
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when another dialog is already open. Configure the dialog with
    /// <c>WithOkResult</c> / <c>WithYesNoResult</c> to get a meaningful result.
    /// </exception>
    public async Task<bool> TryShowAsync(CancellationToken cancellationToken = default)
    {
        var completion = Completion ??= new TaskCompletionSource<bool>();

        using var registration = cancellationToken.Register(() => completion.TrySetCanceled(cancellationToken));
        Dialog.OnDismissed += OnDialogDismissed;

        try
        {
            if (!Manager.TryShowDialog(Dialog))
                throw new InvalidOperationException(
                    "Opening a new dialog failed. There is already a dialog open on this manager.");

            return await completion.Task.ConfigureAwait(true);
        }
        finally
        {
            Dialog.OnDismissed -= OnDialogDismissed;
            Manager.TryDismissDialog(Dialog);
        }

        void OnDialogDismissed(ICherylDialog dialog) => completion.TrySetResult(false);
    }

    public void SetTitle(string title) => Dialog.Title = title;

    public void SetContent(object content) => Dialog.Content = content;

    public void SetShowCardBackground(bool show) => Dialog.ShowCardBackground = show;

    public void SetShowAtBottom(bool show) => Dialog.ShowAtBottom = show;

    public void SetCanDismissWithBackgroundClick(bool canDismiss) => Dialog.CanDismissWithBackgroundClick = canDismiss;

    public void SetOnDismissed(Action<ICherylDialog> onDismissed) => Dialog.OnDismissed = onDismissed;

    public void SetViewModel(Func<ICherylDialog, object> viewModel, bool isViewModelOnly)
    {
        if (isViewModelOnly)
        {
            Dialog.ViewModel = viewModel(Dialog);
            Dialog.IsViewModelOnly = true;
        }
        else
        {
            Dialog.Content = viewModel(Dialog);
            Dialog.IsViewModelOnly = false;
        }
    }

    /// <summary>
    /// Applies one of the built-in message box looks (icon bubble + gradient color).
    /// </summary>
    public void SetType(NotificationType notificationType)
    {
        Dialog.Icon = notificationType switch
        {
            NotificationType.Information => DialogIcons.InformationOutline,
            NotificationType.Success => DialogIcons.Check,
            NotificationType.Warning => DialogIcons.AlertOutline,
            NotificationType.Error => DialogIcons.AlertOutline,
            _ => throw new ArgumentOutOfRangeException(nameof(notificationType), notificationType, null)
        };

        Dialog.IconColor = notificationType switch
        {
            NotificationType.Information => NotificationColor.InfoIconForeground,
            NotificationType.Success => NotificationColor.SuccessIconForeground,
            NotificationType.Warning => NotificationColor.WarningIconForeground,
            NotificationType.Error => NotificationColor.ErrorIconForeground,
            _ => throw new ArgumentOutOfRangeException(nameof(notificationType), notificationType, null)
        };
    }

    public void AddActionButton(object? buttonContent, Action<ICherylDialog> onClicked, bool dismissOnClick,
        string[] classes)
    {
        if (classes.Length == 0)
            classes = ["Text", "Small"];

        // A plain string is wrapped in a TextBlock so CherylUI's "Button TextBlock" rules
        // (padding, font size, foreground per style class) apply, exactly like XAML buttons.
        var content = buttonContent is string text ? new TextBlock { Text = text } : buttonContent;

        var button = new Button { Content = content };
        foreach (var @class in classes)
            button.Classes.Add(@class);

        button.Click += (_, _) =>
        {
            onClicked(Dialog);

            if (dismissOnClick)
                Manager.TryDismissDialog(Dialog);
        };

        Dialog.ActionButtons.Add(button);
    }

    /// <summary>
    /// Intermediate step of the <c>.Dismiss().ByClickingBackground()</c> chain.
    /// </summary>
    public class DismissDialog
    {
        public DismissDialog(CherylDialogBuilder builder)
        {
            Builder = builder;
        }

        public CherylDialogBuilder Builder { get; }
    }
}
