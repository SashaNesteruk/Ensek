using EnsekMeterReadings.Application.Exceptions;
using EnsekMeterReadings.Application.Repositories;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EnsekMeterReadingsUnitTest
{
    public class FileReadRepositoryTest
    {
        private readonly FileReadRepository _sut;

        public FileReadRepositoryTest()
        {
            _sut = new FileReadRepository();
        }

        [Fact]
        public void ReadUserAccountsFile_GivenValidFile()
        {
            //Arrange

            //Act
            var result = _sut.ReadAccountsFile(@".\DataFiles\Test_Accounts.csv").ToList();

            // Assert
            Assert.Equal(27, result.Count);
        }

        [Fact]
        public void ReadUserAccountsFile_GivenCorruptedFile()
        {
            //Arrange

            //Act and Assert
            Assert.Throws<FileReadException>(() => _sut.ReadAccountsFile(@".\DataFiles\Test_Accounts_Corrupted.csv"));
        }

        [Fact]
        public void ReadMeterReadingsFile_GivenValidFile()
        {
            //Arrange
            var testFilePath = @".\DataFiles\Meter_Reading.csv";
            var testFileBytes = File.ReadAllBytes(@".\DataFiles\Meter_Reading.csv");
            using (var ms = new MemoryStream(testFileBytes))
            {

                IFormFile fromFile = new FormFile(ms, 0, ms.Length,
                Path.GetFileNameWithoutExtension(testFilePath),
                Path.GetFileName(testFilePath));

                //Act 
                var result = _sut.ReadMeterReadingsFile(fromFile).ToList();

                // Assert
                Assert.Equal(35, result.Count);
            }
        }

        [Fact]
        public void ReadMeterReadingsFile_GivenCorruptedFile()
        {
            //Arrange
            var testFilePath = @".\DataFiles\Meter_Reading.csv";
            var testFileBytes = File.ReadAllBytes(@".\DataFiles\Meter_Reading_Corrupted.csv");
            using (var ms = new MemoryStream(testFileBytes))
            {

                IFormFile fromFile = new FormFile(ms, 0, ms.Length,
                Path.GetFileNameWithoutExtension(testFilePath),
                Path.GetFileName(testFilePath));

                //Act and Assert
                Assert.Throws<FileReadException>(() => _sut.ReadMeterReadingsFile(fromFile));
            }
        }
    }
}
