using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TigerCompiler
{
    public class Nil_Info : Type_Info
    {
        public Nil_Info(): base(Tiger_Type.Nil)
        {
 
        }

        public override bool Equals(object obj)
        {
            return obj is Record_Info || obj is Array_Info || obj is String_Info || (obj is Alias_Info && (obj as Alias_Info).Equals(this));
        }
    }
}
