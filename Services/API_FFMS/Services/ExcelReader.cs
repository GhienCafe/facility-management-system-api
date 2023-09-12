﻿using API_FFMS.Dtos;
using AppCore.Models;
using ClosedXML.Excel;
using MainData.Entities;

namespace API_FFMS.Services
{
    public static class ExcelReader
    {
        public static List<ImportAssetDto> AssetReader(Stream formStream)
        {
            var assets = new List<ImportAssetDto>();

            try
            {
                using (var workBook = new XLWorkbook(formStream))
                {
                    var worksheet = workBook.Worksheet(1);

                    // Read asset data starting from the second row (assuming the first row contains headers)
                    var nonEmptyDataRows = worksheet.RowsUsed().Skip(1);
                    foreach (var dataRow in nonEmptyDataRows)
                    {
                        assets.Add(new ImportAssetDto
                        {
                            AssetName = dataRow.Cell(1).Value.ToString()?.Trim(),
                            AssetCode = dataRow.Cell(2).Value.ToString()?.Trim(),
                            CategoryCode = dataRow.Cell(3).Value.ToString()?.Trim(),
                            Status = (AssetStatus)Enum.Parse(typeof(AssetStatus), dataRow.Cell(4).Value.ToString().Trim()),
                            ManufacturingYear = DateTime.Parse(dataRow.Cell(5).Value.ToString().Trim()),
                            SerialNumber = dataRow.Cell(6).Value.ToString()?.Trim(),
                            Quantity = double.Parse(dataRow.Cell(7).Value.ToString().Trim()),
                            Description = dataRow.Cell(8).Value.ToString()?.Trim()
                        });
                    }
                }

                return assets;
            }
            catch (Exception exception)
            {
                throw new ApiException(exception.Message);
            }
        }
    }

}