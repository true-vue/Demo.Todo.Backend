using ToDoList.Model;
using Data.Mockup;

var builder = WebApplication.CreateBuilder(args);

// Loose CORS policy
builder.Services.AddCors(o => o.AddDefaultPolicy(o => o.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(new MockupList<ListItem>("Id"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
// app.UseCors();

app.MapGet("/todo/list-item", (MockupList<ListItem> todoStore) =>
{
    return todoStore;
})
.WithName("GetAllItems")
.WithOpenApi();

app.MapGet("/todo/list-item/{id}", (int id, MockupList<ListItem> todoStore) =>
{
    var item = todoStore.FirstOrDefault(i => i.Id == id);

    if (item == null) return Results.NotFound();

    return Results.Ok(item);
})
.WithName("GetItem")
.WithOpenApi();

app.MapPost("/todo/list-item", (ListItem item, MockupList<ListItem> todoStore) =>
{
    if (item.Id == 0)
    {
        // add new
        todoStore.Add(item);
        return Results.Created($"/todo/list-item/{item.Id}", item);
    }
    var updateIndex = todoStore.FindIndex(i => i.Id == item.Id);

    if (updateIndex < 0) return Results.NotFound();

    todoStore[updateIndex] = item;

    return Results.Ok(item);
})
.WithName("SaveItem")
.WithOpenApi();

app.Run();