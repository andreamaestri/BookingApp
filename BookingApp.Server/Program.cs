using BookingApp.Server.Data;
using BookingApp.Server.Repositories;
using Microsoft.EntityFrameworkCore;
using Asp.Versioning;

var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();

// Add API explorer and Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add authorization services
builder.Services.AddAuthorization();

// Add API versioning (minimal configuration for development)
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IAccommodationRepository, AccommodationRepository>();
builder.Services.AddScoped<IGuestRepository, GuestRepository>();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder => builder
            .WithOrigins("https://localhost:61872")
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthorization();
app.MapControllers();

app.Run();
