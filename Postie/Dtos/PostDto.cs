namespace Postie.Dtos
{
    public record RequestPostDto(
        string Text);

    public record CreatedPostDto (
        Guid Id,
        Guid AccountId,
        string Text,
        DateTime CreatedBy,
        DateTime? ModifiedBy);
}
