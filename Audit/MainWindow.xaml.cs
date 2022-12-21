using System;
using System.Windows;

namespace Audit;

public partial class MainWindow : Window
{
    private readonly LogInWindow _logInWindow;
    public MainWindow(LogInWindow logInWindow)
    {
        InitializeComponent();
        _logInWindow = logInWindow;
        var app = (App) Application.Current;
        if (app.ActiveUser == null)
            throw new Exception("Empty user");
        
        UserLogin.Text = app.ActiveUser.Login;
        UserType.Text = app.ActiveUser.Type.ToString();
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
        CenterFrame.Source = new Uri("Pages/CategoriesPage.xaml", UriKind.Relative);
    }

    private void OnClick_BtnNavWorkers(object sender, RoutedEventArgs e)
    {
        CenterFrame.Source = new Uri("Pages/WorkersPage.xaml", UriKind.Relative);
    }

    private void OnClick_BtnNavHoursRecords(object sender, RoutedEventArgs e)
    {
        CenterFrame.Source = new Uri("Pages/HoursRecordsPage.xaml", UriKind.Relative);
    }

    private void OnClick_BtnNavCompany(object sender, RoutedEventArgs e)
    {
        CenterFrame.Source = new Uri("Pages/CompanyPage.xaml", UriKind.Relative);
    }

    private void OnClick_BtnNavUsers(object sender, RoutedEventArgs e)
    {
        CenterFrame.Source = new Uri("Pages/UsersPage.xaml", UriKind.Relative);
    }

    private void Exit(object sender, RoutedEventArgs e)
    {
        var app = (App) Application.Current;
        app.ActiveUser = null;
        
        var window = new LogInWindow(this);
        window.Show();
    }
}