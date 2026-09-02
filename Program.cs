using Microsoft.EntityFrameworkCore;
using WhatsAppMessenger.Data;
using WhatsAppMessenger.Services;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MessengerDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configurar servicio de encriptación
var encryptionKey = builder.Configuration["EncryptionSettings:EncryptionKey"];
builder.Services.AddSingleton<IEncryptionService>(new EncryptionService(encryptionKey));

// Configurar servicio Messenger
builder.Services.AddScoped<IMessengerService, MessengerService>();

// Agregar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configurar middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Aplicar migraciones automáticamente al iniciar
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MessengerDbContext>();
    dbContext.Database.Migrate();
}

app.Run();
