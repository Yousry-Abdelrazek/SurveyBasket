
namespace SurveyBasket.Services;

public class PollService : IPollService
{
    private static readonly List<Poll> _polls = new List<Poll>()
    {
        new Poll()
        {
            Id = 1 ,
            Title = "Poll 1",
            Description = "Description for Poll 1"
        },
        new Poll()
        {
            Id = 2 ,
            Title = "Poll 2",
            Description = "Description for Poll 2"
        },
    };
    public IEnumerable<Poll> GetAll() => _polls;


    public Poll? Get(int id) => _polls.FirstOrDefault(x => x.Id == id);
    public Poll Add(Poll poll)
    {
        poll.Id = _polls.Count + 1;
        _polls.Add(poll);
        return poll;
    }

    public bool Update(int id, Poll poll)
    {
        var existingPoll = Get(id);
        if (existingPoll is null)
        {
            return false;
        }
        existingPoll.Title = poll.Title;
        existingPoll.Description = poll.Description;
        return true;
    }

    public bool Delete(int id)
    {
        var poll = Get(id);
        if (poll is null)
        {
            return false;
        }
        _polls.Remove(poll);
        return true;

    }
}
