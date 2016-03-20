@echo off

set BATCHPATH=%~dp0

if "%1"=="/?" goto :usage
if "%1"=="" goto :usage

set VERSION=%1
set BIN_PATH=%~f2

call %BATCHPATH%\DeployClickOnce %VERSION% "%BIN_PATH%" "\\insertcoin\Automaton\Milestones\ClickOnce"

goto :eof
:usage
echo Usage: %0 Version [BinPath]
echo.

:eof
