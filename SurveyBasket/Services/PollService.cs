
using SurveyBasket.Persistence;

namespace SurveyBasket.Services;

public class PollService(ApplicationDbContext context) : IPollService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<Poll>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Polls.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<Poll> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await _context.Polls.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id , cancellationToken);
        //if (poll == null)
        //    throw new KeyNotFoundException($"Poll with Id {id} not found.");

        return poll;
    }

    public async Task<Poll> AddAsync(Poll poll, CancellationToken cancellationToken = default )
    {
        await _context.Polls.AddAsync(poll, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return poll;
    }




    public async Task<bool> UpdateAsync(int id, Poll poll, CancellationToken cancellationToken = default)
    {
        var existingPoll = await GetAsync(id , cancellationToken);

        if (existingPoll is null)
            return false;
        existingPoll.Title = poll.Title;
        existingPoll.Summary = poll.Summary;
        existingPoll.StartsAt = poll.StartsAt;
        existingPoll.EndsAt = poll.EndsAt;

        _context.Polls.Update(existingPoll);
        await _context.SaveChangesAsync(cancellationToken);


        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await GetAsync(id, cancellationToken);

        if (poll is null) return false;

         _context.Polls.Remove(poll);
         await  _context.SaveChangesAsync( cancellationToken);
        return true;
    }

    public async Task<bool> TogglePublishAsync(int id , CancellationToken cancellationToken = default)
    {
        var currentPoll = await GetAsync(id, cancellationToken);
        if (currentPoll is null) return false;

        currentPoll.IsPublished = !currentPoll.IsPublished;
        _context.Polls.Update(currentPoll);
        await _context.SaveChangesAsync(cancellationToken);
        return true;

    }


}
