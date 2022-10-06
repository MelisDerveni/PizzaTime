#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PizzaTime.Models;
public class Pizza
{
    [Key]
    public int PizzaId { get; set; }
    public string Method { get; set; } 
    public string Size { get; set; }
    public string Crust { get; set; }
    public string Quantity { get; set; }
    public string? Toppings {get; set;}
    public int UserId {get;set;}
    public User? Creator {get;set;}
    


    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}