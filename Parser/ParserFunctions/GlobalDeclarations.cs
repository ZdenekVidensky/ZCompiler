using Lex;
using Parser.ADT;
using Parser.ADT.Functions;
using Parser.ADT.Interfaces;
using Parser.ADT.Operands;
using Parser.ADT.OperationNodes;
using Parser.Precedence;
using Parser.SymbolTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    public static partial class ParserFunctions
    {
        public static Stack<STable> STablesStack = new Stack<STable>();
        public static STable FunctionsST = new STable();
        public static void prog(ADTree tree)
        {

            STablesStack.Push(new STable());
            var printLongFunction = new ADFunctionDeclaration() { Name = "print_long" };
            printLongFunction.Arguments.Add(new ADVariable());

            var printNlFunction = new ADFunctionDeclaration() { Name = "print_nl" };

            var printCharFunction = new ADFunctionDeclaration() { Name = "print_char" };
            printCharFunction.Arguments.Add(new ADVariable());

            FunctionsST.Records.Add(new STRecord() { Access = STAccess.global, Function = printLongFunction, Name = "print_long", Type = STType.function });
            FunctionsST.Records.Add(new STRecord() { Access = STAccess.global, Function = printNlFunction, Name = "print_nl", Type = STType.function });
            FunctionsST.Records.Add(new STRecord() { Access = STAccess.global, Function = printCharFunction, Name = "print_char", Type = STType.function });


            if (MainFSM.PeekNextToken().Type == TokenType.longType)
            {
                var global_declarations = global_decl_list();

                tree.Nodes.Add(global_declarations);
            }

            if(MainFSM.PeekNextToken().Type == TokenType.idType)
            {
                var statements_list = st_main_list();
                foreach (var item in statements_list)
                {
                    tree.Nodes.Add(item);
                }
            }
            
            if(MainFSM.PeekNextToken().Type == TokenType.eofType)
            {
                return;
            }
            else
            {
                SyntaxError("Byla ocekavana deklarace globalni promenne nebo funkce.");
            }
        }

        public static ADDeclaration global_decl_list(ADDeclaration declaration = null)
        {
            if(MainFSM.PeekNextToken().Type == TokenType.longType)
            {
                if (declaration == null)
                {
                    declaration = new ADDeclaration();
                }

                declaration.Variables = declaration.Variables.Concat(glob_decl().ToList()).ToList();
            }
            else if(MainFSM.PeekNextToken().Type == TokenType.idType
                || MainFSM.PeekNextToken().Type == TokenType.eofType)
            {

                STablesStack.Peek().Records.Clear();

                foreach (var item in declaration.Variables)
                {
                    STRecord newRecord = null;

                    if(item.Type == ADVariable.VarType.variable)
                    {
                        newRecord = new STRecord()
                        {
                            Access = STAccess.global,
                            Type = STType.variable,
                            Name = item.Name,
                            Address = (item.STRecord != null) ? item.STRecord.Address : ""
                        };
                    }
                    else if(item.Type == ADVariable.VarType.array)
                    {
                        newRecord = new STRecord()
                        {
                            Access = STAccess.global,
                            Type = STType.array,
                            Name = item.Name,
                            Address = (item.STRecord != null) ? item.STRecord.Address : ""
                        };
                    }

                    item.STRecord = newRecord;
                    STablesStack.Peek().Records.Add(newRecord);
                }

                return declaration;
            }
            else
            {
                SyntaxError("Byla ocekavana deklarace globalni promenne, deklarace funkce nebo konec programu");
            }

            global_decl_list(declaration);

            return declaration;
        }

        public static IEnumerable<ADVariable> glob_decl()
        {
            if (MainFSM.GetNextToken().Type != TokenType.longType)
            {
                SyntaxError("Byla ocekavana deklarace globalni promenne nebo funkce.");
            }

            var id = MainFSM.GetNextToken();

            if (id.Type != TokenType.idType)
            {
                SyntaxError("Byl ocekavan identifikator promenne.");
            }

            var variable = new ADVariable() { Name = id.Attribute };

            variable.STRecord = new STRecord
            {
                Access = STAccess.global,
                Name = variable.Name,
                Type = STType.variable,
                Address = variable.Name
            };

            STablesStack.Peek().Records.Add(variable.STRecord);

            global_decl_type(variable);

            if (variable.ArrayDimensions.Count > 0)
            {
                variable.Type = ADVariable.VarType.array;
            }
            else
            {
                variable.Type = ADVariable.VarType.variable;
            }

            global_assign(variable);

            yield return variable;

            List<ADVariable> variableList = new List<ADVariable>();
            variableList.Add(variable);

            while (true)
            {
                variable = global_variable_list();

                if (variable != null)
                {
                    variableList.Add(variable);
                    yield return variable;
                }
                else
                {
                    foreach(var item in variableList)
                    {
                        if(item.Value == null)
                        {
                            item.Value = variableList.Last().Value;
                        }
                    }
                    break;
                }      
            }

            if(MainFSM.GetNextToken().Type != TokenType.semiType)
            {
                SyntaxError("Byl ocekavan znak \';\'");
            }

            yield break;
        }


        public static ADVariable global_variable_list()
        {
            // Pokud je dalsi token ','
            if(MainFSM.PeekNextToken().Type == TokenType.comType)
            {
                MainFSM.GetNextToken(); // Sezeru ','

                var id = MainFSM.GetNextToken(); // Dalsim znakem musi byt identifikator
                if(id.Type != TokenType.idType)
                {
                    SyntaxError("Byl ocekavan identifikator promenne.");
                }

                var variable = new ADVariable() { Name = id.Attribute };
                global_decl_type(variable);
                global_assign(variable);

                // Pokud jde o pole, nastavim mu takovy typ
                if (variable.ArrayDimensions.Count > 0)
                {
                    variable.Type = ADVariable.VarType.array;
                }
                else
                {
                    variable.Type = ADVariable.VarType.variable;
                }

                return variable;
            }

            return null;
        }

        /// <summary>
        /// Metoda, ktera priradi (pokud je zadana) hodnotu promenne.
        /// </summary>
        /// <param name="variable"></param>
        public static void global_assign(ADVariable variable)
        {
            // Pokud je dalsi token znak /'='/
            if(MainFSM.PeekNextToken().Type == TokenType.asgnType)
            {
                var token = MainFSM.GetNextToken(); // Sezeru '='
                global_assign_value(variable);
            }
        }

        /// <summary>
        /// Metoda, ktera priradi promenne (poli) hodnotu/y
        /// </summary>
        /// <param name="variable"></param>
        public static void global_assign_value(ADVariable variable)
        {     
            // Pokud hodnota zacina na slozenou zavorku '{'
            if (MainFSM.PeekNextToken().Type == TokenType.leftCBType)
            {
                // Pokud slo o pole, je to chyba
                // TODO
                if(variable.Type == ADVariable.VarType.array)
                {
                    SyntaxError("Nelze priradit polozky do jiz deklarovaneho pole s pevnym poctem znaku.");
                }

                MainFSM.GetNextToken(); // Sezeru '{'
                global_array_item(variable);

                if(MainFSM.GetNextToken().Type != TokenType.rightCBType)
                {
                    SyntaxError("Byl ocekavan znak \'}\'");
                }
            }
            // Pokud to neni prirazeni hodnot do pole, prirazuji hodnotu do promenne
            else
            {
                var expression = PrecedenceSyntaxAnalysis.Precedence(TokenType.semiType, true);
                variable.Value = expression;
            }
        }

        public static void global_array_item(ADVariable variable)
        {
            // Pokud neni promenna klasifikovana jako pole, vytvorim z nej pole
            if(variable.ArrayDimensions.Count == 0)
            {
                variable.ArrayDimensions.Add(new ADArrayDimension());
                variable.Type = ADVariable.VarType.array;
            }

            var expression = PrecedenceSyntaxAnalysis.Precedence(TokenType.rightCBType);
            variable.ArrayDimensions[0].Values.Add(expression);

            next_global_array_item(variable);
        }

        public static void next_global_array_item(ADVariable variable)
        {
            var nextToken = MainFSM.PeekNextToken();

            if (nextToken.Type == TokenType.comType)
            {
                MainFSM.GetNextToken(); // Sezeru carku
                global_array_item(variable);
            }

            if(MainFSM.PeekNextToken().Type == TokenType.rightCBType)
            {
                return;
            }
        }

        public static void global_decl_type(ADVariable variable)
        {
            if(MainFSM.PeekNextToken().Type == TokenType.leftABType) // Pokud je dalsi token leva hranata zavorka
            {
                MainFSM.GetNextToken(); // Sezeru levou hranatou zavorku
                // Zavolam precedencni syntakticky analyzator na vyhodnoceni cisla
                var expression = PrecedenceSyntaxAnalysis.Precedence(TokenType.rightABType);

                variable.ArrayDimensions.Add(new ADArrayDimension() { ValuesCount = expression });
                // Dalsi token musi byt prava hranata zavorka, jinak jde o syntaktickou chybu
                if(MainFSM.GetNextToken().Type != TokenType.rightABType)
                {
                    SyntaxError("Byl ocekavan znak /']/'");
                }

                global_decl_type(variable); // Znovu rekurzivne zavolam tuto metodu
            }

            var nextToken = MainFSM.PeekNextToken();
            if(nextToken.Type == TokenType.comType || nextToken.Type == TokenType.semiType || nextToken.Type == TokenType.asgnType)
            {
                return;
            }
            else
            {
                SyntaxError("Byl ocekavan znak \';\' nebo \',\'");
            }
        }

       
        public static void SyntaxError(string text)
        {
            Console.Error.WriteLine($"Syntakticka chyba -> {text}");
            Environment.Exit(2);
        }

        public static void SemanticError(string text)
        {
            Console.Error.WriteLine($"Semanticka chyba -> {text}");
            Environment.Exit(3);
        }

        /// <summary>
        /// Metoda, ktera vrati zaznam (pokud existuje) z globalni nebo lokalni tabulky symbolu. Pokud jde o deklaraci, vyhledavam
        /// jen v lokalni tabulce symbolu
        /// </summary>
        /// <param name="fce"></param>
        /// <param name="decl"></param>
        /// <returns></returns>
        public static STRecord STSearch(string varName, bool decl = false)
        {
            // Pokud jde o deklaraci, hledam jen na aktualni urovni zasobniku tabulek symbolu
            STRecord symbol = null;

            if (decl)
            {
                // U deklarace se prvne podivam, jestli uz neexistuje funkce s timto jmenem, v tom pripade deklarace neni mozna
                symbol = FunctionsST.Records.Where(m => m.Name == varName).FirstOrDefault();

                if(symbol == null)
                {
                    symbol = STablesStack.Peek().Records.Where(m => m.Name == varName).FirstOrDefault();
                }
            }
            else
            {
                // Jestle zkusim vyhledat v tabulce symbolu pro funkce
                symbol = FunctionsST.Records.Where(m => m.Name == varName).FirstOrDefault();

                if(symbol != null)
                {
                    return symbol;
                }

                for (int i = 0; i < STablesStack.Count; i++)
                {
                    symbol = STablesStack.ElementAt(i).Records.Where(m => m.Name == varName).FirstOrDefault();

                    if(symbol != null)
                    {
                        break;
                    }
                }
            }

            return symbol;
        }


        /// <summary>
        /// Metoda, ktera inicializuje tabulku symbolu
        /// </summary>
        public static void Clear()
        {
            STablesStack.Clear();
            FunctionsST.Records.Clear();
        }
    }
}
