# ISCWA – Inter-School Competition Web Application

ISCWA is a full-stack web application built to digitally manage inter-school academic, cultural, and sports competitions. Designed with a 3-tier architecture, ISCWA provides role-based access to admins, schools, and event coordinators, allowing secure registration, score input, real-time leaderboard generation, and feedback collection.

Built using ASP.NET Web Forms, C#, and SQL, ISCWA simplifies outdated paper-based processes with a robust, validated, and user-friendly experience for all stakeholders.

---

## Features

- Role-Based Authentication (Admin, School, Event Coordinator)
- Event & Fest Creation with Eligibility Criteria
- School Participant Registration
- Real-time Score Input and Leaderboard Display
- Top 3 Winners Calculation
- Feedback Collection from Participating Schools
- Password Reset via Email (SMTPClient)
- Client-Side & Server-Side Validations
- GridView, Calendar, and Session Control Integration
- Stored Procedures, Exception Handling, ViewState
- Built with Full SDLC, Agile Model, and COCOMO Estimation

---

## Tech Stack

### Backend:
- C# with ASP.NET Web Forms (3-tier architecture)
- ADO.NET for database operations
- MySQL / SQL Server (via SSMS or Docker)

### Frontend:
- ASP.NET Web Forms
- HTML, CSS, JavaScript (Validation)
- GridView and Calendar Control for data management

### Architecture:
- 3-Tier: Presentation → Business Logic → Data Access

### Tools:
- Visual Studio (IDE)
- SQL Server Management Studio / MySQL Workbench
- MockFlow (UI wireframes), Visual Paradigm (UML Diagrams)
- Draw.io (ER/Class diagrams)

---

## Project Structure

```bash
ISCWA/
├── App_Code/                # C# classes (UserClass, AdminClass, etc.)
├── .aspx Pages              # UI pages for each user role
├── SQL_Scripts/             # Table creation & stored procedures
├── bin/                     # Compiled DLLs
├── Web.config               # DB connection string & settings
└── README.md                # Project documentation
```

---

## Setup Instructions

### Prerequisites

- Visual Studio 2019 or later
- .NET Framework 4.8 (Minimum 4.5)
- SQL Server Management Studio (or MySQL Workbench via Docker for Mac)
- Git (for source control)
- NuGet Packages to install via Package Manager:
    ```bash
    Install-Package System.Data.SqlClient
    Install-Package Microsoft.AspNet.ScriptManager.MSAjax
    Install-Package Microsoft.Web.Infrastructure
    Install-Package AjaxControlToolkit
    ```
- SMTP Mail settings for password reset functionality
- Optional: Docker (for DB containerization)

---

### Setup on Windows

1. Clone the repository
```bash
git clone https://github.com/your-username/ISCWA.git
```
2. Use the provided `SQLqueries.sql` script to create the required database and tables, or you can create it manually using MySQL Workbench or any client.
3. Open the .sln file in Visual Studio
4. In Web.config, configure your database connection string.
5. Run the application via IIS Express
6. Open http://localhost:[port]/ in your browser

---

### Setup on macOS (Using Parallels or Docker)

1. Install Parallels Desktop and create a Windows 10/11 VM
2. Inside the VM:
   - Install Visual Studio with .NET Framework 4.8 development tools
   - Install SQL Server Management Studio
3. On your macOS (host system), install Docker Desktop for Mac
4. Open Terminal on macOS and run:
   ```bash
   docker run --name iscwa-mysql -e MYSQL_ROOT_PASSWORD=admin -p 3306:3306 -d mysql
   ```
5. Use the provided `SQLqueries.sql` script to create the required database and tables, or you can create it manually using MySQL Workbench or any client.
6. In Visual Studio (inside Parallels), configure the connection string to connect to:
   ```bash
   Server=host.docker.internal; Port=3306; Database=iscwa_db; Uid=root; Pwd=admin;
   ```
7. Continue with the Windows setup steps inside the VM

Note: On macOS, both Parallels (for running ASP.NET Web Forms) and Docker (to host the MySQL backend) are required. The Docker container must be created manually before running the project.

---

## Login Credentials
### This app includes 3 roles:

1. Admin: Pre-registered via database
2. School: Register via UI
3. Event Coordinator: Register via UI

Use the appropriate login pages as per your role.

---

## Contributors

- Jay Malhotra – BCA, Bennett University