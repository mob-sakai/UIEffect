UIEffect
===

UIEffect provides visual effect components for Unity UI.

[![](https://img.shields.io/npm/v/com.coffee.ui-effect?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.coffee.ui-effect/)
[![](https://img.shields.io/github/v/release/mob-sakai/UIEffect?include_prereleases)](https://github.com/mob-sakai/UIEffect/releases)
[![](https://img.shields.io/github/release-date/mob-sakai/UIEffect.svg)](https://github.com/mob-sakai/UIEffect/releases)  
![](https://img.shields.io/badge/unity-2017.1%20or%20later-green.svg)
[![](https://img.shields.io/github/license/mob-sakai/UIEffect.svg)](https://github.com/mob-sakai/UIEffect/blob/master/LICENSE.txt)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-orange.svg)](http://makeapullrequest.com)
[![](https://img.shields.io/github/watchers/mob-sakai/UIEffect.svg?style=social&label=Watch)](https://github.com/mob-sakai/UIEffect/subscription)
[![](https://img.shields.io/twitter/follow/mob_sakai.svg?label=Follow&style=social)](https://twitter.com/intent/follow?screen_name=mob_sakai)

<< [Description](#Description) | [WebGL Demo](#demo) | [Installation](#installation) | [Usage](#usage) | [Example of using](#example-of-using) | [Change log](https://github.com/mob-sakai/UIEffect/blob/upm/CHANGELOG.md) | [Support](#support) >>



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
| -- | -- | -- |
| **UIEffect** | Combine some visual effects.<br><br>**Effect Mode:** Grayscale, Sepia, Nega, Pixelation<br>**Color Mode:** Multiply, Fill, Additive, Subtract<br>**Blur Mode:** Fast, Medium, Detail<br>**Advanced Blur:** Enable more beautiful blurring. | ![][eff1] |
| **UIShiny** | Apply shining effect to a graphic.<br>The effect does not require Mask component or normal map.<br><br>**Parameters:** Effect factor, Width, Rotation, Softness, Brightness, Gloss | ![][eff2] |
| **UIDissolve** | Apply dissolve effect to a graphic.<br><br>**Color Mode for edge:** Multiply, Fill, Additive, Subtract<br>**Parameters:** Effect factor, Width, Rotation, Softness, Edge color<br>**Options:** Effect area, Keep effect aspect ratio | ![][eff3] |
| **UIHsvModifier** | Modify HSV for graphic.<br><br>**Target:** Color, Range<br>**Adjustment:** Hue, Saturation, Value | ![][eff4] |
| **UITransition Effect** | Apply transition effect with a single channel texture.<br><br>**Effect Mode:** Cutoff, Fade, Dissolve<br>**Options:** Effect area, Keep effect aspect ratio, transition texture<br>**Pass Ray On Hidden:** Disable the graphic's raycastTarget on hidden. | ![][eff5] |

[eff1]:https://user-images.githubusercontent.com/12690315/46639603-258df180-cba2-11e8-8f50-9e93bdc4c96e.png
[eff2]:https://user-images.githubusercontent.com/12690315/46639689-b1078280-cba2-11e8-8716-cbc634af7293.gif
[eff3]:https://user-images.githubusercontent.com/12690315/46639690-b1078280-cba2-11e8-8aa9-1d2650fe9a62.gif
[eff4]:https://user-images.githubusercontent.com/12690315/43200006-d6e2bf54-904e-11e8-9f22-0c0f9ce5912f.gif
[eff5]:https://user-images.githubusercontent.com/12690315/46639688-b1078280-cba2-11e8-8bbb-16b8498bca5f.gif

<br><br>
##### The following effects can be used with the above components.

| Component | Features | Screenshot |
| -- | -- | -- |
| **UIShadow** | Add shadow/outline to a graphic.<br>The performance is better than the default Shadow/Outline component.<br><br>**ShadowStyle:** Shadow, Shadow3, Outline, Outline8 | ![][meff1] |
| **UIGradient** | Change vertex color as gradient with angle and offset.<br><br>**Direction:** Horizontal, Vertical, Angle, Diagonal<br>**Options:** Offset, Color space | ![][meff2] |
| **UIFlip** | Flip a graphic.<br><br>**Direction:** Horizontal, Vertical, Both | ![][meff3] |

[meff1]:https://user-images.githubusercontent.com/12690315/46639604-258df180-cba2-11e8-98a9-aa31f04c695d.png
[meff2]:https://user-images.githubusercontent.com/12690315/40716995-ca87665e-6445-11e8-8233-ec2e21fefd6b.png
[meff3]:https://user-images.githubusercontent.com/12690315/40716996-cab1fd7e-6445-11e8-9753-962d23991d86.png



<br><br><br><br>
## Demo

[WebGL Demo](http://mob-sakai.github.io/UIEffect)




<br><br><br><br>
## Installation

#### Requirement

* Unity 2017.1 or later
* No other SDK are required

#### Using OpenUPM (for Unity 2018.3 or later)

This package is available on [OpenUPM](https://openupm.com). 
You can install it via [openupm-cli](https://github.com/openupm/openupm-cli).
```
openupm add com.coffee.ui-effect
```

#### Using Git (for Unity 2018.3 or later)

Find the manifest.json file in the Packages folder of your project and edit it to look like this:
```js
{
 "dependencies": {
 "com.coffee.ui-effect": "https://github.com/mob-sakai/UIEffect.git",
 ...
 },
}
```

To update the package, change suffix `#{version}` to the target version.

* e.g. `"com.coffee.ui-effect": "https://github.com/mob-sakai/UIEffect.git#4.0.0",`

Or, use [UpmGitExtension](https://github.com/mob-sakai/UpmGitExtension) to install and update the package.

#### For Unity 2018.2 or earlier

1. Download a source code zip file from [Releases](https://github.com/mob-sakai/UIEffect/releases) page
2. Extract it
3. Import it into the following directory in your Unity project
   - `Packages` (It works as an embedded package. For Unity 2018.1 or later)
   - `Assets` (Legacy way. For Unity 2017.1 or later)



<br><br><br><br>
## How to play demo

- For Unity 2019.1 or later
  - Open `Package Manager` window and select `UI Effect` package in package list and click `Demo > Import in project` button
- For Unity 2018.4 or earlier
  - Click `Assets/Samples/UIEffect/Import Demo` from menu

The assets will be imported into `Assets/Samples/UI Effect/{version}/Demo`.  
Open `



<br><br><br><br>
## Usage

1. Add any effect component to UI element (Image, RawImage, Text, etc...) from `Add Component` in inspector or `Component > UI > UIEffect > ...` menu.  
![](https://user-images.githubusercontent.com/12690315/78853708-811c9200-7a5a-11ea-9826-0606046525b6.png)
2. Adjust the parameters of the effect as you like, in inspector.  
![](https://user-images.githubusercontent.com/12690315/38594668-636dd3ac-3d82-11e8-9951-820964a6a95f.gif)
3. You can add or modify effects from the script.  
```cs
var uieffect = gameObject.AddComponent<UIEffect>();
uieffect.effectMode = EffectMode.Grayscale;
uieffect.effectFactor = 0.85f;
uieffect.colorMode = ColorMode.Add;
uieffect.effectColor = Color.white;
uieffect.colorFactor = 0.1f;
uieffect.blurMode = BlurMode.FastBlur;
uieffect.blurFactor = 1;
```
![](https://user-images.githubusercontent.com/12690315/78853467-e4f28b00-7a59-11ea-82fa-3235aa95e993.png)

4. Enjoy!



<br><br><br><br>
## Example of using

UIEffect can easily be used in a variety of cases in the game.

| Case | Description | Screenshot |
| -- | -- | -- |
| Lock/unlock contents | Use UIEffect to apply grayscale.<br>Indicate to user that the content is unavailable. | ![][ex1] |
| Silhouette | Use UIEffect for filling color. | ![][ex2] |
| Soft shadow/<br>Outer glow | Use UIEffect and UIShadow to blur the shadow. | ![][ex3] |
| Colored shadow | Use UIEffect and UIShadow to fill shadow with color. | ![][ex4] |
| Blurred dynamic font | Use UIEffect to blur text.<br>To blur dynamic font cleanly, enable `Advanced Blur` option. | ![][ex5] |
| Text with outline & shadow | Use two UIShadows to add outline and shadow.<br>There is less overdraw than default Outline/Shadow. | ![][ex6] |
| Shining button | Use UIShiny for shining button.<br>Indicate to user that you can press the button. | ![][ex7] |
| Screen transition | Use UITransitionEffect to transition the screen with any transition texture. | ![][ex8] |

[ex1]:https://user-images.githubusercontent.com/12690315/46563469-aba8fe80-c93c-11e8-850f-949f6f8da742.png
[ex2]:https://user-images.githubusercontent.com/12690315/46563576-3db10700-c93d-11e8-960e-4336ff3ce481.png
[ex3]:https://user-images.githubusercontent.com/12690315/46566001-452edb00-c952-11e8-9cc4-6098a9eb67f3.png
[ex4]:https://user-images.githubusercontent.com/12690315/46566000-452edb00-c952-11e8-8d20-6ccc3fa92ae4.png
[ex5]:https://user-images.githubusercontent.com/12690315/46566002-45c77180-c952-11e8-87cb-4d915e0614be.png
[ex6]:https://user-images.githubusercontent.com/12690315/46566003-45c77180-c952-11e8-9b47-7bf563ffbaa7.png
[ex7]:https://user-images.githubusercontent.com/12690315/46563539-fb87c580-c93c-11e8-8c08-0f21872c47d4.gif
[ex8]:https://user-images.githubusercontent.com/12690315/46565182-dfd5ec80-c947-11e8-834f-a2ef67ad0d95.gif



<br><br><br><br>
## License

* MIT
* © UTJ/UCL



<br><br><br><br>
## Support

This is an open-source project that I am developing in my free time. 
If you like it, you can support me. 
By supporting, you let me spend more time working on better tools that you can use for free. :)

[![become_a_patron_on_patreon](https://user-images.githubusercontent.com/12690315/50731629-3b18b480-11ad-11e9-8fad-4b13f27969c1.png)](https://www.patreon.com/join/2343451?)  
[![become_a_sponsor_on_github](https://user-images.githubusercontent.com/12690315/66942881-03686280-f085-11e9-9586-fc0b6011029f.png)](https://github.com/users/mob-sakai/sponsorship)



## Author

[mob-sakai](https://github.com/mob-sakai)
[![](https://img.shields.io/twitter/follow/mob_sakai.svg?label=Follow&style=social)](https://twitter.com/intent/follow?screen_name=mob_sakai) 



## See Also

* GitHub page : https://github.com/mob-sakai/UIEffect
* Releases : https://github.com/mob-sakai/UIEffect/releases
* Issue tracker : https://github.com/mob-sakai/UIEffect/issues
* Change log : https://github.com/mob-sakai/UIEffect/blob/upm/CHANGELOG.md
