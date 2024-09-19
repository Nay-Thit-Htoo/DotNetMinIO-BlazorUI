using DoNetMinIO.Api.Service;
using Minio;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IMinioClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var endpoint = config["MinIO:Endpoint"];
    var accessKey = config["MinIO:AccessKey"];
    var secretKey = config["MinIO:SecretKey"];
    var secure = bool.Parse(config["MinIO:Secure"]);

    // Initialize MinIO client
    return new MinioClient()
        .WithEndpoint(endpoint)
        .WithCredentials(accessKey, secretKey)
        .WithSSL(secure)       
        .Build();
});

builder.Services.AddScoped<IMinIoService,MinIoService>();


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
