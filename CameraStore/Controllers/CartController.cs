using CameraStore.Data;
using CameraStore.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using CameraStore.Models;

public class CartController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    public CartController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public ActionResult Index()
    {
        // Retrieve the current customer's cart
        Customer customer = GetCurrentCustomer();
        if (customer != null)
        {
            Cart cart = _dbContext.Carts.FirstOrDefault(c => c.customerID == customer.customerID);
            return View(cart);
        }
        return View("Error");
    }

    // GET: Cart/AddToCart/5
    public ActionResult AddToCart(int proID, int quantity)
    {
        // Retrieve the current customer
        Customer customer = GetCurrentCustomer();
        if (customer != null)
        {
            // Retrieve the product
            Product product = _dbContext.Products.Find(proID);
            if (product != null)
            {
                // Check if the product is already in the cart
                Cart cart = _dbContext.Carts.FirstOrDefault(c => c.customerID == customer.customerID);
                if (cart == null)
                {
                    // Create a new cart if the customer doesn't have one
                    cart = new Cart
                    {
                        customerID = customer.customerID,
                        timeStamp = DateTime.UtcNow
                    };
                    _dbContext.Carts.Add(cart);
                }

                // Check if the product is already in the cart details
                CartDetails cartDetails = cart.CartDetails.FirstOrDefault(cd => cd.proID == proID);
                if (cartDetails == null)
                {
                    // Add the product to the cart details
                    cartDetails = new CartDetails
                    {
                        proID = proID,
                        quantity = quantity,
                        price = product.proPrice
                    };
                    cart.CartDetails.Add(cartDetails);
                }
                else
                {
                    // Update the quantity if the product is already in the cart details
                    cartDetails.quantity += quantity;
                }

                // Update the total quantity and price of the cart
                cart.cartQuantityTotal += quantity;
                cart.cartPriceTotal += quantity * product.proPrice;

                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
        }
        return View("Error");
    }

    // Helper method to get the current customer based on authentication or session
    private Customer GetCurrentCustomer()
    {
        // Implement your logic to retrieve the current customer based on authentication or session
        // For demonstration purposes, this method returns a sample customer with ID 1
        return _dbContext.Customers.Find(1);
    }
}

