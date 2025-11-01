
namespace SurveyBasket.Services;

public class PollService : IPollService
{
    private static  readonly List<Poll> _polls = [
    new Poll
        {
            Id = 1,
            Title = "Poll 1",
            Description = "Description for Poll 1"
        }
    ];

    public Poll Add(Poll poll)
    {
        poll.Id = _polls.Count + 1; 
        _polls.Add(poll);
        return poll;
    }

    public Poll Get(int id)
    {
        var poll = _polls.FirstOrDefault(p => p.Id == id);
        return poll;
    }

    public IEnumerable<Poll> GetAll()
    {
        return _polls;
    }

    public bool Update(int id, Poll poll)
    {
        var existingPoll = Get(id);
        if (existingPoll is null) return false;

        existingPoll.Title = poll.Title;
        existingPoll.Description = poll.Description;

        return true;
    }

    public bool Delete(int id)
    {
        var poll = Get(id);
        if (poll is null) return false;
        _polls.Remove(poll);
        return true;
    }


}
