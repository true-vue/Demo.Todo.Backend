using ToDoList.Model;
using Data.Mockup;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.OpenApi.Models;
using ToDoList.Dto;

var builder = WebApplication.CreateBuilder(args);

// Loose CORS policy
builder.Services.AddCors(o => o.AddDefaultPolicy(o => o.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ToDoList application", Version = "v1" });

    // Define the OAuth2.0 scheme that's in use (i.e., Implicit Flow)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add Json web token authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Add authorization services
builder.Services.AddAuthorization();

// Ensure http context accessor will be added to DI.
builder.Services.AddHttpContextAccessor();

// Add UserManager to DI container
builder.Services.AddScoped<IUserManager, UserManager>();

// DATA MOCKUP
var users = new MockupList<User>("Id")
{
    User.New("user1", "Pass123#"),
    User.New("user2", "pass123#"),
    User.New("user3", "pass123#")
};
builder.Services.AddSingleton(users);

var lists = new MockupList<ListItemGroup>("Id") {
    ListItemGroup.New(1, "porządki"),
    ListItemGroup.New(1, "szkoła"),
    ListItemGroup.New(1, "dom"),
};
builder.Services.AddSingleton(lists);

var listItems = new MockupList<ListItem>("Id")
{
    ListItem.New(1,1,"[porządki] Posprzątać kuchnię"),
    ListItem.New(1,1,"[porządki] Wynieść śmieci"),
    ListItem.New(1,1,"[porządki] Zrobić pranie"),
    ListItem.New(1,2,"[szkoła] Odrobić zadanie"),
    ListItem.New(1,2,"[szkoła] Przygotować się do lekcji"),
    ListItem.New(1,2,"[szkoła] Spakować plecak"),
    ListItem.New(1,3,"[dom] Wybrać meble do salonu"),
    ListItem.New(1,3,"[dom] Powiesić obraz"),
    ListItem.New(1,3,"[dom] Naprawić cieknący kran")
};
builder.Services.AddSingleton(listItems);

// build the app.
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// enable http2https redirection
app.UseHttpsRedirection();
// enable CORS policy configured above
app.UseCors();

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();



// API Endpoints 
// *************

// Authentication
app.MapPost("/authenticate", (UserCredentials credentials, MockupList<User> users) =>
{
    var matchedUser = users.FirstOrDefault(u => u.UserName.Equals(credentials.UserName, StringComparison.InvariantCultureIgnoreCase));

    // Validate credentials (this is just an example)
    if (matchedUser?.Password == credentials.Password)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, matchedUser.UserName),
            new Claim(JwtRegisteredClaimNames.Sub, matchedUser.Id.ToString())
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.Now.AddMinutes(30);

        var tokenObject = new JwtSecurityToken(
            issuer: builder.Configuration["Jwt:Issuer"],
            audience: builder.Configuration["Jwt:Audience"],
            claims: claims,
            expires: expiry,
            signingCredentials: creds
        );

        string token = new JwtSecurityTokenHandler().WriteToken(tokenObject);

        return Results.Ok(new AuthResponse()
        {
            Token = token
        });
    }
    return Results.BadRequest();
})
.Produces<AuthResponse>(200)
.ProducesProblem(400)
.WithName("Authenticate")
.WithOpenApi();

// GetItemGroups
app.MapGet("/todo/list-item-groups", (MockupList<ListItemGroup> groupsStore, IUserManager userManager) =>
{
    return groupsStore.Where(g => g.UserId == userManager.User?.Id).Select(g => new ListItemGroupDTO(g));
})
.WithName("GetItemGroups")
.Produces<List<ListItemGroupDTO>>(200)
.WithOpenApi()
.RequireAuthorization();

// GetItemGroupItems
app.MapGet("/todo/list-item-group/items", (int groupId, MockupList<ListItem> todoStore, IUserManager userManager) =>
{
    return todoStore.Where(i => i.UserId == userManager.User?.Id && i.ListItemGroupId == groupId).Select(i => new ListItemDTO(i));
})
.WithName("GetItemGroupItems")
.Produces<List<ListItemDTO>>(200)
.WithOpenApi()
.RequireAuthorization();


// GetAllItems
app.MapGet("/todo/list-item", (MockupList<ListItem> todoStore, IUserManager userManager) =>
{
    return todoStore.Where(i => i.UserId == userManager.User?.Id).Select(i => new ListItemDTO(i));
})
.WithName("GetAllItems")
.Produces<List<ListItemDTO>>(200)
.WithOpenApi()
.RequireAuthorization();

// GetItem
app.MapGet("/todo/list-item/{id}", (int id, MockupList<ListItem> todoStore, IUserManager userManager) =>
{
    var item = todoStore.FirstOrDefault(i => i.Id == id && i.UserId == userManager.User?.Id);

    if (item == null) return Results.NotFound();

    return Results.Ok(new ListItemDTO(item));
})
.WithName("GetItem")
.Produces<ListItemDTO>(200)
.WithOpenApi()
.RequireAuthorization();

// SaveItem
app.MapPost("/todo/list-item", (ListItemDTO itemDTO, MockupList<ListItem> todoStore, IUserManager userManager) =>
{
    var item = itemDTO.ToListItem(userManager.User.Id);
    if (item.Id == 0)
    {
        var isDuplicate = todoStore.Any(i => i.Text.Equals(itemDTO.Text, StringComparison.InvariantCultureIgnoreCase));
        if (isDuplicate)
        {
            return Results.BadRequest(new ApiError
            {
                ErrorCode = ApiErrorCodes.ItemDuplicated
            });
        }
        // add new
        todoStore.Add(item);
        return Results.Created($"/todo/list-item/{item.Id}", new ListItemDTO(item));
    }
    var updateIndex = todoStore.FindIndex(i => i.Id == item.Id);

    if (updateIndex < 0) return Results.NotFound();

    todoStore[updateIndex] = item;

    return Results.Ok(new ListItemDTO(item));
})
.Produces<ListItemDTO>(201)
.Produces<ListItemDTO>(200)
.Produces<ApiError>(400)
.WithName("SaveItem")
.WithOpenApi()
.RequireAuthorization();

// DeleteItem
app.MapDelete("/todo/list-item/{id}", (int id, MockupList<ListItem> todoStore, IUserManager userManager) =>
{
    var deleteIndex = todoStore.FindIndex(i => i.Id == id && i.UserId == userManager.User?.Id);

    if (deleteIndex < 0) return Results.NotFound();

    todoStore.RemoveAt(deleteIndex);

    return Results.Ok();
})
.WithName("DeleteItem")
.WithOpenApi()
.RequireAuthorization();


// Run the appliaction
app.Run();