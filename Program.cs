// See https://aka.ms/new-console-template for more information
using System;
using System.Collections;
using System.IO;

//Console.WriteLine("Welcome to AE Capital trading");


namespace AECapitalSample
{
    public class ExchangeSample
    {
        static void Main(string[] args)
        {
            string strTrade = string.Empty;
            MarketOrders mol = new MarketOrders();
            Console.Write("Enter trade details - ");
            strTrade = Console.ReadLine();
                        
            while (strTrade.Length > 0 && (!strTrade.Equals("exit") && !strTrade.Equals("Exit")))
            {                
                mol.ParseOrders(strTrade);
                Console.WriteLine("Hello, World!" + Convert.ToString(args.Length));
                Console.Write("Enter trade details - ");
                strTrade = Console.ReadLine();
            } 
        }
    }

    public class Order
    {
        public int TradeId { get; set; }
        public string? Counterparty { get; set; }
        public string? Instrument { get; set; }
        public double Price { get; set; }
        public Int32 Quantity { get; set; }
        public string? OrderType { get; set; }

    }
    public class MarketOrders
    {
        public Dictionary<string, List<Order>> _sellMarketOrdersList = new Dictionary<string, List<Order>>();
        public Dictionary<string, List<Order>> _buyMarketOrdersList = new Dictionary<string, List<Order>>();
        Int32 tradeCount = 0;
        public MarketOrders()
        {
        }
        public void ParseOrders(string args)
        {

            Order Order = new Order();
            Order OtherOrder = new Order();
      
            string[] parseArgs = args.Split(':');
            Console.WriteLine("Arg Count: {0}" , parseArgs.Count());
            Console.WriteLine("Count: {0} {1} {2} {3} {4}", parseArgs.Count(), parseArgs[0], parseArgs[1], parseArgs[2], parseArgs[3]);
            Console.WriteLine("trade Count: {0} ", Convert.ToString(tradeCount));

            int iCount = GetTradeCount();
            if (Convert.ToInt32(parseArgs[2]) > 0)
            {
                Order.TradeId = iCount;
                Order.Counterparty = parseArgs[0] + Convert.ToString(Order.TradeId);
                Order.Instrument = parseArgs[1].Substring(0, 3);
                Order.OrderType = "B";
                Order.Quantity = Math.Abs(Convert.ToInt32(parseArgs[2]));
                Order.Price = Convert.ToDouble(parseArgs[3]);

                OtherOrder.TradeId = iCount;
                OtherOrder.Counterparty = string.Empty;
                OtherOrder.Instrument = parseArgs[1].Substring(3, 3);
                OtherOrder.OrderType = "S";
                int i = Math.Abs(Convert.ToInt32(parseArgs[2]));
                double j = Convert.ToDouble(parseArgs[3]);
                //Console.WriteLine(" other 1 Q & P {0} {1}", Convert.ToString(i), Convert.ToString(j));
                OtherOrder.Quantity = (int)(i / j);
                OtherOrder.Price = 0.0;
            }
            else
            {
                Order.TradeId = iCount;
                Order.Counterparty = parseArgs[0] + Convert.ToString(Order.TradeId);
                Order.Instrument = parseArgs[1].Substring(0, 3);
                Order.OrderType = "S";
                Order.Quantity = Math.Abs(Convert.ToInt32(parseArgs[2]));
                Order.Price = Convert.ToDouble(parseArgs[3]);

                OtherOrder.TradeId = iCount;
                OtherOrder.Counterparty = string.Empty;
                OtherOrder.Instrument = parseArgs[1].Substring(3, 3);
                OtherOrder.OrderType = "B";
                int i = Math.Abs(Convert.ToInt32(parseArgs[2]));
                double j = Convert.ToDouble(parseArgs[3]);
                //Console.WriteLine( " other 2 Q & P {0} {1}", Convert.ToString(i), Convert.ToString(j));
                OtherOrder.Quantity = (int) (i / j);
                OtherOrder.Price = 0.0;
            }
         
            Order? ord = FindOrders(Order);

            if (ord.Quantity==0)
            {
                Console.WriteLine("No matching orders were found");
                AddMarketOrders(Order, OtherOrder);

            }
            else
            {
                string retstr = Order.Counterparty + ":" + ord.Counterparty + ":" + Order.Instrument+ OtherOrder.Instrument + ":" + (ord.Quantity - Order.Quantity) + ":" + ord.Price;
                Console.WriteLine("Trade was successfull against order: {0}",retstr);
            }
        }
        public string AddMarketOrders(Order order, Order Other)
        {
            if (order.OrderType.Equals("B"))
            {
                AddBuyMarketOrders(order);
                AddSellMarketOrders(Other);
            }
            else
            {
                AddSellMarketOrders(order);
                AddBuyMarketOrders(Other);
            }
            Console.WriteLine("Successfully added orders");
            return "Successfully added orders";
        }
        private int GetTradeCount()
        {
            return tradeCount++;
        }

        public Order FindOrders(Order Order)
        {
            
            List<Order> orders = new List<Order>();
            Order? ord = new Order();

            if (Order.OrderType == "B")
            {
         
                if (_sellMarketOrdersList.Count() > 0)
                {
                    Console.WriteLine("sell order count {0}", Convert.ToInt32(_buyMarketOrdersList.Count));
                    foreach (var item in _sellMarketOrdersList)
                    {
                        if (item.Key.Equals(Order.Instrument))
                        {
                            orders = item.Value;
                            if( orders.Count !=0)
                            {
                                Console.WriteLine("order count {0}", Convert.ToInt32(orders.Count));
                                for (int i =0; i<= orders.Count; i++)
                                {
                                    if(orders[i].Quantity >0)
                                    {
                                        ord = orders[i];
                                        
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (_buyMarketOrdersList.Count > 0)
                {
                    Console.WriteLine("buy order count {0}", Convert.ToInt32(_buyMarketOrdersList.Count));
                    foreach (var item in _buyMarketOrdersList)
                    {
                        if (item.Key.Equals(Order.Instrument))
                        {
                            orders = item.Value;
                            if (orders.Count != 0)
                            {
                                Console.WriteLine("order count {0}", Convert.ToInt32(orders.Count));
                                for (int i = 0; i < orders.Count; i++)
                                {
                                    if (orders[i].Quantity > 0)
                                    {
                                        ord = orders[i];
                                        
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

            }

            return ord;
        }
        public void AddBuyMarketOrders(Order Order)
        {
            List<Order> orders = new List<Order>();
            string key = Order.Instrument;
            if (_buyMarketOrdersList.Count() == 0)
            {             
                orders.Add(Order);
                _buyMarketOrdersList.Add(key, orders);
            }
            else
            {
                if (_buyMarketOrdersList.TryGetValue(key, out orders))
                {
                    orders.Add(Order);
                    _buyMarketOrdersList.Remove(key);
                    _buyMarketOrdersList.Add(key, orders);
                }
            }
        }
        public void AddSellMarketOrders(Order Order)
        {

            List<Order> orders = new List<Order>();
            string key = Order.Instrument;
            if (_sellMarketOrdersList.Count() == 0)
            {
                Order Ord = new Order();
                orders.Add(Order);
                _sellMarketOrdersList.Add(key, orders);
            }
            else
            {
                if (_sellMarketOrdersList.TryGetValue(key, out orders))
                {
                    orders.Add(Order);
                    _sellMarketOrdersList.Remove(key);
                    _sellMarketOrdersList.Add(key, orders);
                }
            }
        }
    }
}
