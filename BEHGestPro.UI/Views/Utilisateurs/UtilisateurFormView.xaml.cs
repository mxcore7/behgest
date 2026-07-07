using BEHGestPro.UI.ViewModels.Utilisateurs;
using System.Windows;
using System.Windows.Controls;

namespace BEHGestPro.UI.Views.Utilisateurs;

public partial class UtilisateurFormView : UserControl
{
    public UtilisateurFormView()
    {
        InitializeComponent();
    }

    private void PwdBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is UtilisateurFormViewModel vm)
            vm.MotDePasse = PwdBox.Password;
    }

    private void CancelBtn_Click(object sender, RoutedEventArgs e)
    {
        // Ferme le FormDialog parent
        var parent = Window.GetWindow(this);
        parent?.Close();
    }
}
