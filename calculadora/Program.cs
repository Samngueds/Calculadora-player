using System;
using System.IO;
using System.Drawing;
using System.Threading;
using Console = Colorful.Console;
using NAudio.Wave;

public class Program
{
    enum Operadores { Soma = 1, Multiplicação = 2, Divisão = 3, Subtração = 4, Sair = 5, TocaMusica = 6 }

    static void Main()
    {
        bool continuar = true;
        const int DA = 24;
        const int V = 212;
        const int ID = 255;

        while (continuar)
        {
            MostrarMenuPrincipal(DA, V, ID);

            if (!int.TryParse(Console.ReadLine(), out int input))
            {
                Console.WriteLine("Por favor, digite um número válido.", Color.Red);
                continue;
            }

            switch (input)
            {
                case (int)Operadores.Sair:
                    Console.WriteLine("Você escolheu sair do programa.", Color.Gray);
                    continuar = false;
                    break;

                case (int)Operadores.TocaMusica:
                    ExecutarPlayerMusica();
                    break;

                case >= 1 and <= 4:
                    ExecutarCalculadora((Operadores)input);
                    break;

                default:
                    Console.WriteLine("Por favor, digite ou selecione um operador válido.", Color.Red);
                    break;
            }
        }
    }

    private static void MostrarMenuPrincipal(int da, int v, int id)
    {
        Console.Clear();
        Console.WriteAscii("SAMNS", Color.FromArgb(da, v, id));
        Console.WriteLine("Calculadora feita por samns\n", Color.Aquamarine);
        Console.WriteLine("Digite ou escolha uma operação abaixo: (Número ou nome)\n", Color.Cyan);
        Console.WriteLine("\t1 - Soma  \t\t6 - Tocar Música", Color.Green);
        Console.WriteLine("\t2 - Multiplicação", Color.Blue);
        Console.WriteLine("\t3 - Divisão", Color.Red);
        Console.WriteLine("\t4 - Subtração", Color.Yellow);
        Console.WriteLine("\t5 - Sair\n", Color.Gray);
        Console.Write("Escolha: ", Color.Magenta);
    }

    private static void ExecutarCalculadora(Operadores option)
    {
        Console.WriteLine($"\nO operador escolhido foi: {option}\n", Color.Orange);

        if (!ObterNumeros(out decimal num1, out decimal num2))
            return;

        switch (option)
        {
            case Operadores.Soma:
                Console.WriteLine($"Você escolheu soma. O resultado da operação é: {num1 + num2}", Color.Green);
                break;
            case Operadores.Divisão:
                if (num2 == 0 && num1 ==0)
                    Console.WriteLine("Erro: Divisão por zero não permitida.", Color.Red);
                else
                    Console.WriteLine($"Você escolheu divisão. O resultado da operação é: {num1 / num2}", Color.Red);
                break;
            case Operadores.Multiplicação:
                Console.WriteLine($"Você escolheu multiplicação. O resultado da operação é: {num1 * num2}", Color.Blue);
                break;
            case Operadores.Subtração:
                Console.WriteLine($"Você escolheu subtração. O resultado da operação é: {num1 - num2}", Color.Yellow);
                break;
        }

        Console.WriteLine("\nPressione qualquer tecla para continuar...");
        Console.ReadKey();
    }

    private static bool ObterNumeros(out decimal num1, out decimal num2)
    {
        num1 = num2 = 0;
        Console.WriteLine("Digite os valores a serem calculados:");

        Console.Write("Primeiro Operador: ", Color.LightBlue);
        if (!decimal.TryParse(Console.ReadLine(), out num1))
        {
            Console.WriteLine("Valor inválido para o primeiro operador.", Color.Red);
            return false;
        }

        Console.Write("Segundo Operador: ", Color.LightBlue);
        if (!decimal.TryParse(Console.ReadLine(), out num2))
        {
            Console.WriteLine("Valor inválido para o segundo operador.", Color.Red);
            return false;
        }

        return true;
    }

    private static void ExecutarPlayerMusica()
    {
        string musicFolder = @"Music";

        Console.Clear();
        Console.WriteAscii("MUSIC PLAYER", Color.Purple);
        Console.WriteLine("Bem-vindo ao Player de Música!", Color.Aqua);

        if (!Directory.Exists(musicFolder))
        {
            Console.WriteLine($"A pasta {musicFolder} não existe. Criando pasta...", Color.Yellow);
            Directory.CreateDirectory(musicFolder);
            Console.WriteLine("Pasta criada. Adicione arquivos MP3 e tente novamente.", Color.Green);
            Console.WriteLine("\nPressione qualquer tecla para voltar ao menu...");
            Console.ReadKey();
            return;
        }

        string[] musicFiles = Directory.GetFiles(musicFolder, "*.mp3");
   
        if (musicFiles.Length == 0)
        {
            Console.WriteLine("Nenhuma música encontrada na pasta.", Color.Red);
            Console.WriteLine("\nPressione qualquer tecla para voltar ao menu...");
            Console.ReadKey();
            return;
        }

        MostrarListaMusicas(musicFiles);

        if (!SelecionarMusica(musicFiles.Length, out int escolha))
            return;

        TocarMusica(musicFiles[escolha - 1]);
    }

    private static void MostrarListaMusicas(string[] musicFiles)
    {
        Console.WriteLine("\nEscolha uma música para tocar:", Color.Cyan);
        for (int i = 0; i < musicFiles.Length; i++)
        {
            Console.WriteLine($"{i + 1} - {Path.GetFileName(musicFiles[i])}", Color.LightGreen);
        }
    }

    private static bool SelecionarMusica(int totalMusicas, out int escolha)
    {
        escolha = -1;
        while (escolha < 1 || escolha > totalMusicas)
        {
            Console.Write("\nDigite o número da música: ", Color.Magenta);
            if (int.TryParse(Console.ReadLine(), out escolha) && escolha >= 1 && escolha <= totalMusicas)
                return true;

            Console.WriteLine("Opção inválida. Tente novamente.", Color.Red);
        }
        return false;
    }

    private static void TocarMusica(string musicPath)
    {
        Console.WriteLine($"\nVocê escolheu: {Path.GetFileName(musicPath)}", Color.LightBlue);
        Console.WriteLine("\nTocando música... Pressione qualquer tecla para parar.");

        try
        {
            using (var audioFile = new AudioFileReader(musicPath))
            using (var outputDevice = new WaveOutEvent())
            {
                outputDevice.Init(audioFile);
                outputDevice.Play();
                
                while (outputDevice.PlaybackState == PlaybackState.Playing && !Console.KeyAvailable)
                {
                    Thread.Sleep(100);
                }
                
                outputDevice.Stop();
                Console.WriteLine("Música parada.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao tentar tocar a música: {ex.Message}", Color.Red);
        }

        Console.WriteLine("\nPressione qualquer tecla para voltar ao menu...");
        Console.ReadKey();
    }
}