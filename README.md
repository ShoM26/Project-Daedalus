Project Daedalus 🌱

An IoT-Enabled Plant Monitoring System

Project Daedalus is a full-stack system that combines IoT sensors, a C# ASP.NET Core backend, a MySQL database, and a React frontend to help users monitor and care for their plants. It demonstrates professional software architecture, clean coding practices, and IoT-to-web integration.

Author

Developed by Merrick Shorter

Tech Stack

Frontend: React (Visual Studio Code)

Backend: ASP.NET Core Web API (C# in JetBrains Rider)

Database: MySQL (DataGrip)

Hardware: Arduino Uno + soil moisture sensor (USB serial now, Bluetooth planned later)

Authentication: JWT tokens (planned)

Environment: All components run locally during development

Database & Architecture Highlights

Entity Framework Core (EF Core) with migrations

Repository Pattern with generic base repository

Dependency Injection for clean architecture

Multi-user scalable design with proper foreign key relationships

Cascading deletes for safe data cleanup

Optimized schema for IoT data ingestion & retrieval

Core Tables

Users: authentication and profile info

Devices: device ID, name, type/address, linked to user

Plants: scientific/common names, ideal moisture ranges, optional fun facts

UserPlants: association of users, plants, and devices

SensorHistory: timestamped soil moisture readings

Features
✅ Current Features

Complete REST API for all entities

CRUD operations for users, plants, devices, and sensor data

Cascading delete (e.g., deleting a user removes all related data)

Date range queries for historical sensor readings

Device-to-plant assignment management

Batch operations (e.g., DeleteManyAsync)

Data cleanup for old sensor readings

🔮 Planned Features

React frontend with real-time dashboards

JWT authentication for secure access

Bluetooth device connectivity

Watering alert system with notifications

Historical trend visualization

Mobile-responsive UI

Development Approach

API-first development: backend foundation completed and tested in Postman

Repository layer: clean abstraction over EF Core

Hardware integration: Arduino Uno currently streams data via USB serial (Bluetooth planned)

Frontend: React dashboards and live visualization are next

Current Status

✅ Backend foundation complete and tested

✅ Database schema & cascading relationships configured

✅ Controllers implemented for all entities

🔄 Preparing for frontend integration

🔄 Refining hardware integration for reliable data streaming

Roadmap

The roadmap for Project Daedalus is maintained in the repository.

Showcase Purpose

This project serves as a capstone and portfolio project, demonstrating:

Full-stack development

IoT integration

Database design and optimization

Clean architecture in ASP.NET Core

Real-world problem-solving with hardware + software systems
