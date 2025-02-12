using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GestionBibliotheque.Blazor;
using GestionBibliotheque.Blazor.Services;
using GestionBibliotheque.Blazor.Services.Auth;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization; // Ajoutez cette ligne

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5143") });
// Ajouter Blazored.LocalStorage
builder.Services.AddBlazoredLocalStorage();

// Ajouter l'authentification
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

await builder.Build().RunAsync();
