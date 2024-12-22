using INFL.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// ����� ����� ������� �� appsettings.json
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// ����� ������� ������ �� Infrastructure
builder.Services.AddInfrastructureServices(connectionString, builder.Configuration);

//builder.Services.AddSingleton<IWebHostEnvironment>(env => env);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin() // ������ ����� �������
              .AllowAnyMethod() // ������ ����� ����� (GET, POST, PUT, DELETE, ���)
              .AllowAnyHeader(); // ������ ����� ������
    });
});
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    /*   app.UseCors(policy =>
       {
           policy.WithOrigins("https://localhost:7191").AllowAnyMethod().AllowAnyHeader().WithHeaders(HeaderNames.ContentType);
       });*/
    /* app.UseCors(policy =>
     {
         policy.AllowAnyOrigin() // ������ ������� �� �� ����
               .AllowAnyMethod() // ������ ��� ��� �� ������� (GET, POST, PUT, DELETE, ...)
               .AllowAnyHeader(); // ������ ��� ��� (Header)
     });*/

}
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthorization();

app.MapControllers();

app.Run();
