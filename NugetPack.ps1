# relative to script directory
$srcRoot = '.\src'                       
$nuspecRoot = '.\nuspec'

# relative to $srcRoot
[string[]] $buildFiles = 'Utility\Utility.csproj', 'Utility\UtilitySL5.csproj'  
[string[]] $nuspecFiles = 'Utility.nuspec'
$versionFile = 'SharedAssemblyInfo.cs'

$buildConfiguration = 'Release'
$outputPath = "$home\Dropbox\Packages"

Import-Module BuildUtilities

$versionFile = Resolve-Path(Join-Path $srcRoot $versionFile)

$version = Get-Version $versionFile
  
New-Path $outputPath


#foreach($buildFile in $buildFiles)
#{
#  Invoke-Build (Resolve-Path(Join-Path $srcRoot $buildFile)) $buildConfiguration
#}

foreach($nuspecFile in $nuspecFiles)
{
  New-Package (Resolve-Path(Join-Path $nuspecRoot $nuspecFile)) $version $outputPath
}

Remove-Module BuildUtilities
