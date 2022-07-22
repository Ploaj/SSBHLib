# Cross Mod
**v0.22**
This is the final release. Cross Mod is no longer maintained and has been replaced by SSBH Editor.
SSBH Editor can be downloaded from https://github.com/ScanMountGoat/ssbh_editor/releases.
* Added additional material parameter descriptions.
* Added rendering support for additional default textures.

**v0.21**
* Fixed an issue where mesh object names failed to display in the material editor for missing attributes.
* Improved rendering accuracy by toggling alpha testing based on the current shader label. This uses a heuristic based on decompiled shader code and may not always be accurate.
* Added a readonly checkbox for whether a material might use alpha testing to the material editor.

**v0.20**
* Added the ability to reload the current workspace with File > Reload Workspace or using Ctrl + R.
* Fixed a potential crash when starting Cross Mod while not connected to the internet.
* Improved accuracy of scale rendering for skeletal animations.
* Fixed an issue where some models weren't framed properly in the viewport after opening.

**v0.19**
* Fixed texture swizzling for 2D textures.
* Added support for rendering R8Unorm textures, which is used for the spycloak texture.
* Fixed rendering of BC4 and BC5 textures.
* Improved scale accuracy for rendering skeletal animations.
* Fixed a bug where disabling face culling wouldn't update the viewport preview.
* Added rendering for depth settings and sort bias. This fixes alpha rendering issues for some transparent meshes like the fairy bottle item.
* Fixed black shading artifacts for some models. 
* Added a flat shading preset (Zarek).
* Added experimental support for playing camera animations in the viewport.
* Updated shader database to version 13.0.0. This fixes incorrect missing shader warnings (red checkerboard) 
for DLC fighters.
* Fixed closing side panels.
* Adjusted folder items to be easier to expand/collapse.

**v0.18**
* Fixed an issue where a mesh attribute error (yellow checkerboard) would not be displayed for a shared material.
* Fixed material animations not rendering in the viewport.
* Fixed hitboxes not rendering in the viewport.
* Added information for invalid shader labels to the material editor.
* Fixed an issue where some texture sizes wouldn't deswizzle properly, resulting in black artifacts. This applies to Pyra, Mythra, and a few other in game models.
* Improved the accuracy of rim lighting and reduced black artifacts in specular shading.
* Improved texture blending and added CustomBoolean11 rendering. This fixes the washed out eyes on some fighter models.
* Increased the shader bone limit to 512 to fix crashes when rendering some models.

**v0.17**
* Changed the window title to display the current release version.
* Added an automatic check to display details for new releases when launching the program.
* Added the ability to resize the side panels
* Fixed an issue where a mesh attribute error (yellow checkerboard) would be incorrectly displayed for stage ink meshes.
* Added a toggle to render settings for post processing. Disabling post processing may improve rendering accuracy for some stage models.
* Fixed a crash when rendering a model if the model.nusktb file is missing.
* Fixed an issue where the wrap mode and texture filtering was set incorrectly for materials referencing the same texture.
* Fixed missing padding when exporting materials.

**v0.16**
* Minor improvements to rim lighting
* Added rendering for yellow checkerboard in viewport (missing mesh attributes)
* Added toggle for rendering errors in render settings
* Added descriptions for missing attributes in material editor

**v0.15**
* Added material presets. In the material editor, click Material > Select Preset, select a preset from the list, and click Apply Preset. This applies to the currently selected material in the dropdown.
* Improved anisotropic specular shading for hair
* Improved CPU usage while using the material editor or editing render settings
* Added descriptions for some material parameters in the material editor
* Added an option to save a screenshot of the viewport by clicking Viewport > Save Screenshot
* Added the ability to edit the render pass for the selected shader in the material editor.

**v0.14**
* Improvements to ambient diffuse lighting
* Improved skin shading and CustomVector11/CustomVector30 rendering
* Added post processing and color LUT rendering
* Added rendering for alpha sample to coverage and CustomBoolean2
* Added alpha sample to coverage to the material editor
* More compact layout for material editor
* Stability improvements while editing values with the material editor
* Added a list of vertex attributes used by the shader to the material editor