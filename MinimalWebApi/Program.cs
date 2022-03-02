using MinimalWebApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ItemsDb>(opt => opt.UseInMemoryDatabase("Items"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

app.MapGet("/items", async (ItemsDb db) =>
    await db.Items.Select(i => new ItemDTO(i)).ToListAsync());

app.MapGet("/items/{id}", async (int id, ItemsDb db) =>
    await db.Items.FindAsync(id) 
        is Item item 
            ? Results.Ok(new ItemDTO(item)) 
            : Results.NotFound());

app.MapPost("/items", async (ItemDTO itemDTO, ItemsDb db) =>
{
    var item = new Item
    {
        Name = itemDTO.Name,
    };

    db.Items.Add(item);
    await db.SaveChangesAsync();

    return Results.Created($"Created item {item.Name}", new ItemDTO(item));
});

app.MapPut("/items/{id}", async (int id, ItemDTO itemDTO, ItemsDb db) =>
{
    var item = await db.Items.FindAsync(id);
    if (item is null) return Results.NotFound();

    item.Name = itemDTO.Name;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/items/{id}", async (int id, ItemsDb db) =>
{
    if (await db.Items.FindAsync(id) is Item item)
    {
        db.Items.Remove(item);
        await db.SaveChangesAsync();
        return Results.Ok(new ItemDTO(item));
    }

    return Results.NotFound();
});

app.Run();