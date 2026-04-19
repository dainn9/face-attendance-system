# 📌 PROJECT ARCHITECTURE GUIDELINES (Microservices + Clean Architecture + DDD + CQRS)

## 🎯 Mục tiêu dự án
Xây dựng hệ thống backend theo kiến trúc Microservices kết hợp Clean Architecture, áp dụng DDD (Domain-Driven Design) và CQRS để đảm bảo:

- Dễ mở rộng (scalability)
- Dễ bảo trì (maintainability)
- Tách biệt rõ business logic và infrastructure
- Hỗ trợ phát triển theo team độc lập

---

# 🧱 1. Kiến trúc tổng thể

## 1.1 Microservices Architecture
Hệ thống được chia thành các service độc lập:

- Auth Service (đăng nhập, JWT, OTP)
- User Service (quản lý người dùng)
- Exam Service (bài thi, câu hỏi, kết quả)
- Notification Service (email, OTP, thông báo)

👉 Mỗi service:
- Deploy độc lập
- Database riêng (Database per Service)
- Giao tiếp qua HTTP/gRPC/Event Bus

---

## 1.2 Clean Architecture trong từng service

Mỗi microservice phải tuân theo cấu trúc:
Presentation Layer
Application Layer
Domain Layer
Infrastructure Layer


### 📌 Rule quan trọng:
- Domain KHÔNG phụ thuộc layer nào khác
- Application chỉ phụ thuộc Domain
- Infrastructure implement interface từ Application/Domain

---

# 🧠 2. Domain-Driven Design (DDD)

## 2.1 Core concepts

- Entity: object có ID (Exam, User)
- Value Object: object không có ID (Email, Score)
- Aggregate Root: root quản lý consistency (ExamAggregate)
- Domain Service: logic phức tạp không thuộc entity

## 2.2 Business rule MUST nằm trong Domain

Ví dụ:
- Không cho nộp bài sau thời gian thi
- Không cho user thi lại nếu chưa đủ điều kiện

---

# ⚡ 3. CQRS Pattern

## 3.1 Tách Read / Write

### Commands (Write side)
- CreateExamCommand
- SubmitExamCommand
- UpdateUserCommand

### Queries (Read side)
- GetExamByIdQuery
- GetUserResultQuery

## 3.2 Rule
- Command: thay đổi state
- Query: không thay đổi state
- Có thể dùng separate model (Read model / Write model)

---

# 🧩 4. Project Structure (Example .NET)
services/
├── ExamService/
│ ├── Exam.API (Presentation)
│ ├── Exam.Application
│ ├── Exam.Domain
│ ├── Exam.Infrastructure
│
├── AuthService/
│ ├── Auth.API
│ ├── Auth.Application
│ ├── Auth.Domain
│ ├── Auth.Infrastructure


---

# 🔌 5. Communication giữa services

- HTTP REST (simple cases)
- gRPC (high performance)
- Message Broker (RabbitMQ / Kafka) for async events

Example events:
- UserCreatedEvent
- ExamSubmittedEvent

---

# 🗄️ 6. Database Rule

- Database per microservice
- Không share DB giữa services
- Nếu cần data khác service → dùng API hoặc event

---

# 🧪 7. Testing Strategy

- Unit Test: Domain logic
- Integration Test: Application + DB
- Contract Test: API between services

---

# 🚫 8. Rules MUST FOLLOW

- Không để business logic trong Controller
- Không truy cập DB trực tiếp từ API layer
- Không cross-reference database giữa services
- Domain phải độc lập hoàn toàn framework

---

# 🚀 9. Architecture Goal

Hệ thống phải đảm bảo:

✔ Easy to scale  
✔ Easy to test  
✔ Easy to replace infrastructure  
✔ Independent services  
✔ Clear business logic separation  

---

# 🧠 10. Mental Model

- Microservices = chia hệ thống
- Clean Architecture = chia code trong service
- DDD = mô hình nghiệp vụ
- CQRS = tách đọc / ghi
