using ORM;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.RARGB
{
    public class Torrent_BLL : BaseDal
    {
        public int Insert(Model.RARGB.Torrent model)
        {
            try
            {
                db.BeginTransaction();
                var category = SingleCategory(model.Category.CategoryName);
                if (category != null)
                    model.CategoryID = category.CategoryID;
                else
                    model.CategoryID = InsertCategory(model.Category);
                if (model.CategoryID <= 0)
                    throw new Exception("Category写入失败!");
                int id = Convert.ToInt32(db.Insert(model));
                if (id <= 0)
                    throw new Exception("Torrent写入失败!");
                db.CompleteTransaction();
                return id;
            }
            catch (Exception ex)
            {
                db.AbortTransaction();
                return -1;
            }
        }

        public Model.RARGB.Torrent SingleTorrent(string token)
        {
            try
            {
                return db.SingleOrDefault<Model.RARGB.Torrent>(Sql.Builder.Where("[Token]=@0",token));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public int InsertCategory(Model.RARGB.Category model)
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

        public Model.RARGB.Category SingleCategory(string categoryName)
        {
            try
            {
                return db.SingleOrDefault<Model.RARGB.Category>(Sql.Builder.Where("[CategoryName]=@0",categoryName));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string InsertToken(Model.RARGB.RAGGBToken model)
        {
            try
            {
                return Convert.ToString(db.Insert(model));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Model.RARGB.RAGGBToken> GetRAGGBToken(int size)
        {
            try
            {
                var sql = Sql.Builder.Select("TOP(" + size + ") *").From("tb_Token_RARGB").Where("Token not in (Select Token From tb_Torrent_RARGB)").OrderBy("NEWID()");
                return db.Fetch<Model.RARGB.RAGGBToken>(sql);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Model.RARGB.Torrent> GetXXXMovie(int size = 30)
        {
            try
            {
                var sql = Sql.Builder
                    .Select("TOP(" + size + ") tb_Torrent_RARGB.*")
                    .From("tb_Torrent_RARGB")
                    .Where("CategoryID = @0",4)
                    .OrderBy("NEWID()");
                return db.Fetch<Model.RARGB.Torrent>(sql);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
