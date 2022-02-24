using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Mysql.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mysql.Factories;

public class ContextFactory : IDesignTimeDbContextFactory<DataContext>
{
    public DataContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DataContext>();

        return new DataContext(optionsBuilder.Options);
    }
}
