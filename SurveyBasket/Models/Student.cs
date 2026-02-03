namespace SurveyBasket.Models;

public class Student
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    [MinAge(18 )]
    public DateTime? DateOfBirth { get; set; }
}
