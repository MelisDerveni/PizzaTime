#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PizzaTime.Models;
public class Order
{
    [Key]
    public int OrderId { get; set;}
    public int TotalPrize{get;set;}
    
    


    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}