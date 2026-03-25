namespace SurveyBasket.Errors;


public class PollErrors
{
    public static readonly Error PollNotFound = new Error("Poll.NotFoud", "No Poll Was Found with the given Id!");
}
