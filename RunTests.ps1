try {
    Push-Location $PSScriptRoot
    Set-Location tests/TimeWarp.Fixie.Tests
    dotnet fixie
}
finally {
    Pop-Location
}
