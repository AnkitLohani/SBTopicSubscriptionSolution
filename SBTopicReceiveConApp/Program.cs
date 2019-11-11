using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBTopicReceiveConApp
{
    class Program
    {
        static NamespaceManager _namespaceManager;
        static void Main(string[] args)
        {
            CollectSBDetails();
            ReceiveMessage();
            Console.WriteLine($"Done:{DateTime.Now}");
        }
      
        static void ReceiveMessage()
        {
            TokenProvider tokenProvider = _namespaceManager.Settings.TokenProvider;
            if (_namespaceManager.TopicExists("DataCollectionTopic"))
            {
                MessagingFactory factory = MessagingFactory.Create(_namespaceManager.Address, tokenProvider);
                //MessageReceiver receiver = factory.CreateMessageReceiver("DataCollectionTopic/subscriptions/Inventory");
                MessageReceiver receiver = factory.CreateMessageReceiver("DataCollectionTopic/subscriptions/Dashboard");
                BrokeredMessage receivedMessage = null;
                try
                {
                   while ((receivedMessage=receiver.Receive())!=null)
                    {
                        ProcessMessage(receivedMessage);
                        receivedMessage.Complete();
                    }
                    factory.Close();
                    _namespaceManager.DeleteSubscription("DataCollectionTopic", "Inventory");
                   // _namespaceManager.DeleteTopic("DataCollectionTopic");
                }
                
                catch (Exception ex)
                {

                    Console.WriteLine(ex.ToString());
                    receivedMessage.Abandon();
                }
            }
        }

        static void ProcessMessage(BrokeredMessage receivedMessage)
        {
            Console.WriteLine($"Lable: {receivedMessage.Label} ,MessageID:{receivedMessage.MessageId}, StoreName :{receivedMessage.Properties["StoreName"]} and MachineID :{receivedMessage.Properties["MachineID"]}");
        }

        static void CollectSBDetails()
        {
            _namespaceManager = NamespaceManager.CreateFromConnectionString(ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"].ToString());
        }
    }
}
