﻿using System.ComponentModel.DataAnnotations;

namespace BookLibrary.Model
{
    public class Books
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Publication { get; set; }
        public DateTime PublicationDate { get; set; }
        public decimal Price { get; set; }

    }
}
