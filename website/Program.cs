using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using website.Context;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using NSwag;
using NSwag.Generation.AspNetCore;
using Microsoft.OpenApi.Models;
using System.Text;
using website.Services;
using Microsoft.IdentityModel.Logging;

var builder = WebApplication.CreateBuilder(args);

// The dynamic variable is to configure CORS
var allowedOrigin = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

builder.Services.AddControllersWithViews(options =>
{
    options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
})
.AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

builder.Services.AddScoped<IAuthService, AuthService>();

// Config DbContext to connect to Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Config CORS to VueJs call API
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", builder =>
    {
        builder.WithOrigins(allowedOrigin)
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

//Config JWT to make authenication API
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.Zero,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:SecretKey"])),
            };
        });

builder.Services.AddAuthorization();

// Configure NSwag for Swagger documentation
builder.Services.AddSwaggerDocument(config =>
{
    config.DocumentName = "v1";
    config.PostProcess = document =>
    {
        document.Info.Title = "API Manage";
        document.Info.Version = "v1";
        document.Info.Contact = new NSwag.OpenApiContact
        {
            Name = "PhamTienDat",
            Email = "Phamtiendat246@gmail.com"
        };
    };
});

IdentityModelEventSource.ShowPII = true; 
IdentityModelEventSource.LogCompleteSecurityArtifact = true;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Add NSwag middleware to serve Swagger
app.UseOpenApi(config =>
{
    config.Path = "/swagger/v1/swagger.json";
});

IApplicationBuilder applicationBuilder = app.UseSwagger(config =>
{
    config.Path = """/swagger/v1/swagger.json""";
});

app.UseSwaggerUi3(config =>
{
    config.Path = "/api";
    config.DocumentPath = "/swagger/v1/swagger.json";
});

app.UseHttpsRedirection();
app.UseStaticFiles();

// Enable CORS (Cross-Origin Resource Sharing)
app.UseCors("MyPolicy");

app.UseRouting();

//Authenication && Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
