UIEffect
===

![image](https://user-images.githubusercontent.com/12690315/34617503-054e958e-f27f-11e7-89d5-9cced222883a.png)

Easy to use effects for uGUI. Supports following effects.

* Grayscale tone
* Sepia tone
* Nega tone
* Pixelation
* Cutoff (alpha-based)
* Mono (alpha-based)
* Color setting
* Color additive
* Color subtract
* Blur fast(3x4)
* Blur medium(6x4)
* Blur detail(6x8)


#### And, more features!

* Animation  
![1 -06-2018 01-21-15](https://user-images.githubusercontent.com/12690315/34617750-cf2c9464-f27f-11e7-8ee9-e6be3b209943.gif)
* Use for transition  
![image](https://user-images.githubusercontent.com/12690315/34618554-71a32e40-f282-11e7-8b78-6948c50c6b58.gif)
* Multiple shadow effect to reduce rendering vertices  
![image](https://user-images.githubusercontent.com/12690315/34552373-600fdab2-f164-11e7-8565-21c15af92a93.png)
* Capture image with effect, like post effect  
![image](https://user-images.githubusercontent.com/12690315/34619147-868cb36a-f284-11e7-8122-b924ff09077f.gif)
* Dialog with captured image effect.  
![image](https://user-images.githubusercontent.com/12690315/34619468-97e3c134-f285-11e7-90b2-3a75bde13911.gif)




## Requirement

* Unity 5.3+ *(included Unity 2017.x)*
* No other SDK are required




## Usage

1. Download [UIEffect.unitypackage](https://github.com/mob-sakai/UIEffect/raw/master/UIEffect.unitypackage) and install to your project.
1. Import the package into your Unity project. Select `Import Package > Custom Package` from the `Assets` menu.
1. Add `UIEffect` component to UI element (Image, RawImage, Text, etc...) from `Add Component` in inspector.
1. Choose effect type and adjust values in inspector.  
![image](https://user-images.githubusercontent.com/12690315/34595809-3838dc54-f21e-11e7-858b-72821dca8b44.png)
1. Enjoy!




## UIEffect for CapturedImage

![image](https://user-images.githubusercontent.com/12690315/34619147-868cb36a-f284-11e7-8122-b924ff09077f.gif)

`UIEffectCapturedImage` is like post effects, but it only provides effects on the rendered result (= captured image) of a frame.
This effect is non-realtime, light-weight, less-camera, blit only once, but effective.

* Camera for processing effect is unnecessary.
* Process effect only once after `UIEffectCapturedImage.Capture`.
* Using reduction buffer, keep used memory size small and keep rendering load are small.
* When GameObjects with motion are on the screen, a result texture may be stirred.
* You can overlay and display like as: [Screen] | [UIEffectCapturedImage] | [Dialog A] | [UIEffectCapturedImage] | [Dialog B]. See also demo.




## Note: Unity 5.6+

In Unity 5.6+, Canvas supports **Additional Shader Channels**.  
Please enable `TexCoord1` to use UIEffect.  
![image](https://user-images.githubusercontent.com/12690315/28405830-f4f261e8-6d68-11e7-9faf-7e5442062f59.png)
![image](https://user-images.githubusercontent.com/12690315/34560894-191b6cda-f18b-11e7-9de2-9a9d13f72ccd.png)




## Demo

[WebGL Demo](https://developer.cloud.unity3d.com/share/b18LwIciWX/webgl/)

* Effect sample
* Transition
* Dialog window with blur background
* Included in unitypackage




## Release Notes

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
| Materials in build | Instantiate on play, 0 kb | 23 items, about 5.0 kb |
| Total | 219.7 kb | 43.7 kb |


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
* Issue tracker : https://github.com/mob-sakai/UIEffect/issues