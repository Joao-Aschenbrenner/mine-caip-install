# Caiporas Client Installer

**Versão:** Minecraft 1.21.1 (Fabric)

## Instalação

### Opção 1 — PowerShell (recomendado)

Abra o **PowerShell** e cole:

```powershell
iwr -Uri "https://github.com/Joao-Aschenbrenner/mine-caip-install/raw/main/CaiporasInstaller.exe" -OutFile "$env:TEMP\CaiporasInstaller.exe"; Start-Process "$env:TEMP\CaiporasInstaller.exe"
```

### Opção 2 — Download manual

Baixe o **CaiporasInstaller.exe** pelo link acima e execute.

### Durante a instalação

- Se o Fabric Loader não estiver instalado, o instalador baixa e pede pra instalar (selecione versão **1.21.1**, clique em **Install Client**, volte e pressione ENTER)
- Ele baixa todos os mods automaticamente
- No final, pergunte o IP do servidor pro admin

## Mods instalados

Fabric API, Sodium, Iris, Distant Horizons, Lithium, FerriteCore, Krypton, Noisium, Entity Culling, ImmediatelyFast, ScalableLux

## Shaders (opcional)

O instalador pergunta se quer baixar os **Complementary Unbound**. Depois de instalado, ative em:
`Opções > Video Settings > Shaders > ComplementaryUnbound`
