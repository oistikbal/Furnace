using CppSharp;

namespace FurnaceAPIGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string apiPath = string.Empty;
            string outputPath = string.Empty;
            string binPath = string.Empty;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Equals("-apiPath", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
                {
                    apiPath = args[i + 1];
                    i++;
                }
                else if (args[i].Equals("-outputPath", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
                {
                    outputPath = args[i + 1];
                    i++;
                }

                else if (args[i].Equals("-binPath", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
                {
                    binPath = args[i + 1];
                    i++;
                }
            }

            if (apiPath == string.Empty) 
            {
                Console.WriteLine("Error: The -apiPath argument is required.");
                Environment.Exit(1);
            }

            if (outputPath == string.Empty) 
            {
                Console.WriteLine("Error: The -outputPath argument is required.");
                Environment.Exit(1);
            }

            if (binPath == string.Empty)
            {
                Console.WriteLine("Error: The -binPath argument is required.");
                Environment.Exit(1);
            }

            Console.WriteLine($"---Generating API---");
            Console.WriteLine($"API Path: {apiPath}");
            Console.WriteLine($"Output Path: {outputPath}");
            Console.WriteLine($"Bin Path: {binPath}");
            Console.WriteLine($"--------------------");


            ConsoleDriver.Run(new FurnaceAPILibrary(apiPath, outputPath, binPath));
        }
    }
}
