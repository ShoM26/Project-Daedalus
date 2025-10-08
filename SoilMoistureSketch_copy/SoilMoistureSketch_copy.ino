#include <ArduinoJson.h>
#include <EEPROM.h>

// Device configuration
const String hardwareIdentifier;
int prevReading = 0;

const int SAMPLE_COUNT = 10;
const int SAMPLE_DELAY = 100;
int sampleBuffer[SAMPLE_COUNT];
int sampleIndex = 0;
bool bufferFilled = false;

unsigned long lastSendTime = 0;

// Function to generate unique device ID using EEPROM
String getEEPROMDeviceID() {
  String id = "PLANT_";
  
  // Check if we've stored an ID in EEPROM before
  if (EEPROM.read(0) == 0xFF) {
    // Generate random ID and store it permanently
    randomSeed(analogRead(A1));  // Use unconnected pin for randomness
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

int getAveragedReading() {
  long total = 0;
  int validReadings = 0;
  
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
  Serial.begin(9600);
  hardwareIdentifier = getEEPROMDeviceID();
  for(int i =0; i< SAMPLE_COUNT; i++){
    sampleBuffer[i] = 0;
  }
}

void loop() {
  int rawValue = getAveragedReading();  

  if(rawValue < 0){
    sendErrorMessage("Sensor reading failed");
    delay(5000);
    return;
  }

  int smoothedValue = getRollingAverage(rawValue);
  
  unsigned long currentTime = millis();
  int valueDifference = abs(smoothedValue - prevReading);

  if(valueDifference > 5 || (currentTime - lastSendTime) > 30000) {
    sendSensorData(smoothedValue);
    prevReading = smoothedValue;
    lastSendTime = currentTime;
  }
  
  delay(3000);
}

void sendSensorData(int smoothedValue) {
  StaticJsonDocument<200> doc;
  
  doc["hardwareidentifier"] = hardwareIdentifier;
  doc["timestamp"] = millis();
  doc["moisturelevel"] = smoothedValue;
  
  serializeJson(doc, Serial);
  Serial.println();
  delay(100);
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