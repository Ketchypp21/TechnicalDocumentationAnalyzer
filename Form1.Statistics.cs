using System.Data;
using System.Drawing.Drawing2D;

namespace TechnicalDocumentationAnalyzer;

public partial class Form1
{
    private readonly List<StatisticsItem> _statisticsItems = [];

    private void InitializeStatistics()
    {
        statisticsColumnComboBox.SelectedIndexChanged +=
            (_, _) => UpdateStatistics();

        statisticsChartPanel.Paint +=
            StatisticsChartPanel_Paint;

        statisticsChartPanel.Resize +=
            (_, _) => statisticsChartPanel.Invalidate();
    }

    private void FillStatisticsColumnList()
    {
        statisticsColumnComboBox.Items.Clear();

        if (_sourceTable is null)
        {
            ResetStatistics();
            return;
        }

        foreach (DataColumn column in _sourceTable.Columns)
        {
            statisticsColumnComboBox.Items.Add(
                column.ColumnName);
        }

        if (statisticsColumnComboBox.Items.Count > 0)
        {
            statisticsColumnComboBox.SelectedIndex = 0;
        }
        else
        {
            ResetStatistics();
        }
    }

    private void UpdateStatistics()
    {
        UpdateStatisticsFilterLabel();

        if (_sourceTable is null ||
            statisticsColumnComboBox.SelectedItem is null)
        {
            ResetStatistics();
            return;
        }

        string columnName =
            statisticsColumnComboBox.SelectedItem.ToString()!;

        List<string> values = _sourceTable.DefaultView
            .Cast<DataRowView>()
            .Select(row =>
                Convert.ToString(row[columnName])?.Trim()
                ?? string.Empty)
            .ToList();

        int totalCount = values.Count;

        int emptyCount = values.Count(
            string.IsNullOrWhiteSpace);

        int uniqueCount = values
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.CurrentCultureIgnoreCase)
            .Count();

        _statisticsItems.Clear();

        _statisticsItems.AddRange(
            values
                .GroupBy(
                    value => string.IsNullOrWhiteSpace(value)
                        ? "(пусто)"
                        : value,
                    StringComparer.CurrentCultureIgnoreCase)
                .Select(group => new StatisticsItem
                {
                    Value = group.Key,
                    Count = group.Count(),
                    Share = totalCount == 0
                        ? 0
                        : (double)group.Count() / totalCount
                })
                .OrderByDescending(item => item.Count)
                .ThenBy(item => item.Value));

        var statisticsTable = new DataTable();

        statisticsTable.Columns.Add(
            "Значение",
            typeof(string));

        statisticsTable.Columns.Add(
            "Количество",
            typeof(int));

        statisticsTable.Columns.Add(
            "Доля",
            typeof(string));

        foreach (StatisticsItem item in _statisticsItems)
        {
            statisticsTable.Rows.Add(
                item.Value,
                item.Count,
                item.Share.ToString("P1"));
        }

        statisticsGrid.DataSource = statisticsTable;

        statisticsTotalLabel.Text =
            $"Всего записей: {totalCount}";

        statisticsUniqueLabel.Text =
            $"Уникальных значений: {uniqueCount}";

        statisticsEmptyLabel.Text =
            $"Пустых значений: {emptyCount}";

        statisticsChartPanel.Invalidate();
    }

    private void UpdateStatisticsFilterLabel()
    {
        var activeFilters = new List<string>();

        string searchText =
            searchTextBox.Text.Trim();

        if (searchText.Length > 0)
        {
            activeFilters.Add(
                $"поиск «{searchText}»");
        }

        if (filterColumnComboBox.SelectedIndex > 0 &&
            filterValueComboBox.SelectedIndex > 0)
        {
            string columnName =
                filterColumnComboBox.SelectedItem?
                    .ToString()
                ?? string.Empty;

            string selectedValue =
                filterValueComboBox.SelectedItem?
                    .ToString()
                ?? string.Empty;

            activeFilters.Add(
                $"{columnName} = «{selectedValue}»");
        }

        statisticsFilterLabel.Text =
            activeFilters.Count == 0
                ? "Активный фильтр: отсутствует"
                : $"Активный фильтр: " +
                  string.Join("; ", activeFilters);
    }

    private void ResetStatistics()
    {
        _statisticsItems.Clear();
        statisticsGrid.DataSource = null;

        statisticsTotalLabel.Text = "Всего записей: 0";
        statisticsUniqueLabel.Text = "Уникальных значений: 0";
        statisticsEmptyLabel.Text = "Пустых значений: 0";
        statisticsFilterLabel.Text =
            "Активный фильтр: отсутствует";

        statisticsChartPanel.Invalidate();
    }

    private void StatisticsChartPanel_Paint(
        object? sender,
        PaintEventArgs e)
    {
        Graphics graphics = e.Graphics;

        graphics.SmoothingMode =
            SmoothingMode.AntiAlias;

        graphics.Clear(Color.White);

        if (_statisticsItems.Count == 0)
        {
            const string message =
                "Нет данных для отображения";

            SizeF messageSize =
                graphics.MeasureString(message, Font);

            graphics.DrawString(
                message,
                Font,
                Brushes.Gray,
                (statisticsChartPanel.ClientSize.Width -
                 messageSize.Width) / 2,
                (statisticsChartPanel.ClientSize.Height -
                 messageSize.Height) / 2);

            return;
        }

        List<StatisticsItem> items =
            _statisticsItems.Take(10).ToList();

        int chartWidth =
            statisticsChartPanel.ClientSize.Width;

        int chartHeight =
            statisticsChartPanel.ClientSize.Height;

        int labelWidth = Math.Min(
            220,
            Math.Max(120, chartWidth / 3));

        const int topMargin = 50;
        const int rightMargin = 100;
        const int bottomMargin = 15;

        float availableWidth = Math.Max(
            1,
            chartWidth - labelWidth - rightMargin);

        float availableHeight = Math.Max(
            1,
            chartHeight - topMargin - bottomMargin);

        float rowHeight = Math.Min(
            55,
            availableHeight / items.Count);

        float barHeight = Math.Min(
            30,
            Math.Max(10, rowHeight * 0.6f));

        int maximumCount =
            items.Max(item => item.Count);

        using var titleFont =
            new Font(Font, FontStyle.Bold);

        using var barBrush =
            new SolidBrush(Color.SteelBlue);

        using var textBrush =
            new SolidBrush(Color.FromArgb(45, 45, 45));

        using var axisPen =
            new Pen(Color.LightGray);

        using var labelFormat =
            new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            };

        using var titleFormat =
            new StringFormat
            {
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap
            };

        string analyzedColumn =
            statisticsColumnComboBox.SelectedItem?
                .ToString()
            ?? string.Empty;

        string chartTitle =
            $"Распределение по колонке " +
            $"«{analyzedColumn}» (топ-10)";

        graphics.DrawString(
            chartTitle,
            titleFont,
            textBrush,
            new RectangleF(
                10,
                15,
                Math.Max(1, chartWidth - 20),
                25),
            titleFormat);

        graphics.DrawLine(
            axisPen,
            labelWidth,
            topMargin - 5,
            labelWidth,
            topMargin + rowHeight * items.Count);

        for (int i = 0; i < items.Count; i++)
        {
            StatisticsItem item = items[i];

            float rowY =
                topMargin + i * rowHeight;

            float barY =
                rowY + (rowHeight - barHeight) / 2;

            float barWidth =
                availableWidth *
                item.Count /
                maximumCount;

            var labelRectangle = new RectangleF(
                8,
                rowY,
                labelWidth - 16,
                rowHeight);

            graphics.DrawString(
                item.Value,
                Font,
                textBrush,
                labelRectangle,
                labelFormat);

            graphics.FillRectangle(
                barBrush,
                labelWidth,
                barY,
                barWidth,
                barHeight);

            graphics.DrawString(
                $"{item.Count} ({item.Share:P1})",
                Font,
                textBrush,
                labelWidth + barWidth + 6,
                barY + 6);
        }
    }

    private sealed class StatisticsItem
    {
        public string Value { get; init; } = string.Empty;

        public int Count { get; init; }

        public double Share { get; init; }
    }
}