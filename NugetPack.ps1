$srcRoot = '.\src'                     # relative to script directory
$versionFile = 'SharedAssemblyInfo.cs' # relative to $srcRoot
$outputPath = "$home\Dropbox\Packages"

Import-Module "$home\Dropbox\Scripts\NugetUtilities.psm1"

New-Path $outputPath

$version = Get-Version (Join-Path $srcRoot $versionFile)

Pack-Project Utility $srcRoot $version $outputPath

