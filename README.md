# Student Management API

This is a RESTful API built using C# and ASP.NET Core for managing students. It consists of two main components: Authentication API and Student API.

## Authentication API

  - Login API endpoint for user authentication. Username:"admin" & Password: "password123".
  - JWT token generation on successful validation.
  - Provision for refresh token to maintain user sessions.

## Student API

  - CRUD (Create, Read, Update, Delete) operations for managing student records.
  - Only users with the "teacher" role can perform CRUD operations.
  - In-memory database for testing and development.  - To access the protected Student API endpoints, you need to include the JWT token obtained from the Authentication API in the Authorization header of your requests with the "Bearer" scheme in Postman.

	Example Header:						
    Authorization: Bearer YOUR_JWT_TOKEN_HERE