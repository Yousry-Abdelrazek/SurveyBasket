using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SurveyBasket.Contracts.Polls;

namespace SurveyBasket.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]

public class PollsController(IPollService pollService) : ControllerBase
{
    // Using primary constructor syntax to inject the service
    private readonly IPollService _pollService = pollService;

    [HttpGet("")]
    public async Task<IActionResult> GetAll()
    {
        var polls = await _pollService.GetAllAsync();
        return Ok(polls.Adapt<IEnumerable<PollResponse>>());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] int id)
    {
        var result = await _pollService.GetAsync(id);

        return result.IsSuccess
            ? Ok(result.Value)
            : Problem(statusCode: 404, title: result.Error.Code, detail: result.Error.Description);

    }
    [HttpPost("")]
    public async Task<IActionResult> Add([FromBody] PollRequest request, CancellationToken cancellationToken)
    {

        var newPoll = await _pollService.AddAsync(request, cancellationToken); // Poll = request 

        return CreatedAtAction(nameof(Get), new { id = newPoll.Id }, newPoll.Adapt<PollResponse>());
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest request, CancellationToken cancellationToken)
    {
        var result = await _pollService.UpdateAsync(id, request, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : Problem(statusCode: 404, title: result.Error.Code, detail: result.Error.Description);


    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.DeleteAsync(id, cancellationToken);
        return result.IsSuccess
            ? NoContent()
            : Problem(statusCode: 404, title: result.Error.Code, detail: result.Error.Description);
    }

    [HttpPut("{id}/toggle-publish")]
    public async Task<IActionResult> TogglePublishStatus([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.TogglePublishStatusAsync(id, cancellationToken);

        return result.IsSuccess
                    ? NoContent()
                    : Problem(statusCode: 404, title: result.Error.Code, detail: result.Error.Description);
    }


}
