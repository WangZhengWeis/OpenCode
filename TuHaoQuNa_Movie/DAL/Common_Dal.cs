using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL
{
    public class Common_Dal : BaseDal
    {
        public List<T> Query<T>(string strWhere, params object[] parms)
        {
            try
            {
                return db.Fetch<T>(strWhere, parms);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Page<T> Page<T>(long page, long itemsPerPage, string strWhere, params object[] parms)
        {
            try
            {
                return db.Page<T>(page, itemsPerPage, strWhere, parms);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string ExecuteScalar(string strWhere, params object[] parms)
        {
            try
            {
                return db.ExecuteScalar<string>(strWhere, parms);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public int Insert<T>(T model)
        {
            try
            {
                return Convert.ToInt32(db.Insert(model));
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public int Update<T>(T model)
        {
            try
            {
                return Convert.ToInt32(db.Update(model));
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public int Execute(string sql, params object[] parms)
        {
            try
            {
                return db.Execute(sql, parms);
            }
            catch (Exception)
            {
                return -1;
            }
        }
        
    }
}
