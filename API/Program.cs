using API.Contracts;
using API.Data;
using API.Models;
using API.Repositories;
using API.Utilities.Handler;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BookingManagementDbContext>(option => option.UseSqlServer(connectionString));

// Add repositories to the container.
// Mendaftarkan AccountRepository sebagai implementasi IAccountRepository.
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountRoleRepository, AccountRoleRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IEducationRepository, EducationRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IUniversityRepository, UniversityRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Mengkonfigurasi layanan untuk pengendalian API (controllers)
builder.Services.AddControllers()
       .ConfigureApiBehaviorOptions(options =>
       {
           // Konfigurasi respons kustom untuk validasi yang gagal
           options.InvalidModelStateResponseFactory = context =>
           {
               // Mengambil semua pesan kesalahan validasi dari ModelState
               var errors = context.ModelState.Values
                                   .SelectMany(v => v.Errors)
                                   .Select(v => v.ErrorMessage);

               // Mengembalikan respons HTTP 400 Bad Request dengan pesan kesalahan validasi
               return new BadRequestObjectResult(new ResponseValidatorHandler(errors));
           };
       });

//Add FluentValidation Service
builder.Services.AddFluentValidationAutoValidation()
    .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());


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