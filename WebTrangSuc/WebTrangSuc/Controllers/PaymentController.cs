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

public class PaymentController : ApiController
{
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
        const decimal exchangeRate = 24000; // Tỷ giá VND sang USD
        var usdAmount = model.TotalAmount / exchangeRate; // Chuyển đổi VND sang USD

        var payer = new Payer() { payment_method = "paypal" };

        var redirectUrls = new RedirectUrls()
        {
            cancel_url = "http://localhost:55119/giohang/cancel",
            return_url = "http://localhost:55119/giohang/success"
        };


        var amount = new Amount()
        {
            currency = "USD", // Sử dụng USD
            total = usdAmount.ToString("F2") // Tổng tiền phải là chuỗi hợp lệ
        };

        var transactionList = new List<Transaction>
    {
        new Transaction()
        {
            description = "Your purchase description",
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
            return Content(HttpStatusCode.BadRequest, new
            {
                Message = "Có lỗi xảy ra khi tạo thanh toán.",
                PayPalMessage = ex.Response,
                Exception = ex.Message
            });
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
            // Thực hiện thanh toán
            var executedPayment = payment.Execute(apiContext, paymentExecution);

            if (executedPayment.state == "approved")
            {
                // Lấy userId từ thông tin
                var userId = 1; // Thay bằng logic để lấy userId từ session hoặc request

                // Lấy thông tin giỏ hàng
                var cartResponse = await GetCartItems(userId);
                if (!cartResponse.IsSuccessStatusCode)
                {
                    return BadRequest("Không thể tải thông tin giỏ hàng.");
                }

                var cartItems = await cartResponse.Content.ReadAsAsync<List<OrderItemDto>>();

                if (!cartItems.Any())
                {
                    return BadRequest("Giỏ hàng trống, không thể tạo đơn hàng.");
                }

                // Lấy thông tin voucher
                var voucherResponse = await GetAppliedVoucher(userId);
                int? voucherId = null;
                if (voucherResponse.IsSuccessStatusCode)
                {
                    var voucherData = await voucherResponse.Content.ReadAsAsync<AppliedVoucherDto>();
                    voucherId = voucherData.VoucherId; // Sử dụng VoucherId nếu có
                }

                // Chuẩn bị dữ liệu đơn hàng
                var totalAmount = cartItems.Sum(item => item.Quantity * item.Price);
                var orderData = new CreateOrderDto
                {
                    UserId = userId,
                    VoucherId = voucherId, // Nullable int
                    TotalAmount = totalAmount,
                    PaymentMethod = "PayPal",
                    CartItems = cartItems
                };

                // Tạo đơn hàng
                var createOrderResponse = await CreateOrder(orderData);

                if (!createOrderResponse.IsSuccessStatusCode)
                {
                    return BadRequest("Thanh toán thành công nhưng không thể tạo đơn hàng.");
                }

                // Chuyển hướng đến trang đơn hàng
                return Redirect("http://localhost:55119/donhang");
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


    private async Task<HttpResponseMessage> GetCartItems(int userId)
    {
        using (var client = new HttpClient())
        {
            var apiUrl = $"http://localhost:55119/api/cart/{userId}";
            return await client.GetAsync(apiUrl);
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

    private async Task<HttpResponseMessage> CreateOrder(CreateOrderDto orderData)
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var apiUrl = "http://localhost:55119/api/donhang/create";
            return await client.PostAsJsonAsync(apiUrl, orderData);
        }
    }

}

public class PaymentRequestDto
{
    public int TotalAmount { get; set; } 
}


public class ExecutePaymentDto
{
    public string PaymentId { get; set; }
    public string PayerId { get; set; }
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


