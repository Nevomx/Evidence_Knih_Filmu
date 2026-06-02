using EvidenceKnihFilmu.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EvidenceKnihFilmu.Services;
// pracuje primo se soubory na disku solid state drive
/// <summary>
/// Zajišťuje veškerou práci se soubory:
/// - ukládání/načítání dat z AppData
/// - export do čitelného TXT
/// - správu žánrů
/// </summary>
public static class FileService
{
    // Cesta ke složce aplikace v AppData\Roaming
    private static readonly string AppDataFolder = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "EvidenceKnihFilmu");

    // Cesta k datovému souboru
    public static readonly string DataFilePath = Path.Combine(AppDataFolder, "data.txt");

    // Cesta k souboru se žánry
    private static readonly string GenresFilePath = Path.Combine(AppDataFolder, "genres.txt");

    /// <summary>
    /// Vytvoří složku AppData a prázdné soubory, pokud ještě neexistují.
    /// Volá se při startu aplikace.
    /// </summary>
    public static void EnsureAppDataExists()
    {
        // Vytvoř složku pokud neexistuje
        Directory.CreateDirectory(AppDataFolder);

        // Vytvoř prázdný datový soubor
        if (!File.Exists(DataFilePath))
            File.WriteAllText(DataFilePath, string.Empty, Encoding.UTF8);

        // Vytvoř soubor s výchozími žánry
        if (!File.Exists(GenresFilePath))
        {
            var defaultGenres = new[]
            {
                "Akční", "Animovaný", "Dokumentární", "Drama",
                "Fantasy", "Horor", "Komedie", "Romantika",
                "Sci-Fi", "Thriller", "Jiné"
            };
            File.WriteAllLines(GenresFilePath, defaultGenres, Encoding.UTF8);
        }
    }

    // ======================
    // Ukládání a načítání dat
    // ======================

    /// <summary>Uloží seznam položek do AppData/data.txt</summary>
    public static void SaveData(List<MediaItem> items)
    {
        EnsureAppDataExists();
        var lines = items.Select(i => i.ToTxtLine());
        File.WriteAllLines(DataFilePath, lines, Encoding.UTF8);
    }

    /// <summary>Načte seznam položek z AppData/data.txt</summary>
    public static List<MediaItem> LoadData()
    {
        EnsureAppDataExists();

        var items = new List<MediaItem>();

        foreach (var line in File.ReadAllLines(DataFilePath, Encoding.UTF8))
        {
            var item = MediaItem.FromTxtLine(line);
            if (item != null)
                items.Add(item);
        }

        return items;
    }

    // ======================
    // Export do TXT souboru
    // ======================

    /// <summary>
    /// Exportuje seznam do čitelného TXT souboru (pro uživatele).
    /// Liší se od interního formátu – je čitelný i bez aplikace.
    /// </summary>
    public static void ExportToTxt(List<MediaItem> items, string filePath)
    {
        var sb = new StringBuilder();
        sb.AppendLine("╔══════════════════════════════════════════╗");
        sb.AppendLine("║       EVIDENCE KNIH A FILMŮ              ║");
        sb.AppendLine("╚══════════════════════════════════════════╝");
        sb.AppendLine($"Exportováno: {DateTime.Now:dd.MM.yyyy HH:mm}");
        sb.AppendLine($"Celkem položek: {items.Count}");
        sb.AppendLine();

        // Seřadit abecedně
        foreach (var item in items.OrderBy(i => i.Title))
        {
            sb.AppendLine(new string('─', 44));
            sb.AppendLine($"  Název:      {item.Title}");
            sb.AppendLine($"  Typ:        {item.Type}");
            sb.AppendLine($"  Žánr:       {item.Genre}");
            sb.AppendLine($"  Délka:      {item.LengthDisplay}");
            sb.AppendLine($"  Hodnocení:  {item.Rating:F1}/10");
            sb.AppendLine($"  Stav:       {item.CompletedDisplay.TrimStart('✓', '○', ' ')}");

            if (!string.IsNullOrWhiteSpace(item.Description))
                sb.AppendLine($"  Popis:      {item.Description}");

            sb.AppendLine($"  Přidáno:    {item.DateAdded:dd.MM.yyyy}");
        }

        sb.AppendLine(new string('─', 44));
        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
    }

    // ======================
    // Žánry
    // ======================

    /// <summary>Načte seznam uložených žánrů.</summary>
    public static List<string> LoadGenres()
    {
        EnsureAppDataExists();
        return File.ReadAllLines(GenresFilePath, Encoding.UTF8)
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .OrderBy(l => l)
            .ToList();
    }

    /// <summary>Uloží seznam žánrů do AppData/genres.txt</summary>
    public static void SaveGenres(List<string> genres)
    {
        EnsureAppDataExists();
        File.WriteAllLines(GenresFilePath, genres.OrderBy(g => g), Encoding.UTF8);
    }

    // ======================
    // Import z TXT souboru
    // ======================

    /// <summary>
    /// Importuje položky z interního TXT souboru (data.txt formát).
    /// Vrátí seznam načtených položek.
    /// </summary>
    public static List<MediaItem> ImportFromTxt(string filePath)
    {
        var items = new List<MediaItem>();

        foreach (var line in File.ReadAllLines(filePath, Encoding.UTF8))
        {
            var item = MediaItem.FromTxtLine(line);
            if (item != null)
                items.Add(item);
        }

        return items;
    }
}
