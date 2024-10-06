using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PersonalizacionProyectoGradoWASM;
using PersonalizacionProyectoGradoWASM.Helpers;
using PersonalizacionProyectoGradoWASM.Servicios.IServicios;
using PersonalizacionProyectoGradoWASM.Servicios;
using OfficeOpenXml;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<IPedidosServicio, PedidosServicio>();
builder.Services.AddScoped<IAccesoriosServicio,AccesoriosServicio>();
builder.Services.AddScoped<IUsuariosServicio, UsuariosServicio>();
builder.Services.AddScoped<IServicioAutenticacion, ServicioAutenticacion>();

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

/// Servicios
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();


// Agregar esta línea para configurar el HttpClient con la URL base de la API
builder.Services.AddHttpClient("API", client => client.BaseAddress = new Uri(Inicializar.UrlBaseApi));
await builder.Build().RunAsync();
