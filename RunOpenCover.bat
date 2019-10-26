@echo off
SET OPEN_COVER="%~dp0\packages\OpenCover.4.7.922\tools\OpenCover.Console.exe"
SET VSTEST_CONSOLE="%ProgramFiles(x86)%\Microsoft Visual Studio\2019\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe"
SET UNIT_TEST="%~dp0\VSPackage_UnitTests\bin\Debug\VSPackage_UnitTests.exe"
SET REPORT_GENERATOR="%~dp0\packages\ReportGenerator.4.3.2\tools\net47\ReportGenerator.exe"
SET OUTPUT="OpenCover.xml"

%OPEN_COVER% -target:%VSTEST_CONSOLE% -targetargs:%UNIT_TEST% -filter:"+[*]* -[GalaSoft*]*" -register:user -output:%OUTPUT%
%REPORT_GENERATOR% "-reports:%OUTPUT%" "-targetdir:CoverageReport"