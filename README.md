🚀 Friendzone Backend Sistemi
<p align="center"> <img src="https://img.shields.io/badge/.NET-8-purple" /> <img src="https://img.shields.io/badge/PostgreSQL-Database-blue" /> <img src="https://img.shields.io/badge/Architecture-Layered-success" /> <img src="https://img.shields.io/badge/Status-Active-brightgreen" /> </p>

Friendzone Backend, .NET 8 ve PostgreSQL kullanılarak geliştirilmiş lokasyon tabanlı bir sosyal eşleşme REST API sistemidir.

📍 Kullanıcıların konum bilgilerini paylaşmasını
🔍 Yakın kullanıcıları keşfetmesini
🤝 Davet tabanlı etkileşim kurmasını

sağlayan modern bir backend mimarisi üzerine kurulmuştur.

🎯 Proje Amacı

Bu projenin temel amacı modern backend prensipleri kullanılarak güvenli, ölçeklenebilir ve performanslı bir REST API geliştirmektir.

Projenin hedefleri:

🔐 Kullanıcı kimlik doğrulama sisteminin güvenli şekilde uygulanması
📍 Kullanıcı konum bilgilerinin saklanması
🟢 Aktif kullanıcıların belirlenmesi
📏 Coğrafi mesafe hesaplamaları ile yakın kullanıcıların bulunması
📨 Davet mekanizmasının yönetilmesi
🧠 Veri tutarlılığının transaction ve constraint ile sağlanması
🧰 Kullanılan Teknolojiler
🚀 Teknoloji	📄 Açıklama
.NET 8	Modern backend platformu
ASP.NET Core	REST API altyapısı
Entity Framework Core	ORM sistemi
PostgreSQL	İlişkisel veritabanı
JWT	Kimlik doğrulama
BCrypt	Şifre güvenliği
Rate Limiter	İstek kontrolü
Logging	Sistem izleme
🏗️ Sistem Mimarisi

Proje katmanlı mimari prensiplerine göre geliştirilmiştir.

Client (HTTP Request)
        ↓
Controller Layer
        ↓
Service Layer
        ↓
DbContext (EF Core)
        ↓
PostgreSQL Database

Bu yapı sayesinde:

🧩 Modüler geliştirme sağlanmıştır
🛠️ Bakım kolaylığı artırılmıştır
🗄️ Veritabanı işlemleri soyutlanmıştır
⚙️ Temel Modüller
🔐 Authentication Modülü

Kullanıcı kayıt ve giriş işlemlerini yönetir.

Özellikler:

🔑 BCrypt ile şifre hashleme
🎫 JWT token üretimi
✅ Token doğrulama
⏱️ 24 saat token geçerliliği
📍 Location & Presence Modülü

Kullanıcı konumlarını saklar ve aktif kullanıcıları belirler.

Presence Logic:

UpdatedAt >= DateTime.UtcNow.AddMinutes(-5)

🟢 Son 5 dakika içinde güncelleme yapan kullanıcılar aktif kabul edilir.

📏 Matching Modülü

Yakın kullanıcıları hesaplar.

Kullanılan yöntem:

🌍 Haversine formülü
📏 Maksimum 2 km filtre
🟢 Aktif kullanıcı kontrolü
🚫 Self-exclusion

Random sıralama:

.OrderBy(x => x.DistanceKm + 
             (Random.Shared.NextDouble() * 0.3))

🎲 Bu yaklaşım, her istekte farklı kullanıcı sıralaması sağlar.

📨 Invite Modülü

Kullanıcılar arası davet sürecini yönetir.

Özellikler:

📤 Davet gönderme
📥 Davet listeleme
✅ Davet kabul etme
❌ Davet reddetme

Status Enum:

public enum InviteStatus
{
    Pending = 0,
    Accepted = 1,
    Rejected = 2
}
🗄️ Veritabanı Tasarımı

Projede üç ana tablo bulunmaktadır:

👤 Users
📍 Locations
📨 Invites
⚠️ Partial Unique Index

Duplicate invite engellemek için:

CREATE UNIQUE INDEX ux_invite_pending
ON "Invites" ("SenderId", "ReceiverId")
WHERE "Status" = 0;

🛡️ Bu yapı race condition problemini önler.

🛡️ Güvenlik ve Sistem Koruma
🔐 Authentication Security
BCrypt hashing
JWT doğrulama
Token expiration
🚦 Rate Limiting
Endpoint	Limit
Login	5 istek / dakika
Invite	10 istek / dakika
Location	10 istek / 10 sn
Nearby	5 istek / 10 sn

🚫 Sistem kötüye kullanımını önler.

📊 Logging Sistemi

Tüm HTTP istekleri loglanır.

Loglanan bilgiler:

👤 UserId
🌐 HTTP Method
📍 Endpoint
📦 Status Code
⏱️ Duration

Örnek:

[UserId: 42] [POST /api/invite/send] [200] [83ms]
🌐 API Endpointleri
🔐 Authentication
POST /api/auth/register
POST /api/auth/login
📍 Location
POST /api/location/update
GET  /api/location/nearby
📨 Invite
POST /api/invite/send
GET  /api/invite/list
POST /api/invite/respond
▶️ Projeyi Çalıştırma
Gereksinimler
.NET 8 SDK
PostgreSQL
📥 Repository Klonlama
git clone https://github.com/kullaniciadi/friendzone-backend.git

cd friendzone-backend
⚙️ Veritabanı Ayarları
"ConnectionStrings": {
  "DefaultConnection":
  "Host=localhost;Port=5432;Database=friendzone;Username=postgres;Password=yourpassword"
}
🧱 Migration
dotnet ef database update
🚀 Uygulamayı Çalıştırma
dotnet run
🔮 Gelecek Geliştirmeler

Planlanan geliştirmeler:

⚡ Redis Cache
🤖 AI profil üretimi
🔔 SignalR bildirimleri
🩺 Health Check
🧰 Background Jobs
