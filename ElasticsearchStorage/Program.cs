using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Elasticsearch;
using Consumer;
using Processing;
using Validator;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IReportConsumer, ReportConsumer>();
builder.Services.AddSingleton<IValidating, Validating>();
builder.Services.AddSingleton<Process>();
builder.Services.AddSingleton<IReportStorage, ReportStorage>();

using var host = builder.Build();

var process = host.Services.GetRequiredService<Process>();
var storage = host.Services.GetRequiredService<IReportStorage>();

var reports = process.FilteringReports();

System.Console.WriteLine(reports.Count());

await storage.CreateIndexAsync();

foreach(var report in reports)
{
   await storage.SaveAsync(report);
}

