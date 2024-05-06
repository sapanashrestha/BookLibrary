namespace BookLibrary.DTO
{
    public class PutBooksDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Publication { get; set; }
        public DateTime PublicationDate { get; set; }
        public decimal Price { get; set; }
    }
}
