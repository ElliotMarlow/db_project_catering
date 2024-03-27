using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Data.SqlClient;

var builder = WebApplication.CreateBuilder();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options => options.LoginPath = "/login");
builder.Services.AddAuthorization();
var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();


static bool Check(string login, string password)
{
    if ((login == "cook" && password == "CoOk123@") | (login == "manage" && password == "MaNaGeR123@"))
    {
        return true;
    }
    else
    {
        return false;
    }
}

static string GetAllSQL(string table)
{
    string response = "";
    string request = "select * from " + table;
    using (SqlConnection connection = new SqlConnection(@"Data Source=LAPTOP-2MJPKAS0\MSSQLSERVER02;Initial Catalog=MY_DB;Integrated Security=True"))
    {
        connection.Open();
        SqlCommand command = new(request, connection);
        SqlDataReader reader = command.ExecuteReader();
        if (reader.HasRows)
        {
            while (reader.Read())
            {
                response += reader.GetValue(0) + "%&%&" + reader.GetValue(1) + "%&%&" + reader.GetValue(2) + "%&%&" + reader.GetValue(5) + "%&%&";
            }
        }
        reader.Close();
    }
    return response;
}


app.MapGet("/login", (HttpContext context) =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    context.Response.SendFileAsync("html/login.html").GetAwaiter().GetResult();
});
app.MapPost("/login", (string? returnUrl, HttpContext context) =>
{
    var form = context.Request.Form;
    if (form.ContainsKey("login") && form.ContainsKey("password"))
    {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        string login = form["login"];
        string password = form["password"];
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8604 // Possible null reference argument.
        if (Check(login, password))
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, login) };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity)).GetAwaiter().GetResult();
            return Results.Redirect("/" + login);
        }
        else
        {
            return Results.Redirect("/login");
        }
#pragma warning restore CS8604 // Possible null reference argument.
    }
    else { return Results.BadRequest("Email и/или пароль не установлены"); }
});
app.MapGet("/logout", (HttpContext context) =>
{
    context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
});


app.MapGet("/cook", (HttpContext context) =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    context.Response.SendFileAsync("html/cook.html").GetAwaiter().GetResult();
});


app.MapGet("/manage", (HttpContext context) =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    context.Response.SendFileAsync("html/host.html").GetAwaiter().GetResult();
});


app.MapGet("/api/tables", (HttpContext context) =>
{

    context.Response.WriteAsJsonAsync(context.User.Identity.Name.ToString());
});


app.Map("/", (HttpContext context) => { context.Response.Redirect("/login"); });
app.Run();