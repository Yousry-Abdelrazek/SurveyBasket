

using Azure.Core;
using Microsoft.EntityFrameworkCore;
using OneOf;
using SurveyBasket.Entities;
using System.Threading;

namespace SurveyBasket.Services;

public class PollService(ApplicationDbContext context) : IPollService
{
    private readonly ApplicationDbContext _context = context;
    public async  Task<IEnumerable<PollResponse>> GetAllAsync(CancellationToken cancellationToken) =>
        await _context.Polls
        .AsNoTracking()
        .ProjectToType<PollResponse>()
        .ToListAsync(cancellationToken);

    public async Task<IEnumerable<PollResponse>> GetCurrentAsync(CancellationToken cancellationToken = default) =>
        await _context.Polls
        .AsNoTracking()
        .Where(p => p.IsPublished && p.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && p.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow))
        .ProjectToType<PollResponse>()
        .ToListAsync(cancellationToken);



    public async Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await _context.Polls.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        return poll is not null
            ? Result.Success<PollResponse>(poll.Adapt<PollResponse>())
            : Result.Failure<PollResponse>(PollErrors.PollNotFound);

    }




    public async Task<Result<PollResponse>> AddAsync(PollRequest request, CancellationToken cancellationToken = default)
    {
        var isExisting = await _context.Polls.AnyAsync(p => p.Title == request.Title, cancellationToken);
        if (isExisting) 
            return Result.Failure<PollResponse>(PollErrors.PollAlreadyExists);

        var poll = request.Adapt<Poll>();
        await _context.Polls.AddAsync(poll, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success<PollResponse>(poll.Adapt<PollResponse>());

    }

    public async Task<Result> UpdateAsync(int id, PollRequest request, CancellationToken cancellationToken = default)
    {
        var isExisting = await _context.Polls.AnyAsync(p => p.Title == request.Title && p.Id != id, cancellationToken);

        if(isExisting)
            return Result.Failure(PollErrors.PollAlreadyExists);


        var existingPoll = await _context.Polls.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (existingPoll is null)
        {
            return Result.Failure(PollErrors.PollNotFound) ;
        }
        existingPoll.Title = request.Title;
        existingPoll.Summary = request.Summary;
        existingPoll.StartsAt = request.StartsAt;
        existingPoll.EndsAt = request.EndsAt;


        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var poll = await _context.Polls.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
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
        var existingPoll = await _context.Polls.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (existingPoll is null)
        {
            return Result.Failure(PollErrors.PollNotFound);
        }
        existingPoll.IsPublished = !existingPoll.IsPublished; ;


        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

