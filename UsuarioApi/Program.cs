using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UsuarioApi.Authorization;
using UsuarioApi.Data;
using UsuarioApi.Models;
using UsuarioApi.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration["ConnectionStrings:UsuarioConnection"];

// Add services to the container.
builder.Services.AddDbContext<UsuarioDbContext>(opts =>
{
    opts.UseSqlServer(connectionString);
});
builder.Services
    .AddIdentity<Usuario, IdentityRole>()
    .AddEntityFrameworkStores<UsuarioDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Fazendo a injeção do JWT:
builder.Services.AddAuthentication(options =>
{
    //Passando algumas options para esse TOKEN:
    //Definindo qual vai ser o schema que o nosso TOKEN irá utilizar:
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//Fazendo a injeção do tipo de token que iremos utilizar, que nesse caso será o bearer token:
}).AddJwtBearer(options =>
{
    //Passando algumas options para esse bearer token:
    //Definindo quais serão os parâmetros que serão válidados para esse token:
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        //1º parâmetro: ValidateIssuerSigningKey - validar chave de segurança que nós defimos na hora de criar o token (deve ser igual a que está lá no TokenService):
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["SymmetricSecurityKey"])),
        //2º parâmetro: ValidateAudience - evita ataques de redirecionamento:
        ValidateAudience = false,
        //3º parâmetro: ValidateIssuer - forma de segurança para trafego do token via web:
        ValidateIssuer = false,
        //4º parâmetro: ClockSkew - determina o alinhamento do relógio que estamos utilizando para questão de validação de tempo do token:
        ClockSkew = TimeSpan.Zero
    };
});
//Fazendo a injeção da autenticação:
builder.Services.AddSingleton<IAuthorizationHandler, IdadeAuthorization>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("IdadeMinima", policy =>
    {
        policy.AddRequirements(new IdadeMinima(18));
    });
});

builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<TokenService>();

var app = builder.Build();

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
