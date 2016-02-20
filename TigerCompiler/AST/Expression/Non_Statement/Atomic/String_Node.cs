using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

namespace TigerCompiler
{
    public class String_Node : Atomic_Node
    {
        public Scope scp { get; set; }

        #region Constructor
        public String_Node() 
        {
           
        }
        #endregion

        string Value { get; set; }

        #region Methods
        public override void Check_Semantics(Scope scope, Report report)
        {
            Is_Valid = true;

            string s = Remove_Esc_Chars(Text);
            if (s == null)
            {
                Is_Valid = false;
                Type_Info = new Type_Info(Tiger_Type.Error);
                return;
            }
            Value = s;
            Type_Info = new String_Info();
            scp = scope;
        }

        public string Remove_Esc_Chars(string text)
        {
            var result = new StringBuilder();
            for (int i = 1; i < text.Length - 1; i++)
            {
                if (text[i] != '\\')
                    result.Append(text[i]);
                else
                {
                    i++;
                    if (char.IsWhiteSpace(text[i]))
                        while (char.IsWhiteSpace(text[i]))
                            i++;
                    else if (char.IsDigit(text[i]))
                    {
                        byte value = 0;
                        if (!byte.TryParse(text.Substring(i, 3), out value))//la gramatica se encarga de que sea menor que 128
                            return null;                                    // el tryparse esta de mas pues la gramtica solo permite digitos
                        result.Append((char)value);
                        i += 2;
                    }
                    else
                    {
                        char c = ' ';
                        switch (text[i])
                        {
                            case 'n':
                                c = '\n';
                                break;
                            case 't':
                                c = '\t';
                                break;
                            case 'r':
                                c = '\r';
                                break;
                            case '\\':
                                c = '\\';
                                break;
                            case '\"':
                                c = '\"';
                                break;
                        }
                        result.Append(c);
                    }
                }
            }
            return result.ToString();
        }
        
        public override void Generate_Code(IL_Generator g)
        {
            g.Tiger_Emit(OpCodes.Ldstr, Value);
        }
        #endregion
    }
}
