using System.Data;
using System.Data.SqlClient;

using CloverleafTrack.Managers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IDbConnection>(db => new SqlConnection(builder.Configuration.GetConnectionString("CloverleafTrack")));
builder.Services.AddTransient<IAthleteManager, AthleteManager>();
builder.Services.AddTransient<IEventManager, EventManager>();
builder.Services.AddTransient<IMeetManager, MeetManager>();
builder.Services.AddTransient<IPerformanceManager, PerformanceManager>();
builder.Services.AddTransient<ISeasonManager, SeasonManager>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
