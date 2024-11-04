using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{
    public class PaymentsController : BaseAPIController
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;
        // This is to add your Stripe CLI webhook secret for testing your endpoint locally.
        private readonly IConfiguration _configuration;




        public PaymentsController(IPaymentService paymentService, 
            ILogger<PaymentsController> logger,
            IConfiguration configuration
            )
        {
            _paymentService = paymentService;
            _logger = logger;
            _configuration = configuration;
        }


        #region Create Or Update Payment Intent
        [Authorize]
        [ProducesResponseType(typeof(CustomerBasketDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomerBasketDto), StatusCodes.Status400BadRequest)]
        [HttpPost("{basketId}")] // POST : /api/payments/basketId
        public async Task<ActionResult<CustomerBasketDto>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);

            if (basket is null)
                return BadRequest(new ApiResponse(400, "There's A problem with your Basket"));

            return Ok(basket);
        }
        #endregion

        #region Create Stripe Webhook
        [HttpPost("webhook")] // POST : api/payments/webhook
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            var stripeEvent = EventUtility.ConstructEvent(json,
                Request.Headers["Stripe-Signature"], _configuration["StripeSettings:WebhookSecretKey"]);

            var paymentIntent = (PaymentIntent)stripeEvent.Data.Object;

            Order order;

            // Handle the event
            switch (stripeEvent.Type)
            {
                case EventTypes.PaymentIntentSucceeded:
                    order = await _paymentService.UpdatePaymentIntentToSucceededOrFailed(paymentIntent.Id, true);
                    _logger.LogInformation("Payment is Succeeded well", paymentIntent.Id);
                    break;
                case EventTypes.PaymentIntentPaymentFailed:
                    order = await _paymentService.UpdatePaymentIntentToSucceededOrFailed(paymentIntent.Id, false);
                    _logger.LogError("Payment is Failed", paymentIntent.Id);
                    break;
                default:
                    // ... handle other event types
                    Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                    break;
            }

            return Ok();
        }
        #endregion
    }
}
