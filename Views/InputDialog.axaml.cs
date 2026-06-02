using Avalonia.Controls;
using Avalonia.Interactivity;

namespace EvidenceKnihFilmu.Views;

public partial class InputDialog : Window
{
    public InputDialog(string nadpis, string prompt)
    {
        InitializeComponent();
        Title = nadpis;
        PromptLabel.Text = prompt;
    }

    private void Ok_Click(object? sender, RoutedEventArgs e)
        => Close(InputBox.Text?.Trim());

    private void Cancel_Click(object? sender, RoutedEventArgs e)
        => Close(null);
}
