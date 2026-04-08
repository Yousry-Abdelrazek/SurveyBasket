namespace SurveyBasket.Errors;


public class PollErrors
{
    public static readonly Error PollNotFound = new Error("Poll.NotFoud", "No Poll Was Found with the given Id!");
    public static readonly Error PollAlreadyExists = new Error("Poll.AlreadyExists", "Another Poll with same titile is AlreadyExists! ");
}
