using ORM;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL
{
    public class Account_BLL : BaseDal
    {
        public Model.Account Sign(string email, string pwd, string nickname, string key)
        {
            try
            {
                var model = new Model.Account() {
                    CreateTime = DateTime.Now,
                    Email = email,
                    LastTime = DateTime.Now,
                    NickName = nickname,
                    Password = Utility.InEncrypt.MD5(pwd),
                    Status = 0
                };
                model.ID = Convert.ToInt32(db.Insert(model));
                return model;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Model.Account Login(string email, string pwd)
        {
            try
            {
                return db.SingleOrDefault<Model.Account>(Sql.Builder.Where("Email=@0 And Password=@1", email, Utility.InEncrypt.MD5(pwd)));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Model.Account GetUser(string email)
        {
            try
            {
                return db.SingleOrDefault<Model.Account>(Sql.Builder.Where("Email=@0", email));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void ActionKey(string key)
        {
            Convert.ToInt32(db.Update(new Model.Key() { SignKey = key, Status = -1 }));
        }

        public int IsKey(string key)
        {
            try
            {
                return db.SingleOrDefault<Model.Key>(Sql.Builder.Where("[SignKey]=@0", key)).Status;
            }
            catch (Exception)
            {
                return -2;
            }
        }
    }
}
