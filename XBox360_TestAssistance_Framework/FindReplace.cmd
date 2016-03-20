@echo off
setlocal EnableDelayedExpansion

if "%1"=="/?" goto :usage
if "%1"=="" goto :usage

if not exist "%1" (echo "%1" does not exist...)&goto :eof


set infile=%1
set outfile=%2
set findthis=%3
set replacewith=%4


FOR /F "delims=" %%I IN (%findthis%) DO SET findthis=%%I
FOR /F "delims=" %%I IN (%replacewith%) DO SET replacewith=%%I

if exist "%outfile%" del "%outfile%"

for /f "tokens=*" %%z in ('type "%infile%"') do (
	set txt=%%z

	REM TRIM LEFT
	for /f "tokens=* delims= " %%b in ("!txt!") do set str=%%b

	REM TRIM RIGHT
	for /l %%a in (1,1,31) do if "!txt:~-1!"==" " set txt=!txt:~0,-1!

	if "!txt!"=="!findthis!" set txt=!replacewith!

	(echo !txt!)>>%outfile%
)

goto :eof
:usage
echo Usage: %0 InputFile OutputFile FindString ReplaceString
echo.

:eof
