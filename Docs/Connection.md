# GetSerialPort()

## Áttekintés
A `GetSerialPort` metódus végigmegy a számítógépen elérhető összes soros porton (COM portok), megpróbálja megnyitni őket, és egy `PING` parancs segítségével ellenőrzi, hogy a keresett eszköz csatlakozik-e hozzájuk.

Ha a port sikeresen válaszol a PONG üzenettel, a metódus visszaadja a konfigurált, nyitott SerialPort objektumot. Ha egyik port sem ad megfelelő választ, a visszatérési érték null.

## Szintaxis
```cs
public static System.IO.Ports.SerialPort GetSerialPort()
```
A metódus nem fogad paramétereket.

### Visszatérési érték

* Típus: System.IO.Ports.SerialPort
* Egy inicializált és megnyitott SerialPort objektum, ha a kapcsolat sikeresen létrejött a hardverrel.
* null, ha egyetlen elérhető porton sem található meg a megfelelő eszköz.

## Működési elv
A metódus az alábbi lépéseken megy keresztül a végrehajtás során:

1. Portok lekérdezése: Lekéri az operációs rendszertől az aktuálisan elérhető soros portok neveit (SerialPort.GetPortNames()).
2. Inicializálás: Minden egyes talált porthoz létrehoz egy új SerialPort példányt az alábbi alapértelmezett konfigurációval:
    * BaudRate: 9600
    * Parity: Parity.None
    * DataBits: 8
    * StopBits: StopBits.One
    * Encoding: Encoding.ASCII
    * ReadTimeout / WriteTimeout: 1000 ms (1 másodperc)
3. Kézfogás (Handshake): * Megkísérli megnyitni a portot.
    * Küld egy PING üzenetet a  metóduson keresztül.
    * Várja a választ. Ha a válasz pontosan "PONG", a keresés sikeres, és a nyitott port példányát adja vissza.
4. Takarítás: Ha a port nem nyitható meg, vagy a válasz időtúllépés miatt elmarad/hibás, a metódus lezárja az adott portot, és továbblép a következő elérhető portra.

## Példa használat
```cs
using Vascular_Pressure_Measurement_System.Utils;
using System.IO.Ports;

// Megpróbáljuk megkeresni az eszközt a soros portokon
SerialPort activePort = Connection.GetSerialPort();

if (activePort != null)
{
    Console.WriteLine($"Sikeres csatlakozás a következő porton: {activePort.PortName}");
    // A Connection.isConnected állapotjelző frissítése a hívó környezetben...
}
else
{
    Console.WriteLine("Az eszköz nem található egyetlen elérhető soros porton sem.");
}
```
## Kivételkezelés és Megjegyzések

Belső kivételkezelés: A metódus saját maga kezeli a port megnyitásakor fellépő általános kivételeket (`Exception`), valamint a kommunikáció során fellépő időtúllépéseket (`TimeoutException`). Emiatt a hívó oldalon nem szükséges `try-catch` blokkba csomagolni a hívást; hiba esetén a metódus biztonságosan `null` értékkel tér vissza.

Erőforrás-kezelés: Ha a metódus megtalálja a megfelelő eszközt, a `SerialPort` objektumot nyitott (Open) állapotban adja vissza, így az azonnal kész a további adatcserére. Ha a csatlakozás sikertelen, a vizsgált portok minden esetben lezárásra (`Close()`) kerülnek

# ContinousTest()

## Áttekintés
A ContinousTest metódus egy különálló háttérszálat indít el, amely folyamatosan, 1 másodperces időközönként ellenőrzi és fenntartja a soros porti kapcsolatot az eszközzel. Ha a kapcsolat megszakad, vagy a port bezáródik, a metódus megkísérli az automatikus újracsatlakozást a portok ismételt végigpásztázásával.

## Szintaxis
```cs
public static void ContinousTest()
```
### Paraméterek
A metódus nem fogad paramétereket.

### Visszatérési érték

* Típus: void
* A metódus azonnal visszatér a hívóhoz, mivel a benne található logikai ciklust egy új Thread (háttérszál) indításával futtatja le.

## Működési elv
A metódus meghívásakor elindul egy végtelenített ciklus, amely addig fut, amíg a stopConnection változó értéke false. Minden iterációban az alábbi folyamat zajlik le:
1. Szálbiztonság (Lockolás)
A teljes ellenőrzési folyamat egy _serialPortLock objektummal van zárolva (lock). Ez biztosítja, hogy amíg a kapcsolat állapota ellenőrzés vagy javítás alatt áll, más szálak ne tudjanak adatot küldeni a metóduson keresztül.
2. Kapcsolódási állapot vizsgálata
A ciklus két fő ágra szakad a serialPort aktuális állapota alapján:
    * Ha nincs aktív kapcsolat (serialPort == null vagy nincs megnyitva):
        1. Meghívja a  metódust az eszköz felkutatására.
        2. Ha sikeres a keresés, beállítja a globális paramétereket a GlobalData.SetParameters() segítségével.
        3. Megkísérli megnyitni a portot, majd siker esetén beállítja az isConnected és a GlobalData.SerialConnectionStatus jelzőket true értékre.
        4. Ha a port nem található vagy nem nyitható meg, a jelzők false értékre váltanak.
    * Ha a kapcsolat elvileg él (a port nyitva van):
        1. Küld egy PING parancsot az eszköznek.
        2. Ha a válasz sikeresen PONG, megerősíti az isConnected és GlobalData.SerialConnectionStatus állapotok true értékét.
        3. Ha a válasz hibás, vagy a kommunikáció során kivétel (Exception) lép fel, meghívja a belső CloseConnection() metódust a kapcsolat biztonságos lezárásához.
3. Időzítés
Az ellenőrzési ciklus végén a szál 1000 ezredmásodpercre (Thread.Sleep(1000)) elalszik, így másodpercenként egyszer fut le a teszt.

## Példa használat
A metódust általában az alkalmazás indításakor vagy a kommunikációs modul inicializálásakor kell egyszer meghívni:
using Vascular_Pressure_Measurement_System.Utils;
```cs
// Folyamatos kapcsolat-ellenőrzés elindítása a háttérben
Connection.ContinousTest();

// Az alkalmazás futása közben a GlobalData-n keresztül ellenőrizhető az állapot:
if (GlobalData.SerialConnectionStatus)
{
    // Biztonságosan küldhetünk adatot
}
```
A kapcsolat leállításához és a szál lezárásához a stopConnection flaget kell átállítani:
```cs
// Kapcsolatkezelő szál leállítása (pl. az alkalmazás bezárásakor)
Connection.stopConnection = true;
```
##Fontos megjegyzések

Erőforrás-gazdálkodás: Mivel a metódus egy dedikált `Thread`-et indít, ügyelni kell arra, hogy az alkalmazás bezárásakor a `stopConnection` változó értéke true legyen, különben a háttérszál életben tarthatja az alkalmazás folyamatát.

Szálak közötti blokkolás: Mivel a `ContinousTest` másodpercenként lockolja a `_serialPortLock` objektumot az ellenőrzés idejére, a normál adatküldések (`SendMessage()`) minimális késleltetést tapasztalhatnak, ha pontosan az ellenőrzés pillanatában futnak be.

# CloseConnection()

## Áttekintés
A `CloseConnection` egy belső (privát) segédfüggvény, amely a soros porti kapcsolat biztonságos lezárásáért és a kapcsolódási állapotjelzők alaphelyzetbe állításáért felelős. A metódust az osztályon belüli hibakezelő folyamatok vagy az ellenőrző ciklus hívja meg, ha a kommunikáció megszakad vagy sikertelenné válik.

## Szintaxis
```cs
private static void CloseConnection()
```

### Visszatérési érték
* Típus: void
* A metódus közvetlenül végrehajtja a port lezárását és az állapotváltozók módosítását, visszatérési értéke nincs.

## Működési elv
A metódus meghívásakor az alábbi lépések futnak le egymás után:
1. Állapotjelzők frissítése: * Az isConnected logikai változó értékét false állapotra állítja.
    * A GlobalData.SerialConnectionStatus globális tulajdonság értékét false állapotra állítja. ezzel jelezve az alkalmazás többi része felé, hogy a kapcsolat megszakadt.
2. Erőforrások felszabadítása:
    * Ellenőrzi, hogy a serialPort objektum inicializálva van-e (serialPort != null), valamint azt, hogy jelenleg nyitva van-e (serialPort.IsOpen).
    * Amennyiben a port nyitott állapotban van, meghívja a serialPort.Close() metódust, amely lezárja a portot és felszabadítja az operációs rendszer felé a kapcsolódó erőforrásokat.

## Példa használat
Mivel a metódus private láthatóságú, így közvetlenül csak a osztályon belülről érhető el. Tipikus használata a metódusban látható, amikor a PING parancsra nem érkezik megfelelő válasz:
```cs
try
{
    if (SendMessage(CommandType.PING, "")[0] == CommandType.PONG)
    {
        isConnected = true;
        GlobalData.SerialConnectionStatus = true;
    }
    else
    {
        // Hibás válasz esetén lezárjuk a kapcsolatot
        CloseConnection();
    }
}
catch (Exception)
{
    // Kivétel/időtúllépés esetén lezárjuk a kapcsolatot
    CloseConnection();
}
```
## Megjegyzések
Szálbiztonság: A `CloseConnection` metódus önmagában nem tartalmaz belső `lock` mechanizmust. Fontos, hogy a hívó környezet (mint például a `ContinousTest()`) gondoskodjon a `_serialPortLock` zárolásáról a metódus futása közben, megakadályozva, hogy egy másik szál egy időben próbáljon írni a már lezárás alatt álló portra.

Kódduplikáció elkerülése: A metódus elsődleges célja a kód tisztán tartása. Segítségével elkerülhető, hogy a kapcsolat megszakadásakor észlelt hibák helyén (három különböző ponton a ciklusban) újra és újra le kelljen írni a port ellenőrzését és lezárását.

# CalculateChecksum()

## Áttekintés
A CalculateChecksum egy belső (privát) segédfüggvény, amely a soros porton küldött és fogadott üzenetek integritásának ellenőrzésére szolgál. A metódus egy XOR (kizáró vagy) alapú ellenőrző összeget (checksum) számol a megadott szöveges üzenet karaktereiből. Ez biztosítja, hogy a kommunikáció során fellépő esetleges adatbitek sérülései detektálhatóak legyenek.

## Szintaxis
```cs
private static byte CalculateChecksum(string message)
```

### Paraméterek
* `message` (`string`): Az a szöveges üzenet (payload), amelynek az ellenőrző összegét ki kell számítani. A protokoll szerint ez az üzenet `<` és `>` karakterek, valamint a checksum szekció nélküli része (pl. `ID|CMD|DATA`).

### Visszatérési érték
* Típus: byte
* A sztring karaktereinek egymás utáni XOR-olásából származó 8 bites (1 bájtos) ellenőrző összeg.

## Működési elv
A metódus bájtonként halad végig a bemeneti szövegen az alábbiak szerint:
1. Inicializál egy checksum nevű byte típusú változót 0 értékkel.
2. Egy foreach ciklus segítségével végigmegy a message sztring összes karakterén.
3. Minden egyes karaktert bájttá kasztol ((byte)c), majd az aktuális checksum változó értékével XOR műveletet végez (^=).
4. A teljes szöveg feldolgozása után visszaadja a végső 8 bites értéket.

## Példa használat
A metódust az osztályon belül két fő helyen használja a rendszer:

### 1. Üzenet küldésekor (Generálás)
Az üzenet összeállításakor a metódus kiszámolja a payload checksumját, majd azt hexadecimális stringgé alakítva fűzi hozzá a csomaghoz:
```cs
string payload = $"{id}|{cmd}|{data}";
byte chk = CalculateChecksum(payload);

// Formázás kétjegyű hexadecimális számmá (X2)
string msg = $"<{payload}|{chk:X2}>";
```

### 2. Üzenet fogadásakor (Validálás)
A fogadott csomag feldolgozásakor a kapott payloadból újra kiszámításra kerül a checksum, amit összehasonlít a fogadott értékkel:
```cs
string calculatedPayload = $"{responsePayload[0]}|{responsePayload[1]}|{responsePayload[2]}";
byte expectedChecksum = Convert.ToByte(responsePayload[3], 16);

if (CalculateChecksum(calculatedPayload) != expectedChecksum)
{
    // A checksum nem egyezik, az üzenet hibás / sérült!
    return new string[] { CommandType.ERR, "" };
}
```

## Megjegyzések
Protokollformátum: A hálózati csomagban a checksum mindig kétjegyű, nagybetűs hexadecimális formátumban (`:X2`) szerepel (pl. `A5`, `0F`), míg ez a függvény magát a nyers `byte` értéket adja vissza.

XOR tulajdonság: Az XOR (`^`) alapú ellenőrző összeg rendkívül gyorsan kiszámítható és alacsony számítási igénnyel rendelkezik, ami ideálissá teszi a beágyazott rendszerekkel (például Arduino) való mikrokontrolleres kommunikációhoz.


# SendMessage()

## Áttekintés
A `SendMessage` metódus a projekt kommunikációs protokolljának a szíve. Feladata, hogy egy szigorúan formázott, egyedi azonosítóval és ellenőrző összeggel (checksum) ellátott üzenetet küldjön a soros porton keresztül a csatlakoztatott eszköznek, majd megvárja és validálja az arra érkező választ. A metódus teljesen szálbiztos, így egy időben több programszálról is biztonságosan hívható.

## Szintaxis
```cs
public static string[] SendMessage(string cmd, string data)
```

### Paraméterek
* cmd (string): A küldeni kívánt parancs típusa (általában a  konstansai közül, pl. PING, SET_PARAM).
* data (string): A parancshoz tartozó opcionális argumentum vagy adatcsomag. Ha nincs adat, üres sztringként ("") kell átadni.

### Visszatérési érték
* Típus: string[] (2 elemű tömb)
* Sikeres kommunikáció esetén: Az első elem ([0]) a fogadott válasz parancskódja (pl. ACK vagy PONG), a második elem ([1]) pedig a válaszban érkezett adat string.
* Hiba esetén: Az első elem értéke CommandType.ERR ("ERR"), a második elem pedig a hiba oka vagy egy üres sztring.

### A Protokoll Formátuma
A metódus az adatokat egy speciális csomagstruktúrába ágyazza be a küldés előtt:

### Küldött csomag formátuma
```
<ID|CMD|DATA|CHK>
```
* ID: Egy automatikusan növekvő üzenetazonosító számláló (msgId++).
* CMD / DATA: A bemeneti paraméterként átadott értékek.
* CHK: Az ID|CMD|DATA karakterláncból számolt XOR ellenőrző összeg, 2 jegyű hexadecimális formátumban.

Fogadott csomag formátuma
```
<ID|ACK|DATA|CHK>
```
A fogadott üzenet felépítése megegyezik a küldöttével, azzal a kitétellel, hogy az ID-nak pontosan meg kell egyeznie a küldött csomag ID-jával a párosításhoz.

## Működési elv

1. Kapcsolat ellenőrzése: Első lépésként ellenőrzi, hogy a `serialPort` létezik-e és nyitva van-e. Ha nem, azonnal hibával tér vissza.
2. Zárolás (Thread Safety): Belép a `_serialPortLock` kritikus szakaszba, így biztosítja, hogy egyszerre csak egy szál írhasson/olvashasson a soros porton.
3. Csomag-összeállítás: Generálja az egyedi üzenet-azonosítót, kiszámítja a `CalculateChecksum()` segítségével a hibaellenőrző kódot, majd összefűzi a végleges `<...>` keretes stringet.
4. Puffer ürítés és küldés: A küldés előtt kiüríti a soros port bemeneti és kimeneti puffereit (`DiscardInBuffer`, `DiscardOutBuffer`), hogy elkerülje a korábbi üzenetekből maradt szemetet, majd kiküldi az adatot.
5. Válasz beolvasása: A `serialPort.ReadTo(">")` hívással megvárja, amíg a záró karakter megérkezik. Ha a beolvasás sikertelen vagy időtúllépés (`TimeoutException`) történik, meghívja a `HandleFailure()` metódust.
6. Validáció: * Ellenőrzi, hogy a válasz felosztható-e a `|` karakter mentén 4 részre.
    * Ellenőrzi, hogy a kapott ID egyezik-e a küldött ID-val.
    * Újraszámolja a fogadott payload checksumját és összeveti a csomag végén található értékkel.
7. Siker: Ha minden ellenőrzés sikeres, a hibaszámlálót (`faildAttempt`) lenullázza, és visszaadja a kicsomagolt adatokat.

## Példa használat
```cs
using Vascular_Pressure_Measurement_System.Utils;

// Paraméter beállító parancs küldése
string[] response = Connection.SendMessage(Connection.CommandType.SET_PARAM, "TEMP=22");

if (response[0] == Connection.CommandType.ACK)
{
    Console.WriteLine("A paraméter beállítása sikeresen megtörtént!");
}
else if (response[0] == Connection.CommandType.ERR)
{
    Console.WriteLine($"Kommunikációs hiba lépett fel: {response[1]}");
}
```

## Kivételkezelés és Megjegyzések
Hiba-akkumuláció: Ha a metódus futása során a beolvasás elakad, vagy a csomag szerkezetileg sérült, meghívódik a `HandleFailure()` privát metódus. Ez a belső számláló növelésével figyeli a hibákat: 3 egymást követő sikertelen próbálkozás után a metódus automatikusan lezárja a soros portot, és a kapcsolat állapotát megszakadtnak jelöli (`isConnected = false`).

Szinkron blokkolás: A `ReadTo` hívás blokkolja az aktuális szál futását a `SerialPort.ReadTimeout` idejéig (ami a `GetSerialPort()`-ban 1000 ms-ra van állítva). Emiatt ezt a metódust célszerű nem a fő UI (felhasználói felület) szálon, hanem háttérszálon futtatni, ha el akarjuk kerülni a program fagyását egy esetleges eszköz-lekapcsolódás esetén.

# HandleFailure()

## Áttekintés
A `HandleFailure` egy belső (privát) hibakezelő segédfüggvény, amely a soros kommunikáció során fellépő egymást követő hibákat (időtúllépések, sérült csomagok) számlálja. A metódus elsődleges feladata egy automatikus lekapcsolási (fail-safe) mechanizmus biztosítása: ha a kommunikációs hibák száma elér egy kritikus küszöbértéket, a rendszer lezárja a portot, megelőzve ezzel a szoftver elakadását vagy a hibás adatok feldolgozását.

## Szintaxis
```cs
private static void HandleFailure()
```

### Visszatérési érték
* Típus: `void`
* A metódus belső állapotváltozókat és az aktuális soros port példányt módosítja, visszatérési értéke nincs.

## Működési elv
A metódus meghívásakor az alábbi logika fut le:
1. Hibaszámláló növelése: Megnöveli a `faildAttempt` privát statikus egész változó értékét 1-gyel (`faildAttempt++`).
2. Küszöbérték ellenőrzése: Ellenőrzi, hogy a hibák száma elérte-e a 3-at (`faildAttempt >= 3`).
3. Kényszerített lekapcsolás: Amennyiben a hibák száma elérte a kritikus szintet, végrehajtja az alábbi lépéseket:
    * Lezárja a soros portot (`serialPort.Close()`).
    * Az `isConnected` állapotjelzőt `false` értékre állítja.
    * A `GlobalData.SerialConnectionStatus` globális státuszt `false` értékre állítja.
    * Alaphelyzetbe állítja (nullázza) a `faildAttempt` számlálót a jövőbeli újrapróbálkozásokhoz.

## Példa használat
A metódus közvetlenül a `SendMessage()` és a `ReadMessage()` metódusok `catch` ágából, valamint a hibás csomagstruktúra észlelésekor hívódik meg:
```cs
try
{
    response = serialPort.ReadTo(">") + ">";
}
catch
{
    // Időtúllépés vagy port hiba esetén regisztráljuk a hibát
    HandleFailure();
    return new string[] { CommandType.ERR, "" };
}
```

A számláló nullázása: Ha a `SendMessage` metódusban a teljes validációs folyamat sikeresen lefut, a `faildAttempt` értéke automatikusan visszaáll `0`-ra, így csak a folyamatos, egymást követő hibák váltják ki a lekapcsolást.

## Megjegyzések
Szálbiztonság és mellékhatások: Bár a `SendMessage` metóduson belül a `HandleFailure` hívása a `_serialPortLock` zárolás alatt történik, a `ReadMessage` metódusban való futtatása során nincs dedikált lock. Ez kritikus szakaszokhoz vezethet, ha egy háttérszál a háttérben lezárja a portot, miközben a `ContinousTest` szál épp ellenőrizné azt.

Összefüggés a `ContinousTest`-tel: Miután a `HandleFailure` lezárta a portot és a státuszokat `false`-ra állította, a háttérben futó `ContinousTest()` metódus a következő másodpercben érzékelni fogja a szakadást, és automatikusan megkísérli a kapcsolat újbóli felépítését a `GetSerialPort()` segítségével.

# ReadMessage()

## Áttekintés
A `ReadMessage` egy belső (privát) segédfüggvény, amely a soros port beérkező pufferéből olvassa be az eszköz által küldött nyers adatcsomagot, eltávolítja a protokollkeretet (a `<` és `>` karaktereket), majd elvégzi az üzenet szerkezeti és integritási (checksum) ellenőrzését.
> Ez a metódus elsősorban az aszinkron vagy folyamatos adatbeolvasások támogatására szolgál (példálul a mérési adatok folyamatos fogadásakor), kiegészítve a `SendMessage` szinkron írás-olvasási logikáját.

## Szintaxis
```cs
private static string[] ReadMessage()
```

### Visszatérési érték
* Típus: `string[]` (2 elemű tömb)
* Sikeres beolvasás és validálás esetén: Az első elem (`[0]`) a fogadott parancs vagy nyugtázás típusa (pl. `ACK`, `PONG`), a második elem (`[1]`) pedig a parancshoz tartozó adat (payload).
* Hiba esetén: Az első elem értéke `CommandType.ERR` (`"ERR"`), a második elem pedig egy üres sztring (`""`).

## Működési elv
A metódus meghívásakor az alábbi feldolgozási lépések futnak le:
1. Adatbeolvasás és keretbontás
    * A metódus megkísérli beolvasni a soros port puffertartalmát a záró karakterig a `serialPort.ReadTo(">")` függvénnyel, majd manuálisan hozzáfűzi a záró `>` karaktert a visszaolvasott stringhez.
    * Ha a beolvasás közben hiba vagy időtúllépés (`TimeoutException`) lép fel, a `catch` ágban meghívódik a `HandleFailure()` metódus, és a függvény hibával (`ERR`) tér vissza.
    * Sikeres olvasás után a string elejéről és végéről eltávolítja a protokoll által használt `<` és `>` karaktereket a `.Trim('<', '>')` metódussal.

2. Payload felbontása
A megtisztított karakterláncot a | (pipe) karakter mentén darabolja fel (.Split('|')). A protokoll szabályai szerint a kapott tömbnek pontosan 4 elemet kell tartalmaznia:
    1. `responsePayload[0]`: Üzenetazonosító (`ID`)
    2. `responsePayload[1]`: Parancskód / Üzenet típusa (`CMD` / `ACK`)
    3. `responsePayload[2]`: Adatmező (`DATA`)
    4. `responsePayload[3]`: Ellenőrző összeg hexadecimális formában (`CHK`)
    > [!WARNING] Ha a felbontott tömb hossza nem pontosan 4, a metódus ezt csomagsérülésként értékeli, növeli a hibaszámlálót a `HandleFailure()` meghívásával, és `ERR` státuszt ad vissza.
3. Checksum (Integritás) ellenőrzése
    A metódus a CalculateChecksum függvény segítségével újra előállítja az ellenőrző összeget az első három mezőből (ID|CMD|DATA), majd összehasonlítja azt a csomagban érkezett, 16-os számrendszerből bájttá alakított Convert.ToByte(responsePayload[3], 16) értékkel.
    * Ha a checksum nem egyezik: A metódus azonnal `ERR` tömbbel tér vissza (ebben az esetben nem hívja meg a `HandleFailure()`-t).
    * Ha a checksum egyezik: A csomag hiteles, és a metódus visszaadja a parancskódot és az adatot `new string[] { responsePayload[1], responsePayload[2] }` formátumban.

## Példa használat
Mivel a metódus private láthatósággal bír, csak a Connection osztályon belüli adatfogadási folyamatok tudják hasznosítani. Elvi működése megegyezik a SendMessage fogadó oldali logikájával:
```cs
// Osztályon belüli fiktív aszinkron adatfogadási példa
public static string[] ListenForData()
{
    if (serialPort != null && serialPort.IsOpen)
    {
        // Adatbeolvasás a pufferből
        string[] incomingMessage = ReadMessage();
        
        if (incomingMessage[0] != CommandType.ERR)
        {
            // Biztonságosan feldolgozható a beérkező adat
            return incomingMessage; 
        }
    }
    return new string[] { CommandType.ERR, "Olvasási hiba" };
}
```

## Megjegyzések és Kockázatok
> [!CAUTION] Szálbiztonsági hiányosság (Lock hiánya): Fontos megjegyezni, hogy a SendMessage metódussal ellentétben a ReadMessage nem használja a belső _serialPortLock objektumot a soros port elérésének zárolására. Ha ezt a metódust egy külső háttérszálból párhuzamosan hívják, miközben a SendMessage vagy a ContinousTest is aktívan kommunikál a porton, az a puffer tartalmának keveredéséhez, hibás checksumokhoz vagy a program összeomlásához vezethet.

# CommandType osztály

## Áttekintés
A `CommandType` a `Connection` osztályon belül elhelyezkedő statikus segédosztály, amely a számítógépes szoftver és a mérőeszköz (Arduino) közötti kommunikációs protokoll parancskódjait és válaszüzeneteit definiálja konstansok formájában.

Ezen központosított sztring-konstansok használata biztosítja, hogy a protokoll üzenetei típusbiztosak és könnyen karbantarthatóak legyenek a teljes alkalmazásban, minimalizálva az elgépelésből származó kommunikációs hibákat.

## Szintaxis
```cs
public static class CommandType
```

## Konstansok (Mezők)
Az osztály az alábbi `public const string` típusú mezőket tartalmazza, melyek a protokoll csomagjaiban a `CMD` vagy `ACK` szekciókban utaznak:

| Konstans neve     | Érték (String)    | Leírás / Funkció                                                                              |
| ----------------- | ----------------- | --------------------------------------------------------------------------------------------- |
| `PING`            | `"PING"`          | Kapcsolatellenőrző parancs. A PC küldi az eszköz felé.                                        |
| `PONG`            | `"PONG"`          | A PING parancsra érkező elvárt válasz az eszköz részéről.                                     |
| `SET_PARAM`       | `"SET_PARAM"`     | Konfigurációs paraméterek (pl. küszöbértékek) beállítására szolgáló parancs.                  |
| `GET_PARAM`       | `"GET_PARAM"`     | Az eszközön tárolt aktuális paraméterek lekérdezésére szolgáló parancs.                       |
| `START_MEASURE`   | `"START_MEASURE"` | A fizikai mérési folyamat és az adatküldés elindítását kérő parancs.                          |
| `STOP_MEASURE`    | `"STOP_MEASURE"`  | A mérés leállítását és a soros porti adatfolyam megszakítását kérő parancs.                   |
| `ACK`             | `"ACK"`           | Általános nyugtázó üzenet az eszköz részéről sikeres parancsvégrehajtás esetén (Acknowledge). |
| `ERR`             | `"ERR"`           | Belső szoftveres vagy kommunikációs hiba jelzésére szolgáló státuszkód.                       |


## Protokoll Kontextus
A konstansok közvetlenül a keretezett csomagstruktúrába épülnek be küldés előtt:
```
Küldésnél (CMD):  <ID|CommandType.SET_PARAM|DATA|CHK>
Fogadásnál (ACK): <ID|CommandType.ACK|DATA|CHK>
```

## Példa használat
Az alábbi példák bemutatják, hogyan kell használni a konstansokat parancsküldésnél, illetve a beérkező válaszok kiértékelésénél:
1. Parancs küldése
```cs
using Vascular_Pressure_Measurement_System.Utils;

// A mérés elindítása a konstans használatával
string[] response = Connection.SendMessage(Connection.CommandType.START_MEASURE, "");
```

2. Válasz validálása
```cs
using Vascular_Pressure_Measurement_System.Utils;

string[] response = Connection.SendMessage(Connection.CommandType.PING, "");

// A válasz típusának ellenőrzése
if (response[0] == Connection.CommandType.PONG)
{
    Console.WriteLine("Az eszköz aktív és válaszol!");
}
else if (response[0] == Connection.CommandType.ERR)
{
    Console.WriteLine("Kommunikációs hiba történt a PING során.");
}
```

# Megjegyzések
> [!TIP]
> Karbantarthatóság: Ha a mikrokontroller (Arduino) firmware-ében megváltozik egy parancsszó (például a "PING" helyett "HELO" lesz), a PC oldali forráskódban kizárólag ebben az osztályban kell módosítani a konstans értékét, a program többi része változatlanul működni fog.

> [!IMPORTANT]
> Az ERR speciális szerepe: Az ERR sztringet az eszköz is visszaadhatja hiba esetén, de a osztály belső logikája (pl. a vagy ) is ezt az értéket teszi a visszatérési tömb [0] indexére, ha timeout vagy checksum hiba miatt meghiúsul az adatcsere.