

#pragma region Hardware definition
#define DIGITAL_PINS (NUM_DIGITAL_PINS - NUM_ANALOG_INPUTS)
#define ANALOG_PINS NUM_ANALOG_INPUTS

#if defined(ARDUINO_AVR_UNO)
  #define BOARD_TYPE "Arduino UNO"
#elif defined(ARDUINO_AVR_NANO)
  #define BOARD_TYPE "Arduino NANO"
#elif defined(ARDUINO_AVR_MEGA2560) || defined(ARDUINO_AVR_MEGA)
  #define BOARD_TYPE "Arduino MEGA 2560"
#elif defined(ARDUINO_AVR_LEONARDO)
  #define BOARD_TYPE "Arduino LEONARDO"
#elif defined(ARDUINO_AVR_MICRO)
  #define BOARD_TYPE "Arduino MICRO"
#elif defined(ARDUINO_AVR_PRO)
  #define BOARD_TYPE "Arduino PRO MINI"
#elif defined(ARDUINO_AVR_DUEMILANOVE)
  #define BOARD_TYPE "Arduino DUEMILANOVE"
#elif defined(ARDUINO_UNOR4_MINIMA) || defined(ARDUINO_UNOR4_WIFI)
  #define BOARD_TYPE "Arduino UNO R4"
#elif defined(ARDUINO_SAM_DUE)
  #define BOARD_TYPE "Arduino DUE"
#elif defined(ARDUINO_SAMD_ZERO)
  #define BOARD_TYPE "Arduino ZERO"
#elif defined(ARDUINO_SAMD_MKR1000) || defined(ARDUINO_SAMD_MKRWIFI1010)
  #define BOARD_TYPE "Arduino MKR"
#elif defined(ESP8266)
  #define BOARD_TYPE "ESP8266"
#elif defined(ESP32)
  #define BOARD_TYPE "ESP32"
#elif defined(ARDUINO_ARCH_STM32)
  #define BOARD_TYPE "STM32"
#elif defined(ARDUINO_ARCH_RP2040)
  #define BOARD_TYPE "Raspberry Pi Pico"
#else
  #define BOARD_TYPE "UNKNOWN"
#endif

int wiredDigitals[] = {23, 25, 27, 29, 31, 33, 35, 37, 39, 41, 43, 45, 47, 49, 51, 53};
int relays[sizeof(wiredDigitals) / sizeof(wiredDigitals[0])];
int inputs[sizeof(wiredDigitals) / sizeof(wiredDigitals[0])];

#pragma endregion

#pragma region Program header

  #pragma region Method def
  byte calcChecksum(const String &s);
  void handleSerial();
  void processMessage(String msg);
  void processCommand(String id, String cmd, String data);
  float handleMeasure();
  void sendMessage(String id, String command, String data);
  void setParameter(String id, String data);
  void getParameter(String id, String data);
  void setInputOutput(const String &id, const String &data);
  void getInputOutput(const String &id, const String &data);
  int getPinMode(uint8_t pin);
  void getPinModeCommand(const String &id, const String &data);
  #pragma endregion

  #pragma region Measure

  enum MeasureStatus {
    MEASURE_IDLE,
    MEASURE_START_POSITION,
    MEASURE_OPENING_VALVE1,
    MEASURE_WAITING_VALVE3_CLOSE,
    MEASURE_RUNNING,
    MEASURE_FALL_DETECTED,
    MEASURE_STOPPING
  };

  struct Measure {
    MeasureStatus status = MEASURE_IDLE;
    float value = 0.0f;
    bool valid = false;
  };

  Measure measurement;
  bool measureActive = false;

  unsigned long pumpStartTime = 0;
  const unsigned long VALVE3_CLOSE_DELAY_MS = 5000;

  const unsigned long MEASURE_INTERVAL_MS = 10;
  const float MIN_DELTA = 2.0f;
  const int FALL_THRESHOLD = 3;

  float lastValue = 0.0f;
  int fallingCount = 0;
  unsigned long lastMeasure = 0;
  bool firstTrendSample = true;

  #pragma endregion

  #pragma region Valve
  enum ValveState {
    VALVE_IDLE,
    VALVE_OPENING,
    VALVE_CLOSING,
    VALVE_POSITIONING_OPEN,
    VALVE_POSITIONING_CLOSE,
    VALVE_CALIBRATING_STARTING,
    VALVE_CALIBRATING_OPEN,
    VALVE_CALIBRATING_CLOSE
  };

  const unsigned long CALIBRATION_IGNORE_MS = 2000;

  struct ValveControl {
    ValveState state = VALVE_IDLE;
    bool calibrated = false;
    bool calibration = false;

    int openPin;
    int closePin;
    int isOpenPin;
    int isClosePin;

    unsigned long startTime = 0;

    unsigned long openDuration = 0;
    unsigned long closeDuration = 0;

    unsigned long movementDuration = 0;

    int position = 0;
    int targetPosition = 0;
  };

  void startOpenValve(ValveControl &valve, String id);
  void startCloseValve(ValveControl &valve, String id);
  void positioningValve(ValveControl &valve, int position, String id);
  void stopValve(ValveControl &valve);
  void calibrateValve(ValveControl &valve);
  void updateValve(ValveControl &valve);

  #pragma endregion

  #pragma region Pump
  enum PumpState {
    PUMP_IDLE,
    PUMP_RUNNING
  };

  struct PumpControl {
    PumpState state = PUMP_IDLE;

    int pin;

    void begin(uint8_t outputPin) {
      pin = outputPin;
      
      pinMode(pin, OUTPUT);
      digitalWrite(pin, LOW);
      state = PUMP_IDLE;
    }

    void start() {
      digitalWrite(pin, HIGH);
      state = PUMP_RUNNING;
    }

    void stop() {
      digitalWrite(pin, LOW);
      state = PUMP_IDLE;
    }
  };

  #pragma endregion

  #pragma region Analog
  struct PressureMeasurement {
    uint8_t pin;

    float EU0 = 0.0;
    float EU100 = 200.0;
    float EU;

    float U0 = 1.1;
    float U100 = 4.67;
    float U;

    int raw0 = 248;
    int raw100 = 955;
    int raw;

    float percentage;

    bool valid;
  };

  #pragma endregion

  #define STX '<'
  #define ETX '>'

  struct Message {
    String id;
    String command;
    String data;
    String checksum;
  };

  Message inner;
  message.id = -1;
  message.command = "INNER";
  message.data = "";

  #pragma region Parameters
  ValveControl valve1;
  ValveControl valve2;
  ValveControl valve3;

  PressureMeasurement pt0;
  PressureMeasurement pt1;
  PressureMeasurement pt2;

  PumpControl pump;

  String buffer = "";
  bool inFrame = false;

  bool measure = false;
  int measureStatus = 0;

  unsigned long lastMeasure = 0;
  const unsigned long interval = 10;

  int lastValue = -1;
  int fallingCount = 0;

  int FALL_THRESHOLD = 3;   // ennyi egymás utáni csökkenés kell
  int MIN_DELTA = 2;        // zajszűrés
  #pragma endregion

#pragma endregion

#pragma region Setup
void setup() {
  Serial.begin(1000000);

  for (int i = 0; i < sizeof(wiredDigitals) / sizeof(wiredDigitals[0]); i++) {
    pinMode(wiredDigitals[i], INPUT);
  }

  Serial.println(sizeof(wiredDigitals));

  for (int i = 0; i < sizeof(relays) / sizeof(relays[0]); i++) {
    relays[i] = wiredDigitals[i * 2];
    inputs[i] = wiredDigitals[i * 2 + 1];
  }

  for (int i = 0; i < sizeof(relays) / sizeof(relays[0]); i++) {
    pinMode(relays[i], OUTPUT);
  }

  valve1.openPin      = relays[0];
  valve1.closePin     = relays[1];
  valve1.isOpenPin    = inputs[0];
  valve1.isClosePin   = inputs[1];

  valve2.openPin      = relays[2];
  valve2.closePin     = relays[3];
  valve2.isOpenPin    = inputs[2];
  valve2.isClosePin   = inputs[3];

  valve3.openPin      = relays[4];
  valve3.closePin     = relays[5];
  valve3.isOpenPin    = inputs[4];
  valve3.isClosePin   = inputs[5];

  pump.begin(relays[6]);

  pinMode(valve1.isOpenPin, INPUT_PULLUP);     // Valve 1 open
  pinMode(valve1.isClosePin, INPUT_PULLUP);    // Valve 1 close
  pinMode(valve2.isOpenPin, INPUT_PULLUP);     // Valve 2 open
  pinMode(valve2.isClosePin, INPUT_PULLUP);    // Valve 2 close
  pinMode(valve3.isOpenPin, INPUT_PULLUP);     // Valve 3 open
  pinMode(valve3.isClosePin, INPUT_PULLUP);    // Valve 3 close

  pt0.pin = A0;
  pt1.pin = A2;
  pt2.pin = A4;
  /*
  while (true) {
    analogReadEnhanced(pt1);
    Serial.print("EU: ");
    Serial.print(pt1.EU);
    Serial.println(" kPa");
    
    Serial.print("U: ");
    Serial.print(pt1.U);
    Serial.println(" V");
    
    Serial.print("RAW: ");
    Serial.println(pt1.raw);
    delay(2000);
  }
  */
}
#pragma endregion

#pragma region Main
void loop() {
  handleSerial();   // mindig fusson

  if (!valve1.calibrated && !valve1.calibration)
      calibrateValve(valve1);

  if (!valve2.calibrated && !valve2.calibration)
      calibrateValve(valve2);

  if (!valve3.calibrated && !valve3.calibration)
      calibrateValve(valve3);

  updateValve(valve1);
  updateValve(valve2);
  updateValve(valve3);

  analogReadEnhanced(pt0);
  analogReadEnhanced(pt1);
  analogReadEnhanced(pt2);

  updateMeasureValue();
  updateMeasure();
}
#pragma endregion

#pragma region Serial Communication
byte calcChecksum(const String &s) {
  byte chk = 0;
  for (int i = 0; i < s.length(); i++) {
    chk ^= (byte)s[i];
  }
  return chk;
}

void handleSerial() {
  const size_t MAX_FRAME_LENGTH = 128;

  while (Serial.available()) {
    char c = Serial.read();

    if (c == STX) {
      buffer = "";
      inFrame = true;
    }
    else if (c == ETX && inFrame) {
      processMessage(buffer);
      inFrame = false;
    }
    else if (inFrame) {
      if (buffer.length() < MAX_FRAME_LENGTH) {
        buffer += c;
      }
      else {
        buffer = "";
        inFrame = false;
        sendMessage("", "ERR", "FRAME TOO LONG");
      }
    }
    // ha nem vagyunk frame-ben, eldobjuk -> resync
  }
}

void processMessage(String msg) {
  int p1 = msg.indexOf('|');
  int p2 = msg.indexOf('|', p1 + 1);
  int p3 = msg.lastIndexOf('|');

  if (p1 == -1 || p2 == -1 || p3 == -1) return;
  
  Message message;
  message.id = msg.substring(0, p1);
  message.command = msg.substring(p1 + 1, p2);
  message.data = msg.substring(p2 + 1, p3);
  message.checksum = msg.substring(p3 + 1);

  byte chkCalc = calcChecksum(msg.substring(0, p3));
  byte chkRecv = (byte) strtol(message.checksum.c_str(), NULL, 16);

  /*
  if (chkCalc != chkRecv) {
    sendMessage(id, "ERR", "CHK");
    return;
  }*/

  processCommand(message);
}

void sendMessage(String id, String command, String data) {
  String payload = id + "|" + command + "|" + data;
  byte chk = calcChecksum(payload);

  Serial.print(STX);
  Serial.print(payload);
  Serial.print("|");
  if (chk < 16) Serial.print("0");
  Serial.print(chk, HEX);
  Serial.print(ETX);
}
#pragma endregion

#pragma region Command Handler
void processCommand(Message &message) {
  if (message.command == "PING") {
    sendMessage(message.id, "PONG", "ACK");
  }
  
  else if (message.command == "START_MEASURE") {
    if (measurement.status != MEASURE_IDLE) {
      sendMessage(message.id, "ERR", "MEASUREMENT ALREADY ACTIVE");
      return;
    }

    fallingCount = 0;
    firstTrendSample = true;
    lastMeasure = millis();

    measurement.status = MEASURE_START_POSITION;

    sendMessage(message.id, "ACK", "MEASUREMENT STARTED");
  }
  
  else if (message.command == "GET_MEASURE" ||
          message.command == "GET_MEASURE_DATA") {

      String statusText;

      switch (measurement.status) {
        case MEASURE_IDLE:
          statusText = "STOPPED";
          break;
        case MEASURE_START_POSITION:
          statusText = "STARTING";
          break;
        case MEASURE_OPENING_VALVE1:
          statusText = "STARTING";
          break;
        case MEASURE_WAITING_VALVE3_CLOSE:
          statusText = "STARTING";
          break;
        case MEASURE_RUNNING:
          statusText = "RUNNING";
          break;
        case MEASURE_FALL_DETECTED:
          statusText = "RUNNING";
          break;
        case MEASURE_STOPPING:
          statusText = "RUNNING";
          break;
      }

      sendMessage(
          message.id,
          "MEASURE_DATA",
          statusText + ";" + String(measurement.value, 2)
      );
  }

  else if (message.command == "STOP_MEASURE") {
    stopMeasureSequent();
    sendMessage(message.id, "ACK", "STOPPED");
  }
  
  else if (message.command == "SET_PARAM") {
    setParameter(message.id, message.data);
  }
  
  else if (message.command == "GET_PARAM") {
    getParameter(message.id, message.data);
  }
  
  else if (message.command == "GET_BOARD_DATAS") {
    sendMessage(message.id, "BOARD_DATAS", String(BOARD_TYPE) + ";" + String(DIGITAL_PINS) + ";" + String(ANALOG_PINS));
  }
  
  else if (message.command == "SET_IO") {
    setInputOutput(message.id, message.data);
  }
  
  else if (message.command == "GET_IO") {
    getInputOutput(message.id, message.data);
  }
  
  else if (message.command == "GET_PIN_MODE") {
    getPinModeCommand(message.id, message.data);
  }
  
  else if (message.command == "CLOSE_VALVE") {
    int value = message.data.toInt();
    switch (value) {
    
    case 1:
        startCloseValve(valve1, message.id);
        break;
    
    case 2:
        startCloseValve(valve2, message.id);
        break;
    
    case 3:
        startCloseValve(valve3, message.id);
        break;
    default:
        sendMessage(message.id, "ERR", "INVALID VALVE");
        break;
    }
  }
  
  else if (message.command == "OPEN_VALVE") {
    int value = message.data.toInt();
    switch (value) {
    
    case 1:
        startOpenValve(valve1, message.id);
        break;
    
    case 2:
        startOpenValve(valve2, message.id);
        break;
    
    case 3:
        startOpenValve(valve3, message.id);
        break;
    default:
        sendMessage(message.id, "ERR", "INVALID VALVE");
        return;
    }
  }
  
  else if (message.command == "SET_VALVE_POSITION") {
    int p1 = message.data.indexOf(";");

    if (p1 == -1) {
      sendMessage(message.id, "ERR", "INVALID FORMAT");
      return;
    }

    int valveNumber = message.data.substring(0, p1).toInt();
    int position = message.data.substring(p1 + 1).toInt();

    switch (valveNumber) {
      case 1:
        positioningValve(valve1, position, message.id);
        break;
      case 2:
        positioningValve(valve2, position, message.id);
        break;
      case 3:
        positioningValve(valve3, position, message.id);
        break;
      default:
        sendMessage(message.id, "ERR", "INVALID VALVE");
        break;
    }
  }

  else {
    sendMessage(message.id, "ERR", "INVALID COMMAND");
  }
}
#pragma endregion

#pragma region Measure Handler
void updateMeasure() {
  switch (measurement.status) {

    case MEASURE_IDLE:
      break;

    case MEASURE_START_POSITION:
      measurement.status = MEASURE_OPENING_VALVE1;
      startOpenValve(valve1, "");
      break;

    case MEASURE_OPENING_VALVE1:
      if (digitalRead(valve1.isOpenPin) == LOW) {
        pump.start();
        pumpStartTime = millis();

        measurement.status = MEASURE_WAITING_VALVE3_CLOSE;
      }
      break;

    case MEASURE_WAITING_VALVE3_CLOSE:
      if (millis() - pumpStartTime >= VALVE3_CLOSE_DELAY_MS) {
        startCloseValve(valve3, "");
        measurement.status = MEASURE_RUNNING;
      }
      break;

    case MEASURE_RUNNING:
        unsigned long now = millis();

        if (now - lastMeasure < MEASURE_INTERVAL_MS) {
            break;
        }

        lastMeasure = now;

        if (!measurement.valid || !isfinite(measurement.value)) {
            break;
        }

        float currentValue = measurement.value;

        if (firstTrendSample) {
            lastValue = currentValue;
            firstTrendSample = false;
            fallingCount = 0;
            break;
        }

        if (currentValue < lastValue - MIN_DELTA) {
            fallingCount++;
        } else {
            fallingCount = 0;
        }

        lastValue = currentValue;

        if (fallingCount >= FALL_THRESHOLD) {
            measurement.status = MEASURE_FALL_DETECTED;
        }

        break;

    case MEASURE_FALL_DETECTED:
      stopMeasureSequent();
      measurement.status = MEASURE_STOPPING;
      break;

    case MEASURE_STOPPING:
      break;
  }
}

void updateMeasureValue() {
  if (!isfinite(pt1.EU) || !isfinite(pt2.EU)) {
    measurement.valid = false;
    return;
  }

  measurement.value = (pt1.EU + pt2.EU) / 2.0f;
  measurement.valid = true;
}

void stopMeasureSequent() {
  measureActive = false;

  pump.stop();

  startCloseValve(valve1, "");

  startOpenValve(valve2, "");
  startOpenValve(valve3, "");

  measurement.status = MEASURE_IDLE;
}
#pragma endregion

#pragma region Hardware Parameters
void setParameter(String id, String data) {
  int p1 = data.indexOf(';');
  
  if (p1 == -1) {
    sendMessage(id, "ERR", "INVALID FORMAT");
    return;
  }

  String param = data.substring(0, p1);
  String value = data.substring(p1 + 1);

  param.trim();
  value.trim();

  if (param == "MIN_DELTA") {
    MIN_DELTA = value.toInt();
    sendMessage(id, "ACK", String(MIN_DELTA));
  }
  else if (param == "FALL_THRESHOLD") {
    FALL_THRESHOLD = value.toInt();
    sendMessage(id, "ACK", String(FALL_THRESHOLD));
  }
  else {
    sendMessage(id, "ERR", "UNKNOWN PARAMETER");
  }
}

void getParameter(String id, String data) {
  if (data == "MD") sendMessage(id, "ACK", String(MIN_DELTA));
  else if (data == "FT") sendMessage(id, "ACK", String(FALL_THRESHOLD));
  else sendMessage(id, "ERR", "UNKNOWN PARAMETER");
}
#pragma endregion

#pragma region Hardware debug
void setInputOutput(const String &id, const String &data) {
  int p1 = data.indexOf(';');
    
  if (p1 == -1) {
    sendMessage(id, "ERR", "INVALID FORMAT");
    return;
  }

  String param = data.substring(0, p1);
  String value = data.substring(p1 + 1);

  param.trim();
  value.trim();

  if (param.length() < 2) {
    sendMessage(id, "ERR", "INVALID PIN");
    return;
  }

  int pinNum = param.substring(1).toInt();
  digitalWrite(pinNum, (value == "HIGH" || value == "1") ? HIGH : LOW);

  sendMessage(id, "ACK", param + ";" + value);
}

void getInputOutput(const String &id, const String &data) {
  if (data.length() < 2) {
    sendMessage(id, "ERR", "INVALID PARAMETER");
    return;
  }

  String type = data.substring(0, 1);
  type.trim();
  
  int pinNum = data.substring(1).toInt();
  String value = "";

  if (type == "D") {
    value = String(digitalRead(pinNum));
  } 
  else if (type == "A") {
    #if defined(A0)
      value = String(analogRead(A0 + pinNum));
    #else
      value = String(analogRead(pinNum));
    #endif
  } else {
    sendMessage(id, "ERR", "INVALID TYPE");
    return;
  }

  sendMessage(id, "IO_DATA", value);
}

int getPinMode(uint8_t pin) {
  #if defined(ARDUINO_ARCH_AVR)
    uint8_t bit = digitalPinToBitMask(pin);
    uint8_t port = digitalPinToPort(pin);

    if (port == NOT_A_PIN) return -1;

    volatile uint8_t *reg = portModeRegister(port);
    volatile uint8_t *out = portOutputRegister(port);

    if (*reg & bit) {
      return OUTPUT;
    }
    
    if (*out & bit) {
      return INPUT_PULLUP;
    }

    return INPUT;
  #else
    return INPUT; 
  #endif
}

void getPinModeCommand(const String &id, const String &data) {
  if (data.length() < 2) {
    sendMessage(id, "ERR", "INVALID PARAMETER");
    return;
  }

  String type = data.substring(0, 1);
  type.trim();

  if (type != "D") {
    sendMessage(id, "ERR", "ONLY DIGITAL PINS SUPPORTED");
    return;
  }

  int pinNum = data.substring(1).toInt();

  if (pinNum > DIGITAL_PINS || pinNum < 0) {
    sendMessage(id, "ERR", "PIN OUT OF BOUNDS");
    return;
  }

  int mode = getPinMode(pinNum);

  String modeStr = "";
  if (mode == OUTPUT) {
    modeStr = "OUTPUT";
  } else if (mode == INPUT_PULLUP) {
    modeStr = "INPUT_PULLUP";
  } else if (mode == INPUT) {
    modeStr = "INPUT";
  } else {
    sendMessage(id, "ERR", "INVALID PIN");
    return;
  }

  sendMessage(id, "PIN_MODE", modeStr);
}
#pragma endregion

#pragma region Valve Control
void startOpenValve(ValveControl &valve, String id) {
  if (digitalRead(valve.isOpenPin) == LOW) {
    digitalWrite(valve.openPin, LOW);
    valve.state = VALVE_IDLE;
    return;
  }

  stopValve(valve);
  digitalWrite(valve.openPin, HIGH);

  valve.state = VALVE_OPENING;
  sendMessage(id, "ACK", "STARTED OPENING");
}

void startCloseValve(ValveControl &valve, String id) {
  if (digitalRead(valve.isClosePin) == LOW) {
    digitalWrite(valve.closePin, LOW);
    valve.state = VALVE_IDLE;
    return;
  }

  stopValve(valve);
  digitalWrite(valve.closePin, HIGH);

  valve.state = VALVE_CLOSING;
  sendMessage(id, "ACK", "STARTED CLOSING");
}

void positioningValve(ValveControl &valve, int position, String id) {
  if (position < 0 || position > 100) {
    sendMessage(id, "ERR", "INVALID POSITION");
    return;
  }

  if (valve.position == position) return;

  valve.targetPosition = position;

  // Nyitás
  if (position > valve.position) {
    int difference = position - valve.position;

    valve.movementDuration = (unsigned long)(valve.openDuration * difference / 100.0);
    valve.startTime = millis();

    digitalWrite(valve.closePin, LOW);
    digitalWrite(valve.openPin, HIGH);

    valve.state = VALVE_POSITIONING_OPEN;
  }

  // Zárás
  else {
    int difference = valve.position - position;

    valve.movementDuration = (unsigned long)(valve.closeDuration * difference / 100.0);
    valve.startTime = millis();
    
    digitalWrite(valve.openPin, LOW);
    digitalWrite(valve.closePin, HIGH);

    valve.state = VALVE_POSITIONING_CLOSE;
  }
}

void stopValve(ValveControl &valve) {
  digitalWrite(valve.openPin, LOW);
  digitalWrite(valve.closePin, LOW);
  valve.state = VALVE_IDLE;
}

void calibrateValve(ValveControl &valve) {
    stopValve(valve);

    valve.calibration = true;
    valve.calibrated = false;

    valve.startTime = millis();
    valve.state = VALVE_CALIBRATING_STARTING;

    digitalWrite(valve.closePin, LOW);
    digitalWrite(valve.openPin, HIGH);
}

void updateValve(ValveControl &valve) {
  switch (valve.state) {
    case VALVE_IDLE: break;
    case VALVE_OPENING:
      if (digitalRead(valve.isOpenPin) == LOW) {
        stopValve(valve);
        return;
      }
      break;
    case VALVE_CLOSING:
      if (digitalRead(valve.isClosePin) == LOW) {
        stopValve(valve);
        return;
      }
      break;
    case VALVE_POSITIONING_OPEN:
      if (digitalRead(valve.isOpenPin) == LOW) {
        stopValve(valve);
        valve.position = 100;
      }

      if (millis() - valve.startTime >= valve.movementDuration) {
        stopValve(valve);
        valve.position = valve.targetPosition;
      }
      break;
    case VALVE_POSITIONING_CLOSE:
      if (digitalRead(valve.isClosePin) == LOW) {
        stopValve(valve);
        valve.position = 0;
      }

      if (millis() - valve.startTime >= valve.movementDuration) {
        stopValve(valve);
        valve.position = valve.targetPosition;
      }
      break;
    case VALVE_CALIBRATING_STARTING:
      if (digitalRead(valve.isOpenPin) == LOW) {
          digitalWrite(valve.openPin, LOW);
          
          if (millis() - valve.startTime < CALIBRATION_IGNORE_MS)
              break;
          digitalWrite(valve.closePin, HIGH);

          valve.state = VALVE_CALIBRATING_CLOSE;
          valve.startTime = millis();
      }
      break;


    case VALVE_CALIBRATING_CLOSE:
      if (digitalRead(valve.isClosePin) == LOW) {
          digitalWrite(valve.closePin, LOW);

          if (valve.closeDuration == 0) 
            valve.closeDuration = millis() - valve.startTime;
          
          if (millis() - valve.startTime - valve.closeDuration < CALIBRATION_IGNORE_MS)
              break;

          digitalWrite(valve.openPin, HIGH);

          valve.state = VALVE_CALIBRATING_OPEN;
          valve.startTime = millis();
      }
      break;


    case VALVE_CALIBRATING_OPEN:
      if (digitalRead(valve.isOpenPin) == LOW) {
          valve.openDuration = millis() - valve.startTime;

          stopValve(valve);

          valve.calibrated = true;
          valve.calibration = false;
          valve.state = VALVE_IDLE;
      }
      break;
  }
}
#pragma endregion

#pragma region Analog Handler
void analogReadEnhanced(PressureMeasurement &pt) {
    pt.raw = analogRead(pt.pin);

    pt.percentage =
        ((float)(pt.raw - pt.raw0) /
        (pt.raw100 - pt.raw0)) * 100.0f;

    pt.percentage = constrain(pt.percentage, 0.0f, 100.0f);

    pt.U = pt.U0 +
        (pt.U100 - pt.U0) * (pt.percentage / 100.0f);

    pt.EU = pt.EU0 +
        (pt.EU100 - pt.EU0) * (pt.percentage / 100.0f);
}
#pragma endregion