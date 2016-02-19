using System.Web;
using System.Web.Optimization;

namespace Gym_Membership
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));


            //http://formvalidation.io/
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/plugins/bootstrap3-typeahead.js",
                      "~/Scripts/plugins/jquery.dataTables.js",
                      "~/Scripts/plugins/dataTables.bootstrap.js",
                      "~/Scripts/plugins/bootstrap-datepicker.js",
                      "~/Scripts/plugins/FormValidation/formValidation.js",
                      "~/Scripts/plugins/FormValidation/bootstrap.js",

                      //http://keith-wood.name/dateentry.html
                      "~/Scripts/plugins/jquery.plugin.js",
                      "~/Scripts/plugins/jquery.dateentry.js",
                     //https://github.com/customd/jquery-number
                      "~/Scripts/plugins/jquery.number.js",
                //http://firstopinion.github.io/formatter.js/index.html
                        "~/Scripts/plugins/formatter/jquery.formatter.js",
                      "~/Scripts/respond.js"));



            bundles.Add(new ScriptBundle("~/bundles/highcharts").Include(
                      "~/Scripts/plugins/highcharts/highcharts.js",
                      "~/Scripts/plugins/highcharts/highcharts-3d.js",
                      "~/Scripts/plugins/highcharts/highcharts-more.js"

                      ));



            bundles.Add(new ScriptBundle("~/bundles/custom").Include(
                      "~/Scripts/custom/shared.js"
                      ));


            bundles.Add(new ScriptBundle("~/bundles/transaction-year").Include(
                      "~/Scripts/custom/transaction-yearly.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/transaction-month").Include(
                      "~/Scripts/custom/transaction-monthly.js"
                      ));


            bundles.Add(new ScriptBundle("~/bundles/admin").Include(
          "~/Scripts/custom/admin.js"
          ));


            bundles.Add(new ScriptBundle("~/bundles/chart").Include(
                      "~/Scripts/plugins/Chart.js"
                      ));


            //http://www.daterangepicker.com/#usage
            bundles.Add(new ScriptBundle("~/bundles/daterange").Include(
                      "~/Scripts/plugins/daterange/moment.js",
                      "~/Scripts/plugins/daterange/daterangepicker.js"

                      ));

            bundles.Add(new ScriptBundle("~/bundles/jspdf").Include(
                     "~/Scripts/plugins/jsPdf/FileSaver.js",
                     "~/Scripts/plugins/jsPdf/jspdf.js",
                     "~/Scripts/plugins/jsPdf/split_text_to_size.js",
                     "~/Scripts/plugins/jsPdf/standard_fonts_metrics.js",
                     "~/Scripts/plugins/jsPdf/from_html.js"


                     ));


            //http://amsul.ca/pickadate.js/date/
            bundles.Add(new ScriptBundle("~/bundles/pickadate").Include(
          "~/Scripts/plugins/pickadate/picker.js",
                  "~/Scripts/plugins/pickadate/picker.date.js",
          "~/Scripts/plugins/pickadate/picker.time.js",
          "~/Scripts/plugins/pickadate/legacy.js"



          ));



            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/font-awesome.css",
                      "~/Content/bootstrap.css",
                      //"~/Content/bootsnip.css",
                      "~/Content/bootstrap-datatables.css",
                      "~/Content/bootstrap-datepicker.css",
                      "~/Content/formValidation.css",
                      "~/Content/jquery.dateentry.css",
                      "~/Content/daterange/daterangepicker.css",
                      "~/Content/pickadate/default.css",
                      "~/Content/pickadate/default.date.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/biz-css").Include(
                      "~/Content/login-biz.css"));
            


            bundles.Add(new StyleBundle("~/Content/login").Include(
                "~/Content/bootstrap.css",
                "~/Content/font-awesome.css",
                      "~/Content/login.css"));


            bundles.Add(new StyleBundle("~/Content/changepassword").Include(
     "~/Content/bootstrap.css",
     "~/Content/font-awesome.css",
           "~/Content/changePassword.css"));


            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = false;
        }
    }
}
