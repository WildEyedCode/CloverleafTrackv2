using System.Data;
using System.Data.SqlClient;

using CloverleafTrack.Options;
using CloverleafTrack.Services;

using AdminServices = CloverleafTrack.Areas.Admin.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<CloverleafTrackOptions>(builder.Configuration.GetSection(CloverleafTrackOptions.CloverleafTrack));

builder.Services.AddTransient<IDbConnection>(db => new SqlConnection(builder.Configuration.GetConnectionString("CloverleafTrack")));
builder.Services.AddSingleton<IAthleteService, AthleteService>();
builder.Services.AddSingleton<IEventService, EventService>();
builder.Services.AddSingleton<IMeetService, MeetService>();
builder.Services.AddSingleton<IPerformanceService, PerformanceService>();
builder.Services.AddSingleton<ISeasonService, SeasonService>();
builder.Services.AddSingleton<ILeaderboardService, LeaderboardService>();

builder.Services.AddSingleton<AdminServices.IAthleteService, AdminServices.AthleteService>();
builder.Services.AddSingleton<AdminServices.IEventService, AdminServices.EventService>();
builder.Services.AddSingleton<AdminServices.IPerformanceService, AdminServices.PerformanceService>();
builder.Services.AddSingleton<AdminServices.IMeetService, AdminServices.MeetService>();
builder.Services.AddSingleton<AdminServices.ISeasonService, AdminServices.SeasonService>();

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

#pragma warning disable ASP0014
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    
    endpoints.MapAreaControllerRoute(
        "default", 
        "Admin", 
        "{area:exists}/{controller=Home}/{action=Index}/{id?}");
});
#pragma warning restore ASP0014

app.Run();
