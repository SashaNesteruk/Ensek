using AutoFixture;
using EnsekMeterReadings.Application.Validators;
using EnsekMeterReadings.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EnsekMeterReadingsUnitTest
{
    public class MeterReadingValidatorTest
    {
        private MeterReadingValidator _sut;
        private readonly Fixture _fixture;

        public MeterReadingValidatorTest()
        {
            _fixture = new Fixture();
            _sut = new MeterReadingValidator();
        }

        [Fact]
        public void ValidateMeterReading_GivenValidInput()
        {
            // Arrange
            var input = _fixture.Build<MeterReading>().With(x => x.MeterReadValue, "01234").Create();

            // Act
            var result = _sut.IsValidMeterReading(input);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateMeterReading_GivenLongInput()
        {
            // Arrange
            var input = _fixture.Build<MeterReading>().With(x => x.MeterReadValue, "123455555").Create();

            // Act
            var result = _sut.IsValidMeterReading(input);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateMeterReading_GivenShortInput()
        {
            // Arrange
            var input = _fixture.Build<MeterReading>().With(x => x.MeterReadValue, "123").Create();

            // Act
            var result = _sut.IsValidMeterReading(input);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateMeterReading_GivenNonNumericalInput()
        {
            // Arrange
            var input = _fixture.Build<MeterReading>().With(x => x.MeterReadValue, "123X5").Create();

            // Act
            var result = _sut.IsValidMeterReading(input);

            // Assert
            Assert.False(result);
        }
    }
}
