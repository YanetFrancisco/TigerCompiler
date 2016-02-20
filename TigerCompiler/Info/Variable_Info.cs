using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TigerCompiler
{
    public class Variable_Info : Tiger_Info
    {
        public Type_Info Var_Type { get; protected set; }

        public bool Is_Locked { get; protected set; }

        public Variable_Info(Type_Info var_Type, bool is_Locked)
        {
            Var_Type = var_Type;
            Is_Locked = is_Locked;
        }

        public override bool Equals(object obj)
        {
            Variable_Info var = obj as Variable_Info;
            if (var == null)
            {
                Type_Info type = obj as Type_Info;
                if (type == null)
                    return false;
                else
                    return Var_Type.Equals(type);
            }
            return ID == var.ID && Var_Type.Equals(var.Var_Type);
        }
    }
}
