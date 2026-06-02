using Avalonia.Controls;
using Avalonia.Interactivity;
using EvidenceKnihFilmu.Models;
using System.Collections.Generic;

namespace EvidenceKnihFilmu.Views;

public partial class AddEditWindow : Window
{
    public MediaItem? Vysledek { get; private set; } = null;
    private readonly MediaItem? _puvodniPolozka;
    private List<string> _zanry; // vlastní seznam žánrů

    public AddEditWindow(MediaItem? polozka, List<string> zanry)
    {
        InitializeComponent();
        _puvodniPolozka = polozka;
        _zanry = new List<string>(zanry);

        GenreBox.ItemsSource = _zanry;
        GenreBox.SelectedIndex = 0;

        // ValueChanged přihlásit v code-behind (vyhne se problému se signaturou)
        RatingSlider.ValueChanged += (s, e) => RatingValue.Text = e.NewValue.ToString("F1");

        if (polozka != null)
        {
            WindowTitle.Text = "Upravit položku";
            Title = "Upravit položku";
            NaplnFormular(polozka);
        }
    }

    private void NaplnFormular(MediaItem p)
    {
        TitleBox.Text = p.Title;
        TypeBook.IsChecked = p.Type == "Kniha";
        TypeFilm.IsChecked = p.Type == "Film";

        // Použij _zanry místo GenreBox.Items (ItemCollection nejde castovat na List)
        int idx = _zanry.IndexOf(p.Genre);
        GenreBox.SelectedIndex = idx >= 0 ? idx : 0;

        LengthBox.Text = p.Length.ToString();
        RatingSlider.Value = p.Rating;
        CompletedBox.IsChecked = p.IsCompleted;
        DescriptionBox.Text = p.Description;
    }

    private void TypeRadio_Changed(object? sender, RoutedEventArgs e)
    {
        LengthLabel.Text = TypeBook.IsChecked == true ? "Počet stran *" : "Délka (minuty) *";
    }

    private async void NewGenre_Click(object? sender, RoutedEventArgs e)
    {
        var dialog = new InputDialog("Nový žánr", "Zadejte název nového žánru:");
        string? novyZanr = await dialog.ShowDialog<string?>(this);

        if (!string.IsNullOrWhiteSpace(novyZanr))
        {
            if (!_zanry.Contains(novyZanr))
            {
                _zanry.Add(novyZanr);
                GenreBox.ItemsSource = null;
                GenreBox.ItemsSource = _zanry;
            }
            GenreBox.SelectedItem = novyZanr;
        }
    }

    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        Vysledek = null;
        Close();
    }

    private void SaveButton_Click(object? sender, RoutedEventArgs e)
    {
        if (!ValidujFormular()) return;

        var polozka = _puvodniPolozka != null
            ? new MediaItem { Id = _puvodniPolozka.Id, DateAdded = _puvodniPolozka.DateAdded }
            : new MediaItem();

        polozka.Title       = TitleBox.Text!.Trim();
        polozka.Type        = TypeBook.IsChecked == true ? "Kniha" : "Film";
        polozka.Genre       = GenreBox.SelectedItem as string ?? "";
        polozka.Length      = int.TryParse(LengthBox.Text, out var len) ? len : 0;
        polozka.Rating      = RatingSlider.Value;
        polozka.IsCompleted = CompletedBox.IsChecked == true;
        polozka.Description = DescriptionBox.Text?.Trim() ?? "";

        Vysledek = polozka;
        Close();
    }

    private bool ValidujFormular()
    {
        bool ok = true;

        if (string.IsNullOrWhiteSpace(TitleBox.Text))
        { TitleError.Text = "Název nesmí být prázdný."; ok = false; }
        else TitleError.Text = "";

        if (GenreBox.SelectedItem == null)
        { GenreError.Text = "Vyberte žánr."; ok = false; }
        else GenreError.Text = "";

        if (!int.TryParse(LengthBox.Text, out int d) || d < 0)
        { LengthError.Text = "Zadejte platné číslo (≥ 0)."; ok = false; }
        else LengthError.Text = "";

        return ok;
    }
}