using Gym_Membership.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gym_Membership.Models
{
    public class Transaction
    {
        private List<Payment> _PaymentsDue;



        //added as temporary fix for wrong calculations of date and amount

        public DateTime OverridenEndDate { get; set; }
        public DateTime OverridenStartDate { get; set; }
        public double PaidAmountOverriden { get; set; }
        public double DiscountAmountOverriden { get; set; }
        public double RegistrationAmountOverriden { get; set; }
        public double WrittenOffAmountOverriden { get; set; }
        public double UnpaidAmountOverriden { get; set; }
        
        /* transaction fields */



        public bool IsStandingOrder { get; set; }
        public Transaction()
        {
            Member = new GymMember();
            //Membership = new Membership();
            PaymentDueForm = new List<Payment>();
            //ReceivedBy = new AdminUser();
        //    ExistingFacilities = new List<Facility>();
            Receipts = new List<Receipt>();

        }

       // public IList<Facility> Facilities { get; set; }

        public Transaction(GymMember member)
        {
            Member = member;
            //Membership = membership;
            PaymentDueForm = new List<Payment>();
       //     ExistingFacilities = new List<Facility>();
            //ReceivedBy = new AdminUser();
            Receipts = new List<Receipt>();

        }


        public Receipt LastReceipt
        {
            get
            {
                if(Receipts.Count>0)
                {
                    return Receipts.OrderByDescending(x => x.ReceivedOn).ToList().FirstOrDefault();

                } else
                {
                    return new Receipt();

                }

            }

        }

        //not persisted used only as a flag when creating transaction
        public bool IsPass { get; set; }
        public IList<Receipt> Receipts { get; set; }

        public int LastTransactionId { get; set; }

        
        public int TransactionId { get; set; }

        public GymMember Member { get; set; }

       // public Membership Membership { get; set; }

        //passed via form
        public IList<Payment> PaymentDueForm { get; set; }
        //passed via form
        public Double MonthlyDueAmountForm { get; set; }

        public DateTime StartDate {get;set;}
        public DateTime EndDate { get; set; }

        public int MonthsInAdvanceForm { get; set; }

        

        [MaxLength(1000)]
        public String Comment { get; set; }

        public double DiscountAmount { get; set; }
        public double PaidAmount { get; set; }
        public double RegistrationAmount { get; set; }
        public double WrittenOffAmount { get; set; }
        public double FirstInstallmentAmount { get; set; }
        public double SecondInstallmentAmount { get; set; }


        [DisplayName("Payment Method")]
        public string PaymentMethodForm { get; set; }

        /// <summary>
        /// used for yearly payment
        /// </summary>
        public int NumInstallments { get; set; }

        public int InstallmentNum
        {
            get
            {
                var i = 0;
                if(IsPartPayment)
                {
                    if(FirstInstallmentAmount == 0)
                    {
                        i = 1; 

                    } else
                    {
                        i = 2;
                    }
                }
                return i;
            }
        }

        public int NumInstallmentsLeft { get; set; }
        //public bool IsPartPayment { get ; set; }
        public DateTime TransactionDate { get; set; }

        public bool IsPartPayment
        {
            get
            {
                return UnpaidAmount > 0;
            }
        }


        public bool PartPaymentForm { get; set; }




        

        /// <summary>
        /// used for yearly payment
        /// </summary>
        public double InitialDownpayment { get; set; }

        
        public double RegistrationDue
        {
            get
            {
                if(Member.IsRegistrationPaid)
                {
                    return 0;
                } else
                {
                    return Member.Membership.RegistrationFeePerPerson;
                }
            }
        }


        //public double RegistrationAmountDue
        //{
        //    get
        //    {
        //        if (Member.IsRegistrationPaid)
        //        {
        //            return 0;
        //        }

        //        var numMem = Membership.MembershipCode == "CUST"
        //                 || String.IsNullOrWhiteSpace(Membership.MembershipCode)
        //           ? 1 : Membership.NumberMembers;

        //        return Membership.RegistrationFee / numMem;
        //    }

        //}


        //public double RegistrationFeeForm { get; set; }

        public Double Fee
        {
            get
            {
                return Member.Membership.Fee;
            }
        }
        //public double OrigRegistrationFee { get; set; }
        //public double OrigMembershipFee { get; set; }


        private DateTime PaymentUntilDate { get { return Member.PaymentUntilDate; } }
        public  DateTime NextPaymentDate { get { return Member.PaymentUntilDate.AddDays(1); } }


        private DateTime CurrentPaymentEndDate
        {
            get
            {
                return Utils.GetLastDayOfMonth(DateTime.Now).AddDays(CalculatedPostponedDays*-1);
            }
        }


        public bool hasDues
        {
            get
            {
                return CurrentPaymentEndDate > PaymentUntilDate;
            }
        }

        /// <summary>
        /// dues more than one year used for yearly
        /// </summary>
        public bool HasLongDues
        {
            get
            {
                if(hasDues)
                {
                    TimeSpan ts = CurrentPaymentEndDate - PaymentUntilDate;
                    var days = ts.TotalDays;

                    return days > 365D;

                }

                return false;
            }
        }

        public int tempDays
        {
            get
            {
                TimeSpan ts = CurrentPaymentEndDate - PaymentUntilDate;
                var days = ts.TotalDays;

                return (int)days ;

            }
        }

        public bool HasShortDues
        {
            get
            {
                if (hasDues)
                {
                    TimeSpan ts = CurrentPaymentEndDate - PaymentUntilDate;

                    var days = ts.TotalDays;

                    return days <= 365D;

                }

                return false;
            }
        }



        //private int numberOfMembers { 
        //    get
        //    {
        //        var numMem = Membership.MembershipCode == "CUST"
        //              || String.IsNullOrWhiteSpace(Membership.MembershipCode)
        //        ? 1 : Membership.NumberMembers;
        //        return numMem;

        //    }
        //}

        public double MonthlyFee
        {
            get
            {
                //var numMem = Membership.MembershipCode == "CUST" 
                //          || String.IsNullOrWhiteSpace(Membership.MembershipCode)
                //    ? 1 : Membership.NumberMembers;

                return Member.Membership.Fee / (Member.Membership.MonthTerms * Member.Membership.NumberMembers) ;
            }
        }


        public double CalculatedAmountDuePlusRegistration
        {
            get
            {
                return CalculatedAmountDue + RegistrationDue;

            }

        }



        /// <summary>
        /// calculates the amount due by the member 
        /// </summary>
        public double CalculatedAmountDue
        {
            get
            {

                //DateTime lastPaymentDate = Member.PaymentUntilDate.AddDays(1);
                //used to calculate pro-rata amount
                DateTime LastDayOfMonthOfLastPaymentDate = new DateTime(NextPaymentDate.Year, NextPaymentDate.Month, DateTime.DaysInMonth(NextPaymentDate.Year, NextPaymentDate.Month));

                //current payment date is always last day of current month
                DateTime CurrentPaymentEndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));


                //nothing due
                if (NextPaymentDate >= CurrentPaymentEndDate)
                {
                    return 0;

                }
                else
                {

                  //  double Fee = Membership.Fee / Membership.NumberMembers;

                    //find montly fee in case membership is yearly
                    //     double MonthlyFee = Membership.Fee / (Membership.MonthTerms * 1.0);

                    // calculate number of months since last payment date and cuurent date
                    int NumMonths = (((CurrentPaymentEndDate.Year - NextPaymentDate.Year) * 12) + CurrentPaymentEndDate.Month - NextPaymentDate.Month) + 1;

                    //if (NumMonths > 1)
                    //{

                        var fees = (NumMonths - 1) * MonthlyFee;
                        TimeSpan ts = LastDayOfMonthOfLastPaymentDate - NextPaymentDate;
                        var days = ts.TotalDays;

                        var day = NextPaymentDate.Day;
                         var rate = 1.0;
                        if(day <=10)
                        {

                            rate = ConfigurationHelper.ProrataFirst();
                        } else if (day > 10 && day <=20)
                        {
                            rate = ConfigurationHelper.ProrataSecond();

                        }
                        else
                        {
                            rate = ConfigurationHelper.ProrataThird();

                        }

                       // var prorata = days * (MonthlyFee / DateTime.DaysInMonth(lastPaymentDate.Year, lastPaymentDate.Month));
                        var prorata = (MonthlyFee * rate);

                        
                        return fees + prorata;
                    //}
                    //else
                    //{
                    //    return MonthlyFee;
                    //}

                }

            }

        }


        public double CalculatedAmountDueForYearlyLongOverdue
        {
            get
            {

                if (IsYearly && HasLongDues)
                {
                    var yearlyfee = Fee;
                    var lastDayOfLastMonth = Utils.GetLastDayOfMonth(DateTime.Now.AddMonths(-1));
                    TimeSpan ts = lastDayOfLastMonth - NextPaymentDate;
                    var daysOverdue = ts.TotalDays + 1;
                    var dailyFee = Fee / 365;
                    var amountOverdueProrata = dailyFee * daysOverdue;
                    return amountOverdueProrata;
                }
                return 0;
            }
        }


        /// three options yearly
        /// 1. in order - SD - ED current
        /// 2. overdue < 1 year 2 
        /// 3. overdue > 1 year

        /// <summary>
        /// calcualted start of next payment period
        /// last date + 1 day
        /// </summary>
        public DateTime CalculatedStartingPeriodDate
        {
            get
            {
                return NextPaymentDate;

            }

        }



        public DateTime CalculatedEndingPeriodDate
        {
            get
            {

                if (MonthsInAdvanceForm > 0)
                {

                    if (NextPaymentDate > DateTime.Now)
                    {

                        //added to fix bug with if  NPD is the first of the month, then 1st month is current month not next month
                        var removemonthifcurrent = NextPaymentDate.Day == 1 ? -1 : 0;
                        return Utils.GetLastDayOfMonth(NextPaymentDate.AddMonths(MonthsInAdvanceForm + removemonthifcurrent));

                    }
                    else
                    {
                    return Utils.GetLastDayOfMonth(CurrentPaymentEndDate.AddMonths(MonthsInAdvanceForm));

                    }


                }
                else
                {
                    return CurrentPaymentEndDate;
                }
            }
        }


        public List<Payment> CalculatedPaymentAdvances
        {
            get
            {

                var disc = DiscountAmount <= (MonthsInAdvanceForm * MonthlyDueAmountForm)
                    ? DiscountAmount / MonthsInAdvanceForm : MonthlyDueAmountForm;

                List<Payment> advances = new List<Payment>();

                if (MonthsInAdvanceForm > 0)
                {

                    var _numberOfMembers = Member.Membership.NumberMembers <= 0 ? 1 : Member.Membership.NumberMembers;

                    for (var i = 1; i <= MonthsInAdvanceForm; i++)
                    {
                        Payment p = new Payment();
                        p.FeeAmount = MonthlyDueAmountForm/_numberOfMembers;
                        p.YearMonth = Utils.YearMonthCode(CurrentPaymentEndDate.AddMonths(i));
                        p.PaidAmount = p.FeeAmount - disc;
                        p.DiscountedAmount = disc;

                        advances.Add(p);
                    }
                }
                return advances;
            }

        }



        /// <summary>
        /// calculated payments due sent to form
        /// </summary>
        public List<Payment> CalculatedPaymentsDue
        {
            get
            {
                List<Payment> dues = new List<Payment>();

                if (hasDues)
                {
                    DateTime ActualLastPaymentDate = NextPaymentDate;

                    //calculates number of month between due date and now
                    int NumMonths = (((CurrentPaymentEndDate.Year - ActualLastPaymentDate.Year) * 12) +
                    CurrentPaymentEndDate.Month - ActualLastPaymentDate.Month) + 1;

                    // var MonthlyFee = Membership.Fee / (Membership.MonthTerms * 1.0);  

                    //creates a payment object for each month
                    for (int i = 1; i <= NumMonths; i++)
                    {
                        Payment paym = new Payment();
                        paym.YearMonth = Utils.YearMonthCode(ActualLastPaymentDate.AddMonths(i - 1));

                        if (i == 1)
                        {
                            //if first month calculates prorata
                            DateTime LastDayOfMonthOfLastPaymentDate = new DateTime(ActualLastPaymentDate.Year, ActualLastPaymentDate.Month,
                                DateTime.DaysInMonth(ActualLastPaymentDate.Year, ActualLastPaymentDate.Month));

                            TimeSpan ts = LastDayOfMonthOfLastPaymentDate - ActualLastPaymentDate;
                            var days = ts.TotalDays;

                            paym.RemainingDays = (int)days + 1;

                            var day = ActualLastPaymentDate.Day;
                            var rate = 1.0;
                            if (day <= 10)
                            {

                                rate = ConfigurationHelper.ProrataFirst();
                            }
                            else if (day > 10 && day <= 20)
                            {
                                rate = ConfigurationHelper.ProrataSecond();

                            }
                            else
                            {
                                rate = ConfigurationHelper.ProrataThird();

                            }
                            var prorata = MonthlyFee * rate;
                            //var prorata = days * (MonthlyFee / DateTime.DaysInMonth(LastPaymentDate.Year, LastPaymentDate.Month));

                            if (prorata < MonthlyFee)
                            {
                                paym.IsProrata = true;
                            }

                            paym.FeeAmount = prorata;
                        }
                        else
                        {
                            paym.RemainingDays = paym.DaysInMonth[Convert.ToInt32(paym.YearMonth.ToString().Substring(4, 2))-1];
                            paym.FeeAmount = MonthlyFee;
                        }

                        //only adds month for which a fee exists
                        if (paym.FeeAmount > 0)
                        {
                            dues.Add(paym);
                        }

                    }
                }
                return dues;

            }
            set { _PaymentsDue = value; }
        }


        /// <summary>
        /// calculate payment due list returned from form
        /// includes discount
        /// </summary>
        public List<Payment> CalculatedPaymentDueForm
        {
            get
            {

                var extraDisc = DiscountAmount > (MonthsInAdvanceForm * MonthlyDueAmountForm)
                    ? DiscountAmount - (MonthsInAdvanceForm * MonthlyDueAmountForm) : 0;

                List<Payment> dues = new List<Payment>();

                if (PaymentDueForm.Count > 0)
                {

                    var totDue = PaymentDueForm.Where(x => x.IsWrittenOff == false && x.IsPostPoned == false).ToList().Sum(x => x.FeeAmount);

                    var m_YearMonth = PaymentDueForm.Min(x => x.YearMonth);

                    foreach (var due in PaymentDueForm)
                    {
                        if(!due.IsPostPoned)
                        {

                        Payment p = new Payment();
                        p.FeeAmount = due.FeeAmount;
                        p.YearMonth = m_YearMonth;
                            p.IsWrittenOff = due.IsWrittenOff;
                        p.DiscountedAmount = due.IsWrittenOff ? 0 : extraDisc * (p.FeeAmount / totDue);
                        p.PaidAmount = due.IsWrittenOff ? 0 : due.FeeAmount - p.DiscountedAmount;
                        p.IsPostPoned = due.IsPostPoned;
                        p.RemainingDays = due.RemainingDays;

                        dues.Add(p);

                            m_YearMonth = Helpers.Utils.NextYearMth(m_YearMonth);
                        }
                    }
                }
                return dues;
            }

        }

         
        public double CalculatedTotalPaid
        {
            get
            {

                var duePaid = CalculatedPaymentDueForm.Count > 0 ? CalculatedPaymentDueForm.Sum(x => x.PaidAmount)  : 0;
                  var advPaid = CalculatedPaymentAdvances.Count > 0 ? CalculatedPaymentAdvances.Sum(x => x.PaidAmount) : 0;
                  var tot = duePaid + advPaid;
                  return tot;

            }
        }


        public double CalculatedWriteOffs
        {
            get
            {
                var writeoffs = CalculatedPaymentDueForm.Count > 0 ? CalculatedPaymentDueForm.Where(x=>x.IsWrittenOff).ToList().Sum(x => x.FeeAmount) : 0;
                return writeoffs;
            }
        }



        public int CalculatedPostponedDays
        {
            get
            {
                var postponed = PaymentDueForm.Count > 0 ? PaymentDueForm.Where(x => x.IsPostPoned).ToList().Sum(x => x.RemainingDays) : 0;
                return postponed;
            }
        }








        public double CalculatedTotalDiscount
        {
            get
            {
               // dueDi PaymentDue.Count > 0 ? PaymentDue.Sum(x => x.DiscountedAmount) : 0 +
                var disc = CalculatedPaymentAdvances.Count > 0 ? CalculatedPaymentAdvances.Sum(x => x.DiscountedAmount) : 0;
                return disc;

            }
        }



        public double CalculatedFeeIncludingOverdue
        {
            get
            {
                return Fee + CalculatedAmountDueForYearlyLongOverdue;
            }
        }


        public int TotalNumberOfMonths
        {
            get
            {
                return Utils.NumberOfMonthsBetweenDates(StartDate, EndDate) + 1;

            }
        }

        /// <summary>
        /// used for yearly memberships
        /// </summary>
        public List<Payment> CalculatedYearlyDue
        {
            get
            {

                
                List<Payment> dues = new List<Payment>();

                
                
                    var yearlyDiscount = DiscountAmount;
                    var yearlyRegistration = RegistrationDue;
                var totalFee = CalculatedFeeIncludingOverdue;
                var NumMonths = Utils.NumberOfMonthsBetweenDates(StartDate, EndDate) + 1;

                    if (DiscountAmount > totalFee)
                    {
                        yearlyDiscount = totalFee;
                        yearlyRegistration = RegistrationDue - (DiscountAmount - totalFee);
                    }


                var pAmountPaid =  InitialDownpayment - yearlyRegistration ;
                var pAmountDisc =  yearlyDiscount;
                var pAmountReg =  yearlyRegistration ;
                    var pAmountUnpaid =  (RegistrationDue + totalFee) - (InitialDownpayment + DiscountAmount ) ;
                    

                   var monthlyUnpaidAmount = pAmountUnpaid / NumMonths;
                    var monthlyPaidAmount = pAmountPaid / NumMonths;
                    var monthlyFee = totalFee / NumMonths;
                var monthlyDiscount = pAmountDisc / NumMonths;



                for (int i = 0; i < NumMonths; i++ )
                    {
                        Payment p = new Payment();
                        p.FeeAmount = monthlyFee;
                        p.YearMonth = Utils.YearMonthCode(StartDate.AddMonths(i));
                        p.IsWrittenOff = false;
                        p.DiscountedAmount = monthlyDiscount;
                        p.PaidAmount = monthlyPaidAmount;
                        p.RemainingBalanceAmount = monthlyUnpaidAmount;

                        dues.Add(p);
                    }
                
                return dues;
            }

        }

        //public IList<Facility> ExistingFacilities { get; set; }

        public IList<Facility> Facilities { 
            get {
               var _facilities = new List<Facility>();

              // var dueAmount = (Membership.Fee - (InitialDownpayment - RegistrationFeeForm)) / NumInstallments;

               var dueAmount = UnpaidAmount / NumInstallmentsLeft;
                if (NumInstallmentsLeft > 0 && dueAmount > 0)
                {
                    for (var i = 1; i <= NumInstallmentsLeft; i++)
                    {
                     
                        Facility facility = new Facility();
                        facility.InstallmentNum = i;
                        facility.DueDate = Utils.GetLastDayOfMonth(TransactionDate.AddMonths(i));
                        facility.DueAmount = dueAmount;

                        if (NumInstallmentsLeft != NumInstallments)
                        {
                            if (i == 1)
                            {
                                facility.IsPaid = true;
                            }
                            else
                            {
                                facility.IsPaid = false;
                            }
                        
                        }
                        _facilities.Add(facility);
                    }
                }
                return _facilities;
            } 
        
        }

        /// <summary>
        /// used for installments payment to keep amount still due
        /// </summary>
        public double UnpaidAmount { get; set; }

        public bool IsYearly
        {
            get
            {
                return Member.Membership.MonthTerms == 12;
            }
        }

        public double CalculatedYearlyUnpaid { 
            get {
                return NumInstallments > 1 ? Fee - InitialDownpayment : 0;
            } 
        
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var t = obj as Transaction;
            if (t == null)
                return false;
            if (TransactionId == t.TransactionId)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash += (hash * 43) + TransactionId.GetHashCode();

            return hash;

        }

    }
}