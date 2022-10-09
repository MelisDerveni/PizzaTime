#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PizzaTime.Models;

public class User
{
    [Key]
    public int UserId {get ; set ;}

    [Required]
    [MinLength(2,ErrorMessage = "First name must be 2 characters or longer!")]
    public string FirstName {get ; set ;}

    [Required]
    [MinLength(2,ErrorMessage = "Lastname must be 2 characters or longer!")]
    public string Lastname {get ; set ;}

    [Required]
    public string Email {get ; set ;}

    [Required]
    public string City  {get ; set;}

    [Required]
    public string State {get ; set;}
    public int LikerId{get;set;}
    
    public List<Pizza> FavouritePizzas { get; set; } = new List<Pizza>(); 
    
    public List<Pizza> CreatedPizzas {get;set;} = new List<Pizza>();
    

    [DataType(DataType.Password)]
    [Required]
    [MinLength(8, ErrorMessage = "Password must be 8 characters or longer!")]
    public string Password { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    [NotMapped]
    [Compare("Password")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }

    
}


public class LoginUser
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}