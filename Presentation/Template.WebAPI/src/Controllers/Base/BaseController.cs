using Microsoft.AspNetCore.Mvc;
using Template.Common.Models.Response;

namespace Template.WebAPI.Controllers.Base;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]

public class BaseController : ControllerBase
{
    protected static IActionResult ApiResult<T>(Response<T> response)
        => new ObjectResult(response) { StatusCode = response.StatusCode };
}