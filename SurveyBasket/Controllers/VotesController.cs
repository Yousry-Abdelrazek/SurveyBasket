using Microsoft.AspNetCore.Authorization;
using SurveyBasket.Contracts.Votes;
using SurveyBasket.Extensions;

namespace SurveyBasket.Controllers;

[Route("api/polls/{pollId}/vote")]
[ApiController]
[Authorize]
public class VotesController(IQuestionService questionService , IVoteService voteService) : ControllerBase
{
    private readonly IQuestionService _questionService = questionService;
    private readonly IVoteService _voteService = voteService;

    [HttpGet("")]
    public async Task<IActionResult> Start([FromRoute] int pollId , CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var result = await _questionService.GetAvailableAsync(pollId, userId!, cancellationToken);

        if(result.IsSuccess)
            return Ok(result.Value);

        return result.Error.Equals(VoteErrors.VoteAleardyExists)
            ? result.ToProblem(StatusCodes.Status409Conflict)
            : result.ToProblem(StatusCodes.Status404NotFound);
    }


    [HttpPost("")]
    public async Task<IActionResult> Vote([FromRoute] int pollId , [FromBody] VoteRequest request, CancellationToken cancellationToken)
    {
        var result = await _voteService.AddAsync(pollId,User.GetUserId()!, request, cancellationToken);

        if (result.IsSuccess)
            return Created();


        return result.Error.Equals(VoteErrors.VoteAleardyExists)
            ? result.ToProblem(StatusCodes.Status409Conflict)
            : result.ToProblem(StatusCodes.Status404NotFound);


    }
}
