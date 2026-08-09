# `Configuration` névtér – Hardverparaméterek kezelése

Ez a fájl a `Vascular_Pressure_Measurement_System.Configuration` névtér két osztályát
dokumentálja: `Hardware` és `Application`.

⬅ Vissza az [áttekintéshez](https://github.com/Snewkovits/Vascular-Pressure-Measurement-System/blob/master/README.md) · Előző: [Utils névtér](https://github.com/Snewkovits/Vascular-Pressure-Measurement-System/blob/master/Docs/Utils.md)

---

## 1. `Hardware` (`Hardware.cs`)

A mérési hardver **két konfigurációs paraméterét** (`MIN_DELTA`, `FALL_THRESHOLD`) kezeli.
A paraméterek **két helyen** élnek egyszerre:

1. egy helyi CSV-szerű fájlban (`%LocalAppData%\Vascular_Pressure_Measurement_System\hardware_configuration.csv`),
2. magán az eszközön (a soros protokoll `SET_PARAM` parancsával feltöltve).

### Statikus mezők

| Mező | Jelentés |
|---|---|
| `localPath` | `%LocalAppData%` mappa útvonala |
| `appFolder` | `...\Vascular_Pressure_Measurement_System` almappa |
| `configureFile` | `...\hardware_configuration.csv` – a konfigurációs fájl teljes elérési útja |

### `public static void SetParameters(Dictionary<string,string> configs = null, bool writeDefault = false)`

**Ez a metódus három különböző forgatókönyvet kezel egyetlen belépési ponton keresztül:**

```
                    ┌──────────────────────────────┐
                    │ configs == null?             │
                    │  → configs = üres Dictionary │
                    └───────────────┬──────────────┘
                                    ▼
    ┌───────────────────────────────────────────────────────┐
    │ Nincs még mappa/fájl VAGY writeDefault == true?       │
    │  → alapértelmezett értékek beállítása és              │
    │    kiírása FÁJLBA ÉS ESZKÖZRE                         │
    └───────────────────────────────┬───────────────────────┘
                                    ▼ (ha nem)
    ┌───────────────────────────────────────────────────────┐
    │ configs üres (0 elem)?                                │
    │  → a meglévő fájlból beolvasott konfiguráció          │
    │    kiküldése CSAK AZ ESZKÖZRE (fájl nem íródik újra)  │
    └───────────────────────────────┬───────────────────────┘
                                    ▼ (ha nem)
    ┌───────────────────────────────────────────────────────┐
    │ Normál eset: a kapott configs kiírása FÁJLBA ÉS       │
    │ ESZKÖZRE IS                                           │
    └───────────────────────────────────────────────────────┘
```

**Használati módok a projektben:**

| Hívó | Paraméterek | Cél |
|---|---|---|
| `Connection.ContinousTest()` | *(nincs paraméter)* | Frissen csatlakozott/újracsatlakozott eszközre feltölti a **helyben mentett** konfigurációt (vagy ha nincs mentett konfiguráció, létrehozza az alapértelmezettet és mindkét helyre kiírja) |
| `SettingsForm.SaveButton_Click` | felhasználó által megadott `configs` | A felhasználó által módosított értékek mentése **fájlba és eszközre egyaránt** |
| `Hardware.ReadConfiguration()` (belülről) | `(null, true)` | Ha a fájlban érvénytelen/hiányzó adat van, alapértelmezett értékek kényszerített visszaállítása |

**Alapértelmezett értékek:** `MIN_DELTA = "2"`, `FALL_THRESHOLD = "3"`.

### `public static void WriteConfiguration(Dictionary<string,string> configs, string writeType)`

**Mit csinál:** a kapott kulcs-érték párokat kiírja a `writeType` paraméter szerint fájlba
és/vagy az eszközre.

**Működés:**
1. Végigmegy a `configs` szótár minden elemén:
   - felépíti a `"KULCS;ÉRTÉK"` sort, és hozzáfűzi egy összesített stringhez (`\n`
     elválasztással);
   - ha a `writeType` tartalmazza a `"DEVICE"` szót, azonnal elküldi az adott
     kulcs-érték párt a `Connection.SendMessage(SET_PARAM, "KULCS;ÉRTÉK")` hívással
     (tehát **soronként, egyenként** küldi ki az eszközre, nem egy összesített üzenetben).
2. Ha a `writeType` tartalmazza a `"FILE"` szót **és** van legalább egy konfigurációs elem,
   az összesített stringet **egy az egyben felülírja** a `configureFile` fájlban
   (`StreamWriter` felülírással, nem hozzáfűzéssel).

**A `writeType` string-alapú logika:** a `WriteType` beágyazott osztály három konstanst
definiál (lásd lentebb) – ezek `.Contains(...)` vizsgálattal döntik el, hogy az adott hívás
fájlba, eszközre, vagy mindkettőre írjon-e.

### `public static Dictionary<string, string> ReadConfiguration()`

**Mit csinál:** beolvassa és validálja a helyi konfigurációs fájlt.

**Működés:**
1. Ha a fájl nem létezik → üres szótárral tér vissza.
2. Soronként beolvassa a fájlt, minden `"KULCS;ÉRTÉK"` sort szétbont `;` mentén, és a
   (trimmelt) kulcs-érték párt beteszi az eredmény szótárba. Az üres sorokat kihagyja.
3. **Validáció:** ellenőrzi, hogy a szótár nem üres-e, valamint hogy a `MIN_DELTA` és a
   `FALL_THRESHOLD` kulcsok léteznek-e, számmá alakíthatók-e (`double.TryParse`), és
   `>= 0` értékűek-e.
4. **Ha bármelyik feltétel sérül** (hiányzó fájl-tartalom, hibás formátum, negatív érték):
   meghívja a `SetParameters(null, true)`-t (kényszerített alapértelmezett visszaállítás),
   majd **rekurzívan újra meghívja önmagát** (`ReadConfiguration()`), hogy a most már
   érvényes fájlt olvassa be és adja vissza.

**Hívási helyek:** `SettingsForm` konstruktora (a szerkesztő mezők feltöltéséhez, ha van
kapcsolat), illetve közvetve a `Hardware.SetParameters()` a "csak eszközre írás" ágon.

### `static class WriteType` (beágyazott osztály)

| Konstans | Érték | Jelentés |
|---|---|---|
| `Device` | `"DEVICE"` | Csak az eszközre írjon |
| `File` | `"FILE"` | Csak a fájlba írjon |
| `Both` | `Device + File` = `"DEVICEFILE"` | Mindkettőre írjon |

Mivel a `WriteConfiguration` a `writeType.Contains("DEVICE")` / `writeType.Contains("FILE")`
mintát használja, a `"DEVICEFILE"` string mindkét feltételre igaz lesz – ezért működik a
`Both` "vagylagos" jelölés egyetlen string konstansként, külön enum vagy flag nélkül.

---

## 2. `Application` (`Application.cs`)

Jelenleg **funkcionálisan üres** osztály – nincs benne aktív, lefordított kód, csak egy
teljes egészében kikommentezett tervezet:

```csharp
// TODO
/*
static string localPath = ...
static string appFolder = ...
static string configureFile = Path.Combine(appFolder, "application_configuration.csv");

public static Dictionary<string, string> Configs { ... }
*/
```

**Célja a tervek szerint:** a `Hardware`-hez hasonló mintázatban egy **alkalmazás-szintű**
(nem az eszközhöz, hanem magához a szoftverhez tartozó) beállítás-kezelő létrehozása, saját
`application_configuration.csv` fájllal. A kikommentezett `Configs` property jelenlegi
formájában (`get => Configs ?? (Configs = new Dictionary...)`) egyébként végtelen
rekurzióba futna, ha élesítenék – ez egy még be nem fejezett tervezet, nem használatra kész
kód.

---

⬅ Vissza az [áttekintéshez](https://github.com/Snewkovits/Vascular-Pressure-Measurement-System/blob/master/README.md) · Előző: [Utils névtér](https://github.com/Snewkovits/Vascular-Pressure-Measurement-System/blob/master/Docs/Utils.md) · Következő: [Main főablak](https://github.com/Snewkovits/Vascular-Pressure-Measurement-System/blob/master/Docs/Main_form.md)
