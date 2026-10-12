using Avalonia;
using Avalonia.Media;

namespace CherylUI.Dialogs;

/// <summary>
/// Icon brushes used by <c>OfType(NotificationType)</c> message box style dialogs.
/// Values are taken from SukiUI's <c>NotificationColor</c> so the two libraries look the same.
/// </summary>
public static class NotificationColor
{
    public static readonly LinearGradientBrush InfoIconForeground = Gradient(0x59, 0x7E, 0xF7, 0x2F, 0x54, 0xEB);

    public static readonly LinearGradientBrush SuccessIconForeground = Gradient(0x52, 0xC4, 0x1A, 0x38, 0x9E, 0x0D);

    public static readonly LinearGradientBrush WarningIconForeground = Gradient(0xFF, 0xA9, 0x40, 0xFA, 0x8C, 0x16);

    public static readonly LinearGradientBrush ErrorIconForeground = Gradient(0xFF, 0x4D, 0x4F, 0xF5, 0x22, 0x2D);

    private static LinearGradientBrush Gradient(byte r1, byte g1, byte b1, byte r2, byte g2, byte b2) => new()
    {
        StartPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
        EndPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
        GradientStops =
        {
            new GradientStop(Color.FromRgb(r1, g1, b1), 0.3),
            new GradientStop(Color.FromRgb(r2, g2, b2), 1)
        }
    };
}
