using System.Text.Json.Serialization;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using BookLibrary.API;
using BookLibrary.Application.Services;
using BookLibrary.DataAccessLayer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
}).AddControllersAsServices();

builder.Services.AddHostedService<PostgresService>();

builder.Services.AddDbContext<ILibraryDbContext, LibraryDbContext>((x,y) =>
{
    y.UseNpgsql("User ID=test;Password=test;Host=localhost;Port=5432;Database=book-library;", npOpt =>
    {
        npOpt.EnableRetryOnFailure();
    });
    y.UseSnakeCaseNamingConvention();
});

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory(config =>
{
    config.RegisterModule(new AutofacModule());
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.UseHttpsRedirection();

app.Run();
