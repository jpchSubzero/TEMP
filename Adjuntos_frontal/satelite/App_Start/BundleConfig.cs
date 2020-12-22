using System.Web;
using System.Web.Optimization;

namespace satelite
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Recursos genericos usados en el proyecto
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-3.4.1.js",
                        "~/Scripts/jquery-ui.min.js",
                        "~/Scripts/utils/scale_view.js",
                        "~/Scripts/jquery-{version}.js"
                        ));

            //Recursos utilizados en la vista de becas
            bundles.Add(new ScriptBundle("~/bundles/scholarship").Include(
                        "~/Scripts/utils/utils.js",
                        "~/Scripts/scholarship/scholarship.js",
                        "~/Scripts/utils/scale_view.js"
                        ));

            //Recursos utilizados en la vista de servicios
            bundles.Add(new ScriptBundle("~/bundles/services").Include(
                        "~/Scripts/utils/utils.js",
                        "~/Scripts/services/services.js",
                        "~/Scripts/utils/scale_view.js"
                        ));

            //Recursos utilizados para la validacion de formularios
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información. De este modo, estará
            // para la producción, use la herramienta de compilación disponible en https://modernizr.com para seleccionar solo las pruebas que necesite.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/icons/all.min.css"));
        }
    }
}
