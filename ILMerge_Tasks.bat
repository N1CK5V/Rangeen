@echo off
ILMerge.exe /targetplatform:v4 /out:%1_2.dll %1.dll UtilsLib.dll
del %1.dll
move %1_2.dll %1.dll
