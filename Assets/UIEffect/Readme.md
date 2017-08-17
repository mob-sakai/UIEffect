UIEffect
===

Easy to use effects for uGUI. Supports following effects.

* Grayscale tone
* Sepia tone
* Nega tone
* Pixelation
* Cutoff (alpha-based)
* Mono tone (alpha-based)
* Color setting
* Color additive
* Color subtract
* Blur fast
* Blur detail

![demo](https://user-images.githubusercontent.com/12690315/29394440-3479d624-8345-11e7-8b1d-68d35e4a20de.gif)



## Requirement

* Unity 5.3+ *(included Unity 2017.x)*
* No other SDK are required



## Usage

1. Download [UIEffect.unitypackage](https://github.com/mob-sakai/UIEffect/raw/master/UIEffect.unitypackage) and install to your project.
1. Import the package into your Unity project. Select `Import Package > Custom Package` from the `Assets` menu.
1. Add `UIEffect` component to UI element (Image, RawImage, Text, etc...) from `Add Component` in inspector.
1. Choose effect type and adjust values in inspector.  
![image](https://user-images.githubusercontent.com/12690315/28405920-56fa5d96-6d69-11e7-9aaa-832cfecc72c6.png)
1. Enjoy!



## Note: Unity 5.6+

In Unity 5.6+, Canvas supports **Additional Shader Channels**.  
Please enable `TexCoord1` to use UIEffect.  
![image](https://user-images.githubusercontent.com/12690315/28405830-f4f261e8-6d68-11e7-9faf-7e5442062f59.png)



## Demo

[WebGL Demo](https://developer.cloud.unity3d.com/share/W1fv8sYS9f/)



## Release Notes

### ver.1.1.0

* Feature: Add Pixelaration effect.
* Feature: Add Cutoff/Mono effect for patterned alpha images. It can be used for masks and transitions!

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
MIT



## Author
[mob-sakai](https://github.com/mob-sakai)