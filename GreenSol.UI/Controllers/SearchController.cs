using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GreenSol.Domain.Abstract;
using GreenSol.UI.Models;
using GreenSol.Domain.Concrete;
using GreenSol.Domain.Entities;

namespace GreenSol.UI.Controllers
{
    public class SearchController : Controller
    {
        private readonly IAlbumRepository repository;

        private List<string> excludedProperties = new List<string> { "AlbumArtUrl", "AlbumId", "ArtistId", "GenreId", "ImageMimeType", "TrackList" };
        
        public SearchController(IAlbumRepository repo)
        {
            this.repository = repo;
        }

        //Search info about albums
        public ActionResult Index()
        {
            var data = this.repository.Albums.AsQueryable()
                //.GetQuery()
                .ToArray();

            var model = new AlbumSearchViewModel()
            {
                Data = data,
                SearchCriteria = typeof(Album)
                    .GetDefaultSearchCriterias(excludedProperties)
                    .AddCustomSearchCriteria<Album>(s => s.Genre.Name)
                    .AddCustomSearchCriteria<Album>(s => s.Artist.Name)
            };
            
            return View(model);
        }

        [HttpPost]
        public ActionResult Index([Bind(Exclude = "AlbumArtUrl, AlbumId, ArtistId, GenreId, ImageMimeType, TrackList")]
            ICollection<AbstractSearch> searchCriteria)
        {
            var data = this.repository.Albums.AsQueryable()
                //.GetQuery()
                .ApplySearchCriterias(searchCriteria)
                .ToArray();

            var model = new AlbumSearchViewModel()
            {
                Data = data,
                SearchCriteria = searchCriteria
            };
            
            return View(model);
        }

        //This action method only searchs on 1 criteria, using the view 
        public ActionResult DefaultSearch(SearchCriteria criteria, string searchString)
        {
            string targetTypeName = "GreenSol.Domain.Entities.Album";
            
            IQueryable<Album> queryable = repository.Albums.AsQueryable();

            //if (criteria == SearchCriteria.Genres)
            //    queryable = repository.Genres.AsQueryable();
            //else if (criteria == SearchCriteria.Artists)
            //    queryable = repository.Artists.AsQueryable();
            //else
            //    queryable = repository.Albums.AsQueryable();

            List<AbstractSearch> searchCriteria = new List<AbstractSearch>()
            {
                //Initialize all criterias, so that the user can use the advance search
                //without being redirected to Seach/Index
                new DateSearch()
                {
                    SearchTerm = null,
                    SearchTerm2 = null,
                    Comparator = DateComparators.GreaterOrEqual,
                    Property = "Date",
                    TargetTypeName= targetTypeName
                },
                //If criteria = "Genres"
                new TextSearch()
                {
                    SearchTerm = (criteria == SearchCriteria.Genres ? searchString : ""),
                    //Comparator = TextComparators.Contains,
                    Property = "Genre.Name",                  
                    TargetTypeName = targetTypeName
                },
                new TextSearch()
                {
                    SearchTerm = (criteria == SearchCriteria.Titles ? searchString : ""),
                    //Comparator = TextComparators.Contains,
                    Property = "Name",
                    TargetTypeName = targetTypeName
                },
                new TextSearch()
                {
                    SearchTerm = (criteria == SearchCriteria.Artists ? searchString : ""),
                    //Comparator = TextComparators.Contains,
                    Property = "Artist.Name",
                    TargetTypeName = targetTypeName
                },
                new NumericSearch()
                {
                    SearchTerm = null,
                    Comparator = NumericComparators.GreaterOrEqual,
                    Property = "Price",
                    TargetTypeName = targetTypeName
                }
            };

            var result = queryable.ApplySearchCriterias(searchCriteria);

            var viewModel = new AlbumSearchViewModel()
            {
                Data = result.AsEnumerable(),
                SearchCriteria = searchCriteria
            };
            return View("Index", viewModel);
        }

        //private IEnumerable<Album> ToEnumerable(IQueryable<IEntity> target) 
        //{
        //    List<Album> result = new List<Album>();
        //    foreach (IEntity entity in target)
        //        result.Add((Album)entity);                             
        //    return result;
        //}
    }
}