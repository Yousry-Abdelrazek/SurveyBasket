namespace SurveyBasket.Errors;

public class QuestionErrors
{
    public static readonly Error QuestionNotFound = new Error("Question.NotFound", "No Question Was Found with the given Id!");
    public static readonly Error QuestionAlreadyExists = new Error("Question.AlreadyExists", "Another Question with the same content already exists!");
}
