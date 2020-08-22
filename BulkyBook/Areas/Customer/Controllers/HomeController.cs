using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Dapper;


namespace BulkyBook.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public object HttpRuntime { get; private set; }

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(int? id)
        {
            //GEO Location implementation of web application access



            //





            IEnumerable<Product> productList;

            if (id == null)
            {
                productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
            }
            else
            {
                productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType").Where(b => b.Category.Id.Equals(id
                 ));
            }



            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var count = _unitOfWork.ShoppingCart
                    .GetAll(c => c.ApplicationUserId == claim.Value)
                    .ToList().Count();

                HttpContext.Session.SetInt32(SD.ssShoppingCart, count);
            }
            return View(productList);
        }

        public IActionResult Help()
        {
            return View();
        }


        public ActionResult Search(int? id)
        {

            IEnumerable<Product> productList;

            if (id == 0)
            {
                productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
            }
            else
            {
                productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType").Where(b => b.Category.Id.Equals(id
                 ));
            }
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var count = _unitOfWork.ShoppingCart
                    .GetAll(c => c.ApplicationUserId == claim.Value)
                    .ToList().Count();

                HttpContext.Session.SetInt32(SD.ssShoppingCart, count);
            }



            return View(productList);

        }



        /// <summary>
        /// Current code Surya changes 1
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        public IActionResult Details(int id)
        {
            var productFromDb = _unitOfWork.Product.
                       GetFirstOrDefault(u => u.Id == id, includeProperties: "Category,CoverType");
            ShoppingCart cartObj = new ShoppingCart()
            {
                Product = productFromDb,
                ProductId = productFromDb.Id
            };
            //Surya changes
            return View(cartObj);
        }




        [Authorize]
        public IActionResult Details1(int id)
        {
            var productFromDb = _unitOfWork.Product.
                        GetFirstOrDefault(u => u.Id == id, includeProperties: "Category,CoverType");
            ShoppingCart cartObj = new ShoppingCart()
            {
                Product = productFromDb,
                ProductId = productFromDb.Id
            };

            //Surya changes
            cartObj.Id = 0;
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                cartObj.ApplicationUserId = claim.Value;

                ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
                    u => u.ApplicationUserId == cartObj.ApplicationUserId && u.ProductId == cartObj.ProductId
                    , includeProperties: "Product"
                    );
                if (cartFromDb == null)
                {
                    //no records exists in database for that product for that user
                    _unitOfWork.ShoppingCart.Add(cartObj);
                }
                else
                {
                    cartFromDb.Count += cartObj.Count;
                    //_unitOfWork.ShoppingCart.Update(cartFromDb);
                }
                _unitOfWork.Save();

                var count = _unitOfWork.ShoppingCart
                    .GetAll(c => c.ApplicationUserId == cartObj.ApplicationUserId)
                    .ToList().Count();

                //HttpContext.Session.SetObject(SD.ssShoppingCart, CartObject);
                HttpContext.Session.SetInt32(SD.ssShoppingCart, count);

                return RedirectToAction(nameof(Index));
            }

            

            //Surya changes
            return View(cartObj);
    }



        [Authorize]
        public IActionResult Details2(int id)
        {
            var productFromDb = _unitOfWork.Product.
                        GetFirstOrDefault(u => u.Id == id, includeProperties: "Category,CoverType");
            ShoppingCart cartObj = new ShoppingCart()
            {
                Product = productFromDb,
                ProductId = productFromDb.Id
            };

            //Surya changes
            cartObj.Id = 0;
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                cartObj.ApplicationUserId = claim.Value;

                ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
                    u => u.ApplicationUserId == cartObj.ApplicationUserId && u.ProductId == cartObj.ProductId
                    , includeProperties: "Product"
                    );
                if (cartFromDb == null)
                {
                    //no records exists in database for that product for that user
                    _unitOfWork.ShoppingCart.Add(cartObj);
                }
                else
                {
                    cartFromDb.Count += cartObj.Count;
                    //_unitOfWork.ShoppingCart.Update(cartFromDb);
                }
                _unitOfWork.Save();

                var count = _unitOfWork.ShoppingCart
                    .GetAll(c => c.ApplicationUserId == cartObj.ApplicationUserId)
                    .ToList().Count();

                //HttpContext.Session.SetObject(SD.ssShoppingCart, CartObject);
                HttpContext.Session.SetInt32(SD.ssShoppingCart, count);

                // return RedirectToAction(nameof(Index));
                IEnumerable<Product> productList;
                productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType").Where(b => b.Category.Id.Equals(productFromDb.CategoryId
                 ));
                return View("Search", productList);
            }



            //Surya changes
            return View(cartObj);
        }

        /// Current code Surya changes 1








        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart CartObject)
        {
            CartObject.Id = 0;
            if (ModelState.IsValid)
            {
                //then we will add to cart
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                CartObject.ApplicationUserId = claim.Value;

                ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
                    u => u.ApplicationUserId == CartObject.ApplicationUserId && u.ProductId == CartObject.ProductId
                    ,includeProperties:"Product"
                    );

                if (cartFromDb == null)
                {
                    //no records exists in database for that product for that user
                    _unitOfWork.ShoppingCart.Add(CartObject);
                }
                else
                {
                    cartFromDb.Count += CartObject.Count;
                    //_unitOfWork.ShoppingCart.Update(cartFromDb);
                }
                _unitOfWork.Save();

                var count = _unitOfWork.ShoppingCart
                    .GetAll(c => c.ApplicationUserId == CartObject.ApplicationUserId)
                    .ToList().Count();

                //HttpContext.Session.SetObject(SD.ssShoppingCart, CartObject);
                HttpContext.Session.SetInt32(SD.ssShoppingCart, count);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                var productFromDb = _unitOfWork.Product.
                        GetFirstOrDefault(u => u.Id == CartObject.ProductId, includeProperties: "Category,CoverType");
                ShoppingCart cartObj = new ShoppingCart()
                {
                    Product = productFromDb,
                    ProductId = productFromDb.Id
                };
                return View(cartObj);
            }

            
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
