# <img alt="logo" height="26" src="https://github.com/user-attachments/assets/dc72baf5-eadf-463e-abc9-464477fd0b9e"/> UIEffect v5 <!-- omit in toc -->

[![](https://img.shields.io/npm/v/com.coffee.ui-effect?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.coffee.ui-effect/)
[![](https://img.shields.io/github/v/release/mob-sakai/UIEffect?include_prereleases)](https://github.com/mob-sakai/UIEffect/releases)
[![](https://img.shields.io/github/release-date/mob-sakai/UIEffect.svg)](https://github.com/mob-sakai/UIEffect/releases)  
![](https://img.shields.io/badge/Unity-2020.3+-57b9d3.svg?style=flat&logo=unity)
![](https://img.shields.io/badge/Unity-6000.0+-57b9d3.svg?style=flat&logo=unity)
![](https://img.shields.io/badge/TextMeshPro_Support-57b9d3.svg?style=flat)
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

![](https://github.com/user-attachments/assets/bce4ddb2-766f-4bed-bad2-fda91bb8133c)

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
  - [Component: UIEffectTweener](#component-uieffecttweener)
  - [Component: UIEffectReplica](#component-uieffectreplica)
  - [Usage with TextMeshPro](#usage-with-textmeshpro)
  - [Runtime/Editor Preset for UIEffect](#runtimeeditor-preset-for-uieffect)
  - [Usage with Code](#usage-with-code)
  - [Project Settings](#project-settings)
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

![](https://github.com/user-attachments/assets/e5112713-166d-451f-8090-501e6ea156ca)

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

<br><br>

## 🎮 Demo

![](https://github.com/user-attachments/assets/c52f6f40-5fae-484f-b69e-516da706ae54)  
![](https://github.com/user-attachments/assets/02ab1686-03d4-4500-bd89-004b2c401441)

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
  openupm add com.coffee.ui-effect@5.0.0
  ```

### Install via UPM (with Package Manager UI)

- Click `Window > Package Manager` to open Package Manager UI.
- Click `+ > Add package from git URL...` and input the repository URL: `https://github.com/mob-sakai/UIEffect.git?path=Packages/src`  
  ![](https://github.com/user-attachments/assets/f88f47ad-c606-44bd-9e86-ee3f72eac548)
- To update the package, change suffix `#{version}` to the target version.
    - e.g. `https://github.com/mob-sakai/UIEffect.git?path=Packages/src#5.0.0`

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
    - e.g. `"com.coffee.ui-effect": "https://github.com/mob-sakai/UIEffect.git?path=Packages/src#5.0.0",`

### Install as Embedded Package

1. Download the `Source code (zip)` file from [Releases](https://github.com/mob-sakai/UIEffect/releases) and
   extract it.
2. Move the `<extracted_dir>/Packages/src` directory into your project's `Packages` directory.  
   ![](https://github.com/user-attachments/assets/187cbcbe-5922-4ed5-acec-cf19aa17d208)
    - You can rename the `src` directory if needed.
    - If you intend to fix bugs or add features, installing it as an embedded package is recommended.
    - To update the package, re-download it and replace the existing contents.

### Import Additional Resources

Additional resources can be imported to extend functionality.

- [🔄 Upgrading from v4 to v5](#-upgrading-from-v4-to-v5)
- [Usage with TextMeshPro](#usage-with-textmeshpro)

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
   ```

2. Import the `v4 Compatible Components` sample from the Package Manager window.  
   ![](https://github.com/user-attachments/assets/a1b246d3-3a0b-4a70-a9d2-3e75429fa757)

3. All v4 components are obsolete.  
- v4 `UIEffect` component is now `UIEffectV4` component. Change the reference in the code.
- The `effectArea` property in some components are not supported in v5.
- `UIShadow`, `UIGradient`, `UIFlip` components are not supported in v5.
- v4 components can be converted to v5 `UIEffect` component by selecting `Convert To UIEffect` from the context menu.  
  ![](https://github.com/user-attachments/assets/4911f06d-4749-4aa8-a72e-deb0fa06bc5d)

<br><br>

## 🚀 Usage

### Getting Started

1. [Install the package](#-installation).

2. Add a `UIEffect` component to a UI element (Image, RawImage, Text, TextMeshProUGUI, etc...) from the `Add Component` in the inspector or `Component > UI > UIEffect` menu.  
   ![](https://github.com/user-attachments/assets/9e9de844-b265-489e-b8bd-51760996cfd2)

3. Adjust the effect filters and parameters in the inspector.  
   ![](https://github.com/user-attachments/assets/38c2df5a-d7f7-4062-a595-8d3e887855cf)

4. Enjoy!

<br><br>

### Component: UIEffect

![](https://github.com/user-attachments/assets/f1d0d8a3-a0cf-4595-bda2-78165527a99b)

The `UIEffect` component applies visual effects to UI elements, allowing various effects to be achieved by combining multiple filters.

#### Tone Filter

- **Tone Filter**: `None`, `Grayscale`, `Sepia`, `Nega`, `Retro`, `Posterize`
- **Tone Intensity**: 0.0 (no effect) - 1.0 (full effect).

#### Color Filter

- **Color Filter**: `None`, `Multiply`, `Additive`, `Subtractive`, `Replace`, `Multiply Luminance`, `Multiply Additive`, `Hsv Modifier`, `Contrast`
- **Color Intensity**: 0.0 (no effect) - 1.0 (full effect).
- **Color Glow**: Set the color to glow.

#### Sampling Filter

- **Sampling Filter**: `None`, `Blur Fast`, `Blur Medium`, `Blur Detail`, `Pixelation`, `Rgb Shift`, `Edge Luminescence`, `Edge Alpha`
- **Sampling Intensity**: 0.0 (no effect) - 1.0 (full effect).
- **Sampling Scale**: The referencing scale during sampling.
    - Larger values are suitable for high-resolution textures.

#### Transition Filter

- **Transition Filter**: `None`, `Fade`, `Cutoff`, `Dissolve`, `Shiny`, `Mask`, `Melt`, `Burn`
- **Transition Rate**: 0.0 (no effect) - 1.0 (full effect).
- **Transition Tex**: The texture used for the transition filter.- **Transition Width**: The width where the transition color is applied.
- **Transition Softness**: The softness of the boundary for the transition color.
- **Transition Color Filter**: Specifies the transition color.
    - **Transition Color**: The color of the transition.
    - **Transition Color Glow**: Set the transition color to glow.

#### Target Mode

- **Target Mode**: `None`, `Hue`, `Luminance`
  - Restricts the effect application area based on hue or luminance.
- **Target Range**: The range of target hue or luminance.
- **Target Softness**: The softness of the target boundary.

#### Blend Type

- **Blend Type**: `Alpha Blend`, `Multiply`, `Additive`, `Soft Additive`, `Multiply Additive`, `Custom`
  - `Custom` blend type can be set using the `SrcBlend` and `DstBlend` properties. 

#### Shadow Mode

- **Shadow Mode**: `None`, `Shadow`, `Shadow3`, `Outline`, `Outline8`, `Mirror`
- **Shadow Distance**: Distance of the shadow or outline.
- **Shadow Iteration**: Number of times the shadow or outline is applied.
- **Shadow Color Filter**: Specifies the shadow color.
    - **Shadow Color**: The color of the shadow.
    - **Shadow Color Glow**: Set the shadow color to glow.
- **Shadow Fade**: Alpha value of the shadow or outline.
- **Shadow Blur Intensity**: Intensity of the shadow or outline blur.
- **Mirror Reflection**: Distance of the mirrored image.
- **Mirror Offset**: Offset for the mirrored image.
- **Mirror Scale**: Scale of the mirrored image.

#### Gradation Mode

- **Gradation Mode**: `None`, `Horizontal Gradient`, `Horizontal`, `Vertical Gradient`, `Vertical`, `Radial Fast`, `Radial Detail`, `Diagonal To Right Bottom`, `Diagonal To Right Top`, `Diagonal To Left Bottom`, `Angle`, `Angle Gradient`
- **Gradation Gradient**: The gradient of the gradation.
- **Gradation Color 1**: The first color of the gradation.
- **Gradation Color 2**: The second color of the gradation.
- **Gradation Offset**: The offset of the gradation range.
- **Gradation Scale**: The scale of the gradation range.
- **Gradation Rotation**: The rotation of the gradation range.

> [!TIP]
> `Horizontal Gradient` and `Vertical Gradient` divide the mesh horizontally or vertically and apply a gradient.
> This is very fast but only supports Full Rect.  
> `Angle Gradient` divides the mesh at the specified angle and applies a gradient.  
> It can be applied to meshes other than Full Rect because it applies the gradient according to the original mesh shape.

#### Others

- **Allow To Modify Mesh Shape**: If enabled, the mesh shape can be modified.

<br><br>

### Component: UIEffectTweener

![](https://github.com/user-attachments/assets/118df4dc-3c08-4e2b-b6c8-97f15c9e5c9f)

The `UIEffectTweener` component animates the effect, enabling easy control over effect animations without the need for an `AnimationClip`.

- **Culling Mask**: `Tone`, `Color`, `Sampling`, `Transition`
- **Direction**: `Forward (0.0 -> 1.0)`, `Reverse (1.0 -> 0.0)`
- **Curve**: The curve of the animation.
- **Delay**: The delay time before starting the animation.
- **Duration**: The duration of the animation.
- **Interval**: The interval time between the animation.
- **Play On Enable**: `None`, `Forward`, `Reverse`, `Keep Direction`
   - Play the animation automatically when the component is enabled.
   - If `None` is selected, the animation will not play automatically. You can play it using the `Play`, `PlayForward` or `PlayReverse`  method.
- **Wrap Mode**: `Once`, `Loop`, `PingPongOnce`, `PingPongLoop`
- **Update Mode**: `Normal`, `UnscaledTime`, `Manual`
- **On Complete**: A event that is triggered when the animation is completed.

You can preview the animation using the seek bar and play button.

<br><br>

### Component: UIEffectReplica

![](https://github.com/user-attachments/assets/27d1840f-77c2-45cd-bde8-716626479c46)

The `UIEffectReplica` component applies visual effects to UI elements by replicating the settings of another `UIEffect` component. This allows the same effect to be applied across multiple UI elements simultaneously.

- **Target**: The target `UIEffect` component to replicate.
- **Use Target Transform**: Use the target's transform for some effects.
- **Sampling Scale**: Override the sampling scale.  
  ![](https://github.com/user-attachments/assets/127e30fb-15c7-4002-ab65-e8f29e640330)

<br><br>

### Usage with TextMeshPro

To use UIEffect with TextMeshPro, you need to import additional resources.  
When a shader included in the samples is requested, an import dialog will automatically appear.  
Click the `Import` button.

![](https://github.com/user-attachments/assets/78b262fc-1581-49f2-8c93-06d591b525c6)

Alternatively, you can manually import the resources by following these steps:

1. First, you must import [TMP Essential Resources](https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.0/manual/index.html#installation) before using.  
   ![](https://github.com/user-attachments/assets/70653ccf-0b5e-4352-ac62-76bdd49c5f92)

2. Open the `Package Manager` window and select the `UI Effect` package in the package list and click the `TextMeshPro Support > Import` button.  
   ⚠️ If you are using `Unity 2023.2/6000.0+` or `TextMeshPro 3.2/4.0+`, click the `TextMeshPro Support (Unity 6) > Import` button instead.  
   ![](https://github.com/user-attachments/assets/a1b246d3-3a0b-4a70-a9d2-3e75429fa757)

3. The assets will be imported under `Assets/Samples/UI Effect/{version}`.

4. Add the `UIEffect` component to a TextMeshProUGUI element and adjust the effect settings. The `<font>` and `<sprite>` tags are also supported.  
   ![](https://github.com/user-attachments/assets/bfd28f36-2e32-4383-8628-276b02579fd4)

<br><br>

### Runtime/Editor Preset for UIEffect

![](https://github.com/user-attachments/assets/4244d162-a768-463e-8aad-8cafbdf3a5b3)

You can create and use presets for `UIEffect` components.
- In the `UIEffect` component inspector, click "Save As" button to save the current settings as an editor preset.
- Click "Load" button to apply an editor preset.
- In the [Project Settings](#project-settings), you can register saved editor presets as runtime presets.
- Runtime presets can be loaded using the `UIEffect.LoadPreset(presetName)` method.

<br><br>

### Usage with Code

You can control the effect settings and animations via code.

```csharp
var effect = graphic.AddComponent<UIEffect>();

// Apply a preset
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

![](https://github.com/user-attachments/assets/54dd42cf-099d-4fb1-b699-cad29bf211b6)

You can adjust the project-wide settings for UIEffect. (`Edit > Project Settings > UI > UIEffect`)

- **Transform Sensitivity**: `Low`, `Medium`, `High`
  - Set the sensitivity of the transformation when `Use Target Transform` is enabled in `UIEffectReplica` component.
- **Runtime Presets**: A list of presets that can be loaded at runtime. Load them using `UIEffect.LoadPreset(presetName)` method.
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

<br><br>

### :warning: Limitations

Here are the limitations of UIEffect:

- Shader variants used at runtime must be registered in the [Project Settings](#project-settings).
- When using UIEffect with TextMeshProUGUI, `SamplingFilter.BlurMedium` and `SamplingFilter.BlurDetail` are not supported due to performance considerations.
  - These will automatically fall back to `SamplingFilter.BlurFast`.

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
* © UTJ/UCL

## Author

* ![](https://user-images.githubusercontent.com/12690315/96986908-434a0b80-155d-11eb-8275-85138ab90afa.png) [mob-sakai](https://github.com/mob-sakai) [![](https://img.shields.io/twitter/follow/mob_sakai.svg?label=Follow&style=social)](https://twitter.com/intent/follow?screen_name=mob_sakai) ![GitHub followers](https://img.shields.io/github/followers/mob-sakai?style=social)

## See Also

* GitHub page : https://github.com/mob-sakai/UIEffect
* Releases : https://github.com/mob-sakai/UIEffect/releases
* Issue tracker : https://github.com/mob-sakai/UIEffect/issues
* Change log : https://github.com/mob-sakai/UIEffect/blob/main/Packages/src/CHANGELOG.md
