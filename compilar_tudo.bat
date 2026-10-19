@echo off
title SecHex - Compilador e Instalador
chcp 65001 >nul
cd /d "%~dp0"

echo =========================================================
echo       SecHex-Spoofy - Compilador e Instalador
echo =========================================================
echo.

:: Localiza o dotnet
set "DOTNET_CMD=dotnet"
where dotnet >nul 2>&1
if %errorLevel% neq 0 (
    if exist "%LOCALAPPDATA%\Microsoft\dotnet\dotnet.exe" (
        set "DOTNET_CMD=%LOCALAPPDATA%\Microsoft\dotnet\dotnet.exe"
    ) else (
        echo [X] Erro: .NET SDK não encontrado no PATH nem em AppData.
        echo Baixe em: https://dotnet.microsoft.com/download
        pause
        exit /b 1
    )
)

echo [*] Compilando e gerando binários em ./dist ...
"%DOTNET_CMD%" publish "SecHex.sln" -c Release -o "dist"

if %errorLevel% equ 0 (
    echo.
    echo [✓] Compilação concluída com sucesso!
    echo [*] Arquivos gerados na pasta: %~dp0dist\
    echo.
    echo Para executar:
    echo   - Clique em "iniciar_cli.bat" para a versão Console.
    echo   - Clique em "iniciar_gui.bat" para a versão Interface Gráfica.
    echo.
) else (
    echo.
    echo [X] Ocorreu uma falha durante a compilação.
)

pause
