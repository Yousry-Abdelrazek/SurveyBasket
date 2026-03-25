

using Microsoft.EntityFrameworkCore;
using OneOf;
using SurveyBasket.Entities;
using System.Threading;

namespace SurveyBasket.Services;

public class PollService(ApplicationDbContext context) : IPollService
{
    private readonly ApplicationDbContext _context = context;
    public async  Task<IEnumerable<Poll>> GetAllAsync() =>
        await _context.Polls.AsNoTracking().ToListAsync();

    public async Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await _context.Polls.FirstOrDefaultAsync(p => p.Id == id);

        return poll is not null
            ? Result.Success<PollResponse>(poll.Adapt<PollResponse>())
            : Result.Failure<PollResponse>(PollErrors.PollNotFound);

    }




    public async Task<PollResponse> AddAsync(PollRequest request, CancellationToken cancellationToken = default)
    {
        var poll = request.Adapt<Poll>();
        await _context.Polls.AddAsync(poll);
        await _context.SaveChangesAsync();

        return poll.Adapt<PollResponse>(); 
    }

    public async Task<Result> UpdateAsync(int id, PollRequest poll, CancellationToken cancellationToken = default)
    {
        var existingPoll = await _context.Polls.FirstOrDefaultAsync(p => p.Id == id);
        if (existingPoll is null)
        {
            return Result.Failure(PollErrors.PollNotFound) ;
        }
        existingPoll.Title = poll.Title;
        existingPoll.Summary = poll.Summary;
        existingPoll.StartsAt = poll.StartsAt;
        existingPoll.EndsAt = poll.EndsAt;


        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var poll = await _context.Polls.FirstOrDefaultAsync(p => p.Id == id);
        if (poll is null)
        {
            return Result.Failure(PollErrors.PollNotFound);
        }
        _context.Polls.Remove(poll);
        await _context.SaveChangesAsync(cancellationToken);



        return Result.Success();

    }

    public async Task<Result> TogglePublishStatusAsync(int id, CancellationToken cancellationToken = default)
    {
        var existingPoll = await _context.Polls.FirstOrDefaultAsync(p => p.Id == id);
        if (existingPoll is null)
        {
            return Result.Failure(PollErrors.PollNotFound);
        }
        existingPoll.IsPublished = !existingPoll.IsPublished; ;


        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
