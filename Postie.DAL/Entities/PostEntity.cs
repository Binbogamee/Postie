namespace Postie.DAL.Entities
{
    public class PostEntity
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedBy { get; set; }
        public DateTime? ModifiedBy { get; set; } = null;
        public Guid AccountId { get; set; }
        public AccountEntity Account { get; set; }
    }
}
