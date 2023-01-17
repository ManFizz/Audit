﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Audit.Objects;

namespace Audit.Pages;

public partial class UsersPage : Page
{
    public UsersPage()
    {
        InitializeComponent();
        var app = (App) Application.Current;
        UsersGrid.ItemsSource = app.ArrUsers;
        app.LastDataGrid = UsersGrid;
    }
    

    private readonly ObservableCollection<User> _arr = new ();
    private void Search_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        _arr.Clear();
        IdSearch.Foreground = Brushes.White;
        IdWorkerSearch.Foreground = Brushes.White;
        TypeSearch.Foreground = Brushes.White;
        
        var app = (App) Application.Current;
        if (string.IsNullOrWhiteSpace(IdSearch.Text + LoginSearch.Text + PasswordSearch.Text + IdWorkerSearch.Text + TypeSearch.Text))
        {
            UsersGrid.ItemsSource = app.ArrUsers;
            return;
        }
        
        UsersGrid.ItemsSource = _arr;
        
        var id = -1;
        if (!string.IsNullOrWhiteSpace(IdSearch.Text) && !int.TryParse(IdSearch.Text, out id))
        {
            IdSearch.Foreground = Brushes.Red;
            return;
        }
        var sId = id.ToString();
        
        var idWorker = -1;
        if (!string.IsNullOrWhiteSpace(IdWorkerSearch.Text) && !int.TryParse(IdWorkerSearch.Text, out idWorker))
        {
            IdWorkerSearch.Foreground = Brushes.Red;
            return;
        }
        var sIdWorker = idWorker.ToString();
        
        var type = TypeUser.hr;
        var hasType = !string.IsNullOrWhiteSpace(TypeSearch.Text);
        if (hasType && !Enum.TryParse(TypeSearch.Text, out type))
        {
            TypeSearch.Foreground = Brushes.Red;
            return;
        }
        
        var login = LoginSearch.Text;
        var pass = PasswordSearch.Text;

        foreach (var c in app.ArrUsers)
        {
            if (id != -1 && !c.Id.ToString().Contains(sId))
                continue;
            
            if (idWorker != -1 && !c.WorkerId.ToString().Contains(sIdWorker))
                continue;

            if (!string.IsNullOrWhiteSpace(login) && !c.Login.Contains(login))
                continue;
            
            if (!string.IsNullOrWhiteSpace(pass) && !c.Password.Contains(pass))
                continue;
            
            if (hasType && c.Type != type)
                continue;
            
            _arr.Add(c);
        }

        UsersGrid.ItemsSource = _arr;
    }

    private bool _isFull;
    private void UsersGrid_OnPreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        if (e.Command != DataGrid.DeleteCommand)
            return;
        
        ((sender as DataGrid)!.SelectedItem as User)!.Remove();
        e.Handled = true;
        _isFull = false;
    }

    private bool _inEditNewItemMode;
    private User _lastItem;
    private void UsersGrid_OnAddingNewItem(object? sender, AddingNewItemEventArgs e)
    {
        if (e.NewItem != null)
            return;
        
        try
        {
            _lastItem = new User(-1, "", "", -1, TypeUser.hr);
            e.NewItem = _lastItem;
        } catch (Exception exception) {
            MessageBox.Show(exception.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            _isFull = true;
            return;
        }
        
        _inEditNewItemMode = true;
        e.NewItem = _lastItem;
        _skipNextSelect = true;
    }
    
    private bool[] _isFieldOk = {true, false, false, false, true, false};
    private bool _isInEditCell;
    
    
    private void UsersGrid_OnCellEditEnding(object? sender, DataGridCellEditEndingEventArgs e)
    {
        if (_termianteCellEdit)
        {
            _isInEditCell = false;
            return;
        }

        if (e.EditAction != DataGridEditAction.Commit)
        {
            if (_inEditNewItemMode)
            {
                for (var i = 0; i < UsersGrid.Columns.Count; i++)
                {
                    if (e.Column == UsersGrid.Columns[i])
                    {
                        _isFieldOk[i] = false;
                        break;
                    }
                }
            }
            _isInEditCell = false;
            return;
        }

        try {
            if (e.Column == UsersGrid.Columns[1])
            {
                User.CheckLogin(((TextBox) e.EditingElement).Text);
                _isFieldOk[1] = true;
            }
            else if (e.Column == UsersGrid.Columns[2])
            {
                User.CheckPassword(((TextBox) e.EditingElement).Text);
                _isFieldOk[2] = true;
            }
            else if (e.Column == UsersGrid.Columns[3])
            {
                var sWorker = ((TextBox) e.EditingElement).Text;
                if (!int.TryParse(sWorker, out var workerId))
                    throw new Exception("Ожидалось целое число");
                
                var item = (UsersGrid.SelectedItem as User)!;
                item.CheckWorkerId(workerId);
                item.WorkerId = workerId;
                
                _isFieldOk[3] = true;
            }
            else if (e.Column == UsersGrid.Columns[5])
            {
                var sType = ((ComboBox) e.EditingElement).Text;
                if(!Enum.TryParse(sType, false, out TypeUser type))
                    throw new Exception("Ожидалось целое число");
                    
                (UsersGrid.SelectedItem as User)!.CheckTypeUser(type);

                _isFieldOk[5] = true;
            }

            _isInEditCell = false;
        }
        catch (Exception exception)
        {
            e.Cancel = true;
            MessageBox.Show(exception.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
        }
    }
    
    
    private void UsersGrid_OnRowEditEnding(object? sender, DataGridRowEditEndingEventArgs e)
    {
        if (!_inEditNewItemMode) 
            return;
        
        if (_isFieldOk.Any(t => !t))
        {
            if (e.EditAction == DataGridEditAction.Commit && !_termianteCellEdit)
            {
                e.Cancel = true;
                return;
            }
            
            //_lastItem.Remove(); //never return false
        }
        else
        {
            _lastItem.Insert();
        }

        _isFieldOk = new [] {true, false, false, false, true, false};
        
        _inEditNewItemMode = false;
        _termianteCellEdit = false;
        _isInEditCell = false;
    }

    private bool _termianteCellEdit;
    private bool _skipNextSelect;
    private void UsersGrid_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
    {
        if (!_inEditNewItemMode || _skipNextSelect)
        {
            _skipNextSelect = false;
            return;
        }

        TerminateRowEdit();
    }

    public void TerminateRowEdit()
    {
        _termianteCellEdit = true;
        var cc = UsersGrid.CurrentCell;
        foreach (var col in UsersGrid.Columns)
        {
            UsersGrid.CurrentCell = new DataGridCellInfo(_lastItem, col);
            UsersGrid.CancelEdit();
        }
        UsersGrid.CurrentCell = cc;
        _isInEditCell = false;
        _termianteCellEdit = false;
    }
    
    private void UsersGrid_OnPreparingCellForEdit(object? sender, DataGridPreparingCellForEditEventArgs e)
    {
        if (_isInEditCell)
            UsersGrid.CancelEdit();
        _isInEditCell = true;
        
        if (e.Column == UsersGrid.Columns[3])
        {
            RightFrame.Source = new Uri("WorkersPage.xaml", UriKind.Relative);
            BtnWorker.Visibility = Visibility.Visible;
        }
        else
        {
            RightFrame.Source = null;
            BtnWorker.Visibility = Visibility.Collapsed;
        }
    }

    private void UsersGrid_OnInitializingNewItem(object sender, InitializingNewItemEventArgs e)
    {
        if (!_isFull)
            return;
        
        var app = (App) Application.Current;
        app.ArrUsers.Remove((User)e.NewItem);
    }
    
    private void RightFrame_OnContentRendered(object? sender, EventArgs e)
    {
        var app = (App) Application.Current;
        var dg = app.LastDataGrid;
        foreach (var col in dg.Columns)
            col.IsReadOnly = true;
    }

    private void UsersGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        RightFrame.Source = null;
        BtnWorker.Visibility = Visibility.Collapsed;
    }

    private void BtnWorker_OnClick(object sender, RoutedEventArgs e)
    {
        var app = (App) Application.Current;
        var dg = app.LastDataGrid;
        var selectedItem = dg.SelectedItem;
        if (selectedItem == null)
        {
            MessageBox.Show("Для изменения, требуется выбрать запись", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            return;
        }
        ((User) UsersGrid.SelectedItem).WorkerId = ((Worker) selectedItem).Id;
    }
}