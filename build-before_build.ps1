.\version-pull-request-before_build.ps1

Write-Host "Restoring nuget packages"
Push-Location -Path src
nuget restore
Pop-Location