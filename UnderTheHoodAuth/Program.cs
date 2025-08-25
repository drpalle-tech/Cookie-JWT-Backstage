using Microsoft.AspNetCore.Authorization;
using UnderTheHoodAuth.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//Adding cookie as authentication mode
builder.Services.AddAuthentication("DpCookie").AddCookie("DpCookie", options =>
{
    options.Cookie.Name = "DpCookie";
    options.ExpireTimeSpan = TimeSpan.FromSeconds(100);
});

//Adding security policies for authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy
    .RequireClaim("admin")
    //Add probation for custom authorization requirement
    .Requirements.Add(new AdminAuthorization(3)));
});

builder.Services.AddSingleton<IAuthorizationHandler, AdminAuthorizationHandler>();

//Configuration to link web application to web API
// API will have data related stuff and respective HTTP end points
// Kind of OnCall.WebAPI -> used to serve data to OnCall.
builder.Services.AddHttpClient("DpWebAPI", options =>
{
    options.BaseAddress = new Uri("https://localhost:7247/");
});

//Logic to include session middleware as we cannot go and request JWT from web api on every req
builder.Services.AddSession(session =>
{
    //If the app is idle for 20 minutes, forget session
    session.IdleTimeout = TimeSpan.FromMinutes(20);
    session.Cookie.IsEssential = true;
    //DOnt give my cookie to JS or other scripts. It is for HTTP req only.
    session.Cookie.HttpOnly = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseReact(config => { });

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//Configuring middleware for auth
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapDefaultControllerRoute();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
