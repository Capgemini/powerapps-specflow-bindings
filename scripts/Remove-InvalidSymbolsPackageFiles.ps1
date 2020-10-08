[CmdletBinding()]
param (
    [Parameter(Mandatory = $true)]
    [String]
    $SymbolPackageDirectory
)
Add-Type -assembly  System.IO.Compression.FileSystem

Get-ChildItem "$SymbolPackageDirectory/*" -Include *.snupkg | ForEach-Object {
    Write-Host "Removing invalid files from $($_.Name)"
    $oldName = [IO.Path]::GetFileName($_.Name)
    $newName = [IO.Path]::GetFileName([IO.Path]::ChangeExtension($_.Name, "zip"))
    $zipFile = Rename-Item -Path $_.FullName -NewName $newName -PassThru
    $zip = [System.IO.Compression.ZipFile]::Open($zipFile.FullName, [IO.Compression.ZipArchiveMode]::Update)
    
    $allowedExtensions = @('.pdb', '.nuspec', '.xml', '.psmdcp', '.rels', '.p7s')
    ($zip.Entries | Where-Object { $allowedExtensions -notcontains [IO.Path]::GetExtension($_.Name) }) | ForEach-Object { 
        Write-Host "Removing $($_.Name)"
        $_.Delete() 
    }
    
    $zip.Dispose()
    
    Rename-Item -Path $zipFile.FullName -NewName $oldName
}
