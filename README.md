![](https://github.com/OpenCppCoverage/OpenCppCoveragePlugin/workflows/Unit%20tests/badge.svg)
# OpenCppCoveragePlugin for VisualStudio2022
This is a fork of [Official Visual Studio Plugin for OpenCppCoverage](https://github.com/OpenCppCoverage/OpenCppCoveragePlugin).
This works for me. But tests are broken.

**OpenCppCoverage** is an open source code coverage tool for C++ under Windows. You can find more information about this project [here](https://opencppcoverage.codeplex.com/).

This repository contains only the Visual Studio plugin sources.

## Usage

~~To install and use this plugin, please see the [Visual Studio Gallery page](https://visualstudiogallery.msdn.microsoft.com/f45b8e13-f847-4b3b-92df-984df633b60e).
You can also install the NuGet package **OpenCppCoverage Plugin**.~~

**Documentation is available [here](https://github.com/OpenCppCoverage/OpenCppCoveragePlugin/wiki)**.

~~For questions, you can create a discussion [here](https://opencppcoverage.codeplex.com/discussions).
If you find a bug, you can create an issue [here](https://opencppcoverage.codeplex.com/workitem/list/basic).~~

## Development

### Compilation
You have 2 Visual Studio solution files but **Visual Studio 2022 is always required**.
Please also make sure you have Visual Studio 2022 **version 17.3.X**.

#### CppCoveragePlugin.sln
This is the default solution file and it requires only Visual Studio 2022.

#### CppCoveragePluginVS2013.sln
~~**This is a Visual Studio 2017 solution file** but it also requires:~~
 * ~~Visual Studio 2013 Update 5.~~
 * ~~[Microsoft Visual Studio 2013 SDK](https://visualstudiogallery.msdn.microsoft.com/842766ba-1f32-40cf-8617-39365ebfc134/view/). If you have any trouble to install the SDK, [this stack overflow question](https://stackoverflow.com/questions/22949411/visual-studio-2012-install-fails-program-compatibility-mode-is-on/23114542) can help.~~
 * ~~Visual Studio 2015 Update 3 may be required.~~
 
~~This solution should be used only to generate a plugin compatible with Visual Studio 2013, 2015 and 2017.~~

#### OpenCppCoverage
You should install the latest version of [OpenCppCoverage](https://github.com/OpenCppCoverage/OpenCppCoverage/releases/tag/release-0.9.9.0):
* *OpenCppCoverageSetup-x64-X.X.X.exe*: into *VSPackage\OpenCppCoverage-x64* 
* *OpenCppCoverageSetup-x86-X.X.X.exe*: into *VSPackage\OpenCppCoverage-x86*

You can also copy past the binaries from an existing installation into these folders.
Binaries inside *VSPackage\OpenCppCoverage-x86* can be the same as *VSPackage\OpenCppCoverage-x64* (The opposite is not true).  

You also should copy msvcp140.dll and vcruntime140.dll from  
`C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\IDE`
to 
`\your\solution\directory\VSPackage\OpenCppCoverage-x64` and
`\your\solution\directory\VSPackage\OpenCppCoverage-x86`.

You also should copy Newtonsoft.Json.dll from 
`C:\Users\YourName\.nuget\packages\newtonsoft.json\13.0.1\lib\net45`
to 
`\your\solution\directory\VSPackage\`.

### Run the plugin

* Set *VSPackage* as *StartUp Project*.
* In *VSPackage Properties*, tab *Debug*:
  * Select *Start external program* and set value to `C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\IDE\devenv.exe` (Update this path if you installed Visual Studio to another location).
  * Add `/RootSuffix Exp` as *Command line arguments*.

If you have an issue when running the plugin, you can try to reset Visual Studio Experimental instance:

`"C:\Program Files\Microsoft Visual Studio\2022\Communityy\VSSDK\VisualStudioIntegration\Tools\Bin\CreateExpInstance.exe" /Reset /VSInstance=17.0 /RootSuffix=Exp`

### Run unit tests

You can run the tests with *Test Explorer window*.
~~To run *VSPackage_IntegrationTests* you need to expand *Solution items* in *Solution Explorer* and set *Active Load and Web Test Settings* for *IntegrationTests.testsettings*. If you have a COM error when running tests, you can select *IntegrationTests.testsettings* file from Visual Studio menu: Test/Test Settings/Select Test Settings File.~~

On Visual Studio 2022 *VSPackage_IntegrationTests* is broken because VSIDETestAdapter is deprecated and already erased.

For *VSPackage_UnitTests* you need to do with *UnitTests.runsettings*.

If a test failed, you can try to run it again. You can also reset Visual Studio Experimental instance.
