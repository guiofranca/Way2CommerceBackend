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

    public DataContextInMemory()
    {
        //SqliteConnection connection = new SqliteConnection("Data Source=InMemory;Mode=Memory;Cache=Shared");
        SqliteConnection connection = new SqliteConnection("Data Source=InMemory;Mode=Memory");
        connection.Open();

        var options = new DbContextOptionsBuilder<DataContext>()
            .UseSqlite(connection)
            .EnableSensitiveDataLogging()
            .Options;

        _dbContext = new DataContext(options);
        _dbContext.Database.EnsureCreated();
    }

    public DataContext GetContext() => _dbContext;
}
