using Microsoft.AspNetCore.Identity.UI.Services;
using SurveyBasket.Helpers;

namespace SurveyBasket.Services;

public class NotificationService(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    IHttpContextAccessor httpContextAccessor,
    IEmailSender emailSender) : INotificationService
{
    private readonly ApplicationDbContext _context = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IEmailSender _emailSender = emailSender;

    public async Task SendNewPollNotification(int? pollId = null)
    {
        IEnumerable<Poll> polls = [];

        if (pollId.HasValue)
        {
            var poll = await _context.Polls
                .Where(x => x.Id == pollId.Value && x.IsPublished && x.StartsAt == DateOnly.FromDateTime(DateTime.UtcNow))
                .AsNoTracking()
                .FirstOrDefaultAsync();

            polls = [poll!];
        }
        else
        {
            polls = await _context.Polls
                .Where(x => x.IsPublished && x.StartsAt == DateOnly.FromDateTime(DateTime.UtcNow))
                .AsNoTracking()
                .ToListAsync();
        }


        //TODO :: SELECT Members ONly
        //

        var users = await _userManager.Users.ToListAsync();

        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

        foreach (var poll in polls)
        {
            foreach(var user in users)
            {
                var placeHolder = new Dictionary<string, string>
                {
                    {"{{name}}", user.FirstName},
                    {"{{pollTill}}", poll.Title},
                    {"{{endDate}}", poll.EndsAt.ToString()},
                    {"{{url}}",$"{origin}/polls/start/{poll.Id}" }
                };

                var body = EmailBodyBuilder.GenerateEmailBody("PollNotification", placeHolder);

                await _emailSender.SendEmailAsync(user.Email!, $"New Poll Available - {poll.Title}", body);
            }
            
        }
    }
}
