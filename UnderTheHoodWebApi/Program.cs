using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var signingKey = builder.Configuration.GetValue<string>("secret");

//After this step only, the request is treated as authenticated.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    //These are needed to validate JWT token when a HTTP request is received.
    //On every HTTP request, the token is decrypted for hashed claims and they are compared 
    //with the original JWT token hashed claims.
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        //As this is local web api, my local web app is the only audience
        //In real time, these two need to be validated
        ValidateAudience = false,
        ValidateIssuer = false,
        //Honor the token's lifetime
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        //Same key must be used during decryption and generating hashed claims
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey ?? string.Empty)),
        //Important when your audience are across time spans
        //You dont want other time span users to access (set to zero).
        ClockSkew = TimeSpan.Zero
    };
});

//Adding policies for authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireClaim("admin");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Adding middleware here to ensure authentication and authorization happens.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
