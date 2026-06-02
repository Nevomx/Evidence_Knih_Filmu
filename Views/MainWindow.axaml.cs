using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using EvidenceKnihFilmu.Models;
using EvidenceKnihFilmu.Services;
using System.Collections.Generic;

namespace EvidenceKnihFilmu.Views;

public partial class MainWindow : Window
{
    private readonly DataService _dataService = new();

    public MainWindow()
    {
        InitializeComponent();
        NactiZanryDoFiltrů();
        ObnovjSeznam();
    }

    private void NactiZanryDoFiltrů()
    {
        var zanry = new List<string> { "Vše" };
        zanry.AddRange(_dataService.GetGenres());
        GenreFilter.ItemsSource = zanry;
        GenreFilter.SelectedIndex = 0;
    }

    private void ObnovjSeznam()
    {
        // Ochrana před voláním během InitializeComponent
        if (SearchBox is null || ItemsListBox is null || StatusLabel is null) return;

        string query = SearchBox.Text ?? "";
        string zanr  = GenreFilter.SelectedItem as string ?? "Vše";
        string typ   = (TypComboBox.SelectedItem as ComboBoxItem)?.Content as string ?? "Vše";

        var items = _dataService.Search(query, zanr, typ);
        ItemsListBox.ItemsSource = items;

        int celkem = _dataService.Count;
        StatusLabel.Text   = $"Zobrazeno: {items.Count} z {celkem} položek";
        SubtitleLabel.Text = $"{celkem} položek v evidenci";
    }

    private void SearchBox_TextChanged(object? sender, TextChangedEventArgs e)
        => ObnovjSeznam();

    private void Filter_Changed(object? sender, SelectionChangedEventArgs e)
        => ObnovjSeznam();

    private void ItemsListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        bool vybrano = ItemsListBox.SelectedItem is MediaItem;
        EditButton.IsEnabled   = vybrano;
        DeleteButton.IsEnabled = vybrano;
        DetailButton.IsEnabled = vybrano;
    }

    private void ItemsListBox_DoubleTapped(object? sender, RoutedEventArgs e)
    {
        if (ItemsListBox.SelectedItem is MediaItem) ZobrazDetail();
    }

    private async void AddButton_Click(object? sender, RoutedEventArgs e)
    {
        var okno = new AddEditWindow(null, _dataService.GetGenres());
        await okno.ShowDialog(this);

        if (okno.Vysledek != null)
        {
            _dataService.Add(okno.Vysledek);
            NactiZanryDoFiltrů();
            ObnovjSeznam();
        }
    }

    private async void EditButton_Click(object? sender, RoutedEventArgs e)
    {
        if (ItemsListBox.SelectedItem is not MediaItem vybrana) return;

        var okno = new AddEditWindow(vybrana, _dataService.GetGenres());
        await okno.ShowDialog(this);

        if (okno.Vysledek != null)
        {
            _dataService.Update(okno.Vysledek);
            NactiZanryDoFiltrů();
            ObnovjSeznam();
        }
    }

    private async void DeleteButton_Click(object? sender, RoutedEventArgs e)
    {
        if (ItemsListBox.SelectedItem is not MediaItem vybrana) return;

        var dialog = new ConfirmDialog($"Opravdu smazat \"{vybrana.Title}\"?");
        bool potvrdit = await dialog.ShowDialog<bool>(this);

        if (potvrdit)
        {
            _dataService.Delete(vybrana.Id);
            ObnovjSeznam();
        }
    }

    private void DetailButton_Click(object? sender, RoutedEventArgs e)
        => ZobrazDetail();

    private void ZobrazDetail()
    {
        if (ItemsListBox.SelectedItem is not MediaItem vybrana) return;
        var okno = new DetailWindow(vybrana);
        okno.ShowDialog(this);
    }

    private async void ExportButton_Click(object? sender, RoutedEventArgs e)
    {
        var soubor = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Exportovat seznam",
            SuggestedFileName = "evidence_export",
            FileTypeChoices = new[]
            {
                new FilePickerFileType("Textový soubor") { Patterns = new[] { "*.txt" } }
            }
        });

        if (soubor != null)
        {
            _dataService.ExportToTxt(soubor.Path.LocalPath);
            StatusLabel.Text = "✅ Export dokončen!";
        }
    }

    private async void ImportButton_Click(object? sender, RoutedEventArgs e)
    {
        var soubory = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Importovat data",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("Textový soubor") { Patterns = new[] { "*.txt" } }
            }
        });

        if (soubory.Count > 0)
        {
            int pocet = _dataService.ImportFromTxt(soubory[0].Path.LocalPath);
            StatusLabel.Text = $"✅ Importováno {pocet} nových položek.";
            NactiZanryDoFiltrů();
            ObnovjSeznam();
        }
    }
}