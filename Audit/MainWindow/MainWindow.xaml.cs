using System;
using System.Windows;
using System.Windows.Navigation;

namespace Audit.MainWindow;

public partial class MainWindow : Window
{
    private readonly LogInWindow _logInWindow;
    public MainWindow(LogInWindow logInWindow)
    {
        InitializeComponent();
        _logInWindow = logInWindow;
        
    }

    private bool _shown;
    protected override void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);

        if (_shown)
            return;

        _shown = true;
        _logInWindow.Close();
    }
    
    private void OnClick_BtnNavCategories(object sender, RoutedEventArgs e)
    {
        CenterFrame.Source = new Uri("CategoriesPage.xaml", UriKind.Relative);
    }

    private void OnClick_BtnNavWorkers(object sender, RoutedEventArgs e)
    {
        CenterFrame.Source = new Uri("WorkersPage.xaml", UriKind.Relative);
    }

    private void OnClick_BtnNavHoursRecords(object sender, RoutedEventArgs e)
    {
        CenterFrame.Source = new Uri("HoursRecordsPage.xaml", UriKind.Relative);
    }

    private void OnClick_BtnNavCompany(object sender, RoutedEventArgs e)
    {
        CenterFrame.Source = new Uri("CompanyPage.xaml", UriKind.Relative);
    }

    private void OnClick_BtnNavUsers(object sender, RoutedEventArgs e)
    {
        CenterFrame.Source = new Uri("UsersPage.xaml", UriKind.Relative);
    }
}