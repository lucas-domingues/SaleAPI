# Project Setup Guide

This README explains how to set up and run the project, including its dependencies, database, RabbitMQ, and application startup steps.

## **Prerequisites**

Before starting, ensure you have the following installed:
- [.NET SDK](https://dotnet.microsoft.com/download) (version specified in the project)
- [Docker](https://www.docker.com/)
- [PostgreSQL](https://www.postgresql.org/download/)

## **Steps to Run the Project**

### 1. **Clone the Repository**
```bash
git clone https://github.com/lucas-domingues/SaleAPI.git
cd Sales.API
```

### 2. **Database Setup**

#### Using PostgreSQL:
1. Ensure PostgreSQL is running locally or via Docker.
2. Update the connection string in `appsettings.json` located in the main project folder:
   ```json
   "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=SalesDB;Username=your_user;Password=your_password;"
   }
   ```
3. Apply migrations to create the database schema:
   ```bash
   dotnet ef database update
   ```

### 3. **RabbitMQ Setup**

#### Run RabbitMQ using Docker:
```bash
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:management
```
- Access the RabbitMQ Management UI at [http://localhost:15672](http://localhost:15672).
- Default credentials:
  - **Username**: `guest`
  - **Password**: `guest`

### 4. **Environment Configuration**

Ensure the RabbitMQ connection string is correctly configured in `appsettings.json`:
```json
"RabbitMQ": {
    "Host": "localhost",
    "Username": "guest",
    "Password": "guest"
}
```

### 5. **Run the Application**

To start the application, use the following command:
```bash
dotnet run
```

The API will be available at `http://localhost:5000` (or the port specified in the `launchSettings.json`).

### 6. **Testing the Application**

#### Access Endpoints:
Use a tool like [Postman](https://www.postman.com/) or [Swagger](http://localhost:5000/swagger) (if enabled) to interact with the API endpoints.

#### Example Endpoints:
- **Products**: `/api/products`
- **Cart**: `/api/cart`
- **Checkout**: `/api/checkout`

### 7. **Stopping the Services**

To stop RabbitMQ and clean up resources:
```bash
docker stop rabbitmq && docker rm rabbitmq
```

---

## **Troubleshooting**

1. **Database Connection Issues**:
   - Ensure PostgreSQL is running and the connection string matches the instance.

2. **RabbitMQ Connection Issues**:
   - Verify RabbitMQ is running with `docker ps`.
   - Ensure the `Host`, `Username`, and `Password` in `appsettings.json` are correct.

3. **Ports Already in Use**:
   - Check for conflicts using `netstat` or adjust the ports in the Docker command and `appsettings.json`.

---

Feel free to reach out for further assistance!
