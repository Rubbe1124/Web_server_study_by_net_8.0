using Microsoft.EntityFrameworkCore;
using Server;
using Server.Models;
using Server.Services;

var builder = WebApplication.CreateBuilder(args);

var settings = new Settings();
builder.Configuration.Bind("Settings", settings);
builder.Services.AddSingleton(settings);

builder.Services.AddDbContext<GameDbContext>(o=>o.UseSqlServer(builder.Configuration.GetConnectionString("Db")));

// 컨트롤러 등록
builder.Services.AddControllers().AddNewtonsoftJson(o =>
{
    //o.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
});

builder.Services.AddScoped<IPlayerService,PlayerService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

// Swagger/OpenAPI 등록

var app = builder.Build();

// 개발 환경에서 Swagger UI 활성화
if (app.Environment.IsDevelopment())
{

}

app.UseHttpsRedirection();
app.UseAuthorization();

// 컨트롤러 라우팅 활성화
app.MapControllers();

app.Run();
