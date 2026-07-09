# Task Manager (Spring Boot)

CRUD REST API for managing tasks. Spring Boot 3, Spring Data JPA, H2
in-memory DB, Bean Validation, Actuator health endpoint.

## Run locally
```bash
mvn spring-boot:run
```

## Endpoints
| Method | Path              | Description       |
|--------|-------------------|--------------------|
| GET    | /api/tasks        | List all tasks     |
| GET    | /api/tasks/{id}   | Get one task       |
| POST   | /api/tasks        | Create a task      |
| PUT    | /api/tasks/{id}   | Update a task      |
| DELETE | /api/tasks/{id}   | Delete a task       |
| GET    | /actuator/health  | Health check        |

## Test
```bash
mvn test
```

## Build image
```bash
docker build -t task-manager .
docker run -p 8080:8080 task-manager
```
