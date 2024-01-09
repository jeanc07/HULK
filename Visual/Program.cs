using System.Runtime.Serialization.Formatters;

Lexicon lex = new Lexicon();

List<string> lines = new List<string>();
System.Console.WriteLine("Write your lines of code below:");
System.Console.WriteLine("Write a c to compile the code");
string line = "";
do
{
    line = System.Console.ReadLine();   
    if(line == "c")
        break;
    if (line != null || line != "" || line != "c")
    {
        lines.Add(line);
    }else if(line != "c")
    {
        System.Console.WriteLine("Write the code correctly. Line code empty");
    }
} while (true);

for (int i = 0; i < lines.Count; i++)
{
    List<string> listToken = lex.divideElements(lines.ElementAt(i));
    if(listToken.Count > 0)
    { 
        SyntacticAnalisis sa = new SyntacticAnalisis();
        bool syntaxisAnalysis = sa.syntaxisAnalysis(listToken,lex);
        System.Console.WriteLine(syntaxisAnalysis);
        Semantic sea = new Semantic();
        bool semanticAnalysis = sea.SemanticAnalysis(listToken,lex);
        System.Console.WriteLine(semanticAnalysis);
        if (!syntaxisAnalysis && !semanticAnalysis)
        {
            List<string> execute = PredFunction.Executeline(listToken,lex);
            if (execute.Count == 1)
            {
                System.Console.WriteLine(execute[0]);
            }
        }else if (syntaxisAnalysis)
        {
            System.Console.WriteLine($"Syntactic Error in line {i}. Try to write correctly the code line");
        }else
        {
            System.Console.WriteLine($"Semantic Error in line {i}. Try to write correctly the code line");
        }
        
    }else
        System.Console.WriteLine($"Lexic Error in line {i}. Try to write correctly the code line");
}
