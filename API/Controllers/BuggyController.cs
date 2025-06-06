
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class BuggyController : BaseApiController
{
    [HttpGet("not-found")]
    public IActionResult GetNotFound()
    => NotFound();
   
   [HttpGet("bad-request")]
    public IActionResult GetBadRequest()
    => BadRequest("This is a bad request");

    [HttpGet("unauthorized")]
    public IActionResult GetUnauthorized()
    => Unauthorized();

    [HttpGet("validation-error")]
    public IActionResult GetValidationError()
    {
        ModelState.AddModelError("Problem 01", "This is a validation error");
        ModelState.AddModelError("Problem 02", "This is a validation error");

        return ValidationProblem(ModelState);
    }

    [HttpGet("server-error")]
    public IActionResult GetServerError()
    {
        throw new Exception("This is a server error");
    } 
}
