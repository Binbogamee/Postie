using System.ComponentModel.DataAnnotations;

namespace Postie.DAL.Entities
{
    public class AccountEntity
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public ICollection<PostEntity> Posts { get; set; } = new List<PostEntity>();
    }
}
