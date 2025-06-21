# Talent Showcase Backend API

A .NET 8 Web API for the Talent Showcase application with comprehensive user management, content creation, and media handling capabilities.

## Features

- **Authentication & Authorization**: JWT-based authentication with refresh tokens
- **User Management**: User registration, login, and profile management
- **Content Management**: Posts, categories, likes, and comments
- **Media Handling**: Image and video upload via Cloudinary
- **Real-time Features**: Redis integration for caching and real-time features
- **Comprehensive Documentation**: Swagger/OpenAPI documentation optimized for Orval

## Technology Stack

- **.NET 8** - Latest .NET framework
- **Entity Framework Core** - ORM for database operations
- **PostgreSQL** - Primary database
- **Redis** - Caching and real-time features
- **JWT** - Authentication and authorization
- **Cloudinary** - Media storage and management
- **Swagger/OpenAPI** - API documentation
- **Serilog** - Structured logging

## Getting Started

### Prerequisites

- .NET 8 SDK
- PostgreSQL
- Redis
- Cloudinary account

### Configuration

1. **Database Connection**: Update the connection string in `appsettings.json`
2. **Redis Connection**: Configure Redis connection string
3. **JWT Settings**: Set your JWT secret key
4. **Cloudinary**: Configure your Cloudinary credentials
5. **CORS**: Update allowed origins for your frontend

### Running the Application

```bash
# Restore dependencies
dotnet restore

# Run database migrations
dotnet ef database update

# Start the application
dotnet run
```

The API will be available at `http://localhost:5000` and Swagger documentation at `http://localhost:5000`.

## API Documentation

### Swagger UI
The API includes comprehensive Swagger documentation with:
- Interactive API testing
- Request/response examples
- Authentication support
- Detailed parameter descriptions
- Response type documentation

### OpenAPI Specification
The OpenAPI specification is available at `/swagger/v1/swagger.json` and is optimized for use with code generation tools like Orval.

## Frontend Integration with Orval

### Orval Configuration
To generate TypeScript client code using Orval, create an `orval.config.ts` file:

```typescript
import { defineConfig } from 'orval';

export default defineConfig({
  talentShowcase: {
    input: {
      target: 'http://localhost:5000/swagger/v1/swagger.json',
    },
    output: {
      mode: 'split',
      target: './src/api/generated',
      schemas: './src/api/generated/model',
      client: 'react-query',
      override: {
        mutator: {
          path: './src/api/mutator/custom-instance.ts',
          name: 'customInstance',
        },
      },
    },
    hooks: {
      afterAllFilesWrite: 'prettier --write',
    },
  },
});
```

### Generated Client Features
- Type-safe API calls
- React Query integration
- Automatic request/response typing
- Error handling
- Authentication token management

## API Endpoints

### Authentication
- `POST /api/v1/auth/register` - User registration
- `POST /api/v1/auth/login` - User login
- `POST /api/v1/auth/refresh-token` - Refresh access token
- `POST /api/v1/auth/revoke-token` - Revoke refresh token

### User Management
- `GET /api/v1/user/get-profile` - Get user profile
- `PATCH /api/v1/user/create-profile` - Update user profile

### Categories
- `GET /api/v1/category` - Get all categories
- `POST /api/v1/category` - Create new category
- `GET /api/v1/category/{id}` - Get category by ID
- `PUT /api/v1/category/{id}` - Update category
- `DELETE /api/v1/category/{id}` - Delete category

### Posts
- `GET /api/v1/post` - Get all posts with pagination
- `POST /api/v1/post` - Create new post
- `GET /api/v1/post/{id}` - Get post by ID
- `PUT /api/v1/post/{id}` - Update post
- `DELETE /api/v1/post/{id}` - Delete post
- `POST /api/v1/post/{id}/like` - Toggle post like
- `GET /api/v1/post/{id}/like-count` - Get post like count
- `GET /api/v1/post/{id}/comment-count` - Get post comment count

### Comments
- `GET /api/v1/post/{postId}/comments` - Get post comments
- `POST /api/v1/post/{postId}/comments` - Create comment
- `PUT /api/v1/post/{postId}/comments/{commentId}` - Update comment
- `DELETE /api/v1/post/{postId}/comments/{commentId}` - Delete comment

### Media
- `POST /api/v1/media/upload-image` - Upload image
- `POST /api/v1/media/upload-video` - Upload video
- `DELETE /api/v1/media/{publicId}` - Delete media

## Response Format

All API responses follow a consistent format:

```json
{
  "success": true,
  "message": "Operation completed successfully",
  "data": { ... },
  "timestamp": "2025-01-01T00:00:00.000Z"
}
```

## Error Handling

The API includes comprehensive error handling with:
- Validation errors (400)
- Authentication errors (401)
- Authorization errors (403)
- Not found errors (404)
- Server errors (500)

All errors include detailed messages and appropriate HTTP status codes.

## Development

### Project Structure
```
├── Controllers/          # API controllers
├── Services/            # Business logic services
├── Repositories/        # Data access layer
├── Models/              # Entity models
├── DTOs/                # Data transfer objects
├── Middleware/          # Custom middleware
├── Data/                # Database context
└── Migrations/          # Database migrations
```

### Adding New Features
1. Create entity model in `Models/`
2. Add DTOs in `DTOs/`
3. Create repository interface and implementation
4. Create service interface and implementation
5. Add controller endpoints
6. Update Swagger documentation

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License

This project is licensed under the MIT License. 