using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using webNet_courses.Domain.Entities;
using webNet_courses.Persistence;

using webNet_courses.API.Middlewear;
using webNet_courses.Abstruct;
using webNet_courses.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CourseContext>(options =>
{
	options.UseNpgsql(builder.Configuration["ConnectionStrings:DefaultConnection"]);
	options.UseLazyLoadingProxies();
});

builder.Services.AddIdentity<User, IdentityRole<Guid>>()
	.AddEntityFrameworkStores<CourseContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
	options.Password.RequireDigit = true;
	options.Password.RequireLowercase = false;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequireUppercase = false;
	options.Password.RequiredLength = 6;

	options.User.RequireUniqueEmail = true;
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = builder.Configuration["JWT:issuer"],
		ValidAudience = builder.Configuration["JWT:audience"],
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:key"]!)),
		SaveSigninToken = true,
	};
});

builder.Services.AddAuthorization();

builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
	options.SuppressMapClientErrors = true;

}).AddJsonOptions(options =>
{
	options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
	options.SwaggerDoc("v1", new OpenApiInfo
	{
		Version = "v1",
		Title = "Courses"
	});
	options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
	{
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		Scheme = "Bearer",
		BearerFormat = "JWT",
		In = ParameterLocation.Header,
		Description = "Jwt Authorization header using the Bearer scheme. \r\n\r\n Enter your token in the next input below"
	});
	options.AddSecurityRequirement(new OpenApiSecurityRequirement {
	{
		new OpenApiSecurityScheme
		{
			Reference = new OpenApiReference
			{
				Type = ReferenceType.SecurityScheme,
				Id = "Bearer"
			}
		},
		new List<string> { }
	}});
	var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));
});

builder.Services.AddExceptionHandler<ExeptionsHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IUserService, UserSevice>();
builder.Services.AddScoped<ICoursesServise, CourseService>();
builder.Services.AddScoped<IReportsService, ReportsService>();

var app = builder.Build();

using var serviceScope = app.Services.CreateScope();
var context = serviceScope.ServiceProvider.GetService<CourseContext>();
context?.Database.Migrate();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseRouting();
app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseAuthorization();

app.MapControllers();

app.Run();
