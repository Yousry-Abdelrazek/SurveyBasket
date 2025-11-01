

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



        var response = polls.Adapt<IEnumerable<PollResponse>>();

        return Ok(response);
    }
    [HttpGet("{id}")]
    public IActionResult Get([FromRoute] int id)
    {
        var poll = _pollService.Get(id);
        if (poll is null) return NotFound();



        var response = poll.Adapt<PollResponse>();


        return Ok (response);
    }

    [HttpPost("")]
    public IActionResult Add([FromBody] CreatePollRequest request)
    {
        //var poll = _pollService.Add(request.MapToPoll());
        var poll = _pollService.Add(request.Adapt<Poll>());

        return CreatedAtAction(nameof(Get), new { id = poll.Id }, poll);

    }


    [HttpPut("{id}")]  
    public IActionResult Update([FromRoute] int id , [FromBody] CreatePollRequest request)
    {
        var isUpdated = _pollService.Update(id, request.Adapt<Poll>());
        if (!isUpdated)
            return NotFound();

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


    [HttpGet("test")]
    public IActionResult Test()
    {
        var student = new Student
        {
            Id = 1,
            FirstName = "Yousry",
            MiddleName = "Abdelrazek",
            LastName = "Nagdy",
            DateOfBirth = new DateTime(1998, 5, 15), 
            Department = new Department
            {
                Id = 1,
                Name = "Computer Science"
            }
        };

        var studentResponse = student.Adapt<StudentResponse>();
        return Ok(studentResponse);
    }

}


