using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;

namespace TigerCompiler
{
    public class Typefields_Node : Statement_Node
    {
        List<Typefield_Node> fields;

        public Scope scp { get; set; }

        bool init;

        public List<Typefield_Node> Fields
        {
            get
            {
                if (!init)
                {
                    for (int i = 0; i < ChildCount; i++)
                    {
                        fields.Add(GetChild(i) as Typefield_Node);
                    }
                    init = true;
                }
                return fields;
            }
        }

        #region Constructor
        public Typefields_Node()
        {
            fields = new List<Typefield_Node>();
            init = false;
        }
        #endregion

        #region Methods
        public override void Check_Semantics(Scope scope, Report report)
        {
            Is_Valid = true;

            foreach (var field in Fields)
            {
                //verifica que no existan 2 o mas campos con el mismo nombre
                if (Fields.Count(x => x.Name_Field.Text == field.Name_Field.Text) > 1)
                {
                    report.AddError(field.Name_Field.Line, field.Name_Field.CharPositionInLine, "The name " + field.Name_Field.Text + " appears more than once.");
                    Is_Valid = false;
                    return;
                }
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
