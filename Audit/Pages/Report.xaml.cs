using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Audit.Pages;

public partial class Report : Page
{
    public Report()
    {
        InitializeComponent();

        var app = (App) Application.Current;
        Selector.ItemsSource = app.AvailableYears;
        RedrawTable();
        Selector.SelectedItem = app.AvailableYears[0];
    }

    public void RedrawTable(string? sYear = null)
    {
        var app = (App) Application.Current;
        while(Table.RowGroups.Count > 1)
            Table.RowGroups.Remove(Table.RowGroups[1]);

        if (string.IsNullOrEmpty(sYear))
            sYear = app.AvailableYears[0].ToString();
        var megaTotalH = 0;
        var megaTotalP = 0;
        var color = new SolidColorBrush(Color.FromRgb(0xff, 0xab, 0x40));
        foreach (var company in app.ArrCompany)
        {
            var group = new TableRowGroup();
            var arr = app.ArrHoursRecords.Where(hr => hr.CompanyId == company.Id && hr.Date.Contains(sYear)).ToList().OrderBy(t => t.Date).ToList();
            var totalHours = 0;
            var totalPayment = 0;
            foreach (var hr in arr)
            {
                var row = new TableRow();
                
                var p = new Paragraph();
                p.Inlines.Add(new Run(hr.WorkerName));
                row.Cells.Add(new TableCell(p));

                var category = app.ArrCategories.First(c => c.Id == app.ArrWorkers.First(w => w.Id == hr.WorkerId).CategoryId);
                p = new Paragraph();
                p.Inlines.Add(new Run(category.Name));
                row.Cells.Add(new TableCell(p));
                
                p = new Paragraph();
                p.Inlines.Add(new Run(category.Payment.ToString()));
                row.Cells.Add(new TableCell(p));
                
                p = new Paragraph();
                p.Inlines.Add(new Run(hr.Date));
                row.Cells.Add(new TableCell(p));
                
                p = new Paragraph();
                p.Inlines.Add(new Run(hr.Hours.ToString()));
                row.Cells.Add(new TableCell(p));
                
                p = new Paragraph();
                p.Inlines.Add(new Run((hr.Hours*category.Payment).ToString()));
                row.Cells.Add(new TableCell(p));
                
                totalHours += hr.Hours;
                totalPayment += hr.Hours * category.Payment;
                
                group.Rows.Add(row);
            }

            if (group.Rows.Count == 0)
                continue;
            
            var newRow = new TableRow();
            
            var paragraph = new Paragraph();
            paragraph.Inlines.Add("Итог по компании:   " + company.Name);
            paragraph.Background = color;
            newRow.Cells.Add(new TableCell(paragraph) {
                ColumnSpan = 4,
                BorderBrush = Brushes.DarkGray,
                BorderThickness = new Thickness(0, 0, 0, 3),
                Foreground = Brushes.Black,
                Background = color
            });
            
            paragraph = new Paragraph();
            paragraph.Background = color;
            paragraph.Inlines.Add(totalHours.ToString());
            newRow.Cells.Add(new TableCell(paragraph) {
                BorderBrush = Brushes.DarkGray,
                BorderThickness = new Thickness(0, 0, 0, 3),
                Foreground = Brushes.Black,
                Background = color
            });
                
            paragraph = new Paragraph();
            paragraph.Background = color;
            paragraph.Inlines.Add(totalPayment.ToString());
            newRow.Cells.Add(new TableCell(paragraph) {
                BorderBrush = Brushes.DarkGray,
                BorderThickness = new Thickness(0, 0, 0, 3),
                Foreground = Brushes.Black,
                Background = color
            });

            
            group.Rows.Add(newRow);
            Table.RowGroups.Add(group);
            megaTotalH += totalHours;
            megaTotalP += totalPayment;
        }

        var resultRow = new TableRow();
            
        var pr = new Paragraph();
        pr.Background = color;
        pr.Inlines.Add("Итого");
        resultRow.Cells.Add(new TableCell(pr) {
            ColumnSpan = 4,
            BorderBrush = Brushes.Black,
            Foreground = Brushes.Black,
            BorderThickness = new Thickness(0, 0, 0, 3),
            Background = color
        });
        
        pr = new Paragraph();
        pr.Background = color;
        pr.Inlines.Add(megaTotalH.ToString());
        resultRow.Cells.Add(new TableCell(pr) {
            BorderBrush = Brushes.Black,
            Foreground = Brushes.Black,
            BorderThickness = new Thickness(0, 0, 0, 3),
            Background = color
        });
        
        pr = new Paragraph();
        pr.Background = color;
        pr.Inlines.Add(megaTotalP.ToString());
        resultRow.Cells.Add(new TableCell(pr) {
            BorderBrush = Brushes.Black,
            Foreground = Brushes.Black,
            BorderThickness = new Thickness(0, 0, 0, 3),
            Background = color
        });
        var gp = new TableRowGroup();
        gp.Rows.Add(resultRow);
        Table.RowGroups.Add(gp);
    }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        RedrawTable(e.AddedItems[0]!.ToString());
    }
}