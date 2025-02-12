using Gestion_de_Bibliothéque;
using Gestion_de_Bibliothéque.DTOs;
using AutoMapper;
using Gestion_de_Bibliothéque.datas;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;



var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//Configuration du context de la base de donnee
var ConnexionString = builder.Configuration.GetConnectionString("Bibliotheque");
builder.Services.AddSqlite<BookDbContext>(ConnexionString);

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllersWithViews();


// Ajouter CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
// Ajouter Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<BookDbContext>()
    .AddDefaultTokenProviders();

// Ajouter l'authentification JWT
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
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

// Ajouter l'authentification et l'autorisation
app.UseAuthentication();
app.UseAuthorization();


app.MapGet("/", () => "Hello World!");


app.MapPost("/register", async (UserDTO userDto, UserManager<IdentityUser> userManager) =>
{
    var user = new IdentityUser { UserName = userDto.Username, Email = userDto.Email };
    var result = await userManager.CreateAsync(user, userDto.Password);

    return result.Succeeded ? Results.Ok("User registered successfully!") : Results.BadRequest(result.Errors);
});

app.MapPost("/login", async (LoginDTO loginDto, UserManager<IdentityUser> userManager, IConfiguration _configuration) =>
{
    var user = await userManager.FindByNameAsync(loginDto.Username);
    if (user != null && await userManager.CheckPasswordAsync(user, loginDto.Password))
    {
        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new System.Security.Claims.ClaimsIdentity(new[] {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            Audience = _configuration["Jwt:Audience"], // Audience
            Issuer = _configuration["Jwt:Issuer"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Results.Ok(new { Token = tokenHandler.WriteToken(token) });
    }
    return Results.Unauthorized();
});
#region CREATION DE LISTE DE LIVRES ET LES RECUPERER
// EndPoint pour recuperer la liste de livres
app.MapGet("/books", [Authorize] async (BookDbContext dbContext) =>
{
    var book = await dbContext.Books.ToListAsync();
    return Results.Ok(book);
});
#endregion

#region CREATION DE ENDPONIT POUR RECUPERER A PARTIR DE L'ID
//app.MapGet("/books/{id}", (int id) => books.FirstOrDefault(book=>book.Id == id));
app.MapGet("/books/{id}", async (int id, BookDbContext dbContext) =>
{
    try
    {
        var book = await dbContext.Books!.FindAsync(id);
        return Results.Ok(book);
    }
    catch (Exception ex)
    {
        return Results.NotFound(new { Message = $"Le book avec l'ID {id} n'existe pas !" });
    }

});
#endregion

#region ENDPOINTE POUR AJOUTER UN NOUVEAU LIVRE

// Endpoint POST pour créer un nouveau livre
app.MapPost("/book", async (CreateBookDTOs newBookDTO, IMapper mapper, BookDbContext dbContext) =>
{
    var newBook = mapper.Map<Book>(newBookDTO);
    dbContext.Books!.Add(newBook);
    await dbContext.SaveChangesAsync();
    var bookResponse = mapper.Map<BookDTOs>(newBook);
    return Results.Created($"/books/{newBook.Id}", bookResponse);

});
  app.MapPost("/books", async (CreateBookDTOs newBookDto, BookDbContext dbContext, IMapper mapper) =>
        {
            var newBook = mapper.Map<Book>(newBookDto);
            dbContext.Books!.Add(newBook);
                await dbContext.SaveChangesAsync();

            var bookResponseDto = mapper.Map<BookDTOs>(newBook);

            return Results.Created($"/books/{newBook.Id}", bookResponseDto);
        });
#endregion

#region ENDPOINTE POUR MODIFIER

app.MapPut("/books/{id}", async (int id, UpdateBookDTOs newBook, BookDbContext dbContext, IMapper mapper) =>
{
    var bookToUpdate = await dbContext.Books!.FindAsync(id);

    if (bookToUpdate is null)
    {
        return Results.NotFound(new { Message = $"le livre de id {id} n'existe pas !" });
    }
    bookToUpdate.Title = newBook.Title ?? bookToUpdate.Title;
    bookToUpdate.Author = newBook.Author ?? bookToUpdate.Author;
    bookToUpdate.PubDate = newBook.PubDate != default ? newBook.PubDate : bookToUpdate.PubDate;
await dbContext.SaveChangesAsync();

    return Results.NoContent();
});
#endregion

#region ENDPOINTs POUR SUPPRIMER

app.MapDelete("/books/{id}", async (int id, BookDbContext dbContext) =>
{
    var book = await dbContext.Books!.FindAsync(id);

    if (book is null)
    {
        return Results.NotFound(new { Message = $"Le book avec l'ID {id} n'existe pas !" });
    }
    dbContext.Books.Remove(book);
    await dbContext.SaveChangesAsync();
    return Results.Ok(book);
});
#endregion

app.Run();











