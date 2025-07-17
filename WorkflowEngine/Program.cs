using WorkflowEngine.Models;
using WorkflowEngine.Services;
using System.Linq; // For LINQ queries like SingleOrDefault, FirstOrDefault
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Workflow Engine API", Version = "v1" });
});

builder.Services.AddSingleton<WorkflowService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var workflowService = app.Services.GetRequiredService<WorkflowService>();

// Define a new workflow
app.MapPost("/workflows/definitions", (WorkflowDefinition definition) =>
{
    var (createdDefinition, error) = workflowService.CreateWorkflowDefinition(definition);
    if (createdDefinition == null)
    {
        return Results.BadRequest(new { error });
    }
    return Results.Created($"/workflows/definitions/{createdDefinition.Id}", createdDefinition);
})
.WithName("CreateWorkflowDefinition")
.WithOpenApi();

// Get a specific workflow definition
app.MapGet("/workflows/definitions/{definitionId}", (string definitionId) =>
{
    var definition = workflowService.GetWorkflowDefinition(definitionId);
    if (definition == null)
    {
        return Results.NotFound($"Workflow Definition with ID '{definitionId}' not found.");
    }
    return Results.Ok(definition);
})
.WithName("GetWorkflowDefinition")
.WithOpenApi();

// List all workflow definitions
app.MapGet("/workflows/definitions", () =>
{
    var definitions = workflowService.GetAllWorkflowDefinitions();
    return Results.Ok(definitions);
})
.WithName("GetAllWorkflowDefinitions")
.WithOpenApi();

// Start a new workflow instance
app.MapPost("/workflows/instances", (string definitionId) =>
{
    var (instance, error) = workflowService.StartWorkflowInstance(definitionId);
    if (instance == null)
    {
        return Results.BadRequest(new { error });
    }
    return Results.Created($"/workflows/instances/{instance.Id}", instance);
})
.WithName("StartWorkflowInstance")
.WithOpenApi();

// Execute an action on a workflow instance
app.MapPost("/workflows/instances/{instanceId}/execute/{actionId}", (string instanceId, string actionId) =>
{
    var (updatedInstance, error) = workflowService.ExecuteAction(instanceId, actionId);
    if (updatedInstance == null)
    {
        return Results.BadRequest(new { error });
    }
    return Results.Ok(updatedInstance);
})
.WithName("ExecuteWorkflowAction")
.WithOpenApi();

// Get a specific workflow instance's current state and history
app.MapGet("/workflows/instances/{instanceId}", (string instanceId) =>
{
    var instance = workflowService.GetWorkflowInstance(instanceId);
    if (instance == null)
    {
        return Results.NotFound($"Workflow Instance with ID '{instanceId}' not found.");
    }
    return Results.Ok(instance);
})
.WithName("GetWorkflowInstance")
.WithOpenApi();

app.MapGet("/workflows/instances", () =>
{
    var instances = workflowService.GetAllWorkflowInstances();
    return Results.Ok(instances);
})
.WithName("GetAllWorkflowInstances")
.WithOpenApi();


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
