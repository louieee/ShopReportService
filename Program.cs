using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ReportApp.Data;
using ReportApp.Data.Services;
using ReportService.Helpers;
using ReportService.Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllHeaders",
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration["Jwt:Secret"] ?? throw new InvalidOperationException("No JWT Secret"))),
            ValidIssuer = builder.Configuration["Jwt:ValidIssuer"],
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true
        };
    });

builder.Services.AddControllers();
var connString = builder.Configuration.GetConnectionString("ReportDB");
builder.Services.AddDbContext<DataContext>(options =>
    options.UseNpgsql(connString));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<RabbitMQService>();
// SWAGGER CONFIG

var contact = new OpenApiContact()
{
    Name = "Report Service",
    Email = "info@reportservice.org",
    Url = new Uri("http://reportservice.org")
};

var license = new OpenApiLicense()
{
    Name = "Report Service License",
    Url = new Uri("http://reportservice.org/license")
};

var info = new OpenApiInfo()
{
    Version = "v1",
    Title = "Report Service API",
    Description = "Documentation of all APIs in the Report Service Project",
    TermsOfService = new Uri("http://reportservice.org/terms"),
    Contact = contact,
    License = license
};

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", info);
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        Array.Empty<string>()
                    }
                });
    // c.IncludeXmlComments("report-service-doc.xml");
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.ConfigureCustomExceptionMiddleware();

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAllHeaders");
app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint(url: "v1/swagger.json", name: "Report Service API v1");
});
app.MapControllers();            

app.MapControllerRoute(
    name: "swagger",
    pattern: "{controller=Swagger}/{action=Index}/{id?}"
);                                     

app.MapHub<MessageHub>("/messageHub");  
app.MapHub<RideHub>("/rideHub");

var rabbitMqService = app.Services.GetRequiredService<RabbitMQService>();
rabbitMqService.StartListening();
app.Run();