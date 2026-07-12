#define STX '<'
#define ETX '>'

String buffer = "";
bool inFrame = false;

bool measure = false;
String currentId = "";

unsigned long lastMeasure = 0;
const unsigned long interval = 10;

int lastValue = -1;
int fallingCount = 0;

int FALL_THRESHOLD = 3;   // ennyi egymás utáni csökkenés kell
int MIN_DELTA = 2;        // zajszűrés (ADC jitter ellen)

// ================= CHECKSUM =================
byte calcChecksum(const String &s) {
  byte chk = 0;
  for (int i = 0; i < s.length(); i++) {
    chk ^= (byte)s[i];
  }
  return chk;
}

// ================= SETUP =================
void setup() {
  Serial.begin(1000000);

  for (int pin = 2; pin <= 13; pin++) {
    pinMode(pin, INPUT);
  }

  pinMode(5, OUTPUT);
  pinMode(6, OUTPUT);
}

// ================= LOOP =================
void loop() {
  handleSerial();   // mindig fusson
  handleMeasure();  // nem blokkoló mérés
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
    // ha nem vagyunk frame-ben, eldobjuk → resync
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

  if (chkCalc != chkRecv) {
    sendMessage(id, "ERR", "CHK");
    return;
  }

  processCommand(id, cmd, data);
}

// ================= COMMAND HANDLER =================
void processCommand(String id, String cmd, String data) {

  if (cmd == "PING") {
    sendMessage(id, "PONG", "ACK");
  }

  else if (cmd == "START_MEASURE") {
    measure = true;
    currentId = id;

    lastMeasure = millis(); // reset timing
    sendMessage(id, "ACK", "STARTED");
  }

  else if (cmd == "STOP_MEASURE") {
    measure = false;
    sendMessage(id, "ACK", "STOPPED");
  }

  else if (cmd == "SET_PARAM") {
    setParameter(id, data);
  }

  else if (cmd == "GET_PARAM") {
    getParameter(id, data);
  }

  else {
    sendMessage(id, "ERR", "CMD");
  }
}

// ================= MEASURE LOOP =================
void handleMeasure() {
  if (!measure) return;

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
      sendMessage(currentId, "STOP_MEASURE", "FALL_DETECTED");
      return;
    }

    // ===== Normál küldés =====
    sendMessage(currentId, "MEASURE", String(value));
  }
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
  // 1. Megkeressük a pontosvesszőt
  int p1 = data.indexOf(';');
  
  // HA NINCS pontosvessző, azonnal hibával térünk vissza
  if (p1 == -1) {
    sendMessage(id, "ERR", "INVALID FORMAT");
    return;
  }

  // 2. Szétvágjuk a stringet
  String param = data.substring(0, p1);
  String value = data.substring(p1 + 1);

  // 3. Levágjuk a láthatatlan újsor (\n, \r) és szóköz karaktereket
  param.trim();
  value.trim();

  // 4. Paraméterek vizsgálata
  if (param == "MD") {
    MIN_DELTA = value.toInt();
    sendMessage(id, "ACK", String(MIN_DELTA));
  }
  else if (param == "FT") {
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