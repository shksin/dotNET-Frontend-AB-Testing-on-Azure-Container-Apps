using Microsoft.FeatureManagement;
using Microsoft.ApplicationInsights;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);

string config = builder.Configuration.GetValue<string>("AzureAppConfig");
var revisionLabel = builder.Configuration.GetValue<string>("RevisionLabel");

builder.Configuration.AddAzureAppConfiguration(options =>
    options
        .Connect(config)
            .UseFeatureFlags(featureFlagOptions => featureFlagOptions.Label = builder.Configuration.GetValue<string>("RevisionLabel")));

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddFeatureManagement();
builder.Services.AddWebApplicationMonitoring();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Log revision label to Application Insights
var telemetryClient = app.Services.GetRequiredService<TelemetryClient>();
telemetryClient.TrackEvent("RevisionLabel", new Dictionary<string, string> {{ "RevisionLabel", revisionLabel }});

app.Run();

public enum MyFeatureFlags
{
    Beta
}