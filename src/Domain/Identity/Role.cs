using Domain.Models.Shared;

namespace Domain.Identity;

public class Role : BaseModel
{
    public string Name { get; set; } = String.Empty;

    public IEnumerable<User> Users { get; set; } = Enumerable.Empty<User>();
}
