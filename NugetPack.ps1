
$srcRoot = '.\src'                       # relative to script directory
$buildFile = 'Utility.sln'            # relative to $srcRoot
$buildConfiguration = 'Release'
$nuspecFile = 'Utility\Utility.nuspec'   # relative to $srcRoot
$versionFile = 'SharedAssemblyInfo.cs'   # relative to $srcRoot
$outputPath = "$home\Dropbox\Packages"

Import-Module BuildUtilities

$versionFile =Resolve-Path(Join-Path $srcRoot $versionFile)
$buildFile = Resolve-Path(Join-Path $srcRoot $buildFile)
$nuspecFile = Resolve-Path(Join-Path $srcRoot $nuspecFile)

$version = Get-Version $versionFile
  
New-Path $outputPath
#Invoke-Build $buildFile $buildConfiguration
New-Package $nuspecFile $version $outputPath

Remove-Module BuildUtilities
