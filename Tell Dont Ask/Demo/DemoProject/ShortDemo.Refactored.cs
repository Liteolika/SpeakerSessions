using System;
using System.Collections.Generic;
using System.Text;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Xunit;

namespace DemoProject.Refactored
{
    public interface IAlerter { void Alert(string message); }

    public class MemoryObject
    {
        private readonly int _threshold;
        private readonly Action<string> _sendAlert;

        public MemoryObject(int threshold, Action<string> sendAlert)
        {
            _threshold = threshold;
            _sendAlert = sendAlert;
        }

        public int UsedPercentage { get; set; }
        public bool ExceedsThreshold => UsedPercentage >= _threshold;

        public void Sample()
        {
            if (ExceedsThreshold)
                _sendAlert("Alert");
        }
    }

    public class AlertSender
    {
        private readonly IAlerter _alerter;
        public AlertSender(IAlerter alerter) { _alerter = alerter; }
        public void SendAlerts(List<MemoryObject> memories)
        {
            foreach (var memory in memories)
                memory.Sample();
                
        }
    }

    public class MemoryAlertTests
    {
        [Theory]
        [InlineData(89, 90, false)]
        [InlineData(90, 90, true)]
        public void Sends_Alert_If_Above_Threshold(int percentage, int threshold, bool shouldSendAlert)
        {
            // Arrange
            var sendAlert = Substitute.For<Action<string>>();
            var memory = new MemoryObject(threshold, sendAlert) {UsedPercentage = percentage};

            // Act
            memory.Sample();

            // Assert
            sendAlert.ReceivedWithAnyArgs(
                shouldSendAlert
                    ? Quantity.Exactly(1)
                    : Quantity.None());
        }
    }
}
