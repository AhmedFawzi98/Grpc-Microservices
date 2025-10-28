# gRPC Microservices with .NET 9

This is a **demo project** built to practice **hands-on gRPC communication in .NET** between multiple backend services.  
The main goal is to explore **inter-service communication** using gRPC.

---

##  Project Overview

This project simulates a small e-commerce system composed of **four microservices**:

- **Product Service**: Handles product CRUD operations.  
- **ShoppingCart Service**: Manages shopping carts and cart items.  
- **Discount Service**: Provides discount data for products.  
- **Authentication Service** : Provides authentication using **IdentityServer** with **JWT**.

All services communicate **exclusively via gRPC**, using multiple communication patterns like **unary calls**, **client streaming** and **server streaming**.

---

##  Technologies Used

- **.NET 9**
- **gRPC**
- **IdentityServer**
- **Entity Framework Core (InMemory Database)**

---

##  Project Structure

each microservice is kept **simple** and contained within **a single assembly**, organized through clear directory structure for each component.  
This was done intentionally to **focus on gRPC concepts** without the added complexity of layered architectures.

---

##  Documentation

 an overview of the services and their interactions:  
![Services Diagram](Documentation/Services-Diagram.jpg)

---

This project is intended purely for **learning and demonstration**. 
exploring, implementing, and experimenting with **gRPC communication in .NET 9**.
