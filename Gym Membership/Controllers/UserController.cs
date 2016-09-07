using Gym_Membership.Helpers;
using Gym_Membership.Models;
using Gym_Membership.Services.Abstract;
using Gym_Membership.Services.Concrete;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Gym_Membership.Controllers
{
    public class
        UserController : BaseController
    {
        ILog log = log4net.LogManager.GetLogger(typeof(UserController));
        public ActionResult Index()
        {

            try
            {
                //return RedirectToAction("FirstLogin");
                return RedirectToAction("Home");
            }
            catch (Exception e)
            {

                log.Error("[index] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");
            }
        }



        public ActionResult History()
        {
            try
            {
                IUserService userService = new UserService();
                var history = userService.GetHistory().ToList() ;
                HistoryViewModel his = new HistoryViewModel { Items = history }; 


                    ContentResult contentResult = new ContentResult();
                    //var result = RenderRazorViewToString(this.ControllerContext, "_PortfolioProfile", profiles);
                    var result = RenderRazorViewToString(this.ControllerContext, "_history",  his);

                    contentResult.Content = result;
                //TODO: return partial view here using 
                //create partial view to place result in table

               // return contentResult via ajax call

                return contentResult;

            }
            catch (Exception e)
            {


                log.Error("[History] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");

            }
        }


        public ActionResult ChangePassword()
        {
            try
            {

                var form = new ChangePasswordViewModel();


                return View(form);
            }
            catch (Exception e)
            {

                log.Error("[ChangePassword] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");
            }
        }





        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel form_in)
        {
            try
            {
                if ((!string.IsNullOrWhiteSpace(form_in.Username)) && (form_in.Password == form_in.ConfirmPassword) && (form_in.OldPassword != Utils.base64Encode(form_in.Password)))
                {
                    IAdminService adminService = new AdminService();

                    form_in.IsTempPassword = false;
                    adminService.ChangePassword(form_in);

                    return RedirectToAction("Home");
                }

                if (form_in.OldPassword == Utils.base64Encode(form_in.Password))
                {
                    form_in.ValidationErrors.Add("You cannot re-use the old password, please change");

                }

                return View(form_in);
            }
            catch (Exception e)
            {

                log.Error("[ChangePassword] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");
            }

        }



        public ActionResult NotFound()
        {
            return View();
        }



        public ActionResult Pass()
        {

            try
            {
                IAdminService adminService = new AdminService();
                var confs = adminService.GetMemberships(null).Where(x => x.IsPass).ToList(); 
                ViewBag.Prices = confs;

            return View();
            }
            catch (Exception e)
            {

                log.Error("[Pass] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public ActionResult Pass(GymMember member)
        {

            IUserService userService = new UserService();
           var passid=  userService.SavePass(member);

            if(member.IsDayPass)
            {
                return RedirectToAction("Home");
            }

            return RedirectToAction("Member",new { id = passid });
        }



        public ActionResult Home()
        {

            try
            {
                IUserService userService = new UserService();

                var users = userService.GetMembers(null, DateTime.Now, true).OrderByDescending(x => x.LastVisit);

                return View(users);

            }
            catch (Exception e)
            {

                log.Error("[Home] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");
            }
        }


        public ActionResult VisitsOn(string dt)
        {
            try
            {
                ContentResult content = new ContentResult();
                //"Wed Aug 05 2015 00:00:00 GMT+0400 (Arabian Standard Time)"
                //dt = dt.Substring(4, 11);


                var _dt = DateTime.Now;
                DateTime.TryParse(dt, out _dt);

                IUserService userService = new UserService();
                var visits = userService.GetMembers(null, _dt, true).OrderByDescending(x => x.LastVisit);

                var result = RenderRazorViewToString(this.ControllerContext, "_Visits", visits);
                content.Content = result;

                return content;
            }
            catch (Exception e)
            {

                log.Error("[VisitsOn] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");
            }
        }



        public ActionResult GenerateReceipt(int id)
        {
            try
            {
                log.InfoFormat("GenerateReceipt for cir {0}", id);

                IUserService userService = new UserService();
                var items = userService.GetTransactionsByMember(id, true);

                var member = userService.GetMemberById(id);

                if (items.Count > 0)
                {
                    var item = items.FirstOrDefault();
                    item.Member = member;
                    var result = RenderRazorViewToString(this.ControllerContext, "_receipt", item);

                    var htmlContent = result;
                    var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
                    var pdfBytes = htmlToPdf.GeneratePdf(htmlContent);
                    var docname = string.Format("Receipt for {0} dated {1} {2}", item.Member.FullnameFromFirstAndLastName, item.LastReceipt.ReceivedOn.ToString("dd-mm-yyyy"), ".pdf");


                    return File(pdfBytes, "application/pdf", docname);
                }

                return RedirectToAction("Member", new { id = id });

            }

            catch (Exception e)
            {
                log.Error("[GenerateReceipt] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);

                return RedirectToAction("Error", "Home");
            }

        }




        public ActionResult CancelVisit(int id, int memberId)
        {

            try
            {
                IUserService userService = new UserService();

                DateTime? dt = DateTime.Now;

                var success = userService.RemoveVisit(id, memberId);

                return RedirectToAction("Index");

            }
            catch (Exception e)
            {

                log.Error("[index] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");

            }


        }

        public ActionResult Member(string id="")
        {


            log.Info("Member");
            //ContentResult contentResult = new ContentResult();
            try
            {
                

                int intId = Utils.ParseStringToInt(id);



                IUserService userService = new UserService();
                var users = userService.GetMembers(intId, null, true);



                GymMember mem;
                if (users.Count == 1)
                {
                    mem = users.FirstOrDefault();
                    mem.RelatedMembers = userService.GetRelatedMembers(mem.MemberId);

                    mem.Receipts = userService.GetReceiptsByMember(mem.MemberId);
                    mem.Comments = userService.GetMemberComments(mem.MemberId);

                 


                } else if (users.Count == 0)
                {
                    return RedirectToAction("NotFound");
                }
                else
                {
                    mem = new GymMember();
                }

                return View(mem);

            }
            catch (Exception e)
            {
                log.Error("[Users] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");

            }
        }




        public ActionResult Edit(int id)
        {


            log.Info("Edit");
            //ContentResult contentResult = new ContentResult();
            try
            {

                IUserService userService = new UserService();
                var users = userService.GetMembers(id, null, true);
                GymMember mem;
                if (users.Count == 1)
                {
                    mem = users.FirstOrDefault();
                }
                else
                {
                    mem = new GymMember();
                }

                return View(mem);

            }
            catch (Exception e)
            {
                log.Error("[Edit] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");

            }
        }

        [HttpPost]
        public ActionResult Edit(GymMember member)
        {


            log.Info("Edit");
            //ContentResult contentResult = new ContentResult();
            try
            {
                List<string> errors = new List<string>();



                if (String.IsNullOrWhiteSpace(member.Membership.MembershipCode))
                {
                    errors.Add("Select a <b>Membership Code</b>");
                }
                if (String.IsNullOrWhiteSpace(member.Club))
                {
                    errors.Add("Select a <b>Club</b>");
                }
                //validation
                if (String.IsNullOrWhiteSpace(member.Title))
                {
                    errors.Add(string.Format("Select a <b>Title</b> for member "));
                }

                if (String.IsNullOrWhiteSpace(member.Firstname))
                {
                    errors.Add(string.Format("Enter a <b>Firstname</b> for member "));
                }
                if (String.IsNullOrWhiteSpace(member.Lastname))
                {
                    errors.Add(string.Format("Enter a <b>Lastname</b> for member "));
                }
                if (String.IsNullOrWhiteSpace(member.Gender))
                {
                    errors.Add(string.Format("Enter a <b>Gender</b> for member "));
                }
                if (String.IsNullOrWhiteSpace(member.StreetAddress))
                {
                    errors.Add(string.Format("Enter a <b>Street Address</b> for member "));
                }
                if (String.IsNullOrWhiteSpace(member.Town))
                {
                    errors.Add(string.Format("Enter a <b>Town</b>  for member "));
                }
                if (member.Age > 100)
                {
                    errors.Add(string.Format("Enter a <b>Date Of Birth</b>  for member "));
                }

                var verifyContact = string.Concat(member.EmailAddress, member.HomePhone, member.OfficePhone, member.MobilePhone);
                if (string.IsNullOrWhiteSpace(verifyContact))
                {
                    errors.Add(string.Format("Enter at least an <b>email</b> or <b>phone contact</b> for member "));
                }

                if (member.Membership.MembershipCode == "CUST" && member.CustomMonthlyFee <= 0)
                {
                    errors.Add(string.Format("Enter a <b>Monthly Fee</b> for member "));

                }

                if (errors.Count > 0)
                {
                    member.ValidationErrors = errors;
                    return View(member);
                }


                IUserService userService = new UserService();
                var result = userService.SaveMember(member);


                return RedirectToAction("Member", new { id = member.MemberId });

            }
            catch (Exception e)
            {
                log.Error("[Edit] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");

            }
        }



        public ActionResult Members()
        {


            log.Info("Members");
            //ContentResult contentResult = new ContentResult();
            try
            {

                IUserService userService = new UserService();
                var users = userService.GetMembers(null, null, true);



                return View(users);

            }
            catch (Exception e)
            {
                log.Error("[Members] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");

            }
        }

        public ActionResult NewMember()
        {


            IAdminService adminService = new AdminService();
            ViewBag.Memberships = adminService.GetMemberships(null).Where(x => x.IsActive).OrderBy(o => o.DisplayOrder).ToList();

            var m = new AddMemberViewModel();
            return View(m);
        }


        //for testing purposes
        public ActionResult NewMember2()
        {


            IAdminService adminService = new AdminService();
            ViewBag.Memberships = adminService.GetMemberships(null).Where(x => x.IsActive).ToList();

            var m = new AddMemberViewModel();
            return View(m);
        }


        [HttpPost]
        public ActionResult NewMember(AddMemberViewModel members)
        {

            try
            {
                List<string> errors = new List<string>();
                var i = 1;
                var street = members.GymMembers[0].StreetAddress;
                var town = members.GymMembers[0].Town;
                var homephone = members.GymMembers[0].HomePhone;



                if (String.IsNullOrWhiteSpace(members.MembershipCode))
                {
                    errors.Add("Select a <b>Membership Code</b>");
                }
                if (String.IsNullOrWhiteSpace(members.Club))
                {
                    errors.Add("Select a <b>Club</b>");
                }


                //fill ini i
                foreach (var m in members.GymMembers)
                {
                    if (i <= members.NumMembers)
                    {
                        if (!string.IsNullOrWhiteSpace(m.FirstnameLastnameForm))
                        {



                            if (m.FirstnameLastnameForm.Contains(","))
                            {
                                var names = m.FirstnameLastnameForm.Split(',');
                                if (names.Length > 1)
                                {
                                    m.Firstname = names[0];
                                    StringBuilder lastname = new StringBuilder();
                                    var l = 0;
                                    foreach (var n in names)
                                    {
                                        if (l > 0)
                                        {
                                            lastname.AppendFormat("{0} ", n);
                                        }

                                        l++;
                                    }
                                    lastname.Length--;
                                    m.Lastname = lastname.ToString();
                                }

                                else
                                {
                                    m.Firstname = m.FirstnameLastnameForm;

                                }

                            }
                            else
                            {
                                var names = m.FirstnameLastnameForm.Split(' ');
                                if (names.Length > 1)
                                {
                                    m.Firstname = names[0];
                                    StringBuilder lastname = new StringBuilder();
                                    var l = 0;
                                    foreach (var n in names)
                                    {
                                        if (l > 0)
                                        {
                                            lastname.AppendFormat("{0} ", n);
                                        }

                                        l++;
                                    }
                                    lastname.Length--;
                                    m.Lastname = lastname.ToString();
                                }

                                else
                                {
                                    m.Firstname = m.FirstnameLastnameForm;

                                }


                            }
                        }
                        if (m.UseSameAddressForm)
                        {
                            m.StreetAddress = street;
                            m.Town = town;
                            m.HomePhone = homephone;

                        }

                        //adding common fields

                        m.HowYouHeardAboutUs = members.HowYouHeardForm;
                        m.Membership.MembershipCode = members.MembershipCode;
                        m.Club = members.Club;

                        //validation
                        if (String.IsNullOrWhiteSpace(m.Title))
                        {
                            errors.Add(string.Format("Select a <b>Title</b> for member <b>#{0}</b>", i));
                        }


                        if (String.IsNullOrWhiteSpace(m.Firstname))
                        {
                            errors.Add(string.Format("Enter a <b>Firstname</b> for member <b>#{0}</b>", i));
                        }
                        if (String.IsNullOrWhiteSpace(m.Lastname))
                        {
                            errors.Add(string.Format("Enter a <b>Lastname</b> for member <b>#{0}</b>", i));
                        }
                        if (String.IsNullOrWhiteSpace(m.Gender))
                        {
                            errors.Add(string.Format("Enter a <b>Gender</b> for member <b>#{0}</b>", i));
                        }
                        if (String.IsNullOrWhiteSpace(m.StreetAddress))
                        {
                            errors.Add(string.Format("Enter a <b>Street Address</b> for member <b>#{0}</b>", i));
                        }
                        if (String.IsNullOrWhiteSpace(m.Town))
                        {
                            errors.Add(string.Format("Enter a <b>Town</b>  for member <b>#{0}</b>", i));
                        }
                        if (m.Age > 100)
                        {
                            errors.Add(string.Format("Enter a <b>Date Of Birth</b>  for member <b>#{0}</b>", i));
                        }

                        var verifyContact = string.Concat(m.EmailAddress, m.HomePhone, m.OfficePhone, m.MobilePhone);
                        if (string.IsNullOrWhiteSpace(verifyContact))
                        {
                            errors.Add(string.Format("Enter at least an <b>email</b> or <b>phone contact</b> for member <b>#{0}</b>", i));
                        }

                        if (members.IsCustom && m.CustomMonthlyFee <= 0)
                        {
                            errors.Add(string.Format("Enter a <b>Custom Fee (Monthly)</b> for member <b>#{0}</b>", i));

                        }
                        i++;


                    }//if


                } //foreach


                if (members.GymMembers.Sum(x => x.MemberId) > 0)
                {
                    IUserService userservice = new UserService();
                    var memberExist = userservice.CheckIfMemberIdExist(members);
                    if (memberExist.Count > 0)
                    {
                        foreach (var m in memberExist)
                        {
                            errors.Add(string.Format("Member Id <b>{0}</b> exists already and is assigned to <b>{1}></b> ", m.MemberId, m.FullnameFromFirstAndLastName));
                        }

                    }

                }


                if (errors.Count > 0)
                {
                    IAdminService adminService = new AdminService();
                    ViewBag.Memberships = adminService.GetMemberships(null).Where(x => x.IsActive).ToList();


                    members.ValidationErrors = errors;
                    return View(members);
                }

                IUserService userService = new UserService();
                var id = userService.AddMembers(members);

                if (members.MembershipCode == "FREE")
                {

                    return RedirectToAction("Member", new { id = id });
                }


                return RedirectToAction("Transaction", new { id = id });
            }
            catch (Exception e)
            {

                log.Error("[NewMember] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");

            }

        }
        


        public ActionResult Transaction(string id)
        {
            try
            {
                int intId = Utils.ParseStringToInt(id);

                

                IUserService userService = new UserService();
                var member = userService.GetMemberById(intId, true);


                if (member != null)
                {
                    member.RelatedMembers = userService.GetRelatedMembers(member.MemberId);
                }

                Transaction transaction;
                if (member.IsPartPayment)
                {
                    transaction = userService.GetTransactionById(member.LastTransactionId);
                    transaction.Member = member;
                 
                }
                else
                {

                    transaction = new Transaction(member);

                }


                return View(transaction);
            }
            catch (Exception e)
            {
                log.Error("[transaction] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public ActionResult VerifyTransaction(Transaction tran)
        {
            try
            {
                return View(tran);
            }
            catch (Exception)
            {

                throw;
            }
        }



        [HttpPost]
        public ActionResult Transaction(Transaction tran)
        {


            try
            {
                if(!tran.IsYearly && tran.CalculatedTotalPaid == 0 && tran.CalculatedWriteOffs == 0)
                {
                    return RedirectToAction("Member", new { id = tran.Member.MemberId });
                }

                    IUserService userService = new UserService();
                userService.SaveTransaction(tran);

                return RedirectToAction("PaymentSummary", new { id = tran.Member.MemberId });
            }
            catch (Exception e)
            {
                log.Error("[Transaction] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");
            }

        }

        public ActionResult PaymentSummary(int id, bool fromReceipt = false)
        {
            try
            {
                IUserService userService = new UserService();

                if (fromReceipt)
                {
                    //here id represents transactionId
                    var item = userService.GetTransactionDetails(id);
                    return View(item);

                }
                else
                {
                    //here id represents memberId

                    var transactions = userService.GetTransactionsByMember(id, true);
                    if (transactions.Count > 0)
                    {
                        var tran = transactions.FirstOrDefault();
                        return View(tran);

                    }

                    return RedirectToAction("Member", new { id = id });


                }

            }
            catch (Exception e)
            {
                log.Error("[PaymentSummary] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");
            }
        }



        public ActionResult test()
        {
            return View();
        }


        public ActionResult CheckIn(int id, string p)
        {
            try
            {
                
                IUserService userService = new UserService();
                userService.CheckInMember(id, false);

                var user = userService.GetMemberById(id);

                if(p=="Member")
                {
                    return RedirectToAction("Member", new { id = id });

                }

                return RedirectToAction("Home");
            }
            catch (Exception e)
            {


                log.Error("[AjaxCheckIn] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");
            }
        }




        public ActionResult AjaxCheckIn(int id, string s, bool p)
        {
            try
            {
                var isRefresh = s == "Home";
                var isPass = p ;

                ContentResult content = new ContentResult();


                IUserService userService = new UserService();
                userService.CheckInMember(id, isPass);

                var user = userService.GetMemberById(id);

                if (isRefresh)
                {
                    var dt = DateTime.Now;
                    var visits = userService.GetMembers(null, dt, true).OrderByDescending(x => x.LastVisit);

                    var result = RenderRazorViewToString(this.ControllerContext, "_Visits", visits);
                    content.Content = result;

                    return content;
                }
                else
                {
                    content.Content = string.Format("Member #{0} {1} checked in.", user.MemberId, user.FullnameFromFirstAndLastName);
                    return content;

                }

            }
            catch (Exception e)
            {


                log.Error("[AjaxCheckIn] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");
            }
        }

        public ActionResult FindUser(string q, string s)
        {
            try
            {

                var isRefresh = s == "Home";

                IUserService userService = new UserService();
                var users = userService.FindMemberByNameOrId(q);

                ContentResult content = new ContentResult();

                StringBuilder sb = new StringBuilder();

                int n;
                var isNum = (Int32.TryParse(q, out n));
                if (users.Count == 0)
                {
                    content.Content = string.Format("<section id='notfound'>Count not find member with {0} <b>{1}</b></section>", isNum ? " ID number " : " name ", q);
                    return content;
                }

                if (users.Count > 1)
                {
                    var result = RenderRazorViewToString(this.ControllerContext, "_Results", users);
                    content.Content = result;

                    return content;
                }

                if (users.Count == 1)
                {

                    var mbr = users.FirstOrDefault();
                    if (mbr.IsPresent || (!mbr.IsActive) || !mbr.HasVisitsLeft)
                    {
                        //if (mbr.IsPass) {
                        //    content.Content = string.Format("LoadPass_{0}", mbr.MemberId);

                        //}
                        //else

                        //{
                        content.Content = string.Format("Load_{0}", mbr.MemberId);

                        //}
                        return content;

                    }
                    //else if (!mbr.HasVisitsLeft)
                    //{

                    //    content.Content = string.Format("LoadPass_{0}", mbr.MemberId);
                    //    return content;


                    //}
                    //else if (mbr.IsPass)
                    //{

                    //    content.Content = string.Format("ChoosePass_{0}", mbr.MemberId);
                    //    return content;


                    //}
                    else
                    {
                        content.Content = string.Format("Choose_{0}", mbr.MemberId);
                        return content;


                    }


                }
                //do not change starting part "Member #"
                content.Content = string.Format("<section id='userNotActive'>Error cannot check in user</section>");
                return content;


            }
            catch (Exception e)
            {

                log.Error("[FindUser] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");
            }

        }


        public ActionResult CancelTransaction(int id)
        {
            try
            {
                log.InfoFormat("GenerateReceipt for cir {0}", id);

                IUserService userService = new UserService();
                var item = userService.GetTransactionById(id);

                return View(item);

            }

            catch (Exception e)
            {
                log.Error("[CancelTransaction] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);

                return RedirectToAction("Error", "Home");
            }

        }

        [HttpPost]
        public ActionResult CancelTransaction(int id, int memberId)
        {
            try
            {
                log.InfoFormat("GenerateReceipt for cir {0}", id);

                IUserService userService = new UserService();

                var item = userService.CancelTransaction(id, memberId);

                return RedirectToAction("Transaction", new { id = memberId });

            }

            catch (Exception e)
            {
                log.Error("[CancelTransaction] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);

                return RedirectToAction("Error", "Home");
            }

        }


        [HttpPost]
        public ActionResult AddComment(MemberComment comment)
        {


            try
            {
                IUserService userService = new UserService();
                userService.SaveComment(comment);

                return RedirectToAction("Member", new { id = comment.MemberId });
            }
            catch (Exception e)
            {
                log.Error("[Transaction] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");
            }

        }


        [HttpPost, ValidateInput(false)]
        public ActionResult AddProfilePicture(HttpPostedFileBase resFile, int memberId)
        {

            try
            {
                UserService service = new UserService();
                service.SaveMemberProfilePic(memberId, resFile);
                
                return RedirectToAction("Member", new { id = memberId });

            }
            catch (Exception e)
            {
                log.Error("[AddMovieResource] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Maintenance");
            }

        }


    }
}