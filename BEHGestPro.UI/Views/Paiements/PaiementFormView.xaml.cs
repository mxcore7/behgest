using BEHGestPro.UI.Views.Shared;
using System.Windows;
using System.Windows.Controls;

namespace BEHGestPro.UI.Views.Paiements;

public partial class PaiementFormView : UserControl
{
    public PaiementFormView()
    {
        InitializeComponent();
    }

    private void CancelBtn_Click(object sender, RoutedEventArgs e)
    {
        var window = Window.GetWindow(this) as FormDialog;
        window?.Close();
    }
}
