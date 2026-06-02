using Avalonia.Controls;
using Avalonia.Interactivity;
using EvidenceKnihFilmu.Models;

namespace EvidenceKnihFilmu.Views;

public partial class DetailWindow : Window
{
    public DetailWindow(MediaItem polozka)
    {
        InitializeComponent();
        ZobrazDetail(polozka);
    }

    /// <summary>Vyplní všechny ovládací prvky daty z položky.</summary>
    private void ZobrazDetail(MediaItem p)
    {
        Title = $"Detail – {p.Title}";
        HeaderTitle.Text = p.Title;

        TypeLabel.Text      = p.Type == "Kniha" ? "📖  Kniha" : "🎬  Film";
        GenreLabel.Text     = p.Genre;
        LengthLabel.Text    = p.LengthDisplay;
        RatingLabel.Text    = $"⭐ {p.Rating:F1} / 10";
        CompletedLabel.Text = p.CompletedDisplay;
        DateLabel.Text      = p.DateAdded.ToString("dd. MM. yyyy");

        DescriptionLabel.Text = string.IsNullOrWhiteSpace(p.Description)
            ? "(bez popisu)"
            : p.Description;
    }

    private void CloseButton_Click(object? sender, RoutedEventArgs e) => Close();
}
