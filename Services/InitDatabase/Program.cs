using MainData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

//Add DbContext
builder.Services.AddDbContext<DatabaseContext>(options =>
{
    var connectString = "Server=ffms-server.database.windows.net;uid=ffms-admin;pwd=password123@;database=FFMS;TrustServerCertificate=True";
    options.UseSqlServer(connectString,
        b =>
        {
            b.MigrationsAssembly("InitDatabase");
            b.CommandTimeout(1200);
        }
    );
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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