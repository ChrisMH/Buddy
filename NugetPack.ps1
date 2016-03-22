[string[]] $buildFiles = '.\src\Buddy40\Buddy40.csproj', 
                         '.\src\Buddy45\Buddy45.csproj', 
                         '.\src\Buddy451\Buddy451.csproj',  
                         '.\src\Buddy452\Buddy452.csproj',  
                         '.\src\Buddy46\Buddy46.csproj',  
                         '.\src\Buddy461\Buddy461.csproj'

$versionFile = '.\src\SharedAssemblyInfo.cs'

$outputPath = Join-Path $HOME 'Dropbox\Packages'

Import-Module BuildUtilities

$version = Get-Version (Resolve-Path $versionFile)
  
New-Path $outputPath

foreach($buildFile in $buildFiles)
{
  Invoke-Build (Resolve-Path $buildFile) 'Debug'
  Invoke-Build (Resolve-Path $buildFile) 'Release'
}


New-Package (Resolve-Path '.\nuspec\Buddy.nuspec') $version $outputPath
New-Package (Resolve-Path '.\nuspec\Buddy.Debug.nuspec') "$version-debug" $outputPath

Remove-Module BuildUtilities
