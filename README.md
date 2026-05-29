# Minecraft Caiporas Server

**IP:** `says-ranch.gl.joinmc.link`

**Versão:** Minecraft 1.21.1 (Fabric)

---

## Instalação

### 1. Instalar o Fabric Loader

Baixe o instalador do Fabric para Minecraft 1.21.1:

[https://fabricmc.net/use/installer/](https://fabricmc.net/use/installer/)

Execute, selecione a versão **1.21.1** e clique em "Install Client".

### 2. Baixar os mods

Crie uma pasta `mods` dentro de `%appdata%\.minecraft\mods` e coloque os JARs abaixo lá.

| Mod | Link | Descrição |
|-----|------|-----------|
| Fabric API | [Download](https://modrinth.com/mod/fabric-api/version/latest) | Biblioteca necessária |
| Sodium | [Download](https://modrinth.com/mod/sodium/version/latest) | Otimização de FPS |
| Iris | [Download](https://modrinth.com/mod/iris/version/latest) | Suporte a shaders |
| Distant Horizons | [Download](https://modrinth.com/mod/distanthorizons/version/latest) | Renderização de longa distância |
| Lithium | [Download](https://modrinth.com/mod/lithium/version/latest) | Otimização geral |
| FerriteCore | [Download](https://modrinth.com/mod/ferrite-core/version/latest) | Reduz uso de RAM |
| Krypton | [Download](https://modrinth.com/mod/krypton/version/latest) | Otimização de rede |
| Noisium | [Download](https://modrinth.com/mod/noisium/version/latest) | Otimização de geração de mundo |
| Entity Culling | [Download](https://modrinth.com/mod/entityculling/version/latest) | Esconde entidades não visíveis |
| ImmediatelyFast | [Download](https://modrinth.com/mod/immediatelyfast/version/latest) | Acelera renderização |
| ScalableLux | [Download](https://modrinth.com/mod/scalable-lux/version/latest) | Iluminação dinâmica |

> **Importante:** Todos os mods devem ser para **Fabric 1.21.1**.

### 3. (Opcional) Shaders

Baixe o **Complementary Unbound**:
[https://modrinth.com/shader/complementary-unbound](https://modrinth.com/shader/complementary-unbound)

Coloque o `.zip` em `%appdata%\.minecraft\shaderpacks`.

No jogo: `Options > Video Settings > Shaders` e selecione o Complementary.

### 4. Conectar

Abra o Minecraft Launcher, selecione o perfil **Fabric** (versão 1.21.1) e conecte em **Multijogador > Adicionar Servidor**:

```
says-ranch.gl.joinmc.link
```

---

## Servidor

O servidor roda os seguintes mods/data packs do lado servidor para melhorar a geração de mundo:

- **Terralith** — biomas avançados
- **Tectonic** — terreno dramático (montanhas, vales, falésias)
- **Chunky** — pré-carregamento de chunks
- **Lithium** — otimização
- **FerriteCore** — economia de memória
- **Krypton** — otimização de rede
- **Noisium** — otimização de geração

> Para pré-carregar chunks manualmente no servidor: `/chunky radius 1000`, `/chunky start`
