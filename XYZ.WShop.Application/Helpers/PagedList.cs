using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYZ.WShop.Application.Helpers
{
    /// <summary>
    /// Represents a paginated list of items.
    /// </summary>
    /// <typeparam name="T">The type of items in the list.</typeparam>
    public class PagedList<T> : List<T>
    {
        const int maxPageSize = 50;
        private int _pageSize = 10;

        /// <summary>
        /// Gets or sets the page size. It is capped at a maximum value of 50.
        /// </summary>
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }

        /// <summary>
        /// Gets the current page number.
        /// </summary>
        public int CurrentPage { get; private set; } = 1;

        /// <summary>
        /// Gets the total number of pages.
        /// </summary>
        public int TotalPages { get; private set; }

        /// <summary>
        /// Gets the total count of items.
        /// </summary>
        public int TotalCount { get; private set; }

        /// <summary>
        /// Indicates if there is a previous page.
        /// </summary>
        public bool HasPrevious => CurrentPage > 1;

        /// <summary>
        /// Indicates if there is a next page.
        /// </summary>
        public bool HasNext => CurrentPage < TotalPages;

        /// <summary>
        /// Initializes a new instance of the PagedList class.
        /// </summary>
        /// <param name="items">The list of items.</param>
        /// <param name="count">The total count of items.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            AddRange(items);
        }

    }
}
