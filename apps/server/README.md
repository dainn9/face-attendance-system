
README của bạn vừa là **hướng dẫn kiến trúc chung** (microservices + Clean + DDD + CQRS), vừa **gán vai trò ngắn gọn cho Auth Service**. Dưới đây là phần “reverse-engineer”, phạm vi trách nhiệm, và bản thiết kế nền tảng để làm **base** cho phát triển sau.

---

## 1. Kiến trúc hệ thống (suy ra từ README)

**Bối cảnh:** Hệ thống được tách thành các service độc lập, mỗi service có **database riêng**, giao tiếp **HTTP / gRPC / message broker**, và **không share DB** giữa các service.

**Vị trí Auth Service trong bức tranh:**

| Thành phần | Ý nghĩa |
|------------|---------|
| **Auth Service** | Xác thực người dùng, phát hành/đổi **JWT**, luồng **OTP** (theo mục 1.1). |
| **User Service** | Quản lý hồ sơ người dùng (profile, vai trò có thể ở đây hoặc đồng bộ với Auth — cần quyết định rõ ranh giới). |
| **Notification Service** | Gửi email/SMS/push — thường **thực sự gửi** OTP; Auth có thể **tạo/verify** OTP và publish event hoặc gọi API. |
| **Exam Service** | Không thuộc Auth; chỉ **tiêu thụ JWT** do Auth phát hành (qua gateway hoặc introspection tùy thiết kế). |

**Luồng phụ thuộc trong Clean Architecture (trong một service):**  
API → Application → Domain; **Infrastructure** implement interface từ Application/Domain và **không** để business trong Controller hay truy cập DB trực tiếp từ API.

---

## 2. Trách nhiệm Auth Service (theo README + ranh giới hợp lý)

**Trong phạm vi README (rõ ràng):**

- **Đăng nhập / xác thực** (credential → xác minh → cấp token).
- **JWT**: access token (và thường kèm **refresh token** trong thực tế production — README không nêu nhưng nên có trong base).
- **OTP**: tạo thử thách OTP, xác minh, hết hạn, giới hạn số lần (chi tiết “gửi qua kênh nào” có thể ủy quyền Notification).

**Thường gắn với Auth nhưng cần tách rõ với User Service (để scalable):**

- **Định danh đăng nhập** (username/email + mật khẩu hash, hoặc liên kết IdP) — *nếu User Service là “source of truth” cho profile*, Auth vẫn có thể giữ bảng **credential / identity** tối thiểu hoặc chỉ lưu `external_user_id` + secrets token.
- **Phiên / refresh token persistence** (thu hồi token, rotation).
- **Sự kiện domain** (ví dụ `UserLoggedIn`, `OtpVerified`) để các service khác phản ứng async — phù hợp mục 5 (HTTP/gRPC/Event Bus).

**Không nên nhét vàn Auth nếu muốn giữ microservice sạch:** toàn bộ quản lý hồ sơ, avatar, lớp học — đó là **User / Exam** service.

---

## 3. Thiết kế lại nền tảng: Clean Architecture + DDD + CQRS

### Clean Architecture (trong Auth)

- **Domain:** quy tắc nghiệp vụ thuần (mật khẩu đủ mạnh, OTP chưa hết hạn, refresh token hợp lệ, không reuse refresh đã revoke…).
- **Application:** use case — **Commands** (ghi state) và **Queries** (đọc), orchestration, transaction boundary.
- **Infrastructure:** EF Core (hoặc DB khác), JWT signing keys, clock, gửi OTP qua Notification client, outbox cho events.
- **API:** HTTP endpoints, mapping request → command/query, không chứa rule nặng.

### DDD (Auth)

- **Aggregates gợi ý:**  
  - `Credential` / `LoginIdentity` (một aggregate cho “tài khoản đăng nhập” + mật khẩu hash + khóa).  
  - `OtpSession` hoặc `OneTimePasswordChallenge` (một aggregate cho một lần gửi OTP / verify).  
  - `RefreshTokenSession` (nếu refresh là entity có vòng đời riêng) — có thể là entity con hoặc aggregate tùy consistency boundary.
- **Value objects:** `Email`, `HashedPassword`, `JwtId`, `OtpCode` (hoặc hash của code), `TimeRange`, v.v.
- **Domain events:** sau khi commit transaction — ví dụ `PasswordChanged`, `RefreshTokenRotated` — phù hợp async với các service khác.

### CQRS — **có áp dụng**

Auth có nhiều **ghi** (login, refresh, revoke, verify OTP) và **đọc** (introspection nhẹ, lấy public keys, đôi khi read model cho admin). Tách:

- **Commands:** `RegisterCredentials`, `Login`, `RefreshToken`, `RevokeSession`, `RequestOtp`, `VerifyOtp`, `ChangePassword`, …
- **Queries:** `GetJwks` (public keys), `ValidateToken` (nếu làm validation tại Auth), `GetActiveSessions` (nếu cần), …

Có thể **dùng chung một DB** với model đọc/ghi khác nhau (projection đơn giản) hoặc sau này tách read replica — không bắt buộc ngày đầu.

---

## 4. Cây thư mục đề xuất (.NET, production-ready, làm BASE)

Solution-level (mỗi folder là một project `.csproj`):

```text
services/auth-service/
├── src/
│   ├── Auth.Api/
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json
│   │   ├── Controllers/                    # hoặc Endpoints/ nếu Minimal API thuần
│   │   ├── Middleware/
│   │   ├── Filters/
│   │   ├── Extensions/                     # DI registration cho API-only
│   │   └── Properties/
│   │
│   ├── Auth.Application/
│   │   ├── Abstractions/
│   │   │   ├── Persistence/
│   │   │   │   ├── IUnitOfWork.cs
│   │   │   │   └── IAuthDbContext.cs       # nếu cần abstract DbContext
│   │   │   ├── Security/
│   │   │   │   ├── ITokenService.cs
│   │   │   │   ├── IPasswordHasher.cs
│   │   │   │   └── IClock.cs
│   │   │   ├── Messaging/
│   │   │   │   ├── IEventPublisher.cs
│   │   │   │   └── IOutboxStore.cs         # optional: transactional outbox
│   │   │   └── External/
│   │   │       └── INotificationGateway.cs
│   │   ├── Common/
│   │   │   ├── Behaviors/                  # Validation, Logging, Transaction (MediatR pipeline)
│   │   │   ├── Exceptions/
│   │   │   └── Models/
│   │   ├── Contracts/                    # DTOs trả ra API (hoặc đặt riêng Auth.Contracts)
│   │   ├── Features/
│   │   │   ├── Auth/
│   │   │   │   ├── Commands/
│   │   │   │   │   ├── Login/
│   │   │   │   │   ├── RefreshToken/
│   │   │   │   │   ├── RevokeToken/
│   │   │   │   │   └── ...
│   │   │   │   └── Queries/
│   │   │   │       └── GetJwks/
│   │   │   └── Otp/
│   │   │       ├── Commands/
│   │   │       │   ├── RequestOtp/
│   │   │       │   └── VerifyOtp/
│   │   │       └── Queries/                # ít; có thể gộp vào command response
│   │   └── DependencyInjection.cs
│   │
│   ├── Auth.Domain/
│   │   ├── Entities/
│   │   ├── Aggregates/                   # optional: rõ aggregate roots
│   │   ├── ValueObjects/
│   │   ├── Events/
│   │   ├── Services/                     # Domain services (interface + impl thuần)
│   │   ├── Exceptions/
│   │   └── Specifications/               # optional
│   │
│   ├── Auth.Infrastructure/
│   │   ├── Persistence/
│   │   │   ├── Configurations/           # EF Fluent configs
│   │   │   ├── Migrations/
│   │   │   ├── Repositories/
│   │   │   ├── Outbox/                   # nếu dùng outbox
│   │   │   └── AuthDbContext.cs
│   │   ├── Security/
│   │   │   ├── JwtTokenService.cs
│   │   │   └── PasswordHasher.cs
│   │   ├── Messaging/
│   │   │   ├── RabbitMq/                 # hoặc MassTransit folder
│   │   │   └── EventPublisher.cs
│   │   ├── External/
│   │   │   └── NotificationGateway.cs
│   │   ├── Time/
│   │   │   └── SystemClock.cs
│   │   └── DependencyInjection.cs
│   │
│   └── Auth.Contracts/                   # optional: shared với API Gateway / consumers
│       ├── Events/
│       └── RequestsResponses/            # nếu publish package NuGet nội bộ
│
├── tests/
│   ├── Auth.Domain.Tests/
│   ├── Auth.Application.Tests/
│   └── Auth.Api.IntegrationTests/
│
├── Directory.Build.props                 # version, nullable, analyzers chung
└── Auth.sln
```

**Ghi chú mở rộng (khi team lớn hơn):** có thể thêm `Auth.Worker` (consumer outbox / background jobs) mà không đụng `Auth.Api`.

---

## 5. Giải thích từng layer

| Layer | Vai trò | Phụ thuộc |
|-------|---------|-----------|
| **Auth.Domain** | Entity/VO, invariant, domain events, domain services thuần. Không EF, không HTTP. | Không phụ thuộc project khác. |
| **Auth.Application** | CQRS handlers, validation, orchestration, gọi interface repository/UoW/token. | Chỉ Domain + abstractions. |
| **Auth.Infrastructure** | EF, JWT implementation, gửi message, HTTP client tới Notification, đọc config. | Application abstractions + Domain. |
| **Auth.Api** | Endpoints, auth scheme, mapping, problem details; **không** chứa rule nghiệp vụ. | Application (+ Infrastructure qua DI extension). |
| **Auth.Contracts** (tuỳ chọn) | Event contracts / DTO dùng chữ ký public — tránh reference trực tiếp `Auth.Api`. | Tối giản dependencies. |

---

## 6. Gợi ý domain model (Auth-centric)

**Aggregates / entities (có thể tinh giản tùy “User Service giữ gì”):**

- **`LoginIdentity` (aggregate root)**  
  - Id, `Email` (VO), `PasswordHash` (VO), `FailedLoginCount`, `LockedUntil`, …  
  - Hành vi: đổi mật khẩu, khóa tài khoản, reset sau OTP, v.v.

- **`RefreshToken` (có thể entity con hoặc aggregate riêng)**  
  - Token hash, family id (rotation), expires, revoked, replaced by…

- **`OtpChallenge` (aggregate root)**  
  - Purpose (login, reset password), destination (masked), code hash, expires, attempt count, consumed flag.

**Value objects:** `Email`, `PasswordHash`, `RefreshTokenHash`, `OtpCodeHash`, `UtcInstant` (hoặc dùng `IClock` ở application).

**Domain events (ví dụ):** `UserAuthenticated`, `RefreshTokenRotated`, `OtpVerified`, `AccountLocked`.

README của bạn không mô tả chi tiết schema; mô hình trên **khớp** “đăng nhập, JWT, OTP” và vẫn **mở** để sau này gắn OAuth2/OpenId hoặc tách User Service rõ ràng (identity reference thay vì duplicate profile).

---

Nếu bạn muốn bước tiếp theo là **scaffold solution thật trong repo** (`Auth.sln` + các `.csproj` + `Directory.Build.props` + package references MediatR/FluentValidation), nói rõ phiên bản .NET đang dùng (ví dụ .NET 8/9) và có dùng **Minimal API hay Controller** làm mặc định — mình có thể căn chỉnh cây thư mục cho khớp 100% với toolchain của bạn.