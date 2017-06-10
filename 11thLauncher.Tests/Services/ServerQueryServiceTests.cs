using NUnit.Framework;
using _11thLauncher.Model;
using _11thLauncher.Model.Server;
using _11thLauncher.Services;

namespace _11thLauncher.Tests.Services
{
    [TestFixture, Category("Services")]
    public class ServerQueryServiceTests
    {
        private IServerQueryService _serverQueryService;

        [SetUp]
        public void SetUp()
        {
            _serverQueryService = new ServerQueryService();
        }

        [Test]
        public void CheckServerStatus_Online()
        {
            //Arrange
            var server = new Server()
            {
                Name = "TestServer",
                Address = "11thmeu.es",
                Port = 2302,
                IsDefault = true,
                IsEnabled = true
            };

            //Act
            _serverQueryService.CheckServerStatus(server);

            //Assert
            Assert.That(server.ServerStatus, Is.EqualTo(ServerStatus.Online));
        }
    }
}
