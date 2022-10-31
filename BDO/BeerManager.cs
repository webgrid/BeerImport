using System.Net;
using System.Text;
using System.Text.Json;

namespace BDO_Project.BDO
{
    /// <summary>
    /// Program worker that uses a System.Timer for importing beers from a API every 60th seconds and updates data source with new beer.
    /// I uses a in memory database for this small example.
    /// </summary>
    public class BeerManager
    {

         private readonly System.Timers.Timer _timer;

        /// <summary>
        /// API URL for the beers, does not require a apikey.
        /// </summary>
        /// <remarks>
        /// Find public free APIs: https://github.com/public-apis/public-apis
        /// Identified json properties by viewing here: https://codebeautify.org/jsonviewer
        /// </remarks>
        const string APIURL = "https://api.punkapi.com/v2/beers";

        /// <summary>
        /// My Database context for this small project.
        /// </summary>
        public BeerDbContext BeerDb { get; }

        /// <summary>
        /// For displaying the beer in a table.
        /// </summary>
        /// <returns></returns>
        internal string BeersToHtml()
        {
            StringBuilder sb = new();

            foreach (var beer in BeerDb.Beers)
                sb.AppendLine(beer.ToHtml());
            return sb.ToString();

        }

        /// <summary>
        /// a timer that scheduling a new data import every 60th seconds.
        /// </summary>
        public BeerManager(BeerDbContext beerDbContext)
        {
            BeerDb = beerDbContext;
            BeerDb.SaveChangesFailed += BeerDb_SaveChangesFailed;

            _timer = new System.Timers.Timer(60 * 1000);
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
            Timer_Elapsed(null, null);
        }

        /// <summary>
        /// You can add logic here for logging.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BeerDb_SaveChangesFailed(object? sender, Microsoft.EntityFrameworkCore.SaveChangesFailedEventArgs e)
        {
        }

        /// <summary>
        /// Executed every 60th seconds.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs? e)
        {
            string json;
            using (var httpClient = new HttpClient())
                 json = await httpClient.GetStringAsync(APIURL);
            ItemBeer[] itemstoprocess;
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                var jsonarray = document.RootElement.EnumerateArray().ToArray();
                itemstoprocess = new ItemBeer[jsonarray.Length];
                for (int i = 0; i < jsonarray.Length; i++)
                   itemstoprocess[i] = new ItemBeer() { webjson = jsonarray[i] };
                processImportedItems(itemstoprocess);
            }
            Updatedatabase(itemstoprocess);
        }

       

        /// <summary>
        /// Process imported items before inserting them into db.
        /// </summary>
        /// <remarks>
        /// Using parallel processing in case there are many beers to process.
        /// </remarks>
        private static ResultInfo processImportedItems(ItemBeer[] itemstoprocess)
        {
            ///Add logic here for logging and more processing about the beer.
            var exceptions = Utility.ProcessParallel(itemstoprocess, ii => ii.loadJson());
            return ResultInfo.Ok();
        }

        /// <summary>
        /// Update the database.
        /// </summary>
        /// <param name="itemstoprocess"></param>
        private void Updatedatabase(ItemBeer[] itemstoprocess)
        {
           var existingids = BeerDb.Beers.Select(t => t.Id).ToArray();

            itemstoprocess = itemstoprocess.Where(t => !existingids.Contains(t.Id)).ToArray();
            if (itemstoprocess.Length > 0)
            {
                for (int i = 0; i < itemstoprocess.Length; i++)
                    BeerDb.Add(itemstoprocess[i]);
             
                BeerDb.SaveChanges();
            }
        }
    }
}
