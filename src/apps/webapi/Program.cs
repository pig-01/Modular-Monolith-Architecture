using Serilog;
using User.Application;
using User.Application.Endpoints;
using Order.Application;
using Order.Application.Endpoints;
using Product.Application;
using Product.Application.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfiguration) =>
    loggerConfiguration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddUserModule(builder.Configuration);
builder.Services.AddOrderModule(builder.Configuration);
builder.Services.AddProductModule(builder.Configuration);

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();

app.MapUserEndpoints();
app.MapOrderEndpoints();
app.MapProductEndpoints();

app.Run();
