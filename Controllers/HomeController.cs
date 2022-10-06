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
        if(ModelState.IsValid)
        {
            User LoggedInUser = _context.Users.First(c=>c.UserId == (int)HttpContext.Session.GetInt32("userId"));
            

            FromView.Toppings=Toppings;
            _context.Pizzas.Add(FromView);
            LoggedInUser.FavouritePizzas.Add(FromView);
            _context.SaveChanges();
        return RedirectToAction ("Dashboard");
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
        RetrievedUser = EditedUser;
        _context.SaveChanges();
        return RedirectToAction("Dashboard");
        }
        else{
            Console.WriteLine(EditedUser.FirstName);
            Console.WriteLine(EditedUser.Lastname);
            Console.WriteLine(EditedUser.Email);
            Console.WriteLine(EditedUser.City);
            Console.WriteLine(EditedUser.State);
            Console.WriteLine(EditedUser.Password);

            return View("Account");
        }
        
    }
    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
