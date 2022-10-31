namespace BDO_Project.BDO
{
   
    /// <summary>
    /// Result from methods when processing data. returns Ok, Info, Warning or Error.
    /// </summary>
    public class ResultInfo
    {
        internal static ResultInfo Ok()
        {
            return new ResultInfo();
        }
    }
}
