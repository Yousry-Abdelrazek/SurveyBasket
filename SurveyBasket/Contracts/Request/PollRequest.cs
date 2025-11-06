using System.ComponentModel.DataAnnotations;

namespace SurveyBasket.Contracts.Request;

public record PollRequest(
    int Id,
    string Title,
    string Summary,
    bool IsPublished,
    DateTime StartsAt,
    DateTime EndsAt
    );


