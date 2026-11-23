# OrderCase

Sipariş alan, RabbitMQ üzerinden ayrı bir servise işleten ve sonucu asenkron geri alan iki servisli .NET 10 mikroservis örneği.

## Tech Stack

- .NET 10, ASP.NET Core Web API, Worker Service
- PostgreSQL + EF Core 10 (code-first)
- RabbitMQ + DotNetCore.CAP (outbox, retry)
- Redis (cache)
- MediatR 12 (CQRS), FluentValidation
- Serilog (correlation ID ile loglama)
- Swagger, Docker Compose
- xUnit, Moq, FluentAssertions

## Servisler

- **Ordering.Api:** Siparişi alır, kaydeder, `order.created` yayınlar; sonuç event'ini dinleyip statüyü günceller.
- **Processing.Worker:** `order.created`'ı dinler, ödemeyi simüle eder, `order.completed` / `order.failed` yayınlar.

Her servisin kendi veritabanı var (`orderdb`, `processingdb`).

## Çalıştırma

```bash
docker compose up -d --build
```

| Ne | Adres |
|---|---|
| Swagger | http://localhost:8080/swagger |
| CAP Dashboard | http://localhost:8080/cap |
| RabbitMQ | http://localhost:15673 (`guest`/`guest`) |

## Token'lar

| Token | Kullanıcı | Rol |
|---|---|---|
| `admin-token-123` | admin-1 | Admin |
| `user-token-123` | user-1 | User |
| `user-token-456` | user-2 | User |

## Endpoint'ler

| Metot | Endpoint | Açıklama |
|---|---|---|
| POST | `/api/orders` | Sipariş oluştur (202) |
| GET | `/api/orders/{id}` | Sipariş detayı (cache 10 dk) |
| GET | `/api/orders/{userId}` | Kullanıcının siparişleri (cache 2 dk, yeni siparişte invalidate) |
| GET | `/api/orders` | Admin: tümü, User: kendi siparişleri |
| DELETE | `/api/orders/{id}` | Soft delete (Admin) |

## Öne Çıkanlar

- **Clean Architecture:** Domain, Persistence, Application, Infrastructure, Api katmanları.
- **Outbox:** Sipariş ve event aynı transaction'da yazılır, mesaj kaybolmaz.
- **Retry:** Geçici hatalarda CAP otomatik tekrar dener.
- **Idempotency:** Aynı mesaj iki kez gelse de sipariş bir kez işlenir.
- **Correlation ID:** Tek bir ID ile isteğin iki servisteki tüm logları izlenebilir.
- **Cache:** Statü değişince veya yeni sipariş gelince commit sonrası silinir; Redis kapalıysa DB'ye düşülür.
- **Audit & Soft Delete:** Kim oluşturdu/güncelledi/sildi bilgisi otomatik tutulur.

## Demo Senaryoları

| İstek | Sonuç |
|---|---|
| Normal sipariş | `Completed` |
| `quantity` > 50 | `Failed` (ödeme reddi) |
| `productId: "flaky-product"` | 2 hata, retry, `Completed` |

## Testler

```bash
dotnet test
```