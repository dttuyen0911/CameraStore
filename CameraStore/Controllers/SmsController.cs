using Microsoft.AspNetCore.Mvc;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using System;
using System.Linq;

namespace CameraStore.Controllers
{
    public class SmsController : Controller
    {
        // Khởi tạo thông tin tài khoản Twilio
        private readonly string accountSid = "ACc3756bc27f3c53c7e951199dfe0472ac";
        private readonly string authToken = "1c37a52fcdcb3f4a3256154de708e57f";
        private readonly string twilioPhoneNumber = "+12514510371"; // Số điện thoại Twilio

        /*        public ActionResult Index()
                {
                    TwilioClient.Init(accountSid, authToken);
                    Random rnd = new Random();
                    string otp = rnd.Next(100000, 999999).ToString();
                    var message = MessageResource.Create(
                        body: $"Your OTP is: {otp}",
                        from: new Twilio.Types.PhoneNumber(twilioPhoneNumber),
                        to: new Twilio.Types.PhoneNumber("+84967862690")
                        );
                    ViewData["Success"] = message.Sid;
                    return View();  
                }*/
        public ActionResult Index()
        {
            // Lấy danh sách tất cả các số điện thoại đã xác minh trong Twilio
            var verifiedPhoneNumbers = GetVerifiedPhoneNumbers();

            // Gán danh sách số điện thoại đã xác minh vào ViewBag
            ViewBag.VerifiedPhoneNumbers = verifiedPhoneNumbers;

            // Hiển thị view
            return View();
        }
        private List<string> GetVerifiedPhoneNumbers()
        {
            // Khởi tạo danh sách để lưu trữ số điện thoại đã xác minh
            List<string> verifiedPhoneNumbers = new List<string>();

            // Thực hiện truy vấn Twilio API để lấy danh sách các số điện thoại đã xác minh
            var accountSid = "ACc3756bc27f3c53c7e951199dfe0472ac";
            var authToken = "1c37a52fcdcb3f4a3256154de708e57f";

            TwilioClient.Init(accountSid, authToken);

            var incomingPhoneNumbers = IncomingPhoneNumberResource.Read();

            foreach (var incomingPhoneNumber in incomingPhoneNumbers)
            {
                // Kiểm tra xem số điện thoại đã xác minh chưa
                if (incomingPhoneNumber.Capabilities.Voice == true || incomingPhoneNumber.Capabilities.Sms == true)
                {
                    verifiedPhoneNumbers.Add(incomingPhoneNumber.PhoneNumber.ToString());
                }
            }

            return verifiedPhoneNumbers;
        }

        /* // Action để gửi mã OTP
         [HttpPost]
         public ActionResult SendOtp(string phoneNumber)
         {

             try
             {
                 TwilioClient.Init(accountSid, authToken);

                 // Kiểm tra số điện thoại người nhận có trong danh sách Verified Caller IDs hay không
                 if (IsVerifiedCallerId(phoneNumber))
                 {
                     // Tạo mã OTP ngẫu nhiên
                     Random rnd = new Random();
                     string otp = rnd.Next(100000, 999999).ToString();
                     string message = $"Your OTP is: {otp}";

                     // Gửi tin nhắn chứa mã OTP đến số điện thoại người nhận
                     var messageOptions = new CreateMessageOptions(new PhoneNumber(phoneNumber));
                     messageOptions.From = new PhoneNumber(twilioPhoneNumber);
                     messageOptions.Body = message;

                     // Gửi tin nhắn với các tùy chọn đã cung cấp
                     var messageResource = MessageResource.Create(messageOptions);

                     return Json(new { success = true, message = "OTP has been sent successfully." });
                 }
                 else
                 {
                     return Json(new { success = false, message = "Phone number is not verified." });
                 }
             }
             catch (Exception ex)
             {
                 return Json(new { success = false, message = "Failed to send OTP. Please try again later." });
             }
         }*/

        // Hàm để kiểm tra xem số điện thoại có trong danh sách Verified Caller IDs không
        private bool IsVerifiedCallerId(string phoneNumber)
        {
            try
            {
                TwilioClient.Init(accountSid, authToken);

                // Truy vấn danh sách các số điện thoại đã xác minh trong danh sách Verified Caller IDs
                var callerIds = OutgoingCallerIdResource.Read();

                foreach (var callerId in callerIds)
                {
                    // So sánh số điện thoại đã xác minh với số điện thoại nhập vào
                    if (callerId.PhoneNumber.ToString() == phoneNumber)
                    {
                        ViewBag.CallerId = callerId;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to get verified phone numbers. Error: {ex.Message}");
            }

            return false;
        }
    }
}