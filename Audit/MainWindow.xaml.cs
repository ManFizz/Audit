using System;
using System.Windows;
using System.Windows.Controls;

namespace Audit;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var app = (App) Application.Current;
        if (app.ActiveUser == null)
            throw new Exception("Empty user");
        
        UserLogin.Text = app.ActiveUser.Login;
        UserType.Text = app.ActiveUser.Type.ToString();
        if (app.ActiveUser.Type == TypeUser.worker)
        {
            BtnWorkers.Visibility = Visibility.Collapsed;
            TextBlock_Change.Visibility = Visibility.Collapsed;
            Button_Save.Visibility = Visibility.Collapsed;
            Button_Cancel.Visibility = Visibility.Collapsed;
        }
        
        if (app.ActiveUser.Type is not (TypeUser.hr or TypeUser.admin))
        {
            BtnUsers.Visibility = Visibility.Collapsed;
        }

        if (app.ActiveUser.Type is not (TypeUser.bookkeeper or TypeUser.admin))
        {
            BtnReport.Visibility = Visibility.Collapsed;
        }
    }
    
    private bool IsAllOk()
    {
        var app = (App) Application.Current;
        if (app.QueryList.Count > 0)
        {
            var dialogResult = MessageBox.Show("Вы внесли изменения.\nХотите их сохранить?", "Статус изменений", MessageBoxButton.YesNoCancel);
            if (dialogResult == MessageBoxResult.Yes)
            {
                app.SaveQuery();
                return true;
            }

            if (dialogResult == MessageBoxResult.No)
            {
                app.QueryList.Clear();
                app.UpdateAllTables();
                MessageBox.Show("Изменения отменены", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None);
            }

            return false;
        }
        return true;
    }
    
    private void ClearStyleBtns(Control btn)
    {
        BtnCategories.FontWeight = FontWeights.Normal;
        BtnCompany.FontWeight = FontWeights.Normal;
        BtnUsers.FontWeight = FontWeights.Normal;
        BtnHoursRecords.FontWeight = FontWeights.Normal;
        BtnWorkers.FontWeight = FontWeights.Normal;
        BtnReport.FontWeight = FontWeights.Normal;
        
        btn.FontWeight = FontWeights.Bold;
    }
    
    private void OnClick_BtnNavCategories(object sender, RoutedEventArgs e)
    {
        if (!IsAllOk())
            return;
        
        CenterFrame.Source = new Uri("Pages/CategoriesPage.xaml", UriKind.Relative);
        ClearStyleBtns(BtnCategories);
    }

    private void OnClick_BtnNavWorkers(object sender, RoutedEventArgs e)
    {
        if (!IsAllOk())
            return;
        
        CenterFrame.Source = new Uri("Pages/WorkersPage.xaml", UriKind.Relative);
        ClearStyleBtns(BtnWorkers);
    }

    private void OnClick_BtnNavHoursRecords(object sender, RoutedEventArgs e)
    {
        if (!IsAllOk())
            return;
        
        CenterFrame.Source = new Uri("Pages/HoursRecordsPage.xaml", UriKind.Relative);
        ClearStyleBtns(BtnHoursRecords);
    }

    private void OnClick_BtnNavCompany(object sender, RoutedEventArgs e)
    {
        if (!IsAllOk())
            return;
        
        CenterFrame.Source = new Uri("Pages/CompanyPage.xaml", UriKind.Relative);
        ClearStyleBtns(BtnCompany);
    }

    private void OnClick_BtnNavUsers(object sender, RoutedEventArgs e)
    {
        if (!IsAllOk())
            return;
        
        CenterFrame.Source = new Uri("Pages/UsersPage.xaml", UriKind.Relative);
        ClearStyleBtns(BtnUsers);
    }
    private void OnClick_BtnNavReport(object sender, RoutedEventArgs e)
    {
        if (!IsAllOk())
            return;
        
        CenterFrame.Source = new Uri("Pages/Report.xaml", UriKind.Relative);
        ClearStyleBtns(BtnReport);
    }

    private void Exit(object sender, RoutedEventArgs e)
    {
        if (!IsAllOk())
            return;
        
        var app = (App) Application.Current;
        app.ActiveUser = null;

        Hide();
        var logInWindow = new LogInWindow();
        logInWindow.Show();
        Close();
    }

    private void OnClick_BtnSave(object sender, RoutedEventArgs e)
    {
        var app = (App) Application.Current;
        if (app.QueryList.Count == 0)
        {
            MessageBox.Show("Нечего сохранять", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.None);
            return;
        }
        app.SaveQuery();
    }

    private void OnClick_BtnCancel(object sender, RoutedEventArgs e)
    {
        var app = (App) Application.Current;
        if (app.QueryList.Count == 0)
        {
            MessageBox.Show("Нечего отменять", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.None);
            return;
        }
        app.QueryList.Clear();
        MessageBox.Show("Изменения отменены", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None);
    }
}