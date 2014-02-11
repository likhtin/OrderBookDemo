using System;
using System.Collections.Generic;
using System.Threading;
using OrderBook;

namespace OrderBookTestConsole
{
	 class Program
	{

		private static void Main()
		{
	
			Demo demo  = new Demo();
			demo.Run();
		}

	}
	
	class Demo
	{		

	
		internal void Run()
		{

			BookProcessor ordBook = new BookProcessor();

			OrderBook.Order buyOrder1 = new Order("buy1-3-800", OrderSide.BUY, 3, 800, DateTime.Now);
			OrderBook.Order sellOrder1 = new Order("sell1-1-795.24", OrderSide.SELL, 1, 795.24M, DateTime.Now);

			OrderBook.Order sellOrder2 = new Order("sell3-1-805", OrderSide.SELL,  1, 805, DateTime.Now);
			OrderBook.Order sellOrder3 = new Order("sell3-3-800", OrderSide.SELL, 3, 800, DateTime.Now);

			OrderBook.Order buyOrder3 = new Order("buy3-1-806", OrderSide.BUY, 1, 806, DateTime.Now);

			//OrderBook.Order buyOrder3 = new Order(GetFixMessageId(), OrderSide.BUY, 1, 806, DateTime.Now);


			//Post 1st order
			ordBook.PostOrder(buyOrder1);

			//Post two sale order simultaniously to check thread safety
			Barrier b = new Barrier(2);

			Thread tsell1 = new Thread(() =>
				{
					b.SignalAndWait();
					ordBook.PostOrder(sellOrder1);
				});

			Thread tsell3 = new Thread(() =>
				{
					b.SignalAndWait();
					ordBook.PostOrder(sellOrder3);
				});

			tsell1.Start();
			tsell3.Start();

			tsell1.Join();
			tsell3.Join();

			// Posting two more orders
			ordBook.PostOrder(sellOrder2);
			ordBook.PostOrder(buyOrder3);

	
			//Current state of OrderBook
			Console.WriteLine("--List of Original Orders Orderid in format [id]-[quantity]-[price]");
			Console.WriteLine("OrderId\t\t Side\t Price\t OrderTime");

			foreach (var s in ordBook.OrdersCopy)
			{
				Console.WriteLine("{0}\t {1}\t {2}\t {3}", s.ClOrdID, s.Side, s.Price.ToString("#.##"), s.OrderTime.ToString("HH:mm:ss:fffffff"));
			}
			Console.WriteLine("\n--Open orders--------------------------");
			Console.WriteLine("OrderId\t\t Side\t OrderQty\t Price\t OrderTime");
			foreach (var s in ordBook.Orders)
			{
				Console.WriteLine("{0}\t {1}\t {2}\t {3}\t {4}", s.ClOrdID, s.Side, s.OrderQty.ToString("#.########"), s.Price.ToString("#.##"), s.OrderTime.ToString("HH:mm:ss:ffff"));
			}
			Console.WriteLine("\n--Executed Trades --------------------------");
			foreach (var s in ordBook.Trades)
			{
				Console.WriteLine("OrderIdBuy {0} OrderIdSell {1} Quantity {2} Price {3}", s.OrderIdBuy, s.OrderIdSell, s.Quantity, s.Price);
			}
			Console.ReadKey();

		}

		//Used for generation unique OrderId for mass testing
		private List<int> _fixMessageIdInUse = new List<int>();

		private string GetFixMessageId()
		{
			Random fixMessageIdGen = new Random();
			int fixMessageId= fixMessageIdGen.Next(1, 1000);
			
			while (_fixMessageIdInUse.Contains(fixMessageId))
				fixMessageId= fixMessageIdGen.Next(1, 1000);

			_fixMessageIdInUse.Add(fixMessageId);
			return fixMessageId.ToString();
		}
	}
}
