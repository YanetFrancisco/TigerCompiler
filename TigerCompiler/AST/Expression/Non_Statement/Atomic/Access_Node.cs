using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using System.Reflection;
using System.Reflection.Emit;

namespace TigerCompiler
{
    public class Access_Node : Atomic_Node
    {
        public Variable_Info Var_Info { get; set; }

        public Id_Node Id { get; set; }

        public bool Is_Assign { get;  protected set; }

        public Scope scp { get; set; }

        #region Constructor
        public Access_Node()
        {
            Var_Info = new Variable_Info(new Type_Info(Tiger_Type.Error), false);
            Id = new Id_Node();
        }
        #endregion

        #region Methods
        public override void Check_Semantics(Scope scope, Report report)
        {
            Is_Valid = true;

            Type_Info info = Var_Info.Var_Type;

            for (int i = 0; i < ChildCount; i++)
            {
                Expression_Node child = GetChild(i) as Expression_Node;

                if (Is_Valid)
                {
                    if (child == GetChild(ChildCount - 1) && child is Assign_Node)
                    {
                        Is_Assign = true;
                        (child as Assign_Node).Var_Info = Var_Info;
                        (child as Assign_Node).Check_Semantics(scope, report);
                        if (child.Is_Valid)
                        {
                            if (!(child as Assign_Node).Expression.Type_Info.Equals(info))
                            {
                                report.AddError(child.Line, child.CharPositionInLine, "The type of the assignment expression must match the type of the left expression.");
                                Is_Valid = false;
                                break;
                            }
                            if ((child as Assign_Node).Expression.Type_Info.Depth > scope.Depth)
                            {
                                report.AddError(child.Line, child.CharPositionInLine, "The assignment expression type is not visible in this scope.");
                                Is_Valid = false;
                                break;
                            }
                        }
                        else
                        {
                            Is_Valid = false;
                            break;
                        }
                    }
                    else
                    {
                        NonStatement_Node exp = child as NonStatement_Node;
                        if (exp == null)
                        {
                            report.AddError(child.Line, child.CharPositionInLine, "The specified expression must return a value.");
                            Is_Valid = false;
                            break;
                        }
                        exp.Check_Semantics(scope, report);
                        if (exp.Is_Valid)
                        {
                            if (info != null)
                            {
                                switch (info.Basic_Type)
                                {
                                    case Tiger_Type.Array:
                                        if (!(exp is Aaccess_Node))
                                        {
                                            report.AddError(exp.Line, exp.CharPositionInLine, "Invalid array access");
                                            Is_Valid = false;
                                        }
                                        else
                                        {
                                            info = info.Nested_Type;
                                            (exp as Aaccess_Node).Type_Info = info;
                                        }
                                        break;

                                    case Tiger_Type.Record:
                                        if (!(exp is Idaccess_Node) || !(info as Record_Info).Contains_Field((exp as Idaccess_Node).Id.Text))
                                        {
                                            report.AddError(exp.Line, exp.CharPositionInLine, "Invalid record access");
                                            Is_Valid = false;
                                        }
                                        else
                                        {
                                            (exp as Idaccess_Node).Record_Id = info.ID;
                                            //(exp as Idaccess_Node).Type_Info = info;
                                            info = (info as Record_Info).Get_Field((exp as Idaccess_Node).Id.Text);
                                            (exp as Idaccess_Node).Type_Info = info;
                                            //(exp as Idaccess_Node).Record_Id =info.ID;
                                        }
                                        break;
                                    default:
                                        report.AddError(exp.Line, exp.CharPositionInLine, "The type of the specified expression must match with the array/record type.");
                                        Is_Valid = false;
                                        break;
                                }
                            }
                        }
                        else
                            Is_Valid = false;
                    }
                }
                else
                    break;
            }
                        
          Type_Info = Is_Valid ? info : new Type_Info(Tiger_Type.Error);
          scp = scope;
        }

        public override void Generate_Code(IL_Generator g)
        {

            if (GetChild(0) is Assign_Node)
            {
                (GetChild(0) as Assign_Node).Generate_Code(g);
                FieldBuilder var = scp.Find_Var(Id.Text);
                g.Tiger_Emit(OpCodes.Stsfld, var);
            }

            else
            {
                if (ChildCount > 0 && GetChild(ChildCount - 1) is Assign_Node)
                {
                    FieldBuilder var = scp.Find_Var(Id.Text);
                    if (var != null)
                        g.Tiger_Emit(OpCodes.Ldsfld, var);
                    

                    for (int i = 0; i < ChildCount - 2; i++)
                        (GetChild(i) as Expression_Node).Generate_Code(g);

                    if (ChildCount > 1 && GetChild(ChildCount - 2) is Aaccess_Node)
                    {
                        var aux = (GetChild(ChildCount - 2) as Aaccess_Node).Type_Info.Basic_Type;
                        Type type = null;
                        switch (aux)
                        {

                            case Tiger_Type.Int:
                                type = typeof(int);
                                break;
                            case Tiger_Type.String:
                                type = typeof(string);
                                break;
                            case Tiger_Type.Array:
                                type = typeof(Array);
                                break;
                            case Tiger_Type.Record:
                                type = typeof(object);
                                break;
                            case Tiger_Type.Nil:
                                type = typeof(object);
                                break;
                        }

                        (GetChild(ChildCount - 2) as Aaccess_Node).Index.Generate_Code(g);
                        //(GetChild(ChildCount - 2) as Expression_Node).Generate_Code(g);  //genera el codigo del aaccess y lo que necesitamos es el index.
                        (GetChild(ChildCount - 1) as Expression_Node).Generate_Code(g);
                         g.Tiger_Emit(OpCodes.Stelem, type);
                    }
                    else
                    {
                        string recordtype = (GetChild(ChildCount - 2) as Idaccess_Node).Record_Id;
                        KeyValuePair<ConstructorBuilder, List<FieldBuilder>> par = scp.Return_Field(recordtype);
                        if (par.Key != null && par.Value != null)
                        {
                            FieldBuilder field = par.Value.Find(x => x.Name == (GetChild(ChildCount - 2) as Idaccess_Node).Id.Text);
                            (GetChild(ChildCount - 1) as Expression_Node).Generate_Code(g);
                            g.Tiger_Emit(OpCodes.Stfld, field);
                        }
                    }
                }
                                           
                else
                {
                    FieldBuilder var = scp.Find_Var(Id.Text);
                    if (var != null)
                        g.Tiger_Emit(OpCodes.Ldsfld, var);
                    for (int i = 0; i < ChildCount ; i++)
                        (GetChild(i) as Expression_Node).Generate_Code(g);
                    //g.Tiger_Emit(OpCodes.Stsfld, var);
                    
                }
            }
        }
        #endregion
    }
}
