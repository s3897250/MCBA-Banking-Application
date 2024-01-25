using Microsoft.EntityFrameworkCore;
using MCBAAdminAPI.Data;
using MCBAAdminAPI.Models.Repository;
using MCBAAdminAPI.Models.DataManager;
using System.Text.Json.Serialization;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<MCBAAdminContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MCBAAdminContext")));


builder.Services.AddScoped<ICustomerRepository, CustomerManager>();
builder.Services.AddScoped<ILoginRepository, LoginManager>();


// Ignore JSON reference cycles during serialisation.
builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();



app.Run();
