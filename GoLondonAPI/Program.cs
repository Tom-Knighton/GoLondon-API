using GoLondonAPI.Data;
using GoLondonAPI.Domain.Services;
using GoLondonAPI.Services;
using Microsoft.AspNetCore.Http.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson(o =>
{
    o.SerializerSettings.Converters.Add(new StringEnumConverter());
});

SetupServices(builder);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.IncludeXmlComments(XmlComments.CommentFilePath);
});
builder.Services.AddSwaggerGenNewtonsoftSupport();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseResponseCompression();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

static void SetupServices(WebApplicationBuilder builder)
{

    JsonConvert.DefaultSettings = () => new JsonSerializerSettings
    {
        Converters = new JsonConverter[] { new StringEnumConverter() }
    };

    builder.Services.AddDbContext<DataContext>();

    builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("GoLondon"));

    builder.Services.AddScoped<ISearchService, SearchService>();
    builder.Services.AddScoped<IStopPointService, StopPointService>();
    builder.Services.AddScoped<ILineService, LineService>();
    builder.Services.AddScoped<IMetaService, MetaService>();
    builder.Services.AddScoped<IJourneyService, JourneyService>();
    builder.Services.AddScoped<IVehicleService, VehicleService>();

    builder.Services.AddScoped<IUserService, UserService>();

    builder.Services.AddTransient<IAPIClient, APIClient>();

    builder.Services.AddResponseCompression(opt =>
    {
        opt.EnableForHttps = true;
        opt.Providers.Add<BrotliCompressionProvider>();
        opt.Providers.Add<GzipCompressionProvider>();
    });

    builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
    {
        options.Level = System.IO.Compression.CompressionLevel.Optimal;
    });

    builder.Services.AddHealthChecks();
}
