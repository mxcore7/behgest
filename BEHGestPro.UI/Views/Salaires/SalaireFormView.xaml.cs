using BEHGestPro.UI.Views.Shared;
using System.Windows;
using System.Windows.Controls;

namespace BEHGestPro.UI.Views.Salaires;

public partial class SalaireFormView : UserControl
{
    public SalaireFormView()
    {
        InitializeComponent();
    }

    private void CancelBtn_Click(object sender, RoutedEventArgs e)
    {
        var window = Window.GetWindow(this) as FormDialog;
        window?.Close();
    }
}
