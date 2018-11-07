using ORM;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL
{
    public class Movie_BLL : BaseDal
    {
        /// <summary>
        /// 获取电影类型列表
        /// </summary>
        /// <returns></returns>
        public List<Model.Category> GetCategoryList()
        {
            try
            {
                return db.Fetch<Model.Category>(Sql.Builder.OrderBy("ID Asc"));
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取国家列表
        /// </summary>
        /// <returns></returns>
        public List<Model.Country> GetCountryList()
        {
            try
            {
                return db.Fetch<Model.Country>(Sql.Builder.OrderBy("ID Asc"));
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
