UIEffect
===

Composite Effect for uGUI.

![](http://mob-sakai.github.io/UIEffect/images/demo.gif)

![](http://mob-sakai.github.io/UIEffect/images/inspector.png)


## Requirement

* Unity 5.3+
* No other SDK are required.



## Usage

1. Download the package from [releases](https://github.com/mob-sakai/UIEffect/releases).
1. Import the package into your Unity project. Select `Import Package > Custom Package` from the `Assets` menu.
1. Add `UIEffect` component to UI element (Image, RawImage, Text, etc...) from `Add Component` in inspector.
1. Enjoy!



## Demo

[WebGL Demo](https://mob-sakai.github.io/UIEffect/demo/)



## Release Notes

### ver.1.0.0

* Feature: Shader supports pre-processer macros (for example UNITY_VERTEX_OUTPUT_STEREO) for Unity 5.3.x, 5.4.x and 5.5.x.
* Feature: Supports changing value from animation (for Unity 5.4+).
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