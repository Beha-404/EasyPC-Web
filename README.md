# EasyPC - Build Your Dream PC

EasyPC je web aplikacija za izgradnju i kupovinu računara. Korisnici mogu pregledati gotove konfiguracije ili izgraditi sopstveni PC odabirom komponenti.

![Home Page](screenshots/home.png)

## 🚀 Funkcionalnosti

### 🖥️ Pregled Računara
- Pregledajte predefinisane PC konfiguracije sa detaljnim specifikacijama
- Filtriranje po kategorijama, komponentama i cijeni
- Prikaz cijena i dostupnosti

![PC Details](screenshots/details.png)

### 🔧 Build Your Own PC
- Izaberite procesor, grafičku, RAM, matičnu ploču, napajanje i kućište
- Real-time kalkulacija cijene

![Admin Panel](screenshots/admin.png)

### 👤 Korisnički Profil
- Registracija i login
- Uređivanje profila i upload slike

![Edit Profile](screenshots/profile.png)

### 💬 Support Center
- Live chat sa admin timom

![Support](screenshots/support.png)

## 🛠️ Tehnologije

**Backend:** .NET 9, Entity Framework Core, SQL Server, JWT Authentication  
**Frontend:** Angular 19, TypeScript, Bootstrap, NGX Translate

## 📦 Instalacija

### Backend
```bash
cd backend
dotnet ef database update
dotnet run
```

### Frontend
```bash
cd frontend
npm install
ng serve
```

## 🗄️ Seed Podaci

**Admin:** `admin` / `Admin123!`  
**User:** `user` / `User123!`

10 PC konfiguracija sa komponentama automatski se kreira pri prvom pokretanju.

## 📝 Licenca

MIT License

---

⭐ Stavite zvjezdicu ako vam se sviđa projekat!