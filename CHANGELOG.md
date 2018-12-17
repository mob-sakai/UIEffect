# Changelog

## [v3.0.1](https://github.com/mob-sakai/UIEffect/tree/v3.0.1) (2018-11-07)

[Full Changelog](https://github.com/mob-sakai/UIEffect/compare/v3.0.0...v3.0.1)

**Fixed bugs:**

- Compile error in 2018.3 [\#139](https://github.com/mob-sakai/UIEffect/issues/139)

## [v3.0.0](https://github.com/mob-sakai/UIEffect/tree/v3.0.0) (2018-10-09)

[Full Changelog](https://github.com/mob-sakai/UIEffect/compare/v2.8.3...v3.0.0)

### New architecture: easier, faster and more beautiful.
* Simple & easy-to-use
* 20% faster
* High precision parameter

### Immediate capturing (UIEffectCapturedImage option)
* Capture the previous frame immediately without any camera.
* You no longer have to wait one frame to capture!
* *NOTE: LWRP, WebGL and Unity 5.x for iOS/Mac are not supported.*

### Advanced blur (UIEffect option)
* Remove common artifacts in the blur effect for uGUI.  
![](https://user-images.githubusercontent.com/12690315/42547121-80134788-84fb-11e8-97a0-048bba9634ea.png)
* It is effective for small padding size atlases, including dynamic fonts!

**Breaking changes:**

- UIEffectCapturedImage: Remove 'TargetTexture' feature [\#136](https://github.com/mob-sakai/UIEffect/issues/136)
- Remove 'additional shadow' in UIShadow component [\#110](https://github.com/mob-sakai/UIEffect/issues/110)
- Remove 'custom effect' feature in UIEffect component [\#98](https://github.com/mob-sakai/UIEffect/issues/98)
- Remove 'shadow effect' feature in UIEffect component [\#97](https://github.com/mob-sakai/UIEffect/issues/97)
- Remove 'hue effect' in UIEffect component [\#91](https://github.com/mob-sakai/UIEffect/issues/91)
- Remove 'cutoff' and 'mono' effect in UIEffect component [\#78](https://github.com/mob-sakai/UIEffect/issues/78)
- New architecture: Shared texture for effect parameter [\#63](https://github.com/mob-sakai/UIEffect/issues/63)
- Change: Change `ToneMode` to `EffectMode` [\#61](https://github.com/mob-sakai/UIEffect/issues/61)
- Separate shadow effect to other component [\#52](https://github.com/mob-sakai/UIEffect/issues/52)
- Use the graphic color as effect color, to reduce parameters [\#50](https://github.com/mob-sakai/UIEffect/issues/50)

**Implemented enhancements:**

- UITransitionEffect: "Pass ray on hidden" option [\#135](https://github.com/mob-sakai/UIEffect/issues/135)
- Add component menu in editor [\#133](https://github.com/mob-sakai/UIEffect/issues/133)
- UITransitionEffect: Add Show/Hide method [\#132](https://github.com/mob-sakai/UIEffect/issues/132)
- UIEffectCapturedImage: Immediate capturing [\#130](https://github.com/mob-sakai/UIEffect/issues/130)
- Improve blurring for atlas [\#95](https://github.com/mob-sakai/UIEffect/issues/95)
- Use Canvas.willRenderCanvases event instead of Update method [\#87](https://github.com/mob-sakai/UIEffect/issues/87)

**Closed issues:**

- Add tooltip [\#92](https://github.com/mob-sakai/UIEffect/issues/92)
- UIShiny: change parameter name `highlight` to `gloss` [\#93](https://github.com/mob-sakai/UIEffect/issues/93)

## [v2.8.3](https://github.com/mob-sakai/UIEffect/tree/v2.8.3) (2018-09-29)

[Full Changelog](https://github.com/mob-sakai/UIEffect/compare/v2.8.2...v2.8.3)

**Fixed bugs:**

- UIEffectCapturedImage: Black screen with Unity 2018.1+ editor on Windows  [\#131](https://github.com/mob-sakai/UIEffect/issues/131)

## [v2.8.2](https://github.com/mob-sakai/UIEffect/tree/v2.8.2) (2018-09-26)

[Full Changelog](https://github.com/mob-sakai/UIEffect/compare/v2.8.1...v2.8.2)

**Fixed bugs:**

- UICapturedEffectImage does not work with 'ScreenSpace - Overlay' in edit mode [\#128](https://github.com/mob-sakai/UIEffect/issues/128)
- The UIEffectCapturedImage is upside down with 'ScreenSpace - Overlay' mode [\#127](https://github.com/mob-sakai/UIEffect/issues/127)
- When "UI-Effect.mat" is created automatically, Unity hangs up. [\#126](https://github.com/mob-sakai/UIEffect/issues/126)
- UICapturedEffectImage does not work with Lightweight Render Pipeline LWRP [\#125](https://github.com/mob-sakai/UIEffect/issues/125)

## [v2.8.1](https://github.com/mob-sakai/UIEffect/tree/v2.8.1) (2018-08-17)

[Full Changelog](https://github.com/mob-sakai/UIEffect/compare/v2.8.0...v2.8.1)

**Fixed bugs:**

- \(Demo\) "Transition capture & dissolve" is incorrect [\#119](https://github.com/mob-sakai/UIEffect/issues/119)

## [v2.8.0](https://github.com/mob-sakai/UIEffect/tree/v2.8.0) (2018-08-14)

[Full Changelog](https://github.com/mob-sakai/UIEffect/compare/v2.7.1...v2.8.0)

* Create a screen transition!  
![8 -08-2018 19-29-38](https://user-images.githubusercontent.com/12690315/43832265-dbdecc98-9b41-11e8-8ab5-9f49420a6a16.gif)

* Some updates make UIEffectCapturedImage easier to use!


**Implemented enhancements:**

- UIEffectCapturedImage: Supports 'ScreenSpace - Overlay' [\#115](https://github.com/mob-sakai/UIEffect/issues/115)
- UIEffectCapturedImage: Keep aspect ratio [\#114](https://github.com/mob-sakai/UIEffect/issues/114)
- UIEffectCapturedImage: 'Capture on enable' option [\#113](https://github.com/mob-sakai/UIEffect/issues/113)
- UITransitionEffect: Change transition texture [\#111](https://github.com/mob-sakai/UIEffect/issues/111)

**Closed issues:**

- UIEffectCapturedImage: change parameter name `keepCanvasSize` to `fitToScreen` [\#116](https://github.com/mob-sakai/UIEffect/issues/116)

## [v2.7.1](https://github.com/mob-sakai/UIEffect/tree/v2.7.1) (2018-08-06)

[Full Changelog](https://github.com/mob-sakai/UIEffect/compare/v2.7.0...v2.7.1)

**Fixed bugs:**

- In v2.7.0, UIEffectCapturedImage is flipped vertically on Windows [\#112](https://github.com/mob-sakai/UIEffect/issues/112)

## [v2.7.0](https://github.com/mob-sakai/UIEffect/tree/v2.7.0) (2018-07-26)

[Full Changelog](https://github.com/mob-sakai/UIEffect/compare/v2.6.2...v2.7.0)

UIHsvModifier: Modify hue, saturation, and value as you like!  
![](https://user-images.githubusercontent.com/12690315/43200006-d6e2bf54-904e-11e8-9f22-0c0f9ce5912f.gif)  

* Note: `Hue` mode in UIEffect component will be obsolete in the near future. Please use UIHsvModifier component instead.
* Note: `Cutoff` and `Mono` mode in UIEffect component will be obsolete in the near future. Please use UITransitionEffect component instead.

**Implemented enhancements:**

- UIEffectCapturedImage: Support target RenderTexture to use external component [\#108](https://github.com/mob-sakai/UIEffect/issues/108)
- Transition effect as other component [\#105](https://github.com/mob-sakai/UIEffect/issues/105)
- Use multi-pass blurring to capture screenshot [\#96](https://github.com/mob-sakai/UIEffect/issues/96)
- Feature: HSV modifier [\#44](https://github.com/mob-sakai/UIEffect/issues/44)

**Fixed bugs:**

- UIEffectCapturedImage: ColorMode is not working [\#109](https://github.com/mob-sakai/UIEffect/issues/109)
- UIDissolve is not maskable [\#101](https://github.com/mob-sakai/UIEffect/issues/101)

## [v2.6.2](https://github.com/mob-sakai/UIEffect/tree/v2.6.2) (2018-07-18)

[Full Changelog](https://github.com/mob-sakai/UIEffect/compare/v2.6.1...v2.6.2)

**Fixed bugs:**

- Shader has compile errors in D3D9 or D3D11\_9X\(WSA\) [\#99](https://github.com/mob-sakai/UIEffect/issues/99)

## [v2.6.1](https://github.com/mob-sakai/UIEffect/tree/v2.6.1) (2018-06-14)

[Full Changelog](https://github.com/mob-sakai/UIEffect/compare/v2.6.0...v2.6.1)

**Fixed bugs:**

- Errors occurred on build \(v2.6.0\) [\#90](https://github.com/mob-sakai/UIEffect/issues/90)

## [v2.6.0](https://github.com/mob-sakai/UIEffect/tree/v2.6.0) (2018-06-14)

[Full Changelog](https://github.com/mob-sakai/UIEffect/compare/v2.5.2...v2.6.0)

Blurring effect has been improved about 6 times faster!  
![](https://user-images.githubusercontent.com/12690315/41393724-08420b1e-6fe2-11e8-8741-721789c2d029.png)

You can change noise texture for dissolve effect from inspector or script!  
![](https://user-images.githubusercontent.com/12690315/41397570-99bda636-6fef-11e8-827b-932d7a8e74c1.gif)




**Implemented enhancements:**

- Improve blurring performance [\#88](https://github.com/mob-sakai/UIEffect/issues/88)
- Separate the effect with a character [\#86](https://github.com/mob-sakai/UIEffect/issues/86)
- Change dissolve texture [\#75](https://github.com/mob-sakai/UIEffect/issues/75)

**Closed issues:**

- Change BlurMode correctly [\#84](https://github.com/mob-sakai/UIEffect/issues/84)
- Refactoring to prepare v3.0.0 [\#83](https://github.com/mob-sakai/UIEffect/issues/83)
- Change ColorMode correctly [\#51](https://github.com/mob-sakai/UIEffect/issues/51)

## [v2.5.2](https://github.com/mob-sakai/UIEffect/tree/v2.5.2) (2018-06-07)

[Full Changelog](https://github.com/mob-sakai/UIEffect/compare/v2.5.1...v2.5.2)

**Fixed bugs:**

- When `UIEFFECT\_SEPARATE` symbol is defined, UIDissolve does not work well [\#85](https://github.com/mob-sakai/UIEffect/issues/85)

## [v2.5.1](https://github.com/mob-sakai/UIEffect/tree/v2.5.1) (2018-05-31)

[Full Changelog](https://github.com/mob-sakai/UIEffect/compare/v2.5.0...v2.5.1)

**Fixed bugs:**

- Fix demo [\#82](https://github.com/mob-sakai/UIEffect/issues/82)

## [v2.5.0](https://github.com/mob-sakai/UIEffect/tree/v2.5.0) (2018-05-31)

[Full Changelog](https://github.com/mob-sakai/UIEffect/compare/v2.4.1...v2.5.0)

Shiny effect has been update!

![v2.5.0](https://user-images.githubusercontent.com/12690315/40654533-8877c99c-6379-11e8-8ae7-b91995fc230b.gif)

* NOTE: `UIEffect.shadow***` will be obsolete in the near future. Please use `UIShadow` component instead.
* NOTE: `UIEffect.custom***` will be obsolete in the near future. Please use `UICustomEffect` component (experimental) instead.

**Implemented enhancements:**

- UIDissolve: Play effect from script/inspector [\#81](https://github.com/mob-sakai/UIEffect/issues/81)
- UIShiny: Play effect from script/inspector [\#80](https://github.com/mob-sakai/UIEffect/issues/80)
- During play mode, you can change the effect type, color type, and blur type \(in Editor\) [\#73](https://github.com/mob-sakai/UIEffect/issues/73)
- Add shadow effect  as other component [\#72](https://github.com/mob-sakai/UIEffect/issues/72)
- UIShiny : Visual update [\#68](https://github.com/mob-sakai/UIEffect/issues/68)
- Add Custom effect as other component [\#60](https://github.com/mob-sakai/UIEffect/issues/60)

**Fixed bugs:**

- UIDissolve: When width=1 and location=0, image is lacked [\#79](https://github.com/mob-sakai/UIEffect/issues/79)

**Closed issues:**

- UIEffect inherit UIEffectBase  [\#74](https://github.com/mob-sakai/UIEffect/issues/74)
- Change directory structure [\#56](https://github.com/mob-sakai/UIEffect/issues/56)

## [v2.4.1](https://github.com/mob-sakai/UIEffect/tree/v2.4.1) (2018-05-29)

[Full Changelog](https://github.com/mob-sakai/UIEffect/compare/v2.4.0...v2.4.1)

**Fixed bugs:**

- UIEffectCapturedImage: The result image is flipped vertically [\#69](https://github.com/mob-sakai/UIEffect/issues/69)

## [v2.4.0](https://github.com/mob-sakai/UIEffect/tree/v2.4.0) (2018-05-21)

[Full Changelog](https://github.com/mob-sakai/UIEffect/compare/v2.3.2...v2.4.0)

UIDissolve has been updated!

![v2.4.0](https://user-images.githubusercontent.com/12690315/40294019-a0bfb8aa-5d0e-11e8-8451-873502db6a99.gif)
![editor](https://user-images.githubusercontent.com/12690315/40294212-9e1b1ce2-5d0f-11e8-88ce-78a8c0523dc2.png)

**Implemented enhancements:**

- UIDissolve: Add color mode option. [\#64](https://github.com/mob-sakai/UIEffect/issues/64)

## [v2.3.2](https://github.com/mob-sakai/UIEffect/tree/v2.3.2) (2018-05-21)

[Full Changelog](https://github.com/mob-sakai/UIEffect/compare/v2.3.1...v2.3.2)

**Closed issues:**

- Fixed: UIEffectCapturedImage: When iteration count is even. the result image is flipped vertically \(other method\) [\#65](https://github.com/mob-sakai/UIEffect/issues/65)

## [v2.3.1](https://github.com/mob-sakai/UIEffect/tree/v2.3.1) (2018-05-10)

[Full Changelog](https://github.com/mob-sakai/UIEffect/compare/v2.3.0...v2.3.1)

**Fixed bugs:**

- UIEffectCapturedImage: When iteration count is even. the result image is flipped vertically \(on Windows\) [\#62](https://github.com/mob-sakai/UIEffect/issues/62)

## [v2.3.0](https://github.com/mob-sakai/UIEffect/tree/v2.3.0) (2018-05-08)

[Full Changelog](https://github.com/mob-sakai/UIEffect/compare/v2.2.0...v2.3.0)

New components has been available!

* **UIShiny** : Shiny effect WITHOUT Mask component. This will suppress extra draw calls and improve performance.
* **UIDissolve** : Dissolve effect WITHOUT material instancing. This will suppress extra draw calls and improve performance.
* **UIFlip** : Flip graphic horizontal/vertical.

![v2.3.0](https://user-images.githubusercontent.com/12690315/40706142-cb98d2d0-6427-11e8-96fc-5cc5fd9c553a.gif)


**Implemented enhancements:**

- Feature: Flip horizontal/vertical [\#47](https://github.com/mob-sakai/UIEffect/issues/47)
- Feature: Dissolve [\#45](https://github.com/mob-sakai/UIEffect/issues/45)
- Feature: Shining effect [\#9](https://github.com/mob-sakai/UIEffect/issues/9)

## [v2.2.0](https://github.com/mob-sakai/UIEffect/tree/v2.2.0) (2018-04-12)

[Full Changelog](https://github.com/mob-sakai/UIEffect/compare/v2.1.0...v2.2.0)

**Implemented enhancements:**

- Feature: Customize shader/material. You can create a custom ui shader and control it with UIEffct. [\#46](https://github.com/mob-sakai/UIEffect/issues/46)
- Feature: UIEffectCapturedImage supports keep canvas size. [\#54](https://github.com/mob-sakai/UIEffect/issues/54)
- Feature: UIEffectCapturedImage supports `Quality Type` to easy setup. [\#53](https://github.com/mob-sakai/UIEffect/issues/53)

**Fixed bugs:**

- Bug: Color effect on shadow is incorrect. [\#55](https://github.com/mob-sakai/UIEffect/issues/55)

**Closed issues:**

- Change: Reduce the pixelation effect when tone level = 1. [\#57](https://github.com/mob-sakai/UIEffect/issues/57)

## [v2.1.0](https://github.com/mob-sakai/UIEffect/tree/v2.1.0) (2018-04-04)

[Full Changelog](https://github.com/mob-sakai/UIEffect/compare/v2.0.0...v2.1.0)

**Implemented enhancements:**

- Feature: UIEffectCapturedImage support iterative operation [\#48](https://github.com/mob-sakai/UIEffect/issues/48)

## [v2.0.0](https://github.com/mob-sakai/UIEffect/tree/v2.0.0) (2018-01-25)

[Full Changelog](https://github.com/mob-sakai/UIEffect/compare/v1.6.1...v2.0.0)

**Breaking changes:**

- Important: Unity 5.3.x & 5.4.x are no longer supported. [\#40](https://github.com/mob-sakai/UIEffect/issues/40)

**Implemented enhancements:**

- Improve: Reduce the materials. Too many effect materials are exist. [\#15](https://github.com/mob-sakai/UIEffect/issues/15)

**Closed issues:**

- Change: Change namespace to Coffee.UIExtensions [\#6](https://github.com/mob-sakai/UIEffect/issues/6)

## [v1.6.1](https://github.com/mob-sakai/UIEffect/tree/v1.6.1) (2018-01-25)

[Full Changelog](https://github.com/mob-sakai/UIEffect/compare/v1.6.0...v1.6.1)

**Fixed bugs:**

- Bug: Cannot access protected member `UnityEngine.UI.BaseMeshEffect.graphic` [\#41](https://github.com/mob-sakai/UIEffect/issues/41)

## [v1.6.0](https://github.com/mob-sakai/UIEffect/tree/v1.6.0) (2018-01-18)

[Full Changelog](https://github.com/mob-sakai/UIEffect/compare/v1.5.1...v1.6.0)

**Fixed bugs:**

- Bug: Pixelization is incorrect. [\#34](https://github.com/mob-sakai/UIEffect/issues/34)

**Closed issues:**

- Change: UIEffect inherit BaseMeshEffect. [\#35](https://github.com/mob-sakai/UIEffect/issues/35)
- Change:  Blur level range to \[0-1\] [\#32](https://github.com/mob-sakai/UIEffect/issues/32)
- Change: ShadowMode -\> ShadowStyle [\#18](https://github.com/mob-sakai/UIEffect/issues/18)

## [v1.5.1](https://github.com/mob-sakai/UIEffect/tree/v1.5.1) (2018-01-18)

[Full Changelog](https://github.com/mob-sakai/UIEffect/compare/v1.5.0...v1.5.1)

**Fixed bugs:**

- Bug: An error occurs when no effect is specified for UICapturedImage. [\#36](https://github.com/mob-sakai/UIEffect/issues/36)

## [v1.5.0](https://github.com/mob-sakai/UIEffect/tree/v1.5.0) (2018-01-16)

[Full Changelog](https://github.com/mob-sakai/UIEffect/compare/v1.4.4...v1.5.0)

**Implemented enhancements:**

- Feature: ShadowMode 'Shadow 3' [\#11](https://github.com/mob-sakai/UIEffect/issues/11)
- Feature: Hue [\#8](https://github.com/mob-sakai/UIEffect/issues/8)
- Feature: Gradient [\#7](https://github.com/mob-sakai/UIEffect/issues/7)

## [v1.4.4](https://github.com/mob-sakai/UIEffect/tree/v1.4.4) (2018-01-16)

[Full Changelog](https://github.com/mob-sakai/UIEffect/compare/v1.4.3...v1.4.4)

**Fixed bugs:**

- Bug: Error has occur on edit prefab. [\#27](https://github.com/mob-sakai/UIEffect/issues/27)

## [v1.4.3](https://github.com/mob-sakai/UIEffect/tree/v1.4.3) (2018-01-15)

[Full Changelog](https://github.com/mob-sakai/UIEffect/compare/v1.4.2...v1.4.3)

**Fixed bugs:**

- Bug: Color effect is incorrect. [\#19](https://github.com/mob-sakai/UIEffect/issues/19)

## [v1.4.2](https://github.com/mob-sakai/UIEffect/tree/v1.4.2) (2018-01-14)

[Full Changelog](https://github.com/mob-sakai/UIEffect/compare/v1.4.1...v1.4.2)

**Implemented enhancements:**

- Feature: ColorMode `Override` [\#12](https://github.com/mob-sakai/UIEffect/issues/12)

**Fixed bugs:**

- Bug: Error has occur OnAfterDeserialize in editor. [\#16](https://github.com/mob-sakai/UIEffect/issues/16)

## [v1.4.1](https://github.com/mob-sakai/UIEffect/tree/v1.4.1) (2018-01-10)

[Full Changelog](https://github.com/mob-sakai/UIEffect/compare/v1.4.0...v1.4.1)

## [v1.4.0](https://github.com/mob-sakai/UIEffect/tree/v1.4.0) (2018-01-07)

[Full Changelog](https://github.com/mob-sakai/UIEffect/compare/v1.3.0...v1.4.0)

**Implemented enhancements:**

- Feature: Exclude unused shader variants from build. [\#5](https://github.com/mob-sakai/UIEffect/issues/5)

## [v1.3.0](https://github.com/mob-sakai/UIEffect/tree/v1.3.0) (2018-01-06)

[Full Changelog](https://github.com/mob-sakai/UIEffect/compare/v1.2.0...v1.3.0)

## [v1.2.0](https://github.com/mob-sakai/UIEffect/tree/v1.2.0) (2018-01-05)

[Full Changelog](https://github.com/mob-sakai/UIEffect/compare/v1.1.0...v1.2.0)

**Implemented enhancements:**

- Feature: Fast multiple shadow effect. [\#2](https://github.com/mob-sakai/UIEffect/issues/2)

**Fixed bugs:**

- Pixelaration shifts to the lower right. [\#4](https://github.com/mob-sakai/UIEffect/issues/4)

## [v1.1.0](https://github.com/mob-sakai/UIEffect/tree/v1.1.0) (2017-08-17)

[Full Changelog](https://github.com/mob-sakai/UIEffect/compare/v1.0.0...v1.1.0)

**Implemented enhancements:**

- Feature: Pixelization effect [\#1](https://github.com/mob-sakai/UIEffect/issues/1)

## [v1.0.0](https://github.com/mob-sakai/UIEffect/tree/v1.0.0) (2017-03-01)

[Full Changelog](https://github.com/mob-sakai/UIEffect/compare/a9c4ec4e72a055ca5e5c24f6a75c6720f0f6fd7f...v1.0.0)



\* *This Changelog was automatically generated by [github_changelog_generator](https://github.com/skywinder/Github-Changelog-Generator)*