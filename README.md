# Caiporas Client Installer

**Versão do jogo:** Minecraft 1.21.1 (Fabric)
**Servidor:** `made-harry.gl.joinmc.link`

Um instalador que deixa o Minecraft pronto para entrar no servidor: confere o
Fabric Loader, baixa os mods e baixa os shaders (opcional).

---

## Instalação

### Opção 1 — PowerShell (recomendado)

Abra o **PowerShell** e cole a linha abaixo. Ela baixa o instalador, executa e
passa o endereço do servidor:

```powershell
iwr -Uri "https://github.com/Joao-Aschenbrenner/mine-caip-install/raw/main/CaiporasInstaller.exe" -OutFile "$env:TEMP\CaiporasInstaller.exe"; Start-Process "$env:TEMP\CaiporasInstaller.exe" -ArgumentList "made-harry.gl.joinmc.link"
```

> Se o PowerShell reclamar da execução de scripts, use a **Opção 2**.

### Opção 2 — Download manual

Baixe o [`CaiporasInstaller.exe`](https://github.com/Joao-Aschenbrenner/mine-caip-install/raw/main/CaiporasInstaller.exe)
(clique com o botão direito → *Salvar como*), copie para a pasta
`Downloads` e dê dois cliques.

O Windows pode avisar que o arquivo veio da internet. Clique em
**Mais informações → Executar assim mesmo**: o instalador é um programa
pequeno e aberto, com o código fonte neste repositório.

---

## O que acontece durante a instalação

```
[1/4] Lendo a lista de mods...
[2/4] Verificando o Fabric Loader...
[3/4] Baixando os mods...
[4/4] Shaders (opcional)...
```

**Sobre o Fabric Loader:** se o Minecraft 1.21.1 com Fabric ainda não estiver
instalado, o instalador oferece para baixar o instalador oficial do Fabric.
Na janela dele:

- selecione a versão do jogo **1.21.1**
- clique em **Install Client**
- feche a janela e volte ao instalador, que continua sozinho

**Sobre os mods:** cada mod que já está na pasta é pulado. O que falta é
baixado. Se o servidor de arquivos estiver instável, o instalador tenta de
novo sozinho e, no fim, avisa exatamente o que não conseguiu baixar.

**Sobre os shaders:** o instalador pergunta se você quer o *Complementary
Unbound*. É opcional, mas deixa o jogo bem mais bonito.

---

## Depois de instalar

1. Abra o **Minecraft Launcher**
2. Escolha o perfil **`fabric-loader-1.21.1`**
3. Clique em **Play**
4. No jogo: **Multiplayer → Add Server**
   - endereço: `made-harry.gl.joinmc.link`
5. Entre e divirta-se

### Ativando os shaders

**Opções → Video Settings → Shaders → ComplementaryUnbound**

### Ajustes recomendados

Em **Opções → Video Settings**:

| Ajuste | Valor |
| --- | --- |
| Render Distance | 12 |
| Graphics | Fast (ou Fancy, se a máquina aguentar) |
| Shaders | ComplementaryUnbound |

---

## Os mods que são instalados

| Mod | Versão | Para quê |
| --- | --- | --- |
| Fabric API | 0.116.12+1.21.1 | base obrigatória de quase todo mod de Fabric |
| Sodium | 0.6.13+mc1.21.1 | renderização muito mais rápida |
| Iris | 1.8.8+mc1.21.1 | shaders (precisa do Sodium) |
| Distant Horizons | 3.0.3-b | render distance muito grande |
| ImmediatelyFast | 1.6.10+1.21.1 | animações mais leves |
| Entity Culling | 1.10.2 | esconde mobs atrás de blocos |
| Lithium | 0.15.3+mc1.21.1 | deixa o servidor mais rápido |
| FerriteCore | 7.0.3 | economiza memória |
| Krypton | 0.2.8 | melhora a latência |
| Noisium | 2.3.0+mc1.21.1 | menos poluição no log do jogo |
| ScalableLux | 0.1.0.1+fabric | luzes mais bonitas |

A lista vem de um manifesto único, o
[`client-mods.json`](client-mods.json), que é o mesmo que o painel do
servidor usa. As versões são as mesmas do servidor: se a versão do seu mod
diferir da do servidor, o Fabric recusa a conexão.

---

## Problemas comuns

**"Incompatible mod set" ou "Mod X is missing"**
A versão de algum mod do seu cliente é diferente da do servidor. Rode o
instalador de novo: ele atualiza os mods que estão errados.

**"Failed to verify username" / "Bad login"**
O servidor exige conta Microsoft. Entre com a sua conta normal do Minecraft
(versão oficial, não o cracked).

**"Fabric Loader não encontrado"**
O instalador oferece para instalar. Se a janela do Fabric Installer não
abrir, baixe em
[fabricmc.net/use/installer](https://fabricmc.net/use/installer) e marque
**1.21.1**.

**O download de um mod falhou**
O servidor de arquivos do Modrinth estava instável. Rode o instalador de
novo — ele repete o download sozinho. Se persistir, o mod pode ser baixado
na mão em [modrinth.com](https://modrinth.com) e colocado na pasta
`%APPDATA%\.minecraft\mods`.

**Aparece uma tela azul com "Fabric Loader"**
Falta o Java. Instale o
[Java 21 da Adoptium](https://adoptium.net/temurin/releases/?version=21) e
reinicie o launcher.

---

## Para quem mexer no código

O instalador é um programa de console em C# (`CaiporasInstaller.cs`). Ele lê
o manifesto `client-mods.json` do repositório, com uma cópia embutida para o
caso de o repositório estar fora do ar.

Para recompilar:

```powershell
powershell -ExecutionPolicy Bypass -File build.ps1
```

O `build.ps1` usa o compilador que vem com o Windows
(`C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe`), então o
executável resultante é pequeno e roda em qualquer Windows, sem pedir
instalação de runtime.
