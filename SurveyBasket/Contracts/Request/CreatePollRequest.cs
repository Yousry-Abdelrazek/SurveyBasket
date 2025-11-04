using System.ComponentModel.DataAnnotations;

namespace SurveyBasket.Contracts.Request;

public record CreatePollRequest(
    [AllowedValues("New" , "Old" , ErrorMessage ="Allow Only (New , Old)")]
    string Title , 
    string Description
    );


