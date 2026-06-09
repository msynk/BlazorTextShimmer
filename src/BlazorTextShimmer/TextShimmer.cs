using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BlazorTextShimmer;

/// <summary>
/// An animated text shimmer: a bright gradient band sweeps across the text,
/// ideal for AI "thinking"/loading states or progressive reveals.
///
/// The effect is implemented with pure CSS (no JavaScript interop), so it works
/// in every Blazor render mode, including static server-side rendering.
/// </summary>
public class TextShimmer : ComponentBase
{
    /// <summary>
    /// Text to display. When set, it is also used to scale the shimmer band
    /// width based on the number of characters (matching the reference
    /// implementations). Ignored when <see cref="ChildContent"/> is supplied.
    /// </summary>
    [Parameter]
    public string? Text { get; set; }

    /// <summary>
    /// Arbitrary content to shimmer. Takes precedence over <see cref="Text"/>.
    /// When used, supply <see cref="ContentLength"/> (or rely on the default)
    /// so the band width can be scaled.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The HTML element to render. Defaults to <c>"p"</c>. Use <c>"span"</c>
    /// for inline text, <c>"h1"</c> for headings, etc.
    /// </summary>
    [Parameter]
    public string As { get; set; } = "p";

    /// <summary>
    /// Duration of one full shimmer sweep, in seconds. Default is 2.
    /// Shorter feels urgent; longer feels calm.
    /// </summary>
    [Parameter]
    public double Duration { get; set; } = 2;

    /// <summary>
    /// Controls how wide the bright band is. The effective band width (px) is
    /// <c>Spread × character count</c>, so longer text gets a proportionally
    /// wider shine. Default is 2.
    /// </summary>
    [Parameter]
    public double Spread { get; set; } = 2;

    /// <summary>
    /// Character count used for band-width scaling when <see cref="ChildContent"/>
    /// is supplied (since its length cannot be measured). Defaults to 10.
    /// Ignored when <see cref="Text"/> is used.
    /// </summary>
    [Parameter]
    public int ContentLength { get; set; } = 10;

    /// <summary>Resting/dim text color. When null, the stylesheet default (theme-aware) is used.</summary>
    [Parameter]
    public string? BaseColor { get; set; }

    /// <summary>Bright shimmer color. When null, the stylesheet default (theme-aware) is used.</summary>
    [Parameter]
    public string? GradientColor { get; set; }

    /// <summary>Additional CSS class(es) appended to the component's own class.</summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// When <c>true</c>, the shimmer keeps animating even if the user/OS has
    /// requested reduced motion (<c>prefers-reduced-motion: reduce</c>).
    /// Defaults to <c>false</c> so the component is accessible by default.
    /// </summary>
    [Parameter]
    public bool ForceAnimation { get; set; }

    /// <summary>Any other attributes (id, style, aria-*, data-*, ...) are forwarded to the root element.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    private int EffectiveLength => Text is not null ? Text.Length : ContentLength;

    private string CssClass =>
        string.IsNullOrWhiteSpace(Class) ? "blazor-text-shimmer" : $"blazor-text-shimmer {Class}";

    private string BuildStyle()
    {
        var spreadPx = Math.Max(0, EffectiveLength * Spread);
        var ci = CultureInfo.InvariantCulture;

        var style =
            $"--bts-spread:{spreadPx.ToString(ci)}px;" +
            $"--bts-duration:{Duration.ToString(ci)}s;";

        if (!string.IsNullOrWhiteSpace(BaseColor))
            style += $"--bts-base-color:{BaseColor};";
        if (!string.IsNullOrWhiteSpace(GradientColor))
            style += $"--bts-gradient-color:{GradientColor};";

        // Preserve any caller-supplied inline style by appending it.
        if (AdditionalAttributes is not null &&
            AdditionalAttributes.TryGetValue("style", out var userStyle) &&
            userStyle is not null)
        {
            style += userStyle.ToString();
        }

        return style;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var tag = string.IsNullOrWhiteSpace(As) ? "p" : As;

        builder.OpenElement(0, tag);

        // Spread caller attributes first so our explicit class/style win.
        if (AdditionalAttributes is not null)
        {
            builder.AddMultipleAttributes(1, AdditionalAttributes);
        }

        builder.AddAttribute(2, "class", CssClass);
        builder.AddAttribute(3, "style", BuildStyle());

        if (ForceAnimation)
        {
            builder.AddAttribute(4, "data-bts-force-motion", string.Empty);
        }

        if (ChildContent is not null)
        {
            builder.AddContent(5, ChildContent);
        }
        else if (Text is not null)
        {
            builder.AddContent(6, Text);
        }

        builder.CloseElement();
    }
}
