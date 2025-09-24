// src/data/mockData.js
// This simulates what your C# API will return

export const mockPlantPairings = [
  {
    id: 1,
    plant: {
      id: 1,
      scientificName: "Ficus lyrata",
      familiarName: "Fiddle Leaf Fig",
      idealMoistureMin: 40,
      idealMoistureMax: 60,
      funFact: "Native to western Africa, these plants can grow up to 15 feet tall indoors!"
    },
    device: {
      id: 1,
      hardwareIdentifier: "DEVICE_001",
      connectionType: "USB",
      connectionAddress: "COM3",
      userId: 1,
      lastSeen: "2024-09-24T14:30:00Z"
    },
    currentReading: {
      hardwareIdentifier: "DEVICE_001",
      moistureLevel: 45,
      timestamp: "2024-09-24T14:30:00Z"
    },
    user:{
        id: 1,
        username: "Gary12",
        password: "123goat",
        email: "Garyslimedmeout@gmail.com",
        createdAt: "2023-02-20T14:30:30Z"
    },
    status: "healthy" // healthy, needs_water, overwatered, offline
  },
  {
    id: 2,
    plant: {
      id: 2,
      scientificName: "Sansevieria trifasciata",
      familiarName: "Snake Plant",
      idealMoistureMin: 20,
      idealMoistureMax: 40,
      funFact: "Also known as 'Mother-in-Law's Tongue', it's nearly indestructible!"
    },
    device: {
      id: 1,
      hardwareIdentifier: "DEVICE_002",
      connectionType: "USB",
      connectionAddress: "COM4",
      lastSeen: "2024-09-24T14:28:00Z"
    },
    currentReading: {
        hardwareIdentifier: "DEVICE_002",
      moistureLevel: 15,
      timestamp: "2024-09-24T14:28:00Z"
    },
    user:{
        id: 1,
        username: "Gary12",
        password: "123goat",
        email: "Garyslimedmeout@gmail.com",
        createdAt: "2023-02-20T14:30:30Z"
    },
    status: "needs_water"
  },
  {
    id: 3,
    plant: {
      id: 3,
      scientificName: "Pothos aureus",
      familiarName: "Golden Pothos",
      idealMoistureMin: 30,
      idealMoistureMax: 50,
      funFact: "Perfect for beginners - very forgiving and grows in almost any light!"
    },
    device: {
      id: 3,
        hardwareIdentifier: "DEVICE_003",
      connectionType: "USB",
      connectionAddress: "COM5",
      lastSeen: "2024-09-24T14:25:00Z"
    },
    currentReading: {
      hardwareIdentifier: "DEVICE_003",
        moistureLevel: 75,
      timestamp: "2024-09-24T14:25:00Z"
    },
    user:{
        id: 1,
        username: "Gary12",
        password: "123goat",
        email: "Garyslimedmeout@gmail.com",
        createdAt: "2023-02-20T14:30:30Z"
    },
    status: "overwatered"
  },
  {
    id: 4,
    plant: {
      id: 4,
      scientificName: "Monstera deliciosa",
      familiarName: "Swiss Cheese Plant",
      idealMoistureMin: 35,
      idealMoistureMax: 55,
      funFact: "Those holes in the leaves are called fenestrations - they help the plant withstand strong winds!"
    },
    device: {
      id: 4,
        hardwareIdentifier: "DEVICE_004",
      connectionType: "Bluetooth",
      connectionAddress: "MAC03234fsdswsa",
      lastSeen: "2024-09-24T13:45:00Z"
    },
    currentReading: {
      hardwareIdentifier: "DEVICE_004",
        moistureLevel: 0,
      timestamp: "2024-09-24T13:45:00Z"
    },
    user: {
        id: 1,
        username: "Gary12",
        password: "123goat",
        email: "Garyslimedmeout@gmail.com",
        createdAt: "2023-02-20T14:30:30Z"
    },
    status: "offline"
  }
];

// Helper function to get status color (we'll use this later)
export const getStatusColor = (status) => {
  switch (status) {
    case 'healthy': return '#22c55e';     // green
    case 'needs_water': return '#f59e0b'; // yellow/orange
    case 'overwatered': return '#3b82f6'; // blue
    case 'offline': return '#ef4444';     // red
    default: return '#6b7280';            // gray
  }
};

// Helper function to format timestamps (we'll use this later)
export const formatTimestamp = (timestamp) => {
  const date = new Date(timestamp);
  return date.toLocaleString();
};