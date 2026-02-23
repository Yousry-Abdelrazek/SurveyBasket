using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SurveyBasket.Contracts.Polls;

namespace SurveyBasket.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PollsController(IPollService pollService) : ControllerBase
{
    // Using primary constructor syntax to inject the service
    private readonly IPollService _pollService = pollService;

    [HttpGet("")]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        var polls = await _pollService.GetAllAsync();
        return Ok(polls.Adapt<IEnumerable<PollResponse>>());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] int id)
    {
        var poll = await _pollService.GetAsync(id);
        if (poll == null)
        {
            return NotFound();
        }

        PollResponse _response = poll.Adapt<PollResponse>(); // Using Mapster to map Poll to PollResponse
        return Ok(_response);
    }
    [HttpPost("")]
    public async Task<IActionResult>  Add([FromBody] PollRequest request, CancellationToken cancellationToken)
    {

        var newPoll = await _pollService.AddAsync(request.Adapt<Poll>(), cancellationToken); // Poll = request 

        return CreatedAtAction(nameof(Get), new { id = newPoll.Id }, newPoll);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest request, CancellationToken cancellationToken)
    {
        var isUpdated = await _pollService.UpdateAsync(id, request.Adapt<Poll>() , cancellationToken);
        if (!isUpdated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var isDeleted = await _pollService.DeleteAsync(id , cancellationToken);
        if (!isDeleted)
            return NotFound();
        return NoContent();
    }

    [HttpPut("{id}/toggle-publish")]
    public async Task<IActionResult> TogglePublishStatus([FromRoute] int id, CancellationToken cancellationToken)
    {
        var isToggled = await _pollService.TogglePublishStatusAsync(id, cancellationToken);
        if (!isToggled)
            return NotFound();
        return NoContent();
    }


}
