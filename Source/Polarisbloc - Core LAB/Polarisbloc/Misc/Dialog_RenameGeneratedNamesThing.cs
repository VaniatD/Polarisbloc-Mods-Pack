using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using System.Reflection;

namespace Polarisbloc
{
    public  class Dialog_RenameGeneratedNamesThing : Dialog_Rename
    {
        private CompGeneratedNames compName;

        public Dialog_RenameGeneratedNamesThing(CompGeneratedNames compName)
        {
            this.compName = compName;
            this.curName = (string)typeof(CompGeneratedNames).GetField("name", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(compName);
        }

        protected override void SetName(string name)
        {
            typeof(CompGeneratedNames).GetField("name", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(this.compName, name);
        }
    }
}
