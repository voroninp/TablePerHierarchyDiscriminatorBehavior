// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore;

var c = new Testcontainers.PostgreSql.PostgreSqlBuilder()
    .WithDatabase("db")
    .WithUsername("app")
    .WithPassword("password")
    .Build();

await c.StartAsync();

var ctx = new Context(c.GetConnectionString());
await ctx.Database.EnsureCreatedAsync();

var bar1 = new Bar1();
var bar2 = new Bar2();
await ctx.AddAsync(bar1);
await ctx.AddAsync(bar2);

await ctx.SaveChangesAsync();



public abstract class Foo
{
    public int Id { get; private set; }
    public abstract int FooType { get; }
}

public sealed class Bar1 : Foo
{
    public override int FooType => 1;
}

public sealed class Bar2 : Foo
{
    public override int FooType => 2;
}


public sealed class Context : DbContext
{
    private readonly string _connectionString;

    public Context(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Foo>(e =>
        {
            e.HasDiscriminator<int>("FooType")
            .HasValue<Bar1>(1)
            .HasValue<Bar2>(2);

            e.Ignore(p => p.FooType);
        });
            

        modelBuilder.Entity<Bar1>();
        modelBuilder.Entity<Bar2>();
    }
}