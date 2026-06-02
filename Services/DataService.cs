using EvidenceKnihFilmu.Models;
using System.Collections.Generic;
using System.Linq;

namespace EvidenceKnihFilmu.Services;
// pracuje s daty v pameti
/// <summary>
/// Aplikační logika – správa seznamu položek v paměti.
/// DataService neví nic o GUI, pouze pracuje s daty.
/// </summary>
public class DataService
{
    // Interní seznam všech položek
    private List<MediaItem> _items;

    // Interní seznam žánrů
    private List<string> _genres;

    public DataService()
    {
        // Načti data při startu
        _items  = FileService.LoadData();
        _genres = FileService.LoadGenres();
    }

    // ====================
    // CRUD operace
    // ====================

    /// <summary>Vrátí kopii celého seznamu.</summary>
    public List<MediaItem> GetAll() => _items.ToList();

    /// <summary>Přidá novou položku a uloží.</summary>
    public void Add(MediaItem item)
    {
        _items.Add(item);
        Save();
    }

    /// <summary>Aktualizuje existující položku (hledá podle Id) a uloží.</summary>
    public void Update(MediaItem updated)
    {
        int index = _items.FindIndex(i => i.Id == updated.Id);
        if (index >= 0)
        {
            _items[index] = updated;
            Save();
        }
    }

    /// <summary>Smaže položku podle Id a uloží.</summary>
    public void Delete(string id)
    {
        _items.RemoveAll(i => i.Id == id);
        Save();
    }

    // ====================
    // Vyhledávání a filtrování
    // ====================

    /// <summary>
    /// Vrátí filtrovaný a seřazený seznam.
    /// </summary>
    /// <param name="query">Text pro vyhledávání v názvu a popisu.</param>
    /// <param name="genreFilter">"Vše" = bez filtru, jinak konkrétní žánr.</param>
    /// <param name="typeFilter">"Vše" = bez filtru, "Kniha" nebo "Film".</param>
    public List<MediaItem> Search(string query, string genreFilter, string typeFilter)
    {
        IEnumerable<MediaItem> result = _items;

        // Filtr podle textu
        if (!string.IsNullOrWhiteSpace(query))
        {
            string q = query.ToLower();
            result = result.Where(i =>
                i.Title.ToLower().Contains(q) ||
                i.Description.ToLower().Contains(q));
        }

        // Filtr podle žánru
        if (!string.IsNullOrWhiteSpace(genreFilter) && genreFilter != "Vše")
            result = result.Where(i => i.Genre == genreFilter);

        // Filtr podle typu
        if (!string.IsNullOrWhiteSpace(typeFilter) && typeFilter != "Vše")
            result = result.Where(i => i.Type == typeFilter);

        return result.OrderBy(i => i.Title).ToList();
    }

    // ====================
    // Žánry
    // ====================

    /// <summary>Vrátí seznam všech žánrů.</summary>
    public List<string> GetGenres() => _genres.ToList();

    /// <summary>Přidá nový žánr (pokud ještě neexistuje) a uloží.</summary>
    public void AddGenre(string genre)
    {
        if (!_genres.Contains(genre))
        {
            _genres.Add(genre);
            FileService.SaveGenres(_genres);
        }
    }

    // ====================
    // Ukládání a export
    // ====================

    /// <summary>Uloží aktuální data do AppData.</summary>
    public void Save() => FileService.SaveData(_items); // => zkraceny zapis metody

    /// <summary>Exportuje seznam do čitelného TXT souboru.</summary>
    public void ExportToTxt(string filePath) => FileService.ExportToTxt(_items, filePath);

    /// <summary>
    /// Importuje položky z TXT souboru.
    /// Existující položky se stejným Id se přepíší, nové se přidají.
    /// </summary>
    public int ImportFromTxt(string filePath)
    {
        var imported = FileService.ImportFromTxt(filePath);
        int count = 0;

        foreach (var item in imported)
        {
            if (_items.Any(i => i.Id == item.Id))
            {
                // Přepiš existující
                Update(item);
            }
            else
            {
                _items.Add(item);
                count++;
            }
        }

        Save();
        return count;
    }

    /// <summary>Vrátí počet všech položek.</summary>
    public int Count => _items.Count;
}
