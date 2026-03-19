using Microsoft.EntityFrameworkCore;
using tool_system.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Prepara os controladores
builder.Services.AddControllers();

// 2. Adiciona os serviços do Swagger (A "máquina de catálogo")
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. Configura a conexão com o banco de dados
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();

// 4. Ativa a tela visual do Swagger apenas em ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();