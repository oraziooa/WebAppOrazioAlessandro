using WebAppOrazioAlessandro.Services.Implementations;
using WebAppOrazioAlessandro.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using WebAppOrazioAlessandro.Data;
using Microsoft.AspNetCore.Identity;
using WebAppOrazioAlessandro.Entities;
using Hangfire;
using Hangfire.PostgreSql;
using WebAppOrazioAlessandro.Services.DeletionService;
using WebAppOrazioAlessandro.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// DbContext PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();


// Controllers, Swagger & SignalR
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Inserisci il token JWT"
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Dependency Injection Servizi
builder.Services.AddScoped<IPadiglioneService, PadiglioneService>();
builder.Services.AddScoped<ICategoriaMerceologicaService, CategoriaMerceologicaService>();
builder.Services.AddScoped<ISettoreService, SettoreService>();
builder.Services.AddScoped<IStandService, StandService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<StandDeletionService>();
builder.Services.AddScoped<CategoriaMerceologicaDeletionService>();
builder.Services.AddScoped<SettoreDeletionService>();
builder.Services.AddScoped<PadiglioneDeletionService>();

// Hangfire
builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(options =>
        options.UseNpgsqlConnection(
            builder.Configuration.GetConnectionString("DefaultConnection")
        )
    )
);
builder.Services.AddHangfireServer();

// Build App
var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


// Hangfire Dashboard
app.UseHangfireDashboard("/hangfire");

// Seed Ruoli + Admin e User
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    string[] roles = { "Admin", "Supervisor", "User" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    var adminEmail = builder.Configuration["SeedUsers:Admin:Email"];
    var adminPassword = builder.Configuration["SeedUsers:Admin:Password"];

    if (!string.IsNullOrWhiteSpace(adminEmail) &&
        !string.IsNullOrWhiteSpace(adminPassword))
    {
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            var newAdmin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail
            };
            var createResult = await userManager.CreateAsync(newAdmin, adminPassword);
            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(newAdmin, "Admin");
            }
        }
    }
    // USER
    var userEmail = builder.Configuration["SeedUsers:User:Email"];
    var userPassword = builder.Configuration["SeedUsers:User:Password"];

    if (!string.IsNullOrWhiteSpace(userEmail) &&
        !string.IsNullOrWhiteSpace(userPassword))
    {
        var normalUser = await userManager.FindByEmailAsync(userEmail);
        if (normalUser == null)
        {
            var newUser = new ApplicationUser
            {
                UserName = userEmail,
                Email = userEmail
            };

            var createResult = await userManager.CreateAsync(newUser, userPassword);
            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(newUser, "User");
            }
        }
    }
}

// Map Controllers & Hubs

app.MapControllers();
app.MapHub<NotificationHub>("/notifications");

// Run App

app.Run();
