# FixTrack

**FixTrack**, teknik servis ve onarım süreçlerini takip etmek için geliştirilen bir web uygulamasıdır. Projenin amacı; müşteri ve cihaz kayıtlarını, servis emirlerini, teknisyen atamalarını ve servis durum geçmişini tek bir sistem üzerinden yönetebilen, katmanlı mimariye sahip ve ileride genişletilebilecek bir uygulama ortaya çıkarmaktır.

Proje şu anda geliştirme aşamasındadır. Backend altyapısı ve müşteri yönetiminin uygulama katmanı hazırlanmıştır. HTTP endpoint'leri, arayüz ve kimlik doğrulama gibi sonraki modüller geliştirme planındadır.

## İçindekiler

- [Projenin Amacı](#projenin-amacı)
- [Planlanan Özellikler](#planlanan-özellikler)
- [Teknolojiler](#teknolojiler)
- [Mimari](#mimari)
- [Proje Yapısı](#proje-yapısı)
- [Şu Ana Kadar Yapılanlar](#şu-ana-kadar-yapılanlar)
- [Geliştirme Yol Haritası](#geliştirme-yol-haritası)
- [Yerel Geliştirme Ortamı](#yerel-geliştirme-ortamı)
- [Veritabanı ve Migration İşlemleri](#veritabanı-ve-migration-işlemleri)
- [Güvenlik ve Yapılandırma](#güvenlik-ve-yapılandırma)
- [Test ve Derleme](#test-ve-derleme)
- [Git İş Akışı](#git-iş-akışı)

## Projenin Amacı

Teknik servis süreçlerinde müşteri bilgileri, teslim alınan cihazlar, arıza açıklamaları, teknisyen atamaları ve onarımın güncel durumu farklı yerlerde tutulabiliyor. FixTrack, bu süreçleri düzenli ve izlenebilir hâle getirmeyi hedefler.

Uygulamanın temel hedefleri:

- Müşteri bilgilerini merkezi olarak yönetmek.
- Müşterilere ait birden fazla servis emri oluşturabilmek.
- Cihazın marka, model, seri numarası ve fiziksel durum gibi bilgilerini kaydetmek.
- Müşterinin arıza/şikâyet açıklamasını ve teslim ettiği aksesuarları takip etmek.
- Servis emirlerini teknisyenlere atamak ve geçmiş atamaları korumak.
- Servis durumundaki değişiklikleri tarihçesiyle saklamak.
- İlerleyen aşamalarda kullanıcı rolleri ve yetkilendirme ile farklı kullanıcıların kendi sorumluluklarına uygun işlemler yapmasını sağlamak.
- Web arayüzü üzerinden kullanılabilen, API odaklı bir backend sunmak.

FixTrack öncelikle öğrenme ve portföy amacıyla geliştirilmektedir. Bununla birlikte hedef, yalnızca örnek CRUD ekranlarından oluşan bir proje değil; veri ilişkileri, iş kuralları, hata yönetimi, testler ve dağıtım süreçleri düşünülmüş gerçekçi bir uygulama geliştirmektir.

## Planlanan Özellikler

Aşağıdaki özellikler ürünün hedeflenen davranışlarını gösterir. Tamamlanmış olarak işaretlenmeyen özellikler henüz geliştirme planındadır.

### Müşteri Yönetimi

- Müşteri oluşturma, listeleme, detay görüntüleme, güncelleme ve silme.
- Müşterinin servis geçmişine erişme.

### Servis Emri Yönetimi

- Müşteri adına servis emri oluşturma.
- Cihaz ve arıza bilgilerini kaydetme.
- Teşhis, tahmini maliyet ve tahmini teslim tarihi bilgilerini yönetme.
- Servis emrinin mevcut durumunu takip etme.

### Teknisyen Atama

- Servis emrini bir teknisyene atama.
- Aktif atamayı sonlandırıp başka teknisyene atama yapabilme.
- Önceki atama kayıtlarını saklama.

İlk sürümde bir servis emrinin aynı anda en fazla bir aktif teknisyeni olması hedeflenmiştir. Veri modeli, ihtiyaç ortaya çıktığında birden fazla teknisyen desteği eklenebilecek şekilde geliştirilmeye açıktır.

### Servis Durum Geçmişi

- Durum değişikliklerini kaydetme.
- Önceki ve yeni durumu, işlemi yapan kullanıcıyı, zamanı ve isteğe bağlı açıklamayı saklama.
- Servis emrinin geçmiş hareketlerini görüntüleme.

### Kullanıcı ve Yetkilendirme

- Kullanıcı kaydı ve oturum açma.
- Yönetici ve teknisyen gibi roller.
- Kullanıcının rolüne göre endpoint ve işlem yetkilendirmesi.

### Web Arayüzü

- React ve TypeScript tabanlı frontend.
- Müşteri ve servis emri yönetim ekranları.
- Teknisyen atama ve servis durum takibi ekranları.
- Rol bazlı yönetim arayüzü.

## Teknolojiler

### Backend

- **C#** – Uygulama geliştirme dili.
- **.NET 10 / ASP.NET Core Web API** – HTTP API ve uygulama altyapısı.
- **Entity Framework Core 10** – ORM ve veritabanı işlemleri.
- **Npgsql** – PostgreSQL için EF Core sağlayıcısı.
- **xUnit** – Test projesinde kullanılan test altyapısı; kapsamlı testler geliştirme planındadır.

### Veritabanı ve araçlar

- **PostgreSQL 18** – İlişkisel veritabanı.
- **Docker Compose** – Yerel PostgreSQL konteynerini çalıştırmak için.
- **User Secrets** – Yerel geliştirmede API connection string gibi gizli değerleri saklamak için.
- **Visual Studio** – Geliştirme ortamı.
- **Git ve GitHub** – Sürüm kontrolü ve kaynak kod yönetimi.

### Planlanan frontend

- React
- TypeScript
- Vite

Frontend henüz oluşturulmamıştır.

## Mimari

Backend, sorumlulukları birbirinden ayırmak amacıyla katmanlı bir yapı kullanır.

```text
API
  └── HTTP isteklerini karşılar, servisleri çağırır ve HTTP yanıtlarını üretir.

Application
  └── DTO'ları, interface'leri ve uygulama servislerini içerir.

Domain
  └── Uygulamanın temel entity ve enum tanımlarını içerir.

Infrastructure
  └── EF Core, DbContext, veritabanı eşleştirmeleri ve repository implementasyonlarını içerir.

PostgreSQL
  └── Kalıcı veriyi saklar.
```

### Katmanların sorumlulukları

| Katman | Sorumluluk |
|---|---|
| `FixTrack.Domain` | Temel iş nesneleri (entity'ler) ve enum'lar. Veritabanı veya HTTP ayrıntılarını içermez. |
| `FixTrack.Application` | DTO'lar, servis ve repository interface'leri, uygulama kullanım senaryoları. |
| `FixTrack.Infrastructure` | Application tarafından tanımlanan repository sözleşmelerinin EF Core ile gerçekleştirilmesi, `AppDbContext`, entity konfigürasyonları ve migration'lar. |
| `FixTrack.API` | ASP.NET Core giriş noktası, HTTP endpoint'leri, yapılandırma ve Dependency Injection kayıtları. |
| `FixTrack.Tests` | Uygulama için otomatik testlerin bulunacağı test projesi. |

### Customer işlemi için hedeflenen akış

```text
HTTP Request
    ↓
CustomersController
    ↓
ICustomerService / CustomerService
    ↓
ICustomerRepository
    ↓
CustomerRepository
    ↓
AppDbContext (EF Core)
    ↓
PostgreSQL
```

Controller HTTP katmanıyla, Service uygulama akışıyla, Repository ise veri erişimiyle ilgilenir. Application katmanı somut EF Core implementasyonuna bağımlı değildir; Infrastructure bu interface'leri gerçekleştirir.

## Proje Yapısı

```text
FixTrack/
├── backend/
│   ├── FixTrack.slnx
│   ├── src/
│   │   ├── FixTrack.API/
│   │   ├── FixTrack.Application/
│   │   │   ├── DTOs/
│   │   │   │   └── Customers/
│   │   │   ├── Interfaces/
│   │   │   │   ├── Repositories/
│   │   │   │   └── Services/
│   │   │   └── Services/
│   │   ├── FixTrack.Domain/
│   │   │   ├── Entities/
│   │   │   └── Enums/
│   │   └── FixTrack.Infrastructure/
│   │       └── Persistence/
│   │           ├── Configurations/
│   │           ├── Migrations/
│   │           └── Repositories/
│   └── tests/
│       └── FixTrack.Tests/
├── docs/
├── docker-compose.yml
├── .gitignore
└── README.md
```

`frontend/` klasörü yol haritasındadır; henüz oluşturulmamıştır.

## Şu Ana Kadar Yapılanlar

Bu bölüm, repodaki mevcut geliştirme durumunu özetler.

### Solution ve Katmanlar

- [x] .NET 10 solution oluşturuldu.
- [x] Domain, Application, Infrastructure ve API projeleri oluşturuldu.
- [x] Test projesi solution'a eklendi.
- [x] Projeler arasındaki temel referanslar düzenlendi.

### Domain Modeli

- [x] `Customer` entity'si oluşturuldu.
- [x] `ServiceOrder` entity'si oluşturuldu.
- [x] `ServiceOrderAssignment` entity'si oluşturuldu.
- [x] `ServiceStatusHistory` entity'si oluşturuldu.
- [x] `ApprovalDecision` ve `ServiceStatus` enum'ları oluşturuldu.

### Veritabanı

- [x] PostgreSQL için Docker Compose yapılandırması hazırlandı.
- [x] EF Core PostgreSQL sağlayıcısı eklendi.
- [x] `AppDbContext` oluşturuldu.
- [x] Entity konfigürasyonları `IEntityTypeConfiguration<T>` üzerinden ayrıştırıldı.
- [x] İlk EF Core migration oluşturuldu ve veritabanına uygulandı.
- [x] Tablolar, foreign key ilişkileri ve indeksler kontrol edildi.
- [x] Aktif servis emri atamasının tekil olmasını sağlayan filtreli unique index tanımlandı.
- [x] API connection string'i yerel geliştirme için User Secrets üzerinden yapılandırıldı.

### Customer Uygulama Altyapısı

- [x] `CreateCustomerRequest`, `UpdateCustomerRequest` ve `CustomerResponse` DTO'ları oluşturuldu.
- [x] `ICustomerRepository` interface'i tanımlandı.
- [x] `CustomerRepository` EF Core ile gerçekleştirildi.
- [x] `ICustomerService` interface'i tanımlandı.
- [x] `CustomerService` içerisinde oluşturma, listeleme, ID ile getirme, güncelleme ve silme akışları yazıldı.
- [x] Entity ile response DTO arasındaki dönüşüm eklendi.
- [x] Repository ve Service için Dependency Injection kayıtları eklendi.

**Önemli:** Customer servis ve repository altyapısı hazırlanmış olsa da bu işlemleri HTTP üzerinden sunan `CustomersController` ve çalışan müşteri endpoint'leri henüz tamamlanmış değildir. Bu nedenle Customer CRUD, henüz uçtan uca tamamlanmış bir API özelliği olarak değerlendirilmemelidir.

### Proje ve Geliştirme Araçları

- [x] `.gitignore` ile `.env`, .NET derleme çıktıları ve yerel IDE dosyaları hariç tutuldu.
- [x] İlk Git commit'i oluşturuldu.
- [x] GitHub repository bağlantısı hazırlandı.
- [x] İlk solution build işlemi başarıyla tamamlandı.

## Geliştirme Yol Haritası

Aşağıdaki sıra mevcut geliştirme planını gösterir; ihtiyaçlara göre güncellenebilir.

### 1. Customer API'yi Tamamlama

- [ ] Dependency Injection kayıtlarını ve API başlangıç yapılandırmasını doğrulama.
- [ ] `CustomersController` oluşturma.
- [ ] `POST`, `GET`, `PUT` ve `DELETE` endpoint'lerini ekleme.
- [ ] Doğru HTTP durum kodlarını ve hata yanıtlarını döndürme.
- [ ] OpenAPI üzerinden endpoint'leri deneme.
- [ ] Customer işlemleri için unit ve integration testleri yazma.

### 2. Servis Emri Yönetimi

- [ ] Servis emri DTO'larını ve interface'lerini oluşturma.
- [ ] Servis emri repository ve service katmanlarını geliştirme.
- [ ] Servis emri oluşturma, listeleme, detay, güncelleme ve gerekli durumlarda iptal akışlarını ekleme.
- [ ] Müşteri-servis emri ilişkisini API üzerinden kullanıma açma.
- [ ] İş kurallarını ve validasyonları test etme.

### 3. Teknisyen Atama ve Durum Geçmişi

- [ ] Kullanıcı/teknisyen modelini ve gerekli ilişkileri tasarlama.
- [ ] Servis emrine teknisyen atama ve atamayı sonlandırma akışlarını ekleme.
- [ ] Atama geçmişini koruma.
- [ ] Servis durumu değiştirme ve `ServiceStatusHistory` kaydı oluşturma.
- [ ] Birden fazla veritabanı işlemi gerektiren senaryolarda transaction davranışını ele alma.

### 4. Kimlik Doğrulama ve Yetkilendirme

- [ ] ASP.NET Core Identity veya uygun kimlik doğrulama çözümünü entegre etme.
- [ ] Kullanıcı ve rol yönetimini ekleme.
- [ ] Yönetici ve teknisyen gibi roller için yetkilendirme kuralları yazma.
- [ ] Kullanıcı kimliklerini servis kayıtları ve geçmiş tablolarıyla ilişkilendirme.

### 5. Kalite ve Bakım Kolaylığı

- [ ] Merkezi hata yönetimi ekleme.
- [ ] Logging ve gerekli gözlemlenebilirlik altyapısını ekleme.
- [ ] DTO ve domain validasyonlarını netleştirme.
- [ ] Liste endpoint'lerine sayfalama, filtreleme ve sıralama ekleme.
- [ ] Unit ve integration test kapsamını artırma.
- [ ] API dokümantasyonunu geliştirme.

### 6. Frontend

- [ ] React, TypeScript ve Vite ile frontend projesini oluşturma.
- [ ] API istemcisi ve hata yönetimini hazırlama.
- [ ] Giriş ve rol bazlı ekranları geliştirme.
- [ ] Müşteri ve servis emri ekranlarını oluşturma.
- [ ] Teknisyen atama ve servis geçmişi ekranlarını ekleme.

### 7. Yayınlama ve Teslim

- [ ] API ve frontend için Docker yapılandırmasını tamamlama.
- [ ] Geliştirme ve production yapılandırmalarını ayırma.
- [ ] CI sürecinde build ve test adımlarını çalıştırma.
- [ ] Uygulamayı bir ortama yayınlama.
- [ ] Kurulum, kullanım ve mimari dokümantasyonunu güncel tutma.

## Yerel Geliştirme Ortamı

### Gereksinimler

- .NET 10 SDK
- Docker Desktop ve Docker Compose
- Git
- Visual Studio 2026 veya .NET CLI destekleyen bir IDE

### 1. Repoyu klonlama

```bash
git clone https://github.com/aliayass/FixTrack.git
cd FixTrack
```

### 2. PostgreSQL ortam değişkenlerini hazırlama

Proje kök dizininde `.env` adında bir dosya oluştur. Bu dosya Git'e eklenmemelidir.

Örnek içerik:

```env
POSTGRES_DB=fixtrack
POSTGRES_USER=fixtrack_user
POSTGRES_PASSWORD=CHANGE_ME_TO_A_LOCAL_SECRET
```

`POSTGRES_PASSWORD` için kendine ait güçlü bir yerel parola belirle. Gerçek parola, token veya başka bir gizli değeri README'ye ya da Git deposuna yazma.

### 3. PostgreSQL'i başlatma

Proje kök dizininde:

```bash
docker compose up -d
```

Container durumunu kontrol etmek için:

```bash
docker compose ps
```

PostgreSQL container'ı Docker Compose health check ile hazır olana kadar beklenmelidir.

### 4. API için User Secrets ayarlama

Projede `UserSecretsId` zaten tanımlıysa yeniden `user-secrets init` çalıştırmana gerek yoktur. Yeni bir projede bu kimlik henüz tanımlı değilse şu komutla oluşturabilirsin:

```powershell
dotnet user-secrets init --project .\backend\src\FixTrack.API\FixTrack.API.csproj
```

Connection string'i yerel User Secrets deposuna ekle. Parolayı `.env` dosyasında kullandığın yerel PostgreSQL parolasıyla eşleştir:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=fixtrack;Username=fixtrack_user;Password=YOUR_LOCAL_PASSWORD" --project .\backend\src\FixTrack.API\FixTrack.API.csproj
```

`YOUR_LOCAL_PASSWORD` yerine kendi yerel parolanı yaz. Bu komuta gerçek parolanı bir README veya Git dosyasına ekleme.

### 5. Migration'ları uygulama

Repo kök dizinindeyken:

```powershell
dotnet ef database update --project .\backend\src\FixTrack.Infrastructure\FixTrack.Infrastructure.csproj --startup-project .\backend\src\FixTrack.API\FixTrack.API.csproj
```

Bu komut mevcut migration'ları PostgreSQL veritabanına uygular.

### 6. API'yi çalıştırma

```powershell
dotnet run --project .\backend\src\FixTrack.API\FixTrack.API.csproj
```

API'nin yerel adresi kullanılan launch profile'a göre terminalde gösterilir. OpenAPI doküman eşlemesi Development ortamında yapılandırılmıştır. Henüz Customer controller endpoint'leri tamamlanmadığı için müşteri CRUD işlemlerinin kullanılabilir olduğunu varsaymayın.

## Veritabanı ve Migration İşlemleri

`AppDbContext`, Infrastructure katmanında bulunur. Entity eşleştirmeleri `Persistence/Configurations` altında ayrı sınıflarla tanımlanır ve assembly üzerinden yüklenir.

Yeni migration oluşturmak için repo kök dizininden:

```powershell
dotnet ef migrations add MIGRATION_NAME --project .\backend\src\FixTrack.Infrastructure\FixTrack.Infrastructure.csproj --startup-project .\backend\src\FixTrack.API\FixTrack.API.csproj --output-dir Persistence/Migrations
```

Migration'ı veritabanına uygulamak için:

```powershell
dotnet ef database update --project .\backend\src\FixTrack.Infrastructure\FixTrack.Infrastructure.csproj --startup-project .\backend\src\FixTrack.API\FixTrack.API.csproj
```

Docker container'ını durdurmak için:

```bash
docker compose down
```

Bu komut named volume içindeki PostgreSQL verisini korur. **`docker compose down -v` komutu volume'u ve içindeki verileri siler.** Geliştirme verilerini kaybetmek istemiyorsanız `-v` kullanmayın.

## Güvenlik ve Yapılandırma

- `.env` ve `.env.*` dosyaları `.gitignore` ile Git dışında tutulur; `.env.example` istisnadır.
- API connection string gibi yerel gizli değerler ASP.NET Core User Secrets ile saklanır.
- Gizli değerler kaynak koda, README'ye, commit mesajlarına veya GitHub'a eklenmemelidir.
- Production ortamında User Secrets yerine ortama uygun güvenli bir secret yönetimi kullanılmalıdır.
- Kullanıcı ve teknisyen foreign key ilişkileri, ilgili kullanıcı modeli ve kimlik doğrulama tasarımı tamamlandığında ele alınacaktır.

## Test ve Derleme

Backend solution'ını derlemek için:

```powershell
cd backend
dotnet build
```

Testleri çalıştırmak için:

```powershell
dotnet test
```

Test projesi oluşturulmuştur; ancak kapsamlı unit ve integration testleri henüz geliştirme yol haritasındadır. Test kapsamı arttıkça bu bölüm güncellenecektir.

## Git İş Akışı

Geliştirme sırasında her anlamlı özellik veya düzeltme ayrı commit olarak kaydedilir. Önerilen commit türleri:

| Prefix | Kullanım |
|---|---|
| `feat:` | Yeni özellik |
| `fix:` | Hata düzeltmesi |
| `refactor:` | Davranışı değiştirmeden kod düzenlemesi |
| `test:` | Test ekleme veya düzenleme |
| `docs:` | Dokümantasyon değişikliği |
| `chore:` | Araç, yapılandırma veya bakım işi |

Örnek akış:

```bash
git status
git diff
git add .
git diff --cached
git commit -m "feat: add customer endpoints"
git push
```

Commit öncesinde özellikle `.env`, connection string, parola ve token gibi gizli bilgilerin stage edilmediği kontrol edilmelidir.

---

**Proje durumu:** Aktif geliştirme aşamasında. Yol haritası, uygulama ilerledikçe güncellenecektir.
