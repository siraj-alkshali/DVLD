using DVLD.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using DVLD.API.Services;
using DVLD.API.Services.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;


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
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),

            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IGenderService, GenderService>();
builder.Services.AddScoped<ILicenseClassService, LicenseClassService>();
builder.Services.AddScoped<ITestTypeService, TestTypeService>();
builder.Services.AddScoped<IApplicationTypeService, ApplicationTypeService>();
builder.Services.AddScoped<IApplicationStatusService, ApplicationStatusService>();
builder.Services.AddScoped<ILicenseIssueReasonService, LicenseIssueReasonService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<ILocalDrivingLicenseApplicationService, LocalDrivingLicenseApplicationService>();
builder.Services.AddScoped<ITestAppointmentService, TestAppointmentService>();
builder.Services.AddScoped<ITestService, TestService>();
builder.Services.AddScoped<IDriverService, DriverService>();
builder.Services.AddScoped<ILicenseService, LicenseService>();

builder.Services.AddControllers();

builder.Services.AddAuthentication();

builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

var app = builder.Build();

app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
