namespace Postie.Dtos
{
    public record RequestPostDto(
        Guid RequesterId,
        string Text);

    public record CreatedPostDto (
        Guid Id,
        Guid AccountId,
        string Text,
        DateTime CreatedBy,
        DateTime? ModifiedBy);
}
