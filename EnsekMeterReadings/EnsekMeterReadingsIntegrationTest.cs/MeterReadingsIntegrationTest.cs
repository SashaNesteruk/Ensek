using AutoFixture;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using EnsekMeterReadings.Models;

namespace EnsekMeterReadingsIntegrationTest.cs
{
    public class MeterReadingsIntegrationTest : IClassFixture<TestFixture>
    {
        private readonly Fixture _fixture;
        private readonly HttpClient _client;

        public MeterReadingsIntegrationTest(TestFixture testFixture)
        {
            _client = testFixture.Client;
            _fixture = testFixture.Fixture;
        }

        [Fact]
        public async Task UploadMeterReagings_GivenValidReadingsInFile()
        {
            // Arrange
            var file = File.ReadAllBytes(@".\DataFiles\Meter_Reading_Valid.csv");
            var byteArrayContent = new ByteArrayContent(file);

            var multipartContent = new MultipartFormDataContent();
            multipartContent.Add(byteArrayContent, "csvFile", "filename");

            // Act
            var response = await _client.PostAsync("/meter-reading-uploads", multipartContent);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var responseAsString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<int>(responseAsString);

            Assert.Equal(0, result);

        }

        [Fact]
        public async Task UploadMeterReagings_GivenCorruptedReadingsFile()
        {
            // Arrange
            var file = File.ReadAllBytes(@".\DataFiles\Meter_Reading_Corrupted.csv");
            var byteArrayContent = new ByteArrayContent(file);

            var multipartContent = new MultipartFormDataContent();
            multipartContent.Add(byteArrayContent, "csvFile", "filename");

            // Act
            var response = await _client.PostAsync("/meter-reading-uploads", multipartContent);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UploadMeterReagings_GivenDuplicatesInFile()
        {
            // Arrange
            var file = File.ReadAllBytes(@".\DataFiles\Meter_Reading_Duplicate.csv");
            var byteArrayContent = new ByteArrayContent(file);

            var multipartContent = new MultipartFormDataContent();
            multipartContent.Add(byteArrayContent, "csvFile", "filename");

            // Act
            var response = await _client.PostAsync("/meter-reading-uploads", multipartContent);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var responseAsString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<int>(responseAsString);

            Assert.Equal(1, result);
        }

        [Fact]
        public async Task UploadMeterReagings_GivenOneFaultyReadingInFile()
        {
            // Arrange
            var file = File.ReadAllBytes(@".\DataFiles\Meter_Reading_Invalid.csv");
            var byteArrayContent = new ByteArrayContent(file);

            var multipartContent = new MultipartFormDataContent();
            multipartContent.Add(byteArrayContent, "csvFile", "filename");

            // Act
            var response = await _client.PostAsync("/meter-reading-uploads", multipartContent);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var responseAsString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<int>(responseAsString);

            Assert.Equal(1, result);
        }
    }
}
