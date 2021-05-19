using AutoFixture;
using EnsekMeterReadings.Application.Commands;
using EnsekMeterReadings.Application.RepositoryInterfaces;
using EnsekMeterReadings.Application.Validators;
using EnsekMeterReadings.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EnsekMeterReadingsUnitTest
{
    public class AddMeterReadingsCommandHandlerTest
    {
        private AddMeterReadingsCommandHandler _sut;
        private readonly Mock<IMeterReadingRepository> _meterReadingRepository;
        private readonly Mock<IUserAccountRepository> _userAccountRepository;
        private readonly Mock<IMeterReadingValidator> _meterReadingValidator;
        private readonly Mock<IFileReadRepository> _fileReadRepository;
        private readonly Mock<ILogger<AddMeterReadingsCommandHandler>> _logger;
        private readonly Mock<IFormFile> _fileMock;

        private readonly Fixture _fixture;

        public AddMeterReadingsCommandHandlerTest()
        {
            _fixture = new Fixture();

            _meterReadingRepository = new Mock<IMeterReadingRepository>();
            _userAccountRepository = new Mock<IUserAccountRepository>();
            _meterReadingValidator = new Mock<IMeterReadingValidator>();
            _fileReadRepository = new Mock<IFileReadRepository>();
            _logger = new Mock<ILogger<AddMeterReadingsCommandHandler>>();

            _fileMock = new Mock<IFormFile>();
            _fileMock.Setup(f => f.Length).Returns(4000);
            _fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

            _sut = new AddMeterReadingsCommandHandler(_meterReadingRepository.Object, _userAccountRepository.Object, _meterReadingValidator.Object, _fileReadRepository.Object, _logger.Object);
        }

        [Fact]
        public async void AddMeterReadingIntoDB_GivenValidReading()
        {
            // Arrange
            var givenReading = _fixture.Build<MeterReading>().With(x => x.MeterReadValue, "12345").Create();
            var userAccount = _fixture.Build<UserAccount>().Create();
            var readings = new List<MeterReading>() { givenReading };
            var duplicates = _fixture.Build<MeterReading>().CreateMany(0);

            var command = new AddMeterReadingsCommand(_fileMock.Object);
            _fileReadRepository.Setup(x => x.ReadMeterReadingsFile(It.IsAny<IFormFile>())).Returns(readings);
            _meterReadingValidator.Setup(x => x.IsValidMeterReading(It.IsAny<MeterReading>())).Returns(true);
            _userAccountRepository.Setup(x => x.GetUserAccountById(It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(userAccount));
            _meterReadingRepository.Setup(x => x.GetDuplicateMeterReadings(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                   .Returns(Task.FromResult(duplicates.ToList()));

            // Act
            var result = await _sut.Handle(command, CancellationToken.None);

            // Assert 
            Assert.Empty(result);
            _meterReadingRepository.Verify(r => r.AddMeterReading(It.IsAny<MeterReading>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void AddMeterReadingIntoDB_GivenInValidReading()
        {
            // Arrange
            var givenReading = _fixture.Build<MeterReading>().With(x => x.MeterReadValue, "VOID").Create();
            var userAccount = _fixture.Build<UserAccount>().Create();
            var readings = new List<MeterReading>() { givenReading };
            var duplicates = _fixture.Build<MeterReading>().CreateMany(0);

            var command = new AddMeterReadingsCommand(_fileMock.Object);
            _fileReadRepository.Setup(x => x.ReadMeterReadingsFile(It.IsAny<IFormFile>())).Returns(readings);
            _meterReadingValidator.Setup(x => x.IsValidMeterReading(It.IsAny<MeterReading>())).Returns(false);
            _userAccountRepository.Setup(x => x.GetUserAccountById(It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(userAccount));
            _meterReadingRepository.Setup(x => x.GetDuplicateMeterReadings(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                   .Returns(Task.FromResult(duplicates.ToList()));

            // Act
            var result = await _sut.Handle(command, CancellationToken.None);

            // Assert 
            Assert.Single(result);
            _meterReadingRepository.Verify(r => r.AddMeterReading(It.IsAny<MeterReading>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async void AddMeterReadingIntoDB_GivenDuplicateReading()
        {
            // Arrange
            var givenReading = _fixture.Build<MeterReading>().With(x => x.MeterReadValue, "12345").Create();
            var userAccount = _fixture.Build<UserAccount>().Create();
            var readings = new List<MeterReading>() { givenReading };
            var duplicates = _fixture.Build<MeterReading>().With(x => x.AccountId, givenReading.AccountId)
                                                           .With(x => x.MeterReadingDateTime, givenReading.MeterReadingDateTime)
                                                           .With(x => x.MeterReadValue, givenReading.MeterReadValue)
                                                           .CreateMany(1);

            var command = new AddMeterReadingsCommand(_fileMock.Object);
            _fileReadRepository.Setup(x => x.ReadMeterReadingsFile(It.IsAny<IFormFile>())).Returns(readings);
            _userAccountRepository.Setup(x => x.GetUserAccountById(It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(userAccount));
            _meterReadingRepository.Setup(x => x.GetDuplicateMeterReadings(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                   .Returns(Task.FromResult(duplicates.ToList()));

            // Act
            var result = await _sut.Handle(command, CancellationToken.None);

            // Assert 
            Assert.Single(result);
            _meterReadingRepository.Verify(r => r.AddMeterReading(It.IsAny<MeterReading>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async void AddMeterReadingIntoDB_GivenNonExistentAccount()
        {
            // Arrange
            var givenReading = _fixture.Build<MeterReading>().With(x => x.MeterReadValue, "12345").Create();
            var readings = new List<MeterReading>() { givenReading };

            var command = new AddMeterReadingsCommand(_fileMock.Object);
            _fileReadRepository.Setup(x => x.ReadMeterReadingsFile(It.IsAny<IFormFile>())).Returns(readings);
            _userAccountRepository.Setup(x => x.GetUserAccountById(It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult<UserAccount>(null));
            
            // Act
            var result = await _sut.Handle(command, CancellationToken.None);

            // Assert 
            Assert.Single(result);
            _meterReadingRepository.Verify(r => r.AddMeterReading(It.IsAny<MeterReading>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async void AddMeterReadingIntoDB_GivenValidAndInvalidReadings()
        {
            // Arrange
            var givenReadingValid = _fixture.Build<MeterReading>().With(x => x.MeterReadValue, "12345").Create();
            var givenReadingInvalid = _fixture.Build<MeterReading>().With(x => x.MeterReadValue, "VOID").Create();
            var userAccount = _fixture.Build<UserAccount>().Create();
            var readings = new List<MeterReading>() { givenReadingValid, givenReadingInvalid };
            var duplicates = _fixture.Build<MeterReading>().CreateMany(0);

            var command = new AddMeterReadingsCommand(_fileMock.Object);
            _fileReadRepository.Setup(x => x.ReadMeterReadingsFile(It.IsAny<IFormFile>())).Returns(readings);
            _meterReadingValidator.Setup(x => x.IsValidMeterReading(It.Is<MeterReading>(v => v.MeterReadValue == "12345"))).Returns(true);
            _meterReadingValidator.Setup(x => x.IsValidMeterReading(It.Is<MeterReading>(v => v.MeterReadValue == "VOID"))).Returns(false);
            _userAccountRepository.Setup(x => x.GetUserAccountById(It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(userAccount));
            _meterReadingRepository.Setup(x => x.GetDuplicateMeterReadings(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                   .Returns(Task.FromResult(duplicates.ToList()));

            // Act
            var result = await _sut.Handle(command, CancellationToken.None);

            // Assert 
            Assert.Single(result);
            Assert.Equal(givenReadingInvalid, result.First());
            _meterReadingRepository.Verify(r => r.AddMeterReading(It.IsAny<MeterReading>(), It.IsAny<CancellationToken>()), Times.Once);
            _userAccountRepository.Verify(r => r.GetUserAccountById(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
            _meterReadingRepository.Verify(r => r.GetDuplicateMeterReadings(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
