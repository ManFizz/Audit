using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Audit.Objects;
using MySql.Data.MySqlClient;

namespace Audit.Pages;

public partial class WorkersPage : Page
{
    public WorkersPage()
    {
        InitializeComponent();
        var app = (App) Application.Current;
        WorkersGrid.ItemsSource = app.ArrWorkers;
        app.LastDataGrid = WorkersGrid;
        
        if (app.ActiveUser.Type is not (TypeUser.hr or TypeUser.admin))
        {
            var gridSearch = (IdSearch.Parent as Grid)!;
            WorkersGrid.CanUserAddRows = false;
            WorkersGrid.Columns[1].IsReadOnly = true;
            WorkersGrid.Columns[2].IsReadOnly = true;
            WorkersGrid.Columns[3].IsReadOnly = true;
            WorkersGrid.Columns[4].IsReadOnly = true;
            
            IdCategortySearch.Visibility = Visibility.Collapsed;
            gridSearch.ColumnDefinitions[4].Width = new GridLength(0);
            WorkersGrid.Columns[5].Visibility = Visibility.Collapsed;
        }
    }

    private void Search_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var app = (App) Application.Current;
        var view = CollectionViewSource.GetDefaultView(app.ArrWorkers);
        view.Filter = o => {
            var worker = ((o as Worker)!);
            return (string.IsNullOrWhiteSpace(NameSearch.Text) || worker.Name.Contains(NameSearch.Text))
                   && (string.IsNullOrWhiteSpace(IdSearch.Text) || worker.Id.ToString().Contains(IdSearch.Text))
                   && (string.IsNullOrWhiteSpace(PassportSearch.Text) || worker.Passport.Contains(PassportSearch.Text))
                   && (string.IsNullOrWhiteSpace(DataSearch.Text) || worker.Birthday.Contains(DataSearch.Text))
                   && (string.IsNullOrWhiteSpace(IdCategortySearch.Text) || worker.CategoryId.ToString().Contains(IdCategortySearch.Text));
        };
    }

    private bool _isFull;
    private void WorkersGrid_OnPreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        if (e.Command != DataGrid.DeleteCommand)
            return;
        
        ((sender as DataGrid)!.SelectedItem as Worker)!.Remove();
        e.Handled = true;
        _isFull = false;
    }

    private bool _inEditNewItemMode;
    private Worker _lastItem;
    private bool _skipNextSelect;
    private void WorkersGrid_OnAddingNewItem(object? sender, AddingNewItemEventArgs e)
    {
        if (e.NewItem != null)
            return;

        try
        {
            _lastItem = new Worker(-1, "", "", DateTime.UtcNow.Date.ToString("dd.MM.yyyy"), "", -1);
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
    
    private static readonly bool[] FieldInit = {true, false, false, false, false, false};
    private bool[] _isFieldOk = (FieldInit.Clone() as bool[])!;
    private bool _isInEditCell;
    private void WorkersGrid_OnCellEditEnding(object? sender, DataGridCellEditEndingEventArgs e)
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
                for (var i = 0; i < WorkersGrid.Columns.Count; i++)
                {
                    if (e.Column == WorkersGrid.Columns[i])
                    {
                        _isFieldOk[i] = FieldInit[i];
                        break;
                    }
                }
            }
            _isInEditCell = false;
            return;
        }

        try {
            if (e.Column == WorkersGrid.Columns[1])
            {
                Worker.CheckName(((TextBox) e.EditingElement).Text);
                _isFieldOk[1] = true;
            }
            else if (e.Column == WorkersGrid.Columns[2])
            {
                Worker.CheckPassport(((TextBox) e.EditingElement).Text);
                _isFieldOk[2] = true;
            }
            else if (e.Column == WorkersGrid.Columns[3])
            {
                Worker.CheckBirthday(((TextBox) e.EditingElement).Text);
                _isFieldOk[3] = true;
            }
            else if (e.Column == WorkersGrid.Columns[4])
            {
                Worker.CheckPhoneNumber(((TextBox) e.EditingElement).Text);
                _isFieldOk[4] = true;
            }
            else if (e.Column == WorkersGrid.Columns[5])
            {
                var sCategory = ((TextBox) e.EditingElement).Text;
                if (!int.TryParse(sCategory, out var categoryId))
                    throw new Exception("Ожидалось целое число");
                
                var item = (WorkersGrid.SelectedItem as Worker)!;
                Worker.CheckCategoryId(categoryId);
                item.CategoryId = categoryId;

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
    
    
    private void WorkersGrid_OnRowEditEnding(object? sender, DataGridRowEditEndingEventArgs e)
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
        }
        else
        {
            _lastItem.Insert();
        }

        _isFieldOk = (FieldInit.Clone() as bool[])!;
        
        _inEditNewItemMode = false;
        _termianteCellEdit = false;
        _isInEditCell = false;
    }

    private bool _termianteCellEdit;
    private void WorkersGrid_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
    {
        if (!_inEditNewItemMode || _skipNextSelect)
        {
            _skipNextSelect = false;
            return;
        }

        TerminateEdit();
    }

    private void TerminateEdit()
    {
        _termianteCellEdit = true;
        var cc = WorkersGrid.CurrentCell;
        foreach (var col in WorkersGrid.Columns)
        {
            WorkersGrid.CurrentCell = new DataGridCellInfo(_lastItem, col);
            WorkersGrid.CancelEdit();
        }
        WorkersGrid.CurrentCell = cc;
        _isInEditCell = false;
    }

    private void WorkersGrid_OnPreparingCellForEdit(object? sender, DataGridPreparingCellForEditEventArgs e)
    {
        if (_isInEditCell)
            WorkersGrid.CancelEdit();
        _isInEditCell = true;
        
        if (e.Column == WorkersGrid.Columns[5])
        {
            RightFrame.Source = new Uri("CategoriesPage.xaml", UriKind.Relative);
            BtnCategory.Visibility = Visibility.Visible;
        }
        else
        {
            RightFrame.Source = null;
            BtnCategory.Visibility = Visibility.Collapsed;
        }
    }
    
    private void WorkersGrid_OnInitializingNewItem(object sender, InitializingNewItemEventArgs e)
    {
        if (!_isFull)
            return;
        
        var app = (App) Application.Current;
        app.ArrWorkers.Remove((Worker)e.NewItem);
    }

    private void RightFrame_OnContentRendered(object? sender, EventArgs e)
    {
        var app = (App) Application.Current;
        var dg = app.LastDataGrid;
        foreach (var col in dg.Columns)
            col.IsReadOnly = true;
    }

    private void WorkersGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        RightFrame.Source = null;
        BtnCategory.Visibility = Visibility.Collapsed;
    }

    private void BtnCategory_OnClick(object sender, RoutedEventArgs e)
    {
        var app = (App) Application.Current;
        var dg = app.LastDataGrid;
        var selectedItem = dg.SelectedItem;
        if (selectedItem == null)
        {
            MessageBox.Show("Для изменения, требуется выбрать запись", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
            return;
        }
            
        ((Worker) WorkersGrid.SelectedItem).CategoryId = ((Category) selectedItem).Id;
        _isFieldOk[5] = true;
    }

    private void WorkersGrid_OnUnloaded(object sender, RoutedEventArgs e)
    {
        TerminateEdit();
    }
}