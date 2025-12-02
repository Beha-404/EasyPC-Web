using Backend.Data;
using Backend.Interfaces;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DevConnection"));
});
builder.Services.AddScoped<ITokenService,TokenService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var tokenKey = builder.Configuration["TokenKey"] ?? throw new Exception("Token not found");
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<DataContext>();
    
    if (!context.Users.Any())
    {
        var defaultProfilePicturePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "userIcon.png");
        byte[]? defaultProfilePicture = null;
        
        if (File.Exists(defaultProfilePicturePath))
        {
            defaultProfilePicture = await File.ReadAllBytesAsync(defaultProfilePicturePath);
        }

        var adminHmac = new HMACSHA512();
        var adminPassword = "Admin123!";
        var admin = new User
        {
            Username = "admin",
            Password = adminPassword,
            Hash = adminHmac.ComputeHash(Encoding.UTF8.GetBytes(adminPassword)),
            Salt = adminHmac.Key,
            Email = "admin@easypc.com",
            FirstName = "Admin",
            LastName = "User",
            Role = UserRole.Admin,
            profilePicture = defaultProfilePicture
        };

        var userHmac = new HMACSHA512();
        var userPassword = "User123!";
        var user = new User
        {
            Username = "user",
            Password = userPassword,
            Hash = userHmac.ComputeHash(Encoding.UTF8.GetBytes(userPassword)),
            Salt = userHmac.Key,
            Email = "user@easypc.com",
            FirstName = "Regular",
            LastName = "User",
            Role = UserRole.User,
            profilePicture = defaultProfilePicture
        };

        context.Users.AddRange(admin, user);
        await context.SaveChangesAsync();
        
        Console.WriteLine("Seeded default users:");
        Console.WriteLine("Admin - Username: admin, Password: Admin123!");
        Console.WriteLine("User - Username: user, Password: User123!");
    }

    if (!context.Pcs.Any())
    {
        var processors = new List<Processor>
        {
            new Processor { Name = "Intel Core i9-13900K", Socket = "LGA1700", Type = "CPU", Price = 589, CoreCount = 24, ThreadCount = 32 },
            new Processor { Name = "AMD Ryzen 9 7950X", Socket = "AM5", Type = "CPU", Price = 699, CoreCount = 16, ThreadCount = 32 },
            new Processor { Name = "Intel Core i7-13700K", Socket = "LGA1700", Type = "CPU", Price = 419, CoreCount = 16, ThreadCount = 24 },
            new Processor { Name = "AMD Ryzen 7 7700X", Socket = "AM5", Type = "CPU", Price = 399, CoreCount = 8, ThreadCount = 16 },
            new Processor { Name = "Intel Core i5-13600K", Socket = "LGA1700", Type = "CPU", Price = 319, CoreCount = 14, ThreadCount = 20 },
            new Processor { Name = "AMD Ryzen 5 7600X", Socket = "AM5", Type = "CPU", Price = 299, CoreCount = 6, ThreadCount = 12 }
        };
        context.Processors.AddRange(processors);

        var gpus = new List<Graphics_Card>
        {
            new Graphics_Card { Name = "NVIDIA RTX 4090", VRAM = "24GB GDDR6X", Type = "GPU", Price = 1599 },
            new Graphics_Card { Name = "NVIDIA RTX 4080", VRAM = "16GB GDDR6X", Type = "GPU", Price = 1199 },
            new Graphics_Card { Name = "AMD RX 7900 XTX", VRAM = "24GB GDDR6", Type = "GPU", Price = 999 },
            new Graphics_Card { Name = "NVIDIA RTX 4070 Ti", VRAM = "12GB GDDR6X", Type = "GPU", Price = 799 },
            new Graphics_Card { Name = "AMD RX 7800 XT", VRAM = "16GB GDDR6", Type = "GPU", Price = 649 },
            new Graphics_Card { Name = "NVIDIA RTX 4060 Ti", VRAM = "8GB GDDR6", Type = "GPU", Price = 399 }
        };
        context.Graphics_Cards.AddRange(gpus);

        var rams = new List<RAM>
        {
            new RAM { Name = "Corsair Vengeance DDR5 32GB", Type = "RAM", Price = 159, Speed = "6000MHz" },
            new RAM { Name = "G.Skill Trident Z5 RGB 32GB", Type = "RAM", Price = 179, Speed = "6400MHz" },
            new RAM { Name = "Kingston Fury Beast 16GB", Type = "RAM", Price = 89, Speed = "5600MHz" },
            new RAM { Name = "Corsair Dominator Platinum 64GB", Type = "RAM", Price = 319, Speed = "6200MHz" }
        };
        context.RAMs.AddRange(rams);

        var motherboards = new List<Motherboard>
        {
            new Motherboard { Name = "ASUS ROG MAXIMUS Z790", Socket = "LGA1700", Type = "MotherBoard", Price = 589, Model = "ATX", SupportsOverclocking = true },
            new Motherboard { Name = "MSI MPG B650 EDGE", Socket = "AM5", Type = "MotherBoard", Price = 269, Model = "ATX", SupportsOverclocking = true },
            new Motherboard { Name = "Gigabyte Z790 AORUS", Socket = "LGA1700", Type = "MotherBoard", Price = 399, Model = "ATX", SupportsOverclocking = true },
            new Motherboard { Name = "ASRock X670E Taichi", Socket = "AM5", Type = "MotherBoard", Price = 499, Model = "ATX", SupportsOverclocking = true }
        };
        context.Motherboards.AddRange(motherboards);

        var psus = new List<PSU>
        {
            new PSU { Name = "Corsair RM1000x", Power = "1000W", Type = "PSU", Price = 199 },
            new PSU { Name = "EVGA SuperNOVA 850 G6", Power = "850W", Type = "PSU", Price = 149 },
            new PSU { Name = "Seasonic Focus GX-750", Power = "750W", Type = "PSU", Price = 129 },
            new PSU { Name = "be quiet! Dark Power 12", Power = "1200W", Type = "PSU", Price = 299 }
        };
        context.PSUs.AddRange(psus);

        var cases = new List<Case>
        {
            new Case { Name = "NZXT H7 Flow", Type = "CASE", Price = 129, FormFactor = "Mid Tower" },
            new Case { Name = "Corsair 5000D Airflow", Type = "CASE", Price = 164, FormFactor = "Mid Tower" },
            new Case { Name = "Lian Li O11 Dynamic EVO", Type = "CASE", Price = 179, FormFactor = "Mid Tower" },
            new Case { Name = "Fractal Design Torrent", Type = "CASE", Price = 199, FormFactor = "Mid Tower" }
        };
        context.Cases.AddRange(cases);

        await context.SaveChangesAsync();

        var pcs = new List<PC>
        {
            new PC { Name = "Gaming Beast Ultra", ProcessorId = 1, GraphicsCardId = 1, RamId = 2, MotherBoardId = 1, PsuId = 4, CaseId = 3, Available = true, Picture = "images/pc1.png", Price = 3965 },
            new PC { Name = "AMD Powerhouse", ProcessorId = 2, GraphicsCardId = 3, RamId = 4, MotherBoardId = 4, PsuId = 1, CaseId = 2, Available = true, Picture = "images/pc2.png", Price = 2970 },
            new PC { Name = "Intel Elite Pro", ProcessorId = 3, GraphicsCardId = 2, RamId = 1, MotherBoardId = 3, PsuId = 2, CaseId = 4, Available = true, Picture = "images/pc3.png", Price = 2765 },
            new PC { Name = "Budget Gaming Rig", ProcessorId = 6, GraphicsCardId = 6, RamId = 3, MotherBoardId = 2, PsuId = 3, CaseId = 1, Available = true, Picture = "images/pc4.webp", Price = 1245 },
            new PC { Name = "Workstation Pro", ProcessorId = 2, GraphicsCardId = 1, RamId = 4, MotherBoardId = 4, PsuId = 4, CaseId = 2, Available = true, Picture = "images/pc5.webp", Price = 3780 },
            new PC { Name = "Streamer Special", ProcessorId = 1, GraphicsCardId = 4, RamId = 2, MotherBoardId = 1, PsuId = 2, CaseId = 3, Available = true, Picture = "images/pc6.webp", Price = 2505 },
            new PC { Name = "Content Creator", ProcessorId = 4, GraphicsCardId = 5, RamId = 1, MotherBoardId = 2, PsuId = 3, CaseId = 1, Available = true, Picture = "images/pc7.webp", Price = 1885 },
            new PC { Name = "RTX Supreme", ProcessorId = 5, GraphicsCardId = 2, RamId = 3, MotherBoardId = 3, PsuId = 2, CaseId = 4, Available = true, Picture = "images/pc8.webp", Price = 2345 },
            new PC { Name = "AMD Budget Build", ProcessorId = 6, GraphicsCardId = 5, RamId = 3, MotherBoardId = 2, PsuId = 3, CaseId = 1, Available = true, Picture = "images/pc9.png", Price = 1525 },
            new PC { Name = "Ultimate 4K Machine", ProcessorId = 1, GraphicsCardId = 1, RamId = 4, MotherBoardId = 1, PsuId = 4, CaseId = 3, Available = true, Picture = "images/pc10.png", Price = 3965 }
        };
        context.Pcs.AddRange(pcs);
        await context.SaveChangesAsync();

        Console.WriteLine("Seeded 10 PCs with components!");
    }
}

app.UseStaticFiles();

app.UseCors(x => x.AllowAnyHeader()
    .AllowAnyMethod()
    .WithOrigins("http://localhost:4200","https://localhost:4200"));

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
