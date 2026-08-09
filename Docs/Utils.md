# `Utils` névtér – Kommunikáció és mérés

Ez a fájl a `Vascular_Pressure_Measurement_System.Utils` névtér négy osztályát dokumentálja:
`Connection`, `Measure`, `GlobalData`, `Trace`. Ezek együtt alkotják az alkalmazás
"motorját": a soros porti kommunikációt, a mérési ciklust és a hozzájuk tartozó
szálbiztos, globálisan elérhető állapotot.

⬅ Vissza az [áttekintéshez](./00-README.md)

---

## 1. `Trace` (`Trace.cs`)

Egyszerű, statikus fájl-naplózó segédosztály.

### Mezők / állapot
Nincs tárolt állapot – minden hívás önállóan nyitja-zárja a naplófájlt.

### `public static void WriteTrace(string message)`

**Mit csinál:** időbélyeggel ellátott sort ír a `trace.log` fájlba.

**Működés lépésről lépésre:**
1. Meghatározza a felhasználó `%LocalAppData%` mappáját
   (`Environment.SpecialFolder.LocalApplicationData`).
2. Ebben létrehozza/megkeresi a `Vascular_Pressure_Measurement_System` almappát.
3. Ha az almappa még nem létezik, létrehozza (`Directory.CreateDirectory`).
4. Megnyitja hozzáfűzés (`append = true`) módban a `trace.log` fájlt, és egy sort ír bele
   `yyyy-MM-dd HH:mm:ss - <üzenet>` formátumban.

**Hívás helye a projektben:** jelenleg egyetlen helyről hívják – a
`Connection.ContinousTest()`-ből, amikor a `Hardware.SetParameters()` hiba nélkül nem fut le
(azaz kivételt dob) az eszköz konfigurálásakor.

**Megjegyzés:** minden híváskor újra megnyitja a fájlt (`using StreamWriter`), tehát nem tart
nyitva fájlleírót – ez biztonságos több szálról hívva is, de gyakori naplózásnál lassabb
lehet (fájl I/O minden sornál).

---

## 2. `GlobalData` (`GlobalData.cs`)

Statikus, alkalmazás-szintű "tábla", amely globálisan elérhető színeket és egy
esemény-alapú kapcsolat-állapotot biztosít.

### Mezők / tulajdonságok

| Tag | Típus | Jelentés |
|---|---|---|
| `UDGreen` | `Color` | Az alkalmazás sötétzöld márkaszíne (`RGB(20, 68, 56)`) – a legtöbb ablak háttérszíne |
| `UDYellow` | `Color` | Az alkalmazás sárga márkaszíne (`RGB(245, 180, 24)`) – pl. a grafikon egyik szériaszíne |
| `SerialConnectionStatus` | `bool` (property) | Az aktuális kapcsolat-állapot; **csak akkor** vált ki eseményt, ha az érték ténylegesen megváltozik |
| `SerialConnectionStatusChanged` | `event EventHandler` | Feliratkozható esemény, amikor a kapcsolat állapota (csatlakozva/nincs csatlakozva) megváltozik |

### `SerialConnectionStatus` property

```csharp
public static bool SerialConnectionStatus
{
    get => _serialConnectionStatus;
    set
    {
        if (_serialConnectionStatus != value)
        {
            _serialConnectionStatus = value;
            SerialConnectionStatusChanged?.Invoke(null, EventArgs.Empty);
        }
    }
}
```

**Mit csinál:** ez a **publish/subscribe** mintázat gerince az alkalmazásban. A `Connection`
osztály írja ezt a property-t (amikor kapcsolódik/lecsatlakozik), a `Main` és a
`Diagnostics` ablakok pedig feliratkoznak a `SerialConnectionStatusChanged` eseményre, hogy
azonnal reagálhassanak (gombok engedélyezése/tiltása, cím frissítése, dinamikus UI
törlése/újragenerálása).

**Fontos:** az esemény **csak tényleges változáskor** tüzel (nem minden setter-hívásra),
így elkerülhető a felesleges UI-frissítés, ha pl. a `ContinousTest` szál minden ciklusban
újra `true`-ra állítaná ugyanazt az értéket.

---

## 3. `Connection` (`Connection.cs`)

Statikus osztály, amely a **soros port teljes életciklusát** kezeli: portkeresés, nyitás,
folyamatos élőség-ellenőrzés (heartbeat), automatikus újracsatlakozás, illetve a
kérés-válasz alapú üzenetküldést a fent leírt keretprotokoll szerint.

### Statikus mezők

| Mező | Típus | Jelentés |
|---|---|---|
| `isConnected` | `bool` | Igaz, ha jelenleg él a kapcsolat az eszközzel |
| `stopConnection` | `bool` | Ha `true`-ra állítják, a `ContinousTest` háttérszála leáll (app bezáráskor használt) |
| `serialPort` | `SerialPort` | Az aktuálisan (esetleg) nyitott port objektum |
| `msgId` | `int` (privát) | Növekvő üzenetazonosító számláló |
| `faildAttempt` | `int` | Egymást követő sikertelen üzenetváltások száma |
| `_serialPortLock` | `object` (belső, csak az assembly-n belül látható) | A porthoz való hozzáférést védő zár |

### `public static SerialPort GetSerialPort()`

**Mit csinál:** végigpróbálja a rendszeren elérhető **összes soros portot**, és megkeresi,
melyiken válaszol az eszköz.

**Működés:**
1. Lekéri az összes elérhető port nevét (`SerialPort.GetPortNames()`).
2. Minden port névre létrehoz egy `SerialPort` objektumot a rögzített paraméterekkel
   (1 000 000 baud, 8N1, ASCII, 100 ms időtúllépés, 64 KB puffer).
3. Megpróbálja megnyitni a portot; ha nem sikerül (kivétel), a következő portra lép.
4. Ha sikerült megnyitni, elküld egy `PING` üzenetet (`SendMessage("PING", "")`) és
   megnézi, hogy a válasz `"PONG"`-e.
5. Ha igen → **ez a helyes port**, visszaadja azt.
6. Ha `TimeoutException` történik (nincs válasz), csendben a következő portra lép.
7. **Minden ágon** (siker vagy hiba esetén is) a `finally` blokk lezárja az aktuálisan
   próbált portot, mielőtt továbblépne / visszatérne.
8. Ha egyik port sem válaszolt megfelelően, `null`-t ad vissza.

> ⚠️ **Fontos részlet:** mivel a `finally` blokk *sikeres* találat esetén is lefut a
> `return` előtt, a visszaadott `SerialPort` objektum **már zárt állapotban** van!
> A hívónak (`ContinousTest`) ezért újra meg kell nyitnia a portot a kapott objektumon.
> Ez a tervezés tudatosan választja szét a "melyik port a jó" keresést a tényleges,
> hosszú távú port-életciklus kezeléstől.

### `public static void ContinousTest()`

**Mit csinál:** elindít egy **örökké futó háttérszálat**, amely 100 ms-onként ellenőrzi és
szükség esetén helyreállítja a kapcsolatot az eszközzel. Ezt a `Main` konstruktora indítja
el az alkalmazás indulásakor, és csak az alkalmazás bezárásakor (`stopConnection = true`)
áll le.

**Ciklus minden iterációban (`lock(_serialPortLock)` védelem alatt):**

- **Ha nincs nyitott port** (`serialPort == null || !serialPort.IsOpen`):
  - meghívja a `GetSerialPort()`-ot;
  - ha talált portot, megnyitja (`serialPort.Open()`), `isConnected = true`,
    `GlobalData.SerialConnectionStatus = true`, és jelzi, hogy a konfiguráció még nincs
    elküldve (`isConfigSent = false`);
  - ha a nyitás kivételt dob, vagy nem talált portot → `isConnected = false`,
    `GlobalData.SerialConnectionStatus = false`.
- **Ha már van nyitott port:**
  - `PING`-et küld; ha a válasz `PONG`:
    - ha még **nem küldte el** az induláskori hardver-konfigurációt
      (`!isConfigSent`), meghívja a `Configuration.Hardware.SetParameters()`-t
      (paraméterek nélkül → ez a mentett fájl-konfigurációt tölti fel az eszközre,
      lásd [02. fájl](./02-Configuration-Hardver-Beallitasok.md)); hibát `Trace.WriteTrace`-szel naplóz;
    - `isConnected = true`, állapot frissítése;
  - ha a válasz **nem** `PONG`, vagy kivétel történt → `CloseConnection()` hívása, és
    `isConfigSent = false` (a következő sikeres csatlakozáskor újra elküldi a konfigot).
- A ciklus végén `Thread.Sleep(100)`.

**Miért fontos ez a zár?** Amíg a `ContinousTest` a `_serialPortLock`-ot tartja (akár mert
portot keres, akár mert PING-et küld), addig sem a `Measure`, sem a `Diagnostics` nem tud a
porton üzenetet küldeni (mivel a `SendMessage` is ugyanezt a zárat kéri) – így elkerülhető,
hogy két üzenet összekeveredjen a vezetéken.

### `private static void CloseConnection()`

Segédfüggvény: `isConnected = false`, `GlobalData.SerialConnectionStatus = false`, és ha a
port nyitva van, bezárja azt. Három helyről hívják a `ContinousTest`-en belül (sikertelen
PONG, kivétel a PING közben), hogy ne kelljen a lezárási logikát duplikálni.

### `static byte CalculateChecksum(string message)`

A `message` (jellemzően az `ID|CMD|DATA` payload) minden karakterét XOR-olja össze,
`byte`-ként visszaadva az eredményt. Ez képezi a keret `CHK` mezőjét (2 jegyű hex formában).

### `public static string[] SendMessage(string cmd, string data)`

**Ez a kommunikáció szíve** – egy kérés-válasz ciklust hajt végre, és **soha nem dob
kivételt kifelé**: minden hibaágon `["ERR", "<leírás>"]` tömböt ad vissza.

**Bemenet:** `cmd` – parancs neve (pl. `"PING"`, vagy `Connection.CommandType.*`
konstansok); `data` – a parancshoz tartozó adat (lehet üres string).

**Visszatérési érték:** siker esetén `[válasz_CMD, válasz_DATA]` (2 elemű tömb); hiba esetén
`["ERR", "<hibaüzenet>"]`.

**Lépések (mind `lock(_serialPortLock)` alatt fut):**

1. Ha nincs nyitott port → azonnal `["ERR", "Device is not connected"]`.
2. Összeállítja az üzenetet: `id = msgId++`, `payload = "{id}|{cmd}|{data}"`,
   `chk = CalculateChecksum(payload)`, `msg = "<{payload}|{chk:X2}>"`.
3. `ClearBuffers()` – kiüríti a be- és kimeneti puffereket, hogy ne maradjon "szemét" a
   soros vonalon a korábbi üzenetváltásból. Ha ez kivételt dob → `HandleFailure()` +
   `["ERR", "Buffer error"]`.
4. Kiírja az üzenetet a portra (`serialPort.Write(msg)`).
5. Beolvassa a választ a következő `>` karakterig (`serialPort.ReadTo(">")`), majd
   levágja a `<`/`>` határolójeleket, és `|` mentén 4 részre bontja.
6. **Formátum-ellenőrzés:** ha nem pontosan 4 rész jött → `HandleFailure()` +
   `["ERR", "Invalid format"]`.
7. **ID-ellenőrzés:** ha a válasz ID-je nem egyezik a küldött ID-vel →
   `["ERR", "ID mismatch"]` (ez az egyetlen hibaág, amely **nem** hívja a
   `HandleFailure()`-t).
8. **Ellenőrzőösszeg-ellenőrzés:** ha a kiszámolt XOR checksum nem egyezik a válaszban
   kapott `CHK` mezővel → `["ERR", "Checksum error"]` (ez sem hívja a `HandleFailure()`-t).
9. Siker esetén `faildAttempt = 0`, és visszaadja a `[CMD, DATA]` párost.
10. `TimeoutException` esetén → `HandleFailure()` + `["ERR", "Timeout"]`.
11. Bármely más kivétel esetén → `HandleFailure()` + `["ERR", ex.Message]`.

### `private static void ClearBuffers()`

Ha a port nyitva van, törli a bejövő és kimenő puffereket (`DiscardInBuffer`,
`DiscardOutBuffer`). Kivétel esetén csak `Debug.WriteLine`-nal naplóz, nem dob tovább.

### `private static void HandleFailure()`

Növeli a `faildAttempt` számlálót; ha elérte a **3**-at, lezárja a portot, beállítja
`isConnected = false`-t, frissíti a `GlobalData`-t, és visszaállítja a számlálót 0-ra.
Ez a "3 egymást követő hiba → tekintsük megszakadtnak a kapcsolatot" logika közös
implementációja.

### `public static string[] ReadMessage()`

Passzív olvasás: **nem küld semmit**, csak megpróbál egy üzenetet beolvasni a portról,
és ugyanúgy validálja (4 részes formátum, checksum), mint a `SendMessage` a válasz
feldolgozásakor. Hiba esetén szintén növeli a `faildAttempt`-et, és 3 hibánál lezárja a
kapcsolatot. **A mellékelt kódban jelenleg nincs meghívva** – valószínűleg jövőbeli,
eszköz-kezdeményezésű (aszinkron) üzenetek fogadására készült elő.

### `public static class CommandType`

Névtér-szerű konstansgyűjtemény a protokoll parancsszavaihoz (lásd táblázat a
[README-ben](./00-README.md#32-parancstípusok-connectioncommandtype)). Ezeket használja
minden hívó a "mágikus string" elgépelések elkerülésére (bár néhány helyen, pl.
`Measure.Start()`-ban, a kód közvetlenül `"START_MEASURE"` string literált használ a
konstans helyett – funkcionálisan ekvivalens).

---

## 4. `Measure` (`Measure.cs`)

Egy **mérési munkamenetet** (session) reprezentál: elindítja a mérést az eszközön,
külön szálon folyamatosan lekérdezi az adatokat, és egy szálbiztos pufferbe teszi őket,
amit a `Main` form UI-időzítője olvas ki.

### Mezők

| Mező | Típus | Jelentés |
|---|---|---|
| `stopMeasure` | `bool` | Kooperatív leállítási jelzőzászló a mérési szál felé |
| `running` | `bool` | Igaz, amíg a mérési szál fut |
| `_lock` | `object` (publikus) | A `Buffer`-hez való hozzáférést védő zár |
| `Buffer` | `Queue` (publikus) | A beérkezett mérési minták FIFO sora (stringként tárolva) |
| `mainForm` | `Form` | Referencia a főablakra (UI-frissítéshez) |
| `isFormClosing` | `bool` | Ha igaz, a mérési szál a leállás után **nem** próbál UI-t frissíteni |

### `public Measure(Form mainForm)`

Konstruktor: inicializálja a `Buffer`-t (`new Queue()`) és a `_lock` objektumot, illetve
eltárolja a főablak referenciáját, hogy a mérés végén vissza tudjon írni a felületre.

### `public bool isRunning()`

Egyszerű lekérdező metódus, a `running` mező aktuális értékét adja vissza. A `Main` ezt
használja pl. annak eldöntésére, hogy a `chartUpdateTimer` timer-t még pörgesse-e, illetve
hogy indítható-e új mérés vagy megnyitható-e a diagnosztika ablak.

### `public void Stop(bool isFormClosing = false)`

**Mit csinál:** jelzi a futó mérési szálnak, hogy fejezze be a munkát – **nem** erőszakos
szálmegszakítás (`Thread.Abort`), hanem kooperatív jelzés: beállítja a `stopMeasure = true`
és `isFormClosing` mezőket. A szál a következő ciklusiterációban veszi észre a jelzést és
lép ki a `while` ciklusból.

**Hívási helyek:**
- `Main.ForceStopButton_Click` → `measure.Stop()` (felhasználói leállítás, UI-frissítéssel).
- `Main.Main_FormClosing` → `measure.Stop(true)` (az ablak bezárásakor – ekkor **nem**
  próbál a szál a (már bezáródó) UI-ra visszaírni).

### `public void Start()`

**Mit csinál:** elindítja a mérést – először egy szinkron kézfogást végez az eszközzel,
majd elindít egy dedikált háttérszálat a folyamatos adatgyűjtéshez.

**Lépések:**

1. Ha nincs kapcsolat (`!Connection.isConnected`) → azonnal visszatér, nem csinál semmit.
2. Beállítja `running = true`, `stopMeasure = false` (biztonsági reset), `counter = 0`
   (lokális változó – csak a ciklusban növekszik, később nincs felhasználva).
3. Elküldi a `"START_MEASURE"` parancsot (`Connection.SendMessage`).
4. Ha a válasz nem legalább 2 elemű, vagy az első elem nem `"ACK"` → `running = false`,
   **a szálat el sem indítja** (nincs értelme adatot kérni, ha az eszköz nem nyugtázta az
   indítást).
5. Ha kapott `ACK`-ot, elindít egy új `Thread`-et:
   - **A teljes szál törzse `lock (Connection._serialPortLock)` alatt fut** – tehát amíg a
     mérés tart, a `ContinousTest` PING-szála nem tud a portba "beszólni".
   - `while (!stopMeasure && Connection.isConnected)` ciklus:
     - `GET_MEASURE_DATA` küldése;
     - ha a válasz `"ERR"` → `Connection.faildAttempt++`; ha elérte a 3-at, akkor
       `Connection.stopConnection = true` és `break` (ez a hiba a globális
       kapcsolat-figyelő szálat is leállítja – a rendszer teljes lecsatlakozottnak
       tekinti magát);
     - ha a válasz `"STOP_MEASURE"` → `break` (az **eszköz saját maga** jelezte a mérés
       végét, pl. esésérzékelés miatt);
     - ha a válasz `"MEASURE_DATA"` → a kapott értéket (`data[1]`) `lock(_lock)` alatt
       beteszi a `Buffer`-be, majd `counter++`;
     - `Thread.Sleep(1)` – rövid szünet, hogy a ciklus ne pörögjön feleslegesen (CPU
       kímélés), miközben gyakorlatilag folyamatos mintavételezést biztosít.
   - A ciklus után: ha a leállítást a **felhasználó** kezdeményezte
     (`stopMeasure == true`) és a kapcsolat még él, elküldi a `"STOP_MEASURE"` parancsot
     az eszköznek is (hogy az is tudja, a mérésnek vége). Mivel ez a hívás már a
     `_serialPortLock` birtokában történik, és a `SendMessage` belső zárolása **re-entrant**
     (ugyanaz a szál újra megszerezheti a saját zárját), ez nem okoz holtpontot.
   - A `lock` blokk után: ha **nem** ablakbezárás miatt állt le (`!isFormClosing`), a
     `mainForm.Invoke(...)` segítségével UI-szálon visszaállítja a gombok állapotát:
     - `ForceStopButton` letiltva (szürke),
     - `StartMeasuring`, `SaveButton`, `SettingsButton` engedélyezve (fehér).
   - Végül `running = false`, `stopMeasure = false` (készen áll a következő mérésre).
6. `.Start()` – elindítja a fent leírt szálat.

**Összefoglalva a `Measure` szerepe:** egy állapotgép, amely biztosítja, hogy egyszerre
csak egy mérés fusson, a soros port kizárólagos hozzáférését garantálja mérés közben, és
a beérkező adatokat egy szálbiztos pufferen keresztül teszi elérhetővé a UI számára anélkül,
hogy a UI-szálat blokkolná.

---

⬅ Vissza az [áttekintéshez](./00-README.md) · Következő: [Configuration névtér](./02-Configuration-Hardver-Beallitasok.md)
