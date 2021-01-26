# SSBHLib
SSBHLib is a library for reading and writing the SSBH format for Smash Ultimate. If you discover a bug in any of these projects, report it in
[issues](https://github.com/Ploaj/CrossMod/issues).

## Project Structure 
- SSBHLib - SSBH format definitions, parsing, exporting, and decoding/encoding of animation and mesh data
- CrossMod - projects related to CrossModGUI
    - CrossMod - rendering and file parsing functionality for CrossModGui
    - CrossMod.MaterialValidation - queries an SQLite database of in game shaders for validating materials
    - CrossModGUI - a desktop model viewer and material editor for Windows using WPF + OpenTK
- MatLab - a command line tool for editing MATL files (`.numatb`) by converting to and from XML
- Tools - a collection of command line tools for exporting SSBH files to more common formats for reverse engineering
    - BatchExportNumatbToXml - uses MatLab to convert `.numatb` files to `.xml`
    - BatchExportShaderBinaries - dumps the compiled shader data from `.nushdb` files
    - NuanmbToJson - converts `.nuanmb` files to JSON but does not support editing
    - SSBHBatchProcess - tests reading/writing various SSBH formats for a specified directory

## Cross Mod Gui
<img src="https://github.com/Ploaj/SSBHLib/blob/master/CrossModApp.jpg" align="top" height="auto" width="auto"><br>
A Smash Ultimate model viewer with a material editor. An executable can be downloaded from [releases](https://github.com/Ploaj/SSBHLib/releases). For creating model imports, see [StudioSB](https://github.com/Ploaj/StudioSB). The code for the original WinForms application has been moved to [Cross-Mod-Old](https://github.com/Ploaj/SSBHLib/tree/cross-mod-old). 

### System Requirements
The recommended OpenGL version for Cross Mod is 4.20. Version 3.30 or higher may still work as long as the necessary OpenGL extensions are present. Cross Mod GUI as well as the command line programs require .NET Core. See the release for installation instructions.

## Mat Lab
A simple program for converting .numatb files to .xml and .xml files to .numatb. The output path will be generated as `<input>_out.xml` or `<input>_out.numatb` if not specified. Files can also be dragged onto the executable to convert them. An executable can be downloaded from [releases](https://github.com/Ploaj/SSBHLib/releases).

**Usage**
`MatLab.exe <input> [output]`  

# Building
Compile in Visual Studio 2019 or later. Requires .NET Core 3.1. Cross Mod GUI and Cross Mod require Windows specific components.
SSBHLib itself and the other command line programs will work on any platform supporting .Net Core. 
