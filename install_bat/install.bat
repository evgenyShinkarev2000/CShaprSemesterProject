:: BatchGotAdmin
:-------------------------------------
REM  --> Check for permissions
    IF "%PROCESSOR_ARCHITECTURE%" EQU "amd64" (
>nul 2>&1 "%SYSTEMROOT%\SysWOW64\cacls.exe" "%SYSTEMROOT%\SysWOW64\config\system"
) ELSE (
>nul 2>&1 "%SYSTEMROOT%\system32\cacls.exe" "%SYSTEMROOT%\system32\config\system"
)

REM --> If error flag set, we do not have admin.
if '%errorlevel%' NEQ '0' (
    echo Requesting administrative privileges...
    goto UACPrompt
) else ( goto gotAdmin )

:UACPrompt
    echo Set UAC = CreateObject^("Shell.Application"^) > "%temp%\getadmin.vbs"
    set params= %*
    echo UAC.ShellExecute "cmd.exe", "/c ""%~s0"" %params:"=""%", "", "runas", 1 >> "%temp%\getadmin.vbs"

    "%temp%\getadmin.vbs"
    del "%temp%\getadmin.vbs"
    exit /B

:gotAdmin
    pushd "%CD%"
    CD /D "%~dp0"
:--------------------------------------  
reg add "HKEY_CLASSES_ROOT\*\shell\MyCustomArhivator" /d pack
reg add "HKEY_CLASSES_ROOT\*\shell\MyCustomArhivator\command" /d C:\projects\CSharp\CShaprSemesterProject\CShaprSemesterProject\bin\Debug\net6.0-windows\CShaprSemesterProject.exe
reg add "HKEY_CLASSES_ROOT\*\shell\MyCustomArhivator\DefaultIcon" /d C:\projects\CSharp\CShaprSemesterProject\CShaprSemesterProject\bin\Debug\net6.0-windows\CShaprSemesterProject.exe
reg add HKEY_CLASSES_ROOT\.myrar\shell\MyCustomArhivator /d unpack
reg add HKEY_CLASSES_ROOT\.myrar\shell\MyCustomArhivator\command /d C:\projects\CSharp\CShaprSemesterProject\CShaprSemesterProject\bin\Debug\net6.0-windows\CShaprSemesterProject.exe
reg add HKEY_CLASSES_ROOT\.myrar\shell\MyCustomArhivator\DefaultIcon /d C:\projects\CSharp\CShaprSemesterProject\CShaprSemesterProject\bin\Debug\net6.0-windows\CShaprSemesterProject.exe

::C:\projects\CSharp\CShaprSemesterProject\CShaprSemesterProject\bin\Debug\net6.0-windows\CShaprSemesterProject.exe