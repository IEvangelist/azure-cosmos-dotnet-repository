using Aspire.Microsoft.Azure.CosmosRepository;
using AspireSample.ApiService.Endpoints;
using AspireSample.ApiService.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add cosmos repository
builder.AddAzureCosmosRepository("cosmos")
    .AddItemConfiguration<EmployeeItem, EmployeeItemConfiguration>();

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.MapGet(
    "/",
    context =>
    {
        context.Response.Redirect("/swagger");
        return Task.CompletedTask;
    });

app.UseSwagger();
app.UseSwaggerUI();
app.MapEmployeeEndpoints();
app.MapDefaultEndpoints();

app.Run();