using MeasurementPublisherAPI.Context;
using MeasurementPublisherAPI.Services;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddScoped<IMessageProducer, MessageProducer>();
//builder.Services.AddScoped<IMessageConsumer, MessageConsumer>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:7205/") });

builder.Services.AddHostedService<MessageConsumer>();
builder.Services.AddHostedService<DeviceConsumer>();

builder.Services.AddDbContext<MeasurementContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(
    op => {
        op.AddPolicy("AllowAll", p =>
        {
            p.WithOrigins("http://localhost/frontend", "http://localhost/deviceapi", "http://localhost/userapi");
            p.AllowAnyHeader();
            p.AllowAnyMethod();
            p.AllowAnyOrigin();
        });
    });


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
