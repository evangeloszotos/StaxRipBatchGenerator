namespace StaxRipBatchGenerator
{
    using CommandLine;
    using System;
    using System.IO;
    using Console = Colorful.Console;

    public class Program
    {
        public class Parameters
        {
            [Value(0, MetaName = "SourceDirectory", Required = true)]
            public string SourceDirectory { get; set; }

            [Value(1, MetaName = "DestinationDirectory", Required = true)]
            public string DestinationDirectory { get; set; }

            [Value(2, MetaName = "ProfileName", Required = true)]
            public string ProfileName { get; set; }
        }

        private static void Exit(Action writeExtraMessage = null, int exitCode = 0)
        {
            if (writeExtraMessage != null)
            {
                Console.WriteLine("Error:");
                Console.WriteLine(Environment.NewLine);
                writeExtraMessage.Invoke();
            }

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("Press any key to quit...");
            Console.ReadKey();
            Environment.Exit(exitCode);
        }

        public static void ExitOn(bool condition, string errorMessage)
        {
            if (condition)
            {
                Exit(() =>
                {
                    Console.WriteLine(errorMessage, System.Drawing.Color.Red);
                });
            }
        }

        public static void Execute(Parameters args)
        {
            ExitOn(!Directory.Exists(args.SourceDirectory), "The Specified Source Directory does not exist!");
            ExitOn(!Directory.Exists(args.DestinationDirectory), "The Specified Destination Directory does not exist!");

            string[] filesInDestination = Directory.GetFiles(args.DestinationDirectory);
            ExitOn(filesInDestination.Length > 0, "The Destination Directory is not Empty, please make move all files out of there!");

            string[] filesInSource = Directory.GetFiles(args.SourceDirectory);
            ExitOn(filesInSource.Length == 0, "The Source Directory is empty!");

            Console.WriteLine($"Found {filesInSource.Length} files in Source Directory");

            string staxRipPath = ".\\StaxRip.exe";

            string loadTemplate = $"-LoadTemplate:{args.ProfileName}";
            string command = $"{staxRipPath} {loadTemplate}";

            foreach (string file in filesInSource)
            {
                FileInfo fInfo = new FileInfo(file);
                command += $" -LoadSourceFile:\"{file}\" -SetTargetFile:\"{args.DestinationDirectory}\\{fInfo.Name}\" -AddJob:false,{args.ProfileName}";
            }

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("StaxRip Command Generated Successfully:");
            Console.WriteLine(Environment.NewLine);

            Console.WriteLine(command);
            Exit();
        }

        private static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<Parameters>(args);

            Console.WriteLine("StaxRipBatchGenerator v1.0.0", System.Drawing.Color.LimeGreen);

            if (result.Tag == ParserResultType.NotParsed)
            {
                Exit(null);
            }

            result.WithParsed(Execute);
        }
    }
}