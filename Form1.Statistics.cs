using System.Data;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace TechnicalDocumentationAnalyzer;

public partial class Form1
{
    private readonly List<StatisticsItem> _statisticsItems = [];

    private string _chartTitle = "Распределение значений";

    private string _chartMessage =
        "Нет данных для отображения";

    private void InitializeStatistics()
    {
        FillStatisticsModeList();

        statisticsColumnComboBox.SelectedIndexChanged +=
            (_, _) =>
            {
                if (!_isUpdatingInterface)
                    UpdateStatistics();
            };

        statisticsModeComboBox.SelectedIndexChanged +=
            (_, _) =>
            {
                if (!_isUpdatingInterface)
                    UpdateStatistics();
            };

        statisticsChartPanel.Paint +=
            StatisticsChartPanel_Paint;

        statisticsChartPanel.Resize +=
            (_, _) => statisticsChartPanel.Invalidate();
    }

    private void FillStatisticsModeList()
    {
        statisticsModeComboBox.Items.Clear();

        statisticsModeComboBox.Items.Add(
            new StatisticsModeItem(
                "Автоматически",
                null));

        statisticsModeComboBox.Items.Add(
            new StatisticsModeItem(
                "Категории",
                AnalysisKind.Category));

        statisticsModeComboBox.Items.Add(
            new StatisticsModeItem(
                "Даты",
                AnalysisKind.Date));

        statisticsModeComboBox.Items.Add(
            new StatisticsModeItem(
                "Числа",
                AnalysisKind.Number));

        statisticsModeComboBox.Items.Add(
            new StatisticsModeItem(
                "Текст",
                AnalysisKind.Text));

        statisticsModeComboBox.SelectedIndex = 0;
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

        List<string> filledValues = values
            .Where(value =>
                !string.IsNullOrWhiteSpace(value))
            .ToList();

        int totalCount = values.Count;
        int emptyCount = totalCount - filledValues.Count;

        int uniqueCount = filledValues
            .Distinct(
                StringComparer.CurrentCultureIgnoreCase)
            .Count();

        statisticsTotalLabel.Text =
            $"Всего записей: {totalCount}";

        statisticsUniqueLabel.Text =
            $"Уникальных значений: {uniqueCount}";

        statisticsEmptyLabel.Text =
            $"Пустых значений: {emptyCount}";

        AnalysisKind analysisKind =
            ResolveAnalysisKind(filledValues);

        _statisticsItems.Clear();

        switch (analysisKind)
        {
            case AnalysisKind.Category:
                UpdateCategoryStatistics(
                    columnName,
                    filledValues);
                break;

            case AnalysisKind.Date:
                UpdateDateStatistics(filledValues);
                break;

            case AnalysisKind.Number:
                UpdateNumberStatistics(filledValues);
                break;

            case AnalysisKind.Identifier:
                UpdateIdentifierStatistics(
                    filledValues,
                    uniqueCount);
                break;

            case AnalysisKind.Text:
                UpdateTextStatistics(filledValues);
                break;
        }

        statisticsChartPanel.Invalidate();
    }

    private AnalysisKind ResolveAnalysisKind(
        List<string> values)
    {
        if (statisticsModeComboBox.SelectedItem
            is StatisticsModeItem selectedMode &&
            selectedMode.AnalysisKind.HasValue)
        {
            return selectedMode.AnalysisKind.Value;
        }

        if (values.Count == 0)
            return AnalysisKind.Text;

        bool allDates =
            values.All(value =>
                TryParseDate(value, out _));

        if (allDates)
            return AnalysisKind.Date;

        bool allNumbers =
            values.All(value =>
                TryParseNumber(value, out _));

        if (allNumbers)
            return AnalysisKind.Number;

        int uniqueCount = values
            .Distinct(
                StringComparer.CurrentCultureIgnoreCase)
            .Count();

        double uniqueShare =
            (double)uniqueCount / values.Count;

        double averageLength =
            values.Average(value => value.Length);

        if (uniqueShare >= 0.8)
        {
            return averageLength >= 25
                ? AnalysisKind.Text
                : AnalysisKind.Identifier;
        }

        return AnalysisKind.Category;
    }

    private void UpdateCategoryStatistics(
        string columnName,
        List<string> values)
    {
        statisticsDetailsLabel.Text =
            "Тип данных: категория";

        _chartTitle =
            $"Распределение по колонке " +
            $"«{columnName}» (топ-15)";

        _chartMessage =
            "Нет заполненных значений для диаграммы";

        _statisticsItems.AddRange(
            values
                .GroupBy(
                    value => value,
                    StringComparer.CurrentCultureIgnoreCase)
                .Select(group => new StatisticsItem
                {
                    Value = group.Key,
                    Count = group.Count(),
                    Share = values.Count == 0
                        ? 0
                        : (double)group.Count() / values.Count
                })
                .OrderByDescending(item => item.Count)
                .ThenBy(item => item.Value));

        var table = new DataTable();

        table.Columns.Add(
            "Значение",
            typeof(string));

        table.Columns.Add(
            "Количество",
            typeof(int));

        table.Columns.Add(
            "Доля",
            typeof(string));

        foreach (StatisticsItem item in _statisticsItems)
        {
            table.Rows.Add(
                item.Value,
                item.Count,
                item.Share.ToString("P1"));
        }

        statisticsGrid.DataSource = table;
    }

    private void UpdateDateStatistics(
        List<string> values)
    {
        var dates = new List<DateTime>();
        int invalidCount = 0;

        foreach (string value in values)
        {
            if (TryParseDate(value, out DateTime date))
            {
                dates.Add(date.Date);
            }
            else
            {
                invalidCount++;
            }
        }

        statisticsDetailsLabel.Text =
            statisticsModeComboBox.SelectedIndex == 0
                ? "Тип данных: дата — определено автоматически"
                : "Тип данных: дата — выбран вручную";

        _chartTitle = "Анализ дат";

        _chartMessage =
            "Для дат показаны диапазон и период";

        if (dates.Count == 0)
        {
            SetSummaryTable(
                ("Корректных дат", "0"),
                ("Некорректных значений",
                    invalidCount.ToString()));

            return;
        }

        DateTime minimumDate = dates.Min();
        DateTime maximumDate = dates.Max();

        int periodDays =
            (maximumDate - minimumDate).Days;

        int uniqueDates =
            dates.Distinct().Count();

        SetSummaryTable(
            ("Корректных дат", dates.Count.ToString()),
            ("Уникальных дат", uniqueDates.ToString()),
            ("Самая ранняя",
                minimumDate.ToString("dd.MM.yyyy")),
            ("Самая поздняя",
                maximumDate.ToString("dd.MM.yyyy")),
            ("Период",
                $"{periodDays} дн."),
            ("Некорректных значений",
                invalidCount.ToString()));
    }

    private void UpdateNumberStatistics(
        List<string> values)
    {
        var numbers = new List<double>();
        int invalidCount = 0;

        foreach (string value in values)
        {
            if (TryParseNumber(value, out double number))
            {
                numbers.Add(number);
            }
            else
            {
                invalidCount++;
            }
        }

        statisticsDetailsLabel.Text =
            statisticsModeComboBox.SelectedIndex == 0
                ? "Тип данных: число — определено автоматически"
                : "Тип данных: число — выбран вручную";

        _chartTitle = "Числовая статистика";

        _chartMessage =
            "Для чисел показаны расчётные показатели";

        if (numbers.Count == 0)
        {
            SetSummaryTable(
                ("Корректных чисел", "0"),
                ("Некорректных значений",
                    invalidCount.ToString()));

            return;
        }

        numbers.Sort();

        double median;

        if (numbers.Count % 2 == 1)
        {
            median =
                numbers[numbers.Count / 2];
        }
        else
        {
            int rightIndex =
                numbers.Count / 2;

            median =
                (numbers[rightIndex - 1] +
                 numbers[rightIndex]) / 2;
        }

        SetSummaryTable(
            ("Корректных чисел",
                numbers.Count.ToString()),
            ("Минимум",
                FormatNumber(numbers.Min())),
            ("Максимум",
                FormatNumber(numbers.Max())),
            ("Среднее",
                FormatNumber(numbers.Average())),
            ("Медиана",
                FormatNumber(median)),
            ("Сумма",
                FormatNumber(numbers.Sum())),
            ("Некорректных значений",
                invalidCount.ToString()));
    }

    private void UpdateIdentifierStatistics(
        List<string> values,
        int uniqueCount)
    {
        int duplicateCount =
            values.Count - uniqueCount;

        statisticsDetailsLabel.Text =
            "Тип данных: идентификатор — " +
            "определено автоматически";

        _chartTitle = "Анализ идентификаторов";

        _chartMessage =
            "Диаграмма не построена: " +
            "значения являются уникальными";

        SetSummaryTable(
            ("Заполненных значений",
                values.Count.ToString()),
            ("Уникальных значений",
                uniqueCount.ToString()),
            ("Повторяющихся записей",
                duplicateCount.ToString()),
            ("Доля уникальных",
                values.Count == 0
                    ? "0,0 %"
                    : ((double)uniqueCount /
                       values.Count).ToString("P1")));
    }

    private void UpdateTextStatistics(
        List<string> values)
    {
        statisticsDetailsLabel.Text =
            statisticsModeComboBox.SelectedIndex == 0
                ? "Тип данных: текст — определено автоматически"
                : "Тип данных: текст — выбран вручную";

        _chartTitle = "Анализ текста";

        _chartMessage =
            "Для текста показана статистика длины";

        if (values.Count == 0)
        {
            SetSummaryTable(
                ("Заполненных значений", "0"));

            return;
        }

        List<int> lengths = values
            .Select(value => value.Length)
            .OrderBy(length => length)
            .ToList();

        double medianLength;

        if (lengths.Count % 2 == 1)
        {
            medianLength =
                lengths[lengths.Count / 2];
        }
        else
        {
            int rightIndex =
                lengths.Count / 2;

            medianLength =
                (lengths[rightIndex - 1] +
                 lengths[rightIndex]) / 2.0;
        }

        SetSummaryTable(
            ("Заполненных значений",
                values.Count.ToString()),
            ("Минимальная длина",
                $"{lengths.Min()} симв."),
            ("Максимальная длина",
                $"{lengths.Max()} симв."),
            ("Средняя длина",
                $"{lengths.Average():0.0} симв."),
            ("Медианная длина",
                $"{medianLength:0.0} симв."));
    }

    private void SetSummaryTable(
        params (string Name, string Value)[] rows)
    {
        var table = new DataTable();

        table.Columns.Add(
            "Показатель",
            typeof(string));

        table.Columns.Add(
            "Значение",
            typeof(string));

        foreach ((string name, string value) in rows)
        {
            table.Rows.Add(name, value);
        }

        statisticsGrid.DataSource = table;

        DataGridViewColumn indicatorColumn =
            statisticsGrid.Columns["Показатель"];

        DataGridViewColumn valueColumn =
            statisticsGrid.Columns["Значение"];

        indicatorColumn.DisplayIndex = 0;
        valueColumn.DisplayIndex = 1;

        // Для сводных показателей сортировка не нужна:
        // строки должны идти в заданном порядке.
        indicatorColumn.SortMode =
            DataGridViewColumnSortMode.NotSortable;

        valueColumn.SortMode =
            DataGridViewColumnSortMode.NotSortable;

        statisticsGrid.ClearSelection();
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

        statisticsTotalLabel.Text =
            "Всего записей: 0";

        statisticsUniqueLabel.Text =
            "Уникальных значений: 0";

        statisticsEmptyLabel.Text =
            "Пустых значений: 0";

        statisticsFilterLabel.Text =
            "Активный фильтр: отсутствует";

        statisticsDetailsLabel.Text =
            "Тип данных: —";

        _chartTitle =
            "Распределение значений";

        _chartMessage =
            "Нет данных для отображения";

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
            SizeF messageSize =
                graphics.MeasureString(
                    _chartMessage,
                    Font);

            graphics.DrawString(
                _chartMessage,
                Font,
                Brushes.Gray,
                (statisticsChartPanel.ClientSize.Width -
                 messageSize.Width) / 2,
                (statisticsChartPanel.ClientSize.Height -
                 messageSize.Height) / 2);

            return;
        }

        List<StatisticsItem> items =
            _statisticsItems.Take(15).ToList();

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
            new SolidBrush(
                Color.FromArgb(45, 45, 45));

        using var axisPen =
            new Pen(Color.LightGray);

        using var labelFormat =
            new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming =
                    StringTrimming.EllipsisCharacter
            };

        using var titleFormat =
            new StringFormat
            {
                Trimming =
                    StringTrimming.EllipsisCharacter,
                FormatFlags =
                    StringFormatFlags.NoWrap
            };

        graphics.DrawString(
            _chartTitle,
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
                rowY +
                (rowHeight - barHeight) / 2;

            float barWidth =
                availableWidth *
                item.Count /
                maximumCount;

            var labelRectangle =
                new RectangleF(
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

    private static bool TryParseDate(
        string value,
        out DateTime result)
    {
        result = default;

        bool hasDateSeparator =
            value.Contains('-') ||
            value.Contains('.') ||
            value.Contains('/');

        if (!hasDateSeparator)
            return false;

        return DateTime.TryParse(
                   value,
                   CultureInfo.CurrentCulture,
                   DateTimeStyles.AllowWhiteSpaces,
                   out result)
               ||
               DateTime.TryParse(
                   value,
                   CultureInfo.InvariantCulture,
                   DateTimeStyles.AllowWhiteSpaces,
                   out result);
    }

    private static bool TryParseNumber(
        string value,
        out double result)
    {
        return double.TryParse(
                   value,
                   NumberStyles.Any,
                   CultureInfo.CurrentCulture,
                   out result)
               ||
               double.TryParse(
                   value,
                   NumberStyles.Any,
                   CultureInfo.InvariantCulture,
                   out result);
    }

    private static string FormatNumber(
        double number)
    {
        return number.ToString("0.##");
    }

    private enum AnalysisKind
    {
        Category,
        Date,
        Number,
        Identifier,
        Text
    }

    private sealed class StatisticsModeItem
    {
        public string DisplayName { get; }

        public AnalysisKind? AnalysisKind { get; }

        public StatisticsModeItem(
            string displayName,
            AnalysisKind? analysisKind)
        {
            DisplayName = displayName;
            AnalysisKind = analysisKind;
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }

    private sealed class StatisticsItem
    {
        public string Value { get; init; } =
            string.Empty;

        public int Count { get; init; }

        public double Share { get; init; }
    }
}