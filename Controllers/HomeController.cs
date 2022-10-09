using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PizzaTime.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace PizzaTime.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private MyContext _context;
    public HomeController(ILogger<HomeController> logger, MyContext context)
    {
        _context = context;
        _logger = logger;
    }

    public IActionResult Index()
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("Register");
        }
        return View("Dashboard");
    }

    [HttpGet("Register")]
    public IActionResult Register()
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return View();
        }
        return RedirectToAction("Dashboard");
    }

    [HttpPost("Register")]
    public IActionResult Register(User user)
    {

        if (ModelState.IsValid)
        {
            if (_context.Users.Any(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Email already in use!");

                return View();
            }
            PasswordHasher<User> Hasher = new PasswordHasher<User>();
            user.Password = Hasher.HashPassword(user, user.Password);
            _context.Users.Add(user);
            _context.SaveChanges();
            HttpContext.Session.SetInt32("userId", user.UserId);
            return RedirectToAction("Dashboard");
        }
        return View();
    }

    [HttpGet("Login")]
    public IActionResult Login()
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return View();
        }
        return RedirectToAction("Index");
    }

    [HttpPost("Login")]
    public IActionResult LoginSubmit(LoginUser userSubmission)
    {
        if (ModelState.IsValid)
        {
            var userInDb = _context.Users.FirstOrDefault(u => u.Email == userSubmission.Email);
            if (userInDb == null)
            {
                ModelState.AddModelError("User", "Invalid UserName/Password");
                return View("Register");
            }

            var hasher = new PasswordHasher<LoginUser>();

            var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);

            if (result == 0)
            {
                ModelState.AddModelError("Password", "Invalid Password");
                return View("Register");
            }
            HttpContext.Session.SetInt32("userId", userInDb.UserId);

            return RedirectToAction("Dashboard");
        }
        return View("Register");
    }
    [HttpGet("Dashboard")]
    public IActionResult Dashboard()
    {
        return View();
    }
    [HttpPost("CreatePizza")]
    public IActionResult CreatePizza(Pizza FromView, string Toppings)
    {   
         if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("Register");
        }
        
        
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("Register");
        }
        
        
        if(ModelState.IsValid)
        {
            User LoggedInUser = _context.Users.First(c=>c.UserId == (int)HttpContext.Session.GetInt32("userId"));
            Order NewOrder = new Order{
                TotalPrize = 20

            };

            FromView.Toppings=Toppings;
            FromView.Order = NewOrder;
            FromView.OrderId = NewOrder.OrderId;
            FromView.UserId = LoggedInUser.UserId;
            FromView.Order = NewOrder;
            FromView.OrderId = NewOrder.OrderId;
            _context.Pizzas.Add(FromView);

            // LoggedInUser.FavouritePizzas.Add(FromView);
            
            NewOrder.PizzaOrdered.Add(FromView);
            _context.SaveChanges();
        return RedirectToAction ("AllOrders");
        }
        else{
            return View("Order");
        }
    }
    [HttpGet("Order")]
    public IActionResult Order()
    {
        return View();
    }
    [HttpGet("Account")]
    public IActionResult Account()
    {
        ViewBag.LoggedInUser =  _context.Users.First(c=>c.UserId == (int)HttpContext.Session.GetInt32("userId"));
        return View();
    }
    [HttpPost("UpdateUser/{id}")]
    public IActionResult UpdateUser(int Userid, User EditedUser)
    {
        if(ModelState.IsValid)
        {
        User RetrievedUser = _context.Users.FirstOrDefault(user => user.UserId == Userid);
        RetrievedUser.FirstName = EditedUser.FirstName;
        RetrievedUser.Lastname = EditedUser.Lastname;
        RetrievedUser.Email = EditedUser.Email;
        RetrievedUser.City = EditedUser.City;
        RetrievedUser.State = EditedUser.State;
        RetrievedUser.Password = EditedUser.Password;

        _context.SaveChanges();
        return RedirectToAction("Dashboard");
        }
        else{

            return View("Account");
        }
        
    }
    [HttpGet("PastOrders")]
    public IActionResult PastOrders()
    {   
        ViewBag.Pizzas = _context.Pizzas.Where(c=>c.LikerId ==(int)HttpContext.Session.GetInt32("userId") ).ToList();
        List<Pizza> Favorites = _context.Pizzas.Where(c=>c.LikerId ==(int)HttpContext.Session.GetInt32("userId") ).ToList();
        ViewBag.PastOrders = _context.Pizzas.Include(c=>c.Creator).Where(c=>c.Creator.UserId == (int)HttpContext.Session.GetInt32("userId")).OrderByDescending(e => e.CreatedAt).ToList();
        List<Pizza> NotFavorited = ViewBag.PastOrders;
        ViewBag.NotFavorited = NotFavorited.Except(Favorites).ToList();

        
        return View();
    }
    [HttpGet("Home/AllOrders")]
    public IActionResult YourOrder()
    {
        ViewBag.Order = _context.Orders.Include(c=>c.PizzaOrdered).OrderBy(e => e.CreatedAt).Last(e=> e.PizzaOrdered.Any(c=>c.UserId == (int)HttpContext.Session.GetInt32("userId")) );

        return View();
    }
    [HttpGet("RemoveFavorite/{id}")]
    public IActionResult RemoveFavorite(int id)
    {
        Pizza ToRemove = _context.Pizzas.Single(c=>c.PizzaId == id);
        User LoggedInUser = _context.Users.Single(c=>c.UserId == (int)HttpContext.Session.GetInt32("userId"));
        LoggedInUser.FavouritePizzas.Remove(ToRemove);
        _context.SaveChanges();
        return RedirectToAction("PastOrders");
    }
    [HttpGet("AddFavorite/{id}")]
    public IActionResult AddFavorite(int id)
    {
        Pizza ToAdd = _context.Pizzas.Single(c=>c.PizzaId == id);
        User LoggedInUser = _context.Users.Single(c=>c.UserId == (int)HttpContext.Session.GetInt32("userId"));
        LoggedInUser.FavouritePizzas.Add(ToAdd);
        _context.SaveChanges();
        return RedirectToAction("PastOrders");
    }
    [HttpGet("RndOrder")]
    public IActionResult RndOrder(){
        var rand = new Random();
        string[] Methods = new string[]{
            "Carry-Out",
            "Sit-Here",
            "Delivery"
        };
        string[] Sizes = new string[]{
            "Small",
            "Medium",
            "Large"
        };
        string[] Crusts = new string[]{
            "Thin",
            "Medium",
            "Thick"
        }; 
        string[] Quantities = new string[]{
            "One",
            "Two",
            "Three"
        }; 
        string[] Toppings = new string[]{
            "Ham",
            "Mushroom",
            "Cheese",
            "Sausage",
            "Onion",
            "GreenPepper",
            "BlackOlives",
            "FreshGarlick",
            "Tomato",
            "FreshBasil",
            "Pepperoni",
            "SeaFood"
        }; 
        String AddedToppings = Toppings[rand.Next(0,12)] + ","  + Toppings[rand.Next(0,12)] + "," + Toppings[rand.Next(0,12)] ;
        Pizza NewPizza = new Pizza{
            Method= Methods[rand.Next(0,3)],
            Size= Sizes[rand.Next(0,3)],
            Crust= Crusts[rand.Next(0,3)],
            Quantity= Quantities[rand.Next(0,3)],
            Toppings = AddedToppings,
            UserId = (int)HttpContext.Session.GetInt32("userId")

            
        };
        Order NewOrder = new Order{
                TotalPrize = 20

            };
        NewPizza.Order = NewOrder;
        NewPizza.OrderId = NewOrder.OrderId;
        _context.Pizzas.Add(NewPizza);
        _context.SaveChanges();
        ViewBag.ThisPizza = NewPizza;
        
        return View();
    }
    
    [HttpGet("Delete/{id}")]
    public IActionResult Delete(int id)
    {
               
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("Register");
        }
        Pizza removePizza = _context.Pizzas.First(e => e.PizzaId == id);
        int idFromSession = (int)HttpContext.Session.GetInt32("userId");   
        _context.Pizzas.Remove(removePizza);
        _context.SaveChanges();
       return RedirectToAction("Dashboard");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
