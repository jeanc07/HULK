using ParsingLibrary;
using System.Data;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Serialization;
using System.Security.AccessControl;
using System.Xml.XPath;
/// <summary>
/// Clase que implementa todas las funcionalidades del lenguaje HULK.
/// </summary>
public class PredFunction
{   
    private static bool mathexception;
    //private static bool makeclear = true;
    /// <summary>
    /// Método con el cual se resuelven las operaciones matemáticas implementadas por el usuario.
    /// </summary>
    /// <param name="tokens"></Combinada ingresada por el usuario>
    /// <returns></Resultado de la operación matemática ingresada por el usuario>
    public static double doArithmetic(List<string> tokens, Lexicon lex)
    {
        double result = 0;
        double token1 = -1;
        double token2 = -1;
        double number1 = 0;
        double number2 = 0;
        bool exist = false;
        int pos = 0;
        List<SpecialTokensClass> tokenlist = lex.SpecialTokensClass();
        if (tokens.Count == 1)
        {
            if(lex.Variables().Any(x => x.Nombre == tokens[0]))
            {
                Variable temp = (Variable)lex.Variables().Where(x => x.Nombre == tokens[0]).ElementAt(0);
                double a = double.Parse(temp.Valor);
                return a;
            }else
                return Convert.ToDouble(tokens[0]);
        }
        if(tokens.ElementAt(0) == "(" && tokens.ElementAt(tokens.Count-1) == ")")
        {
            tokens.RemoveAt(0);
            tokens.RemoveAt(tokens.Count-1);
        }

        int cont1 = 0;
        int cont2 = 0;
        List<string> tempTokens = new List<string>();
        for (int i = 0; i < tokens.Count; i++)
        {
            if(tokens.ElementAt(i) == "(")
            {     
                if(cont1 == 0)                                
                    pos = i;
                cont1++;                    
            }

            if(tokens.ElementAt(i) == ")")
            {
                cont2++;
                if(cont1 == cont2)
                {                        
                    tempTokens.AddRange(tokens.GetRange(pos, (i-pos)+1));
                    tokens.RemoveRange(pos, (i-pos)+1);
                    break;
                }
            }                                          
        }    

        if(tempTokens.Count > 0)                         
        {                                                       
            if(tokens.ElementAt(0) != "log")
                result += doArithmetic(tempTokens, lex);

            if(tokens.ElementAt(0) == "cos")
            {           
                tokens.RemoveAt(0);         
                tokens.Insert(0, Math.Cos(result).ToString());
            }
            else if(tokens.ElementAt(0) == "sin")
            {           
                tokens.RemoveAt(0);         
                tokens.Insert(0, Math.Sin(result).ToString());
            }
            else if(tokens.ElementAt(0) == "log")
            {           
                tokens.RemoveAt(0);                                          
                int splitter = tempTokens.IndexOf(",");  
                double result1 = 0, result2 = 0;

                string part1 = createString(tempTokens.GetRange(1, splitter-1));                            
                if(!double.TryParse(part1, out _))
                    result1 = doArithmetic(tempTokens.GetRange(1, splitter-1), lex);     
                else
                    result1 = double.Parse(part1);


                string part2 = createString(tempTokens.GetRange(splitter+1, ((tempTokens.Count-1)-(splitter+1))));                            
                if(!double.TryParse(part2, out _))
                    result2 = doArithmetic(tempTokens.GetRange(splitter+1, ((tempTokens.Count-1)-(splitter+1))), lex);     
                else
                    result2 = double.Parse(part2);

                if(result1 != 0)
                {
                    if(Math.Log(result1) != 0)
                        tokens.Insert(0, (Math.Log(result2)/Math.Log(result1)).ToString());
                    else
                        tokens.Insert(0, "0");
                }
                else
                    tokens.Insert(0, "0");
            }                                 
            else
                tokens.Insert(pos, result.ToString());
            tempTokens.Clear();
        }  

        int posParenthesis = tokens.IndexOf("(");
        if(posParenthesis >= 0)
            result=doArithmetic(tokens, lex);  

        for (int i = 0; i < tokens.Count; i++)
        {
            if(i%2!=0)
            {
                if (tokenlist.Count > 0 && checktokens(tokens))
                {
                    for (int j = 0; j < tokenlist.Count; j++)
                    {
                        if(tokens.ElementAt(i-1) == tokenlist.ElementAt(j).Nombre)
                            token1 = Convert.ToDouble(((Variable)tokenlist.ElementAt(j)).Valor);                            

                        if(tokens.ElementAt(i+1) == tokenlist.ElementAt(j).Nombre)
                            token2 = Convert.ToDouble(((Variable)tokenlist.ElementAt(j)).Valor);                          
                    }
                     
                        if(token1 != -1)  
                            result = token1;
                        else{
                            number1 = double.Parse(tokens.ElementAt(i-1)); 
                            result = number1;
                        }   

                        if(token2 == -1)
                            number2 = double.Parse(tokens.ElementAt(i+1));                              

                        if(tokens.ElementAt(i) == "^")
                        {
                            if(token2 != -1)  
                                result = Math.Pow(result,token2);
                            else
                                result = Math.Pow(result,number2);

                            tokens.RemoveAt(i-1);
                            tokens.RemoveAt(i-1);
                            tokens.RemoveAt(i-1);
                            if(i < tokens.Count)
                                tokens.Insert(i-1, result.ToString());
                            else
                                tokens.Add(result.ToString());
                            
                            i--;
                            if(tokens.Count == 1)
                                break;
                        }  
                        if(tokens.ElementAt(i) == "/")
                        {
                            if (token2 == 0 || number2 == 0)
                            {
                                mathexception = true;
                                break;
                            }
                            if(token2 != -1)  
                                result/= token2;
                            else
                                result/= number2;

                            tokens.RemoveAt(i-1);
                            tokens.RemoveAt(i-1);
                            tokens.RemoveAt(i-1);
                            if(i < tokens.Count)
                                tokens.Insert(i-1, result.ToString());
                            else
                                tokens.Add(result.ToString());
                            
                            i--;
                            if(tokens.Count == 1)
                                break;
                        }   

                        if(tokens.ElementAt(i) == "*")
                        {
                            if(token2 != -1)  
                                result*= token2;
                            else
                                result*= number2;

                            tokens.RemoveAt(i-1);
                            tokens.RemoveAt(i-1);
                            tokens.RemoveAt(i-1);
                            if(i < tokens.Count)
                                tokens.Insert(i-1, result.ToString());
                            else
                                tokens.Add(result.ToString());
                            
                            i--;
                            if(tokens.Count == 1)
                                break;
                        }
                        if (tokens.ElementAt(i) == "%")
                        {
                            if(token2 != -1)  
                                result%= token2;
                            else
                                result%= number2;

                            tokens.RemoveAt(i-1);
                            tokens.RemoveAt(i-1);
                            tokens.RemoveAt(i-1);
                            if(i < tokens.Count)
                                tokens.Insert(i-1, result.ToString());
                            else
                                tokens.Add(result.ToString());
                            
                            i--;
                            if(tokens.Count == 1)
                                break;
                        }
                }else
                {
                    number1 = double.Parse(tokens.ElementAt(i-1));                            
                    number2 = double.Parse(tokens.ElementAt(i+1)); 
                    result = number1;

                    if(tokens.ElementAt(i) == "^")
                    { 
                        result = Math.Pow(result,number2);

                        tokens.RemoveAt(i-1);
                        tokens.RemoveAt(i-1);
                        tokens.RemoveAt(i-1);
                        if(i < tokens.Count)
                            tokens.Insert(i-1, result.ToString());
                        else
                            tokens.Add(result.ToString());
                        exist = true;
                        
                        i--;
                        if(tokens.Count == 1)
                            break;
                    }  
                    if(tokens.ElementAt(i) == "*")
                    { 
                        result*= number2;

                        tokens.RemoveAt(i-1);
                        tokens.RemoveAt(i-1);
                        tokens.RemoveAt(i-1);
                        if(i < tokens.Count)
                            tokens.Insert(i-1, result.ToString());
                        else
                            tokens.Add(result.ToString());
                        exist = true;
                        
                        i--;
                        if(tokens.Count == 1)
                            break;
                    }     

                    if(tokens.ElementAt(i) == "/")
                    { 
                        if (number2 == 0)
                        {
                            mathexception = true;
                            break;
                        }
                        result/= number2;

                        tokens.RemoveAt(i-1);
                        tokens.RemoveAt(i-1);
                        tokens.RemoveAt(i-1);
                        if(i < tokens.Count)
                            tokens.Insert(i-1, result.ToString());
                        else
                            tokens.Add(result.ToString());
                        exist = true;
                        
                        i--;
                        if(tokens.Count == 1)
                            break;
                    } 

                    if (tokens.ElementAt(i) == "%")
                    {
                        result%= number2;

                        tokens.RemoveAt(i-1);
                        tokens.RemoveAt(i-1);
                        tokens.RemoveAt(i-1);
                        if(i < tokens.Count)
                            tokens.Insert(i-1, result.ToString());
                        else
                            tokens.Add(result.ToString());
                        exist = true;
                        
                        i--;
                        if(tokens.Count == 1)
                            break;
                    }  
                }                                                               
            }               
        }

        if(tokens.Count > 1 && exist == false){
            for (int i = 0; i < tokens.Count; i++)
            {
                if(i%2!=0)
                {
                    if (tokenlist.Count > 0 && checktokens(tokens))
                    {
                    for (int j = 0; j < tokenlist.Count; j++)
                    {
                        if(tokens.ElementAt(i-1) == tokenlist.ElementAt(j).Nombre)
                            token1 = Convert.ToDouble(((Variable)tokenlist.ElementAt(j)).Valor);                            

                        if(tokens.ElementAt(i+1) == tokenlist.ElementAt(j).Nombre)
                            token2 = Convert.ToDouble(((Variable)tokenlist.ElementAt(j)).Valor);                          
                    }
                     
                        if(token1 != -1)  
                            result = token1;
                        else{
                            number1 = double.Parse(tokens.ElementAt(i-1)); 
                            result = number1;
                        }   

                        if(token2 == -1)
                            number2 = double.Parse(tokens.ElementAt(i+1));     

                            if(tokens.ElementAt(i) == "+")
                            {
                                if(token2 != -1)  
                                    result+= token2;
                                else
                                    result+= number2;

                                tokens.RemoveAt(i-1);
                                tokens.RemoveAt(i-1);
                                tokens.RemoveAt(i-1);
                                if(i < tokens.Count)
                                    tokens.Insert(i-1, result.ToString());
                                else
                                    tokens.Add(result.ToString());
                                
                                i--;
                                if(tokens.Count == 1)
                                    break;
                            }   

                            if(tokens.ElementAt(i) == "-")
                            {
                                if(token2 != -1)  
                                    result-= token2;
                                else
                                    result-= number2;

                                tokens.RemoveAt(i-1);
                                tokens.RemoveAt(i-1);
                                tokens.RemoveAt(i-1);
                                if(i < tokens.Count)
                                    tokens.Insert(i-1, result.ToString());
                                else
                                    tokens.Add(result.ToString());
                                
                                i--;
                                if(tokens.Count == 1)
                                    break;                                
                            }                                                               
                    }else
                    {
                        number1 = double.Parse(tokens.ElementAt(i-1));                            
                        number2 = double.Parse(tokens.ElementAt(i+1)); 
                        result = number1;    

                        if(tokens.ElementAt(i) == "+")
                        {
                            result+= number2;

                            tokens.RemoveAt(i-1);
                            tokens.RemoveAt(i-1);
                            tokens.RemoveAt(i-1);
                            if(i < tokens.Count)
                                tokens.Insert(i-1, result.ToString());
                            else
                                tokens.Add(result.ToString());
                            
                            i--;
                            if(tokens.Count == 1)
                                break;                            
                        }   

                        if(tokens.ElementAt(i) == "-")
                        {
                            result-= number2;

                            tokens.RemoveAt(i-1);
                            tokens.RemoveAt(i-1);
                            tokens.RemoveAt(i-1);
                            if(i < tokens.Count)
                                tokens.Insert(i-1, result.ToString());
                            else
                                tokens.Add(result.ToString());

                            i--;
                            if(tokens.Count == 1)
                                break;
                        }                                                                   
                
                    }
                }
            }      
        }      

        if(tokens.Count > 1){
            result = doArithmetic(tokens, lex);
        }                

        return result;
    }

    /// <summary>
    /// Método que implemental a funcionalidad de concatenación de cadenas.
    /// </summary>
    /// <param name="lines"></Línea de código ingresada por el usuario.>
    /// <param name="lex"></Resultado del análisis léxico>
    /// <returns></Retorna las cadenas concatenadas>
    public static string Concat(List<string> lines, Lexicon lex)
    {
        string result = "";
        List<int> concat = checkif(lines,"@");
        List<SpecialTokensClass> specials = lex.SpecialTokensClass();
        for (int i = 0; i < lines.Count; i++)
        {
            if (specials.Any(x => x is Variable && ((Variable)x).Nombre == lines[i] && (((Variable)x).Tipo == "string" || ((Variable)x).Tipo == "int")))
            {
                string temp = lines[i];
                SpecialTokensClass? specialtemp = specials.Find(x => x is Variable && ((Variable)x).Nombre == lines[i] && (((Variable)x).Tipo == "string" || ((Variable)x).Tipo == "int"));
                string temp2 = "";
                if (specialtemp is Variable)
                {
                    temp2 = ((Variable)specialtemp).Valor;
                }
                lines.Insert(i,temp2);
                lines.Remove(temp);
            }
        }
        for (int i = 0; i < concat.Count; i++)
        {   
            if(i==0 || concat.Count == 1)
            {
                string temp = lines.ElementAt(concat.ElementAt(i)-1).Split(new char[] {'"'}).ElementAt(0);
                if (lines.ElementAt(concat.ElementAt(i)-1).Split(new char[] {'"'}).Count() > 1)
                {
                    temp = lines.ElementAt(concat.ElementAt(i)-1).Split(new char[] {'"'}).ElementAt(1);
                }

                result+=temp;
            }
            string temp2 = lines.ElementAt(concat.ElementAt(i)+1).Split(new char[] {'"'}).ElementAt(0);
            if (lines.ElementAt(concat.ElementAt(i)+1).Split(new char[] {'"'}).Count() > 1)
            {
                temp2 = lines.ElementAt(concat.ElementAt(i)+1).Split(new char[] {'"'}).ElementAt(1);
            }

            result+=" "+temp2;
        }
        if (lines.Count == 1)
        {
            string temp = lines[0].Split(new char[] {'"'}).ElementAt(0);
            if (lines[0].Split(new char[]{'"'}).Count() > 1)
            {
                temp = lines[0].Split(new char[]{'"'}).ElementAt(1);
            }
            result = temp;
        }

        return result;
    }

    /// <summary>
    /// Método usado para facilitar el cálculo de senos, cosenos y logaritmos.
    /// </summary>
    /// <param name="tokens"></Operación matemática ingresada>
    /// <returns></True: Si se uso alguna de las operaciones mencionadas anteriormente.
    /// False: Si no se uso ninguna de las operaciones mencionadas anteriormente.>
    public static bool checktokens(List<string> tokens)
    {
        bool result = false;
        List<string> temp = new List<string>();
        for (int i = 0; i < tokens.Count-1; i++)
        {
            if (!int.TryParse(tokens.ElementAt(i), out _) && tokens[i] != "/"
            && tokens[i] != "*" && tokens[i] != "+" && tokens[i] != "-" &&
            tokens[i] != "sin" && tokens[i] != "cos" && tokens[i] != "log" && tokens[i] != ",")
            {
                temp.Add(tokens.ElementAt(i));
            }
        }
        if (temp.Count > 0)
        {
            result = true;
        }

        return result;
    }
    /// <summary>
    /// Método usado para crear un string en el uso de los senos, cosenos y logarítmos.
    /// </summary>
    /// <param name="subTokens"></Combianada que contiene las operaciones anteriores>
    /// <returns></Retorna el string de la operación que se uso de las mencionadas anteriormente>
    public static string createString(List<string> subTokens)
    {
        string part = "";

        for (int i = 0; i < subTokens.Count; i++)
        {
            part+=subTokens[i];
        }

        return part;
    }
    /// <summary>
    /// Método que reemplaza una variable por su valor en la línea
    /// </summary>
    /// <param name="tokens"></Línea de código ingresada por el usuario>
    /// <param name="lex"></Resultado del análisis léxico>
    private static void ReplaceValue(List<string> tokens,Lexicon lex)
    {
        List<SpecialTokensClass> specials = lex.SpecialTokensClass();
        if (specials.Any(x => x is Variable && tokens.Any(y => y == x.Nombre)))
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                string value = "";
                for (int j = 0; j < specials.Count; j++)
                {
                    if (tokens[i] == specials[j].Nombre)
                    {
                        if (specials[j] is Variable)
                            value = ((Variable)specials[j]).Valor;
                    }
                }
                if (value != "")
                {
                    tokens.Insert(i,value);
                    tokens.Remove(tokens.ElementAt(i+1));
                }
            }
        }
    }

    /// <summary>
    /// Método que me prepara la línea de código para hacerle print
    /// </summary>
    /// <param name="tokens"></Código que está dentro del contexto del print>
    /// <param name="lex"></Resultado del análisis léxico>
    public static void print2(List<string> tokens, Lexicon lex)
    {
        tokens.Remove(tokens[0]);
        tokens.Remove(tokens[0]);
        if(tokens[tokens.Count-1] == ";")
            tokens.RemoveAt(tokens.Count-2);
        else
            tokens.RemoveAt(tokens.Count-1);
        ReplaceValue(tokens,lex);
        if (next(tokens,lex) == "")
        {
            print(tokens,lex);
        }else
        {
            Executeline(tokens,lex);
        }
    }
    /// <summary>
    /// Método que implementa la funcionalidad del print del lenguaje HULK.
    /// </summary>
    /// <param name="tokens"></Línea de código que ingresó el usuario ya fraccionada>
    /// <param name="lex"></Resultado del análisis léxico>
    public static void print(List<string> tokens, Lexicon lex)
    {
        if (tokens.Count == 1)
        {
            System.Console.WriteLine(tokens[0]);   
        }
        else if (tokens.Any(x => x == "@"))
        {
            System.Console.WriteLine(Concat(tokens,lex));
        }else
        {
            List<string> inside = tokens.GetRange(2, tokens.Count-3); 
            if (inside.Count == 1)
            {
                if (int.TryParse(inside.ElementAt(0), out _)) 
                {
                    System.Console.WriteLine(inside[0]);
                }
                else if (inside[0].Contains('"'))
                {
                    System.Console.WriteLine(inside[0].ToString().Substring(1, inside[0].ToString().Length-2));
                }
                else
                {                    
                    for (int i = 0; i < lex.SpecialTokensClass().Count; i++)
                    {
                        if (inside[0] == lex.SpecialTokensClass().ElementAt(i).Nombre)
                        {
                            if(lex.SpecialTokensClass().ElementAt(i) is TokenValue)
                                System.Console.WriteLine(((TokenValue)lex.SpecialTokensClass().ElementAt(i)).Valor);
                        }
                    }
                }                   
            }         
        }
    }

    /// <summary>
    /// Método que me dice que funcionalidad es la que se debe ejecutar posteriormente.
    /// </summary>
    /// <param name="lines"></Línea de código ingresada por el usuario.>
    /// <param name="lex"></Resultado del análisis léxico.>
    /// <returns></Retorna un string con el nombre de la siguiente funcionalidad a ejecutar>
    public static string next(List<string> lines,Lexicon lex)
    {
        string result = "";
        List<SpecialTokensClass> special = lex.SpecialTokensClass();
        if (lines.Count == 2 && lines[lines.Count-1] == ";")
        {
            lines.RemoveAt(lines.Count-1);
        }
        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i] == "let")
            {
                result = "let";
                break;
            }else if (lines[i] == "print")
            {
                result = "print";
                break;
            }else if (lines[i] == "if")
            {
                result = "if";
                break;
            }else if (special.Any(x =>  x is Function && ((Function)x).Nombre == lines[i]))
            {
                result = "function";
                break;
            }else if (lines.Count == 1 || lines.Contains("@"))
            {
                result = "";
                continue;
            }else if((int.TryParse(lines[i],out _) || special.Any(x => x is Variable && ((Variable)x).Nombre == lines[i] && ((Variable)x).Tipo == "int"))
            && lines.Count > 1)
            {
                result= "math";
                continue;
            }
        }


        return result;
    }

    /// <summary>
    /// Método que implementa la funcionalidad de let-in.
    /// </summary>
    /// <param name="tokens"></Línea de código ya fraccionada>
    /// <param name="valid"></Indica si es valida la expresión dentro del in>
    /// <returns></Retorna una lista con los elementos que se encuentran dentro del cuerpo del in>
    public static List<string> let_in(List<string> tokens, ref bool valid)
    {
        List<string> part = new List<string>();
        int posLet = tokens.IndexOf("let");
        int posIn = tokens.IndexOf("in", posLet);
        int posEnd = 0, countOpen = 0, countClose = 0;

        // buscar posicion de cierre
        for (int i = posIn + 1; i < tokens.Count; i++)
        {
            if(tokens.ElementAt(i) == "(")
                countOpen++;
            else if(tokens.ElementAt(i) == ")")
                countClose++;

            if(tokens.ElementAt(i) == ";" || countClose > countOpen)
            {
                posEnd = i-1;
                break;
            }
        }

        part = tokens.GetRange(posIn+1, (posEnd-(posIn+1))+1);

        int posAssign = part.IndexOf("=");
        if(posAssign >= 0)
        {
            posLet = part.IndexOf("let");
            if(posLet == -1 || posLet > posAssign)
                valid = false;
        }
        if (part[0] == "(")
        {
            part.RemoveAt(0);
            if (part[part.Count-1] == ")")
                part.RemoveAt(part.Count-1);
        }
        return part;
    }
    /// <summary>
    /// Método que me chequea todas las condiciones dentro de la instrucción if-else
    /// </summary>
    /// <param name="ifconditiondata"></Condiciones del if>
    /// <param name="lex"></Resultado del análisis léxico>
    /// <returns></Retorna True: si se cumplen las condiciones del if, False: si no se cumplen>
    private static bool CheckMultiCondition(ref List<string> ifconditiondata, Lexicon lex)
    {
        bool result = false;
        ifconditiondata.RemoveAt(0);
        ifconditiondata.RemoveAt(ifconditiondata.Count-1);
        Dictionary<List<string>,bool> conditionvalues = new Dictionary<List<string>, bool>();
        List<string> save = new List<string>();
        save.AddRange(ifconditiondata);
        do
        {
            int orpos = save.ToList().IndexOf("|");
            int andpos = save.ToList().IndexOf("&");
            if (orpos == andpos)
            {
                List<string> temp = save.GetRange(0,save.Count);
                bool tempbool = Evaluate(temp,lex);
                conditionvalues.Add(temp,tempbool);
                save.RemoveRange(0,save.Count);
            }
            else if ((orpos < andpos && orpos != -1) || andpos == -1)
            {
                List<string> temp = save.GetRange(0,orpos);
                bool tempbool = Evaluate(temp,lex);
                conditionvalues.Add(temp,tempbool);
                save.RemoveRange(0,temp.Count);
                save.RemoveAt(0);
            }else
            {
                List<string> temp = save.GetRange(0,andpos);
                bool tempbool = Evaluate(temp,lex);
                conditionvalues.Add(temp,tempbool);
                save.RemoveRange(0,temp.Count);
                save.RemoveAt(0);
            }
        } while (save.Count != 0);

        if (conditionvalues.Values.All(x => x == true))
        {
            return true;
        }

        List<int> andposes = checkif(ifconditiondata,"&");
        List<int> orposes = checkif(ifconditiondata,"|");
        List<bool> finalresults = new List<bool>();
        do
        {
            string operatortemp = CheckFirst(andposes,orposes);
            if (operatortemp == "&")
            {
                if (conditionvalues.Values.ElementAt(0) && conditionvalues.Values.ElementAt(1))
                {
                    finalresults.Add(true);
                }else
                {
                    finalresults.Add(false);
                }
                conditionvalues.Remove(conditionvalues.Keys.ElementAt(0));
                andposes.RemoveAt(0);
            }else
            {
                if (conditionvalues.Values.ElementAt(0) || conditionvalues.Values.ElementAt(1))
                {
                    finalresults.Add(true);
                }else
                {
                    finalresults.Add(false);
                }
                conditionvalues.Remove(conditionvalues.Keys.ElementAt(0));
                orposes.RemoveAt(0);
            }

        } while (andposes.Count != 0 || orposes.Count != 0);
        if(finalresults.All(x => x))
            result = true;
        else
            result = false;
        return result;
    }
    /// <summary>
    /// Método que chequea el operador de condición que siguiente en la línea
    /// </summary>
    /// <param name="andposes"></Posiciones de los operadores & usados>
    /// <param name="orposes"></Posiciones de los operadores | usados>
    /// <returns></Retorna el siguiente operador>
    private static string CheckFirst(List<int> andposes,List<int> orposes)
    {
        string result = "";
        if (andposes.Count > 0 && orposes.Count == 0)
        {
            result = "&";
        }else if (orposes.Count > 0 && andposes.Count == 0)
        {
            result = "|";
        }else
        {
            if (andposes[0] < orposes[0])
            {
                result = "&";
            }else
            {
                result = "|";
            }
        }
        return result;
    }
    /// <summary>
    /// Método que evalua las condiciones de la instrucción if
    /// </summary>
    /// <param name="tempif"></Condición a evaluar actual>
    /// <param name="lex"></Resultado del análisis léxico>
    /// <returns></Retorna True: si la condición se cumple False: si la condición no se cumple>
    private static bool Evaluate(List<string> tempif, Lexicon lex)
    {
        bool result = false;
        bool isnumber = false;
        bool isstring = false;
        List<SpecialTokensClass> special = lex.SpecialTokensClass();
        string? operatorcondition = tempif.Find(x => lex.ConditionOperators().Contains(x));
        for (int i = 0; i < tempif.Count; i++)
        {
            for (int j = 0; j < special.Count; j++)
            {
                if (tempif[i] == special[j].Nombre && special[j] is Variable)
                {
                    tempif[i] = ((Variable)special[j]).Valor.ToString();
                    if (double.TryParse(tempif[i], out _))
                    {
                        isnumber = true; 
                        //break;  
                    }else if(tempif[i] != "true" || tempif[i] != "false")
                    {
                        isstring = true;
                        //break;
                    }
                }else
                {
                    if (double.TryParse(tempif[i], out _))
                    {
                        isnumber = true; 
                        //break;  
                    }else if(tempif[i] != "true" || tempif[i] != "false")
                    {
                        isstring = true;
                        //break;
                    }
                }
            }
        }
        
        if (isnumber || isstring)
        {
            List<int> numbers = checkif(tempif,"%");
            List<int> equals = checkif(tempif,operatorcondition);
            List<int> concat = checkif(tempif,"@");
            double resultdouble1 = 0, resultdouble2 = 0;
            string resultstring1 = "", resultstring2 = "";
            if (numbers.Count > 0 && isnumber)
            {
                for (int i = 0; i < equals.Count; i++)
                {                       
                    List<string> temp1 = tempif.GetRange(0,equals.ElementAt(i));
                    resultdouble1 = doArithmetic(temp1,lex);
                    List<string> temp2 = new List<string>();

                    temp2 = tempif.GetRange(equals.ElementAt(i)+1,tempif.Count-(equals.ElementAt(i)+1));
                    resultdouble2 = doArithmetic(temp2,lex);
                }

            }else if(numbers.Count == 0 && isnumber)
            {
                List<string> prueba1 = tempif.GetRange(0,equals.ElementAt(0));
                List<string> prueba2 = tempif.GetRange(equals.ElementAt(0)+1,(tempif.Count)-(equals.ElementAt(0)+1));
                resultdouble1 = doArithmetic(tempif.GetRange(0,equals.ElementAt(0)),lex);
                resultdouble2 = doArithmetic(tempif.GetRange(equals.ElementAt(0)+1,(tempif.Count)-(equals.ElementAt(0)+1)),lex);
                
            }else if (concat.Count > 0 && isstring)
            {
                for (int i = 0; i < equals.Count; i++)
                {                       
                    List<string> temp1 = tempif.GetRange(0,equals.ElementAt(i));
                    resultstring1 = Concat(temp1,lex);
                    List<string> temp2 = new List<string>();

                    temp2 = tempif.GetRange(equals.ElementAt(i)+1,tempif.Count-(equals.ElementAt(i)+1));
                    resultstring2 = Concat(temp2,lex);
                }
            }else if(concat.Count == 0 && isstring)
            {
                resultstring1 = tempif.ElementAt(equals.ElementAt(0)-1);
                resultstring2 = tempif.ElementAt(equals.ElementAt(0)+1);
            }

            if (operatorcondition == "==")
            {
                if (isnumber)
                {
                    if(resultdouble1 == resultdouble2)
                        result = true;
                    else
                        result = false;
                }else if (isstring)
                {
                    if(resultstring1 == resultstring2)
                        result = true;
                    else
                        result = false;
                }
            }else if (operatorcondition == "!=")
            {
                if (isnumber)
                {
                    if(resultdouble1 != resultdouble2)
                        result = true;
                    else 
                        result = false;
                }else if (isstring)
                {
                    if(resultstring1 != resultstring2)
                        result = true;
                    else
                        result = false;
                }
            }else if (operatorcondition == ">=")
            {
                if (isnumber)
                {
                    if(resultdouble1 >= resultdouble2)
                        result = true;
                    else
                        result = false;
                }
            }else if (operatorcondition == "<=")
            {
                if (isnumber)
                {
                    if(resultdouble1 <= resultdouble2)
                        result = true;
                    else
                        result = false;
                }
            }else if (operatorcondition == ">")
            {
                if (isnumber)
                {
                    if(resultdouble1 > resultdouble2)
                        result = true;
                    else
                        result = false;
                }
            }else if (operatorcondition == "<")
            {
                if (isnumber)
                {
                    if(resultdouble1 < resultdouble2)
                        result = true;
                    else
                        result = false;
                }
            }
        }else if (tempif.Any(x => x == "true") || tempif.Any(x => x == "false"))
        {
            List<int> equals = checkif(tempif,operatorcondition);
            int operatorindex = tempif.FindIndex(x => x == operatorcondition);
            if (equals.Count > 0)
            {
                if (operatorcondition == "==")
                {
                    if (operatorindex != -1)
                    {
                        if (tempif[operatorindex-1] == tempif[operatorindex+1])
                            result = true;
                        else
                            result = false;
                    }
                }else
                {
                    if (operatorindex != -1)
                    {
                        if (tempif[operatorindex-1] != tempif[operatorindex+1])
                            result = true;
                        else
                            result = false;
                    }
                }
            }else
            {
                List<int> truepos = checkif(tempif,"true");
                List<int> falsepos = checkif(tempif,"false");
                if (truepos.Count > 0)
                    result = true;
                else
                    result = false;  
            }
        }
        
        return result;
    }

    /// <summary>
    /// Método que implementa la funcionalidad de if-else.
    /// </summary>
    /// <param name="tokens"></Línea de código ya fraccionada>
    /// <param name="ifData"></Cuerpo de la instrucción if>
    /// <param name="elseData"></Cuerpo de la instrucción else>
    /// <param name="ifConditionData"></Condición usada en el if>
    /// <returns></True: Si está bien estructurada la instrucción if-else
    ///False: Si está mal estructurada la instrucción if-else>
    public static List<string> if_else(List<string> tokens, ref List<string> ifData, ref List<string> elseData, ref List<string> ifConditionData,Lexicon lex)
    {
        int posIf = tokens.IndexOf("if");
        int countIf = 0, countElse = 0, posElse = -1;
        List<string> result = new List<string>();
        //encontrar las posicion del if y el else
        if (posIf != -1)
        {
            for (int i = posIf; i < tokens.Count; i++)
            {
                if(tokens.ElementAt(i) == "if")
                    countIf++;
                else if(tokens.ElementAt(i) == "else")
                    countElse++;   

                if(countElse == countIf)
                {
                    posElse = i;
                    break;
                }
                                            
            }
        }
        
        if(posElse == -1)
            return new List<string>();
        // encontrar donde termina el else
        int posEnd = -1, countOpen = 0, countClose = 0;
        for (int i = posElse + 1; i < tokens.Count; i++)
        {
            if(tokens.ElementAt(i) == "(")
                countOpen++;
            else if(tokens.ElementAt(i) == ")")
                countClose++;

            if(tokens.ElementAt(i) == ";" || countClose > countOpen)
            {
                posEnd = i-1;
                break;
            }else
            {
                posEnd = i;
            }
        }      

        if(posEnd == -1)
            return new List<string>();

        //cargar la condicion del if
        int posEndIfCondition = -1;
        countOpen = 0; countClose = 0;
        for (int i = posIf + 1; i < tokens.Count; i++)
        {
            if(tokens.ElementAt(i) == "(")
                countOpen++;
            else if(tokens.ElementAt(i) == ")")
                countClose++;

            if(countClose == countOpen)
            {
                posEndIfCondition = i;
                break;
            }
        }      

        if(posEndIfCondition == -1)
            return new List<string>();

        elseData = tokens.GetRange(posElse+1, (posEnd-(posElse+1))+1);
        ifData = tokens.GetRange(posEndIfCondition+1, ((posElse-1)-(posEndIfCondition+1))+1);
        ifConditionData = tokens.GetRange(posIf+1, ((posEndIfCondition)-(posIf+1))+1);
        Dictionary<string,int> conditionTokenPos = checkifCondition(ifConditionData);
        List<SpecialTokensClass> special = lex.SpecialTokensClass();

        if (conditionTokenPos.Count > 0)
        {
            bool temp = CheckMultiCondition(ref ifConditionData,lex);
            if (temp)
            {
                result = ifData;
            }else
            {
                result = elseData;
            }
        }else
        {
            ifConditionData.Remove(ifConditionData.ElementAt(0));
            ifConditionData.Remove(ifConditionData.ElementAt(ifConditionData.Count-1));
            bool temp = Evaluate(ifConditionData,lex);
            if (temp)
            {
                result = ifData;
            }else
            {
                result = elseData;
            }
        }
        return result;
    }
    /// <summary>
    /// Método que chequea si una función es recursiva
    /// </summary>
    /// <param name="tokens"></Línea de código ingresada por el usuario>
    /// <param name="function"></nombre de la función>
    /// <returns></returns>
    private static bool CheckRec(List<string> tokens, string function)
    {
        bool resuelt = false;
        for (int i = 0; i < tokens.Count; i++)
        {
            if (tokens[i] == function)
            {
                resuelt = true;
            }
        }

        return resuelt;
    }
    /// <summary>
    /// Método que implementa la funcionalidad de las funciones.
    /// </summary>
    /// <param name="lines"></Línead de código ingresada por el usuario.>
    /// <param name="lex"></Resultado del análisis léxico>
    /// <returns></Retorna el resultado de haber ejecutado dicha funcion>
    public static List<string> ExecuteFunction(List<string> lines,string function,List<string> Parametro, Lexicon lex)
    {
        //REVISAR PROBLEMA CON EL PARAMETRO DE LA FUNCION
        List<string> result = new List<string>();
        List<SpecialTokensClass> specials = lex.SpecialTokensClass();
        Parametro.RemoveAll(x => x == ",");
        List<string> paramscopy = new List<string>();
        paramscopy.AddRange(Parametro);
        if (specials.Any(x => x is Function && x.Nombre == function && ((Function)x).Cuerpo.Contains(function)))
        {
            Function temp = (Function)specials.Find(x => x is Function && x.Nombre == function);
            Cleartrash(temp);
            List<string> tempcorp = new List<string>();
            tempcorp.AddRange(temp.Cuerpo);
            List<Variable> tempparams = new List<Variable>();
            tempparams.AddRange(temp.Parametro);
            for (int i = 0; i < tempcorp.Count; i++)
            {
                for (int j = 0; j < tempparams.Count; j++)
                {
                    if (tempcorp[i] == tempparams[j].Nombre)
                    {
                        List<string> parametrotemp = Giveme(paramscopy,j);
                        parametrotemp.Add(";");
                        //makeclear = false;
                        string doneparam = Executeline(parametrotemp,lex).ElementAt(0);
                    
                        tempcorp[i] = doneparam;
                    }
                }
            }

            List<string> ifData = new List<string>();
            List<string> elseData = new List<string>();
            List<string> ifConditionData = new List<string>();
            List<string> tempif = if_else(tempcorp,ref ifData,ref elseData,ref ifConditionData,lex);
            bool isrec = CheckRec(tempif,function);
            if (isrec)
            {
                tempif.Add(";");
                for (int i = 0; i < tempif.Count; i++)
                {
                    if (tempif[i] == function)
                    {
                        int temppos = CloseFunc(tempif,i);
                        List<string> paramtemp = tempif.GetRange(i+2,temppos-(i+2));
                        List<string> functtemp = tempif.GetRange(i,temppos+1-i);
                        List<string> finalfunc = new List<string>();
                        finalfunc.AddRange(functtemp);
                        finalfunc.Add(";");
                        paramtemp.RemoveAll(x => x == ",");
                        int poscom = finalfunc.FindIndex(finalfunc.IndexOf(function), x => x == "(");
                        for (int j = 0; j < paramtemp.Count; j++)
                        {
                            //parametros para llamada recursiva
                            int poscomtemp = poscom+1;
                            if(poscom != -1)
                                poscom = finalfunc.FindIndex(poscom, x => x == ",");
                            paramtemp.Add(";");
                            paramtemp[j] = Executeline(paramtemp,lex).ElementAt(0);
                            finalfunc.Insert(poscomtemp,paramtemp[j]);
                            int tempint = CloseFunc(finalfunc,finalfunc.IndexOf(function));
                            finalfunc.RemoveRange(poscomtemp+1,tempint-(poscomtemp+1));
                        }
                        List<string> tempresult = ExecuteFunction(finalfunc,function,paramtemp,lex);
                        tempif.InsertRange(i,tempresult);
                        temppos = CloseFunc(tempif,tempif.FindIndex(i,x => x == function));
                        tempif.RemoveRange(i+tempresult.Count,temppos-(i+tempresult.Count)+1);
                        finalfunc.Remove(";");
                    }
                }
                tempif.Remove(";");
                if (!tempif.Any(x => x == function) && next(tempif,lex) == "math")
                {
                    string temres = doArithmetic(tempif,lex).ToString();
                    result.Add(temres);
                    return result;
                }else if (next(tempif,lex) != "math" && next(tempif,lex) != "")
                {
                    
                }
            }else
            {
                //makeclear = true;
                return tempif;
            }

        }else
        {
            Function temp = lex.Functions().Find(x => x is Function && x.Nombre == function);
            List<string> tempcorp = new List<string>();
            Cleartrash(temp);
            tempcorp.AddRange(temp.Cuerpo);
            List<Variable> tempparams = new List<Variable>();
            tempparams.AddRange(temp.Parametro);
            for (int i = 0; i < tempcorp.Count; i++)
            {
                for (int j = 0; j < tempparams.Count; j++)
                {
                    if (tempcorp[i] == tempparams[j].Nombre)
                    {
                        List<string> parametrotemp = Giveme(paramscopy,j);
                        tempcorp.InsertRange(i,parametrotemp);
                        tempcorp.RemoveAt(i+parametrotemp.Count);
                    }
                }
            }
            result = tempcorp;
        }
        return result;
    }
    /// <summary>
    /// Método encargado de retornar los parámetros de la función
    /// </summary>
    /// <param name="parametro"></Lista con los parámetros de la función>
    /// <returns></Retorna una lista con los parámetros de la función ordenados>
    private static List<string> Giveme(List<string> parametro, int parampos)
    {
        List<string> result = new List<string>();
        bool check = false;
        int count = 0 , pos = 0;
        if (parametro.Count == 1)
            return parametro;
        for (int i = 0; i < parametro.Count-1; i++)
        {
            if (double.TryParse(parametro[i],out _))
                check = true;
            else
                check = false;
            if (check && double.TryParse(parametro[i+1],out _))
            {
                count++;
                if (count == parampos)
                {
                    pos = i+1;
                }
            }
        }

        for (int i = pos; i < parametro.Count-1; i++)
        {
            if (double.TryParse(parametro[i],out _))
                check = true;
            else
                check = false;
            if (check && double.TryParse(parametro[i+1],out _))
            {
                result.Add(parametro[i]);
                break;
            }else
            {
                result.Add(parametro[i]);
            }
            if (i == parametro.Count -2)
                result.Add(parametro[i+1]);
        }
        return result;
    }
    /// <summary>
    /// Método encargado de eliminar caracteres innecesarios en el cuerpo de la función
    /// </summary>
    /// <param name="actual"></Función sobre la cual se va a realizar la acción>
    public static void Cleartrash(Function actual)
    {
        int tempos = actual.Cuerpo.FindIndex(x => x == "=>");
        actual.Cuerpo.RemoveRange(0,tempos+1);
        if(actual.Cuerpo.ElementAt(actual.Cuerpo.Count-1) == ";")
            actual.Cuerpo.RemoveAt(actual.Cuerpo.Count-1);
    }
    /// <summary>
    /// Método que me devuelve la poscición de cierre de la llamada de la función en la línea
    /// </summary>
    /// <param name="lines"></Línea de código ingresada por el usuario>
    /// <param name="posIn"></posición de la funcion en la línea>
    /// <returns></Retorna la posición de cierre de la llamada de la función en la línea>
    public static int CloseFunc(List<string> lines, int posIn)
    {
        int result = 0;
        int countOpen = 0, countClose = 0;

        for (int i = posIn + 1; i < lines.Count; i++)
        {
            if(lines.ElementAt(i) == "(")
                countOpen++;
            else if(lines.ElementAt(i) == ")")
                countClose++;

            if(lines.ElementAt(i) == ";")
            {
                result = i-1;
                break;
            }else if (countClose >= countOpen && countClose != 0 && countOpen != 0)
            {
                result = i;
                break;
            }
        }
        return result;
    }

    /// <summary>
    /// Método que retorna la posición de cierre de la instrucción dada
    /// </summary>
    /// <param name="lines"></Línea de código ingresada por el usuario.>
    /// <param name="lex"></Resultado del análisis léxico>
    /// <param name="token"></Parámetro a buscar>
    /// <returns></Retorna la posición de cierre de la instrucción dada>
    public static int ClosePos(List<string> lines, int posIn)
    {
        int result = 0;
        int countOpen = 0, countClose = 0;

        for (int i = posIn + 1; i < lines.Count; i++)
        {
            if(lines.ElementAt(i) == "(")
                countOpen++;
            else if(lines.ElementAt(i) == ")")
                countClose++;

            if(lines.ElementAt(i) == ";")
            {
                result = i-1;
                break;
            }else if (countClose > countOpen && (countClose != 0 || countOpen != 0))
            {
                result = i-1;
                break;
            }
        }
        return result;
    }
    /// <summary>
    /// Método que retorna el nombre de la función que se llamara proximamente en caso de existir otro llamado
    /// </summary>
    /// <param name="lines"></Línea de código ingresada por el usuario>
    /// <param name="lex"></Resultado del análisis léxico>
    /// <returns></Retorna el nombre de una función en caso de existir otro llamado>
    public static string nextFunc(List<string> lines,Lexicon lex)
    {
        string result = "";
        List<SpecialTokensClass> special = lex.SpecialTokensClass();
        if (lines.Count == 2 && lines[lines.Count-1] == ";")
        {
            lines.RemoveAt(lines.Count-1);
        }
        for (int i = 0; i < lines.Count; i++)
        {
            if (special.Any(x =>  x is Function && ((Function)x).Nombre == lines[i]))
            {
                result = special.Where(x => x is Function && x.Nombre == lines[i]).ElementAt(0).Nombre;
                break;
            }
        }
        return result;
    }

    /// <summary>
    /// Método con el cual se ejecutan las líneas igresadas por el usuario
    /// </summary>
    /// <param name="lines"></Líneas ingresadas por el usuario>
    /// <param name="lex"></Resultado del análisis léxico>
    /// <returns></El resultado de haber ejecutado la línea de código.>
    public static List<string> Executeline(List<string> lines,Lexicon lex)
    {
        List<string> result = new List<string>();
        List<SpecialTokensClass> tokens = lex.SpecialTokensClass();
        if(lines[0] == "function")
            return result;
        if (next(lines,lex) == "")
        {
            result = lines;
        }else
        {
            //chequear los removerange
            if (next(lines,lex) == "let")
            {
                int tem = lines.FindIndex(0,x => x == "let");
                bool valid = true;
                List<string> temp = let_in(lines,ref valid);  
                lines.InsertRange(tem,temp);
                tem = lines.FindIndex(temp.Count,x => x == "let");
                int tempclosein = ClosePos(lines,lines.FindIndex(temp.Count, x => x == "in"));
                lines.RemoveRange(tem,tempclosein-tem+1); //Buscar cierre del in
                result = Executeline(lines,lex);
            }else if (next(lines,lex) == "if")
            {
                List<string> ifdata = new List<string>();
                List<string> ifconditiondata = new List<string>();
                List<string> elsedata = new List<string>();
                int tem = lines.FindIndex(0,x => x == "if");
                List<string> temp = if_else(lines,ref ifdata,ref elsedata,ref ifconditiondata,lex);
                lines.InsertRange(tem,temp);
                tem = lines.FindIndex(temp.Count,x => x == "if");
                int tempclosein = ClosePos(lines,lines.FindIndex(temp.Count, x => x == "if"));
                lines.RemoveRange(tem,tempclosein-tem+1);
                result = Executeline(lines,lex);
            }else if (next(lines,lex) == "print")
            {
                print2(lines, lex);
                print(lines,lex);
            }else if(next(lines,lex) == "function")
            {
                string functiontemp = nextFunc(lines,lex);
                int tem = lines.FindIndex(0, x => x == functiontemp);
                int closetem = ClosePos(lines,tem);
                List<string> temp = new List<string>();
                List<string> parameters = lines.GetRange(tem+2,closetem - (tem+2));
                temp = ExecuteFunction(lines,functiontemp,parameters,lex);
                lines.InsertRange(tem,temp);
                tem = lines.FindIndex(temp.Count+tem, x => x == functiontemp);
                closetem = ClosePos(lines,tem);
                lines.RemoveRange(tem,closetem - tem+1);
                result = Executeline(lines,lex);
            }else
            {
                List<string> temp = new List<string>();
                temp.AddRange(lines);
                temp.Remove(";");
                string resulttemp = doArithmetic(temp,lex).ToString();
                lines.Insert(0,resulttemp);
                lines.RemoveRange(1,lines.Count-1);
                result = Executeline(lines,lex);
            }
        }
        ReplaceValue(lines,lex);
        //if(makeclear)
            //Clearvar(lex);
        return result;
    }

    /// <summary>
    /// Método que chequea la cantidad de condiciones ingresada por el usuario
    /// en la instrucción if-else.
    /// </summary>
    /// <param name="ifConditionData"></Lista que contiene las condiciones usadas en el if>
    /// <returns></Diccionario con las posiciones de los operadores de condición usados.>
    public static Dictionary<string,int> checkifCondition(List<string> ifConditionData)
    {
        Dictionary<string,int> result = new Dictionary<string, int>();
        for (int i = 0; i < ifConditionData.Count; i++)
        {
            if (ifConditionData[i] == "&")
            {
                result.Add("&",i);
            }else if (ifConditionData[i] == "|")
            {
                result.Add("|",i);
            }
        }

        return result;
    }
    
    /// <summary>
    /// Método que chequea el uso de operadores en la instrucción if.
    /// </summary>
    /// <param name="ifConditionData"></Condiciones empleadas en el if>
    /// <param name="data"></Operador a chequear>
    /// <returns></Diccionario con las posiciones del operador ingresado.>
    public static List<int> checkif(List<string> ifConditionData, string data)
    {
        List<int> result = new List<int>();
        for (int i = 0; i < ifConditionData.Count; i++)
        {
            if (ifConditionData[i] == data)
            {
                result.Add(i);
            }
        }
        return result;
    }
    /// <summary>
    /// Método que me elimina las variables usadas en la línea que acabo de ejecutar para ejecutar la próxima línea
    /// </summary>
    /// <param name="lex"></Resultado del análisis léxico>
    /*private static void Clearvar(Lexicon lex)
    {
        List<SpecialTokensClass> specials = lex.SpecialTokensClass();
        List<string> specialtok = lex.SpecialTokens();
        List<Variable> variables = lex.Variables();
        List<StringToken> stringTokens = lex.StringTokens();
        for (int i = 0; i < specials.Count; i++)
        {
            if(specials[i] is Variable)
            {
                if(specials[i].Nombre != null && specials[i].Nombre != "PI")
                    specialtok.Remove(specials[i].Nombre);
                specials.RemoveAll(x => x.Nombre != "PI");
            }
        }
        variables.RemoveAll(x => x.Nombre != "PI");
        stringTokens.RemoveAll(x => x.Nombre != "PI");
    }*/
}