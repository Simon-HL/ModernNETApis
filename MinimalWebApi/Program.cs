using MinimalWebApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ItemsDb>(opt => opt.UseInMemoryDatabase("Items"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/items", async (ItemsDb db) =>
    await db.Items.Select(i => new ItemDTO(i)).ToListAsync()
    )
    .Produces<List<ItemDTO>>(StatusCodes.Status200OK)
    .WithName("GetAllItems")
    .WithTags("Getters");

app.MapGet("/items/{id}", async (int id, ItemsDb db) =>
    await db.Items.FindAsync(id) 
        is Item item 
            ? Results.Ok(new ItemDTO(item)) 
            : Results.NotFound()
     )
    .Produces<ItemDTO>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("GetItem")
    .WithTags("Getters");

app.MapPost("/items", async (ItemDTO itemDTO, ItemsDb db) =>
{
    var item = new Item
    {
        Name = itemDTO.Name,
    };

    db.Items.Add(item);
    await db.SaveChangesAsync();

    return Results.Created($"Created item {item.Name}", new ItemDTO(item));
})
.Accepts<ItemDTO>("application/json")
.Produces<ItemDTO>(StatusCodes.Status201Created)
.WithName("CreateItem")
.WithTags("Creators");

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