::
::
@echo off

::
:: Display Unity value
set UNITY_EXECUTABLE_PATH="%UNITY_HINT_PATH%\2021.2.8f1\Editor\Unity.exe"
:: set UNITY_EXECUTABLE_PATH="C:\Program Files\Unity\2021.2.8f1\Editor\Unity.exe"
echo Unity path : %UNITY_EXECUTABLE_PATH%

::
:: Build the game
%UNITY_EXECUTABLE_PATH% -batchmode -quit -logfile - -buildTarget WebGL --projectPath ./GGJ22_project/ -executeMethod WebGLBuilder.Build