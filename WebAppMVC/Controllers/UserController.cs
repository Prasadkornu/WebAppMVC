using Microsoft.AspNetCore.Mvc;
using WebAppMVC.Data;
using WebAppMVC.Models;

public class UserController : Controller
{
    private readonly UserContext _context;

    public UserController(UserContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    //Register

    [HttpPost]
    public IActionResult Register(string username, string password)
    {
        if (!ModelState.IsValid)
        {
            return View(User);
        }

        var user = new User
        {
            Username = username,
            Password = password
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        return RedirectToAction("Login","User");
    }
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    //Login

    [HttpPost]
    public IActionResult Login(string Username, string Password)
    {
        var user = _context.Users.SingleOrDefault(u => u.Username == Username && u.Password == Password);

        if (user != null)
        {
           
            return RedirectToAction("Index","Home");
        }

        ViewBag.ErrorMessage = "Invalid username or password.";

        return View("Login");
    }



}
