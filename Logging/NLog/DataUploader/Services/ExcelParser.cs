using DataUploader.CascadAuthirization;
using DataUploader.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataUploader.Services
{
    public class ExcelParser : IExcelParser
    {
        private readonly ICascadClientFactory _cascadClientFactory;
        private readonly ILogger<ExcelParser> _logger;
        public ExcelParser(ICascadClientFactory cascadClientFactory, ILogger<ExcelParser> logger)
        {
            _cascadClientFactory = cascadClientFactory;
            _logger = logger;
        }

        //public async Task<List<string>> Parse(string path)
        //{
        //    try
        //    {
        //        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        //        var file = new FileInfo(path);
        //        ExcelWorksheet excelWorksheet = await GetWorksheet(file);

        //        var excelResult = await LoadExcelFile(excelWorksheet);

        //        string json = JsonSerializer.Serialize(excelResult);

        //        IRestResponse cascsdResponse = _cascadClientFactory.PostExcelFile(json);
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogTrace(e, "ExcelParser");
        //    }
         


        //    return null;
        //}

        public async Task<string> Parse(IFormFile ifromFile, int subdivisionId)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                var excelResult = await LoadExcelFile(ifromFile, subdivisionId);

                string json = JsonSerializer.Serialize(excelResult);
                _logger.LogInformation($"начинаю отправку в Каскад {excelResult.Count} Записей");
                IRestResponse cascsdResponse = _cascadClientFactory.PostExcelFile(json);
                _logger.LogInformation($"Отпрваленов Каскад {excelResult.Count} Записей");
                return "Done";
            }
            catch (Exception e)
            {
                _logger.LogTrace(e, "ExcelParser - Ошибка");
                return "Error";
            }
        }


        //private async Task<List<ImportDataDto>> LoadExcelFile(ExcelWorksheet ws)
        //{
        //    List<ImportDataDto> output = new();

        //    int row = 2;
        //    int col = 1;

        //    while (string.IsNullOrWhiteSpace(ws.Cells[row, col].Value?.ToString()) == false)
        //    {
        //        ImportDataDto importData = new()
        //        {
        //            ID_DELO = ParseInt(ws.Cells[row, ColoumnIndexes.ID].Value.ToString()),
        //            FileName = ParseString(ws.Cells[row, ColoumnIndexes.FILENAME].Value),
        //            auction = ParseString(ws.Cells[row, ColoumnIndexes.AUCTION].Value),
        //            num_doc = ParseString(ws.Cells[row, ColoumnIndexes.NUM_DOC].Value),
        //            date_doc = ParseString(ws.Cells[row, ColoumnIndexes.DATE_DOC].Value),
        //            naimem_arend = ParseString(ws.Cells[row, ColoumnIndexes.NAIMEN_AREND].Value),
        //            fio_arend = ParseString(ws.Cells[row, ColoumnIndexes.FIO_AREND].Value),
        //            inn_arend = ParseString(ws.Cells[row, ColoumnIndexes.INN_AREND].Value),
        //            dolzh_arend = ParseString(ws.Cells[row, ColoumnIndexes.DOLZH_AREND].Value),
        //            adress_arend = ParseString(ws.Cells[row, ColoumnIndexes.ADRESS_AREND].Value),
        //            ezhegod_plata_1 = ParseString(ws.Cells[row, ColoumnIndexes.EZHEGOD_PLATA].Value),
        //            ezhemes_plata = ParseString(ws.Cells[row, ColoumnIndexes.EZHEMES_PLATA].Value),
        //            percent_neyst = ParseString(ws.Cells[row, ColoumnIndexes.PERCENT_NEYST].Value),
        //            data_nach = ParseString(ws.Cells[row, ColoumnIndexes.DATA_NACH].Value),
        //            data_okonch = ParseString(ws.Cells[row, ColoumnIndexes.DATA_OKONCH].Value),
        //            srok_dog = ParseString(ws.Cells[row, ColoumnIndexes.SROK_DOG].Value),
        //            kadastr_number_1 = ParseString(ws.Cells[row, ColoumnIndexes.KADASTR_NUMBER].Value),
        //            vid_ob_prava_1 = ParseString(ws.Cells[row, ColoumnIndexes.VID_OB_PRAVA].Value),
        //            place_1 = ParseString(ws.Cells[row, ColoumnIndexes.PLACE].Value),
        //            area_1 = ParseString(ws.Cells[row, ColoumnIndexes.AREA].Value),
        //            cel_naznach_1 = ParseString(ws.Cells[row, ColoumnIndexes.CEL_NAZNACH].Value),
        //            razresh_ispolz_1 = ParseString(ws.Cells[row, ColoumnIndexes.RAZRESH_ISPOLZ].Value),
        //            pasport_FIO = ParseString(ws.Cells[row, ColoumnIndexes.PASSPORT_FIO].Value),
        //            pasport_series_number = ParseString(ws.Cells[row, ColoumnIndexes.PASSPORT_SERIES_NUMBER].Value),
        //            pasport_kemvidan = ParseString(ws.Cells[row, ColoumnIndexes.PASSPORT_KEM_VIDAN].Value),
        //            pasport_adress = ParseString(ws.Cells[row, ColoumnIndexes.PASPORT_ADDRESS].Value),
        //            pasport_birthdate = ParseString(ws.Cells[row, ColoumnIndexes.PASSPORT_BIRTH_DATE].Value),
        //            pasport_date = ParseString(ws.Cells[row, ColoumnIndexes.PASSPORT_DATE].Value)
        //        };
        //        output.Add(importData);
        //        row += 1;
        //    }

        //    return output;
        //}


        private async Task<List<ImportDataDto>> LoadExcelFile(IFormFile ifromFile, int subdivisionId)
        {
            using var fileStream = ifromFile.OpenReadStream();

            using var package = new ExcelPackage(fileStream);

            var ws = package.Workbook.Worksheets[0];

            List<ImportDataDto> output = new();

            int row = 2;
            int col = 1;

            while (string.IsNullOrWhiteSpace(ws.Cells[row, col].Value?.ToString()) == false)
            {
                ImportDataDto importData = new()
                {
                    ID_DELO = ParseInt(ws.Cells[row, ColoumnIndexes.ID].Value.ToString()),
                    FileName = ParseString(ws.Cells[row, ColoumnIndexes.FILENAME].Value),
                    auction = ParseString(ws.Cells[row, ColoumnIndexes.AUCTION].Value),
                    num_doc = ParseString(ws.Cells[row, ColoumnIndexes.NUM_DOC].Value),
                    date_doc = ParseString(ws.Cells[row, ColoumnIndexes.DATE_DOC].Value),
                    naimem_arend = ParseString(ws.Cells[row, ColoumnIndexes.NAIMEN_AREND].Value),
                    fio_arend = ParseString(ws.Cells[row, ColoumnIndexes.FIO_AREND].Value),
                    inn_arend = ParseString(ws.Cells[row, ColoumnIndexes.INN_AREND].Value),
                    dolzh_arend = ParseString(ws.Cells[row, ColoumnIndexes.DOLZH_AREND].Value),
                    adress_arend = ParseString(ws.Cells[row, ColoumnIndexes.ADRESS_AREND].Value),
                    ezhegod_plata_1 = ParseString(ws.Cells[row, ColoumnIndexes.EZHEGOD_PLATA].Value),
                    ezhemes_plata = ParseString(ws.Cells[row, ColoumnIndexes.EZHEMES_PLATA].Value),
                    percent_neyst = ParseString(ws.Cells[row, ColoumnIndexes.PERCENT_NEYST].Value),
                    data_nach = ParseString(ws.Cells[row, ColoumnIndexes.DATA_NACH].Value),
                    data_okonch = ParseString(ws.Cells[row, ColoumnIndexes.DATA_OKONCH].Value),
                    srok_dog = ParseString(ws.Cells[row, ColoumnIndexes.SROK_DOG].Value),
                    kadastr_number_1 = ParseString(ws.Cells[row, ColoumnIndexes.KADASTR_NUMBER].Value),
                    vid_ob_prava_1 = ParseString(ws.Cells[row, ColoumnIndexes.VID_OB_PRAVA].Value),
                    place_1 = ParseString(ws.Cells[row, ColoumnIndexes.PLACE].Value),
                    area_1 = ParseString(ws.Cells[row, ColoumnIndexes.AREA].Value),
                    cel_naznach_1 = ParseString(ws.Cells[row, ColoumnIndexes.CEL_NAZNACH].Value),
                    razresh_ispolz_1 = ParseString(ws.Cells[row, ColoumnIndexes.RAZRESH_ISPOLZ].Value),
                    pasport_FIO = ParseString(ws.Cells[row, ColoumnIndexes.PASSPORT_FIO].Value),
                    pasport_series_number = ParseString(ws.Cells[row, ColoumnIndexes.PASSPORT_SERIES_NUMBER].Value),
                    pasport_kemvidan = ParseString(ws.Cells[row, ColoumnIndexes.PASSPORT_KEM_VIDAN].Value),
                    pasport_adress = ParseString(ws.Cells[row, ColoumnIndexes.PASPORT_ADDRESS].Value),
                    pasport_birthdate = ParseString(ws.Cells[row, ColoumnIndexes.PASSPORT_BIRTH_DATE].Value),
                    pasport_date = ParseString(ws.Cells[row, ColoumnIndexes.PASSPORT_DATE].Value),
                    SubdivisionId = subdivisionId
                };
                output.Add(importData);
                row += 1;
            }

            return output;
        }

        private int ParseInt(string value)
        {
            if (int.TryParse(value, out int result))
            {
                return result;
            }
            return 0;
        }

        private string ParseString(object value)
        {
            if (value is null)
            {
                return string.Empty;
            }

            return value.ToString();
        }
    }
}
