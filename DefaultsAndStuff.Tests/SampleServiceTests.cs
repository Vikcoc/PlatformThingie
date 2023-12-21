using System.Text.RegularExpressions;

namespace DefaultsAndStuff.Tests
{
    public class SampleServiceTests
    {
        private readonly SampleService _sampleService;
        public SampleServiceTests()
        {
            _sampleService = new SampleService();
        }

        [Theory]
        [InlineData(-1, -5, 5, -1)]
        [InlineData(4, -5, 5, 4)]
        [InlineData(2, -5, 5, 2)]
        [InlineData(2, 0, 5, 2)]
        [InlineData(7, 5, 15, 7)]
        [InlineData(12, 10, 50, 12)]
        [InlineData(32, 20, 300, 32)]
        public void ModuloToInterval_WithinInterval_Tests(int value, int start, int end, int expected)
        {
            var result = _sampleService.ModuloToInterval(value, start, end);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(20, 0, 5, 0)]
        [InlineData(22, 4, 9, 7)]
        [InlineData(33, 5, 15, 13)]
        public void ModuloToInterval_OutsideIntervalPositive_Tests(int value, int start, int end, int expected)
        {
            var result = _sampleService.ModuloToInterval(value, start, end);

            Assert.Equal(expected, result);
        }
    }
}