using Microsoft.EntityFrameworkCore;
using MiniProject.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<MiniProject.Data.ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<MiniProject.Services.IPatientService, MiniProject.Services.PatientService>();
builder.Services.AddScoped<MiniProject.Services.IAppointmentService, MiniProject.Services.AppointmentService>();
builder.Services.AddScoped<MiniProject.Services.ILabOrderService, MiniProject.Services.LabOrderService>();
builder.Services.AddScoped<MiniProject.Services.IReportService, MiniProject.Services.ReportService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseStaticFiles();
app.MapRazorPages();

app.Run();
