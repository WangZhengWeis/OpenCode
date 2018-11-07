using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication_PC.Controllers
{
    public class AccountController : Controller
    {
        BLL.Account_BLL bll = new BLL.Account_BLL();
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult SignUp()
        {
            return View();
        }

        public ActionResult ActionSign(string Email, string NickName, string Password, string Password2, string Key)
        {
            if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(NickName) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(Password2) && !string.IsNullOrEmpty(Key))
            {
                if (Password == Password2)
                {
                    var a = bll.GetUser(Email);
                    if (a == null)
                    {
                        if (bll.IsKey(Key) == 0)
                        {
                            var user = bll.Sign(Email, Password, NickName, Key);
                            if (user != null)
                            {
                                return JavaScript("<script>alert('账号注册成功，请联系管理员审核！');</script>");
                            }
                            else
                            {
                                return JavaScript("<script>alert('注册失败！');</script>");
                            }
                        }
                        else
                        {
                            return JavaScript("<script>alert('这个邀请码无效！');</script>");
                        }
                    }
                    else
                    {
                        return JavaScript("<script>alert('该账号已经被注册了！');</script>");
                    }
                }
                else
                {
                    return JavaScript("<script>alert('两次输入的密码不一致！');</script>");
                }
            }
            else
            {
                return JavaScript("<script>alert('提交的数据不合法！');</script>");
            }
        }

        public ActionResult ActionLogin(string Email, string Password)
        {
            if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password))
            {
                var user = bll.Login(Email, Password);
                if (user != null)
                {
                    if (user.Status == 1)
                    {
                        Session["Account"] = user;
                        return RedirectToAction("Index", "Movie");
                    }
                    else
                    {
                        return JavaScript("<script>alert('账号当前状态不正确，请联系管理员！');</script>");
                    }
                }
                else
                {
                    return JavaScript("<script>alert('登陆失败！');</script>");
                }
            }
            else
            {
                return JavaScript("<script>alert('提交的数据不合法！');</script>");
            }
        }
    }
}