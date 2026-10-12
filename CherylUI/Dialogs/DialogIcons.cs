using Avalonia.Media;

namespace CherylUI.Dialogs;

/// <summary>
/// Geometries used by the built-in dialog types. Path data comes from SukiUI's
/// <c>SukiUI.Content.Icons</c> (same author lineage) so the message box look matches.
/// </summary>
public static class DialogIcons
{
    /// <summary>"i" glyph, meant to sit inside a filled circle.</summary>
    public static readonly StreamGeometry InformationOutline = StreamGeometry.Parse("M11 9H13V7H11V9M11 17H13V11H11V17Z");

    /// <summary>Check mark.</summary>
    public static readonly StreamGeometry Check = StreamGeometry.Parse("M21,7L9,19L3.5,13.5L4.91,12.09L9,16.17L19.59,5.59L21,7Z");

    /// <summary>Exclamation mark, meant to sit inside a filled circle.</summary>
    public static readonly StreamGeometry AlertOutline = StreamGeometry.Parse("M 11,4L 13,4L 13,15L 11,15L 11,4 Z M 13,18L 13,20L 11,20L 11,18L 13,18 Z");
}
