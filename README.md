# Restore - E-commerce Application

A full-stack e-commerce application built with .NET Core API and React TypeScript frontend.

## Project Structure

- `API/` - .NET Core Web API backend
- `client/` - React TypeScript frontend

## Setup Instructions

### Backend Setup

1. Navigate to the `API` directory
2. Copy `appsettings.Development.template.json` to `appsettings.Development.json`
3. Update the configuration with your actual values:
   - Database connection string
   - Stripe API keys (get them from [Stripe Dashboard](https://dashboard.stripe.com/apikeys))

```bash
cd API
cp appsettings.Development.template.json appsettings.Development.json
# Edit appsettings.Development.json with your actual values
```

4. Run the API:
```bash
dotnet run
```

### Frontend Setup

1. Navigate to the `client` directory
2. Install dependencies:
```bash
npm install
```

3. Start the development server:
```bash
npm run dev
```

## Security Note

The `appsettings.Development.json` file is excluded from git to prevent sensitive information (like API keys and database passwords) from being committed to the repository. Always use the template file to create your local development configuration.

## Features

- User authentication and authorization
- Product catalog with filtering and search
- Shopping basket functionality
- Stripe payment integration
- Order management
- Responsive design

## Technologies Used

### Backend
- .NET Core 8
- Entity Framework Core
- PostgreSQL
- Stripe API

### Frontend
- React 18
- TypeScript
- Material-UI
- Redux Toolkit
- React Router
- Stripe Elements 