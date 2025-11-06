

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SurveyBasket.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PollsController(IPollService pollService) : ControllerBase
{

    private readonly IPollService _pollService = pollService;

    [HttpGet("")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var polls = await _pollService.GetAllAsync(cancellationToken);
        var response = polls.Adapt<IEnumerable<PollResponse>>();

        return Ok(response);
    }
    [HttpGet("{id}")]  
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var poll = await _pollService.GetAsync(id, cancellationToken);
        if (poll is null) return NotFound();
        var response = poll.Adapt<PollResponse>();

        return Ok (response);
    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromBody] PollRequest request
        , CancellationToken cancellationToken = default)
    {
        
        var poll = await _pollService.AddAsync(request.Adapt<Poll>(), cancellationToken);

        return CreatedAtAction(nameof(Get), new { id = poll.Id }, poll);

    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest request
        , CancellationToken cancellationToken = default)
    {
        var isUpdated = await _pollService.UpdateAsync(id, request.Adapt<Poll>() , cancellationToken);
        if (!isUpdated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id
        , CancellationToken cancellationToken)
    {
        var isDeleted = await _pollService.DeleteAsync(id, cancellationToken);
        if (!isDeleted)
        {
            return NotFound();
        }
        return NoContent();
    }


    [HttpPut("{id}/TogglePublish")]
    public async Task<IActionResult> TogglePublish([FromRoute] int id,
        CancellationToken cancellationToken = default)
    {
        var isToggled = await _pollService.TogglePublishAsync(id, cancellationToken);

        if (!isToggled)
            return NotFound();
        return NoContent();

    }

}


