using BusinessObject;
using ClosedXML.Excel;
using DataAccess.Repository.OrderDetailRepo;
using DataAccess.Repository.OrderRepo;
using DataAccess.Repository.ProductRepo;
using eStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace eStore.Controllers
{
    class OrderExportData
    {
        [Display(Name = "Order ID")]
        public int OrderID { get; set; }

        [Display(Name = "Member Name")]
        public string MemberName { get; set; }

        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Order Total")]
        public decimal OrderTotal { get; set; }
    }

    [Authorize]
    public class OrderController : Controller
    {
        private IOrderRepository orderRepository;
        private IOrderDetailRepository orderDetailRepository;

        public OrderController()
        {
            orderRepository = new OrderRepository();
            orderDetailRepository = new OrderDetailRepository();
        }

        
        public async Task<IActionResult> Index(DateTime? start, DateTime? end, int? page)
        {
            try
            {
                ViewBag.Start = (start == null) ? "" : start.Value.Date.ToString("yyyy-MM-dd");
                ViewBag.End = (end == null) ? "" : end.Value.Date.ToString("yyyy-MM-dd");

                if (page == null)
                {
                    page = 1;
                }

                int memberId = int.Parse(User.Claims.First(c => c.Type.Equals("MemberId")).Value);
                IEnumerable<Order> orders = orderRepository.GetOrders(memberId);

                if (start != null && end != null)
                {
                    if (start > end)
                    {
                        DateTime? temp = start;
                        start = end;
                        end = temp;
                    }
                    orders = orderRepository.GetOrders(memberId, start.Value, end.Value);
                }
                else if ((start != null && end == null) || (start == null && end != null))
                {
                    throw new Exception("Please fill both of the Start and End Date inputs to filter or leave them blank!");
                } else if (start == null && end == null)
                {
                    ViewBag.Start = orders.Min(or => or.OrderDate).Date.ToString("yyyy-MM-dd");
                    ViewBag.End = orders.Max(or => or.OrderDate).Date.ToString("yyyy-MM-dd");
                }

                List<OrderExportData> orderExport = new List<OrderExportData>();
                
                foreach (var order in orders)
                {
                    decimal total = orderDetailRepository.GetOrderTotal(order.OrderId);
                    ViewData[order.OrderId.ToString()] = Math.Round(total, 2);
                    orderExport.Add(new OrderExportData
                    {
                        OrderID = order.OrderId,
                        MemberName = order.Member.Fullname,
                        OrderDate = order.OrderDate,
                        OrderTotal = total
                    });
                }
                HttpContext.Session.SetComplexData("OrderData", orderExport);
                int pageSize = 10;

                return View(await PaginatedList<Order>.CreateAsync(orders.AsQueryable(), page ?? 1, pageSize));
            } catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    throw new Exception("Order ID is not found!!!");
                }
                Order order = orderRepository.GetOrder(id.Value);
                
                if (order == null)
                {
                    throw new Exception("Product ID is not found!!!");
                }
                order.OrderDetails = orderDetailRepository.GetOrderDetails(order.OrderId).ToList();
                decimal orderTotal = orderDetailRepository.GetOrderTotal(order.OrderId);
                ViewBag.OrderTotal = orderTotal;
                return View(order);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        public IActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    throw new Exception("Order ID is not existed!!");
                }
                Order order = orderRepository.GetOrder(id.Value);
                if (order == null)
                {
                    throw new Exception("Order ID is not existed!!");
                }
                order.OrderDetails = orderDetailRepository.GetOrderDetails(order.OrderId).ToList();
                decimal orderTotal = orderDetailRepository.GetOrderTotal(order.OrderId);
                ViewBag.OrderTotal = orderTotal;
                return View(order);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                Order order = orderRepository.GetOrder(id);
                if (order == null)
                {
                    throw new Exception("Order ID is not existed!!");
                }
                orderDetailRepository.DeleteOrderDetails(id);
                orderRepository.DeleteOrder(id);
                TempData["Delete"] = "Delete Order with the ID <strong>" + id + "</strong> successfully!!";
                return RedirectToAction(nameof(Index));
            } catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        public IActionResult Export()
        {
            try
            {
                IEnumerable<OrderExportData> orderData = HttpContext.Session.GetComplexData<IEnumerable<OrderExportData>>("OrderData");
                //return Json(orderData);
                DataTable dtOrder = new DataTable("OrderReport");
                orderData = orderData.ToList();
                dtOrder.Columns.AddRange(new DataColumn[4]
                {
                    new DataColumn("OrderID"),
                    new DataColumn("MemberName"),
                    new DataColumn("OrderDate"),
                    new DataColumn("OrderTotal")
                });

                foreach (var order in orderData)
                {
                    dtOrder.Rows.Add(order.OrderID, order.MemberName, order.OrderDate, order.OrderTotal);
                }

                ExportToExcel(dtOrder);
                return null;
            } catch (Exception ex)
            {
                //return Json(ex.Message);
                //ViewBag.Error = ex.Message;
                return null;
            }
        }

        private void ExportToExcel(DataTable orders)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Orders");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Order ID";
                worksheet.Cell(currentRow, 2).Value = "Member Name";
                worksheet.Cell(currentRow, 3).Value = "Order Date";
                worksheet.Cell(currentRow, 4).Value = "Order Total";

                for (int i = 0; i < orders.Rows.Count; i++)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = orders.Rows[i]["OrderID"];
                    worksheet.Cell(currentRow, 2).Value = orders.Rows[i]["MemberName"];
                    worksheet.Cell(currentRow, 3).Value = orders.Rows[i]["OrderDate"];
                    worksheet.Cell(currentRow, 4).Value = orders.Rows[i]["OrderTotal"];
                }

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                Response.Clear();
                Response.Headers.Add("content-disposition", "attachment;filename=OrderReport.xls");
                Response.ContentType = "application/xls";
                Response.Body.WriteAsync(content);
                Response.Body.Flush();
            }
        }

    }
}
