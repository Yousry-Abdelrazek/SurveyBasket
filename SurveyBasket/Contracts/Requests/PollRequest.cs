namespace SurveyBasket.Contracts.Requests;

public record PollRequest( 
    
    string Title, 
    string Summary,
    DateOnly StartsAt,
    DateOnly EndsAt
    );
