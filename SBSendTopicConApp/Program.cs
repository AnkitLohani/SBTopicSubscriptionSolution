using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBSendTopicConApp
{
    class Program
    {
        static NamespaceManager _namespacemanager;
        static void Main(string[] args)
        {
            CollectSBDetails();
            SendMessage();
         //   Console.WriteLine("All Message Sent successfully....");
        }

        static void SendMessage()
        {
            TokenProvider tokenProvider = _namespacemanager.Settings.TokenProvider;
            if (!_namespacemanager.TopicExists("DataCollectionTopic"))
            {
                _namespacemanager.CreateTopic("DataCollectionTopic");
                if (!_namespacemanager.SubscriptionExists("DataCollectionTopic","Inventory"))
                {
                    _namespacemanager.CreateSubscription("DataCollectionTopic", "Inventory");
                }
                if (!_namespacemanager.SubscriptionExists("DataCollectionTopic", "Dashboard"))
                {
                    _namespacemanager.CreateSubscription("DataCollectionTopic", "Dashboard");
                }
                if (!_namespacemanager.SubscriptionExists("DataCollectionTopic", "Sales"))
                {
                    _namespacemanager.CreateSubscription("DataCollectionTopic", "Sales");
                }
                if (!_namespacemanager.SubscriptionExists("DataCollectionTopic", "Feedback"))
                {
                    _namespacemanager.CreateSubscription("DataCollectionTopic", "Feedback");
                }

                MessagingFactory factory = MessagingFactory.Create(_namespacemanager.Address, tokenProvider);
                BrokeredMessage message = new BrokeredMessage();//can pass a user defined class
                message.Label = "SalesReport";
                message.Properties["StoreName"] = "Nike";
                message.Properties["MachineID"] = "POS1";

                BrokeredMessage message1 = new BrokeredMessage();//can pass a user defined class
                message1.Label = "SalesReport";
                message1.Properties["StoreName"] = "Adidas";
                message1.Properties["MachineID"] = "POS3";

                MessageSender sender = factory.CreateMessageSender("DataCollectionTopic");
                sender.Send(message);
                sender.Send(message1);
                Console.WriteLine("Message Send Successfully");
            }
        }
        

        static void CollectSBDetails()
        {
            _namespacemanager = NamespaceManager.CreateFromConnectionString(ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"].ToString());
        }
    }

}
