using System;
using System.Collections.Generic;
using System.Timers;
using RGT.Core.Entities;

namespace RGT.Infrastructure.Services
{
    public class OrderSimulator
    {
        private static readonly Random random = new Random();
        private static readonly string[] OrderItems = { "오징어버거", "치킨버거", "불고기버거", "치즈버거", "라이스버거" };
        private System.Timers.Timer Timer;
        private List<OrderData> orderCache = new List<OrderData>(); // 캐싱을 위한 리스트

        #region Events

        // 이벤트 선언
        public event EventHandler<OrderData> OnOrderGenerated;
        public event EventHandler<List<OrderData>> OnOrdersGenerated;

        #endregion

        #region Start and Stop

        public void Start()
        {
            try
            {
                Timer = new System.Timers.Timer(60000);
                Timer.Elapsed += GenerateOrders;
                Timer.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OrderSimulator 시작 중 오류가 발생했습니다: {ex.Message}");
            }
        }

        public void Stop()
        {
            try
            {
                Timer?.Stop();
                Timer?.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OrderSimulator 중지 중 오류가 발생했습니다: {ex.Message}");
            }
        }

        #endregion

        #region Order Generation

        private void GenerateOrders(object sender, ElapsedEventArgs e)
        {
            try
            {
                int orderCount = random.Next(1, 4);
                var orders = new List<OrderData>();

                for (int i = 0; i < orderCount; i++)
                {
                    var orderData = new OrderData
                    {
                        OrderId = random.Next(1000, 9999),
                        TableNumber = "RGT" + random.Next(1, 9),
                        OrderItems = GenerateRandomItems(),
                        OrderTime = DateTime.Now
                    };

                    // 캐싱
                    orderCache.Add(orderData);
                    if (orderCache.Count > 100)
                    {
                        orderCache.RemoveAt(0); // 캐시 크기 제한
                    }

                    OnOrderGenerated?.Invoke(this, orderData);
                    orders.Add(orderData);
                }

                OnOrdersGenerated?.Invoke(this, orders);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"주문 생성 중 오류가 발생했습니다: {ex.Message}");
            }
        }

        private List<string> GenerateRandomItems()
        {
            try
            {
                int itemCount = random.Next(1, 4);
                var items = new List<string>();

                for (int i = 0; i < itemCount; i++)
                {
                    items.Add(OrderItems[random.Next(OrderItems.Length)]);
                }

                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"랜덤 아이템 생성 중 오류가 발생했습니다: {ex.Message}");
                return new List<string> { "오류 발생" };
            }
        }

        #endregion

       
    }
}
