using Microsoft.AspNetCore.Mvc;

namespace APIVersionDemo.Controllers
{
    [ApiController]

    [ApiVersion("1.0")]

    [Route("api/v{version:apiVersion}/students")]

    public class StudentController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetStudents()
        {
            return Ok(new
            {
                Version = "V1",
                Students = new string[] { "Ram", "John", "Sara" }
            });
        }
    }
}
