# Tickets Wizard API
Check out the API Swagger documentation [here](https://ticketsapi.somee.com/swagger/index.html)

## Overview

Tickets Wizard API is an ASP.NET Core Web API for managing events, tickets, performers, and users. It supports JWT authentication and is designed to be consumed by my [Tickets Wizard Angular App](https://github.com/abtlb/TicketsWizardAngular).

## Features

- **Event Management:**  
  Create, update, delete, and list events with details such as date, location, and description.

- **Ticket Services:**  
  Issue, reserve, and manage tickets for events. Includes support for ticket status, pricing, and availability.

- **Performer Management:**  
  Add and manage performers linked to events. Retrieve performer profiles and event participation history.

- **User Authentication & Authorization:**  
  Secure endpoints using JWT-based authentication. Supports user registration, login, and role-based access (admin/user).

- **Admin Services:**  
  Admin users can manage all entities, view system statistics, and perform bulk operations.

- **Search & Filtering:**  
  Search events, tickets, and performers by various criteria (date, location, name, etc.).

- **Swagger/OpenAPI Documentation:**  
  Interactive API documentation for testing and exploring endpoints.

- **CORS Support:**  
  Configured for integration with Angular and other front-end frameworks.

## Authentication

- The API uses JWT Bearer authentication.
- Obtain a token via the login endpoint (see Swagger for details).
- Include the token in the `Authorization` header as `Bearer <token>` for protected endpoints.

## Project Structure

- `Controllers/` – API controllers for each entity
- `Models/` – Entity models
- `Data/` – Entity Framework Core DbContext
- `RequestInput/` – Request models
- `Program.cs` – Application entry point and configuration
