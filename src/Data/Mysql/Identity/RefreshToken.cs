using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mysql.Identity;

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public ApplicationUser? User { get; set; }
    public Guid UserId { get; set; }
    public DateTime Expiry { get; set; }
}
