using System.Web;
using System.Web.Optimization;

namespace CuttingMrpWeb
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/jquery.datetimepicker.full.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/bootstrap-typeahead.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/basic").Include(
                   "~/Scripts/basics.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-plug-in").Include(
                      "~/Scripts/jquery-popModal.js"));

            bundles.Add(new ScriptBundle("~/bundles/highcharts").Include(
                      "~/Scripts/highcharts/highcharts.js"));

            bundles.Add(new ScriptBundle("~/bundles/requirements").Include(
                      "~/Scripts/requirement.js"));

            bundles.Add(new ScriptBundle("~/bundles/stocks").Include(
                      "~/Scripts/stock.js"));

            bundles.Add(new ScriptBundle("~/bundles/mrp-rounds").Include(
                     "~/Scripts/mrp-round.js"));

            bundles.Add(new ScriptBundle("~/bundles/process-orders").Include(
                      "~/Scripts/process-orders.js"));

            bundles.Add(new ScriptBundle("~/bundles/dashboard").Include(
                      "~/Scripts/dashboard/dashboard.js"));

            bundles.Add(new ScriptBundle("~/bundles/part_stock").Include(
                      "~/Scripts/dashboard/part-stock.js"));

            bundles.Add(new ScriptBundle("~/bundles/complete_rate").Include(
                      "~/Scripts/dashboard/complete-rate.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/jquery.datetimepicker.css",
                      "~/Content/site.css",
                      "~/Content/pagination.css",
                      "~/Content/filter.css",
                      "~/Content/jquery-popModal.css"));

            bundles.Add(new StyleBundle("~/Content/requirement").Include(
                     "~/Content/requirement.css"));

            bundles.Add(new StyleBundle("~/Content/stock").Include(
                     "~/Content/stock.css"));

            bundles.Add(new StyleBundle("~/Content/process-order").Include(
                     "~/Content/process-orders.css"));

            bundles.Add(new StyleBundle("~/Content/mrp-rounds").Include(
                     "~/Content/mrp-round.css"));

            bundles.Add(new StyleBundle("~/Content/dashboards").Include(
                     "~/Content/dashboards.css"));

            bundles.Add(new StyleBundle("~/Content/complete_rate").Include(
                     "~/Content/complete-rate.css"));

            bundles.Add(new StyleBundle("~/Content/part_stock").Include(
                     "~/Content/part-stock.css"));
        }
    }
}
