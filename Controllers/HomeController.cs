using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace RoomBookingApi.Controllers
{

    [ApiController]
    [Route("/home")]

    public class HomeController : ControllerBase
    {

        // public string GetHome() {
        //     return "Hello World !";
        // }

        public IActionResult GetHome()
        {
            return Ok("Hello World !");
        }
    }
}
