# Ocelot Gateway Microservices

This project demonstrates a microservices architecture using the Ocelot API Gateway in .NET Core with comprehensive support for routing, authentication, authorization, load balancing, and WebSocket handling. The API Gateway serves as a centralized entry point, streamlining management and security for various microservices within the system.

## Overview

The Ocelot Gateway Microservices project showcases how to utilize Ocelot as an API gateway to manage a suite of microservices effectively. With features like JWT-based authentication, role-based authorization, and WebSocket support, this setup provides a robust foundation for secure, scalable microservices architectures.

## Features

- **API Gateway Routing**: Efficient routing of HTTP and WebSocket .
- **Authentication & Authorization**: JWT Bearer token authentication and role-based authorization for fine-grained access control.
- **Load Balancing**: Configurable load balancing to distribute requests across multiple service instances.
- **Rate Limiting**: Controls to manage request rates and prevent abuse of the gateway.
- **WebSocket Support**: Handles WebSocket connections for real-time data flow between clients and services.

## Project Structure

- **ApiGateway**: Contains the Ocelot configuration and middleware setup for secure, authorized access.
- **Microservices**:
  - **IdentityServerApi**: Handles user authentication and JWT token generation.
  - **ProductWebApi**: Manages product-related data.
  - **CustomerWebApi** & **CustomerWebApi2**: Handles customer data and operations.
  - **OrderWebApi**: Manages order-related functionalities.
- **WebSocket Components**:
  - **WebSocketsChatApp**: Client-facing WebSocket application for real-time chat features.
  - **WebSocket.Server**: Manages server-side WebSocket connections and real-time communication logic.

## Getting Started

### Prerequisites

- **.NET Core 6**
- **Visual Studio 2022**

### Installation

#### Step 1: Clone the Repository

```bash
git clone https://github.com/merveozan/Ocelot-Gateway-Microservices.git
cd Ocelot-Gateway-Microservices
```

#### Step 2: Run the Solution with Docker

1. Run the services using Docker Compose:

```bash
docker-compose up -d
```

2. Access the application:
   - **API Gateway**: [http://localhost:7000](http://localhost:7000)
   - **Customer API**: [http://localhost:7001](http://localhost:7001)
   - **Customer API 2**: [http://localhost:7002](http://localhost:7002)
   - **Order API**: [http://localhost:7003](http://localhost:7003)
   - **Product API**: [http://localhost:7004](http://localhost:7004) 
   - **Identity Server**: [http://localhost:7005](http://localhost:7005)
   - **WebSocket Chat App**: [http://localhost:7007](http://localhost:7007)
  

3. The services should be accessible without additional building steps since Docker Compose handles the build and setup process automatically.

#### Step 3: Testing the Setup

A ready-to-use **Postman collection** is included so you can try all flows quickly.
  
You can test all features through the **API Gateway** at `http://localhost:7000`:

- **Load Balancing (CustomerWebApi)**: Send multiple requests to `/api/Customer`. Requests will be distributed across `CustomerWebApi` and `CustomerWebApi2` using the **RoundRobin** strategy.

- **Rate Limiting (OrderWebApi)**: Test rate limiting by making repeated requests to `/api/Order`. After exceeding the defined request limit, you will receive a `429 Too Many Requests` response.

- **Authorization (ProductWebApi)**:
    1. Obtain a JWT token by sending credentials to **IdentityServerApi** at `/api/Auth`:
       ```json
       POST http://localhost:7000/api/Auth
       {
           "username": "admin",
           "password": "admin123"
       }
       ```
    2. Use the retrieved token to access protected endpoints like `/api/Product`. Without a valid token, you will receive a `401 Unauthorized` response.

- **Swagger UI**: Explore and test API endpoints via Swagger:
    - Product API: [http://localhost:7004/swagger/](http://localhost:7004/swagger/)
    - Customer API: [http://localhost:7001/swagger/](http://localhost:7001/swagger/)
    - Order API: [http://localhost:7003/swagger/](http://localhost:7003/swagger/)


### Troubleshooting

#### 502 Bad Gateway

- Ensure all microservices are running.
- Verify `ocelot.json` for correct paths, schemes, and ports.
- Check firewall settings and make sure ports are open.

#### 401 Unauthorized

- Make sure JWT tokens are correctly configured and valid.
- Verify that the token has necessary claims and roles for the endpoint.
