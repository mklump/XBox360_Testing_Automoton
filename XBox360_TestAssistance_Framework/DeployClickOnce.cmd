@echo off

set CURDIR=%CD%
set BATCHPATH=%~dp0

if "%1"=="/?" goto :usage
if "%1"=="" goto :usage

set VERSION=%1
set BIN_PATH=%~f2
set SERVER_PATH=%~f3

if "%BIN_PATH%"=="" set BIN_PATH=%CURDIR%
if "%SERVER_PATH%"=="" set SERVER_PATH=\\insertcoin\ToolsWorkshop\CAT

echo Creating fresh output folder: "%TEMP%\%VERSION%"
if exist "%TEMP%\%VERSION%" rmdir "%TEMP%\%VERSION%" /s /q
mkdir "%TEMP%\%VERSION%"

echo Deleting old files: "%TEMP%\CAT.application"
if exist "%TEMP%\CAT.application" del "%TEMP%\CAT.application"

echo Deleting old files: "%TEMP%\CAT.application.orig"
if exist "%TEMP%\CAT.application.orig" del "%TEMP%\CAT.application.orig"

echo Generating application manifest: "%TEMP%\%VERSION%\CAT.exe.manifest"
mage -New Application -Processor x86 -ToFile "%TEMP%\%VERSION%\CAT.exe.manifest" -Name "CAT" -Version %VERSION% -FromDirectory "%BIN_PATH%" -IconFile CAT.ico

echo Creating new deployment manifest: "%TEMP%\CAT.application.orig"
mage -New Deployment -Processor x86 -ToFile "%TEMP%\CAT.application.orig" -Name "CAT" -Version %VERSION% -AppManifest "%TEMP%\%VERSION%\CAT.exe.manifest" -providerUrl "%SERVER_PATH%\CAT.application" -i true

echo Updating deployment manifest to enforce new version as min version, and Published by Experis: "%TEMP%\CAT.application.orig"
mage -Update "%TEMP%\CAT.application.orig" -MinVersion %VERSION% -Publisher Experis

echo Creating updated deployment manifest with on-launch update policy: "%TEMP%\CAT.application"
call %BATCHPATH%\FindReplace "%TEMP%\CAT.application.orig" "%TEMP%\CAT.application" "<expiration maximumAge="0" unit="days" />" "<beforeApplicationStartup/>"

echo Copying files to "%SERVER_PATH%\%VERSION%\"
mkdir  "%SERVER_PATH%\%VERSION%\"
copy "%TEMP%\CAT.application" "%SERVER_PATH%\CAT.application"
copy "%TEMP%\%VERSION%\CAT.exe.manifest" "%SERVER_PATH%\%VERSION%\CAT.exe.manifest"
xcopy "%BIN_PATH%" "%SERVER_PATH%\%VERSION%\" /e /y

echo Removing old temp files
rmdir "%TEMP%\%VERSION%" /s /q
if exist "%TEMP%\CAT.application" del "%TEMP%\CAT.application"
if exist "%TEMP%\CAT.application.orig" del "%TEMP%\CAT.application.orig"


goto :eof
:usage
echo Usage: %0 Version [BinPath] [ServerPath]
echo.

:eof
