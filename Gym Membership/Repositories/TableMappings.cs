using Gym_Membership.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gym_Membership.Repositories
{
    public class TableMappings
    {
        protected static string DOT = ".";
        protected static string SCHEMA_DBO = "dbo";

        //Functions
        public static string FUNC_GET_AGE_GROUP = SCHEMA_DBO + DOT + "GetAgeGroup";

        //Views
        public static string VW_PAYMENT = SCHEMA_DBO + DOT + "v_payment";
        public static string VW_BUDGETED = SCHEMA_DBO + DOT + "v_budgeted";
        



        //SQL Server
        public static string TBL_ADMIN = SCHEMA_DBO + DOT + "tbl_admin";
        
        
#if test
        public static string TBL_MEMBER = SCHEMA_DBO + DOT + "tbl_member_data";
        public static string TBL_RELATIONSHIP = SCHEMA_DBO + DOT + "tbl_relationship_test";


#else

        public static string TBL_MEMBER = SCHEMA_DBO + DOT + "tbl_member";
        public static string TBL_RELATIONSHIP = SCHEMA_DBO + DOT + "tbl_relationship";
        

#endif 


        public static string TBL_COMMENT = SCHEMA_DBO + DOT + "tbl_comment";
        public static string TBL_VISIT = SCHEMA_DBO + DOT + "tbl_visit";
        //public static string TBL_FEE_HIST = SCHEMA_DBO + DOT + "tbl_fee_history";
        public static string TBL_LOG = SCHEMA_DBO + DOT + "tbl_log";
        public static string TBL_MEMBERSHIP = SCHEMA_DBO + DOT + "tbl_membership";

        

        public static string TBL_TRANSACTION = SCHEMA_DBO + DOT + "tbl_transaction";
        public static string TBL_PAYMENT = SCHEMA_DBO + DOT + "tbl_payment";
        public static string TBL_FACILITY = SCHEMA_DBO + DOT + "tbl_facility";
        public static string TBL_RECEIPT = SCHEMA_DBO + DOT + "tbl_receipt";
    //    public static string TBL_CONFIGURATION = SCHEMA_DBO + DOT + "tbl_configuration";
        //public static string TBL_PASS   = SCHEMA_DBO + DOT + "tbl_pass";



        public static string SQL_LIMIT_ROWS(int numRows)
        {
            return string.Empty;
        }

    }
}