
using SurveyBasket.Mapping;

namespace SurveyBasket.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PollsController(IPollService pollService) : ControllerBase
{

    private readonly IPollService _pollService = pollService;

    [HttpGet("")]
    public IActionResult GetAll()
    {
        var polls = _pollService.GetAll();
        return Ok(polls.MapToResponse());
    }
    [HttpGet("{id}")]
    public IActionResult Get([FromRoute] int id)
    {
        var poll = _pollService.Get(id);
        if (poll is null) return NotFound();

        return Ok (poll.MapToResponse());
    }

    [HttpPost("")]
    public IActionResult Add([FromBody] CreatePollRequest request)
    {
        var poll = _pollService.Add(request.MapToPoll());
        //return CreatedAtAction(nameof(Get), new { id = poll.Id }, poll);
        return CreatedAtAction(nameof(Get), new { id = poll.Id }, poll);
    }


    [HttpPut("{id}")]  
    public IActionResult Update([FromRoute] int id , [FromBody] CreatePollRequest request)
    {
        var isUpdated = _pollService.Update(id, request.MapToPoll());
        if (!isUpdated)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var isDeleted = _pollService.Delete(id);
        if (!isDeleted)
        {
            return NotFound();
        }
        return NoContent();
    }



}


