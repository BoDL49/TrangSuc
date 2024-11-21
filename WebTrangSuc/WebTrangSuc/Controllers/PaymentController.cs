using PayPal.Api;
using System.Web.Http;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System;
using PayPal;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WebTrangSuc.Models;
using System.Data.Entity;


public class PaymentController : ApiController
{
    private readonly shoptrangsucEntities1 _context;

    public PaymentController()
    {
        _context = new shoptrangsucEntities1();
    }

    public PaymentController(shoptrangsucEntities1 context)
    {
        _context = context;
    }

    private APIContext GetAPIContext()
    {
        var clientId = ConfigurationManager.AppSettings["PayPal:ClientID"];
        var clientSecret = ConfigurationManager.AppSettings["PayPal:ClientSecret"];
        var config = new Dictionary<string, string>
        {
            { "mode", ConfigurationManager.AppSettings["PayPal:Environment"] }
        };
        var accessToken = new OAuthTokenCredential(clientId, clientSecret, config).GetAccessToken();
        return new APIContext(accessToken) { Config = config };
    }

    [HttpPost]
    [Route("api/paypal/create-payment")]
    public IHttpActionResult CreatePayment([FromBody] PaymentRequestDto model)
    {
        var apiContext = GetAPIContext();
        const decimal exchangeRate = 24000; // Tỷ giá từ VND sang USD
        var usdAmount = model.TotalAmount / exchangeRate;

        var payer = new Payer() { payment_method = "paypal" };
        var redirectUrls = new RedirectUrls()
        {
            cancel_url = "http://localhost:55119/cart/cancel",
            return_url = $"http://localhost:55119/api/paypal/success?userId={model.UserId}" // Chuyển userId
        };

        var amount = new Amount()
        {
            currency = "USD",
            total = usdAmount.ToString("F2")
        };

        var transactionList = new List<Transaction>
    {
        new Transaction()
        {
            description = "Thanh toán sản phẩm",
            invoice_number = Guid.NewGuid().ToString(),
            amount = amount
        }
    };

        var payment = new Payment()
        {
            intent = "sale",
            payer = payer,
            transactions = transactionList,
            redirect_urls = redirectUrls
        };

        try
        {
            var createdPayment = payment.Create(apiContext);
            var approvalUrl = createdPayment.links.FirstOrDefault(link => link.rel.Equals("approval_url"))?.href;
            return Ok(new { ApprovalUrl = approvalUrl });
        }
        catch (PaymentsException ex)
        {
            return Content(HttpStatusCode.BadRequest, new { Message = "Có lỗi xảy ra khi tạo thanh toán.", Details = ex.Response });
        }
    }


    [HttpGet]
    [Route("api/paypal/success")]
    public async Task<IHttpActionResult> Success(string paymentId, string token, string payerId, int userId)
    {
        var apiContext = GetAPIContext();
        var paymentExecution = new PaymentExecution { payer_id = payerId };
        var payment = new Payment { id = paymentId };

        try
        {
            // Thực hiện thanh toán với PayPal
            var executedPayment = payment.Execute(apiContext, paymentExecution);

            if (executedPayment.state == "approved")
            {
                // Lấy dữ liệu giỏ hàng từ database
                var cartItems = await _context.SanPhamGioHangs
                    .Where(c => c.IDUser == userId)
                    .Select(c => new OrderItemDto
                    {
                        ProductId = (int) c.IDSanPham,
                        Quantity = (int) c.SoLuongSanPham,
                        Price = (int) c.SanPham.Gia
                    }).ToListAsync();

                if (!cartItems.Any())
                {
                    return Content(HttpStatusCode.BadRequest, new { Message = "Giỏ hàng trống." });
                }

                // Tính tổng tiền từ giỏ hàng
                var totalAmount = cartItems.Sum(item => item.Quantity * item.Price);

                // Tạo dữ liệu đơn hàng
                var orderData = new CreateOrderDto
                {
                    UserId = userId,
                    VoucherId = null, // Nếu có mã giảm giá thì xử lý tại đây
                    TotalAmount = totalAmount,
                    PaymentMethod = "PayPal", // Phương thức thanh toán
                    CartItems = cartItems
                };

                // Gọi API tạo đơn hàng
                var createOrderResponse = await CreateOrder(orderData);

                if (!createOrderResponse.IsSuccessStatusCode)
                {
                    var errorDetails = await createOrderResponse.Content.ReadAsStringAsync();
                    return Content(HttpStatusCode.BadRequest, new { Message = "Không thể tạo đơn hàng.", Details = errorDetails });
                }

                // Lấy ID đơn hàng từ phản hồi
                var createdOrder = await createOrderResponse.Content.ReadAsAsync<dynamic>();
                var orderId = createdOrder.OrderId;

                // Chuyển hướng đến trang thanh toán thành công
                return Redirect($"http://localhost:55119/thanhtoanthanhcong?orderId={orderId}");
            }
            else
            {
                return Content(HttpStatusCode.BadRequest, new { Message = "Thanh toán không thành công." });
            }
        }
        catch (Exception ex)
        {
            return InternalServerError(new Exception("Lỗi khi xử lý thanh toán.", ex));
        }
    }

    // Hàm trợ giúp để lấy giỏ hàng
    private async Task<HttpResponseMessage> GetCartItems(int userId)
    {
        using (var client = new HttpClient())
        {
            var apiUrl = $"http://localhost:55119/api/cart/{userId}";
            return await client.GetAsync(apiUrl);
        }
    }

    // Hàm trợ giúp để tạo đơn hàng
    private async Task<HttpResponseMessage> CreateOrder(CreateOrderDto orderData)
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var apiUrl = "http://localhost:55119/api/donhang/create";
            return await client.PostAsJsonAsync(apiUrl, orderData);
        }
    }





    [HttpPost]
    [Route("api/paypal/execute-payment")]
    public async Task<IHttpActionResult> ExecutePayment([FromBody] ExecutePaymentDto model)
    {
        var apiContext = GetAPIContext();
        var paymentExecution = new PaymentExecution() { payer_id = model.PayerId };
        var payment = new Payment() { id = model.PaymentId };

        try
        {
            // Thực hiện thanh toán với PayPal
            var executedPayment = payment.Execute(apiContext, paymentExecution);

            if (executedPayment.state == "approved")
            {
                var userId = (int)model.UserId;

                // Lấy thông tin giỏ hàng
                var cartItemsResponse = await GetCartItems(userId);
                if (!cartItemsResponse.IsSuccessStatusCode)
                {
                    return BadRequest("Không thể tải thông tin giỏ hàng.");
                }

                var cartItems = await cartItemsResponse.Content.ReadAsAsync<List<OrderItemDto>>();
                if (!cartItems.Any())
                {
                    return BadRequest("Giỏ hàng trống, không thể tạo đơn hàng.");
                }

                // Tính tổng tiền đơn hàng
                var totalAmount = cartItems.Sum(item => item.Quantity * item.Price);

                // Tạo dữ liệu để gửi lên API tạo đơn hàng
                var orderData = new CreateOrderDto
                {
                    UserId = userId,
                    VoucherId = null, // Nếu có mã giảm giá thì cập nhật
                    TotalAmount = totalAmount,
                    PaymentMethod = "PayPal",
                    CartItems = cartItems
                };

                // Gọi API tạo đơn hàng
                var createOrderResponse = await CreateOrder(orderData);
                if (!createOrderResponse.IsSuccessStatusCode)
                {
                    return BadRequest("Thanh toán thành công nhưng không thể tạo đơn hàng.");
                }

                var createdOrder = await createOrderResponse.Content.ReadAsAsync<dynamic>();

                // Lấy mã đơn hàng vừa tạo
                var orderId = createdOrder.OrderId;

                // Chuyển hướng đến trang "Thanh toán thành công"
                return Redirect($"http://localhost:55119/thanhtoanthanhcong?orderId={orderId}");
            }
            else
            {
                return BadRequest("Thanh toán không thành công.");
            }
        }
        catch (Exception ex)
        {
            return InternalServerError(new Exception("Lỗi khi xử lý thanh toán.", ex));
        }
    }


    private async Task<HttpResponseMessage> GetAppliedVoucher(int userId)
    {
        using (var client = new HttpClient())
        {
            var apiUrl = $"http://localhost:55119/api/cart/voucher/{userId}";
            return await client.GetAsync(apiUrl);
        }
    }


    public class PaymentRequestDto
    {
        public int TotalAmount { get; set; }
        public int UserId { get; set; }
    }


    public class ExecutePaymentDto
    {
        public string PaymentId { get; set; }
        public string PayerId { get; set; }
        public int UserId { get; set; }
    }

    public class CreateOrderDto
    {
        public int UserId { get; set; }
        public int? VoucherId { get; set; }
        public string PaymentMethod { get; set; }
        public int TotalAmount { get; set; }
        public List<OrderItemDto> CartItems { get; set; }
    }



    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
    }


    public class AppliedVoucherDto
    {
        public int VoucherId { get; set; }
        public string VoucherCode { get; set; }
        public decimal DiscountValue { get; set; }
    }
}


