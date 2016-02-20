using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using System.Reflection;
using System.Reflection.Emit;

namespace TigerCompiler
{
    public class Fieldlist_Node : Statement_Node
    {
        List<Field_Node> fields;

        public Scope scp { get; set; }
		
		bool init;

        public List<Field_Node> Fields
        {
            get
            {
                if (!init)
                {
                    for (int i = 0; i < ChildCount; i++)
                    {
                        fields.Add(GetChild(i) as Field_Node);
                    }
                    init = true;
                }
                return fields;
            }
        }

        #region Constructor
        public Fieldlist_Node()
        {
            fields = new List<Field_Node>();
			init = false;
        }
        #endregion

        #region Methods
        public override void Check_Semantics(Scope scope, Report report)
        {
            Is_Valid = true;

            foreach (var field in Fields)
            {
                field.Check_Semantics(scope, report);
                if (!field.Is_Valid)
                {
                    Is_Valid = false;
                    return;
                } 
            }
            scp = scope;
        }

        public override void Generate_Code(IL_Generator g)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
