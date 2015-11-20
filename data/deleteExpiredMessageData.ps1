param([Parameter(Mandatory=$True)][string]$HostName,[Parameter(Mandatory=$True)][string]$UserLogin,[Parameter(Mandatory=$True)][string]$UserPwd)

Write-Output    "Deleting messages that have passed their expiration date"

mongo deleteExpiredMessageData.js --verbose -host "$HostName" -u "$UserLogin" -p "$UserPwd" --authenticationDatabase admin 

Write-Output    "Done"