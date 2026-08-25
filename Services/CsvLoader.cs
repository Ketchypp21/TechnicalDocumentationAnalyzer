using System.Data;
using System.Text;
using Microsoft.VisualBasic.FileIO;

namespace TechnicalDocumentationAnalyzer.Services;

public static class CsvLoader
{
    private static readonly char[] PossibleDelimiters =
    {
        ';',
        ',',
        '\t'
    };

    public static DataTable Load(
        string filePath,
        Encoding? selectedEncoding = null)
    {
        bool automaticEncoding = selectedEncoding is null;

        Encoding encoding =
            selectedEncoding ?? DetectEncoding(filePath);

        char delimiter = DetectDelimiter(
            filePath,
            encoding,
            automaticEncoding);

        using var parser = new TextFieldParser(
            filePath,
            encoding,
            detectEncoding: automaticEncoding);

        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters(delimiter.ToString());
        parser.HasFieldsEnclosedInQuotes = true;
        parser.TrimWhiteSpace = true;

        if (parser.EndOfData)
            throw new InvalidDataException(
                "Выбранный CSV-файл пуст.");

        string[] headers = parser.ReadFields()
            ?? throw new InvalidDataException(
                "Не удалось прочитать заголовки CSV.");

        var table = new DataTable();

        var usedColumnNames = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < headers.Length; i++)
        {
            string columnName = CreateUniqueColumnName(
                headers[i],
                i,
                usedColumnNames);

            table.Columns.Add(columnName, typeof(string));
        }

        while (!parser.EndOfData)
        {
            string[]? fields;

            try
            {
                fields = parser.ReadFields();
            }
            catch (MalformedLineException exception)
            {
                throw new InvalidDataException(
                    $"Ошибка формата CSV в строке " +
                    $"{exception.Message}.",
                    exception);
            }

            if (fields is null ||
                fields.All(string.IsNullOrWhiteSpace))
            {
                continue;
            }

            if (fields.Length > table.Columns.Count)
            {
                throw new InvalidDataException(
                    $"В строке обнаружено {fields.Length} значений, " +
                    $"но заголовков только {table.Columns.Count}.");
            }

            DataRow row = table.NewRow();

            for (int i = 0; i < table.Columns.Count; i++)
            {
                row[i] = i < fields.Length
                    ? fields[i]
                    : string.Empty;
            }

            table.Rows.Add(row);
        }

        return table;
    }

    private static Encoding DetectEncoding(string filePath)
    {
        byte[] sample = ReadFileSample(filePath);

        if (HasPrefix(sample, 0x00, 0x00, 0xFE, 0xFF))
            return new UTF32Encoding(
                bigEndian: true,
                byteOrderMark: true);

        if (HasPrefix(sample, 0xFF, 0xFE, 0x00, 0x00))
            return Encoding.UTF32;

        if (HasPrefix(sample, 0xEF, 0xBB, 0xBF))
            return new UTF8Encoding(
                encoderShouldEmitUTF8Identifier: true);

        if (HasPrefix(sample, 0xFF, 0xFE))
            return Encoding.Unicode;

        if (HasPrefix(sample, 0xFE, 0xFF))
            return Encoding.BigEndianUnicode;

        if (IsValidUtf8(sample))
            return new UTF8Encoding(
                encoderShouldEmitUTF8Identifier: false);

        // Наиболее распространённая кодировка
        // старых русскоязычных SQL-выгрузок.
        return Encoding.GetEncoding(1251);
    }

    private static byte[] ReadFileSample(string filePath)
    {
        const int maximumSampleSize = 65_536;

        using var stream = File.OpenRead(filePath);

        int sampleSize = (int)Math.Min(
            stream.Length,
            maximumSampleSize);

        var sample = new byte[sampleSize];

        int totalRead = 0;

        while (totalRead < sample.Length)
        {
            int read = stream.Read(
                sample,
                totalRead,
                sample.Length - totalRead);

            if (read == 0)
                break;

            totalRead += read;
        }

        if (totalRead == sample.Length)
            return sample;

        return sample[..totalRead];
    }

    private static bool IsValidUtf8(byte[] bytes)
    {
        try
        {
            var strictUtf8 = new UTF8Encoding(
                encoderShouldEmitUTF8Identifier: false,
                throwOnInvalidBytes: true);

            strictUtf8.GetString(bytes);
            return true;
        }
        catch (DecoderFallbackException)
        {
            return false;
        }
    }

    private static bool HasPrefix(
        byte[] bytes,
        params byte[] prefix)
    {
        if (bytes.Length < prefix.Length)
            return false;

        for (int i = 0; i < prefix.Length; i++)
        {
            if (bytes[i] != prefix[i])
                return false;
        }

        return true;
    }

    private static char DetectDelimiter(
        string filePath,
        Encoding encoding,
        bool detectBom)
    {
        using var reader = new StreamReader(
            filePath,
            encoding,
            detectEncodingFromByteOrderMarks: detectBom);

        string? firstLine;

        do
        {
            firstLine = reader.ReadLine();
        }
        while (firstLine is not null &&
               string.IsNullOrWhiteSpace(firstLine));

        if (firstLine is null)
            throw new InvalidDataException(
                "Выбранный CSV-файл пуст.");

        char selectedDelimiter = PossibleDelimiters
            .OrderByDescending(delimiter =>
                CountDelimiterOutsideQuotes(
                    firstLine,
                    delimiter))
            .First();

        int delimiterCount =
            CountDelimiterOutsideQuotes(
                firstLine,
                selectedDelimiter);

        if (delimiterCount == 0)
        {
            throw new InvalidDataException(
                "Не удалось определить разделитель CSV.");
        }

        return selectedDelimiter;
    }

    private static int CountDelimiterOutsideQuotes(
        string line,
        char delimiter)
    {
        bool insideQuotes = false;
        int count = 0;

        foreach (char character in line)
        {
            if (character == '"')
            {
                insideQuotes = !insideQuotes;
            }
            else if (character == delimiter &&
                     !insideQuotes)
            {
                count++;
            }
        }

        return count;
    }

    private static string CreateUniqueColumnName(
        string originalName,
        int columnIndex,
        HashSet<string> usedNames)
    {
        string baseName = string.IsNullOrWhiteSpace(originalName)
            ? $"Колонка {columnIndex + 1}"
            : originalName.Trim();

        string result = baseName;
        int duplicateNumber = 2;

        while (!usedNames.Add(result))
        {
            result = $"{baseName} ({duplicateNumber})";
            duplicateNumber++;
        }

        return result;
    }
}