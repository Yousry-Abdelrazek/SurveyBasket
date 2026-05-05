namespace SurveyBasket.Services;

public interface INotificationService
{
    Task SendNewPollNotification(int? pollId = null);
}
