/*
Project Daedalus - Unified Sensor Firmware
Supports: Arduino Uno (w/HC-05) and ESP32
Features: Auto-ID, Secure Handshake, Dual-Port Data Transmission
*/

#include <ArduinoJson.h>
#include "HardwareSecret.h"

//######################################
//  Hardware Selection & Configuration
// #####################################

// Define pins
#define SOIL_PIN A0

// Timing Configuration
const long HANDSHAKE_INTERVAL = 2000;
const long DATA_INTERVAL = 15000;

// ###################################
// Platform Specific Includes & Setup
// ###################################

const String hardwareIdentifier;
bool isRegistered = false;

#ifdef ESP32
  #include "BluetoothSerial.h"
  #include "WiFi.h"
  BluetoothSerial SerialBT;

  void setupComms() {
    Serial.begin(9600);     // USB Serial
    SerialBT.begin("Daedalus_Plant_Sensor"); // Bluetooth Name
    Serial.println("--- ESP32 DETECTED ---");
    Serial.println("Bluetooth Started: Daedalus_Plant_Sensor");
  }

  String generateDeviceId() {
    return WiFi.macAddress();
  }

 //Wrapper to send to either ports
  void sendToBridge(JsonDocument& doc){
    serializeJson(doc, Serial);
    Serial.println();

    if(SerialBT.hasClient()){
      serializeJson(doc, SerialBT);
      SerialBT.println();
    }
  }

  bool readFromBridge(JsonDocument& doc){
    if(Serial.available()){
      DeserializationError error = deserializeJson(doc, Serial);
      if(!error) return true;
    }
    if(SerialBT.available()){
      DeserializationError error = deserializeJson(doc, SerialBT);
      if(!error) return true;
    }
    return false;
  }

#else
  // Arduino Uno Configuration
  #include <SoftwareSerial.h>
  #include <EEPROM.h>
  
  #define BT_RX_PIN 2
  #define BT_TX_PIN 3

  SoftwareSerial SerialBT(BT_RX_PIN, BT_TX_PIN);

  void setupComms(){
    Serial.begin(9600);
    SerialBT.begin(9600);
    Serial.println("--- Arduino Uno Detected ---");
  } 

  String generateDeviceId() {
    String id = "PLANT_";
    
    // Check if we've stored an ID in EEPROM before
    if (EEPROM.read(0) == 0xFF) {  // First time setup
      // Generate random ID and store it permanently
      randomSeed(analogRead(A1) ^ analogRead(A2) ^ millis());  // Use unconnected pin for randomness
      long uniqueNum = random(10000, 99999);
      
      // Store in EEPROM (permanent storage)
      EEPROM.write(0, 0x01);  // Mark as initialized
      EEPROM.write(1, (uniqueNum >> 24) & 0xFF);
      EEPROM.write(2, (uniqueNum >> 16) & 0xFF);
      EEPROM.write(3, (uniqueNum >> 8) & 0xFF);
      EEPROM.write(4, uniqueNum & 0xFF);
    }
    long storedNum = 0;
    storedNum |= ((long)EEPROM.read(1) << 24);
    storedNum |= ((long)EEPROM.read(2) << 16);
    storedNum |= ((long)EEPROM.read(3) << 8);
    storedNum |= EEPROM.read(4);
    
    id += String(storedNum);
    return id;
  }

  void sendToBridge(JsonDocument& doc){
    serializeJson(doc, Serial);
    Serial.println();

    serializeJson(doc, SerialBT);
    SerialBT.println();
  }

  bool readFromBridge(JsonDocument& doc){
    if(Serial.available()){
      DeserializationError error = deserializeJson(doc, Serial);
      if(!error) return true;
    }
    if(SerialBT.available()){
      DeserializationError error = deserializeJson(doc, SerialBT);
      if(!error) return true;
    }
    return false;
  }
#endif

int prevReading = 0;
const int dry=539;
const int wet=350;

const int SAMPLE_COUNT = 10;
const int SAMPLE_DELAY = 100;
int sampleBuffer[SAMPLE_COUNT];
int sampleIndex = 0;
bool bufferFilled = false;
unsigned long lastSendTime = 0;

int getAveragedReading() {
  long total = 0;
  int validReadings = 0;
  
  // Take multiple readings
  for (int i = 0; i < SAMPLE_COUNT; i++) {
    int reading = analogRead(A0);
    
    if (reading >= 0 && reading <= 1023) {
      total += reading;
      validReadings++;
    }
    delay(SAMPLE_DELAY);  
  }
  
  if (validReadings > 0) {
    return total / validReadings;
  } else {
    return -1;  
  }
}

int getRollingAverage(int newReading) {

  sampleBuffer[sampleIndex] = newReading;
  sampleIndex = (sampleIndex + 1) % SAMPLE_COUNT;
  
  if (sampleIndex == 0) {
    bufferFilled = true;
  }
    long total = 0;
  int count = bufferFilled ? SAMPLE_COUNT : sampleIndex;
  
  for (int i = 0; i < count; i++) {
    total += sampleBuffer[i];
  }
  return total / count;
}

void setup() {
  setupComms();
  hardwareIdentifier = generateDeviceId();
  for(int i =0; i< SAMPLE_COUNT; i++){
    sampleBuffer[i] = 0;
  }
}

void loop() {

//Listen for ACK
  StaticJsonDocument<200> inputDoc;
  if(readFromBridge(inputDoc)){
    const char* type = inputDoc["type"];

    if(strcmp(type, "ACK") == 0){
      isRegistered = true;
      Serial.println(">> Registration Confirmed. Switching to Data Mode. ");
    }
  }

//Timed action based on state
  unsigned long currentMillis = millis();
  if(!isRegistered){
    //Handshake
    if(currentMillis - lastSendTime >= HANDSHAKE_INTERVAL){
      lastSendTime = currentMillis;

      StaticJsonDocument<200> doc;
      doc["type"] = "HANDSHAKE";
      doc["hardwareIdentifier"] = hardwareIdentifier;
      doc["secret"] = HARDWARE_SECRET;

      sendToBridge(doc);
    }
  }
  else{
    //Data sending mode
    if(currentMillis - lastSendTime >= DATA_INTERVAL){
      lastSendTime = currentMillis;

      StaticJsonDocument<200> doc;
      int rawValue = getAveragedReading();
      int smoothedValue = getRollingAverage(rawValue);
      int clampedValue = constrain(smoothedValue, wet, dry);
      int percentageValue = map(clampedValue, wet, dry, 100, 0);
      doc["type"] = "DATA";
      doc["hardwareIdentifier"] = hardwareIdentifier;
      doc["moisture"] = percentageValue;
      sendToBridge(doc);
    }
  }
}

void sendErrorMessage(const char* errorMsg) {
  StaticJsonDocument<200> doc;
  doc["hardwareidentifier"] = hardwareIdentifier;
  doc["error"] = errorMsg;
  doc["timestamp"] = millis();
  
  serializeJson(doc, Serial);
  Serial.println();
  Serial.flush();
  delay(100);
}