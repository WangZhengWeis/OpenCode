using System;
using System.Linq;
using ORM;
using PetaPoco;
using System.Collections.Generic;

namespace BLL.GaoQing
{
    public class MovieMagnet_BLL:BaseDal
    {

        public int IsMagnet(string magnet)
        {
            try
            {
                var pd = PocoData.ForType(typeof(Model.GaoQing.MovieMagnet), db.DefaultMapper);
                var sql = PetaPoco.Sql.Builder.Select(pd.TableInfo.PrimaryKey).From(pd.TableInfo.TableName).Where(pd.GetColumnName("Magnet") + "=@0", magnet);
                return db.ExecuteScalar<int>(sql);
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public int Add(Model.GaoQing.MovieMagnet model)
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

        public int Update(Model.GaoQing.MovieMagnet model)
        {
            return db.Update(model);
        }

        public List<Model.GaoQing.MovieMagnet> GetMovieMagnets(int movieID)
        {
            return db.Fetch<Model.GaoQing.MovieMagnet>(Sql.Builder.Where("MovieID = @0", movieID));
        }
    }
}
