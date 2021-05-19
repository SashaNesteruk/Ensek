using EnsekMeterReadings.Application.Exceptions;
using EnsekMeterReadings.Application.RepositoryInterfaces;
using EnsekMeterReadings.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EnsekMeterReadings.Application.Repositories
{
    public class FileReadRepository : IFileReadRepository
    {
        public IEnumerable<UserAccount> ReadAccountsFile(string path)
        {
            try
            {
                return File.ReadAllLines(path)
                                        .Skip(1)
                                        .Select(v =>
                                        {
                                            try
                                            {
                                                string[] values = v.Split(',');
                                                return new UserAccount()
                                                {
                                                    AccountId = Convert.ToInt32(values[0]),
                                                    FirstName = Convert.ToString(values[1]),
                                                    LastName = Convert.ToString(values[2])
                                                };
                                            }
                                            catch (Exception ex)
                                            {
                                                throw new FileReadException(ex.Message);
                                            }
                                        }).ToList();
            }
            catch (Exception ex)
            {
                throw new FileReadException(ex.Message);
            }
        }

        public IEnumerable<MeterReading> ReadMeterReadingsFile([FromForm] IFormFile file)
        {
            try
            {
                var strings = new List<string>();
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    while (reader.Peek() >= 0)
                        strings.Add(reader.ReadLine());
                }

                var result = strings.Skip(1).Select(x =>
                {
                    try
                    {
                        string[] values = x.Split(',');
                        return new MeterReading()
                        {
                            RecordId = Guid.NewGuid(),
                            AccountId = Convert.ToInt32(values[0]),
                            MeterReadingDateTime = Convert.ToDateTime(values[1]),
                            MeterReadValue = Convert.ToString(values[2])
                        };
                    }
                    catch (Exception ex)
                    {
                        throw new FileReadException(ex.Message);
                    }
                });

                return result.ToList();
            }
            catch (Exception ex)
            {
                throw new FileReadException(ex.Message);
            }
        }
    }
}
