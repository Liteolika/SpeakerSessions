using System.Collections.Generic;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Xunit;

namespace DemoProject.Original
{
    public interface IAlerter { void Alert(string message); }

    public class MemoryObject
    {
        public int UsedPercentage { get; set; }
    }

    public class AlertSender
    {
        private readonly IAlerter _alerter;
        public AlertSender(IAlerter alerter) { _alerter = alerter; }
        public void SendAlerts(List<MemoryObject> memories)
        {
            foreach (var memory in memories)
                if (memory.UsedPercentage >= 90)
                    _alerter.Alert($"Memory usage is {memory.UsedPercentage}.");
        }
    }

    public class MemoryAlertTests
    {
        [Theory]
        [InlineData(89, false)]
        [InlineData(90, true)]
        public void Sends_Alert_If_Above_Threshold(int percentage, bool shouldSendAlert)
        {
            // Arrange
            var alerter = Substitute.For<IAlerter>();
            var memories = new List<MemoryObject>()
            {
                new MemoryObject() {UsedPercentage = percentage}
            };
            var alertSender = new AlertSender(alerter);

            // Act
            alertSender.SendAlerts(memories);

            // Assert
            alerter.ReceivedWithAnyArgs(
                    shouldSendAlert
                        ? Quantity.Exactly(1)
                        : Quantity.None())
                .Alert(string.Empty);
        }
    }
}
