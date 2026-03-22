using System;
using System.Collections.Generic;

namespace Loan_CRM.Models
{
    public class TongdunModelGreb
    {

        public int id { get; set; }
        public string userid { get; set; }
        public string order_no { get; set; }
        public string date { get; set; }
        public string country { get; set; }
        public string distance { get; set; }
        public string city { get; set; }
        public string evaluation { get; set; }
        public string car_driver { get; set; }
        public string from_longitude { get; set; }
        public string from_ { get; set; }
        public string pay_type { get; set; }
        public string tag { get; set; }
        public string phone_type { get; set; }
        public string vehicle_type { get; set; }
        public string to_detail { get; set; }
        public string currency_unit { get; set; }
        public string from_latitude { get; set; }
        public string to_longitude { get; set; }
        public string car_no { get; set; }
        public string intermediate { get; set; }
        public string order_fee { get; set; }
        public string to_latitude { get; set; }
        public string to_ { get; set; }
        public string time_ { get; set; }
        public string from_detail { get; set; }
        public string status { get; set; }
        public DateTime createdon { get; set; }

    }
    public class TongdunModelShopee
    {
        public int id { get; set; }
        public string order_no { get; set; }
        public string package_shipping { get; set; }
        public string package_sold_by { get; set; }
        public string shipping_cost { get; set; }
        public string payment_time { get; set; }
        public string sub_total { get; set; }
        public string product_name { get; set; }
        public string grand_total { get; set; }
        public string shipping_address { get; set; }
        public string phone { get; set; }
        public string customername { get; set; }
        public string jumio_reference { get; set; }
    }
    public class TongdunModelLazada
    {
        public int id { get; set; }
        public string order_no { get; set; }
        public string shipping_cost { get; set; }
        public string sub_total { get; set; }
        public string billing_address { get; set; }
        public string billing_address_phone { get; set; }
        public string billing_address_name { get; set; }
        public string order_time { get; set; }
        public string grand_total { get; set; }
        public string shipping_address { get; set; }
        public string shipping_address_name { get; set; }
        public string shipping_address_phone { get; set; }
        public string jumio_reference { get; set; }
    }

    public class TongdunVM
    {
        public string ReferenceNo { get; set; }
        public List<TongdunModelShopee> shoppeData { get; set; }
        public List<TongdunModelGreb> tongdunModels { get; set; }
        public List<TongdunModelLazada> lazadas { get; set; }
        public TongdunVM()
        {
            tongdunModels = new List<TongdunModelGreb>();
            shoppeData = new List<TongdunModelShopee>();
            lazadas = new List<TongdunModelLazada>();
        }
    }
}