using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GHDY.Core.DocumentModel
{
    public class Speaker
    {
        public string Name { get; set; }

        public Sex Sex { get; set; }

        public Speaker()
            : this(string.Empty, Sex.Male)
        {

        }

        public Speaker(string name, Sex sex)
        {
            this.Name = name;
            this.Sex = sex;
        }
    }

    public enum Sex
    {
        Male, Female
    }
}
