@echo off
title SecHex-Spoofy GUI
cd /d "%~dp0"

:: Solicita elevação se não for administrador
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo [!] Solicitando privilegios de Administrador para a interface...
    powershell -Command "Start-Process '%~dp0dist\SecHex-GUI.exe' -Verb RunAs"
    exit /b
)

start "" "%~dp0dist\SecHex-GUI.exe"
