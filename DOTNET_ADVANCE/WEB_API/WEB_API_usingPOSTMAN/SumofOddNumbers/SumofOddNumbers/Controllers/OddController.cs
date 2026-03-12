using Microsoft.AspNetCore.Mvc;

namespace SumofOddNumbers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OddController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            int s = 0;
            for(int i=0;i<100;i++)
            {
                if(i%2!=0)
                    {
                        s+=i;
                    }

            }
            return Ok(s);
        }
    }
}
