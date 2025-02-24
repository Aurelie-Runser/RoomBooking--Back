using Microsoft.AspNetCore.Mvc;
using RoomBookingApi.Validations;

namespace RoomBookingApi.Controllers
{

    [ApiController]
    [Route("/equipment")]

    public class EquipmentController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<string>> GetAvailableEquipments()
        {
            return Ok(AvailableEquipments.AllowedEquipments);
        }
    }
}
