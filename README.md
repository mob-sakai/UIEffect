UIEffect
===

UIEffect contains visual effect components for uGUI element in Unity.

[![](https://img.shields.io/github/release/mob-sakai/UIEffect.svg?label=latest%20version)](https://github.com/mob-sakai/UIEffect/releases)
[![](https://img.shields.io/github/release-date/mob-sakai/UIEffect.svg)](https://github.com/mob-sakai/UIEffect/releases)
![](https://img.shields.io/badge/unity-5.5%2B%2C+2017.1%2B%2C+2018.1%2B-green.svg)
[![](https://img.shields.io/github/license/mob-sakai/UIEffect.svg)](https://github.com/mob-sakai/UIEffect/blob/master/LICENSE.txt)



<< [Description](#Description) | [WebGL Demo](#demo) | [Download](https://github.com/mob-sakai/UIEffect/releases) | [Usage](#usage) | [Development Note](#development-note) >>

### What's new? [See changelog ![](https://img.shields.io/github/release-date/mob-sakai/UIEffect.svg?label=last%20updated)](https://github.com/mob-sakai/UIEffect/blob/develop/CHANGELOG.md)
### Do you want to receive notifications for new releases? [Watch this repo ![](https://img.shields.io/github/watchers/mob-sakai/UIEffect.svg?style=social&label=Watch)](https://github.com/mob-sakai/UIEffect/subscription)



<br><br><br><br>
## Description

Let's decorate your UI with effects!  
You can control parameters as you like from the script as well as inspector.  
AnimationClip is supported as a matter of course!

![thumbnail](https://user-images.githubusercontent.com/12690315/41398364-155cf5a6-6ff2-11e8-8124-9d16ef6ca267.gif)
![image](https://user-images.githubusercontent.com/12690315/38594668-636dd3ac-3d82-11e8-9951-820964a6a95f.gif)

<br><br>
#### Available effects

| Component | Features | Screenshot |
|-|-|-|
|**UIEffect**|Combine some visual effects.<br><br>**ToneMode:** Grayscale, Sepia, Nega, Pixelation, Hue shift, Cutoff (alpha-based), Mono (alpha-based)<br>**ColorMode:** Multiply, Fill, Additive, Subtract<br>**BlurMode:** Fast, Medium, Detail<br><br>*NOTE: ToneMode, ColorMode and BlurMode are changeable only in editor.*<br>*NOTE: Custom Effect feature is deprecated. Please use `UICustomEffect` component instead.*<br>*NOTE: Shadow feature is deprecated. Please use `UIShadow` component instead.*<br>*NOTE: Hue is deprecated. Please use UIHsvModifier component instead.*<br>*NOTE: Cutoff and Mono is deprecated. Please use UITransitionEffect component instead.*|<img src="https://user-images.githubusercontent.com/12690315/40757428-68277f0c-64c3-11e8-89e5-4a19616a85c1.png" width="1000px">|
|**UICaptured EffectImage**|Capture a screenshot of a specific frame with effect, and display it.<br>This effect is non-realtime, light-weight, less-camera, but be effective enough.<br><br>**ToneMode:** Grayscale, Sepia, Nega, Pixelation<br>**ColorMode:** Multiply, Fill, Additive, Subtract<br>**BlurMode:** Fast, Medium, Detail<br>**QualityMode:** Fast, Medium, Detail, Custom<br>**Options:** Capture on enable, Blur iterations, Reduction rate, Target texture, Fit size to screen.<br><br>NOTE: Use this component in 'ScreenSpace - Camera' or  'ScreenSpace - Overlay' mode.|<img src="https://user-images.githubusercontent.com/12690315/44078752-4c2e4114-9fe2-11e8-97f0-54d3a36a562e.gif"  width="1000px">|
|**UIShiny**|Apply shinning effect to a graphic.<br>The effect does not require Mask component or normal map.<br><br>**Options:** Location, Width, Rotation, Softness, Brightness, Highlight|<img  src="https://user-images.githubusercontent.com/12690315/41398144-5c2ca126-6ff1-11e8-954b-394b611e2316.gif" width="1000px">|
|**UIDissolve**|Apply dissolve effect to a graphic.<br><br>**ColorMode:** Overwrite, Additive, Subtract<br>**Options:** Location, Width, Rotation, Softness, Color|<img src="https://user-images.githubusercontent.com/12690315/41397973-d1038f7e-6ff0-11e8-919f-affb427393ee.gif" width="1000px">|
|**UIHsvModifier**|Modify HSV for graphic.<br><br>**Target:** Color, Range<br>**Adjustment:** Hue, Saturation, Value|<img src="https://user-images.githubusercontent.com/12690315/43200006-d6e2bf54-904e-11e8-9f22-0c0f9ce5912f.gif"  width="1000px">|
|**UITransition Effect**|Apply transition effect with a single channel texture.<br><br>**EffectMode:** Cutoff, Fade|<img src="https://user-images.githubusercontent.com/12690315/44077521-f1b07c82-9fde-11e8-8487-2294c8d8f843.gif"  width="1000px">|

<br><br>
##### The following effects can be used with the above components.

| Component | Features | Screenshot |
|-|-|-|
|**UIShadow**|Add shadow/outline to a graphic.<br>Performance is better than the default Shadow/Outline component.<br>See also [Development note](#the-issue-of-default-outline-component).<br><br>**ShadowStyle:** Shadow, Shadow3, Outline, Outline8<br>**Additional Shadows** reduce vertices for multiple Shadows or Outlines.<br>*NOTE: AdditionalShadow is deprecated. Please use multiple `UIShadow` component instead.*|<img src="https://user-images.githubusercontent.com/12690315/40716994-ca5c13d2-6445-11e8-96d0-25a3ce704e2e.png" width="1000px">|
|**UIGradient**|Change vertex color as gradient with angle and offset.<br><br>**Direction:** Horizontal, Vertical, Angle, Diagonal<br>**Options:** Offset, Color space|<img src="https://user-images.githubusercontent.com/12690315/40716995-ca87665e-6445-11e8-8233-ec2e21fefd6b.png" width="1000px">|
|**UIFlip**|Flip a graphic.<br><br>**Direction:** Horizontal, Vertical, Both|<img src="https://user-images.githubusercontent.com/12690315/40716996-cab1fd7e-6445-11e8-9753-962d23991d86.png"  width="1000px">|




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
1. In Unity5.6+, enable `TexCoord1` channel of canvas. See also [Development Note](##note-unity-56).
1. Add any effect component to UI element (Image, RawImage, Text, etc...) from `Add Component` in inspector.
1. Enjoy!


##### Requirement

* Unity 5.5+ *(included Unity 2018.x)*
* No other SDK are required




<br><br><br><br>
## Development Note

#### How does UIEffectCapturedImage work?

![image](https://user-images.githubusercontent.com/12690315/34619147-868cb36a-f284-11e7-8122-b924ff09077f.gif)

`UIEffectCapturedImage` is similar to post effect, but uses `CommandBuffer` to give effect only on the rendering result (= captured image) of a specific frame.  
This effect is non-realtime, light-weight, less-camera, blit only once, but be effective enough.

* Camera for processing effect is unnecessary.
* Process effect only once after `UIEffectCapturedImage.Capture`.
* Using reduction buffer, keep using memory size small and keep the rendering load are small.
* When GameObjects with motion are on the screen, a result texture may be stirred.
* You can overlay and display like as:  
`[Screen] | [UIEffectCapturedImage] | [Dialog A] | [UIEffectCapturedImage] | [Dialog B]`.  
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
    * The parameters are lower precision, but sufficient.

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


#### The issue of default Outline component

Graphic with multiple outline components will generate a lot of overdraw.

![image](https://user-images.githubusercontent.com/12690315/34552373-600fdab2-f164-11e7-8565-21c15af92a93.png)

This is an overdraw view of image with three outline components.  
Because there are many overdraws, it is very bright!  
For each Outline component, increase the mesh by 5 times. (In the case of the Shadow component, it doubles the mesh.)  
In the image above, `1 x 5 x 5 x 5 = 125` overdraws are generated.

UIShadow's 'Addition Shadow' feature solves this issue by adding only the necessary mesh, `1 + 4 + 4 + 4 = 13` overdraws are generated.




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
