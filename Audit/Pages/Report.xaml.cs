using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using MySqlX.XDevAPI.Relational;

namespace Audit.Pages;

public partial class Report : Page
{
    public Report()
    {
        InitializeComponent();

        var megaTotalH = 0;
        var megaTotalP = 0;
        var app = (App) Application.Current;
        foreach (var company in app.ArrCompany)
        {
            var group = new TableRowGroup();
            var arr = app.ArrHoursRecords.Where(hr => hr.CompanyId == company.Id).ToList().OrderBy(t => t.Date).ToList();
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
            newRow.Cells.Add(new TableCell(
                new Paragraph() {
                Inlines = { "Итог по компании:" }
                }) {
                ColumnSpan = 6,
                BorderThickness = new Thickness(0,0,0,0)
            });
            group.Rows.Add(newRow);
            newRow = new TableRow();
            
            var paragraph = new Paragraph();
            paragraph.Inlines.Add(company.Name);
            newRow.Cells.Add(new TableCell(paragraph) {
                ColumnSpan = 4,
                BorderBrush = Brushes.DarkGray,
                BorderThickness = new Thickness(0, 0, 0, 3)
            });
            
            paragraph = new Paragraph();
            paragraph.Inlines.Add(totalHours.ToString());
            newRow.Cells.Add(new TableCell(paragraph) {
                BorderBrush = Brushes.DarkGray,
                BorderThickness = new Thickness(0, 0, 0, 3)
            });
                
            paragraph = new Paragraph();
            paragraph.Inlines.Add(totalPayment.ToString());
            newRow.Cells.Add(new TableCell(paragraph) {
                BorderBrush = Brushes.DarkGray,
                BorderThickness = new Thickness(0, 0, 0, 3)
            });

            
            group.Rows.Add(newRow);
            Table.RowGroups.Add(group);
            megaTotalH += totalHours;
            megaTotalP += totalPayment;
        }

        var resultRow = new TableRow();
            
        var pr = new Paragraph();
        pr.Inlines.Add("Итого");
        resultRow.Cells.Add(new TableCell(pr) {
            ColumnSpan = 4,
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(0, 0, 0, 3)
        });
        
        pr = new Paragraph();
        pr.Inlines.Add(megaTotalH.ToString());
        resultRow.Cells.Add(new TableCell(pr) {
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(0, 0, 0, 3)
        });
        
        pr = new Paragraph();
        pr.Inlines.Add(megaTotalP.ToString());
        resultRow.Cells.Add(new TableCell(pr) {
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(0, 0, 0, 3)
        });
        var gp = new TableRowGroup();
        gp.Rows.Add(resultRow);
        Table.RowGroups.Add(gp);
    }
}