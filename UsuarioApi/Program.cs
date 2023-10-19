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

//Fazendo a inje��o do JWT:
builder.Services.AddAuthentication(options =>
{
    //Passando algumas options para esse TOKEN:
    //Definindo qual vai ser o schema que o nosso TOKEN ir� utilizar:
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//Fazendo a inje��o do tipo de token que iremos utilizar, que nesse caso ser� o bearer token:
}).AddJwtBearer(options =>
{
    //Passando algumas options para esse bearer token:
    //Definindo quais ser�o os par�metros que ser�o v�lidados para esse token:
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        //1� par�metro: ValidateIssuerSigningKey - validar chave de seguran�a que n�s defimos na hora de criar o token (deve ser igual a que est� l� no TokenService):
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["SymmetricSecurityKey"])),
        //2� par�metro: ValidateAudience - evita ataques de redirecionamento:
        ValidateAudience = false,
        //3� par�metro: ValidateIssuer - forma de seguran�a para trafego do token via web:
        ValidateIssuer = false,
        //4� par�metro: ClockSkew - determina o alinhamento do rel�gio que estamos utilizando para quest�o de valida��o de tempo do token:
        ClockSkew = TimeSpan.Zero
    };
});
//Fazendo a inje��o da autentica��o:
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
