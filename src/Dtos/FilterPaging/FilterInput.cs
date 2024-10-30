using System;
using System.Collections.Generic;

namespace PetProject
{
    public class FilterInput
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchString { get; set; }
        public string? OrderColumn { get; set; }
        public bool? IsSortDesc { get; set; }
        public string[] SearchColumns { get; set; } = Array.Empty<string>();
    }
}
