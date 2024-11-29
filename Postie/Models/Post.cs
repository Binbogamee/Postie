namespace Postie.Models
{
    public class Post
    {
        public Post()
        {
        }

        public Post(string text, Guid accountId, Guid? id = null, DateTime? createdby = null, DateTime? modifiedby = null)
        {
            Text = text;
            AccountId = accountId;

            if (id == null)
            {
                Id = Guid.NewGuid();
                CreatedBy = DateTime.UtcNow;
            }
            else
            {
                Id = id.Value;
                if (createdby != null)
                {
                    CreatedBy = createdby.Value;
                }
                ModifiedBy = modifiedby;
            }
        }

        public Guid Id { get; set; } = Guid.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedBy { get; set; } = DateTime.MinValue;
        public DateTime? ModifiedBy { get; set; } = null;
        public Guid AccountId { get; set; } = Guid.Empty;

    }

}
