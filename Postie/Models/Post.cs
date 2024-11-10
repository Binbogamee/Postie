namespace Postie.Models
{
    public class Post
    {
        public Post()
        {
            Id = Guid.Empty;
            CreatedBy = DateTime.MinValue;
        }

        public Post(string text, Guid? id = null, DateTime? createdby = null, DateTime? modifiedby = null)
        {
            Text = text;

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

        public Guid Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedBy { get; set; } = DateTime.MinValue;
        public DateTime? ModifiedBy { get; set; } = null;

    }

}
