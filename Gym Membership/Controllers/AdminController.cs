using Gym_Membership.Models;
using Gym_Membership.Services.Abstract;
using Gym_Membership.Services.Concrete;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using ExportToExcel;
using Gym_Membership.Helpers;

namespace Gym_Membership.Controllers
{
    public class AdminController : BaseController
    {
        ILog log = log4net.LogManager.GetLogger(typeof(AdminController));


        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult FlushCache()
        {
            Helpers.CacheHelper.FlushCache();
            return RedirectToAction("Index", "Admin");
        }

        public ActionResult Memberships(int? id)
        {

            try
            {
                IAdminService adminService = new AdminService();

                var memberships = adminService.GetMemberships(id).OrderBy(x => x.DisplayOrder).ToList() ;

                return View(memberships);

            }
            catch (Exception e)
            {

                log.Error("[index] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");
            }
        }

        public ActionResult AddMembership()
        {
            return View();
        }

        public ActionResult StandingOrders()
        {

            try
            {
                IUserService userService = new UserService();
                var standingOrders = userService.GetStandingOrders();

                return View(standingOrders);

            }
            catch (Exception e)
            {


                log.Error("[StandingOrders] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");
            }
        }


        public ActionResult ExportToExcel(Enumerations.ExcelExport id = Enumerations.ExcelExport.ALL)
        { 
            try
            {
                IAdminService adminService = new AdminService();
                var listofMemberships = adminService.GetMemberships(null).ToList();


                IUserService userService = new UserService();
                var members= userService.GetMembers(null, null, true);


                if (id == Enumerations.ExcelExport.ALL)
                {

                }

                var memberlist = members.
                                 Select(group =>
                                     new
                                     {
                                         memberid = group.MemberId,
                                         name = group.TitleFullname,
                                         Age = group.Age,
                                         DateOfBirth = group.DateOfBirth,
                                         Town = group.Town,
                                         Mobile = group.MobilePhone,
                                         Phone = group.HomePhone,
                                         Office = group.OfficePhone,
                                         Email = group.EmailAddress,
                                         NumVisits = group.NumberVisits,
                                         LastVisit = group.LastVisit,
                                         Membership = group.Membership.Name,
                                         HasArrears = (group.HasArrears?"Yes":"No"),
                                         LongOverdue = (group.IsLongOverdue ? "Yes" : "No"),
                                         Birthday = (group.IsBirthday ? "Yes" : "No"),
                                         IsActive = (group.IsActive ? "Yes" : "No"),
                                         LastOrNextPaymentDate = group.PaymentUntilDate,
                                         DuetoDate = (group.HasArrears?group.AmountDueToDate:0)
                                     }).ToList();



                var Dt = DateTime.Now;

                ContentResult excel = new ContentResult();

                var target = ConfigurationHelper.GetReportFolder() + "Members as at " + Dt.ToString("dd-MMM-yyyy-HHmmss")+ ".xlsx";

                ViewBag.Target = target;
                 CreateExcelFile.CreateExcelDocument(memberlist, target);

                return View();
            }
            catch (Exception e)
            {

                log.Error("ExportToExcel] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");
            }
        }


        public ActionResult ChangeMembership(int id)
        {
            try
            {

                IUserService userService = new UserService();

                ChangeMembershipViewModel model = new ChangeMembershipViewModel();
                model.Member = userService.GetMemberById(id,true);
                model.RelatedMembers = userService.GetRelatedMembers(id).Members;


                var allmembers = userService.GetMembers();
                model.SingleMembers = allmembers.Where(x => x.IsActive).Where(m => m.Membership.IsSingleMembership).ToList();

                model.SingleMembers.Remove(new GymMember { MemberId = id });



                IAdminService adminService = new AdminService();
               model.Memberships = adminService.GetMemberships(null);
                return View(model);
                
            }
            catch (Exception e)
            {
                
                log.Error("[ChangeMembership] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");
            }

        }


        public ActionResult PassManagement()
        {
            try
            {

            IAdminService adminService = new AdminService();

                var memberships = adminService.GetMemberships(null, true).Where(x => x.IsPass).ToList();
            
            return View(memberships);

            }
            catch (Exception e)
            {

                log.Error("[ChangeMembership] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");
            }
        }


        public ActionResult UpdatePassPrice(string key, float value)
        {
            try
            {

                IAdminService adminService = new AdminService();

                
                var confs = adminService.UpdateMembershipFee(key, value);
                return RedirectToAction("PassManagement");

            }
            catch (Exception e)
            {

                log.Error("[UpdatePassPrice] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");
            }
        }



        [HttpPost]
        public ActionResult ChangeMembership(ChangeMembershipViewModel member)
        {
            try
            {

                IAdminService adminService = new AdminService();
                ChangeMembershipModel model = new ChangeMembershipModel();
                HashSet<Int32> set = new HashSet<Int32>();

                foreach(var i in member.MemberIds)
                {
                    if(i > 0)
                    {
                        set.Add(i);
                    }

                }

                if(member.NewMembershipCode=="COUP")
                {
                    if(set.Count != 1)
                    {
                        member.ValidationErrors.Add("Missing or Duplicate member for couple");
                    }
                }

                if (member.NewMembershipCode == "FAM3")
                {
                    if (set.Count != 2)
                    {
                        member.ValidationErrors.Add("Missing or Duplicate member(s) for Family 3");
                    }
                }
                if (member.NewMembershipCode == "FAM4")
                {
                    if (set.Count != 3)
                    {
                        member.ValidationErrors.Add("Missing or Duplicate member(s) for Family 3");
                    }
                }
                if (member.NewMembershipCode == "FAM5")
                {
                    if (set.Count != 5)
                    {
                        member.ValidationErrors.Add("Missing or Duplicate member(s) for Family 3");
                    }
                }


                if(member.NewMembershipCode == null )
                {
                    member.ValidationErrors.Add("Select a new membership type.");

                }
                
                if(member.HasValidationErrors)
                {

                    IUserService userService = new UserService();

                    
                    member.Member = userService.GetMemberById(member.MainMemberId);
                    member.RelatedMembers = userService.GetRelatedMembers(member.MainMemberId).Members;


                    var allmembers = userService.GetMembers();
                    member.SingleMembers = allmembers.Where(x => x.IsActive).Where(m => m.Membership.IsSingleMembership).ToList();

                    member.SingleMembers.Remove(new GymMember { MemberId = member.MainMemberId });

                    member.Memberships = adminService.GetMemberships(null);

                    return View(member);
                }

     
                model.MainMemberId = member.MainMemberId;
                model.MembershipCode = member.NewMembershipCode;


                foreach(var x in set)
                {

                        model.MemberIdsList.Add(x);
                }

                

                adminService.ChangeMembersMembership(model);



                return RedirectToAction("Member", "User", new  { id = member.MainMemberId });

            }
            catch (Exception e)
            {

                log.Error("[ChangeMembership] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");
            }

        }
        public ActionResult EditMembership(int id)
        {

            try
            {
                IAdminService adminService = new AdminService();

                var membership = adminService.GetMemberships(id).FirstOrDefault();


                return View(membership);

            }
            catch (Exception e)
            {

                log.Error("[index] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");
            }
        }

        public ActionResult ManageUsers()
        {
            try
            {
                IAdminService adminService = new AdminService();
                var admins = adminService.GetAdmins();
                return View(admins);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public ActionResult CreateUser()
        {
            try
            {
                var user = new AdminUser();
                return View(user);
            }
            catch (Exception e)
            {

                log.Error("[CreateUser] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");
            }

        }

        [HttpPost]
        public ActionResult CreateUser(AdminUser user)
        {
            try
            {
                IAdminService adminService = new AdminService();
                var adms = adminService.GetAdmins();
                var userExists = adms.Where(x => x.Username == user.Username).ToList().Count > 0;

                if (userExists)
                {
                    user.ValidationErrors.Add("Username already exists, please change");
                }

                if (user.HasValidationErrors)
                {
                    return View(user);
                }


                adminService.SaveAdmin(user);
                return RedirectToAction("ManageUsers");
            }
            catch (Exception e)
            {

                log.Error("[CreateUser] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");
            }

        }

        public ActionResult DeleteAdminUser(string id)
        {
            try
            {
                IAdminService adminService = new AdminService();

                adminService.DeleteAdmin(id);
                return RedirectToAction("ManageUsers");
            }
            catch (Exception e)
            {

                log.Error("[DeleteAdminUser] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");
            }

        }



        [HttpPost]
        public ActionResult EditMembership(Membership membership)
        {


            try
            {
                IAdminService adminService = new AdminService();

                if (String.IsNullOrWhiteSpace(membership.Name))
                {
                    membership.ValidationErrors.Add("Name is mandatory, Please input one.");
                }

                if (membership.MonthTerms < 1)
                {
                    membership.ValidationErrors.Add("Month should be more than 0.");
                }

                if (!(membership.NumberMembers >= 1 && membership.NumberMembers <= Helpers.ConfigurationHelper.MaxMembersAllowed()))
                {
                    membership.ValidationErrors.Add(String.Format("Number of Members should be between 1 and {0}.", Helpers.ConfigurationHelper.MaxMembersAllowed().ToString()));
                }

                if (membership.HasValidationErrors)
                {
                    return View(membership);
                }

                var users = adminService.SaveMembership(membership);

                return RedirectToAction("Memberships");

            }
            catch (Exception e)
            {

                log.Error("[EditMembership] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");
            }
        }


        [HttpPost]
        public ActionResult AddMembership(Membership membership)
        {


            try
            {
                IAdminService adminService = new AdminService();

                var existingMemberShips = adminService.GetMemberships(null);
                var isExists = existingMemberShips != null && existingMemberShips.Where(x => x.MembershipCode == membership.MembershipCode).ToList().Count > 0;

                if (isExists)
                {
                    membership.ValidationErrors.Add("Membership already exists, please change");
                }

                if (String.IsNullOrWhiteSpace(membership.Name))
                {
                    membership.ValidationErrors.Add("Name is mandatory, Please input one.");
                }

                if (membership.MonthTerms < 1)
                {
                    membership.ValidationErrors.Add("Month should be more than 0.");
                }

                if (!(membership.NumberMembers >= 1 && membership.NumberMembers <= Helpers.ConfigurationHelper.MaxMembersAllowed()))
                {
                    membership.ValidationErrors.Add(String.Format("Number of Members should be between 1 and {0}.", Helpers.ConfigurationHelper.MaxMembersAllowed().ToString()));
                }

                if (membership.HasValidationErrors)
                {
                    return View(membership);
                }

                var users = adminService.AddMembership(membership);

                return RedirectToAction("Memberships");

            }
            catch (Exception e)
            {

                log.Error("[index] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");
            }
        }

        public ActionResult AjaxAdminUpdates(string username, string item, string value)
        {
            ContentResult content = new ContentResult();
            try
            {
                IAdminService adminService = new AdminService();
                content.Content = adminService.SaveAdminDetails(username, item, value);

                return content;
            }
            catch (Exception e)
            {

                log.Error("[AjaxAdminUpdates] - Exception Caught" + e.ToString());
                content.Content = "error occured while updating admin details";
                return content;
            }

        }


        public ActionResult RemoveMember(int id)
        {
            try
            {
                IUserService userService = new UserService();
                var member = userService.GetMemberById(id);

                return View(member);

            }
            catch (Exception e)
            {
                log.Error("[RemoveMember] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");
                
            }
        }

        
        [HttpPost]
        public ActionResult RemoveMemberConfirm(int id)
        {
            try
            {
                IAdminService adminService = new AdminService();
                var member = adminService.DeleteMember(id);

                return RedirectToAction("Index");

            }
            catch (Exception e)
            {
                log.Error("[RemoveMember] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");

            }
        }

    }
}