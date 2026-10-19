using BeeMart.WebClient.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Session holds the JWT + logged-in user + cart (WebClient state management).
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(o =>
{
    o.IdleTimeout = TimeSpan.FromHours(8);
    o.Cookie.HttpOnly = true;
    o.Cookie.IsEssential = true;
});

// Typed API client — the ONLY way the WebClient reaches data (no direct DB).
builder.Services.AddHttpClient<BeeMartApiClient>(c =>
{
    c.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]!);
});

var app = builder.Build();

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

app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
