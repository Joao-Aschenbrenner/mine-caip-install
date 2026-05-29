using System;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class CaiporasInstaller
{
    [DllImport("kernel32.dll")]
    static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    const int SW_HIDE = 0;

    public static void Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "/hide")
        {
            IntPtr hWnd = GetConsoleWindow();
            ShowWindow(hWnd, SW_HIDE);
        }

        Console.WriteLine("=== Caiporas Installer ===");
        Console.WriteLine("Instalador de mods para Minecraft 1.21.1 (Fabric)");
        Console.WriteLine();

        string mcDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            ".minecraft"
        );
        string modsDir = Path.Combine(mcDir, "mods");
        string shadersDir = Path.Combine(mcDir, "shaderpacks");

        Directory.CreateDirectory(modsDir);
        Directory.CreateDirectory(shadersDir);

        string fabricProfile = Path.Combine(mcDir, "versions", "fabric-loader-0.18.6-1.21.1");
        if (!Directory.Exists(fabricProfile))
        {
            Console.WriteLine("Fabric Loader nao encontrado.");
            Console.WriteLine("Deseja instalar? (S/N): ");
            ConsoleKeyInfo key = Console.ReadKey();
            Console.WriteLine();
            if (key.Key == ConsoleKey.S || key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine("Baixando Fabric Installer...");
                string installerPath = Path.Combine(Path.GetTempPath(), "fabric-installer.exe");
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFile(
                        "https://maven.fabricmc.net/net/fabricmc/fabric-installer/1.0.1/fabric-installer-1.0.1.exe",
                        installerPath
                    );
                }
                Console.WriteLine("Execute o Fabric Installer, selecione versao 1.21.1 e clique em Install Client.");
                Console.WriteLine("Depois de instalado, feche o instalador e pressione ENTER aqui...");
                Process.Start(installerPath);
                Console.ReadLine();
            }
        }

        Dictionary<string, string> mods = new Dictionary<string, string>();
        mods["Fabric API"] = "https://cdn.modrinth.com/data/P7dR8mSH/versions/Lwt6YYHL/fabric-api-0.116.12%2B1.21.1.jar";
        mods["Sodium"] = "https://cdn.modrinth.com/data/AANobbMI/versions/GsiR0f56/sodium-fabric-0.6.13%2Bmc1.21.1.jar";
        mods["Iris"] = "https://cdn.modrinth.com/data/YL57xBhU/versions/I1zC86Uo/iris-fabric-1.8.8%2Bmc1.21.1.jar";
        mods["Distant Horizons"] = "https://cdn.modrinth.com/data/uC6C4lJC/versions/wEjydoVf/DistantHorizons-3.0.3-b-1.21.1-fabric-neoforge.jar";
        mods["Lithium"] = "https://cdn.modrinth.com/data/gvQqBUqZ/versions/XQJtuOTA/lithium-fabric-0.15.3%2Bmc1.21.1.jar";
        mods["FerriteCore"] = "https://cdn.modrinth.com/data/uXXizFIs/versions/sOzRw3CG/ferritecore-7.0.3-fabric.jar";
        mods["Krypton"] = "https://cdn.modrinth.com/data/fQEb0iXm/versions/Acz3ttTp/krypton-0.2.8.jar";
        mods["Noisium"] = "https://cdn.modrinth.com/data/KuNKN7d2/versions/4sGQgiu2/noisium-fabric-2.3.0%2Bmc1.21-1.21.1.jar";
        mods["Entity Culling"] = "https://cdn.modrinth.com/data/NNAgCjsB/versions/TrcUikAF/entityculling-fabric-1.10.2-mc1.21.1.jar";
        mods["ImmediatelyFast"] = "https://cdn.modrinth.com/data/5ZwdcRci/versions/StXZyPNX/ImmediatelyFast-Fabric-1.6.10%2B1.21.1.jar";
        mods["ScalableLux"] = "https://cdn.modrinth.com/data/GtsBHhDO/versions/lxmkflP7/ScalableLux-0.1.0.1%2Bfabric.d0d58ab-all.jar";

        Console.WriteLine("Baixando mods...");
        using (WebClient wc = new WebClient())
        {
            int i = 1;
            foreach (KeyValuePair<string, string> mod in mods)
            {
                string url = mod.Value;
                string fileName = url.Substring(url.LastIndexOf('/') + 1);
                fileName = Uri.UnescapeDataString(fileName);
                string filePath = Path.Combine(modsDir, fileName);

                if (File.Exists(filePath))
                {
                    Console.WriteLine("[" + i + "/" + mods.Count + "] " + mod.Key + " - ja existe");
                    i++;
                    continue;
                }

                Console.Write("[" + i + "/" + mods.Count + "] " + mod.Key + "... ");
                try
                {
                    wc.DownloadFile(url, filePath);
                    Console.WriteLine("OK");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERRO: " + ex.Message);
                }
                i++;
            }
        }

        string shaderPath = Path.Combine(shadersDir, "ComplementaryUnbound_r5.8.1.zip");
        if (!File.Exists(shaderPath))
        {
            Console.WriteLine();
            Console.WriteLine("Deseja baixar os shaders Complementary Unbound? (S/N): ");
            ConsoleKeyInfo key = Console.ReadKey();
            Console.WriteLine();
            if (key.Key == ConsoleKey.S)
            {
                Console.WriteLine("Baixando shaders...");
                using (WebClient wc = new WebClient())
                {
                    try
                    {
                        wc.DownloadFile(
                            "https://cdn.modrinth.com/data/R6Ne0fPy/versions/VRzLPHPp/ComplementaryUnbound_r5.8.1.zip",
                            shaderPath
                        );
                        Console.WriteLine("Shaders baixados!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ERRO ao baixar shaders: " + ex.Message);
                        Console.WriteLine("Baixe manualmente: https://modrinth.com/shader/complementary-unbound");
                    }
                }
            }
        }

        Console.WriteLine();
        Console.WriteLine("=== Instalacao concluida! ===");
        Console.WriteLine();
        Console.WriteLine("Pergunte o IP do servidor para o admin.");
        Console.WriteLine();
        Console.WriteLine("Pressione ENTER para sair...");
        Console.ReadLine();
    }
}
