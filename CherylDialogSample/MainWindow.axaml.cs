using System;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace CherylDialogSample;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override async void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        // Open one dialog automatically so the visual effect is visible right away.
        await Task.Delay(700);
        (DataContext as MainViewModel)?.ShowWelcomeDialog();
    }
}
