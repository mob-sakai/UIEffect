## [5.10.3](https://github.com/mob-sakai/UIEffect/compare/5.10.2...5.10.3) (2025-10-22)


### Bug Fixes

* displays as black rectangle when opening editor ([4a8d341](https://github.com/mob-sakai/UIEffect/commit/4a8d341625f73d70c512f7e6f0b864be1adbf459)), closes [#376](https://github.com/mob-sakai/UIEffect/issues/376)

## [5.10.2](https://github.com/mob-sakai/UIEffect/compare/5.10.1...5.10.2) (2025-10-16)


### Bug Fixes

* [improve performance] code optimization ([e6fb78a](https://github.com/mob-sakai/UIEffect/commit/e6fb78a0f67ed6f70be08259f674ab2f83364bd7))

## [5.10.1](https://github.com/mob-sakai/UIEffect/compare/5.10.0...5.10.1) (2025-10-06)


### Bug Fixes

* [improve performance] `UpdateViewMatrix` is called even though (direct-)parent canvas is disabled ([533d7b8](https://github.com/mob-sakai/UIEffect/commit/533d7b8c0a33871b1849fbda862d4fa1899822f4)), closes [#373](https://github.com/mob-sakai/UIEffect/issues/373)

# [5.10.0](https://github.com/mob-sakai/UIEffect/compare/5.9.7...5.10.0) (2025-09-26)


### Bug Fixes

* `TmpProxy` causes loss of modifications from other `IMeshModifiers` ([c4202b4](https://github.com/mob-sakai/UIEffect/commit/c4202b45d28fa84567a5671baaef8fa3e3df85dc)), closes [#371](https://github.com/mob-sakai/UIEffect/issues/371)
* display may disappear when using TMP SubMesh (multi-language or sprites, etc.) ([b4adb32](https://github.com/mob-sakai/UIEffect/commit/b4adb325120eeed64af1d9cb2468a1e1e8e10812)), closes [#372](https://github.com/mob-sakai/UIEffect/issues/372)
* TMP Text sprite submesh lost when changing fontMaterial ([59874dd](https://github.com/mob-sakai/UIEffect/commit/59874ddffe356f927dc8ff9b157c54043e149522)), closes [#368](https://github.com/mob-sakai/UIEffect/issues/368)


### Features

* [improve performance] merged apply_color function and reduced shader compilation size ([ce69258](https://github.com/mob-sakai/UIEffect/commit/ce692580a33c0c433fd7fadb64a3dcda0121b2ff))
* [improve performance] various view matrices are only updated when necessary ([6c808aa](https://github.com/mob-sakai/UIEffect/commit/6c808aa3417450fd1ce0fc6fbfe0adffdff9a79a))

## [5.9.7](https://github.com/mob-sakai/UIEffect/compare/5.9.6...5.9.7) (2025-09-01)


### Bug Fixes

* edges of `TransitionFilter.Shiny` appear sharp ([b077d97](https://github.com/mob-sakai/UIEffect/commit/b077d97ed706acbbdd3b20e765ec0552fa464f9b)), closes [#366](https://github.com/mob-sakai/UIEffect/issues/366)
* error message sometimes after importing assets ([3207105](https://github.com/mob-sakai/UIEffect/commit/3207105a067801696de789d282c74a93b798ed57)), closes [#365](https://github.com/mob-sakai/UIEffect/issues/365)
* flickering issue when scale Y changes when used `ShadowMode` with TextMeshPro ([4cca2fb](https://github.com/mob-sakai/UIEffect/commit/4cca2fb5ab34ea454ff8df3def5f1730a71a198c)), closes [#367](https://github.com/mob-sakai/UIEffect/issues/367)
* skip monitoring TMP objects that can ignore Y-axis scaling ([d1adb33](https://github.com/mob-sakai/UIEffect/commit/d1adb33884971aa7d6c473fcd08cdf25e58cd261))

## [5.9.6](https://github.com/mob-sakai/UIEffect/compare/5.9.5...5.9.6) (2025-07-17)


### Bug Fixes

* `ColorFilter` does not output intended color when vertex color is not white ([9dffdb0](https://github.com/mob-sakai/UIEffect/commit/9dffdb0ed88750a915fdd5e437b79ab766ce45c2)), closes [#363](https://github.com/mob-sakai/UIEffect/issues/363)
* draw calls increase even when using `UIEffectReplica` ([83128eb](https://github.com/mob-sakai/UIEffect/commit/83128ebb09305e4c15a58968a94550179e96ffac)), closes [#364](https://github.com/mob-sakai/UIEffect/issues/364)

## [5.9.5](https://github.com/mob-sakai/UIEffect/compare/5.9.4...5.9.5) (2025-06-26)


### Bug Fixes

* `GetGradientKeys` and `GetTransitionGradientKeys` cannot get GradientMode ([fd2d1b8](https://github.com/mob-sakai/UIEffect/commit/fd2d1b80592ac4ea62e15b6150be63964b113fa2)), closes [#358](https://github.com/mob-sakai/UIEffect/issues/358)

## [5.9.4](https://github.com/mob-sakai/UIEffect/compare/5.9.3...5.9.4) (2025-06-25)


### Bug Fixes

* colors specified in UIEffect are output differently in linear color space ([0d7fb1e](https://github.com/mob-sakai/UIEffect/commit/0d7fb1eb63881b67f2658567750a83c92187df35)), closes [#359](https://github.com/mob-sakai/UIEffect/issues/359)

## [5.9.3](https://github.com/mob-sakai/UIEffect/compare/5.9.2...5.9.3) (2025-06-23)


### Bug Fixes

* add `GetGradientKeys` and `GetTransitionGradientKeys` APIs ([a0c3e49](https://github.com/mob-sakai/UIEffect/commit/a0c3e4997af181ae16744c53ad3867e2106ccff3)), closes [#358](https://github.com/mob-sakai/UIEffect/issues/358)
* support world canvas without UI events ([cadbb8d](https://github.com/mob-sakai/UIEffect/commit/cadbb8d68d17a2ec7e2d6e8e885d711e61fa53af)), closes [#356](https://github.com/mob-sakai/UIEffect/issues/356) [#357](https://github.com/mob-sakai/UIEffect/issues/357)

## [5.9.2](https://github.com/mob-sakai/UIEffect/compare/5.9.1...5.9.2) (2025-06-12)


### Bug Fixes

* TextMeshPro 3.2.0-pre support ([804b48c](https://github.com/mob-sakai/UIEffect/commit/804b48c73ec648eaceb720cb79551fdbf3729fc9)), closes [#355](https://github.com/mob-sakai/UIEffect/issues/355)

## [5.9.1](https://github.com/mob-sakai/UIEffect/compare/5.9.0...5.9.1) (2025-06-11)


### Bug Fixes

* prefabs will be deselected when editing them ([8addbad](https://github.com/mob-sakai/UIEffect/commit/8addbad4941340807d824e295280473d197d889f)), closes [#354](https://github.com/mob-sakai/UIEffect/issues/354)

# [5.9.0](https://github.com/mob-sakai/UIEffect/compare/5.8.7...5.9.0) (2025-06-09)


### Bug Fixes

* `GradationMode.RadialFast` and `GradationMode.RadialDetail` are obsolete ([1a923dd](https://github.com/mob-sakai/UIEffect/commit/1a923dd72c263de5480d71bd17b697feca317ef1))
* soft masking not working with `DetailFilter.Masking` ([ffdeaa7](https://github.com/mob-sakai/UIEffect/commit/ffdeaa7e2f1c2a9ccb7eb21d5674e2c19021aef9))


### Features

* `GradationGradient` supports HDR color ([50f6b3c](https://github.com/mob-sakai/UIEffect/commit/50f6b3c5d20b93a2245f6d54b6eb4bc13565c3b5))
* add `Clear` method to reset properties via script ([264c453](https://github.com/mob-sakai/UIEffect/commit/264c453bb6ebad08ef950b0cec2f38e60a1710d8)), closes [#337](https://github.com/mob-sakai/UIEffect/issues/337)
* add `GradationMode.RadialGradient` feature ([179ec89](https://github.com/mob-sakai/UIEffect/commit/179ec89a0215af9d8046e150d98fa06371155e93)), closes [#348](https://github.com/mob-sakai/UIEffect/issues/348)
* add `GradationReverse` option for gradient ([c0fec32](https://github.com/mob-sakai/UIEffect/commit/c0fec32d502ec1f9d03172ed200f4d9233506eb9)), closes [#348](https://github.com/mob-sakai/UIEffect/issues/348)
* add `GradationWrapMode` option ([f98929d](https://github.com/mob-sakai/UIEffect/commit/f98929d7e9656b4c02d5545b3893748c963d1118))
* add `TransitionFilter.Blaze` feature ([ed4716d](https://github.com/mob-sakai/UIEffect/commit/ed4716d7ada102fe0053480941925e9f27b1c2c3))
* add new built-in presets ([4f2c9f1](https://github.com/mob-sakai/UIEffect/commit/4f2c9f12b5e115bab1418a7a0b1187269a041fc4))
* improve performance for TextMeshPro ([6af520e](https://github.com/mob-sakai/UIEffect/commit/6af520ed408f2e5eb4d84ec0df484338719ebc8e))
* timeline package support ([742a5cd](https://github.com/mob-sakai/UIEffect/commit/742a5cd8dc47500de4b111a132d7a8b242844342)), closes [#347](https://github.com/mob-sakai/UIEffect/issues/347)
* update shaders in samples ([4c6cd11](https://github.com/mob-sakai/UIEffect/commit/4c6cd11d86582f2351c40374180828ad1c8955c3))

## [5.8.7](https://github.com/mob-sakai/UIEffect/compare/5.8.6...5.8.7) (2025-06-09)


### Bug Fixes

* `GradationGradient` and `TransitionGradient` in reprica will be not dirty (editor) ([58b197e](https://github.com/mob-sakai/UIEffect/commit/58b197e551aea8efca10e12da8df132853d4a24e))
* changes not reflected in rendering when TextMeshPro is cleared ([f508eb3](https://github.com/mob-sakai/UIEffect/commit/f508eb32caf6757eb4c27b98ac29a1582dcd14b4)), closes [#351](https://github.com/mob-sakai/UIEffect/issues/351)
* setting `gradationScale` to 0 throws `DivideByZeroException` ([ecd265e](https://github.com/mob-sakai/UIEffect/commit/ecd265e4828c7f81c96a76c16df147b6c4804429))

## [5.8.6](https://github.com/mob-sakai/UIEffect/compare/5.8.5...5.8.6) (2025-05-22)


### Bug Fixes

* some methods may be called multiple times when domain reload on play is disabled (editor) ([3b52c3a](https://github.com/mob-sakai/UIEffect/commit/3b52c3a8fc67980d426b8e4e2ed2067c725ca262))

## [5.8.5](https://github.com/mob-sakai/UIEffect/compare/5.8.4...5.8.5) (2025-05-22)


### Bug Fixes

* skip processing when graphic is invalid ([5cd48a4](https://github.com/mob-sakai/UIEffect/commit/5cd48a463309270ec3d6501bde2add9944bbaf02))
* TextMeshPro strikethrough and underline not displaying ([68ebae2](https://github.com/mob-sakai/UIEffect/commit/68ebae28e02045cb45996bf5b77d7d93d2f0ca82)), closes [#344](https://github.com/mob-sakai/UIEffect/issues/344)
* TMP SubMeshUI generates overhead every frame (since 5.6.0) ([791d867](https://github.com/mob-sakai/UIEffect/commit/791d8670761902466d81412d832cddf013cd28cd)), closes [#343](https://github.com/mob-sakai/UIEffect/issues/343) [#345](https://github.com/mob-sakai/UIEffect/issues/345)

## [5.8.4](https://github.com/mob-sakai/UIEffect/compare/5.8.3...5.8.4) (2025-04-30)


### Bug Fixes

* NRE while canvas is disabled ([a3773e3](https://github.com/mob-sakai/UIEffect/commit/a3773e33028c60517fb4e6c534f6280194df8e64)), closes [#342](https://github.com/mob-sakai/UIEffect/issues/342)

## [5.8.3](https://github.com/mob-sakai/UIEffect/compare/5.8.2...5.8.3) (2025-04-30)


### Bug Fixes

* the alpha of the gradation color and gradient was being ignored ([3807e4c](https://github.com/mob-sakai/UIEffect/commit/3807e4c69aefaef04b2245522e7c5085ccc65a36)), closes [#341](https://github.com/mob-sakai/UIEffect/issues/341)

## [5.8.2](https://github.com/mob-sakai/UIEffect/compare/5.8.1...5.8.2) (2025-04-23)


### Bug Fixes

* `Flip` is ignored when loading a preset ([e1f3abf](https://github.com/mob-sakai/UIEffect/commit/e1f3abf7a65b62718b28e33ac2f7d700c3ae3f5a))
* fix detail color ([90ab16c](https://github.com/mob-sakai/UIEffect/commit/90ab16c3fd46435ebf7c8e47ffb4ec521d2f61c9))
* indicate in the inspector using tooltips and asterisks that `Rotation` and `Keep Aspect Ratio` are shared properties ([2cca822](https://github.com/mob-sakai/UIEffect/commit/2cca822413aa7580e1eb777c3295d1de928cd65f))

## [5.8.1](https://github.com/mob-sakai/UIEffect/compare/5.8.0...5.8.1) (2025-04-17)


### Bug Fixes

* built-in presets cannot be used with `UIEffectReplica` ([1106f4b](https://github.com/mob-sakai/UIEffect/commit/1106f4bf4632e1372d44ced9a38e98209a6b55e8))
* New presets ignore `DetailColor` property ([1143fbb](https://github.com/mob-sakai/UIEffect/commit/1143fbb84a1e08817de93f1349ee5cdad0972db5)), closes [#336](https://github.com/mob-sakai/UIEffect/issues/336)
* prevent built-in preset assets from being editable ([0898d73](https://github.com/mob-sakai/UIEffect/commit/0898d730add42970fb74bf2ad8aca9809bf6dd39))

# [5.8.0](https://github.com/mob-sakai/UIEffect/compare/5.7.0...5.8.0) (2025-04-17)


### Bug Fixes

* `ShadowMode.Mirror` does not flip the effect ([37daad0](https://github.com/mob-sakai/UIEffect/commit/37daad01c4f3f4c259650b1493aeae204e0bd247))
* fix alpha clipping for ShaderGraph ([ae177a8](https://github.com/mob-sakai/UIEffect/commit/ae177a800cba1561b523292279d7986d4eff9f9c))
* unable to add editor presets as runtime presets to project settings ([726bb10](https://github.com/mob-sakai/UIEffect/commit/726bb100a0ad327619634ea73ccdccc36276d0d0)), closes [#330](https://github.com/mob-sakai/UIEffect/issues/330)


### Features

* add `DetailColor` property ([30bf783](https://github.com/mob-sakai/UIEffect/commit/30bf783320398d68b90b09b0dcf93738b4683e00)), closes [#332](https://github.com/mob-sakai/UIEffect/issues/332)
* ScriptableObject based preset system ([f8682f3](https://github.com/mob-sakai/UIEffect/commit/f8682f3c9889fb56ff3cd9010934f998e121553a)), closes [#333](https://github.com/mob-sakai/UIEffect/issues/333)

# [5.7.0](https://github.com/mob-sakai/UIEffect/compare/5.6.4...5.7.0) (2025-04-11)


### Bug Fixes

* colors appeared darker when alpha was low ([21172f3](https://github.com/mob-sakai/UIEffect/commit/21172f37c21792906ac9e33827c771d388397653))
* shrink shader variants for color filters ([7f60a56](https://github.com/mob-sakai/UIEffect/commit/7f60a56a9af6b4ecee223b466f1b2499af0194f4))


### Features

* `UIEffectReplica.flip` has been deprecated ([6af34e2](https://github.com/mob-sakai/UIEffect/commit/6af34e2994378fd33025b41fe08898b552aa08ab))
* add `GradationIntensity` and `GradationColorFilter` properties ([a6781b9](https://github.com/mob-sakai/UIEffect/commit/a6781b92899c38f49eccfebf4173028955a5aced))
* calculate the normalized position on the shader side ([01bbeed](https://github.com/mob-sakai/UIEffect/commit/01bbeedde7cf2d757d6d295475f0ad9acd24dce3)), closes [#323](https://github.com/mob-sakai/UIEffect/issues/323) [#324](https://github.com/mob-sakai/UIEffect/issues/324)
* ShaderGraph support ([2dd2e26](https://github.com/mob-sakai/UIEffect/commit/2dd2e261666b611be3a1fd5ac112db34fd99038e)), closes [#326](https://github.com/mob-sakai/UIEffect/issues/326)

## [5.6.4](https://github.com/mob-sakai/UIEffect/compare/5.6.3...5.6.4) (2025-04-10)


### Bug Fixes

* avoid potential allocations ([26da24d](https://github.com/mob-sakai/UIEffect/commit/26da24dbc4022dcd71e7a58e5f02a6922ba2ddff))
* optional shaders do not support all shader combinations ([6014ab5](https://github.com/mob-sakai/UIEffect/commit/6014ab54d010872f8fee63fc961ac5ab2edd08e1))

## [5.6.3](https://github.com/mob-sakai/UIEffect/compare/5.6.2...5.6.3) (2025-04-06)


### Bug Fixes

* fix a potential `ArgumentOutOfRangeException` that could occur when changing the FontAsset in TextMeshPro ([3b095bc](https://github.com/mob-sakai/UIEffect/commit/3b095bcebd8b1264c373b8b71c27d3c35f673f3b)), closes [#327](https://github.com/mob-sakai/UIEffect/issues/327)

## [5.6.2](https://github.com/mob-sakai/UIEffect/compare/5.6.1...5.6.2) (2025-04-04)


### Bug Fixes

* fix a potential `ArgumentOutOfRangeException` that could occur when changing the FontAsset in TextMeshPro ([909281c](https://github.com/mob-sakai/UIEffect/commit/909281c66b702d8497e9f847c91b688716b9c6a7)), closes [#327](https://github.com/mob-sakai/UIEffect/issues/327)
* Remove unnecessary vertex expansion ([974e3e8](https://github.com/mob-sakai/UIEffect/commit/974e3e8769d456c5cdd8eb1767a7cf57ab4d52fc))
* undo now works correctly after clicking the 'Clear' button in the inspector ([75ed355](https://github.com/mob-sakai/UIEffect/commit/75ed355e27faf8962390eb6788e1887ffd1b5f89))

## [5.6.1](https://github.com/mob-sakai/UIEffect/compare/5.6.0...5.6.1) (2025-03-28)


### Bug Fixes

* changes were not correctly marked as dirty when loading presets ([76be0fc](https://github.com/mob-sakai/UIEffect/commit/76be0fc860c27b89235b7fb9a766dcc38b3d9924))

# [5.6.0](https://github.com/mob-sakai/UIEffect/compare/5.5.3...5.6.0) (2025-03-28)


### Bug Fixes

* fix edge detection ([9462170](https://github.com/mob-sakai/UIEffect/commit/946217069f10714798070481d8576f5470e243db))
* fix project settings icon ([202601c](https://github.com/mob-sakai/UIEffect/commit/202601c96d6fb564ee5d6b8868b0ff0a8dba0ba1))
* the strength of ColorGlow, TransitionColorGlow, and ShadowColorGlow now changes according to intensity ([aa4ec8d](https://github.com/mob-sakai/UIEffect/commit/aa4ec8d9c449bfe361aa277e25cbb389e0daa983)), closes [#316](https://github.com/mob-sakai/UIEffect/issues/316)
* UIEffect with TextMeshPro SubMeshUI appears black in the editor when saving assets ([de91871](https://github.com/mob-sakai/UIEffect/commit/de918718d062979c8e9b4202b6b2ae389ae10bab))


### Features

* `TransitionAutoPlaySpeed`, `TransitionTexSpeed`, and `EdgeShinyAutoPlaySpeed` are previewed only when the object is selected in edit mode ([952a94f](https://github.com/mob-sakai/UIEffect/commit/952a94f50b3ae4279a48b03d019f341e46b2f5b7)), closes [#319](https://github.com/mob-sakai/UIEffect/issues/319)
* add 'Flip' feature ([def49c3](https://github.com/mob-sakai/UIEffect/commit/def49c34f5b963c8b0cce6fb8cace3f7d93b5a1e))
* add `Detail Filter` feature ([3e2c5c1](https://github.com/mob-sakai/UIEffect/commit/3e2c5c1613d9a2c08487d94c4eef28f066f2eb7b))
* add `EdgeColorGlow` option ([53db1e6](https://github.com/mob-sakai/UIEffect/commit/53db1e62546384f04b9c526df2623c4598dd34bf))
* add `onChangeRate` event for UIEffectTweener ([8ad57c8](https://github.com/mob-sakai/UIEffect/commit/8ad57c828ef4f1d7df6dd22c1edb7af3efa04d75)), closes [#321](https://github.com/mob-sakai/UIEffect/issues/321)
* add `TransitionTexSpeed` option ([6b8dc5e](https://github.com/mob-sakai/UIEffect/commit/6b8dc5ec338590d909511eba3bb1b2e0ec6627f3))
* add a tool that automatically fixes shaders when 'TextMeshPro Essential Resources' are located in a non-default path ([8388d59](https://github.com/mob-sakai/UIEffect/commit/8388d590457755cda0699835b37b5bf0673c4386))
* add built-in presets ([3db94e0](https://github.com/mob-sakai/UIEffect/commit/3db94e00f83195ff90d26ee270c7984825fa22b7))
* add shaders for TextMeshPro Overlay/SSD ([51cd6d8](https://github.com/mob-sakai/UIEffect/commit/51cd6d8525a12581c8afd4ab5d16e5968e15c8fe))
* Allow specifying a preset as the target for `UIEffectReplica` ([cb0e8d2](https://github.com/mob-sakai/UIEffect/commit/cb0e8d283bb8f9085d4910051b5ae0b77368db76))
* custom transform for transition and gradation ([045db9c](https://github.com/mob-sakai/UIEffect/commit/045db9c3b79234cd12bdfb4ea52898b7d86240af))
* expose color subfields as float-type public properties ([396d06f](https://github.com/mob-sakai/UIEffect/commit/396d06f774e10adee84d06156a7fc8723d986bbd)), closes [#321](https://github.com/mob-sakai/UIEffect/issues/321)
* load presets partially and append to existing instance ([6542543](https://github.com/mob-sakai/UIEffect/commit/654254342265fc239804e60a64d01b80076e5e8e))
* separate reverse curve for UIEffectTweener ([2540b75](https://github.com/mob-sakai/UIEffect/commit/2540b75349af92f1359433e73ffa3b5400479b02)), closes [#312](https://github.com/mob-sakai/UIEffect/issues/312)
* support `SoftMaskForUGUI ` package with `UIEffect` ([65457e2](https://github.com/mob-sakai/UIEffect/commit/65457e263644c36cd772ad1f42fb9fb3f84ab753)), closes [#270](https://github.com/mob-sakai/UIEffect/issues/270)
* support UIFlip as v4 compatible component ([e965b9e](https://github.com/mob-sakai/UIEffect/commit/e965b9e9ab33dcfab067542abf2f19bc1d5243fc)), closes [#322](https://github.com/mob-sakai/UIEffect/issues/322)
* update demo sample ([abc9a41](https://github.com/mob-sakai/UIEffect/commit/abc9a4184c4aeac85287a882d174f9cdc2c28c04)), closes [#318](https://github.com/mob-sakai/UIEffect/issues/318) [#320](https://github.com/mob-sakai/UIEffect/issues/320)

## [5.5.3](https://github.com/mob-sakai/UIEffect/compare/5.5.2...5.5.3) (2025-03-06)


### Bug Fixes

* blur with 'Filled' Image type may cause error ([f0a7642](https://github.com/mob-sakai/UIEffect/commit/f0a76424e690355c101cc2e12183d1a7e77b16c3)), closes [#313](https://github.com/mob-sakai/UIEffect/issues/313)
* IL2CPP build fails on older versions of Unity ([1af89af](https://github.com/mob-sakai/UIEffect/commit/1af89af2c58320f67b8d753767b3b4cf905f844d)), closes [#315](https://github.com/mob-sakai/UIEffect/issues/315)

## [5.5.2](https://github.com/mob-sakai/UIEffect/compare/5.5.1...5.5.2) (2025-02-28)


### Bug Fixes

* `PlayOnEnable.Reverse` in UIEffectTweener does not work ([3f5a7dc](https://github.com/mob-sakai/UIEffect/commit/3f5a7dcd2d8248b9e47fb9d38c083f26b798e063)), closes [#310](https://github.com/mob-sakai/UIEffect/issues/310)
* `TransitionFilter` and `GradationMode` do not work correctly when the initial scale is 0 ([bdbf293](https://github.com/mob-sakai/UIEffect/commit/bdbf293c1489c0b248078969b37ec66890620d44)), closes [#311](https://github.com/mob-sakai/UIEffect/issues/311)
* component icons will no longer be displayed in the scene view (Unity 2021.2 or later) ([40ff828](https://github.com/mob-sakai/UIEffect/commit/40ff828ff0697dc8f6597cfcf094cc63532a19dd))
* fix potential issues ([a748adf](https://github.com/mob-sakai/UIEffect/commit/a748adf1bde5e400fcc84ec902a1438867105f75))
* reset time button does not work correctly in the inspector when `UIEffectTweener.direction=Reverse` in the editor ([8513970](https://github.com/mob-sakai/UIEffect/commit/8513970e6f71d393430a573bfb0207428998bee1)), closes [#310](https://github.com/mob-sakai/UIEffect/issues/310)

## [5.5.1](https://github.com/mob-sakai/UIEffect/compare/5.5.0...5.5.1) (2025-02-14)


### Bug Fixes

* fix v4 compatible components ([6f6a8c9](https://github.com/mob-sakai/UIEffect/commit/6f6a8c95f878a290b1c4238cd0519f14febc04b6))
* fully visible when the transition rate is set to 1 ([0a99bb7](https://github.com/mob-sakai/UIEffect/commit/0a99bb7765443a84070e19c9ea0096808442932e)), closes [#309](https://github.com/mob-sakai/UIEffect/issues/309)
* UIEffect with TextMeshPro appears black in the editor when saving scene ([8cbcd76](https://github.com/mob-sakai/UIEffect/commit/8cbcd765ed9e11c38e3257191c2fc7dcd2c9916e))

# [5.5.0](https://github.com/mob-sakai/UIEffect/compare/5.4.0...5.5.0) (2025-02-12)


### Features

* add `GradationMode.Diagonal` with 4 colors ([b72a1c0](https://github.com/mob-sakai/UIEffect/commit/b72a1c05c6d1d7b3c0aa11582d4a97da80d7fce9)), closes [#308](https://github.com/mob-sakai/UIEffect/issues/308)
* add built-in presets ([8930b56](https://github.com/mob-sakai/UIEffect/commit/8930b563cb580463e2265483ccd3a42cb84e644b))

# [5.4.0](https://github.com/mob-sakai/UIEffect/compare/5.3.4...5.4.0) (2025-02-10)


### Bug Fixes

* gradient color is not updated when calling the `SetGradientKeys` method ([a959fdd](https://github.com/mob-sakai/UIEffect/commit/a959fddd34a93dd0846369938b1cb6be514eee2d))


### Features

* tweener supports `EdgeMode.Shiny` ([32558d4](https://github.com/mob-sakai/UIEffect/commit/32558d4481de31ed9020c82861dc2a2d8ca7d53c)), closes [#307](https://github.com/mob-sakai/UIEffect/issues/307)

## [5.3.4](https://github.com/mob-sakai/UIEffect/compare/5.3.3...5.3.4) (2025-02-10)


### Bug Fixes

* UIEffect v5.3.3 with TextMeshPro is not working in play mode in editor ([365c43e](https://github.com/mob-sakai/UIEffect/commit/365c43e21dcaaff349cf1362f9bd25b7deb163f7)), closes [#306](https://github.com/mob-sakai/UIEffect/issues/306)

## [5.3.3](https://github.com/mob-sakai/UIEffect/compare/5.3.2...5.3.3) (2025-02-06)


### Bug Fixes

* when modifying TextMeshPro vertices, element disapear ([7529a7c](https://github.com/mob-sakai/UIEffect/commit/7529a7c4cd8db27a6d3312bce0a6ed6b13e40f35)), closes [#305](https://github.com/mob-sakai/UIEffect/issues/305)

## [5.3.2](https://github.com/mob-sakai/UIEffect/compare/5.3.1...5.3.2) (2025-02-04)


### Bug Fixes

* some effects are not working correctly for TextMeshProUGUI in Unity 6 ([63de374](https://github.com/mob-sakai/UIEffect/commit/63de3742ecd7fcdcaa8dcca8dc104c7288291211))

## [5.3.1](https://github.com/mob-sakai/UIEffect/compare/5.3.0...5.3.1) (2025-01-27)


### Bug Fixes

* shader keywords for UIEfect are not recognized in Unity 6 ([fd39abf](https://github.com/mob-sakai/UIEffect/commit/fd39abf5a71138919f8feee587576ba187075304)), closes [#301](https://github.com/mob-sakai/UIEffect/issues/301)

# [5.3.0](https://github.com/mob-sakai/UIEffect/compare/5.2.4...5.3.0) (2025-01-26)


### Features

* `SamplingFilter.EdgeAlpha` and `SamplingFilter.EdgeLuminance` now support the `Sampling Width` property for edge width ([33b1177](https://github.com/mob-sakai/UIEffect/commit/33b1177815b935a887c5a8918748cbeafec32b6b))
* add `EdgeMode` feature ([eadb477](https://github.com/mob-sakai/UIEffect/commit/eadb477bcd91f8909bfa7b8976241411363ee12c))
* add `ResetOnEnable` option for `UIEffectTweener` ([e326bb7](https://github.com/mob-sakai/UIEffect/commit/e326bb77537b921df775646142105a490ae54851)), closes [#299](https://github.com/mob-sakai/UIEffect/issues/299)
* add `TransitionAutoSpeed` property ([7a765c3](https://github.com/mob-sakai/UIEffect/commit/7a765c32d87f6dfbdb990d5b3a272dd80696b374))
* add `TransitionFilter.Pattern` feature ([b57e98b](https://github.com/mob-sakai/UIEffect/commit/b57e98b64c61a7c7eb3ca97f3bd7e909956ee469))

## [5.2.4](https://github.com/mob-sakai/UIEffect/compare/5.2.3...5.2.4) (2025-01-26)


### Bug Fixes

* properties may not update correctly when changed via code or animation ([5994f88](https://github.com/mob-sakai/UIEffect/commit/5994f8875ed2280e5c863c30f9c7900d4dce70ed)), closes [#300](https://github.com/mob-sakai/UIEffect/issues/300)

## [5.2.3](https://github.com/mob-sakai/UIEffect/compare/5.2.2...5.2.3) (2025-01-23)


### Bug Fixes

* `SamplingFilter.None` now correctly respects the sampling scale setting ([bc1578b](https://github.com/mob-sakai/UIEffect/commit/bc1578b0ca91b830b976bf200d387d0ac6963ae6))
* fix the shadow mode inspector in editor ([da53585](https://github.com/mob-sakai/UIEffect/commit/da535854ccbeeb98b2556423425d39058bd5f479))
* UIEffect may not work correctly when using `FallbackAtlas` with TextMeshProUGUI ([d3779f9](https://github.com/mob-sakai/UIEffect/commit/d3779f9f57d0410fa4526527ebed30885bfbc2dd)), closes [#298](https://github.com/mob-sakai/UIEffect/issues/298)

## [5.2.2](https://github.com/mob-sakai/UIEffect/compare/5.2.1...5.2.2) (2025-01-18)


### Bug Fixes

* in Unity 6, `Shadow Color` has no effect on TextMeshPro ([6e4d4ab](https://github.com/mob-sakai/UIEffect/commit/6e4d4ab03db786e4c28d0f2c57d9e58655c5d029)), closes [#297](https://github.com/mob-sakai/UIEffect/issues/297)

## [5.2.1](https://github.com/mob-sakai/UIEffect/compare/5.2.0...5.2.1) (2025-01-14)


### Bug Fixes

* shadow color being transparent at maximum `Shadow Fade` ([1d849e5](https://github.com/mob-sakai/UIEffect/commit/1d849e54cb57218a67afc2f8ad36d345bfdd0f74)), closes [#296](https://github.com/mob-sakai/UIEffect/issues/296)
* TextMeshPro is not displayed in prefab mode ([2b44837](https://github.com/mob-sakai/UIEffect/commit/2b44837bb9c93e1cc817a2c1cd112b244cd09391)), closes [#295](https://github.com/mob-sakai/UIEffect/issues/295)
* TextMeshProUGUI disappears when the Y-axis scale is changed ([8159c09](https://github.com/mob-sakai/UIEffect/commit/8159c091e8a6805a7781046ae0f660a3be13862c)), closes [#295](https://github.com/mob-sakai/UIEffect/issues/295)

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
