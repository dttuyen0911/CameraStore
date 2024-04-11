using Newtonsoft.Json;
using Stripe;

namespace CameraStore.Models
{
    public class PaymentIntentCreateRequest
    {
        [JsonProperty("carts")]
        public Cart[] Carts { get; set; }
        public string ClientSecret { get; set; }
    }
}
