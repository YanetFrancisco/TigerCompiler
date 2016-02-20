using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime;
using System.Reflection;
using System.Reflection.Emit;

namespace TigerCompiler
{
    public class Value_Node : Atomic_Node
    {
        public Scope scp { get; set; }

        public Id_Node Id { get { return GetChild(0) as Id_Node; } }
       
        public bool Is_Assign { get; protected set; }

        public NonStatement_Node Right_Side { get { return (ChildCount != 1) ? GetChild(1) as NonStatement_Node : null; } }

        #region Constructor
        public Value_Node()
        {
        }
        #endregion

        #region Methods
        public override void Check_Semantics(Scope scope, Report report)
        {
            Is_Valid = true;

            if (Right_Side != null)
            {
                if (Right_Side is Recorddec_Node)
                {
                    Type_Info type_info = scope.Find_Type_Info(Id.Text, true);

                    if (type_info == null || type_info.Basic_Type != Tiger_Type.Record)
                    {
                        report.AddError(Id.Line, Id.CharPositionInLine,
                            "The record type" + Id.Text + "cannot be found in the current context.");
                        Type_Info = new Type_Info(Tiger_Type.Error);
                        Is_Valid = false;
                        return;
                    }
                    Recorddec_Node record_node = Right_Side as Recorddec_Node;
                    record_node.Id = Id;
                    record_node.Check_Semantics(scope, report);

                    if (record_node.Is_Valid)
                    {
                        Record_Info record = null;
                        if (type_info is Alias_Info)
                            record = (type_info as Alias_Info).Aliased_Type as Record_Info;
                        else
                            record = type_info as Record_Info;

                        if (record_node.Field_List != null)
                        {
                            List<Field_Node> fields = record_node.Field_List.Fields;
                            Dictionary<string, Type_Info> field_types = new Dictionary<string, Type_Info>();

                            foreach (Field_Node field in fields)
                                field_types.Add(field.Name_Field.Text, field.Value_Field.Type_Info);

                            if (!record.Match_Fields(field_types))
                            {
                                report.AddError(Id.Line, Id.CharPositionInLine,
                                    "The record type " + Id.Text + " does not match with the specified fields.");
                                Type_Info = new Type_Info(Tiger_Type.Error);
                                Is_Valid = false;
                                return;
                            }
                        }
                        Type_Info = record;
                    }
                    else
                    {
                        Is_Valid = false;
                        Type_Info = new Type_Info(Tiger_Type.Error);
                    }
                }

                else if (Right_Side is Arraydec_Node)
                {
                    Type_Info type_info = scope.Find_Type_Info(Id.Text, true);

                    if (type_info == null || type_info.Basic_Type != Tiger_Type.Array)
                    {
                        report.AddError(Id.Line, Id.CharPositionInLine,
                            "The type" + Id.Text + "cannot be found in the current context.");
                        Is_Valid = false;
                        return;
                    }
                    Arraydec_Node array_node = Right_Side as Arraydec_Node;
                    array_node.Check_Semantics(scope, report);

                    if (array_node.Is_Valid)
                    {
                        Array_Info array = null;
                        if (type_info is Alias_Info)
                            array = (type_info as Alias_Info).Aliased_Type as Array_Info;
                        else
                            array = type_info as Array_Info;

                        if (!array.Nested_Type.Equals(array_node.Type_Info))
                        {
                            report.AddError(array_node.Array_Type.Line, array_node.Array_Type.CharPositionInLine,
                                "The value returned by the expression must match the array's type.");
                            Is_Valid = false;
                            Type_Info = new Type_Info(Tiger_Type.Error);
                            return;
                        }
                        Type_Info = array;
                    }
                    else
                    {
                        Is_Valid = false;
                        Type_Info = new Type_Info(Tiger_Type.Error);
                    }
                }

                else if (Right_Side is Call_Node)
                {
                    //////////////newwwwwwww las variables ocultan a la funcion
                    Variable_Info var_info = scope.Find_Variable_Info(Id.Text, false);
                    if (var_info != null)
                    {
                        report.AddError(Id.Line, Id.CharPositionInLine,
                           Id.Text + " is a variable, not a procedure/function");
                        Type_Info = new Type_Info(Tiger_Type.Error);
                        Is_Valid = false;
                        return;
                    }
                    /////////////

                    Procedure_Info procedure_info = scope.Find_Procedure_Info(Id.Text);

                    if (procedure_info == null)
                    {
                        report.AddError(Id.Line, Id.CharPositionInLine,
                            "The procedure/function " + Id.Text + " is not defined in the current context.");
                        Type_Info = new Type_Info(Tiger_Type.Error);
                        Is_Valid = false;
                        return;
                    }
                    Call_Node call_node = Right_Side as Call_Node;
                    call_node.Id = Id;
                    call_node.Check_Semantics(scope, report);

                    if (call_node.Is_Valid)
                    {
                        if (call_node.Params != null)
                        {
                            List<NonStatement_Node> expressions = call_node.Params.Expressions;
                            List<Type_Info> infos = new List<Type_Info>();

                            foreach (NonStatement_Node exp in expressions)
                                infos.Add(exp.Type_Info);

                            if (!procedure_info.Match_Params_Types(infos))
                            {
                                report.AddError(Id.Line, Id.CharPositionInLine,
                                    "The parameters specified do not match the parameters required for the procedure/function " +
                                    Id.Text + ".");
                                Is_Valid = false;
                                Type_Info = new Type_Info(Tiger_Type.Error);
                                return;
                            }
                        }
                        if (procedure_info.Return_Value)
                            Type_Info = procedure_info.Return_Type;
                        else
                            Type_Info = new Type_Info(Tiger_Type.Void);

                        call_node.Type_Info = Type_Info;
                    }
                    else
                    {
                        Is_Valid = false;
                        Type_Info = new Type_Info(Tiger_Type.Error);
                    }
                }

                else if (Right_Side is Access_Node)
                {
                    Variable_Info variable_info = scope.Find_Variable_Info(Id.Text,true);

                    if (variable_info == null)
                    {
                        report.AddError(Id.Line, Id.CharPositionInLine,
                            "The variable " + Id.Text + " does not exist in the current context.");
                        Type_Info = new Type_Info(Tiger_Type.Error);
                        Is_Valid = false;
                        return;
                    }
                    Access_Node access_node = Right_Side as Access_Node;
                    access_node.Var_Info = variable_info;
                    access_node.Id = Id;
                    access_node.Check_Semantics(scope, report);

                    Is_Assign = access_node.Is_Assign;
                    if (access_node.Is_Valid)
                    {
                        if (Is_Assign)
                            Type_Info = new Type_Info(Tiger_Type.Void);
                        else
                            Type_Info = access_node.Type_Info;
                    }
                    else
                    {
                        Is_Valid = false;
                        Type_Info = new Type_Info(Tiger_Type.Error);
                    }
                }
            }
            else
            {
                Variable_Info variable_info = scope.Find_Variable_Info(Id.Text, true);

                if (variable_info == null)
                {
                    report.AddError(Id.Line, Id.CharPositionInLine, "The variable " + Id.Text + " does not exist in the current context.");
                    Type_Info = new Type_Info(Tiger_Type.Error);
                    Is_Valid = false;
                    return;
                }
                Type_Info = variable_info.Var_Type;
            }
            scp = scope;
        }

        public override void Generate_Code(IL_Generator g)
        {
            if (Right_Side != null)
            {
                Right_Side.Generate_Code(g);
            }
        }
        #endregion
    }
}
