using System.ComponentModel;
/// <summary>
/// Clase implementada para realizar el análisis semántico en la línea ingresada por el usuario.
/// </summary>
public class Semantic
{      
    public bool SemanticAnalysis(List<string> lines, Lexicon lex)
    {
        bool bad = false;
        List<string> currentTokens = lines; 
        string token;       
        
        for (int i = 0; i < lines.Count; i++)
        {
            token = currentTokens.ElementAt(i);
            List<SpecialTokensClass> specials = lex.SpecialTokensClass();
            List<Variable> variables = lex.Variables();
            List<StringToken> stringTokens = lex.StringTokens();
            List<Function> functions = lex.Functions();
            if (lex.Operators().Any(x => x == token && token != "="))
            {
                if (!double.TryParse(currentTokens.ElementAt(i-1),out _) && !variables.Any(x =>x is Variable && ((Variable)x).Nombre == currentTokens.ElementAt(i-1) && (((Variable)x).Tipo == "int" || ((Variable)x).Tipo == null)) && !lex.MathTokens().Contains(currentTokens.ElementAt(i-1))
                && currentTokens.ElementAt(i-1) != ")" && currentTokens.ElementAt(i-1) != "(" && !functions.Any(x => x is Function && x.Nombre == currentTokens.ElementAt(i-1)) && !functions.Any(x => x is Function && ((Function)x).Parametro.Any(x => x.Nombre == currentTokens.ElementAt(i-1))))
                {
                    bad = true;
                }else if (!double.TryParse(currentTokens.ElementAt(i+1),out _) && !variables.Any(x =>x is Variable && ((Variable)x).Nombre == currentTokens.ElementAt(i+1) && (((Variable)x).Tipo == "int" || ((Variable)x).Tipo == null)) && !lex.MathTokens().Contains(currentTokens.ElementAt(i+1))
                && currentTokens.ElementAt(i+1) != ")" && currentTokens.ElementAt(i+1) != "(" && !functions.Any(x => x is Function && x.Nombre == currentTokens.ElementAt(i+1)) && !functions.Any(x => x is Function && ((Function)x).Parametro.Any(x => x.Nombre == currentTokens.ElementAt(i+1))))
                {
                    bad = true;
                }
            }else if (lex.MathTokens().Any(x => x == token))
            {
                int count1 = 0;
                bool check = false;
                int pos = 0;
                for (int j = i; j < lines.Count; j++)
                {
                    if (lines[j] == "(")
                    {
                        count1++;
                    }
                    if(lines[j] == ")")
                    {
                        count1--;
                        check = true;
                    }
                    if (count1 == 0 && check)
                    {
                        pos = j;
                        break;
                    }
                }
                List<string> templist = lines.GetRange(i+1,pos-(i+1));
                for (int j = 0; j < templist.Count; j++)
                {
                    if ((!double.TryParse(templist.ElementAt(j),out _) && !lex.MathTokens().Contains(templist[j])
                    && !lex.Operators().Contains(templist[j]) && !lex.Symbols().Contains(templist[j]))
                    && !variables.Any(x => x is Variable && ((Variable)x).Nombre == templist[j] && ((Variable)x).Tipo == "int")
                    && !functions.Any(x => x is Function && ((Function)x).Parametro.Any(x => x.Nombre == templist[j])))
                    {
                        bad = true;
                    }
                }
            }else if (lex.ConditionOperators().Any(x => x == token))
            {
                bool isbool = false;
                bool isint = false;

                if (double.TryParse(currentTokens.ElementAt(i-1),out _) || variables.Any(x => x is Variable && ((Variable)x).Nombre == currentTokens.ElementAt(i-1) && ((Variable)x).Tipo == "int"))
                {
                    isint = true;
                }else if (stringTokens.Any(x => x is StringToken && ((StringToken)x).Valor == token))
                {
                    isint = false;
                    isbool = false;
                }else if (lex.BoolValues().Contains(currentTokens.ElementAt(i-1)) || variables.Any(x =>x is Variable &&((Variable)x).Nombre == currentTokens.ElementAt(i-1) && (((Variable)x).Tipo == "bool")))
                {
                    isbool = true;
                }

                if (isint)
                {
                    if (!double.TryParse(currentTokens.ElementAt(i+1),out _) && !variables.Any(x => ((Variable)x).Nombre == currentTokens.ElementAt(i+1) && ((Variable)x).Tipo == "int"))
                    {
                        bad = true;
                    }
                }else if (isbool)
                {
                    if (!lex.BoolValues().Contains(currentTokens.ElementAt(i+1)) && !variables.Any(x => ((Variable)x).Nombre == currentTokens.ElementAt(i+1) && (((Variable)x).Tipo == "bool")))
                    {
                        bad = true;
                    }
                }else
                {
                    if(functions.Any(x => x is Function && ((Function)x).Parametro.Any(x => x.Nombre == currentTokens.ElementAt(i+1))))
                        if ((double.TryParse(currentTokens.ElementAt(i+1),out _) || lex.BoolValues().Contains(currentTokens.ElementAt(i+1)) 
                        || variables.Any(x =>x is Variable && ((Variable)x).Nombre == currentTokens.ElementAt(i+1) && (((Variable)x).Tipo == "int" || (((Variable)x).Tipo == "bool")))))
                        {
                            bad = true;
                        }
                }         
            }else if (token == "@")
            {
                if (!double.TryParse(currentTokens.ElementAt(i-1),out _) && !variables.Any(x =>x is Variable &&((Variable)x).Nombre == currentTokens.ElementAt(i-1) && (((Variable)x).Tipo == "int") || ((Variable)x).Tipo == "string")
                && !stringTokens.Any(x => x is StringToken && ((StringToken)x).Valor == currentTokens.ElementAt(i-1)))
                {
                    bad = true;
                }else if (!double.TryParse(currentTokens.ElementAt(i+1),out _) && !variables.Any(x =>x is Variable &&((Variable)x).Nombre == currentTokens.ElementAt(i+1) && (((Variable)x).Tipo == "int") ||((Variable)x).Tipo == "string")
                && !stringTokens.Any(x => x is StringToken && ((StringToken)x).Valor == currentTokens.ElementAt(i+1)))
                {
                    bad = true;
                }
            }
        }
        return bad;
    }
}