﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<ProductViewModel> Products { get; set; }
        public List<SelectListItem> Categories { get; set; } //dropdown list yerine

        public List<SelectListItem> Authors { get; set; }

    }
}
