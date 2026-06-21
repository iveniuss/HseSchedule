using System.Globalization;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using HtmlAgilityPack;
using NPOI.SS.Util;

public class ExcelDownloader
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ExcelDownloader(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<List<IWorkbook>> DownloadWorkbook()
    {
        var baseUrl = _configuration["ScheduleUrl"];
        if (string.IsNullOrEmpty(baseUrl))
            throw new InvalidOperationException("ScheduleUrl is not set in configuration");

        var urls = await GetDownloadUrlAsync(baseUrl);
        var workbooks = new List<IWorkbook>();
        foreach (var url in urls)
        {
            await using var stream = await _httpClient.GetStreamAsync(FormatString(url));

            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var extension = Path.GetExtension(new Uri(url).AbsolutePath).ToLowerInvariant();

            workbooks.Add(extension switch
                {
                    ".xls" => new HSSFWorkbook(memoryStream),
                    ".xlsx" => new XSSFWorkbook(memoryStream),
                    _ => throw new NotSupportedException($"Unsupported Excel format: {extension}")
                }
            );
        }
        UnmergeCells(workbooks);
        return workbooks;
    }
    
    private void UnmergeCells(List<IWorkbook> workbooks)
    {
        foreach (var workbook in workbooks)
        {
            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                var sheet = workbook.GetSheetAt(i);
                var mergedRegions = new List<CellRangeAddress>();

                for (int j = 0; j < sheet.NumMergedRegions; j++)
                {
                    mergedRegions.Add(sheet.GetMergedRegion(j));
                }

                foreach (var region in mergedRegions)
                {
                    var firstRow = sheet.GetRow(region.FirstRow);
                    var firstCell = firstRow?.GetCell(region.FirstColumn);
                    var cellValue = firstCell?.ToString();

                    for (int rowNum = region.FirstRow; rowNum <= region.LastRow; rowNum++)
                    {
                        var row = sheet.GetRow(rowNum) ?? sheet.CreateRow(rowNum);
                        for (int colNum = region.FirstColumn; colNum <= region.LastColumn; colNum++)
                        {
                            var cell = row.GetCell(colNum) ?? row.CreateCell(colNum);
                            if (!string.IsNullOrEmpty(cellValue))
                            {
                                cell.SetCellValue(cellValue);
                            }
                        }
                    }
                }

                for (int j = sheet.NumMergedRegions - 1; j >= 0; j--)
                {
                    sheet.RemoveMergedRegion(j);
                }
            }
        }
    }

    private string FormatString(string url)
    {
        if (url.Contains("//perm.hse.ru"))
        {
            url = url.Replace("//www.hse.ru", "https://perm.hse.ru");
        }
        else if (url.Contains("//www.hse.ru"))
        {
            url = url.Replace("//www.hse.ru", "https://www.hse.ru");
        }
        else if (!url.Contains("https://perm.hse.ru"))
        {
            url = "https://perm.hse.ru" + url;
        }

        return url;
    }

    private async Task<List<string>> GetDownloadUrlAsync(string url)
    {
        var html = await _httpClient.GetStringAsync(url);

        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        var anchorNodes = htmlDoc.DocumentNode.SelectNodes("//a");
        
        var urls = new List<string>();

        if (anchorNodes != null)
        {
            foreach (var anchor in anchorNodes)
            {
                var text = anchor.InnerText.Trim();
                if (text.StartsWith("Расписание занятий (") || text.StartsWith("СЕССИЯ ("))
                {
                    urls.Add( anchor.GetAttributeValue("href", string.Empty));
                }
            }
            
            return urls;
        }
        

        throw new InvalidOperationException("Excel File URL not found");
    }
}