@echo off
rem "%programfiles(x86)%\Microsoft\ILMerge\ILMerge.exe"
rem ILMerge.exe /targetplatform:v4 /log:ilmerge.log /out:Rangeen2.exe tmp.exe ActiveUp.dll UtilsLib.dll
ILMerge.exe /targetplatform:v4 /out:Rangeen2.exe Rangeen.exe ActiveUp.dll UtilsLib.dll
del Rangeen.exe
move Rangeen2.exe Rangeen.exe
