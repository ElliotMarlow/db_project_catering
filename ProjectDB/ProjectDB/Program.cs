using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http.Extensions;

#pragma warning disable CS8600
#pragma warning disable CS8604
#pragma warning disable CS8602

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
    else{return false;}
}
static string RightNow()
{
    string dt = DateTime.Now.ToString();
string datetime = "'" + dt.Substring(6, 4) + "-" + dt.Substring(3, 2) + "-" + dt.Substring(0, 2) + " " + dt.Substring(11, 2) + ":" + dt.Substring(14, 2) + ":00'";
  
    return datetime;
}
static string Season()
{
    string dt = DateTime.Now.ToString();
    string month = dt.Substring(3, 2);

    string season = "";
    switch (month)
    {
        case "01":
        season = "winter";
        break;
        case "02":
        season = "winter";
        break;
                case "03":
season = "spring";
        break;
                case "04":
season = "spring";
        break;
                case "05":
season = "spring";
        break;
                case "06":
season = "summer";
        break;
                case "07":
season = "summer";
        break;
                case "08":
season = "summer";
        break;
                case "09":
season = "autumn";
        break;
                case "10":
season = "autumn";
        break;
                case "11":
season = "autumn";
        break;
                case "12":
season = "winter";
        break;
    }
    return season;
}

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
                 response += "###";
            }
        }
        reader.Close();
    }
    return response;
}
static string GetCondSQL(string table, string condition)
{
    string response = "";
    string request = "select * from " + table + " where " + condition;
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
                 response += "###";
            }
        }
        reader.Close();
    }
    return response;
}
    static void DoSQL(string request)
    {
        SqlConnection connection = new SqlConnection(@"Server=DESKTOP-PHIG5LD\MSSQLSERVER01;Database=Restaurant1;Trusted_Connection=True;TrustServerCertificate=True;");
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = request;
        cmd.Connection = connection;
        connection.Open();
        cmd.ExecuteNonQuery();
        connection.Close();
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


        string login = form["login"];
        string password = form["password"];

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
    context.Response.WriteAsJsonAsync(GetAllSQL("tbl_Table"));
});
app.MapGet("/api/table/{id}", (HttpContext context) =>
{
    string id = context.Request.GetDisplayUrl().Split("/")[5];
    context.Response.WriteAsJsonAsync(GetCondSQL("tbl_CountDish join tbl_Order on id_order = id", "table_id = " + id));
});
app.MapGet("/api/menu", (HttpContext context) =>
{
    context.Response.WriteAsJsonAsync(GetCondSQL("tbl_Menu", "season = '" + Season() + "'"));
});
app.MapPost("/api/open_order", (HttpContext context) =>
{
    string table = context.Request.Form["table"];
    string order = "INSERT INTO tbl_Order(table_id, datentime) values (" 
    + table.ToString() + ", " + RightNow() + ")";
    string update = "UPDATE tbl_Table SET reserved = 1 WHERE id = " + table;
    DoSQL(order);
    DoSQL(update);
    context.Response.Redirect("/cook");
});

app.Map("/", (HttpContext context) => { context.Response.Redirect("/login"); });
app.Run();
#pragma warning disable CS8602
#pragma warning disable CS8604
#pragma warning restore CS8600