param([Parameter(Mandatory=$True)][string]$HostName,[Parameter(Mandatory=$True)][string]$UserLogin,[Parameter(Mandatory=$True)][string]$UserPwd)

Write-Output "Deleting messages that have passed their expiration date"

$scriptpath = $MyInvocation.MyCommand.Path
$dir = Split-Path $scriptpath
$path = $dir + "\deleteExpiredMessageData.js"

mongo $path --verbose -host "$HostName" -u "$UserLogin" -p "$UserPwd" --authenticationDatabase admin 

Write-Output "Done"