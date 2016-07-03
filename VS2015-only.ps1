function ReplaceInFile($file, $oldText, $newText){
	(Get-Content $file) -replace $oldText, $newText | Out-File -Encoding "UTF8" $file
}

ReplaceInFile 'VSPackage/VSPackage.csproj' 'Shell.12.0, Version=12' 'Shell.14.0, Version=14'
ReplaceInFile 'VSPackage/VSPackage.csproj' 'VCProjectEngine, Version=12' 'VCProjectEngine, Version=14' 
ReplaceInFile 'VSPackage_IntegrationTests/IntegrationTestsSolution/ConsoleApplicationInFolder/ConsoleApplicationInFolder.vcxproj' '<PlatformToolset>v120' '<PlatformToolset>v140'
ReplaceInFile 'VSPackage_IntegrationTests/IntegrationTestsSolution/CppConsoleApplication/CppConsoleApplication.vcxproj' '<PlatformToolset>v120' '<PlatformToolset>v140'
ReplaceInFile 'VSPackage_IntegrationTests/IntegrationTestsSolution/CppConsoleApplication2/CppConsoleApplication2.vcxproj' '<PlatformToolset>v120' '<PlatformToolset>v140'
ReplaceInFile 'VSPackage_IntegrationTests/IntegrationTestsSolution/CppConsoleApplicationDll/CppConsoleApplicationDll.vcxproj' '<PlatformToolset>v120' '<PlatformToolset>v140'
 
