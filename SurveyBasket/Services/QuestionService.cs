using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Memory;
using SurveyBasket.Contracts.Answers;
using SurveyBasket.Contracts.Questions;

namespace SurveyBasket.Services;

public class QuestionService(
    ApplicationDbContext context,
    HybridCache hybridCache ,
    ILogger<QuestionService> logger) : IQuestionService
{
    private readonly ApplicationDbContext _context = context;
    private readonly HybridCache _hybridCache = hybridCache;
    private readonly ILogger<QuestionService> _logger = logger;
    private const string _cachePrefix = "availableQuestions"; 

    public async Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var pollIsExists = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);
        if (!pollIsExists)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

        var questions = await _context.Questions
            .Where(q => q.PollId == pollId)
            .Include(q => q.Answers)
            //.Select(q => new QuestionResponse(
            //    q.Id, 
            //    q.Content,
            //    q.Answers.Select(a => new AnswerResponse(a.Id, a.Content))
            //    ))
            .ProjectToType<QuestionResponse>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);


        return Result.Success<IEnumerable<QuestionResponse>>(questions);

    }

    public async Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(int pollId, string userId, CancellationToken cancellationToken = default)
    {
        var hasVote = await _context.Votes.AnyAsync(x => x.PollId == pollId && x.UserId == userId, cancellationToken);

        if (hasVote)
            return Result.Failure<IEnumerable<QuestionResponse>>(VoteErrors.VoteAleardyExists);


        var pollIsExists = await _context.Polls.
            AnyAsync(p => p.IsPublished && p.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && p.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);

        if (!pollIsExists)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

        var cacheKey = $"{_cachePrefix}_{pollId}";

        var questions = await _hybridCache.GetOrCreateAsync<IEnumerable<QuestionResponse>>(
            cacheKey,
            async cacheEntry => await _context.Questions
            .Where(x => x.PollId == pollId && x.IsActive)
            .Include(q => q.Answers)
            .Select(q => new QuestionResponse(
                q.Id,
                q.Content,
                q.Answers.Where(a => a.IsActive).Select(a => new AnswerResponse(a.Id, a.Content))

                ))
            .AsNoTracking()
            .ToListAsync(cancellationToken)

            );

        



        return Result.Success<IEnumerable<QuestionResponse>>(questions!);


    } 

    public async Task<Result<QuestionResponse>> GetAsync(int pollId, int id, CancellationToken cancellationToken = default)
    {
        var question = await _context.Questions
            .Where(q => q.PollId == pollId && q.Id == id)
            .Include(q => q.Answers)
            .ProjectToType<QuestionResponse>()
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);

        if(question is null)
            return Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound);

        return Result.Success(question);

    }

    public async Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken = default)
    {
        var pollIsExists = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);
        if (!pollIsExists)
            return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

        var questionIsExists = await _context.Questions.AnyAsync(q => q.Content == request.Content && q.PollId == pollId, cancellationToken);

        if (questionIsExists)
            return Result.Failure<QuestionResponse>(QuestionErrors.QuestionAlreadyExists);

        var question = request.Adapt<Question>();
        question.PollId = pollId;

        //request.Answers.ForEach(
        //    a => question.Answers.Add(new Answer { Content = a })
        //    );
        // Done by Mapster's AfterMapping


        await _context.Questions.AddAsync(question, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        await _hybridCache.RemoveAsync($"{_cachePrefix}_{pollId}", cancellationToken);

        return Result.Success(question.Adapt<QuestionResponse>());
    }

    public async Task<Result> UpdateAsync(int pollId, int id, QuestionRequest request, CancellationToken cancellationToken = default)
    {


        var pollIsExists = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);
        if (!pollIsExists)
            return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

        var questionIsExists = await _context.Questions.AnyAsync(q => q.Content == request.Content && q.PollId == pollId && q.Id != id, cancellationToken);

        if (questionIsExists)
            return Result.Failure(QuestionErrors.QuestionAlreadyExists);

        var question = await _context.Questions
            .Include(q => q.Answers)
            .SingleOrDefaultAsync(x => x.PollId == pollId && x.Id == id, cancellationToken);

        if (question is null)
            return Result.Failure(QuestionErrors.QuestionNotFound);

        question.Content = request.Content;

        // Update answers
        // current answers

        var currentAnswers = question.Answers.Select(a => a.Content).ToList();

        // new answers 
        var newAnswers = request.Answers.Except(currentAnswers).ToList();

        // add new answers to the question       
        newAnswers.ForEach(a => question.Answers.Add(new Answer { Content = a }));

        // mark existing answers as active or inactive based on the request => Soft delete
        question.Answers.ToList().ForEach(answer =>
        {
            answer.IsActive = request.Answers.Contains(answer.Content);
        });

        await _context.SaveChangesAsync(cancellationToken);

        await _hybridCache.RemoveAsync($"{_cachePrefix}_{pollId}", cancellationToken);



        return Result.Success();
        


    }


    public async Task<Result> ToggleStatusAsync(int pollId, int id, CancellationToken cancellationToken = default)
    {
        var question = await _context.Questions.SingleOrDefaultAsync(x => x.PollId == pollId && x.Id == id, cancellationToken);
        if (question is null)
            return Result.Failure(QuestionErrors.QuestionNotFound);


        question.IsActive = !question.IsActive; 


        await _context.SaveChangesAsync(cancellationToken);
        await _hybridCache.RemoveAsync($"{_cachePrefix}_{pollId}", cancellationToken);


        return Result.Success();
    }


}
