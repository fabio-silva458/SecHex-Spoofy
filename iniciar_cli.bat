@echo off
title SecHex-Spoofy CLI
cd /d "%~dp0"

:: Solicita elevação se não for administrador
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo [!] Solicitando privilegios de Administrador...
    powershell -Command "Start-Process '%~dp0dist\RPZ-HWID.exe' -Verb RunAs"
    exit /b
)

"%~dp0dist\RPZ-HWID.exe"
pause
