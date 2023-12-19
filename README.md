# BattleShips.API

## Overview

This repository holds the backend API for my trainee project Battleships game.

## Build and Run

The project is built with .Net 6 with a Microsoft SQL Server to store the games data. To
run the app, simply clone, run `dotnet restore`, then `dotnet run` in your terminal. The
project also connects to a Microsoft SQL Server to store the data for the game, and uses
Entity Framework to manage the data.

## Authentication

This project uses Azure B2C authentication which allows the user to login and make calls
to the API with a JWT token.

## Testing

To run the unit tests, use `dotnet test`.

## Creating the Docker Image & Container (Notes)

### Build the Image

```bash
docker build -t battleships-api -f Dockerfile .
```

### Create the Container

```bash
docker create --name battleships-api battleships-api
```

### Run the Container

```bash
docker start battleships-api   
```
