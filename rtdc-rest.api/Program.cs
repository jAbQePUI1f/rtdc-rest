using rtdc_rest.api.BackgroundServices;
using rtdc_rest.api.Services.Abstract;
using rtdc_rest.api.Services.Concrete;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddHostedService<ClCardSyncJob>();
//builder.Services.AddHostedService<StockFlSyncJob>();
builder.Services.AddHostedService<StockLvSyncJob>();
builder.Services.AddScoped<IClCardService,ClCardManager>();
builder.Services.AddScoped<IStockFlService, StockFlManager>();
builder.Services.AddScoped<IStockLvService, StockLvManager>();

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
