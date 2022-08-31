using CompanyApp.Context;
using CompanyApp.Repositories;

var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder.Services);

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

// Add services to the container.
void ConfigureServices(IServiceCollection services)
{
    // Controller support
    services.AddControllers();
    
    // Swagger
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
    
    // Our services, interface, or DB Contexts that we want to inject
    services.AddSingleton<DapperContext>();
    services.AddScoped<ICompanyRepository, CompanyRepository>();
}


