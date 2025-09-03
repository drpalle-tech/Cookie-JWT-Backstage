using Identity.Data;
using Identity.Services;
using Identity.SMTP;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    //Once the connection string is added
    //Run Add-Migration in package manager console 
    //Ex: "Add-Migration init" will generate init.cs as migration
    //Then run "Update-Database init" to reflect the identity tables in DB
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//This will take care of adding identity and defining basic constraints
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;

    options.Lockout.MaxFailedAccessAttempts = 5;
    options.User.RequireUniqueEmail = true;

    //To ensure email confirmation is done.
    options.SignIn.RequireConfirmedEmail = true;
})
    //Add DB at the end as this is our store
    .AddEntityFrameworkStores<ApplicationDbContext>()
    //To add token provider
    .AddDefaultTokenProviders();

//Similar to DpCookie in Web APP
//To configure app cookie and a login path
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Signup";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddControllersWithViews();

//To read SMTP values for mail sending from appSettings and bind it to model.
builder.Services.Configure<SMTPModel>(builder.Configuration.GetSection("SMTP"));

//Add singleton dependency as EMailService is constant throught the web app.
builder.Services.AddSingleton<IEmailService, EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapDefaultControllerRoute();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//Middle ware is simply plug and play
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
