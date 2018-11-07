using ORM;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL
{
    public class Player_BLL : BaseDal
    {
        public int Save(Model.Player model)
        {
            try
            {
                var id = GetID(model.VID, model.Platform);
                if (id > 0)
                {
                    model.ID = id;
                    return Update(model);
                }
                else
                    return Convert.ToInt32(db.Insert(model));
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public int GetID(string vid, string platform)
        {
            try
            {
                return db.ExecuteScalar<int>(Sql.Builder.Select("[ID]").From("tb_Player").Where("[VID]=@0 And [Platform] = @1", vid, platform));
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public int Update(Model.Player model)
        {
            try
            {
                return db.Execute("Update tb_Player Set MovieID = @1, Platform = @2, VID = @3, Name = @4, PlayerUrl = @5, Poster = @6, Power = @7, Category = @8, Episodes = @9, LastTime = @10 Where ID = @0", 
                    model.ID,
                    model.MovieID,
                    model.Platform,
                    model.VID,
                    model.Name,
                    model.PlayerUrl,
                    model.Poster,
                    model.Power,
                    model.Category,
                    model.Episodes,
                    DateTime.Now
                    );
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public List<Model.Player> GetPlayerByMovieID(int movieID)
        {
            try
            {
                return db.Fetch<Model.Player>(Sql.Builder.Where("[MovieID] = @0", movieID));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Model.Player SinglePlayer(int id)
        {
            try
            {
                return db.SingleOrDefault<Model.Player>(Sql.Builder.Where("[ID]=@0", id));
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
