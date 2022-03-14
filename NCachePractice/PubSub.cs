using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Alachisoft.NCache.Runtime.Caching;
using Alachisoft.NCache.Runtime.Events;
using Alachisoft.NCache.Client;
using System.Configuration;

namespace NCachePractice
{
    public class PubSub
    {
        private static ICache cache = null;

        public static void Run()
        {
            // Initialize the cache
            Initialize();
            ShowMenu();
            //create topic
            //string topic = "UserCreated";
            //CreateTopic(topic);
            //SubscribeToTopicNonDurably(topic);
            //PublishMessage(topic);
            while (true)
            {

            }
        }

        private static void ShowMenu()
        {
            bool isActive = true;

            while (isActive)
            {
                Thread.Sleep(500);
                Console.WriteLine("\n======= Main Menu =======\n");

                Console.WriteLine("1. Subscribe to Topic");
                Console.WriteLine("2. Publish to Topic ");
                Console.WriteLine("3. Unsubscribe to Topic");
                Console.WriteLine("4. Delete Topic");
                Console.WriteLine("5. Exist");

                Console.Write("Enter your choice: ");

                string inputString = Console.ReadLine();


                if (Int32.TryParse(inputString, out int input))
                {
                    switch (input)
                    {
                        case 1:
                            Subscribe();
                            break;
                        case 2:
                            Publish();
                            break;
                        case 3:
                            Unsubscribe();
                            break;
                        case 4:
                            Delete();
                            break;
                        case 5:
                            break; 
                        default:
                            Console.WriteLine("Please enter number from the given choices");
                            break;
                    }
                }
                else
                    Console.WriteLine("Please enter valid input\n");
            }
        }

        // Get user input for publishing data to a topic
        private static void Publish()
        {
            string key;
            User user = new User();

            Console.Write("\nEnter topic name:");
            string topic = Console.ReadLine();

            //create topic if it does not exist
            ITopic topic1 = cache.MessagingService.GetTopic(topic);
            if(topic1 == null)
            {
                CreateTopic(topic);
            }
            
            Console.Write("\nEnter key:");
            key = Console.ReadLine();

            Console.Write("\nEnter User Name:");
            user.Name = Console.ReadLine();

            Console.Write("\nEnter User Age:");
            var ageAsString = Console.ReadLine();
            int age;
            while (!int.TryParse(ageAsString, out age))
            {
                Console.WriteLine("Age must be a number!");
                Console.Write("\nEnter Customer Age:");
                ageAsString = Console.ReadLine();
            }
            user.Age = age;

            Console.Write("\nEnter User Profession:");
            user.Group = Console.ReadLine();

            PublishMessage(topic, user);
        }

        // Get topic name from user to subscribe 
        private static void Subscribe()
        {
            Console.Write("\nEnter topic name:");
            string topic = Console.ReadLine();
            //create topic if it does not exist
            ITopic topic1 = cache.MessagingService.GetTopic(topic);
            if (topic1 == null)
            {
                CreateTopic(topic);
            }
            SubscribeToTopicNonDurably(topic);
        }

        //Unsubscribe Topic
        private static void Unsubscribe()
        {
            Console.Write("\nEnter topic name:");
            string topic = Console.ReadLine();
            SubscribeToTopicNonDurably(topic);
        }

        // Delete Topic
        private static void Delete()
        {
            Console.Write("\nEnter topic name:");
            string topic = Console.ReadLine();
            DeleteTopic(topic);
        }

       
        private static void Initialize()
        {
            cache = CacheManager.GetCache("ClusteredCache-2");
        }

        private static void CreateTopic(string topicName)
        {
            ITopic topic = cache.MessagingService.CreateTopic(topicName);
            Console.WriteLine("Topic " + topicName + " has been created successfully");
        }
        private static void PublishMessage(string topicName, User user)
        {
            ITopic topic = cache.MessagingService.GetTopic(topicName);
            if (topic != null)
            {
                // Create the new message with the object order
                var userMessage = new Message(user);

                // Set the expiration time of the message
                userMessage.ExpirationTime = TimeSpan.FromSeconds(5000);

                // Register message delivery failure
                topic.MessageDeliveryFailure += OnFailureMessageReceived;

                //Register topic deletion notification
                topic.OnTopicDeleted = TopicDeleted;

                //publish message to all subscribers and register failuer notification
                topic.Publish(userMessage, DeliveryOption.All, true);
            }
            else
            {
                Console.WriteLine("Provided topic does not exist to publish message");
            }

        }

        //Create non-duarable subscription
        private static void SubscribeToTopicNonDurably(string topicName)
        {
            ITopic topic = cache.MessagingService.GetTopic(topicName);
            if(topic != null)
            {
                //Create and register subscriber for the provided topic
                ITopicSubscription topicSubscriber = topic.CreateSubscription(MessageReceived);
            }
            else
            {
                Console.WriteLine("provided topic does not exist to subscribe");
            }
        }

        // create duarable subscription
        private static void SubscribeToTopicDurably(string topicName, string subscriptionName)
        {
            ITopic topic = cache.MessagingService.GetTopic(topicName);

            //Create IDurable Subscription
            IDurableTopicSubscription durableTopicSubscription = topic.CreateDurableSubscription(subscriptionName,
                SubscriptionPolicy.Shared, MessageReceived, TimeSpan.FromSeconds(25));
        }

        // Delete tpoics
        private static void DeleteTopic(string topicName)
        {
            ITopic topic = cache.MessagingService.GetTopic(topicName);
            if(topic != null)
            {
                cache.MessagingService.DeleteTopic(topicName);
            }
        }

        //Message Receve Callback
        private static void MessageReceived(object sender, MessageEventArgs args)
        {
            if(args.Message.Payload is User user)
            {
                Console.WriteLine("Message received!!!!");
                DisplayUserDetails(user);
            }
            else
            {
                Console.WriteLine("Message failed to receive");
            }
        }

        // Display user object information on console
        private static void DisplayUserDetails(User user)
        {
            Console.WriteLine("Name:       " + user.Name);
            Console.WriteLine("Age:        " + user.Age);
            Console.WriteLine("Profession  " + user.Group);
            Console.WriteLine("==========================================");
        }

        private static void OnFailureMessageReceived(object sender, MessageFailedEventArgs args)
        {
            // Failure reason can be get from args.MessageFailureReason
            Console.WriteLine("Messaged Failed to Publish due to " + args.MessageFailureReason);
        }

        private static void TopicDeleted(object sender, TopicDeleteEventArgs args)
        {
            // Deleted topic is args.TopicName
            Console.WriteLine("Topic " + args.TopicName + " has been deleted");
        }

        private static void Topic_MessageDeliveryFailure(object sender, MessageFailedEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
