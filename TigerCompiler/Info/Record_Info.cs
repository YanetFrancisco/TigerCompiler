using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TigerCompiler
{
    public class Record_Info : Type_Info
    {
        public Dictionary<string, Type_Info> Fields { get; set; }

        public Record_Info(): base(Tiger_Type.Record)
        {
            Fields = new Dictionary<string, Type_Info>();
        }

        public void Add_Field(string id, Type_Info type)
        {
            Fields.Add(id, type);
        }

        public Type_Info Get_Field(string id)
        {
            if (Contains_Field(id))
                return Fields[id];
            return null;
        }

        public bool Contains_Field(string id)
        {
            return Fields.ContainsKey(id);
        }

        public bool Match_Fields(Dictionary<string, Type_Info> other_Fields)
        {
            if (Fields.Count != other_Fields.Count)
                return false;

            List<string> id_fields = new List<string>(Fields.Keys);
            List<string> other_id_fields =  new List<string>(other_Fields.Keys);

            List<Type_Info> type_fields = new List<Type_Info>(Fields.Values);
            List<Type_Info> other_type_fields = new List<Type_Info>(other_Fields.Values);

            for (int i = 0; i < Fields.Count; i++)
                if (id_fields[i] != other_id_fields[i] || !type_fields[i].Equals(other_type_fields[i]))
                    return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is Nil_Info || base.Equals(obj);
        }

        public override void Update(Type_Info type)
        {
            base.Update(type);
            Record_Info r = type as Record_Info;
            if (r == null)
                throw new Exception();
            Fields = r.Fields;
        }
    }
}
