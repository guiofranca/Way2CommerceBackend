using Domain.Models;
using Domain.Models.Relations;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Mysql.Context;
using System;

namespace Mysql.Test.Context;

public class DataContextInMemory
{
    private readonly DataContext _dbContext;
    private readonly SqliteConnection _connection;

    public DataContextInMemory()
    {
        _connection = new SqliteConnection("DataSource=file:memdb1?mode=memory&cache=shared");
        _connection.Open();

        var options = new DbContextOptionsBuilder<DataContext>()
            .UseSqlite(_connection)
            .EnableSensitiveDataLogging()
            .Options;

        _dbContext = new DataContext(options);
        //_dbContext.Database.Migrate();
    }

    public DataContext GetContext() => _dbContext;
}
