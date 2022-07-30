using Microsoft.AspNetCore.Mvc;

namespace Core.Application.Contracts
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {

    }
}