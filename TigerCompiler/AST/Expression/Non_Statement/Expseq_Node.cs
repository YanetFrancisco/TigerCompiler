using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using System.Reflection;
using System.Reflection.Emit;

namespace TigerCompiler
{
    public class Expseq_Node : NonStatement_Node
    {
        public Scope scp { get; set; }

        public bool Contains_Break { get; set; }//////////////////////////

        #region Constructor
        public Expseq_Node() { }
        #endregion

        #region Methods
        public override void Check_Semantics(Scope scope, Report report)
        {
            Is_Valid = true;

            for (int i = 0; i < ChildCount; i++)
            {
                Expression_Node exp = GetChild(i) as Expression_Node;

                exp.Check_Semantics(scope, report);
                if (exp.Is_Valid)
                {
                    if (exp is NonStatement_Node && (exp as NonStatement_Node).Type_Info.Depth > scope.Depth)
                    {
                        report.AddError(exp.Line, exp.CharPositionInLine, "The specified expression returns a value which is not visible in the current scope.");
                        Type_Info = new Type_Info(Tiger_Type.Error);
                        Is_Valid = false;
                        return;
                    }
                }
                else
                {
                    Is_Valid = false;
                    break;
                }
            }

            if (Is_Valid)
            {
                if (!Contains_Break)
                {
                    Type_Info = new Type_Info(Tiger_Type.Void);

                    if (ChildCount > 0 && GetChild(ChildCount - 1) is NonStatement_Node)
                        if (GetChild(ChildCount - 1) is Value_Node && (GetChild(ChildCount - 1) as Value_Node).Is_Assign)
                            Type_Info = new Type_Info(Tiger_Type.Void);
                        else
                            Type_Info = ((NonStatement_Node)GetChild(ChildCount - 1)).Type_Info;
                }
                //lista de expressions que contienen un break_node, en la expseq
                //List<Expression_Node> inner_breaks = Contain_Children_That(x => x is Break_Node).ToList();

                //if (inner_breaks.Count() > 0)
                    //Type_Info = new Type_Info(Tiger_Type.Void);
            }
            else
                Type_Info = new Type_Info(Tiger_Type.Error);
            scp = scope;
        }

        public override void Generate_Code(IL_Generator g)
        {
            NonStatement_Node child;
            bool isStatement = false;
            if (ChildCount > 0)
            {
                for (int i = 0; i < ChildCount - 1; i++)
                {
                    (GetChild(i) as Expression_Node).Generate_Code(g);
                    isStatement = GetChild(i) is Statement_Node;
                    if (!isStatement)
                    {
                        child = GetChild(i) as NonStatement_Node;
                        if (child != null && child.Type_Info.Basic_Type != Tiger_Type.Void)
                            g.Tiger_Emit(OpCodes.Pop);
                    }
                }
                (GetChild(ChildCount - 1) as Expression_Node).Generate_Code(g);

                isStatement = GetChild(ChildCount - 1) is Statement_Node;
                if (!isStatement)
                {
                    child = GetChild(ChildCount - 1) as NonStatement_Node;
                    if (Type_Info.Basic_Type == Tiger_Type.Void && child != null && child.Type_Info.Basic_Type != Tiger_Type.Void)
                    {
                        g.Tiger_Emit(OpCodes.Pop);
                    }
                }
            }
        }
        #endregion
    }
}
