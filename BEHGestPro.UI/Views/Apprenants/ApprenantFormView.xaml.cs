using BEHGestPro.UI.Views.Shared;
using System.Windows;
using System.Windows.Controls;

namespace BEHGestPro.UI.Views.Apprenants;

public partial class ApprenantFormView : UserControl
{
    public ApprenantFormView()
    {
        InitializeComponent();
    }

    private void CancelBtn_Click(object sender, RoutedEventArgs e)
    {
        // Find parent FormDialog and close it
        var window = Window.GetWindow(this) as FormDialog;
        window?.Close();
    }
}
