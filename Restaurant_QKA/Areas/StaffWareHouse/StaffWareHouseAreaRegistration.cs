using System.Web.Mvc;

namespace Restaurant_QKA.Areas.StaffWareHouse
{
    public class StaffWareHouseAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "StaffWareHouse";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "StaffWareHouse_default",
                "StaffWareHouse/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}