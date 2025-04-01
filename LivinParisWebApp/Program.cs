using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load("../.env");

// Razor Pages
builder.Services.AddRazorPages();

builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();

app.UseRouting();

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/Login");
        return;
    }

    await next();
});


app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();

app.Run();