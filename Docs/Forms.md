# További ablakok: `SettingsForm`, `Diagnostics`, `AnalysisForm`, `AboutForm`

Ez a fájl a `Vascular_Pressure_Measurement_System.Forms` névtér négy "kiegészítő" ablakát
dokumentálja (a `Main` főablakot lásd külön: [`03-Main-Foablak.md`](./03-Main-Foablak.md)).

⬅ Vissza az [áttekintéshez](./00-README.md) · Előző: [Main főablak](./03-Main-Foablak.md)

> **Közös mintázat mind a négy ablakban:** mindegyik `KeyPreview = true`-t állít be a
> konstruktorban, nyomon követi a `Ctrl` billentyű állapotát egy `isCtrlPressed` mezővel
> (bár ezt magát funkcionálisan nem használják fel máshol), és az `Escape` billentyűre az
> ablak bezárásával (`this.Close()`) reagál. Ezt a lentiekben ablakonként csak röviden
> említjük, hogy ne ismétlődjön feleslegesen.

---

## 1. `SettingsForm` (`SettingsForm.cs`)

A hardver mérési paramétereinek (`MIN_DELTA`, `FALL_THRESHOLD`) szerkesztő ablaka. A
`Main.SettingsButton_Click` nyitja meg (nem modálisan, de a főablak közben letiltásra
kerül).

### Felület

| Vezérlő | Szerep |
|---|---|
| `parameterBox` | `GroupBox`, amely a paraméter-mezőket és az újrainicializáló gombot tartalmazza |
| `parameterMinDelta` | Szövegmező – a "Fall Delta" (`MIN_DELTA`) paraméter |
| `parameterFallTreshold` | Szövegmező – a "Falling Treshold" (`FALL_THRESHOLD`) paraméter |
| `ReinitButton` | "Start" feliratú gomb – a Designer-ben létre van hozva, de **nincs `Click` eseménykezelője** a mellékelt kódban (jelenleg funkció nélküli UI elem) |
| `SaveButton` | Mentés és bezárás |
| `CancelButton` | Mégse (megerősítéssel) |

### `public SettingsForm()`

1. `InitializeComponent()`, `KeyPreview = true`.
2. Ha van eszközkapcsolat (`Connection.isConnected`), beolvassa a mentett konfigurációt
   (`Configuration.Hardware.ReadConfiguration()`), és feltölti vele a két szövegmezőt.
3. Ha **nincs** kapcsolat, letiltja az egész `parameterBox` csoportot (nem lehet
   paramétert szerkeszteni eszköz nélkül).

### `private void CancelButton_Click(object sender, EventArgs e)`

Megerősítő `MessageBox`-ot mutat ("Changes will not be saved!", OK/Cancel). Ha a
felhasználó `OK`-t nyom, bezárja az ablakot (`CloseForm()`); ha `Cancel`-t, semmi nem
történik, az ablak nyitva marad.

### `private void SaveButton_Click(object sender, EventArgs e)`

1. Megpróbálja `int`-té alakítani mindkét mezőt (`int.TryParse`), sikertelen konverzió
   esetén `-1` marad az érték.
2. **Ha a paraméter-szerkesztés engedélyezve volt** (`parameterBox.Enabled`) és bármelyik
   érték `<= 0`, hibaüzenetet mutat és megszakítja a mentést (a felhasználónak pozitív
   egész számokat kell megadnia).
3. Összeállítja a `Dictionary<string,string>` konfigurációt a két értékkel.
4. Ha a szerkesztés engedélyezve volt, meghívja a
   `Configuration.Hardware.SetParameters(configs)`-t (ez egyszerre írja fájlba **és**
   küldi ki az eszközre – lásd [`02-Configuration-Hardver-Beallitasok.md`](./02-Configuration-Hardver-Beallitasok.md)).
5. Bezárja az ablakot.

> Megjegyzés: ha nem volt eszközkapcsolat (tehát `parameterBox.Enabled == false`), a
> mentés gomb egyszerűen csak bezárja az ablakot ténykedés nélkül, hiszen a mezők tartalma
> ilyenkor irreleváns.

### `private void CloseForm()`

Egysoros segédfüggvény: `this.Close()`.

### Billentyűzet

`Settings_KeyDown`/`Settings_KeyUp` – a fent leírt közös minta (`Ctrl` követés,
`Escape` → bezárás).

---

## 2. `Diagnostics` (`Diagnostics.cs` / `Diagnostics_Designer.cs`)

Fejlesztői/szervizmód ablak az eszköz **digitális és analóg I/O lábjainak** élő
figyelésére és (kimeneti láb esetén) vezérlésére. Csak `Ctrl+Shift+D`-vel érhető el a
`Main`-ből, és csak akkor, ha nem fut mérés, illetve még nincs nyitva másik példány.

### Mezők

| Mező | Jelentés |
|---|---|
| `mainForm` | Referencia a `Main` ablakra (bezáráskor a `testPad` mező nullázásához) |
| `BOARD_TYPE` | A csatlakoztatott eszköz típusneve (az eszköztől lekérdezve) |
| `DIGITAL_PINS` / `ANALOG_PINS` | A digitális / analóg lábak száma az eszközön |
| `closeThreads` | Jelzőzászló a háttérszálak (poller-ek) leállítására |

### `public Diagnostics(Main mainForm)`

Konstruktor: elmenti a főablak referenciáját, `InitializeComponent()`,
`KeyPreview = true`.

### `private void Diagnostics_Load(object sender, EventArgs e)`

Az ablak megjelenésekor: ha van kapcsolat, azonnal elindítja a `BoardInitialize()`-t;
mindenképp feliratkozik a `GlobalData.SerialConnectionStatusChanged` eseményre
(`ConnectionChanged`), hogy a kapcsolat közbeni elvesztésére/visszatérésére is tudjon
reagálni.

### `private void ConnectionChanged(object sender, EventArgs e)`

A kapcsolat-állapot változásakor fut le: `closeThreads = !Connection.isConnected`
(kapcsolat esetén `false`, hiány esetén `true`), majd újra meghívja a
`BoardInitialize()`-t, amely a `closeThreads` értéke alapján dönt arról, hogy
újragenerálja-e a lábakat leíró vezérlőket, vagy törölje azokat.

### `private void BoardInitialize()`

1. Elküldi a `GET_BOARD_DATAS` parancsot, a választ `;` mentén 3 részre bontja
   (board típus, digitális lábszám, analóg lábszám).
2. Ha az ablak handle-je még nincs létrehozva, vagy az ablak már el van dobva (`Disposed`),
   kilép (elkerülve a kivételt egy nem létező vezérlőre való `Invoke`-nál).
3. `Invoke`-on keresztül UI-frissítés:
   - **ha nincs `closeThreads`** (van kapcsolat): beállítja a `BOARD_TYPE`,
     `DIGITAL_PINS`, `ANALOG_PINS` mezőket a válaszból, frissíti az ablak címét
     (`"{BOARD_TYPE} connected"`), és meghívja a `GenerateMembers()`-t;
   - **egyébként** (nincs kapcsolat): visszaállítja az alapállapotot (`"Disconnected"`
     cím, nullázott mezők), és meghívja a `DeleteAllMembers()`-t.

### `private void Diagnostics_FormClosing(object sender, FormClosingEventArgs e)`

Bezáráskor `closeThreads = true` (a poller szálak leállnak), és `mainForm.testPad = null`
(hogy a `Main` tudja: az ablak bezárult, újra megnyitható a diagnosztika).

### `private void GenerateMembers()`

**Dinamikusan generálja** a lábakhoz tartozó vezérlőket, a lekérdezett `ANALOG_PINS` és
`DIGITAL_PINS` szám alapján:

- **Analóg lábankénti (0-tól `ANALOG_PINS`-ig):** egy `Label` (`"Analog IN {i}"` felirat)
  és egy csak-olvasható jellegű `TextBox` (`Name = "A{i}"`), amely az aktuális mért
  értéket mutatja majd.
- **Digitális lábankénti (0-tól `DIGITAL_PINS`-ig):** egy `Label` (`Name = "D{i}L"`,
  kezdetben `"???"` felirattal – ez fogja mutatni, hogy a láb `INPUT` vagy `OUTPUT` módú),
  és egy `Button` (`Name = "D{i}"`), amelynek `Click` eseménye a
  `DigitalButtonClicked`-hez van kötve – ez mutatja az aktuális értéket **és** ezzel
  lehet kimeneti lábat kapcsolgatni.

A vezérlők egymás alá kerülnek, 28 pixeles léptékkel (`gap`).

Ezután elindít egy **külön szálat**, amely (`Invoke`-on belül) minden digitális lábra
lekérdezi a módját (`GET_PIN_MODE`), és frissíti a hozzá tartozó `"D{i}L"` label szövegét
(`"Digital {mode} {i}"` formában) – ez párhuzamosan fut a lentebb induló folyamatos
lekérdezéssel.

Végül meghívja a `RefreshMembers()`-t, amely elindítja a folyamatos I/O-lekérdező szálat.

### `private void RefreshMembers()`

Elindít egy háttérszálat, amely **amíg a kapcsolat él és `!closeThreads`**, folyamatosan
körbejárja az összes analóg és digitális lábat:

- minden analóg lábra `GET_IO` lekérdezés → `Invoke`-on keresztül frissíti a megfelelő
  `"A{i}"` nevű `TextBox` szövegét;
- minden digitális lábra `GET_IO` lekérdezés → `Invoke`-on keresztül frissíti a megfelelő
  `"D{i}"` nevű `Button` szövegét (tehát ugyanaz a gomb mutatja az értéket **és**
  kattintható a vezérléshez).

A ciklus minden teljes körbejárás után `Thread.Sleep(10)`-et alszik. Bármilyen kivételt
csendben elnyel (`catch { }`), hogy pl. az ablak bezárása közbeni átmeneti hibák ne
okozzanak kezeletlen kivételt a háttérszálon.

### `private void DeleteAllMembers()`

`closeThreads = true`, majd törli az ablak összes dinamikusan hozzáadott vezérlőjét
(`Controls.Clear()`) – kapcsolat-vesztéskor hívódik.

### `private void DigitalButtonClicked(object sender, EventArgs e)`

Egy digitális láb gombjára kattintva:
1. Lekérdezi a láb aktuális módját (`GET_PIN_MODE`).
2. **Csak ha a mód `"OUTPUT"`**, akkor:
   - lekéri a láb jelenlegi értékét (`GET_IO`);
   - ha az `"1"`, az új érték `"LOW"`, egyébként `"HIGH"` (tehát a jelenlegi bináris
     értéket "megfordítja", és a beállításnál szöveges `LOW`/`HIGH` formát küld);
   - elküldi a `SET_IO` parancsot `"{pinName};{newValue}"` adattal.

Bemeneti (`INPUT`) lábon kattintásra nem történik semmi (a gomb ilyenkor csak
kijelzésre szolgál).

### Billentyűzet

`Diagnostics_KeyDown`/`Diagnostics_KeyUp` – a közös minta (`Escape` → bezárás).

---

## 3. `AnalysisForm` (`AnalysisForm.cs` / `AnalysisForm_Designer.cs`)

A `Main.Chart_MouseUp` által a kijelölt görbeszakaszra számolt **numerikus derivált**
megjelenítő ablaka: grafikon + statisztikák.

### Felület

| Vezérlő | Szerep |
|---|---|
| `chartDerivative` | `Chart`, két szériával: `Original` (a kijelölt eredeti pontok) és `Derivated` (a számolt meredekség-görbe), spline vonaltípussal |
| `dxAvg` | Az átlagos derivált értéke |
| `dxRise` | A legnagyobb (pozitív) meredekség és annak X-helye |
| `dxFall` | A legkisebb (legnegatívabb) meredekség és annak X-helye |
| `dxEvaluated` | A kiértékelt szakaszok (pontpárok) száma |

> A `label2` UI-felirata a Designer fájlban `"Avarage Derivative"` – elgépelés
> ("Average" helyett), de ez pusztán a megjelenített szöveget érinti.

### `public AnalysisForm(List<DataPoint> originalPoints, List<double> derivativeX, List<double> derivatives)`

1. `InitializeComponent()`, `KeyPreview = true`.
2. Beállítja a `chartDerivative` pozícióját/méretét a kliensterület alapján (200 px-től
   indul, hogy helyet hagyjon a statisztika-céduláknak).
3. `AxisY.IsStartedFromZero = false` – az Y tengely nem kényszerül nullától indulni, így a
   görbe alakja jobban kirajzolódik (szorosabb skálázás).
4. Feltölti az `"Original"` szériát az `originalPoints` (a felhasználó által kijelölt
   eredeti mérési pontok) alapján.
5. Feltölti a `"Derivated"` szériát a `derivativeX`/`derivatives` párokból (a
   `Main.Chart_MouseUp`-ban kiszámolt meredekség-értékek).
6. **Ha van legalább egy derivált érték:**
   - `avgDerivative = derivatives.Average()`;
   - `maxSlope = derivatives.Max()`, és megkeresi ennek X-helyét (`derivativeX[maxIndex]`);
   - `minSlope = derivatives.Min()`, és megkeresi ennek X-helyét;
   - a négy szövegmezőt formázva feltölti: az átlag és a szélsőértékek 4 tizedesjegyre,
     az X-pozíciók 2 tizedesjegyre, a kiértékelt szakaszok száma pedig a `derivatives`
     lista elemszáma.

### `private void Derivate_SizeChanged(object sender, EventArgs e)`

Az ablak átméretezésekor újraszámolja a `chartDerivative` pozícióját/méretét (ugyanaz a
logika, mint a konstruktorban).

### Billentyűzet

`Analysis_KeyDown`/`Analysis_KeyUp` – a közös minta (`Escape` → bezárás). *(Megjegyzés: a
Designer-ben a `KeyUp` esemény véletlenül a `Analysis_KeyDown` metódushoz van kötve, nem
egy külön `Analysis_KeyUp`-hoz – funkcionálisan ez azt jelenti, hogy billentyű felengedésekor
is lefut a "lenyomás" logika, ami itt csak az `isCtrlPressed = true` beállítást és az
`Escape`-re való bezárást jelenti, tehát gyakorlati hatása minimális.)*

---

## 4. `AboutForm` (`AboutForm.cs` / `AboutForm_Designer.cs`)

Egyszerű, statikus információs ablak – verzió és kiadási dátum megjelenítése, logóval.

### Felület

| Vezérlő | Szerep |
|---|---|
| `pictureBox1` | Az alkalmazás logója |
| `version` | Verziószám (dinamikusan generálva) |
| `releaseDate` | Kiadási dátum (dinamikusan generálva) |
| `label4` | "Made by Ádám Szabó" – szerző felirat |

Az ablak mérete rögzített (`MinimumSize == MaximumSize`), tehát nem átméretezhető.

### `public AboutForm()`

1. `InitializeComponent()`, `KeyPreview = true`.
2. Lekéri a futó `.exe` (assembly) fájl elérési útját
   (`Assembly.GetExecutingAssembly().Location`); ha ez üres (pl. bizonyos
   publikálási/futtatási módoknál előfordulhat), tartalék megoldásként az
   `AppContext.BaseDirectory`-t használja.
3. Lekéri az assembly fájl **utolsó módosítási időpontját**
   (`File.GetLastWriteTime`) – ez adja a "verziószámot": az alkalmazás nem tárol külön
   verziószámot, hanem a lefordított `.exe` fájl dátumából generálja azt.
4. Beállítja:
   - `version.Text = "v1.{yyMMdd}"` (pl. `v1.260626`);
   - `releaseDate.Text = "{Év}. {Hónap:00}. {Nap:00}."`.

### Billentyűzet

`About_KeyDown`/`About_KeyUp` – a közös minta (`Escape` → bezárás).

---

⬅ Vissza az [áttekintéshez](./00-README.md) · Előző: [Main főablak](./03-Main-Foablak.md)
