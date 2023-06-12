using System.Data;
using System.Data.SqlClient;

using CloverleafTrack.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IDbConnection>(db => new SqlConnection(builder.Configuration.GetConnectionString("CloverleafTrack")));
builder.Services.AddSingleton<IAthleteService, AthleteService>();
builder.Services.AddSingleton<IEventService, EventService>();
builder.Services.AddSingleton<IMeetService, MeetService>();
builder.Services.AddSingleton<IPerformanceService, PerformanceService>();
builder.Services.AddSingleton<ISeasonService, SeasonService>();
builder.Services.AddSingleton<ILeaderboardService, LeaderboardService>();
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
