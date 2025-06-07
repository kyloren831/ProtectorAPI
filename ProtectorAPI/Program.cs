using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProtectorAPI.Data;
using ProtectorAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddTransient<IEmailService, EmailService>();

builder.Services.AddTransient<IUserService, UserService>();

builder.Services.AddTransient<IAuthorizationService, AuthorizationService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ProtectorDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleConnectionString")));

var key = builder.Configuration.GetValue<string>("JwtSettings:key");
var keyBytes = Encoding.ASCII.GetBytes(key);

builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config =>
{
    config.RequireHttpsMetadata = false;
    config.SaveToken = true;
    config.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,  // Validar que la clave de firma sea correcta
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),  // Usar la clave secreta
        ValidateIssuer = false,  // No validamos el emisor del token
        ValidateAudience = false,  // No validamos la audiencia del token
        ValidateLifetime = true,  // Validamos que el token no haya expirado
        ClockSkew = TimeSpan.Zero,  // No hay tolerancia para la expiración del token
       
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
