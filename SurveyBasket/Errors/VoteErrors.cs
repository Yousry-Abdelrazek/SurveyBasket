namespace SurveyBasket.Errors;

public class VoteErrors
{
    public static readonly Error VoteAleardyExists = new Error("Vote.AlreadExists", "Another Vote with same vote content already exists!");
    public static readonly Error InvalidQuestions = new Error("Vote.InvalidQuestion", "Invalid Questions");

}
