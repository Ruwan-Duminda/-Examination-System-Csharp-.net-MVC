using Examination_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using PagedList.Mvc;
using PagedList;

namespace Examination_System.Controllers
{
    
    public class HomeController : Controller
    {
        
        DBEXAMINATIONEntities db = new DBEXAMINATIONEntities();
        [HttpGet]

        public ActionResult tregister()
        {

            return View();
        }

        [HttpPost]
        public ActionResult tregister(TBL_ADMIN adreg, HttpPostedFileBase imgfileadmin)
        {
            TBL_ADMIN a = new TBL_ADMIN();
            try
            {
                a.AD_NAME = adreg.AD_NAME;
                a.AD_EMAIL = adreg.AD_EMAIL;
                a.AD_PASSWORD = adreg.AD_PASSWORD;
                a.AD_IMAGE = uploadadimgfile(imgfileadmin);
                db.TBL_ADMIN.Add(a);
                db.SaveChanges();
                return RedirectToAction("tlogin");
            }
            catch(Exception)
            {
                ViewBag.msg = "Data could not be inserted.....";
            }
            return View();
        }
        public string uploadadimgfile(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";

            int random = r.Next();

            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                {
                    try
                    {
                        path = Path.Combine(Server.MapPath("~/Content/img"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/Content/img" + random + Path.GetFileName(file.FileName);
                    }
                    catch (Exception ex)
                    {
                        path = "-1";
                    }
                }
                else
                {
                    Response.Write("<script>alert('Only jpg,jpeg or png formats are acceptable.....')</script>");
                }
            }
            else
            {
                Response.Write("<script>alert('Please Select a file')</script>");
                path = "-1";
            }

            return path;
        }


        public ActionResult sregister()
        {

            return View();
        }

        [HttpPost]
        public ActionResult sregister(TBL_STUDENT svw,HttpPostedFileBase imgfile)
        {
            TBL_STUDENT s = new TBL_STUDENT();

            try
            {
                s.S_NAME = svw.S_NAME;
                s.S_PASSWORD = svw.S_PASSWORD;
                s.S_IMAGE = uploadimgfile(imgfile);
                s.S_EMAIL = svw.S_EMAIL;
                db.TBL_STUDENT.Add(s);
                db.SaveChanges();
                return RedirectToAction("slogin");
            }
            catch(Exception)
            {
                ViewBag.msg = "Data could not be inserted.....";
            }
            

            return View();
        }

        public string uploadimgfile(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";

            int random = r.Next();

            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                {
                    try
                    {
                        path = Path.Combine(Server.MapPath("~/Content/img"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/Content/img" + random + Path.GetFileName(file.FileName);
                    }
                    catch (Exception ex)
                    {
                        path = "-1";
                    }
                }
                else
                {
                    Response.Write("<script>alert('Only jpg,jpeg or png formats are acceptable.....')</script>");
                }
            }
            else
            {
                Response.Write("<script>alert('Please Select a file')</script>");
                path = "-1";
            }

            return path;
            //return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session.Abandon();
            Session.RemoveAll();
            return RedirectToAction("Index");
        }

        public ActionResult tlogin()
        {
            
            return View();
        }

        [HttpPost]
        public ActionResult tlogin(TBL_ADMIN a)
        {
            TBL_ADMIN ad = db.TBL_ADMIN.Where(x => x.AD_EMAIL == a.AD_EMAIL && x.AD_PASSWORD == a.AD_PASSWORD).SingleOrDefault();
            if (ad != null)
            {
                Session["ad_id"] = ad.AD_ID;
                Session["ad_email"] = ad.AD_EMAIL;
                Session["ad_name"] = ad.AD_NAME;
                TempData["name"]= Session["ad_name"];
                TempData.Keep();
                return RedirectToAction("Dashboard");
            }
            else
            {
                ViewBag.msg = "Invalid username or password";
            }

            return View();
        }

        public ActionResult slogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult slogin(TBL_STUDENT s)
        {
            TBL_STUDENT std = db.TBL_STUDENT.Where(x => x.S_EMAIL == s.S_EMAIL && x.S_PASSWORD == s.S_PASSWORD).SingleOrDefault();
            if(std!=null)
            {
                Session["std_id"] = std.S_ID;
                Session["std_name"] = std.S_NAME;
                Session["std_img"] = std.S_IMAGE;

                

                TempData["s_id"] = Session["std_id"];
                TempData["s_name"] = Session["std_name"];
                TempData.Keep();


                HttpCookie cookie = new HttpCookie("userdetails");
                cookie["sname"] = std.S_NAME;
                if (std.S_IMAGE.Equals("-1"))
                {
                    cookie["simage"] = "";
                }
                else
                {
                    cookie["simage"] = std.S_IMAGE;
                }

                cookie.Expires = DateTime.Now.AddDays(30);
                Response.Cookies.Add(cookie);

                return RedirectToAction("StudentExam");
                //ViewBag.msg = "Invalid Username or Password";
            }
            else
            {
                //Session["std_id"] = std.S_ID;
                //return RedirectToAction("StudentExam");
                ViewBag.msg = "Invalid Username or Password";
            }
            return View();
        }


        public ActionResult viewAllquestions(int?id,int?page)
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("tlogin");
            }
            if (id==null)
            {
                return RedirectToAction("Dashboard");
            }
            int pagesize = 15, pageindex = 1;
            pageindex = page.HasValue? Convert.ToInt32(page) : 1;

            List<TBL_QUESTIONS> li = db.TBL_QUESTIONS.Where(x => x.q_fk_catid == id).ToList();
            
            IPagedList<TBL_QUESTIONS> stu = li.ToPagedList(pageindex, pagesize);
            //return View(db.TBL_QUESTIONS.Where(x=>x.q_fk_catid==id).ToList());

            return View(stu);
        }



        public ActionResult viewReport(int? id, int? page)
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("tlogin");
            }
            //if (id == null)
            //{
            //    return RedirectToAction("Dashboard");
            //}
            int pagesize = 4, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;

            List<TBL_SETEXAME> li = db.TBL_SETEXAME.ToList();

            IPagedList<TBL_SETEXAME> stu = li.ToPagedList(pageindex, pagesize);
            //return View(db.TBL_QUESTIONS.Where(x=>x.q_fk_catid==id).ToList());

            return View(stu);
        }


        public ActionResult StudentExam()
        {
            if (Session["std_id"] == null)
            {
                return RedirectToAction("slogin");
            }

            return View();
        }

        [HttpPost]
        public ActionResult StudentExam(string room)
        {
            List<tbl_category> list = db.tbl_category.ToList();
            foreach(var item in list)
            {
                if(item.cat_encyptedstring==room)
                {
                    List<TBL_QUESTIONS> li = db.TBL_QUESTIONS.Where(x => x.q_fk_catid == item.cat_id).ToList();
                    Queue<TBL_QUESTIONS> queue = new Queue<TBL_QUESTIONS>();
                    foreach(TBL_QUESTIONS a in li)
                    {
                        queue.Enqueue(a);
                    }
                    TempData["examname"] = item.cat_name;
                    TempData["examid"] = item.cat_id;
                    TempData["questions"] = queue;
                    TempData["score"] = 0;
                    TempData.Keep();
                    return RedirectToAction("QuizStart");
                }
                else
                {
                    ViewBag.error = "No Room Found.....";
                }
            }
            return View();
        }

        public ActionResult QuizStart()
        {
            ////TempData["i"] = 1;
            //if(TempData["i"]==null)
            //{
            //    TempData["i"] = 1;
            //}
            //if (Session["std_id"] == null)
            //{
            //    return RedirectToAction("slogin");
            //}
            ////if (TempData["exampid"]==null)
            ////{
            ////    RedirectToAction("StudentExam");
            ////}
            //try
            //{
            //    TBL_QUESTIONS q = null;
            //    int examid = Convert.ToInt32(TempData["exampid"].ToString());
            //    if (TempData["qid"] == null)
            //    {
            //        q = db.TBL_QUESTIONS.First(x => x.q_fk_catid == examid);
            //        //var list = db.TBL_QUESTIONS.Skip(1);
            //        var list = db.TBL_QUESTIONS.Skip(Convert.ToInt32(TempData["i"].ToString()));
            //        int qid = list.First().QUESTION_ID;
            //        TempData["qid"] = qid;
            //        //TempData["qid"] = ++q.QUESTION_ID;
            //        //TBL_QUESTIONS q = db.TBL_QUESTIONS.Where(x => x.q_fk_catid == examid).SingleOrDefault();
            //    }
            //    else
            //    {
            //        int qid = Convert.ToInt32(TempData["qid"].ToString());
            //        q = db.TBL_QUESTIONS.Where(x => x.QUESTION_ID == qid && x.q_fk_catid == examid).SingleOrDefault();
            //        //TempData["qid"] = ++q.QUESTION_ID;
            //        var list = db.TBL_QUESTIONS.Skip(Convert.ToInt32(TempData["i"].ToString()));
            //        qid = list.First().QUESTION_ID;
            //        TempData["qid"] = qid;
            //        TempData["i"] = Convert.ToInt32(TempData["i"].ToString()) + 1;
            //    }
            //    TempData.Keep();
            //    return View(q);
            //}
            //catch(Exception)
            //{

            //}
            //return RedirectToAction("StudentExam");
            if(Session["std_id"]==null)
            {
                return RedirectToAction("slogin");
            }

            TBL_QUESTIONS q = null;

            if (TempData["questions"]!=null)
            {
                Queue<TBL_QUESTIONS> qlist = (Queue<TBL_QUESTIONS>)TempData["questions"];
                if (qlist.Count>0)
                {
                    q = qlist.Peek();
                    qlist.Dequeue();
                    TempData["questions"] = qlist;
                    TempData.Keep();
                }
                else
                {
                    return RedirectToAction("EndExam");
                }
            }
            else
            {
                return RedirectToAction("StudentExam");
            }


            return View(q);
        }

        [HttpPost]
        public ActionResult QuizStart(TBL_QUESTIONS q)
        {
            string correctans = null;
            if (q.OPA!= null)
            {
                correctans = "A";
                //if(q.OPA.Equals("A"))
                //{
                //    TempData["score"] = Convert.ToInt32(TempData["score"]) + 1;
                //}

            }
            else if(q.OPB!= null)
            {
                correctans = "B";
                //if (q.OPB.Equals("B"))
                //{
                //    TempData["score"] = Convert.ToInt32(TempData["score"]) + 1;
                //}

            }
            else if(q.OPC!= null)
            {
                correctans = "C";
                //if (q.OPC.Equals("C"))
                //{
                //    TempData["score"] = Convert.ToInt32(TempData["score"]) + 1;
                //}

            }
            else if(q.OPD!= null)
            {
                correctans = "D";
                //if (q.OPD.Equals("D"))
                //{
                //    TempData["score"] = Convert.ToInt32(TempData["score"]) + 1;
                //}

            }
            else
            {
                correctans = "";
            }
 
            if (correctans.Equals(q.OPP))
            {
                TempData["score"] = Convert.ToInt32(TempData["score"]) + 1;
            }

            TempData.Keep();
            return RedirectToAction("QuizStart");
        }

        public ActionResult EndExam()
        {
            DateTime thisDay = DateTime.Today;

            TBL_SETEXAME se = new TBL_SETEXAME();
            //se.EXAME_ID = Convert.ToInt32(Session["examid"].ToString());
            se.EXAME_FK_STU = Convert.ToInt32(Session["std_id"].ToString());
            se.EXAME_DATE = Convert.ToDateTime(thisDay.ToString("g"));
            se.EXAME_NAME = Convert.ToString(TempData["examname"].ToString());
            se.EXAME_STD_SCORE = Convert.ToInt32(TempData["score"].ToString());
            db.TBL_SETEXAME.Add(se);
            db.SaveChanges();
            TempData.Clear();
            return View();

        }

        public ActionResult Dashboard()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public ActionResult Addcategory()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Index");
            }
            //Session["ad_id"] = 2;//we will remove it soon.......
            int adid = Convert.ToInt32(Session["ad_id"].ToString());
            List<tbl_category> li = db.tbl_category.Where(x=>x.cat_fk_adid==adid ).OrderByDescending(x => x.cat_id).ToList();
            ViewData["list"] = li;
            return View();
        }


        public ActionResult ExamEnd(TBL_SETEXAME tset)
        {
            DateTime thisDay = DateTime.Now;
           
         
            TBL_SETEXAME se = new TBL_SETEXAME();
            //se.EXAME_ID = Convert.ToInt32(Session["examid"].ToString());
            se.EXAME_FK_STU= Convert.ToInt32(Session["std_id"].ToString());
            se.EXAME_DATE =Convert.ToDateTime(thisDay.ToString("d")) ;
            se.EXAME_NAME= Convert.ToString(TempData["examname"].ToString());
            se.EXAME_STD_SCORE= Convert.ToInt32(TempData["score"].ToString());
            db.TBL_SETEXAME.Add(se);
            db.SaveChanges();
            TempData.Clear();
            return View();
        }

        [HttpPost]
        public ActionResult Addcategory(tbl_category cat)
        {
           
            List<tbl_category> li = db.tbl_category.OrderByDescending(x => x.cat_id).ToList();
            ViewData["list"] = li;

            Random r = new Random();
            tbl_category c = new tbl_category();
            c.cat_name = cat.cat_name;
            c.cat_encyptedstring = cyptop.Encrypt(cat.cat_name.Trim()+r.Next().ToString(),true);
            c.cat_fk_adid = Convert.ToInt32(Session["ad_id"].ToString());
            db.tbl_category.Add(c);
            db.SaveChanges();


            return RedirectToAction("Addcategory");
        }

        [HttpGet]
        [ValidateInput(false)]
        public ActionResult Addquestion()
        {
            int sid =Convert.ToInt32(Session["ad_id"]);
            List<tbl_category> li = db.tbl_category.Where(x => x.cat_fk_adid == sid).ToList();
            ViewBag.list = new SelectList(li, "cat_id", "cat_name");
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Addquestion(TBL_QUESTIONS q)
        {
            int sid = Convert.ToInt32(Session["ad_id"]);
            List<tbl_category> li = db.tbl_category.Where(x => x.cat_fk_adid == sid).ToList();
            ViewBag.list = new SelectList(li, "cat_id", "cat_name");

            TBL_QUESTIONS QA = new TBL_QUESTIONS();
            QA.Q_TEXT = q.Q_TEXT;
            QA.OPA = q.OPA;
            QA.OPB = q.OPB;
            QA.OPC = q.OPC;
            QA.OPD = q.OPD;
            QA.OPP = q.OPP;
            QA.q_fk_catid = q.q_fk_catid;

            db.TBL_QUESTIONS.Add(QA);
            db.SaveChanges();
            TempData["msg"]= "Question added successfully....";
            TempData.Keep();
            return RedirectToAction("Addquestion");

        }

        public ActionResult Index()
        {
            if (Session["ad_id"]!=null)
            {
                return RedirectToAction("Dashboard");
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        //public ActionResult records(int?page)
        //{
        //    if (Session["ad_id"] == null)
        //    {
        //        return RedirectToAction("Dashboard");
        //    }

        //    Score = new Score;

        //    return View();
        //}

        [HttpPost]
        public ActionResult Delete(int?id)
        {
            TBL_QUESTIONS d = db.TBL_QUESTIONS.Where(x => x.QUESTION_ID == id).SingleOrDefault();
            db.TBL_QUESTIONS.Remove(d);
            db.SaveChanges();


            return View();
        }


      }
}