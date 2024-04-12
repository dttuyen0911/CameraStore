using Microsoft.AspNetCore.Mvc;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using System;
using System.Linq;
using Twilio.Clients;

namespace CameraStore.Controllers
{
    public class SmsController : Controller
    {
        private readonly ITwilioRestClient _twilioClient;

        public SmsController(ITwilioRestClient twilioClient)
        {
            _twilioClient = twilioClient;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SendOtp(string phoneNumber)
        {
            try
            {
                var accountSid = "ACc3756bc27f3c53c7e951199dfe0472ac";
                var authToken = "1c37a52fcdcb3f4a3256154de708e57f";
                TwilioClient.Init(accountSid, authToken);

                Random rnd = new Random();
                string otp = rnd.Next(100000, 999999).ToString();
                string message = $"Your OTP is: {otp}";

                // Chú ý rằng bạn cần thay đổi số điện thoại này để truyền vào số điện thoại người nhận thực sự
                var toPhoneNumber = "+840967862690";

                var messageOptions = new CreateMessageOptions(new PhoneNumber(toPhoneNumber));
                messageOptions.From = new PhoneNumber("+12514510371");
                messageOptions.Body = message;

                // Gửi tin nhắn với các tùy chọn đã cung cấp
                var messageResource = MessageResource.Create(messageOptions);

                return Json(new { success = true, message = "OTP has been sent successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to send OTP. Please try again later." });
            }
        }

        // Kiểm tra số điện thoại có hợp lệ không
       

    }
}
