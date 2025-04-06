# <img alt="logo" height="26" src="https://github.com/mob-sakai/mob-sakai/releases/download/docs/uieffect5.7.0-logo.png"/> UIEffect v5 <!-- omit in toc -->

[![](https://img.shields.io/npm/v/com.coffee.ui-effect?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.coffee.ui-effect/)
[![](https://img.shields.io/github/v/release/mob-sakai/UIEffect?include_prereleases)](https://github.com/mob-sakai/UIEffect/releases)
[![](https://img.shields.io/github/release-date/mob-sakai/UIEffect.svg)](https://github.com/mob-sakai/UIEffect/releases)  
![](https://img.shields.io/badge/Unity-2020.3+-57b9d3.svg?style=flat&logo=unity)
![](https://img.shields.io/badge/Unity-6000.0+-57b9d3.svg?style=flat&logo=unity)  
![](https://img.shields.io/badge/TextMeshPro_Support-57b9d3.svg?style=flat)
![](https://img.shields.io/badge/ShaderGraph_Support-57b9d3.svg?style=flat)
![](https://img.shields.io/badge/UPR%2FHDPR_Support-57b9d3.svg?style=flat)
![](https://img.shields.io/badge/VR_Support-57b9d3.svg?style=flat)  
[![](https://img.shields.io/github/license/mob-sakai/UIEffect.svg)](https://github.com/mob-sakai/UIEffect/blob/main/LICENSE.md)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-orange.svg)](http://makeapullrequest.com)
[![](https://img.shields.io/github/watchers/mob-sakai/UIEffect.svg?style=social&label=Watch)](https://github.com/mob-sakai/UIEffect/subscription)
[![](https://img.shields.io/twitter/follow/mob_sakai.svg?label=Follow&style=social)](https://twitter.com/intent/follow?screen_name=mob_sakai)

<< [📝 Description](#-description-) | [📌 Key Features](#-key-features) | [🎮 Demo](#-demo) | [⚙ Installation](#-installation) | [🔄 Upgrading from v4 to v5](#-upgrading-from-v4-to-v5) | [🚀 Usage](#-usage) | [🤝 Contributing](#-contributing) >>

⚠️ This README is for v5. For v4, please visit [here](https://github.com/mob-sakai/UIEffect/blob/upm/README.md).

## 📝 Description <!-- omit in toc -->

**_"Decorate your UI, simply and powerfully."_**  
UIEffect is an open-source package that allows you to intuitively apply rich Unity UI effects directly from the Inspector or via code.  
Combine various filters, such as grayscale, blur, and dissolve, to decorate your UI with a unique visual style!

![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/uieffect5.7.0-demo-title.png)

- [📌 Key Features](#-key-features)
- [🎮 Demo](#-demo)
- [⚙ Installation](#-installation)
  - [Install via OpenUPM](#install-via-openupm)
  - [Install via UPM (with Package Manager UI)](#install-via-upm-with-package-manager-ui)
  - [Install via UPM (Manually)](#install-via-upm-manually)
  - [Install as Embedded Package](#install-as-embedded-package)
  - [Import Additional Resources](#import-additional-resources)
- [🔄 Upgrading from v4 to v5](#-upgrading-from-v4-to-v5)
- [🚀 Usage](#-usage)
  - [Getting Started](#getting-started)
  - [Component: UIEffect](#component-uieffect)
    - [Tone Filter](#tone-filter)
    - [Color Filter](#color-filter)
    - [Sampling Filter](#sampling-filter)
    - [Transition Filter](#transition-filter)
    - [Target Mode](#target-mode)
    - [Blend Type](#blend-type)
    - [Shadow Mode](#shadow-mode)
    - [Gradation Mode](#gradation-mode)
    - [Edge Mode](#edge-mode)
    - [Detail Filter](#detail-filter)
    - [Others](#others)
  - [Component: UIEffectTweener](#component-uieffecttweener)
  - [Component: UIEffectReplica](#component-uieffectreplica)
  - [Usage with TextMeshPro](#usage-with-textmeshpro)
  - [Usage with SoftMaskForUGUI](#usage-with-softmaskforugui)
  - [Usage with ShaderGraph](#usage-with-shadergraph)
  - [Runtime/Editor Preset for UIEffect](#runtimeeditor-preset-for-uieffect)
  - [Usage with Code](#usage-with-code)
  - [Project Settings](#project-settings)
    - [Settings](#settings)
    - [Editor](#editor)
    - [Shader](#shader)
  - [:warning: Limitations](#warning-limitations)
- [🤝 Contributing](#-contributing)
  - [Issues](#issues)
  - [Pull Requests](#pull-requests)
  - [Support](#support)
- [License](#license)
- [Author](#author)
- [See Also](#see-also)

<br><br>

## 📌 Key Features

![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/uieffect5.7.0-keyfeatures.png)

- **UIEffect is out-of-the-box!**: Easily apply effects by adjusting parameters directly in the inspector.
- **Rich effect combinations**: Decorate your UI with a variety of effects, such as grayscale, blur, and dissolve, by combining multiple filters and parameters.
- **Efficient shader builds**: Only the shader variants in use are built, resulting in shorter build times.
- **URP/HDRP/VR support**: Compatible with URP, HDRP, and VR environments.
- **Runtime and editor presets**: Presets are available both at runtime and in the editor.
- **TextMeshPro support**: Supports TextMeshPro, including `<font>` and `<sprite>` tags.
- **UIEffectReplica Component**: Duplicate effects and apply them to multiple UI elements at once.
- **UIEffectTweener Component**: A simple tweener component to play, stop, pause, and resume effects.
- **AnimationClip support**: Allows control of effect animations using `AnimationClips`.
- **v4 compatible components (optional)**: For easy upgrading with minimal changes, compatible v4 components are available optionally.
- **SoftMaskForUGUI support (optional)**: Compatible with SoftMaskForUGUI for creating soft masks for UI elements. See for more details [here](#usage-with-softmaskforugui).
- **ShaderGraph support (optional)**: Add `Canvas (UIEffect)` sub target (material) for UIEffect shaders. See for more details [here](#usage-with-shadergraph).

<br><br>

## 🎮 Demo


![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/uieffect5.7.0-demo-available.png)  
<img src="https://github.com/mob-sakai/mob-sakai/releases/download/docs/uieffect5.7.0-demo-pattern.gif" width="582"/>  
![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/uieffect5.7.0-demo-showcase.png)  
![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/uieffect5.7.0-demo-tmp.png)  

[WebGL Demo](https://mob-sakai.github.io/UIEffect/)

<br><br>

## ⚙ Installation

_This package requires **Unity 2020.3 or later**._

### Install via OpenUPM

- This package is available on [OpenUPM](https://openupm.com/packages/com.coffee.ui-effect/) package registry.
- This is the preferred method of installation, as you can easily receive updates as they're released.
- If you have [openupm-cli](https://github.com/openupm/openupm-cli) installed, then run the following command in your project's directory:
  ```
  openupm add com.coffee.ui-effect
  ```
- To update the package, use Package Manager UI (`Window > Package Manager`) or run the following command with `@{version}`:
  ```
  openupm add com.coffee.ui-effect@5.7.0
  ```

### Install via UPM (with Package Manager UI)

- Click `Window > Package Manager` to open Package Manager UI.
- Click `+ > Add package from git URL...` and input the repository URL: `https://github.com/mob-sakai/UIEffect.git?path=Packages/src`  
  ![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/upm-add-from-url.png)
- To update the package, change suffix `#{version}` to the target version.
    - e.g. `https://github.com/mob-sakai/UIEffect.git?path=Packages/src#5.7.0`

### Install via UPM (Manually)

- Open the `Packages/manifest.json` file in your project. Then add this package somewhere in the `dependencies` block:
  ```json
  {
    "dependencies": {
      "com.coffee.ui-effect": "https://github.com/mob-sakai/UIEffect.git?path=Packages/src",
      ...
    }
  }
  ```

- To update the package, change suffix `#{version}` to the target version.
    - e.g. `"com.coffee.ui-effect": "https://github.com/mob-sakai/UIEffect.git?path=Packages/src#5.7.0",`

### Install as Embedded Package

1. Download the `Source code (zip)` file from [Releases](https://github.com/mob-sakai/UIEffect/releases) and
   extract it.
2. Move the `<extracted_dir>/Packages/src` directory into your project's `Packages` directory.  
   ![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/upm-add-as-embedded.png)
    - You can rename the `src` directory if needed.
    - If you intend to fix bugs or add features, installing it as an embedded package is recommended.
    - To update the package, re-download it and replace the existing contents.

### Import Additional Resources

Additional resources can be imported to extend functionality.

- [🔄 Upgrading from v4 to v5](#-upgrading-from-v4-to-v5)
- [Usage with TextMeshPro](#usage-with-textmeshpro)
- [Usage with ShaderGraph](#usage-with-shadergraph)

<br><br>

## 🔄 Upgrading from v4 to v5

If upgrading from UIEffect v4 to v5, note the following breaking changes:

1. If you are installing via git URL, add `?path=Packages/src`. The default branch is changed `upm` to `main`.
   ```json
   // v4
   "com.coffee.ui-effect": "https://github.com/mob-sakai/UIEffect.git",
   "com.coffee.ui-effect": "https://github.com/mob-sakai/UIEffect.git#upm",
   
   // v5
   "com.coffee.ui-effect": "https://github.com/mob-sakai/UIEffect.git?path=Packages/src",
   "com.coffee.ui-effect": "https://github.com/mob-sakai/UIEffect.git?path=Packages/src#main",
   ```

2. Import the `v4 Compatible Components` sample from the Package Manager window.  

3. All v4 components are obsolete.  
- v4 `UIEffect` component is now `UIEffectV4` component. Change the reference in the code.
- The `effectArea` property in some components are not supported in v5.
- `UIShadow`, `UIGradient` components are not supported in v5.
- v4 components can be converted to v5 `UIEffect` component by selecting `Convert To UIEffect` from the context menu.  
  ![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/uieffect5.7.0-v4-convert.png)

<br><br>

## 🚀 Usage

### Getting Started

1. [Install the package](#-installation).

2. Add a `UIEffect` component to a UI element (Image, RawImage, Text, TextMeshProUGUI, etc...) from the `Add Component` in the inspector or `Component > UI > UIEffect` menu.  
   ![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/uieffect5.7.0-add-component.png)

3. Adjust the effect filters and parameters in the inspector.  
   ![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/1744308178360.gif)

4. Enjoy!

<br><br>

### Component: UIEffect

The `UIEffect` component applies visual effects to UI elements, allowing various effects to be achieved by combining multiple filters.

![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/uieffect5.7.0-inspector-preset.png)

> [!TIP]  
> From the top menu, you can load and save the editor presets or clear settings.  
> For details, refer to the [Runtime/Editor Preset for UIEffect](#runtimeeditor-preset-for-uieffect).

<br>

#### Tone Filter

![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/uieffect5.7.0-inspector-tone.png)

- **Tone Filter**: `None`, `Grayscale`, `Sepia`, `Nega`, `Retro`, `Posterize`  
  ![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/uieffect5.7.0-sample-tone.png)
- **Tone Intensity**: 0.0 (no effect) - 1.0 (full effect).

<br>

#### Color Filter

![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/uieffect5.7.0-inspector-color.png)

- **Color Filter**: `None`, `Multiply`, `Additive`, `Subtractive`, `Replace`, `Multiply Luminance`, `Multiply Additive`, `Hsv Modifier`, `Contrast`  
  ![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/uieffect5.7.0-sample-color.png)
- **Color Intensity**: 0.0 (no effect) - 1.0 (full effect).
- **Color Glow**: Set the color to glow.

<br>

#### Sampling Filter

![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/uieffect5.7.0-inspector-sampling.png)

- **Sampling Filter**: `None`, `Blur Fast`, `Blur Medium`, `Blur Detail`, `Pixelation`, `Rgb Shift`, `Edge Luminescence`, `Edge Alpha`  
  ![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/uieffect5.7.0-sample-sampling.png)
- **Sampling Intensity**: 0.0 (no effect) - 1.0 (full effect).
- **Sampling Width**: The width of the sampling effect.

<br>

#### Transition Filter

![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/uieffect5.7.0-inspector-transition.png)

- **Transition Filter**: `None`, `Fade`, `Cutoff`, `Dissolve`, `Shiny`, `Mask`, `Melt`, `Burn`, `Pattern`  
  ![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/uieffect5.7.0-sample-transition.png)
- **Transition Rate**: 0.0 (no effect) - 1.0 (full effect).
- **Transition Tex**: The texture used for the transition filter.
    - **Scale**, **Offset**, **Speed**, **Rotation**, **Keep Aspect Ratio**, **Reverse**: Additional properties for the texture.
    - ⚠️ NOTE: `Rotation` is shared with `Transition Filter` and `Detail Filter`.
    - ⚠️ NOTE: `Keep Aspect Ratio` is shared with `Transition Filter` and `Gradation Mode` and `Detail Filter`.
- **Transition Width**: The width where the transition color is applied.
- **Transition Pattern Range**: The range of the pattern.
- **Transition Pattern Reverse**: Reverse the pattern.
- **Transition Softness**: The softness of the boundary for the transition color.
- **Transition Color Filter**: `None`, `Multiply`, `Additive`, `Subtractive`, `Replace`, `Multiply Luminance`, `Multiply Additive`, `Hsv Modifier`, `Contrast`
    - **Transition Color**: The color of the transition.
    - **Transition Color Glow**: Set the transition color to glow.
- **Transition Auto Play Speed**: The speed of the transition animation (using shader side `_Time`). You can use this property to loop the `TransitionRate` without tweener or animation.

> [!TIP]  
> **Transition Tex** applies transitions using its alpha channel.  
> If you use `Scale`, `Offset`, or `Speed`, set `Wrap Mode = Repeat` in the texture import settings.  
> For details, refer to the textures included in the built-in presets.  
> ![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/17443051097852.png)

<br>

#### Target Mode

![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/uieffect5.7.0-inspector-target.png)

- **Target Mode**: `None`, `Hue`, `Luminance`  
  ![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/uieffect5.7.0-sample-target.png)
  - Restricts the effect application area based on hue or luminance.
- **Target Range**: The range of target hue or luminance.
- **Target Softness**: The softness of the target boundary.

<br>

#### Blend Type

![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/uieffect5.7.0-inspector-blend.png)

- **Blend Type**: `Alpha Blend`, `Multiply`, `Additive`, `Soft Additive`, `Multiply Additive`, `Custom`  
  ![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/uieffect5.7.0-sample-blend.png)
  - `Custom` blend type can be set using the `SrcBlend` and `DstBlend` properties. 

<br>

#### Shadow Mode

![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/uieffect5.7.0-inspector-shadow.png)

- **Shadow Mode**: `None`, `Shadow`, `Shadow3`, `Outline`, `Outline8`, `Mirror`  
  ![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/uieffect5.7.0-sample-shadow.png)
- **Shadow Distance**: Distance of the shadow or outline.
- **Shadow Iteration**: Number of times the shadow or outline is applied.
- **Shadow Color Filter**: `None`, `Multiply`, `Additive`, `Subtractive`, `Replace`, `Multiply Luminance`, `Multiply Additive`, `Hsv Modifier`, `Contrast`
    - **Shadow Color**: The color of the shadow.
    - **Shadow Color Glow**: Set the shadow color to glow.
- **Shadow Fade**: Alpha value of the shadow or outline.
- **Shadow Blur Intensity** (`SamplingFilter = Blur***` required): Intensity of the shadow or outline blur.
- **Mirror Reflection** (`Mirror` only): Distance of the mirrored image.
- **Mirror Offset** (`Mirror` only): Offset for the mirrored image.
- **Mirror Scale** (`Mirror` only): Scale of the mirrored image.

<br>

#### Gradation Mode

![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/1744305109784.png)

- **Gradation Mode**: `None`, `Horizontal`, `HorizontalGradient`, `Vertical`, `VerticalGradient`, `Radial`, `Diagonal`, `DiagonalToRightBottom`, `DiagonalToLeftBottom`, `Angle`, `AngleGradient`  
  ![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/uieffect5.7.0-sample-gradation.png)
- **Gradation Color Filter**: `None`, `Multiply`, `Additive`, `Subtractive`, `Replace`, `Multiply Luminance`, `Multiply Additive`
- **Gradation Gradient**: The gradient of the gradation.
  - `Blend` and `Fixed` modes are available.
- **Gradation Color 1**: The first color of the gradation.
- **Gradation Color 2**: The second color of the gradation.
- **Gradation Intensity**: 0.0 (no effect) - 1.0 (full effect).
- **Gradation Offset**: The offset of the gradation range.
- **Gradation Scale**: The scale of the gradation range.
- **Gradation Rotation** (`Angle` or `AngleGradient` only): The rotation of the gradation range.
- **Keep Aspect Ratio**: 
  - ⚠️ NOTE: `Keep Aspect Ratio` is shared with `Transition Filter` and `Gradation Mode` and `Detail Filter`.

<br>

#### Edge Mode

![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/uieffect5.7.0-inspector-edge.png)

- **Edge Mode**: `None`, `Plain`, `Shiny`  
  ![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/uieffect5.7.0-sample-edge.png)
- **Edge Width**: The width of the edge.
- **Edge Color Filter**: `None`, `Multiply`, `Additive`, `Subtractive`, `Replace`, `Multiply Luminance`, `Multiply Additive`, `Hsv Modifier`, `Contrast`
    - **Edge Color**: The color of the edge.
    - **Edge Color Glow**: Set the shadow color to glow.
- **Edge Shiny Width**: The width of the edge shiny.
- **Edge Shiny Auto Play Speed**: The speed of the edge shiny (using shader side `_Time`).

<br>

#### Detail Filter

![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/uieffect5.7.0-inspector-detail.png)

- **Detail Filter**: `None`, `Masking`, `Multiply`, `Additive`, `Replace`, `MuliplyAdditive`  
  ![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/uieffect5.7.0-sample-detail.png)
- **Detail Intensity**: 0.0 (no effect) - 1.0 (full effect).
- **Detail Threshold** (`Masking` only): The minimum and maximum alpha values used for masking. The larger the gap
    between these values, the stronger the softness effect.
- **Detail Tex**: The texture used for the transition filter.
  - **Scale**, **Offset**, **Speed**, **Rotation**, **Keep Aspect Ratio**: Additional properties for the texture.
  - ⚠️ NOTE: `Rotation` is shared with `Transition Filter` and `Detail Filter`.
  - ⚠️ NOTE: `Keep Aspect Ratio` is shared with `Transition Filter` and `Gradation Mode` and `Detail Filter`.

<br>

#### Others

![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/uieffect5.7.0-inspector-others.png)

- **Sampling Scale**: The referencing scale during sampling.
    - Larger values are suitable for high-resolution textures.
- **Allow To Modify Mesh Shape**: If enabled, the mesh shape can be modified.
- **Custom Root**: Use the custom transform for some effects.
- **Flip**:
    - `Nothing`
    - `Horizontal`
    - `Vertical`
    - `Effect`: Flip with the effect.
    - `Shadow`: Flip with the shadow.  
    ![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/uieffect5.7.0-sample-flip.png)

> [!TIP]  
> `Transition Filter`, `Gradation Filter`, and `Detail Filter` usually refer to their own `RectTransform` when applying effects.  
> For example, if the mesh shape exceeds the `RectTransform` boundaries, the effect may not be applied correctly.  
> By specifying `Custom Root`, you can refer to the specified transform.
> ![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/uieffect5.7.0-sample-custom-root.gif)

<br><br>

### Component: UIEffectTweener

![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/1744304918490.png)

The `UIEffectTweener` component animates the effect, enabling easy control over effect animations without the need for an `AnimationClip`.

- **Culling Mask**: `Tone`, `Color`, `Sampling`, `Transition`, `GradiationOffset`, `GradiationRotation`, `EdgeShiny`, `Event`
- **OnChangeRate** (`Event` only): A event that is triggered when the tweening rate changes.
- **Direction**: `Forward (0.0 -> 1.0)`, `Reverse (1.0 -> 0.0)`
- **Curve**: The curve of the animation.
- **Separate Reverse Curve**: If enabled, `Reverse Curve` will be used for the reverse direction.
- **Reverse Curve**: The curve of the reverse animation.
- **Delay**: The delay time before starting the animation.
- **Duration**: The duration of the animation.
- **Interval**: The interval time between the animation.
- **Play On Enable**: `None`, `Forward`, `Reverse`, `Keep Direction`
   - Play the animation automatically when the component is enabled.
   - If `None` is selected, the animation will not play automatically. You can play it using the `Play`, `PlayForward` or `PlayReverse`  method.
- **Reset Time On Enable**: Reset the tweening time when the component is enabled.
- **Wrap Mode**:
   - `Once`: delay -> duration(0-1) -> onComplete
   - `Loop`: delay -> duration(0-1) -> interval -> ...
   - `PingPongOnce`: delay -> duration(0-1) -> interval -> duration(1-0) -> onComplete
   - `PingPongLoop`: delay -> duration(0-1) -> interval -> duration(1-0) -> interval -> ...
- **Update Mode**: `Normal`, `UnscaledTime`, `Manual`
- **On Complete**: A event that is triggered when the animation is completed.

You can preview the animation using the seek bar and play button.

<br><br>

### Component: UIEffectReplica

![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/1744308511445.png)

The `UIEffectReplica` component applies visual effects to UI elements by replicating the settings of another `UIEffect` component. This allows the same effect to be applied across multiple UI elements simultaneously.

- **Target**: The target `UIEffect` component to replicate.
  > [!TIP]
  > You can specify a preset as well as an instance as the target for `UIEffectReplica`.
- **Use Target Transform**: Use the target's transform for some effects.  
  ![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/uieffect5.7.0-sample-custom-root.gif)
- **Custom Root**: Use the custom transform for some effects.
- **Sampling Scale**: Override the sampling scale.
- **Allow To Modify Mesh Shape**: If enabled, the mesh shape can be modified.

<br><br>

### Usage with TextMeshPro

To use UIEffect with TextMeshPro, you need to import additional resources.  
When a shader included in the samples is requested, an import dialog will automatically appear.  
Click the `Import` button.

![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/1744304977486.png)

Alternatively, you can manually import the resources by following these steps:

1. First, you must import [TMP Essential Resources](https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.0/manual/index.html#installation) before using.  
   ![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/1744305000411.png)

2. Open the `Package Manager` window and select the `UI Effect` package in the package list and click the `TextMeshPro Support > Import` button.  
   ⚠️ If you are using `Unity 2023.2/6000.0+` or `TextMeshPro 3.2/4.0+`, click the `TextMeshPro Support (Unity 6) > Import` button instead.  

3. The assets will be imported under `Assets/Samples/UI Effect/{version}`.

4. Add the `UIEffect` component to a TextMeshProUGUI element and adjust the effect settings. The `<font>` and `<sprite>` tags are also supported.  
   ![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/1744305022250.png)

> [!TIP]
> `TextMeshPro Support` may change with updates to the UIEffect package.  
> If issues occur, try importing it again.

> [!TIP]
> If you have moved `TMPro_Properties.cginc` and `TMPro.cginc` from their default install path
> (`Assets/TextMesh Pro/Shaders/...`), you will need to manually update the paths in the shaders under
> `TextMeshPro Support` or `TextMeshPro Support (Unity 6)`.

<br><br>

### Usage with SoftMaskForUGUI

![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/1744305036318.png)

[SoftMaskForUGUI](https://github.com/mob-sakai/SoftMaskForUGUI) is a package that allows you to create soft masks for UI elements.

`SoftMaskForUGUI (v3.3.0+)` supports `UIEffect (v5.7.0+)`.  
When a shader included in the samples is requested, an import dialog will automatically appear.  
Click the `Import` button.

<br><br>

### Usage with ShaderGraph

![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/1744307236288.png)

To use UIEffect with ShaderGraph, you need to import additional resources.

1. Open the `Package Manager` window and select the `UI Effect` package in the package list and click the `ShaderGraph Support (Unity6, BuiltIn) > Import` button.  
   ⚠️ If you are using Universal Render Pipeline (URP), click the `ShaderGraph Support (Unity6, URP) > Import` button.

2. The assets will be imported under `Assets/Samples/UI Effect/{version}`.

3. Change the sub target (Material) of the existing ShaderGraph file to `Canvas (UIEffect)`.  
   Alternatively, create a new ShaderGraph from `Assets/Create/Shader Graph/BuiltIn/Canvas Shader Graph (UIEffect)`.

> [!TIP]
> Use `(UIEffect)` in the shader name or register as an optional shader in [ProjectSettings](#project-settings).

<br><br>

### Runtime/Editor Preset for UIEffect

![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/uieffect5.7.0-inspector-preset.png)

You can create and use presets for `UIEffect` components.

- Click the `Load` button to apply an editor preset.
> Click the `Append` button to load only non-default settings from the editor preset.
- Click the `Save As New` button to save the current settings as an editor preset.
- In the [Project Settings](#project-settings), you can register saved editor presets as runtime presets.
- Runtime presets can be loaded using the `UIEffect.LoadPreset(presetName)` method.

> [!TIP]
> Editor presets are saved in the `UIEffectPresets` directory.
> You can also create subdirectories to customize the preset menu.

<br><br>

### Usage with Code

You can control the effect settings and animations via code.

```csharp
var effect = graphic.AddComponent<UIEffect>();

// Apply a runtime preset
effect.LoadPreset("Dissolve");

// Set the effect parameters
effect.transitionWidth = 0.1f;
effect.transitionColor = Color.red;

// Add a tweener
var tweener = graphic.AddComponent<UIEffectTweener>();
tweener.cullingMask = UICullingMask.Tone;
tweener.wrapMode = UIWrapMode.PingPongLoop;

// Warm up the shader variant collection.
UIEffectProjectSettings.shaderVariantCollection.WarmUp();
```

<br><br>

### Project Settings

![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/1744305055425.png)

You can adjust the project-wide settings for UIEffect. (`Edit > Project Settings > UI > UIEffect`)

#### Settings

- **Transform Sensitivity**: `Low`, `Medium`, `High`
  - Set the sensitivity of the transformation when `Use Target Transform` is enabled in `UIEffectReplica` component.
- **Runtime Presets**: A list of presets that can be loaded at runtime. Load them using `UIEffect.LoadPreset(presetName)` method.

#### Editor

- **Use HDR Color Picker**: If enabled, the HDR color picker will be used in the inspector.

#### Shader

- **Optional Shaders (UIEffect)**: A list of shaders that will be prioritized when a ui-effect shader is
  requested.
    - If the shader is included in the list, that shader will be used.
    - If it is not in the list, the following shaders will be used in order:
        - If the shader name contains `(UIEffect)`, that shader will be used.
        - If `Hidden/<shader_name> (UIEffect)` exists, that shader will be used.
        - As a fallback, `UI/Default (UIEffect)` will be used.
- **Registered Variants**: A list of shader variants available at runtime. Use "-" button to remove unused variants,
  reducing build time and file size.
    - By default, the used ui-effect shaders will be included in the build. You can remove them if you don't need.
- **Unregistered Variants**: A list of shader variants that are not registered. Use "+" button to add variants.
- **Error On Unregistered Variant**: If enabled, an error will be displayed when an unregistered shader variant is used.
    - The shader variant will be automatically added to the `Unregistered Variants` list.

> [!IMPORTANT]
> - The setting file is usually saved in `Assets/ProjectSettings/UIEffectProjectSettings.asset`. Include this file in your version control system.
> - The setting file is automatically added as a preloaded asset in `ProjectSettings/ProjectSettings.asset`.

<br><br>

### :warning: Limitations

Here are the limitations of UIEffect:

- Shader variants used at runtime must be registered in the [Project Settings](#project-settings).
- When using UIEffect with TextMeshProUGUI, `SamplingFilter.BlurMedium` and `SamplingFilter.BlurDetail` are not supported due to performance considerations.
  - These will automatically fall back to `SamplingFilter.BlurFast`.
- UIEffect supports default UI components (`Image`, `RawImage`, `Text`) and `TextMeshProUGUI`.
  - If you want to use UIEffect with other components, create a issue or pull request.

<br><br>

## 🤝 Contributing

### Issues

Issues are incredibly valuable to this project:

- Ideas provide a valuable source of contributions that others can make.
- Problems help identify areas where this project needs improvement.
- Questions indicate where contributors can enhance the user experience.

### Pull Requests

Pull requests offer a fantastic way to contribute your ideas to this repository.  
Please refer to [CONTRIBUTING.md](https://github.com/mob-sakai/UIEffect/tree/main/CONTRIBUTING.md)
and use [develop branch](https://github.com/mob-sakai/UIEffect/tree/develop) for development.

### Support

This is an open-source project developed during my spare time.  
If you appreciate it, consider supporting me.  
Your support allows me to dedicate more time to development. 😊

[![](https://user-images.githubusercontent.com/12690315/66942881-03686280-f085-11e9-9586-fc0b6011029f.png)](https://github.com/users/mob-sakai/sponsorship)  
[![](https://user-images.githubusercontent.com/12690315/50731629-3b18b480-11ad-11e9-8fad-4b13f27969c1.png)](https://www.patreon.com/join/2343451?)

<br><br>

## License

* MIT

## Author

* ![](https://user-images.githubusercontent.com/12690315/96986908-434a0b80-155d-11eb-8275-85138ab90afa.png) [mob-sakai](https://github.com/mob-sakai) [![](https://img.shields.io/twitter/follow/mob_sakai.svg?label=Follow&style=social)](https://twitter.com/intent/follow?screen_name=mob_sakai) ![GitHub followers](https://img.shields.io/github/followers/mob-sakai?style=social)

## See Also

* GitHub page : https://github.com/mob-sakai/UIEffect
* Releases : https://github.com/mob-sakai/UIEffect/releases
* Issue tracker : https://github.com/mob-sakai/UIEffect/issues
* Change log : https://github.com/mob-sakai/UIEffect/blob/main/Packages/src/CHANGELOG.md