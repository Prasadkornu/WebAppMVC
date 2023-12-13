using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using WebAppMVC.Data;
using WebAppMVC.Models;
using Microsoft.Extensions.Configuration;

public class UserController : Controller
{
    private readonly UserContext _context;
    private readonly IConfiguration _configuration;

    public UserController(UserContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(string username, string password)
    {
        if (!ModelState.IsValid)
        {
            return View("Register");
        }

        string secretKey = _configuration["AppSettings:SecretKey"];
        string hashedPassword = HashPasswordBcrypt(password, secretKey);

        var user = new User
        {
            Username = username,
            Password = hashedPassword
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        return RedirectToAction("Login", "User");
    }

    private string HashPasswordBcrypt(string password, string secretKey)
    {
        string combinedString = $"{password}{secretKey}";
        return BCrypt.Net.BCrypt.HashPassword(combinedString);
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string Username, string Password)
    {
        var user = _context.Users.FirstOrDefault(u => u.Username == Username);

        if (user != null)
        {
            string secretKey = _configuration["AppSettings:SecretKey"];

            if (VerifyPasswordBcrypt(Password, user.Password, secretKey))
            {
                return View("LoginSuccess");
            }
        }

        ViewBag.ErrorMessage = "Invalid username or password.";
        return View("LoginError");
    }

    private bool VerifyPasswordBcrypt(string enteredPassword, string storedHashedPassword, string secretKey)
    {
        string combinedString = $"{enteredPassword}{secretKey}";
        return BCrypt.Net.BCrypt.Verify(combinedString, storedHashedPassword);
    }
}