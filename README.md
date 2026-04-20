Friendzone Backend Sistemi

Friendzone Backend, .NET 8 ve PostgreSQL kullanılarak geliştirilmiş lokasyon tabanlı bir sosyal eşleşme REST API sistemidir. Sistem, kullanıcıların konum bilgilerini paylaşarak yakın kullanıcıları keşfetmesini ve davet tabanlı etkileşim kurmasını sağlayacak şekilde tasarlanmıştır.

Proje geliştirilirken ölçeklenebilirlik, veri tutarlılığı ve sistem güvenliği temel mühendislik prensipleri olarak ele alınmıştır.

Proje Amacı

Bu projenin temel amacı modern backend mimari prensipleri kullanılarak güvenli ve performanslı bir REST API geliştirmektir.

Projenin hedefleri:

Kullanıcı kimlik doğrulama sisteminin güvenli şekilde uygulanması
Kullanıcı konum bilgilerinin saklanması
Aktif kullanıcıların belirlenmesi
Coğrafi mesafe hesaplamaları ile yakın kullanıcıların bulunması
Kullanıcılar arası davet mekanizmasının yönetilmesi
Veri tutarlılığının transaction ve constraint mekanizmaları ile sağlanması
Kullanılan Teknolojiler
Teknoloji	Açıklama
.NET 8	Modern ve yüksek performanslı backend geliştirme platformu
ASP.NET Core Web API	REST API geliştirme altyapısı
Entity Framework Core	ORM (Object Relational Mapping)
PostgreSQL	İlişkisel veritabanı sistemi
JWT	Kimlik doğrulama mekanizması
BCrypt	Şifre hashleme algoritması
Rate Limiter	İstek sınırlama mekanizması
Logging	Sistem izleme ve hata yönetimi
Sistem Mimarisi

Proje, katmanlı mimari prensiplerine göre geliştirilmiştir.

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

Kodun bakım kolaylığı artırılmıştır
İş mantığı kontrol altında tutulmuştur
Veritabanı işlemleri soyutlanmıştır
Temel Modüller
Authentication Modülü

Kullanıcı kayıt ve giriş işlemlerini yönetir.

Özellikler:

BCrypt ile şifre hashleme
JWT token üretimi
Token doğrulama
24 saat token geçerliliği
Location ve Presence Modülü

Kullanıcı konumlarını saklar ve aktif kullanıcıları belirler.

Presence Logic:

UpdatedAt >= DateTime.UtcNow.AddMinutes(-5)

Bu kurala göre son 5 dakika içinde konum güncelleyen kullanıcılar aktif kabul edilir.

Matching Modülü

Kullanıcılar arası mesafe hesaplaması yaparak yakın kullanıcıları bulur.

Kullanılan yöntem:

Haversine formülü ile mesafe hesaplama
Maksimum 2 km filtreleme
Aktif kullanıcı filtreleme
Kendini hariç tutma

Random sıralama faktörü:

.OrderBy(x => x.DistanceKm + 
             (Random.Shared.NextDouble() * 0.3))

Bu yaklaşım, her istekte aynı kullanıcıların listelenmesini engeller.

Invite Modülü

Kullanıcılar arası davet sürecini yönetir.

Özellikler:

Davet gönderme
Davet listeleme
Davet kabul etme
Davet reddetme

Invite Status Enum:

public enum InviteStatus
{
    Pending = 0,
    Accepted = 1,
    Rejected = 2
}
Veritabanı Tasarımı

Projede üç ana tablo bulunmaktadır:

Users
Locations
Invites
Partial Unique Index

Aynı kullanıcıya birden fazla bekleyen davet gönderilmesini engellemek için:

CREATE UNIQUE INDEX ux_invite_pending
ON "Invites" ("SenderId", "ReceiverId")
WHERE "Status" = 0;

Bu yapı race condition problemlerini önlemek için kritik öneme sahiptir.

Güvenlik ve Sistem Koruma

Sistem güvenliği için aşağıdaki mekanizmalar uygulanmıştır:

Authentication Security
BCrypt password hashing
JWT doğrulama
Token expiration
Rate Limiting
Endpoint	Limit
Login	5 istek / dakika
Invite	10 istek / dakika
Location Update	10 istek / 10 saniye
Nearby Query	5 istek / 10 saniye

Bu mekanizma sistemin kötüye kullanımını önler.

Logging Sistemi

Tüm HTTP istekleri loglanmaktadır.

Loglanan bilgiler:

UserId
HTTP Method
Endpoint
Status Code
Execution Duration

Örnek log:

[UserId: 42] [POST /api/invite/send] [200] [83ms]
API Endpointleri
Authentication
POST /api/auth/register
POST /api/auth/login
Location
POST /api/location/update
GET  /api/location/nearby
Invite
POST /api/invite/send
GET  /api/invite/list
POST /api/invite/respond
Projeyi Çalıştırma
Gereksinimler
.NET 8 SDK
PostgreSQL
Repository Klonlama
git clone https://github.com/kullaniciadi/friendzone-backend.git

cd friendzone-backend
Veritabanı Ayarları

appsettings.json dosyasını düzenleyin:

"ConnectionStrings": {
  "DefaultConnection":
  "Host=localhost;Port=5432;Database=friendzone;Username=postgres;Password=yourpassword"
}
Migration Uygulama
dotnet ef database update
Uygulamayı Çalıştırma
dotnet run
Gelecek Geliştirmeler

Planlanan geliştirmeler:

Redis Cache entegrasyonu
AI destekli profil oluşturma
SignalR ile gerçek zamanlı bildirim
Health Check endpointleri
Background job sistemi






Friendzone Backend

Friendzone Backend is a location-based social matching REST API developed using .NET 8 and PostgreSQL.
The system is designed with scalability, security, and concurrency safety in mind, following modern backend engineering practices.

Overview

This project provides backend services for a location-aware social interaction platform.
It enables users to authenticate, share their location, discover nearby active users, and exchange interaction requests.

The system emphasizes:

Secure authentication
Real-time location presence
Distance-based user matching
Controlled interaction workflows
System hardening and reliability
Architecture

The application follows a layered architecture with clear separation of concerns.

API Controllers
        │
        ▼
Application Services
        │
        ▼
Data Access Layer (EF Core)
        │
        ▼
PostgreSQL Database
Key Architectural Principles
Separation of concerns
Dependency injection
Stateless authentication
Centralized error handling
Defensive database design
Technology Stack
Category	Technology
Runtime	.NET 8
Framework	ASP.NET Core Web API
ORM	Entity Framework Core
Database	PostgreSQL
Authentication	JWT
Password Security	BCrypt
Rate Limiting	ASP.NET Core RateLimiter
Logging	Microsoft.Extensions.Logging
Core Modules
Authentication Module

Handles user registration and login operations.

Features

BCrypt password hashing
JWT-based authentication
Issuer and audience validation
Token expiration (24 hours)
Location & Presence Module

Manages user location data and activity status.

Presence Rule

UpdatedAt >= DateTime.UtcNow.AddMinutes(-5)

Users are considered active if they updated their location within the last 5 minutes.

Matching Module

Finds nearby users based on geographic distance.

Matching Rules

Maximum radius: 2 km
Only active users included
Self-exclusion enforced
Distance-based sorting
Randomization factor applied
.OrderBy(x => x.DistanceKm +
             (Random.Shared.NextDouble() * 0.3))
Invite Module

Manages user-to-user interaction requests.

Supported Actions

Send invite
List invites
Accept invite
Reject invite

Constraints

Duplicate pending invites prevented
Cooldown applied between requests
Expiration enforced
Database Design

The system includes three primary entities:

Users
Locations
Invites
Users Table
Column	Type	Notes
Id	UUID	Primary Key
Username	Text	Required
Email	Text	Unique
PasswordHash	Text	BCrypt
CreatedAt	Timestamp	Default UTC
Locations Table
Column	Type	Notes
Id	UUID	Primary Key
UserId	UUID	Foreign Key
Latitude	Double	Required
Longitude	Double	Required
UpdatedAt	Timestamp	Presence Logic

Index

CREATE INDEX idx_location_user_updatedat
ON "Locations" ("UserId", "UpdatedAt");
Invites Table
Column	Type	Notes
Id	UUID	Primary Key
SenderId	UUID	Foreign Key
ReceiverId	UUID	Foreign Key
Status	Integer	Enum
CreatedAt	Timestamp	Required

Status Enum

public enum InviteStatus
{
    Pending = 0,
    Accepted = 1,
    Rejected = 2
}
Partial Unique Index

Prevents duplicate pending invites.

CREATE UNIQUE INDEX ux_invite_pending
ON "Invites" ("SenderId", "ReceiverId")
WHERE "Status" = 0;
Security & Hardening

The system includes multiple defensive mechanisms.

Authentication Security
BCrypt password hashing
JWT validation
Token expiration enforcement
Concurrency Safety
Database-level unique constraints
Transactional operations
Duplicate detection logic
Rate Limiting
Endpoint	Limit
Login	5 requests / minute
Invite	10 requests / minute
Location Update	10 requests / 10 seconds
Nearby Query	5 requests / 10 seconds
Logging & Monitoring

All HTTP requests are logged with structured metadata.

Logged Fields

UserId
HTTP Method
Request Path
Response Status
Execution Duration

Example:

[UserId: 42] [POST /api/invite/send] [200] [83ms]
Data Transfer Objects (DTOs)

The API uses DTOs to isolate domain entities from external contracts.

Implemented DTOs

RegisterRequestDto
LoginRequestDto
UpdateLocationDto
NearbyUserDto
SendInviteDto
InviteResponseDto
API Endpoints
Authentication
POST /api/auth/register
POST /api/auth/login
Location
POST /api/location/update
GET  /api/location/nearby
Invite
POST /api/invite/send
GET  /api/invite/list
POST /api/invite/respond
Database Migrations

The following migrations are included:

InitialCreate
AddLocationTable
AddLocationIndexes
AddInviteEntity

Migrations are used for schema version control and deployment consistency.

Getting Started
Prerequisites
.NET 8 SDK
PostgreSQL
Clone Repository
git clone https://github.com/yourusername/friendzone-backend.git

cd friendzone-backend
Configure Database

Update appsettings.json:

"ConnectionStrings": {
  "DefaultConnection":
  "Host=localhost;Port=5432;Database=friendzone;Username=postgres;Password=yourpassword"
}
Apply Migrations
dotnet ef database update
Run Application
dotnet run
Future Enhancements

The following components are planned:

AI-based profile generation
Redis caching layer
Background job processing
Distributed logging
Metrics and health monitoring
License

MIT License
