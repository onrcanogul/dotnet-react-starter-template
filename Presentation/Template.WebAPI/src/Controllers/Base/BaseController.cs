using Microsoft.AspNetCore.Mvc;
using Template.Common.Models.Response;

namespace Template.WebAPI.Controllers.Base;

[Route("api/[controller]")]
[ApiController]
public class BaseController : ControllerBase
{
    protected static IActionResult ApiResult<T>(Response<T> response)
        => new ObjectResult(response) { StatusCode = response.StatusCode };
}