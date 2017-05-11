using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace speedDials2ccs
{
    class Program
    {
        static void Main(string[] args)
        {
            printHead();
            if (args.Length < 2)
            {
                printUsage();
                return;
            }
            run(args[0], args[1]);
        }

        private static void run(string inputFile, string outputFile)
        {
            IptcParser parser = new IptcParser();
            CCSBuilder builder = new CCSBuilder();
            if (!File.Exists(inputFile))
            {
                Console.WriteLine("Input XML file not found!");
                return;
            }
            try
            {
                Console.WriteLine("Parsing input XML...");
                parser.parse(inputFile);
                Console.WriteLine("Building output CCS...");
                builder.build(outputFile, parser.Users);
                Console.WriteLine("Conversion OK!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Conversion ERROR: " + e.Message);
            }
        }

        private static void printHead()
        {
            Console.WriteLine("Convert PhoneBook from IPTC XML to IPTC-K CCS");
            Console.WriteLine();
        }

        private static void printUsage() 
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("phoneBook2ccs <input XML file> <output CCS file>");
        }
    }
}
