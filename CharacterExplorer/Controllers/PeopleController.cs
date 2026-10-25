using CharacterExplorer.Contracts.People;
using CharacterExplorer.Services.People;
using Microsoft.AspNetCore.Mvc;

namespace CharacterExplorer.Controllers;

[ApiController, Route("api/people")]
public sealed class PeopleController(IPersonService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string? search)
    {
        var result = await service.SearchAsync(search);
        return result.Success
            ? Ok(result.People.Select(PersonResponseMapper.ToResponse))
            : BadRequest(new { result.FailureCode, result.Message });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await service.GetByIdAsync(id);
        return result.Success
            ? Ok(PersonResponseMapper.ToResponse(result.Person!))
            : NotFound(new { result.FailureCode, result.Message });
    }
}
