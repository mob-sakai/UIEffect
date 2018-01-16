UIEffect
===

![image](https://user-images.githubusercontent.com/12690315/34973050-c7f3b262-fac8-11e7-8feb-64b656190532.png)

Easy to use effects for uGUI. Supports following effects.

* Grayscale
* Sepia tone
* Nega tone
* Pixelation
* Hue shift
* Cutoff (alpha-based)
* Mono (alpha-based)
* Color override
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


#### And, more features!

* Control effect parameter with Animation  
![image](https://user-images.githubusercontent.com/12690315/34617750-cf2c9464-f27f-11e7-8ee9-e6be3b209943.gif)
* Use for screen transition  
![image](https://user-images.githubusercontent.com/12690315/34618554-71a32e40-f282-11e7-8b78-6948c50c6b58.gif)
* Multiple shadow effect to reduce rendering vertices  
![image](https://user-images.githubusercontent.com/12690315/34552373-600fdab2-f164-11e7-8565-21c15af92a93.png)
* Capture image with effect, like post effect  
![image](https://user-images.githubusercontent.com/12690315/34619147-868cb36a-f284-11e7-8122-b924ff09077f.gif)
* Dialog with captured image effect  
![image](https://user-images.githubusercontent.com/12690315/34619468-97e3c134-f285-11e7-90b2-3a75bde13911.gif)
* Change vertex color as gradient, with rotation, offset, color correction  
![image](https://user-images.githubusercontent.com/12690315/34931353-b1824df2-fa11-11e7-9dda-18aba10a8607.gif)
![image](https://user-images.githubusercontent.com/12690315/34930676-000c41d4-fa0e-11e7-825b-d5a1e757fea8.png)




## Requirement

* Unity 5.3+ *(included Unity 2017.x)*
* No other SDK are required




## Usage

1. Download UIEffect.unitypackage from [Releases](https://github.com/mob-sakai/UIEffect/releases) and install to your project.
1. Import the package into your Unity project. Select `Import Package > Custom Package` from the `Assets` menu.
1. Add `UIEffect` component to UI element (Image, RawImage, Text, etc...) from `Add Component` in inspector.
1. Choose effect type and adjust values in inspector.  
![image](https://user-images.githubusercontent.com/12690315/34595809-3838dc54-f21e-11e7-858b-72821dca8b44.png)
1. Enjoy!



## Development Note

### How does UIEffectCapturedImage work?

![image](https://user-images.githubusercontent.com/12690315/34619147-868cb36a-f284-11e7-8122-b924ff09077f.gif)

`UIEffectCapturedImage` is similar to post effect, but uses `CommandBuffer` to give effect only on the rendering result (= captured image) of a specific frame.
This effect is non-realtime, light-weight, less-camera, blit only once, but effective.

* Camera for processing effect is unnecessary.
* Process effect only once after `UIEffectCapturedImage.Capture`.
* Using reduction buffer, keep used memory size small and keep rendering load are small.
* When GameObjects with motion are on the screen, a result texture may be stirred.
* You can overlay and display like as: [Screen] | [UIEffectCapturedImage] | [Dialog A] | [UIEffectCapturedImage] | [Dialog B].  
See also [WebGL Demo](https://developer.cloud.unity3d.com/share/WyQetw3p-X/webgl/).




## Why is UIEffect lightweight?

* UIEffect pre-generates material from a combination of effects. This has the following benefits.
    * Draw call batching If possible, draw calls will decrease.
    * Since only the required material and shader variants are included in the build, the build size will be smaller.




## How to control effect parameters for uGUI element WITHOUT MaterialPropertyBlock?

* In general, you can use [MaterialPropertyBlock](https://docs.unity3d.com/ScriptReference/MaterialPropertyBlock.html) for renderers to control minor changes in the material without different batches.
* However, changing the [MaterialPropertyBlock](https://docs.unity3d.com/ScriptReference/MaterialPropertyBlock.html) of the uGUI element from the script will cause different batches and draw calls to increase.
* So UIEffect encodes multiple effect parameters to UV1 channel with [IMeshModifier](https://docs.unity3d.com/ScriptReference/UI.IMeshModifier.html).
* Up to four [0-1] parameters are packed in one float.
* 6 bit has 64 steps and 12 bit has 4096 steps.
* Effect parameters are low precision, but sufficient.

| uv1 | - | - | - | - |
|-|-|-|-|-|
| x(32bit float) | Tone[0-1] (6bit) | Empty (6bit) | Blur[0-1] (12bit) | - |
| y(32bit float) | Red[0-1] (6bit) | Green[0-1] (6bit) | Blue[0-1] (6bit) | Alpha[0-1] (6bit) |




## Note: Unity 5.6+

In Unity 5.6+, Canvas supports **Additional Shader Channels**.  
Please enable `TexCoord1` to use UIEffect.  
![image](https://user-images.githubusercontent.com/12690315/28405830-f4f261e8-6d68-11e7-9faf-7e5442062f59.png)
![image](https://user-images.githubusercontent.com/12690315/34560894-191b6cda-f18b-11e7-9de2-9a9d13f72ccd.png)




## Demo

[WebGL Demo](https://developer.cloud.unity3d.com/share/WJTY06hfG7/webgl/)

* Effect sample
* Transition
* Dialog window with blur background
* Included in unitypackage




## Release Notes

### ver.1.5.0

* Feature: Add ToneMode `Hue Shift`.  
![image](https://user-images.githubusercontent.com/12690315/34830409-18114e44-f727-11e7-849a-0d61a35f03ea.png)
* Feature: Add ShadowMode `Shadow3`.  
![image](https://user-images.githubusercontent.com/12690315/34930812-cb87dc24-fa0e-11e7-90a7-0013e2ba55d1.png)
* Feature: Add `UIGradient` component to change vertex color as gradient.
    * Gradient direction
        * Horizontal
        * Vertical
        * Angle
        * Diagonal
    * Rotation and offset
    * Text gradient style
        * Rect
        * Fit
        * Split
    * Color space
        * Uninitialized
        * Gammma
        * Linear
    * Ignore aspect ratio  
    ![image](https://user-images.githubusercontent.com/12690315/34930676-000c41d4-fa0e-11e7-825b-d5a1e757fea8.png)


### ver.1.4.4

* Fixed: Error has occur on edit prefab. [#27](https://github.com/mob-sakai/UIEffect/issues/27)


### ver.1.4.3

* Fixed: Color effect is incorrect. [#19](https://github.com/mob-sakai/UIEffect/issues/19)


### ver.1.4.2
 
* Fixed: Error has occur on after deserialize. [#16](https://github.com/mob-sakai/UIEffect/issues/16)


### ver.1.4.1

* Fixed: Demo link in `readme.md` is broken.


### ver.1.4.0

* Changed: ToneMode, ColorMode, BlurMode can be changed only in editor.
* Changed: Cutoff's level is reversed.
* Feature: Exclude unused shader variant from build.
* Add: Add many materials for shader variant, but exclude unused materials from build.  
Build report of demo project is as following.

|  | ver.1.3.0 | ver.1.4.0 |
|--|-----------|-----------|
| UI-Effect.shader | All 112 variants, 170.7 kb | 21 variants, 31.4 kb |
| UI-EffectCapturedImage.shader | All 80 variants, 49.0 kb | 2 variants, 7.3 kb |
| Materials in build | Instantiate on play, 0 kb | 23 items, about 6.9 kb |
| Total | 219.7 kb | 45.6 kb |


### ver.1.3.0

* Feature: Capture image with effect, like post effect.
* Demo: Add transition blur button & dialog with captured image effect.  
![image](https://user-images.githubusercontent.com/12690315/34619468-97e3c134-f285-11e7-90b2-3a75bde13911.gif)


### ver.1.2.0

* Fixed : Pixelaration shifts to the lower right.
* Fixed : Cutoff alpha is incorrect.
* Feature: Supports multiple shadow effect for reduce rendering vertices.  
![image](https://user-images.githubusercontent.com/12690315/34552373-600fdab2-f164-11e7-8565-21c15af92a93.png)
* Feature: New blur effect mode : `Medium`.
* Feature: Fix button to enable TexCoord1 of Canvas.additionalShaderChannels to use UIEffect.  
![image](https://user-images.githubusercontent.com/12690315/34560894-191b6cda-f18b-11e7-9de2-9a9d13f72ccd.png)
* Changed : UIEffect.color is obsolete, use UIEffect.effectColor instead.
* Changed: Blur range is [0-2].
* Refactoring: Shader refactoring.
* Demo: Add transition button & multiple shadows.


### ver.1.1.0

* Feature: Add Pixelaration effect.
* Feature: Add Cutoff/Mono effect for patterned alpha images. It can be used for masks and transitions!  
![image](https://user-images.githubusercontent.com/12690315/34618554-71a32e40-f282-11e7-8b78-6948c50c6b58.gif)


### ver.1.0.0

* Feature: Shader supports pre-processer macros (for example UNITY_VERTEX_OUTPUT_STEREO) for Unity 5.3.x, 5.4.x and 5.5.x.
* Feature: Supports changing value from animation.
* Feature: Supports Tone effect.
    * grayscale
    * sepia
    * nega
* Feature: Supports Color effect.
    * set
    * add
    * sub
* Feature: Supports Blur effect.
    * fast
    * detail
* Fix: Color effect will not be properly when it was rotated.
* Performance: Shader supports multi-compile for performance.
* Performance: Cache same effect materials to reduce draw calls.
* Improve: Supports mobile devices.
* Improve: Simplify component & inspector.




## License

* MIT
* © UTJ/UCL




## Author

[mob-sakai](https://github.com/mob-sakai)




## See Also

* GitHub Page : https://github.com/mob-sakai/UIEffect
* Releases : https://github.com/mob-sakai/UIEffect/releases
* Issue tracker : https://github.com/mob-sakai/UIEffect/issues
* Current Project : https://github.com/mob-sakai/UIEffect/projects/1