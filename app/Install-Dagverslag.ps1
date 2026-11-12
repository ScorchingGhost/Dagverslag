$ErrorActionPreference = "Stop"

$installDirectory = Join-Path $env:LOCALAPPDATA "Programs\Dagverslag"
$executable = Join-Path $installDirectory "Dagverslag.exe"
$sourceMedia = Join-Path $PSScriptRoot "media"

New-Item -ItemType Directory -Path $installDirectory -Force | Out-Null
Copy-Item -Path (Join-Path $PSScriptRoot "Dagverslag.exe") -Destination $executable -Force
Copy-Item -Path $sourceMedia -Destination $installDirectory -Recurse -Force

$desktop = [Environment]::GetFolderPath([Environment+SpecialFolder]::DesktopDirectory)
$shortcutPath = Join-Path $desktop "Dagverslag.lnk"
$shell = New-Object -ComObject WScript.Shell
$shortcut = $shell.CreateShortcut($shortcutPath)
$shortcut.TargetPath = $executable
$shortcut.WorkingDirectory = $installDirectory
$shortcut.IconLocation = "$executable,0"
$shortcut.Description = "Dagverslag openen"
$shortcut.Save()

Write-Host "Dagverslag is geïnstalleerd. De snelkoppeling staat op je bureaublad."
Start-Process -FilePath $executable
