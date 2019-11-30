using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SocialNetwork.Models
{
    public class MyCommunitiesViewModels
    {
        [Required]
        [Display(Name = "Наименование сообщества")]
        public string Name { get; set; }

        public List<Communities> Communities { get; set; }

    }

    public class PageInfo
    {
        public int PageNumber { get; set; } // номер текущей страницы
        public int PageSize { get; set; } // кол-во объектов на странице
        public int TotalItems { get; set; } // всего объектов
        public int TotalPages  // всего страниц
        {
            get { return (int)Math.Ceiling((decimal)TotalItems / PageSize); }
        }
    }
    public class IndexViewModel1
    {
        public IEnumerable<Communities> Communities { get; set; }
        public PageInfo PageInfo { get; set; }
    }
}