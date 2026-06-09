# Blazor Text Shimmer

A native Blazor component that renders an animated **text shimmer** - a bright
gradient band that sweeps across text. Great for AI "thinking"/loading states,
progressive reveals, or drawing attention to dynamic content.

It is a from-scratch Blazor port of the shimmer effect popularized by JS/TS
component libraries such as Vercel's [AI Elements Shimmer](https://elements.ai-sdk.dev/components/shimmer)
and [motion-primitives](https://motion-primitives.com/docs/text-shimmer), built
with the same underlying CSS technique (transparent text + `background-clip: text`
over a moving gradient) but with **zero JavaScript** and **no external dependencies**.

## How it works

The text is rendered transparent and clipped to a background made of two layers:

1. A bright gradient **band** wider than the element that slides across it (the shine).
2. A flat **base color** that fills the rest of the glyphs.

Animation is a pure CSS `@keyframes` sweep of `background-position`, so the
component works in **every Blazor render mode**, including static SSR. The band
width scales with text length, matching the reference implementations.

## Project layout

```
src/
  BlazorTextShimmer.slnx           Solution
  BlazorTextShimmer/               Reusable Razor Class Library (the component)
    TextShimmer.cs                 The component (pure C#, renders a configurable tag)
    wwwroot/text-shimmer.css       The shimmer styles + keyframes
  BlazorTextShimmer.Demo/          Blazor Web App showcasing every feature
```

## Usage

1. Reference the library and add the stylesheet once in your host page
   (`App.razor` / `index.html`):

   ```razor
   <link rel="stylesheet" href="@Assets["_content/BlazorTextShimmer/text-shimmer.css"]" />
   ```

2. Add `@using BlazorTextShimmer` (e.g. in `_Imports.razor`) and use it:

   ```razor
   <TextShimmer Text="This text has a shimmer effect" />

   <TextShimmer As="h1" Text="Large heading" Duration="3" Spread="1.2" />

   <TextShimmer Text="Custom colors"
                BaseColor="#3f3f46" GradientColor="#22d3ee" />

   <TextShimmer As="span" ContentLength="22">
       Thinking <strong>really</strong> hard…
   </TextShimmer>
   ```

## Parameters

| Parameter        | Type     | Default | Description                                                                 |
| ---------------- | -------- | ------- | --------------------------------------------------------------------------- |
| `Text`           | `string?`| `null`  | Text to display; also used to scale the band width by character count.      |
| `ChildContent`   | fragment | `null`  | Rich content to shimmer. Takes precedence over `Text`.                      |
| `As`             | `string` | `"p"`   | HTML element to render (`p`, `span`, `h1`, `div`, ...).                     |
| `Duration`       | `double` | `2`     | Seconds for one full sweep. Lower = more urgent.                            |
| `Spread`         | `double` | `2`     | Band width multiplier; effective width is `Spread × character count` (px).  |
| `ContentLength`  | `int`    | `10`    | Character count used for scaling when `ChildContent` is supplied.           |
| `BaseColor`      | `string?`| `#a1a1aa` | Resting/dim text color.                                                   |
| `GradientColor`  | `string?`| `#18181b` | Bright highlight color that sweeps across.                                |
| `Class`          | `string?`| `null`  | Extra CSS class(es).                                                       |
| `ForceAnimation` | `bool`   | `false` | Keep animating even when the OS requests reduced motion (see note below).  |
| *(unmatched)*    | -        | -       | Any other attribute (`id`, `style`, `aria-*`, `data-*`) is forwarded.       |

The default colors (dim gray base with a near-black highlight) are tuned for
light backgrounds. For dark UIs, pass your own `BaseColor`/`GradientColor`
(e.g. a light/white highlight).

### Reduced motion

By default the component honors `prefers-reduced-motion: reduce` and shows a
static, dim state instead of animating. **If your OS/browser has "reduce motion"
enabled, you will not see the sweep** unless you set `ForceAnimation="true"`.
The demo sets this on every example so the effect is always visible.

## Run the demo

```bash
dotnet run --project src/BlazorTextShimmer.Demo
```

Then open the printed URL.
