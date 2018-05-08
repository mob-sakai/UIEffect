UIEffect
===

UIEffect is an effect component for uGUI in Unity.  
Let's decorate your UI with effects!

[![](https://img.shields.io/github/release/mob-sakai/UIEffect.svg?label=latest%20version)](https://github.com/mob-sakai/UIEffect/release)
[![](https://img.shields.io/github/release-date/mob-sakai/UIEffect.svg)](https://github.com/mob-sakai/UIEffect/releases)  
![](https://img.shields.io/badge/requirement-Unity%205.5%2B-green.svg)
[![](https://img.shields.io/github/license/mob-sakai/UIEffect.svg)](https://github.com/mob-sakai/UIEffect/blob/master/LICENSE.txt)  
[![](https://img.shields.io/github/last-commit/mob-sakai/UIEffect/develop.svg?label=last%20commit%20to%20develop)](https://github.com/mob-sakai/UIEffect/commits/develop)
[![](https://img.shields.io/github/issues/mob-sakai/UIEffect.svg)](https://github.com/mob-sakai/UIEffect/issues)



<< [Description](#Description) | [WebGL Demo](#demo) | [Download](https://github.com/mob-sakai/UIEffect/releases) | [Usage](#usage) | [Development Note](#development-note) | [Change log](https://github.com/mob-sakai/UIEffect/blob/develop/CHANGELOG.md) >>



<br><br><br><br>
## Description

### Supports following effects!
![image](https://user-images.githubusercontent.com/12690315/35077815-5ff8094e-fc42-11e7-92dd-99916c9f9da7.png)

* Grayscale
* Sepia tone
* Nega tone
* Pixelation
* Hue shift
* Cutoff (alpha-based)
* Mono (alpha-based)
* Color overwrite
* Color additive
* Color subtract
* Blur fast(3x4)
* Blur medium(6x4)
* Blur detail(6x8)
* Shadow with effect
    * Shadow
    * Shadow3
    * Outline
    * Outline8
* Gradient vertex color
    * Horizontal
    * Vertical
    * Angle
    * Diagonal
* Shiny
* Dissolve


### Easy to control effect parameters!

![image](https://user-images.githubusercontent.com/12690315/38594668-636dd3ac-3d82-11e8-9951-820964a6a95f.gif)


### And, more features!

* Control the effect parameters without instantiating the material
* Control the effect parameters with AnimationClip  
![image](https://user-images.githubusercontent.com/12690315/34617750-cf2c9464-f27f-11e7-8ee9-e6be3b209943.gif)
* Use for screen transition  
![image](https://user-images.githubusercontent.com/12690315/34618554-71a32e40-f282-11e7-8b78-6948c50c6b58.gif)
* Multiple shadow effect to reduce rendering vertices  
![image](https://user-images.githubusercontent.com/12690315/34552373-600fdab2-f164-11e7-8565-21c15af92a93.png)
* UICapturedEffectImage: The captured image with effect, like post effect  
![image](https://user-images.githubusercontent.com/12690315/34619147-868cb36a-f284-11e7-8122-b924ff09077f.gif)
* UICapturedEffectImage: Dialog window with the captured image effect  
![image](https://user-images.githubusercontent.com/12690315/34619468-97e3c134-f285-11e7-90b2-3a75bde13911.gif)
* UIGradient: Change vertex color as gradient, with rotation, offset, color correction  
![image](https://user-images.githubusercontent.com/12690315/38594997-2a39f03c-3d84-11e8-865d-9ce12a8bb3db.gif)
* UIShiny: Shiny effect WITHOUT Mask component. This will suppress extra draw calls and improve performance.  
Merged from [ShinyEffectForUGUI](https://github.com/mob-sakai/ShinyEffectForUGUI).  
![image](https://user-images.githubusercontent.com/12690315/38501362-c56e3768-3c47-11e8-9ec1-50343d8b83ad.gif)
* UIDissolve: Dissolve effect WITHOUT material instancing. This will suppress extra draw calls and improve performance.  
Merged from [DissolveEffectForUGUI](https://github.com/mob-sakai/DissolveEffectForUGUI).  
![demo](https://user-images.githubusercontent.com/12690315/39131616-dcf7ea60-474a-11e8-8e20-f9e5bd8b3f5c.gif)



<br><br><br><br>
## Demo

[WebGL Demo](http://mob-sakai.github.io/UIEffect)

* Effect sample
* Transition
* Dialog window with blured background
* Included in unitypackage




<br><br><br><br>
## Usage

1. Download UIEffect.unitypackage from [Releases](https://github.com/mob-sakai/UIEffect/releases).
1. Import the package into your Unity project. Select `Import Package > Custom Package` from the `Assets` menu.
1. Add `UIEffect` component to UI element (Image, RawImage, Text, etc...) from `Add Component` in inspector.
1. Choose effect type and adjust values in inspector.  
![image](https://user-images.githubusercontent.com/12690315/34595809-3838dc54-f21e-11e7-858b-72821dca8b44.png)
1. Enjoy!


##### Requirement

* Unity 5.5+ *(included Unity 2017.x)*
* No other SDK are required




<br><br><br><br>
## Development Note

#### How does UIEffectCapturedImage work?

![image](https://user-images.githubusercontent.com/12690315/34619147-868cb36a-f284-11e7-8122-b924ff09077f.gif)

`UIEffectCapturedImage` is similar to post effect, but uses `CommandBuffer` to give effect only on the rendering result (= captured image) of a specific frame.
This effect is non-realtime, light-weight, less-camera, blit only once, but effective.

* Camera for processing effect is unnecessary.
* Process effect only once after `UIEffectCapturedImage.Capture`.
* Using reduction buffer, keep used memory size small and keep rendering load are small.
* When GameObjects with motion are on the screen, a result texture may be stirred.
* You can overlay and display like as: [Screen] | [UIEffectCapturedImage] | [Dialog A] | [UIEffectCapturedImage] | [Dialog B].  
See also [Demo](#demo).


#### Why is UIEffect lightweight?

* UIEffect pre-generates material from a combination of effects. This has the following benefits.
    * Draw call batching If possible, draw calls will decrease.
    * Since only the required material and shader variants are included in the build, the build size will be smaller.


#### How to control effect parameters for uGUI element WITHOUT MaterialPropertyBlock?

* In general, you can use [MaterialPropertyBlock](https://docs.unity3d.com/ScriptReference/MaterialPropertyBlock.html) for renderers to control minor changes in the material without different batches.
* However, changing the [MaterialPropertyBlock](https://docs.unity3d.com/ScriptReference/MaterialPropertyBlock.html) of the uGUI element from the script will cause different batches and draw calls to increase.
* So UIEffect encodes multiple effect parameters to UV1 channel with [IMeshModifier](https://docs.unity3d.com/ScriptReference/UI.IMeshModifier.html).
    * Pack four 6-bit [0-1] (64 steps) parameters into one float value.
    * The parameters are low precision, but sufficient.

| uv1 | 6-bit [0-1] | 6-bit [0-1] | 6-bit [0-1] | 6-bit [0-1] |
|-|-|-|-|-|
| x(32bit float) | Tone level | *Empty* | Blur level | *Empty* |
| y(32bit float) | Red channel | Green channel | Blue channel | Alpha channel |


#### Note: Unity 5.6+

In Unity 5.6+, Canvas supports **Additional Shader Channels**.  
Please enable `TexCoord1` to use UIEffect.  
![image](https://user-images.githubusercontent.com/12690315/28405830-f4f261e8-6d68-11e7-9faf-7e5442062f59.png)
![image](https://user-images.githubusercontent.com/12690315/34560894-191b6cda-f18b-11e7-9de2-9a9d13f72ccd.png)


#### Note: if you include prefabs / scenes containing UIEffect in AssetBundle.

Use script define symbol `UIEFFECT_SEPARATE`.  
Unused shader variants and materials will be excluded from AssetBundles.

||Combined mode (default)|Separated mode|
|-|-|-|
|Script define symbol| - |`UIEFFECT_SEPARATE`|
|Included in build|Only used variants|Only used variants|
|Included in AssetBundle|All variants (Heavy!)|Only used variants|
|Look in editor|![comb](https://user-images.githubusercontent.com/12690315/35324040-df4f1684-0132-11e8-9534-f958b93de158.png)|![sep](https://user-images.githubusercontent.com/12690315/35324405-fd5e89a6-0133-11e8-9d23-71ccc424fa21.png)|


#### How to improve performance?

* Use `ShaderVariantCollection` to preload shader.  
https://docs.unity3d.com/Manual/OptimizingShaderLoadTime.html
* Set camera's clear flag to "Solid Color".
* Enable multi thread rendering.




<br><br><br><br>
## License

* MIT
* © UTJ/UCL



## Author

[mob-sakai](https://github.com/mob-sakai)



## See Also

* GitHub page : https://github.com/mob-sakai/UIEffect
* Releases : https://github.com/mob-sakai/UIEffect/releases
* Issue tracker : https://github.com/mob-sakai/UIEffect/issues
* Current project : https://github.com/mob-sakai/UIEffect/projects/1
* Change log : https://github.com/mob-sakai/UIEffect/blob/master/CHANGELOG.md
