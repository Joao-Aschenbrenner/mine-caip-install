using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

/// <summary>
/// Caiporas Installer
///
/// Prepara o Minecraft de um amigo para entrar no servidor: confere o Fabric
/// Loader, baixa os mods e baixa os shaders.
///
/// A lista de mods vem do manifesto publico do repositorio. A copia que
/// esta embutida aqui so e usada quando o manifesto nao pode ser lido, para
/// que o instalador nunca deixe a pessoa na mão sem internet.
/// </summary>
public class CaiporasInstaller
{
    [DllImport("kernel32.dll")]
    static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    const int SW_HIDE = 0;

    const string ManifestUrl =
        "https://raw.githubusercontent.com/Joao-Aschenbrenner/mine-caip-install/main/client-mods.json";

    const string FabricInstallerUrl =
        "https://maven.fabricmc.net/net/fabricmc/fabric-installer/1.0.1/fabric-installer-1.0.1.exe";

    const string MinecraftVersion = "1.21.1";
    const string FabricLoaderVersion = "0.16.10";
    const string ProfileName = "fabric-loader-1.21.1";

    class Mod
    {
        public string Id = "";
        public string Version = "";
        public string File = "";
        public string Url = "";
    }

    class Manifest
    {
        public string MinecraftVersion = "1.21.1";
        public string FabricLoaderVersion = "0.16.10";
        public string ProfileName = "fabric-loader-1.21.1";
        public List<Mod> Mods = new List<Mod>();
        public Mod Shaders;
        public bool FromNetwork;
    }

    public static void Main(string[] args)
    {
        string serverAddress = "";
        bool hide = false;

        foreach (string argument in args)
        {
            string value = argument.Trim();
            if (value.StartsWith("/hide") || value == "--hide")
            {
                hide = true;
            }
            else if (!value.StartsWith("/") && !value.StartsWith("--") && serverAddress == "")
            {
                serverAddress = value;
            }
        }

        if (hide)
        {
            ShowWindow(GetConsoleWindow(), SW_HIDE);
        }

        Console.WriteLine();
        Console.WriteLine("=== Caiporas Installer ===");
        Console.WriteLine("Preparando o Minecraft para o servidor Caiporas.");
        Console.WriteLine();

        string mcDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            ".minecraft"
        );
        string modsDir = Path.Combine(mcDir, "mods");
        string shadersDir = Path.Combine(mcDir, "shaderpacks");
        Directory.CreateDirectory(modsDir);
        Directory.CreateDirectory(shadersDir);

        // ------------------------------------------------------------------
        // A lista de mods
        // ------------------------------------------------------------------
        Console.WriteLine("[1/4] Lendo a lista de mods...");
        Manifest manifest = LoadManifest();
        if (manifest.FromNetwork)
        {
            Console.WriteLine("      Lista atualizada do repositorio.");
        }
        else
        {
            Console.WriteLine("      Sem acesso a lista online. Usando a copia local.");
        }
        Console.WriteLine();

        // ------------------------------------------------------------------
        // O Fabric Loader
        // ------------------------------------------------------------------
        Console.WriteLine("[2/4] Verificando o Fabric Loader...");
        if (HasFabricProfile(mcDir, manifest.MinecraftVersion))
        {
            Console.WriteLine("      Fabric Loader encontrado.");
        }
        else
        {
            Console.WriteLine("      Fabric Loader para o Minecraft " + manifest.MinecraftVersion + " nao encontrado.");
            Console.WriteLine("      Deseja instalar agora? (S/N): ");
            ConsoleKeyInfo key = Console.ReadKey();
            Console.WriteLine();
            if (key.Key == ConsoleKey.S || key.Key == ConsoleKey.Enter)
            {
                InstallFabric(manifest.MinecraftVersion);
            }
            else
            {
                Console.WriteLine("      Instale o Fabric Loader antes de jogar.");
                Console.WriteLine();
            }
        }
        Console.WriteLine();

        // ------------------------------------------------------------------
        // Os mods
        // ------------------------------------------------------------------
        Console.WriteLine("[3/4] Baixando os mods...");
        int installed = 0;
        int kept = 0;
        List<string> failedMods = new List<string>();

        for (int i = 0; i < manifest.Mods.Count; i++)
        {
            Mod mod = manifest.Mods[i];
            string filePath = Path.Combine(modsDir, mod.File);

            if (File.Exists(filePath) && new FileInfo(filePath).Length > 10240)
            {
                Console.WriteLine("      [" + (i + 1) + "/" + manifest.Mods.Count + "] " + mod.Id + " - ja existe");
                kept++;
                continue;
            }

            Console.Write("      [" + (i + 1) + "/" + manifest.Mods.Count + "] " + mod.Id + "... ");
            if (Download(mod.Url, filePath))
            {
                Console.WriteLine("OK");
                installed++;
            }
            else
            {
                Console.WriteLine("ERRO");
                failedMods.Add(mod.Id + " (" + mod.File + ")");
            }
        }

        Console.WriteLine();
        Console.WriteLine("      " + installed + " baixados, " + kept + " ja estavam, " + failedMods.Count + " com falha.");

        if (failedMods.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine("      Nao foi possivel baixar:");
            foreach (string failed in failedMods)
            {
                Console.WriteLine("        - " + failed);
            }
            Console.WriteLine();
            Console.WriteLine("      O servidor de arquivos estava instavel. Rode o instalador de novo");
            Console.WriteLine("      para tentar pegar o que faltou. Se o aviso persists, o mod pode ser");
            Console.WriteLine("      baixado na mao pelo site do Modrinth.");
        }
        Console.WriteLine();

        // ------------------------------------------------------------------
        // Os shaders
        // ------------------------------------------------------------------
        Console.WriteLine("[4/4] Shaders (opcional)...");
        if (manifest.Shaders != null)
        {
            string shaderPath = Path.Combine(shadersDir, manifest.Shaders.File);
            if (File.Exists(shaderPath) && new FileInfo(shaderPath).Length > 10240)
            {
                Console.WriteLine("      Complementar Unbound ja esta instalado.");
            }
            else
            {
                Console.WriteLine("      Baixar os shaders Complementar Unbound? (S/N): ");
                ConsoleKeyInfo key = Console.ReadKey();
                Console.WriteLine();
                if (key.Key == ConsoleKey.S)
                {
                    Console.Write("      Baixando... ");
                    if (Download(manifest.Shaders.Url, shaderPath))
                    {
                        Console.WriteLine("OK");
                    }
                    else
                    {
                        Console.WriteLine("ERRO - baixe em https://modrinth.com/shader/complementary-unbound");
                    }
                }
            }
        }
        Console.WriteLine();

        // ------------------------------------------------------------------
        // O endereco do servidor
        // ------------------------------------------------------------------
        if (serverAddress == "")
        {
            Console.WriteLine("Digite o endereco do servidor: ");
            serverAddress = Console.ReadLine();
            serverAddress = (serverAddress ?? "").Trim();
        }

        // ------------------------------------------------------------------
        // O que fazer agora
        // ------------------------------------------------------------------
        Console.WriteLine();
        Console.WriteLine("=== Instalacao concluida! ===");
        Console.WriteLine();
        Console.WriteLine("  1. Abra o Minecraft Launcher");
        Console.WriteLine("  2. Escolha o perfil \"" + manifest.ProfileName + "\"");
        Console.WriteLine("  3. Clique em Play");
        Console.WriteLine("  4. No jogo: Multiplayer > Add Server");
        if (serverAddress != "")
        {
            Console.WriteLine("  5. Endereco: " + serverAddress);
        }
        Console.WriteLine();
        Console.WriteLine("  Se usar shaders: Opcoes > Video Settings > Shaders > ComplementarUnbound");
        Console.WriteLine();
        Console.WriteLine("Pressione ENTER para sair...");
        Console.ReadLine();
    }

    /// <summary>
    /// Procura um perfil do Fabric Loader para a versao do jogo.
    /// </summary>
    static bool HasFabricProfile(string mcDir, string minecraftVersion)
    {
        string versionsDir = Path.Combine(mcDir, "versions");
        if (!Directory.Exists(versionsDir)) return false;

        foreach (string directory in Directory.GetDirectories(versionsDir))
        {
            string name = Path.GetFileName(directory);
            if (name.StartsWith("fabric-loader-") && name.EndsWith("-" + minecraftVersion))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Baixa e abre o instalador do Fabric.
    /// </summary>
    static void InstallFabric(string minecraftVersion)
    {
        string installerPath = Path.Combine(Path.GetTempPath(), "fabric-installer.exe");
        Console.WriteLine("      Baixando o Fabric Installer...");
        if (!Download(FabricInstallerUrl, installerPath))
        {
            Console.WriteLine("      Nao foi possivel baixar o instalador.");
            Console.WriteLine("      Baixe em " + FabricInstallerUrl);
            return;
        }

        Console.WriteLine();
        Console.WriteLine("      Na janela do Fabric Installer:");
        Console.WriteLine("        - selecione a versao do jogo: " + minecraftVersion);
        Console.WriteLine("        - clique em Install Client");
        Console.WriteLine("        - feche a janela");
        Console.WriteLine();
        Process.Start(installerPath);
        Console.WriteLine("      Pressione ENTER aqui quando terminar...");
        Console.ReadLine();
    }

    /// <summary>
    /// Baixa um arquivo.
    ///
    /// O servidor de arquivos do Modrinth migra os arquivos antigos e, durante
    /// a migracao, a leitura falha. Por isso a tentativa e repetida antes de
    /// desistir. O download so e aceito quando o tamanho faz sentido, para que
    /// uma pagina de erro do servidor de arquivos nao vire um mod.
    /// </summary>
    static bool Download(string url, string destination)
    {
        for (int attempt = 1; attempt <= 4; attempt++)
        {
            try
            {
                string temporary = destination + ".part";
                WebClient client = new WebClient();
                client.Headers.Add("User-Agent", "CaiporasInstaller/2.0");
                client.DownloadFile(url, temporary);

                FileInfo info = new FileInfo(temporary);
                if (info.Length < 10240)
                {
                    File.Delete(temporary);
                }
                else
                {
                    if (File.Exists(destination)) File.Delete(destination);
                    File.Move(temporary, destination);
                    return true;
                }
            }
            catch (Exception)
            {
                /* a proxima tentativa cuida */
            }

            if (attempt < 4)
            {
                System.Threading.Thread.Sleep(3000 * attempt);
            }
        }
        return false;
    }

    /// <summary>
    /// Le o manifesto publico. A copia embutida e usada quando a rede falha.
    /// </summary>
    static Manifest LoadManifest()
    {
        Manifest manifest = new Manifest();

        try
        {
            WebClient client = new WebClient();
            client.Headers.Add("User-Agent", "CaiporasInstaller/2.0");
            string json = client.DownloadString(ManifestUrl);
            Manifest parsed = ParseManifest(json);
            if (parsed.Mods.Count > 0)
            {
                parsed.FromNetwork = true;
                return parsed;
            }
        }
        catch (Exception)
        {
            /* a copia embutida abaixo cobre a falta de rede */
        }

        manifest.Mods = FallbackMods();
        manifest.Shaders = new Mod();
        manifest.Shaders.Id = "complementary-unbound";
        manifest.Shaders.File = "ComplementaryUnbound_r5.8.1.zip";
        manifest.Shaders.Url =
            "https://cdn.modrinth.com/data/R6NEzAwj/versions/VMHXIk50/ComplementaryUnbound_r5.8.1.zip";
        return manifest;
    }

    /// <summary>
    /// Le o manifesto sem usar uma biblioteca de JSON. O formato e fixo e
    /// pequeno, entao a leitura direta resolve.
    /// </summary>
    static Manifest ParseManifest(string json)
    {
        Manifest manifest = new Manifest();

        string minecraftVersion = ReadString(json, "minecraftVersion");
        if (minecraftVersion != "") manifest.MinecraftVersion = minecraftVersion;

        string loaderVersion = ReadString(json, "fabricLoaderVersion");
        if (loaderVersion != "") manifest.FabricLoaderVersion = loaderVersion;

        string profile = ReadString(json, "profileName");
        if (profile != "") manifest.ProfileName = profile;

        int modsStart = json.IndexOf("\"mods\"");
        if (modsStart >= 0)
        {
            int open = json.IndexOf('[', modsStart);
            int close = open >= 0 ? json.IndexOf(']', open) : -1;
            if (open >= 0 && close > open)
            {
                string array = json.Substring(open, close - open + 1);
                int index = 0;
                while (true)
                {
                    int objectStart = array.IndexOf('{', index);
                    if (objectStart < 0) break;
                    int objectEnd = array.IndexOf('}', objectStart);
                    if (objectEnd < 0) break;

                    string block = array.Substring(objectStart, objectEnd - objectStart + 1);
                    Mod mod = new Mod();
                    mod.Id = ReadString(block, "id");
                    mod.Version = ReadString(block, "version");
                    mod.File = ReadString(block, "file");
                    mod.Url = ReadString(block, "url");
                    if (mod.Url != "") manifest.Mods.Add(mod);

                    index = objectEnd + 1;
                }
            }
        }

        int shadersStart = json.IndexOf("\"shaders\"");
        if (shadersStart >= 0)
        {
            int open = json.IndexOf('{', shadersStart);
            int close = open >= 0 ? json.IndexOf('}', open) : -1;
            if (open >= 0 && close > open)
            {
                string block = json.Substring(open, close - open + 1);
                Mod shaders = new Mod();
                shaders.Id = ReadString(block, "id");
                shaders.Version = ReadString(block, "version");
                shaders.File = ReadString(block, "file");
                shaders.Url = ReadString(block, "url");
                if (shaders.Url != "") manifest.Shaders = shaders;
            }
        }

        return manifest;
    }

    /// <summary>
    /// Le um valor de texto de um objeto do manifesto.
    /// </summary>
    static string ReadString(string text, string key)
    {
        Match match = Regex.Match(text, "\"" + key + "\"\\s*:\\s*\"([^\"]*)\"");
        return match.Success ? match.Groups[1].Value : "";
    }

    /// <summary>
    /// A lista de mods que fica embutida no instalador.
    /// </summary>
    static List<Mod> FallbackMods()
    {
        List<Mod> mods = new List<Mod>();
        mods.Add(NewMod("fabric-api", "0.116.12+1.21.1", "fabric-api-0.116.12+1.21.1.jar",
            "https://cdn.modrinth.com/data/P7dR8mSH/versions/Lwt6YYHL/fabric-api-0.116.12%2B1.21.1.jar"));
        mods.Add(NewMod("sodium", "0.6.13+mc1.21.1", "sodium-fabric-0.6.13+mc1.21.1.jar",
            "https://cdn.modrinth.com/data/AANobbMI/versions/u1OEbNKx/sodium-fabric-0.6.13%2Bmc1.21.1.jar"));
        mods.Add(NewMod("iris", "1.8.8+mc1.21.1", "iris-fabric-1.8.8+mc1.21.1.jar",
            "https://cdn.modrinth.com/data/YL57xq9U/versions/zsoi0dso/iris-fabric-1.8.8%2Bmc1.21.1.jar"));
        mods.Add(NewMod("distanthorizons", "3.0.3-b", "DistantHorizons-3.0.3-b-1.21.1-fabric-neoforge.jar",
            "https://cdn.modrinth.com/data/uCdwusMi/versions/oYXIfeus/DistantHorizons-3.0.3-b-1.21.1-fabric-neoforge.jar"));
        mods.Add(NewMod("immediatelyfast", "1.6.10+1.21.1", "ImmediatelyFast-Fabric-1.6.10+1.21.1.jar",
            "https://cdn.modrinth.com/data/5ZwdcRci/versions/yHztKxR5/ImmediatelyFast-Fabric-1.6.10%2B1.21.1.jar"));
        mods.Add(NewMod("entityculling", "1.10.2", "entityculling-fabric-1.10.2-mc1.21.1.jar",
            "https://cdn.modrinth.com/data/NNAgCjsB/versions/FX8eJJiJ/entityculling-fabric-1.10.2-mc1.21.1.jar"));
        mods.Add(NewMod("lithium", "0.15.3+mc1.21.1", "lithium-fabric-0.15.3+mc1.21.1.jar",
            "https://cdn.modrinth.com/data/gvQqBUqZ/versions/XQJtuOTA/lithium-fabric-0.15.3%2Bmc1.21.1.jar"));
        mods.Add(NewMod("ferritecore", "7.0.3", "ferritecore-7.0.3-fabric.jar",
            "https://cdn.modrinth.com/data/uXXizFIs/versions/sOzRw3CG/ferritecore-7.0.3-fabric.jar"));
        mods.Add(NewMod("krypton", "0.2.8", "krypton-0.2.8.jar",
            "https://cdn.modrinth.com/data/fQEb0iXm/versions/Acz3ttTp/krypton-0.2.8.jar"));
        mods.Add(NewMod("noisium", "2.3.0+mc1.21-1.21.1", "noisium-fabric-2.3.0+mc1.21-1.21.1.jar",
            "https://cdn.modrinth.com/data/KuNKN7d2/versions/4sGQgiu2/noisium-fabric-2.3.0%2Bmc1.21-1.21.1.jar"));
        mods.Add(NewMod("scalablelux", "0.1.0.1+fabric", "ScalableLux-0.1.0.1+fabric.d0d58ab-all.jar",
            "https://cdn.modrinth.com/data/Ps1zyz6x/versions/Yx1tgJMI/ScalableLux-0.1.0.1%2Bfabric.d0d58ab-all.jar"));
        return mods;
    }

    static Mod NewMod(string id, string version, string file, string url)
    {
        Mod mod = new Mod();
        mod.Id = id;
        mod.Version = version;
        mod.File = file;
        mod.Url = url;
        return mod;
    }
}
