namespace SurveyBasket.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PollsController(IPollService pollService) : ControllerBase
{
    // Using primary constructor syntax to inject the service
    private readonly IPollService _pollService = pollService;

    [HttpGet("")]
    public IActionResult GetAll()
    {
        var polls = _pollService.GetAll();
        return Ok(polls);
    }

    [HttpGet("{id}")]
    public IActionResult Get([FromRoute] int id)
    {
        var poll = _pollService.Get(id);
        if (poll == null)
        {
            return NotFound();
        }

        //PollResponse _response = (PollResponse)poll;

        var config = new TypeAdapterConfig();
        config.NewConfig<Poll, PollResponse>()
            .Map(dest => dest.Notes, src => src.Description); // Example of custom mapping

        PollResponse _response = poll.Adapt<PollResponse>(config); // Using Mapster to map Poll to PollResponse
        return Ok(_response);
    }
    //[HttpPost("")]
    //public IActionResult Add([FromBody] CreatePollRequest request)
    //{
    //    var newPoll = _pollService.Add((Poll)request); // Poll = request 

    //    return CreatedAtAction(nameof(Get), new { id = newPoll.Id }, newPoll);
    //}

    //[HttpPut("{id}")]
    //public IActionResult Update([FromRoute] int id ,[FromBody] CreatePollRequest request)
    //{
    //    var isUpdated = _pollService.Update(id,request);   
    //    if (!isUpdated)
    //        return NotFound();

    //    return NoContent();
    //}

    //[HttpDelete("{id}")]
    //public IActionResult Delete([FromRoute] int id)
    //{
    //    var isDeleted = _pollService.Delete(id);
    //    if (!isDeleted)
    //        return NotFound();
    //    return NoContent();
    //}
}
