using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Identity.Relations;

public class UserRole
{
    public int UserId { get; set; }
    public int RoleId { get; set; }

    public UserRole(int userId, int roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }
}
