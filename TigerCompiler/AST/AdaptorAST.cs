using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Antlr.Runtime;
using Antlr.Runtime.Tree;

using System.Reflection;
using TigerCompiler.Grammar;

namespace TigerCompiler.AST
{
    class Adaptor_AST : CommonTreeAdaptor
    {
        #region Constructor
        public Adaptor_AST() : base() { }
        #endregion

        #region Methods
        public override object Create(IToken token)
        {
            if (token == null)
                //return new Nil_Node(token);
                return base.Create(null);

            //FieldInfo[] fields = typeof(tigerParser).GetFields();
            //foreach (var field in fields)
            //{
            //    if(field.IsStatic && (int)field.GetRawConstantValue() == token.Type)
            //    {
            //        //string name = string.Format("TigerCompiler.AST.{0}_Node", field.Name[0] + field.Name.Substring(1).ToLower());
            //        //Type type = Assembly.GetExecutingAssembly().GetType(name);
            //        //return Activator.CreateInstance(type, token);


            //    }
            //}
            switch (token.Type)
            {

                case tigerParser.AACCESS:
                    return new Aaccess_Node() { Token = token };
                case tigerParser.ACCESS:
                    return new Access_Node() { Token = token };
                case tigerParser.AND:
                    return new And_Node() { Token = token };
                case tigerParser.ARRAYDEC:
                    return new Arraydec_Node() { Token = token };
                case tigerParser.ARRAYTYPE:
                    return new Arraytype_Node() { Token = token };
                case tigerParser.ASSIGN:
                    return new Assign_Node() { Token = token };
                case tigerParser.BREAK:
                    return new Break_Node() { Token = token };
                case tigerParser.CALL:
                    return new Call_Node() { Token = token };
                case tigerParser.DECLIST:
                    return new Declist_Node() { Token = token };
                case tigerParser.DIV:
                    return new Div_Node() { Token = token };
                case tigerParser.EQ:
                    return new Eq_Node() { Token = token };
                case tigerParser.EXPLIST:
                    return new Explist_Node() { Token = token };
                case tigerParser.EXPSEQ:
                    return new Expseq_Node() { Token = token };
                case tigerParser.FIELD:
                    return new Field_Node() { Token = token };
                case tigerParser.FIELDLIST:
                    return new Fieldlist_Node() { Token = token };
                case tigerParser.FOR:
                    return new For_Node() { Token = token };
                case tigerParser.FUNC:
                    return new Func_Node() { Token = token };
                case tigerParser.FUNCDEC:
                    return new Funcdec_Node() { Token = token };
                case tigerParser.FUNCBLOCK:
                    return new Funcblock_Node() { Token = token };
                case tigerParser.GT:
                    return new Gt_Node() { Token = token };
                case tigerParser.GTEQ:
                    return new Gteq_Node() { Token = token };
                case tigerParser.ID:
                    return new Id_Node() { Token = token };
                case tigerParser.IDACCESS:
                    return new Idaccess_Node() { Token = token };
                case tigerParser.IF:
                    return new If_Node() { Token = token };
                case tigerParser.INT:
                    return new Int_Node() { Token = token };
                case tigerParser.LT:
                    return new Lt_Node() { Token = token };
                case tigerParser.LTEQ:
                    return new Lteq_Node() { Token = token };
                case tigerParser.MINUS:
                    return new Minus_Node() { Token = token };
                case tigerParser.MULT:
                    return new Mult_Node() { Token = token };
                case tigerParser.NEG:
                    return new Neg_Node() { Token = token };
                case tigerParser.NIL:
                    return new Nil_Node() { Token = token };
                case tigerParser.NOTEQ:
                    return new Noteq_Node() { Token = token };
                case tigerParser.OR:
                    return new Or_Node() { Token = token };
                case tigerParser.PLUS:
                    return new Plus_Node() { Token = token };
                case tigerParser.PROC:
                    return new Proc_Node() { Token = token };
                case tigerParser.RECORDDEC:
                    return new Recorddec_Node() { Token = token };
                case tigerParser.RECORDTYPE:
                    return new Recordtype_Node() { Token = token };
                case tigerParser.STRING:
                    return new String_Node() { Token = token };
                case tigerParser.TYPE:
                    return new Type_Node() { Token = token };
                case tigerParser.TYPEDEC:
                    return new Typedec_Node() { Token = token };
                case tigerParser.TYPEFIELD:
                    return new Typefield_Node() { Token = token };
                case tigerParser.TYPEFIELDS:
                    return new Typefields_Node() { Token = token };
                case tigerParser.TYPEBLOCK:
                    return new Typeblock_Node() { Token = token };
                case tigerParser.VALUE:
                    return new Value_Node() { Token = token };
                case tigerParser.VAR:
                    return new Var_Node() { Token = token };
                case tigerParser.VARBLOCK: 
                    return new Varblock_Node() { Token = token };
                case   tigerParser.WHILE:
                    return new While_Node { Token = token };
                default:
                    return base.Create(token);
            }

        }
        #endregion
    }
}
