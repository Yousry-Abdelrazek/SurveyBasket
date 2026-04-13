namespace SurveyBasket.Errors;

public class VoteErrors
{
    public static readonly Error VoteAleardyExists = 
        new Error("Vote.AlreadExists", "This user already voted before for this poll", StatusCodes.Status409Conflict);
    public static readonly Error InvalidQuestions = 
        new Error("Vote.InvalidQuestion", "Invalid Questions", StatusCodes.Status400BadRequest);

}
