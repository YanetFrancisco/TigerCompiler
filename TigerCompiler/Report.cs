using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TigerCompiler
{
    public class Report
    {
        public List<string> List_Errors { get; set; }

        #region Constructor
        public Report()
        {
            List_Errors = new List<string>();
        }
        #endregion

        #region Methods
        public void AddError(int line, int column, string text)
        {
            List_Errors.Add("(" + line + ", " + column + "): " + text);
        }
        #endregion
    }
}
