using Elastic.Clients.Elasticsearch;
using Pipeline;
var builder = WebApplication.CreateBuilder(args);

var elasticsearchUri = builder.Configuration["Elasticsearch:Uri"];

var settings = new ElasticsearchClientSettings(new Uri(elasticsearchUri!));

var client = new ElasticsearchClient(settings);

builder.Services.AddSingleton(client);
builder.Services.AddScoped<IElasticsearchPipline, ElasticsearchPipline>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
