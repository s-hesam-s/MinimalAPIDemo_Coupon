using MagicVilla_CouponAPI;
using MagicVilla_CouponAPI.Data;
using FluentValidation;
using System.Net;
using Microsoft.EntityFrameworkCore;
using MagicVilla_CouponAPI.Repository;
using MagicVilla_CouponAPI.Repository.IRepository;
using MagicVilla_CouponAPI.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICouponRepository, CouponRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

builder.Services.AddDbContext<ApplicationDbContext>(option =>
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(typeof(MappingConfig));

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseAuthentication();
//app.UseAuthorization();

app.ConfigureCouponEndpoints();
app.ConfigureAuthEndpoints();

app.UseHttpsRedirection();

app.Run();
