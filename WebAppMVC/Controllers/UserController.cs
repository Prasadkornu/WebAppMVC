using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
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
    public IActionResult Register(string username, string password, [FromServices] IConfiguration configuration)
    {
        if (!ModelState.IsValid)
        {
            return View("Register");
        }
        string secretKey = configuration["AppSettings:SecretKey"];
        string hashedPassword = HashPasswordMD5(password,secretKey);

        var user = new User
        {
            Username = username,
            Password = hashedPassword
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        return RedirectToAction("Login","User");
    }

    private string HashPasswordMD5(string password, string secretKey)
    {
        using (MD5 md5 = MD5.Create())
        {
            // Combine password and secret key
            string combinedString = $"{password}{secretKey}";

            byte[] hashedBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(combinedString));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }


    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    //Login

    [HttpPost]
    public IActionResult Login(string Username, string Password, [FromServices] IConfiguration configuration)
    {
        var user = _context.Users.FirstOrDefault(u => u.Username == Username);

        if (user != null)
        {
            string secretKey = configuration["AppSettings:SecretKey"];

            if (VerifyPasswordMD5(Password, user.Password,secretKey))
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // User not found or password verification failed
        ViewBag.ErrorMessage = "Invalid username or password.";

        return View("LoginError");
    }


    private bool VerifyPasswordMD5(string enteredPassword, string storedHashedPassword, string secretKey)
    {
        using (MD5 md5 = MD5.Create())
        {
            // Combine entered password and secret key
            string combinedString = $"{enteredPassword}{secretKey}";

            byte[] hashedBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(combinedString));
            string enteredPasswordHash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

            // Compare the entered password hash with the stored hashed password
            return string.Equals(enteredPasswordHash, storedHashedPassword, StringComparison.OrdinalIgnoreCase);
        }
    }



}
