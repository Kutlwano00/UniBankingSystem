# UniBankingSystem

## University Banking Management System

UniBankingSystem is a web-based banking management system developed to provide university students, staff, and other users with access to essential banking services through an online platform. The system includes role-based functionality for administrators, clients, consultants, and financial advisors.

The project aims to make banking operations easier to manage by providing features such as money transfers, transaction history, user management, notifications, customer reviews, and financial advice.

## Features

### Client Features
- Register and log in to the system.
- Update personal profile information.
- Access personal banking information.
- Send money using the transfer functionality.
- Access e-wallet services.
- View transaction history and statements.
- Receive notifications.
- View financial advice.
- Submit ratings and reviews.

### Administrator Features
- Create users.
- Manage and view registered users.
- View customer information.
- View consultants and financial advisors.
- View customer reviews.
- Generate system reports.

### Consultant Features
- Access the consultant dashboard.
- Create users.
- View customer reviews.
- Generate system reports.

### Financial Advisor Features
- Access the financial advisor dashboard.
- Support users through financial advice functionality.

### General Features
- Role-based navigation.
- Authentication and logout functionality.
- Responsive user interface.
- Sidebar navigation with active-page highlighting.

## Technologies Used

- **C#** – Application programming language.
- **ASP.NET Core MVC** – Web application framework.
- **Razor Views** – Dynamic user interface rendering.
- **HTML5 and CSS3** – Page structure and styling.
- **Bootstrap** – Responsive user interface components.
- **JavaScript** – Client-side functionality.
- **Font Awesome** – Navigation icons.
- **Entity Framework Core** – Database access and object-relational mapping, where configured.
- **SQL Server** – Database management, where configured.
- **ASP.NET Core Identity** – Authentication and role management, where configured.

## User Roles

The system supports the following application roles:

| Role | Purpose |
|---|---|
| Admin | Manages users and administrative functions. |
| User | Accesses personal banking services. |
| Consultant | Accesses consultant tools and reporting functions. |
| FAdvisor | Accesses financial advisor functionality. |

Access to protected pages should be enforced through server-side authorization.

## Getting Started

### Prerequisites

Before running the project, ensure you have:

- Visual Studio 2022 or later.
- The .NET SDK version targeted by the project.
- SQL Server and SQL Server Management Studio, if using SQL Server.
- Git for version control.

### Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/YOUR-USERNAME/UniBankingSystem.git
   ```

2. Open the project solution (`.sln`) in Visual Studio.

3. Restore the NuGet packages.

4. Configure the database connection string in `appsettings.json` to match your local SQL Server configuration.

5. Open the Package Manager Console and run the database migrations if the project uses Entity Framework Core migrations:

   ```powershell
   Update-Database
   ```

6. Build the solution.

7. Run the application using Visual Studio or the configured .NET launch profile.

> **Note:** Replace `YOUR-USERNAME` with your GitHub username. Database setup and migrations depend on the project's actual configuration.

## Security

- Use authentication to identify signed-in users.
- Apply role-based authorization to protected controllers and actions.
- Keep database credentials and other secrets out of source control.
- Do not use real banking credentials or sensitive customer information in a development environment.

## Project Purpose

This project demonstrates the development of a role-based web application using ASP.NET Core MVC. It brings together web development, authentication, database integration, user management, and banking-related functionality in a university banking environment.

## Future Improvements

Potential enhancements include:

- Improved dashboard analytics.
- Enhanced transaction validation and audit trails.
- Stronger security and fraud-prevention controls.
- Improved accessibility and mobile responsiveness.
- Automated unit and integration testing.
- Deployment to a cloud hosting platform.

## Author

**Kutlwano Tshabalala**

BSc in Information Technology, specialising in Computer Science, from the University of the Free State.

## Disclaimer

UniBankingSystem is an educational software project. It is not a production banking platform and should not be used to process real financial transactions or store real banking credentials.
