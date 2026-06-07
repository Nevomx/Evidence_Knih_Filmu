# Evidence knih a filmů

Aplikace pro správu knih a filmů vytvořená v Avalonia UI (.NET 9).

## Spuštění projektu

### Požadavky
- .NET 9 SDK (https://dotnet.microsoft.com/download)
- Rider

### Jet Brains
1. Otevři `EvidenceKnihFilmu.csproj`
2. Počkej na obnovení NuGet balíčků (automaticky)
3. Stiskni F5 nebo tlačítko "Start"

### Příkazová řádka
```
dotnet restore
dotnet run
```

## Struktura projektu

```
EvidenceKnihFilmu/
├── Models/
│   └── MediaItem.cs          – datový model položky
├── Services/
│   ├── DataService.cs        – aplikační logika (CRUD, filtrování)
│   └── FileService.cs        – práce se soubory a AppData
└── Views/
    ├── MainWindow             – hlavní okno (seznam + filtrování)
    ├── AddEditWindow          – formulář pro přidání/editaci
    ├── DetailWindow           – detail položky
    ├── ConfirmDialog          – potvrzení smazání
    └── InputDialog            – zadání textu (nový žánr)
```

## Data

Aplikace ukládá data do:
`%AppData%\EvidenceKnihFilmu\`

- `data.txt` – seznam položek (interní formát)
- `genres.txt` – uložené žánry

## Funkce

### Povinné
- ✅ Přidání/editace/smazání knihy nebo filmu
- ✅ Hodnocení (1–10), žánr, popis, délka
- ✅ Označení přečteno/zhlédnuto
- ✅ Zobrazení detailu po kliknutí
- ✅ Export do TXT
- ✅ Ukládání dat v AppData
- ✅ Validace vstupů
- ✅ 3 okna (MainWindow, AddEditWindow, DetailWindow)

### Bonus
- ✅ Vyhledávání podle textu
- ✅ Filtrování podle žánru a typu
- ✅ Editace záznamů
- ✅ Import dat z TXT souboru
- ✅ Přidání vlastních žánrů
