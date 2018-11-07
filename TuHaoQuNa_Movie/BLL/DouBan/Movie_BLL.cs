using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetaPoco;
using Utillity;
using Utility;
using System.Net;

namespace BLL.DouBan
{
    public class Movie_BLL : BaseDal
    {
        public string Insert(Model.DouBan.Movie model)
        {
            try
            {
                db.BeginTransaction();
                //Countrys
                if (model.Countrys!=null)
                    model.Countrys.ForEach((Action<Model.DouBan.Country>)(m => {
                        if (m != null)
                        {
                            var country = SingleCountry(m.CountryName);
                            if (country == null)
                            {
                                m.ID = InsertCountry(m);
                                if (m.ID<=0)
                                    throw new Exception("Country数据写入失败！");
                            }
                            else
                                m.ID = country.ID;
                            var relation_moviecountry_id = InsertRelation_MovieCountry(new Model.DouBan.Relation_MovieCountry() { CountryID = m.ID, DouBanID = model.DouBanID });
                            if (relation_moviecountry_id <= 0)
                                throw new Exception("Relation_MovieCountry数据写入失败！");
                        }
                    }));
                //Director
                if (model.Director!=null)
                    model.Director.ForEach(m => {
                        if (m!=null)
                        {
                            m = AddCelebrity(m);
                            var relation_moviedirector_id = InsertRelation_MovieDirector(new Model.DouBan.Relation_MovieDirector() { DirectorID = m.CelebrityID, DouBanID = model.DouBanID });
                            if (relation_moviedirector_id <= 0)
                                throw new Exception("Relation_MovieDirector数据写入失败！");
                        }
                    });
                //FilmGenres
                if (model.FilmGenres!=null)
                    model.FilmGenres.ForEach(m => {
                        if (m!=null)
                        {
                            var filmgenre = SingleFilmGenre(m.FilmGenreName);
                            if (filmgenre == null)
                            {
                                m.ID = InsertFilmGenre(m);
                                if (m.ID<=0)
                                    throw new Exception("FilmGenre数据写入失败！");
                            }
                            else
                                m.ID = filmgenre.ID;
                            var relation_moviefilmgenre_id = InsertRelation_MovieFilmGenre(new Model.DouBan.Relation_MovieFilmGenre() { FilmGenreID = m.ID, DouBanID = model.DouBanID });
                            if (relation_moviefilmgenre_id <= 0)
                                throw new Exception("Relation_MovieFilmGenre数据写入失败！");
                        }
                    });
                //Languages
                if (model.Languages!=null)
                    model.Languages.ForEach(m => {
                        if (m!=null)
                        {
                            var language = SingleLanguage(m.LanguageName);
                            if (language == null)
                            {
                                m.ID = InsertLanguage(m);
                                if (m.ID<=0)
                                    throw new Exception("Language数据写入失败！");
                            }
                            else
                                m.ID = language.ID;
                            var relation_movielanguage_id = InsertRelation_MovieLanguage(new Model.DouBan.Relation_MovieLanguage() { LanguageID = m.ID, DouBanID = model.DouBanID });
                            if (relation_movielanguage_id <= 0)
                                throw new Exception("Relation_MovieLanguage数据写入失败！");
                        }
                    });
                //MoreName
                if (model.MoreName!=null)
                    model.MoreName.ForEach(m => {
                        if (m!=null)
                        {
                            m.DouBanID = model.DouBanID;
                            m.ID = InsertMovieName(m);
                            if (m.ID<=0)
                                throw new Exception("MovieName数据写入失败！");
                        }
                    });
                //Performer
                if (model.Performer!=null)
                    model.Performer.ForEach(m => {
                        if (m!=null)
                        {
                            m = AddCelebrity(m);
                            var relation_movieperformer_id = InsertRelation_MoviePerformer(new Model.DouBan.Relation_MoviePerformer() { PerformerID = m.CelebrityID, DouBanID = model.DouBanID });
                            if (relation_movieperformer_id <= 0)
                                throw new Exception("Relation_MoviePerformer数据写入失败！");
                        }
                    });
                //ReleaseTimes
                if (model.ReleaseTimes!=null)
                    model.ReleaseTimes.ForEach(m => {
                        if (m!=null)
                        {
                            m.DouBanID = model.DouBanID;
                            m.ID = InsertReleaseTime(m);
                            if (m.ID <= 0)
                                throw new Exception("ReleaseTime数据写入失败！");
                        }
                    });
                //Screenwriter
                if (model.Screenwriter!=null)
                    model.Screenwriter.ForEach(m => {
                        if (m!=null)
                        {
                            m = AddCelebrity(m);
                            var relation_moviescreenwriter_id = InsertRelation_MovieScreenwriter(new Model.DouBan.Relation_MovieScreenwriter() { ScreenwriterID = m.CelebrityID, DouBanID = model.DouBanID });
                            if (relation_moviescreenwriter_id <= 0)
                                throw new Exception("Relation_MovieScreenwriter数据写入失败！");
                        }
                    });
                //Poster
                if (model.Poster != null)
                {
                    var image = SingleImage(model.Poster.Large, model.Poster.Medium, model.Poster.Small);
                    if (image == null)
                    {
                        model.Poster.ID = InsertImage(model.Poster);
                        if (model.Poster.ID <= 0)
                            throw new Exception("Poster数据写入失败！");
                    }
                    else
                        model.Poster.ID = image.ID;
                    model.PosterID = model.Poster.ID;
                }
                else
                    model.PosterID = null;
                string id = Convert.ToString(db.Insert(model));
                if (!string.IsNullOrEmpty(id))
                    db.CompleteTransaction();
                else
                    throw new Exception("Movie数据写入失败！");
                return Convert.ToString(id);
            }
            catch (Exception ex)
            {
                db.AbortTransaction();
                return null;
            }
        }

        public Model.DouBan.Celebrity AddCelebrity(Model.DouBan.Celebrity model)
        {
            var celebrity = SingleCelebrity(model.CelebrityID);
            if (celebrity == null)
            {
                if (model.Occupations != null)
                    model.Occupations.ForEach(o => {
                        if (o != null)
                        {
                            var occupation = SingleOccupation(o.OccupationName);
                            if (occupation == null)
                            {
                                o.ID = InsertOccupation(o);
                                if (o.ID <= 0)
                                    throw new Exception("Occupation数据写入失败！");
                            }
                            else
                                o.ID = occupation.ID;
                            var relation_celebrityoccupation_id = InsertRelation_CelebrityOccupation(new Model.DouBan.Relation_CelebrityOccupation() { CelebrityID = model.CelebrityID, OccupationID = o.ID });
                            if (relation_celebrityoccupation_id <= 0)
                                throw new Exception("Relation_CelebrityOccupation数据写入失败！");
                        }
                    });
                if (model.Picture != null)
                {
                    var image = SingleImage(model.Picture.Large, model.Picture.Medium, model.Picture.Small);
                    if (image == null)
                    {
                        model.Picture.ID = InsertImage(model.Picture);
                        if (model.Picture.ID <= 0)
                            throw new Exception("Picture数据写入失败！");
                    }
                    else
                        model.Picture.ID = image.ID;
                    model.PictureID = model.Picture.ID;
                }
                else
                    model.PictureID = null;
                model.CelebrityID = InsertCelebrity(model);
                if (string.IsNullOrEmpty(model.CelebrityID))
                    throw new Exception("Celebrity数据写入失败！");
            }
            else
                model.CelebrityID = celebrity.CelebrityID;
            return model;
        }

        public int InsertImage(Model.DouBan.Image model)
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
        public int InsertRelation_CelebrityOccupation(Model.DouBan.Relation_CelebrityOccupation model)
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
        public int InsertInnerText(Model.DouBan.ReleaseTime model)
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

        public int InsertRelation_MovieScreenwriter(Model.DouBan.Relation_MovieScreenwriter model)
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

        public int InsertRelation_MoviePerformer(Model.DouBan.Relation_MoviePerformer model)
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

        public int InsertRelation_MovieLanguage(Model.DouBan.Relation_MovieLanguage model)
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

        public int InsertRelation_MovieFilmGenre(Model.DouBan.Relation_MovieFilmGenre model)
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

        public int InsertRelation_MovieDirector(Model.DouBan.Relation_MovieDirector model)
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

        public int InsertRelation_MovieCountry(Model.DouBan.Relation_MovieCountry model)
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

        public int InsertReleaseTime(Model.DouBan.ReleaseTime model)
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

        public int InsertOccupation(Model.DouBan.Occupation model)
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

        public int InsertMovieName(Model.DouBan.MovieName model)
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

        public int InsertLanguage(Model.DouBan.Language model)
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

        public int InsertFilmGenre(Model.DouBan.FilmGenre model)
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

        public int InsertCountry(Model.DouBan.Country model)
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

        public string InsertCelebrity(Model.DouBan.Celebrity model)
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

        public Model.DouBan.Celebrity SingleCelebrity(string id)
        {
            try
            {
                return db.SingleOrDefault<Model.DouBan.Celebrity>(Sql.Builder.Where("[CelebrityID]=@0",id));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Model.DouBan.FilmGenre SingleFilmGenre(string genre)
        {
            try
            {
                return db.SingleOrDefault<Model.DouBan.FilmGenre>(Sql.Builder.Where("[FilmGenreName]=@0", genre));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Model.DouBan.Country SingleCountry(string countryName)
        {
            try
            {
                return db.SingleOrDefault<Model.DouBan.Country>(Sql.Builder.Where("[CountryName]=@0",countryName));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Model.DouBan.Language SingleLanguage(string languageName)
        {
            try
            {
                return db.SingleOrDefault<Model.DouBan.Language>(Sql.Builder.Where("[LanguageName]=@0", languageName));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Model.DouBan.Occupation SingleOccupation(string occupationName)
        {
            try
            {
                return db.SingleOrDefault<Model.DouBan.Occupation>(Sql.Builder.Where("[OccupationName]=@0", occupationName));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Model.DouBan.MovieName SingleMovieName(string movieTitle)
        {
            try
            {
                return db.SingleOrDefault<Model.DouBan.MovieName>(Sql.Builder.Where("[MovieTitle]=@0", movieTitle));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Model.DouBan.Image SingleImage(string large, string medium, string small)
        {
            try
            {
                return db.SingleOrDefault<Model.DouBan.Image>(Sql.Builder.Where("[Large]=@0 And [Medium]=@1 And [Small]=@2", large, medium, small));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Model.DouBan.FilmGenre> GetFilmGenreList()
        {
            try
            {
                return db.Fetch<Model.DouBan.FilmGenre>("");
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Model.DouBan.Country> GetCountryList()
        {
            try
            {
                return db.Fetch<Model.DouBan.Country>("");
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<string> GetYearList()
        {
            try
            {
                var sql = Sql.Builder.Select("Year(ReleaseDate) as [Year]").From("tb_ReleaseTime_DouBan").Where("ReleaseDate is not null").GroupBy("Year(ReleaseDate)").OrderBy("[Year] Desc");
                return db.Fetch<string>(sql);
            }
            catch (Exception)
            {
                return null;
            }
        }


        public List<Model.DouBan.Movie> GetNewMovie(int page, int pageSize, int? filmGenreID = null, int? countryID = null, int? directorID = null, int? screenwriterID = null, int? performerID = null, string year = null)
        {
            try
            {
                var strWhere = "";
                if (filmGenreID.HasValue)
                    strWhere += " And tb_Movie_DouBan.[DouBanID] in (Select [DouBanID] From [tb_Relation_MovieFilmGenre_DouBan] Where [FilmGenreID] = @0)";
                if (countryID.HasValue)
                    strWhere += " And tb_Movie_DouBan.[DouBanID] in (Select [DouBanID] From [tb_Relation_MovieCountry_DouBan] Where [CountryID] = @1)";
                if (directorID.HasValue)
                    strWhere += " And tb_Movie_DouBan.[DouBanID] in (Select [DouBanID] From [tb_Relation_MovieDirector_DouBan] Where [DirectorID] = @2)";
                if (performerID.HasValue)
                    strWhere += " And tb_Movie_DouBan.[DouBanID] in (Select [DouBanID] From [tb_Relation_MoviePerformer_DouBan] Where [PerformerID] = @3)";
                if (!string.IsNullOrWhiteSpace(year))
                    strWhere += " And YEAR([ReleaseDate]) = @4";
                if (screenwriterID.HasValue)
                    strWhere += " And tb_Movie_DouBan.[DouBanID] in (Select [DouBanID] From [tb_Relation_MovieScreenwriter_DouBan] Where [ScreenwriterID] = @6)";

                var sql = Sql.Builder
                    .Select("tb_Movie_DouBan.DouBanID, tb_Movie_DouBan.Title, tb_Movie_DouBan.LongTime, tb_Movie_DouBan.Score, tb_Movie_DouBan.PosterID, tb_Movie_DouBan.IMDbID,tb_Image_DouBan.*,tb_ReleaseTime_DouBan.ReleaseDate")
                    .From("tb_Movie_DouBan")
                    .LeftJoin("tb_ReleaseTime_DouBan").On("tb_ReleaseTime_DouBan.DouBanID = tb_Movie_DouBan.DouBanID")
                    .InnerJoin("tb_Image_DouBan").On("tb_Image_DouBan.ID = tb_Movie_DouBan.PosterID")
                    .Where("[ReleaseDate] <= @5 " + strWhere, filmGenreID, countryID, directorID, performerID, year, DateTime.Now, screenwriterID);
                var sql_str = "Select DouBanID, Title, LongTime, Score, PosterID, IMDbID, ID, Large, Medium, Small"
                    + " From ("
                    + " Select *, ROW_NUMBER() Over (Partition By DouBanID Order By ReleaseDate Desc) As rownum "
                    + " From (" + sql.SQL + ") As t1"
                    + ") As t2"
                    + " Where rownum = 1"
                    + " Order By ReleaseDate Desc";

                return db.Fetch<Model.DouBan.Movie, Model.DouBan.Image, Model.DouBan.Movie>(page, pageSize, (movie, image) => { movie.Poster = image; return movie; }, sql_str, sql.Arguments);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<Model.DouBan.Movie> GetScoreMovie(int page, int pageSize, int? filmGenreID = null, int? countryID = null, int? directorID = null, int? performerID = null, string year = null)
        {
            try
            {
                var strWhere = "";
                if (filmGenreID.HasValue)
                    strWhere += " And tb_Movie_DouBan.[DouBanID] in (Select [DouBanID] From [tb_Relation_MovieFilmGenre_DouBan] Where [FilmGenreID] = @0)";
                if (countryID.HasValue)
                    strWhere += " And tb_Movie_DouBan.[DouBanID] in (Select [DouBanID] From [tb_Relation_MovieCountry_DouBan] Where [CountryID] = @1)";
                if (directorID.HasValue)
                    strWhere += " And tb_Movie_DouBan.[DouBanID] in (Select [DouBanID] From [tb_Relation_MovieDirector_DouBan] Where [DirectorID] = @2)";
                if (performerID.HasValue)
                    strWhere += " And tb_Movie_DouBan.[DouBanID] in (Select [DouBanID] From [tb_Relation_MoviePerformer_DouBan] Where [PerformerID] = @3)";
                if (!string.IsNullOrWhiteSpace(year))
                    strWhere += " And YEAR([ReleaseDate]) = @4";

                var sql = Sql.Builder
                    .Select("tb_Movie_DouBan.DouBanID, tb_Movie_DouBan.Title, tb_Movie_DouBan.LongTime, tb_Movie_DouBan.Score, tb_Movie_DouBan.PosterID, tb_Movie_DouBan.IMDbID,tb_Image_DouBan.*,tb_ReleaseTime_DouBan.ReleaseDate")
                    .From("tb_Movie_DouBan")
                    .LeftJoin("tb_ReleaseTime_DouBan").On("tb_ReleaseTime_DouBan.DouBanID = tb_Movie_DouBan.DouBanID")
                    .InnerJoin("tb_Image_DouBan").On("tb_Image_DouBan.ID = tb_Movie_DouBan.PosterID")
                    .Where("[ReleaseDate] <= @5 " + strWhere, filmGenreID, countryID, directorID, performerID, year, DateTime.Now);
                var sql_str = "Select DouBanID, Title, LongTime, Score, PosterID, IMDbID, ID, Large, Medium, Small"
                    + " From ("
                    + " Select *, ROW_NUMBER() Over (Partition By DouBanID Order By ReleaseDate Desc) As rownum "
                    + " From (" + sql.SQL + ") As t1"
                    + ") As t2"
                    + " Where rownum = 1"
                    + " Order By Score Desc";

                return db.Fetch<Model.DouBan.Movie, Model.DouBan.Image, Model.DouBan.Movie>(page, pageSize, (movie, image) => { movie.Poster = image; return movie; }, sql_str, sql.Arguments);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<Model.DouBan.Movie> GetHigMovie(int page, int pageSize, int? filmGenreID = null, int? countryID = null, int? directorID = null, int? performerID = null, string year = null)
        {
            try
            {
                var list_billBoard = GetBillBoard(Model.DouBan.EnumBillBoard.豆瓣高分);
                list_billBoard = list_billBoard.Where(w => (w.Sort >= (page - 1) * pageSize)).Take(pageSize).ToList();
                var ids = list_billBoard.Select(s => s.ID).ToArray();
                var cs = "";
                for (int i = 0; i < ids.Length; i++)
                    cs += "@" + i + ",";
                cs = cs.TrimEnd(',');

                var strWhere = "";
                if (filmGenreID.HasValue)
                    strWhere += " And tb_Movie_DouBan.[DouBanID] in (Select [DouBanID] From [tb_Relation_MovieFilmGenre_DouBan] Where [FilmGenreID] = @0)";
                if (countryID.HasValue)
                    strWhere += " And tb_Movie_DouBan.[DouBanID] in (Select [DouBanID] From [tb_Relation_MovieCountry_DouBan] Where [CountryID] = @1)";
                if (directorID.HasValue)
                    strWhere += " And tb_Movie_DouBan.[DouBanID] in (Select [DouBanID] From [tb_Relation_MovieDirector_DouBan] Where [DirectorID] = @2)";
                if (performerID.HasValue)
                    strWhere += " And tb_Movie_DouBan.[DouBanID] in (Select [DouBanID] From [tb_Relation_MoviePerformer_DouBan] Where [PerformerID] = @3)";
                if (!string.IsNullOrWhiteSpace(year))
                    strWhere += " And YEAR([ReleaseDate]) = @4";

                var sql = Sql.Builder
                    .Select("tb_Movie_DouBan.DouBanID, tb_Movie_DouBan.Title, tb_Movie_DouBan.LongTime, tb_Movie_DouBan.Score, tb_Movie_DouBan.PosterID, tb_Movie_DouBan.IMDbID,tb_Image_DouBan.*,tb_ReleaseTime_DouBan.ReleaseDate")
                    .From("tb_Movie_DouBan")
                    .LeftJoin("tb_ReleaseTime_DouBan").On("tb_ReleaseTime_DouBan.DouBanID = tb_Movie_DouBan.DouBanID")
                    .InnerJoin("tb_Image_DouBan").On("tb_Image_DouBan.ID = tb_Movie_DouBan.PosterID")
                    .Where("[ReleaseDate] <= @5 And tb_Movie_DouBan.DouBanID IN (@6)" + strWhere, filmGenreID, countryID, directorID, performerID, year, DateTime.Now, ids);
                var sql_str = "Select DouBanID, Title, LongTime, Score, PosterID, IMDbID, ID, Large, Medium, Small"
                    + " From ("
                    + " Select *, ROW_NUMBER() Over (Partition By DouBanID Order By ReleaseDate Desc) As rownum "
                    + " From (" + sql.SQL + ") As t1"
                    + ") As t2"
                    + " Where rownum = 1"
                    + " Order By ReleaseDate Desc";

                return db.Fetch<Model.DouBan.Movie, Model.DouBan.Image, Model.DouBan.Movie>(page, pageSize, (movie, image) => { movie.Poster = image; return movie; }, sql_str, sql.Arguments);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Model.DouBan.Movie> GetHotMovie(int page, int pageSize, int? filmGenreID = null, int? countryID = null, int? directorID = null, int? performerID = null, string year = null)
        {
            try
            {
                var list_billBoard = GetBillBoard(Model.DouBan.EnumBillBoard.热门);
                list_billBoard = list_billBoard.Where(w => (w.Sort >= (page - 1) * pageSize)).Take(pageSize).ToList();
                var ids = list_billBoard.Select(s => s.ID).ToArray();
                var cs = "";
                for (int i = 0; i < ids.Length; i++)
                    cs += "@" + i + ",";
                cs = cs.TrimEnd(',');

                var strWhere = "";
                if (filmGenreID.HasValue)
                    strWhere += " And tb_Movie_DouBan.[DouBanID] in (Select [DouBanID] From [tb_Relation_MovieFilmGenre_DouBan] Where [FilmGenreID] = @0)";
                if (countryID.HasValue)
                    strWhere += " And tb_Movie_DouBan.[DouBanID] in (Select [DouBanID] From [tb_Relation_MovieCountry_DouBan] Where [CountryID] = @1)";
                if (directorID.HasValue)
                    strWhere += " And tb_Movie_DouBan.[DouBanID] in (Select [DouBanID] From [tb_Relation_MovieDirector_DouBan] Where [DirectorID] = @2)";
                if (performerID.HasValue)
                    strWhere += " And tb_Movie_DouBan.[DouBanID] in (Select [DouBanID] From [tb_Relation_MoviePerformer_DouBan] Where [PerformerID] = @3)";
                if (!string.IsNullOrWhiteSpace(year))
                    strWhere += " And YEAR([ReleaseDate]) = @4";

                var sql = Sql.Builder
                    .Select("tb_Movie_DouBan.DouBanID, tb_Movie_DouBan.Title, tb_Movie_DouBan.LongTime, tb_Movie_DouBan.Score, tb_Movie_DouBan.PosterID, tb_Movie_DouBan.IMDbID,tb_Image_DouBan.*,tb_ReleaseTime_DouBan.ReleaseDate")
                    .From("tb_Movie_DouBan")
                    .LeftJoin("tb_ReleaseTime_DouBan").On("tb_ReleaseTime_DouBan.DouBanID = tb_Movie_DouBan.DouBanID")
                    .InnerJoin("tb_Image_DouBan").On("tb_Image_DouBan.ID = tb_Movie_DouBan.PosterID")
                    .Where("[ReleaseDate] <= @5 And tb_Movie_DouBan.DouBanID IN (@6)" + strWhere, filmGenreID, countryID, directorID, performerID, year, DateTime.Now, ids);
                var sql_str = "Select DouBanID, Title, LongTime, Score, PosterID, IMDbID, ID, Large, Medium, Small"
                    + " From ("
                    + " Select *, ROW_NUMBER() Over (Partition By DouBanID Order By ReleaseDate Desc) As rownum "
                    + " From (" + sql.SQL + ") As t1"
                    + ") As t2"
                    + " Where rownum = 1"
                    + " Order By ReleaseDate Desc";

                return db.Fetch<Model.DouBan.Movie, Model.DouBan.Image, Model.DouBan.Movie>(page, pageSize, (movie, image) => { movie.Poster = image; return movie; }, sql_str, sql.Arguments);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Model.DouBan.Movie> GetClassicMovie(int page, int pageSize, int? filmGenreID = null, int? countryID = null, int? directorID = null, int? performerID = null, string year = null)
        {
            try
            {
                var list_billBoard = GetBillBoard(Model.DouBan.EnumBillBoard.经典);
                list_billBoard = list_billBoard.Where(w => (w.Sort >= (page - 1) * pageSize)).Take(pageSize).ToList();
                var ids = list_billBoard.Select(s => s.ID).ToArray();
                var cs = "";
                for (int i = 0; i < ids.Length; i++)
                    cs += "@" + i + ",";
                cs = cs.TrimEnd(',');

                var strWhere = "";
                if (filmGenreID.HasValue)
                    strWhere += " And tb_Movie_DouBan.[DouBanID] in (Select [DouBanID] From [tb_Relation_MovieFilmGenre_DouBan] Where [FilmGenreID] = @0)";
                if (countryID.HasValue)
                    strWhere += " And tb_Movie_DouBan.[DouBanID] in (Select [DouBanID] From [tb_Relation_MovieCountry_DouBan] Where [CountryID] = @1)";
                if (directorID.HasValue)
                    strWhere += " And tb_Movie_DouBan.[DouBanID] in (Select [DouBanID] From [tb_Relation_MovieDirector_DouBan] Where [DirectorID] = @2)";
                if (performerID.HasValue)
                    strWhere += " And tb_Movie_DouBan.[DouBanID] in (Select [DouBanID] From [tb_Relation_MoviePerformer_DouBan] Where [PerformerID] = @3)";
                if (!string.IsNullOrWhiteSpace(year))
                    strWhere += " And YEAR([ReleaseDate]) = @4";

                var sql = Sql.Builder
                    .Select("tb_Movie_DouBan.DouBanID, tb_Movie_DouBan.Title, tb_Movie_DouBan.LongTime, tb_Movie_DouBan.Score, tb_Movie_DouBan.PosterID, tb_Movie_DouBan.IMDbID,tb_Image_DouBan.*,tb_ReleaseTime_DouBan.ReleaseDate")
                    .From("tb_Movie_DouBan")
                    .LeftJoin("tb_ReleaseTime_DouBan").On("tb_ReleaseTime_DouBan.DouBanID = tb_Movie_DouBan.DouBanID")
                    .InnerJoin("tb_Image_DouBan").On("tb_Image_DouBan.ID = tb_Movie_DouBan.PosterID")
                    .Where("[ReleaseDate] <= @5 And tb_Movie_DouBan.DouBanID IN (@6)" + strWhere, filmGenreID, countryID, directorID, performerID, year, DateTime.Now, ids);
                var sql_str = "Select DouBanID, Title, LongTime, Score, PosterID, IMDbID, ID, Large, Medium, Small"
                    + " From ("
                    + " Select *, ROW_NUMBER() Over (Partition By DouBanID Order By ReleaseDate Desc) As rownum "
                    + " From (" + sql.SQL + ") As t1"
                    + ") As t2"
                    + " Where rownum = 1"
                    + " Order By ReleaseDate Desc";

                return db.Fetch<Model.DouBan.Movie, Model.DouBan.Image, Model.DouBan.Movie>(page, pageSize, (movie, image) => { movie.Poster = image; return movie; }, sql_str, sql.Arguments);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<Model.DouBan.Movie> GetUpsetMovie(int page, int pageSize, int? filmGenreID = null, int? countryID = null, int? directorID = null, int? performerID = null, string year = null)
        {
            try
            {
                var list_billBoard = GetBillBoard(Model.DouBan.EnumBillBoard.冷门佳片);
                list_billBoard = list_billBoard.Where(w => (w.Sort >= (page - 1) * pageSize)).Take(pageSize).ToList();
                var ids = list_billBoard.Select(s => s.ID).ToArray();
                var cs = "";
                for (int i = 0; i < ids.Length; i++)
                    cs += "@" + i + ",";
                cs = cs.TrimEnd(',');

                var strWhere = "";
                if (filmGenreID.HasValue)
                    strWhere += " And tb_Movie_DouBan.[DouBanID] in (Select [DouBanID] From [tb_Relation_MovieFilmGenre_DouBan] Where [FilmGenreID] = @0)";
                if (countryID.HasValue)
                    strWhere += " And tb_Movie_DouBan.[DouBanID] in (Select [DouBanID] From [tb_Relation_MovieCountry_DouBan] Where [CountryID] = @1)";
                if (directorID.HasValue)
                    strWhere += " And tb_Movie_DouBan.[DouBanID] in (Select [DouBanID] From [tb_Relation_MovieDirector_DouBan] Where [DirectorID] = @2)";
                if (performerID.HasValue)
                    strWhere += " And tb_Movie_DouBan.[DouBanID] in (Select [DouBanID] From [tb_Relation_MoviePerformer_DouBan] Where [PerformerID] = @3)";
                if (!string.IsNullOrWhiteSpace(year))
                    strWhere += " And YEAR([ReleaseDate]) = @4";

                var sql = Sql.Builder
                    .Select("tb_Movie_DouBan.DouBanID, tb_Movie_DouBan.Title, tb_Movie_DouBan.LongTime, tb_Movie_DouBan.Score, tb_Movie_DouBan.PosterID, tb_Movie_DouBan.IMDbID,tb_Image_DouBan.*,tb_ReleaseTime_DouBan.ReleaseDate")
                    .From("tb_Movie_DouBan")
                    .LeftJoin("tb_ReleaseTime_DouBan").On("tb_ReleaseTime_DouBan.DouBanID = tb_Movie_DouBan.DouBanID")
                    .InnerJoin("tb_Image_DouBan").On("tb_Image_DouBan.ID = tb_Movie_DouBan.PosterID")
                    .Where("[ReleaseDate] <= @5 And tb_Movie_DouBan.DouBanID IN (@6)" + strWhere, filmGenreID, countryID, directorID, performerID, year, DateTime.Now, ids);
                var sql_str = "Select DouBanID, Title, LongTime, Score, PosterID, IMDbID, ID, Large, Medium, Small"
                    + " From ("
                    + " Select *, ROW_NUMBER() Over (Partition By DouBanID Order By ReleaseDate Desc) As rownum "
                    + " From (" + sql.SQL + ") As t1"
                    + ") As t2"
                    + " Where rownum = 1"
                    + " Order By ReleaseDate Desc";

                return db.Fetch<Model.DouBan.Movie, Model.DouBan.Image, Model.DouBan.Movie>(page, pageSize, (movie, image) => { movie.Poster = image; return movie; }, sql_str, sql.Arguments);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Model.DouBan.BillBoard> GetBillBoard(Model.DouBan.EnumBillBoard type)
        {
            var obj = CacheHelper.GetCache("BillBoard_" + Convert.ToInt32(type));
            if (obj == null)
            {
                CacheHelper.InsertCache("BillBoard_" + Convert.ToInt32(type), CollectionDouBanTop(type), (12 * 60 * 60));
                return GetBillBoard(type);
            }
            else
            {
                return obj as List<Model.DouBan.BillBoard>;
            }
        }

        List<Model.DouBan.BillBoard> CollectionDouBanTop(Model.DouBan.EnumBillBoard type, int timeout = 30000)
        {
            try
            {
                List<Model.DouBan.BillBoard> list = new List<Model.DouBan.BillBoard>();
                string res_html = null;
                CookieCollection res_cookie = null;
                var url = "https://movie.douban.com/j/search_subjects?type=movie&sort=recommend&page_limit=100000&tag=" + Enum.GetName(typeof(Model.DouBan.EnumBillBoard), type);
                new HttpWebResponseUtility().SendRequest(
                        url,
                        null,
                        null,
                        null,
                        null,
                        "GET",
                        out res_html,
                        out res_cookie,
                        Encoding.UTF8,
                        timeout
                    );
                var jobj = res_html.ParseJSONJObject();
                var job_list = jobj["subjects"].ToList();
                for (int i = 0; i < job_list.Count; i++)
                {
                    list.Add(new Model.DouBan.BillBoard()
                    {
                        ID = job_list[i]["id"].ToString(),
                        Sort = i + 1
                    });
                }
                return list;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public Model.DouBan.Movie GetMovieFullInfo(string doubanID)
        {
            try
            {
                var movie = db.SingleOrDefault<Model.DouBan.Movie>(Sql.Builder.Where("[DouBanID] = @0", doubanID));
                movie.Countrys = GetCountry(doubanID);
                movie.Director = GetDirector(doubanID);
                movie.FilmGenres = GetFilmGenre(doubanID);
                movie.Languages = GetLanguage(doubanID);
                movie.MoreName = GetMoreName(doubanID);
                movie.Performer = GetPerformer(doubanID);
                movie.Poster = GetPoster(movie.PosterID.Value);
                movie.ReleaseTimes = GetReleaseTime(doubanID);
                movie.Screenwriter = GetScreenwriter(doubanID);

                return movie;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<Model.DouBan.Country> GetCountry(string doubanID)
        {
            try
            {
                return db.Fetch<Model.DouBan.Country>(Sql.Builder.Where("[ID] in (Select [CountryID] From tb_Relation_MovieCountry_DouBan Where DouBanID = @0)", doubanID));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Model.DouBan.Celebrity> GetDirector(string doubanID)
        {
            try
            {
                return db.Fetch<Model.DouBan.Celebrity>(Sql.Builder.Where("[CelebrityID] in (Select [DirectorID] From tb_Relation_MovieDirector_DouBan Where DouBanID = @0)", doubanID));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Model.DouBan.FilmGenre> GetFilmGenre(string doubanID)
        {
            try
            {
                return db.Fetch<Model.DouBan.FilmGenre>(Sql.Builder.Where("[ID] in (Select [FilmGenreID] From tb_Relation_MovieFilmGenre_DouBan Where DouBanID = @0)", doubanID));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Model.DouBan.Language> GetLanguage(string doubanID)
        {
            try
            {
                return db.Fetch<Model.DouBan.Language>(Sql.Builder.Where("[ID] in (Select LanguageID From tb_Relation_MovieLanguage_DouBan Where DouBanID = @0)", doubanID));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Model.DouBan.MovieName> GetMoreName(string doubanID)
        {
            try
            {
                return db.Fetch<Model.DouBan.MovieName>(Sql.Builder.Where("[DouBanID] = @0", doubanID));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Model.DouBan.Celebrity> GetPerformer(string doubanID)
        {
            try
            {
                return db.Fetch<Model.DouBan.Celebrity>(Sql.Builder.Where("[CelebrityID] in (Select [PerformerID] From tb_Relation_MoviePerformer_DouBan Where DouBanID = @0)", doubanID));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Model.DouBan.Image GetPoster(int posterID)
        {
            try
            {
                return db.SingleOrDefault<Model.DouBan.Image>(Sql.Builder.Where("[ID]=@0", posterID));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Model.DouBan.ReleaseTime> GetReleaseTime(string doubanID)
        {
            try
            {
                return db.Fetch<Model.DouBan.ReleaseTime>(Sql.Builder.Where("DouBanID=@0", doubanID));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Model.DouBan.Celebrity> GetScreenwriter(string doubanID)
        {
            try
            {
                return db.Fetch<Model.DouBan.Celebrity>(Sql.Builder.Where("[CelebrityID] in (Select [ScreenwriterID] From tb_Relation_MovieScreenwriter_DouBan Where DouBanID = @0)", doubanID));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Model.DouBan.Movie> GetFilmWithDirectorMovie(string doubanID, int size = 5)
        {
            try
            {
                var sql = Sql.Builder
                    .Select("TOP("+ size + ") tb_Movie_DouBan.*, tb_Image_DouBan.*")
                    .From("tb_Movie_DouBan")
                    .InnerJoin("tb_Image_DouBan")
                    .On("tb_Image_DouBan.ID=tb_Movie_DouBan.PosterID")
                    .Where("tb_Movie_DouBan.DouBanID in (Select DouBanID From tb_Relation_MovieDirector_DouBan Where DouBanID != @0 And DirectorID in (Select DirectorID From tb_Relation_MovieDirector_DouBan Where DouBanID = @0))", doubanID)
                    .OrderBy("NEWID()");
                return db.Fetch<Model.DouBan.Movie, Model.DouBan.Image, Model.DouBan.Movie>((movie, image) => { movie.Poster = image; return movie; }, sql);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<Model.DouBan.Movie> GetStarringFilmMovie(string doubanID, int size = 5)
        {
            try
            {
                var sql = Sql.Builder
                    .Select("TOP(" + size + ") tb_Movie_DouBan.*, tb_Image_DouBan.*")
                    .From("tb_Movie_DouBan")
                    .InnerJoin("tb_Image_DouBan")
                    .On("tb_Image_DouBan.ID=tb_Movie_DouBan.PosterID")
                    .Where("tb_Movie_DouBan.DouBanID in (Select DouBanID From tb_Relation_MoviePerformer_DouBan Where DouBanID != @0 And PerformerID in (Select PerformerID From tb_Relation_MoviePerformer_DouBan Where DouBanID = @0))", doubanID)
                    .OrderBy("NEWID()");
                return db.Fetch<Model.DouBan.Movie, Model.DouBan.Image, Model.DouBan.Movie>((movie, image) => { movie.Poster = image; return movie; }, sql);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<Model.DouBan.Movie> GetFilmOfTheSameGenreMovie(string doubanID, int size = 5)
        {
            try
            {
                var sql = Sql.Builder
                    .Select("TOP(" + size + ") tb_Movie_DouBan.*, tb_Image_DouBan.*")
                    .From("tb_Movie_DouBan")
                    .InnerJoin("tb_Image_DouBan")
                    .On("tb_Image_DouBan.ID=tb_Movie_DouBan.PosterID")
                    .Where("tb_Movie_DouBan.DouBanID in (Select DouBanID From tb_Relation_MovieFilmGenre_DouBan Where DouBanID != @0 And FilmGenreID in (Select FilmGenreID From tb_Relation_MovieFilmGenre_DouBan Where DouBanID = @0))", doubanID)
                    .OrderBy("NEWID()");
                return db.Fetch<Model.DouBan.Movie, Model.DouBan.Image, Model.DouBan.Movie>((movie, image) => { movie.Poster = image; return movie; }, sql);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Model.DouBan.BaseMovie SingleMovie(string doubanID)
        {
            try
            {
                return db.SingleOrDefault<Model.DouBan.BaseMovie>(Sql.Builder.Where("[DouBanID]=@0", doubanID));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Model.DouBan.Movie> SearchMovie(int page, int pageSize, string q)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                {
                    throw new Exception();
                }

                var sql = Sql.Builder
                    .Select("tb_Movie_DouBan.*, tb_Image_DouBan.*")
                    .From("tb_Movie_DouBan")
                    .InnerJoin("tb_Image_DouBan")
                    .On("tb_Image_DouBan.ID=tb_Movie_DouBan.PosterID")
                    .Where("[Title] like '%'+@0+'%' Or [DouBanID] in (Select DouBanID From tb_MovieName_DouBan Where [MovieTitle] like '%'+@0+'%') " +
                    "Or DouBanID in (Select DouBanID From tb_Relation_MovieDirector_DouBan Where DirectorID in (Select CelebrityID From tb_Celebrity_DouBan Where [Name] like '%'+@0+'%' Or [MoreNameEn] like '%'+@0+'%' Or [MoreNameZh] like '%'+@0+'%')) " +
                    "Or DouBanID in (Select DouBanID From tb_Relation_MoviePerformer_DouBan Where PerformerID in (Select CelebrityID From tb_Celebrity_DouBan Where [Name] like '%'+@0+'%' Or [MoreNameEn] like '%'+@0+'%' Or [MoreNameZh] like '%'+@0+'%')) " +
                    "Or DouBanID in (Select DouBanID From tb_Relation_MovieFilmGenre_DouBan Where FilmGenreID in (Select CelebrityID From tb_Celebrity_DouBan Where [Name] like '%'+@0+'%' Or [MoreNameEn] like '%'+@0+'%' Or [MoreNameZh] like '%'+@0+'%'))", q);

                return db.Fetch<Model.DouBan.Movie, Model.DouBan.Image, Model.DouBan.Movie>(page, pageSize, (movie, image) => {
                    movie.Poster = image;
                    movie.Countrys = GetCountry(movie.DouBanID);
                    movie.Director = GetDirector(movie.DouBanID);
                    movie.FilmGenres = GetFilmGenre(movie.DouBanID);
                    movie.Languages = GetLanguage(movie.DouBanID);
                    movie.MoreName = GetMoreName(movie.DouBanID);
                    movie.Performer = GetPerformer(movie.DouBanID);
                    movie.ReleaseTimes = GetReleaseTime(movie.DouBanID);
                    movie.Screenwriter = GetScreenwriter(movie.DouBanID);
                    return movie;
                }, sql);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
