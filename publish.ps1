Push-Location $PSScriptRoot
try {
    if (!$Nuget_Key) { throw "Nuget_Key is not set"}
    dotnet tool restore
    dotnet cleanup -y
    dotnet pack ./source/TimeWarp.Fixie/TimeWarp.Fixie.csproj -c Release
    dotnet nuget push **/*.nupkg --source https://api.nuget.org/v3/index.json --api-key $Nuget_Key    
}
finally {
    Pop-Location
}
