# 📚 Kitabika (BookStore)

Kitabika is a modern, full-featured online bookstore and library management system built with **ASP.NET Core 9.0**. It follows the **Onion (Clean) Architecture** principles to ensure scalability, maintainability, and testability.

The application serves a dual purpose: it functions as an **E-commerce platform** for selling books and a **Library system** for managing book loans.

---

## 🚀 Features

### 🛒 E-Commerce Module
*   **Product Browsing:** Advanced search and filtering by category, author, and availability.
*   **Shopping Cart:** Persistent shopping cart experience.
*   **Order Management:** Secure checkout process and order history tracking.
*   **Coupons & Discounts:** Admin-managed promotional codes for discounts.

### 📖 Library & Lending Module
*   **Book Loans:** System for users to borrow books for a specific period.
*   **Overdue Tracking:** Automatic calculation of overdue loans.
*   **Availability Status:** Real-time stock and loan status for books.

### 👤 User Features
*   **Authentication:** Secure user registration and login (Custom Auth with BCrypt).
*   **Profile Management:** Update personal details and password.
*   **Social Interactions:** 
    *   **Reviews & Ratings:** Users can rate and review books.
    *   **Favorites:** Mark books as favorites for quick access.
    *   **Wishlist:** Save books for future purchase.

### 🛠️ Admin Panel
A comprehensive dashboard for administrators to manage the platform:
*   **Dashboard:** Key metrics and statistics.
*   **Catalog Management:** CRUD operations for **Books** and **Categories**.
*   **Order Management:** View and update order statuses.
*   **Loan Management:** Track active and returned loans.
*   **User Management:** Manage customer accounts.
*   **Coupon Management:** Create and manage discount coupons.

### 🌍 Other Highlights
*   **Localization:** Multi-language support (English & Turkish supported).
*   **Responsive Design:** Mobile-friendly UI built with **Bootstrap**.
*   **Clean Architecture:** Separation of concerns (Core, Data, Service, Web).

---

## 🏗️ Technical Architecture

The solution is divided into four main projects adhering to the **N-Layer Architecture**:

1.  **BookStore.Core** 🧱
    *   Contains the domain entities, interfaces, enums, and constants.
    *   Has no external dependencies (Pure C#).
    
2.  **BookStore.Data** 💾
    *   Implements data access logic using **Entity Framework Core**.
    *   Handles Database Context, Migrations, and Repositories.
    
3.  **BookStore.Service** ⚙️
    *   Contains business logic and DTOs (Data Transfer Objects).
    *   Uses **AutoMapper** for object mapping.
    *   Implements interfaces defined in the Core layer.

4.  **BookStore.Web** 🌐
    *   The Presentation Layer (ASP.NET Core MVC).
    *   Handles HTTP requests, View rendering, and UI logic.
    *   Configures Dependency Injection and Middleware.

---

## 💻 Tech Stack

*   **Framework:** [.NET 9.0](https://dotnet.microsoft.com/download/dotnet/9.0)
*   **Web Framework:** ASP.NET Core MVC
*   **Database:** Microsoft SQL Server
*   **ORM:** Entity Framework Core 9.0
*   **Mapping:** AutoMapper
*   **Security:** BCrypt.Net-Next (Password Hashing)
*   **Frontend:** Razor Views, Bootstrap 5, jQuery, jQuery Validation
*   **IDE:** Visual Studio 2022 / VS Code

---

## 🛠️ Getting Started

### Prerequisites
*   [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
*   SQL Server (LocalDB or Standard)

### Installation

1.  **Clone the repository**
    ```bash
    git clone https://github.com/yourusername/Kitabika.git
    cd Kitabika
    ```

2.  **Configure Database**
    Update the connection string in `BookStore.Web/appsettings.json`:
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=.;Database=KitabikaDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
    }
    ```

3.  **Run Migrations**
    Apply the database schema:
    ```bash
    cd BookStore.Web
    dotnet ef database update --project ../BookStore.Data
    ```

4.  **Run the Application**
    ```bash
    dotnet run
    ```
    The application will be available at `https://localhost:7148` (or similar port).

---

## 📸 Project Structure

```text
Kitabika/
├── 📂 BookStore.Core      # Domain Layer (Entities, Interfaces)
├── 📂 BookStore.Data      # Data Layer (EF Core Context, Repositories)
├── 📂 BookStore.Service   # Business Layer (Services, DTOs)
└── 📂 BookStore.Web       # Presentation Layer (MVC Controllers, Views)
```

---

## 🤝 Contribution

Contributions are welcome! Please feel free to submit a Pull Request.

1.  Fork the project
2.  Create your feature branch (`git checkout -b feature/AmazingFeature`)
3.  Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4.  Push to the branch (`git push origin feature/AmazingFeature`)
5.  Open a Pull Request

---

## 📄 License

This project is licensed under the MIT License.
