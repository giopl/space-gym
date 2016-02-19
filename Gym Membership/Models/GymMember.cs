using Gym_Membership.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gym_Membership.Models
{
    public class GymMember
    {

        public List<string> ValidationErrors { get; set; }

        [DisplayName("Id")]
        public int MemberId { get; set; }

        [Required]
        public string Title { get; set; }

        public int MemberOrderNum { get; set; }

        public bool HasRelatedMembers
        {
            get
            {
                return RelatedMembers.Members.Count > 0;
            }
        }

        public RelatedMembers RelatedMembers { get; set; }

        [StringLength(20, MinimumLength = 3)]
        [Required]
        public string Firstname { get; set; }

        [StringLength(20, MinimumLength = 3)]
        [Required]
        [DisplayName("Family Name")]
        public string Lastname { get; set; }


        /// <summary>
        /// used to capture name in single textbox John,Smith 
        /// </summary>
        public string FirstnameLastnameForm { get; set; }

        public bool UseSameAddressForm { get; set; }
     //   public IList<Stat> Statistics { get; set; }

        public string FullnameFromFirstAndLastName
        {
            get
            {
                return String.Concat(Firstname, ' ', Lastname);
            }
        }

        public string Fullname { get; set; }

        public IList<Receipt> Receipts { get; set; }

        public double AmountDueToDate { get; set; }

        public bool HasReceipts
        {
            get
            {
                return Receipts.Count > 0;
            }
        }
        public string TitleFullname
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Firstname) || string.IsNullOrWhiteSpace(Lastname))
                {
                    return String.Concat(Title, ". ", Fullname);
                }
                    return String.Concat(Title, ". ", Firstname, ' ', Lastname);
            }
        }

        public bool IsCustomerMembership { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        [DisplayName("D.O.B")]
        public DateTime DateOfBirth { get; set; }

        public string FormattedDOB
        {
            get
            {
                return DateOfBirth.ToString("dd-MMM-yyyy");
            }
        }

        public int BirthdayInDays
        {
            get
            {
                var currentYearBirthday = new DateTime(DateTime.Now.Year, DateOfBirth.Month, DateOfBirth.Day);
                TimeSpan ts = DateTime.Now - currentYearBirthday;
                int tsdays = Convert.ToInt32(ts.TotalDays);
                return tsdays;

            }
        }


        public bool IsBirthday
        {
            get
            {
                return BirthdayInDays == 1;
            }
        }
        public string ComingBirthday
        {
            get
            {

                switch (BirthdayInDays)
                {
                    case 3: return "-2";
                    case 2: return "-1";
                    //case 1: return "!";
                    case 0: return "+1";
                    case -1: return "+2";
                    default:
                        return string.Empty;

                }

            }

        }

        public bool IsPartPayment { get; set; }


        /// <summary>
        /// date at which next installment period ends
        /// +1 day for next installment date
        /// </summary>
        public DateTime InstallmentDate { get; set; }

        public int LastTransactionId { get; set; }

        public string ComingBirthdayDescription
        {
            get
            {

                switch (BirthdayInDays)
                {
                    case 3: return "Birthday was 2 days ago";
                    case 2: return "Birthday was yesterday";
                    case 1: return "Birthday is today";
                    case 0: return "Birthday is tomorrow";
                    case -1: return "Birthday is in 2 days";
                    default:
                        return string.Empty;

                }

            }

        }


        public bool HasTransaction
        {
            get
            {
                return LastTransactionId > 0;
            }

        }

        public bool VisitedWhileOverdue
        {
            get
            {
                return DaysSinceLastVisit < DaysOverDue;
            }

        }

        public bool HasComingBirthday
        {
            get
            {
                return BirthdayInDays > -2 && BirthdayInDays < 4;
            }
        }

        [Required]
        [DisplayName("Street")]
        public string StreetAddress { get; set; }

        [Required]
        public string Town { get; set; }


        [DisplayName("Email")]
        public string EmailAddress { get; set; }

        [DisplayName("Home Tel")]
        public string HomePhone { get; set; }

        [DisplayName("Office Tel")]
        public string OfficePhone { get; set; }

        [DisplayName("Mobile")]
        public string MobilePhone { get; set; }


        [Required]
        public string Club { get; set; }

        [DisplayName("Joined On")]
        public DateTime RegistrationDate { get; set; }

        [DisplayName("Num. Visits")]
        public int NumberVisits { get; set; }

        [DisplayName("Last Visit")]
        public DateTime LastVisit { get; set; }

        public String LastVisitFmt {
            
            get
            {
                if (LastVisit < new DateTime(1980,1,1))
                {
                    return "no visit";
                }
                return LastVisit.ToString("dd MMM yyyy");
            }
            }
        public int LastVisitId { get; set; }

        [DisplayName("Is Active")]
        public bool IsActive { get; set; }

        [Required]
        [DisplayName("Membership")]
        //public string MembershipType { get; set; }
        public Membership Membership { get; set; }

        public bool IsTemporary { get { return Membership.MembershipCode == "TEMP"; } }





        [DisplayName("How you Heard About Us")]
        public string HowYouHeardAboutUs { get; set; }

        [DisplayName("Employer")]
        public string Company { get; set; }

        [DisplayName("Paid Until")]
        public DateTime PaymentUntilDate { get; set; }

        [DisplayName("Reason for leaving")]
        public string ReasonForLeaving { get; set; }

        public IList<MemberComment> Comments { get; set; }

        public bool HasComments
        {
            get
            {
                return Comments.Count > 0;
            }
        }
        public IList<MemberVisit> Visits { get; set; }

        [DisplayName("Payment Option")]
        public PaymentOptionEnum PaymentOption { get; set; }

        public bool HasArrears
        {
            get
            {
                return DateTime.Now > PaymentUntilDate;
            }
        }

        public bool IsPresent
        {
            get
            {
                TimeSpan ts = DateTime.Now - LastVisit;
                return ts.TotalMinutes < 91;
            }
        }

        public int AverageVisitsPerMonth
        {
            get
            {

                return (int)(NumberVisits / MemberSinceInMonths);
            }
        }


        public int DaysSinceLastVisit
        {
            get
            {

                var beginDate = LastVisit < new DateTime(2000, 1, 1) ? RegistrationDate : LastVisit;
                TimeSpan ts = DateTime.Now - beginDate;

                int days = Convert.ToInt32(ts.TotalDays);
                return days;


            }
        }

        public string DaysSinceLastVisitDesc
        {
            get
            {

                if (DaysSinceLastVisit == 0)
                {
                    return "Today";
                }
                else if (DaysSinceLastVisit == 1)
                {
                    return "Yesterday";
                }
                else if (DaysSinceLastVisit > 1 && DaysSinceLastVisit < 5000)
                {
                    return string.Format("{0} days ago", DaysSinceLastVisit);
                }
                else
                {
                    return "Not Available";
                }

            }

        }

        [AllowHtml]
        public string DayStatsChart { get; set; }
        [AllowHtml]
        public string HourStatsChart { get; set; }
        [AllowHtml]
        public string MonthStatsChart { get; set; }

        [AllowHtml]
        public string OnloadFunction { get; set; }


        public int StandingOrderMonths { get; set; }
        public enum PaymentOptionEnum
        {

            ADHOC = 1,
            TICKET,
            MONTHLY,
            STANDING_ORDER,
        }


        public AdminUser CreatedBy { get; set; }
        public AdminUser UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        [DisplayName("Monthly Fee")]
        public double CustomMonthlyFee { get; set; }

        [DisplayName("Registration Fee")]
        public double CustomRegistrationFee { get; set; }

        public string Occupation { get; set; }

        /// <summary>
        /// should be displayed as raw, i.e Html.Raw(FormattedFullname)
        /// </summary>


        public int Age
        {
            get
            {
                return DateTime.Now.Year - DateOfBirth.Year;
            }
        }

        [DisplayName("Age")]
        public int AgeInputted { get; set; }

        //public bool IsPass { get; set; }
        public bool HasVisitsLeft {
            get
            {
                if(IsPass)
                {
                return VisitsLeft > 0;

                } else
                {
                    return true;
                }

            }
                
                }

        [DisplayName("Overdue")]
        public int DaysOverDue
        {
            get
            {
                TimeSpan days = DateTime.Now - PaymentUntilDate;
                return Convert.ToInt32(days.TotalDays);

            }
        }



        public bool IsLongOverdue
        {
            get
            {
                var lo = ConfigurationHelper.GetLongOverdueDays();
                return DaysOverDue >= ConfigurationHelper.GetLongOverdueDays();
            }

        }

        public bool IsRegistrationPaid { get; set; }

        [DisplayName("Member Since (Mths)")]
        public double MemberSinceInMonths
        {
            get
            {
                TimeSpan span = DateTime.Now - RegistrationDate;
                return span.TotalDays / 30D;
            }

        }

        public string ProfilePicExt { get; set; }

        public bool HasProfilePicture { get { return !String.IsNullOrWhiteSpace(ProfilePicExt); } }

        [DisplayName("Visits / Mth")]

        public double VisitPerMonth
        {
            get
            {
                return NumberVisits / MemberSinceInMonths;
            }
        }

        
        
        
        /// <summary>
        /// used only when creating pass type
        /// </summary>
        public String PaymentMethod { get; set; }


        [DisplayName("Pass Type")]
        public int MaxVisits { get; set; }
        public int VisitsLeft { get; set; }

        public bool IsPass { get
            {
                return MaxVisits > 0;
            }
        }


        public bool IsDayPass
        {
            get
            {
                return MaxVisits == 1;
            }
        }
        public GymMember()
        {
            Comments = new List<MemberComment>();
            CreatedBy = new AdminUser();
            UpdatedBy = new AdminUser();          
            Membership = new Membership();
            RelatedMembers = new RelatedMembers();
            ValidationErrors = new List<String>();
            Receipts = new List<Receipt>();
            
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var t = obj as GymMember;
            if (t == null)
                return false;
            if (MemberId == t.MemberId)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash += (hash * 43) + MemberId.GetHashCode();

            return hash;

        }
    }
}