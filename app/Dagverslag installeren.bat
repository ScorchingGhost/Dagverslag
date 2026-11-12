@echo off
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0Install-Dagverslag.ps1"
if errorlevel 1 (
    echo.
    echo De installatie is niet gelukt.
    pause
    exit /b 1
)
