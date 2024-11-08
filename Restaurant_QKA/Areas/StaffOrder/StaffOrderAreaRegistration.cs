using System.Web.Mvc;

namespace Restaurant_QKA.Areas.StaffOrder
{
    public class StaffOrderAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "StaffOrder";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "StaffOrder_default",
                "StaffOrder/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}