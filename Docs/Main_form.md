# `Main` – A főablak (`Main.cs` / `Main_Designer.cs`)

A `Main` a program **belépési ablaka**: itt fut a valós idejű nyomásgörbe, innen indítható
és állítható le a mérés, itt lehet menteni/betölteni CSV-t, és innen érhető el az összes
többi ablak (Beállítások, About, illetve a rejtett Diagnosztika).

⬅ Vissza az [áttekintéshez](./00-README.md) · Előző: [Configuration névtér](./02-Configuration-Hardver-Beallitasok.md)

---

## 1. Felület felépítése (a Designer alapján)

| Vezérlő | Típus | Szerep |
|---|---|---|
| `Chart` | `Chart` (Windows Forms DataVisualization) | A mért nyomásgörbe megjelenítése; `FastLine` széria típus, X = idő (s), Y = nyomás (hgmm); egyedi paletta (sárga `#F5B418` és sötétzöld `#155744`); egérrel kijelölhető tartomány (kurzor engedélyezve X és Y tengelyen is) |
| `StartMeasuring` | `Button` | Mérés indítása |
| `ForceStopButton` | `Button` | Mérés azonnali leállítása |
| `SettingsButton` | `Button` | Beállítások ablak megnyitása |
| `SaveButton` | `Button` | Aktuális görbe mentése CSV-be |
| `LoadButton` | `Button` | Korábbi CSV betöltése a görbébe |
| `AboutButton` | `Button` | About ablak megnyitása |

A gombok bal oldalon, egymás alatt helyezkednek el; a `Chart` a fennmaradó helyet tölti ki
(dinamikusan méreteződik az ablak átméretezésekor).

## 2. Mezők

| Mező | Típus | Jelentés |
|---|---|---|
| `testPad` | `Diagnostics` (publikus) | A nyitott Diagnosztika ablak referenciája, vagy `null` |
| `settingsForm` | `SettingsForm` | A nyitott Beállítások ablak referenciája, vagy `null` |
| `measure` | `Measure` | A mérési logikát megvalósító objektum |
| `counter` | `int` | A grafikonon megjelenített minták sorszáma (az X-tengely idő-értékének kiszámításához: `counter * 0.01` s) |
| `chartUpdateTimer` | `System.Windows.Forms.Timer` | 10 ms-onként frissíti a grafikont a mérés közben beérkezett adatokból |
| `connectionFaildOnDisplayed` | `bool` | Megakadályozza, hogy több "nincs kapcsolat" hibaüzenet nyíljon meg egyszerre |

## 3. Konstruktor – `public Main()`

1. `InitializeComponent()` – Designer által generált UI felépítése.
2. `Connection.ContinousTest()` – elindítja a **folyamatos kapcsolat-figyelő háttérszálat**
   (lásd [`01-Utils-Kommunikacio-Meres.md`](./01-Utils-Kommunikacio-Meres.md)).
3. Feliratkozik a `GlobalData.SerialConnectionStatusChanged` eseményre
   (`GlobalData_SerialConnectionStatus` kezelő).
4. Létrehozza a `Measure` példányt (`this`-t adva át, hogy a mérés vissza tudjon írni az
   ablakra), illetve a `chartUpdateTimer`-t.
5. Induláskor letiltja a `ForceStopButton`, `StartMeasuring`, `SaveButton` gombokat (nincs
   még kapcsolat, nincs mérés).
6. `KeyPreview = true` – az ablak minden billentyűleütést megkap, mielőtt a fókuszban lévő
   vezérlőhöz eljutna (ez teszi lehetővé a globális billentyűparancsokat).

## 4. UI segédmetódusok

### `public void EnableButton(Button button)` / `public void DisableButton(Button button)`

Egyszerű, ismétlődő minta kiváltására szolgáló segédfüggvények: a gomb `Enabled`
tulajdonságát és háttérszínét (`WhiteSmoke` engedélyezve / `Gray` letiltva) állítják be
egyszerre, konzisztens vizuális visszajelzést adva a gomb állapotáról.

### `private void Main_Load(object sender, EventArgs e)`

Az ablak első megjelenésekor fut le:
- beállítja a `Chart` pozícióját és méretét a kliensterület alapján;
- meghívja a `RefreshButtonPosition()`-t (a jobb oldali gombsor pozicionálása);
- a `Chart` széria típusát `FastLine`-ra állítja (teljesítmény-optimalizált vonalrajzolás
  nagy pontszámhoz);
- az X tengely minimumát/maximumát `NaN`-ra állítja, ami az automatikus skálázást
  engedélyezi;
- letiltja a `ForceStopButton` és `StartMeasuring` gombokat.

### `private void Main_SizeChanged(object sender, EventArgs e)`

Az ablak átméretezésekor újraszámolja a `Chart` méretét (a gombsor szélességét levonva),
és újrapozícionálja a gombokat. Védekezik negatív méret ellen (ha az ablakot nagyon kicsire
húzzák, a metódus egyszerűen nem csinál semmit).

### `private void RefreshButtonPosition()`

Kiszámolja az `AboutButton`, `SettingsButton`, `LoadButton`, `SaveButton` függőleges
pozícióját **alulról felfelé haladva**, egymás tetejére "pakolva" őket, fix (10–20 px-es)
résekkel. Ez biztosítja, hogy az ablak átméretezésekor a gombok mindig az alsó szélhez
igazodva, egymás fölött maradjanak.

### `private void Main_FormClosing(object sender, FormClosingEventArgs e)`

Az ablak bezárásakor:
- `Connection.stopConnection = true` – leállítja a kapcsolat-figyelő háttérszálat;
- `measure.Stop(true)` – leállítja az esetlegesen futó mérést, **`isFormClosing = true`**
  jelzéssel, hogy a `Measure` szála a leállás után **ne** próbáljon egy már bezáródó
  ablakra `Invoke`-olni (ami kivételt dobna).

## 5. Kapcsolat-állapot kezelése

### `private void GlobalData_SerialConnectionStatus(object sender, EventArgs e)`

Ez a metódus reagál a `GlobalData.SerialConnectionStatusChanged` eseményre – tehát
**bármikor lefut**, amikor a soros kapcsolat állapota megváltozik (akár a `ContinousTest`,
akár a `Measure` hibaágán keresztül).

- **Ha csatlakozva** (`Connection.isConnected == true`):
  - `Invoke`-on keresztül beállítja az ablak címét `"VRP - Device connected"`-re, és
    engedélyezi a `StartMeasuring` gombot.
- **Ha nincs csatlakozva:**
  - `Invoke`-on keresztül `"VRP - Device disconnected"` címet állít be, letiltja a
    `ForceStopButton` és `StartMeasuring` gombokat; ha nyitva van a `SettingsForm`, bezárja
    azt, és újra engedélyezi a főablakot (`this.Enabled = true`);
  - **külön szálon** egy `MessageBox`-ot jelenít meg ("No connection to the device!"),
    de csak akkor, ha még nincs ilyen felugró ablak megjelenítve
    (`connectionFaildOnDisplayed` őrzi ezt, hogy ismételt lecsatlakozás/kapcsolódás
    esetén ne halmozódjanak fel a hibaablakok).

## 6. Grafikon frissítése mérés közben

### `private void ContinousChartUpdate(object sender, EventArgs e)`

A `chartUpdateTimer` `Tick` eseménykezelője, 10 ms-onként fut, amíg a timer aktív.

1. Ha a mérés már nem fut (`!measure.isRunning()`) → leállítja a timert (`chartUpdateTimer.Stop()`)
   és visszatér (nincs több feldolgoznivaló).
2. Egyébként `lock(measure._lock)` védelemben kiüríti a `measure.Buffer`-t:
   - minden elemet stringgé alakít, megpróbálja `double`-lé konvertálni;
   - siker esetén hozzáadja a `Chart.Series[0]`-hoz `(counter * 0.01, result)` koordinátával
     (tehát minden minta 0,01 másodpercnyi időt reprezentál – 100 Hz mintavételi
     feltételezés), majd növeli a `counter`-t.

Ez a metódus tehát **UI-szálon** fut (mivel timer eseményként hívódik), és ez az egyetlen
hely, ahol a `Chart` ténylegesen bővül mérés közben – a `Measure` szála maga sosem nyúl a
UI-hoz, csak a pufferbe ír.

## 7. Gombok eseménykezelői

### `private void StartMeasuring_Click(object sender, EventArgs e)`

1. Ha már fut mérés, azonnal visszatér (védelem a duplikált indítás ellen).
2. Ha nyitva van a Diagnosztika ablak (`testPad != null`), bezárja azt (a diagnosztika és a
   mérés nem futhat egyszerre, mivel mindkettő a soros portot használná ütközően).
3. Törli a `Chart` korábbi pontjait (`Chart.Series[0].Points.Clear()`).
4. Gombállapotok frissítése: `ForceStopButton` engedélyezve, `StartMeasuring`,
   `SaveButton`, `SettingsButton` letiltva (amíg a mérés fut, ezek nem elérhetők).
5. Beállítja és elindítja a `chartUpdateTimer`-t (10 ms intervallum, `ContinousChartUpdate`
   eseménykezelő hozzáadva).
6. `measure.Start()` – elindítja a tényleges mérést (lásd
   [`01-Utils-Kommunikacio-Meres.md`](./01-Utils-Kommunikacio-Meres.md)).

### `private void SettingsButton_Click(object sender, EventArgs e)`

Létrehoz egy új `SettingsForm`-ot, letiltja a főablakot (`this.Enabled = false` – **modális
jellegű** viselkedés, bár a form maga nem `ShowDialog()`-gal, hanem `Show()`-val nyílik),
feliratkozik a `FormClosed` eseményre (ami visszaállítja `settingsForm = null` és
`this.Enabled = true`), majd megjeleníti az ablakot.

### `private void ForceStopButton_Click(object sender, EventArgs e)`

Azonnal leállítja a mérést (`measure.Stop()`), letiltja a `ForceStopButton`-t és
engedélyezi a `StartMeasuring` gombot. (A többi gomb – `SaveButton`, `SettingsButton` –
visszaállítását maga a `Measure.Start()` szála végzi a mérési szál leállása *után*,
`Invoke`-on keresztül.)

### `private void SaveButton_Click(object sender, EventArgs e)`

1. Felépít egy CSV-tartalmat `"timestamp;measured\n"` fejléccel, majd soronként a
   `Chart.Series[0].Points` minden pontjának X (idő) és Y (nyomás) értékét
   `;`-vel elválasztva.
2. `SaveFileDialog`-ot nyit (`.csv` szűrővel, alapértelmezett fájlnév a mai dátum
   `yyMMdd` formátumban).
3. Ha a felhasználó megerősíti, `File.WriteAllText`-tel kiírja a fájlt.

### `private void LoadButton_Click(object sender, EventArgs e)`

1. `OpenFileDialog`-ot nyit (`.csv` szűrővel).
2. Ha a felhasználó `Cancel`-t nyom, egyszerűen visszatér.
3. Ha OK, beolvassa a fájl minden sorát (`File.ReadAllLines`).
4. Ha a beolvasott adat `null` vagy kevesebb, mint 2 sor (nincs érdemi tartalom a fejlécen
   kívül), figyelmeztető `MessageBox`-ot mutat és visszatér.
5. Letiltja a `SaveButton`-t (a betöltött adat még nincs "sajátként" elmentve).
6. Törli a `Chart` jelenlegi pontjait, majd az 1. sortól (a fejlécet kihagyva) minden sort
   `;` mentén feldolgoz: ha mindkét oszlop `double`-re konvertálható, hozzáadja a ponthoz.
   A hibás/hiányos sorokat egyszerűen átugorja (`continue`).

## 8. Szakasz-kijelölés és derivált-elemzés

### `private void Chart_MouseUp(object sender, MouseEventArgs e)`

Ez a metódus valósítja meg a **"jelöld ki a görbe egy szakaszát, és elemezze a program"**
funkciót.

1. Csak bal egérgombra reagál.
2. Kiolvassa a `Chart` kurzorral kijelölt X-tartományt (`CursorX.SelectionStart/End`); ha a
   kezdő és végpont megegyezik (nincs valódi kijelölés), visszatér.
3. Meghatározza a `minX`/`maxX` határokat, majd összegyűjti a `Chart.Series[0].Points`
   közül azokat a pontokat, amelyek X értéke ebbe a tartományba esik
   (`selectedPoints` lista).
4. Ha kevesebb, mint 2 pont van kijelölve, visszatér (nem lehet deriváltat számolni).
5. **Numerikus derivált (szomszédos pontok közti meredekség) számítása:** minden
   `i`, `i+1` szomszédos pontpárra
   `derivatives[i] = (y2 - y1) / (x2 - x1)`, a hozzá tartozó X-koordináta pedig a két pont
   középpontja: `derivativeX[i] = (x1 + x2) / 2`. A nulla `dx` (azonos X) eseteket
   kihagyja, hogy elkerülje a nullával osztást.
6. Létrehoz egy `AnalysisForm`-ot a kijelölt pontokkal és a kiszámolt deriváltakkal, és
   megjeleníti (`Show()` – nem modális).

Az `AnalysisForm` maga végzi a statisztikák (átlag, max/min meredekség és helyük)
kiszámítását – lásd [`04-Tovabbi-Formok.md`](./04-Tovabbi-Formok.md).

## 9. About ablak

### `private void AboutButton_Click(object sender, EventArgs e)`

Létrehoz és **modálisan** (`ShowDialog()`) megjelenít egy `AboutForm`-ot.

## 10. Billentyűparancsok

### `private void Main_KeyDown(object sender, KeyEventArgs e)`

Nyomon követi a `Ctrl` és `Shift` billentyűk állapotát (`isCtrlPressed`,
`isShiftPressed`), majd a következő kombinációkra reagál:

| Kombináció | Feltétel | Hatás |
|---|---|---|
| `Ctrl+S` | `SaveButton.Enabled` | Meghívja a `SaveButton_Click`-et |
| `Ctrl+,` (`Oemcomma`) | `SettingsButton.Enabled` | Meghívja a `SettingsButton_Click`-et |
| `F1` | mindig | Meghívja az `AboutButton_Click`-et |
| `Ctrl+Shift+D` | `testPad == null && !measure.isRunning()` | Létrehozza és megjeleníti a `Diagnostics` ablakot (rejtett fejlesztői/szerviz mód) |

Minden kombináció kezelése után visszaállítja az `isCtrlPressed`/`isShiftPressed`
zászlókat, hogy elkerülje az esetleges "beragadt" billentyű-állapotot.

### `private void Main_KeyUp(object sender, KeyEventArgs e)`

Amikor a `Ctrl` billentyűt felengedik, `isCtrlPressed = false`.

---

⬅ Vissza az [áttekintéshez](./00-README.md) · Előző: [Configuration névtér](./02-Configuration-Hardver-Beallitasok.md) · Következő: [További formok](./04-Tovabbi-Formok.md)
