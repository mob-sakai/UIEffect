# [5.2.0](https://github.com/mob-sakai/UIEffect/compare/5.1.0...5.2.0) (2025-01-07)


### Bug Fixes

* `Gradation` properties do not update when a preset is loaded ([edf6943](https://github.com/mob-sakai/UIEffect/commit/edf694313f101b66f760bca64593fb87037dcb3d)), closes [#291](https://github.com/mob-sakai/UIEffect/issues/291)
* an error occurs when saving a preset ([894c561](https://github.com/mob-sakai/UIEffect/commit/894c5610d044f211067037b6c69f9860ac496ffc)), closes [#293](https://github.com/mob-sakai/UIEffect/issues/293)
* fix gradation gradient edge (angle gradient) ([0ef6027](https://github.com/mob-sakai/UIEffect/commit/0ef60273714c7b68131f2f0d233b16335a5cd367))


### Features

* `Angle Gradient` gradation supports fixed mode gradient ([7cb8325](https://github.com/mob-sakai/UIEffect/commit/7cb8325e445350d2175865f736186f719e1cbe00))
* `Angle Gradient` gradation supports RectTransform pivot ([2f63adf](https://github.com/mob-sakai/UIEffect/commit/2f63adfba057bec7f8183a5b34654cdf84163a34))
* add `Use HDR Color Picker` option for project settings (editor) ([1be0cd4](https://github.com/mob-sakai/UIEffect/commit/1be0cd45db952d05e51d508c58f65a6541df9de0)), closes [#290](https://github.com/mob-sakai/UIEffect/issues/290)
* the `Allow To Modify Mesh Shape` property should remain unchanged when loading a preset ([f7175c0](https://github.com/mob-sakai/UIEffect/commit/f7175c037371278eb7a0c7883cffccb5c31dba0a)), closes [#294](https://github.com/mob-sakai/UIEffect/issues/294)
* the `Sampling Scale` property should remain unchanged when loading a preset ([238a17d](https://github.com/mob-sakai/UIEffect/commit/238a17d57ddf1f6b859ce830e80a1be6c3d739f1)), closes [#292](https://github.com/mob-sakai/UIEffect/issues/292)

# [5.1.0](https://github.com/mob-sakai/UIEffect/compare/5.0.2...5.1.0) (2024-12-31)


### Bug Fixes

* changing the `shadowMirrorScale` property via code does not update the effect ([d9bf0e1](https://github.com/mob-sakai/UIEffect/commit/d9bf0e12fe918fc36120be3e1df2acbb00b1762b))
* fix reverse direction mode for UIEffectTweener ([54825ee](https://github.com/mob-sakai/UIEffect/commit/54825eedc8070e74fc276e1e08ea0073cf265189)), closes [#281](https://github.com/mob-sakai/UIEffect/issues/281) [#282](https://github.com/mob-sakai/UIEffect/issues/282) [#283](https://github.com/mob-sakai/UIEffect/issues/283)


### Features

* add `Color Glow`, `Transition Color Glow` and `Shadow Color Glow` options ([e9522d3](https://github.com/mob-sakai/UIEffect/commit/e9522d3f0e3bc0c429ef32f240625e903ca790ee))
* add `GradationMode.Angle` and `GradationMode.AngleGradient` ([08e9ba7](https://github.com/mob-sakai/UIEffect/commit/08e9ba73d78ba52502a6e795587441061377cd99))
* add `OnComplete` event for UIEffectTweener ([aeb78ed](https://github.com/mob-sakai/UIEffect/commit/aeb78ed577d70c47aacd754ea2aa3916e6e49c1b)), closes [#289](https://github.com/mob-sakai/UIEffect/issues/289) [#188](https://github.com/mob-sakai/UIEffect/issues/188)
* add `PlayOnEnable` option for UIEffectTweener ([1558736](https://github.com/mob-sakai/UIEffect/commit/155873669d77fc3281caaa0fc9f861e62aa489f2))
* add `SamplingScale` option ([2b43bc4](https://github.com/mob-sakai/UIEffect/commit/2b43bc42d4a9e0a1c224139ccf91f7a6f76656fc)), closes [#269](https://github.com/mob-sakai/UIEffect/issues/269)
* add `Shadow Color` and `Shadow Blur` options ([d0a3ca6](https://github.com/mob-sakai/UIEffect/commit/d0a3ca60904c0eebf63178a0fb899bb17ecefe75))
* add a button to swap gradation colors (editor) ([20a6ca0](https://github.com/mob-sakai/UIEffect/commit/20a6ca0815aaa899934fde318beecc1dde5e1132))
* add option to not automatically plays tweener effect ([88609f5](https://github.com/mob-sakai/UIEffect/commit/88609f5e368e38224a698270113b4f3190580a19))
* automatically display a dialog to import TextMeshPro support ([f2df188](https://github.com/mob-sakai/UIEffect/commit/f2df1889d152ae68926067978c60b4810fdef3f1))
* gradation feature ([bbe57df](https://github.com/mob-sakai/UIEffect/commit/bbe57df7a0a184793bd6d98a2b929055f69860b3)), closes [#277](https://github.com/mob-sakai/UIEffect/issues/277) [#66](https://github.com/mob-sakai/UIEffect/issues/66)
* on-demand UIEffect shader support ([bdd8e08](https://github.com/mob-sakai/UIEffect/commit/bdd8e0857235c56b7013fe768a333c5575aaa0ef)), closes [#212](https://github.com/mob-sakai/UIEffect/issues/212) [#271](https://github.com/mob-sakai/UIEffect/issues/271)
* support `TextMeshPro/Bitmap` and `TextMeshPro/Mobile/Bitmap` shaders. ([23ac398](https://github.com/mob-sakai/UIEffect/commit/23ac3985d2a2d9497b2d50b3755be671f3a07aa9)), closes [#284](https://github.com/mob-sakai/UIEffect/issues/284)
* support non full-rect graphics for some effect ([a66baea](https://github.com/mob-sakai/UIEffect/commit/a66baea1e8d895c424ba3dc1fce64f969d79561e))
* UIEffectTweener animation preview in edit mode ([e17b4a0](https://github.com/mob-sakai/UIEffect/commit/e17b4a0bfd262dc22ad2a13af5b0571b43a74abd)), closes [#279](https://github.com/mob-sakai/UIEffect/issues/279)

## [5.0.2](https://github.com/mob-sakai/UIEffect/compare/5.0.1...5.0.2) (2024-12-24)


### Bug Fixes

* incorrect transparency when applying the shiny filter to TextMeshProUGUI ([f74279f](https://github.com/mob-sakai/UIEffect/commit/f74279fb8353226b494f00582ed5eb7fe87d147f)), closes [#287](https://github.com/mob-sakai/UIEffect/issues/287)
* TextMeshProUGUI disappears when the Y-axis scale is changed ([9d98c5d](https://github.com/mob-sakai/UIEffect/commit/9d98c5d3e1c6ead8e3312a7e3a36fe89b38aed68)), closes [#286](https://github.com/mob-sakai/UIEffect/issues/286)

## [5.0.1](https://github.com/mob-sakai/UIEffect/compare/5.0.0...5.0.1) (2024-12-14)


### Bug Fixes

* build AssetBundle error ([7374747](https://github.com/mob-sakai/UIEffect/commit/73747478a4e5754891ebd4fd2d54955e82a72eb3))
* fix cutoff transition effect ([c893cb0](https://github.com/mob-sakai/UIEffect/commit/c893cb0ce2e90bf13456adbc7d2e0e0642cbac1d))
* fix TextMeshPro shader (underlay and bevel) ([f39097a](https://github.com/mob-sakai/UIEffect/commit/f39097a73fc8560a50f848290ce4c5a235b68cac))
* TextMeshPro objects appeared as black blocks when saving prefabs in prefab mode ([5889486](https://github.com/mob-sakai/UIEffect/commit/5889486f18afde6dec203bbe4aaca45760a9dce1)), closes [#285](https://github.com/mob-sakai/UIEffect/issues/285)

# [5.0.0](https://github.com/mob-sakai/UIEffect/compare/4.0.0...5.0.0) (2024-11-11)


### Features

* add built-in UIEffect preset ([4adb58c](https://github.com/mob-sakai/UIEffect/commit/4adb58ca61d75897e67ef8674f9be9b9d6156b24))
* add support for TextMeshPro, including `<font>` and `<sprite>` tags ([a4f85cb](https://github.com/mob-sakai/UIEffect/commit/a4f85cb0ee58a60e20572fcce0ebf204effce51e))
* add UIEffectProjectSettings ([1306e19](https://github.com/mob-sakai/UIEffect/commit/1306e19cab9aeb9ec86a8047d59c2a9ad11bb128))
* add UIEffectReplica component ([3bdc61c](https://github.com/mob-sakai/UIEffect/commit/3bdc61c2efe51e198650171e7a235276b82cb1f0))
* add UIEffectTweener component ([e2c1605](https://github.com/mob-sakai/UIEffect/commit/e2c1605c1986c34f23c85dc9ec22f00f16bca21d))
* add v4 compatible components ([d40a019](https://github.com/mob-sakai/UIEffect/commit/d40a0198c439b47c5b87f554df7eeb6ad5741fcd))
* completely redesigned UIEffect architecture ([4e069a4](https://github.com/mob-sakai/UIEffect/commit/4e069a45d9817095f727f3bd18d793683fb052c2))


### BREAKING CHANGES

* All v4 components are obsolete. See README to upgrade.
