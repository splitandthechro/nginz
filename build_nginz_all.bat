@echo off
set msbuildpath="%programfiles(x86)%\MSBuild\14.0\Bin\"
cd src
%msbuildpath%msbuild.exe /p:Configuration=Release /p:outdir="..\bin\"
echo Done.
pause >nul