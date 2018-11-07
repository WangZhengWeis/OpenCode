using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ORM;

namespace DAL
{
    public class BaseDal
    {

        protected Database db;
        public BaseDal()
        {
            Type t = this.GetType();
            object[] attrs = t.GetCustomAttributes(typeof(DbConnection), true);
            if (attrs != null && attrs.Length > 0)
            {
                DbConnection methodAttr = (DbConnection)attrs[0];
                db = new Database(methodAttr.Value);
            }
            else
                db = new Database("DBConnection");
        }
        public string queuePath = "FormatName:Direct=OS:" + System.Environment.MachineName + "\\private$\\";

    }
}
