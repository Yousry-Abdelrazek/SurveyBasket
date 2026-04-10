
namespace SurveyBasket.Services;

public class ResultService(ApplicationDbContext context) : IResultService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result<PollVotesResponse>> GetPollVotesAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var pollVotes = await _context.Polls
            .Where(p => p.Id == pollId)
            .Select(p => new PollVotesResponse
            (
                p.Title,
                p.Votes.Select(v => new VoteResponse
                (
                    $"{v.User.FirstName} {v.User.LastName}",
                    v.SubmittedOn,
                    v.VoteAnswers.Select(answer => new QuestionAnswerResposne
                    (
                        answer.Question.Content,
                        answer.Answer.Content
                    )
                )
            )
            ))
                ).FirstOrDefaultAsync(cancellationToken);

        return pollVotes is not null
            ? Result.Success(pollVotes)
            : Result.Failure<PollVotesResponse>(PollErrors.PollNotFound);

    }
    public async Task<Result<IEnumerable<VotesPerDayResponse>>> GetVotesPerDayAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var pollIsExists = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);

        if (!pollIsExists)
            return Result.Failure<IEnumerable<VotesPerDayResponse>>(PollErrors.PollNotFound);

        var votesPerDay = await _context.Votes
            .Where(v => v.PollId == pollId)
            .GroupBy(v => new { Date = DateOnly.FromDateTime(v.SubmittedOn) })
            .Select(g => new VotesPerDayResponse
            (
                g.Key.Date,
                g.Count()
            )).ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<VotesPerDayResponse>>(votesPerDay);

    }
    public async Task<Result<IEnumerable<VotesPerQuestionResponse>>> GetVotesPerQuestionAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var pollIsExists = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);

        if (!pollIsExists)
            return Result.Failure<IEnumerable<VotesPerQuestionResponse>>(PollErrors.PollNotFound);


        var votesPerQuestion = await _context.VoteAnswers
            .Where(va => va.Vote.PollId == pollId)
            .Select(va => new VotesPerQuestionResponse(
                va.Question.Content,
                va.Question.VoteAnswers
                .GroupBy(x => new { AnswerId = x.AnswerId, AnswerContent = x.Answer.Content })
                .Select(g => new VotesPerAnswerResponse
                (
                    g.Key.AnswerContent,
                    g.Count()
                     ))
                ))
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<VotesPerQuestionResponse>>(votesPerQuestion);
    }
}