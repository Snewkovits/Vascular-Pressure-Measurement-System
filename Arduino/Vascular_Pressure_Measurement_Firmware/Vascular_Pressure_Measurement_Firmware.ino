#define STX '<'
#define ETX '>'

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

byte calcChecksum(const String &s);
void handleSerial();
void processMessage(String msg);
void processCommand(String id, String cmd, String data);
int handleMeasure();
void sendMessage(String id, String command, String data);
void setParameter(String id, String data);
void getParameter(String id, String data);
void setInputOutput(const String &id, const String &data);
void getInputOutput(const String &id, const String &data);
int getPinMode(uint8_t pin);
void getPinModeCommand(const String &id, const String &data);

enum ValveState {
  VALVE_IDLE,
  VALVE_OPENING,
  VALVE_CLOSING,
  VALVE_POSITIONING_OPEN,
  VALVE_POSITIONING_CLOSE
};

struct ValveControl {
  ValveState state = VALVE_IDLE;

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
void updateValve(ValveControl &valve);

int wiredDigitals[] = {23, 25, 27, 29, 31, 33, 35, 37, 39, 41, 43, 45, 47, 49, 51, 53};
int relays[sizeof(wiredDigitals) / sizeof(wiredDigitals[0])];
int inputs[sizeof(wiredDigitals) / sizeof(wiredDigitals[0])];

// ======= VALVES PARAMETERS ======
ValveControl valve1;
ValveControl valve2;
ValveControl valve3;

bool ValveCalibrated = false;

String buffer = "";
bool inFrame = false;

bool measure = false;

unsigned long lastMeasure = 0;
const unsigned long interval = 10;

int lastValue = -1;
int fallingCount = 0;

int FALL_THRESHOLD = 3;   // ennyi egymás utáni csökkenés kell
int MIN_DELTA = 2;        // zajszűrés



// ================= SETUP =================
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

  pinMode(valve1.isOpenPin, INPUT_PULLUP);     // Valve 1 open
  pinMode(valve1.isClosePin, INPUT_PULLUP);    // Valve 1 close
  pinMode(valve2.isOpenPin, INPUT_PULLUP);     // Valve 2 open
  pinMode(valve2.isClosePin, INPUT_PULLUP);    // Valve 2 close
  pinMode(valve3.isOpenPin, INPUT_PULLUP);     // Valve 3 open
  pinMode(valve3.isClosePin, INPUT_PULLUP);    // Valve 3 close
}

// ================= LOOP =================
void loop() {
  handleSerial();   // mindig fusson
  updateValve(valve1);
  updateValve(valve2);
  updateValve(valve3);
}

// ================= CHECKSUM =================
byte calcChecksum(const String &s) {
  byte chk = 0;
  for (int i = 0; i < s.length(); i++) {
    chk ^= (byte)s[i];
  }
  return chk;
}

// ================= SERIAL PARSER =================
void handleSerial() {
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
      buffer += c;
    }
    // ha nem vagyunk frame-ben, eldobjuk -> resync
  }
}

// ================= MESSAGE PROCESS =================
void processMessage(String msg) {
  int p1 = msg.indexOf('|');
  int p2 = msg.indexOf('|', p1 + 1);
  int p3 = msg.lastIndexOf('|');

  if (p1 == -1 || p2 == -1 || p3 == -1) return;

  String id = msg.substring(0, p1);
  String cmd = msg.substring(p1 + 1, p2);
  String data = msg.substring(p2 + 1, p3);
  String chkStr = msg.substring(p3 + 1);

  byte chkCalc = calcChecksum(msg.substring(0, p3));
  byte chkRecv = (byte) strtol(chkStr.c_str(), NULL, 16);

  /*
  if (chkCalc != chkRecv) {
    sendMessage(id, "ERR", "CHK");
    return;
  }*/

  processCommand(id, cmd, data);
}

// ================= COMMAND HANDLER =================
void processCommand(String id, String cmd, String data) {
  if (cmd == "PING") {
    sendMessage(id, "PONG", "ACK");
  }
  
  else if (cmd == "START_MEASURE") {
    measure = true;
    fallingCount = 0;
    lastValue = -1;
    lastMeasure = millis(); // reset timing
    sendMessage(id, "ACK", String(fallingCount));
  }
  
  else if (cmd == "GET_MEASURE_DATA") {
    if (measure) {
      int value = handleMeasure();
      if (value != -1001) {
          sendMessage(id, "MEASURE_DATA", String(value));
      }
      else {
          sendMessage(id, "STOP_MEASURE", "FALL_DETECTED");
          measure = false;
      }
    }
    else {
      sendMessage(id, "ERR", "Measure is not started yet.");
    }
  }
  
  else if (cmd == "STOP_MEASURE") {
    measure = false;
    // ide kell majd a measure leállítása
    sendMessage(id, "ACK", "STOPPED");
  }
  
  else if (cmd == "SET_PARAM") {
    setParameter(id, data);
  }
  
  else if (cmd == "GET_PARAM") {
    getParameter(id, data);
  }
  
  else if (cmd == "GET_BOARD_DATAS") {
    sendMessage(id, "BOARD_DATAS", String(BOARD_TYPE) + ";" + String(DIGITAL_PINS) + ";" + String(ANALOG_PINS));
  }
  
  else if (cmd == "SET_IO") {
    setInputOutput(id, data);
  }
  
  else if (cmd == "GET_IO") {
    getInputOutput(id, data);
  }
  
  else if (cmd == "GET_PIN_MODE") {
    getPinModeCommand(id, data);
  }
  
  else if (cmd == "CLOSE_VALVE") {
    int value = data.toInt();
    switch (value) {
    
    case 1:
        startCloseValve(valve1, id);
        break;
    
    case 2:
        startCloseValve(valve2, id);
        break;
    
    case 3:
        startCloseValve(valve3, id);
        break;
    default:
        sendMessage(id, "ERR", "INVALID VALVE");
        break;
    }
  }
  
  else if (cmd == "OPEN_VALVE") {
    int value = data.toInt();
    switch (value) {
    
    case 1:
        startOpenValve(valve1, id);
        break;
    
    case 2:
        startOpenValve(valve2, id);
        break;
    
    case 3:
        startOpenValve(valve3, id);
        break;
    default:
        sendMessage(id, "ERR", "INVALID VALVE");
        return;
    }
  }
  
  else if (cmd == "SET_VALVE_POSITION") {
    int p1 = data.indexOf(";");

    if (p1 == -1) {
      sendMessage(id, "ERR", "INVALID FORMAT");
      return;
    }

    int valveNumber = data.substring(0, p1).toInt();
    int position = data.substring(p1 + 1).toInt();

    switch (valveNumber) {
      case 1:
        positioningValve(valve1, position, id);
        break;
      case 2:
        positioningValve(valve2, position, id);
        break;
      case 3:
        positioningValve(valve3, position, id);
        break;
      default:
        sendMessage(id, "ERR", "INVALID VALVE");
        break;
    }
  }

  else {
    sendMessage(id, "ERR", "INVALID COMMAND");
  }
}

// ================= MEASURE LOOP =================
int handleMeasure() {
  unsigned long now = millis();

  if (now - lastMeasure >= interval) {
    lastMeasure = now;

    int value = analogRead(A0);

    // ===== Trend figyelés =====
    if (lastValue != -1) {
      if (value < lastValue - MIN_DELTA) {
        fallingCount++;
      } else {
        fallingCount = 0;
      }
    }

    lastValue = value;

    // ===== Stop feltétel =====
    if (fallingCount >= FALL_THRESHOLD) {
      measure = false;
      return -1001;
    }
    return value;
  }
  return -1;
}

void stopMeasureSequent() {
  // ide kell majd a measure leállítási rész
}

// ================= SEND =================
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

// ================= PARAM =================
void setParameter(String id, String data) {
  // Megkeressük a pontosvesszőt
  int p1 = data.indexOf(';');
  
  // HA NINCS pontosvessző, azonnal hibával térünk vissza
  if (p1 == -1) {
    sendMessage(id, "ERR", "INVALID FORMAT");
    return;
  }

  // Szétvágjuk a stringet
  String param = data.substring(0, p1);
  String value = data.substring(p1 + 1);

  // Levágjuk a láthatatlan újsor (\n, \r) és szóköz karaktereket
  param.trim();
  value.trim();

  // 4. Paraméterek vizsgálata
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

// ============== VALVES ==============

void startOpenValve(ValveControl &valve, String id) {
  if (digitalRead(valve.isOpenPin) == LOW) {
    digitalWrite(valve.openPin, LOW);
    return;
  }

  digitalWrite(valve.closePin, LOW);
  digitalWrite(valve.openPin, HIGH);

  valve.state = VALVE_OPENING;
  sendMessage(id, "ACK", "STARTED");
}

void startCloseValve(ValveControl &valve, String id) {
  if (digitalRead(valve.isClosePin) == LOW) {
    digitalWrite(valve.closePin, LOW);
    return;
  }

  digitalWrite(valve.openPin, LOW);
  digitalWrite(valve.closePin, HIGH);

  valve.state = VALVE_CLOSING;
  sendMessage(id, "ACK", "STARTED");
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

void updateValve(ValveControl &valve) {
  switch (valve.state) {
    case VALVE_IDLE: break;
    case VALVE_OPENING:
      if (digitalRead(valve.isOpenPin) == LOW) {
        digitalWrite(valve.openPin, LOW);
        valve.state = VALVE_IDLE;
        return;
      }
      break;
    case VALVE_CLOSING:
      if (digitalRead(valve.isClosePin) == LOW) {
        digitalWrite(valve.closePin, LOW);
        valve.state = VALVE_IDLE;
        return;
      }
      break;
    case VALVE_POSITIONING_OPEN:
      if (digitalRead(valve.isOpenPin) == LOW) {
        digitalWrite(valve.openPin, LOW);
        valve.position = 100;
        valve.state = VALVE_IDLE;
      }

      if (millis() - valve.startTime >= valve.movementDuration) {
        digitalWrite(valve.openPin, LOW);
        valve.position = valve.targetPosition;
        valve.state = VALVE_IDLE;
      }
      break;
    case VALVE_POSITIONING_CLOSE:
      if (digitalRead(valve.isClosePin) == LOW) {
        digitalWrite(valve.closePin, LOW);
        valve.position = 0;
        valve.state = VALVE_IDLE;
      }

      if (millis() - valve.startTime >= valve.movementDuration) {
        digitalWrite(valve.closePin, LOW);
        valve.position = valve.targetPosition;
        valve.state = VALVE_IDLE;
      }
      break;
  }
}