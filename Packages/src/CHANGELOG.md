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
