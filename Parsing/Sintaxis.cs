/// <summary>
/// Clase designada para realizar el análisis sintáctico de las líneas de código
/// </summary>
public class SyntacticAnalisis
{
    /// <summary>
    /// Método para analizar si los paréntesis estan balanceados en la línea de código.
    /// </summary>
    /// <param name="tok"></Línea de código dividida por cada token>
    /// <returns></True: Si no están balanceados. False: Si están balanceados>
    public bool BalancedParentexis(List<string> tok)
    {
        bool bad = false;
        int contar2 = 0;
        //Chequeo de ( )
        for (int j = 0; j < tok.Count && bad == false; j++)        
        {
            if(tok.ElementAt(j) == "(")
            {
                contar2++;
            }    
            if(tok.ElementAt(j) == ")")
            {
                contar2--;
            }          

            if(contar2 < 0)
                bad = true;                                        
        }    

        if(contar2 != 0 ){
            bad = true;
        }

        return bad;
    }

    /// <summary>
    /// Método que realiza el análisis sintáctico y comprueba su validez.
    /// </summary>
    /// <param name="lines"></Lista que contiene todas las líneas de código implementadas por el usuario>
    /// <returns></True: Si existe algun error. False: Si esta bien sintacticamente.>
    public bool syntaxisAnalysis (List<string> currentTokens, Lexicon lex)
    {
        bool bad = false;
        List<string> temp;

        for (int i = 0; i < currentTokens.Count && bad == false; i++)
        {       
            if(currentTokens.ElementAt(currentTokens.Count-1) != ";")  //Chequeo de ;
            {
                bad = true;
            }
                int contar1 = 0;
                for (int j = 0; j < currentTokens.Count && bad == false; j++)
                {
                    if(currentTokens.ElementAt(j) == ";")
                        contar1++;                       
                }   

                if(contar1 > 1)
                {
                    bad = true;
                }                        
            

            if(BalancedParentexis(currentTokens)){
                bad = true;
            }

            int contar2 = 0;
            for (int j = 0; j < currentTokens.Count && bad == false; j++)        //Chequeo de if-else
            {
                if(currentTokens.ElementAt(j) == "if")
                {
                    contar2++;
                }    
                if(currentTokens.ElementAt(j) == "else")
                {
                    contar2--;
                }          

                if(contar2 < 0)
                    bad = true;                                        
            }    

            if(contar2 < 0 ){
                bad = true;
            }
            
            if(bad) {
                Console.WriteLine("Mal (2)");
            }

            for (int j = 0; j < currentTokens.Count && bad == false; j++)
            {
                if(!lex.Symbols().Contains(currentTokens.ElementAt(j)) && !lex.Tokens().Contains(currentTokens.ElementAt(j)) && 
                    !lex.Operators().Contains(currentTokens.ElementAt(j)) && !lex.ConditionToken().Contains(currentTokens.ElementAt(j)) &&
                    !lex.ConditionOperators().Contains(currentTokens.ElementAt(j)) && !lex.SpecialTokens().Contains(currentTokens.ElementAt(j)) &&
                    !lex.MathTokens().Contains(currentTokens.ElementAt(j)) && !int.TryParse(currentTokens.ElementAt(j), out _) 
                    && !lex.BoolValues().Contains(currentTokens.ElementAt(j)) && !lex.IfConditionOperator().Contains(currentTokens.ElementAt(j))){
                    bad = true;
                    }
            }

            if(bad == false)
            {
                if((int.TryParse(currentTokens.ElementAt(0), out _) || lex.Symbols().Contains(currentTokens.ElementAt(0)) || 
                    lex.Operators().Contains(currentTokens.ElementAt(0)) ||lex.ConditionOperators().Contains(currentTokens.ElementAt(0)) ||
                    lex.SpecialTokens().Contains(currentTokens.ElementAt(0))) && !lex.SpecialTokensClass().Any(x => x is Function && x.Nombre == currentTokens.ElementAt(0))){   //Chequeo comienzo con operadores
                    bad = true;
                    }
                else
                {
                    int posIn = -1;

                    int contLet = currentTokens.FindAll(x => x == "let").Count;
                    int contIn = currentTokens.FindAll(x => x == "in").Count;

                    if(contLet != contIn)
                    {
                        bad = true;
                    }

                    if(bad == false)
                    {
                        if(contIn > 0)
                        {
                            if(currentTokens.Any(x => x == "function") || currentTokens.Any(x => x == "=>"))
                                bad = true;
                        }
                    }

                    List<string> updateCurrentToken = new List<string>();
                    for (int j = 0; j < currentTokens.Count() && bad == false; j++)
                    {                            
                        //Chequeo de instruccion let
                        if(currentTokens.ElementAt(j) == "let")                                                       
                        {
                            updateCurrentToken = new List<string>();
                            updateCurrentToken.AddRange(currentTokens.GetRange(j, currentTokens.Count()-j));
                            posIn = updateCurrentToken.FindIndex(x => x == "in") + j;
                            if (posIn == -1)
                            {
                                bad = true;
                                continue;
                            }                         

                            List<string> subList = currentTokens.GetRange(j+1,posIn-Math.Abs(j+1));
                            for (int k = 0; k < subList.Count && bad == false; k++) 
                            {
                                if(!int.TryParse(subList.ElementAt(k), out _) && !lex.Operators().Contains(subList.ElementAt(k)) &&
                                    !lex.MathTokens().Contains(subList.ElementAt(k)) && !lex.SpecialTokens().Contains(subList.ElementAt(k)) &&
                                    subList.ElementAt(k) != "(" && subList.ElementAt(k) != ")" &&  subList.ElementAt(k) != "," && subList.ElementAt(k) != "let"
                                    && subList.ElementAt(k) != "in" && subList.ElementAt(k) != "if" && subList.ElementAt(k) != "else" && subList.ElementAt(k) != "print"
                                    && !lex.Functions().Any(x => x.Nombre == subList.ElementAt(k)))
                                    {
                                        bad = true;
                                    }
                            }                         
                        }
                    }

                    if(contIn == 0 && bad == true && currentTokens.ElementAt(0) == "function")
                    {
                        int posOp = -1;

                        int contF = currentTokens.FindAll(x => x == "function").Count;
                        int contOp = currentTokens.FindAll(x => x == "=>").Count;

                        if(contLet != contIn)
                        {
                            bad = true;
                            goto Next;
                        }

                        if(contOp > 1)
                        {
                            bad = true;
                            goto Next;
                        }  

                        posOp = currentTokens.FindIndex(x => x == "=>");                      
                        List<string> subList = currentTokens.GetRange(1,Math.Abs(posOp-1));
                        for (int k = 0; k < subList.Count && bad == false; k++) 
                        {
                            if(!lex.SpecialTokens().Contains(subList.ElementAt(k)) &&
                                subList.ElementAt(k) != "(" && subList.ElementAt(k) != ")" &&  subList.ElementAt(k) != ",")
                                {
                                    bad = true;
                                }
                        }                                                                                                          
                    }

                    Next:
                    for (int j = 0; j < currentTokens.Count && bad == false; j++)
                    {
                        //Chequeo de instruccion print
                        if(currentTokens.ElementAt(j) == "print")                                              
                        {
                            if(currentTokens.ElementAt(j+1) != "(")
                            {
                                bad = true;
                                continue;
                            }

                            bool balanced = false;
                            int tempos = 1;
                            int closePosition = 0; 
                            do{
                                closePosition = currentTokens.FindIndex(j+tempos, x=>x==")");
                                temp = currentTokens.GetRange(j+1, closePosition-j);
                                if (!BalancedParentexis(temp))
                                {
                                    balanced = true;
                                }
                                else
                                {
                                    if(closePosition > -1)
                                        tempos = (closePosition - j) + 1;   
                                    else
                                        break;                                     
                                }
                            }while(balanced == false && j+tempos < currentTokens.Count);

                            if(balanced == true)
                            {
                                List<string> subList = currentTokens.GetRange(j+2, (closePosition-1) - (j+2));
                                if(subList.Any(x => x == "function") || subList.Any(x => x == "=>")|| subList.Any(x => x == "print"))
                                    bad = true;
                            }       
                            else
                                bad = true; 
                        }
                    }                       


                    for (int j = 0; j < currentTokens.Count && bad == false; j++)
                    {
                        //Chequeo de instruccion if
                        if(currentTokens.ElementAt(j) == "if")            
                        {
                            if(currentTokens.ElementAt(j+1) != "(") {
                                bad = true; 
                                }
                            else
                            {
                                bool balanced = false;
                                int tempos = 1;
                                int closePosition = 0; 

                                do{
                                    closePosition = currentTokens.FindIndex(j+tempos, x=>x==")");
                                    temp = currentTokens.GetRange(j+1, closePosition-j);
                                    if (!BalancedParentexis(temp))
                                    {
                                        balanced = true;
                                    }
                                    else
                                    {
                                        if(closePosition > -1)
                                            tempos = (closePosition - j) + 1;   
                                        else
                                            break;                                     
                                    }
                                }while(balanced == false && j+tempos < currentTokens.Count);                                    


                                if(balanced == true)
                                {
                                    temp = currentTokens.GetRange(j+2, closePosition-(Math.Abs((j+2))));
                                    List<string> tempcount = new List<string>();
                                    tempcount.AddRange(temp);
                                    tempcount.RemoveAll(x => x =="|" || x == "&");
                                    string[] save = new string[temp.Count];
                                    temp.CopyTo(0,save,0,temp.Count);
                                    int count = save.ToList().RemoveAll(x => x == "|" || x == "&");
                                    if(tempcount.Count != count+1 && count != 0)
                                        bad = true;
                                    else
                                    {
                                        for (int k = 0; k < temp.Count && bad == false; k++)
                                        {
                                            if (!int.TryParse(temp[k], out _) && !lex.BoolValues().Contains(temp[k]) && !lex.IfConditionOperator().Contains(temp[k]) &&
                                                !lex.SpecialTokens().Contains(temp[k]) && !lex.MathTokens().Contains(temp[k]) && !lex.Operators().Contains(temp[k]) && 
                                                temp[k] != "(" && temp[k] != ")" && temp[k] != "@" && !lex.ConditionOperators().Contains(temp[k]))
                                                {
                                                    bad = true;
                                                    continue;
                                                }

                                            if (temp[k] == "=")
                                                bad = true;
                                        }
                                    }
                                }                                    
                            }
                        }
                    }

                    for (int j = 1; j < currentTokens.Count() && bad == false; j++)
                    {
                        if(lex.Operators().Contains(currentTokens.ElementAt(j)))
                        {
                            if (!int.TryParse(currentTokens.ElementAt(j-1), out _) && 
                                !lex.SpecialTokens().Contains(currentTokens.ElementAt(j-1)) &&
                                currentTokens.ElementAt(j-1) != lex.MathTokens().ElementAt(lex.MathTokens().Count()-1) && 
                                currentTokens.ElementAt(j-1) != ")")
                            {
                                bad = true;
                                continue;
                            }

                            if (!int.TryParse(currentTokens.ElementAt(j+1), out _) && !lex.SpecialTokens().Contains(currentTokens.ElementAt(j+1)) &&
                                !lex.MathTokens().Contains(currentTokens.ElementAt(j+1)) && currentTokens.ElementAt(j+1) != "let" && currentTokens.ElementAt(j+1) != "if" &&
                                currentTokens.ElementAt(j+1) != "print" && !lex.Functions().Any(x => x.Nombre == currentTokens.ElementAt(j+1)) 
                                && currentTokens.ElementAt(j+1) != "(")
                            {
                                bad = true;
                            }
                        }
                        else if(currentTokens.ElementAt(j) == "cos" || currentTokens.ElementAt(j) == "sin")                                                         //Chequeo de instruccion if
                        {
                            if(currentTokens.ElementAt(j+1) != "(")
                            {
                                bad = true;
                                continue;
                            }

                            bool balanced = false;
                            int tempos = 1;
                            int closePosition = 0; 
                            do{
                                closePosition = currentTokens.FindIndex(j+tempos, x=>x==")");
                                temp = currentTokens.GetRange(j+1, closePosition-j);
                                if (!BalancedParentexis(temp))
                                {
                                    balanced = true;
                                }
                                else
                                {
                                    if(closePosition > -1)
                                        tempos = (closePosition - j) + 1;   
                                    else
                                        break;                                     
                                }
                            }while(balanced == false && j+tempos < currentTokens.Count);

                            if(balanced == true)
                            {
                                List<string> subList = currentTokens.GetRange(j+2, (closePosition) - (j+2));
                                if(subList.Any(x => lex.ConditionOperators().Contains(x)) || subList.Any(x => lex.Tokens().Contains(x)) ||
                                                subList.Any(x => lex.ConditionToken().Contains(x)) || subList.Any(x => x == "=") || 
                                                subList.Any(x => x == "@")  || subList.Any(x => x == "=>"))
                                    bad = true;
                            }       
                            else
                                bad = true; 
                        }
                        else if(currentTokens.ElementAt(j) == "log")          //Chequeo de instruccion if
                        {
                            if(currentTokens.ElementAt(j+1) != "(")
                            {
                                bad = true;
                                continue;
                            }

                            bool balanced = false;
                            int tempos = 1;
                            int closePosition = 0; 
                            do{
                                closePosition = currentTokens.FindIndex(j+tempos, x=>x==")");
                                temp = currentTokens.GetRange(j+1, closePosition-j);
                                if (!BalancedParentexis(temp))
                                {
                                    balanced = true;
                                }
                                else
                                {
                                    if(closePosition > -1)
                                        tempos = (closePosition - j) + 1;   
                                    else
                                        break;                                     
                                }
                            }while(balanced == false && j+tempos < currentTokens.Count);

                            if(balanced == true)
                            {
                                int comaPos = currentTokens.FindIndex(j+1, x => x == ",");

                                if(comaPos > 0 && comaPos < closePosition)
                                {
                                    List<string> subList = currentTokens.GetRange(j+2, (comaPos) - (j+2));
                                    if(subList.Any(x => lex.ConditionOperators().Contains(x)) || subList.Any(x => lex.Tokens().Contains(x)) ||
                                                subList.Any(x => lex.ConditionToken().Contains(x)) || subList.Any(x => x == "=") || 
                                                subList.Any(x => x == ";") || subList.Any(x => x == "@") || subList.Any(x => x == ",") || 
                                                subList.Any(x => x == "=>"))
                                        bad = true;

                                    subList = currentTokens.GetRange(comaPos+1, (closePosition) - (comaPos));
                                    if(subList.Any(x => lex.ConditionOperators().Contains(x)) || subList.Any(x => lex.Tokens().Contains(x)) ||
                                                subList.Any(x => lex.ConditionToken().Contains(x)) || subList.Any(x => x == "=") || 
                                                subList.Any(x => x == ";") || subList.Any(x => x == "@") || subList.Any(x => x == ",") || 
                                                subList.Any(x => x == "=>"))
                                        bad = true;                                            
                                }
                                else
                                    bad = true;
                            }       
                            else
                                bad = true; 
                        }else if(lex.Symbols().Contains(currentTokens.ElementAt(j)))
                        {
                            if (currentTokens.ElementAt(j) == "(")
                            {
                                if (currentTokens.ElementAt(j-1) != "sin" && currentTokens.ElementAt(j-1) != "cos" &&
                                currentTokens.ElementAt(j-1) != "log" && currentTokens.ElementAt(j-1) != "(" && currentTokens.ElementAt(j-1) != "print"
                                && currentTokens.ElementAt(j-1) != "if" && !lex.Operators().Contains(currentTokens.ElementAt(j-1)) &&
                                lex.ConditionOperators().Contains(currentTokens.ElementAt(j-1)))
                                {
                                    bad = true;   
                                    goto Nnnn;
                                }

                                if (currentTokens.ElementAt(j+1) != "sin" && currentTokens.ElementAt(j+1) != "cos" && currentTokens.ElementAt(j+1) != "let" &&
                                currentTokens.ElementAt(j+1) != "log" && currentTokens.ElementAt(j+1) != "(" && currentTokens.ElementAt(j+1) != "print" &&
                                currentTokens.ElementAt(j+1) != "if" && !int.TryParse(currentTokens.ElementAt(j+1), out _) && !lex.SpecialTokens().Contains(currentTokens.ElementAt(j+1))
                                && !lex.BoolValues().Contains(currentTokens.ElementAt(j+1)))
                                {
                                    bad = true;   
                                }
                            }
                            Nnnn:
                            if (currentTokens.ElementAt(j) == ")")
                            {
                                if (currentTokens.ElementAt(j-1) != ")" && currentTokens.ElementAt(j-1) != "PI"
                                && !int.TryParse(currentTokens.ElementAt(j-1), out _) && !lex.SpecialTokens().Contains(currentTokens.ElementAt(j-1))
                                && !lex.BoolValues().Contains(currentTokens.ElementAt(j-1)))
                                {
                                    bad = true;   
                                    goto Nnnn2;
                                }

                                if (currentTokens.ElementAt(j+1) != ")" && currentTokens.ElementAt(j+1) != "PI" && currentTokens.ElementAt(j+1) != "=>" && currentTokens.ElementAt(j+1) != "let"
                                && !int.TryParse(currentTokens.ElementAt(j+1), out _) && !lex.SpecialTokens().Contains(currentTokens.ElementAt(j+1)) && currentTokens.ElementAt(j+1) != "if"
                                && currentTokens.ElementAt(j+1) != "else" && currentTokens.ElementAt(j+1) != "print" && currentTokens.ElementAt(j+1) != ";" && !lex.Functions().Any(x => x.Nombre == currentTokens.ElementAt(j+1))
                                && !lex.Operators().Contains(currentTokens.ElementAt(j+1)) && !lex.ConditionOperators().Contains(currentTokens.ElementAt(j+1)))
                                {
                                    bad = true;   
                                }
                            }
                            Nnnn2:
                            if (currentTokens.ElementAt(j) == "=>")
                            {
                                if (currentTokens.ElementAt(j-1) != ")")
                                {
                                    bad = true;   
                                    goto Nnnn3;
                                }

                                if ((currentTokens.ElementAt(j+1) != "if" && !lex.SpecialTokens().Contains(currentTokens.ElementAt(j+1)) && !lex.MathTokens().Contains(currentTokens.ElementAt(j+1))))
                                {
                                    bad = true;   
                                }
                            }
                            Nnnn3:
                            if (currentTokens.ElementAt(j) == "@")
                            {
                                if (!lex.SpecialTokens().Contains(currentTokens.ElementAt(j-1)) && !int.TryParse(currentTokens.ElementAt(j-1), out _))
                                {
                                    bad = true;   
                                    goto Nnnn4;
                                }

                                if (!lex.SpecialTokens().Contains(currentTokens.ElementAt(j+1)) && !int.TryParse(currentTokens.ElementAt(j+1), out _))
                                {
                                    bad = true;   
                                }
                            }
                            Nnnn4:
                            if (currentTokens.ElementAt(j) == ",")
                            {
                                if (currentTokens.ElementAt(j-1) != ")" && !int.TryParse(currentTokens.ElementAt(j-1), out _) && !lex.SpecialTokens().Contains(currentTokens.ElementAt(j-1)))
                                {
                                    bad = true; 
                                    goto Nnnn5;  
                                }

                                if (bad == false &&!int.TryParse(currentTokens.ElementAt(j+1), out _) && !lex.SpecialTokens().Contains(currentTokens.ElementAt(j+1))
                                )
                                {
                                    bad = true;   
                                }
                            }
                            Nnnn5:
                            if (currentTokens.ElementAt(j) == ".")
                            {
                                if (!int.TryParse(currentTokens.ElementAt(j-1), out _))
                                {
                                    bad = true;   
                                }

                                if (bad == false &&!int.TryParse(currentTokens.ElementAt(j+1), out _))                                
                                {
                                    bad = true;   
                                }
                            }                            
                        }else if (lex.ConditionOperators().Contains(currentTokens.ElementAt(j)))
                        {
                            if (currentTokens.ElementAt(j-1) != ")" && !int.TryParse(currentTokens.ElementAt(j-1), out _) && !lex.SpecialTokens().Contains(currentTokens.ElementAt(j-1)))
                            {
                                bad = true;
                            }
                            if (currentTokens.ElementAt(j+1) != "(" && !int.TryParse(currentTokens.ElementAt(j+1), out _) && !lex.SpecialTokens().Contains(currentTokens.ElementAt(j+1)) && bad == false)
                            {
                                bad = true;
                            }
                        }else if (lex.SpecialTokens().Contains(currentTokens.ElementAt(j)))
                        {
                            if (currentTokens.ElementAt(j-1) != "(" && currentTokens.ElementAt(j-1) != "," && currentTokens.ElementAt(j-1) != "let" &&
                                currentTokens.ElementAt(j-1) != "@" && currentTokens.ElementAt(j-1) != ")" && currentTokens.ElementAt(j-1) != "else" && currentTokens.ElementAt(j-1) != "=>"
                                && currentTokens.ElementAt(j-1) != "function" && currentTokens.ElementAt(j-1) != "in" && !lex.IfConditionOperator().Contains(currentTokens.ElementAt(j-1))
                                && !lex.Operators().Contains(currentTokens.ElementAt(j-1)) && !lex.ConditionOperators().Contains(currentTokens.ElementAt(j-1)))
                            {
                                bad = true;   
                            }
                            if (bad == false && currentTokens.ElementAt(j+1) != "(" && currentTokens.ElementAt(j+1) != "," && currentTokens.ElementAt(j+1) != "in" &&
                                currentTokens.ElementAt(j+1) != "@" && currentTokens.ElementAt(j+1) != ")" && currentTokens.ElementAt(j+1) != "else"
                                && !lex.Operators().Contains(currentTokens.ElementAt(j+1)) && !lex.ConditionOperators().Contains(currentTokens.ElementAt(j+1))
                                && currentTokens.ElementAt(j+1) != ";" && !lex.IfConditionOperator().Contains(currentTokens.ElementAt(j+1)))
                            {
                                bad = true;
                            }
                        }else if (int.TryParse(currentTokens.ElementAt(j), out _))
                        {
                            if (currentTokens.ElementAt(j-1) != "(" && currentTokens.ElementAt(j-1) != "," && currentTokens.ElementAt(j-1) != "else" && currentTokens.ElementAt(j-1) != "in"
                                && currentTokens.ElementAt(j-1) != ")" && currentTokens.ElementAt(j-1) != "@" && !lex.IfConditionOperator().Contains(currentTokens.ElementAt(j-1))
                                && !lex.Operators().Contains(currentTokens.ElementAt(j-1)) && !lex.ConditionOperators().Contains(currentTokens.ElementAt(j-1)))
                            {
                                bad = true;   
                            }
                            if (bad == false && currentTokens.ElementAt(j+1) != "(" && currentTokens.ElementAt(j+1) != "," && currentTokens.ElementAt(j+1) != "in"
                                && !lex.Operators().Contains(currentTokens.ElementAt(j+1)) && !lex.ConditionOperators().Contains(currentTokens.ElementAt(j+1))
                                && currentTokens.ElementAt(j+1) != ")" && currentTokens.ElementAt(j+1) != "@" && !lex.IfConditionOperator().Contains(currentTokens.ElementAt(j+1))
                                && currentTokens.ElementAt(j+1) != ";" && currentTokens.ElementAt(j+1) != "else")
                            {
                                bad = true;
                            }
                        }
                    }
                }
            }
        }
        return bad;
    } 
}