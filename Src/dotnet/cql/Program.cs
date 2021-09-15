using gen.cql.cqlframework.og;
using System;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System.IO;

namespace cql.cqlframework.og
{
    public class Program
    {
        static void Main(string[] args)
        {
            string inputFile = null;

            if (args.Length > 0)
            {
                inputFile = args[0];
            }

            if (inputFile == null)
            {
                Console.WriteLine("cql file name required");
                return;
            }

            if (!File.Exists(inputFile))
            {
                Console.WriteLine("cql file name does not exist");
                return;
            }

            ICharStream charStream = CharStreams.fromPath(inputFile);            
            cqlLexer lexer = new cqlLexer(charStream);
            CommonTokenStream tokenStream = new CommonTokenStream(lexer);
            cqlParser parser = new cqlParser(tokenStream);
            parser.BuildParseTree = true;
            IParseTree tree = parser.library();

            // show tree in text form
            Console.WriteLine(tree.ToStringTree(parser));
        }
    }
}
