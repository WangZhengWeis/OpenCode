using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ORM
{
    public class DbConnection : Attribute
    {
        public DbConnection(string description)
        {
            Value = description;
        }
        public string Value { get; private set; }
    }
}
