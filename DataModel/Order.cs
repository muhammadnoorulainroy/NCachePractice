﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    [Serializable]
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int Price { get; set; }
        public string ShippingAddress { get; set; }

    }
}
