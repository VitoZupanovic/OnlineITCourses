# Online IT Courses Web API

A university project built with ASP.NET Core Web API and SQL Server for browsing online IT courses and managing course applications.

## Features

- Course management with search and pagination
- Course groups and instructor management
- Assigning instructors to courses
- User registration, login, and password changes
- JWT authentication and role-based authorization
- Course applications and application status management
- Application logs
- Swagger API documentation
- Basic HTML and JavaScript pages for login and viewing logs

## Technologies

- C# and .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- JWT authentication
- Swagger
- HTML, CSS, JavaScript, and jQuery

## Project structure

- OnlineITCourses/WebAPI — API controllers, authentication, and static pages
- OnlineITCourses/DAL — database models and repositories
- Database/Database.sql — database schema and sample data

## Setup

### 1. Prepare the database

Create an empty SQL Server database named OnlineITCourses.
Run Database/Database.sql against that database to create the tables and sample data.

### 2. Configure local settings

Open OnlineITCourses/OnlineITCourses.sln in Visual Studio 2022.

Right-click the WebAPI project and select Manage User Secrets.
Add the following configuration, replacing the example values with your local settings:

```json
{
  "ConnectionStrings": {
    "DefaultConn": "Server=.;Database=OnlineITCourses;Integrated Security=True;TrustServerCertificate=True"
  },
  "JWT": {
    "SecureKey": "REPLACE_WITH_A_RANDOM_SECRET_OF_AT_LEAST_32_BYTES"
  }
}
```

The example connection string uses Windows authentication.
Adjust the server name and authentication settings for your SQL Server installation.

### 3. Run the API

Set WebAPI as the startup project and run it in Development mode.
Open /swagger at the application's local address.

### 4. Create an account

Use POST /api/auth/register to register a local test account.
New accounts receive the User role.

To test administrator features, update that account's RoleId to 1
in your local database, then log in again to obtain a new token.

## Configuration

Database credentials and the JWT signing key are supplied through
local User Secrets and are not included in this repository.

The application requires a configured SQL Server database to use
database endpoints and authentication.