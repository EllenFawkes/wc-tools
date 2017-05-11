using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace speedDials2ccs
{
    class Program
    {
        const string SPEED_DIALS_XML = "phoneButtons.xml";
        const string PHONE_BOOK_XML = "phoneBook.xml";

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

        private static void run(string inputDir, string outputFile)
        {
            if (!checkFiles(inputDir))
                return;
            try
            {
                IptcParser parser = new IptcParser();
                CCSBuilder builder = new CCSBuilder();
                Console.WriteLine("Parsing XML...");
                XDocument doc = parser.parse(inputDir + "/" + SPEED_DIALS_XML, inputDir + "/" + PHONE_BOOK_XML);
                Console.WriteLine("Building CCS...");
                builder.build(doc, outputFile);
                Console.WriteLine("Conversion OK!");
            }
            catch (Exception e)
            {

            }
        }

        private static bool checkFiles(string inputDir)
        {
            bool isOk = true;
            string[] files = { SPEED_DIALS_XML, PHONE_BOOK_XML };
            foreach (string file in files)
            {
                Console.Write("Checking file " + file + "...");
                if (!File.Exists(inputDir + "/" + file))
                {
                    isOk = false;
                    Console.Write("\t ERROR! File not found in input directory: " + inputDir);
                }
                else
                    Console.Write("\t OK!");
                Console.WriteLine();
            }
            return isOk;
        }

        private static void printHead()
        {
            Console.WriteLine("Convert speeddials (hotline buttons) from IPTC XML to IPTC-K CCS");
            Console.WriteLine("NOTE: This tool converts only AUT and MB tabs!");
            Console.WriteLine();
        }

        private static void printUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("speedDials2ccs <input DIR with XML> <output CCS file>");
            Console.WriteLine("In import directory searches XML files " + SPEED_DIALS_XML + " and " + PHONE_BOOK_XML);
        }
    }
}
