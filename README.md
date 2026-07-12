<div align="center">
  <h1>Vascular Pressure Measurment System</h1>
  <h1>VRP</h1>
  <p><strong>Valós Idejű Adatgyűjtő és Elemző Rendszer</strong></p>
  <p><em>Diplomamunka / Szakdolgozat Szoftverkomponens</em></p>
</div>

---

A **Venduino** egy komplex mérnök-informatikai célfeladat megvalósítására létrehozott asztali kliensalkalmazás. A rendszer egy egyedi fejlesztésű mikrokontrolleres mérőeszköz (Arduino) és egy .NET alapú vizualizációs szoftver (Windows Forms) szoros együttműködésére épül. Elsődleges feladata az analóg szenzorbemenetek nagy sebességű, valós idejű mintavételezése, a hibatűrő soros adatátvitel, a szálbiztos pufferelés, valamint a gyűjtött adathalmaz utólagos matematikai analízise.

A szoftverarchitektúra az **eseményvezérelt programszerkezet**, a **többszálú végrehajtás** (Multithreading) és a **numerikus módszerek** gyakorlati alkalmazását hivatott demonstrálni.

## Rendszerarchitektúra és Szoftverstruktúra

Az alkalmazás moduláris felépítésű, követve a felelősségi körök elválasztásának (*Separation of Concerns*) elvét. A forráskód az alábbi objektumorientált struktúrára tagozódik:

```text
├── Main.cs / Main.Designer.cs          # Főprogram és UI
├── Forms/
│   ├── SettingsForm.cs / .Designer.cs  # Konfigurációs felület
│   └── AnalysisForm.cs / .Designer.cs  # Matematikai modul
└── Utils/
    ├── Connection.cs                   # Adatkapcsolati réteg
    ├── Measure.cs                      # Mérőszál
    └── GlobalData.cs                   # Globális állapot és perzisztencia
```

## Főbb Mérnöki Megoldások és Funkciók
1. Robusztus, Keret-alapú Soros Kommunikáció (Connection.cs)

A mikrokontroller és a PC közötti adatcsere nem nyers adatfolyamként, hanem egyedi tervezésű, szigorúan validált üzenetkeretekben történik a hibás csomagok kiszűrésére:
```text
Formátum: < ÜzenetID | Parancs | Adat | Checksum >
```
* Átviteli biztonság: Minden beérkező keret Checksum ellenőrzésen esik át (CalculateChecksum), kivédve a zajos soros vonal okozta adatkorrupciót.

* Életjel-ellenőrzés (Keep-Alive): A háttérben egy folyamatos, aszinkron PING-PONG mechanizmus fut. Ha az eszköz egymás után 3 alkalommal hibás választ ad vagy nem válaszol (Timeout), a szoftver automatikusan lezárja a portot és jelzi a kapcsolat megszakadását, megvédve a rendszert a kritikus futásidejű hibáktól.

2. Többszálú Adatfeldolgozás és Szálbiztosság (Measure.cs)

A nagy sebességű mintavételezés során kritikus követelmény, hogy a felhasználói felület (UI) teljesen reszponzív maradjon.

* Producer-Consumer mintázat: A mérés indításakor a szoftver egy dedikált háttérszálat (Thread) nyit. Ez a mérőszál folyamatosan olvassa a soros portot és pakolja az adatokat egy elsőbbségi sorba (Queue).

* Kölcsönös kizárás: A kritikus szakaszok védelmét és a szálbiztosságot a lock (_lock) objektum biztosítja, megakadályozva, hogy a grafikonfrissítő UI-szál és a mérőszál egyszerre módosítsa a memóriapuffert.

3. Autonóm Trendfigyelés és Vészleállítás

A rendszer rendelkezik egy intelligens, hardver-szoftver kooperatív védelmi vonallal:

* A SettingsForm-on keresztül konfigurálható a Min Delta (a minimális változási küszöb) és a Fall Threshold (az egymást követő eső minták kritikus száma).

* Ezeket a paramétereket az alkalmazás indításkor szinkronizálja a hardverrel, illetve helyileg egy config.csv fájlban tárolja az AppData mappában.

* Az Arduino firmware ezen értékek alapján képes autonóm módon detektálni a mért fizikai mennyiség hirtelen zuhanását (pl. csőtörés, nyomásesés), és azonnali STOP_MEASURE / FALL_DETECTED paranccsal leállítani a folyamatot, amit a PC-s szoftver azonnal naplóz.

4. Matematikai Analízis és Numerikus Deriválás (AnalysisForm.cs)

A mérés lezárultával a felhasználónak lehetősége van a grafikon egy tetszőleges szakaszát kijelölni. A szoftver ekkor automatikusan végrehajtja a kijelölt diszkrét adathalmaz első numerikus deriváltjának kiszámítását (változási sebesség, meredekség):

$\large{\frac{dy}{dx}=\cfrac{y_i+1-y_i}{x_i+1-x_i}}$
* Statisztikai modul: Meghatározásra kerül a szakasz átlagos változási sebessége, valamint a maximális emelkedési (Peak Rise) és maximális esési (Peak Fall) pontok pontos koordinátái.

* Kettős Vizualizáció: Az AnalysisForm egymásra szinkronizálva jeleníti meg az eredeti mérési görbét és a kiszámított derivált függvényt, segítve a fizikai folyamat anomáliáinak matematikai elemzését.

## Alkalmazott Technológiai Stog
* Fejlesztőkörnyezet: Microsoft Visual Studio
* Programozási nyelv: C# (Erősen típusos, OOP szemlélet)
* Keretrendszer: .NET Framework (Windows Forms)
* Adatvizualizáció: System.Windows.Forms.DataVisualization.Charting (MSChart)
* I/O Kommunikáció: System.IO.Ports.SerialPort (Aszinkron Serial Communication, 9600 Baud)
* Adatperzisztencia: CSV (Comma-Separated Values) adatstruktúra

Konklúzió: A szoftver sikeresen demonstrálja egy ipari jellegű HMI/SCADA (Human-Machine Interface) alkalmazás alapelveit. A futásidejű hibák kezelése (try-catch blokkok a portkezelésnél, beviteli mezők validálása a konfigurációnál), a szálbiztos adatpufferelés és a numerikus matematikai módszerek integrációját.
