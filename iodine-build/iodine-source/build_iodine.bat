@echo off
set msbuildpath="C:\Program Files (x86)\MSBuild\14.0\Bin\"
cd src\Iodine
%msbuildpath%msbuild.exe /p:Configuration=Release /p:outdir="..\..\bin\"
pause >nul