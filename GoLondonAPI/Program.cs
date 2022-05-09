using GoLondonAPI.Data;
using GoLondonAPI.Domain.Services;
using GoLondonAPI.Services;
using Microsoft.AspNetCore.Http.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Microsoft.AspNetCore.ResponseCompression;
using AspNetCoreRateLimit;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson(o =>
{
    o.SerializerSettings.Converters.Add(new StringEnumConverter());
    o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
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

app.UseClientRateLimiting();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

using (var scope = app.Services.CreateScope())
{
    var clientPolicyStore = scope.ServiceProvider.GetRequiredService<IClientPolicyStore>();
    var metaService = scope.ServiceProvider.GetRequiredService<IMetaService>();
    await clientPolicyStore.SeedAsync();
    await metaService.SyncLimits();
}

app.Run();

static void SetupServices(WebApplicationBuilder builder)
{

    JsonConvert.DefaultSettings = () => new JsonSerializerSettings
    {
        Converters = new JsonConverter[] { new StringEnumConverter() }
    };

    builder.Services.AddDbContext<DataContext>();

    builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("GoLondon"));

    builder.Services.AddOptions();
    builder.Services.AddMemoryCache();
    builder.Services.Configure<ClientRateLimitOptions>(builder.Configuration.GetSection("ClientRateLimiting"));
    builder.Services.Configure<ClientRateLimitPolicies>(builder.Configuration.GetSection("ClientRateLimitPolicies"));
    builder.Services.AddInMemoryRateLimiting();

    builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IProjectService, ProjectService>();

    builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
    builder.Services.AddScoped<ISearchService, SearchService>();
    builder.Services.AddScoped<IStopPointService, StopPointService>();
    builder.Services.AddScoped<ILineService, LineService>();
    builder.Services.AddScoped<IMetaService, MetaService>();
    builder.Services.AddScoped<IJourneyService, JourneyService>();
    builder.Services.AddScoped<IVehicleService, VehicleService>();


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

    builder.Services.AddAuthentication()
        .AddJwtBearer(o =>
        {
            o.SaveToken = true;
            o.RequireHttpsMetadata = true;
            o.TokenValidationParameters = new()
            {
                ValidIssuer = builder.Configuration["GoLondon:Audience"],
                ValidAudience = builder.Configuration["GoLondon:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["GoLondon:Secret"])),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });
}
