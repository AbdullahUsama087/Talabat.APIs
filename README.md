# Talabat.APIs.Solution

#	Talabat API App  : 
•	Developed web application using ASP.NET API with clean architecture with core layers dedicated to different responsibilities.
•	 Using Repository and Unit of Work Patterns to separates data access logic from business logic, providing more straightforward, organized interactions with the database, supporting transaction management, and enabling testing without a real database. The repository pattern simplifies CRUD operations, while the Unit of Work pattern ensures that multiple operations are wrapped in a single transaction.
•	Using JWT-based tokens to secure API access, managing claims and roles (e.g., Admin, User).
•	Data validation with attributes (e.g., required fields, custom validators) ensures data integrity before reaching the business layer
•	Implement pagination to manage large datasets and Utilize caching mechanisms
•	Managing Users can filter products based on attributes like category, price range.
•	Managing Users can sort products by price, name.
•	Creating a payment service that integrates with third-party payment gateways
