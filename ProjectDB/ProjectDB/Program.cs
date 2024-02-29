using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options => options.LoginPath = "/login");
builder.Services.AddAuthorization();
var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();


app.MapGet("/login", async (HttpContext context) =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    context.Response.SendFileAsync("html/login.html").GetAwaiter().GetResult();
});
app.MapPost("/login", async (string? returnUrl, HttpContext context) =>
{
    var form = context.Request.Form;
    if (form.ContainsKey("login") && form.ContainsKey("password"))
    {
        string login = form["login"];
        string password = form["password"];
        if (true)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, login) };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity)).GetAwaiter().GetResult();
            return Results.Redirect("/home");
        }
        else
        {
            return Results.Redirect("/login");
        }
    }
    else { return Results.BadRequest("Email и/или пароль не установлены"); }
});
app.MapGet("/logout", (HttpContext context) =>
{
    context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
});
app.Map("/", (HttpContext context) => { context.Response.Redirect("/login"); });
app.Run();