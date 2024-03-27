using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Data.SqlClient;





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
//Restaurant1

static string GetAllSQL(string table)
{
    string response = "";
    string request = "select * from " + table;
    using (SqlConnection connection = new SqlConnection(@"Server=DESKTOP-PHIG5LD\MSSQLSERVER01;Database=Restaurant1;Trusted_Connection=True;TrustServerCertificate=True;"))
    {
        connection.Open();
        SqlCommand command = new(request, connection);
        SqlDataReader reader = command.ExecuteReader();
        if (reader.HasRows)
        {
            while (reader.Read())
            {
                for (int i = 0; reader.FieldCount > i; i++)
                {
                    response += reader.GetValue(i) + "%&%&";
                }
                 //+ reader.GetValue(1) + "%&%&" + reader.GetValue(2) + "%&%&" + reader.GetValue(5) + "%&%&";
                 response += "&&&";
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
    Console.WriteLine(GetAllSQL("tbl_Table"));
    //context.Response.WriteAsJsonAsync(GetAllSQL("tbl_Table"));
});


app.Map("/", (HttpContext context) => { context.Response.Redirect("/login"); });
app.Run();