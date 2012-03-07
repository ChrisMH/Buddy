[string[]] $buildFiles = '.\src\Utility\Utility.csproj', 
                         '.\src\Utility\UtilitySL5.csproj'  
[string[]] $nuspecFiles = '.\nuspec\Utility.nuspec'
$versionFile = '.\src\SharedAssemblyInfo.cs'

$buildConfiguration = 'Release'
$outputPath = "$home\Dropbox\Packages"

Import-Module BuildUtilities

$versionFile = Resolve-Path $versionFile

$version = Get-Version $versionFile
  
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
