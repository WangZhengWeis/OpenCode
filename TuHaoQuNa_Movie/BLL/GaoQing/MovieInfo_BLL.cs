using ORM;
using PetaPoco;
using System;
using System.Linq;
using Utility;
namespace BLL.GaoQing
{
    public class MovieInfo_BLL:BaseDal
    {

        public bool IsID(int id)
        {
            return db.Exists<Model.GaoQing.MovieInfo>(id);
        }

        public int Add(Model.GaoQing.MovieInfo model)
        {
            try
            {
                return Convert.ToInt32(db.Insert(model));
            }
            catch (Exception)
            {
                return -1;
            };
        }

        public int Update(Model.GaoQing.MovieInfo model)
        {
            return db.Update(model);
        }

        public Model.GaoQing.MovieInfo GetMovieInfo(int id)
        {
            return db.SingleOrDefault<Model.GaoQing.MovieInfo>(id);
        }
    }
}
