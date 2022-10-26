using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using SocialBrothersCase.Data;

var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddDbContext<SocialBrothersCaseContext>(options =>
//   options.UseSqlServer(builder.Configuration.GetConnectionString("SocialBrothersCaseContext") ?? throw new InvalidOperationException("Connection string 'SocialBrothersCaseContext' not found.")));
builder.Services.AddDbContext<SocialBrothersCaseContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("SocialBrothersCaseContext") ?? throw new InvalidOperationException("Connection string 'SocialBrothersCaseContext' not found.")));
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.EnableAnnotations();
    //c.SwaggerDoc("Social Brothers Case",new Swashbuckle.AspNetCore.Swagger.)
});
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
