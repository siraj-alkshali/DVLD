using DVLD.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using DVLD.API.Services;
using DVLD.API.Services.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<DVLDContext>(options =>
{
    var connectionString = builder.Configuration
        .GetConnectionString("DVLDConnection");

    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    );
});

builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IGenderService, GenderService>();
builder.Services.AddScoped<ILicenseClassService, LicenseClassService>();
builder.Services.AddScoped<ITestTypeService, TestTypeService>();

builder.Services.AddControllers();

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
