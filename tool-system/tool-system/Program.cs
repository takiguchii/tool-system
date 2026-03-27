using Microsoft.EntityFrameworkCore;
using tool_system.Data;
using Microsoft.IdentityModel.Tokens; 
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Cole o seu Token JWT aqui. Exemplo: Bearer {seu_token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});

var chaveSecreta = builder.Configuration.GetValue<string>("Jwt:Chave");
var chaveEmBytes = Encoding.ASCII.GetBytes(chaveSecreta!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(chaveEmBytes),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddScoped<ToolingSystem.API.Services.MoldeService>();
builder.Services.AddScoped<ToolingSystem.API.Services.MachoService>();

var app = builder.Build();
var pastaImagens = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagens");
Directory.CreateDirectory(Path.Combine(pastaImagens, "moldes"));
Directory.CreateDirectory(Path.Combine(pastaImagens, "machos"));

app.UseCors(options => options
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();
app.UseAuthentication(); 
app.UseAuthorization();  

app.MapControllers();

app.Run();