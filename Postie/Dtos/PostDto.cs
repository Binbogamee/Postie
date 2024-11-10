namespace Postie.Dtos
{
    public record PostDto (
        Guid Id,
        string Text,
        DateTime CreatedBy,
        DateTime? ModifiedBy);
}
