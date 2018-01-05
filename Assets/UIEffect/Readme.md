UIEffect
===

Easy to use effects for uGUI. Supports following effects.

![image](https://user-images.githubusercontent.com/12690315/34595716-695a3bee-f21d-11e7-9a34-7a089ab7f6ea.gif)

* Grayscale tone
* Sepia tone
* Nega tone
* Pixelation
* Cutoff (alpha-based)
* Mono tone (alpha-based)
* Color setting
* Color additive
* Color subtract
* Blur fast(3x4)
* Blur medium(6x4)
* Blur detail(6x8)
* Use for transition  
![image](https://user-images.githubusercontent.com/12690315/34595717-6b00e8c6-f21d-11e7-98b6-798cf03f2ed5.gif)
* Supports multiple shadow effect for reduce rendering vertices  
![image](https://user-images.githubusercontent.com/12690315/34552373-600fdab2-f164-11e7-8565-21c15af92a93.png)




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




## Note: Unity 5.6+

In Unity 5.6+, Canvas supports **Additional Shader Channels**.  
Please enable `TexCoord1` to use UIEffect.  
![image](https://user-images.githubusercontent.com/12690315/28405830-f4f261e8-6d68-11e7-9faf-7e5442062f59.png)
![image](https://user-images.githubusercontent.com/12690315/34560894-191b6cda-f18b-11e7-9de2-9a9d13f72ccd.png)




## Demo

[WebGL Demo](https://developer.cloud.unity3d.com/share/b1Ow2w4KbX/webgl/)




## Release Notes

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
![image](https://user-images.githubusercontent.com/12690315/34595717-6b00e8c6-f21d-11e7-98b6-798cf03f2ed5.gif)


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