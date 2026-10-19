using System.Text;
using BeeMart.Api.Data;
using BeeMart.Api.Entities;
using BeeMart.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// EF Core — SQL Server. Connection string in appsettings.json ("BeeMartDb").
builder.Services.AddDbContext<BeeMartDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BeeMartDb")));

// Password hashing (PBKDF2) and app services.
builder.Services.AddScoped<IPasswordHasher<MsUser>, PasswordHasher<MsUser>>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<INumberGenerator, NumberGenerator>();

// JWT bearer authentication.
var jwt = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
        };
    });
builder.Services.AddAuthorization();

// Allow the WebClient (different origin) to call the API.
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
