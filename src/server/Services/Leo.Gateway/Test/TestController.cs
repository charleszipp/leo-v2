using System.Web.Http;

namespace Leo.Gateway.Test
{
    public class TestController : ApiController
    {
        [HttpGet]
        [Route("helloworld")]
        public IHttpActionResult Test()
        {
            return Ok("hello world");
        }
    }
}