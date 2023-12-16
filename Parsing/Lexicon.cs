using System.Diagnostics;
using System.Runtime.InteropServices;
/// <summary>
/// Clase lexicon donde se guarda el lenguaje y se analiza la escritura correcta de cada token.
/// </summary>
public class Lexicon
{
    private List<string> symbols;

    private List<string> tokens;

    private List<string> mathTokens;

    private List<string> operators;

    private List<string> conditionOperators;

    private List<string> condicionToken;        

    private List<string> specialTokens;

    private List<SpecialTokensClass> specialTokensClass;

    private List<string> ifConditionOperators;
    private List<string> boolvalues;
    private bool isfunction;

    public bool IsFunction ()
    {
        return isfunction;
    }

    public  List<string> Symbols ()
    {
        return symbols;
    }
    public  List<string> Tokens ()
    {
        return tokens;
    }
    public  List<string> Operators ()
    {
        return operators;
    }
    public  List<string> ConditionOperators ()
    {
        return conditionOperators;
    }
    public  List<string> ConditionToken ()
    {
        return condicionToken;
    }
    public  List<string> SpecialTokens ()
    {
        return specialTokens;
    }
    public  List<string> MathTokens ()
    {
        return mathTokens;
    }
    public  List<SpecialTokensClass> SpecialTokensClass ()
    {
        return specialTokensClass;
    }
    public List<string> BoolValues()
    {
        return boolvalues;
    }
    public List<string> IfConditionOperator()
    {
        return ifConditionOperators;
    }
    public Lexicon()
    {

        symbols = new List<string>();
        symbols.Add("(");symbols.Add(")");symbols.Add(";");
        symbols.Add("=>");
        symbols.Add("@");
        symbols.Add(",");

        operators = new List<string>();
        operators.Add("*");
        operators.Add("/"); 
        operators.Add("-"); 
        operators.Add("+");
        operators.Add("^");  
        operators.Add("=");
        operators.Add("%");

        conditionOperators = new List<string>();
        conditionOperators.Add("<=");
        conditionOperators.Add(">=");
        conditionOperators.Add("==");
        conditionOperators.Add("!=");
        conditionOperators.Add("<"); conditionOperators.Add(">");            
        

        condicionToken = new List<string>();
        condicionToken.Add("if"); condicionToken.Add("else");

        tokens = new List<string>();
        tokens.Add("let"); 
        tokens.Add("in"); 
        tokens.Add("function");  
        tokens.Add("print");  

        boolvalues = new List<string>();
        boolvalues.Add("true");
        boolvalues.Add("false");

        mathTokens = new List<string>();
        mathTokens.Add("sin");
        mathTokens.Add("cos"); 
        mathTokens.Add("log");
        mathTokens.Add("PI");  

        
        ifConditionOperators = new List<string>();
        ifConditionOperators.Add("&");
        ifConditionOperators.Add("|");

        specialTokens = new List<string>();
        specialTokensClass = new List<SpecialTokensClass>();
        isfunction = false;
    }

    /// <summary>
    /// Método para analizar la linea de código para realizar el análisis léxico.
    /// </summary>
    /// <param name="line"></Línea de codigo ingresada por el usuario.>
    /// <returns></Retorna una lista de la línea dividida por cada elemento.>
    public List<string> divideElements(string line)
    {
        List<string> elements = new List<string>();
        string wordToken = "";
        string wordNumber = "";
        string wordConditionOperator = "";
        
        for (int i = 0; i < line.Length; i++)
        {
            if((line.ElementAt(i) >= 97 && line.ElementAt(i) <= 122) || line[i] == '@')
            {
                if(wordNumber != ""){
                    elements.Add(wordNumber);
                    if (isfunction)
                    {
                        int a = specialTokensClass.FindLastIndex(x => x is Function);
                        ((Function)specialTokensClass.ElementAt(a)).Cuerpo.Add(wordNumber);
                    }

                    addIntegerAsToken(ref elements, wordNumber);
                    wordNumber = "";
                }

                if(wordConditionOperator != ""){
                    elements.Add(wordConditionOperator);
                    if (isfunction)
                    {
                        int a = specialTokensClass.FindLastIndex(x => x is Function);
                        ((Function)specialTokensClass.ElementAt(a)).Cuerpo.Add(wordConditionOperator);
                    }                    
                    wordConditionOperator = "";
                }

                wordToken += line.ElementAt(i);             
            }
            else if(line.ElementAt(i) >= 65 && line.ElementAt(i) <= 90)
            {
                if(wordNumber != ""){
                    elements.Add(wordNumber);
                    if (isfunction)
                    {
                        int a = specialTokensClass.FindLastIndex(x => x is Function);
                        ((Function)specialTokensClass.ElementAt(a)).Cuerpo.Add(wordNumber);
                    }   

                    addIntegerAsToken(ref elements, wordNumber);
                    wordNumber = "";
                }

                if(wordConditionOperator != ""){
                    elements.Add(wordConditionOperator);
                    if (isfunction)
                    {
                        int a = specialTokensClass.FindLastIndex(x => x is Function);
                        ((Function)specialTokensClass.ElementAt(a)).Cuerpo.Add(wordConditionOperator);
                    }                    
                    wordConditionOperator = "";
                }

                wordToken += line.ElementAt(i);    
                //procesar variables booleanas
                if (wordToken == "true" || wordToken == "false")
                {
                    bool process = true;

                    if(line.ElementAt((i-1)-wordToken.Length).ToString()+line.ElementAt((i-1)-(wordToken.Length+1)).ToString() == "==" ||
                        line.ElementAt((i-1)-wordToken.Length+1).ToString()+line.ElementAt((i-1)-(wordToken.Length+2)).ToString() == "==")
                        process = false;

                    if((line.ElementAt((i-1)-wordToken.Length).ToString() == "=" || line.ElementAt((i-1)-(wordToken.Length+1)).ToString() == "=") &&
                        process)
                    {
                        for (int j = 0; j < specialTokensClass.Count; j++)
                        {
                            for (int k = 0; k < elements.Count; k++)
                            {
                                if (elements[k] == specialTokensClass.ElementAt(j).Nombre)
                                {
                                    ((Variable)specialTokensClass.ElementAt(j)).Valor = wordToken;
                                    ((Variable)specialTokensClass.ElementAt(j)).Tipo = "bool";
                                }
                            }
                        }
                    }
                    else
                    {
                        BoolToken v = new BoolToken(null, wordToken);
                        if(!specialTokensClass.Exists(x => x.Nombre == null && ((Variable)x).Valor == wordToken))
                            specialTokensClass.Add(v);                            
                    }
                }           
            }                    
            else if((line.ElementAt(i) >= 48 && line.ElementAt(i) <= 57) || line[i] == '+' || line[i] == '-'
            || line[i] == '*' || line[i] == '/')
            {
                if(wordToken != ""){
                    bool check = checkToken(elements, wordToken,line);
                    if(!check)
                        return new List<string>();
                    wordToken = "";
                }

                if(wordConditionOperator != ""){
                    elements.Add(wordConditionOperator);
                    if (isfunction)
                    {
                        int a = specialTokensClass.FindLastIndex(x => x is Function);
                        ((Function)specialTokensClass.ElementAt(a)).Cuerpo.Add(wordConditionOperator);
                    }                    
                    wordConditionOperator = "";
                }

                wordNumber += line.ElementAt(i); 

  
            }
            else if(line.ElementAt(i).ToString() == ('"').ToString())
            {
                if(wordToken != ""){
                    bool check = checkToken(elements, wordToken,line);
                    if(!check)
                        return new List<string>();
                    wordToken = "";
                }

                if(wordConditionOperator != ""){
                    elements.Add(wordConditionOperator);
                    if (isfunction)
                    {
                        int a = specialTokensClass.FindLastIndex(x => x is Function);
                        ((Function)specialTokensClass.ElementAt(a)).Cuerpo.Add(wordConditionOperator);
                    }                    
                    wordConditionOperator = "";
                }

                if(wordNumber != ""){
                    elements.Add(wordNumber);
                    if (isfunction)
                    {
                        int a = specialTokensClass.FindLastIndex(x => x is Function);
                        ((Function)specialTokensClass.ElementAt(a)).Cuerpo.Add(wordNumber);
                    }                    

                    addIntegerAsToken(ref elements, wordNumber);
                    wordNumber = "";
                }                     

                int count = 0;
                string phrase = ('"').ToString();
                for (int j = i+1; j < line.Count(); j++)
                {
                    if(line.ElementAt(j).ToString() != ('"').ToString())
                    {
                        phrase+=line.ElementAt(j).ToString();
                        count++;
                    }
                    else{
                        phrase+=line.ElementAt(j).ToString();
                        break; 
                    }
                }  

                if(count > 0)
                {
                    if(i + count + 1 >= line.Count()) 
                    {
                        return new List<string>();
                    }
                    elements.Add(phrase);
                    if (isfunction)
                    {
                        int a = specialTokensClass.FindLastIndex(x => x is Function);
                        ((Function)specialTokensClass.ElementAt(a)).Cuerpo.Add(phrase);
                    }                        
                    specialTokens.Add(phrase);
                    int previous = i-1;

                    if(line.ElementAt(i-1).ToString() == " ")
                        previous = i-2;

                    if(line.ElementAt(previous).ToString() == "(")
                    {   
                        int index = specialTokensClass.FindLastIndex(x => x is Function);
                        if(index == specialTokens.Count-1){                                
                            ((Function)specialTokensClass.ElementAt(index)).Parametro.ElementAt(0).Valor = phrase;
                        }
                    }
                    if(line.ElementAt(previous).ToString() == ",")
                    {
                        int index = specialTokensClass.FindLastIndex(x => x is Function);

                        for (int k = 0; k < ((Function)specialTokensClass.ElementAt(index)).Parametro.Count(); k++)
                        {
                            if (((Function)specialTokensClass.ElementAt(index)).Parametro.ElementAt(k).Valor == null)
                            {
                                ((Function)specialTokensClass.ElementAt(index)).Parametro.ElementAt(k).Valor = phrase;
                            }
                        }                                             
                        
                    }                          

                    bool process = true;

                    if(line.ElementAt(i-1).ToString()+line.ElementAt(i-2).ToString() == "==" ||
                        line.ElementAt(i-2).ToString()+line.ElementAt(i-3).ToString() == "==")
                        process = false;

                    if((line.ElementAt(i-1).ToString() == "=" || line.ElementAt(i-2).ToString() == "=") &&
                        process)
                    {
                        if (line.ElementAt(previous).ToString() == "=")
                        {
                            for (int j = specialTokensClass.Count - 1; j >= 0; j--)
                            {
                                if (specialTokensClass.ElementAt(j) is Variable)
                                {
                                    for (int k = 0; k < elements.Count; k++)
                                    {
                                        if(specialTokensClass.ElementAt(j).Nombre == elements[k])
                                        {
                                            List<string> templist = phrase.Split(new char[] {'@'}).ToList();
                                            string tempstr = "";
                                            List<string> temp = new List<string>();
                                            if (templist.Count > 1)
                                            {
                                                for (int l = 0; l < phrase.Length; l++)
                                                {
                                                    if (phrase[i] != '@')
                                                    {
                                                        tempstr+=phrase[i];
                                                    }else
                                                    {
                                                        temp.Add(tempstr);
                                                        tempstr="";
                                                        temp.Add(phrase[i].ToString());
                                                    }
                                                }
                                                phrase = PredFunction.Concat(temp,this);
                                            }
                                            ((Variable)specialTokensClass.ElementAt(j)).Valor = phrase;
                                            ((Variable)specialTokensClass.ElementAt(j)).Tipo = "string";
                                        } 
                                    }
                                }
                                break;
                            } 
                        }
                    }
                    else
                    {
                        StringToken stringtokenvar = new StringToken(null,phrase);
                        specialTokensClass.Add(stringtokenvar);                            
                    }

                    i += phrase.Length - 1;
                }
                else
                    return new List<string>();
            }                    
            else if (operators.Contains(line.ElementAt(i).ToString()) || symbols.Contains(line.ElementAt(i).ToString()))
            {
                if(wordNumber != ""){
                    elements.Add(wordNumber);
                    if (isfunction)
                    {
                        int a = specialTokensClass.FindLastIndex(x => x is Function);
                        ((Function)specialTokensClass.ElementAt(a)).Cuerpo.Add(wordNumber);
                    }                               

                    addIntegerAsToken(ref elements, wordNumber);
                    wordNumber = "";
                }

                if(wordConditionOperator != ""){
                    elements.Add(wordConditionOperator);
                    if (isfunction)
                    {
                        int a = specialTokensClass.FindLastIndex(x => x is Function);
                        ((Function)specialTokensClass.ElementAt(a)).Cuerpo.Add(wordConditionOperator);
                    }                        
                    wordConditionOperator = "";
                }

                if(wordToken != ""){
                    bool check = checkToken(elements, wordToken,line);
                    if(!check)
                        return new List<string>();
                    wordToken = "";                                                          
                }

                bool exec = true;
                if(line.Count() > i+1)
                {
                    if(line.ElementAt(i).ToString() == "=")
                    {
                        if(line.ElementAt(i+1).ToString() == ">" || line.ElementAt(i+1).ToString() == "=")
                            exec = false;
                    }                            
                }

                if(exec){
                    elements.Add(line.ElementAt(i).ToString());
                    if (isfunction)
                    {
                        int a = specialTokensClass.FindLastIndex(x => x is Function);
                        ((Function)specialTokensClass.ElementAt(a)).Cuerpo.Add(line.ElementAt(i).ToString());
                    }                         
                }else
                {
                    elements.Add(line.ElementAt(i).ToString()+line.ElementAt(i+1).ToString());
                    if (isfunction)
                    {
                        int a = specialTokensClass.FindLastIndex(x => x is Function);
                        ((Function)specialTokensClass.ElementAt(a)).Cuerpo.Add(line.ElementAt(i).ToString()+line.ElementAt(i+1).ToString());
                    }                          
                    i++;
                    continue;                
                }                     
            }   
            else
            {
                if(wordNumber != ""){
                    elements.Add(wordNumber);
                    if (isfunction)
                    {
                        int a = specialTokensClass.FindLastIndex(x => x is Function);
                        ((Function)specialTokensClass.ElementAt(a)).Cuerpo.Add(wordNumber);
                    }                              

                    addIntegerAsToken(ref elements, wordNumber);
                    wordNumber = "";
                }

                if(wordConditionOperator != ""){
                    elements.Add(wordConditionOperator);
                    if (isfunction)
                    {
                        int a = specialTokensClass.FindLastIndex(x => x is Function);
                        ((Function)specialTokensClass.ElementAt(a)).Cuerpo.Add(wordConditionOperator);
                    }                     
                    wordConditionOperator = "";
                }

                if(wordToken != ""){
                    bool check = checkToken(elements, wordToken,line);
                    if(!check)
                        return new List<string>();
                    wordToken = "";
                }                      
                if (line.ElementAt(i).ToString() == "!" && line.ElementAt(i+1).ToString() == "=")
                {
                    elements.Add(line.ElementAt(i).ToString()+line.ElementAt(i+1).ToString());
                    if (isfunction)
                    {
                        int a = specialTokensClass.FindLastIndex(x => x is Function);
                        ((Function)specialTokensClass.ElementAt(a)).Cuerpo.Add(line.ElementAt(i).ToString()+line.ElementAt(i+1).ToString());
                    }                      
                    i++;
                    continue;
                }                                         

                if(line.ElementAt(i).ToString() != " ")
                {
                    elements.Add(line.ElementAt(i).ToString());  
                    if (isfunction)
                    {
                        int a = specialTokensClass.FindLastIndex(x => x is Function);
                        ((Function)specialTokensClass.ElementAt(a)).Cuerpo.Add(line.ElementAt(i).ToString());
                    }                     
                }
                                          
            }                               
        }

        if(wordToken != ""){
            elements.Add(wordToken);
            if (isfunction)
            {
                int a = specialTokensClass.FindLastIndex(x => x is Function);
                ((Function)specialTokensClass.ElementAt(a)).Cuerpo.Add(wordToken);
            }            
            wordToken = "";
        }    

        if(wordNumber != ""){
            elements.Add(wordNumber);
            if (isfunction)
            {
                int a = specialTokensClass.FindLastIndex(x => x is Function);
                ((Function)specialTokensClass.ElementAt(a)).Cuerpo.Add(wordNumber);
            }           

            addIntegerAsToken(ref elements, wordNumber);
            wordNumber = "";
        }                

        return elements;
    }        
    /// <summary>
    /// Método que se encarga de crear las variables tipo int y guardarlas en su respectiva clase.
    /// </summary>
    /// <param name="elements"></Lista de tokens que ya se han analizado.>
    /// <param name="wordNumber"></Valor que recibirá la variable int.>
    public void addIntegerAsToken(ref List<string> elements, string wordNumber)
    {
        bool process = true;

        if(elements.Count > 1)
        {
            if( elements.ElementAt(elements.Count - 2) == "==")
                process = false;

            if((elements.ElementAt(elements.Count - 2) == "=") && process)
            {
                for (int j = 0; j < specialTokensClass.Count; j++)
                {
                    if (elements[elements.Count - 3] == specialTokensClass.ElementAt(j).Nombre)
                    {
                        List<string> splitlist = wordNumber.Split(new char[] {'+','-','*','/'}).ToList();
                        List<string> temp = new List<string>();
                        string tempstr = "";
                        if (splitlist.Count > 1)
                        {
                            for (int i = 0; i < wordNumber.Length; i++)
                            {
                                if (double.TryParse(wordNumber[i].ToString(), out _))
                                {
                                    tempstr+=wordNumber[i];
                                }else
                                {
                                    temp.Add(tempstr);
                                    tempstr = "";
                                    temp.Add(wordNumber[i].ToString());
                                }
                            }
                            temp.Add(tempstr);
                            wordNumber = PredFunction.doArithmetic(temp,this).ToString();
                        }
                        elements.RemoveAt(elements.Count-1);
                        elements.Add(wordNumber);
                        ((Variable)specialTokensClass.ElementAt(j)).Valor = wordNumber;
                        ((Variable)specialTokensClass.ElementAt(j)).Tipo = "int";
                    }
                }
            }
            else
            {
                Variable v = new Variable(null, "int", wordNumber);
                if(!specialTokensClass.Exists(x => x.Nombre == null && ((TokenValue)x).Valor == wordNumber && x is Variable))
                    specialTokensClass.Add(v);                                            
            }  
        }      
        else
        {
            Variable v = new Variable(null, "int", wordNumber);
            if(!specialTokensClass.Exists(x => x.Nombre == null && ((TokenValue)x).Valor == wordNumber && x is Variable))
                specialTokensClass.Add(v);
        }
    }
    /// <summary>
    /// Método usado para guardar cada variable en su respectiva clase.
    /// </summary>
    /// <param name="elements"></Lista de tokens que ya se han analizado>
    /// <param name="wordToken"></Toma cada token por separado>
    /// <param name="line"></Línea ingresada por el usuario>
    /// <returns></True: Si se empleo algún elemento no válido. False: Si los elementos que se emplearon
    /// en la línea de código escrita por el usuario son válidos.>
    bool checkToken(List<string> elements, string wordToken, string line)
    {
        List<string> lineclone = line.Split(new char[] {' ',',',(char)'\\'}).ToList();
        if(mathTokens.Contains(wordToken) || tokens.Contains(wordToken) || specialTokens.Contains(wordToken) || condicionToken.Contains(wordToken))
        {
            if(specialTokens.Contains(wordToken) && (elements.ElementAt(elements.Count-1) == "let" || elements.ElementAt(elements.Count-1) == ","))
                return false;

            elements.Add(wordToken);
            if (isfunction)
            {
                int a = specialTokensClass.FindLastIndex(x => x is Function);
                ((Function)specialTokensClass.ElementAt(a)).Cuerpo.Add(wordToken);
            }           
        }
        else
        {
            if(elements.Count > 0)
            {
                if(elements.ElementAt(elements.Count-1) == "let" )
                {
                    specialTokens.Add(wordToken);
                    elements.Add(wordToken);

                    Variable v = new Variable(wordToken, null, null);
                    if(!specialTokensClass.Exists(x => x.Nombre == v.Nombre))
                        specialTokensClass.Add(v);
                    else
                        return false;
                }
                else if(elements.ElementAt(elements.Count-1) == "function")
                {
                    specialTokens.Add(wordToken);
                    elements.Add(wordToken);                  

                    Function f = new Function(wordToken, new List<Variable>(),new List<string>());
                    if(!specialTokensClass.Exists(x => x.Nombre == f.Nombre))
                    {
                        specialTokensClass.Add(f);
                        isfunction = true;
                    }
                }                    
                else if (elements.ElementAt(elements.Count-1) == ",")
                {
                    if(elements.Count >= 5)
                    {
                        if( elements.ElementAt((elements.Count-1)-4) == "let")
                        {
                            specialTokens.Add(wordToken);
                            elements.Add(wordToken);                            

                            Variable v = new Variable(wordToken, null, null);
                            
                            if(!specialTokensClass.Exists(x => x.Nombre == v.Nombre))
                                specialTokensClass.Add(v);   
                            else
                                return false;                                                                                                  
                        }
                        else if(elements.ElementAt((elements.Count-1)-4) == "function")
                        {
                            specialTokens.Add(wordToken);
                            elements.Add(wordToken);                            

                            Variable v = new Variable(wordToken, null, null);
                            int index = specialTokensClass.FindLastIndex(x => x is Function);
                            ((Function)specialTokensClass.ElementAt(index)).Parametro.Add(v);                                      

                        }
                        else
                            return false;
                    }
                    else
                        return false;
                }
                else if (elements.ElementAt(elements.Count-1) == "(")
                {
                    if(specialTokens.Contains(elements.ElementAt(elements.Count-2)))
                    {
                        specialTokens.Add(wordToken);
                        elements.Add(wordToken);
                        if (isfunction)
                        {
                            int a = specialTokensClass.FindLastIndex(x => x is Function);
                            ((Function)specialTokensClass.ElementAt(a)).Cuerpo.Add(wordToken);
                        }                         

                        Variable v = new Variable(wordToken, null, null);
                        if(!specialTokensClass.Exists(x => x.Nombre == v.Nombre))
                            specialTokensClass.Add(v);    
                        else
                            return false;                                                      
                    }else if (elements.ElementAt(elements.Count-2) == "if")
                    {
                        if (wordToken == "true" || wordToken == "false")
                        {
                            elements.Add(wordToken);
                            BoolToken temp = new BoolToken("",wordToken);
                        }
                    }
                    else
                        return false;
                }else if(wordToken == "@")
                {
                    elements.Add(wordToken);
                }                                                     
                else
                    return false;
            }
            else
            {
                return false;
            }
        }

        return true;
    }
}