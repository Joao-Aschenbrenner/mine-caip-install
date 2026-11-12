<#
.SYNOPSIS
    Compila o instalador do cliente.

.DESCRIPTION
    O instalador e um programa de console para o .NET Framework. O
    compilador que vem com o Windows e suficiente e produz um executavel
    pequeno que roda em qualquer Windows, sem pedir o .NET moderno.

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File build.ps1
#>

[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$source = Join-Path $scriptDir 'CaiporasInstaller.cs'
$output = Join-Path $scriptDir 'CaiporasInstaller.exe'

# O compilador do .NET Framework 4 vem com o Windows.
$candidates = @(
    'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe',
    'C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe'
)
$compiler = $candidates | Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1

if (-not $compiler) {
    throw "O compilador do .NET Framework nao foi encontrado."
}

$framework = Split-Path -Parent $compiler

Write-Output "Compilando com $compiler"
& $compiler /nologo /target:exe /optimize+ "/out:$output" `
    "/reference:$framework\mscorlib.dll" `
    "/reference:$framework\System.dll" `
    "/reference:$framework\System.Net.dll" `
    $source

if ($LASTEXITCODE -ne 0) {
    throw "A compilacao falhou."
}

$info = Get-Item -LiteralPath $output
Write-Output "[OK] $output ($($info.Length) bytes)"
