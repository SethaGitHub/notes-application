## Prerequisites

Ensure you have the following installed on your system:

- [Docker](https://docs.docker.com/get-docker/)
- [Docker Compose](https://docs.docker.com/compose/install/)

## Getting Started

### 1. Clone the Repository

```sh
git clone https://github.com/SethaGitHub/notes-application.git
cd notes-application
```

### 2. Build the Project

```sh
docker-compose build
```

### 3. Start the Containers

```sh
docker-compose up
```

This will start all necessary services defined in the `docker-compose.yml` file.

### 4. Access the Application

Once the services are up, the application should be accessible at:

```
http://localhost:80
```

Replace `PORT` with the actual port configured in your `docker-compose.yml` file.

## Stopping the Application

To stop the running containers, use:

```sh
docker-compose down
```