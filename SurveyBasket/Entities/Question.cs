namespace SurveyBasket.Entities;

public sealed class Question : AuditableEntity
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;

    public int PollId { get; set; }

    public bool IsActive { get; set; } = true;
    // We will use Soft Delete, so we need to keep track of the deleted questions.
    public Poll Poll { get; set; } = default!;

    public ICollection<Answer> Answers { get; set; } = [];

}
