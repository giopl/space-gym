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
    public class ReportController : Controller
    {
        ILog log = log4net.LogManager.GetLogger(typeof(AdminController));


        private string[] colors = { "#675332", "#0E3D59", "#87A61C", "#F29F05", "#F25C05", "#D92526", "#868BEA", "#4C4C51",
                "#00C2C2", "#EBFB36", "#EC0031", "#4B507B", "#469D00", "#FF3B00", "#57553F", "#B4AF97"};


        // GET: Report
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Admin");
        }


        public ActionResult VisitDetails(string range = null)
        {
            try
            {

                var from = new DateTime(2000, 1, 1);
                var to = new DateTime(2020, 1, 1);



                if (!string.IsNullOrWhiteSpace(range))
                {
                    var ranges = range.Split('-');


                    DateTime.TryParse(ranges[0], out from);
                    DateTime.TryParse(ranges[1], out to);


                }

                IAdminService adminService = new AdminService();
                var visits = adminService.GetVisitDetails().Where(x => x.VisitDate >= from && x.VisitDate <= to).OrderBy(o => o.YearMonthInt).ToList();

                VisitDataViewModel Model = new VisitDataViewModel();
                Model.DateRange = range;

                var AgeGroupCount = visits.OrderBy(o => o.AgeGroup).GroupBy(n => n.AgeGroupDesc).
                                  Select(group =>
                                      new
                                      {
                                          AgeGroupDesc = group.Key,
                                          Count = group.Count()
                                      });
                //       string[] colors = { "#675332", "#0E3D59", "#87A61C", "#F29F05", "#F25C05", "#D92526", "#868BEA", "#4C4C51",
                //      "#00C2C2", "#EBFB36", "#EC0031", "#4B507B", "#469D00", "#FF3B00", "#57553F", "#B4AF97"};
                var i = 0;
                List<Serie> AgeGroupSerie = new List<Serie>();
                foreach (var agc in AgeGroupCount)
                {
                    if (!string.IsNullOrWhiteSpace(agc.AgeGroupDesc))
                    {
                        Serie s = new Serie
                        {
                            name = agc.AgeGroupDesc,
                            y = agc.Count,
                            color = colors[i]
                        };
                        i++;
                        AgeGroupSerie.Add(s);
                    }

                }
                Model.VisitPerAgeGroup = AgeGroupSerie;


                /* membership group count */
                var MembershipGroupCount = visits.OrderBy(o => o.Membership).GroupBy(n => n.MembershipCategory).
                  Select(group =>
                      new
                      {
                          MembershipCateg = group.Key,
                          Count = group.Count()
                      });


                var j = 0;
                List<Serie> MembershipSerie = new List<Serie>();
                foreach (var item in MembershipGroupCount)
                {
                    if (!string.IsNullOrWhiteSpace(item.MembershipCateg))
                    {
                        Serie s = new Serie
                        {
                            name = item.MembershipCateg,
                            y = item.Count,
                            color = colors[j]
                        };
                        j++;
                        MembershipSerie.Add(s);
                    }

                }
                Model.VisitPerMembership = MembershipSerie;

                /* year month visit */
                var ymc = visits.OrderBy(o => o.YearMonthInt).Select(x => x.YearMonth).Distinct();
                StringBuilder sbYmc = new StringBuilder();
                foreach (var s in ymc)
                {
                    sbYmc.AppendFormat("'{0}',", s);
                }
                if (sbYmc.Length > 0)
                    sbYmc.Length--;

                Model.YearMonthCodeCateg = sbYmc.ToString();

                /* visit count of by year month */
                var VisitCount = visits.GroupBy(n => n.YearMonth).
                                     Select(group =>
                                         new
                                         {
                                             YearMonth = group.Key,
                                             Count = group.Count()
                                         });

                var VisitFemaleCount = visits.Where(x => x.Gender == "F").GroupBy(n => n.YearMonth).
                     Select(group =>
                         new
                         {
                             YearMonth = group.Key,
                             Count = group.Count()
                         });


                StringBuilder sbYmcCnt = new StringBuilder();
                foreach (var v in VisitCount)
                {
                    sbYmcCnt.AppendFormat("{0},", v.Count);
                }

                if (sbYmcCnt.Length > 0)
                    sbYmcCnt.Length--;

                Model.VisitsPerYearMonth = sbYmcCnt.ToString();


                StringBuilder sbYmcFemaleCnt = new StringBuilder();
                foreach (var v in VisitFemaleCount)
                {
                    sbYmcFemaleCnt.AppendFormat("{0},", v.Count);

                }

                if (sbYmcFemaleCnt.Length > 0)
                    sbYmcFemaleCnt.Length--;
                Model.FemaleVisitsPerYearMonth = sbYmcFemaleCnt.ToString();


                /* count per day of week */
                var DayOfWeekCount = visits.OrderBy(o => o.DayOfWeekInt).GroupBy(n => n.DayOfWeek).
                                     Select(group =>
                                         new
                                         {
                                             DayOfWeek = group.Key,
                                             Count = group.Count()
                                         });

                StringBuilder sbDoWCateg = new StringBuilder();
                StringBuilder sbDoWData = new StringBuilder();
                foreach (var v in DayOfWeekCount)
                {
                    sbDoWCateg.AppendFormat("'{0}',", v.DayOfWeek);
                    sbDoWData.AppendFormat("{0},", v.Count);

                }

                if (sbDoWCateg.Length > 0)
                    sbDoWCateg.Length--;
                if (sbYmcFemaleCnt.Length > 0)
                    sbYmcFemaleCnt.Length--;

                Model.DayOfWeekCateg = sbDoWCateg.ToString();
                Model.DayOfWeekData = sbDoWData.ToString();


                /* count per day of month */
                var DayOfMonthCount = visits.OrderBy(o => o.DayDate).GroupBy(n => n.DayDate).
                                     Select(group =>
                                         new
                                         {
                                             DayDate = group.Key,
                                             Count = group.Count()
                                         });



                StringBuilder sbDoMCateg = new StringBuilder();
                StringBuilder sbDoMData = new StringBuilder();
                foreach (var v in DayOfMonthCount)
                {
                    sbDoMCateg.AppendFormat("'{0}',", v.DayDate);
                    sbDoMData.AppendFormat("{0},", v.Count);

                }

                if (sbDoWCateg.Length > 0)
                    sbDoMCateg.Length--;

                if (sbDoMData.Length > 0)
                    sbDoMData.Length--;

                Model.DayOfMonthCateg = sbDoMCateg.ToString();
                Model.DayOfMonthData = sbDoMData.ToString();


                /* count per time of day */
                var HourOfDayCount = visits.OrderBy(o => o.VisitHour).GroupBy(n => n.VisitHour).
                                  Select(group =>
                                      new
                                      {
                                          VisitHour = group.Key,
                                          Count = group.Count()
                                      });

                StringBuilder SbHourOfDay = new StringBuilder();
                int[] HoursOfDay = { 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22 };

                foreach (var m in HourOfDayCount)
                {
                    var exists = HoursOfDay.Contains(m.VisitHour);

                    if (m.VisitHour >= 5 && m.VisitHour <= 22)
                    {

                        if (exists)
                        {

                            SbHourOfDay.AppendFormat("{0},", m.Count);
                        }
                        else
                        {
                            SbHourOfDay.AppendFormat("{0},", 0);

                        }
                    }

                }


                if (SbHourOfDay.Length > 0)
                    SbHourOfDay.Length--;
                Model.HourOfDayData = SbHourOfDay.ToString();

                var memberships = adminService.GetMemberships(null).OrderBy(o => o.MembershipCode).ToList();
                var visitsPerMembership = visits.OrderBy(o => o.Membership).GroupBy(n => n.MembershipName).
                               Select(group =>
                                   new
                                   {
                                       Membership = group.Key,
                                       Count = group.Count()
                                   });


                ViewBag.VisitsPerMembership = visitsPerMembership;


                return View(Model);

            }
            catch (Exception e)
            {

                log.Error("[VisitDetails] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");

            }
        }

        public ActionResult MemberProfiles()
        {

            IAdminService adminService = new AdminService();
            var profiles = adminService.GetMemberProfiles();

            var Model = new MemberProfilesViewModel();


            Model.MaleCount = profiles.Where(x => x.Gender == "M").ToList().Count();
            Model.FemaleCount = profiles.Where(x => x.Gender == "F").ToList().Count();


            var membershipsCount = profiles.OrderBy(o => o.AgeGroup).GroupBy(n => n.AgeGroupDesc).
                                 Select(group =>
                                     new
                                     {
                                         Membership = group.Key,
                                         Count = group.Select(x => x.Count).Sum()
                                     });

            var i = 0;
            var AgeGroupSeries = new List<Serie>();
            foreach (var m in membershipsCount)
            {
                if (!String.IsNullOrWhiteSpace(m.Membership))
                {

                    Serie s = new Serie
                    {
                        name = m.Membership,
                        y = m.Count,
                        color = colors[i]
                    };

                    i++;
                    AgeGroupSeries.Add(s);
                }
            }

            Model.AgeGroup = AgeGroupSeries;


            var memberships = adminService.GetMemberships(null).OrderBy(o => o.Category).ToList();

            StringBuilder SbMembershipCategs = new StringBuilder();
            StringBuilder SbMembershipFemale = new StringBuilder();
            foreach (var mem in memberships)
            {
                SbMembershipCategs.AppendFormat("'{0}',", mem.Name);

            }
            SbMembershipCategs.Length--;
            var cm = SbMembershipCategs.ToString();
            Model.MembershipCategories = SbMembershipCategs.ToString();



            var membershipsFemale = profiles.Where(x => x.Gender == "F").OrderBy(o => o.MembershipCategory).GroupBy(n => n.Membership).
                                 Select(group =>
                                     new
                                     {
                                         Membership = group.Key,
                                         Count = group.Select(x => x.Count).Sum()
                                     });

            foreach (var mf in memberships)
            {

                var exists = membershipsFemale.Where(x => x.Membership == mf.MembershipCode).ToList();
                if (exists.Count > 0)
                {

                    SbMembershipFemale.AppendFormat("{0},", exists.FirstOrDefault().Count);
                }
                else
                {
                    SbMembershipFemale.AppendFormat("{0},", 0);

                }

            }
            SbMembershipFemale.Length--;

            Model.CountMembershipFemale = SbMembershipFemale.ToString();

            var membershipsMale = profiles.Where(x => x.Gender == "M").OrderBy(o => o.MembershipCategory).GroupBy(n => n.Membership).

                                 Select(group =>
                                     new
                                     {
                                         Membership = group.Key,
                                         Count = group.Select(x => x.Count).Sum()
                                     });



            StringBuilder SbMembershipMale = new StringBuilder();

            foreach (var mf in memberships)
            {
                var exists = membershipsMale.Where(x => x.Membership == mf.MembershipCode).ToList();
                if (exists.Count > 0)
                {

                    SbMembershipMale.AppendFormat("{0},", exists.FirstOrDefault().Count);
                }
                else
                {
                    SbMembershipMale.AppendFormat("{0},", 0);

                }



            }
            SbMembershipMale.Length--;
            //SbMembershipMale.Append("]");

            Model.CountMembershipMale = SbMembershipMale.ToString();
            //agGrp
            var AgeGroupMale = profiles.Where(x => x.Gender == "M").OrderBy(o => o.AgeGroup).GroupBy(n => n.AgeGroup).

                               Select(group =>
                                   new
                                   {
                                       AgeGroup = group.Key,
                                       Count = group.Select(x => x.Count).Sum()
                                   });

            var AgeGroupFemale = profiles.Where(x => x.Gender == "F").OrderBy(o => o.AgeGroup).GroupBy(n => n.AgeGroup).

                               Select(group =>
                                   new
                                   {
                                       AgeGroup = group.Key,
                                       Count = group.Select(x => x.Count).Sum()
                                   });



            StringBuilder SbAgeGroupMale = new StringBuilder();
            string[] AgeGroupCodes = { "A", "B", "C", "D", "E", "F", "G", "H" };
            foreach (var m in AgeGroupMale)
            {
                var exists = AgeGroupCodes.Contains(m.AgeGroup);
                //var exists = AgeGroupMale.Where(x => x.AgeGroup == AgeGroups).ToList();

                if (!string.IsNullOrWhiteSpace(m.AgeGroup))
                {

                    if (exists)
                    {

                        SbAgeGroupMale.AppendFormat("{0},", m.Count);
                    }
                    else
                    {
                        SbAgeGroupMale.AppendFormat("{0},", 0);

                    }
                }

            }

            Model.CountAgeGroupMale = SbAgeGroupMale.ToString();



            StringBuilder SbAgeGroupFemale = new StringBuilder();
            foreach (var m in AgeGroupFemale)
            {
                var exists = AgeGroupCodes.Contains(m.AgeGroup);
                //var exists = AgeGroupFemale.Where(x => x.AgeGroup == AgeGroups).ToList();


                if (!string.IsNullOrWhiteSpace(m.AgeGroup))
                {


                    if (exists)
                    {

                        SbAgeGroupFemale.AppendFormat("{0},", m.Count);
                    }
                    else
                    {
                        SbAgeGroupFemale.AppendFormat("{0},", 0);

                    }

                }
            }

            Model.CountAgeGroupFemale = SbAgeGroupFemale.ToString();

            return View(Model);
        }

        public ActionResult RevenueAnalysis(string range = null)
        {
            try
            {

                var from = new DateTime(2000, 1, 1);
                var to = new DateTime(2020, 1, 1);



                if (!string.IsNullOrWhiteSpace(range))
                {
                    var ranges = range.Split('-');


                    DateTime.TryParse(ranges[0], out from);
                    DateTime.TryParse(ranges[1], out to);


                }



                IAdminService adminService = new AdminService();
                var revenue = adminService.GetRevenueDetails();

                RevenueAnalysisViewModel Model = new RevenueAnalysisViewModel();

                var fromYMC = (from.Year * 100) + from.Month;
                var toYMC = (to.Year * 100) + to.Month;

                var RevenuePerYearMonth = revenue.Where(x => x.YearMonthPayment >= fromYMC && x.YearMonthPayment <= toYMC).OrderBy(o => o.YearMonthPayment).GroupBy(n => n.YearMonthPayment).

                            Select(group =>
                                new
                                {
                                    YearMonth = group.Key,
                                    SumFee = group.Select(x => x.Bill).Sum(),
                                    SumWo = group.Select(x => x.WrittenOff).Sum(),
                                    SumDsc = group.Select(x => x.Discounted).Sum(),
                                    SumReg = group.Select(x => x.RegistrationAmount).Sum(),
                                    SumPaid = group.Select(x => x.Paid).Sum(),

                                });


                var budgeted = adminService.GetBudgetedRevenue();
                var potentialRevenueAmt = budgeted.Sum(x => x.PotentialRevenue);


                StringBuilder categ = new StringBuilder();
                StringBuilder data1 = new StringBuilder();
                StringBuilder data2 = new StringBuilder();

                foreach (var r in RevenuePerYearMonth)
                {
                    categ.AppendFormat("'{0}',", r.YearMonth);
                    data1.AppendFormat("{0},", r.SumFee);
                    data2.AppendFormat("{0},", r.SumWo + r.SumDsc);
                    Stat stat = new Stat()
                    {
                        YearMonthPayment = r.YearMonth,
                        Bill = r.SumFee,
                        WrittenOff = r.SumWo,
                        Discounted = r.SumDsc,
                        RegistrationAmount = r.SumReg,
                        Paid = r.SumPaid,
                        PotentialRevenue = potentialRevenueAmt
                    };
                    Model.RevenueTable.Add(stat);
                }
                if (categ.Length > 0)
                    categ.Length--;

                if (data1.Length > 0)
                    data1.Length--;

                if (data2.Length > 0)
                    data2.Length--;


                Model.RevenuePerMonthCategory = categ.ToString();
                Model.RevenuePerMonthActual = data1.ToString();
                Model.RevenuePerMonthDiscounted = data2.ToString();



                var RevenueReceivedMonth = revenue.Where(x => x.ReceiptYearMonth >= fromYMC && x.ReceiptYearMonth <= toYMC)
                    .OrderBy(o => o.ReceiptYearMonth).GroupBy(n => n.ReceiptYearMonth).

                            Select(group =>
                                new
                                {
                                    YearMonth = group.Key,
                                    Sum = group.Select(x => x.FeeAndRegistration).Sum()

                                });

                foreach (var rrm in RevenueReceivedMonth)
                {
                    Stat stat = new Stat
                    {
                        YearMonthPayment = rrm.YearMonth,
                        Paid = rrm.Sum

                    };
                    Model.ReceptionTable.Add(stat);
                }
                var PaymentMethod = revenue.Where(x => x.YearMonthPayment >= fromYMC && x.YearMonthPayment <= toYMC).OrderBy(o => o.PaymentMethod).GroupBy(n => n.PaymentMethod).

                           Select(group =>
                               new
                               {
                                   PaymentMethod = group.Key,
                                   Bill = group.Select(x => x.Paid).Sum(),

                               });

                List<Serie> paymentMethodSeries = new List<Serie>();
                var i = 0;
                foreach (var p in PaymentMethod)
                {
                    Serie s = new Serie
                    {
                        color = colors[i],
                        name = p.PaymentMethod,
                        yy = p.Bill

                    };
                    paymentMethodSeries.Add(s);
                    i++;
                }

                Model.PaymentMethod = paymentMethodSeries;


                //Revenue by age group

                var RevenueAgeGroup = revenue.Where(x => x.YearMonthPayment >= fromYMC && x.YearMonthPayment <= toYMC)
                    .OrderBy(o => o.AgeGroup).GroupBy(n => n.AgeGroupDesc).

                           Select(group =>
                               new
                               {
                                   AgeGroup = group.Key,
                                   Paid = group.Select(x => x.Paid).Sum(),

                               });

                List<Serie> RevenueAgeGroupSeries = new List<Serie>();
                var j = 0;
                foreach (var p in RevenueAgeGroup)
                {
                    Serie s = new Serie
                    {
                        color = colors[j],
                        name = p.AgeGroup,
                        yy = p.Paid

                    };
                    RevenueAgeGroupSeries.Add(s);
                    j++;
                }

                Model.RevenueAgeGroup = RevenueAgeGroupSeries;

                // revenue membership
                var RevenueMembership = revenue.Where(x => x.YearMonthPayment >= fromYMC && x.YearMonthPayment <= toYMC)
                    .OrderBy(o => o.Membership).GroupBy(n => n.MembershipCategory).

                           Select(group =>
                               new
                               {
                                   Membership = group.Key,
                                   Paid = group.Select(x => x.Paid).Sum(),

                               });

                List<Serie> RevenueMembershipSeries = new List<Serie>();
                var k = 0;
                foreach (var p in RevenueMembership)
                {
                    Serie s = new Serie
                    {
                        color = colors[k],
                        name = p.Membership,
                        yy = p.Paid

                    };
                    RevenueMembershipSeries.Add(s);
                    k++;
                }

                Model.RevenueMembership = RevenueMembershipSeries;





                Model.DateRange = range;

                return View(Model);
            }
            catch (Exception)
            {

                throw;
            }
        }


        public ActionResult DailyTransactionReport(string range = null)
        {

            try
            {

                var from = DateTime.Now;
                var to =  DateTime.Now;



                if (!string.IsNullOrWhiteSpace(range))
                {
                    var ranges = range.Split('-');


                    DateTime.TryParse(ranges[0], out from);
                    DateTime.TryParse(ranges[1], out to);


                }


                IAdminService adminService = new AdminService();
                var reports = adminService.GetReceiptReport(from, to).Where(x=> x.IsCancelled == false).ToList();

                DailyTransactionsViewModel model = new DailyTransactionsViewModel { Transactions = reports };

                return View(model);
            }
            catch (Exception e)
            {


                log.Error("[TransactionReport] - Exception Caught" + e.ToString());
                TempData["errorLog"] = new ErrorLog(e);
                return RedirectToAction("Error", "Home");
            }
        }




    }
}