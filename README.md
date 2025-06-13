# .NET Starter Template

A robust and scalable .NET starter template following **Onion Architecture**, integrated with essential tools like **OpenTelemetry**, **Swagger**, and **Rate Limiter** to provide a production-ready foundation for building modern web applications.

## Features 🛠️

**Onion Architecture** - Clean and maintainable code structure  
**OpenTelemetry** - Distributed tracing and monitoring  
**Swagger** - API documentation for easy testing and interaction  
**Rate Limiting** - Prevent abuse and improve performance  
**Logging** - Structured logging with built-in integrations  
**Docker Support** - Containerized development and deployment  

## Getting Started

### **Prerequisites**
Ensure you have the following installed before running the project:
- [.NET SDK](https://dotnet.microsoft.com/download) (LTS version)
- [Docker](https://www.docker.com/get-started) (Optional, for containerized deployment)

### **Installation & Setup**
1. Clone the repository:
   ```sh
   git clone https://github.com/onrcanogul/dotnet-webapi-starter-template.git
   cd your-repo-folder
   ```

2. Install dependencies:
   ```sh
   dotnet restore
   ```

3. Configure environment variables (e.g., database connection, telemetry settings).

4. Run the project:
   ```sh
   dotnet run
   ```

### **Docker Setup**
To run the project in a **Docker container**, use:
```sh
docker build -t your-app-name .
docker run -p 5000:5000 your-app-name
```

## Architecture

This template follows the **Onion Architecture**, which enforces separation of concerns and enables maintainability.

**Core Layer**  
- Domain models

**Infrastructure Layer**  
- Database interactions
- Middlewares  

**Application Layer**  
- Use cases  
- Service interfaces  

**API Layer**  
- Controllers  


## Swagger UI 📖
After running the project, access Swagger UI at:
```
http://localhost:5000/swagger
```
