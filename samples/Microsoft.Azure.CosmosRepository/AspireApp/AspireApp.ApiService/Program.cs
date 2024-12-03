var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

builder.AddCosmosRepositoryClient("cosmos-repository");

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.UseSwagger();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.MapTodoApi();
app.MapDefaultEndpoints();

app.Run();
