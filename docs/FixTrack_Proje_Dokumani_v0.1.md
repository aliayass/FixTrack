# FixTrack | Proje Tanım, Gereksinim ve Teknik Mimari Dokümanı

**Sürüm:** 0.1  
**Tarih:** 03 Kasım 2026  
**Durum:** Geliştirme başlangıcı / yaşayan doküman  
**Proje türü:** Teknik servis takip uygulaması  

> Bu doküman FixTrack için şimdiye kadar alınan kararları, mevcut geliştirme durumunu ve planlanan işleri bir arada tutar. Taslak veya henüz kesinleşmemiş konular ayrıca işaretlenmiştir. Proje ilerledikçe güncellenecektir.

## 1. Proje özeti

FixTrack, küçük ve orta ölçekli teknik servis işletmelerinin müşteri, cihaz kabulü, arıza tespiti, fiyat teklifi, teknisyen ataması ve onarım süreçlerini tek bir sistem üzerinden yönetmesini amaçlayan web tabanlı bir uygulamadır.

Temel problem; servis bilgilerinin farklı Excel dosyalarında, mesajlaşma uygulamalarında veya kâğıt üzerinde tutulması nedeniyle kayıtların dağılması, geçmiş işlemlerin izlenmesinin zorlaşması ve servis sürecinin şeffaf olmamasıdır. FixTrack, servis kaydının açılmasından cihazın müşteriye teslim edilmesine kadar olan süreci takip edilebilir hâle getirecektir.

**Projenin teknik amacı:** Gerçek bir problemi çözen, kapsamı kontrol altında tutulan ve GitHub portföyünde incelenebilir bir ASP.NET Core Web API + React uygulaması geliştirmek. Proje aynı zamanda backend mimarisi, ilişkisel veri modelleme, kimlik doğrulama, iş kuralları, test ve frontend/API entegrasyonu konularında uygulamalı öğrenme alanı olacaktır.

## 2. Hedef kullanıcılar ve roller

İlk sürümde iki çalışan rolü bulunacaktır:

| Rol | Sorumluluklar |
|---|---|
| **Admin** | Müşteri ve servis kayıtlarını yönetir, teknisyen ataması yapar, servis sürecini takip eder ve çalışan yetkilerini yönetir. |
| **Technician** | Kendisine atanan servis kayıtlarını görüntüler, arıza tespiti ve tahmini maliyet bilgilerini girer, onarım durumunu günceller. |

Müşteriler ilk sürümde sisteme kullanıcı hesabıyla giriş yapmayacaktır. Müşteri bilgileri servis çalışanları tarafından kaydedilecek; müşteri portalı MVP kapsamına alınmayacaktır.

## 3. MVP kapsamı

### 3.1 İlk sürümde bulunacak özellikler

1. **Kullanıcı ve rol yönetimi:** Admin ve Technician rolleri; çalışanların sisteme giriş yapması.
2. **Müşteri yönetimi:** Müşteri oluşturma, listeleme, arama ve iletişim bilgilerini güncelleme.
3. **Cihaz kabulü / servis kaydı:** Müşteriye bağlı bir servis kaydı oluşturma; cihaz türü, marka, model, seri numarası, fiziksel durum, müşteri şikâyeti ve teslim alınan aksesuarları kaydetme.
4. **Teknisyen ataması:** Servis kaydına aynı anda tek bir aktif teknisyen atama; atama/değiştirme geçmişini saklama.
5. **Arıza tespiti ve fiyat teklifi:** Teknisyenin arıza açıklaması ve tahmini maliyet girmesi.
6. **Müşteri onayı:** Teklifin beklemede, kabul edildi veya reddedildi olarak izlenmesi; onayın gerekmeyeceği servislerin de desteklenmesi.
7. **Servis durum takibi:** Durum geçişlerinin iş kurallarıyla kontrol edilmesi ve her değişikliğin geçmiş kaydının tutulması.
8. **Teslimat:** Hazır olan cihazın müşteriye teslim edilmesi ve teslim tarihinin kaydedilmesi.

### 3.2 İlk sürüm dışında bırakılanlar

Aşağıdaki özellikler MVP'yi büyütmemek için sonraki sürümlere bırakılmıştır:

- Stok ve yedek parça yönetimi
- Online ödeme ve faturalandırma
- SMS / e-posta bildirimleri
- Müşteri hesabı ve müşteri portalı
- Birden fazla şube yönetimi
- Native mobil uygulama
- Gelişmiş raporlama ve analitik ekranları
- Aynı servis kaydında eşzamanlı birden fazla aktif teknisyen

Bunlar gelecekte değerlendirilebilir; ancak ilk sürümün tamamlanması için zorunlu değildir.

## 4. İş akışı ve durum modeli

Servis durumuyla müşteri onay kararı birbirinden ayrı tutulur. Çünkü cihazın servis sürecindeki konumu ile müşterinin fiyat teklifine verdiği yanıt aynı bilgi değildir.

### 4.1 ServiceStatus

| Değer | Anlamı |
|---|---|
| `Received` | Cihaz servise kabul edildi. |
| `Diagnosing` | Arıza tespiti yapılıyor. |
| `WaitingForApproval` | Fiyat teklifi için müşteri onayı bekleniyor. |
| `Repairing` | Onarım yapılıyor. |
| `ReadyForPickup` | Cihaz teslim edilmeye hazır. |
| `Delivered` | Cihaz müşteriye teslim edildi. |
| `Cancelled` | Servis süreci iptal edildi. |

### 4.2 ApprovalDecision

Önerilen değerler:

| Değer | Anlamı |
|---|---|
| `NotRequested` | Onay süreci henüz başlatılmadı. |
| `NotRequired` | Bu işlem için müşteri onayı gerekmiyor. |
| `Pending` | Müşteri yanıtı bekleniyor. |
| `Approved` | Müşteri teklifi kabul etti. |
| `Rejected` | Müşteri teklifi reddetti. |

**Önemli tasarım notu:** İlk taslakta varsayılan değer `NotRequired` olarak belirlenmişti. Ancak servis kaydı ilk açıldığında henüz arıza tespiti yapılmamış olabileceği için `NotRequested` daha doğru bir başlangıç değeridir. Bu değişiklik öneridir; kodda kesinleştirilmesi gerekir.

### 4.3 Öngörülen temel akış

- Cihaz kabul edilir: `Received`
- Arıza tespiti başlar: `Diagnosing`
- Onay gerekiyorsa: `WaitingForApproval` + `Pending`
- Müşteri kabul ederse: `Repairing` + `Approved`
- Müşteri reddederse: `ReadyForPickup` + `Rejected`
- Onay gerekmiyorsa: `Repairing` + `NotRequired`
- Onarım tamamlanır: `ReadyForPickup`
- Cihaz teslim edilir: `Delivered`

`Cancelled` dâhil tüm izin verilen/geçersiz durum geçişleri ayrıca tanımlanacak ve Application katmanında kontrol edilecektir. Nihai geçiş matrisi henüz tamamlanmamıştır.

## 5. Alan modeli ve veri tasarımı

İlk sürüm için temel varlıklar aşağıdaki gibidir.

### 5.1 Customer

Müşteri bilgilerini tutar. Bir müşteri zaman içinde birden fazla servis kaydına sahip olabilir.

| Alan | Tip | Açıklama |
|---|---|---|
| `CustomerId` | `Guid` | Birincil anahtar |
| `FullName` | `string` | Ad soyad |
| `PhoneNumber` | `string` | Telefon |
| `Email` | `string?` | İsteğe bağlı e-posta |
| `CreatedAt` | `DateTime` | Oluşturulma tarihi (UTC) |

**Karar:** `IsActive` alanı ilk taslaktan çıkarılmıştır. Müşterinin devam eden servisi olup olmadığı bu alanla temsil edilmeyecektir; aktif servis durumu `ServiceOrder.Status` üzerinden takip edilir. Müşteri arşivleme/pasifleştirme ihtiyacı doğarsa ayrıca değerlendirilecektir.

### 5.2 ApplicationUser

Sisteme giriş yapan çalışanları temsil eder. ASP.NET Core Identity kullanılarak oluşturulacaktır. Taslak yaklaşım, `IdentityUser<Guid>` sınıfından türeyen bir kullanıcı sınıfı kullanmak ve `FirstName` / `LastName` gibi ek alanlar eklemektir. Admin ve Technician rolleri Identity rol sistemiyle yönetilecektir.

Domain entity'leri kullanıcı nesnesine bağımlı olmayacak; yalnızca gerekli kullanıcı kimliklerini (`Guid`) tutacaktır.

### 5.3 ServiceOrder

Tek bir cihazın servise kabul edilmesini ve bu cihaz için yürütülen servis sürecini temsil eder. İlk sürümde ayrı bir `Device` tablosu oluşturulmayacaktır. Cihaz bilgileri ServiceOrder üzerinde tutulacaktır. Aynı müşteri cihazını tekrar getirdiğinde yeni bir ServiceOrder oluşturulacaktır.

| Alan | Tip | Açıklama |
|---|---|---|
| `ServiceOrderId` | `Guid` | Birincil anahtar |
| `ServiceNumber` | `string` | Müşteriye verilecek takip numarası |
| `CustomerId` | `Guid` | Müşteri foreign key'i |
| `CreatedByUserId` | `Guid` | Kaydı oluşturan çalışan |
| `DeviceType` | `string` | Cihaz türü |
| `Brand` | `string` | Marka |
| `Model` | `string` | Model |
| `SerialNumber` | `string?` | İsteğe bağlı seri numarası |
| `PhysicalCondition` | `string` | Cihazın teslim alındığı fiziksel durum |
| `CustomerComplaint` | `string` | Müşterinin bildirdiği sorun |
| `ReceivedAccessories` | `string?` | Teslim alınan aksesuarlar |
| `Diagnosis` | `string?` | Teknisyenin arıza tespiti |
| `EstimatedCost` | `decimal?` | Tahmini onarım tutarı |
| `ApprovalDecision` | `ApprovalDecision` | Müşteri onay kararı |
| `Status` | `ServiceStatus` | Servis durumu |
| `ReceivedAt` | `DateTime` | Kabul tarihi (UTC) |
| `ExpectedCompletionAt` | `DateTime?` | Tahmini tamamlanma tarihi |
| `DeliveredAt` | `DateTime?` | Teslim tarihi |

Para değerleri için `float` / `double` yerine `decimal` kullanılacaktır. Tarih/saat değerleri uygulama genelinde UTC tutulacaktır.

### 5.4 ServiceOrderAssignment

Bir servis kaydının hangi teknisyene, kim tarafından ve ne zaman atandığını tutar. İlk sürümde aynı servis kaydında aynı anda yalnızca bir aktif atama bulunabilir. Teknisyen değiştiğinde eski atama silinmez; `UnassignedAt` doldurularak kapatılır ve yeni atama kaydı oluşturulur.

| Alan | Tip | Açıklama |
|---|---|---|
| `Id` | `Guid` | Birincil anahtar |
| `ServiceOrderId` | `Guid` | Servis kaydı foreign key'i |
| `TechnicianId` | `Guid` | Atanan teknisyen |
| `AssignedByUserId` | `Guid` | Atamayı yapan çalışan |
| `AssignedAt` | `DateTime` | Atama tarihi (UTC) |
| `UnassignedAt` | `DateTime?` | Atamanın sona erdiği tarih; null ise aktif |

Veritabanında aynı servis kaydı için `UnassignedAt IS NULL` olan en fazla bir kayıt bulunması SQL Server filtered unique index ile güvence altına alınacaktır. Gelecekte çoklu teknisyen desteği istenirse bu kısıt ve ilgili iş kuralları yeniden ele alınabilir.

### 5.5 ServiceStatusHistory

Her servis durumu değişikliğini geçmiş olarak saklar. Böylece yalnızca son durum değil, işlemin kim tarafından ve ne zaman yapıldığı da izlenebilir.

| Alan | Tip | Açıklama |
|---|---|---|
| `Id` | `Guid` | Birincil anahtar |
| `ServiceOrderId` | `Guid` | Servis kaydı foreign key'i |
| `PreviousStatus` | `ServiceStatus?` | Önceki durum; ilk kayıtta null olabilir |
| `NewStatus` | `ServiceStatus` | Yeni durum |
| `ChangedByUserId` | `Guid` | Değişikliği yapan çalışan |
| `ChangedAt` | `DateTime` | Değişiklik tarihi (UTC) |
| `Note` | `string?` | İsteğe bağlı açıklama |

### 5.6 İlişkiler

| İlişki | Kardinalite | Açıklama |
|---|---|---|
| Customer → ServiceOrder | 1 - N | Bir müşteri birçok servis kaydı açtırabilir. |
| ApplicationUser → ServiceOrder | 1 - N | Bir çalışan birçok servis kaydı oluşturabilir. |
| ServiceOrder → ServiceOrderAssignment | 1 - N | Bir servis kaydının zaman içinde birden fazla atama geçmişi olabilir. |
| ApplicationUser → ServiceOrderAssignment | 1 - N | Kullanıcı teknisyen veya atamayı yapan çalışan rolünde ilişkilendirilebilir. |
| ServiceOrder → ServiceStatusHistory | 1 - N | Bir servis kaydının birçok durum geçmişi olabilir. |

## 6. Teknik mimari

Backend için dört katmanlı, Clean Architecture yaklaşımından yararlanan ancak gereksiz soyutlamalardan kaçınan bir yapı kullanılacaktır.

### 6.1 Katmanların sorumlulukları

| Katman | Sorumluluk |
|---|---|
| **FixTrack.API** | HTTP endpoint'leri, controller'lar, middleware, API konfigürasyonu ve dış dünyaya açılan arayüz |
| **FixTrack.Application** | Kullanım senaryoları, servisler, DTO'lar, doğrulama ve ihtiyaç duyulan arayüzler |
| **FixTrack.Domain** | Entity'ler, enum'lar ve teknoloji bağımsız temel iş kavramları |
| **FixTrack.Infrastructure** | EF Core, SQL Server, Identity, repository implementasyonları ve teknik servislerin gerçeklenmesi |

### 6.2 Proje bağımlılıkları

| Kaynak proje | Referans verdiği projeler |
|---|---|
| FixTrack.API | FixTrack.Application, FixTrack.Infrastructure |
| FixTrack.Application | FixTrack.Domain |
| FixTrack.Infrastructure | FixTrack.Application, FixTrack.Domain |
| FixTrack.Domain | Hiçbiri |
| FixTrack.Tests | FixTrack.Application, FixTrack.Domain (gerektikçe diğerleri) |

Temel ilke: Domain bağımsız kalır. Application, Domain'i kullanır. Infrastructure, Application tarafından tanımlanan arayüzlerin teknik implementasyonlarını sağlar. API, uygulama servislerini çağırır ve başlangıçta bağımlılıkları DI ile kaydeder.

### 6.3 Hedef klasör yapısı

```text
FixTrack/
├── backend/
│   ├── FixTrack.slnx
│   ├── src/
│   │   ├── FixTrack.API/
│   │   ├── FixTrack.Application/
│   │   ├── FixTrack.Domain/
│   │   └── FixTrack.Infrastructure/
│   └── tests/
│       └── FixTrack.Tests/
├── frontend/
├── docs/
├── README.md
└── .gitignore
```

Bu, hedeflenen fiziksel klasör yapısıdır. Visual Studio içindeki Solution Folder ile Windows dosya sistemindeki fiziksel klasör aynı şey değildir. Proje dosyalarının doğru fiziksel dizinlerde bulunması esastır.

### 6.4 Şimdilik kullanılmayacak yaklaşımlar

İlk sürümde ihtiyaç ortaya çıkmadan MediatR, CQRS, AutoMapper, Generic Repository, Docker veya mikroservis mimarisi eklenmeyecektir. Bunlar ileride somut bir ihtiyaç oluşursa değerlendirilecektir.

## 7. Teknoloji seçimleri

| Alan | Seçim | Durum / not |
|---|---|---|
| Backend | ASP.NET Core Web API, .NET 10 | Seçildi |
| API yaklaşımı | Controller tabanlı | Seçildi |
| ORM | Entity Framework Core | Planlandı |
| Veritabanı | SQL Server | Seçildi |
| Kullanıcı yönetimi | ASP.NET Core Identity | Planlandı |
| API kimlik doğrulama | JWT | Planlandı |
| API dokümantasyonu | OpenAPI / Swagger arayüzü | Planlandı |
| Frontend | React + TypeScript | Seçildi |
| Frontend geliştirme ortamı | VS Code | Seçildi |
| Backend geliştirme ortamı | Visual Studio | Seçildi |
| Frontend başlangıç aracı | Vite | Planlandı |
| Test | xUnit | Test projesi oluşturuldu; testler yazılacak |
| Validasyon | FluentValidation veya uygun bir doğrulama yaklaşımı | Kesinleştirilecek |
| Stil sistemi | CSS / Tailwind CSS | Henüz kesinleşmedi |
| Versiyon kontrolü | Git + GitHub | Tek repository (monorepo) planlandı |

## 8. Önemli teknik kararlar

1. **Monorepo:** Backend ve frontend ayrı uygulamalar olacak; ancak tek GitHub repository'sinde tutulacak.
2. **Ayrı geliştirme ortamları:** Backend Visual Studio'da, frontend VS Code'da geliştirilecek.
3. **Tek cihaz / tek servis kaydı:** Her ServiceOrder bir cihazın bir servis sürecini temsil edecek. İlk sürümde ayrı Device tablosu yok.
4. **Tek aktif teknisyen:** Bir servis kaydına aynı anda bir teknisyen atanacak; atama geçmişi korunacak. Veri tasarımı gelecekte çoklu atamaya açılacak.
5. **Servis numarası:** Sayısal sıra SQL Server `SEQUENCE` ile üretilecek; `FT-YYYY-000001` biçimindeki metin Application katmanında oluşturulacak; benzersizlik veritabanı unique index ile güvenceye alınacak. Sequence kullanımı işlem geri alınsa bile numara boşluklarına neden olabilir; bu kabul edilebilir.
6. **Encapsulation yaklaşımı:** Her property'ye otomatik olarak `private set` veya her entity'ye metot eklenmeyecek. Gerçek bir iş kuralı/invariant varsa kontrollü metot kullanılacak; basit veri alanlarında gereksiz metot üretilmeyecek.
7. **`required` ve `[Required]` ayrımı:** C# `required`, nesne oluşturulurken property'nin atanmasını derleme zamanında zorunlu kılar; boş string'i engellemez. `[Required]` ise bir validation mekanizması çalıştığında null/boş değer doğrulamasına katılır. API DTO doğrulaması ayrıca tasarlanacaktır.
8. **Tarih ve para:** Tarihler UTC, para değerleri decimal olarak tutulacak.
9. **Müşteri aktifliği:** İlk sürümde Customer üzerinde `IsActive` alanı bulunmayacak; bu alanın çözdüğü net bir MVP iş kuralı belirlenmedi.

## 9. Şu ana kadarki geliştirme durumu

### 9.1 Tamamlananlar

- Ana FixTrack klasör yapısı oluşturuldu.
- `FixTrack.slnx` blank solution dosyası oluşturuldu.
- `FixTrack.API`, `FixTrack.Domain`, `FixTrack.Application`, `FixTrack.Infrastructure` ve `FixTrack.Tests` projeleri oluşturuldu.
- Projeler arasındaki temel Project Reference ilişkileri eklendi.
- Solution build edildi ve tüm projeler başarılı sonuç verdi.
- Domain içinde `Entities` ve `Enums` klasörleri oluşturuldu.
- `Customer` entity'si taslak olarak oluşturuldu; `IsActive` alanı gereksiz bulunduğu için kaldırıldı.
- `ServiceStatus` ve `ApprovalDecision` enum'ları ile `ServiceOrder` entity'si üzerinde çalışmaya başlandı.
- Servis numarası için SQL Server Sequence + Application formatlama yaklaşımı seçildi.

### 9.2 Devam eden / gözden geçirilecek işler

- `Customer` entity'sinin son kodu; zorunlu alanlar, başlangıç değerleri ve erişim seviyeleri açısından son kez gözden geçirilecek.
- `ServiceOrder` taslağında `CostemerId` yazım hatası `CustomerId` olarak düzeltilecek.
- `ReceivedAccessorires` yazım hatası `ReceivedAccessories` olarak düzeltilecek.
- Eksik olan `CustomerComplaint` alanı eklenecek.
- `ApprovalDecision` varsayılan değerinin `NotRequested` olması değerlendirilecek; enum'a bu değer eklenmesi önerildi.
- ServiceOrder alanlarının `required`, nullable ve setter tercihleri son hâline getirilecek.
- Henüz Navigation Property ve EF Core ilişkileri eklenmedi.

### 9.3 Henüz başlanmayanlar

- EF Core DbContext ve Entity Configuration sınıfları
- SQL Server bağlantısı ve ilk migration
- SQL Server Sequence ve unique index konfigürasyonu
- Application servisleri, DTO'lar ve arayüzler
- Identity, JWT ve rol bazlı authorization
- Controller ve endpoint geliştirme
- Durum geçiş kurallarının uygulanması
- Unit / integration testleri
- React arayüzü ve API entegrasyonu
- README, mimari diyagramlar, CI/CD ve deployment

## 10. Geliştirme yol haritası

| Aşama | İçerik | Durum |
|---|---|---|
| 0. Solution kurulumu | Klasör yapısı, projeler ve referanslar | **Tamamlandı** |
| 1. Domain modeli | Entity'ler, enum'lar, ilişkiler ve temel iş kuralları | Devam ediyor |
| 2. Application | DTO'lar, servisler, arayüzler, validasyon | Bekliyor |
| 3. Infrastructure | DbContext, EF Core konfigürasyonları, Identity, SQL Server, migration, Sequence | Bekliyor |
| 4. API | Auth, controller'lar, hata yönetimi, OpenAPI ve authorization | Bekliyor |
| 5. Frontend | React/TypeScript arayüzü, admin ve teknisyen ekranları, API entegrasyonu | Bekliyor |
| 6. Kalite | Unit/integration testleri, loglama, güvenlik ve hata senaryoları | Bekliyor |
| 7. Yayınlama | README, ekran görüntüleri, deployment ve GitHub düzeni | Bekliyor |

### Önerilen geliştirme sırası

1. ServiceOrder taslağını düzelt ve Customer entity'sini son hâline getir.
2. `ServiceOrderAssignment` ve `ServiceStatusHistory` entity'lerini tasarla.
3. Navigation Property ilişkilerini ve entity konfigürasyonlarını belirle.
4. Durum geçişleri ve atama kurallarını kesinleştir.
5. Application katmanında DTO ve servisleri oluştur.
6. Infrastructure katmanında SQL Server, EF Core, Identity ve Sequence altyapısını kur.
7. API endpoint'lerini ve yetkilendirmeyi geliştir.
8. React arayüzünü oluşturup API'ye bağla.
9. Test, dokümantasyon ve yayınlama adımlarını tamamla.

## 11. Açık kararlar ve riskler

Aşağıdaki maddeler henüz kesinleştirilmemiştir; ihtiyaç duyulan aşamada karara bağlanacaktır.

- Servis numarasındaki yıl bölümünün takvim yılı değişince sıfırlanıp sıfırlanmayacağı; Sequence'in tüm sistemde mi yoksa yıl bazında mı çalışacağı.
- `DeviceType` için string mi, enum mı kullanılacağı; cihaz türlerinin yönetilebilir bir listeye dönüşüp dönüşmeyeceği.
- `PhysicalCondition` alanının serbest metin mi, kontrollü seçenekler mi olacağı.
- `Brand` ve `Model` için serbest metin yaklaşımının yeterli olup olmadığı.
- `Cancelled` durumuna hangi aşamalarda izin verileceği ve iptal nedeninin zorunlu olup olmayacağı.
- Onay gerekmeyen servislerde arıza tespitinden onarıma geçişin kesin kuralı.
- Müşteri kaydının silinmesi, arşivlenmesi veya geçmiş servis kayıtları nedeniyle silmenin engellenmesi politikası.
- UI ekran tasarımı, bileşen sistemi ve Tailwind CSS kullanımı.
- Admin kullanıcısının ilk kurulumda nasıl oluşturulacağı ve teknisyen hesabı açma akışı.

## 12. Geliştirme ilkeleri

- Önce çalışan ve anlaşılır bir çözüm; sonra ihtiyaç varsa soyutlama.
- Her mimari kararın gerekçesi olmalı; yalnızca popüler olduğu için pattern veya paket eklenmemeli.
- Controller'lar HTTP katmanıyla sınırlı kalmalı; iş kuralları Application katmanında yönetilmeli.
- Veritabanı bütünlüğü yalnızca uygulama koduna bırakılmamalı; unique constraint ve foreign key gibi mekanizmalar gerektiğinde kullanılmalı.
- Entity'lerde encapsulation gerçek iş kurallarını korumak için kullanılmalı; her property'yi private yapmak hedef değildir.
- DTO ile Entity sorumlulukları ayrılmalı; dışarıdan gelen veri doğrudan entity'ye bağlanmamalı.
- Her önemli geliştirme adımı küçük, açıklayıcı Git commit'leriyle kaydedilmeli.
- Öğrenme amacı korunmalı: kodun yalnızca çalışması değil, neden o şekilde yazıldığının anlaşılması da hedeflenmeli.

## 13. Bir sonraki somut çalışma

Bir sonraki geliştirme oturumunda önce `ServiceOrder` taslağı gözden geçirilip yazım hataları ve eksik alanlar düzeltilecektir. Ardından `ServiceOrderAssignment` entity'si tasarlanacaktır. Bu tasarımda tek aktif teknisyen kuralı, atama geçmişi ve ileride çoklu teknisyen desteğine geçiş ihtiyacı dikkate alınacaktır. Sonrasında `ServiceStatusHistory` ve entity ilişkileri ele alınacaktır.

---

**Doküman durumu:** Bu belge bir başlangıç sürümüdür (v0.1). Uygulama geliştikçe karar kayıtları, alan modeli, endpoint listesi, ekran tasarımları ve test senaryolarıyla güncellenecektir.
