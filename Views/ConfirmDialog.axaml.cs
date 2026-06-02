using Avalonia.Controls;
using Avalonia.Interactivity;

namespace EvidenceKnihFilmu.Views;

public partial class ConfirmDialog : Window
{
    public ConfirmDialog(string zprava)
    {
        InitializeComponent();
        MessageLabel.Text = zprava;
    }

    private void Yes_Click(object? sender, RoutedEventArgs e) => Close(true);
    private void No_Click(object? sender, RoutedEventArgs e)  => Close(false);
}
