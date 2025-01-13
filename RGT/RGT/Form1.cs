using RGT.Infrastructure.Services;
using RGT.Core.Entities;
namespace RGT
{
    public partial class Form1 : Form
    {
        private Dictionary<string, Panel> tablePanels = new Dictionary<string, Panel>();
        private OrderSimulator orderSimulator;
        private Dictionary<string, string> tableStates = new Dictionary<string, string>();
        private Dictionary<string, Dictionary<string, int>> dailyStatistics = new Dictionary<string, Dictionary<string, int>>();
        private Dictionary<int, int> hourlyStatistics = new Dictionary<int, int>();

        public Form1()
        {
            InitializeComponent();
            InitializeOrderDataGrid();
            InitializeOrderSimulator();
        }

        #region Panel Creation and Layout

        private Point CalculatePanelPosition(int index)
        {
            int panelsPerRow = 4;
            int x = 10 + (index % panelsPerRow) * 230;
            int y = 10 + (index / panelsPerRow) * 180;
            return new Point(x, y);
        }

        private void CreateTablePanel(string tableNumber)
        {
            try
            {
                Panel panel = new Panel
                {
                    Name = tableNumber,
                    Size = new Size(200, 150),
                    BorderStyle = BorderStyle.FixedSingle,
                    Location = CalculatePanelPosition(tablePanels.Count),
                    BackColor = Color.LightGray
                };

                panel.Click += (s, e) => OnPanelClick(tableNumber);

                Label titleLabel = new Label
                {
                    Text = tableNumber,
                    Font = new Font("Arial", 12, FontStyle.Bold),
                    Dock = DockStyle.Top,
                    TextAlign = ContentAlignment.MiddleCenter,
                    AutoSize = false,
                    Height = 30
                };

                titleLabel.Click += (s, e) => panel_Click(panel, EventArgs.Empty);
                panel.Controls.Add(titleLabel);

                Label orderItemsLabel = new Label
                {
                    Name = tableNumber + "_items",
                    Font = new Font("Arial", 10),
                    Location = new Point(5, 40),
                    AutoSize = false,
                    Size = new Size(190, 70),
                    TextAlign = ContentAlignment.TopLeft,
                    Text = "주문 내역 없음"
                };

                orderItemsLabel.Click += (s, e) => panel_Click(panel, EventArgs.Empty);
                panel.Controls.Add(orderItemsLabel);

                Label timeLabel = new Label
                {
                    Name = tableNumber + "_time",
                    Font = new Font("Arial", 9),
                    Dock = DockStyle.Bottom,
                    TextAlign = ContentAlignment.MiddleRight,
                    AutoSize = false,
                    Height = 20,
                    Text = "시간: --:--"
                };

                timeLabel.Click += (s, e) => panel_Click(panel, EventArgs.Empty);
                panel.Controls.Add(timeLabel);

                tablePanels[tableNumber] = panel;
                tabPage1.Controls.Add(panel);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"패널 생성 중 오류가 발생했습니다: {ex.Message}");
            }
        }

        #endregion

        #region Panel State Management

        private void OnPanelClick(string tableNumber)
        {
            try
            {
                string currentState = tableStates.ContainsKey(tableNumber) ? tableStates[tableNumber] : "주문 접수";
                string nextState = GetNextState(currentState);
                tableStates[tableNumber] = nextState;
                UpdatePanelState(tablePanels[tableNumber], nextState);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"패널 상태 업데이트 중 오류가 발생했습니다: {ex.Message}");
            }
        }

        private void panel_Click(object sender, EventArgs e)
        {
            if (sender is Panel panel)
            {
                OnPanelClick(panel.Name);
            }
        }

        private string GetNextState(string currentState)
        {
            return currentState switch
            {
                "주문 접수" => "주문 처리",
                "주문 처리" => "처리 완료",
                "처리 완료" => "주문 접수",
                _ => "주문 접수"
            };
        }

        private void UpdatePanelState(Panel panel, string state)
        {
            try
            {
                switch (state)
                {
                    case "주문 접수":
                        panel.BackColor = Color.LightBlue;
                        break;
                    case "주문 처리":
                        panel.BackColor = Color.Orange;
                        break;
                    case "처리 완료":
                        panel.BackColor = Color.Green;
                        break;
                }

                var titleLabel = panel.Controls.OfType<Label>().FirstOrDefault(lbl => lbl.Dock == DockStyle.Top);
                if (titleLabel != null)
                {
                    titleLabel.Text = $"{panel.Name} - {state}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"패널 상태 변경 중 오류가 발생했습니다: {ex.Message}");
            }
        }

        #endregion

        #region Order Processing

        private void InitializeOrderSimulator()
        {
            try
            {
                orderSimulator = new OrderSimulator();
                orderSimulator.OnOrdersGenerated += UpdateTabPagePanels;
                orderSimulator.OnOrdersGenerated += (sender, orders) =>
                {
                    foreach (var order in orders)
                    {
                        UpdateStatistics(order);
                    }
                };

                orderSimulator.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"주문 시뮬레이터 초기화 중 오류가 발생했습니다: {ex.Message}");
            }
        }

        private void UpdateTabPagePanels(object sender, List<OrderData> orders)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateTabPagePanels(sender, orders)));
                return;
            }

            try
            {
                foreach (var order in orders)
                {
                    if (!tablePanels.ContainsKey(order.TableNumber))
                    {
                        CreateTablePanel(order.TableNumber);
                    }

                    UpdatePanelWithOrder(tablePanels[order.TableNumber], order);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"패널 업데이트 중 오류가 발생했습니다: {ex.Message}");
            }
        }

        private void UpdatePanelWithOrder(Panel panel, OrderData order)
        {
            try
            {
                var orderItemsLabel = panel.Controls.OfType<Label>().FirstOrDefault(lbl => lbl.Name == order.TableNumber + "_items");
                if (orderItemsLabel != null)
                {
                    orderItemsLabel.Text = string.Join("\n", order.OrderItems.Select(item => $"{item}"));
                }

                var timeLabel = panel.Controls.OfType<Label>().FirstOrDefault(lbl => lbl.Name == order.TableNumber + "_time");
                if (timeLabel != null)
                {
                    timeLabel.Text = $"시간: {order.OrderTime:HH:mm:ss}";
                }

                var titleLabel = panel.Controls.OfType<Label>().FirstOrDefault(lbl => lbl.Dock == DockStyle.Top);
                if (titleLabel != null)
                {
                    titleLabel.Text = $"{panel.Name} - 주문 접수";
                }

                panel.BackColor = Color.LightBlue;
                BlinkPanel(panel);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"패널 주문 업데이트 중 오류가 발생했습니다: {ex.Message}");
            }
        }

        private void BlinkPanel(Panel panel, int blinkCount = 4, int interval = 500)
        {
            var originalColor = panel.BackColor;
            var blinkTimer = new System.Windows.Forms.Timer();
            int currentBlink = 0;

            blinkTimer.Tick += (s, e) =>
            {
                try
                {
                    if (currentBlink >= blinkCount)
                    {
                        blinkTimer.Stop();
                        panel.BackColor = originalColor;
                        blinkTimer.Dispose();
                        return;
                    }
                    panel.BackColor = panel.BackColor == Color.Yellow ? originalColor : Color.Yellow;
                    currentBlink++;
                }
                catch
                {
                    blinkTimer.Stop();
                    blinkTimer.Dispose();
                }
            };

            blinkTimer.Interval = interval;
            blinkTimer.Start();
        }

        #endregion

        #region Statistics Management

        private void UpdateStatistics(OrderData order)
        {
            try
            {
                string date = order.OrderTime.ToString("yyyy-MM-dd");
                string hour = order.OrderTime.ToString("HH");

                if (!dailyStatistics.ContainsKey(date))
                {
                    dailyStatistics[date] = new Dictionary<string, int>();
                }

                if (!dailyStatistics[date].ContainsKey(hour))
                {
                    dailyStatistics[date][hour] = 0;
                }
                dailyStatistics[date][hour]++;

                int hourKey = order.OrderTime.Hour;
                if (!hourlyStatistics.ContainsKey(hourKey))
                {
                    hourlyStatistics[hourKey] = 0;
                }
                hourlyStatistics[hourKey]++;

                UpdateDashboard();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"통계 업데이트 중 오류가 발생했습니다: {ex.Message}");
            }
        }

        private void UpdateDashboard()
        {
            // UI 스레드에서 실행되도록 Invoke로 감쌉니다.
            if (orderData.InvokeRequired)
            {
                orderData.Invoke(new Action(UpdateDashboard));
                return;
            }

            // DataGridView 초기화
            orderData.Rows.Clear();

            // 일별 통계 데이터 추가
            foreach (var date in dailyStatistics.Keys)
            {
                foreach (var hour in dailyStatistics[date].Keys.OrderBy(h => h))
                {
                    int totalOrders = dailyStatistics[date][hour];
                    orderData.Rows.Add(date, $"{hour}:00", totalOrders);
                }
            }

            // 시간대별 통계 데이터 추가
            foreach (var hour in hourlyStatistics.Keys.OrderBy(k => k))
            {
                orderData.Rows.Add("전체", $"{hour}:00", hourlyStatistics[hour]);
            }
        }




        private void InitializeOrderDataGrid()
        {
            try
            {
                orderData.Columns.Clear();
                orderData.Columns.Add("Date", "날짜");
                orderData.Columns.Add("Hour", "시간대");
                orderData.Columns.Add("OrderCount", "주문 수");

                orderData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                orderData.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                orderData.AllowUserToAddRows = false;
                orderData.ReadOnly = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"데이터 그리드 초기화 중 오류가 발생했습니다: {ex.Message}");
            }
        }

        #endregion
    }
}
