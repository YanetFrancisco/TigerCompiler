using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TigerCompiler
{
    public class Procedure_Info : Tiger_Info
    {
        public bool Return_Value { get; set; }

        public Type_Info Return_Type { get; protected set; }

        public Dictionary<string, Variable_Info> Params { get; set; }

        public Procedure_Info(Dictionary<string, Variable_Info> list_params, Type_Info return_Type)
        {
            if (return_Type != null)
            {
                Return_Value = true;
                Return_Type = return_Type;
            }

            Params = list_params;
        }
		
		public List<Variable_Info> Params_info()
		{
		    return new List<Variable_Info>(Params.Values);
		}

        public bool Match_Params_Types(List<Type_Info> params_Types)
        {
            if (Params.Count != params_Types.Count)
                return false;

            List<Variable_Info> values = new List<Variable_Info>(Params.Values);
            for (int i = 0; i < values.Count; i++)
                if (!values[i].Equals(params_Types[i]))
                    return false;
            return true;
        }
    }
}
