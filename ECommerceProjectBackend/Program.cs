using DotNetEnv;
using ECommerceProject.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);

Env.Load();

builder
    .Services
    .ConfigureHttpJsonOptions(options => {
        options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
    });

builder.Services.AddCors(options => {
    options.AddPolicy("AllowLocal", policy => {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
                   .AllowAnyMethod();
    });
});

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

string GenerateJwtToken(User user) {
    var claims = new[]
 {
        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
  new Claim(ClaimTypes.Email, user.UserEmail),
        new Claim(ClaimTypes.Name, user.UserName)
     };
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var token = new JwtSecurityToken(
   issuer: builder.Configuration["Jwt:Issuer"],
        audience: builder.Configuration["Jwt:Audience"],
 claims: claims,
        expires: DateTime.UtcNow.AddMinutes(30),
     signingCredentials: creds
    );
    return new JwtSecurityTokenHandler().WriteToken(token);
}

builder.Services.AddDbContext<ECommerceDbContext>(options => {
    string pass = Env.GetString("PASSWORD");
    string host = Env.GetString("HOST");
    string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if(connectionString != null) {
        connectionString = connectionString.Replace("myPassword", pass);
        connectionString = connectionString.Replace("db-host", host);
        options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 4, 0)));
    }
});

var app = builder.Build();

app.UseCors("AllowLocal");
app.UseAuthentication();
app.UseAuthorization();

var productsApi = app.MapGroup("/products");
productsApi.MapGet("/", (ECommerceDbContext db) => {
    var res = db.Products.ToList();
    return Results.Ok(res);
});

productsApi.MapGet(
    "/{id}",
    (ECommerceDbContext db, int id) =>
        db.Products.Find(id) is Product product ? Results.Ok(product) : Results.NotFound()
);

productsApi.MapPost("/", (ECommerceDbContext db, Product product) => {
    db.Products.Add(product);
    db.SaveChanges();
    return Results.Created($"/products/{product.ProductId}", product);
});

// Authentication endpoints
var authApi = app.MapGroup("/auth");

authApi.MapPost("/register", (ECommerceDbContext db, RegisterRequest request) => {
    // Validate input
    if(string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.UserEmail) || string.IsNullOrWhiteSpace(request.UserPassword)) {
        return Results.BadRequest(new { message = "Username, email, and password are required" });
    }

    // Check if user already exists
    if(db.Users.Any(u => u.UserName == request.UserName)) {
        return Results.Conflict(new { message = "Username already exists" });
    }

    if(db.Users.Any(u => u.UserEmail == request.UserEmail)) {
        return Results.Conflict(new { message = "Email already exists" });
    }

    // Create new user
    var user = new User {
        UserName = request.UserName,
        UserEmail = request.UserEmail,
        UserPassword = request.UserPassword, // TODO: Hash password with BCrypt before storing in production
        UserBirthDate = request.UserBirthDate ?? "",
        UserGender = request.UserGender
    };

    db.Users.Add(user);
    db.SaveChanges();

    // Generate JWT token
    var token = GenerateJwtToken(user);

    return Results.Created($"/users/{user.UserId}", new {
        token,
        userId = user.UserId,
        userName = user.UserName,
        userEmail = user.UserEmail
    });
});

authApi.MapPost("/login", (ECommerceDbContext db, LoginRequest request) => {
    // Find user by username or email
    var user = db.Users.FirstOrDefault(u =>
        u.UserName == request.UsernameOrEmail || u.UserEmail == request.UsernameOrEmail);

    if(user == null) {
        return Results.Unauthorized();
    }

    // Verify password (TODO: Use BCrypt.Net.BCrypt.Verify for hashed passwords in production)
    if(user.UserPassword != request.UserPassword) {
        return Results.Unauthorized();
    }

    // Generate JWT token
    var token = GenerateJwtToken(user);

    return Results.Ok(new {
        token,
        userId = user.UserId,
        userName = user.UserName,
        userEmail = user.UserEmail
    });
});

// User endpoints (protected)
var usersApi = app.MapGroup("/users");

usersApi.MapGet("/{id}", [Authorize] (ECommerceDbContext db, int id) => {
    var user = db.Users.Find(id);
    if(user == null) {
        return Results.NotFound();
    }

    // Return user without password
    return Results.Ok(new {
        userId = user.UserId,
        userName = user.UserName,
        userEmail = user.UserEmail,
        userBirthDate = user.UserBirthDate,
        userGender = user.UserGender
    });
});

usersApi.MapGet("/", [Authorize] (ECommerceDbContext db) => {
    var users = db.Users.Select(u => new {
        userId = u.UserId,
        userName = u.UserName,
        userEmail = u.UserEmail,
        userBirthDate = u.UserBirthDate,
        userGender = u.UserGender
    }).ToList();

    return Results.Ok(users);
});

usersApi.MapPut("/{id}", [Authorize] (ECommerceDbContext db, int id, UpdateUserRequest request) => {
    var user = db.Users.Find(id);
    if(user == null) {
        return Results.NotFound();
    }

    // Update user properties if provided
    if(!string.IsNullOrWhiteSpace(request.UserEmail))
        user.UserEmail = request.UserEmail;

    if(!string.IsNullOrWhiteSpace(request.UserBirthDate))
        user.UserBirthDate = request.UserBirthDate;

    if(request.UserGender.HasValue)
        user.UserGender = request.UserGender.Value;

    db.SaveChanges();

    return Results.Ok(new {
        userId = user.UserId,
        userName = user.UserName,
        userEmail = user.UserEmail,
        userBirthDate = user.UserBirthDate,
        userGender = user.UserGender
    });
});

usersApi.MapDelete("/{id}", [Authorize] (ECommerceDbContext db, int id) => {
    var user = db.Users.Find(id);
    if(user == null) {
        return Results.NotFound();
    }

    db.Users.Remove(user);
    db.SaveChanges();

    return Results.NoContent();
});

app.Run();

// Request/Response DTOs
record LoginRequest(string UsernameOrEmail, string UserPassword);
record RegisterRequest(string UserName, string UserEmail, string UserPassword, string? UserBirthDate, sbyte UserGender);
record UpdateUserRequest(string? UserEmail, string? UserBirthDate, sbyte? UserGender);

[JsonSerializable(typeof(Product))]
[JsonSerializable(typeof(Product[]))]
[JsonSerializable(typeof(User))]
[JsonSerializable(typeof(LoginRequest))]
[JsonSerializable(typeof(RegisterRequest))]
[JsonSerializable(typeof(UpdateUserRequest))]
internal partial class AppJsonSerializerContext : JsonSerializerContext { }
