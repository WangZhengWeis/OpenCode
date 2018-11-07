﻿using System;
using System.Linq;
using ORM;
using System.Collections.Generic;
using PetaPoco;
using Utility;
using System.Net;
using System.Text;

namespace BLL.GaoQing
{
    public class Movie_BLL : BaseDal
    {
        /// <summary>
        /// 判断这个Hash是否存在，如果存在返回ID
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public int IsHash(string hash)
        {
            try
            {
                var pd = PocoData.ForType(typeof(Model.GaoQing.Movie), db.DefaultMapper);
                var sql = PetaPoco.Sql.Builder.Select(pd.TableInfo.PrimaryKey).From(pd.TableInfo.TableName).Where(pd.GetColumnName("Hash") + "=@0", hash);
                return db.ExecuteScalar<int>(sql);
            }
            catch (Exception)
            {
                return -1;
            }
        }
        /// <summary>
        /// 判断这个下载地址是否存在，如果存在返回ID
        /// </summary>
        /// <param name="magnet"></param>
        /// <param name="torrent"></param>
        /// <returns></returns>
        public int IsTorrent(string magnet, string torrent)
        {
            try
            {
                return db.ExecuteScalar<int>(Sql.Builder.Select("ID").From("tb_MovieTorrent_GaoQing").Where("[Magnet]=@0 And [Torrent]=@1", magnet, torrent));
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// 判断这个下载地址是否存在，如果存在返回ID
        /// </summary>
        /// <param name="download"></param>
        /// <returns></returns>
        public int IsSubtitle(string download)
        {
            try
            {
                return db.ExecuteScalar<int>(Sql.Builder.Select("ID").From("tb_MovieSubtitle_GaoQing").Where("[Download]=@0", download));
            }
            catch (Exception)
            {
                return -1;
            }
        }
        /// <summary>
        /// 插入下载地址
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int InsertTorrent(Model.GaoQing.MovieTorrent model)
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
        /// <summary>
        /// 插入字幕下载地址
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int InsertSubtitle(Model.GaoQing.Subtitle model)
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
        /// <summary>
        /// 插入电影信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddBaseMovie(Model.GaoQing.BaseMovie model)
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
        /// <summary>
        /// 更新电影信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateBaseMovie(Model.GaoQing.BaseMovie model)
        {
            return db.Update(model);
        }
        /// <summary>
        /// 根据ID获取电影信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.GaoQing.Movie SingleMovie(int id)
        {
            try
            {
                return db.SingleOrDefault<Model.GaoQing.Movie>(Sql.Builder.Where("[ID]=@0", id));
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// 根据电影ID更新热度跟评分
        /// </summary>
        /// <param name="id"></param>
        /// <param name="heat"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public int UpdateMovieHeatAndRate(int id, int? heat, decimal? rate)
        {
            try
            {
                return db.Execute("Update tb_Movie_GaoQing Set [Heat] = @1, [Rate] = @2, [LastTime] = @3 Where [ID] = @0", id, heat, rate, DateTime.Now);
            }
            catch (Exception)
            {
                return -1;
            }
        }
        /// <summary>
        /// 添加电影完整信息 事物操作
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddMovie(Model.GaoQing.Movie model)
        {
            try
            {
                db.BeginTransaction();
                int id = AddBaseMovie(model);
                if (id < 0)
                    throw new Exception("写入BaseMovie数据失败");
                model.ID = id;
                if (model.Performer != null)
                    model.Performer.ForEach(m=> {
                        if (m!=null)
                        {
                            var obj = SingleCelebrity(m.Name);
                            if (obj != null)
                                m = obj;
                            else
                                m.ID = InsertCelebrity(m);
                            if (m.ID < 0)
                                throw new Exception("写入Celebrity数据失败");
                            if (!InsertRelation_MoviePerformer(new Model.GaoQing.Relation_MoviePerformer() { CelebrityID = m.ID, MovieID = model.ID }))
                                throw new Exception("写入Relation_MoviePerformer数据失败");
                        }
                    });
                if (model.Director != null)
                    model.Director.ForEach(m=> {
                        if (m != null)
                        {
                            var obj = SingleCelebrity(m.Name);
                            if (obj != null)
                                m = obj;
                            else
                                m.ID = InsertCelebrity(m);
                            if (m.ID < 0)
                                throw new Exception("写入Celebrity数据失败");
                            if (!InsertRelation_MovieDirector(new Model.GaoQing.Relation_MovieDirector() { CelebrityID = m.ID, MovieID = model.ID }))
                                throw new Exception("写入Relation_MovieDirector数据失败");
                        }
                    });
                if (model.Screenwriter != null)
                    model.Screenwriter.ForEach(m=> {
                        if (m != null)
                        {
                            var obj = SingleCelebrity(m.Name);
                            if (obj != null)
                                m = obj;
                            else
                                m.ID = InsertCelebrity(m);
                            if (m.ID < 0)
                                throw new Exception("写入Celebrity数据失败");
                            if (!InsertRelation_MovieScreenwriter(new Model.GaoQing.Relation_MovieScreenwriter() { CelebrityID = m.ID, MovieID = model.ID }))
                                throw new Exception("写入Relation_MovieScreenwriter数据失败");
                        }
                    });
                if (model.Country!=null)
                    model.Country.ForEach(m=> {
                        if (m != null)
                        {
                            var obj = SingleCountry(m.Name);
                            if (obj != null)
                                m = obj;
                            else
                                m.ID = InsertCountry(m);
                            if (m.ID < 0)
                                throw new Exception("写入Country数据失败");
                            if (!InsertRelation_MovieCountry(new Model.GaoQing.Relation_MovieCountry() { CountryID = m.ID, MovieID = model.ID }))
                                throw new Exception("写入Relation_MovieCountry数据失败");
                        }
                    });
                if (model.LongTime != null)
                    model.LongTime.ForEach(m=> {
                        if (m != null)
                        {
                            m.MovieID = model.ID;
                            if (!InsertLongTime(m))
                                throw new Exception("写入LongTime数据失败");
                        }
                    });
                if (model.ReleaseTime != null)
                    model.ReleaseTime.ForEach(m=> {
                        if (m != null)
                        {
                            m.MovieID = model.ID;
                            if (!InsertReleaseTime(m))
                                throw new Exception("写入ReleaseTime数据失败");
                        }
                    });
                if (model.MoreName != null)
                    model.MoreName.ForEach(m=> {
                        if (m != null)
                        {
                            m.MovieID = model.ID;
                            if (!InsertMovieName(m))
                                throw new Exception("写入MoreName数据失败");
                        }
                    });
                if (model.Category != null)
                    model.Category.ForEach(m=> {
                        if (m != null)
                        {
                            var obj = SingleCategory(m.Name);
                            if (obj != null)
                                m = obj;
                            else
                                m.ID = InsertCategory(m);
                            if (m.ID < 0)
                                throw new Exception("写入Category数据失败");
                            if (!InsertRelation_MovieCategory(new Model.GaoQing.Relation_MovieCategory() { CategoryID = m.ID, MovieID = model.ID }))
                                throw new Exception("写入Relation_MovieCategory数据失败");
                        }
                    });
                if (DeleteSeasonByHash(model.Hash)<0)
                    throw new Exception("删除Season数据失败");
                if (model.Season != null)
                {
                    string guid = System.Guid.NewGuid().ToString();
                    model.Season.ForEach(m => {
                        if (m != null)
                        {
                            int movieID = HashToMovieID(m.Hash);
                            if (movieID <= 0)
                                m.MovieID = null;
                            else
                                m.MovieID = movieID;
                            m.SeasonID = guid;

                            if (!InsertSeason(m))
                                throw new Exception("写入Season数据失败");
                        }
                    });
                }
                    

                db.CompleteTransaction();

                return model.ID;
            }
            catch (Exception)
            {
                db.AbortTransaction();
                return -1;
            }
        }
        /// <summary>
        /// 更新电影完整信息 事物操作
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateMovie(Model.GaoQing.Movie model)
        {
            try
            {
                db.BeginTransaction();
                if (UpdateBaseMovie(model) < 0)
                    throw new Exception("写入BaseMovie数据失败");
                if (DeleteRelation_MoviePerformer(model.ID)<=0)
                    throw new Exception("删除Relation_MoviePerformer数据失败");
                if (model.Performer != null)
                    model.Performer.ForEach(m => {
                        if (m != null)
                        {
                            var obj = SingleCelebrity(m.Name);
                            if (obj != null)
                                m = obj;
                            else
                                m.ID = InsertCelebrity(m);
                            if (m.ID < 0)
                                throw new Exception("写入Celebrity数据失败");
                            if (!InsertRelation_MoviePerformer(new Model.GaoQing.Relation_MoviePerformer() { CelebrityID = m.ID, MovieID = model.ID }))
                                throw new Exception("写入Relation_MoviePerformer数据失败");
                        }
                    });
                if (DeleteRelation_MovieDirector(model.ID) < 0)
                    throw new Exception("删除Relation_MovieDirector数据失败");
                if (model.Director != null)
                    model.Director.ForEach(m => {
                        if (m != null)
                        {
                            var obj = SingleCelebrity(m.Name);
                            if (obj != null)
                                m = obj;
                            else
                                m.ID = InsertCelebrity(m);
                            if (m.ID < 0)
                                throw new Exception("写入Celebrity数据失败");
                            if (!InsertRelation_MovieDirector(new Model.GaoQing.Relation_MovieDirector() { CelebrityID = m.ID, MovieID = model.ID }))
                                throw new Exception("写入Relation_MovieDirector数据失败");
                        }
                    });
                if (DeleteRelation_MovieScreenwriter(model.ID) < 0)
                    throw new Exception("删除Relation_MovieScreenwriter数据失败");
                if (model.Screenwriter != null)
                    model.Screenwriter.ForEach(m => {
                        if (m != null)
                        {
                            var obj = SingleCelebrity(m.Name);
                            if (obj != null)
                                m = obj;
                            else
                                m.ID = InsertCelebrity(m);
                            if (m.ID < 0)
                                throw new Exception("写入Celebrity数据失败");
                            if (!InsertRelation_MovieScreenwriter(new Model.GaoQing.Relation_MovieScreenwriter() { CelebrityID = m.ID, MovieID = model.ID }))
                                throw new Exception("写入Relation_MovieScreenwriter数据失败");
                        }
                    });
                if (DeleteRelation_MovieCountry(model.ID) < 0)
                    throw new Exception("删除Relation_MovieCountry数据失败");
                if (model.Country != null)
                    model.Country.ForEach(m => {
                        if (m != null)
                        {
                            var obj = SingleCountry(m.Name);
                            if (obj != null)
                                m = obj;
                            else
                                m.ID = InsertCountry(m);
                            if (m.ID < 0)
                                throw new Exception("写入Country数据失败");
                            if (!InsertRelation_MovieCountry(new Model.GaoQing.Relation_MovieCountry() { CountryID = m.ID, MovieID = model.ID }))
                                throw new Exception("写入Relation_MovieCountry数据失败");
                        }
                    });
                
                if (DeleteLongTime(model.ID) < 0)
                    throw new Exception("删除LongTime数据失败");
                if (model.LongTime != null)
                    model.LongTime.ForEach(m => {
                        if (m != null)
                        {
                            m.MovieID = model.ID;
                            if (!InsertLongTime(m))
                                throw new Exception("写入LongTime数据失败");
                        }
                    });
                if (DeleteReleaseTime(model.ID) < 0)
                    throw new Exception("删除ReleaseTime数据失败");
                if (model.ReleaseTime != null)
                    model.ReleaseTime.ForEach(m => {
                        if (m != null)
                        {
                            m.MovieID = model.ID;
                            if (!InsertReleaseTime(m))
                                throw new Exception("写入ReleaseTime数据失败");
                        }
                    });
                if (DeleteMovieName(model.ID) < 0)
                    throw new Exception("删除MovieName数据失败");
                if (model.MoreName != null)
                    model.MoreName.ForEach(m => {
                        if (m != null)
                        {
                            m.MovieID = model.ID;
                            if (!InsertMovieName(m))
                                throw new Exception("写入MoreName数据失败");
                        }
                    });
                if (DeleteRelation_MovieCategory(model.ID) < 0)
                    throw new Exception("删除Relation_MovieCategory数据失败");
                if (model.Category != null)
                    model.Category.ForEach(m => {
                        if (m != null)
                        {
                            var obj = SingleCategory(m.Name);
                            if (obj != null)
                                m = obj;
                            else
                                m.ID = InsertCategory(m);
                            if (m.ID < 0)
                                throw new Exception("写入Category数据失败");
                            if (!InsertRelation_MovieCategory(new Model.GaoQing.Relation_MovieCategory() { CategoryID = m.ID, MovieID = model.ID }))
                                throw new Exception("写入Relation_MovieCategory数据失败");
                        }
                    });
                if (DeleteSeasonByHash(model.Hash) < 0)
                    throw new Exception("删除Season数据失败");
                if (model.Season != null)
                {
                    string guid = System.Guid.NewGuid().ToString();
                    model.Season.ForEach(m => {
                        if (m != null)
                        {
                            int movieID = HashToMovieID(m.Hash);
                            if (movieID <= 0)
                                m.MovieID = null;
                            else
                                m.MovieID = movieID;
                            m.SeasonID = guid;

                            if (!InsertSeason(m))
                                throw new Exception("写入Season数据失败");
                        }
                    });
                }

                db.CompleteTransaction();

                return model.ID;
            }
            catch (Exception)
            {
                db.AbortTransaction();
                return -1;
            }
        }
        /// <summary>
        /// 更新电影完整信息，BaseMovie中只更新标题、集数、描述信息 事物操作
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateMovieInfo(Model.GaoQing.Movie model)
        {
            try
            {
                db.BeginTransaction();
                if (db.Execute("Update tb_Movie_GaoQing Set [Title] = @1, [Description] = @2, [Episodes] = @3, [LastTime] = @4 Where [ID] = @0", model.ID, model.Title, model.Description, model.Episodes, DateTime.Now) < 0)
                    throw new Exception("写入BaseMovie数据失败");
                if (DeleteRelation_MoviePerformer(model.ID) < 0)
                    throw new Exception("删除Relation_MoviePerformer数据失败");
                if (model.Performer != null)
                    model.Performer.ForEach(m => {
                        if (m != null)
                        {
                            var obj = SingleCelebrity(m.Name);
                            if (obj != null)
                                m = obj;
                            else
                                m.ID = InsertCelebrity(m);
                            if (m.ID < 0)
                                throw new Exception("写入Celebrity数据失败");
                            if (!InsertRelation_MoviePerformer(new Model.GaoQing.Relation_MoviePerformer() { CelebrityID = m.ID, MovieID = model.ID }))
                                throw new Exception("写入Relation_MoviePerformer数据失败");
                        }
                    });
                if (DeleteRelation_MovieDirector(model.ID) < 0)
                    throw new Exception("删除Relation_MovieDirector数据失败");
                if (model.Director != null)
                    model.Director.ForEach(m => {
                        if (m != null)
                        {
                            var obj = SingleCelebrity(m.Name);
                            if (obj != null)
                                m = obj;
                            else
                                m.ID = InsertCelebrity(m);
                            if (m.ID < 0)
                                throw new Exception("写入Celebrity数据失败");
                            if (!InsertRelation_MovieDirector(new Model.GaoQing.Relation_MovieDirector() { CelebrityID = m.ID, MovieID = model.ID }))
                                throw new Exception("写入Relation_MovieDirector数据失败");
                        }
                    });
                if (DeleteRelation_MovieScreenwriter(model.ID) < 0)
                    throw new Exception("删除Relation_MovieScreenwriter数据失败");
                if (model.Screenwriter != null)
                    model.Screenwriter.ForEach(m => {
                        if (m != null)
                        {
                            var obj = SingleCelebrity(m.Name);
                            if (obj != null)
                                m = obj;
                            else
                                m.ID = InsertCelebrity(m);
                            if (m.ID < 0)
                                throw new Exception("写入Celebrity数据失败");
                            if (!InsertRelation_MovieScreenwriter(new Model.GaoQing.Relation_MovieScreenwriter() { CelebrityID = m.ID, MovieID = model.ID }))
                                throw new Exception("写入Relation_MovieScreenwriter数据失败");
                        }
                    });
                if (DeleteRelation_MovieCountry(model.ID) < 0)
                    throw new Exception("删除Relation_MovieCountry数据失败");
                if (model.Country != null)
                    model.Country.ForEach(m => {
                        if (m != null)
                        {
                            var obj = SingleCountry(m.Name);
                            if (obj != null)
                                m = obj;
                            else
                                m.ID = InsertCountry(m);
                            if (m.ID < 0)
                                throw new Exception("写入Country数据失败");
                            if (!InsertRelation_MovieCountry(new Model.GaoQing.Relation_MovieCountry() { CountryID = m.ID, MovieID = model.ID }))
                                throw new Exception("写入Relation_MovieCountry数据失败");
                        }
                    });

                if (DeleteLongTime(model.ID) < 0)
                    throw new Exception("删除LongTime数据失败");
                if (model.LongTime != null)
                    model.LongTime.ForEach(m => {
                        if (m != null)
                        {
                            m.MovieID = model.ID;
                            if (!InsertLongTime(m))
                                throw new Exception("写入LongTime数据失败");
                        }
                    });
                if (DeleteReleaseTime(model.ID) < 0)
                    throw new Exception("删除ReleaseTime数据失败");
                if (model.ReleaseTime != null)
                    model.ReleaseTime.ForEach(m => {
                        if (m != null)
                        {
                            m.MovieID = model.ID;
                            if (!InsertReleaseTime(m))
                                throw new Exception("写入ReleaseTime数据失败");
                        }
                    });
                if (DeleteMovieName(model.ID) < 0)
                    throw new Exception("删除MovieName数据失败");
                if (model.MoreName != null)
                    model.MoreName.ForEach(m => {
                        if (m != null)
                        {
                            m.MovieID = model.ID;
                            if (!InsertMovieName(m))
                                throw new Exception("写入MoreName数据失败");
                        }
                    });
                if (DeleteRelation_MovieCategory(model.ID) < 0)
                    throw new Exception("删除Relation_MovieCategory数据失败");
                if (model.Category != null)
                    model.Category.ForEach(m => {
                        if (m != null)
                        {
                            var obj = SingleCategory(m.Name);
                            if (obj != null)
                                m = obj;
                            else
                                m.ID = InsertCategory(m);
                            if (m.ID < 0)
                                throw new Exception("写入Category数据失败");
                            if (!InsertRelation_MovieCategory(new Model.GaoQing.Relation_MovieCategory() { CategoryID = m.ID, MovieID = model.ID }))
                                throw new Exception("写入Relation_MovieCategory数据失败");
                        }
                    });
                if (DeleteSeasonByHash(model.Hash) < 0)
                    throw new Exception("删除Season数据失败");
                if (model.Season != null)
                {
                    string guid = System.Guid.NewGuid().ToString();
                    model.Season.ForEach(m => {
                        if (m != null)
                        {
                            int movieID = HashToMovieID(m.Hash);
                            if (movieID <= 0)
                                m.MovieID = null;
                            else
                                m.MovieID = movieID;
                            m.SeasonID = guid;

                            if (!InsertSeason(m))
                                throw new Exception("写入Season数据失败");
                        }
                    });
                }

                db.CompleteTransaction();

                return model.ID;
            }
            catch (Exception ex)
            {
                db.AbortTransaction();
                return -1;
            }
        }
        /// <summary>
        /// 插入电影类型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int InsertCategory(Model.GaoQing.Category model)
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
        /// <summary>
        /// 插入电影名称
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool InsertMovieName(Model.GaoQing.MovieName model)
        {
            try
            {
                db.Insert(model);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 插入上映时间
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool InsertReleaseTime(Model.GaoQing.ReleaseTime model)
        {
            try
            {
                db.Insert(model);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 插入电影时长
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool InsertLongTime(Model.GaoQing.LongTime model)
        {
            try
            {
                db.Insert(model);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 插入名人
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int InsertCelebrity(Model.GaoQing.Celebrity model)
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
        /// <summary>
        /// 插入国家
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int InsertCountry(Model.GaoQing.Country model)
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
        /// <summary>
        /// 插入季
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool InsertSeason(Model.GaoQing.Season model)
        {
            try
            {
                db.Insert(model);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 插入明星与电影的关系
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool InsertRelation_MoviePerformer(Model.GaoQing.Relation_MoviePerformer model)
        {
            try
            {
                db.Insert(model);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 插入类型与电影的关系
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool InsertRelation_MovieCategory(Model.GaoQing.Relation_MovieCategory model)
        {
            try
            {
                db.Insert(model);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 插入导演与电影的关系
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool InsertRelation_MovieDirector(Model.GaoQing.Relation_MovieDirector model)
        {
            try
            {
                db.Insert(model);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 插入编剧与电影的关系
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool InsertRelation_MovieScreenwriter(Model.GaoQing.Relation_MovieScreenwriter model)
        {
            try
            {
                db.Insert(model);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 插入国家与电影的关系
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool InsertRelation_MovieCountry(Model.GaoQing.Relation_MovieCountry model)
        {
            try
            {
                db.Insert(model);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 删除明星与电影的关系
        /// </summary>
        /// <param name="movieID">电影ID</param>
        /// <returns></returns>
        public int DeleteRelation_MoviePerformer(int movieID)
        {
            try
            {
                return db.Delete<Model.GaoQing.Relation_MoviePerformer>(Sql.Builder.Where("[MovieID]=@0", movieID));
            }
            catch (Exception)
            {
                return -1;
            }
        }
        /// <summary>
        /// 删除国家与电影的关系
        /// </summary>
        /// <param name="movieID">电影ID</param>
        /// <returns></returns>
        public int DeleteRelation_MovieCountry(int movieID)
        {
            try
            {
                return db.Delete<Model.GaoQing.Relation_MovieCountry>(Sql.Builder.Where("[MovieID]=@0", movieID));
            }
            catch (Exception)
            {
                return -1;
            }
        }
        /// <summary>
        /// 删除导演与电影的关系
        /// </summary>
        /// <param name="movieID">电影ID</param>
        /// <returns></returns>
        public int DeleteRelation_MovieDirector(int movieID)
        {
            try
            {
                return db.Delete<Model.GaoQing.Relation_MovieDirector>(Sql.Builder.Where("[MovieID]=@0", movieID));
            }
            catch (Exception)
            {
                return -1;
            }
        }
        /// <summary>
        /// 删除编剧与电影的关系
        /// </summary>
        /// <param name="movieID">电影ID</param>
        /// <returns></returns>
        public int DeleteRelation_MovieScreenwriter(int movieID)
        {
            try
            {
                return db.Delete<Model.GaoQing.Relation_MovieScreenwriter>(Sql.Builder.Where("[MovieID]=@0", movieID));
            }
            catch (Exception)
            {
                return -1;
            }
        }
        /// <summary>
        /// 删除类型与电影的关系
        /// </summary>
        /// <param name="movieID">电影ID</param>
        /// <returns></returns>
        public int DeleteRelation_MovieCategory(int movieID)
        {
            try
            {
                return db.Delete<Model.GaoQing.Relation_MovieCategory>(Sql.Builder.Where("[MovieID]=@0", movieID));
            }
            catch (Exception)
            {
                return -1;
            }
        }
        /// <summary>
        /// 删除电影的上映时间
        /// </summary>
        /// <param name="movieID">电影ID</param>
        /// <returns></returns>
        public int DeleteReleaseTime(int movieID)
        {
            try
            {
                return db.Delete<Model.GaoQing.ReleaseTime>(Sql.Builder.Where("[MovieID]=@0", movieID));
            }
            catch (Exception)
            {
                return -1;
            }
        }
        /// <summary>
        /// 删除电影的片长
        /// </summary>
        /// <param name="movieID">电影ID</param>
        /// <returns></returns>
        public int DeleteLongTime(int movieID)
        {
            try
            {
                return db.Delete<Model.GaoQing.LongTime>(Sql.Builder.Where("[MovieID]=@0", movieID));
            }
            catch (Exception)
            {
                return -1;
            }
        }
        /// <summary>
        /// 删除电影的名字
        /// </summary>
        /// <param name="movieID">电影ID</param>
        /// <returns></returns>
        public int DeleteMovieName(int movieID)
        {
            try
            {
                return db.Delete<Model.GaoQing.MovieName>(Sql.Builder.Where("[MovieID]=@0", movieID));
            }
            catch (Exception)
            {
                return -1;
            }
        }
        /// <summary>
        /// 删除季组
        /// </summary>
        /// <param name="seasonID">季ID</param>
        /// <returns></returns>
        public int DeleteSeason(string seasonID)
        {
            try
            {
                return db.Delete<Model.GaoQing.Season>(Sql.Builder.Where("[SeasonID]=@0", seasonID));
            }
            catch (Exception)
            {
                return -1;
            }
        }
        /// <summary>
        /// 删除季组
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public int DeleteSeasonByHash(string hash)
        {
            try
            {
                return DeleteSeason(db.SingleOrDefault<string>(Sql.Builder.Select("[SeasonID]").From("tb_Season_GaoQing").Where("[Hash]=@0", hash)));
            }
            catch (Exception)
            {
                return -1;
            }
        }
        /// <summary>
        /// 通过Hash获取MovieID
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public int HashToMovieID(string hash)
        {
            try
            {
                return db.SingleOrDefault<int>(Sql.Builder.Select("[ID]").From("tb_Movie_GaoQing").Where("[Hash]=@0", hash));
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// 获取单个电影类型
        /// </summary>
        /// <param name="name">类型名称</param>
        /// <returns></returns>
        public Model.GaoQing.Category SingleCategory(string name)
        {
            try
            {
                return db.SingleOrDefault<Model.GaoQing.Category>(Sql.Builder.Where("[Name]=@0", name));
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// 获取单个名人
        /// </summary>
        /// <param name="name">名人名称</param>
        /// <returns></returns>
        public Model.GaoQing.Celebrity SingleCelebrity(string name)
        {
            try
            {
                return db.SingleOrDefault<Model.GaoQing.Celebrity>(Sql.Builder.Where("[Name]=@0", name));
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取单个名人
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Model.GaoQing.Celebrity SingleCelebrity(int id)
        {
            try
            {
                return db.SingleOrDefault<Model.GaoQing.Celebrity>(Sql.Builder.Where("[ID]=@0", id));
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// 获取单个国家
        /// </summary>
        /// <param name="name">国家名称</param>
        /// <returns></returns>
        public Model.GaoQing.Country SingleCountry(string name)
        {
            try
            {
                return db.SingleOrDefault<Model.GaoQing.Country>(Sql.Builder.Where("[Name]=@0", name));
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取所有电影的Hash,Name和ID  只含ID,Name跟Hash
        /// </summary>
        /// <returns></returns>
        public List<Model.GaoQing.BaseMovie> GetFullHashAndID()
        {
            try
            {
                return db.Fetch<Model.GaoQing.BaseMovie>(Sql.Builder.Select("ID,Hash,[Name]").From("tb_Movie_GaoQing").OrderBy("CreateTime Desc"));
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        /// <summary>
        /// 分页获取没有下载地址的电影的Hash,Name和ID  只含ID,Name跟Hash
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="family"></param>
        /// <param name="notHasDownload"></param>
        /// <param name="notHasPlayer"></param>
        /// <param name="platform"></param>
        /// <returns></returns>
        public Page<Model.GaoQing.BaseMovie> GetFullHashAndIDByPage(int page, int pageSize, Model.GaoQing.Family? family = null, bool notHasDownload = true, bool notHasPlayer = false, string platform = "")
        {
            try
            {
                string strWhere = "";
                if (family!=null)
                {
                    strWhere += " And [Family] = "+(int)family+"";
                }
                if (notHasDownload)
                {
                    strWhere += " And ID not in (Select MovieID From tb_MovieTorrent_GaoQing Group By MovieID)";
                }
                if (notHasPlayer)
                {
                    strWhere += " And ID not in (Select MovieID From tb_Player Where MovieID is not null "+(string.IsNullOrWhiteSpace(platform)?"": " And [Platform] = '"+platform+"' ") +" Group By MovieID)";
                }
                return db.Page<Model.GaoQing.BaseMovie>(page, pageSize, Sql.Builder.Select("ID,Hash,[Name]").From("tb_Movie_GaoQing").Where("1=1" + strWhere).OrderBy("CreateTime Desc"));
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 根据Hash获取单个BaseMovie对象
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public Model.GaoQing.BaseMovie SingleBaseMovieByHash(string hash)
        {
            try
            {
                return db.SingleOrDefault<Model.GaoQing.BaseMovie>(Sql.Builder.Where("[Hash]=@0", hash));
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 根据Hash获取单个完整的Movie对象
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public Model.GaoQing.Movie SingleMovieFullByHash(string hash)
        {
            try
            {
                Model.GaoQing.Movie model = db.SingleOrDefault<Model.GaoQing.Movie>(Sql.Builder.Where("[Hash]=@0", hash));
                model.Category = GetCategoryByMovieID(model.ID);
                model.Country = GetCountryByMovieID(model.ID);
                model.Director = GetDirectorByMovieID(model.ID);
                model.Performer = GetPerformerByMovieID(model.ID);
                model.Screenwriter = GetScreenwriterByMovieID(model.ID);
                model.LongTime = GetLongTimeByMovieID(model.ID);
                model.MoreName = GetMoreNameByMovieID(model.ID);
                model.ReleaseTime = GetReleaseTimeByMovieID(model.ID);
                model.Season = GetSeasonByMovieID(model.ID);
                return model;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 根据电影ID获取同导演作品
        /// </summary>
        /// <param name="movieID"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public List<Model.GaoQing.BaseMovie> GetFilmWithDirectorMovieByID(int movieID, int size = 5)
        {
            try
            {
                var sql = Sql.Builder
                    .Select("TOP(" + size + ") tb_Movie_GaoQing.*")
                    .From("tb_Movie_GaoQing")
                    .Where("tb_Movie_GaoQing.ID in (Select MovieID From tb_Relation_MovieDirector_GaoQing Where [MovieID] != @0 And CelebrityID in (Select CelebrityID From tb_Relation_MovieDirector_GaoQing Where [MovieID] = @0))", movieID)
                    .OrderBy("NEWID()");
                return db.Fetch<Model.GaoQing.BaseMovie>(sql);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 根据Hash获取同导演作品
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public List<Model.GaoQing.BaseMovie> GetFilmWithDirectorMovieByHash(string hash, int size = 5)
        {
            return GetFilmWithDirectorMovieByID(HashToMovieID(hash), size);
        }


        /// <summary>
        /// 根据电影ID获取同演员作品
        /// </summary>
        /// <param name="movieID"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public List<Model.GaoQing.BaseMovie> GetStarringFilmMovieByID(int movieID, int size = 5)
        {
            try
            {
                var sql = Sql.Builder
                    .Select("TOP(" + size + ") tb_Movie_GaoQing.*")
                    .From("tb_Movie_GaoQing")
                    .Where("tb_Movie_GaoQing.ID in (Select MovieID From tb_Relation_MoviePerformer_GaoQing Where [MovieID] != @0 And CelebrityID in (Select CelebrityID From tb_Relation_MoviePerformer_GaoQing Where [MovieID] = @0))", movieID)
                    .OrderBy("NEWID()");
                return db.Fetch<Model.GaoQing.BaseMovie>(sql);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 根据Hash获取同演员作品
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public List<Model.GaoQing.BaseMovie> GetStarringFilmMovieByHash(string hash, int size = 5)
        {
            return GetStarringFilmMovieByID(HashToMovieID(hash));
        }


        /// <summary>
        /// 根据电影ID获取同类型作品
        /// </summary>
        /// <param name="movieID"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public List<Model.GaoQing.BaseMovie> GetFilmOfTheSameGenreMovieByID(int movieID, int size = 5)
        {
            try
            {
                var sql = Sql.Builder
                    .Select("TOP(" + size + ") tb_Movie_GaoQing.*")
                    .From("tb_Movie_GaoQing")
                    .Where("tb_Movie_GaoQing.ID in (Select MovieID From tb_Relation_MovieCategory_GaoQing Where [MovieID] != @0 And CategoryID in (Select CategoryID From tb_Relation_MovieCategory_GaoQing Where [MovieID] = @0))", movieID)
                    .OrderBy("NEWID()");
                return db.Fetch<Model.GaoQing.BaseMovie>(sql);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 根据Hash获取同类型作品
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public List<Model.GaoQing.BaseMovie> GetFilmOfTheSameGenreMovieByHash(string hash, int size = 5)
        {
            return GetFilmOfTheSameGenreMovieByID(HashToMovieID(hash));
        }

        /// <summary>
        /// 根据电影ID获取下载地址
        /// </summary>
        /// <param name="movieID"></param>
        /// <returns></returns>
        public List<Model.GaoQing.MovieTorrent> GetMovieTorrentByID(int movieID)
        {
            try
            {
                return db.Fetch<Model.GaoQing.MovieTorrent>(Sql.Builder.Where("[MovieID]=@0", movieID).OrderBy("CreateTime Desc"));
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 根据Hash获取下载地址
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public List<Model.GaoQing.MovieTorrent> GetMovieTorrentByHash(string hash)
        {
            return GetMovieTorrentByID(HashToMovieID(hash));
        }

        /// <summary>
        /// 根据电影ID获取字幕
        /// </summary>
        /// <param name="movieID"></param>
        /// <returns></returns>
        public List<Model.GaoQing.Subtitle> GetMovieSubtitleByID(int movieID)
        {
            try
            {
                return db.Fetch<Model.GaoQing.Subtitle>(Sql.Builder.Where("MovieID=@0",movieID).OrderBy("CreateTime Desc"));
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 根据Hash获取字幕
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public List<Model.GaoQing.Subtitle> GetMovieSubtitleByHash(string hash)
        {
            return GetMovieSubtitleByID(HashToMovieID(hash));
        }

        /// <summary>
        /// 根据电影ID获取类型
        /// </summary>
        /// <param name="movieID"></param>
        /// <returns></returns>
        public List<Model.GaoQing.Category> GetCategoryByMovieID(int movieID)
        {
            try
            {
                var sql = Sql.Builder.Where("[ID] in (Select [CategoryID] From tb_Relation_MovieCategory_GaoQing Where [MovieID] = @0)", movieID);
                return db.Fetch<Model.GaoQing.Category>(sql);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 根据电影ID获取国家
        /// </summary>
        /// <param name="movieID"></param>
        /// <returns></returns>
        public List<Model.GaoQing.Country> GetCountryByMovieID(int movieID)
        {
            try
            {
                var sql = Sql.Builder.Where("[ID] in (Select [CountryID] From tb_Relation_MovieCountry_GaoQing Where [MovieID] = @0)", movieID);
                return db.Fetch<Model.GaoQing.Country>(sql);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 根据电影ID获取导演
        /// </summary>
        /// <param name="movieID"></param>
        /// <returns></returns>
        public List<Model.GaoQing.Celebrity> GetDirectorByMovieID(int movieID)
        {
            try
            {
                var sql = Sql.Builder.Where("[ID] in (Select [CelebrityID] From tb_Relation_MovieDirector_GaoQing Where [MovieID] = @0)", movieID);
                return db.Fetch<Model.GaoQing.Celebrity>(sql);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 根据电影ID获取演员
        /// </summary>
        /// <param name="movieID"></param>
        /// <returns></returns>
        public List<Model.GaoQing.Celebrity> GetPerformerByMovieID(int movieID)
        {
            try
            {
                var sql = Sql.Builder.Where("[ID] in (Select [CelebrityID] From tb_Relation_MoviePerformer_GaoQing Where [MovieID] = @0)", movieID);
                return db.Fetch<Model.GaoQing.Celebrity>(sql);
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        /// <summary>
        /// 根据电影ID获取编剧
        /// </summary>
        /// <param name="movieID"></param>
        /// <returns></returns>
        public List<Model.GaoQing.Celebrity> GetScreenwriterByMovieID(int movieID)
        {
            try
            {
                var sql = Sql.Builder.Where("[ID] in (Select [CelebrityID] From tb_Relation_MovieScreenwriter_GaoQing Where [MovieID] = @0)", movieID);
                return db.Fetch<Model.GaoQing.Celebrity>(sql);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 根据电影ID获取片长
        /// </summary>
        /// <param name="movieID"></param>
        /// <returns></returns>
        public List<Model.GaoQing.LongTime> GetLongTimeByMovieID(int movieID)
        {
            try
            {
                return db.Fetch<Model.GaoQing.LongTime>(Sql.Builder.Where("[MovieID]=@0",movieID));
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 根据电影ID获取更多名称
        /// </summary>
        /// <param name="movieID"></param>
        /// <returns></returns>
        public List<Model.GaoQing.MovieName> GetMoreNameByMovieID(int movieID)
        {
            try
            {
                return db.Fetch<Model.GaoQing.MovieName>(Sql.Builder.Where("[MovieID]=@0",movieID));
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 根据电影ID获取上映时间
        /// </summary>
        /// <param name="movieID"></param>
        /// <returns></returns>
        public List<Model.GaoQing.ReleaseTime> GetReleaseTimeByMovieID(int movieID)
        {
            try
            {
                return db.Fetch<Model.GaoQing.ReleaseTime>(Sql.Builder.Where("[MovieID]=@0", movieID));
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 根据电影ID获取季组
        /// </summary>
        /// <param name="movieID"></param>
        /// <returns></returns>
        public List<Model.GaoQing.Season> GetSeasonByMovieID(int movieID)
        {
            try
            {
                return db.Fetch<Model.GaoQing.Season>(Sql.Builder.Where("SeasonID=@0", db.SingleOrDefault<Model.GaoQing.Season>(Sql.Builder.Where("[MovieID]=@0", movieID)).SeasonID));
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 分页展示影片数据
        /// </summary>
        /// <param name="orderby">排序方式</param>
        /// <param name="family">类型 电影还是电视剧</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页多少条</param>
        /// <param name="filmGenreID">类型ID</param>
        /// <param name="countryID">国家ID</param>
        /// <param name="directorID">导演ID</param>
        /// <param name="screenwriterID">编剧ID</param>
        /// <param name="performerID">演员ID</param>
        /// <param name="year">年份</param>
        /// <returns></returns>
        public List<Model.GaoQing.BaseMovie> PageMovie(Model.GaoQing.MovieOrderBy orderby, Model.GaoQing.Family? family, int page, int pageSize, int? filmGenreID = null, int? countryID = null, int? directorID = null, int? screenwriterID = null, int? performerID = null, string year = null)
        {
            try
            {
                List<Model.GaoQing.BaseMovie> list = new List<Model.GaoQing.BaseMovie>();

                var strWhere = "";//" And ((Select COUNT(ID) From tb_MovieTorrent_GaoQing Where MovieID = tb_Movie_GaoQing.ID) >0 Or (Select COUNT(ID) From tb_MovieSubtitle_GaoQing Where MovieID = tb_Movie_GaoQing.ID) >0)";
                if (filmGenreID.HasValue)
                    strWhere += " And tb_Movie_GaoQing.[ID] in (Select [MovieID] From [tb_Relation_MovieCategory_GaoQing] Where [CategoryID] = @1)";
                if (countryID.HasValue)
                    strWhere += " And tb_Movie_GaoQing.[ID] in (Select [MovieID] From [tb_Relation_MovieCountry_GaoQing] Where [CountryID] = @2)";
                if (directorID.HasValue)
                    strWhere += " And tb_Movie_GaoQing.[ID] in (Select [MovieID] From [tb_Relation_MovieDirector_GaoQing] Where [CelebrityID] = @3)";
                if (performerID.HasValue)
                    strWhere += " And tb_Movie_GaoQing.[ID] in (Select [MovieID] From [tb_Relation_MoviePerformer_GaoQing] Where [CelebrityID] = @4)";
                if (!string.IsNullOrWhiteSpace(year))
                    strWhere += " And (CASE WHEN Nd IS NULL THEN 'OTHER' ELSE CONVERT(varchar,Nd) END) = @5";
                if (screenwriterID.HasValue)
                    strWhere += " And tb_Movie_GaoQing.[ID] in (Select [MovieID] From [tb_Relation_MovieScreenwriter_GaoQing] Where [CelebrityID] = @6)";
                if (family.HasValue)
                {
                    strWhere += " And [Family]=@0 ";
                }

                switch (orderby)
                {
                    case Model.GaoQing.MovieOrderBy.热门:
                        list = db.Fetch<Model.GaoQing.BaseMovie>(page, pageSize, Sql.Builder.Where("[Title] IS NOT NULL"+ strWhere, family.HasValue ? (int)family.Value : 0, filmGenreID, countryID, directorID, performerID, year, screenwriterID).OrderBy("[Heat] Desc"));
                        break;
                    case Model.GaoQing.MovieOrderBy.评分:
                        list = db.Fetch<Model.GaoQing.BaseMovie>(page, pageSize, Sql.Builder.Where("[Title] IS NOT NULL"+ strWhere, family.HasValue ? (int)family.Value : 0, filmGenreID, countryID, directorID, performerID, year, screenwriterID).OrderBy("[Rate] Desc"));
                        break;
                    case Model.GaoQing.MovieOrderBy.最新:
                        list = db.Fetch<Model.GaoQing.BaseMovie>(page, pageSize, Sql.Builder.Where("[Title] IS NOT NULL"+ strWhere, family.HasValue ? (int)family.Value : 0, filmGenreID, countryID, directorID, performerID, year, screenwriterID).OrderBy("[CreateTime] Desc"));
                        break;
                }
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 分页展示影片数据    返回Page
        /// </summary>
        /// <param name="orderby">排序方式</param>
        /// <param name="family">类型 电影还是电视剧</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页多少条</param>
        /// <param name="filmGenreID">类型ID</param>
        /// <param name="countryID">国家ID</param>
        /// <param name="directorID">导演ID</param>
        /// <param name="screenwriterID">编剧ID</param>
        /// <param name="performerID">演员ID</param>
        /// <param name="year">年份</param>
        /// <returns></returns>
        public Page<Model.GaoQing.BaseMovie> PageMovieByPage(Model.GaoQing.MovieOrderBy orderby, Model.GaoQing.Family? family, int page, int pageSize, int? genreID = null, int? countryID = null, int? directorID = null, int? screenwriterID = null, int? performerID = null, string year = null)
        {
            try
            {
                Page<Model.GaoQing.BaseMovie> pageObj = new Page<Model.GaoQing.BaseMovie>();

                var strWhere = "";//" And ((Select COUNT(ID) From tb_MovieTorrent_GaoQing Where MovieID = tb_Movie_GaoQing.ID) >0 Or (Select COUNT(ID) From tb_MovieSubtitle_GaoQing Where MovieID = tb_Movie_GaoQing.ID) >0)";
                if (genreID.HasValue)
                    strWhere += " And tb_Movie_GaoQing.[ID] in (Select [MovieID] From [tb_Relation_MovieCategory_GaoQing] Where [CategoryID] in (Select ID From tb_Category_GaoQing Where CID = @1))";
                if (countryID.HasValue)
                    strWhere += " And tb_Movie_GaoQing.[ID] in (Select [MovieID] From [tb_Relation_MovieCountry_GaoQing] Where [CountryID] in (Select ID From tb_Country_GaoQing Where CID = @2))";
                if (directorID.HasValue)
                    strWhere += " And tb_Movie_GaoQing.[ID] in (Select [MovieID] From [tb_Relation_MovieDirector_GaoQing] Where [CelebrityID] = @3)";
                if (performerID.HasValue)
                    strWhere += " And tb_Movie_GaoQing.[ID] in (Select [MovieID] From [tb_Relation_MoviePerformer_GaoQing] Where [CelebrityID] = @4)";
                if (!string.IsNullOrWhiteSpace(year))
                {
                    if (year=="other")
                        strWhere += " And (([Nd]>" + DateTime.Now.Year + " And [Nd]<" + (DateTime.Now.Year - 20) + ") Or Nd IS NULL)";
                    else
                        strWhere += " And [Nd] = @5";
                }
                if (screenwriterID.HasValue)
                    strWhere += " And tb_Movie_GaoQing.[ID] in (Select [MovieID] From [tb_Relation_MovieScreenwriter_GaoQing] Where [CelebrityID] = @6)";
                if (family.HasValue)
                {
                    strWhere += " And [Family]=@0 ";
                }

                switch (orderby)
                {
                    case Model.GaoQing.MovieOrderBy.热门:
                        pageObj = db.Page<Model.GaoQing.BaseMovie>(page, pageSize, Sql.Builder.Where("[Title] IS NOT NULL" + strWhere, family.HasValue ? (int)family.Value : 0, genreID, countryID, directorID, performerID, year, screenwriterID).OrderBy("[Heat] Desc"));
                        break;
                    case Model.GaoQing.MovieOrderBy.评分:
                        pageObj = db.Page<Model.GaoQing.BaseMovie>(page, pageSize, Sql.Builder.Where("[Title] IS NOT NULL" + strWhere, family.HasValue ? (int)family.Value : 0, genreID, countryID, directorID, performerID, year, screenwriterID).OrderBy("[Rate] Desc"));
                        break;
                    case Model.GaoQing.MovieOrderBy.最新:
                        pageObj = db.Page<Model.GaoQing.BaseMovie>(page, pageSize, Sql.Builder.Where("[Title] IS NOT NULL" + strWhere, family.HasValue ? (int)family.Value : 0, genreID, countryID, directorID, performerID, year, screenwriterID).OrderBy("[CreateTime] Desc"));
                        break;
                    case Model.GaoQing.MovieOrderBy.可在线播放:
                        pageObj = db.Page<Model.GaoQing.BaseMovie>(page, pageSize, Sql.Builder.Where("[Title] IS NOT NULL And ID in (Select [MovieID] From tb_Player Where [MovieID] is not null Group By [MovieID])" + strWhere, family.HasValue ? (int)family.Value : 0, genreID, countryID, directorID, performerID, year, screenwriterID).OrderBy("[CreateTime] Desc"));
                        break;
                }
                return pageObj;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 根据搜索类容加载Movie
        /// </summary>
        /// <param name="q"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<Model.GaoQing.Movie> SearchMovie(string q, int page, int pageSize = 6)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(q))
                {
                    List<Model.GaoQing.Movie> list = new List<Model.GaoQing.Movie>();
                    var sql = Sql.Builder.Where("[Name] like '%'+@0+'%' " +
                        "Or [Title] like '%'+@0+'%' " +
                        "Or [ID] in (Select [MovieID] From tb_MovieName_GaoQing Where [Name] like '%'+@0+'%') " +
                        "Or [ID] in  (Select [MovieID] From tb_Relation_MovieDirector_GaoQing Where [CelebrityID] in (Select [ID] From tb_Category_GaoQing Where [Name] like '%'+@0+'%')) " +
                        "Or [ID] in  (Select [MovieID] From tb_Relation_MovieScreenwriter_GaoQing Where [CelebrityID] in (Select [ID] From tb_Category_GaoQing Where [Name] like '%'+@0+'%')) " +
                        "Or [ID] in  (Select [MovieID] From tb_Relation_MoviePerformer_GaoQing Where [CelebrityID] in (Select [ID] From tb_Category_GaoQing Where [Name] like '%'+@0+'%')) ", q).OrderBy("CreateTime Desc");

                    list = db.Fetch<Model.GaoQing.Movie>(page, pageSize, sql);
                    list.ForEach(model=> {
                        model.Category = GetCategoryByMovieID(model.ID);
                        model.Country = GetCountryByMovieID(model.ID);
                        model.Director = GetDirectorByMovieID(model.ID);
                        model.Performer = GetPerformerByMovieID(model.ID);
                        model.Screenwriter = GetScreenwriterByMovieID(model.ID);
                        model.LongTime = GetLongTimeByMovieID(model.ID);
                        model.MoreName = GetMoreNameByMovieID(model.ID);
                        model.ReleaseTime = GetReleaseTimeByMovieID(model.ID);
                        model.Season = GetSeasonByMovieID(model.ID);
                    });
                    return list;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }


        /// <summary>
        /// 根据搜索类容加载Movie
        /// </summary>
        /// <param name="q"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Page<Model.GaoQing.Movie> SearchMovieByPage(string q, int page, int pageSize = 6)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(q))
                {
                    Page<Model.GaoQing.Movie> pageObj = new Page<Model.GaoQing.Movie>();
                    var sql = Sql.Builder.Where("[Name] like '%'+@0+'%' " +
                        "Or [Title] like '%'+@0+'%' " +
                        "Or [ID] in (Select [MovieID] From tb_MovieName_GaoQing Where [Name] like '%'+@0+'%') " +
                        "Or [ID] in  (Select [MovieID] From tb_Relation_MovieDirector_GaoQing Where [CelebrityID] in (Select [ID] From tb_Category_GaoQing Where [Name] like '%'+@0+'%')) " +
                        "Or [ID] in  (Select [MovieID] From tb_Relation_MovieScreenwriter_GaoQing Where [CelebrityID] in (Select [ID] From tb_Category_GaoQing Where [Name] like '%'+@0+'%')) " +
                        "Or [ID] in  (Select [MovieID] From tb_Relation_MoviePerformer_GaoQing Where [CelebrityID] in (Select [ID] From tb_Category_GaoQing Where [Name] like '%'+@0+'%')) ", q).OrderBy("CreateTime Desc");

                    pageObj = db.Page<Model.GaoQing.Movie>(page, pageSize, sql);
                    pageObj.Items.ForEach(model => {
                        model.Category = GetCategoryByMovieID(model.ID);
                        model.Country = GetCountryByMovieID(model.ID);
                        model.Director = GetDirectorByMovieID(model.ID);
                        model.Performer = GetPerformerByMovieID(model.ID);
                        model.Screenwriter = GetScreenwriterByMovieID(model.ID);
                        model.LongTime = GetLongTimeByMovieID(model.ID);
                        model.MoreName = GetMoreNameByMovieID(model.ID);
                        model.ReleaseTime = GetReleaseTimeByMovieID(model.ID);
                        model.Season = GetSeasonByMovieID(model.ID);
                    });
                    return pageObj;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 根据名人ID搜索
        /// </summary>
        /// <param name="celebrity"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Page<Model.GaoQing.BaseMovie> SearchMovieByCelebrity(int celebrity, int page, int pageSize = 6)
        {
            try
            {
                Page<Model.GaoQing.BaseMovie> pageObj = new Page<Model.GaoQing.BaseMovie>();
                var sql = Sql.Builder.Where(
                    "[ID] in  (Select [MovieID] From tb_Relation_MovieDirector_GaoQing Where [CelebrityID] = @0) " +
                    "Or [ID] in  (Select [MovieID] From tb_Relation_MovieScreenwriter_GaoQing Where [CelebrityID] = @0) " +
                    "Or [ID] in  (Select [MovieID] From tb_Relation_MoviePerformer_GaoQing Where [CelebrityID] = @0) ", celebrity)
                    .OrderBy("CreateTime Desc");

                pageObj = db.Page<Model.GaoQing.BaseMovie>(page, pageSize, sql);
                return pageObj;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 根据搜索类容加载Movie
        /// </summary>
        /// <param name="q"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Page<Model.GaoQing.BaseMovie> SearchMovieByKeyword(string q, int page, int pageSize = 6)
        {
            try
            {
                Page<Model.GaoQing.BaseMovie> pageObj = new Page<Model.GaoQing.BaseMovie>();
                if (!string.IsNullOrWhiteSpace(q))
                {
                    var sql = Sql.Builder.Where("[Name] like '%'+@0+'%' " +
                        "Or [Title] like '%'+@0+'%' " +
                        "Or [ID] in (Select [MovieID] From tb_MovieName_GaoQing Where [Name] like '%'+@0+'%') " +
                        "Or [ID] in  (Select [MovieID] From tb_Relation_MovieDirector_GaoQing Where [CelebrityID] in (Select [ID] From tb_Category_GaoQing Where [Name] like '%'+@0+'%')) " +
                        "Or [ID] in  (Select [MovieID] From tb_Relation_MovieScreenwriter_GaoQing Where [CelebrityID] in (Select [ID] From tb_Category_GaoQing Where [Name] like '%'+@0+'%')) " +
                        "Or [ID] in  (Select [MovieID] From tb_Relation_MoviePerformer_GaoQing Where [CelebrityID] in (Select [ID] From tb_Category_GaoQing Where [Name] like '%'+@0+'%')) ", q).OrderBy("CreateTime Desc");

                    pageObj = db.Page<Model.GaoQing.BaseMovie>(page, pageSize, sql);
                    
                }
                else
                    pageObj = db.Page<Model.GaoQing.BaseMovie>(page, pageSize, Sql.Builder.OrderBy("CreateTime Desc"));

                return pageObj;
            }
            catch (Exception)
            {
                return null;
            }
        }


        /// <summary>
        /// 获取所有年份组
        /// </summary>
        /// <returns></returns>
        public List<string> GetYearGroup()
        {
            try
            {
                var sql = Sql.Builder
                    .Select("CASE WHEN Nd IS NULL THEN 'OTHER' ELSE CONVERT(varchar,Nd) END")
                    .From("tb_Movie_GaoQing")
                    .GroupBy("Nd")
                    .OrderBy("Nd Desc");
                return db.Fetch<string>(sql);
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
        public List<Model.GaoQing.Country> GetCountryList()
        {
            try
            {
                return db.Fetch<Model.GaoQing.Country>(Sql.Builder.OrderBy("[Name] Asc"));
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取电影类型列表
        /// </summary>
        /// <returns></returns>
        public List<Model.GaoQing.Category> GetCategoryList()
        {
            try
            {
                return db.Fetch<Model.GaoQing.Category>(Sql.Builder.OrderBy("[Name] Asc"));
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取电影类型
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Model.GaoQing.Category SingleCategoryByName(string name)
        {
            try
            {
                return db.SingleOrDefault<Model.GaoQing.Category>(Sql.Builder.Where("[Name] = @0", name));
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
