using MVC.Interfaces; 
using MVC.Repositorios; 
using MVC.Services; // ❗ Nuevo using

var builder = WebApplication.CreateBuilder(args);

// Habilitar servicios de sesiones
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de expiración de la sesión
    options.Cookie.HttpOnly = true; // Solo accesible desde HTTP, no JavaScript
    options.Cookie.IsEssential = true; // Necesario incluso si el usuario no acepta cookies
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserRepository, UsuarioRepository>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IPresupuestoRepository, PresupuestoRepository>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Usar sesiones
app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
