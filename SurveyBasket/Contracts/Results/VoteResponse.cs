namespace SurveyBasket.Contracts.Results;

public record VoteResponse
(
    string VoteName , 
    DateTime VoteDate,
    IEnumerable<QuestionAnswerResposne> SelectedAnswers
);