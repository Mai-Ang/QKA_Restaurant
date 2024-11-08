using System.Web.Mvc;

namespace Restaurant_QKA.Areas.StaffChef
{
    public class StaffChefAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "StaffChef";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "StaffChef_default",
                "StaffChef/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}