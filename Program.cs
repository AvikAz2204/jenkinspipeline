using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using Prometheus;
using App.Metrics;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddMetrics();

var metrics = AppMetrics.CreateDefaultBuilder().Build();



builder.Services.AddMetrics(metrics);
builder.Services.AddMetricsTrackingMiddleware();

var snapshot = metrics.Snapshot.Get();

foreach (var formatter in metrics.OutputMetricsFormatters)
{
    using (var stream = new MemoryStream())
    {
        await formatter.WriteAsync(stream, snapshot);

        var result = Encoding.UTF8.GetString(stream.ToArray());

        System.Console.WriteLine(result);
    }
}

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

//app.UseMetricsApdexTrackingMiddleware();
//app.UseMetricsRequestTrackingMiddleware();
//app.UseMetricsErrorTrackingMiddleware();
//app.UseMetricsActiveRequestMiddleware();
//app.UseMetricsPostAndPutSizeTrackingMiddleware();
//app.UseMetricsOAuth2TrackingMiddleware();

app.UseHttpMetrics();
app.UseMetricsTextEndpoint();
app.UseMetricsAllMiddleware();
app.UseMetricServer();


app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");




app.Run();
