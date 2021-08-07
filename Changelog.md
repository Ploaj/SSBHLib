# Cross Mod
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