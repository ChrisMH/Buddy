[string[]] $buildFiles = '.\src\Buddy\Buddy45.csproj', 
                         '.\src\Buddy\Buddy451.csproj',  
                         '.\src\Buddy\Buddy452.csproj'
[string[]] $nuspecFiles = '.\nuspec\Buddy.nuspec'
$versionFile = '.\src\SharedAssemblyInfo.cs'

$buildConfiguration = 'Release'
$outputPath = Join-Path $HOME "Dropbox\Packages"

Import-Module BuildUtilities

$version = Get-Version (Resolve-Path $versionFile)
  
New-Path $outputPath

#foreach($buildFile in $buildFiles)
#{
#  Invoke-Build (Resolve-Path $buildFile) $buildConfiguration
#}

foreach($nuspecFile in $nuspecFiles)
{
  New-Package (Resolve-Path $nuspecFile) $version $outputPath
}

Remove-Module BuildUtilities
