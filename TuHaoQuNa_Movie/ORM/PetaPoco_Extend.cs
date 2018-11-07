using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PetaPoco
{
    public partial class Database
    {
        public List<TRet> Fetch<T1, T2, TRet>(long page, long itemsPerPage, Func<T1, T2, TRet> cb, string sql, params object[] args)
        {
            return SkipTake<T1, T2, TRet>((page - 1) * itemsPerPage, itemsPerPage, cb, sql, args);
        }

        public List<TRet> Fetch<T1, T2, TRet>(long page, long itemsPerPage, Func<T1, T2, TRet> cb, Sql sql)
        {
            return SkipTake<T1, T2, TRet>((page - 1) * itemsPerPage, itemsPerPage, cb, sql.SQL, sql.Arguments);
        }

        public List<TRet> Fetch<T1, T2, T3, TRet>(long page, long itemsPerPage, Func<T1, T2, T3, TRet> cb, Sql sql)
        {
            return SkipTake<T1, T2, T3, TRet>((page - 1) * itemsPerPage, itemsPerPage, cb, sql.SQL, sql.Arguments);
        }

        public List<TRet> SkipTake<T1, T2, TRet>(long skip, long take, Func<T1, T2, TRet> cb, string sql, params object[] args)
        {
            string sqlCount, sqlPage;
            BuildPageQueries<T1>(skip, take, sql, ref args, out sqlCount, out sqlPage);
            return Fetch<T1, T2, TRet>(cb, sqlPage, args);
        }

        public List<TRet> SkipTake<T1, T2, T3, TRet>(long skip, long take, Func<T1, T2, T3, TRet> cb, string sql, params object[] args)
        {
            string sqlCount, sqlPage;
            BuildPageQueries<T1>(skip, take, sql, ref args, out sqlCount, out sqlPage);
            return Fetch<T1, T2, T3, TRet>(cb, sqlPage, args);
        }
    }
}
