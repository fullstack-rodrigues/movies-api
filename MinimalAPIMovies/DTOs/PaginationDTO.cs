namespace MinimalAPIMovies.DTOs
{
    public class PaginationDTO
    {
        public int Page { get; set; } = 1;
        private int itemsPerPage = 10;
        private readonly int recordsPerPageMax = 50;

        public int ItemsPerPage
        {
            get
            {
                return itemsPerPage;
            }

            set
            {
                if(value > recordsPerPageMax)
                {
                    itemsPerPage = recordsPerPageMax;
                } else
                {
                    itemsPerPage = value;
                }
            }
        }
    }
}
