using System;
using System.Globalization;

namespace EvidenceKnihFilmu.Models;

/// <summary>
/// Reprezentuje jednu položku (knihu nebo film) v evidenci.
/// </summary>
public class MediaItem
{
    // Unikátní identifikátor položky
    public string Id { get; set; } = Guid.NewGuid().ToString();

    // Název knihy nebo filmu
    public string Title { get; set; } = string.Empty;

    // Typ: "Kniha" nebo "Film"
    public string Type { get; set; } = "Kniha";

    // Žánr (např. Akční, Drama, Sci-Fi...)
    public string Genre { get; set; } = string.Empty;

    // Popis nebo anotace
    public string Description { get; set; } = string.Empty;

    // Délka: počet stran pro knihy, minuty pro filmy
    public int Length { get; set; } = 0;

    // Hodnocení uživatele (1–10)
    public double Rating { get; set; } = 7.0;

    // Přečteno / Zhlédnuto
    public bool IsCompleted { get; set; } = false;

    // Datum přidání do evidence
    public DateTime DateAdded { get; set; } = DateTime.Now;

    // -------------------------
    // Pomocné vlastnosti pro UI
    // -------------------------

    /// <summary>Zobrazí délku s jednotkou (stran / min).</summary>
    public string LengthDisplay =>
        Type == "Kniha" ? $"{Length} stran" : $"{Length} min";

    /// <summary>Zobrazí stav přečtení/zhlédnutí.</summary>
    public string CompletedDisplay =>
        Type == "Kniha"
            ? (IsCompleted ? "✓ Přečteno" : "○ Nepřečteno")
            : (IsCompleted ? "✓ Zhlédnuto" : "○ Nezhlédnuto");

    /// <summary>Hodnocení jako text pro listbox.</summary>
    public string RatingDisplay => $"⭐ {Rating:F1}/10";

    /// <summary>Emoji ikona podle typu.</summary>
    public string TypeIcon => Type == "Kniha" ? "📖" : "🎬";

    // -------------------------
    // Serializace do/z TXT
    // -------------------------

    /// <summary>
    /// Uloží položku jako jeden řádek TXT souboru.
    /// Pole jsou oddělena znakem '|', svislítka v popisu jsou nahrazena středníkem.
    /// </summary>
    public string ToTxtLine()
    {
        string safeDesc = Description.Replace("|", ";");
        return string.Join("|",
            Id, Title, Type, Genre, safeDesc,
            Length.ToString(),
            Rating.ToString("F1", CultureInfo.InvariantCulture),
            IsCompleted.ToString(),
            DateAdded.ToString("yyyy-MM-dd"));
    }

    /// <summary>
    /// Načte položku z jednoho řádku TXT souboru.
    /// Vrátí null pokud řádek nemá správný formát.
    /// </summary>
    public static MediaItem? FromTxtLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line)) return null;

        var parts = line.Split('|');
        if (parts.Length < 9) return null;

        return new MediaItem // to jsou ty filmy a knihy uprostred
        {
            Id          = parts[0],
            Title       = parts[1],
            Type        = parts[2],
            Genre       = parts[3],
            Description = parts[4].Replace(";", "|"),
            Length      = int.TryParse(parts[5], out var len) ? len : 0,
            Rating      = double.TryParse(parts[6], NumberStyles.Any,
                              CultureInfo.InvariantCulture, out var rat) ? rat : 5.0,
            IsCompleted = bool.TryParse(parts[7], out var comp) && comp,
            DateAdded   = DateTime.TryParse(parts[8], out var date) ? date : DateTime.Now
        };
    }
}
