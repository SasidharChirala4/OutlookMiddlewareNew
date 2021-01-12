using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace Edreams.OutlookMiddleware.Common.ServiceBus
{
    public class Class1
    {
        public async Task Test()
        {
            Azure.Messaging.ServiceBus.ServiceBusClient client = new ServiceBusClient("");
            var sender = client.CreateSender("QUEUE");
            var message = new ServiceBusMessage(BinaryData.FromObjectAsJson("string"));
            await sender.SendMessageAsync(message);

            var receiver = client.CreateReceiver("QUEUE");
            await foreach (var x in receiver.ReceiveMessagesAsync())
            {
                
            }

            var processor = client.CreateProcessor("QUEUEUE");
            processor.ProcessMessageAsync += Process;
            await processor.StartProcessingAsync();
        }

        private Task Process(ProcessMessageEventArgs e)
        {
            e.
        }
    }
}