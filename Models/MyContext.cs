#pragma warning disable CS8618
/* 
Disabled Warning: "Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable."
We can disable this safely because we know the framework will assign non-null values when it constructs this class for us.
*/
using Microsoft.EntityFrameworkCore;
namespace PizzaTime.Models;
// the MyContext class representing a session with our MySQL database, allowing us to query for or save data
public class MyContext : DbContext 
{ 
    public MyContext(DbContextOptions options) : base(options) { }
    // the "Monsters" table name will come from the DbSet property name
    // public DbSet<Monster> Monsters { get; set; } 
    public DbSet<User> Users { get; set; }
    public DbSet<Pizza> Pizzas { get; set; }
    public DbSet<Order> Orders { get; set; }

    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    
        modelBuilder.Entity<Pizza>()
        .HasOne(c=>c.Creator)
        .WithMany(c=>c.CreatedPizzas)
        .HasForeignKey(c=>c.UserId);

        modelBuilder.Entity<Pizza>()
        .HasOne(c=>c.Liker)
        .WithMany(c=>c.FavouritePizzas)
        .HasForeignKey(c=>c.LikerId);

        modelBuilder.Entity<Pizza>()
        .HasOne(c=>c.Order)
        .WithMany(c=>c.PizzaOrdered);
    }
}