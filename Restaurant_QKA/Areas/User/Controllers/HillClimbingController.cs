using Restaurant_QKA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Restaurant_QKA.Areas.User.Controllers
{
    public class HillClimbingController : Controller
    {
        private Restaurant_Entities db;

        public HillClimbingController()
        {
            db = new Restaurant_Entities();
        }

        [HttpGet]
        public ActionResult Index()
        {
            // Hiển thị tất cả món ăn ban đầu với điểm số mặc định là 0
            var menuItems = db.MenuItems
                .Where(m => m.IsActive == true)
                .ToList();

            var menuItemsWithScore = menuItems.Select(item => new MenuItemWithScore
            {
                MenuItem = item,
                Score = 0
            }).ToList();

            return View(menuItemsWithScore);
        }

        [HttpPost]
        public ActionResult Filter(string priceRange, string taste)
        {
            // Bước 1: Kiểm tra đăng nhập
            int? cusId = Session["UserID"] != null ? Convert.ToInt32(Session["UserID"]) : (int?)null;
            if (cusId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Bước 2: Lọc món tiềm năng
            List<MenuItem> potentialItems = FilterPotentialItems(priceRange, taste);
            if (potentialItems.Count == 0)
            {
                return View("Index", new List<MenuItemWithScore>());
            }

            // Bước 3: Tính điểm số cho các món tiềm năng
            double[] scores = CalculateScores(potentialItems, cusId.Value, taste);

            // Bước 4: Xây ma trận kề
            double[,] adjacencyMatrix = BuildAdjacencyMatrix(potentialItems);

            // Bước 5: Áp dụng Hill Climbing
            List<int> path = HillClimbing(scores, adjacencyMatrix);

            // Bước 6: Chuẩn bị danh sách món được chọn cùng với điểm số
            List<MenuItemWithScore> recommendedItems = path.Select(index => new MenuItemWithScore
            {
                MenuItem = potentialItems[index],
                Score = scores[index]
            }).ToList();

            return View("Index", recommendedItems);
        }

        // Bước 2: Hàm lọc món tiềm năng
        private List<MenuItem> FilterPotentialItems(string priceRange, string taste)
        {
            var menuItems = db.MenuItems.Where(m => m.IsActive == true);

            // Lọc theo khoảng giá
            if (!string.IsNullOrEmpty(priceRange))
            {
                var range = priceRange.Split('-');
                decimal minPrice = decimal.Parse(range[0]);
                decimal? maxPrice = range.Length > 1 && range[1] != "+" ? decimal.Parse(range[1]) : (decimal?)null;

                menuItems = menuItems.Where(m => m.Price >= minPrice);
                if (maxPrice.HasValue)
                {
                    menuItems = menuItems.Where(m => m.Price <= maxPrice);
                }
            }

            // Lọc theo khẩu vị
            if (!string.IsNullOrEmpty(taste))
            {
                menuItems = menuItems.Where(m => m.Description.Contains(taste));
            }

            return menuItems.Take(20).ToList();
        }

        // Bước 3: Hàm tính điểm số
        private double[] CalculateScores(List<MenuItem> potentialItems, int cusId, string taste)
        {
            int size = potentialItems.Count;
            double[] scores = new double[size];

            for (int i = 0; i < size; i++)
            {
                var item = potentialItems[i];
                double score = 0;
                score += 10; // Giá: đã lọc nên luôn đúng
                if (item.Description.Contains(taste)) score += 10; // Khẩu vị
                int orderCount = db.OrderDetails
                    .Where(od => od.MenuItemID == item.ItemID && od.Order.CusID == cusId)
                    .Sum(od => od.Quantity) ?? 0;
                score += orderCount * 10; // Lịch sử
                scores[i] = score;
            }

            return scores;
        }

        // Bước 4: Hàm xây ma trận kề
        private double[,] BuildAdjacencyMatrix(List<MenuItem> potentialItems)
        {
            int size = potentialItems.Count;
            double[,] adjacencyMatrix = new double[size, size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (i != j)
                    {
                        decimal price1 = potentialItems[i].Price.Value;
                        decimal price2 = potentialItems[j].Price.Value;
                        decimal priceDifference = Math.Abs(price1 - price2);

                        // Tiêu chí kề: khoảng cách giá không quá 50k
                        if (priceDifference <= 50000)
                        {
                            // Trọng số là khoảng cách giá
                            adjacencyMatrix[i, j] = (double)priceDifference;
                        }
                        else
                        {
                            adjacencyMatrix[i, j] = 0; // Không kề
                        }
                    }
                    else
                    {
                        adjacencyMatrix[i, j] = 0; // Không có cạnh từ đỉnh đến chính nó
                    }
                }
            }

            return adjacencyMatrix;
        }

        // Bước 5: Hàm Hill Climbing
        private List<int> HillClimbing(double[] scores, double[,] adjacencyMatrix)
        {
            int size = scores.Length;
            List<int> path = new List<int>();
            HashSet<int> visited = new HashSet<int>();

            // Tiếp tục cho đến khi không còn món nào để chọn hoặc đạt giới hạn (5 món)
            while (visited.Count < size && path.Count < 5)
            {
                // Tìm món chưa được chọn có điểm số cao nhất
                int bestIndex = -1;
                double bestScore = double.MinValue;

                for (int i = 0; i < size; i++)
                {
                    if (!visited.Contains(i) && scores[i] > bestScore)
                    {
                        bestScore = scores[i];
                        bestIndex = i;
                    }
                }

                if (bestIndex == -1) break; // Không còn món nào để chọn

                path.Add(bestIndex);
                visited.Add(bestIndex);

                // Tìm láng giềng tốt nhất của món vừa chọn
                int current = bestIndex;
                bool improved;
                do
                {
                    improved = false;
                    bestScore = double.MinValue;
                    int bestNeighbor = -1;

                    for (int i = 0; i < size; i++)
                    {
                        if (adjacencyMatrix[current, i] > 0 && !visited.Contains(i) && scores[i] > bestScore)
                        {
                            bestScore = scores[i];
                            bestNeighbor = i;
                            improved = true;
                        }
                    }

                    if (improved)
                    {
                        path.Add(bestNeighbor);
                        visited.Add(bestNeighbor);
                        current = bestNeighbor;
                    }
                } while (improved && path.Count < 5);
            }

            return path;
        }
    }
}