using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using System.Reflection;
using System.Reflection.Emit;

namespace TigerCompiler
{
    public class Break_Node : Flow_Control_Node
    {
        public Statement_Node Owner { get; private set; }

        public Scope scp { get; set; }

        #region Constructor
        public Break_Node() { }
        #endregion

        #region Methods
        public override void Check_Semantics(Scope scope, Report report)
        {
            Is_Valid = true;

            foreach (var node in Get_Nodes_To_Root())
            {
                var exprSeq = node as Expseq_Node;
                if (exprSeq != null)
                {
                    exprSeq.Type_Info.Basic_Type = Tiger_Type.Void;
                    exprSeq.Contains_Break = true;
                }

                if (node is For_Node )
                {
                    Owner = node as For_Node;
                    break;
                }
                if (node is While_Node)
                {
                    Owner = node as While_Node;
                    break;
                }

                if (node is Funcdec_Node)
                {
                    report.AddError(Line, CharPositionInLine, "Break loop control structure not found within function.");
                    break;
                }
            }


            if (Owner == null)
            {
                report.AddError(Line, CharPositionInLine, "A break statement can only be used within a while or for context.");
                Is_Valid = false;
                return;
            }
            scp = scope;
        }

        public override void Generate_Code(IL_Generator g)
        {
            Label break_label;
            if (Owner != null)
            {
                if (Owner is For_Node)
                    break_label = ((For_Node)Owner).Last_Label;
                else
                    break_label = ((While_Node)Owner).Last_Label;
                g.Tiger_Emit(OpCodes.Br, break_label);
            }    
        }
        #endregion
    }
}
