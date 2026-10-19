@echo off
title SecHex - Teste Automatizado da Aplicacao
chcp 65001 >nul
cd /d "%~dp0"

"%~dp0dist\RPZ-HWID.exe" --test

echo.
pause
