using Autopart.Api.Extensions;
using Autopart.API.Extensions;
using Autopart.Data.Extensions;
using Microsoft.OpenApi.Models;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder => builder
            .WithOrigins("https://www.easypartshub.com", "https://admin.easypartshub.com", "http://localhost:3003", "http://localhost:3002", "http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

builder.Services.AddControllers();
builder.Services.AddDatabase();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Autopart.API ", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});
builder.Services.AddApplicationServices();
builder.Services.AddAppOptions(configuration);
builder.Services.AddAuth(configuration);
builder.Services.AddCustomApi();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.MappingExtension();
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = configuration["Jwt:Issuer"],
//        ValidAudience = configuration["Jwt:Audience"],
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
//    };
//})
//.AddGoogle(options =>
//{
//    options.ClientId = configuration["Google:ClientId"];
//    options.ClientSecret = configuration["Google:ClientSecret"];
//});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{

//}

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigins");
app.UseMiddleware<JwtBlacklistMiddleware>();
//app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();
app.Services.Seed();
app.Run();

